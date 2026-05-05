namespace Prototype1
{
    partial class BlockListForm
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
            this.CurrentBlockList = new System.Windows.Forms.GroupBox();
            this.blockListView = new System.Windows.Forms.ListView();
            this.processName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.BlockingState = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.BlockedTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnCategory1 = new System.Windows.Forms.Button();
            this.btnCategory2 = new System.Windows.Forms.Button();
            this.btnBlockListSave = new System.Windows.Forms.Button();
            this.btnUserSpecified = new System.Windows.Forms.Button();
            this.btnClosingBlockListForm = new System.Windows.Forms.Button();
            this.CurrentBlockList.SuspendLayout();
            this.SuspendLayout();
            // 
            // CurrentBlockList
            // 
            this.CurrentBlockList.Controls.Add(this.blockListView);
            this.CurrentBlockList.Location = new System.Drawing.Point(111, 61);
            this.CurrentBlockList.Name = "CurrentBlockList";
            this.CurrentBlockList.Size = new System.Drawing.Size(579, 656);
            this.CurrentBlockList.TabIndex = 0;
            this.CurrentBlockList.TabStop = false;
            this.CurrentBlockList.Text = "Current Block List";
            // 
            // blockListView
            // 
            this.blockListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.processName,
            this.BlockingState,
            this.BlockedTime});
            this.blockListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blockListView.FullRowSelect = true;
            this.blockListView.HideSelection = false;
            this.blockListView.Location = new System.Drawing.Point(3, 31);
            this.blockListView.Name = "blockListView";
            this.blockListView.Size = new System.Drawing.Size(573, 622);
            this.blockListView.TabIndex = 0;
            this.blockListView.UseCompatibleStateImageBehavior = false;
            this.blockListView.View = System.Windows.Forms.View.Details;
            // 
            // processName
            // 
            this.processName.Text = "프로세스";
            this.processName.Width = 159;
            // 
            // BlockingState
            // 
            this.BlockingState.Text = "차단상태";
            this.BlockingState.Width = 175;
            // 
            // BlockedTime
            // 
            this.BlockedTime.Text = "시간";
            this.BlockedTime.Width = 180;
            // 
            // btnCategory1
            // 
            this.btnCategory1.Location = new System.Drawing.Point(82, 769);
            this.btnCategory1.Name = "btnCategory1";
            this.btnCategory1.Size = new System.Drawing.Size(293, 51);
            this.btnCategory1.TabIndex = 1;
            this.btnCategory1.Text = "Category1 (그림판)";
            this.btnCategory1.UseVisualStyleBackColor = true;
            this.btnCategory1.Click += new System.EventHandler(this.btnCategory1_Click);
            // 
            // btnCategory2
            // 
            this.btnCategory2.Location = new System.Drawing.Point(397, 769);
            this.btnCategory2.Name = "btnCategory2";
            this.btnCategory2.Size = new System.Drawing.Size(293, 51);
            this.btnCategory2.TabIndex = 2;
            this.btnCategory2.Text = "Category2 (카카오톡)";
            this.btnCategory2.UseVisualStyleBackColor = true;
            this.btnCategory2.Click += new System.EventHandler(this.btnCategory2_Click);
            // 
            // btnBlockListSave
            // 
            this.btnBlockListSave.Location = new System.Drawing.Point(503, 833);
            this.btnBlockListSave.Name = "btnBlockListSave";
            this.btnBlockListSave.Size = new System.Drawing.Size(187, 70);
            this.btnBlockListSave.TabIndex = 3;
            this.btnBlockListSave.Text = "저장";
            this.btnBlockListSave.UseVisualStyleBackColor = true;
            this.btnBlockListSave.Click += new System.EventHandler(this.btnBlockListSave_Click);
            // 
            // btnUserSpecified
            // 
            this.btnUserSpecified.Location = new System.Drawing.Point(82, 833);
            this.btnUserSpecified.Name = "btnUserSpecified";
            this.btnUserSpecified.Size = new System.Drawing.Size(215, 70);
            this.btnUserSpecified.TabIndex = 4;
            this.btnUserSpecified.Text = "사용자 설정 추가";
            this.btnUserSpecified.UseVisualStyleBackColor = true;
            this.btnUserSpecified.Click += new System.EventHandler(this.btnUserSpecified_Click);
            // 
            // btnClosingBlockListForm
            // 
            this.btnClosingBlockListForm.Location = new System.Drawing.Point(313, 833);
            this.btnClosingBlockListForm.Name = "btnClosingBlockListForm";
            this.btnClosingBlockListForm.Size = new System.Drawing.Size(173, 70);
            this.btnClosingBlockListForm.TabIndex = 5;
            this.btnClosingBlockListForm.Text = "확인";
            this.btnClosingBlockListForm.UseVisualStyleBackColor = true;
            this.btnClosingBlockListForm.Click += new System.EventHandler(this.btnClosingBlockListForm_Click);
            // 
            // BlockListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(785, 933);
            this.Controls.Add(this.btnClosingBlockListForm);
            this.Controls.Add(this.btnUserSpecified);
            this.Controls.Add(this.btnBlockListSave);
            this.Controls.Add(this.btnCategory2);
            this.Controls.Add(this.btnCategory1);
            this.Controls.Add(this.CurrentBlockList);
            this.Name = "BlockListForm";
            this.Text = "BlockListForm";
            this.Load += new System.EventHandler(this.BlockListForm_Load);
            this.Shown += new System.EventHandler(this.BlockListForm_Shown);
            this.CurrentBlockList.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox CurrentBlockList;
        private System.Windows.Forms.ListView blockListView;
        private System.Windows.Forms.ColumnHeader processName;
        private System.Windows.Forms.ColumnHeader BlockingState;
        private System.Windows.Forms.ColumnHeader BlockedTime;
        private System.Windows.Forms.Button btnCategory1;
        private System.Windows.Forms.Button btnCategory2;
        private System.Windows.Forms.Button btnBlockListSave;
        private System.Windows.Forms.Button btnUserSpecified;
        private System.Windows.Forms.Button btnClosingBlockListForm;
    }
}