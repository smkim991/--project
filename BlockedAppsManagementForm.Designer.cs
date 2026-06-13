namespace Prototype1.UI
{
    partial class BlockedAppsManagementForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlModes = new System.Windows.Forms.Panel();
            this.lblSidebarTitle = new System.Windows.Forms.Label();
            this.btnAddCategory = new System.Windows.Forms.Button();
            this.btnSelectExaminee = new System.Windows.Forms.Button();
            this.btnSelectEditor = new System.Windows.Forms.Button();
            this.btnSelectDeveloper = new System.Windows.Forms.Button();
            this.btnSelectStudent = new System.Windows.Forms.Button();
            this.pnlBlockedItems = new System.Windows.Forms.Panel();
            this.btnPickRunningApp = new System.Windows.Forms.Button();
            this.btnAddCustomApp = new System.Windows.Forms.Button();
            this.txtCustomProcessName = new System.Windows.Forms.TextBox();
            this.lblHelpText = new System.Windows.Forms.Label();
            this.lblBlockedItemsTitle = new System.Windows.Forms.Label();
            this.clbBlockableItems = new System.Windows.Forms.CheckedListBox();
            this.lblCurrentModeDisplay = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlModes.SuspendLayout();
            this.pnlBlockedItems.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlModes
            //
            this.pnlModes.AutoScroll = true;
            this.pnlModes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.pnlModes.Controls.Add(this.lblSidebarTitle);
            this.pnlModes.Controls.Add(this.btnAddCategory);
            this.pnlModes.Controls.Add(this.btnSelectExaminee);
            this.pnlModes.Controls.Add(this.btnSelectEditor);
            this.pnlModes.Controls.Add(this.btnSelectDeveloper);
            this.pnlModes.Controls.Add(this.btnSelectStudent);
            this.pnlModes.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlModes.Location = new System.Drawing.Point(0, 0);
            this.pnlModes.Name = "pnlModes";
            this.pnlModes.Size = new System.Drawing.Size(279, 620);
            this.pnlModes.TabIndex = 0;
            //
            // lblSidebarTitle
            //
            this.lblSidebarTitle.AutoSize = true;
            this.lblSidebarTitle.Font = new System.Drawing.Font("맑은 고딕", 10.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblSidebarTitle.ForeColor = System.Drawing.Color.White;
            this.lblSidebarTitle.Location = new System.Drawing.Point(34, 38);
            this.lblSidebarTitle.Name = "lblSidebarTitle";
            this.lblSidebarTitle.Size = new System.Drawing.Size(206, 40);
            this.lblSidebarTitle.TabIndex = 4;
            this.lblSidebarTitle.Text = "Focus Blocker";
            //
            // btnAddCategory
            //
            this.btnAddCategory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnAddCategory.FlatAppearance.BorderSize = 0;
            this.btnAddCategory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddCategory.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.btnAddCategory.ForeColor = System.Drawing.Color.White;
            this.btnAddCategory.Location = new System.Drawing.Point(38, 358);
            this.btnAddCategory.Name = "btnAddCategory";
            this.btnAddCategory.Size = new System.Drawing.Size(205, 45);
            this.btnAddCategory.TabIndex = 5;
            this.btnAddCategory.Text = "+ 카테고리 추가";
            this.btnAddCategory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddCategory.UseVisualStyleBackColor = false;
            this.btnAddCategory.Click += new System.EventHandler(this.btnAddCategory_Click);
            //
            // btnSelectExaminee
            //
            this.btnSelectExaminee.FlatAppearance.BorderSize = 0;
            this.btnSelectExaminee.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectExaminee.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.btnSelectExaminee.ForeColor = System.Drawing.Color.White;
            this.btnSelectExaminee.Location = new System.Drawing.Point(38, 296);
            this.btnSelectExaminee.Name = "btnSelectExaminee";
            this.btnSelectExaminee.Size = new System.Drawing.Size(205, 45);
            this.btnSelectExaminee.TabIndex = 3;
            this.btnSelectExaminee.Text = "수험생";
            this.btnSelectExaminee.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSelectExaminee.UseVisualStyleBackColor = false;
            this.btnSelectExaminee.Click += new System.EventHandler(this.btnSelectExaminee_Click);
            //
            // btnSelectEditor
            //
            this.btnSelectEditor.FlatAppearance.BorderSize = 0;
            this.btnSelectEditor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectEditor.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.btnSelectEditor.ForeColor = System.Drawing.Color.White;
            this.btnSelectEditor.Location = new System.Drawing.Point(38, 239);
            this.btnSelectEditor.Name = "btnSelectEditor";
            this.btnSelectEditor.Size = new System.Drawing.Size(205, 45);
            this.btnSelectEditor.TabIndex = 2;
            this.btnSelectEditor.Text = "영상편집자";
            this.btnSelectEditor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSelectEditor.UseVisualStyleBackColor = false;
            this.btnSelectEditor.Click += new System.EventHandler(this.btnSelectEditor_Click);
            //
            // btnSelectDeveloper
            //
            this.btnSelectDeveloper.FlatAppearance.BorderSize = 0;
            this.btnSelectDeveloper.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectDeveloper.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.btnSelectDeveloper.ForeColor = System.Drawing.Color.White;
            this.btnSelectDeveloper.Location = new System.Drawing.Point(38, 182);
            this.btnSelectDeveloper.Name = "btnSelectDeveloper";
            this.btnSelectDeveloper.Size = new System.Drawing.Size(205, 45);
            this.btnSelectDeveloper.TabIndex = 1;
            this.btnSelectDeveloper.Text = "개발자";
            this.btnSelectDeveloper.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSelectDeveloper.UseVisualStyleBackColor = false;
            this.btnSelectDeveloper.Click += new System.EventHandler(this.btnSelectDeveloper_Click);
            //
            // btnSelectStudent
            //
            this.btnSelectStudent.FlatAppearance.BorderSize = 0;
            this.btnSelectStudent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectStudent.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.btnSelectStudent.ForeColor = System.Drawing.Color.White;
            this.btnSelectStudent.Location = new System.Drawing.Point(38, 125);
            this.btnSelectStudent.Name = "btnSelectStudent";
            this.btnSelectStudent.Size = new System.Drawing.Size(205, 45);
            this.btnSelectStudent.TabIndex = 0;
            this.btnSelectStudent.Text = "대학생";
            this.btnSelectStudent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSelectStudent.UseVisualStyleBackColor = false;
            this.btnSelectStudent.Click += new System.EventHandler(this.btnSelectStudent_Click);
            //
            // pnlBlockedItems
            //
            this.pnlBlockedItems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.pnlBlockedItems.Controls.Add(this.btnPickRunningApp);
            this.pnlBlockedItems.Controls.Add(this.btnAddCustomApp);
            this.pnlBlockedItems.Controls.Add(this.txtCustomProcessName);
            this.pnlBlockedItems.Controls.Add(this.lblHelpText);
            this.pnlBlockedItems.Controls.Add(this.lblBlockedItemsTitle);
            this.pnlBlockedItems.Controls.Add(this.clbBlockableItems);
            this.pnlBlockedItems.Location = new System.Drawing.Point(337, 123);
            this.pnlBlockedItems.Name = "pnlBlockedItems";
            this.pnlBlockedItems.Padding = new System.Windows.Forms.Padding(22);
            this.pnlBlockedItems.Size = new System.Drawing.Size(435, 430);
            this.pnlBlockedItems.TabIndex = 1;
            //
            // btnPickRunningApp
            //
            this.btnPickRunningApp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnPickRunningApp.FlatAppearance.BorderSize = 0;
            this.btnPickRunningApp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPickRunningApp.Font = new System.Drawing.Font("맑은 고딕", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnPickRunningApp.ForeColor = System.Drawing.Color.White;
            this.btnPickRunningApp.Location = new System.Drawing.Point(28, 375);
            this.btnPickRunningApp.Name = "btnPickRunningApp";
            this.btnPickRunningApp.Size = new System.Drawing.Size(379, 38);
            this.btnPickRunningApp.TabIndex = 6;
            this.btnPickRunningApp.Text = "실행 중인 앱에서 선택";
            this.btnPickRunningApp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnPickRunningApp.UseVisualStyleBackColor = false;
            this.btnPickRunningApp.Click += new System.EventHandler(this.btnPickRunningApp_Click);
            //
            // btnAddCustomApp
            //
            this.btnAddCustomApp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.btnAddCustomApp.FlatAppearance.BorderSize = 0;
            this.btnAddCustomApp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddCustomApp.Font = new System.Drawing.Font("맑은 고딕", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnAddCustomApp.ForeColor = System.Drawing.Color.White;
            this.btnAddCustomApp.Location = new System.Drawing.Point(278, 326);
            this.btnAddCustomApp.Name = "btnAddCustomApp";
            this.btnAddCustomApp.Size = new System.Drawing.Size(129, 38);
            this.btnAddCustomApp.TabIndex = 5;
            this.btnAddCustomApp.Text = "추가";
            this.btnAddCustomApp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnAddCustomApp.UseVisualStyleBackColor = false;
            this.btnAddCustomApp.Click += new System.EventHandler(this.btnAddCustomApp_Click);
            //
            // txtCustomProcessName
            //
            this.txtCustomProcessName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.txtCustomProcessName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCustomProcessName.Font = new System.Drawing.Font("맑은 고딕", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.txtCustomProcessName.ForeColor = System.Drawing.Color.White;
            this.txtCustomProcessName.Location = new System.Drawing.Point(28, 326);
            this.txtCustomProcessName.Name = "txtCustomProcessName";
            this.txtCustomProcessName.Size = new System.Drawing.Size(242, 38);
            this.txtCustomProcessName.TabIndex = 4;
            this.txtCustomProcessName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCustomProcessName_KeyDown);
            //
            // lblHelpText
            //
            this.lblHelpText.AutoSize = true;
            this.lblHelpText.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblHelpText.ForeColor = System.Drawing.Color.Silver;
            this.lblHelpText.Location = new System.Drawing.Point(24, 63);
            this.lblHelpText.Name = "lblHelpText";
            this.lblHelpText.Size = new System.Drawing.Size(309, 30);
            this.lblHelpText.TabIndex = 2;
            this.lblHelpText.Text = "선택한 모드에서 차단할 항목";
            //
            // lblBlockedItemsTitle
            //
            this.lblBlockedItemsTitle.AutoSize = true;
            this.lblBlockedItemsTitle.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblBlockedItemsTitle.ForeColor = System.Drawing.Color.White;
            this.lblBlockedItemsTitle.Location = new System.Drawing.Point(22, 20);
            this.lblBlockedItemsTitle.Name = "lblBlockedItemsTitle";
            this.lblBlockedItemsTitle.Size = new System.Drawing.Size(207, 45);
            this.lblBlockedItemsTitle.TabIndex = 1;
            this.lblBlockedItemsTitle.Text = "차단 항목";
            //
            // clbBlockableItems
            //
            this.clbBlockableItems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.clbBlockableItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbBlockableItems.CheckOnClick = true;
            this.clbBlockableItems.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.clbBlockableItems.ForeColor = System.Drawing.Color.White;
            this.clbBlockableItems.FormattingEnabled = true;
            this.clbBlockableItems.Location = new System.Drawing.Point(28, 107);
            this.clbBlockableItems.Name = "clbBlockableItems";
            this.clbBlockableItems.Size = new System.Drawing.Size(379, 166);
            this.clbBlockableItems.TabIndex = 0;
            this.clbBlockableItems.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbBlockableItems_ItemCheck);
            this.clbBlockableItems.MouseDown += new System.Windows.Forms.MouseEventHandler(this.clbBlockableItems_MouseDown);
            //
            // lblCurrentModeDisplay
            //
            this.lblCurrentModeDisplay.AutoSize = true;
            this.lblCurrentModeDisplay.Font = new System.Drawing.Font("맑은 고딕", 10.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblCurrentModeDisplay.ForeColor = System.Drawing.Color.White;
            this.lblCurrentModeDisplay.Location = new System.Drawing.Point(334, 74);
            this.lblCurrentModeDisplay.Name = "lblCurrentModeDisplay";
            this.lblCurrentModeDisplay.Size = new System.Drawing.Size(167, 40);
            this.lblCurrentModeDisplay.TabIndex = 2;
            this.lblCurrentModeDisplay.Text = "현재 모드";
            //
            // btnSave
            //
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(632, 555);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(140, 42);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "저장";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("맑은 고딕", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(329, 24);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(336, 59);
            this.lblTitle.TabIndex = 4;
            this.lblTitle.Text = "차단 앱 관리";
            //
            // BlockedAppsManagementForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.ClientSize = new System.Drawing.Size(840, 620);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblCurrentModeDisplay);
            this.Controls.Add(this.pnlBlockedItems);
            this.Controls.Add(this.pnlModes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "BlockedAppsManagementForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "차단 앱 관리";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BlockedAppsManagementForm_FormClosing);
            this.pnlModes.ResumeLayout(false);
            this.pnlModes.PerformLayout();
            this.pnlBlockedItems.ResumeLayout(false);
            this.pnlBlockedItems.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlModes;
        private System.Windows.Forms.Label lblSidebarTitle;
        private System.Windows.Forms.Button btnAddCategory;
        private System.Windows.Forms.Button btnSelectExaminee;
        private System.Windows.Forms.Button btnSelectEditor;
        private System.Windows.Forms.Button btnSelectDeveloper;
        private System.Windows.Forms.Button btnSelectStudent;
        private System.Windows.Forms.Panel pnlBlockedItems;
        private System.Windows.Forms.Button btnPickRunningApp;
        private System.Windows.Forms.Button btnAddCustomApp;
        private System.Windows.Forms.TextBox txtCustomProcessName;
        private System.Windows.Forms.Label lblHelpText;
        private System.Windows.Forms.Label lblBlockedItemsTitle;
        private System.Windows.Forms.CheckedListBox clbBlockableItems;
        private System.Windows.Forms.Label lblCurrentModeDisplay;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblTitle;
    }
}
