using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Prototype1.UI
{
    public partial class StudyPlanForm : Form
    {
        private const string StudyWorkspaceFolderName = "StudyWorkspace";
        private const string DefaultStudyFolderName = "학습 자료";
        private const string LegacyMemoFileName = "StudyPlanMemo.txt";

        private string studyWorkspacePath;
        private string defaultStudyFolderPath;
        private SplitContainer mainSplit;
        private TreeView studyTreeView;
        private ContextMenuStrip treeContextMenu;
        private TabControl dashboardTabs;
        private DateTimePicker dayPicker;
        private Label lblDaySummary;
        private SessionTimelinePanel timelinePanel;
        private Panel timelineHostPanel;
        private DataGridView dgvSessions;
        private TextBox txtSelectedSessionSummary;
        private ListView lvAppUsage;
        private Label lblFileTitle;
        private Button btnSaveFile;
        private Button btnStartFocusFromPlan;
        private string currentFilePath;
        private Point lastTreeMouseLocation;
        private bool hasLastTreeMouseLocation;
        private bool isLoadingFile;
        private bool isFileDirty;
        private List<FocusSessionRecord> sessions = new List<FocusSessionRecord>();
        private List<FocusSessionRecord> daySessions = new List<FocusSessionRecord>();

        public bool FocusSessionStarted { get; private set; }

        public StudyPlanForm()
        {
            InitializeComponent();
            studyWorkspacePath = Path.Combine(Application.UserAppDataPath, StudyWorkspaceFolderName);
            defaultStudyFolderPath = Path.Combine(studyWorkspacePath, DefaultStudyFolderName);
            Directory.CreateDirectory(Application.UserAppDataPath);
            Directory.CreateDirectory(studyWorkspacePath);
            Directory.CreateDirectory(defaultStudyFolderPath);
            EnsureDefaultStudyFile();
            BuildDashboardLayout();
        }

        private void BuildDashboardLayout()
        {
            SuspendLayout();

            Text = "학습 계획 및 대시보드";
            MinimumSize = new Size(1040, 680);
            ClientSize = new Size(1180, 720);
            Controls.Clear();

            mainSplit = new SplitContainer();
            mainSplit.Dock = DockStyle.Fill;
            mainSplit.Size = ClientSize;
            mainSplit.FixedPanel = FixedPanel.Panel1;
            mainSplit.Panel1MinSize = 220;
            mainSplit.Panel2MinSize = 360;
            mainSplit.Resize += delegate { SetSafeMainSplitterDistance(); };

            BuildSidebar(mainSplit.Panel1);
            BuildMainTabs(mainSplit.Panel2);

            Controls.Add(mainSplit);
            SetSafeMainSplitterDistance();
            ResumeLayout(false);
        }

        private void SetSafeMainSplitterDistance()
        {
            if (mainSplit == null || mainSplit.Width <= 0)
            {
                return;
            }

            int preferredSidebarWidth = 270;
            int maxDistance = mainSplit.Width - mainSplit.Panel2MinSize - mainSplit.SplitterWidth;
            if (maxDistance < mainSplit.Panel1MinSize)
            {
                return;
            }

            int safeDistance = Math.Max(mainSplit.Panel1MinSize, Math.Min(preferredSidebarWidth, maxDistance));
            if (mainSplit.SplitterDistance != safeDistance)
            {
                mainSplit.SplitterDistance = safeDistance;
            }
        }

        private void BuildSidebar(Control parent)
        {
            Panel root = new Panel();
            root.Dock = DockStyle.Fill;
            root.Padding = new Padding(10);

            Label title = new Label();
            title.Dock = DockStyle.Top;
            title.Height = 32;
            title.Text = "학습 파일";
            title.Font = new Font(Font.FontFamily, 11F, FontStyle.Bold);
            title.TextAlign = ContentAlignment.MiddleLeft;

            FlowLayoutPanel actions = new FlowLayoutPanel();
            actions.Dock = DockStyle.Top;
            actions.Height = 42;
            actions.FlowDirection = FlowDirection.LeftToRight;

            Button addFolderButton = new Button();
            addFolderButton.Size = new Size(88, 30);
            addFolderButton.Text = "폴더 추가";
            addFolderButton.Click += delegate { AddFolderFromSelection(); };

            Button addFileButton = new Button();
            addFileButton.Size = new Size(88, 30);
            addFileButton.Text = "파일 추가";
            addFileButton.Click += delegate { AddFileFromSelection(); };

            actions.Controls.Add(addFolderButton);
            actions.Controls.Add(addFileButton);

            studyTreeView = new TreeView();
            studyTreeView.Dock = DockStyle.Fill;
            studyTreeView.HideSelection = false;
            studyTreeView.AfterSelect += studyTreeView_AfterSelect;
            studyTreeView.MouseDown += studyTreeView_MouseDown;
            studyTreeView.NodeMouseClick += studyTreeView_NodeMouseClick;

            BuildTreeContextMenu();
            studyTreeView.ContextMenuStrip = treeContextMenu;

            root.Controls.Add(studyTreeView);
            root.Controls.Add(actions);
            root.Controls.Add(title);
            parent.Controls.Add(root);
        }

        private void BuildTreeContextMenu()
        {
            treeContextMenu = new ContextMenuStrip();
            treeContextMenu.Opening += treeContextMenu_Opening;
            treeContextMenu.Items.Add("폴더 추가", null, delegate { AddFolderFromSelection(); });
            treeContextMenu.Items.Add("파일 추가", null, delegate { AddFileFromSelection(); });
            treeContextMenu.Items.Add("이름 변경", null, delegate { RenameSelectedNode(); });
            treeContextMenu.Items.Add(new ToolStripSeparator());
            treeContextMenu.Items.Add("삭제", null, delegate { DeleteSelectedNode(); });
        }

        private void BuildMainTabs(Control parent)
        {
            dashboardTabs = new TabControl();
            dashboardTabs.Dock = DockStyle.Fill;

            TabPage fileTab = new TabPage("파일 편집");
            TabPage timelineTab = new TabPage("일별 시계열");
            TabPage historyTab = new TabPage("세션 내역");
            TabPage appUsageTab = new TabPage("앱 사용");

            BuildFileTab(fileTab);
            BuildTimelineTab(timelineTab);
            BuildHistoryTab(historyTab);
            BuildAppUsageTab(appUsageTab);

            dashboardTabs.TabPages.Add(fileTab);
            dashboardTabs.TabPages.Add(timelineTab);
            dashboardTabs.TabPages.Add(historyTab);
            dashboardTabs.TabPages.Add(appUsageTab);
            parent.Controls.Add(dashboardTabs);
        }

        private void BuildFileTab(TabPage fileTab)
        {
            Panel root = new Panel();
            root.Dock = DockStyle.Fill;
            root.Padding = new Padding(14);

            Panel header = new Panel();
            header.Dock = DockStyle.Top;
            header.Height = 44;

            lblFileTitle = new Label();
            lblFileTitle.Dock = DockStyle.Fill;
            lblFileTitle.Font = new Font(Font.FontFamily, 11F, FontStyle.Bold);
            lblFileTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblFileTitle.Text = "파일을 선택하세요";

            FlowLayoutPanel fileActions = new FlowLayoutPanel();
            fileActions.Dock = DockStyle.Right;
            fileActions.Width = 250;
            fileActions.FlowDirection = FlowDirection.RightToLeft;
            fileActions.WrapContents = false;

            btnSaveFile = new Button();
            btnSaveFile.Size = new Size(74, 30);
            btnSaveFile.Text = "저장";
            btnSaveFile.Enabled = false;
            btnSaveFile.Click += delegate { SaveCurrentFile(); };

            btnStartFocusFromPlan = new Button();
            btnStartFocusFromPlan.Size = new Size(158, 30);
            btnStartFocusFromPlan.Text = "집중 시작";
            btnStartFocusFromPlan.Enabled = false;
            btnStartFocusFromPlan.Click += btnStartFocusFromPlan_Click;

            txtMemo.Dock = DockStyle.Fill;
            txtMemo.Multiline = true;
            txtMemo.ScrollBars = ScrollBars.Both;
            txtMemo.AcceptsReturn = true;
            txtMemo.AcceptsTab = true;
            txtMemo.WordWrap = true;
            txtMemo.Enabled = false;
            txtMemo.TextChanged += txtMemo_TextChanged;

            fileActions.Controls.Add(btnSaveFile);
            fileActions.Controls.Add(btnStartFocusFromPlan);

            header.Controls.Add(lblFileTitle);
            header.Controls.Add(fileActions);
            root.Controls.Add(txtMemo);
            root.Controls.Add(header);
            fileTab.Controls.Add(root);
        }

        private void BuildTimelineTab(TabPage timelineTab)
        {
            Panel root = new Panel();
            root.Dock = DockStyle.Fill;
            root.Padding = new Padding(14);

            Panel header = new Panel();
            header.Dock = DockStyle.Top;
            header.Height = 44;

            Label dateLabel = new Label();
            dateLabel.Dock = DockStyle.Left;
            dateLabel.Width = 86;
            dateLabel.Text = "조회 날짜";
            dateLabel.TextAlign = ContentAlignment.MiddleLeft;

            dayPicker = new DateTimePicker();
            dayPicker.Dock = DockStyle.Left;
            dayPicker.Width = 150;
            dayPicker.Format = DateTimePickerFormat.Short;
            dayPicker.ValueChanged += delegate { ApplyDayFilter(); };

            Button btnRefresh = new Button();
            btnRefresh.Dock = DockStyle.Right;
            btnRefresh.Width = 110;
            btnRefresh.Text = "새로고침";
            btnRefresh.Click += delegate { RefreshDashboard(); };

            lblDaySummary = new Label();
            lblDaySummary.Dock = DockStyle.Top;
            lblDaySummary.Height = 150;
            lblDaySummary.BorderStyle = BorderStyle.FixedSingle;
            lblDaySummary.Padding = new Padding(12);
            lblDaySummary.Font = new Font(Font.FontFamily, 10F);

            timelineHostPanel = new Panel();
            timelineHostPanel.Dock = DockStyle.Fill;
            timelineHostPanel.AutoScroll = true;
            timelineHostPanel.BorderStyle = BorderStyle.FixedSingle;
            timelineHostPanel.Resize += delegate { ResizeTimelinePanel(); };

            timelinePanel = new SessionTimelinePanel();
            timelinePanel.Location = new Point(0, 0);
            timelinePanel.Width = 760;
            timelinePanel.Height = 280;
            timelinePanel.SessionSelected += timelinePanel_SessionSelected;
            timelineHostPanel.Controls.Add(timelinePanel);

            header.Controls.Add(btnRefresh);
            header.Controls.Add(dayPicker);
            header.Controls.Add(dateLabel);
            root.Controls.Add(timelineHostPanel);
            root.Controls.Add(lblDaySummary);
            root.Controls.Add(header);
            timelineTab.Controls.Add(root);
        }

        private void BuildHistoryTab(TabPage historyTab)
        {
            SplitContainer split = new SplitContainer();
            split.Dock = DockStyle.Fill;
            split.Orientation = Orientation.Horizontal;
            split.SplitterDistance = 320;
            split.Panel1.Padding = new Padding(12);
            split.Panel2.Padding = new Padding(12);

            dgvSessions = new DataGridView();
            dgvSessions.Dock = DockStyle.Fill;
            dgvSessions.AllowUserToAddRows = false;
            dgvSessions.AllowUserToDeleteRows = false;
            dgvSessions.AutoGenerateColumns = false;
            dgvSessions.ReadOnly = true;
            dgvSessions.RowHeadersVisible = false;
            dgvSessions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSessions.MultiSelect = false;
            dgvSessions.SelectionChanged += delegate { UpdateSelectedSessionDetails(); };
            dgvSessions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "시작", DataPropertyName = "StartedAtText", Width = 70 });
            dgvSessions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "종료", DataPropertyName = "EndedAtText", Width = 70 });
            dgvSessions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "목표", DataPropertyName = "Goal", Width = 190 });
            dgvSessions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "총 시간", DataPropertyName = "TotalText", Width = 88 });
            dgvSessions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "활성", DataPropertyName = "ActiveText", Width = 88 });
            dgvSessions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "휴식", DataPropertyName = "BreakText", Width = 88 });
            dgvSessions.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "전환", DataPropertyName = "SwitchCount", Width = 64 });

            txtSelectedSessionSummary = new TextBox();
            txtSelectedSessionSummary.Dock = DockStyle.Fill;
            txtSelectedSessionSummary.Multiline = true;
            txtSelectedSessionSummary.ReadOnly = true;
            txtSelectedSessionSummary.ScrollBars = ScrollBars.Vertical;

            Button btnOpenReport = new Button();
            btnOpenReport.Dock = DockStyle.Bottom;
            btnOpenReport.Height = 38;
            btnOpenReport.Text = "선택 세션 리포트 열기";
            btnOpenReport.Click += btnOpenReport_Click;

            split.Panel1.Controls.Add(dgvSessions);
            split.Panel2.Controls.Add(txtSelectedSessionSummary);
            split.Panel2.Controls.Add(btnOpenReport);
            historyTab.Controls.Add(split);
        }

        private void BuildAppUsageTab(TabPage appUsageTab)
        {
            lvAppUsage = new ListView();
            lvAppUsage.Dock = DockStyle.Fill;
            lvAppUsage.View = View.Details;
            lvAppUsage.FullRowSelect = true;
            lvAppUsage.GridLines = true;
            lvAppUsage.Columns.Add("앱", 180);
            lvAppUsage.Columns.Add("활성", 95);
            lvAppUsage.Columns.Add("휴식", 95);
            lvAppUsage.Columns.Add("전환", 75);

            appUsageTab.Padding = new Padding(12);
            appUsageTab.Controls.Add(lvAppUsage);
        }

        private void StudyPlanForm_Load(object sender, EventArgs e)
        {
            RefreshStudyTree();
            SelectFirstFileNode();
            RefreshDashboard();
        }

        private void StudyPlanForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveCurrentFile();
        }

    }
}
