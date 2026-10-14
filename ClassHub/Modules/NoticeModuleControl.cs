// Modules/NoticeModuleControl.cs
// 班级公告模块（WinForms）
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ClassHub.Forms;
using ClassHub.Models;
using ClassHub.Services;

namespace ClassHub.Modules
{
    public class NoticeModuleControl : ModuleControlBase
    {
        private readonly ListBox _noticeList = new();
        private readonly TextBox _titleInput = new();
        private readonly TextBox _contentInput = new();
        private readonly CheckBox _pinCheck = new();
        private readonly Button _publishButton = new();
        private readonly Button _togglePinButton = new();
        private readonly Button _deleteButton = new();

        public NoticeModuleControl()
        {
            BuildLayout();
            BindEvents();
        }

        protected override void OnInitialize()
        {
            RefreshNoticeList();
        }

        private void BuildLayout()
        {
            Dock = DockStyle.Fill;

            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                // 设置初始宽度后再指定分隔位置，避免默认宽度过小时启动失败。
                Size = new Size(900, 500),
                SplitterDistance = 420
            };

            _noticeList.Dock = DockStyle.Fill;

            _titleInput.Dock = DockStyle.Top;
            _titleInput.PlaceholderText = "公告标题";

            _contentInput.Dock = DockStyle.Fill;
            _contentInput.Multiline = true;

            _pinCheck.Dock = DockStyle.Top;
            _pinCheck.Text = "发布时置顶";

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 48,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new Size(0, 48),
                FlowDirection = FlowDirection.LeftToRight
            };

            _publishButton.Text = "发布";
            _publishButton.Width = 100;
            _publishButton.AutoSize = true;

            _togglePinButton.Text = "置顶/取消置顶";
            _togglePinButton.Width = 120;
            _togglePinButton.AutoSize = true;

            _deleteButton.Text = "删除";
            _deleteButton.Width = 100;
            _deleteButton.AutoSize = true;

            buttonPanel.Controls.Add(_publishButton);
            buttonPanel.Controls.Add(_togglePinButton);
            buttonPanel.Controls.Add(_deleteButton);

            var editorPanel = new Panel { Dock = DockStyle.Fill };
            editorPanel.Controls.Add(_contentInput);
            editorPanel.Controls.Add(buttonPanel);
            editorPanel.Controls.Add(_pinCheck);
            editorPanel.Controls.Add(_titleInput);

            split.Panel1.Controls.Add(_noticeList);
            split.Panel2.Controls.Add(editorPanel);

            Controls.Add(split);
            Controls.Add(new Label
            {
                Dock = DockStyle.Top,
                Height = 38,
                Padding = new Padding(8),
                Text = "公告可直接查看；发布、置顶和删除每次均需管理员验证。",
                ForeColor = Color.DimGray
            });
        }

        private void BindEvents()
        {
            _publishButton.Click += (_, _) => PublishNotice();
            _togglePinButton.Click += (_, _) => TogglePin();
            _deleteButton.Click += (_, _) => DeleteNotice();

            _noticeList.SelectedIndexChanged += (_, _) => FillEditorFromSelection();
        }

        private void PublishNotice()
        {
            if (AppContext == null)
            {
                return;
            }

            var title = _titleInput.Text.Trim();
            var content = _contentInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
            {
                MessageBox.Show("请填写标题和内容后再发布。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!RunAdministratorAction(() => AppContext.NoticeService.Add(title, content, _pinCheck.Checked)))
            {
                return;
            }

            _titleInput.Clear();
            _contentInput.Clear();
            _pinCheck.Checked = false;

            RefreshNoticeList();
        }

        private void TogglePin()
        {
            if (AppContext == null || _noticeList.SelectedItem is not NoticeItem selected)
            {
                return;
            }

            if (!RunAdministratorAction(() => AppContext.NoticeService.TogglePin(selected.Id)))
            {
                return;
            }
            RefreshNoticeList();
        }

        private void DeleteNotice()
        {
            if (AppContext == null || _noticeList.SelectedItem is not NoticeItem selected)
            {
                return;
            }

            if (MessageBox.Show(this, "确定删除选中的公告吗？", "删除公告", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            if (!RunAdministratorAction(() => AppContext.NoticeService.Delete(selected.Id)))
            {
                return;
            }
            RefreshNoticeList();
        }

        private bool RunAdministratorAction(Action action)
        {
            if (AppContext == null)
            {
                return false;
            }

            var security = AppContext.SecurityService;
            security.Logout();
            try
            {
                using var login = new LoginForm(security);
                if (login.ShowDialog(this) != DialogResult.OK)
                {
                    return false;
                }

                // 公告服务自身也验证权限，防止其他调用方绕过此窗口。
                security.DemandPermission(PermissionKey.NoticeEdit);
                action();
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(this, "管理员验证已失效，或当前账号没有公告编辑权限。请重新验证后再试。", "操作未完成", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (AccountStorageException)
            {
                MessageBox.Show(this, "管理员账户无法读取或保存。请检查本地数据目录的权限或恢复账户文件备份。查看公告仍可使用。", "管理员验证不可用", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (IOException)
            {
                MessageBox.Show(this, "公告无法保存，请检查本地数据目录的权限和磁盘空间后重试。输入内容已保留。", "操作未完成", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                // 共享教室电脑不保留解锁状态；下一次修改必须重新验证。
                security.Logout();
            }
            return false;
        }

        private void RefreshNoticeList()
        {
            if (AppContext == null)
            {
                return;
            }

            _noticeList.Items.Clear();
            foreach (var notice in AppContext.NoticeService.GetAll())
            {
                _noticeList.Items.Add(notice);
            }
        }

        private void FillEditorFromSelection()
        {
            if (_noticeList.SelectedItem is not NoticeItem selected)
            {
                return;
            }

            _titleInput.Text = selected.Title;
            _contentInput.Text = selected.Content;
            _pinCheck.Checked = selected.IsPinned;
        }
    }
}
