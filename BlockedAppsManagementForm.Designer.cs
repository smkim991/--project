namespace Prototype1.UI
{
    partial class BlockedAppsManagementForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlModes = new System.Windows.Forms.Panel();
            this.btnSelectExaminee = new System.Windows.Forms.Button();
            this.btnSelectEditor = new System.Windows.Forms.Button();
            this.btnSelectDeveloper = new System.Windows.Forms.Button();
            this.btnSelectStudent = new System.Windows.Forms.Button();
            this.pnlBlockedItems = new System.Windows.Forms.Panel();
            this.clbBlockableItems = new System.Windows.Forms.CheckedListBox();
            this.lblCurrentModeDisplay = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.pnlModes.SuspendLayout();
            this.pnlBlockedItems.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlModes
            // 
            this.pnlModes.Controls.Add(this.btnSelectExaminee);
            this.pnlModes.Controls.Add(this.btnSelectEditor);
            this.pnlModes.Controls.Add(this.btnSelectDeveloper);
            this.pnlModes.Controls.Add(this.btnSelectStudent);
            this.pnlModes.Location = new System.Drawing.Point(13, 13);
            this.pnlModes.Name = "pnlModes";
            this.pnlModes.Size = new System.Drawing.Size(200, 412);
            this.pnlModes.TabIndex = 0;
            // 
            // btnSelectExaminee
            // 
            this.btnSelectExaminee.Location = new System.Drawing.Point(4, 310);
            this.btnSelectExaminee.Name = "btnSelectExaminee";
            this.btnSelectExaminee.Size = new System.Drawing.Size(193, 96);
            this.btnSelectExaminee.TabIndex = 3;
            this.btnSelectExaminee.Text = "수험생";
            this.btnSelectExaminee.UseVisualStyleBackColor = true;
            this.btnSelectExaminee.Click += new System.EventHandler(this.btnSelectExaminee_Click);
            // 
            // btnSelectEditor
            // 
            this.btnSelectEditor.Location = new System.Drawing.Point(4, 208);
            this.btnSelectEditor.Name = "btnSelectEditor";
            this.btnSelectEditor.Size = new System.Drawing.Size(193, 96);
            this.btnSelectEditor.TabIndex = 2;
            this.btnSelectEditor.Text = "영상편집자";
            this.btnSelectEditor.UseVisualStyleBackColor = true;
            this.btnSelectEditor.Click += new System.EventHandler(this.btnSelectEditor_Click);
            // 
            // btnSelectDeveloper
            // 
            this.btnSelectDeveloper.Location = new System.Drawing.Point(4, 106);
            this.btnSelectDeveloper.Name = "btnSelectDeveloper";
            this.btnSelectDeveloper.Size = new System.Drawing.Size(193, 96);
            this.btnSelectDeveloper.TabIndex = 1;
            this.btnSelectDeveloper.Text = "개발자";
            this.btnSelectDeveloper.UseVisualStyleBackColor = true;
            this.btnSelectDeveloper.Click += new System.EventHandler(this.btnSelectDeveloper_Click);
            // 
            // btnSelectStudent
            // 
            this.btnSelectStudent.Location = new System.Drawing.Point(4, 4);
            this.btnSelectStudent.Name = "btnSelectStudent";
            this.btnSelectStudent.Size = new System.Drawing.Size(193, 96);
            this.btnSelectStudent.TabIndex = 0;
            this.btnSelectStudent.Text = "대학생";
            this.btnSelectStudent.UseVisualStyleBackColor = true;
            this.btnSelectStudent.Click += new System.EventHandler(this.btnSelectStudent_Click);
            // 
            // pnlBlockedItems
            // 
            this.pnlBlockedItems.Controls.Add(this.clbBlockableItems);
            this.pnlBlockedItems.Location = new System.Drawing.Point(389, 13);
            this.pnlBlockedItems.Name = "pnlBlockedItems";
            this.pnlBlockedItems.Size = new System.Drawing.Size(260, 412);
            this.pnlBlockedItems.TabIndex = 1;
            // 
            // clbBlockableItems
            // 
            this.clbBlockableItems.FormattingEnabled = true;
            this.clbBlockableItems.Location = new System.Drawing.Point(4, 25);
            this.clbBlockableItems.Name = "clbBlockableItems";
            this.clbBlockableItems.Size = new System.Drawing.Size(253, 379);
            this.clbBlockableItems.TabIndex = 0;
            this.clbBlockableItems.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbBlockableItems_ItemCheck);
            // 
            // lblCurrentModeDisplay
            // 
            this.lblCurrentModeDisplay.AutoSize = true;
            this.lblCurrentModeDisplay.Location = new System.Drawing.Point(219, 210);
            this.lblCurrentModeDisplay.Name = "lblCurrentModeDisplay";
            this.lblCurrentModeDisplay.Size = new System.Drawing.Size(54, 18);
            this.lblCurrentModeDisplay.TabIndex = 2;
            this.lblCurrentModeDisplay.Text = "label1";
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(656, 359);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(132, 60);
            this.button1.TabIndex = 3;
            this.button1.Text = "저장";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // BlockedAppsManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblCurrentModeDisplay);
            this.Controls.Add(this.pnlBlockedItems);
            this.Controls.Add(this.pnlModes);
            this.Name = "BlockedAppsManagementForm";
            this.Text = "차단 앱 관리";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BlockedAppsManagementForm_FormClosing);
            this.pnlModes.ResumeLayout(false);
            this.pnlBlockedItems.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlModes;
        private System.Windows.Forms.Button btnSelectExaminee;
        private System.Windows.Forms.Button btnSelectEditor;
        private System.Windows.Forms.Button btnSelectDeveloper;
        private System.Windows.Forms.Button btnSelectStudent;
        private System.Windows.Forms.Panel pnlBlockedItems;
        private System.Windows.Forms.CheckedListBox clbBlockableItems;
        private System.Windows.Forms.Label lblCurrentModeDisplay;
        private System.Windows.Forms.Button button1;
    }
}
