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
                        Console.WriteLine($"{processName} 차단 완료!");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"오류 발생: {ex.Message}");
                    }
                    finally
                    {
                        if (p != null && !p.HasExited) // NEW: 프로세스가 아직 실행 중이면 Dispose() 호출 전 안전하게 종료 시도
                            {
                                try { p.Kill(); } catch { }
                            }
                        p?.Dispose();
                    }
                }
            }
        }

        /*private void btnExit_Click(object sender, EventArgs e)
        {
            if (DataModel.IsBlockingActive)
            {
                MessageBox.Show("집중모드가 실행 중입니다. 종료하려면 먼저 집중모드를 정지해 주세요.");
            }
            else
            {
                this.Close();
            }
        }*/

        private void btnCategorySettings_Click(object sender, EventArgs e)
        {
            using (CategorySettingsForm categorySettingsForm = new CategorySettingsForm(currentBlockedItems))
            {
                categorySettingsForm.ShowDialog(this);
                LoadBlockProfiles(); // MODIFIED: CategorySettingsForm에서 변경사항이 저장될 수 있으므로, 다시 로드
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
            }
        }

        private void btnActivateBlocking_Click(object sender, EventArgs e)
        {
            if (DataModel.IsBlockingActive)
            {
                // 집중모드 활성화 상태에서 버튼 클릭 시
                if (DataModel.IsBreakActive)
                {
                    // 자유시간 중이라면 "집중모드 재개" 기능 수행
                    DataModel.EndLifeBreak(); // 자유시간 종료 및 집중모드 재개
                    UpdateBlockingUi();
                    MessageBox.Show("집중모드가 재개되었습니다!");
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

            // minInput 처리 로직을 수정하여 "00"을 0으로 제대로 파싱하고, 비어있는 경우 메시지를 표시
            if (!string.IsNullOrEmpty(minInput))
            {
                if (minInput == "00")
                {
                    minutes = 0;
                }
                else if (!int.TryParse(minInput, out minutes))
                {
                    MessageBox.Show("분에 올바른 숫자를 입력해 주세요!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("분을 선택해 주세요!"); // 분이 비어있을 경우 메시지
                return;
            }


            int totalMinutes = (hours * 60) + minutes;
            if (totalMinutes <= 0)
            {
                MessageBox.Show("집중 시간은 1분 이상으로 설정해 주세요.");
                return;
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

            if (DataModel.IsBreakActive)
            {
                if (DateTime.Now >= DataModel.BreakEndTime)
                {
                    DataModel.EndLifeBreak();
                    UpdateBlockingUi();

                    if (DataModel.Life == 0 && DataModel.IsEmergencyLockedOut)
                    {
                        blockingtimer.Stop();
                        DataModel.CompleteFocusSession();
                        lblShowTimeLeft.Text = "00시간 00분 00초";
                        UpdateBlockingUi();
                        MessageBox.Show($"라이프를 모두 소진했습니다. {DataModel.EmergencyLockUntil:yyyy-MM-dd HH:mm}까지 집중모드를 다시 시작할 수 없습니다.");
                        return;
                    }

                    MessageBox.Show("자유시간이 종료되었습니다. 차단을 다시 시작합니다.");
                    return;
                }
                else
                {
                    TimeSpan breakLeft = DataModel.BreakEndTime - DateTime.Now;
                    lblShowTimeLeft.Text = FormatTimeSpan(DataModel.PausedFocusRemainingTime);
                    label4.Text = string.Format("자유시간 중입니다. 남은 자유시간: {0}분 {1:D2}초 / 남은 라이프: {2}개",
                        (int)breakLeft.TotalMinutes,
                        breakLeft.Seconds,
                        DataModel.Life);
                    return;
                }
            }
            else // 자유시간이 아닐 때 (즉, 집중모드 활성화 중)
            {
                if (DataModel.SkipNextFocusEndCheck)
                {
                    DataModel.SkipNextFocusEndCheck = false;
                    TimeSpan timeLeftForUI = DataModel.FocusEndTime - DateTime.Now;
                    lblShowTimeLeft.Text = FormatTimeSpan(timeLeftForUI);

                }
                else // 정상적인 집중모드 실행 중 (SkipNextFocusEndCheck == false)
                {
                    if (DateTime.Now >= DataModel.FocusEndTime)
                    {
                        blockingtimer.Stop();
                        DataModel.CompleteFocusSession();
                        lblShowTimeLeft.Text = "00시간 00분 00초";
                        UpdateBlockingUi();
                        MessageBox.Show("정해진 집중 시간이 끝났습니다! 차단이 해제됩니다.");
                        return;
                    }

                    if (DataModel.Life == 0 && DataModel.IsEmergencyLockedOut)
                    {
                        blockingtimer.Stop();
                        DataModel.CompleteFocusSession();
                        lblShowTimeLeft.Text = "00시간 00분 00초";
                        UpdateBlockingUi();
                        MessageBox.Show($"라이프를 모두 소진했습니다. {DataModel.EmergencyLockUntil:yyyy-MM-dd HH:mm}까지 집중모드를 다시 시작할 수 없습니다.");
                        return;
                    }

                    TimeSpan timeLeft = DataModel.FocusEndTime - DateTime.Now;
                    lblShowTimeLeft.Text = FormatTimeSpan(timeLeft);
                }

                // MODIFIED: finalBlockList 초기화 및 KillProcesses 호출 부분을 이 위치로 옮겨,
                // SkipNextFocusEndCheck 여부와 관계없이 매 틱마다 실행되도록 합니다.
                List<string> finalBlockList;
                // NEW: DataModel.SavedBlockList가 null일 경우를 방어하는 코드 추가
                if (DataModel.SavedBlockList == null)
                {
                    finalBlockList = new List<string>();
                    Debug.WriteLine("경고: DataModel.SavedBlockList가 null이어서 빈 리스트로 초기화됩니다. 이 상황은 발생해서는 안됩니다.");
                }
                else
                {
                    finalBlockList = new List<string>(DataModel.SavedBlockList);
                }


                if (!finalBlockList.Contains("taskmgr"))
                {
                    finalBlockList.Add("taskmgr");
                }

                KillProcesses(finalBlockList);
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

        private void btnStopBlocking_Click(object sender, EventArgs e)
        {
            DataModel.CompleteFocusSession();

            if (blockingtimer.Enabled)
            {
                blockingtimer.Stop();
            }

            lblShowTimeLeft.Text = "00시간 00분 00초";
            UpdateBlockingUi();
            MessageBox.Show("개발용 정지 버튼으로 차단이 종료되었습니다!");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            DataModel.LoadFromJson(); // 애플리케이션 시작 시 저장된 데이터 로드
            LoadBlockProfiles(); // 차단 프로필 로드

            btnStopBlocking.Text = "집중모드 정지(개발용)";
            cmbHour.TextChanged += ComboBox_TextChanged;
            cmbMin.TextChanged += ComboBox_TextChanged;

            // 콤보박스에 "00" 및 "0"을 추가하는 로직
            if (!cmbMin.Items.Contains("00"))
            {
                cmbMin.Items.Insert(0, "00");
            }
            if (!cmbHour.Items.Contains("0")) // 0시간도 선택 가능하도록 추가
            {
                cmbHour.Items.Insert(0, "0");
            }


            // 애플리케이션 시작 시 집중모드가 이미 활성화되어 있었다면 타이머 재시작
            if (DataModel.IsBlockingActive && !blockingtimer.Enabled)
            {
                blockingtimer.Start();
            }

            UpdateBlockingUi(); // 초기 UI 상태 업데이트
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
                if (!DataModel.StartLifeBreak()) // 라이프 사용 및 자유시간 시작
                {
                    MessageBox.Show("남은 라이프가 없습니다.");
                    return;
                }

                UpdateBlockingUi(); // UI 업데이트 (버튼 텍스트 변경 등)
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
            // Life == 0이면 긴급 종료 버튼이 없으므로 다이얼로그 크기를 조절
            dialog.ClientSize = (DataModel.Life > 0 && DataModel.Life <= 1) ? new Size(460, 215) : new Size(460, 180);

            descriptionLabel.Location = new Point(18, 18);
            descriptionLabel.Size = new Size(424, 92);
            descriptionLabel.Text = BuildStopDialogMessage(secondsLeft);

            useLifeButton.Location = new Point(18, (DataModel.Life > 0 && DataModel.Life <= 1) ? 130 : 120);
            useLifeButton.Size = new Size(150, 38);
            useLifeButton.Enabled = false;
            useLifeButton.Text = $"대기 중 {secondsLeft}초";
            useLifeButton.Click += delegate
            {
                selectedAction = StopRequestAction.LifeBreak;
                dialog.Close();
            };

            cancelButton.Location = new Point((DataModel.Life > 0 && DataModel.Life <= 1) ? 322 : 292, (DataModel.Life > 0 && DataModel.Life <= 1) ? 130 : 120);
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

            if (DataModel.Life > 0 && DataModel.Life <= 1) // 마지막 라이프가 남았을 때만 긴급 종료 버튼 표시 (Life==1일 때)
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

            // 마지막 라이프가 1개일 때만 "마지막 라이프입니다" 메시지 추가
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

            if (!DataModel.EmergencyStopFocusSession()) // EmergencyStopFocusSession()은 이제 Life 감소 로직을 포함
            {
                MessageBox.Show("남은 라이프가 없습니다."); // 이 경우는 발생하지 않을 것으로 예상 (HandleFocusStopRequest에서 Life==0이면 이미 차단)
                return;
            }

            if (blockingtimer.Enabled)
            {
                blockingtimer.Stop();
            }

            lblShowTimeLeft.Text = "00시간 00분 00초";
            UpdateBlockingUi();
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
                if (DataModel.IsBreakActive)
                {
                    btnActivateBlocking.Text = "집중모드 재개"; // 변경: 자유시간 중에는 '집중모드 재개'
                    // 자유시간 중에는 타이머 설정 콤보박스도 비활성화되어야 함 (이미 위에서 처리)
                    // label4에 자유시간 관련 메시지 출력은 timer_Tick에서 처리
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

            int minutes;
            bool isMinValid = false;
            if (!string.IsNullOrEmpty(cmbMin.Text))
            {
                if (cmbMin.Text == "00")
                {
                    isMinValid = true;
                }
                else if (int.TryParse(cmbMin.Text, out minutes) && minutes >= 0 && minutes < 60)
                {
                    isMinValid = true;
                }
            }

            btnActivateBlocking.Enabled = hasHour && isMinValid && !DataModel.IsEmergencyLockedOut;
        }
    }
}
