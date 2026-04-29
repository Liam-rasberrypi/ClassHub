// AppSettings.cs
// 应用设置数据模型，集中管理全局设置
namespace ClassHub.Models
{
    public class AppSettings
    {
        // TODO: 浅色/深色主题
        public string Theme { get; set; } = "Light";

        // TODO: 窗口模式（Windowed/Fullscreen）
        public string WindowMode { get; set; } = "Windowed";

        // TODO: UI 缩放比例
        public float UiScale { get; set; } = 1.0f;

        // TODO: 是否开机自启
        public bool AutoStart { get; set; }

        // TODO: 数据目录（默认本地）
        public string DataFolder { get; set; } = string.Empty;
    }
}

