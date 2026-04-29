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

            // TODO: 如果有登录流程，可先弹出 LoginForm 做身份验证，再进入主窗体
            Application.Run(new MainForm(appContext));
        }
    }
}
