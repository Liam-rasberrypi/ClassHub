using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text.Json;
using ClassHub.Models;

namespace ClassHub.Services
{
    public interface IAccountService
    {
        bool RequiresAdministratorSetup { get; }
        Account InitializeAdministrator(string userName, string password);
        IEnumerable<Account> GetAll();
        Account? GetByUserName(string userName);
        bool ValidateCredentials(string userName, string password);
        bool ValidateUsbBinding(string userName, string usbDeviceId);
        bool ValidateLogin(string userName, string? password, string? usbDeviceId);
        void Create(Account account);
        void Update(Account account);
        void Delete(string userName);
    }

    /// <summary>Offline account storage. Only a missing store allows first-time setup.</summary>
    public class AccountService : IAccountService
    {
        private const int MaximumStoreBytes = 1024 * 1024;
        private const int MaximumAccounts = 1000;
        private readonly string _storagePath;

        public AccountService()
            : this(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "accounts.json"))
        {
        }

        public AccountService(string storagePath)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(storagePath);
            _storagePath = Path.GetFullPath(storagePath);
        }

        public bool RequiresAdministratorSetup => LoadAccounts() == null;

        public Account InitializeAdministrator(string userName, string password)
        {
            // Never treat a damaged/empty store or a store with disabled accounts as new.
            if (LoadAccounts() != null)
                throw new InvalidOperationException("已存在本地账户，不能重复设置首次管理员。");

            string canonicalUserName = userName?.Trim() ?? string.Empty;
            if (!IsValidUserName(canonicalUserName))
                throw new ArgumentException("管理员账号应为 1–64 个字符，且不能包含控制字符。", nameof(userName));
            PasswordHasher.ValidateNewPassword(password);

            var administrator = new Account
            {
                Id = Guid.NewGuid().ToString("D"),
                UserName = canonicalUserName,
                Role = Role.Admin,
                AuthMode = AuthMode.Password,
                PasswordHash = PasswordHasher.Hash(password),
                UsbDeviceId = string.Empty,
                Enabled = true
            };

            SaveFirstAdministrator(administrator);
            return Copy(administrator);
        }

        public IEnumerable<Account> GetAll()
        {
            // Reread for each request so an externally disabled/deleted account is revoked.
            return (LoadAccounts() ?? new List<Account>()).Select(Copy).ToArray();
        }

        public Account? GetByUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;
            Account? account = LoadAccounts()?.FirstOrDefault(a =>
                string.Equals(a.UserName, userName.Trim(), StringComparison.OrdinalIgnoreCase));
            return account == null ? null : Copy(account);
        }

        public bool ValidateCredentials(string userName, string password)
        {
            if (password == null || password.Length > 128)
                return false;
            Account? account = GetByUserName(userName);
            return account is { Enabled: true, AuthMode: AuthMode.Password }
                && PasswordHasher.Verify(password, account.PasswordHash);
        }

        // USB authentication is not implemented; it must not silently become password-only.
        public bool ValidateUsbBinding(string userName, string usbDeviceId) => false;

        public bool ValidateLogin(string userName, string? password, string? usbDeviceId)
        {
            return password != null && ValidateCredentials(userName, password);
        }

        // Account management needs its own authorized workflow before these APIs can write.
        public void Create(Account account) => throw new NotSupportedException("尚未提供受保护的账户管理功能。");
        public void Update(Account account) => throw new NotSupportedException("尚未提供受保护的账户管理功能。");
        public void Delete(string userName) => throw new NotSupportedException("尚未提供受保护的账户管理功能。");

        private List<Account>? LoadAccounts()
        {
            try
            {
                using var stream = new FileStream(_storagePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                if (stream.Length == 0 || stream.Length > MaximumStoreBytes)
                    throw InvalidStore();

                // A bounded copy also limits files that grow after the initial length check.
                using var content = new MemoryStream();
                var buffer = new byte[8192];
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    if (content.Length + read > MaximumStoreBytes)
                        throw InvalidStore();
                    content.Write(buffer, 0, read);
                }
                content.Position = 0;
                using JsonDocument document = JsonDocument.Parse(content, new JsonDocumentOptions { MaxDepth = 8 });
                JsonElement root = document.RootElement;
                if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0 || root.GetArrayLength() > MaximumAccounts)
                    throw InvalidStore();

                var accounts = new List<Account>();
                var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (JsonElement entry in root.EnumerateArray())
                {
                    Account account = ReadAccount(entry);
                    if (!ids.Add(account.Id) || !names.Add(account.UserName))
                        throw InvalidStore();
                    accounts.Add(account);
                }
                return accounts;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }
            catch (JsonException ex)
            {
                throw InvalidStore(ex);
            }
            catch (InvalidOperationException ex)
            {
                // JsonDocument also reports malformed UTF-16 escapes during string access.
                throw InvalidStore(ex);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is SecurityException)
            {
                throw new AccountStorageException("无法读取本地管理员账户。公告仍可查看，请检查数据目录权限或恢复账户备份。", ex);
            }
        }

        private static Account ReadAccount(JsonElement entry)
        {
            if (entry.ValueKind != JsonValueKind.Object)
                throw InvalidStore();
            var properties = new HashSet<string>(StringComparer.Ordinal);
            foreach (JsonProperty property in entry.EnumerateObject())
            {
                if (!properties.Add(property.Name) || property.Name is not
                    ("Id" or "UserName" or "Role" or "AuthMode" or "PasswordHash" or "UsbDeviceId" or "Enabled"))
                    throw InvalidStore();
            }
            if (properties.Count != 7)
                throw InvalidStore();

            string id = ReadString(entry, "Id");
            string userName = ReadString(entry, "UserName");
            string hash = ReadString(entry, "PasswordHash");
            string usbDeviceId = ReadString(entry, "UsbDeviceId");
            JsonElement role = entry.GetProperty("Role");
            JsonElement mode = entry.GetProperty("AuthMode");
            JsonElement enabled = entry.GetProperty("Enabled");
            if (!IsValidIdentifier(id, 128) || !IsValidUserName(userName) ||
                role.ValueKind != JsonValueKind.Number || mode.ValueKind != JsonValueKind.Number ||
                !role.TryGetInt32(out int roleNumber) || !Enum.IsDefined(typeof(Role), roleNumber) ||
                !mode.TryGetInt32(out int modeNumber) || !Enum.IsDefined(typeof(AuthMode), modeNumber) ||
                enabled.ValueKind is not (JsonValueKind.True or JsonValueKind.False) ||
                usbDeviceId.Length > 256 || usbDeviceId.Any(char.IsControl))
                throw InvalidStore();

            // Password modes always require a well-formed hash, including disabled accounts.
            // A USB-only account may have no password, but a supplied hash must still be valid.
            if (((AuthMode)modeNumber != AuthMode.Usb || hash.Length != 0) && !PasswordHasher.IsValidHash(hash))
                throw InvalidStore();

            return new Account
            {
                Id = id,
                UserName = userName,
                Role = (Role)roleNumber,
                AuthMode = (AuthMode)modeNumber,
                PasswordHash = hash,
                UsbDeviceId = usbDeviceId,
                Enabled = enabled.GetBoolean()
            };
        }

        private static string ReadString(JsonElement entry, string property)
        {
            JsonElement value = entry.GetProperty(property);
            if (value.ValueKind != JsonValueKind.String)
                throw InvalidStore();
            return value.GetString()!;
        }

        private void SaveFirstAdministrator(Account account)
        {
            string directory = Path.GetDirectoryName(_storagePath)!;
            string temporaryPath = Path.Combine(directory, ".accounts-" + Guid.NewGuid().ToString("N") + ".tmp");
            try
            {
                Directory.CreateDirectory(directory);
                using (var stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                    4096, FileOptions.WriteThrough))
                {
                    JsonSerializer.Serialize(stream, new[] { account }, new JsonSerializerOptions { WriteIndented = true });
                    stream.Flush(flushToDisk: true);
                }
                // Same-directory rename is atomic; the existing destination is never overwritten.
                File.Move(temporaryPath, _storagePath, overwrite: false);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is SecurityException)
            {
                throw new AccountStorageException("首次管理员账户保存失败，尚未授予管理权限。请检查数据目录权限，或重新打开登录窗口检查是否已完成设置。", ex);
            }
            finally
            {
                try { File.Delete(temporaryPath); }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
                catch (SecurityException) { }
            }
        }

        private static bool IsValidUserName(string value) => IsValidIdentifier(value, 64);

        private static bool IsValidIdentifier(string value, int maximumLength)
        {
            return !string.IsNullOrWhiteSpace(value) && value.Length <= maximumLength &&
                string.Equals(value, value.Trim(), StringComparison.Ordinal) && !value.Any(char.IsControl);
        }

        private static Account Copy(Account account) => new Account
        {
            Id = account.Id,
            UserName = account.UserName,
            Role = account.Role,
            AuthMode = account.AuthMode,
            PasswordHash = account.PasswordHash,
            UsbDeviceId = account.UsbDeviceId,
            Enabled = account.Enabled
        };

        private static AccountStorageException InvalidStore(Exception? inner = null) =>
            new AccountStorageException("本地管理员账户文件损坏或无效。公告仍可查看，请恢复有效的账户备份；不会自动重新设置管理员。", inner);
    }
}


