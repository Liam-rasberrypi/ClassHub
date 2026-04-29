// Account.cs
// 账户数据模型
namespace ClassHub.Models
{
    public class Account
    {
        // TODO: 账户唯一标识
        public string Id { get; set; } = string.Empty;

        // TODO: 登录名（班委/老师/学生）
        public string UserName { get; set; } = string.Empty;

        // TODO: 账户角色
        public Role Role { get; set; } = Role.Student;

        // TODO: 验证方式（密码/U盘/组合）
        public AuthMode AuthMode { get; set; } = AuthMode.Password;

        // TODO: 密码哈希
        public string PasswordHash { get; set; } = string.Empty;

        // TODO: 绑定 U 盘设备标识
        public string UsbDeviceId { get; set; } = string.Empty;

        // TODO: 是否启用
        public bool Enabled { get; set; } = true;
    }
}

