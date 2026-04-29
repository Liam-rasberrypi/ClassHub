// Services/SecurityService.cs
// 安全服务：账户验证方式（密码/U盘）、模块密码、权限判定
namespace ClassHub.Services
{
    public interface ISecurityService
    {
        // TODO: 验证账户登录（密码验证、U盘验证、组合验证）
        bool VerifyLogin(string accountId, string? password, string? usbDeviceId);

        // TODO: 验证功能级密码（点名册、值日、作业、公告、画板等）
        bool VerifyFeaturePassword(string featureKey, string password);

        // TODO: 判断账户是否有指定功能操作权限（查看/编辑/删除/设置）
        bool HasPermission(string accountId, string permissionKey);
    }

    public class SecurityService : ISecurityService
    {
        public bool VerifyLogin(string accountId, string? password, string? usbDeviceId)
        {
            // TODO: 根据账户绑定策略执行密码/U盘校验
            return false;
        }

        public bool VerifyFeaturePassword(string featureKey, string password)
        {
            // TODO: 校验模块独立密码
            return false;
        }

        public bool HasPermission(string accountId, string permissionKey)
        {
            // TODO: 根据角色与自定义授权规则返回权限判定结果
            return false;
        }
    }
}
