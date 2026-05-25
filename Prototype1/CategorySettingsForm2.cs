using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;


namespace Prototype1.UI
{
    public partial class CategorySettingsForm2 : Form
    {
        private readonly Dictionary<string, List<string>> currentBlockedItems;
        private MainForm mainForm;
        private string currentSelectedCategory = "대학생";
        private Guna.UI2.WinForms.Guna2Button lastClickedCategoryButton = null;

        public CategorySettingsForm2(Dictionary<string, List<string>> blockedItems, MainForm form)
        {
            InitializeComponent();
            currentBlockedItems = blockedItems ?? new Dictionary<string, List<string>>();
            UpdateCategorySettingsDisplay(currentSelectedCategory);
            btnConfirmSelection.Enabled = false;
            mainForm = form;
        }

        private void CategoryButton_Click(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2Button clickedButton =
                sender as Guna.UI2.WinForms.Guna2Button;

            clickedButton.FillColor = Color.FromArgb(139, 92, 246);
            if (clickedButton == null)
            {
                return;
            }

           
            currentSelectedCategory = clickedButton.Text;

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

        private void lblBlockedItemsDisplay_Click(object sender, EventArgs e)
        {
        }

        private void btnConfirmSelection_Click(object sender, EventArgs e)
        {
            using (FocusReasonForm messageInputForm = new FocusReasonForm())
            {
                if (messageInputForm.ShowDialog(this) == DialogResult.OK)
                {
                    SaveSelectedCategoryAsActiveBlockList();

                    using (FocusSetupPendingForm nextForm = new FocusSetupPendingForm())
                    {
                        nextForm.ShowDialog(this);
                    }

                    ResetCategorySelectionUI();
                }
            }
        }

        private void SaveSelectedCategoryAsActiveBlockList()
        {
            DataModel.UpdateBlockProfiles(currentBlockedItems);
            DataModel.SetActiveBlockListForCategory(currentSelectedCategory);
            mainForm.SetCurrentCategory(currentSelectedCategory);
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

        private void guna2Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
