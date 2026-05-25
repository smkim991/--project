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
            this.btnStopBlocking = new System.Windows.Forms.Button();
            this.blockingtimer = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblShowTimeLeft = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.btnActivateBlocking = new Guna.UI2.WinForms.Guna2Button();
            this.cmbMin = new Guna.UI2.WinForms.Guna2ComboBox();
            this.cmbHour = new Guna.UI2.WinForms.Guna2ComboBox();
            this.guna2Panel2 = new Guna.UI2.WinForms.Guna2Panel();
            this.btnExit = new Guna.UI2.WinForms.Guna2Button();
            this.btnStudyPlan = new Guna.UI2.WinForms.Guna2Button();
            this.btnManageBlockedApps = new Guna.UI2.WinForms.Guna2Button();
            this.btnCategorySettings = new Guna.UI2.WinForms.Guna2Button();
            this.label5 = new System.Windows.Forms.Label();
            this.guna2Panel3 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2ControlBox1 = new Guna.UI2.WinForms.Guna2ControlBox();
            this.label6 = new System.Windows.Forms.Label();
            this.guna2DragControl1 = new Guna.UI2.WinForms.Guna2DragControl(this.components);
            this.guna2Panel1.SuspendLayout();
            this.guna2Panel2.SuspendLayout();
            this.guna2Panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStopBlocking
            // 
            this.btnStopBlocking.Location = new System.Drawing.Point(1153, 741);
            this.btnStopBlocking.Name = "btnStopBlocking";
            this.btnStopBlocking.Size = new System.Drawing.Size(192, 71);
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
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(74, 259);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 24);
            this.label1.TabIndex = 5;
            this.label1.Text = "세션 시간: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(376, 261);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 24);
            this.label2.TabIndex = 6;
            this.label2.Text = "시간";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(623, 257);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 24);
            this.label3.TabIndex = 7;
            this.label3.Text = "분";
            // 
            // lblShowTimeLeft
            // 
            this.lblShowTimeLeft.Font = new System.Drawing.Font("맑은 고딕", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblShowTimeLeft.ForeColor = System.Drawing.Color.White;
            this.lblShowTimeLeft.Location = new System.Drawing.Point(11, 39);
            this.lblShowTimeLeft.Name = "lblShowTimeLeft";
            this.lblShowTimeLeft.Size = new System.Drawing.Size(833, 132);
            this.lblShowTimeLeft.TabIndex = 10;
            this.lblShowTimeLeft.Text = "00시간 00분 00초";
            this.lblShowTimeLeft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblShowTimeLeft.Click += new System.EventHandler(this.lblShowTimeLeft_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(646, 833);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(714, 24);
            this.label4.TabIndex = 11;
            this.label4.Text = "집중모드 정지버튼은 개발 편의성을 위해서 일단 남겨두겠습니다.";
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.guna2Panel1.BorderRadius = 25;
            this.guna2Panel1.Controls.Add(this.label7);
            this.guna2Panel1.Controls.Add(this.btnActivateBlocking);
            this.guna2Panel1.Controls.Add(this.cmbMin);
            this.guna2Panel1.Controls.Add(this.cmbHour);
            this.guna2Panel1.Controls.Add(this.label3);
            this.guna2Panel1.Controls.Add(this.label2);
            this.guna2Panel1.Controls.Add(this.label1);
            this.guna2Panel1.Controls.Add(this.lblShowTimeLeft);
            this.guna2Panel1.Location = new System.Drawing.Point(415, 215);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Size = new System.Drawing.Size(856, 430);
            this.guna2Panel1.TabIndex = 15;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("맑은 고딕", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(315, 184);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(212, 37);
            this.label7.TabIndex = 14;
            this.label7.Text = "현재 모드 : 없음";
            // 
            // btnActivateBlocking
            // 
            this.btnActivateBlocking.BorderRadius = 18;
            this.btnActivateBlocking.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnActivateBlocking.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnActivateBlocking.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnActivateBlocking.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnActivateBlocking.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.btnActivateBlocking.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnActivateBlocking.ForeColor = System.Drawing.Color.White;
            this.btnActivateBlocking.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(58)))), ((int)(((byte)(237)))));
            this.btnActivateBlocking.Location = new System.Drawing.Point(272, 335);
            this.btnActivateBlocking.Name = "btnActivateBlocking";
            this.btnActivateBlocking.Size = new System.Drawing.Size(320, 63);
            this.btnActivateBlocking.TabIndex = 13;
            this.btnActivateBlocking.Text = "집중 세션 활성화";
            this.btnActivateBlocking.Click += new System.EventHandler(this.btnActivateBlocking_Click);
            // 
            // cmbMin
            // 
            this.cmbMin.BackColor = System.Drawing.Color.Transparent;
            this.cmbMin.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.cmbMin.BorderRadius = 10;
            this.cmbMin.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbMin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMin.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.cmbMin.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbMin.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbMin.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbMin.ForeColor = System.Drawing.Color.White;
            this.cmbMin.ItemHeight = 30;
            this.cmbMin.Location = new System.Drawing.Point(464, 250);
            this.cmbMin.Name = "cmbMin";
            this.cmbMin.Size = new System.Drawing.Size(140, 36);
            this.cmbMin.TabIndex = 12;
            // 
            // cmbHour
            // 
            this.cmbHour.BackColor = System.Drawing.Color.Transparent;
            this.cmbHour.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.cmbHour.BorderRadius = 10;
            this.cmbHour.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbHour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHour.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.cmbHour.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbHour.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbHour.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbHour.ForeColor = System.Drawing.Color.White;
            this.cmbHour.ItemHeight = 30;
            this.cmbHour.Location = new System.Drawing.Point(222, 249);
            this.cmbHour.Name = "cmbHour";
            this.cmbHour.Size = new System.Drawing.Size(140, 36);
            this.cmbHour.TabIndex = 11;
            // 
            // guna2Panel2
            // 
            this.guna2Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.guna2Panel2.Controls.Add(this.btnExit);
            this.guna2Panel2.Controls.Add(this.btnStudyPlan);
            this.guna2Panel2.Controls.Add(this.btnManageBlockedApps);
            this.guna2Panel2.Controls.Add(this.btnCategorySettings);
            this.guna2Panel2.Controls.Add(this.label5);
            this.guna2Panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.guna2Panel2.Location = new System.Drawing.Point(0, 0);
            this.guna2Panel2.Name = "guna2Panel2";
            this.guna2Panel2.Size = new System.Drawing.Size(279, 900);
            this.guna2Panel2.TabIndex = 16;
            this.guna2Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.guna2Panel2_Paint);
            // 
            // btnExit
            // 
            this.btnExit.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnExit.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.btnExit.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnExit.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnExit.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnExit.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnExit.FillColor = System.Drawing.Color.Transparent;
            this.btnExit.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(45)))));
            this.btnExit.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnExit.Location = new System.Drawing.Point(33, 812);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(180, 45);
            this.btnExit.TabIndex = 18;
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnStudyPlan
            // 
            this.btnStudyPlan.BorderRadius = 10;
            this.btnStudyPlan.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnStudyPlan.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.btnStudyPlan.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnStudyPlan.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnStudyPlan.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnStudyPlan.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnStudyPlan.FillColor = System.Drawing.Color.Transparent;
            this.btnStudyPlan.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnStudyPlan.ForeColor = System.Drawing.Color.White;
            this.btnStudyPlan.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(45)))));
            this.btnStudyPlan.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnStudyPlan.Location = new System.Drawing.Point(24, 237);
            this.btnStudyPlan.Name = "btnStudyPlan";
            this.btnStudyPlan.Size = new System.Drawing.Size(234, 45);
            this.btnStudyPlan.TabIndex = 17;
            this.btnStudyPlan.Text = "학습 계획 및 관리";
            this.btnStudyPlan.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnStudyPlan.Click += new System.EventHandler(this.btnStudyPlan_Click);
            // 
            // btnManageBlockedApps
            // 
            this.btnManageBlockedApps.BorderRadius = 10;
            this.btnManageBlockedApps.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnManageBlockedApps.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.btnManageBlockedApps.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnManageBlockedApps.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnManageBlockedApps.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnManageBlockedApps.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnManageBlockedApps.FillColor = System.Drawing.Color.Transparent;
            this.btnManageBlockedApps.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnManageBlockedApps.ForeColor = System.Drawing.Color.White;
            this.btnManageBlockedApps.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(45)))));
            this.btnManageBlockedApps.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnManageBlockedApps.Location = new System.Drawing.Point(51, 182);
            this.btnManageBlockedApps.Name = "btnManageBlockedApps";
            this.btnManageBlockedApps.Size = new System.Drawing.Size(180, 45);
            this.btnManageBlockedApps.TabIndex = 16;
            this.btnManageBlockedApps.Text = "차단 앱 관리";
            this.btnManageBlockedApps.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnManageBlockedApps.Click += new System.EventHandler(this.btnManageBlockedApps_Click);
            // 
            // btnCategorySettings
            // 
            this.btnCategorySettings.BorderRadius = 10;
            this.btnCategorySettings.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnCategorySettings.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(92)))), ((int)(((byte)(246)))));
            this.btnCategorySettings.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnCategorySettings.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnCategorySettings.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnCategorySettings.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnCategorySettings.FillColor = System.Drawing.Color.Transparent;
            this.btnCategorySettings.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCategorySettings.ForeColor = System.Drawing.Color.White;
            this.btnCategorySettings.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(45)))));
            this.btnCategorySettings.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnCategorySettings.Location = new System.Drawing.Point(42, 125);
            this.btnCategorySettings.Name = "btnCategorySettings";
            this.btnCategorySettings.Size = new System.Drawing.Size(192, 45);
            this.btnCategorySettings.TabIndex = 15;
            this.btnCategorySettings.Text = "카테고리 설정";
            this.btnCategorySettings.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnCategorySettings.Click += new System.EventHandler(this.btnCategorySettings_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 10.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(34, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(206, 40);
            this.label5.TabIndex = 14;
            this.label5.Text = "Focus Blocker";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // guna2Panel3
            // 
            this.guna2Panel3.Controls.Add(this.guna2ControlBox1);
            this.guna2Panel3.Controls.Add(this.label6);
            this.guna2Panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.guna2Panel3.Location = new System.Drawing.Point(279, 0);
            this.guna2Panel3.Name = "guna2Panel3";
            this.guna2Panel3.Size = new System.Drawing.Size(1121, 43);
            this.guna2Panel3.TabIndex = 17;
            // 
            // guna2ControlBox1
            // 
            this.guna2ControlBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.guna2ControlBox1.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBox1.HoverState.FillColor = System.Drawing.Color.Red;
            this.guna2ControlBox1.IconColor = System.Drawing.Color.White;
            this.guna2ControlBox1.Location = new System.Drawing.Point(1035, 2);
            this.guna2ControlBox1.Name = "guna2ControlBox1";
            this.guna2ControlBox1.Size = new System.Drawing.Size(81, 38);
            this.guna2ControlBox1.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(14, 4);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(172, 32);
            this.label6.TabIndex = 0;
            this.label6.Text = "Focus Blocker";
            // 
            // guna2DragControl1
            // 
            this.guna2DragControl1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2DragControl1.TargetControl = this.guna2Panel3;
            this.guna2DragControl1.UseTransparentDrag = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1400, 900);
            this.Controls.Add(this.guna2Panel3);
            this.Controls.Add(this.guna2Panel2);
            this.Controls.Add(this.guna2Panel1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnStopBlocking);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "집중 모드";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.guna2Panel1.ResumeLayout(false);
            this.guna2Panel1.PerformLayout();
            this.guna2Panel2.ResumeLayout(false);
            this.guna2Panel2.PerformLayout();
            this.guna2Panel3.ResumeLayout(false);
            this.guna2Panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnStopBlocking;
        private System.Windows.Forms.Timer blockingtimer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblShowTimeLeft;
        private System.Windows.Forms.Label label4;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2ComboBox cmbMin;
        private Guna.UI2.WinForms.Guna2ComboBox cmbHour;
        private Guna.UI2.WinForms.Guna2Button btnActivateBlocking;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel2;
        private System.Windows.Forms.Label label5;
        private Guna.UI2.WinForms.Guna2Button btnCategorySettings;
        private Guna.UI2.WinForms.Guna2Button btnManageBlockedApps;
        private Guna.UI2.WinForms.Guna2Button btnStudyPlan;
        private Guna.UI2.WinForms.Guna2Button btnExit;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel3;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox1;
        private System.Windows.Forms.Label label6;
        private Guna.UI2.WinForms.Guna2DragControl guna2DragControl1;
        private System.Windows.Forms.Label label7;
    }
}

