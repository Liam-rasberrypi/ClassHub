using System.Text.Json;
using ClassHub.Models;
using ClassHub.Services;

namespace ClassHub.Tests;

public sealed class AccountServiceTests
{
    private const string Password = "ClassHub-Test-Password";

    [Fact]
    public void FirstAdministratorIsPersistedAndUsableAfterRestart()
    {
        using var workspace = new TestWorkspace();
        var accounts = new AccountService(workspace.AccountPath);
        Assert.True(accounts.RequiresAdministratorSetup);
        var administrator = accounts.InitializeAdministrator("teacher", Password);
        Assert.Equal(Role.Admin, administrator.Role);
        Assert.True(administrator.Enabled);
        Assert.Equal(AuthMode.Password, administrator.AuthMode);
        Assert.False(string.IsNullOrWhiteSpace(administrator.Id));
        Assert.False(accounts.RequiresAdministratorSetup);
        Assert.DoesNotContain(Password, File.ReadAllText(workspace.AccountPath));
        var restarted = new AccountService(workspace.AccountPath);
        Assert.False(restarted.RequiresAdministratorSetup);
        Assert.True(restarted.ValidateCredentials("teacher", Password));
        Assert.True(restarted.ValidateLogin("teacher", Password, null));
        Assert.False(restarted.ValidateCredentials("teacher", "Incorrect-Password"));
        Assert.False(restarted.ValidateCredentials("unknown", Password));
        Assert.False(restarted.ValidateUsbBinding("teacher", "usb-id"));
    }

    [Fact]
    public void ExistingAdministratorCannotBeOverwrittenByFirstSetup()
    {
        using var workspace = new TestWorkspace();
        var first = new AccountService(workspace.AccountPath);
        var second = new AccountService(workspace.AccountPath);
        first.InitializeAdministrator("teacher", Password);
        var stored = File.ReadAllBytes(workspace.AccountPath);
        Assert.Throws<InvalidOperationException>(() => second.InitializeAdministrator("attacker", "Attacker-Password"));
        Assert.Equal(stored, File.ReadAllBytes(workspace.AccountPath));
        Assert.Null(second.GetByUserName("attacker"));
    }

    [Fact]
    public async Task ConcurrentFirstSetupPersistsExactlyOneAdministrator()
    {
        using var workspace = new TestWorkspace();
        using var start = new ManualResetEventSlim();
        Task<Exception?> Attempt(string userName) => Task.Run<Exception?>(() =>
        {
            var accounts = new AccountService(workspace.AccountPath);
            start.Wait();
            return Record.Exception(() => accounts.InitializeAdministrator(userName, Password));
        });
        var first = Attempt("teacher-one");
        var second = Attempt("teacher-two");
        start.Set();
        var results = await Task.WhenAll(first, second);
        Assert.Single(results, result => result == null);
        Assert.Single(results, result => result is InvalidOperationException or AccountStorageException);
        var persisted = Assert.Single(new AccountService(workspace.AccountPath).GetAll());
        Assert.True(new AccountService(workspace.AccountPath).ValidateCredentials(persisted.UserName, Password));
    }

    [Fact]
    public void ReturnedAccountModelsCannotMutateStoredRolesOrPasswords()
    {
        using var workspace = new TestWorkspace();
        var accounts = new AccountService(workspace.AccountPath);
        var created = accounts.InitializeAdministrator("teacher", Password);
        created.Role = Role.Student;
        created.PasswordHash = "plaintext";
        var found = accounts.GetByUserName("teacher")!;
        found.Enabled = false;
        var all = Assert.Single(accounts.GetAll());
        all.Role = Role.Student;
        Assert.True(accounts.ValidateCredentials("teacher", Password));
        Assert.Equal(Role.Admin, accounts.GetByUserName("teacher")!.Role);
        Assert.True(accounts.GetByUserName("teacher")!.Enabled);
        Assert.True(PasswordHasher.IsValidHash(accounts.GetByUserName("teacher")!.PasswordHash));
    }

    [Theory]
    [InlineData("")]
    [InlineData("{")]
    [InlineData("null")]
    [InlineData("{}")]
    [InlineData("[]")]
    public void CorruptExistingStorageIsNotTreatedAsFirstRunOrOverwritten(string contents)
    {
        using var workspace = new TestWorkspace();
        File.WriteAllText(workspace.AccountPath, contents);
        var accounts = new AccountService(workspace.AccountPath);
        Assert.Throws<AccountStorageException>(() => _ = accounts.RequiresAdministratorSetup);
        Assert.Throws<AccountStorageException>(() => accounts.InitializeAdministrator("teacher", Password));
        Assert.Equal(contents, File.ReadAllText(workspace.AccountPath));
    }

