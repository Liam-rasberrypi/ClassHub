// Forms/LoginForm.cs
// 登录窗体：预留账号密码、U盘校验、角色判断等流程
using System.Windows.Forms;

namespace ClassHub.Forms
{
    public class LoginForm : Form
    {
        public LoginForm()
        {
            // TODO: 构建登录界面（账号、密码、U盘绑定、记住登录）
            Text = "ClassHub 登录";
            Width = 420;
            Height = 320;
            StartPosition = FormStartPosition.CenterParent;
        }

        // TODO: 暴露登录结果对象（账户信息、角色、令牌、失败原因）
    }
}
