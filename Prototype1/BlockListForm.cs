using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Prototype1
{
    public partial class BlockListForm : Form
    {
        public BlockListForm()
        {
            InitializeComponent();
        }

        private void AddBlockingLog(string procName)
        {

            // 중복검사
            bool isDuplicate = false;
            foreach (ListViewItem i in blockListView.Items)
            {
                if (i.Text == procName)
                {
                    isDuplicate = true;
                    break;
                }
            }
            if (!isDuplicate)
            {
                // 1. 새로운 ListViewItem 생성 (첫 번째 열 데이터)
                ListViewItem item = new ListViewItem(procName);
                // 2. 나머지 열 데이터 추가 (SubItems)
                item.SubItems.Add("OFF");
                item.SubItems.Add(DateTime.Now.ToString("HH:mm:ss"));

                // 3. ListView에 최종 추가
                blockListView.Items.Add(item);

                // 4. (옵션) 가장 최근 항목이 보이도록 스크롤 이동
                blockListView.EnsureVisible(blockListView.Items.Count - 1);
            }
        }

        private void btnCategory1_Click(object sender, EventArgs e)
        {
            List<string> DefaultBlockListOfCategory1 = new List<string> { "mspaint" }; // 임시 category1 = {"mspaint"} (그림판)
            foreach (string str in DefaultBlockListOfCategory1)
            {
                AddBlockingLog(str);
            }

        }
        private void btnCategory2_Click(object sender, EventArgs e)
        {
            List<string> DefaultBlockListOfCategory1 = new List<string> { "KakaoTalk" }; // 임시 category1 = {"mspaint"} (그림판)
            foreach (string str in DefaultBlockListOfCategory1)
            {
                AddBlockingLog(str);
            }
        }

        private void btnBlockListSave_Click(object sender, EventArgs e)
        {
            // 1. 전역 리스트를 비우고 새로 담을 준비를 합니다.
            DataModel.SavedBlockList.Clear();

            // 2. ListView에 있는 모든 프로세스 이름을 가져와 저장합니다.
            foreach (ListViewItem item in blockListView.Items)
            {
                DataModel.SavedBlockList.Add(item.Text);
            }

            // 3. 작업이 끝났으므로 창을 닫습니다.
            this.Close();
        }

        private void BlockListForm_Load(object sender, EventArgs e)
        {
            blockListView.Items.Clear();

            // 저장된 목록을 하나씩 다시 리스트뷰에 뿌려줍니다.
            foreach (string procName in DataModel.SavedBlockList)
            {
                ListViewItem item = new ListViewItem(procName);

                // 현재 차단 상태(IsBlockingActive)에 따라 문구를 결정합니다.
                string statusText = DataModel.IsBlockingActive ? "ON" : "OFF";
                item.SubItems.Add(statusText);

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

        }

        private void BlockListForm_Shown(object sender, EventArgs e)
        {
            blockListView.Columns[2].Width = -2;
        }
    }
}
