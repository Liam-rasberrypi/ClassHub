using ClassHub.Models;

namespace ClassHub.Services
{
    public interface ISecurityService
    {
        bool RequiresAdministratorSetup { get; }
        string? CurrentAccountId { get; }
        bool InitializeAdministrator(string userName, string password);
        bool Login(string userName, string password);
        void Logout();
        bool VerifyLogin(string accountId, string? password, string? usbDeviceId);
        bool VerifyFeaturePassword(string featureKey, string password);
        bool HasPermission(string accountId, string permissionKey);
        void DemandPermission(string permissionKey);
    }

    public class SecurityService : ISecurityService
    {
        private static readonly TimeSpan SessionLifetime = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan LockoutDuration = TimeSpan.FromSeconds(30);
        private const int MaximumFailures = 5;
        private readonly IAccountService _accounts;
        private readonly TimeProvider _time;
        private readonly object _sync = new();
        private Account? _authenticatedAccount;
        private long _sessionStarted;
        private int _failedAttempts;
        private bool _lockedOut;
        private long _lockoutStarted;

        public SecurityService(IAccountService accountService, TimeProvider? timeProvider = null)
        {
            _accounts = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _time = timeProvider ?? TimeProvider.System;
        }

        public bool RequiresAdministratorSetup
        {
            get
            {
                lock (_sync)
                {
                    try { return _accounts.RequiresAdministratorSetup; }
                    catch (AccountStorageException)
                    {
                        _authenticatedAccount = null;
                        throw;
                    }
                }
            }
        }

        public string? CurrentAccountId
        {
            get
            {
                lock (_sync)
                {
                    ExpireSession();
                    return _authenticatedAccount?.Id;
                }
            }
        }

        public bool InitializeAdministrator(string userName, string password)
        {
            lock (_sync)
            {
                _authenticatedAccount = null;
                if (IsLockedOut()) return false;
                // Grant a session only after saving the first account successfully.
                var account = _accounts.InitializeAdministrator(userName, password);
                StartSession(account);
                return true;
            }
        }

        public bool Login(string userName, string password)
        {
            lock (_sync)
            {
                // Failed replacement logins also invalidate the previous identity.
                _authenticatedAccount = null;
                if (IsLockedOut()) return false;
                var account = _accounts.GetByUserName(userName);
                if (account == null || !account.Enabled || account.AuthMode != AuthMode.Password ||
                    !_accounts.ValidateLogin(account.UserName, password, null))
                {
                    RegisterFailure();
                    return false;
                }
                StartSession(account);
                return true;
            }
        }

        public void Logout()
        {
            lock (_sync)
            {
                _authenticatedAccount = null;
                // Logout must not let callers bypass the failed-login limit.
            }
        }

        // The legacy ID entry point establishes the same verified session as Login.
        public bool VerifyLogin(string accountId, string? password, string? usbDeviceId)
        {
            lock (_sync)
            {
                _authenticatedAccount = null;
                if (IsLockedOut()) return false;
                var account = _accounts.GetAll().FirstOrDefault(x => x.Id == accountId);
                if (account == null)
                {
                    RegisterFailure();
                    return false;
                }
                return Login(account.UserName, password ?? string.Empty);
            }
        }

        // USB and feature passwords have no trusted verification implementation yet.
        public bool VerifyFeaturePassword(string featureKey, string password) => false;

        public bool HasPermission(string accountId, string permissionKey)
        {
            if (permissionKey == PermissionKey.NoticeView) return true;
            if (permissionKey != PermissionKey.NoticeEdit) return false;
            lock (_sync)
            {
                ExpireSession();
                var verified = _authenticatedAccount;
                if (verified == null || verified.Id != accountId || verified.Role != Role.Admin)
                    return false;
                // Disabling, demoting or changing a password revokes existing access.
                Account? current;
                try { current = _accounts.GetByUserName(verified.UserName); }
                catch (AccountStorageException)
                {
                    _authenticatedAccount = null;
                    throw;
                }
                if (current == null || current.Id != verified.Id || !current.Enabled ||
                    current.Role != Role.Admin || current.AuthMode != AuthMode.Password ||
                    current.PasswordHash != verified.PasswordHash)
                {
                    _authenticatedAccount = null;
                    return false;
                }
                return true;
            }
        }

        public void DemandPermission(string permissionKey)
        {
            lock (_sync)
            {
                if (!HasPermission(CurrentAccountId ?? string.Empty, permissionKey))
                    throw new UnauthorizedAccessException("此操作需要管理员身份验证，请重新登录。");
            }
        }

        private void StartSession(Account account)
        {
            _authenticatedAccount = new Account
            {
                Id = account.Id, UserName = account.UserName, Role = account.Role,
                AuthMode = account.AuthMode, Enabled = account.Enabled,
                PasswordHash = account.PasswordHash
            };
            _sessionStarted = _time.GetTimestamp();
            _failedAttempts = 0;
            _lockedOut = false;
        }

        private void ExpireSession()
        {
            if (_authenticatedAccount != null &&
                _time.GetElapsedTime(_sessionStarted) >= SessionLifetime)
                _authenticatedAccount = null;
        }

        private bool IsLockedOut()
        {
            if (!_lockedOut) return false;
            if (_time.GetElapsedTime(_lockoutStarted) < LockoutDuration) return true;
            _lockedOut = false;
            _failedAttempts = 0;
            return false;
        }

        private void RegisterFailure()
        {
            if (++_failedAttempts < MaximumFailures) return;
            _lockedOut = true;
            _lockoutStarted = _time.GetTimestamp();
        }
    }
}
