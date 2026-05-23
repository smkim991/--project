using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Prototype1.UI
{
    public partial class StudyPlanForm : Form
    {
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
                        summary = new AppUsageSummary { AppName = app.AppName };
                        byApp.Add(app.AppName, summary);
                    }

                    summary.ActiveSeconds += app.ActiveSeconds;
                    summary.BreakSeconds += app.BreakSeconds;
                    summary.SwitchEntries += app.SwitchEntries;
                }
            }

            return byApp.Values
                .OrderByDescending(a => a.ActiveSeconds)
                .ThenByDescending(a => a.ActiveSeconds + a.BreakSeconds)
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

    }
}
