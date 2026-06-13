using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Prototype1.UI;

namespace Prototype1
{
    public partial class MainForm : Form
    {
        private enum StopRequestAction
        {
            Cancel,
            LifeBreak,
            EmergencyStop
        }

        private const int TimePickerDropDownVisibleItems = 10;
        private static readonly Color FocusPanelBackColor = Color.FromArgb(24, 24, 24);
        private static readonly Color PausePanelBackColor = Color.FromArgb(56, 118, 121);

        private Dictionary<string, List<string>> currentBlockedItems;

        private static readonly Dictionary<string, string> BlockedProcessAliases =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "유튜브", "chrome" },
            { "넷플릭스", "chrome" },
            { "네이버웹툰", "chrome" },
            { "카카오톡", "KakaoTalk" },
            { "틱톡", "TikTok" },
            { "인스타그램", "Instagram" },
            { "메모장", "notepad" },
            { "멜론", "Melon" },
            { "엑셀", "EXCEL" }
        };

        private static readonly HashSet<string> BrowserProcessNames =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "chrome",
                "msedge",
                "whale",
                "firefox"
            };

        public MainForm()
        {
            InitializeComponent();
            ConfigureMainScreenLayout();
        }

        private void ConfigureMainScreenLayout()
        {
            guna2Panel1.Size = new Size(856, 500);
            btnActivateBlocking.Size = new Size(320, 68);

            lblShowTimeLeft.AutoSize = false;
            lblShowTimeLeft.TextAlign = ContentAlignment.MiddleCenter;

            label1.AutoSize = false;
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label2.AutoSize = false;
            label2.TextAlign = ContentAlignment.MiddleCenter;
            label3.AutoSize = false;
            label3.TextAlign = ContentAlignment.MiddleCenter;
            label4.AutoSize = false;
            label4.TextAlign = ContentAlignment.MiddleCenter;
            label7.AutoSize = false;
            label7.TextAlign = ContentAlignment.MiddleCenter;

            btnStopBlocking.TextAlign = ContentAlignment.MiddleCenter;
            btnActivateBlocking.TextAlign = HorizontalAlignment.Center;

            cmbHour.TextAlign = HorizontalAlignment.Center;
            cmbMin.TextAlign = HorizontalAlignment.Center;
            cmbHour.ItemHeight = 34;
            cmbMin.ItemHeight = 34;
            cmbHour.DrawItem += timeComboBox_DrawItem;
            cmbMin.DrawItem += timeComboBox_DrawItem;

            Resize += delegate { ArrangeMainScreenLayout(); };
            guna2Panel1.Resize += delegate { ArrangeFocusPanelContent(); };
            ArrangeMainScreenLayout();
        }

        private void ArrangeMainScreenLayout()
        {
            if (guna2Panel1 == null || guna2Panel2 == null || guna2Panel3 == null)
            {
                return;
            }

            int contentLeft = guna2Panel2.Width;
            int contentTop = guna2Panel3.Height;
            int contentWidth = Math.Max(0, ClientSize.Width - contentLeft);
            int contentHeight = Math.Max(0, ClientSize.Height - contentTop);

            guna2Panel1.Location = new Point(
                contentLeft + Math.Max(0, (contentWidth - guna2Panel1.Width) / 2),
                contentTop + Math.Max(0, (contentHeight - guna2Panel1.Height) / 2));

            btnStopBlocking.Location = new Point(
                contentLeft + Math.Max(0, contentWidth - btnStopBlocking.Width - 98),
                Math.Max(contentTop, ClientSize.Height - btnStopBlocking.Height - 106));

            ArrangeFocusPanelContent();
        }

        private void ArrangeFocusPanelContent()
        {
            if (guna2Panel1 == null)
            {
                return;
            }

            int panelWidth = guna2Panel1.Width;
            int panelHeight = guna2Panel1.Height;
            int contentInset = 34;
            int contentWidth = Math.Max(120, panelWidth - (contentInset * 2));
            int rowHeight = 44;
            int buttonY = Math.Max(370, panelHeight - btnActivateBlocking.Height - 36);
            int rowY = buttonY - 88;

            lblShowTimeLeft.Location = new Point(0, 40);
            lblShowTimeLeft.Size = new Size(panelWidth, 124);

            label4.Location = new Point(contentInset, 178);
            label4.Size = new Size(contentWidth, 50);

            label7.Location = new Point(contentInset, 240);
            label7.Size = new Size(contentWidth, 42);

            int titleWidth = 112;
            int comboWidth = 140;
            int unitWidth = 44;
            int minuteUnitWidth = 34;
            int gap = 12;
            int wideGap = 22;
            int groupWidth = titleWidth + gap + comboWidth + gap + unitWidth + wideGap + comboWidth + gap + minuteUnitWidth;
            int groupX = Math.Max(contentInset, (panelWidth - groupWidth) / 2);

            label1.Location = new Point(groupX, rowY);
            label1.Size = new Size(titleWidth, rowHeight);
            cmbHour.Location = new Point(groupX + titleWidth + gap, rowY);
            cmbHour.Size = new Size(comboWidth, rowHeight);
            label2.Location = new Point(cmbHour.Right + gap, rowY);
            label2.Size = new Size(unitWidth, rowHeight);
            cmbMin.Location = new Point(label2.Right + wideGap, rowY);
            cmbMin.Size = new Size(comboWidth, rowHeight);
            label3.Location = new Point(cmbMin.Right + gap, rowY);
            label3.Size = new Size(minuteUnitWidth, rowHeight);

            btnActivateBlocking.Location = new Point((panelWidth - btnActivateBlocking.Width) / 2, buttonY);
        }

        private void timeComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            Guna.UI2.WinForms.Guna2ComboBox comboBox = sender as Guna.UI2.WinForms.Guna2ComboBox;
            if (comboBox == null || e.Index < 0)
            {
                return;
            }

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            using (SolidBrush background = new SolidBrush(selected ? Color.FromArgb(139, 92, 246) : Color.FromArgb(35, 35, 35)))
            {
                e.Graphics.FillRectangle(background, e.Bounds);
            }

            TextRenderer.DrawText(
                e.Graphics,
                comboBox.Items[e.Index].ToString(),
                comboBox.Font,
                e.Bounds,
                Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);

            e.DrawFocusRectangle();
        }

        private void LoadBlockProfiles() { currentBlockedItems = DataModel.GetBlockProfilesCopy(); }

        private void KillProcesses(IEnumerable<string> blockList)
        {
            if (blockList == null)
            {
                return;
            }

            int currentProcessId;
            using (Process currentProcess = Process.GetCurrentProcess())
            {
                currentProcessId = currentProcess.Id;
            }

            foreach (string processName in blockList)
            {
                if (string.IsNullOrWhiteSpace(processName))
                {
                    continue;
                }

                Process[] processes = Process.GetProcessesByName(processName);

                foreach (Process p in processes)
                {
                    try
                    {
                        if (p.Id == currentProcessId)
                        {
                            continue;
                        }

                        p.Kill();
                        p.WaitForExit(1000);
                        Console.WriteLine($"{processName} 차단 완료!");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"오류 발생: {ex.Message}");
                    }
                    finally
                    {
                        p?.Dispose();
                    }
                }
            }
        }

        private void btnCategorySettings_Click(object sender, EventArgs e)
        {
            using (CategorySettingsForm categorySettingsForm = new CategorySettingsForm(currentBlockedItems, this))
            {
                categorySettingsForm.ShowDialog(this);
                LoadBlockProfiles();
            }
        }

        private void btnManageBlockedApps_Click(object sender, EventArgs e)
        {
            using (BlockedAppsManagementForm manageBlockedAppsForm = new BlockedAppsManagementForm(currentBlockedItems))
            {
                if (manageBlockedAppsForm.ShowDialog(this) == DialogResult.OK)
                {
                    currentBlockedItems = manageBlockedAppsForm.GetUpdatedBlockedItems();
                    DataModel.UpdateBlockProfiles(currentBlockedItems);
                }
            }
        }

        private void btnStudyPlan_Click(object sender, EventArgs e)
        {
            using (StudyPlanForm studyPlanForm = new StudyPlanForm())
            {
                studyPlanForm.ShowDialog(this);

                if (studyPlanForm.FocusSessionStarted)
                {
                    if (!blockingtimer.Enabled)
                    {
                        blockingtimer.Start();
                    }

                    UpdateBlockingUi();
                    AlertDialog.Show(this, "계획 기반 집중 세션을 시작했습니다.");
                }
            }
        }

        private void btnActivateBlocking_Click(object sender, EventArgs e)
        {
            if (DataModel.IsBlockingActive)
            {
                // 집중모드 활성화 상태에서 버튼 클릭 시
                if (DataModel.IsBreakActive)
                {
                    // 일시정지 중이라면 "집중모드 재개" 기능 수행
                    DataModel.EndLifeBreak(); // 일시정지 종료 및 집중모드 재개
                    UpdateBlockingUi();
                    AlertDialog.Show(this, "집중모드가 재개되었습니다!");
                }
                else
                {
                    // 집중모드 중이라면 "집중모드 정지" 요청 처리 (라이프 사용 또는 긴급 종료)
                    HandleFocusStopRequest();
                }
                return;
            }

            // 집중모드 비활성화 상태에서 버튼 클릭 시 (새 집중모드 시작)
            if (DataModel.IsEmergencyLockedOut)
            {
                AlertDialog.Show(this, $"라이프를 모두 소진해 {DataModel.EmergencyLockUntil:yyyy-MM-dd HH:mm}까지 집중모드를 다시 시작할 수 없습니다.");
                UpdateBlockingUi();
                return;
            }

            int totalMinutes;
            if (!TryGetFocusDuration(cmbHour.Text, cmbMin.Text, out totalMinutes, true))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(DataModel.CurrentFocusGoal))
            {
                DataModel.CurrentFocusGoal = "집중 세션";
            }

            if (string.IsNullOrWhiteSpace(DataModel.CurrentFocusCategory))
            {
                DataModel.CurrentFocusCategory = "직접 시작";
            }

            StartFocusSession(totalMinutes, "차단이 시작되었습니다!");
        }

        public bool PromptAndStartFocusSessionFromCategory()
        {
            if (DataModel.IsBlockingActive)
            {
                AlertDialog.Show(this, "이미 집중모드가 실행 중입니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (DataModel.IsEmergencyLockedOut)
            {
                AlertDialog.Show(this, $"라이프를 모두 소진해 {DataModel.EmergencyLockUntil:yyyy-MM-dd HH:mm}까지 집중모드를 다시 시작할 수 없습니다.");
                UpdateBlockingUi();
                return false;
            }

            int totalMinutes;
            if (!ShowCategoryFocusStartDialog(out totalMinutes))
            {
                return false;
            }

            StartFocusSession(totalMinutes, "차단이 시작되었습니다!");
            return true;
        }

        private bool ShowCategoryFocusStartDialog(out int totalMinutes)
        {
            totalMinutes = 0;

            using (Form dialog = new Form())
            using (Label promptLabel = new Label())
            using (Label hourLabel = new Label())
            using (ComboBox hourBox = new ComboBox())
            using (Label minLabel = new Label())
            using (ComboBox minBox = new ComboBox())
            using (Button startButton = new Button())
            using (Button cancelButton = new Button())
            {
                dialog.Text = "집중 세션 시작";
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.BackColor = Color.FromArgb(18, 18, 18);
                dialog.ForeColor = Color.White;
                dialog.ClientSize = new Size(430, 205);

                promptLabel.Text = "집중세션을 시작하시겠습니까?";
                promptLabel.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
                promptLabel.ForeColor = Color.White;
                promptLabel.Location = new Point(20, 20);
                promptLabel.Size = new Size(390, 34);

                hourLabel.Text = "시간";
                hourLabel.ForeColor = Color.White;
                hourLabel.Location = new Point(22, 76);
                hourLabel.Size = new Size(58, 26);
                hourLabel.TextAlign = ContentAlignment.MiddleLeft;

                hourBox.DropDownStyle = ComboBoxStyle.DropDownList;
                hourBox.BackColor = Color.FromArgb(35, 35, 35);
                hourBox.ForeColor = Color.White;
                hourBox.Location = new Point(82, 76);
                hourBox.Size = new Size(100, 26);

                minLabel.Text = "분";
                minLabel.ForeColor = Color.White;
                minLabel.Location = new Point(212, 76);
                minLabel.Size = new Size(42, 26);
                minLabel.TextAlign = ContentAlignment.MiddleLeft;

                minBox.DropDownStyle = ComboBoxStyle.DropDownList;
                minBox.BackColor = Color.FromArgb(35, 35, 35);
                minBox.ForeColor = Color.White;
                minBox.Location = new Point(258, 76);
                minBox.Size = new Size(100, 26);

                for (int i = 0; i <= 23; i++)
                {
                    hourBox.Items.Add(i.ToString());
                }

                for (int i = 0; i <= 59; i++)
                {
                    minBox.Items.Add(i.ToString("D2"));
                }

                ConfigureTimePickerDropDown(hourBox);
                ConfigureTimePickerDropDown(minBox);
                SelectTimePickerValue(hourBox, cmbHour.Text, "1");
                SelectTimePickerValue(minBox, cmbMin.Text, "00");

                startButton.Text = "시작";
                startButton.DialogResult = DialogResult.OK;
                startButton.BackColor = Color.FromArgb(139, 92, 246);
                startButton.FlatAppearance.BorderSize = 0;
                startButton.FlatStyle = FlatStyle.Flat;
                startButton.Font = new Font("맑은 고딕", 9F, FontStyle.Bold);
                startButton.ForeColor = Color.White;
                startButton.Location = new Point(226, 143);
                startButton.Size = new Size(88, 38);

                cancelButton.Text = "취소";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Location = new Point(322, 143);
                cancelButton.Size = new Size(88, 38);
                AlertDialog.StyleButton(cancelButton, false, true);

                dialog.Controls.Add(promptLabel);
                dialog.Controls.Add(hourLabel);
                dialog.Controls.Add(hourBox);
                dialog.Controls.Add(minLabel);
                dialog.Controls.Add(minBox);
                dialog.Controls.Add(startButton);
                dialog.Controls.Add(cancelButton);
                dialog.AcceptButton = startButton;
                dialog.CancelButton = cancelButton;

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return false;
                }

                if (!TryGetFocusDuration(hourBox.Text, minBox.Text, out totalMinutes, true))
                {
                    return false;
                }

                SelectTimePickerValue(cmbHour, NormalizeHourText(hourBox.Text), "1");
                SelectTimePickerValue(cmbMin, NormalizeMinuteText(minBox.Text), "00");
                return true;
            }
        }

        private void StartFocusSession(int totalMinutes, string successMessage)
        {
            DataModel.ClearCurrentFocusTasks();
            DataModel.StartFocusSession(DateTime.Now.AddMinutes(totalMinutes));

            if (!blockingtimer.Enabled)
            {
                blockingtimer.Start();
            }

            UpdateBlockingUi();
            AlertDialog.Show(this, successMessage);
        }

        private void blockingtimer_Tick(object sender, EventArgs e)
        {
            if (!DataModel.IsBlockingActive)
            {
                return;
            }

            FocusSessionTelemetry.CaptureTick();

            if (DataModel.IsBreakActive)
            {
                UpdateBreakState();
                return;
            }

            if (DataModel.SkipNextFocusEndCheck)
            {
                DataModel.SkipNextFocusEndCheck = false;
                lblShowTimeLeft.Text = FormatTimeSpan(DataModel.FocusEndTime - DateTime.Now);
            }
            else if (DateTime.Now >= DataModel.FocusEndTime)
            {
                CompleteCurrentFocusSession("정해진 집중 시간이 끝났습니다! 차단이 해제됩니다.");
                return;
            }
            else if (DataModel.Life == 0 && DataModel.IsEmergencyLockedOut)
            {
                CompleteCurrentFocusSession($"라이프를 모두 소진했습니다. {DataModel.EmergencyLockUntil:yyyy-MM-dd HH:mm}까지 집중모드를 다시 시작할 수 없습니다.");
                return;
            }
            else
            {
                lblShowTimeLeft.Text = FormatTimeSpan(DataModel.FocusEndTime - DateTime.Now);
            }

            EnforceBlockingRules();
        }

        private void UpdateBreakState()
        {
            if (DateTime.Now >= DataModel.BreakEndTime)
            {
                DataModel.EndLifeBreak();
                UpdateBlockingUi();
                AlertDialog.Show(this, "일시정지가 종료되었습니다. 차단을 다시 시작합니다.");
                return;
            }

            TimeSpan breakLeft = DataModel.BreakEndTime - DateTime.Now;
            lblShowTimeLeft.Text = FormatTimeSpan(DataModel.PausedFocusRemainingTime);
            label4.Text = FormatPauseStatusText(breakLeft);
            ApplyFocusPanelVisualState();
        }

        private void CompleteCurrentFocusSession(string message)
        {
            blockingtimer.Stop();
            DataModel.CompleteFocusSession();
            lblShowTimeLeft.Text = "00시간 00분 00초";
            UpdateBlockingUi();
            ShowLastSessionReport();
            AlertDialog.Show(this, message);
        }

        private void EnforceBlockingRules()
        {
            KillProcesses(BuildBlockedProcessNames());
            KillWebBrowser(DataModel.SavedWebBlockKeywordList);
        }

        private List<string> BuildBlockedProcessNames()
        {
            HashSet<string> processNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (DataModel.SavedBlockList != null)
            {
                foreach (string item in DataModel.SavedBlockList)
                {
                    if (string.IsNullOrWhiteSpace(item))
                    {
                        continue;
                    }

                    if (BlockedProcessAliases.TryGetValue(item, out string processName))
                    {
                        processNames.Add(processName);
                    }
                    else
                    {
                        processNames.Add(item);
                    }
                }
            }

            processNames.Add("taskmgr");
            return processNames.ToList();
        }

        private void KillWebBrowser(List<string> keywords)
        {
            if (keywords == null)
            {
                return;
            }

            List<string> normalizedKeywords = keywords
                .Where(keyword => !string.IsNullOrWhiteSpace(keyword))
                .Select(keyword => keyword.ToLower())
                .Distinct()
                .ToList();

            if (normalizedKeywords.Count == 0)
            {
                return;
            }

            Process[] allProcesses = Process.GetProcesses();

            foreach (Process p in allProcesses)
            {
                try
                {
                    if (!BrowserProcessNames.Contains(p.ProcessName) || string.IsNullOrEmpty(p.MainWindowTitle))
                    {
                        continue;
                    }

                    string windowTitle = p.MainWindowTitle.ToLower();

                    if (normalizedKeywords.Any(keyword => windowTitle.Contains(keyword)))
                    {
                        p.Kill();
                        p.WaitForExit(1000);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"브라우저 종료 실패: {ex.Message}");
                }
                finally
                {
                    p.Dispose();
                }
            }
        }
        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan < TimeSpan.Zero)
            {
                timeSpan = TimeSpan.Zero;
            }

            return string.Format("{0}시간 {1:D2}분 {2:D2}초", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
        }

        private string FormatPauseStatusText(TimeSpan pauseLeft)
        {
            if (pauseLeft < TimeSpan.Zero)
            {
                pauseLeft = TimeSpan.Zero;
            }

            return string.Format("일시정지 상태입니다. 재개까지 남은 시간 : {0}분 {1:D2}초 / 남은 라이프: {2}개",
                (int)pauseLeft.TotalMinutes,
                pauseLeft.Seconds,
                DataModel.Life);
        }

        private void ApplyFocusPanelVisualState()
        {
            Color panelColor = DataModel.IsBlockingActive && DataModel.IsBreakActive
                ? PausePanelBackColor
                : FocusPanelBackColor;

            guna2Panel1.BackColor = panelColor;
            guna2Panel1.FillColor = panelColor;
            label4.BackColor = panelColor;
            label7.BackColor = panelColor;
        }

        private void btnStopBlocking_Click(object sender, EventArgs e)
        {
            DataModel.CompleteFocusSession();

            if (blockingtimer.Enabled)
            {
                blockingtimer.Stop();
            }

            lblShowTimeLeft.Text = "00시간 00분 00초";
            UpdateBlockingUi();
            ShowLastSessionReport();
            AlertDialog.Show(this, "개발용 정지 버튼으로 차단이 종료되었습니다!");
        }

        private void ShowLastSessionReport()
        {
            FocusSessionRecord session = FocusSessionTelemetry.LastCompletedSession;
            if (session == null)
            {
                return;
            }

            using (FocusSessionReportForm reportForm = new FocusSessionReportForm(session))
            {
                reportForm.ShowDialog(this);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            DataModel.LoadFromJson();
            LoadBlockProfiles();

            InitializeTimeComboBoxes();

            cmbHour.TextChanged += ComboBox_TextChanged;
            cmbMin.TextChanged += ComboBox_TextChanged;

            if (DataModel.IsBlockingActive && !blockingtimer.Enabled)
            {
                blockingtimer.Start();
            }

            UpdateBlockingUi();
        }

        private void InitializeTimeComboBoxes()
        {
            cmbHour.Items.Clear();
            for (int i = 0; i <= 23; i++)
            {
                cmbHour.Items.Add(i.ToString());
            }

            cmbMin.Items.Clear();
            for (int i = 0; i <= 59; i++)
            {
                cmbMin.Items.Add(i.ToString("D2"));
            }

            cmbHour.SelectedItem = "1";
            cmbMin.SelectedItem = "00";

            ConfigureTimePickerDropDown(cmbHour);
            ConfigureTimePickerDropDown(cmbMin);
        }

        private void ConfigureTimePickerDropDown(ComboBox comboBox)
        {
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.IntegralHeight = false;
            comboBox.MaxDropDownItems = TimePickerDropDownVisibleItems;
            comboBox.DropDownHeight = comboBox.ItemHeight * TimePickerDropDownVisibleItems;
        }

        private void SelectTimePickerValue(ComboBox comboBox, string desiredValue, string fallbackValue)
        {
            string selectedValue = comboBox.Items.Contains(desiredValue) ? desiredValue : fallbackValue;
            if (comboBox.Items.Contains(selectedValue))
            {
                comboBox.SelectedItem = selectedValue;
            }
        }

        private bool TryGetFocusDuration(string hourText, string minuteText, out int totalMinutes, bool showMessage)
        {
            totalMinutes = 0;

            if (!TryParseHour(hourText, out int hours))
            {
                if (showMessage)
                {
                    AlertDialog.Show(this, "시간은 0 이상 23 이하의 숫자로 입력해 주세요.");
                }

                return false;
            }

            if (!TryParseMinute(minuteText, out int minutes))
            {
                if (showMessage)
                {
                    AlertDialog.Show(this, "분은 0 이상 59 이하의 숫자로 입력해 주세요.");
                }

                return false;
            }

            totalMinutes = (hours * 60) + minutes;
            if (totalMinutes <= 0)
            {
                if (showMessage)
                {
                    AlertDialog.Show(this, "집중 시간은 1분 이상으로 설정해 주세요.");
                }

                return false;
            }

            return true;
        }

        private bool TryParseHour(string value, out int hours)
        {
            return int.TryParse((value ?? string.Empty).Trim(), out hours) &&
                   hours >= 0 &&
                   hours <= 23;
        }

        private bool TryParseMinute(string value, out int minutes)
        {
            return int.TryParse((value ?? string.Empty).Trim(), out minutes) &&
                   minutes >= 0 &&
                   minutes <= 59;
        }

        private string NormalizeHourText(string value)
        {
            return int.TryParse((value ?? string.Empty).Trim(), out int hours)
                ? hours.ToString()
                : "0";
        }

        private string NormalizeMinuteText(string value)
        {
            return int.TryParse((value ?? string.Empty).Trim(), out int minutes)
                ? minutes.ToString("D2")
                : "00";
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DataModel.IsBlockingActive)
            {
                e.Cancel = true;
                AlertDialog.Show(this, "집중모드가 실행 중입니다. 종료하려면 먼저 집중모드를 정지해 주세요.");
            }
            else
            {
                DataModel.SaveToJson();
            }
        }

        private void HandleFocusStopRequest()
        {
            if (DataModel.Life == 0)
            {
                AlertDialog.Show(this, "남은 라이프가 없습니다. 집중 시간이 끝날 때까지 차단이 유지됩니다.");
                return;
            }

            StopRequestAction action = ShowStopRequestDialog();

            if (action == StopRequestAction.LifeBreak)
            {
                if (!DataModel.StartLifeBreak()) // 라이프 사용 및 일시정지 시작
                {
                    AlertDialog.Show(this, "남은 라이프가 없습니다.");
                    return;
                }

                UpdateBlockingUi(); // UI 업데이트 (버튼 텍스트 변경 등)
                AlertDialog.Show(this, $"일시정지를 시작했습니다. {DataModel.LIFE_BREAK_MINUTES}분 후 집중모드가 재개됩니다.");
            }
            else if (action == StopRequestAction.EmergencyStop)
            {
                ConfirmEmergencyStop();
            }
        }

        private StopRequestAction ShowStopRequestDialog()
        {
            StopRequestAction selectedAction = StopRequestAction.Cancel;
            int secondsLeft = DataModel.STOP_COUNTDOWN_SECONDS;
            List<DataModel.FocusTaskProgress> focusTasks = DataModel.GetCurrentFocusTasksCopy();
            bool showEmergencyButton = DataModel.Life > 0 && DataModel.Life <= 1;

            Form dialog = new Form();
            Label descriptionLabel = new Label();
            Label progressTitleLabel = new Label();
            CheckedListBox taskList = new CheckedListBox();
            Label emptyTaskLabel = new Label();
            Button useLifeButton = new Button();
            Button cancelButton = new Button();
            Button emergencyButton = null;
            Timer countdownTimer = new Timer();

            AlertDialog.ApplyDialogTheme(dialog);
            dialog.Text = "집중모드 정지";
            dialog.ClientSize = new Size(560, showEmergencyButton ? 395 : 360);

            descriptionLabel.Location = new Point(18, 18);
            descriptionLabel.Size = new Size(524, 82);
            descriptionLabel.Text = BuildStopDialogMessage(secondsLeft);
            descriptionLabel.BackColor = AlertDialog.AppBackColor;
            descriptionLabel.ForeColor = AlertDialog.SubtleTextColor;
            descriptionLabel.Font = new Font("맑은 고딕", 10F, FontStyle.Regular);

            progressTitleLabel.Location = new Point(18, 112);
            progressTitleLabel.Size = new Size(524, 24);
            progressTitleLabel.Text = "현재 세션 진행상황";
            progressTitleLabel.BackColor = AlertDialog.AppBackColor;
            progressTitleLabel.ForeColor = AlertDialog.TextColor;
            progressTitleLabel.Font = new Font("맑은 고딕", 10F, FontStyle.Bold);

            taskList.CheckOnClick = true;
            taskList.BackColor = AlertDialog.FieldColor;
            taskList.BorderStyle = BorderStyle.FixedSingle;
            taskList.ForeColor = AlertDialog.TextColor;
            taskList.Font = new Font("맑은 고딕", 9F, FontStyle.Regular);
            taskList.HorizontalScrollbar = true;
            taskList.Location = new Point(18, 142);
            taskList.Size = new Size(524, showEmergencyButton ? 178 : 143);

            foreach (DataModel.FocusTaskProgress task in focusTasks)
            {
                taskList.Items.Add(task.Text, task.IsCompleted);
            }

            taskList.ItemCheck += delegate(object sender, ItemCheckEventArgs e)
            {
                if (e.Index < 0 || e.Index >= focusTasks.Count)
                {
                    return;
                }

                focusTasks[e.Index].IsCompleted = e.NewValue == CheckState.Checked;
                SaveFocusTaskProgress(focusTasks);
            };

            emptyTaskLabel.Location = new Point(18, 142);
            emptyTaskLabel.Size = new Size(524, 70);
            emptyTaskLabel.Text = "현재 세션에 등록된 체크박스 태스크가 없습니다.\r\n학습 계획에 - [ ] 형식으로 태스크를 작성하면 여기에 표시됩니다.";
            emptyTaskLabel.BackColor = AlertDialog.FieldColor;
            emptyTaskLabel.ForeColor = AlertDialog.SubtleTextColor;
            emptyTaskLabel.Font = new Font("맑은 고딕", 9F, FontStyle.Regular);
            emptyTaskLabel.Padding = new Padding(10);

            int buttonY = dialog.ClientSize.Height - 56;

            useLifeButton.Location = new Point(18, buttonY);
            useLifeButton.Size = new Size(150, 38);
            useLifeButton.Enabled = false;
            useLifeButton.Text = $"대기 중 {secondsLeft}초";
            AlertDialog.StyleButton(useLifeButton, true);
            useLifeButton.Click += delegate
            {
                selectedAction = StopRequestAction.LifeBreak;
                dialog.Close();
            };

            cancelButton.Location = new Point(dialog.ClientSize.Width - 138, buttonY);
            cancelButton.Size = new Size(120, 38);
            cancelButton.Text = "취소";
            AlertDialog.StyleButton(cancelButton, false, true);
            cancelButton.Click += delegate
            {
                selectedAction = StopRequestAction.Cancel;
                dialog.Close();
            };

            dialog.Controls.Add(descriptionLabel);
            dialog.Controls.Add(progressTitleLabel);
            if (focusTasks.Count > 0)
            {
                dialog.Controls.Add(taskList);
            }
            else
            {
                dialog.Controls.Add(emptyTaskLabel);
            }

            dialog.Controls.Add(useLifeButton);
            dialog.Controls.Add(cancelButton);

            if (showEmergencyButton)
            {
                emergencyButton = new Button();
                emergencyButton.Location = new Point(184, buttonY);
                emergencyButton.Size = new Size(142, 38);
                emergencyButton.Text = "긴급 종료";
                AlertDialog.StyleButton(emergencyButton, false, true);
                emergencyButton.Click += delegate
                {
                    selectedAction = StopRequestAction.EmergencyStop;
                    dialog.Close();
                };
                dialog.Controls.Add(emergencyButton);
            }

            countdownTimer.Interval = 1000;
            countdownTimer.Tick += delegate
            {
                secondsLeft--;

                if (secondsLeft <= 0)
                {
                    countdownTimer.Stop();
                    useLifeButton.Enabled = true;
                    useLifeButton.Text = $"{DataModel.LIFE_BREAK_MINUTES}분 일시정지";
                    descriptionLabel.Text = BuildStopDialogMessage(0);
                    return;
                }

                useLifeButton.Text = $"대기 중 {secondsLeft}초";
                descriptionLabel.Text = BuildStopDialogMessage(secondsLeft);
            };

            dialog.Shown += delegate
            {
                countdownTimer.Start();
            };

            dialog.FormClosed += delegate
            {
                countdownTimer.Stop();
                countdownTimer.Dispose();
                dialog.Dispose();
            };

            dialog.ShowDialog(this);
            return selectedAction;
        }

        private string BuildStopDialogMessage(int secondsLeft)
        {
            string message = $"라이프 1개를 사용하면 {DataModel.LIFE_BREAK_MINUTES}분 동안 집중모드가 일시정지됩니다.\r\n" +
                             $"오늘 남은 라이프: {DataModel.Life}개\r\n";

            if (secondsLeft > 0)
            {
                message += "정말 해제하시겠습니까?";
            }
            else
            {
                message += "정말 해제하시겠습니까?\r\n이제 라이프를 사용할 수 있습니다.";
            }

            // 마지막 라이프가 1개일 때만 "마지막 라이프입니다" 메시지 추가
            if (DataModel.Life == 1)
            {
                message += "\r\n마지막 라이프입니다. 긴급 종료는 대기 없이 선택할 수 있습니다.";
            }

            return message;
        }

        private void SaveFocusTaskProgress(List<DataModel.FocusTaskProgress> focusTasks)
        {
            DataModel.SetCurrentFocusTasks(focusTasks);
            UpdateFocusTaskSourceFiles(focusTasks);
        }

        private void UpdateFocusTaskSourceFiles(IEnumerable<DataModel.FocusTaskProgress> focusTasks)
        {
            if (focusTasks == null)
            {
                return;
            }

            foreach (IGrouping<string, DataModel.FocusTaskProgress> group in focusTasks
                         .Where(task => task != null &&
                                        !string.IsNullOrWhiteSpace(task.SourceFilePath) &&
                                        task.SourceLineIndex >= 0)
                         .GroupBy(task => task.SourceFilePath, StringComparer.OrdinalIgnoreCase))
            {
                try
                {
                    if (!File.Exists(group.Key))
                    {
                        continue;
                    }

                    string[] lines = File.ReadAllLines(group.Key);
                    bool changed = false;

                    foreach (DataModel.FocusTaskProgress task in group)
                    {
                        if (task.SourceLineIndex < 0 || task.SourceLineIndex >= lines.Length)
                        {
                            continue;
                        }

                        string replacement = "${1}" + (task.IsCompleted ? "x" : " ") + "${3}";
                        string updatedLine = Regex.Replace(
                            lines[task.SourceLineIndex],
                            @"^(\s*[-*+]\s+\[)( |x|X)(\]\s+)",
                            replacement,
                            RegexOptions.None,
                            TimeSpan.FromMilliseconds(100));

                        if (!string.Equals(lines[task.SourceLineIndex], updatedLine, StringComparison.Ordinal))
                        {
                            lines[task.SourceLineIndex] = updatedLine;
                            changed = true;
                        }
                    }

                    if (changed)
                    {
                        File.WriteAllLines(group.Key, lines);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("태스크 진행 상태 저장 실패: " + ex.Message);
                }
            }
        }

        private void ConfirmEmergencyStop()
        {
            DialogResult result = AlertDialog.Show(
                this,
                "긴급 종료 시 현재 집중세션이 완전히 해제됩니다.\r\n이후 남은 시간 동안 이 앱에서 집중모드를 다시 사용할 수 없습니다.\r\n그래도 긴급 종료하시겠습니까?",
                "긴급 종료 확인",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning);

            if (result != DialogResult.OK)
            {
                return;
            }

            if (!DataModel.EmergencyStopFocusSession()) // EmergencyStopFocusSession()은 이제 Life 감소 로직을 포함
            {
                AlertDialog.Show(this, "남은 라이프가 없습니다."); // 이 경우는 발생하지 않을 것으로 예상 (HandleFocusStopRequest에서 Life==0이면 이미 차단)
                return;
            }

            if (blockingtimer.Enabled)
            {
                blockingtimer.Stop();
            }

            lblShowTimeLeft.Text = "00시간 00분 00초";
            UpdateBlockingUi();
            ShowLastSessionReport();
            AlertDialog.Show(this, $"긴급 종료되었습니다. {DataModel.EmergencyLockUntil:yyyy-MM-dd HH:mm}까지 집중모드를 다시 시작할 수 없습니다.");
        }

        private void ComboBox_TextChanged(object sender, EventArgs e)
        {
            UpdateMainActionButtonEnabled();
        }

        private void UpdateBlockingUi()
        {
            btnStopBlocking.Text = "집중모드 정지(개발용)";
            cmbHour.Enabled = !DataModel.IsBlockingActive;
            cmbMin.Enabled = !DataModel.IsBlockingActive;
            ApplyFocusPanelVisualState();

            if (DataModel.IsBlockingActive)
            {
                if (DataModel.IsBreakActive)
                {
                    btnActivateBlocking.Text = "집중모드 재개";
                    label4.Text = FormatPauseStatusText(DataModel.BreakEndTime - DateTime.Now);
                }
                else
                {
                    btnActivateBlocking.Text = "집중모드 정지"; // 변경: 집중모드 중에는 '집중모드 정지'
                    label4.Text = $"집중모드 실행 중입니다. 남은 라이프: {DataModel.Life}개";
                }
            }
            else
            {
                btnActivateBlocking.Text = "집중모드 활성화";

                if (DataModel.IsEmergencyLockedOut)
                {
                    label4.Text = $"라이프 소진 상태입니다. {DataModel.EmergencyLockUntil:yyyy-MM-dd HH:mm}까지 집중모드를 다시 시작할 수 없습니다.";
                }
                else
                {
                    label4.Text = $"오늘 남은 라이프: {DataModel.Life}개 / 오른쪽 정지 버튼은 개발용입니다.";
                }
            }

            UpdateMainActionButtonEnabled();
        }

        private void UpdateMainActionButtonEnabled()
        {
            if (DataModel.IsBlockingActive)
            {
                btnActivateBlocking.Enabled = true; // 집중모드 중에는 정지/재개 버튼 항상 활성화
                return;
            }

            // 집중모드 비활성화 상태에서는 시간/분 입력 여부로 활성화 결정
            bool hasHour = !string.IsNullOrWhiteSpace(cmbHour.Text);
            bool hasMin = !string.IsNullOrWhiteSpace(cmbMin.Text);

            bool isDurationValid = hasHour && hasMin && TryGetFocusDuration(cmbHour.Text, cmbMin.Text, out _, false);
            btnActivateBlocking.Enabled = isDurationValid && !DataModel.IsEmergencyLockedOut;
        }

        public void SetCurrentCategory(string category)
        {
            label7.Text = $"현재 모드 : {category}";
        }
    }
}
