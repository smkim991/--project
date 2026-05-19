namespace WindowsFormsApp15
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCategorySettings = new System.Windows.Forms.Button();
            this.btnManageBlockedApps = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.btnSelectStudent = new System.Windows.Forms.Button();
            this.btnSelectExaminee = new System.Windows.Forms.Button();
            this.btnSelectEditor = new System.Windows.Forms.Button();
            this.btnSelectDeveloper = new System.Windows.Forms.Button();
            this.lblBlockedItemsDisplay = new System.Windows.Forms.Label();
            this.btnConfirmSelection = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCategorySettings
            // 
            this.btnCategorySettings.Location = new System.Drawing.Point(12, 70);
            this.btnCategorySettings.Name = "btnCategorySettings";
            this.btnCategorySettings.Size = new System.Drawing.Size(169, 53);
            this.btnCategorySettings.TabIndex = 0;
            this.btnCategorySettings.Text = "카테고리 설정";
            this.btnCategorySettings.UseVisualStyleBackColor = true;
            this.btnCategorySettings.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnManageBlockedApps
            // 
            this.btnManageBlockedApps.Location = new System.Drawing.Point(12, 129);
            this.btnManageBlockedApps.Name = "btnManageBlockedApps";
            this.btnManageBlockedApps.Size = new System.Drawing.Size(169, 53);
            this.btnManageBlockedApps.TabIndex = 1;
            this.btnManageBlockedApps.Text = "차단 앱 관리";
            this.btnManageBlockedApps.UseVisualStyleBackColor = true;
            this.btnManageBlockedApps.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(12, 188);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(169, 53);
            this.button3.TabIndex = 2;
            this.button3.Text = "학습 계획 및 관리";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnSelectStudent
            // 
            this.btnSelectStudent.Location = new System.Drawing.Point(270, 114);
            this.btnSelectStudent.Name = "btnSelectStudent";
            this.btnSelectStudent.Size = new System.Drawing.Size(176, 127);
            this.btnSelectStudent.TabIndex = 3;
            this.btnSelectStudent.Text = "대학생";
            this.btnSelectStudent.UseVisualStyleBackColor = true;
            this.btnSelectStudent.Click += new System.EventHandler(this.CategoryButton_Click);
            // 
            // btnSelectExaminee
            // 
            this.btnSelectExaminee.Location = new System.Drawing.Point(452, 247);
            this.btnSelectExaminee.Name = "btnSelectExaminee";
            this.btnSelectExaminee.Size = new System.Drawing.Size(176, 127);
            this.btnSelectExaminee.TabIndex = 4;
            this.btnSelectExaminee.Text = "수험생";
            this.btnSelectExaminee.UseVisualStyleBackColor = true;
            this.btnSelectExaminee.Click += new System.EventHandler(this.CategoryButton_Click);
            // 
            // btnSelectEditor
            // 
            this.btnSelectEditor.Location = new System.Drawing.Point(270, 247);
            this.btnSelectEditor.Name = "btnSelectEditor";
            this.btnSelectEditor.Size = new System.Drawing.Size(176, 127);
            this.btnSelectEditor.TabIndex = 5;
            this.btnSelectEditor.Text = "영상편집자";
            this.btnSelectEditor.UseVisualStyleBackColor = true;
            this.btnSelectEditor.Click += new System.EventHandler(this.CategoryButton_Click);
            // 
            // btnSelectDeveloper
            // 
            this.btnSelectDeveloper.Location = new System.Drawing.Point(452, 114);
            this.btnSelectDeveloper.Name = "btnSelectDeveloper";
            this.btnSelectDeveloper.Size = new System.Drawing.Size(176, 127);
            this.btnSelectDeveloper.TabIndex = 6;
            this.btnSelectDeveloper.Text = "개발자";
            this.btnSelectDeveloper.UseVisualStyleBackColor = true;
            this.btnSelectDeveloper.Click += new System.EventHandler(this.CategoryButton_Click);
            // 
            // lblBlockedItemsDisplay
            // 
            this.lblBlockedItemsDisplay.AutoSize = true;
            this.lblBlockedItemsDisplay.Location = new System.Drawing.Point(436, 381);
            this.lblBlockedItemsDisplay.Name = "lblBlockedItemsDisplay";
            this.lblBlockedItemsDisplay.Size = new System.Drawing.Size(188, 18);
            this.lblBlockedItemsDisplay.TabIndex = 7;
            this.lblBlockedItemsDisplay.Text = "lblBlockedItemsDisplay";
            this.lblBlockedItemsDisplay.Click += new System.EventHandler(this.lblBlockedItemsDisplay_Click);
            // 
            // btnConfirmSelection
            // 
            this.btnConfirmSelection.Enabled = false;
            this.btnConfirmSelection.Location = new System.Drawing.Point(651, 323);
            this.btnConfirmSelection.Name = "btnConfirmSelection";
            this.btnConfirmSelection.Size = new System.Drawing.Size(137, 51);
            this.btnConfirmSelection.TabIndex = 8;
            this.btnConfirmSelection.Text = "확인";
            this.btnConfirmSelection.UseVisualStyleBackColor = true;
            this.btnConfirmSelection.Click += new System.EventHandler(this.btnConfirmSelection_Click);
            // 
            // Form1
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
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnManageBlockedApps);
            this.Controls.Add(this.btnCategorySettings);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCategorySettings;
        private System.Windows.Forms.Button btnManageBlockedApps;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button btnSelectStudent;
        private System.Windows.Forms.Button btnSelectExaminee;
        private System.Windows.Forms.Button btnSelectEditor;
        private System.Windows.Forms.Button btnSelectDeveloper;
        private System.Windows.Forms.Label lblBlockedItemsDisplay;
        private System.Windows.Forms.Button btnConfirmSelection;
    }
}

