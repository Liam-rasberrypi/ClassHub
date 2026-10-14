using ClassHub.Models;
using ClassHub.Services;

namespace ClassHub.Tests;

public sealed class NoticeAuthorizationTests
{
    private const string Password = "ClassHub-Test-Password";

    [Fact]
    public void GuestReadsExistingNoticesAndEveryWriteFailsWithoutChangingDiskOrMemory()
    {
        using var workspace = new TestWorkspace();
        var original = new NoticeItem { Title = "Existing notice", Content = "Keep this content", IsPinned = true };
        workspace.WriteNotices(original);
        var persisted = File.ReadAllBytes(workspace.NoticePath);
        var security = new SecurityService(new AccountService(workspace.AccountPath));
        var notices = new NoticeService(security, workspace.NoticePath);
        Assert.Equal(original.Title, Assert.Single(notices.GetAll()).Title);
        Assert.Throws<UnauthorizedAccessException>(() => notices.Add("New", "Unauthorized", false));
        Assert.Throws<UnauthorizedAccessException>(() => notices.TogglePin(original.Id));
        Assert.Throws<UnauthorizedAccessException>(() => notices.Delete(original.Id));
        Assert.Throws<UnauthorizedAccessException>(() => notices.TogglePin(Guid.NewGuid()));
        Assert.Throws<UnauthorizedAccessException>(() => notices.Delete(Guid.NewGuid()));
        Assert.Equal(persisted, File.ReadAllBytes(workspace.NoticePath));
        Assert.Equal(original.Id, Assert.Single(notices.GetAll()).Id);
        Assert.True(Assert.Single(notices.GetAll()).IsPinned);
    }

    [Fact]
    public void AuthenticatedAdministratorCanPublishPinAndDeleteWithPersistence()
    {
        using var workspace = new TestWorkspace();
        var security = new SecurityService(new AccountService(workspace.AccountPath));
        Assert.True(security.InitializeAdministrator("teacher", Password));
        var notices = new NoticeService(security, workspace.NoticePath);
        var created = notices.Add(" Published title ", " Published body ", false);
        Assert.Equal("Published title", created.Title);
        Assert.Equal("Published body", created.Content);
        notices.TogglePin(created.Id);
        Assert.True(Assert.Single(notices.GetAll()).IsPinned);
        var guestAfterRestart = new NoticeService(new SecurityService(new AccountService(workspace.AccountPath)), workspace.NoticePath);
        Assert.Equal(created.Id, Assert.Single(guestAfterRestart.GetAll()).Id);
        Assert.True(Assert.Single(guestAfterRestart.GetAll()).IsPinned);
        notices.Delete(created.Id);
        Assert.Empty(notices.GetAll());
        Assert.Empty(new NoticeService(security, workspace.NoticePath).GetAll());
    }

    [Fact]
    public void NoticeReturnValuesCannotBypassPermissionChecks()
    {
        using var workspace = new TestWorkspace();
        var security = new SecurityService(new AccountService(workspace.AccountPath));
        security.InitializeAdministrator("teacher", Password);
        var notices = new NoticeService(security, workspace.NoticePath);
        var created = notices.Add("Original", "Original content", false);
        var persisted = File.ReadAllBytes(workspace.NoticePath);
        security.Logout();
        created.Title = "Changed through Add return value";
        var returned = Assert.Single(notices.GetAll());
        returned.Title = "Changed through list item";
        returned.IsPinned = true;
        var current = Assert.Single(notices.GetAll());
        Assert.Equal("Original", current.Title);
        Assert.False(current.IsPinned);
        Assert.Equal(persisted, File.ReadAllBytes(workspace.NoticePath));
    }

    [Fact]
    public void ExpiredSessionBlocksAllNoticeMutationsButPreservesViewing()
    {
        using var workspace = new TestWorkspace();
        var clock = new ManualTimeProvider();
        var security = new SecurityService(new AccountService(workspace.AccountPath), clock);
        security.InitializeAdministrator("teacher", Password);
        var notices = new NoticeService(security, workspace.NoticePath);
        var created = notices.Add("Original", "Body", false);
        var persisted = File.ReadAllBytes(workspace.NoticePath);
        clock.Advance(TimeSpan.FromMinutes(2));
        Assert.Throws<UnauthorizedAccessException>(() => notices.Add("New", "Body", false));
        Assert.Throws<UnauthorizedAccessException>(() => notices.TogglePin(created.Id));
        Assert.Throws<UnauthorizedAccessException>(() => notices.Delete(created.Id));
        Assert.Single(notices.GetAll());
        Assert.Equal(persisted, File.ReadAllBytes(workspace.NoticePath));
    }

    [Fact]
    public void BrokenAccountStorageKeepsExistingNoticesReadableAndCannotGrantWrites()
    {
        using var workspace = new TestWorkspace();
        workspace.WriteNotices(new NoticeItem { Title = "Still readable", Content = "Existing classroom notice" });
        File.WriteAllText(workspace.AccountPath, "{broken JSON");
        var security = new SecurityService(new AccountService(workspace.AccountPath));
        var notices = new NoticeService(security, workspace.NoticePath);
        Assert.Equal("Still readable", Assert.Single(notices.GetAll()).Title);
        Assert.Throws<AccountStorageException>(() => security.Login("teacher", Password));
        Assert.Null(security.CurrentAccountId);
        Assert.Throws<UnauthorizedAccessException>(() => notices.Add("New", "Body", false));
        Assert.Equal("{broken JSON", File.ReadAllText(workspace.AccountPath));
        Assert.Single(notices.GetAll());
    }

    [Fact]
    public void FailedNoticeSaveDoesNotMutateInMemoryItems()
    {
        using var workspace = new TestWorkspace();
        var security = new SecurityService(new AccountService(workspace.AccountPath));
        security.InitializeAdministrator("teacher", Password);
        var notices = new NoticeService(security, workspace.NoticePath);
        var original = notices.Add("Original", "Body", false);
        File.Delete(workspace.NoticePath);
        Directory.CreateDirectory(workspace.NoticePath);
        Assert.ThrowsAny<IOException>(() => notices.Add("New", "Body", false));
        Assert.ThrowsAny<IOException>(() => notices.TogglePin(original.Id));
        Assert.ThrowsAny<IOException>(() => notices.Delete(original.Id));
        var remaining = Assert.Single(notices.GetAll());
        Assert.Equal(original.Id, remaining.Id);
        Assert.False(remaining.IsPinned);
    }
}
