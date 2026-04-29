// Models/PermissionKey.cs
// 权限标识：用于模块级与操作级权限控制
namespace ClassHub.Models
{
    public static class PermissionKey
    {
        // 班级管理
        public const string AttendanceView = "attendance.view";
        public const string AttendanceEdit = "attendance.edit";
        public const string DutyView = "duty.view";
        public const string DutyEdit = "duty.edit";
        public const string HomeworkView = "homework.view";
        public const string HomeworkEdit = "homework.edit";
        public const string NoticeView = "notice.view";
        public const string NoticeEdit = "notice.edit";

        // 教学辅助
        public const string PaintboardUse = "paintboard.use";
        public const string ClassFilesView = "classfiles.view";
        public const string ClassFilesEdit = "classfiles.edit";

        // 系统与设置
        public const string OptimizerRun = "optimizer.run";
        public const string AccountManage = "account.manage";
        public const string SettingsEdit = "settings.edit";
        public const string BackupManage = "backup.manage";
    }
}
