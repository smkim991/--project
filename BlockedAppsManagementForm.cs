using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prototype1.UI
{
    public partial class BlockedAppsManagementForm : Form
    {
        private List<string> allBlockableItems;
        private Dictionary<string, List<string>> modeBlockedItems;
        private string currentSelectedMode;

        public BlockedAppsManagementForm(Dictionary<string, List<string>> initialBlockedItems)
        {
            InitializeComponent();
            InitializeAllBlockableItems(); // 모든 차단 가능 항목 초기화

            // 메인 폼에서 전달받은 데이터를 modeBlockedItems에 복사
            this.modeBlockedItems = new Dictionary<string, List<string>>();
            foreach (var kvp in initialBlockedItems)
            {
                this.modeBlockedItems.Add(kvp.Key, new List<string>(kvp.Value));
            }

            SetupUI();        // UI 설정 (CheckedListBox 채우기)
            SelectMode("대학생");
        }

        private void InitializeAllBlockableItems()
        {
            // 모든 차단 가능한 항목들을 정의합니다. 이 리스트는 CheckedListBox를 채우는 데 사용됩니다.
            allBlockableItems = new List<string>
            {
                "넷플릭스", "네이버웹툰", "유튜브",
                "메모장", "멜론", "인스타그램",
                "엑셀", "카카오톡", "틱톡"
            };
        }

        public Dictionary<string, List<string>> GetUpdatedBlockedItems()
        {
            return this.modeBlockedItems;
        }

        private void SetupUI()
        {
            clbBlockableItems.Items.AddRange(allBlockableItems.ToArray());
            clbBlockableItems.CheckOnClick = true;
        }

        private void SelectMode(string modeName)
        {
            if (!string.IsNullOrEmpty(currentSelectedMode) && currentSelectedMode != modeName)
            {
                SaveCurrentModeChanges();
            }

            currentSelectedMode = modeName;
            lblCurrentModeDisplay.Text = $"현재 모드: {currentSelectedMode}";

            // CheckedListBox의 모든 항목 체크 상태 초기화
            for (int i = 0; i < clbBlockableItems.Items.Count; i++)
            {
                clbBlockableItems.SetItemChecked(i, false);
            }

            // 새로 선택된 모드의 차단 항목들을 CheckedListBox에 반영
            if (modeBlockedItems.ContainsKey(currentSelectedMode))
            {
                List<string> blockedItemsForMode = modeBlockedItems[currentSelectedMode];
                foreach (string item in blockedItemsForMode)
                {
                    int index = clbBlockableItems.Items.IndexOf(item);
                    if (index != -1)
                    {
                        clbBlockableItems.SetItemChecked(index, true);
                    }
                }
            }
        }

        private void SaveCurrentModeChanges()
        {
            if(string.IsNullOrEmpty(currentSelectedMode)) return;

            List<string> updatedBlockedItems = new List<string>();
            foreach (object checkedItem in clbBlockableItems.CheckedItems)
            {
                updatedBlockedItems.Add(checkedItem.ToString());
            }
            modeBlockedItems[currentSelectedMode] = updatedBlockedItems;
        }

        private void BlockedAppsManagementForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectMode("대학생");
        }

        private void btnSelectDeveloper_Click(object sender, EventArgs e)
        {
            SelectMode("개발자");
        }

        private void btnSelectEditor_Click(object sender, EventArgs e)
        {
            SelectMode("영상편집자");
        }

        private void btnSelectExaminee_Click(object sender, EventArgs e)
        {
            SelectMode("수험생");
        }

        private void lblCurrentModeDisplay_Click(object sender, EventArgs e)
        {

        }

        private void clbBlockableItems_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string item = clbBlockableItems.Items[e.Index].ToString();

            if (e.NewValue == CheckState.Checked)
            {
                if (!modeBlockedItems[currentSelectedMode].Contains(item))
                {
                    modeBlockedItems[currentSelectedMode].Add(item);
                }
            }
            else if (e.NewValue == CheckState.Unchecked)
            {
                modeBlockedItems[currentSelectedMode].Remove(item);
            }
        }
        private void BlockedAppsManagementForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveCurrentModeChanges(); // 폼이 닫히기 전에 최종 변경사항 저장
            this.DialogResult = DialogResult.OK;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            SaveCurrentModeChanges(); // 현재 모드의 변경사항을 저장
            MessageBox.Show("변경사항이 성공적으로 저장되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
