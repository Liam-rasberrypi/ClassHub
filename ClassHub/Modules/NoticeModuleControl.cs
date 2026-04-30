// Modules/NoticeModuleControl.cs
// 班级公告模块（WinForms）
using System;
using System.Windows.Forms;
using ClassHub.Models;

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
                FlowDirection = FlowDirection.LeftToRight
            };

            _publishButton.Text = "发布";
            _publishButton.Width = 100;

            _togglePinButton.Text = "置顶/取消置顶";
            _togglePinButton.Width = 120;

            _deleteButton.Text = "删除";
            _deleteButton.Width = 100;

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

            AppContext.NoticeService.Add(title, content, _pinCheck.Checked);

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

            AppContext.NoticeService.TogglePin(selected.Id);
            RefreshNoticeList();
        }

        private void DeleteNotice()
        {
            if (AppContext == null || _noticeList.SelectedItem is not NoticeItem selected)
            {
                return;
            }

            AppContext.NoticeService.Delete(selected.Id);
            RefreshNoticeList();
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
