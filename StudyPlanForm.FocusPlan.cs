using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Prototype1.UI
{
    public partial class StudyPlanForm : Form
    {
        private void btnStartFocusFromPlan_Click(object sender, EventArgs e)
        {
            if (DataModel.IsBlockingActive)
            {
                AlertDialog.Show(this, "이미 집중 모드가 실행 중입니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (DataModel.IsEmergencyLockedOut)
            {
                AlertDialog.Show(
                    this,
                    "라이프를 모두 소진해서 지금은 집중 모드를 시작할 수 없습니다.\r\n다시 시작 가능 시간: " + DataModel.EmergencyLockUntil.ToString("yyyy-MM-dd HH:mm"),
                    "알림",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(currentFilePath) || !File.Exists(currentFilePath))
            {
                AlertDialog.Show(this, "먼저 파일을 선택하세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            DataModel.SetCurrentFocusTasks(ExtractFocusTasks(txtMemo.Text));
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

        private List<DataModel.FocusTaskProgress> ExtractFocusTasks(string planText)
        {
            List<DataModel.FocusTaskProgress> tasks = new List<DataModel.FocusTaskProgress>();
            string[] lines = (planText ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                Match taskMatch = Regex.Match(lines[i], @"^\s*[-*+]\s+\[( |x|X)\]\s+(.+)$");
                if (!taskMatch.Success)
                {
                    continue;
                }

                string taskText = taskMatch.Groups[2].Value.Trim();
                if (string.IsNullOrWhiteSpace(taskText))
                {
                    continue;
                }

                tasks.Add(new DataModel.FocusTaskProgress
                {
                    Text = taskText,
                    IsCompleted = !taskMatch.Groups[1].Value.Equals(" ", StringComparison.Ordinal),
                    SourceFilePath = currentFilePath ?? string.Empty,
                    SourceLineIndex = i
                });
            }

            return tasks;
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
                dialog.BackColor = AppBackColor;
                dialog.ForeColor = TextColor;
                dialog.ClientSize = new Size(460, 330);

                goalLabel.Text = "목표";
                goalLabel.ForeColor = TextColor;
                goalLabel.Location = new Point(16, 16);
                goalLabel.Size = new Size(420, 22);

                goalBox.Text = draft.Goal;
                goalBox.BackColor = FieldColor;
                goalBox.BorderStyle = BorderStyle.FixedSingle;
                goalBox.ForeColor = TextColor;
                goalBox.Location = new Point(16, 42);
                goalBox.Size = new Size(420, 26);

                durationLabel.Text = "집중 시간";
                durationLabel.ForeColor = TextColor;
                durationLabel.Location = new Point(16, 82);
                durationLabel.Size = new Size(120, 22);

                durationBox.BackColor = FieldColor;
                durationBox.ForeColor = TextColor;
                durationBox.Minimum = 5;
                durationBox.Maximum = 240;
                durationBox.Increment = 5;
                durationBox.Value = Math.Max(durationBox.Minimum, Math.Min(durationBox.Maximum, draft.DurationMinutes));
                durationBox.Location = new Point(16, 108);
                durationBox.Size = new Size(120, 26);

                modeLabel.Text = "추천 모드";
                modeLabel.ForeColor = TextColor;
                modeLabel.Location = new Point(156, 82);
                modeLabel.Size = new Size(120, 22);

                modeBox.BackColor = FieldColor;
                modeBox.DropDownStyle = ComboBoxStyle.DropDownList;
                modeBox.ForeColor = TextColor;
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
                blockLabel.ForeColor = TextColor;
                blockLabel.Location = new Point(16, 150);
                blockLabel.Size = new Size(420, 22);

                blockBox.BackColor = FieldColor;
                blockBox.BorderStyle = BorderStyle.FixedSingle;
                blockBox.ForeColor = TextColor;
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
                StylePrimaryButton(startButton);

                cancelButton.Text = "취소";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Location = new Point(348, 278);
                cancelButton.Size = new Size(88, 34);
                AlertDialog.StyleButton(cancelButton, false, true);

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

        private sealed class PlanFocusDraft
        {
            public string Goal { get; set; } = "집중 세션";
            public int DurationMinutes { get; set; } = 50;
            public string Category { get; set; } = "대학생";
        }

    }
}
