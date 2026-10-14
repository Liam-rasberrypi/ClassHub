// Forms/LoginForm.cs
// 离线管理员设置及单次公告操作的身份验证。
using System;
using System.Drawing;
using System.Windows.Forms;
using ClassHub.Services;

namespace ClassHub.Forms
{
    public class LoginForm : Form
    {
        private readonly ISecurityService _securityService;
        private readonly bool _isInitialSetup;
        private readonly TextBox _userNameInput = new() { Name = "UserNameInput", MaxLength = 64 };
        private readonly TextBox _passwordInput = new() { Name = "PasswordInput", UseSystemPasswordChar = true, MaxLength = 128 };
        private readonly TextBox _confirmPasswordInput = new() { Name = "ConfirmPasswordInput", UseSystemPasswordChar = true, MaxLength = 128 };
        private readonly Label _errorLabel = new() { Name = "AuthenticationError", AutoSize = false, ForeColor = Color.Firebrick };
        private readonly Button _submitButton = new() { Name = "SubmitAuthentication", AutoSize = true };

        public LoginForm(ISecurityService securityService)
        {
            _securityService = securityService ?? throw new ArgumentNullException(nameof(securityService));
            _securityService.Logout();
            // 账户存储不可读时抛给调用方，保留公告浏览入口。
            _isInitialSetup = _securityService.RequiresAdministratorSetup;

            Text = _isInitialSetup ? "首次设置管理员" : "管理员验证";
            ClientSize = new Size(440, _isInitialSetup ? 370 : 325);
            Font = new Font("Microsoft YaHei UI", 9F);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;

            BuildLayout();
            _submitButton.Click += (_, _) => Authenticate();
            Shown += (_, _) => _userNameInput.Focus();
            FormClosing += (_, _) =>
            {
                ClearPasswords();
                if (DialogResult != DialogResult.OK)
                {
                    _securityService.Logout();
                }
            };
        }

        private void BuildLayout()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(18),
                ColumnCount = 2,
                RowCount = _isInitialSetup ? 6 : 5
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var explanation = new Label
            {
                Dock = DockStyle.Fill,
                Text = _isInitialSetup
                    ? "请由教师设置本机管理员账号。\n密码须为 12–128 个字符，请妥善保存。"
                    : "发布、置顶和删除公告需要管理员验证。\n验证仅用于本次操作，查看公告无需登录。"
            };
            layout.Controls.Add(explanation, 0, 0);
            layout.SetColumnSpan(explanation, 2);
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));

            AddInputRow(layout, "管理员账号", _userNameInput, 1);
            AddInputRow(layout, "密码", _passwordInput, 2);
            var nextRow = 3;
            if (_isInitialSetup)
            {
                AddInputRow(layout, "确认密码", _confirmPasswordInput, nextRow++);
            }

            _errorLabel.Dock = DockStyle.Fill;
            layout.Controls.Add(_errorLabel, 0, nextRow++);
            layout.SetColumnSpan(_errorLabel, 2);
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false
            };
            var cancelButton = new Button { Text = "取消", AutoSize = true, DialogResult = DialogResult.Cancel };
            _submitButton.Text = _isInitialSetup ? "设置并验证" : "验证";
            buttons.Controls.Add(cancelButton);
            buttons.Controls.Add(_submitButton);
            layout.Controls.Add(buttons, 0, nextRow);
            layout.SetColumnSpan(buttons, 2);
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));

            AcceptButton = _submitButton;
            CancelButton = cancelButton;
            Controls.Add(layout);
        }

        private static void AddInputRow(TableLayoutPanel layout, string title, TextBox input, int row)
        {
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            layout.Controls.Add(new Label { Text = title, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, row);
            input.Dock = DockStyle.Fill;
            input.Margin = new Padding(0, 8, 0, 0);
            layout.Controls.Add(input, 1, row);
        }

        private void Authenticate()
        {
            _errorLabel.Text = "";
            _submitButton.Enabled = false;
            try
            {
                var userName = _userNameInput.Text.Trim();
                if (userName.Length == 0 || userName.Length > 64)
                {
                    _errorLabel.Text = "请输入 1–64 个字符的管理员账号。";
                    return;
                }

                if (_isInitialSetup)
                {
                    PasswordHasher.ValidateNewPassword(_passwordInput.Text);
                    if (!string.Equals(_passwordInput.Text, _confirmPasswordInput.Text, StringComparison.Ordinal))
                    {
                        _errorLabel.Text = "两次输入的密码不一致，请重新输入。";
                        ClearPasswords();
                        _passwordInput.Focus();
                        return;
                    }
                }

                var authenticated = _isInitialSetup
                    ? _securityService.InitializeAdministrator(userName, _passwordInput.Text)
                    : _securityService.Login(userName, _passwordInput.Text);
                ClearPasswords();
                if (!authenticated)
                {
                    _securityService.Logout();
                    _errorLabel.Text = "账号或密码不正确，或尝试过于频繁。请稍后重试。";
                    _passwordInput.Focus();
                    return;
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (ArgumentException)
            {
                _securityService.Logout();
                _errorLabel.Text = "账号应为 1–64 个字符且不含控制字符；密码应为 12–128 个字符。";
                ClearPasswords();
                _passwordInput.Focus();
            }
            catch (InvalidOperationException) when (_isInitialSetup)
            {
                _securityService.Logout();
                _errorLabel.Text = "管理员已由其他窗口设置。请取消并重新打开验证窗口。";
                ClearPasswords();
            }
            catch (AccountStorageException)
            {
                _securityService.Logout();
                _errorLabel.Text = "管理员账户无法读取或保存。请检查本地数据目录的权限或恢复账户文件备份。查看公告仍可使用。";
                ClearPasswords();
            }
            finally
            {
                _submitButton.Enabled = true;
            }
        }

        private void ClearPasswords()
        {
            if (!_passwordInput.IsDisposed)
            {
                _passwordInput.Clear();
            }
            if (!_confirmPasswordInput.IsDisposed)
            {
                _confirmPasswordInput.Clear();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ClearPasswords();
            }
            base.Dispose(disposing);
        }
    }
}
