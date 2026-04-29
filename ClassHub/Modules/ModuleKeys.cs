// Modules/ModuleKeys.cs
// 模块标识：按“主模块 -> 子功能”组织导航 Key，匹配产品大纲四大模块
namespace ClassHub.Modules
{
    public static class ModuleKeys
    {
        // 一级模块（主界面卡片）
        public const string ClassManagementCenter = "class-management-center";
        public const string TeachingAssistCenter = "teaching-assist-center";
        public const string SystemOptimizationCenter = "system-optimization-center";
        public const string SettingsCenter = "settings-center";

        // 二级功能：班级日常管理
        public const string Attendance = "attendance";
        public const string Duty = "duty";
        public const string Homework = "homework";
        public const string Notice = "notice";

        // 二级功能：教学辅助
        public const string Paintboard = "paintboard";
        public const string ClassFiles = "class-files";

        // 二级功能：系统优化
        public const string Optimizer = "optimizer";
        public const string Account = "account";

        // 二级功能：软件基础设置
        public const string AppSettings = "app-settings";
        public const string DataBackup = "data-backup";
    }
}
