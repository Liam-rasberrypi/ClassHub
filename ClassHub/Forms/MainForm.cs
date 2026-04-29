// Forms/MainForm.cs
// WinForms 主窗体：主界面为“四大模块卡片”，支持进入各模块二级页面
using System.Windows.Forms;
using ClassHub.Infrastructure;
using ClassHub.Modules;

namespace ClassHub.Forms
{
    public class MainForm : Form
    {
        private ClassHubAppContext _appContext = null!;
        private const bool UseDesigner = true;

        // TODO: 后续替换为“卡片式主界面 + 二级功能区”的标准布局
        private readonly Panel _moduleHost = new();
        private TableLayoutPanel tableLayoutPanel1;
        private SplitContainer splitContainer1;
        private Label label1;
        private TableLayoutPanel tableLayoutPanel2;
        private Panel panel1;
        private Button button3;
        private Button button1;
        private Button button2;
        private Button button4;
        private readonly FlowLayoutPanel _navPanel = new();

        public MainForm()
        {
            InitializeComponent();
        }
        public void SetContext(ClassHubAppContext appContext)
        {
            _appContext = appContext;
        }


        private void InitializeComponent()
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            splitContainer1 = new SplitContainer();
            tableLayoutPanel2 = new TableLayoutPanel();
            button3 = new Button();
            panel1 = new Panel();
            label1 = new Label();
            button4 = new Button();
            button2 = new Button();
            button1 = new Button();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = SystemColors.ActiveCaption;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(splitContainer1, 0, 0);
            tableLayoutPanel1.Controls.Add(label1, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(782, 453);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // splitContainer1
            // 
            splitContainer1.BackColor = SystemColors.GradientActiveCaption;
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(3, 3);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tableLayoutPanel2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(panel1);
            splitContainer1.Size = new Size(776, 427);
            splitContainer1.SplitterDistance = 258;
            splitContainer1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel2.Controls.Add(button1, 0, 0);
            tableLayoutPanel2.Controls.Add(button2, 0, 1);
            tableLayoutPanel2.Controls.Add(button4, 0, 3);
            tableLayoutPanel2.Controls.Add(button3, 0, 2);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(0, 0);
            tableLayoutPanel2.Margin = new Padding(0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 4;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel2.Size = new Size(258, 427);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // button3
            // 
            button3.BackColor = SystemColors.ButtonHighlight;
            button3.Dock = DockStyle.Fill;
            button3.FlatAppearance.BorderSize = 0;
            button3.FlatAppearance.MouseDownBackColor = SystemColors.GradientActiveCaption;
            button3.FlatAppearance.MouseOverBackColor = SystemColors.GradientInactiveCaption;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Location = new Point(3, 215);
            button3.Name = "button3";
            button3.Size = new Size(252, 100);
            button3.TabIndex = 2;
            button3.Text = "button3";
            button3.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.GradientInactiveCaption;
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(514, 427);
            panel1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Left;
            label1.Location = new Point(3, 433);
            label1.Name = "label1";
            label1.Size = new Size(151, 20);
            label1.TabIndex = 1;
            label1.Text = "ClassHub-Welcome";
            // 
            // button4
            // 
            button4.BackColor = SystemColors.ButtonHighlight;
            button4.Dock = DockStyle.Fill;
            button4.FlatAppearance.BorderSize = 0;
            button4.FlatAppearance.MouseDownBackColor = SystemColors.GradientActiveCaption;
            button4.FlatAppearance.MouseOverBackColor = SystemColors.GradientInactiveCaption;
            button4.FlatStyle = FlatStyle.Flat;
            button4.Location = new Point(3, 321);
            button4.Name = "button4";
            button4.Size = new Size(252, 103);
            button4.TabIndex = 3;
            button4.Text = "button4";
            button4.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            button2.BackColor = SystemColors.ButtonHighlight;
            button2.Dock = DockStyle.Fill;
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.MouseDownBackColor = SystemColors.GradientActiveCaption;
            button2.FlatAppearance.MouseOverBackColor = SystemColors.GradientInactiveCaption;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Location = new Point(3, 109);
            button2.Name = "button2";
            button2.Size = new Size(252, 100);
            button2.TabIndex = 4;
            button2.Text = "button2";
            button2.UseVisualStyleBackColor = false;
            // 
            // button1
            // 
            button1.BackColor = SystemColors.ButtonHighlight;
            button1.Dock = DockStyle.Fill;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatAppearance.MouseDownBackColor = SystemColors.GradientActiveCaption;
            button1.FlatAppearance.MouseOverBackColor = SystemColors.GradientInactiveCaption;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(3, 3);
            button1.Name = "button1";
            button1.Size = new Size(252, 100);
            button1.TabIndex = 5;
            button1.Text = "忘了";
            button1.UseVisualStyleBackColor = false;
            // 
            // MainForm
            // 
            ClientSize = new Size(782, 453);
            Controls.Add(tableLayoutPanel1);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ClassHub";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);

        }

        private void OpenModule(string moduleKey)
        {
            // TODO: 切换模块前执行权限校验、未保存提示、低配模式资源释放
            var control = _appContext.NavigationService.Create(moduleKey);
            if (control is ModuleControlBase moduleControl)
            {
                moduleControl.Initialize(_appContext);
            }

            control.Dock = DockStyle.Fill;
            _moduleHost.Controls.Clear();
            _moduleHost.Controls.Add(control);
        }
    }
}
