using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Prototype1.UI
{
    public partial class BlockedAppsManagementForm : Form
    {
        private static readonly string[] AllBlockableItems =
        {
            "넷플릭스", "네이버웹툰", "유튜브",
            "메모장", "멜론", "인스타그램",
            "엑셀", "카카오톡", "틱톡"
        };

        private readonly Dictionary<string, List<string>> modeBlockedItems;
        private string currentSelectedMode;
        private bool isLoadingMode;

        public BlockedAppsManagementForm(Dictionary<string, List<string>> initialBlockedItems)
        {
            InitializeComponent();
            modeBlockedItems = CloneBlockedItems(initialBlockedItems);
            clbBlockableItems.Items.AddRange(AllBlockableItems);
            clbBlockableItems.CheckOnClick = true;
            SelectMode("대학생");
        }

        public Dictionary<string, List<string>> GetUpdatedBlockedItems()
        {
            SaveCurrentModeChanges();
            return modeBlockedItems;
        }

        private void SelectMode(string modeName)
        {
            if (!string.IsNullOrEmpty(currentSelectedMode) && currentSelectedMode != modeName)
            {
                SaveCurrentModeChanges();
            }

            currentSelectedMode = modeName;
            if (!modeBlockedItems.ContainsKey(currentSelectedMode))
            {
                modeBlockedItems[currentSelectedMode] = new List<string>();
            }

            lblCurrentModeDisplay.Text = "현재 모드: " + currentSelectedMode;

            isLoadingMode = true;
            try
            {
                for (int i = 0; i < clbBlockableItems.Items.Count; i++)
                {
                    string item = clbBlockableItems.Items[i].ToString();
                    clbBlockableItems.SetItemChecked(i, modeBlockedItems[currentSelectedMode].Contains(item));
                }
            }
            finally
            {
                isLoadingMode = false;
            }
        }

        private void SaveCurrentModeChanges()
        {
            if (string.IsNullOrEmpty(currentSelectedMode))
            {
                return;
            }

            modeBlockedItems[currentSelectedMode] = clbBlockableItems.CheckedItems
                .Cast<object>()
                .Select(item => item.ToString())
                .ToList();
        }

        private void btnSelectStudent_Click(object sender, EventArgs e)
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

        private void clbBlockableItems_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (isLoadingMode || string.IsNullOrEmpty(currentSelectedMode))
            {
                return;
            }

            string item = clbBlockableItems.Items[e.Index].ToString();
            List<string> blockedItems = modeBlockedItems[currentSelectedMode];

            if (e.NewValue == CheckState.Checked && !blockedItems.Contains(item))
            {
                blockedItems.Add(item);
            }
            else if (e.NewValue == CheckState.Unchecked)
            {
                blockedItems.Remove(item);
            }
        }

        private void BlockedAppsManagementForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveCurrentModeChanges();
            DialogResult = DialogResult.OK;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveCurrentModeChanges();
            MessageBox.Show("변경사항이 성공적으로 저장되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }

        private static Dictionary<string, List<string>> CloneBlockedItems(Dictionary<string, List<string>> source)
        {
            Dictionary<string, List<string>> clone = new Dictionary<string, List<string>>();
            if (source == null)
            {
                return clone;
            }

            foreach (KeyValuePair<string, List<string>> item in source)
            {
                clone[item.Key] = item.Value == null ? new List<string>() : new List<string>(item.Value);
            }

            return clone;
        }
    }
}
