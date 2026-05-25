namespace Prototype1.UI
{
    partial class CategorySettingsForm2
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
            this.lblBlockedItemsDisplay = new System.Windows.Forms.Label();
            this.btnConfirmSelection = new System.Windows.Forms.Button();
            this.btnSelectStudent = new Guna.UI2.WinForms.Guna2Button();
            this.btnSelectDeveloper = new Guna.UI2.WinForms.Guna2Button();
            this.btnSelectEditor = new Guna.UI2.WinForms.Guna2Button();
            this.btnSelectExaminee = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.guna2ControlBox1 = new Guna.UI2.WinForms.Guna2ControlBox();
            this.guna2Panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblBlockedItemsDisplay
            // 
            this.lblBlockedItemsDisplay.AutoSize = true;
            this.lblBlockedItemsDisplay.Font = new System.Drawing.Font("맑은 고딕", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblBlockedItemsDisplay.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblBlockedItemsDisplay.Location = new System.Drawing.Point(420, 675);
            this.lblBlockedItemsDisplay.Name = "lblBlockedItemsDisplay";
            this.lblBlockedItemsDisplay.Size = new System.Drawing.Size(297, 37);
            this.lblBlockedItemsDisplay.TabIndex = 5;
            this.lblBlockedItemsDisplay.Text = "lblBlockedItemsDisplay";
            this.lblBlockedItemsDisplay.Click += new System.EventHandler(this.lblBlockedItemsDisplay_Click);
            // 
            // btnConfirmSelection
            // 
            this.btnConfirmSelection.Location = new System.Drawing.Point(863, 697);
            this.btnConfirmSelection.Name = "btnConfirmSelection";
            this.btnConfirmSelection.Size = new System.Drawing.Size(210, 47);
            this.btnConfirmSelection.TabIndex = 10;
            this.btnConfirmSelection.Text = "확인";
            this.btnConfirmSelection.UseVisualStyleBackColor = true;
            this.btnConfirmSelection.Click += new System.EventHandler(this.btnConfirmSelection_Click);
            // 
            // btnSelectStudent
            // 
            this.btnSelectStudent.Animated = true;
            this.btnSelectStudent.BackColor = System.Drawing.Color.Transparent;
            this.btnSelectStudent.BorderRadius = 20;
            this.btnSelectStudent.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnSelectStudent.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnSelectStudent.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnSelectStudent.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnSelectStudent.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnSelectStudent.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSelectStudent.ForeColor = System.Drawing.Color.White;
            this.btnSelectStudent.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.btnSelectStudent.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnSelectStudent.ImageSize = new System.Drawing.Size(64, 64);
            this.btnSelectStudent.Location = new System.Drawing.Point(176, 208);
            this.btnSelectStudent.Name = "btnSelectStudent";
            this.btnSelectStudent.ShadowDecoration.Enabled = true;
            this.btnSelectStudent.Size = new System.Drawing.Size(371, 180);
            this.btnSelectStudent.TabIndex = 11;
            this.btnSelectStudent.Text = "대학생";
            this.btnSelectStudent.Click += new System.EventHandler(this.CategoryButton_Click);
            // 
            // btnSelectDeveloper
            // 
            this.btnSelectDeveloper.BackColor = System.Drawing.Color.Transparent;
            this.btnSelectDeveloper.BorderRadius = 20;
            this.btnSelectDeveloper.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnSelectDeveloper.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnSelectDeveloper.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnSelectDeveloper.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnSelectDeveloper.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnSelectDeveloper.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectDeveloper.ForeColor = System.Drawing.Color.White;
            this.btnSelectDeveloper.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.btnSelectDeveloper.Location = new System.Drawing.Point(597, 428);
            this.btnSelectDeveloper.Name = "btnSelectDeveloper";
            this.btnSelectDeveloper.ShadowDecoration.Enabled = true;
            this.btnSelectDeveloper.Size = new System.Drawing.Size(361, 180);
            this.btnSelectDeveloper.TabIndex = 12;
            this.btnSelectDeveloper.Text = "개발자";
            this.btnSelectDeveloper.Click += new System.EventHandler(this.CategoryButton_Click);
            // 
            // btnSelectEditor
            // 
            this.btnSelectEditor.BackColor = System.Drawing.Color.Transparent;
            this.btnSelectEditor.BorderRadius = 20;
            this.btnSelectEditor.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnSelectEditor.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnSelectEditor.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnSelectEditor.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnSelectEditor.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnSelectEditor.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectEditor.ForeColor = System.Drawing.Color.White;
            this.btnSelectEditor.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.btnSelectEditor.Location = new System.Drawing.Point(176, 428);
            this.btnSelectEditor.Name = "btnSelectEditor";
            this.btnSelectEditor.ShadowDecoration.Enabled = true;
            this.btnSelectEditor.Size = new System.Drawing.Size(371, 180);
            this.btnSelectEditor.TabIndex = 13;
            this.btnSelectEditor.Text = "영상편집자";
            this.btnSelectEditor.Click += new System.EventHandler(this.CategoryButton_Click);
            // 
            // btnSelectExaminee
            // 
            this.btnSelectExaminee.BackColor = System.Drawing.Color.Transparent;
            this.btnSelectExaminee.BorderRadius = 20;
            this.btnSelectExaminee.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnSelectExaminee.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnSelectExaminee.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnSelectExaminee.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnSelectExaminee.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.btnSelectExaminee.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectExaminee.ForeColor = System.Drawing.Color.White;
            this.btnSelectExaminee.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.btnSelectExaminee.Location = new System.Drawing.Point(597, 208);
            this.btnSelectExaminee.Name = "btnSelectExaminee";
            this.btnSelectExaminee.ShadowDecoration.Enabled = true;
            this.btnSelectExaminee.Size = new System.Drawing.Size(361, 180);
            this.btnSelectExaminee.TabIndex = 14;
            this.btnSelectExaminee.Text = "수험생";
            this.btnSelectExaminee.Click += new System.EventHandler(this.CategoryButton_Click);
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.guna2Panel1.Controls.Add(this.guna2ControlBox1);
            this.guna2Panel1.Controls.Add(this.label6);
            this.guna2Panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.guna2Panel1.Location = new System.Drawing.Point(0, 0);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Size = new System.Drawing.Size(1222, 43);
            this.guna2Panel1.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 16.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(33, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(298, 59);
            this.label1.TabIndex = 16;
            this.label1.Text = "카테고리 설정";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(7, 5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(172, 32);
            this.label6.TabIndex = 17;
            this.label6.Text = "Focus Blocker";
            // 
            // guna2ControlBox1
            // 
            this.guna2ControlBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.guna2ControlBox1.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBox1.HoverState.FillColor = System.Drawing.Color.Red;
            this.guna2ControlBox1.IconColor = System.Drawing.Color.White;
            this.guna2ControlBox1.Location = new System.Drawing.Point(1138, 5);
            this.guna2ControlBox1.Name = "guna2ControlBox1";
            this.guna2ControlBox1.Size = new System.Drawing.Size(81, 38);
            this.guna2ControlBox1.TabIndex = 15;
            // 
            // CategorySettingsForm2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.ClientSize = new System.Drawing.Size(1222, 815);
            this.Controls.Add(this.btnSelectDeveloper);
            this.Controls.Add(this.btnConfirmSelection);
            this.Controls.Add(this.btnSelectExaminee);
            this.Controls.Add(this.btnSelectStudent);
            this.Controls.Add(this.btnSelectEditor);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.guna2Panel1);
            this.Controls.Add(this.lblBlockedItemsDisplay);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CategorySettingsForm2";
            this.Text = "CategorySettingsForm2";
            this.guna2Panel1.ResumeLayout(false);
            this.guna2Panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblBlockedItemsDisplay;
        private System.Windows.Forms.Button btnConfirmSelection;
        private Guna.UI2.WinForms.Guna2Button btnSelectStudent;
        private Guna.UI2.WinForms.Guna2Button btnSelectDeveloper;
        private Guna.UI2.WinForms.Guna2Button btnSelectEditor;
        private Guna.UI2.WinForms.Guna2Button btnSelectExaminee;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox1;
    }
}