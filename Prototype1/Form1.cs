using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

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
            foreach (string processName in DataModel.NormalizeProcessNames(blockList))
            {
                Process[] processes = Process.GetProcessesByName(processName);

                foreach (Process p in processes)
                {
                    TryKillProcess(p, processName);
                }
            }
        }

        private void TryKillProcess(Process process, string processName)
        {
            try
            {
                if (process.Id == Process.GetCurrentProcess().Id)
                {
                    return;
                }

                process.Kill();
                process.WaitForExit();
                Console.WriteLine($"{processName} 차단 완료!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"오류 발생: {ex.Message}");
            }
            finally
            {
                process.Dispose();
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

        private void btnBlockList_Click(object sender, EventArgs e)
        {
            UpdateSelectedProfileFromComboBox();

            using (BlockListForm subForm = new BlockListForm(DataModel.SelectedProfileId))
            {
                subForm.ShowDialog(this);
            }

            PopulateFocusProfileComboBox();
        }

        private void btnActivateBlocking_Click(object sender, EventArgs e)
        {
            UpdateSelectedProfileFromComboBox();

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

            DataModel.FocusEndTime = DateTime.Now.AddMinutes(totalMinutes);
            DataModel.IsBlockingActive = true;
            DataModel.SaveToJson();

            if (!blockingtimer.Enabled)
            {
                blockingtimer.Start();
            }

            MessageBox.Show($"{DataModel.GetSelectedProfile().Name}으로 차단이 시작되었습니다!");
        }

        private void blockingtimer_Tick(object sender, EventArgs e)
        {
            if (!DataModel.IsBlockingActive)
            {
                return;
            }

            if (DateTime.Now >= DataModel.FocusEndTime)
            {
                blockingtimer.Stop();
                DataModel.IsBlockingActive = false;
                DataModel.SaveToJson();

                MessageBox.Show("정해진 집중 시간이 끝났습니다! 차단이 해제됩니다.");
                return;
            }

            TimeSpan timeLeft = DataModel.FocusEndTime - DateTime.Now;
            lblShowTimeLeft.Text = string.Format("{0}시간 {1:D2}분 {2:D2}초", timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);

            List<string> finalBlockList = DataModel.GetActiveBlockedProcesses();

            if (!finalBlockList.Contains("taskmgr", StringComparer.OrdinalIgnoreCase))
            {
                finalBlockList.Add("taskmgr");
            }

            KillProcesses(finalBlockList);
        }

        private void btnStopBlocking_Click(object sender, EventArgs e)
        {
            DataModel.IsBlockingActive = false;
            DataModel.SaveToJson();

            if (blockingtimer.Enabled)
            {
                blockingtimer.Stop();
            }

            MessageBox.Show("차단이 종료되었습니다!");
        }

        private void Prototype1_Load(object sender, EventArgs e)
        {
            DataModel.LoadFromJson();
            PopulateFocusProfileComboBox();

            btnActivateBlocking.Enabled = false;
            cmbHour.TextChanged += ComboBox_TextChanged;
            cmbMin.TextChanged += ComboBox_TextChanged;
        }

        private void Prototype1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DataModel.IsBlockingActive)
            {
                e.Cancel = true;
                MessageBox.Show("집중모드가 실행 중입니다. 종료하려면 먼저 집중모드를 정지해 주세요.");
            }
            else
            {
                UpdateSelectedProfileFromComboBox();
                DataModel.SaveToJson();
            }
        }

        private void CmbFocusProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelectedProfileFromComboBox();
            ComboBox_TextChanged(sender, e);
        }

        private void PopulateFocusProfileComboBox()
        {
            cmbFocusProfile.SelectedIndexChanged -= CmbFocusProfile_SelectedIndexChanged;
            cmbFocusProfile.DataSource = null;
            cmbFocusProfile.DisplayMember = "Name";
            cmbFocusProfile.ValueMember = "Id";
            cmbFocusProfile.DataSource = DataModel.FocusProfiles.ToList();
            cmbFocusProfile.SelectedValue = DataModel.SelectedProfileId;
            cmbFocusProfile.SelectedIndexChanged += CmbFocusProfile_SelectedIndexChanged;
        }

        private void UpdateSelectedProfileFromComboBox()
        {
            if (cmbFocusProfile.SelectedItem is FocusProfile selectedProfile)
            {
                DataModel.SetSelectedProfile(selectedProfile.Id);
            }
        }

        private void ComboBox_TextChanged(object sender, EventArgs e)
        {
            bool hasHour = !string.IsNullOrWhiteSpace(cmbHour.Text);
            bool hasMin = !string.IsNullOrWhiteSpace(cmbMin.Text);
            bool hasProfile = cmbFocusProfile.SelectedItem != null;
            btnActivateBlocking.Enabled = hasHour && hasMin && hasProfile;
        }
    }
}
