// Forms/MainForm.cs
// WinForms 主窗体：主界面为“四大模块卡片”，支持进入各模块二级页面
using System.Windows.Forms;
using ClassHub.Infrastructure;
using ClassHub.Modules;

namespace ClassHub.Forms
{
    public class MainForm : Form
    {
        private readonly ClassHubAppContext _appContext;

        // TODO: 后续替换为“卡片式主界面 + 二级功能区”的标准布局
        private readonly Panel _moduleHost = new();
        private readonly FlowLayoutPanel _navPanel = new();

        public MainForm(ClassHubAppContext appContext)
        {
            _appContext = appContext;

            // TODO: 设置窗口基础属性（标题、尺寸、图标、DPI、最小尺寸）
            Text = "班级全能管理助手";
            Width = 1280;
            Height = 800;
            StartPosition = FormStartPosition.CenterScreen;

            InitializeLayout();
            InitializeNavigation();

            // TODO: 默认进入“主界面四大模块卡片”
            OpenModule(ModuleKeys.ClassManagementCenter);
        }

        private void InitializeLayout()
        {
            // TODO: 布局要求（适配低分辨率）：左侧大按钮导航 + 右侧模块承载区 + 底部状态栏
            _navPanel.Dock = DockStyle.Left;
            _navPanel.Width = 240;
            _navPanel.FlowDirection = FlowDirection.TopDown;
            _navPanel.WrapContents = false;

            _moduleHost.Dock = DockStyle.Fill;

            Controls.Add(_moduleHost);
            Controls.Add(_navPanel);
        }

        private void InitializeNavigation()
        {
            // 一级模块（与产品大纲保持一致）
            AddNavButton("班级日常管理", ModuleKeys.ClassManagementCenter);
            AddNavButton("教学辅助工具", ModuleKeys.TeachingAssistCenter);
            AddNavButton("电脑一键优化", ModuleKeys.SystemOptimizationCenter);
            AddNavButton("软件基础设置", ModuleKeys.SettingsCenter);
        }

        private void AddNavButton(string text, string moduleKey)
        {
            var button = new Button
            {
                Text = text,
                Width = 220,
                Height = 54,
                Margin = new Padding(10, 10, 10, 0)
            };

            button.Click += (_, _) => OpenModule(moduleKey);
            _navPanel.Controls.Add(button);
        }

        private void OpenModule(string moduleKey)
        {
            // TODO: 切换模块前执行权限校验、未保存提示、低配模式资源释放
            var control = _appContext.NavigationService.Create(moduleKey);
            if (control is ModuleControlBase moduleControl)
            {
                moduleControl.Initialize(_appContext);
            }

            control.Dock = DockStyle.Fill;
            _moduleHost.Controls.Clear();
            _moduleHost.Controls.Add(control);
        }
    }
}
