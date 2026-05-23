using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics.Eventing.Reader; // 프로세스 제어를 위한 필수 네임스페이스
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

        private Dictionary<string, List<string>> currentBlockedItems;

        public MainForm()
        {
            InitializeComponent();
            LoadBlockProfiles();
        }

        private void LoadBlockProfiles()
        {
            currentBlockedItems = DataModel.GetBlockProfilesCopy();
        }

        public void KillProcesses(List<string> blockList)
        {
            foreach (string processName in blockList)
            {
                Process[] processes = Process.GetProcessesByName(processName);

                foreach (Process p in processes)
                {
                    try
                    {
                        if (p.Id == Process.GetCurrentProcess().Id)
                        {
                            continue;
                        }

                        p.Kill();
                        p.WaitForExit();
                        FocusSessionTelemetry.RegisterBlockedProcessAttempt(processName);
                        Console.WriteLine($"{processName} 차단 완료!");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"오류 발생: {ex.Message}");
                    }
                    finally
                    {
                        p.Dispose();
                    }
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (DataModel.IsBlockingActive)
            {
                MessageBox.Show("집중모드가 실행 중입니다. 종료하려면 먼저 집중모드를 정지해 주세요.");
            }
            else
            {
                this.Close();
            }
        }

        private void btnCategorySettings_Click(object sender, EventArgs e)
        {
            using (CategorySettingsForm categorySettingsForm = new CategorySettingsForm(currentBlockedItems))
            {
                categorySettingsForm.ShowDialog(this);
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
                    MessageBox.Show("계획 기반 집중 세션을 시작했습니다.");
                }
            }
        }

        private void btnActivateBlocking_Click(object sender, EventArgs e)
        {
            if (DataModel.IsBlockingActive)
            {
                HandleFocusStopRequest();
                return;
            }

            if (DataModel.IsEmergencyLockedOut)
            {
                MessageBox.Show($"라이프를 모두 소진해 {DataModel.EmergencyLockUntil:yyyy-MM-dd HH:mm}까지 집중모드를 다시 시작할 수 없습니다.");
                UpdateBlockingUi();
                return;
            }

            string hourInput = cmbHour.Text;
            string minInput = cmbMin.Text;

            int hours = 0;
            int minutes = 0;

            if (!string.IsNullOrEmpty(hourInput) && !int.TryParse(hourInput, out hours))
            {
                MessageBox.Show("시간에 올바른 숫자를 입력해 주세요!");
                return;
            }

            if (!string.IsNullOrEmpty(minInput) && !int.TryParse(minInput, out minutes))
            {
                MessageBox.Show("분에 올바른 숫자를 입력해 주세요!");
                return;
            }

            int totalMinutes = (hours * 60) + minutes;
            if (totalMinutes <= 0)
            {
                MessageBox.Show("집중 시간은 1분 이상으로 설정해 주세요.");
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

            DataModel.StartFocusSession(DateTime.Now.AddMinutes(totalMinutes));

            if (!blockingtimer.Enabled)
            {
                blockingtimer.Start();
            }

            UpdateBlockingUi();
            MessageBox.Show("차단이 시작되었습니다!");
        }

        private void blockingtimer_Tick(object sender, EventArgs e)
        {
            if (!DataModel.IsBlockingActive)
            {
                return;
            }

            FocusSessionTelemetry.CaptureTick();

            if (DateTime.Now >= DataModel.FocusEndTime)
            {
                blockingtimer.Stop();
                DataModel.CompleteFocusSession("Completed");
                lblShowTimeLeft.Text = "00시간 00분 00초";
                UpdateBlockingUi();
                ShowLastSessionReport();
                MessageBox.Show("정해진 집중 시간이 끝났습니다! 차단이 해제됩니다.");
                return;
            }

            if (DataModel.IsBreakActive)
            {
                if (DateTime.Now >= DataModel.BreakEndTime)
                {
                    DataModel.EndLifeBreak();
                    UpdateBlockingUi();

                    if (DataModel.Life == 0 && DataModel.IsEmergencyLockedOut)
                    {
                        blockingtimer.Stop();
                        DataModel.CompleteFocusSession("Life exhausted");
                        lblShowTimeLeft.Text = "00시간 00분 00초";
                        UpdateBlockingUi();
                        ShowLastSessionReport();
                        MessageBox.Show($"라이프를 모두 소진했습니다. {DataModel.EmergencyLockUntil:yyyy-MM-dd HH:mm}까지 집중모드를 다시 사용할 수 없습니다.");
                        return;
                    }

                    MessageBox.Show("자유시간이 종료되었습니다. 차단을 다시 시작합니다.");
                }
                else
                {
                    TimeSpan breakLeft = DataModel.BreakEndTime - DateTime.Now;
                    TimeSpan pausedFocusLeft = DataModel.FocusEndTime - DataModel.BreakEndTime;
                    lblShowTimeLeft.Text = FormatTimeSpan(pausedFocusLeft);
                    label4.Text = string.Format("자유시간 중입니다. 남은 자유시간: {0:D2}분 {1:D2}초 / 남은 라이프: {2}개",
                        breakLeft.Minutes,
                        breakLeft.Seconds,
                        DataModel.Life);
                    return;
                }
            }

            if (DataModel.Life == 0 && DataModel.IsEmergencyLockedOut)
            {
                blockingtimer.Stop();
                DataModel.CompleteFocusSession("Life exhausted");
                lblShowTimeLeft.Text = "00시간 00분 00초";
                UpdateBlockingUi();
                ShowLastSessionReport();
                MessageBox.Show($"라이프를 모두 소진했습니다. {DataModel.EmergencyLockUntil:yyyy-MM-dd HH:mm}까지 집중모드를 다시 사용할 수 없습니다.");
                return;
            }

            TimeSpan timeLeft = DataModel.FocusEndTime - DateTime.Now;
            lblShowTimeLeft.Text = FormatTimeSpan(timeLeft);

            List<string> finalBlockList = new List<string>(DataModel.SavedBlockList);

            if (!finalBlockList.Contains("taskmgr"))
            {
                finalBlockList.Add("taskmgr");
            }

            KillProcesses(finalBlockList);
        }

        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan < TimeSpan.Zero)
            {
                timeSpan = TimeSpan.Zero;
            }

            return string.Format("{0}시간 {1:D2}분 {2:D2}초", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
        }

        private void btnStopBlocking_Click(object sender, EventArgs e)
        {
            DataModel.CompleteFocusSession("Stopped manually");

            if (blockingtimer.Enabled)
            {
                blockingtimer.Stop();
            }

            lblShowTimeLeft.Text = "00시간 00분 00초";
            UpdateBlockingUi();
            ShowLastSessionReport();
            MessageBox.Show("개발용 정지 버튼으로 차단이 종료되었습니다!");
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
            btnActivateBlocking.Enabled = false;
            btnStopBlocking.Text = "집중모드 정지(개발용)";
            cmbHour.TextChanged += ComboBox_TextChanged;
            cmbMin.TextChanged += ComboBox_TextChanged;
            UpdateBlockingUi();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DataModel.IsBlockingActive)
            {
                e.Cancel = true;
                MessageBox.Show("집중모드가 실행 중입니다. 종료하려면 먼저 집중모드를 정지해 주세요.");
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
                MessageBox.Show("남은 라이프가 없습니다. 집중 시간이 끝날 때까지 차단이 유지됩니다.");
                return;
            }

            StopRequestAction action = ShowStopRequestDialog();

            if (action == StopRequestAction.LifeBreak)
            {
                if (!DataModel.StartLifeBreak())
                {
                    MessageBox.Show("남은 라이프가 없습니다.");
                    return;
                }

                UpdateBlockingUi();
                MessageBox.Show($"라이프 1개를 사용했습니다. {DataModel.LIFE_BREAK_MINUTES}분 동안 앱 차단이 해제됩니다.");
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

            Form dialog = new Form();
            Label descriptionLabel = new Label();
            Button useLifeButton = new Button();
            Button cancelButton = new Button();
            Button emergencyButton = null;
            Timer countdownTimer = new Timer();

            dialog.Text = "집중모드 정지";
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            dialog.MaximizeBox = false;
            dialog.MinimizeBox = false;
            dialog.ClientSize = DataModel.Life == 1 ? new Size(460, 215) : new Size(460, 180);

            descriptionLabel.Location = new Point(18, 18);
            descriptionLabel.Size = new Size(424, 92);
            descriptionLabel.Text = BuildStopDialogMessage(secondsLeft);

            useLifeButton.Location = new Point(18, DataModel.Life == 1 ? 130 : 120);
            useLifeButton.Size = new Size(150, 38);
            useLifeButton.Enabled = false;
            useLifeButton.Text = $"대기 중 {secondsLeft}초";
            useLifeButton.Click += delegate
            {
                selectedAction = StopRequestAction.LifeBreak;
                dialog.Close();
            };

            cancelButton.Location = new Point(DataModel.Life == 1 ? 322 : 292, DataModel.Life == 1 ? 130 : 120);
            cancelButton.Size = new Size(120, 38);
            cancelButton.Text = "취소";
            cancelButton.Click += delegate
            {
                selectedAction = StopRequestAction.Cancel;
                dialog.Close();
            };

            dialog.Controls.Add(descriptionLabel);
            dialog.Controls.Add(useLifeButton);
            dialog.Controls.Add(cancelButton);

            if (DataModel.Life == 1)
            {
                emergencyButton = new Button();
                emergencyButton.Location = new Point(174, 130);
                emergencyButton.Size = new Size(142, 38);
                emergencyButton.Text = "긴급 종료";
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
                    useLifeButton.Text = $"{DataModel.LIFE_BREAK_MINUTES}분 사용";
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
            string message = $"라이프 1개를 사용하면 {DataModel.LIFE_BREAK_MINUTES}분 동안 앱 차단이 해제됩니다.\r\n" +
                             $"오늘 남은 라이프: {DataModel.Life}개\r\n";

            if (secondsLeft > 0)
            {
                message += $"충동적인 해제를 막기 위해 {secondsLeft}초 후 사용할 수 있습니다.";
            }
            else
            {
                message += "이제 라이프를 사용할 수 있습니다.";
            }

            if (DataModel.Life == 1)
            {
                message += "\r\n마지막 라이프입니다. 긴급 종료는 대기 없이 선택할 수 있습니다.";
            }

            return message;
        }

        private void ConfirmEmergencyStop()
        {
            DialogResult result = MessageBox.Show(
                "긴급 종료 시 현재 집중세션이 완전히 해제됩니다.\r\n이후 남은 시간 동안 이 앱에서 집중모드를 다시 사용할 수 없습니다.\r\n그래도 긴급 종료하시겠습니까?",
                "긴급 종료 확인",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning);

            if (result != DialogResult.OK)
            {
                return;
            }

            if (!DataModel.EmergencyStopFocusSession())
            {
                MessageBox.Show("남은 라이프가 없습니다.");
                return;
            }

            if (blockingtimer.Enabled)
            {
                blockingtimer.Stop();
            }

            lblShowTimeLeft.Text = "00시간 00분 00초";
            UpdateBlockingUi();
            ShowLastSessionReport();
            MessageBox.Show($"긴급 종료되었습니다. {DataModel.EmergencyLockUntil:yyyy-MM-dd HH:mm}까지 집중모드를 다시 시작할 수 없습니다.");
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

            if (DataModel.IsBlockingActive)
            {
                btnActivateBlocking.Text = "집중모드 정지";

                if (!DataModel.IsBreakActive)
                {
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
                btnActivateBlocking.Enabled = true;
                return;
            }

            bool hasHour = !string.IsNullOrWhiteSpace(cmbHour.Text);
            bool hasMin = !string.IsNullOrWhiteSpace(cmbMin.Text);
            btnActivateBlocking.Enabled = hasHour && hasMin && !DataModel.IsEmergencyLockedOut;
        }
    }
}
