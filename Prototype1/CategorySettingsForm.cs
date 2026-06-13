using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;


namespace Prototype1.UI
{
    public partial class CategorySettingsForm : Form
    {
        private static readonly string[] DefaultCategories =
        {
            "대학생", "개발자", "영상편집자", "수험생"
        };

        private readonly Dictionary<string, List<string>> currentBlockedItems;
        private readonly List<Guna2Button> customCategoryButtons = new List<Guna2Button>();
        private MainForm mainForm;
        private string currentSelectedCategory = "대학생";
        private Guna.UI2.WinForms.Guna2Button lastClickedCategoryButton = null;

        public CategorySettingsForm(Dictionary<string, List<string>> blockedItems, MainForm form)
        {
            InitializeComponent();
            currentBlockedItems = blockedItems ?? new Dictionary<string, List<string>>();
            AutoScroll = true;
            BuildCustomCategoryButtons();
            UpdateCategorySettingsDisplay(currentSelectedCategory);
            btnConfirmSelection.Enabled = false;
            mainForm = form;
        }

        private void CategoryButton_Click(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2Button clickedButton =
                sender as Guna.UI2.WinForms.Guna2Button;

            if (clickedButton == null)
            {
                return;
            }

            if (lastClickedCategoryButton != null)
            {
                lastClickedCategoryButton.FillColor = Color.FromArgb(35, 35, 35);
            }

            currentSelectedCategory = clickedButton.Text;
            clickedButton.FillColor = Color.FromArgb(139, 92, 246);
            lastClickedCategoryButton = clickedButton;

            UpdateCategorySettingsDisplay(currentSelectedCategory);

            btnConfirmSelection.Enabled = true;
        }

        private void BuildCustomCategoryButtons()
        {
            foreach (Guna2Button button in customCategoryButtons)
            {
                Controls.Remove(button);
                button.Dispose();
            }

            customCategoryButtons.Clear();

            int leftColumnX = 176;
            int rightColumnX = 597;
            int top = 628;
            int buttonHeight = 82;
            int rowGap = 18;
            int index = 0;

            foreach (string category in currentBlockedItems.Keys
                         .Where(category => !DefaultCategories.Contains(category, StringComparer.OrdinalIgnoreCase))
                         .OrderBy(category => category))
            {
                Guna2Button button = CreateCategoryButton(category);
                int row = index / 2;
                bool isRightColumn = index % 2 == 1;
                button.Location = new Point(isRightColumn ? rightColumnX : leftColumnX, top + row * (buttonHeight + rowGap));
                button.Size = new Size(isRightColumn ? 361 : 371, buttonHeight);
                Controls.Add(button);
                customCategoryButtons.Add(button);
                index++;
            }

            if (customCategoryButtons.Count == 0)
            {
                return;
            }

            int rows = (int)Math.Ceiling(customCategoryButtons.Count / 2.0);
            int detailTop = top + rows * (buttonHeight + rowGap) + 20;
            lblBlockedItemsDisplay.Location = new Point(lblBlockedItemsDisplay.Left, detailTop);
            btnConfirmSelection.Location = new Point(btnConfirmSelection.Left, detailTop + 22);
        }

        private Guna2Button CreateCategoryButton(string category)
        {
            Guna2Button button = new Guna2Button();
            button.Animated = true;
            button.BackColor = Color.Transparent;
            button.BorderRadius = 20;
            button.FillColor = Color.FromArgb(35, 35, 35);
            button.Font = new Font("맑은 고딕", 11F, FontStyle.Bold, GraphicsUnit.Point, 129);
            button.ForeColor = Color.White;
            button.HoverState.FillColor = Color.FromArgb(55, 55, 60);
            button.ShadowDecoration.Enabled = true;
            button.Text = category;
            button.Click += CategoryButton_Click;
            return button;
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

                    if (mainForm.PromptAndStartFocusSessionFromCategory())
                    {
                        DialogResult = DialogResult.OK;
                        Close();
                        return;
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
                lastClickedCategoryButton.FillColor = Color.FromArgb(35, 35, 35);
                lastClickedCategoryButton = null;
            }

            btnConfirmSelection.Enabled = false;
            currentSelectedCategory = string.Empty;
        }
    }
}
