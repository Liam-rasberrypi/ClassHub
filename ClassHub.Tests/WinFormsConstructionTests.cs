using System.Runtime.ExceptionServices;
using System.Reflection;
using System.Windows.Forms;
using ClassHub.Forms;
using ClassHub.Infrastructure;
using ClassHub.Models;
using ClassHub.Modules;
using ClassHub.Services;

namespace ClassHub.Tests;

public sealed class WinFormsConstructionTests
{
    [Fact]
    public void NoticeModuleCanBeConstructedOnAWindowsStaThread()
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                using var control = new NoticeModuleControl();
                control.CreateControl();
            }
            catch (Exception exception) { failure = exception; }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(10)), "Notice module construction timed out.");
        if (failure != null) ExceptionDispatchInfo.Capture(failure).Throw();
    }

    [Fact]
    public void MainNavigationOpensNoticesWithoutAuthentication()
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                using var workspace = new TestWorkspace();
                workspace.WriteNotices(new NoticeItem { Title = "公开公告", Content = "无需登录查看" });
                var accounts = new AccountService(workspace.AccountPath);
                var security = new SecurityService(accounts);
                var notices = new NoticeService(security, workspace.NoticePath);
                var navigation = new NavigationService();
                navigation.Register(ModuleKeys.ClassManagementCenter, () => new ClassManagementCenterModuleControl());
                navigation.Register(ModuleKeys.Notice, () => new NoticeModuleControl());
                var context = new ClassHubAppContext(accounts, new DataService(), new FileService(),
                    new SettingsService(), new SystemOptimizerService(), security, new ClassroomPolicyService(),
                    notices, navigation);
                using var form = new MainForm(context);
                var nav = form.Controls.OfType<FlowLayoutPanel>().Single();
                var button = nav.Controls.OfType<Button>().Single(x => x.Text == "班级公告");
                // Raise the real button event without displaying a test window on the desktop.
                typeof(Button).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic)!
                    .Invoke(button, new object[] { EventArgs.Empty });
                var host = form.Controls.OfType<Panel>().Single(x => x is not FlowLayoutPanel);
                Assert.IsType<NoticeModuleControl>(Assert.Single(host.Controls.Cast<Control>()));
                Assert.Null(security.CurrentAccountId);
                Assert.False(File.Exists(workspace.AccountPath));
                Assert.Single(notices.GetAll());
            }
            catch (Exception exception) { failure = exception; }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(10)), "Guest notice navigation timed out.");
        if (failure != null) ExceptionDispatchInfo.Capture(failure).Throw();
    }
}
