// Account.cs
// 账户数据模型
namespace ClassHub.Models
{
    public class Account
    {
        // 账户唯一标识；首次管理员设置使用随机 GUID。
        public string Id { get; set; } = string.Empty;

        // 登录名，1–64 个字符，首尾空白移除后按不区分大小写方式匹配。
        public string UserName { get; set; } = string.Empty;

        // 账户角色；目前仅启用的 Admin 可编辑公告。
        public Role Role { get; set; } = Role.Student;

        // 验证方式；当前仅支持密码，U 盘及组合验证拒绝登录。
        public AuthMode AuthMode { get; set; } = AuthMode.Password;

        // 版本化 PBKDF2-SHA256 哈希，包含算法、迭代次数、随机盐和摘要。
        // 不保存明文密码。
        public string PasswordHash { get; set; } = string.Empty;

        // 为后续受保护的 U 盘绑定流程保留，目前不参与身份验证。
        public string UsbDeviceId { get; set; } = string.Empty;

        // 禁用账户不能登录；每次权限检查重新读取以撤销已有会话。
        public bool Enabled { get; set; } = true;
    }
}

