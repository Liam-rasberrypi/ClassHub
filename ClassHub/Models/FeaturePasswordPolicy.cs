// Models/FeaturePasswordPolicy.cs
// 功能级密码策略（点名册/值日/作业/公告/画板等）
namespace ClassHub.Models
{
    public class FeaturePasswordPolicy
    {
        // TODO: 功能标识（与模块 key 对应）
        public string FeatureKey { get; set; } = string.Empty;

        // TODO: 密文摘要（不可逆哈希）
        public string PasswordHash { get; set; } = string.Empty;

        // TODO: 是否启用功能密码
        public bool Enabled { get; set; }
    }
}
