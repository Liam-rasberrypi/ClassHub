// SettingsService.cs
// 设置服务：应用配置读取、变更、持久化
using ClassHub.Models;

namespace ClassHub.Services
{
    public interface ISettingsService
    {
        AppSettings Load();
        void Save(AppSettings settings);
        void Apply(AppSettings settings);

        // TODO: 开机自启设置
        void SetAutoStart(bool enabled);

        // TODO: 全屏/窗口模式切换偏好
        void SetWindowMode(string modeKey);
    }

    public class SettingsService : ISettingsService
    {
        public AppSettings Load()
        {
            // TODO: 从本地配置文件加载设置
            return new AppSettings();
        }

        public void Save(AppSettings settings)
        {
            // TODO: 保存设置并触发变更通知
        }

        public void Apply(AppSettings settings)
        {
            // TODO: 应用主题、字体、窗口布局、启动行为等设置
        }

        public void SetAutoStart(bool enabled)
        {
            // TODO: 设置开机自启策略
        }

        public void SetWindowMode(string modeKey)
        {
            // TODO: 设置窗口模式（全屏/窗口）
        }
    }
}


