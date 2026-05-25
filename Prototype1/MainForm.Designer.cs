namespace Prototype1
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.btnCategorySettings = new System.Windows.Forms.Button();
            this.btnManageBlockedApps = new System.Windows.Forms.Button();
            this.btnStudyPlan = new System.Windows.Forms.Button();
            this.btnActivateBlocking = new System.Windows.Forms.Button();
            this.btnStopBlocking = new System.Windows.Forms.Button();
            this.blockingtimer = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbHour = new System.Windows.Forms.ComboBox();
            this.cmbMin = new System.Windows.Forms.ComboBox();
            this.lblShowTimeLeft = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCategorySettings
            // 
            this.btnCategorySettings.Location = new System.Drawing.Point(538, 52);
            this.btnCategorySettings.Margin = new System.Windows.Forms.Padding(2);
            this.btnCategorySettings.Name = "btnCategorySettings";
            this.btnCategorySettings.Size = new System.Drawing.Size(148, 53);
            this.btnCategorySettings.TabIndex = 1;
            this.btnCategorySettings.Text = "카테고리 설정";
            this.btnCategorySettings.UseVisualStyleBackColor = true;
            this.btnCategorySettings.Click += new System.EventHandler(this.btnCategorySettings_Click);
            // 
            // btnManageBlockedApps
            // 
            this.btnManageBlockedApps.Location = new System.Drawing.Point(538, 112);
            this.btnManageBlockedApps.Margin = new System.Windows.Forms.Padding(2);
            this.btnManageBlockedApps.Name = "btnManageBlockedApps";
            this.btnManageBlockedApps.Size = new System.Drawing.Size(148, 53);
            this.btnManageBlockedApps.TabIndex = 12;
            this.btnManageBlockedApps.Text = "차단 앱 관리";
            this.btnManageBlockedApps.UseVisualStyleBackColor = true;
            this.btnManageBlockedApps.Click += new System.EventHandler(this.btnManageBlockedApps_Click);
            // 
            // btnStudyPlan
            // 
            this.btnStudyPlan.Location = new System.Drawing.Point(538, 172);
            this.btnStudyPlan.Margin = new System.Windows.Forms.Padding(2);
            this.btnStudyPlan.Name = "btnStudyPlan";
            this.btnStudyPlan.Size = new System.Drawing.Size(148, 53);
            this.btnStudyPlan.TabIndex = 13;
            this.btnStudyPlan.Text = "학습 계획 및 관리";
            this.btnStudyPlan.UseVisualStyleBackColor = true;
            this.btnStudyPlan.Click += new System.EventHandler(this.btnStudyPlan_Click);
            // 
            // btnActivateBlocking
            // 
            this.btnActivateBlocking.Location = new System.Drawing.Point(23, 287);
            this.btnActivateBlocking.Margin = new System.Windows.Forms.Padding(2);
            this.btnActivateBlocking.Name = "btnActivateBlocking";
            this.btnActivateBlocking.Size = new System.Drawing.Size(328, 99);
            this.btnActivateBlocking.TabIndex = 3;
            this.btnActivateBlocking.Text = "집중모드 활성화";
            this.btnActivateBlocking.UseVisualStyleBackColor = true;
            this.btnActivateBlocking.Click += new System.EventHandler(this.btnActivateBlocking_Click);
            // 
            // btnStopBlocking
            // 
            this.btnStopBlocking.Location = new System.Drawing.Point(538, 232);
            this.btnStopBlocking.Margin = new System.Windows.Forms.Padding(2);
            this.btnStopBlocking.Name = "btnStopBlocking";
            this.btnStopBlocking.Size = new System.Drawing.Size(148, 53);
            this.btnStopBlocking.TabIndex = 4;
            this.btnStopBlocking.Text = "집중모드 정지(개발용)";
            this.btnStopBlocking.UseVisualStyleBackColor = true;
            this.btnStopBlocking.Click += new System.EventHandler(this.btnStopBlocking_Click);
            // 
            // blockingtimer
            // 
            this.blockingtimer.Interval = 1000;
            this.blockingtimer.Tick += new System.EventHandler(this.blockingtimer_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 193);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 18);
            this.label1.TabIndex = 5;
            this.label1.Text = "세션 시간: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(191, 193);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 18);
            this.label2.TabIndex = 6;
            this.label2.Text = "시간";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(325, 193);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 18);
            this.label3.TabIndex = 7;
            this.label3.Text = "분";
            // 
            // cmbHour
            // 
            this.cmbHour.FormattingEnabled = true;
            this.cmbHour.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4"});
            this.cmbHour.Location = new System.Drawing.Point(114, 190);
            this.cmbHour.Margin = new System.Windows.Forms.Padding(2);
            this.cmbHour.Name = "cmbHour";
            this.cmbHour.Size = new System.Drawing.Size(62, 26);
            this.cmbHour.TabIndex = 8;
            // 
            // cmbMin
            // 
            this.cmbMin.FormattingEnabled = true;
            this.cmbMin.Items.AddRange(new object[] {
            "00",
            "10",
            "20",
            "30",
            "40",
            "50"});
            this.cmbMin.Location = new System.Drawing.Point(240, 190);
            this.cmbMin.Margin = new System.Windows.Forms.Padding(2);
            this.cmbMin.Name = "cmbMin";
            this.cmbMin.Size = new System.Drawing.Size(62, 26);
            this.cmbMin.TabIndex = 9;
            // 
            // lblShowTimeLeft
            // 
            this.lblShowTimeLeft.AutoSize = true;
            this.lblShowTimeLeft.Font = new System.Drawing.Font("굴림", 22F);
            this.lblShowTimeLeft.Location = new System.Drawing.Point(76, 64);
            this.lblShowTimeLeft.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblShowTimeLeft.Name = "lblShowTimeLeft";
            this.lblShowTimeLeft.Size = new System.Drawing.Size(375, 44);
            this.lblShowTimeLeft.TabIndex = 10;
            this.lblShowTimeLeft.Text = "00시간 00분 00초";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 15);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(536, 18);
            this.label4.TabIndex = 11;
            this.label4.Text = "집중모드 정지버튼은 개발 편의성을 위해서 일단 남겨두겠습니다.";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(715, 404);
            this.Controls.Add(this.btnStudyPlan);
            this.Controls.Add(this.btnManageBlockedApps);
            this.Controls.Add(this.btnCategorySettings);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblShowTimeLeft);
            this.Controls.Add(this.cmbMin);
            this.Controls.Add(this.cmbHour);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnStopBlocking);
            this.Controls.Add(this.btnActivateBlocking);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Text = "집중 모드";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnCategorySettings;
        private System.Windows.Forms.Button btnManageBlockedApps;
        private System.Windows.Forms.Button btnStudyPlan;
        private System.Windows.Forms.Button btnActivateBlocking;
        private System.Windows.Forms.Button btnStopBlocking;
        private System.Windows.Forms.Timer blockingtimer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbHour;
        private System.Windows.Forms.ComboBox cmbMin;
        private System.Windows.Forms.Label lblShowTimeLeft;
        private System.Windows.Forms.Label label4;
    }
}

