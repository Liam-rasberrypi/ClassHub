using ClassHub.Models;
using ClassHub.Services;

namespace ClassHub.Tests;

public sealed class SecurityServiceTests
{
    private const string Password = "ClassHub-Test-Password";

    [Fact]
    public void GuestCanViewButCannotClaimAdministratorBySupplyingAnId()
    {
        using var workspace = new TestWorkspace();
        var accounts = new AccountService(workspace.AccountPath);
        var administrator = accounts.InitializeAdministrator("teacher", Password);
        var security = new SecurityService(accounts);
        Assert.Null(security.CurrentAccountId);
        Assert.True(security.HasPermission(string.Empty, PermissionKey.NoticeView));
        Assert.False(security.HasPermission(administrator.Id, PermissionKey.NoticeEdit));
        Assert.False(security.HasPermission("unknown-account", PermissionKey.NoticeEdit));
        Assert.Throws<UnauthorizedAccessException>(() => security.DemandPermission(PermissionKey.NoticeEdit));
    }

    [Fact]
    public void SuccessfulLoginGrantsOnlySupportedPermissionAndLogoutRevokesIt()
    {
        using var workspace = new TestWorkspace();
        var accounts = new AccountService(workspace.AccountPath);
        var administrator = accounts.InitializeAdministrator("teacher", Password);
        var security = new SecurityService(accounts);
        Assert.True(security.Login("teacher", Password));
        Assert.Equal(administrator.Id, security.CurrentAccountId);
        Assert.True(security.HasPermission(administrator.Id, PermissionKey.NoticeEdit));
        Assert.False(security.HasPermission("spoofed-id", PermissionKey.NoticeEdit));
        Assert.False(security.HasPermission(administrator.Id, PermissionKey.AccountManage));
        Assert.False(security.HasPermission(administrator.Id, "unknown.permission"));
        security.DemandPermission(PermissionKey.NoticeEdit);
        security.Logout();
        Assert.Null(security.CurrentAccountId);
        Assert.False(security.HasPermission(administrator.Id, PermissionKey.NoticeEdit));
    }

    [Fact]
    public void FailedLoginClearsAnyPreviouslyAuthenticatedSession()
    {
        using var workspace = new TestWorkspace();
        var accounts = new AccountService(workspace.AccountPath);
        var administrator = accounts.InitializeAdministrator("teacher", Password);
        var security = new SecurityService(accounts);
        Assert.True(security.Login("teacher", Password));
        Assert.False(security.Login("teacher", "Incorrect-Password"));
        Assert.Null(security.CurrentAccountId);
        Assert.False(security.HasPermission(administrator.Id, PermissionKey.NoticeEdit));
        Assert.False(security.VerifyLogin("unknown-account", Password, null));
    }

    [Fact]
    public void VerifyLoginChecksCredentialsInsteadOfTrustingTheAccountId()
    {
        using var workspace = new TestWorkspace();
        var accounts = new AccountService(workspace.AccountPath);
        var administrator = accounts.InitializeAdministrator("teacher", Password);
        var security = new SecurityService(accounts);
        Assert.False(security.VerifyLogin(administrator.Id, "Incorrect-Password", null));
        Assert.False(security.HasPermission(administrator.Id, PermissionKey.NoticeEdit));
        Assert.True(security.VerifyLogin(administrator.Id, Password, null));
        Assert.True(security.HasPermission(administrator.Id, PermissionKey.NoticeEdit));
    }

    [Fact]
    public void FirstSetupGrantsSessionOnlyAfterSuccessfulPersistence()
    {
        using var workspace = new TestWorkspace();
        var security = new SecurityService(new AccountService(workspace.AccountPath));
        Assert.True(security.RequiresAdministratorSetup);
        Assert.Throws<ArgumentException>(() => security.InitializeAdministrator("teacher", "short"));
        Assert.Null(security.CurrentAccountId);
        Assert.False(File.Exists(workspace.AccountPath));
        Assert.True(security.InitializeAdministrator("teacher", Password));
        Assert.NotNull(security.CurrentAccountId);
        security.DemandPermission(PermissionKey.NoticeEdit);
        Assert.True(new AccountService(workspace.AccountPath).ValidateCredentials("teacher", Password));
    }

    [Fact]
    public void FailedSetupPersistenceDoesNotGrantSession()
    {
        using var workspace = new TestWorkspace();
        Directory.CreateDirectory(workspace.AccountPath);
        var security = new SecurityService(new AccountService(workspace.AccountPath));
        Assert.Throws<AccountStorageException>(() => security.InitializeAdministrator("teacher", Password));
        Assert.Null(security.CurrentAccountId);
        Assert.Throws<UnauthorizedAccessException>(() => security.DemandPermission(PermissionKey.NoticeEdit));
    }

