using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Prototype1
{
    public partial class BlockListForm : Form
    {
        private readonly string profileId;

        public BlockListForm()
            : this(DataModel.SelectedProfileId)
        {
        }

        public BlockListForm(string profileId)
        {
            InitializeComponent();
            this.profileId = profileId;
        }

        private FocusProfile CurrentProfile
        {
            get { return DataModel.GetProfileById(profileId) ?? DataModel.GetSelectedProfile(); }
        }

        private void AddProcessRule(string procName)
        {
            string normalizedName = DataModel.NormalizeProcessName(procName);
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                return;
            }

            bool isDuplicate = false;
            foreach (ListViewItem i in blockListView.Items)
            {
                if (string.Equals(i.Text, normalizedName, StringComparison.OrdinalIgnoreCase))
                {
                    isDuplicate = true;
                    break;
                }
            }

            if (!isDuplicate)
            {
                ListViewItem item = new ListViewItem(normalizedName);
                item.SubItems.Add("BLOCK");
                item.SubItems.Add(DateTime.Now.ToString("HH:mm:ss"));

                blockListView.Items.Add(item);
                blockListView.EnsureVisible(blockListView.Items.Count - 1);
            }
        }

        private void ConfigureProfileView()
        {
            FocusProfile profile = CurrentProfile;

            CurrentBlockList.Text = $"{profile.Name} - 차단 프로세스 목록";
            BlockingState.Text = "차단상태";
            btnUserSpecified.Text = "입력값 추가";
            btnClosingBlockListForm.Text = "닫기";
            btnCategory1.Text = "메신저 차단 추가";
            btnCategory2.Text = "게임/미디어 차단 추가";
        }

        private void btnCategory1_Click(object sender, EventArgs e)
        {
            foreach (string processName in GetMessengerBlockProcesses())
            {
                AddProcessRule(processName);
            }
        }

        private void btnCategory2_Click(object sender, EventArgs e)
        {
            foreach (string processName in GetEntertainmentBlockProcesses())
            {
                AddProcessRule(processName);
            }
        }

        private List<string> GetMessengerBlockProcesses()
        {
            return new List<string>
            {
                "KakaoTalk",
                "Discord",
                "Telegram"
            };
        }

        private List<string> GetEntertainmentBlockProcesses()
        {
            return new List<string>
            {
                "Steam",
                "LeagueClient",
                "Spotify",
                "EpicGamesLauncher",
                "Battle.net",
                "RiotClientServices"
            };
        }

        private void btnBlockListSave_Click(object sender, EventArgs e)
        {
            List<string> processNames = new List<string>();

            foreach (ListViewItem item in blockListView.Items)
            {
                processNames.Add(item.Text);
            }

            DataModel.SaveEditableProcesses(profileId, processNames);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BlockListForm_Load(object sender, EventArgs e)
        {
            ConfigureProfileView();
            blockListView.Items.Clear();

            foreach (string procName in DataModel.GetEditableProcesses(profileId))
            {
                ListViewItem item = new ListViewItem(procName);
                item.SubItems.Add("BLOCK");
                item.SubItems.Add(DateTime.Now.ToString("HH:mm:ss"));
                blockListView.Items.Add(item);
            }
        }

        private void btnClosingBlockListForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnUserSpecified_Click(object sender, EventArgs e)
        {
            string processName = txtProcessName.Text;

            if (string.IsNullOrWhiteSpace(processName))
            {
                MessageBox.Show("추가할 프로세스명을 입력해 주세요.");
                return;
            }

            AddProcessRule(processName);
            txtProcessName.Clear();
            txtProcessName.Focus();
        }

        private void BlockListForm_Shown(object sender, EventArgs e)
        {
            blockListView.Columns[2].Width = -2;
        }
    }
}
