using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Prototype1.UI
{
    public partial class StudyPlanForm : Form
    {
        private void EnsureDefaultStudyFile()
        {
            string defaultFilePath = Path.Combine(defaultStudyFolderPath, "학습 메모.txt");
            string legacyWorkspaceMemoPath = Path.Combine(studyWorkspacePath, "학습 메모.txt");

            if (File.Exists(defaultFilePath))
            {
                return;
            }

            if (File.Exists(legacyWorkspaceMemoPath))
            {
                File.Move(legacyWorkspaceMemoPath, defaultFilePath);
                return;
            }

            string legacyMemoPath = Path.Combine(Application.UserAppDataPath, LegacyMemoFileName);
            string initialText = string.Empty;
            if (File.Exists(legacyMemoPath))
            {
                try
                {
                    initialText = File.ReadAllText(legacyMemoPath);
                }
                catch
                {
                    initialText = string.Empty;
                }
            }

            File.WriteAllText(defaultFilePath, initialText);
        }

        private void RefreshStudyTree()
        {
            studyTreeView.BeginUpdate();
            studyTreeView.Nodes.Clear();

            foreach (string directory in Directory.GetDirectories(studyWorkspacePath).OrderBy(Path.GetFileName))
            {
                TreeNode directoryNode = CreateDirectoryNode(directory, Path.GetFileName(directory));
                studyTreeView.Nodes.Add(directoryNode);
                PopulateDirectoryNode(directoryNode);
                directoryNode.Expand();
            }

            foreach (string file in Directory.GetFiles(studyWorkspacePath).OrderBy(Path.GetFileName))
            {
                studyTreeView.Nodes.Add(CreateFileNode(file));
            }

            studyTreeView.EndUpdate();
        }

        private TreeNode CreateDirectoryNode(string path, string name)
        {
            TreeNode node = new TreeNode(name);
            node.Tag = path;
            node.NodeFont = new Font(studyTreeView.Font, FontStyle.Bold);
            return node;
        }

        private TreeNode CreateFileNode(string path)
        {
            TreeNode node = new TreeNode(Path.GetFileName(path));
            node.Tag = path;
            return node;
        }

        private void PopulateDirectoryNode(TreeNode directoryNode)
        {
            string path = directoryNode.Tag as string;
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                return;
            }

            foreach (string directory in Directory.GetDirectories(path).OrderBy(Path.GetFileName))
            {
                TreeNode child = CreateDirectoryNode(directory, Path.GetFileName(directory));
                directoryNode.Nodes.Add(child);
                PopulateDirectoryNode(child);
            }

            foreach (string file in Directory.GetFiles(path).OrderBy(Path.GetFileName))
            {
                directoryNode.Nodes.Add(CreateFileNode(file));
            }
        }

        private void SelectFirstFileNode()
        {
            TreeNode firstFile = null;
            foreach (TreeNode node in studyTreeView.Nodes)
            {
                firstFile = FindFirstFileNode(node);
                if (firstFile != null)
                {
                    break;
                }
            }

            if (firstFile != null)
            {
                studyTreeView.SelectedNode = firstFile;
            }
        }

        private TreeNode FindFirstFileNode(TreeNode node)
        {
            if (node == null)
            {
                return null;
            }

            string path = node.Tag as string;
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                return node;
            }

            foreach (TreeNode child in node.Nodes)
            {
                TreeNode found = FindFirstFileNode(child);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private void studyTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            LoadSelectedNodeFile();
        }

        private void studyTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            lastTreeMouseLocation = e.Location;
            hasLastTreeMouseLocation = true;

            TreeNode node = studyTreeView.GetNodeAt(e.Location) ?? FindVisibleNodeAtY(e.Y);
            studyTreeView.SelectedNode = node;
        }

        private void studyTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                studyTreeView.SelectedNode = e.Node;
            }
        }

        private TreeNode FindVisibleNodeAtY(int y)
        {
            foreach (TreeNode node in EnumerateVisibleNodes())
            {
                if (y >= node.Bounds.Top && y <= node.Bounds.Bottom)
                {
                    return node;
                }
            }

            return null;
        }

        private IEnumerable<TreeNode> EnumerateVisibleNodes()
        {
            foreach (TreeNode root in studyTreeView.Nodes)
            {
                foreach (TreeNode node in EnumerateVisibleNodes(root))
                {
                    yield return node;
                }
            }
        }

        private IEnumerable<TreeNode> EnumerateVisibleNodes(TreeNode node)
        {
            if (node == null || !node.IsVisible)
            {
                yield break;
            }

            yield return node;

            if (!node.IsExpanded)
            {
                yield break;
            }

            foreach (TreeNode child in node.Nodes)
            {
                foreach (TreeNode visibleChild in EnumerateVisibleNodes(child))
                {
                    yield return visibleChild;
                }
            }
        }

        private void treeContextMenu_Opening(object sender, CancelEventArgs e)
        {
            TreeNode selected = studyTreeView.SelectedNode;
            bool hasSelection = selected != null;
            bool isRoot = hasSelection && IsWorkspaceRoot(selected.Tag as string);

            treeContextMenu.Items[0].Enabled = true;
            treeContextMenu.Items[1].Enabled = true;
            treeContextMenu.Items[2].Enabled = hasSelection && !isRoot;
            treeContextMenu.Items[4].Enabled = hasSelection && !isRoot;
        }

        private void LoadSelectedNodeFile()
        {
            SaveCurrentFile();

            TreeNode selected = studyTreeView.SelectedNode;
            string path = selected == null ? null : selected.Tag as string;

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                currentFilePath = null;
                isLoadingFile = true;
                txtMemo.Text = "폴더를 선택했습니다. 왼쪽 사이드바에서 파일을 선택하거나 마우스 오른쪽 버튼으로 새 파일을 추가하세요.";
                isLoadingFile = false;
                txtMemo.Enabled = false;
                btnSaveFile.Enabled = false;
                btnStartFocusFromPlan.Enabled = false;
                lblFileTitle.Text = selected == null ? "파일을 선택하세요" : selected.Text;
                return;
            }

            try
            {
                currentFilePath = path;
                isLoadingFile = true;
                txtMemo.Text = File.ReadAllText(path);
                isLoadingFile = false;
                isFileDirty = false;
                txtMemo.Enabled = true;
                btnSaveFile.Enabled = false;
                btnStartFocusFromPlan.Enabled = true;
                lblFileTitle.Text = Path.GetFileName(path);
                dashboardTabs.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("파일을 불러오는 데 실패했습니다: " + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isLoadingFile = false;
            }
        }

        private void txtMemo_TextChanged(object sender, EventArgs e)
        {
            if (isLoadingFile || string.IsNullOrWhiteSpace(currentFilePath))
            {
                return;
            }

            isFileDirty = true;
            btnSaveFile.Enabled = true;
        }

        private void SaveCurrentFile()
        {
            if (!isFileDirty || string.IsNullOrWhiteSpace(currentFilePath) || !File.Exists(currentFilePath))
            {
                return;
            }

            try
            {
                File.WriteAllText(currentFilePath, txtMemo.Text);
                isFileDirty = false;
                btnSaveFile.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("파일을 저장하는 데 실패했습니다: " + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddFolderFromSelection()
        {
            string parentDirectory = GetTargetDirectoryForCreate();
            string name = PromptForName("폴더 추가", "폴더 이름", "새 폴더");
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            string folderPath = GetUniquePath(parentDirectory, SanitizeFileName(name), false);
            Directory.CreateDirectory(folderPath);
            RefreshStudyTree();
            SelectPath(folderPath);
        }

        private void AddFileFromSelection()
        {
            string parentDirectory = GetTargetDirectoryForCreate();
            string name = PromptForName("파일 추가", "파일 이름", "새 파일");
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            string fileName = SanitizeFileName(name);
            string filePath = GetUniquePath(parentDirectory, fileName, true);
            File.WriteAllText(filePath, string.Empty);
            RefreshStudyTree();
            SelectPath(filePath);
        }

        private void RenameSelectedNode()
        {
            TreeNode selected = studyTreeView.SelectedNode;
            string path = selected == null ? null : selected.Tag as string;
            if (string.IsNullOrWhiteSpace(path) || IsWorkspaceRoot(path))
            {
                return;
            }

            string currentName = File.Exists(path) ? Path.GetFileName(path) : new DirectoryInfo(path).Name;
            string newName = PromptForName("이름 변경", "새 이름", currentName);
            if (string.IsNullOrWhiteSpace(newName))
            {
                return;
            }

            string parent = Path.GetDirectoryName(path);
            string sanitized = SanitizeFileName(newName);

            string destination = Path.Combine(parent, sanitized);
            if (string.Equals(path, destination, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (File.Exists(destination) || Directory.Exists(destination))
            {
                MessageBox.Show("같은 이름의 파일 또는 폴더가 이미 있습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveCurrentFile();
            if (File.Exists(path))
            {
                File.Move(path, destination);
            }
            else if (Directory.Exists(path))
            {
                Directory.Move(path, destination);
            }

            RefreshStudyTree();
            SelectPath(destination);
        }

        private void DeleteSelectedNode()
        {
            TreeNode selected = studyTreeView.SelectedNode;
            string path = selected == null ? null : selected.Tag as string;
            if (string.IsNullOrWhiteSpace(path) || IsWorkspaceRoot(path))
            {
                return;
            }

            DialogResult result = MessageBox.Show(
                selected.Text + " 항목을 삭제할까요?",
                "삭제 확인",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning);

            if (result != DialogResult.OK)
            {
                return;
            }

            SaveCurrentFile();

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            if (IsCurrentFileAffectedByDelete(path))
            {
                currentFilePath = null;
                isFileDirty = false;
                txtMemo.Text = string.Empty;
                txtMemo.Enabled = false;
                btnSaveFile.Enabled = false;
                btnStartFocusFromPlan.Enabled = false;
            }

            RefreshStudyTree();
            SelectFirstFileNode();
        }

        private bool IsCurrentFileAffectedByDelete(string deletedPath)
        {
            if (string.IsNullOrWhiteSpace(currentFilePath) || string.IsNullOrWhiteSpace(deletedPath))
            {
                return false;
            }

            if (File.Exists(deletedPath))
            {
                return string.Equals(currentFilePath, deletedPath, StringComparison.OrdinalIgnoreCase);
            }

            string normalizedFile = Path.GetFullPath(currentFilePath);
            string normalizedFolder = Path.GetFullPath(deletedPath).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            return normalizedFile.StartsWith(normalizedFolder, StringComparison.OrdinalIgnoreCase);
        }

        private string GetTargetDirectoryForCreate()
        {
            if (!hasLastTreeMouseLocation)
            {
                return GetSelectedDirectory();
            }

            TreeNode clickedNode = studyTreeView.GetNodeAt(lastTreeMouseLocation) ?? FindVisibleNodeAtY(lastTreeMouseLocation.Y);
            return clickedNode == null ? studyWorkspacePath : GetDirectoryForNode(clickedNode);
        }

        private string GetDirectoryForNode(TreeNode node)
        {
            if (node == null)
            {
                return studyWorkspacePath;
            }

            string path = node.Tag as string;
            if (string.IsNullOrWhiteSpace(path))
            {
                return studyWorkspacePath;
            }

            if (Directory.Exists(path))
            {
                return path;
            }

            if (File.Exists(path))
            {
                return Path.GetDirectoryName(path);
            }

            return studyWorkspacePath;
        }

        private string GetSelectedDirectory()
        {
            TreeNode selected = studyTreeView.SelectedNode;
            return GetDirectoryForNode(selected);
        }

        private bool IsWorkspaceRoot(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            return string.Equals(
                Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar),
                Path.GetFullPath(studyWorkspacePath).TrimEnd(Path.DirectorySeparatorChar),
                StringComparison.OrdinalIgnoreCase);
        }

        private string SanitizeFileName(string name)
        {
            string value = string.IsNullOrWhiteSpace(name) ? "새 파일" : name.Trim();
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                value = value.Replace(invalidChar, '_');
            }

            return value;
        }

        private string GetUniquePath(string directory, string name, bool isFile)
        {
            string candidate = Path.Combine(directory, name);
            if (!File.Exists(candidate) && !Directory.Exists(candidate))
            {
                return candidate;
            }

            string fileName = isFile ? Path.GetFileNameWithoutExtension(name) : name;
            string extension = isFile ? Path.GetExtension(name) : string.Empty;

            for (int i = 2; i < 1000; i++)
            {
                candidate = Path.Combine(directory, fileName + " " + i + extension);
                if (!File.Exists(candidate) && !Directory.Exists(candidate))
                {
                    return candidate;
                }
            }

            return Path.Combine(directory, Guid.NewGuid().ToString("N") + extension);
        }

        private void SelectPath(string path)
        {
            foreach (TreeNode rootNode in studyTreeView.Nodes)
            {
                TreeNode node = FindNodeByPath(rootNode, path);
                if (node != null)
                {
                    studyTreeView.SelectedNode = node;
                    node.EnsureVisible();
                    return;
                }
            }
        }

        private TreeNode FindNodeByPath(TreeNode node, string path)
        {
            if (node == null)
            {
                return null;
            }

            string nodePath = node.Tag as string;
            if (!string.IsNullOrWhiteSpace(nodePath) && string.Equals(nodePath, path, StringComparison.OrdinalIgnoreCase))
            {
                return node;
            }

            foreach (TreeNode child in node.Nodes)
            {
                TreeNode found = FindNodeByPath(child, path);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private string PromptForName(string title, string label, string defaultValue)
        {
            using (Form dialog = new Form())
            using (Label prompt = new Label())
            using (TextBox input = new TextBox())
            using (Button okButton = new Button())
            using (Button cancelButton = new Button())
            {
                dialog.Text = title;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MinimizeBox = false;
                dialog.MaximizeBox = false;
                dialog.ClientSize = new Size(360, 130);

                prompt.Text = label;
                prompt.Location = new Point(14, 14);
                prompt.Size = new Size(330, 22);

                input.Text = defaultValue;
                input.Location = new Point(14, 42);
                input.Size = new Size(330, 26);
                input.SelectAll();

                okButton.Text = "확인";
                okButton.DialogResult = DialogResult.OK;
                okButton.Location = new Point(180, 84);
                okButton.Size = new Size(78, 30);

                cancelButton.Text = "취소";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Location = new Point(266, 84);
                cancelButton.Size = new Size(78, 30);

                dialog.Controls.Add(prompt);
                dialog.Controls.Add(input);
                dialog.Controls.Add(okButton);
                dialog.Controls.Add(cancelButton);
                dialog.AcceptButton = okButton;
                dialog.CancelButton = cancelButton;

                return dialog.ShowDialog(this) == DialogResult.OK ? input.Text.Trim() : string.Empty;
            }
        }

    }
}