    [Theory]
    [InlineData(Role.Student, true, AuthMode.Password)]
    [InlineData(Role.Teacher, true, AuthMode.Password)]
    [InlineData(Role.Deputy, true, AuthMode.Password)]
    [InlineData(Role.Admin, false, AuthMode.Password)]
    [InlineData(Role.Admin, true, AuthMode.Usb)]
    [InlineData(Role.Admin, true, AuthMode.PasswordAndUsb)]
    public void UnsupportedAccountsCannotGrantNoticeWritePermission(Role role, bool enabled, AuthMode mode)
    {
        using var workspace = new TestWorkspace();
        workspace.WriteAccounts(new Account
        {
            Id = "test-account", UserName = "teacher", Role = role, Enabled = enabled,
            AuthMode = mode, UsbDeviceId = "bound-usb", PasswordHash = PasswordHasher.Hash(Password)
        });
        var security = new SecurityService(new AccountService(workspace.AccountPath));
        security.Login("teacher", Password);
        Assert.False(security.HasPermission("test-account", PermissionKey.NoticeEdit));
        security.VerifyLogin("test-account", Password, "bound-usb");
        Assert.False(security.HasPermission("test-account", PermissionKey.NoticeEdit));
        Assert.Throws<UnauthorizedAccessException>(() => security.DemandPermission(PermissionKey.NoticeEdit));
    }

    [Theory]
    [InlineData("role")]
    [InlineData("enabled")]
    [InlineData("password")]
    [InlineData("auth-mode")]
    [InlineData("deleted")]
    public void PermissionsRereadStorageAndRevokeChangedAccounts(string change)
    {
        using var workspace = new TestWorkspace();
        var accounts = new AccountService(workspace.AccountPath);
        var administrator = accounts.InitializeAdministrator("teacher", Password);
        var security = new SecurityService(accounts);
        Assert.True(security.Login("teacher", Password));
        switch (change)
        {
            case "role": administrator.Role = Role.Student; break;
            case "enabled": administrator.Enabled = false; break;
            case "password": administrator.PasswordHash = PasswordHasher.Hash("Replacement-Password"); break;
            case "auth-mode": administrator.AuthMode = AuthMode.Usb; break;
        }
        // Keep a valid store after deleting the administrator; an empty store is corruption.
        var student = new Account
        {
            Id = "student-id", UserName = "student", Role = Role.Student,
            PasswordHash = administrator.PasswordHash
        };
        workspace.WriteAccounts(change == "deleted" ? new[] { student } : new[] { administrator });
        Assert.False(security.HasPermission(administrator.Id, PermissionKey.NoticeEdit));
        Assert.Throws<UnauthorizedAccessException>(() => security.DemandPermission(PermissionKey.NoticeEdit));
    }

    [Fact]
    public void SessionExpiresWithoutRealTimeWaitingAndDoesNotRenewOnReads()
    {
        using var workspace = new TestWorkspace();
        var clock = new ManualTimeProvider();
        var accounts = new AccountService(workspace.AccountPath);
        var administrator = accounts.InitializeAdministrator("teacher", Password);
        var security = new SecurityService(accounts, clock);
        Assert.True(security.Login("teacher", Password));
        clock.Advance(TimeSpan.FromSeconds(119));
        Assert.True(security.HasPermission(administrator.Id, PermissionKey.NoticeEdit));
        clock.Advance(TimeSpan.FromSeconds(1));
        Assert.False(security.HasPermission(administrator.Id, PermissionKey.NoticeEdit));
        Assert.Null(security.CurrentAccountId);
        Assert.True(security.HasPermission(string.Empty, PermissionKey.NoticeView));
    }

    [Theory]
    [InlineData("permission")]
    [InlineData("setup-status")]
    public void CorruptStoreRevokesSessionEvenIfAValidBackupIsLaterRestored(string check)
    {
        using var workspace = new TestWorkspace();
        var accounts = new AccountService(workspace.AccountPath);
        var administrator = accounts.InitializeAdministrator("teacher", Password);
        var security = new SecurityService(accounts);
        Assert.True(security.Login("teacher", Password));
        var backup = File.ReadAllBytes(workspace.AccountPath);
        File.WriteAllText(workspace.AccountPath, "{corrupt JSON");
        if (check == "permission")
            Assert.Throws<AccountStorageException>(() => security.DemandPermission(PermissionKey.NoticeEdit));
        else
            Assert.Throws<AccountStorageException>(() => _ = security.RequiresAdministratorSetup);
        File.WriteAllBytes(workspace.AccountPath, backup);
        Assert.Null(security.CurrentAccountId);
        Assert.False(security.HasPermission(administrator.Id, PermissionKey.NoticeEdit));
        Assert.Throws<UnauthorizedAccessException>(() => security.DemandPermission(PermissionKey.NoticeEdit));
        Assert.True(security.Login("teacher", Password));
        security.DemandPermission(PermissionKey.NoticeEdit);
    }

    [Fact]
    public void FiveFailuresTemporarilyLockLoginAndLogoutDoesNotBypassIt()
    {
        using var workspace = new TestWorkspace();
        var clock = new ManualTimeProvider();
        var accounts = new AccountService(workspace.AccountPath);
        accounts.InitializeAdministrator("teacher", Password);
        var security = new SecurityService(accounts, clock);
        for (var attempt = 0; attempt < 5; attempt++)
            Assert.False(security.Login("teacher", "Incorrect-Password"));
        security.Logout();
        Assert.False(security.Login("teacher", Password));
        clock.Advance(TimeSpan.FromSeconds(29));
        Assert.False(security.Login("teacher", Password));
        clock.Advance(TimeSpan.FromSeconds(1));
        Assert.True(security.Login("teacher", Password));
    }
}
