using System;
using System.Drawing;
using System.Windows.Forms;
using Prototype1.UI;

namespace Prototype1
{
    public sealed class FocusSessionReportForm : Form
    {
        private readonly FocusSessionRecord session;
        private Label titleLabel;
        private Label summaryTitleLabel;
        private Label appUsageTitleLabel;
        private TextBox summaryTextBox;
        private ListView appUsageListView;
        private Button closeButton;

        public FocusSessionReportForm()
            : this(null)
        {
        }

        public FocusSessionReportForm(FocusSessionRecord session)
        {
            this.session = session;
            InitializeComponent();
            LoadReport();
        }

        private void InitializeComponent()
        {
            titleLabel = new Label();
            summaryTitleLabel = new Label();
            appUsageTitleLabel = new Label();
            summaryTextBox = new TextBox();
            appUsageListView = new ListView();
            closeButton = new Button();

            SuspendLayout();

            Text = "세션 리포트";
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(760, 500);
            ClientSize = new Size(820, 560);
            BackColor = AlertDialog.AppBackColor;
            ForeColor = AlertDialog.TextColor;
            Font = new Font("맑은 고딕", 10F, FontStyle.Regular);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            titleLabel.AutoSize = false;
            titleLabel.Font = new Font("맑은 고딕", 18F, FontStyle.Bold);
            titleLabel.ForeColor = AlertDialog.TextColor;
            titleLabel.Location = new Point(24, 22);
            titleLabel.Size = new Size(760, 42);

            summaryTitleLabel.AutoSize = false;
            summaryTitleLabel.Font = new Font("맑은 고딕", 10F, FontStyle.Bold);
            summaryTitleLabel.ForeColor = AlertDialog.SubtleTextColor;
            summaryTitleLabel.Location = new Point(24, 78);
            summaryTitleLabel.Size = new Size(760, 24);
            summaryTitleLabel.Text = "세션 요약";

            summaryTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            summaryTextBox.BackColor = AlertDialog.FieldColor;
            summaryTextBox.BorderStyle = BorderStyle.FixedSingle;
            summaryTextBox.Font = new Font("맑은 고딕", 10F, FontStyle.Regular);
            summaryTextBox.ForeColor = AlertDialog.TextColor;
            summaryTextBox.Location = new Point(24, 106);
            summaryTextBox.Multiline = true;
            summaryTextBox.ReadOnly = true;
            summaryTextBox.ScrollBars = ScrollBars.Vertical;
            summaryTextBox.Size = new Size(760, 134);

            appUsageTitleLabel.AutoSize = false;
            appUsageTitleLabel.Font = new Font("맑은 고딕", 10F, FontStyle.Bold);
            appUsageTitleLabel.ForeColor = AlertDialog.SubtleTextColor;
            appUsageTitleLabel.Location = new Point(24, 260);
            appUsageTitleLabel.Size = new Size(760, 24);
            appUsageTitleLabel.Text = "앱 사용 기록";

            appUsageListView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            appUsageListView.BackColor = AlertDialog.FieldColor;
            appUsageListView.BorderStyle = BorderStyle.FixedSingle;
            appUsageListView.ForeColor = AlertDialog.TextColor;
            appUsageListView.Location = new Point(24, 290);
            appUsageListView.Size = new Size(760, 208);
            appUsageListView.View = View.Details;
            appUsageListView.FullRowSelect = true;
            appUsageListView.GridLines = false;
            appUsageListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            appUsageListView.OwnerDraw = true;
            appUsageListView.DrawColumnHeader += appUsageListView_DrawColumnHeader;
            appUsageListView.DrawItem += appUsageListView_DrawItem;
            appUsageListView.DrawSubItem += appUsageListView_DrawSubItem;
            appUsageListView.Columns.Add("앱", 260);
            appUsageListView.Columns.Add("활성", 150);
            appUsageListView.Columns.Add("일시정지", 150);
            appUsageListView.Columns.Add("전환", 110);

            closeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            closeButton.Location = new Point(690, 512);
            closeButton.Size = new Size(110, 34);
            closeButton.Text = "닫기";
            AlertDialog.StyleButton(closeButton, true);
            closeButton.Click += closeButton_Click;

            Controls.Add(titleLabel);
            Controls.Add(summaryTitleLabel);
            Controls.Add(summaryTextBox);
            Controls.Add(appUsageTitleLabel);
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

            if (appUsageListView.Items.Count == 0)
            {
                ListViewItem emptyItem = new ListViewItem("기록된 앱 사용 없음");
                emptyItem.SubItems.Add("-");
                emptyItem.SubItems.Add("-");
                emptyItem.SubItems.Add("-");
                appUsageListView.Items.Add(emptyItem);
            }
        }

        private void appUsageListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            using (SolidBrush background = new SolidBrush(AlertDialog.SurfaceColor))
            {
                e.Graphics.FillRectangle(background, e.Bounds);
            }

            TextRenderer.DrawText(
                e.Graphics,
                e.Header.Text,
                new Font("맑은 고딕", 9F, FontStyle.Bold),
                e.Bounds,
                AlertDialog.SubtleTextColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
        }

        private void appUsageListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
        }

        private void appUsageListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            Color backgroundColor = e.Item.Selected ? AlertDialog.AccentColor : AlertDialog.FieldColor;
            Color textColor = e.Item.Selected ? AlertDialog.TextColor : AlertDialog.SubtleTextColor;

            using (SolidBrush background = new SolidBrush(backgroundColor))
            {
                e.Graphics.FillRectangle(background, e.Bounds);
            }

            Rectangle textBounds = new Rectangle(e.Bounds.Left + 8, e.Bounds.Top, e.Bounds.Width - 10, e.Bounds.Height);
            TextRenderer.DrawText(
                e.Graphics,
                e.SubItem.Text,
                Font,
                textBounds,
                textColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