    [Fact]
    public void PlaintextPasswordInExistingAccountFileIsRejected()
    {
        using var workspace = new TestWorkspace();
        workspace.WriteAccounts(new Account { Id = "teacher-id", UserName = "teacher", Role = Role.Admin, PasswordHash = Password });
        var accounts = new AccountService(workspace.AccountPath);
        Assert.Throws<AccountStorageException>(() => accounts.ValidateCredentials("teacher", Password));
    }

    [Theory]
    [InlineData("string-role")]
    [InlineData("unknown-role")]
    [InlineData("null-enabled")]
    [InlineData("missing-enabled")]
    [InlineData("duplicate-enabled")]
    [InlineData("duplicate-id")]
    [InlineData("invalid-unicode")]
    [InlineData("oversized")]
    public void InvalidAccountSchemaAndOversizedStoresFailClosed(string invalidKind)
    {
        using var workspace = new TestWorkspace();
        var accounts = new AccountService(workspace.AccountPath);
        var administrator = accounts.InitializeAdministrator("teacher", Password);
        var entry = JsonSerializer.Serialize(administrator);
        var invalid = invalidKind switch
        {
            "string-role" => "[" + entry.Replace("\"Role\":0", "\"Role\":\"Admin\"") + "]",
            "unknown-role" => "[" + entry.Replace("\"Role\":0", "\"Role\":999") + "]",
            "null-enabled" => "[" + entry.Replace("\"Enabled\":true", "\"Enabled\":null") + "]",
            "missing-enabled" => "[" + entry.Replace(",\"Enabled\":true", string.Empty) + "]",
            "duplicate-enabled" => "[" + entry.Replace("\"Enabled\":true", "\"Enabled\":false,\"Enabled\":true") + "]",
            "duplicate-id" => "[" + entry + "," + entry + "]",
            "invalid-unicode" => "[" + entry.Replace("\"UserName\":\"teacher\"", "\"UserName\":\"\\uD800\"") + "]",
            "oversized" => "[" + entry + "]" + new string(' ', 1024 * 1024),
            _ => throw new InvalidOperationException()
        };
        File.WriteAllText(workspace.AccountPath, invalid);
        Assert.Throws<AccountStorageException>(() => _ = accounts.RequiresAdministratorSetup);
        Assert.Throws<AccountStorageException>(() => accounts.ValidateCredentials("teacher", Password));
        Assert.Throws<AccountStorageException>(() => accounts.InitializeAdministrator("replacement", Password));
        Assert.Equal(invalid, File.ReadAllText(workspace.AccountPath));
    }

    [Fact]
    public void FailedSetupDoesNotLeaveAnAdministratorOrOverwriteExistingPath()
    {
        using var workspace = new TestWorkspace();
        Directory.CreateDirectory(workspace.AccountPath);
        var accounts = new AccountService(workspace.AccountPath);
        Assert.Throws<AccountStorageException>(() => accounts.InitializeAdministrator("teacher", Password));
        Assert.True(Directory.Exists(workspace.AccountPath));
        Assert.Empty(Directory.GetFiles(workspace.AccountPath));
    }

    [Fact]
    public void UnauthenticatedAccountCrudCannotProvisionOrChangeAnAdministrator()
    {
        using var workspace = new TestWorkspace();
        var accounts = new AccountService(workspace.AccountPath);
        Assert.Throws<NotSupportedException>(() => accounts.Create(new Account { Role = Role.Admin }));
        Assert.Throws<NotSupportedException>(() => accounts.Update(new Account { Role = Role.Admin }));
        Assert.Throws<NotSupportedException>(() => accounts.Delete("teacher"));
        Assert.False(File.Exists(workspace.AccountPath));
    }

    [Fact]
    public void LockedAccountFileCannotEnableSetupOrAuthentication()
    {
        using var workspace = new TestWorkspace();
        var accounts = new AccountService(workspace.AccountPath);
        accounts.InitializeAdministrator("teacher", Password);
        var original = File.ReadAllBytes(workspace.AccountPath);
        using (var locked = new FileStream(workspace.AccountPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
        {
            Assert.Throws<AccountStorageException>(() => _ = accounts.RequiresAdministratorSetup);
            Assert.Throws<AccountStorageException>(() => accounts.ValidateCredentials("teacher", Password));
            Assert.Throws<AccountStorageException>(() => accounts.InitializeAdministrator("replacement", Password));
        }
        Assert.Equal(original, File.ReadAllBytes(workspace.AccountPath));
    }
}
