using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics; // 프로세스 제어를 위한 필수 네임스페이스

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
                KillProcesses(DataModel.SavedBlockList);
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
        }

        private void Prototype1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DataModel.SaveToJson();
        }
    }
}
