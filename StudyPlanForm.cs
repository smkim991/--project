using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Prototype1.UI
{
    public partial class StudyPlanForm : Form
    {
        private const string MemoFileName = "StudyPlanMemo.txt";
        private const string StudyWorkspaceFolderName = "StudyWorkspace";
        private const string DefaultStudyFolderName = "학습 자료";

        private string memoFilePath;
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
        private Button btnRefresh;
        private Button btnOpenReport;
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
            memoFilePath = Path.Combine(Application.UserAppDataPath, MemoFileName);
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

            btnRefresh = new Button();
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

            btnOpenReport = new Button();
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

        private void RefreshDashboard()
        {
            sessions = FocusSessionStore.LoadSessions()
                .OrderByDescending(s => s.StartedAt)
                .ToList();

            ApplyDayFilter();
        }

        private void ApplyDayFilter()
        {
            if (dayPicker == null)
            {
                return;
            }

            DateTime dayStart = dayPicker.Value.Date;
            DateTime dayEnd = dayStart.AddDays(1);

            daySessions = sessions
                .Where(s => s.StartedAt < dayEnd && s.EndedAt >= dayStart)
                .OrderBy(s => s.StartedAt)
                .ToList();

            UpdateDaySummary();
            UpdateTimeline();
            UpdateSessionGrid();
            UpdateSelectedSessionDetails();
        }

        private void UpdateDaySummary()
        {
            int active = daySessions.Sum(s => s.ActiveSeconds);
            int breakSeconds = daySessions.Sum(s => s.BreakSeconds);
            int switches = daySessions.Sum(s => s.AppSwitchCount);

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(dayPicker.Value.ToString("yyyy-MM-dd") + " 집중 요약");
            builder.AppendLine();
            builder.AppendLine("세션 수: " + daySessions.Count);
            builder.AppendLine("활성 집중 시간: " + FocusSessionReportBuilder.FormatDuration(active));
            builder.AppendLine("휴식 시간: " + FocusSessionReportBuilder.FormatDuration(breakSeconds));
            builder.AppendLine("앱 전환 횟수: " + switches);

            lblDaySummary.Text = builder.ToString();
        }

        private void UpdateTimeline()
        {
            timelinePanel.SetSessions(dayPicker.Value.Date, daySessions);
            ResizeTimelinePanel();
        }

        private void ResizeTimelinePanel()
        {
            if (timelinePanel == null || timelineHostPanel == null)
            {
                return;
            }

            int width = Math.Max(760, timelineHostPanel.ClientSize.Width - 24);
            int height = Math.Max(280, 96 + (Math.Max(1, daySessions.Count) * 44));
            timelinePanel.Size = new Size(width, height);
        }

        private void UpdateSessionGrid()
        {
            dgvSessions.DataSource = new BindingList<SessionRow>(
                daySessions
                    .OrderByDescending(s => s.StartedAt)
                    .Select(s => new SessionRow(s))
                    .ToList());

            if (dgvSessions.Rows.Count > 0)
            {
                dgvSessions.Rows[0].Selected = true;
                dgvSessions.CurrentCell = dgvSessions.Rows[0].Cells[0];
            }
        }

        private void UpdateSelectedSessionDetails()
        {
            FocusSessionRecord selected = GetSelectedSession();
            txtSelectedSessionSummary.Text = selected == null
                ? BuildDayDetailText()
                : FocusSessionReportBuilder.BuildSessionSummaryText(selected);

            UpdateAppUsageList(selected);

            if (timelinePanel != null)
            {
                timelinePanel.SelectedSessionId = selected == null ? string.Empty : selected.Id;
                timelinePanel.Invalidate();
            }
        }

        private string BuildDayDetailText()
        {
            if (daySessions.Count == 0)
            {
                return "선택한 날짜에 기록된 세션이 없습니다.";
            }

            return "세션을 선택하면 상세 요약이 표시됩니다.";
        }

        private void UpdateAppUsageList(FocusSessionRecord selectedSession)
        {
            lvAppUsage.Items.Clear();

            List<AppUsageSummary> appSummaries = selectedSession == null
                ? BuildDayAppUsage(daySessions)
                : FocusSessionReportBuilder.BuildAppUsage(selectedSession);

            foreach (AppUsageSummary app in appSummaries)
            {
                if (app.ActiveSeconds <= 0 && app.BreakSeconds <= 0)
                {
                    continue;
                }

                ListViewItem item = new ListViewItem(app.AppName);
                item.SubItems.Add(FocusSessionReportBuilder.FormatDuration(app.ActiveSeconds));
                item.SubItems.Add(FocusSessionReportBuilder.FormatDuration(app.BreakSeconds));
                item.SubItems.Add(app.SwitchEntries.ToString());
                lvAppUsage.Items.Add(item);
            }
        }

        private List<AppUsageSummary> BuildDayAppUsage(List<FocusSessionRecord> sourceSessions)
        {
            Dictionary<string, AppUsageSummary> byApp = new Dictionary<string, AppUsageSummary>(StringComparer.OrdinalIgnoreCase);

            foreach (FocusSessionRecord session in sourceSessions)
            {
                foreach (AppUsageSummary app in FocusSessionReportBuilder.BuildAppUsage(session))
                {
                    if (!byApp.TryGetValue(app.AppName, out AppUsageSummary summary))
                    {
                        summary = new AppUsageSummary { AppName = app.AppName, ExecutablePath = app.ExecutablePath };
                        byApp.Add(app.AppName, summary);
                    }

                    summary.ActiveSeconds += app.ActiveSeconds;
                    summary.BreakSeconds += app.BreakSeconds;
                    summary.SwitchEntries += app.SwitchEntries;
                }
            }

            return byApp.Values
                .OrderByDescending(a => a.ActiveSeconds)
                .ThenByDescending(a => a.TotalSeconds)
                .ToList();
        }

        private FocusSessionRecord GetSelectedSession()
        {
            if (dgvSessions == null || dgvSessions.CurrentRow == null)
            {
                return null;
            }

            SessionRow row = dgvSessions.CurrentRow.DataBoundItem as SessionRow;
            return row == null ? null : row.Session;
        }

        private void SelectSessionInGrid(FocusSessionRecord session)
        {
            if (session == null || dgvSessions == null)
            {
                return;
            }

            foreach (DataGridViewRow row in dgvSessions.Rows)
            {
                SessionRow sessionRow = row.DataBoundItem as SessionRow;
                if (sessionRow != null && sessionRow.Session.Id == session.Id)
                {
                    row.Selected = true;
                    dgvSessions.CurrentCell = row.Cells[0];
                    dashboardTabs.SelectedIndex = 2;
                    return;
                }
            }
        }

        private void btnOpenReport_Click(object sender, EventArgs e)
        {
            FocusSessionRecord selected = GetSelectedSession();
            if (selected == null)
            {
                MessageBox.Show("선택된 세션이 없습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (FocusSessionReportForm reportForm = new FocusSessionReportForm(selected))
            {
                reportForm.ShowDialog(this);
            }
        }

        private void timelinePanel_SessionSelected(object sender, SessionSelectedEventArgs e)
        {
            SelectSessionInGrid(e.Session);
        }

        private void EnsureDefaultStudyFile()
        {
            string defaultFilePath = Path.Combine(defaultStudyFolderPath, "학습 메모.txt");
            string legacyWorkspaceMemoPath = Path.Combine(studyWorkspacePath, "학습 메모.txt");

            if (File.Exists(defaultFilePath))
            {
                return;
            }

            if (File.Exists(legacyWorkspaceMemoPath))
            {
                File.Move(legacyWorkspaceMemoPath, defaultFilePath);
                return;
            }

            string initialText = string.Empty;
            if (File.Exists(memoFilePath))
            {
                try
                {
                    initialText = File.ReadAllText(memoFilePath);
                }
                catch
                {
                    initialText = string.Empty;
                }
            }

            File.WriteAllText(defaultFilePath, initialText);
        }

        private void RefreshStudyTree()
        {
            studyTreeView.BeginUpdate();
            studyTreeView.Nodes.Clear();

            foreach (string directory in Directory.GetDirectories(studyWorkspacePath).OrderBy(Path.GetFileName))
            {
                TreeNode directoryNode = CreateDirectoryNode(directory, Path.GetFileName(directory));
                studyTreeView.Nodes.Add(directoryNode);
                PopulateDirectoryNode(directoryNode);
                directoryNode.Expand();
            }

            foreach (string file in Directory.GetFiles(studyWorkspacePath).OrderBy(Path.GetFileName))
            {
                studyTreeView.Nodes.Add(CreateFileNode(file));
            }

            studyTreeView.EndUpdate();
        }

        private TreeNode CreateDirectoryNode(string path, string name)
        {
            TreeNode node = new TreeNode(name);
            node.Tag = path;
            node.NodeFont = new Font(studyTreeView.Font, FontStyle.Bold);
            return node;
        }

        private TreeNode CreateFileNode(string path)
        {
            TreeNode node = new TreeNode(Path.GetFileName(path));
            node.Tag = path;
            return node;
        }

        private void PopulateDirectoryNode(TreeNode directoryNode)
        {
            string path = directoryNode.Tag as string;
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                return;
            }

            foreach (string directory in Directory.GetDirectories(path).OrderBy(Path.GetFileName))
            {
                TreeNode child = CreateDirectoryNode(directory, Path.GetFileName(directory));
                directoryNode.Nodes.Add(child);
                PopulateDirectoryNode(child);
            }

            foreach (string file in Directory.GetFiles(path).OrderBy(Path.GetFileName))
            {
                directoryNode.Nodes.Add(CreateFileNode(file));
            }
        }

        private void SelectFirstFileNode()
        {
            TreeNode firstFile = null;
            foreach (TreeNode node in studyTreeView.Nodes)
            {
                firstFile = FindFirstFileNode(node);
                if (firstFile != null)
                {
                    break;
                }
            }

            if (firstFile != null)
            {
                studyTreeView.SelectedNode = firstFile;
            }
        }

        private TreeNode FindFirstFileNode(TreeNode node)
        {
            if (node == null)
            {
                return null;
            }

            string path = node.Tag as string;
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                return node;
            }

            foreach (TreeNode child in node.Nodes)
            {
                TreeNode found = FindFirstFileNode(child);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private void studyTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            LoadSelectedNodeFile();
        }

        private void studyTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            lastTreeMouseLocation = e.Location;
            hasLastTreeMouseLocation = true;

            TreeNode node = studyTreeView.GetNodeAt(e.Location) ?? FindVisibleNodeAtY(e.Y);
            studyTreeView.SelectedNode = node;
        }

        private void studyTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                studyTreeView.SelectedNode = e.Node;
            }
        }

        private TreeNode FindVisibleNodeAtY(int y)
        {
            foreach (TreeNode node in EnumerateVisibleNodes())
            {
                if (y >= node.Bounds.Top && y <= node.Bounds.Bottom)
                {
                    return node;
                }
            }

            return null;
        }

        private IEnumerable<TreeNode> EnumerateVisibleNodes()
        {
            foreach (TreeNode root in studyTreeView.Nodes)
            {
                foreach (TreeNode node in EnumerateVisibleNodes(root))
                {
                    yield return node;
                }
            }
        }

        private IEnumerable<TreeNode> EnumerateVisibleNodes(TreeNode node)
        {
            if (node == null || !node.IsVisible)
            {
                yield break;
            }

            yield return node;

            if (!node.IsExpanded)
            {
                yield break;
            }

            foreach (TreeNode child in node.Nodes)
            {
                foreach (TreeNode visibleChild in EnumerateVisibleNodes(child))
                {
                    yield return visibleChild;
                }
            }
        }

        private void treeContextMenu_Opening(object sender, CancelEventArgs e)
        {
            TreeNode selected = studyTreeView.SelectedNode;
            bool hasSelection = selected != null;
            bool isRoot = hasSelection && IsWorkspaceRoot(selected.Tag as string);

            treeContextMenu.Items[0].Enabled = true;
            treeContextMenu.Items[1].Enabled = true;
            treeContextMenu.Items[2].Enabled = hasSelection && !isRoot;
            treeContextMenu.Items[4].Enabled = hasSelection && !isRoot;
        }

        private void LoadSelectedNodeFile()
        {
            SaveCurrentFile();

            TreeNode selected = studyTreeView.SelectedNode;
            string path = selected == null ? null : selected.Tag as string;

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                currentFilePath = null;
                isLoadingFile = true;
                txtMemo.Text = "폴더를 선택했습니다. 왼쪽 사이드바에서 파일을 선택하거나 마우스 오른쪽 버튼으로 새 파일을 추가하세요.";
                isLoadingFile = false;
                txtMemo.Enabled = false;
                btnSaveFile.Enabled = false;
                btnStartFocusFromPlan.Enabled = false;
                lblFileTitle.Text = selected == null ? "파일을 선택하세요" : selected.Text;
                return;
            }

            try
            {
                currentFilePath = path;
                isLoadingFile = true;
                txtMemo.Text = File.ReadAllText(path);
                isLoadingFile = false;
                isFileDirty = false;
                txtMemo.Enabled = true;
                btnSaveFile.Enabled = false;
                btnStartFocusFromPlan.Enabled = true;
                lblFileTitle.Text = Path.GetFileName(path);
                dashboardTabs.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("파일을 불러오는 데 실패했습니다: " + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isLoadingFile = false;
            }
        }

        private void txtMemo_TextChanged(object sender, EventArgs e)
        {
            if (isLoadingFile || string.IsNullOrWhiteSpace(currentFilePath))
            {
                return;
            }

            isFileDirty = true;
            btnSaveFile.Enabled = true;
        }

        private void SaveCurrentFile()
        {
            if (!isFileDirty || string.IsNullOrWhiteSpace(currentFilePath) || !File.Exists(currentFilePath))
            {
                return;
            }

            try
            {
                File.WriteAllText(currentFilePath, txtMemo.Text);
                isFileDirty = false;
                btnSaveFile.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("파일을 저장하는 데 실패했습니다: " + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStartFocusFromPlan_Click(object sender, EventArgs e)
        {
            if (DataModel.IsBlockingActive)
            {
                MessageBox.Show("이미 집중 모드가 실행 중입니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (DataModel.IsEmergencyLockedOut)
            {
                MessageBox.Show(
                    "라이프를 모두 소진해서 지금은 집중 모드를 시작할 수 없습니다.\r\n다시 시작 가능 시간: " + DataModel.EmergencyLockUntil.ToString("yyyy-MM-dd HH:mm"),
                    "알림",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(currentFilePath) || !File.Exists(currentFilePath))
            {
                MessageBox.Show("먼저 파일을 선택하세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveCurrentFile();

            string planText = string.IsNullOrWhiteSpace(txtMemo.SelectedText) ? txtMemo.Text : txtMemo.SelectedText;
            PlanFocusDraft draft = BuildPlanFocusDraft(planText);
            if (!ShowPlanFocusDraftDialog(draft))
            {
                return;
            }

            DataModel.CurrentFocusGoal = draft.Goal;
            DataModel.CurrentFocusCategory = draft.Category;
            DataModel.CurrentPlanFilePath = currentFilePath;
            DataModel.CurrentPlanSnapshot = txtMemo.Text;
            DataModel.CurrentPlannedMinutes = draft.DurationMinutes;
            DataModel.SetActiveBlockListForCategory(draft.Category);
            DataModel.StartFocusSession(DateTime.Now.AddMinutes(draft.DurationMinutes));

            FocusSessionStarted = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private PlanFocusDraft BuildPlanFocusDraft(string planText)
        {
            string text = planText ?? string.Empty;
            return new PlanFocusDraft
            {
                Goal = ExtractGoal(text),
                DurationMinutes = ExtractDurationMinutes(text),
                Category = RecommendCategory(text)
            };
        }

        private string ExtractGoal(string planText)
        {
            if (string.IsNullOrWhiteSpace(planText))
            {
                return Path.GetFileNameWithoutExtension(currentFilePath) ?? "집중 세션";
            }

            string[] lines = planText.Replace("\r\n", "\n").Split('\n');
            foreach (string rawLine in lines)
            {
                string line = CleanupPlanLine(rawLine);
                if (string.IsNullOrWhiteSpace(line) || IsPlanMetadataLine(line))
                {
                    continue;
                }

                Match labeledGoal = Regex.Match(line, @"^(목표|goal|task)\s*[:：]\s*(.+)$", RegexOptions.IgnoreCase);
                if (labeledGoal.Success && !string.IsNullOrWhiteSpace(labeledGoal.Groups[2].Value))
                {
                    return TrimGoal(labeledGoal.Groups[2].Value);
                }

                if (!line.Equals("오늘 목표", StringComparison.OrdinalIgnoreCase) &&
                    !line.Equals("목표", StringComparison.OrdinalIgnoreCase))
                {
                    return TrimGoal(line);
                }
            }

            return Path.GetFileNameWithoutExtension(currentFilePath) ?? "집중 세션";
        }

        private string CleanupPlanLine(string line)
        {
            string value = (line ?? string.Empty).Trim();
            value = Regex.Replace(value, @"^#{1,6}\s*", string.Empty);
            value = Regex.Replace(value, @"^[-*+]\s*(\[[ xX]\]\s*)?", string.Empty);
            value = Regex.Replace(value, @"^\d+[\.)]\s*", string.Empty);
            return value.Trim();
        }

        private bool IsPlanMetadataLine(string line)
        {
            return Regex.IsMatch(line, @"^(mode|모드|duration|time|시간|예상\s*시간|소요\s*시간)\s*[:：]", RegexOptions.IgnoreCase) ||
                   Regex.IsMatch(line, @"^@\d+\s*(분|시간|m|h)$", RegexOptions.IgnoreCase);
        }

        private string TrimGoal(string value)
        {
            string goal = (value ?? string.Empty).Trim();
            return goal.Length <= 80 ? goal : goal.Substring(0, 80);
        }

        private int ExtractDurationMinutes(string planText)
        {
            string text = planText ?? string.Empty;
            int minutes = 0;

            Match hourMatch = Regex.Match(text, @"(\d{1,2})\s*(시간|시|hours?|hrs?|h)", RegexOptions.IgnoreCase);
            if (hourMatch.Success)
            {
                minutes += int.Parse(hourMatch.Groups[1].Value) * 60;
            }

            Match minuteMatch = Regex.Match(text, @"(\d{1,3})\s*(분|minutes?|mins?|m)", RegexOptions.IgnoreCase);
            if (minuteMatch.Success)
            {
                minutes += int.Parse(minuteMatch.Groups[1].Value);
            }

            if (minutes <= 0)
            {
                Match metadataMinuteMatch = Regex.Match(
                    text,
                    @"(duration|time|예상\s*시간|소요\s*시간|목표\s*시간)\s*[:=：]?\s*(\d{1,3})",
                    RegexOptions.IgnoreCase);

                if (metadataMinuteMatch.Success)
                {
                    minutes = int.Parse(metadataMinuteMatch.Groups[2].Value);
                }
            }

            if (minutes <= 0)
            {
                minutes = 50;
            }

            return Math.Max(5, Math.Min(240, minutes));
        }

        private string RecommendCategory(string planText)
        {
            string text = (planText ?? string.Empty).ToLowerInvariant();

            string explicitCategory = FindExplicitCategory(text);
            if (!string.IsNullOrWhiteSpace(explicitCategory))
            {
                return explicitCategory;
            }

            if (ContainsAny(text, "개발", "코딩", "알고리즘", "프로그래밍", "visual studio", "github", "git", "c#", "python", "java"))
            {
                return "개발자";
            }

            if (ContainsAny(text, "영상", "편집", "프리미어", "after effects", "애프터", "포토샵", "photoshop"))
            {
                return "영상편집자";
            }

            if (ContainsAny(text, "수학", "영어", "국어", "기출", "시험", "암기", "문제집", "수능", "공무원"))
            {
                return "수험생";
            }

            if (ContainsAny(text, "과제", "리포트", "레포트", "논문", "강의", "수업", "전공", "발표", "ppt"))
            {
                return "대학생";
            }

            return DataModel.BlockProfiles.ContainsKey("대학생") ? "대학생" : DataModel.BlockProfiles.Keys.FirstOrDefault() ?? "직접 시작";
        }

        private string FindExplicitCategory(string planText)
        {
            foreach (string category in DataModel.BlockProfiles.Keys)
            {
                string lowerCategory = category.ToLowerInvariant();
                if (planText.Contains("#" + lowerCategory) ||
                    Regex.IsMatch(planText, @"(mode|모드)\s*[:=：]\s*" + Regex.Escape(lowerCategory), RegexOptions.IgnoreCase))
                {
                    return category;
                }
            }

            return string.Empty;
        }

        private bool ContainsAny(string text, params string[] tokens)
        {
            foreach (string token in tokens)
            {
                if (text.Contains(token.ToLowerInvariant()))
                {
                    return true;
                }
            }

            return false;
        }

        private bool ShowPlanFocusDraftDialog(PlanFocusDraft draft)
        {
            Dictionary<string, List<string>> profiles = DataModel.GetBlockProfilesCopy();

            using (Form dialog = new Form())
            using (Label goalLabel = new Label())
            using (TextBox goalBox = new TextBox())
            using (Label durationLabel = new Label())
            using (NumericUpDown durationBox = new NumericUpDown())
            using (Label modeLabel = new Label())
            using (ComboBox modeBox = new ComboBox())
            using (Label blockLabel = new Label())
            using (TextBox blockBox = new TextBox())
            using (Button startButton = new Button())
            using (Button cancelButton = new Button())
            {
                dialog.Text = "계획 기반 집중 시작";
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MinimizeBox = false;
                dialog.MaximizeBox = false;
                dialog.ClientSize = new Size(460, 330);

                goalLabel.Text = "목표";
                goalLabel.Location = new Point(16, 16);
                goalLabel.Size = new Size(420, 22);

                goalBox.Text = draft.Goal;
                goalBox.Location = new Point(16, 42);
                goalBox.Size = new Size(420, 26);

                durationLabel.Text = "집중 시간";
                durationLabel.Location = new Point(16, 82);
                durationLabel.Size = new Size(120, 22);

                durationBox.Minimum = 5;
                durationBox.Maximum = 240;
                durationBox.Increment = 5;
                durationBox.Value = Math.Max(durationBox.Minimum, Math.Min(durationBox.Maximum, draft.DurationMinutes));
                durationBox.Location = new Point(16, 108);
                durationBox.Size = new Size(120, 26);

                modeLabel.Text = "추천 모드";
                modeLabel.Location = new Point(156, 82);
                modeLabel.Size = new Size(120, 22);

                modeBox.DropDownStyle = ComboBoxStyle.DropDownList;
                modeBox.Location = new Point(156, 108);
                modeBox.Size = new Size(150, 26);
                foreach (string category in profiles.Keys)
                {
                    modeBox.Items.Add(category);
                }

                if (!modeBox.Items.Contains(draft.Category))
                {
                    modeBox.Items.Add(draft.Category);
                }

                modeBox.SelectedItem = draft.Category;
                if (modeBox.SelectedIndex < 0 && modeBox.Items.Count > 0)
                {
                    modeBox.SelectedIndex = 0;
                }

                blockLabel.Text = "차단 앱";
                blockLabel.Location = new Point(16, 150);
                blockLabel.Size = new Size(420, 22);

                blockBox.Location = new Point(16, 176);
                blockBox.Size = new Size(420, 82);
                blockBox.Multiline = true;
                blockBox.ReadOnly = true;
                blockBox.ScrollBars = ScrollBars.Vertical;

                modeBox.SelectedIndexChanged += delegate
                {
                    string category = modeBox.SelectedItem == null ? string.Empty : modeBox.SelectedItem.ToString();
                    blockBox.Text = profiles.TryGetValue(category, out List<string> blocks)
                        ? string.Join(", ", blocks)
                        : string.Empty;
                };

                startButton.Text = "바로 시작";
                startButton.DialogResult = DialogResult.OK;
                startButton.Location = new Point(250, 278);
                startButton.Size = new Size(88, 34);

                cancelButton.Text = "취소";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Location = new Point(348, 278);
                cancelButton.Size = new Size(88, 34);

                dialog.Controls.Add(goalLabel);
                dialog.Controls.Add(goalBox);
                dialog.Controls.Add(durationLabel);
                dialog.Controls.Add(durationBox);
                dialog.Controls.Add(modeLabel);
                dialog.Controls.Add(modeBox);
                dialog.Controls.Add(blockLabel);
                dialog.Controls.Add(blockBox);
                dialog.Controls.Add(startButton);
                dialog.Controls.Add(cancelButton);
                dialog.AcceptButton = startButton;
                dialog.CancelButton = cancelButton;

                if (modeBox.SelectedItem != null)
                {
                    string category = modeBox.SelectedItem.ToString();
                    blockBox.Text = profiles.TryGetValue(category, out List<string> blocks)
                        ? string.Join(", ", blocks)
                        : string.Empty;
                }

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return false;
                }

                draft.Goal = string.IsNullOrWhiteSpace(goalBox.Text) ? "집중 세션" : goalBox.Text.Trim();
                draft.DurationMinutes = (int)durationBox.Value;
                draft.Category = modeBox.SelectedItem == null ? draft.Category : modeBox.SelectedItem.ToString();
                return true;
            }
        }

        private void AddFolderFromSelection()
        {
            string parentDirectory = GetTargetDirectoryForCreate();
            string name = PromptForName("폴더 추가", "폴더 이름", "새 폴더");
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            string folderPath = GetUniquePath(parentDirectory, SanitizeFileName(name), false);
            Directory.CreateDirectory(folderPath);
            RefreshStudyTree();
            SelectPath(folderPath);
        }

        private void AddFileFromSelection()
        {
            string parentDirectory = GetTargetDirectoryForCreate();
            string name = PromptForName("파일 추가", "파일 이름", "새 파일");
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            string fileName = SanitizeFileName(name);
            string filePath = GetUniquePath(parentDirectory, fileName, true);
            File.WriteAllText(filePath, string.Empty);
            RefreshStudyTree();
            SelectPath(filePath);
        }

        private void RenameSelectedNode()
        {
            TreeNode selected = studyTreeView.SelectedNode;
            string path = selected == null ? null : selected.Tag as string;
            if (string.IsNullOrWhiteSpace(path) || IsWorkspaceRoot(path))
            {
                return;
            }

            string currentName = File.Exists(path) ? Path.GetFileName(path) : new DirectoryInfo(path).Name;
            string newName = PromptForName("이름 변경", "새 이름", currentName);
            if (string.IsNullOrWhiteSpace(newName))
            {
                return;
            }

            string parent = Path.GetDirectoryName(path);
            string sanitized = SanitizeFileName(newName);

            string destination = Path.Combine(parent, sanitized);
            if (string.Equals(path, destination, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (File.Exists(destination) || Directory.Exists(destination))
            {
                MessageBox.Show("같은 이름의 파일 또는 폴더가 이미 있습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveCurrentFile();
            if (File.Exists(path))
            {
                File.Move(path, destination);
            }
            else if (Directory.Exists(path))
            {
                Directory.Move(path, destination);
            }

            RefreshStudyTree();
            SelectPath(destination);
        }

        private void DeleteSelectedNode()
        {
            TreeNode selected = studyTreeView.SelectedNode;
            string path = selected == null ? null : selected.Tag as string;
            if (string.IsNullOrWhiteSpace(path) || IsWorkspaceRoot(path))
            {
                return;
            }

            DialogResult result = MessageBox.Show(
                selected.Text + " 항목을 삭제할까요?",
                "삭제 확인",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning);

            if (result != DialogResult.OK)
            {
                return;
            }

            SaveCurrentFile();

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            if (IsCurrentFileAffectedByDelete(path))
            {
                currentFilePath = null;
                isFileDirty = false;
                txtMemo.Text = string.Empty;
                txtMemo.Enabled = false;
                btnSaveFile.Enabled = false;
                btnStartFocusFromPlan.Enabled = false;
            }

            RefreshStudyTree();
            SelectFirstFileNode();
        }

        private bool IsCurrentFileAffectedByDelete(string deletedPath)
        {
            if (string.IsNullOrWhiteSpace(currentFilePath) || string.IsNullOrWhiteSpace(deletedPath))
            {
                return false;
            }

            if (File.Exists(deletedPath))
            {
                return string.Equals(currentFilePath, deletedPath, StringComparison.OrdinalIgnoreCase);
            }

            string normalizedFile = Path.GetFullPath(currentFilePath);
            string normalizedFolder = Path.GetFullPath(deletedPath).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            return normalizedFile.StartsWith(normalizedFolder, StringComparison.OrdinalIgnoreCase);
        }

        private string GetTargetDirectoryForCreate()
        {
            if (!hasLastTreeMouseLocation)
            {
                return GetSelectedDirectory();
            }

            TreeNode clickedNode = studyTreeView.GetNodeAt(lastTreeMouseLocation) ?? FindVisibleNodeAtY(lastTreeMouseLocation.Y);
            return clickedNode == null ? studyWorkspacePath : GetDirectoryForNode(clickedNode);
        }

        private string GetDirectoryForNode(TreeNode node)
        {
            if (node == null)
            {
                return studyWorkspacePath;
            }

            string path = node.Tag as string;
            if (string.IsNullOrWhiteSpace(path))
            {
                return studyWorkspacePath;
            }

            if (Directory.Exists(path))
            {
                return path;
            }

            if (File.Exists(path))
            {
                return Path.GetDirectoryName(path);
            }

            return studyWorkspacePath;
        }

        private string GetSelectedDirectory()
        {
            TreeNode selected = studyTreeView.SelectedNode;
            return GetDirectoryForNode(selected);
        }

        private bool IsWorkspaceRoot(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            return string.Equals(
                Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar),
                Path.GetFullPath(studyWorkspacePath).TrimEnd(Path.DirectorySeparatorChar),
                StringComparison.OrdinalIgnoreCase);
        }

        private string SanitizeFileName(string name)
        {
            string value = string.IsNullOrWhiteSpace(name) ? "새 파일" : name.Trim();
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                value = value.Replace(invalidChar, '_');
            }

            return value;
        }

        private string GetUniquePath(string directory, string name, bool isFile)
        {
            string candidate = Path.Combine(directory, name);
            if (!File.Exists(candidate) && !Directory.Exists(candidate))
            {
                return candidate;
            }

            string fileName = isFile ? Path.GetFileNameWithoutExtension(name) : name;
            string extension = isFile ? Path.GetExtension(name) : string.Empty;

            for (int i = 2; i < 1000; i++)
            {
                candidate = Path.Combine(directory, fileName + " " + i + extension);
                if (!File.Exists(candidate) && !Directory.Exists(candidate))
                {
                    return candidate;
                }
            }

            return Path.Combine(directory, Guid.NewGuid().ToString("N") + extension);
        }

        private void SelectPath(string path)
        {
            foreach (TreeNode rootNode in studyTreeView.Nodes)
            {
                TreeNode node = FindNodeByPath(rootNode, path);
                if (node != null)
                {
                    studyTreeView.SelectedNode = node;
                    node.EnsureVisible();
                    return;
                }
            }
        }

        private TreeNode FindNodeByPath(TreeNode node, string path)
        {
            if (node == null)
            {
                return null;
            }

            string nodePath = node.Tag as string;
            if (!string.IsNullOrWhiteSpace(nodePath) && string.Equals(nodePath, path, StringComparison.OrdinalIgnoreCase))
            {
                return node;
            }

            foreach (TreeNode child in node.Nodes)
            {
                TreeNode found = FindNodeByPath(child, path);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private string PromptForName(string title, string label, string defaultValue)
        {
            using (Form dialog = new Form())
            using (Label prompt = new Label())
            using (TextBox input = new TextBox())
            using (Button okButton = new Button())
            using (Button cancelButton = new Button())
            {
                dialog.Text = title;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MinimizeBox = false;
                dialog.MaximizeBox = false;
                dialog.ClientSize = new Size(360, 130);

                prompt.Text = label;
                prompt.Location = new Point(14, 14);
                prompt.Size = new Size(330, 22);

                input.Text = defaultValue;
                input.Location = new Point(14, 42);
                input.Size = new Size(330, 26);
                input.SelectAll();

                okButton.Text = "확인";
                okButton.DialogResult = DialogResult.OK;
                okButton.Location = new Point(180, 84);
                okButton.Size = new Size(78, 30);

                cancelButton.Text = "취소";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Location = new Point(266, 84);
                cancelButton.Size = new Size(78, 30);

                dialog.Controls.Add(prompt);
                dialog.Controls.Add(input);
                dialog.Controls.Add(okButton);
                dialog.Controls.Add(cancelButton);
                dialog.AcceptButton = okButton;
                dialog.CancelButton = cancelButton;

                return dialog.ShowDialog(this) == DialogResult.OK ? input.Text.Trim() : string.Empty;
            }
        }

        private sealed class PlanFocusDraft
        {
            public string Goal { get; set; } = "집중 세션";
            public int DurationMinutes { get; set; } = 50;
            public string Category { get; set; } = "대학생";
        }

        private sealed class SessionRow
        {
            public SessionRow(FocusSessionRecord session)
            {
                Session = session;
            }

            public FocusSessionRecord Session { get; private set; }

            public string StartedAtText
            {
                get { return Session.StartedAt.ToString("HH:mm"); }
            }

            public string EndedAtText
            {
                get { return Session.EndedAt.ToString("HH:mm"); }
            }

            public string Goal
            {
                get { return string.IsNullOrWhiteSpace(Session.Goal) ? "-" : Session.Goal; }
            }

            public string TotalText
            {
                get { return FocusSessionReportBuilder.FormatDuration(Session.TotalSeconds); }
            }

            public string ActiveText
            {
                get { return FocusSessionReportBuilder.FormatDuration(Session.ActiveSeconds); }
            }

            public string BreakText
            {
                get { return FocusSessionReportBuilder.FormatDuration(Session.BreakSeconds); }
            }

            public int SwitchCount
            {
                get { return Session.AppSwitchCount; }
            }
        }

        private sealed class SessionTimelinePanel : Panel
        {
            private readonly List<Tuple<Rectangle, FocusSessionRecord>> hitAreas = new List<Tuple<Rectangle, FocusSessionRecord>>();
            private DateTime day = DateTime.Today;
            private List<FocusSessionRecord> sessions = new List<FocusSessionRecord>();

            public event EventHandler<SessionSelectedEventArgs> SessionSelected;
            public string SelectedSessionId { get; set; } = string.Empty;

            public SessionTimelinePanel()
            {
                DoubleBuffered = true;
                BackColor = Color.White;
            }

            public void SetSessions(DateTime selectedDay, List<FocusSessionRecord> selectedSessions)
            {
                day = selectedDay.Date;
                sessions = selectedSessions == null
                    ? new List<FocusSessionRecord>()
                    : selectedSessions.OrderBy(s => s.StartedAt).ToList();
                Invalidate();
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);

                foreach (Tuple<Rectangle, FocusSessionRecord> area in hitAreas)
                {
                    if (area.Item1.Contains(e.Location))
                    {
                        SelectedSessionId = area.Item2.Id;
                        Invalidate();

                        EventHandler<SessionSelectedEventArgs> handler = SessionSelected;
                        if (handler != null)
                        {
                            handler(this, new SessionSelectedEventArgs(area.Item2));
                        }

                        return;
                    }
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                Graphics g = e.Graphics;
                g.Clear(Color.White);
                hitAreas.Clear();

                Rectangle bounds = ClientRectangle;
                int left = 190;
                int right = 24;
                int top = 34;
                int axisY = Math.Max(top + 36, bounds.Height - 42);
                int plotWidth = Math.Max(120, bounds.Width - left - right);

                using (Pen gridPen = new Pen(Color.FromArgb(220, 224, 230)))
                using (Pen axisPen = new Pen(Color.FromArgb(130, 140, 150)))
                using (Brush textBrush = new SolidBrush(Color.FromArgb(45, 52, 60)))
                using (Font smallFont = new Font(Font.FontFamily, 8.5F))
                using (Font rowFont = new Font(Font.FontFamily, 9F))
                {
                    g.DrawString(day.ToString("yyyy-MM-dd") + " 세션 타임라인", Font, textBrush, 12, 10);

                    for (int hour = 0; hour <= 24; hour += 2)
                    {
                        int x = left + (int)Math.Round(plotWidth * (hour / 24.0));
                        g.DrawLine(gridPen, x, top, x, axisY);
                        g.DrawString(hour.ToString("D2"), smallFont, textBrush, x - 9, axisY + 6);
                    }

                    g.DrawLine(axisPen, left, axisY, left + plotWidth, axisY);

                    if (sessions.Count == 0)
                    {
                        g.DrawString("선택한 날짜에 기록된 세션이 없습니다.", Font, textBrush, left, top + 35);
                        return;
                    }

                    DrawLegend(g, bounds, smallFont, textBrush);

                    int rowY = top + 24;
                    for (int i = 0; i < sessions.Count; i++)
                    {
                        FocusSessionRecord session = sessions[i];
                        int y = rowY + (i * 44);
                        DrawSession(g, session, new Rectangle(left, y, plotWidth, 28), rowFont, textBrush);
                    }
                }
            }

            private void DrawLegend(Graphics g, Rectangle bounds, Font font, Brush textBrush)
            {
                int x = Math.Max(260, bounds.Width - 200);
                if (bounds.Width >= 0)
                {
                    DrawLegendItem(g, x, 10, Color.FromArgb(55, 158, 132), "활성", font, textBrush);
                    DrawLegendItem(g, x + 78, 10, Color.FromArgb(238, 188, 84), "휴식", font, textBrush);
                    return;
                }
                DrawLegendItem(g, x, 10, Color.FromArgb(55, 158, 132), "활성", font, textBrush);
                DrawLegendItem(g, x + 150, 10, Color.FromArgb(238, 188, 84), "휴식", font, textBrush);
            }

            private void DrawLegendItem(Graphics g, int x, int y, Color color, string text, Font font, Brush textBrush)
            {
                using (Brush brush = new SolidBrush(color))
                {
                    g.FillRectangle(brush, x, y + 4, 14, 10);
                }

                g.DrawString(text, font, textBrush, x + 18, y);
            }

            private void DrawSession(Graphics g, FocusSessionRecord session, Rectangle track, Font rowFont, Brush textBrush)
            {
                DateTime dayStart = day;
                DateTime dayEnd = day.AddDays(1);
                DateTime sessionStart = session.StartedAt < dayStart ? dayStart : session.StartedAt;
                DateTime sessionEnd = session.EndedAt > dayEnd ? dayEnd : session.EndedAt;

                int startX = GetX(sessionStart, track);
                int endX = GetX(sessionEnd, track);
                Rectangle sessionRect = new Rectangle(startX, track.Y, Math.Max(4, endX - startX), track.Height);
                bool selected = string.Equals(SelectedSessionId, session.Id, StringComparison.OrdinalIgnoreCase);

                using (Brush background = new SolidBrush(Color.FromArgb(235, 239, 244)))
                using (Pen border = new Pen(selected ? Color.FromArgb(28, 99, 175) : Color.FromArgb(155, 163, 175), selected ? 2 : 1))
                {
                    g.FillRectangle(background, sessionRect);
                    g.DrawRectangle(border, sessionRect);
                }

                if (session.Segments != null && session.Segments.Count > 0)
                {
                    foreach (AppUsageSegment segment in session.Segments)
                    {
                        if (segment.State == FocusUsageState.Idle)
                        {
                            continue;
                        }

                        DateTime segStart = segment.StartAt < dayStart ? dayStart : segment.StartAt;
                        DateTime segEnd = segment.EndAt > dayEnd ? dayEnd : segment.EndAt;
                        if (segEnd <= dayStart || segStart >= dayEnd || segEnd <= segStart)
                        {
                            continue;
                        }

                        Rectangle segmentRect = new Rectangle(
                            GetX(segStart, track),
                            track.Y + 3,
                            Math.Max(2, GetX(segEnd, track) - GetX(segStart, track)),
                            track.Height - 6);

                        using (Brush brush = new SolidBrush(GetSegmentColor(segment)))
                        {
                            g.FillRectangle(brush, segmentRect);
                        }
                    }
                }

                hitAreas.Add(Tuple.Create(sessionRect, session));

                string label = session.StartedAt.ToString("HH:mm") + "-" + session.EndedAt.ToString("HH:mm") +
                               " " + (string.IsNullOrWhiteSpace(session.Goal) ? "집중 세션" : session.Goal) +
                               string.Empty;

                label = session.StartedAt.ToString("HH:mm") + "-" + session.EndedAt.ToString("HH:mm") +
                        " " + (string.IsNullOrWhiteSpace(session.Goal) ? "집중 세션" : session.Goal);

                using (StringFormat format = new StringFormat())
                {
                    format.Trimming = StringTrimming.EllipsisCharacter;
                    format.FormatFlags = StringFormatFlags.NoWrap;
                    g.DrawString(label, rowFont, textBrush, new RectangleF(8, track.Y + 6, track.Left - 16, track.Height), format);
                }
            }

            private int GetX(DateTime time, Rectangle track)
            {
                double seconds = (time - day).TotalSeconds;
                seconds = Math.Max(0, Math.Min(86400, seconds));
                return track.Left + (int)Math.Round(track.Width * (seconds / 86400.0));
            }

            private Color GetSegmentColor(AppUsageSegment segment)
            {
                if (segment.IsBlocked)
                {
                    return Color.FromArgb(55, 158, 132);
                }

                if (segment.State == FocusUsageState.Break)
                {
                    return Color.FromArgb(238, 188, 84);
                }

                return Color.FromArgb(55, 158, 132);
            }
        }

        private sealed class SessionSelectedEventArgs : EventArgs
        {
            public SessionSelectedEventArgs(FocusSessionRecord session)
            {
                Session = session;
            }

            public FocusSessionRecord Session { get; private set; }
        }
    }
}
