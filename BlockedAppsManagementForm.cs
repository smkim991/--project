using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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

        private static readonly string[] DefaultCategories =
        {
            "대학생", "개발자", "영상편집자", "수험생"
        };

        private readonly Dictionary<string, List<string>> modeBlockedItems;
        private readonly List<Button> customCategoryButtons = new List<Button>();
        private string currentSelectedMode;
        private bool isLoadingMode;

        private static readonly Color MenuFill = Color.FromArgb(24, 24, 24);
        private static readonly Color MenuHoverFill = Color.FromArgb(40, 40, 45);
        private static readonly Color AccentFill = Color.FromArgb(139, 92, 246);

        public BlockedAppsManagementForm(Dictionary<string, List<string>> initialBlockedItems)
        {
            InitializeComponent();
            modeBlockedItems = CloneBlockedItems(initialBlockedItems);
            clbBlockableItems.CheckOnClick = true;
            RefreshBlockableItems();
            StyleModeButtons();
            StyleCustomAppControls();
            RefreshCustomCategoryButtons();
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
            UpdateModeButtonStates();
            EnsureModeItemsVisible(currentSelectedMode);

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

        private void RefreshBlockableItems()
        {
            HashSet<string> defaultItems = new HashSet<string>(AllBlockableItems, StringComparer.OrdinalIgnoreCase);
            List<string> customItems = modeBlockedItems.Values
                .Where(items => items != null)
                .SelectMany(items => items)
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => item.Trim())
                .Where(item => !defaultItems.Contains(item))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item)
                .ToList();

            isLoadingMode = true;
            try
            {
                clbBlockableItems.Items.Clear();
                clbBlockableItems.Items.AddRange(AllBlockableItems);

                foreach (string item in customItems)
                {
                    clbBlockableItems.Items.Add(item);
                }
            }
            finally
            {
                isLoadingMode = false;
            }
        }

        private void EnsureModeItemsVisible(string modeName)
        {
            if (string.IsNullOrWhiteSpace(modeName) || !modeBlockedItems.TryGetValue(modeName, out List<string> items) || items == null)
            {
                return;
            }

            foreach (string item in items.Where(item => !string.IsNullOrWhiteSpace(item)))
            {
                EnsureBlockableItemVisible(item);
            }
        }

        private int EnsureBlockableItemVisible(string item)
        {
            string normalizedItem = item.Trim();
            for (int i = 0; i < clbBlockableItems.Items.Count; i++)
            {
                if (string.Equals(clbBlockableItems.Items[i].ToString(), normalizedItem, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return clbBlockableItems.Items.Add(normalizedItem);
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

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            string categoryName = PromptForCategoryName();
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return;
            }

            if (modeBlockedItems.Keys.Any(key => string.Equals(key, categoryName, StringComparison.OrdinalIgnoreCase)))
            {
                AlertDialog.Show(this, "이미 같은 이름의 카테고리가 있습니다.");
                return;
            }

            SaveCurrentModeChanges();
            modeBlockedItems[categoryName] = new List<string>();
            RefreshCustomCategoryButtons();
            SelectMode(categoryName);
        }

        private string PromptForCategoryName()
        {
            using (Form dialog = new Form())
            using (Label prompt = new Label())
            using (TextBox input = new TextBox())
            using (Button okButton = new Button())
            using (Button cancelButton = new Button())
            {
                dialog.Text = "카테고리 추가";
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MinimizeBox = false;
                dialog.MaximizeBox = false;
                dialog.BackColor = Color.FromArgb(18, 18, 18);
                dialog.ForeColor = Color.White;
                dialog.ClientSize = new Size(380, 146);

                prompt.Text = "새 카테고리 이름";
                prompt.ForeColor = Color.White;
                prompt.Location = new Point(16, 18);
                prompt.Size = new Size(340, 24);

                input.BackColor = Color.FromArgb(35, 35, 35);
                input.BorderStyle = BorderStyle.FixedSingle;
                input.ForeColor = Color.White;
                input.Location = new Point(16, 50);
                input.Size = new Size(344, 30);

                okButton.Text = "추가";
                okButton.DialogResult = DialogResult.OK;
                okButton.Location = new Point(162, 98);
                okButton.Size = new Size(92, 34);
                StyleDialogButton(okButton, true);

                cancelButton.Text = "취소";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Location = new Point(268, 98);
                cancelButton.Size = new Size(92, 34);
                StyleDangerButton(cancelButton);

                dialog.Controls.Add(prompt);
                dialog.Controls.Add(input);
                dialog.Controls.Add(okButton);
                dialog.Controls.Add(cancelButton);
                dialog.AcceptButton = okButton;
                dialog.CancelButton = cancelButton;

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return string.Empty;
                }

                return input.Text.Trim();
            }
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

        private void clbBlockableItems_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            int index = clbBlockableItems.IndexFromPoint(e.Location);
            if (index < 0 || index >= clbBlockableItems.Items.Count)
            {
                return;
            }

            clbBlockableItems.SelectedIndex = index;
            string item = clbBlockableItems.Items[index].ToString();
            RemoveBlockItem(item);
        }

        private void btnAddCustomApp_Click(object sender, EventArgs e)
        {
            AddCustomBlockItem(txtCustomProcessName.Text, true);
        }

        private void txtCustomProcessName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;
            AddCustomBlockItem(txtCustomProcessName.Text, true);
        }

        private void btnPickRunningApp_Click(object sender, EventArgs e)
        {
            string processName = ShowRunningProcessPicker();
            if (string.IsNullOrWhiteSpace(processName))
            {
                return;
            }

            txtCustomProcessName.Text = processName;
            AddCustomBlockItem(processName, false, true);
        }

        private void AddCustomBlockItem(string rawProcessName, bool showMessage)
        {
            AddCustomBlockItem(rawProcessName, showMessage, false);
        }

        private void AddCustomBlockItem(string rawProcessName, bool showMessage, bool verifiedFromProcessList)
        {
            string processName = NormalizeProcessName(rawProcessName);
            if (string.IsNullOrWhiteSpace(processName))
            {
                if (showMessage)
                {
                    AlertDialog.Show(this, "차단할 앱의 프로세스명을 입력해 주세요.");
                }

                return;
            }

            if (!IsProcessNameFormatValid(processName))
            {
                AlertDialog.Show(this, "프로세스명에는 파일명으로 사용할 수 없는 문자를 넣을 수 없습니다.");
                return;
            }

            if (string.IsNullOrWhiteSpace(currentSelectedMode))
            {
                AlertDialog.Show(this, "먼저 모드를 선택해 주세요.");
                return;
            }

            if (!modeBlockedItems.ContainsKey(currentSelectedMode))
            {
                modeBlockedItems[currentSelectedMode] = new List<string>();
            }

            if (!verifiedFromProcessList &&
                !IsKnownBlockItem(processName) &&
                !IsExistingExecutablePath(rawProcessName) &&
                !IsCurrentlyRunningProcess(processName))
            {
                DialogResult result = AlertDialog.Show(
                    this,
                    $"{processName} 프로세스를 현재 실행 중인 앱에서 찾지 못했습니다.\r\n오타일 수 있으니 프로세스명을 다시 확인해 주세요.\r\n그래도 이 이름으로 추가할까요?",
                    "프로세스명 확인",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.OK)
                {
                    return;
                }
            }

            int itemIndex = EnsureBlockableItemVisible(processName);
            List<string> blockedItems = modeBlockedItems[currentSelectedMode];
            if (!blockedItems.Any(item => string.Equals(item, processName, StringComparison.OrdinalIgnoreCase)))
            {
                blockedItems.Add(processName);
            }

            clbBlockableItems.SetItemChecked(itemIndex, true);
            clbBlockableItems.SelectedIndex = itemIndex;
            txtCustomProcessName.Clear();

            if (showMessage)
            {
                AlertDialog.Show(this, $"{processName} 항목을 현재 모드에 추가했습니다.");
            }
        }

        private void RemoveBlockItem(string item)
        {
            if (string.IsNullOrWhiteSpace(item) || string.IsNullOrWhiteSpace(currentSelectedMode))
            {
                return;
            }

            if (!ShowDeleteBlockItemDialog(item, out bool deleteAllCategories))
            {
                return;
            }

            SaveCurrentModeChanges();
            if (deleteAllCategories)
            {
                foreach (List<string> blockedItems in modeBlockedItems.Values.Where(items => items != null))
                {
                    RemoveBlockItemFromList(blockedItems, item);
                }
            }
            else if (modeBlockedItems.TryGetValue(currentSelectedMode, out List<string> currentModeItems) && currentModeItems != null)
            {
                RemoveBlockItemFromList(currentModeItems, item);
            }

            RefreshBlockableItems();
            SelectMode(currentSelectedMode);
        }

        private bool ShowDeleteBlockItemDialog(string item, out bool deleteAllCategories)
        {
            deleteAllCategories = false;

            using (Form dialog = new Form())
            using (Label messageLabel = new Label())
            using (CheckBox deleteAllCheckBox = new CheckBox())
            using (Button okButton = new Button())
            using (Button cancelButton = new Button())
            {
                dialog.Text = "항목 삭제";
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MinimizeBox = false;
                dialog.MaximizeBox = false;
                dialog.BackColor = Color.FromArgb(18, 18, 18);
                dialog.ForeColor = Color.White;
                dialog.ClientSize = new Size(440, 190);

                messageLabel.Text = $"정말 삭제하시겠습니까?\r\n{item} 항목을 현재 카테고리에서 삭제합니다.";
                messageLabel.Font = new Font("맑은 고딕", 10F, FontStyle.Bold);
                messageLabel.ForeColor = Color.White;
                messageLabel.Location = new Point(20, 20);
                messageLabel.Size = new Size(400, 62);

                deleteAllCheckBox.Text = "모두 삭제";
                deleteAllCheckBox.Font = new Font("맑은 고딕", 9F, FontStyle.Regular);
                deleteAllCheckBox.ForeColor = Color.Silver;
                deleteAllCheckBox.BackColor = dialog.BackColor;
                deleteAllCheckBox.Location = new Point(20, 90);
                deleteAllCheckBox.Size = new Size(160, 28);
                deleteAllCheckBox.UseVisualStyleBackColor = false;

                okButton.Text = "확인";
                okButton.DialogResult = DialogResult.OK;
                okButton.Location = new Point(228, 132);
                okButton.Size = new Size(92, 36);
                StyleDialogButton(okButton, true);

                cancelButton.Text = "취소";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Location = new Point(328, 132);
                cancelButton.Size = new Size(92, 36);
                StyleDangerButton(cancelButton);

                dialog.Controls.Add(messageLabel);
                dialog.Controls.Add(deleteAllCheckBox);
                dialog.Controls.Add(okButton);
                dialog.Controls.Add(cancelButton);
                dialog.AcceptButton = okButton;
                dialog.CancelButton = cancelButton;

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return false;
                }

                deleteAllCategories = deleteAllCheckBox.Checked;
                return true;
            }
        }

        private void RemoveBlockItemFromList(List<string> blockedItems, string item)
        {
            blockedItems.RemoveAll(blockedItem => string.Equals(blockedItem, item, StringComparison.OrdinalIgnoreCase));
        }

        private string NormalizeProcessName(string rawProcessName)
        {
            string value = (rawProcessName ?? string.Empty).Trim().Trim('"');
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            try
            {
                if (value.IndexOfAny(Path.GetInvalidPathChars()) < 0 && (value.Contains("\\") || value.Contains("/")))
                {
                    value = Path.GetFileNameWithoutExtension(value);
                }
                else if (value.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    value = Path.GetFileNameWithoutExtension(value);
                }
            }
            catch
            {
                if (value.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    value = value.Substring(0, value.Length - 4);
                }
            }

            return value.Trim();
        }

        private bool IsProcessNameFormatValid(string processName)
        {
            if (string.IsNullOrWhiteSpace(processName))
            {
                return false;
            }

            if (processName.Equals(".", StringComparison.Ordinal) ||
                processName.Equals("..", StringComparison.Ordinal))
            {
                return false;
            }

            return processName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 &&
                   !processName.Contains("\\") &&
                   !processName.Contains("/");
        }

        private bool IsKnownBlockItem(string processName)
        {
            return AllBlockableItems.Any(item => string.Equals(item, processName, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsExistingExecutablePath(string rawProcessName)
        {
            string value = (rawProcessName ?? string.Empty).Trim().Trim('"');
            if (string.IsNullOrWhiteSpace(value) ||
                (!value.Contains("\\") && !value.Contains("/")) ||
                !value.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            try
            {
                return File.Exists(value);
            }
            catch
            {
                return false;
            }
        }

        private bool IsCurrentlyRunningProcess(string processName)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(processName);
                try
                {
                    return processes.Length > 0;
                }
                finally
                {
                    foreach (Process process in processes)
                    {
                        process.Dispose();
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private string ShowRunningProcessPicker()
        {
            List<RunningProcessOption> options = GetRunningProcessOptions();
            if (options.Count == 0)
            {
                AlertDialog.Show(this, "선택할 수 있는 실행 중인 앱이 없습니다.");
                return string.Empty;
            }

            using (Form dialog = new Form())
            using (Label title = new Label())
            using (ListBox processList = new ListBox())
            using (Button selectButton = new Button())
            using (Button cancelButton = new Button())
            {
                dialog.Text = "실행 중인 앱 선택";
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MinimizeBox = false;
                dialog.MaximizeBox = false;
                dialog.BackColor = Color.FromArgb(18, 18, 18);
                dialog.ForeColor = Color.White;
                dialog.ClientSize = new Size(520, 420);

                title.Text = "차단할 실행 중인 앱을 선택하세요.";
                title.Font = new Font("맑은 고딕", 11F, FontStyle.Bold);
                title.ForeColor = Color.White;
                title.Location = new Point(18, 18);
                title.Size = new Size(484, 32);

                processList.BackColor = Color.FromArgb(35, 35, 35);
                processList.BorderStyle = BorderStyle.FixedSingle;
                processList.DisplayMember = "DisplayText";
                processList.ForeColor = Color.White;
                processList.Font = new Font("맑은 고딕", 9F);
                processList.ItemHeight = 24;
                processList.Location = new Point(18, 62);
                processList.Size = new Size(484, 276);
                processList.DataSource = options;
                processList.DoubleClick += delegate
                {
                    if (processList.SelectedItem != null)
                    {
                        dialog.DialogResult = DialogResult.OK;
                        dialog.Close();
                    }
                };

                selectButton.Text = "선택";
                selectButton.DialogResult = DialogResult.OK;
                selectButton.Location = new Point(298, 358);
                selectButton.Size = new Size(96, 38);
                StyleDialogButton(selectButton, true);

                cancelButton.Text = "취소";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Location = new Point(406, 358);
                cancelButton.Size = new Size(96, 38);
                StyleDangerButton(cancelButton);

                dialog.Controls.Add(title);
                dialog.Controls.Add(processList);
                dialog.Controls.Add(selectButton);
                dialog.Controls.Add(cancelButton);
                dialog.AcceptButton = selectButton;
                dialog.CancelButton = cancelButton;

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return string.Empty;
                }

                RunningProcessOption selected = processList.SelectedItem as RunningProcessOption;
                return selected == null ? string.Empty : selected.ProcessName;
            }
        }

        private List<RunningProcessOption> GetRunningProcessOptions()
        {
            int currentProcessId;
            using (Process currentProcess = Process.GetCurrentProcess())
            {
                currentProcessId = currentProcess.Id;
            }

            List<RunningProcessOption> options = new List<RunningProcessOption>();
            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    if (process.Id == currentProcessId || string.IsNullOrWhiteSpace(process.ProcessName))
                    {
                        continue;
                    }

                    string windowTitle = process.MainWindowTitle;
                    if (string.IsNullOrWhiteSpace(windowTitle))
                    {
                        continue;
                    }

                    options.Add(new RunningProcessOption(process.ProcessName, windowTitle));
                }
                catch
                {
                }
                finally
                {
                    process.Dispose();
                }
            }

            return options
                .GroupBy(option => option.ProcessName, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.OrderBy(option => option.WindowTitle).First())
                .OrderBy(option => option.ProcessName)
                .ToList();
        }

        private void BlockedAppsManagementForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveCurrentModeChanges();
            DialogResult = DialogResult.OK;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveCurrentModeChanges();
            if (!ConfirmUnverifiedCustomProcesses())
            {
                return;
            }

            AlertDialog.Show(this, "변경사항이 성공적으로 저장되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }

        private bool ConfirmUnverifiedCustomProcesses()
        {
            List<string> unverifiedItems = modeBlockedItems.Values
                .Where(items => items != null)
                .SelectMany(items => items)
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => item.Trim())
                .Where(item => !IsKnownBlockItem(item))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Where(item => !IsCurrentlyRunningProcess(item))
                .OrderBy(item => item)
                .ToList();

            if (unverifiedItems.Count == 0)
            {
                return true;
            }

            string itemList = string.Join(", ", unverifiedItems.Take(8));
            if (unverifiedItems.Count > 8)
            {
                itemList += " 외 " + (unverifiedItems.Count - 8) + "개";
            }

            DialogResult result = AlertDialog.Show(
                this,
                "현재 실행 중인 프로세스에서 찾지 못한 직접 추가 항목이 있습니다.\r\n" +
                itemList + "\r\n\r\n" +
                "앱이 꺼져 있어서 확인되지 않는 것일 수도 있지만, 오타라면 차단이 동작하지 않습니다.\r\n그래도 저장할까요?",
                "검증되지 않은 프로세스",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning);

            return result == DialogResult.OK;
        }

        private void StyleModeButtons()
        {
            foreach (Button button in GetModeButtons())
            {
                StyleCategoryButton(button);
            }

            StyleDialogButton(btnAddCategory, false);
            btnAddCategory.FlatAppearance.MouseDownBackColor = AccentFill;
        }

        private void RefreshCustomCategoryButtons()
        {
            foreach (Button button in customCategoryButtons)
            {
                pnlModes.Controls.Remove(button);
                button.Dispose();
            }

            customCategoryButtons.Clear();

            int top = 415;
            foreach (string category in modeBlockedItems.Keys
                         .Where(category => !DefaultCategories.Contains(category, StringComparer.OrdinalIgnoreCase))
                         .OrderBy(category => category))
            {
                Button button = new Button();
                button.Name = "btnCustomCategory_" + customCategoryButtons.Count;
                button.Text = category;
                button.TextAlign = ContentAlignment.MiddleLeft;
                button.Location = new Point(38, top);
                button.Size = new Size(205, 45);
                button.Font = new Font("맑은 고딕", 9F);
                button.Tag = category;
                button.Click += customCategoryButton_Click;
                StyleCategoryButton(button);

                customCategoryButtons.Add(button);
                pnlModes.Controls.Add(button);
                top += 57;
            }
        }

        private void customCategoryButton_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string category = button == null ? string.Empty : button.Tag as string;
            if (!string.IsNullOrWhiteSpace(category))
            {
                SelectMode(category);
            }
        }

        private void StyleCategoryButton(Button button)
        {
            button.BackColor = MenuFill;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = MenuHoverFill;
            button.FlatAppearance.MouseDownBackColor = AccentFill;
            button.FlatStyle = FlatStyle.Flat;
            button.ForeColor = Color.White;
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.UseVisualStyleBackColor = false;
        }

        private void StyleCustomAppControls()
        {
            txtCustomProcessName.BackColor = Color.FromArgb(35, 35, 35);
            txtCustomProcessName.ForeColor = Color.White;
            StyleDialogButton(btnAddCustomApp, true);
            StyleDialogButton(btnPickRunningApp, false);
        }

        private void StyleDialogButton(Button button, bool primary)
        {
            Color fill = primary ? AccentFill : Color.FromArgb(35, 35, 35);
            Color hover = primary ? Color.FromArgb(124, 58, 237) : MenuHoverFill;
            button.BackColor = fill;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = hover;
            button.FlatAppearance.MouseDownBackColor = AccentFill;
            button.FlatStyle = FlatStyle.Flat;
            button.ForeColor = Color.White;
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.UseVisualStyleBackColor = false;
        }

        private void StyleDangerButton(Button button)
        {
            button.BackColor = Color.FromArgb(239, 68, 68);
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 38, 38);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(185, 28, 28);
            button.FlatStyle = FlatStyle.Flat;
            button.ForeColor = Color.White;
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.UseVisualStyleBackColor = false;
        }

        private void UpdateModeButtonStates()
        {
            foreach (Button button in GetModeButtons())
            {
                string category = button.Tag as string ?? button.Text;
                bool isSelected = string.Equals(category, currentSelectedMode, StringComparison.Ordinal);
                button.BackColor = isSelected ? AccentFill : MenuFill;
            }
        }

        private IEnumerable<Button> GetModeButtons()
        {
            yield return btnSelectStudent;
            yield return btnSelectDeveloper;
            yield return btnSelectEditor;
            yield return btnSelectExaminee;

            foreach (Button button in customCategoryButtons)
            {
                yield return button;
            }
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

        private sealed class RunningProcessOption
        {
            public RunningProcessOption(string processName, string windowTitle)
            {
                ProcessName = processName;
                WindowTitle = windowTitle;
            }

            public string ProcessName { get; private set; }

            public string WindowTitle { get; private set; }

            public string DisplayText
            {
                get { return ProcessName + " - " + WindowTitle; }
            }
        }
    }
}
