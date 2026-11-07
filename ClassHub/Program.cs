// Program.cs
// WinForms 应用程序入口（替代 WinUI 的 App.xaml 启动方式）
using System;
using System.Windows.Forms;
using ClassHub.Infrastructure;
using ClassHub.Forms;

namespace ClassHub
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // TODO: 在此完成应用初始化流程（日志、配置加载、异常处理、依赖注入等）
            var appContext = Bootstrapper.Build();

            // 以访客方式启动，公告查看无需登录。每次公告修改由模块弹出管理员验证，
            // 并由公告服务校验权限；账户不可用也不会阻断主窗体和公告浏览。
            Application.Run(new MainForm(appContext));
        }
    }
}
