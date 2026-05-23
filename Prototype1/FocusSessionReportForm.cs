using System;
using System.Drawing;
using System.Windows.Forms;

namespace Prototype1
{
    public sealed class FocusSessionReportForm : Form
    {
        private readonly FocusSessionRecord session;
        private Label titleLabel;
        private TextBox summaryTextBox;
        private ListView appUsageListView;
        private Button closeButton;

        public FocusSessionReportForm(FocusSessionRecord session)
        {
            this.session = session;
            InitializeComponent();
            LoadReport();
        }

        private void InitializeComponent()
        {
            titleLabel = new Label();
            summaryTextBox = new TextBox();
            appUsageListView = new ListView();
            closeButton = new Button();

            SuspendLayout();

            Text = "세션 리포트";
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(760, 500);
            ClientSize = new Size(820, 560);

            titleLabel.AutoSize = false;
            titleLabel.Font = new Font(Font.FontFamily, 14F, FontStyle.Bold);
            titleLabel.Location = new Point(18, 18);
            titleLabel.Size = new Size(782, 34);

            summaryTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            summaryTextBox.Location = new Point(18, 62);
            summaryTextBox.Multiline = true;
            summaryTextBox.ReadOnly = true;
            summaryTextBox.ScrollBars = ScrollBars.Vertical;
            summaryTextBox.Size = new Size(782, 138);

            appUsageListView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            appUsageListView.Location = new Point(18, 218);
            appUsageListView.Size = new Size(782, 280);
            appUsageListView.View = View.Details;
            appUsageListView.FullRowSelect = true;
            appUsageListView.GridLines = true;
            appUsageListView.Columns.Add("앱", 180);
            appUsageListView.Columns.Add("활성", 110);
            appUsageListView.Columns.Add("휴식", 110);
            appUsageListView.Columns.Add("전환", 80);

            closeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            closeButton.Location = new Point(690, 512);
            closeButton.Size = new Size(110, 34);
            closeButton.Text = "닫기";
            closeButton.Click += delegate { Close(); };

            Controls.Add(titleLabel);
            Controls.Add(summaryTextBox);
            Controls.Add(appUsageListView);
            Controls.Add(closeButton);

            ResumeLayout(false);
        }

        private void LoadReport()
        {
            if (session == null)
            {
                titleLabel.Text = "세션 없음";
                summaryTextBox.Text = "완료된 세션이 없습니다.";
                return;
            }

            titleLabel.Text = session.StartedAt.ToString("HH:mm") + " 세션";
            summaryTextBox.Text = FocusSessionReportBuilder.BuildSessionSummaryText(session);

            appUsageListView.Items.Clear();
            foreach (AppUsageSummary app in FocusSessionReportBuilder.BuildAppUsage(session))
            {
                if (app.ActiveSeconds <= 0 && app.BreakSeconds <= 0)
                {
                    continue;
                }

                ListViewItem item = new ListViewItem(app.AppName);
                item.SubItems.Add(FocusSessionReportBuilder.FormatDuration(app.ActiveSeconds));
                item.SubItems.Add(FocusSessionReportBuilder.FormatDuration(app.BreakSeconds));
                item.SubItems.Add(app.SwitchEntries.ToString());
                appUsageListView.Items.Add(item);
            }
        }
    }
}
