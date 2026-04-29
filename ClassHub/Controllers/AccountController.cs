// Controllers/AccountController.cs
// 账户模块控制器
using ClassHub.Services;

namespace ClassHub.Controllers
{
    public class AccountController : IModuleController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public void Load() { /* TODO: 加载账户与角色 */ }
        public void Activate() { /* TODO: 刷新权限显示 */ }
        public void Deactivate() { /* TODO: 提交账号变更 */ }
    }
}
