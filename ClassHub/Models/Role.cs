// Role.cs
// 账户角色数据模型
namespace ClassHub.Models
{
    public enum Role
    {
        Admin,
        Teacher,
        Deputy,
        Student
        // TODO: 后续可支持“访客”“只读审计”等扩展角色
    }
}

