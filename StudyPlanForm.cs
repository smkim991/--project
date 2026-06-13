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
        private const string TutorialStudyFileName = "학습 계획 사용법.txt";
        private const string LegacyMemoFileName = "StudyPlanMemo.txt";

        private enum StudyTreeSortMode
        {
            CreatedDescending,
            NameAscending,
            NameDescending
        }

        private static readonly Color AppBackColor = Color.FromArgb(18, 18, 18);
        private static readonly Color SurfaceColor = Color.FromArgb(24, 24, 24);
        private static readonly Color FieldColor = Color.FromArgb(35, 35, 35);
        private static readonly Color BorderColor = Color.FromArgb(60, 60, 60);
        private static readonly Color AccentColor = Color.FromArgb(139, 92, 246);
        private static readonly Color AccentHoverColor = Color.FromArgb(124, 58, 237);
        private static readonly Color TextColor = Color.White;
        private static readonly Color MutedTextColor = Color.Silver;

        private string studyWorkspacePath;
        private string defaultStudyFolderPath;
        private SplitContainer mainSplit;
        private TreeView studyTreeView;
        private Button btnCreateStudyFile;
        private Button btnCreateStudyFolder;
        private Button btnStudyTreeFilter;
        private ContextMenuStrip studyTreeFilterMenu;
        private ToolTip studyTreeToolbarToolTip;
        private StudyTreeSortMode studyTreeSortMode = StudyTreeSortMode.CreatedDescending;
        private ContextMenuStrip treeContextMenu;
        private TabControl dashboardTabs;
        private FlowLayoutPanel dashboardTabBar;
        private List<Button> dashboardTabButtons = new List<Button>();
        private DateTimePicker dayPicker;
        private Label lblDaySummary;
        private SessionTimelinePanel timelinePanel;
        private Panel timelineHostPanel;
        private DataGridView dgvSessions;
        private TextBox txtSelectedSessionSummary;
        private ListView lvAppUsage;
        private Label lblFileTitle;
        private Label lblFileDate;
        private Button btnSaveFile;
        private Button btnStartFocusFromPlan;
        private Button btnToggleMarkdownPreview;
        private Panel markdownEditorHost;
        private WebBrowser markdownPreviewBrowser;
        private string currentFilePath;
        private Point lastTreeMouseLocation;
        private bool hasLastTreeMouseLocation;
        private bool isLoadingFile;
        private bool isFileDirty;
        private bool isMarkdownPreviewMode;
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
            BackColor = AppBackColor;
            ForeColor = TextColor;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            KeyPreview = true;
            Controls.Clear();

            mainSplit = new SplitContainer();
            mainSplit.Dock = DockStyle.Fill;
            mainSplit.Size = ClientSize;
            mainSplit.BackColor = AppBackColor;
            mainSplit.FixedPanel = FixedPanel.Panel1;
            mainSplit.Panel1MinSize = 220;
            mainSplit.Panel2MinSize = 360;
            mainSplit.Panel1.BackColor = SurfaceColor;
            mainSplit.Panel2.BackColor = AppBackColor;
            mainSplit.SplitterWidth = 1;
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

        // 왼쪽 학습 파일 영역을 만든다.
        private void BuildSidebar(Control parent)
        {
            Panel root = new Panel();
            root.Dock = DockStyle.Fill;
            root.BackColor = SurfaceColor;
            root.Padding = new Padding(24, 28, 20, 24);

            Label title = new Label();
            title.Dock = DockStyle.Top;
            title.Height = 46;
            title.Text = "학습 파일";
            title.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            title.ForeColor = TextColor;
            title.TextAlign = ContentAlignment.MiddleLeft;

            FlowLayoutPanel actions = new FlowLayoutPanel();
            actions.Dock = DockStyle.Top;
            actions.Height = 48;
            actions.FlowDirection = FlowDirection.LeftToRight;
            actions.WrapContents = false;
            actions.BackColor = SurfaceColor;
            actions.Padding = new Padding(0, 2, 0, 0);

            studyTreeToolbarToolTip = new ToolTip();
            studyTreeFilterMenu = BuildStudyTreeFilterMenu();

            btnCreateStudyFile = CreateToolbarIconButton("파일 생성", "study-file-create.png");
            btnCreateStudyFile.Click += delegate { AddFileFromSelection(); };

            btnCreateStudyFolder = CreateToolbarIconButton("폴더 생성", "study-folder-create.png");
            btnCreateStudyFolder.Click += delegate { AddFolderFromSelection(); };

            btnStudyTreeFilter = CreateToolbarIconButton("필터", "study-filter.png");
            btnStudyTreeFilter.Click += delegate
            {
                studyTreeFilterMenu.Show(btnStudyTreeFilter, new Point(0, btnStudyTreeFilter.Height + 4));
            };

            actions.Controls.Add(btnCreateStudyFile);
            actions.Controls.Add(btnCreateStudyFolder);
            actions.Controls.Add(btnStudyTreeFilter);

            studyTreeView = new TreeView();
            studyTreeView.Dock = DockStyle.Fill;
            studyTreeView.BackColor = SurfaceColor;
            studyTreeView.BorderStyle = BorderStyle.None;
            studyTreeView.Font = new Font("맑은 고딕", 9.5F);
            studyTreeView.ForeColor = TextColor;
            studyTreeView.HideSelection = false;
            studyTreeView.LineColor = BorderColor;
            studyTreeView.ShowLines = true;
            studyTreeView.ShowPlusMinus = true;
            studyTreeView.ShowRootLines = true;
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

        private Button CreateToolbarIconButton(string tooltip, string iconFileName)
        {
            Button button = new Button();
            button.Size = new Size(36, 36);
            button.Margin = new Padding(0, 0, 12, 0);
            button.BackColor = SurfaceColor;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseDownBackColor = AccentColor;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(42, 42, 42);
            button.FlatStyle = FlatStyle.Flat;
            button.TabStop = false;
            button.UseVisualStyleBackColor = false;
            button.Image = LoadTintedToolbarIcon(iconFileName);
            button.ImageAlign = ContentAlignment.MiddleCenter;
            studyTreeToolbarToolTip.SetToolTip(button, tooltip);
            return button;
        }

        private Image LoadTintedToolbarIcon(string iconFileName)
        {
            string iconPath = Path.Combine(Application.StartupPath, "Assets", iconFileName);
            if (!File.Exists(iconPath))
            {
                return null;
            }

            using (Bitmap source = new Bitmap(iconPath))
            {
                return TintToolbarIcon(source, Color.FromArgb(166, 166, 166), new Size(24, 24));
            }
        }

        private Image TintToolbarIcon(Bitmap source, Color color, Size size)
        {
            Bitmap resized = new Bitmap(size.Width, size.Height);
            using (Graphics graphics = Graphics.FromImage(resized))
            {
                graphics.Clear(Color.Transparent);
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.DrawImage(source, new Rectangle(Point.Empty, size));
            }

            Bitmap tinted = new Bitmap(size.Width, size.Height);
            for (int x = 0; x < resized.Width; x++)
            {
                for (int y = 0; y < resized.Height; y++)
                {
                    Color pixel = resized.GetPixel(x, y);
                    tinted.SetPixel(x, y, pixel.A == 0 ? Color.Transparent : Color.FromArgb(pixel.A, color));
                }
            }

            resized.Dispose();
            return tinted;
        }

        private ContextMenuStrip BuildStudyTreeFilterMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.BackColor = SurfaceColor;
            menu.ForeColor = TextColor;
            menu.RenderMode = ToolStripRenderMode.System;

            ToolStripMenuItem nameAscendingItem = new ToolStripMenuItem("파일 이름 A to Z");
            nameAscendingItem.Click += delegate { SetStudyTreeSortMode(StudyTreeSortMode.NameAscending); };

            ToolStripMenuItem nameDescendingItem = new ToolStripMenuItem("파일 이름 Z to A");
            nameDescendingItem.Click += delegate { SetStudyTreeSortMode(StudyTreeSortMode.NameDescending); };

            ToolStripMenuItem createdDescendingItem = new ToolStripMenuItem("생성 시간 최신순");
            createdDescendingItem.Click += delegate { SetStudyTreeSortMode(StudyTreeSortMode.CreatedDescending); };

            menu.Items.Add(nameAscendingItem);
            menu.Items.Add(nameDescendingItem);
            menu.Items.Add(createdDescendingItem);
            menu.Opening += delegate { UpdateStudyTreeFilterMenuChecks(); };
            return menu;
        }

        private void SetStudyTreeSortMode(StudyTreeSortMode sortMode)
        {
            studyTreeSortMode = sortMode;
            UpdateStudyTreeFilterMenuChecks();
            RefreshStudyTree();
        }

        private void UpdateStudyTreeFilterMenuChecks()
        {
            if (studyTreeFilterMenu == null || studyTreeFilterMenu.Items.Count < 3)
            {
                return;
            }

            studyTreeFilterMenu.Items[0].Text = studyTreeSortMode == StudyTreeSortMode.NameAscending
                ? "✓ 파일 이름 A to Z"
                : "파일 이름 A to Z";
            studyTreeFilterMenu.Items[1].Text = studyTreeSortMode == StudyTreeSortMode.NameDescending
                ? "✓ 파일 이름 Z to A"
                : "파일 이름 Z to A";
            studyTreeFilterMenu.Items[2].Text = studyTreeSortMode == StudyTreeSortMode.CreatedDescending
                ? "✓ 생성 시간 최신순"
                : "생성 시간 최신순";
        }

        // 트리에서 우클릭했을 때 나오는 메뉴를 만든다.
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

        // 오른쪽 탭 4개를 만든다.
        private void BuildMainTabs(Control parent)
        {
            Panel root = new Panel();
            root.Dock = DockStyle.Fill;
            root.BackColor = AppBackColor;

            dashboardTabBar = new FlowLayoutPanel();
            dashboardTabBar.Dock = DockStyle.Top;
            dashboardTabBar.Height = 58;
            dashboardTabBar.BackColor = AppBackColor;
            dashboardTabBar.FlowDirection = FlowDirection.LeftToRight;
            dashboardTabBar.WrapContents = false;
            dashboardTabBar.Padding = new Padding(20, 10, 20, 8);

            dashboardTabs = new HiddenHeaderTabControl();
            dashboardTabs.Dock = DockStyle.Fill;
            dashboardTabs.Font = new Font("맑은 고딕", 9F);
            dashboardTabs.Appearance = TabAppearance.FlatButtons;
            dashboardTabs.ItemSize = new Size(1, 1);
            dashboardTabs.Padding = Point.Empty;
            dashboardTabs.SizeMode = TabSizeMode.Fixed;
            dashboardTabs.TabStop = false;
            dashboardTabs.SelectedIndexChanged += delegate { UpdateDashboardTabButtons(); };

            TabPage fileTab = new TabPage("파일 편집");
            TabPage timelineTab = new TabPage("일별 시계열");
            TabPage historyTab = new TabPage("세션 내역");
            TabPage appUsageTab = new TabPage("앱 사용");

            StyleTabPage(fileTab);
            StyleTabPage(timelineTab);
            StyleTabPage(historyTab);
            StyleTabPage(appUsageTab);

            BuildFileTab(fileTab);
            BuildTimelineTab(timelineTab);
            BuildHistoryTab(historyTab);
            BuildAppUsageTab(appUsageTab);

            dashboardTabs.TabPages.Add(fileTab);
            dashboardTabs.TabPages.Add(timelineTab);
            dashboardTabs.TabPages.Add(historyTab);
            dashboardTabs.TabPages.Add(appUsageTab);

            dashboardTabButtons.Clear();
            for (int i = 0; i < dashboardTabs.TabPages.Count; i++)
            {
                Button tabButton = CreateDashboardTabButton(dashboardTabs.TabPages[i].Text, i);
                dashboardTabButtons.Add(tabButton);
                dashboardTabBar.Controls.Add(tabButton);
            }

            UpdateDashboardTabButtons();

            root.Controls.Add(dashboardTabs);
            root.Controls.Add(dashboardTabBar);
            parent.Controls.Add(root);
        }

        private void BuildFileTab(TabPage fileTab)
        {
            Panel root = new Panel();
            root.Dock = DockStyle.Fill;
            root.BackColor = AppBackColor;
            root.Padding = new Padding(20);

            Panel header = new Panel();
            header.Dock = DockStyle.Top;
            header.BackColor = AppBackColor;
            header.Height = 54;

            lblFileTitle = new Label();
            lblFileTitle.Dock = DockStyle.Fill;
            lblFileTitle.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            lblFileTitle.ForeColor = TextColor;
            lblFileTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblFileTitle.Text = "파일을 선택하세요";

            lblFileDate = new Label();
            lblFileDate.Dock = DockStyle.Right;
            lblFileDate.Width = 132;
            lblFileDate.Font = new Font("맑은 고딕", 9.5F, FontStyle.Regular);
            lblFileDate.ForeColor = MutedTextColor;
            lblFileDate.TextAlign = ContentAlignment.MiddleRight;

            FlowLayoutPanel fileActions = new FlowLayoutPanel();
            fileActions.Dock = DockStyle.Right;
            fileActions.Width = 390;
            fileActions.FlowDirection = FlowDirection.RightToLeft;
            fileActions.WrapContents = false;
            fileActions.BackColor = AppBackColor;

            btnSaveFile = new Button();
            btnSaveFile.Size = new Size(82, 34);
            btnSaveFile.Text = "저장";
            btnSaveFile.Enabled = false;
            btnSaveFile.Click += delegate { SaveCurrentFile(); };
            StylePrimaryButton(btnSaveFile);

            btnStartFocusFromPlan = new Button();
            btnStartFocusFromPlan.Size = new Size(174, 34);
            btnStartFocusFromPlan.Text = "집중 시작";
            btnStartFocusFromPlan.Enabled = false;
            btnStartFocusFromPlan.Click += btnStartFocusFromPlan_Click;
            StyleSecondaryButton(btnStartFocusFromPlan);

            btnToggleMarkdownPreview = new Button();
            btnToggleMarkdownPreview.Size = new Size(112, 34);
            btnToggleMarkdownPreview.Text = "미리보기";
            btnToggleMarkdownPreview.Enabled = false;
            btnToggleMarkdownPreview.Click += delegate { ToggleMarkdownPreview(); };
            StyleSecondaryButton(btnToggleMarkdownPreview);

            markdownEditorHost = new Panel();
            markdownEditorHost.Dock = DockStyle.Fill;
            markdownEditorHost.BackColor = FieldColor;

            txtMemo.Dock = DockStyle.Fill;
            txtMemo.BackColor = FieldColor;
            txtMemo.BorderStyle = BorderStyle.FixedSingle;
            txtMemo.Font = new Font("맑은 고딕", 10F);
            txtMemo.ForeColor = TextColor;
            txtMemo.Multiline = true;
            txtMemo.ScrollBars = ScrollBars.Both;
            txtMemo.AcceptsReturn = true;
            txtMemo.AcceptsTab = true;
            txtMemo.WordWrap = true;
            txtMemo.Enabled = false;
            txtMemo.TextChanged += txtMemo_TextChanged;

            markdownPreviewBrowser = new WebBrowser();
            markdownPreviewBrowser.Dock = DockStyle.Fill;
            markdownPreviewBrowser.AllowWebBrowserDrop = false;
            markdownPreviewBrowser.IsWebBrowserContextMenuEnabled = false;
            markdownPreviewBrowser.ScriptErrorsSuppressed = true;
            markdownPreviewBrowser.WebBrowserShortcutsEnabled = false;
            markdownPreviewBrowser.Visible = false;

            fileActions.Controls.Add(btnSaveFile);
            fileActions.Controls.Add(btnStartFocusFromPlan);
            fileActions.Controls.Add(btnToggleMarkdownPreview);

            markdownEditorHost.Controls.Add(markdownPreviewBrowser);
            markdownEditorHost.Controls.Add(txtMemo);
            header.Controls.Add(lblFileTitle);
            header.Controls.Add(lblFileDate);
            header.Controls.Add(fileActions);
            root.Controls.Add(markdownEditorHost);
            root.Controls.Add(header);
            fileTab.Controls.Add(root);
        }

        private void BuildTimelineTab(TabPage timelineTab)
        {
            Panel root = new Panel();
            root.Dock = DockStyle.Fill;
            root.BackColor = AppBackColor;
            root.Padding = new Padding(20);

            Panel header = new Panel();
            header.Dock = DockStyle.Top;
            header.BackColor = AppBackColor;
            header.Height = 54;

            Label dateLabel = new Label();
            dateLabel.Dock = DockStyle.Left;
            dateLabel.Width = 98;
            dateLabel.Text = "조회 날짜";
            dateLabel.ForeColor = TextColor;
            dateLabel.Font = new Font("맑은 고딕", 9.5F);
            dateLabel.TextAlign = ContentAlignment.MiddleLeft;

            dayPicker = new DateTimePicker();
            dayPicker.Dock = DockStyle.Left;
            dayPicker.Width = 160;
            dayPicker.Format = DateTimePickerFormat.Short;
            dayPicker.ValueChanged += delegate { ApplyDayFilter(); };

            Button btnRefresh = new Button();
            btnRefresh.Dock = DockStyle.Right;
            btnRefresh.Width = 110;
            btnRefresh.Text = "새로고침";
            btnRefresh.Click += delegate { RefreshDashboard(); };
            StyleSecondaryButton(btnRefresh);

            lblDaySummary = new Label();
            lblDaySummary.Dock = DockStyle.Top;
            lblDaySummary.Height = 156;
            lblDaySummary.BackColor = SurfaceColor;
            lblDaySummary.ForeColor = TextColor;
            lblDaySummary.BorderStyle = BorderStyle.None;
            lblDaySummary.Padding = new Padding(12);
            lblDaySummary.Font = new Font("맑은 고딕", 10F);

            timelineHostPanel = new Panel();
            timelineHostPanel.Dock = DockStyle.Fill;
            timelineHostPanel.AutoScroll = true;
            timelineHostPanel.BackColor = SurfaceColor;
            timelineHostPanel.BorderStyle = BorderStyle.None;
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
            split.BackColor = AppBackColor;
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
            StyleDataGridView(dgvSessions);
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
            txtSelectedSessionSummary.BackColor = FieldColor;
            txtSelectedSessionSummary.BorderStyle = BorderStyle.FixedSingle;
            txtSelectedSessionSummary.Font = new Font("맑은 고딕", 10F);
            txtSelectedSessionSummary.ForeColor = TextColor;
            txtSelectedSessionSummary.Multiline = true;
            txtSelectedSessionSummary.ReadOnly = true;
            txtSelectedSessionSummary.ScrollBars = ScrollBars.Vertical;

            Button btnOpenReport = new Button();
            btnOpenReport.Dock = DockStyle.Bottom;
            btnOpenReport.Height = 42;
            btnOpenReport.Text = "선택 세션 리포트 열기";
            btnOpenReport.Click += btnOpenReport_Click;
            StylePrimaryButton(btnOpenReport);

            split.Panel1.Controls.Add(dgvSessions);
            split.Panel2.Controls.Add(txtSelectedSessionSummary);
            split.Panel2.Controls.Add(btnOpenReport);
            historyTab.Controls.Add(split);
        }

        private void BuildAppUsageTab(TabPage appUsageTab)
        {
            lvAppUsage = new ListView();
            lvAppUsage.Dock = DockStyle.Fill;
            lvAppUsage.BackColor = FieldColor;
            lvAppUsage.BorderStyle = BorderStyle.None;
            lvAppUsage.Font = new Font("맑은 고딕", 10F);
            lvAppUsage.ForeColor = TextColor;
            lvAppUsage.View = View.Details;
            lvAppUsage.FullRowSelect = true;
            lvAppUsage.GridLines = true;
            lvAppUsage.Columns.Add("앱", 180);
            lvAppUsage.Columns.Add("활성", 95);
            lvAppUsage.Columns.Add("휴식", 95);
            lvAppUsage.Columns.Add("전환", 75);

            appUsageTab.Padding = new Padding(12);
            appUsageTab.BackColor = AppBackColor;
            appUsageTab.Controls.Add(lvAppUsage);
        }

        private void StyleTabPage(TabPage page)
        {
            page.BackColor = AppBackColor;
            page.ForeColor = TextColor;
            page.UseVisualStyleBackColor = false;
        }

        private Button CreateDashboardTabButton(string text, int tabIndex)
        {
            Button button = new Button();
            button.Size = new Size(112, 34);
            button.Margin = new Padding(0, 0, 10, 0);
            button.FlatAppearance.BorderSize = 0;
            button.FlatStyle = FlatStyle.Flat;
            button.Font = new Font("맑은 고딕", 9F, FontStyle.Bold);
            button.TabStop = false;
            button.Tag = tabIndex;
            button.Text = text;
            button.UseVisualStyleBackColor = false;
            button.Click += delegate
            {
                dashboardTabs.SelectedIndex = (int)button.Tag;
                UpdateDashboardTabButtons();
            };
            return button;
        }

        private void UpdateDashboardTabButtons()
        {
            for (int i = 0; i < dashboardTabButtons.Count; i++)
            {
                Button button = dashboardTabButtons[i];
                bool selected = dashboardTabs != null && dashboardTabs.SelectedIndex == i;
                button.BackColor = selected ? AccentColor : FieldColor;
                button.ForeColor = selected ? TextColor : MutedTextColor;
                button.FlatAppearance.MouseDownBackColor = selected ? AccentHoverColor : AccentColor;
                button.FlatAppearance.MouseOverBackColor = selected ? AccentHoverColor : Color.FromArgb(48, 48, 54);
            }
        }

        private sealed class HiddenHeaderTabControl : TabControl
        {
            private const int TcmAdjustRect = 0x1328;

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == TcmAdjustRect && !DesignMode)
                {
                    m.Result = new IntPtr(1);
                    return;
                }

                base.WndProc(ref m);
            }
        }

        private void StylePrimaryButton(Button button)
        {
            StyleButton(button, AccentColor, AccentHoverColor);
        }

        private void StyleSecondaryButton(Button button)
        {
            StyleButton(button, FieldColor, Color.FromArgb(48, 48, 54));
        }

        private void StyleButton(Button button, Color fillColor, Color hoverColor)
        {
            button.BackColor = fillColor;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseDownBackColor = AccentColor;
            button.FlatAppearance.MouseOverBackColor = hoverColor;
            button.FlatStyle = FlatStyle.Flat;
            button.Font = new Font("맑은 고딕", 9F, FontStyle.Bold);
            button.ForeColor = TextColor;
            button.UseVisualStyleBackColor = false;
        }

        private void StyleDataGridView(DataGridView grid)
        {
            grid.BackgroundColor = FieldColor;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.EnableHeadersVisualStyles = false;
            grid.GridColor = BorderColor;
            grid.DefaultCellStyle.BackColor = FieldColor;
            grid.DefaultCellStyle.ForeColor = TextColor;
            grid.DefaultCellStyle.SelectionBackColor = AccentColor;
            grid.DefaultCellStyle.SelectionForeColor = TextColor;
            grid.ColumnHeadersDefaultCellStyle.BackColor = SurfaceColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = TextColor;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("맑은 고딕", 9F, FontStyle.Bold);
            grid.RowTemplate.Height = 30;
        }

        private void StudyPlanForm_Load(object sender, EventArgs e)
        {
            RefreshStudyTree();
            SelectInitialStudyFile();
            RefreshDashboard();
        }

        private void StudyPlanForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveCurrentFile();
        }

    }
}
