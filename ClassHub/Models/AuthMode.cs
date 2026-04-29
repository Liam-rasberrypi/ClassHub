// Models/AuthMode.cs
// 账户验证方式枚举：支持密码/U盘/组合验证
namespace ClassHub.Models
{
    public enum AuthMode
    {
        Password,
        Usb,
        PasswordAndUsb
    }
}
