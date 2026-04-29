// Infrastructure/AppContext.cs
// 应用级上下文：集中持有服务实例与全局状态（WinForms 组合根）
using ClassHub.Services;

namespace ClassHub.Infrastructure
{
    public class ClassHubAppContext
    {
        // TODO: 在这里统一注册并暴露全局服务实例（可后续替换为 DI 容器）
        public IAccountService AccountService { get; }
        public IDataService DataService { get; }
        public IFileService FileService { get; }
        public ISettingsService SettingsService { get; }
        public ISystemOptimizerService SystemOptimizerService { get; }
        public ISecurityService SecurityService { get; }
        public IClassroomPolicyService ClassroomPolicyService { get; }
        public INavigationService NavigationService { get; }

        public ClassHubAppContext(
            IAccountService accountService,
            IDataService dataService,
            IFileService fileService,
            ISettingsService settingsService,
            ISystemOptimizerService systemOptimizerService,
            ISecurityService securityService,
            IClassroomPolicyService classroomPolicyService,
            INavigationService navigationService)
        {
            AccountService = accountService;
            DataService = dataService;
            FileService = fileService;
            SettingsService = settingsService;
            SystemOptimizerService = systemOptimizerService;
            SecurityService = securityService;
            ClassroomPolicyService = classroomPolicyService;
            NavigationService = navigationService;
        }
    }
}
