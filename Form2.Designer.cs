namespace WindowsFormsApp15
{
    partial class Form2
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lbStudentBlockedItems = new System.Windows.Forms.ListBox();
            this.lbEditorBlockedItems = new System.Windows.Forms.ListBox();
            this.lbDeveloperBlockedItems = new System.Windows.Forms.ListBox();
            this.lbExamineeBlockedItems = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbStudentBlockedItems);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(311, 186);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "대학생 모드";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbEditorBlockedItems);
            this.groupBox2.Location = new System.Drawing.Point(12, 252);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(311, 186);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "영상편집자 모드";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lbDeveloperBlockedItems);
            this.groupBox3.Location = new System.Drawing.Point(477, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(311, 186);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "개발자 모드";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lbExamineeBlockedItems);
            this.groupBox4.Location = new System.Drawing.Point(477, 252);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(311, 186);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "수험생 모드";
            // 
            // lbStudentBlockedItems
            // 
            this.lbStudentBlockedItems.FormattingEnabled = true;
            this.lbStudentBlockedItems.ItemHeight = 18;
            this.lbStudentBlockedItems.Location = new System.Drawing.Point(7, 28);
            this.lbStudentBlockedItems.Name = "lbStudentBlockedItems";
            this.lbStudentBlockedItems.Size = new System.Drawing.Size(298, 148);
            this.lbStudentBlockedItems.TabIndex = 0;
            // 
            // lbEditorBlockedItems
            // 
            this.lbEditorBlockedItems.FormattingEnabled = true;
            this.lbEditorBlockedItems.ItemHeight = 18;
            this.lbEditorBlockedItems.Location = new System.Drawing.Point(7, 28);
            this.lbEditorBlockedItems.Name = "lbEditorBlockedItems";
            this.lbEditorBlockedItems.Size = new System.Drawing.Size(298, 148);
            this.lbEditorBlockedItems.TabIndex = 0;
            // 
            // lbDeveloperBlockedItems
            // 
            this.lbDeveloperBlockedItems.FormattingEnabled = true;
            this.lbDeveloperBlockedItems.ItemHeight = 18;
            this.lbDeveloperBlockedItems.Location = new System.Drawing.Point(7, 28);
            this.lbDeveloperBlockedItems.Name = "lbDeveloperBlockedItems";
            this.lbDeveloperBlockedItems.Size = new System.Drawing.Size(298, 148);
            this.lbDeveloperBlockedItems.TabIndex = 0;
            // 
            // lbExamineeBlockedItems
            // 
            this.lbExamineeBlockedItems.FormattingEnabled = true;
            this.lbExamineeBlockedItems.ItemHeight = 18;
            this.lbExamineeBlockedItems.Location = new System.Drawing.Point(7, 28);
            this.lbExamineeBlockedItems.Name = "lbExamineeBlockedItems";
            this.lbExamineeBlockedItems.Size = new System.Drawing.Size(298, 148);
            this.lbExamineeBlockedItems.TabIndex = 0;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lbStudentBlockedItems;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox lbEditorBlockedItems;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListBox lbDeveloperBlockedItems;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListBox lbExamineeBlockedItems;
    }
}