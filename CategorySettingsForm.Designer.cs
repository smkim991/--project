namespace Prototype1.UI
{
    partial class CategorySettingsForm
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
            this.btnSelectStudent = new System.Windows.Forms.Button();
            this.btnSelectExaminee = new System.Windows.Forms.Button();
            this.btnSelectEditor = new System.Windows.Forms.Button();
            this.btnSelectDeveloper = new System.Windows.Forms.Button();
            this.lblBlockedItemsDisplay = new System.Windows.Forms.Label();
            this.btnConfirmSelection = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSelectStudent
            // 
            this.btnSelectStudent.Location = new System.Drawing.Point(170, 82);
            this.btnSelectStudent.Name = "btnSelectStudent";
            this.btnSelectStudent.Size = new System.Drawing.Size(176, 127);
            this.btnSelectStudent.TabIndex = 0;
            this.btnSelectStudent.Text = "대학생";
            this.btnSelectStudent.UseVisualStyleBackColor = true;
            this.btnSelectStudent.Click += new System.EventHandler(this.CategoryButton_Click);
            // 
            // btnSelectExaminee
            // 
            this.btnSelectExaminee.Location = new System.Drawing.Point(352, 215);
            this.btnSelectExaminee.Name = "btnSelectExaminee";
            this.btnSelectExaminee.Size = new System.Drawing.Size(176, 127);
            this.btnSelectExaminee.TabIndex = 1;
            this.btnSelectExaminee.Text = "수험생";
            this.btnSelectExaminee.UseVisualStyleBackColor = true;
            this.btnSelectExaminee.Click += new System.EventHandler(this.CategoryButton_Click);
            // 
            // btnSelectEditor
            // 
            this.btnSelectEditor.Location = new System.Drawing.Point(170, 215);
            this.btnSelectEditor.Name = "btnSelectEditor";
            this.btnSelectEditor.Size = new System.Drawing.Size(176, 127);
            this.btnSelectEditor.TabIndex = 2;
            this.btnSelectEditor.Text = "영상편집자";
            this.btnSelectEditor.UseVisualStyleBackColor = true;
            this.btnSelectEditor.Click += new System.EventHandler(this.CategoryButton_Click);
            // 
            // btnSelectDeveloper
            // 
            this.btnSelectDeveloper.Location = new System.Drawing.Point(352, 82);
            this.btnSelectDeveloper.Name = "btnSelectDeveloper";
            this.btnSelectDeveloper.Size = new System.Drawing.Size(176, 127);
            this.btnSelectDeveloper.TabIndex = 3;
            this.btnSelectDeveloper.Text = "개발자";
            this.btnSelectDeveloper.UseVisualStyleBackColor = true;
            this.btnSelectDeveloper.Click += new System.EventHandler(this.CategoryButton_Click);
            // 
            // lblBlockedItemsDisplay
            // 
            this.lblBlockedItemsDisplay.AutoSize = true;
            this.lblBlockedItemsDisplay.Location = new System.Drawing.Point(170, 365);
            this.lblBlockedItemsDisplay.Name = "lblBlockedItemsDisplay";
            this.lblBlockedItemsDisplay.Size = new System.Drawing.Size(188, 18);
            this.lblBlockedItemsDisplay.TabIndex = 4;
            this.lblBlockedItemsDisplay.Text = "lblBlockedItemsDisplay";
            this.lblBlockedItemsDisplay.Click += new System.EventHandler(this.lblBlockedItemsDisplay_Click);
            // 
            // btnConfirmSelection
            // 
            this.btnConfirmSelection.Enabled = false;
            this.btnConfirmSelection.Location = new System.Drawing.Point(610, 309);
            this.btnConfirmSelection.Name = "btnConfirmSelection";
            this.btnConfirmSelection.Size = new System.Drawing.Size(137, 51);
            this.btnConfirmSelection.TabIndex = 5;
            this.btnConfirmSelection.Text = "확인";
            this.btnConfirmSelection.UseVisualStyleBackColor = true;
            this.btnConfirmSelection.Click += new System.EventHandler(this.btnConfirmSelection_Click);
            // 
            // CategorySettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnConfirmSelection);
            this.Controls.Add(this.lblBlockedItemsDisplay);
            this.Controls.Add(this.btnSelectDeveloper);
            this.Controls.Add(this.btnSelectEditor);
            this.Controls.Add(this.btnSelectExaminee);
            this.Controls.Add(this.btnSelectStudent);
            this.Name = "CategorySettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "카테고리 설정";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnSelectStudent;
        private System.Windows.Forms.Button btnSelectExaminee;
        private System.Windows.Forms.Button btnSelectEditor;
        private System.Windows.Forms.Button btnSelectDeveloper;
        private System.Windows.Forms.Label lblBlockedItemsDisplay;
        private System.Windows.Forms.Button btnConfirmSelection;
    }
}
