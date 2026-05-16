using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader; // 프로세스 제어를 위한 필수 네임스페이스

namespace Prototype1
{
    public partial class Prototype1 : Form
    {
        public Prototype1()
        {
            InitializeComponent();
        }


        public void KillProcesses(List<string> blockList)
        {
            foreach (string processName in blockList)
            {
                // 현재 실행 중인 프로세스 중 이름이 일치하는 것들을 모두 가져옴
                Process[] processes = Process.GetProcessesByName(processName);

                foreach (Process p in processes)
                {
                    try
                    {
                        p.Kill(); // 프로세스 강제 종료
                        p.WaitForExit(); // 완전히 종료될 때까지 대기
                        Console.WriteLine($"{processName} 차단 완료!");
                    }
                    catch (Exception ex)
                    {
                        // 시스템 프로세스나 권한 문제로 종료 실패 시 예외 처리
                        Debug.WriteLine($"오류 발생: {ex.Message}");
                    }
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {

            if (DataModel.IsBlockingActive)
            {
                MessageBox.Show("시각적 손실 유발!");
            }
            else
            {
                this.Close();
            }
        }

        private void btnBlockList_Click(object sender, EventArgs e)
        {
            BlockListForm subForm = new BlockListForm();

            subForm.ShowDialog();
        }

        private void btnActivateBlocking_Click(object sender, EventArgs e)
        {
            string hourInput = cmbHour.Text;
            string minInput = cmbMin.Text; // 사용자의 입력(콤보박스)를 읽는다

            int hours = 0;
            int minutes = 0;

            if (!string.IsNullOrEmpty(hourInput) && !int.TryParse(hourInput, out hours)) // 사용자가 숫자가 아닌 값을 입력했을때 함수 종료
            {
                MessageBox.Show(" 시간에 올바른 숫자를 입력해 주세요 !");
                return;
            }

            if (!string.IsNullOrEmpty(minInput) && !int.TryParse(minInput, out minutes))
            {
                MessageBox.Show(" 분에 올바른 숫자를 입력해 주세요 !");
                return;
            }
            int totalMinutes = (hours * 60) + minutes;
            DataModel.FocusEndTime = DateTime.Now.AddMinutes(totalMinutes);

            // 전역 상태를 ON으로 변경
            DataModel.IsBlockingActive = true;
            // 2. 타이머가 중지 상태라면 시작시킵니다.
            if (!blockingtimer.Enabled)
            {
                blockingtimer.Start();
            }

            MessageBox.Show("차단이 시작되었습니다 !");
        }

        private void blockingtimer_Tick(object sender, EventArgs e)
        {
            // 차단 활성화 상태일 때만 작동합니다.
            if (DataModel.IsBlockingActive)
            {
                if (DateTime.Now >= DataModel.FocusEndTime) // 세션 종료 시간 도달
                {
                    blockingtimer.Stop();
                    DataModel.IsBlockingActive = false;
                    DataModel.SaveToJson();

                    MessageBox.Show("정해진 집중 시간이 끝났습니다! 차단이 해제됩니다.");
                }
                else // 아직 세션 종료 시간에 도달하지 못한 경우
                {
                    TimeSpan timeLeft = DataModel.FocusEndTime - DateTime.Now; // 남은시간 실시간으로 계산해서 사용자에 Display
                    lblShowTimeLeft.Text = string.Format("{0}시간 {1:D2}분 {2:D2}초", timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
                    List<string> finalBlockList = new List<string>(DataModel.SavedBlockList); // savedBlockList 복사

                    if (!finalBlockList.Contains("taskmgr")) // List 요소 중복을 피하기 위해 포함유무 검사
                    {
                        finalBlockList.Add("taskmgr"); // Default로 on session이면 작업관리자 항상 차단 (cmd, powershell??)
                    }
                    KillProcesses(finalBlockList);
                }
            }
        }

        private void btnStopBlocking_Click(object sender, EventArgs e)
        {
            // 1. 전역 차단 상태를 OFF로 변경
            DataModel.IsBlockingActive = false;

            // 2. 실행 중인 감시 타이머 중지
            if (blockingtimer.Enabled)
            {
                blockingtimer.Stop();
            }
            MessageBox.Show("차단이 종료되었습니다 !");
        }

        private void Prototype1_Load(object sender, EventArgs e)
        {
            DataModel.LoadFromJson();
            btnActivateBlocking.Enabled = false; // 집중모드 활성화 버튼 세션시간 입력하기 전까지 사용 불가
            cmbHour.TextChanged += ComboBox_TextChanged;
            cmbMin.TextChanged += ComboBox_TextChanged;
        }

        private void Prototype1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DataModel.IsBlockingActive)
            {
                e.Cancel = true;
                MessageBox.Show("시각적 손실 유발!");
            }
            else
            {
                DataModel.SaveToJson();
            }
        }

        private void ComboBox_TextChanged(object sender, EventArgs e)
        {
            bool hasHour = !string.IsNullOrWhiteSpace(cmbHour.Text);
            bool hasMin = !string.IsNullOrWhiteSpace(cmbMin.Text);
            btnActivateBlocking.Enabled = hasHour && hasMin;
        }
    }
}
