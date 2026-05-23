using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Prototype1.UI
{
    public partial class CategorySettingsForm : Form
    {
        private readonly Dictionary<string, List<string>> currentBlockedItems;
        private string currentSelectedCategory = "대학생";
        private Button lastClickedCategoryButton = null;

        public CategorySettingsForm(Dictionary<string, List<string>> blockedItems)
        {
            InitializeComponent();
            currentBlockedItems = blockedItems ?? new Dictionary<string, List<string>>();
            UpdateCategorySettingsDisplay(currentSelectedCategory);
            btnConfirmSelection.Enabled = false;
        }

        private void CategoryButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton == null)
            {
                return;
            }

            if (lastClickedCategoryButton != null)
            {
                lastClickedCategoryButton.BackColor = SystemColors.Control;
            }

            currentSelectedCategory = clickedButton.Text;
            clickedButton.BackColor = Color.LightBlue;
            lastClickedCategoryButton = clickedButton;

            UpdateCategorySettingsDisplay(currentSelectedCategory);
            btnConfirmSelection.Enabled = true;
        }

        private void UpdateCategorySettingsDisplay(string categoryName)
        {
            if (currentBlockedItems.ContainsKey(categoryName))
            {
                List<string> blocked = currentBlockedItems[categoryName];
                lblBlockedItemsDisplay.Text = $"'{categoryName}' 차단 항목: {string.Join(", ", blocked)}";
            }
            else
            {
                lblBlockedItemsDisplay.Text = $"'{categoryName}' 차단 항목: 없음";
            }
        }

        private void btnConfirmSelection_Click(object sender, EventArgs e)
        {
            using (FocusReasonForm messageInputForm = new FocusReasonForm())
            {
                if (messageInputForm.ShowDialog(this) == DialogResult.OK)
                {
                    DataModel.CurrentFocusGoal = messageInputForm.EnteredMessage;
                    DataModel.CurrentFocusCategory = currentSelectedCategory;
                    SaveSelectedCategoryAsActiveBlockList();

                    ResetCategorySelectionUI();
                }
            }
        }

        private void SaveSelectedCategoryAsActiveBlockList()
        {
            DataModel.UpdateBlockProfiles(currentBlockedItems);
            DataModel.SetActiveBlockListForCategory(currentSelectedCategory);
        }

        private void ResetCategorySelectionUI()
        {
            if (lastClickedCategoryButton != null)
            {
                lastClickedCategoryButton.BackColor = SystemColors.Control;
                lastClickedCategoryButton = null;
            }

            btnConfirmSelection.Enabled = false;
            currentSelectedCategory = string.Empty;
        }
    }
}
