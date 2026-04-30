// Infrastructure/Bootstrapper.cs
// 应用启动装配器：负责初始化服务、导航注册、配置加载
using ClassHub.Modules;
using ClassHub.Services;

namespace ClassHub.Infrastructure
{
    public static class Bootstrapper
    {
        public static ClassHubAppContext Build()
        {
            // TODO: 初始化基础服务（日志、配置、存储、权限、异常处理）
            var accountService = new AccountService();
            var dataService = new DataService();
            var fileService = new FileService();
            var settingsService = new SettingsService();
            var systemOptimizerService = new SystemOptimizerService();
            var securityService = new SecurityService();
            var classroomPolicyService = new ClassroomPolicyService();
            var noticeService = new NoticeService();

            // TODO: 配置模块导航注册（一级模块 + 二级功能）
            var navigationService = new NavigationService();

            // 一级模块（主界面四大卡片）
            navigationService.Register(ModuleKeys.ClassManagementCenter, () => new ClassManagementCenterModuleControl());
            navigationService.Register(ModuleKeys.TeachingAssistCenter, () => new TeachingAssistCenterModuleControl());
            navigationService.Register(ModuleKeys.SystemOptimizationCenter, () => new SystemOptimizationCenterModuleControl());
            navigationService.Register(ModuleKeys.SettingsCenter, () => new SettingsCenterModuleControl());

            // 二级功能：班级日常管理
            navigationService.Register(ModuleKeys.Attendance, () => new AttendanceModuleControl());
            navigationService.Register(ModuleKeys.Duty, () => new DutyModuleControl());
            navigationService.Register(ModuleKeys.Homework, () => new HomeworkModuleControl());
            navigationService.Register(ModuleKeys.Notice, () => new NoticeModuleControl());

            // 二级功能：教学辅助
            navigationService.Register(ModuleKeys.Paintboard, () => new PaintboardModuleControl());
            navigationService.Register(ModuleKeys.ClassFiles, () => new ClassFilesModuleControl());

            // 二级功能：系统优化
            navigationService.Register(ModuleKeys.Optimizer, () => new OptimizerModuleControl());
            navigationService.Register(ModuleKeys.Account, () => new AccountModuleControl());

            // 二级功能：软件设置
            navigationService.Register(ModuleKeys.AppSettings, () => new AppSettingsModuleControl());
            navigationService.Register(ModuleKeys.DataBackup, () => new DataBackupModuleControl());

            return new ClassHubAppContext(
                accountService,
                dataService,
                fileService,
                settingsService,
                systemOptimizerService,
                securityService,
                classroomPolicyService,
                noticeService,
                navigationService);
        }
    }
}
