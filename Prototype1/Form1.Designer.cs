namespace Prototype1
{
    partial class Prototype1
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
            this.btnBlockList = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
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
            // btnBlockList
            // 
            this.btnBlockList.Location = new System.Drawing.Point(505, 290);
            this.btnBlockList.Name = "btnBlockList";
            this.btnBlockList.Size = new System.Drawing.Size(192, 132);
            this.btnBlockList.TabIndex = 1;
            this.btnBlockList.Text = "차단목록관리";
            this.btnBlockList.UseVisualStyleBackColor = true;
            this.btnBlockList.Click += new System.EventHandler(this.btnBlockList_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(505, 444);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(192, 71);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnActivateBlocking
            // 
            this.btnActivateBlocking.Location = new System.Drawing.Point(30, 383);
            this.btnActivateBlocking.Name = "btnActivateBlocking";
            this.btnActivateBlocking.Size = new System.Drawing.Size(426, 132);
            this.btnActivateBlocking.TabIndex = 3;
            this.btnActivateBlocking.Text = "집중모드 활성화";
            this.btnActivateBlocking.UseVisualStyleBackColor = true;
            this.btnActivateBlocking.Click += new System.EventHandler(this.btnActivateBlocking_Click);
            // 
            // btnStopBlocking
            // 
            this.btnStopBlocking.Location = new System.Drawing.Point(533, 165);
            this.btnStopBlocking.Name = "btnStopBlocking";
            this.btnStopBlocking.Size = new System.Drawing.Size(164, 103);
            this.btnStopBlocking.TabIndex = 4;
            this.btnStopBlocking.Text = "집중모드 정지";
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
            this.label1.Location = new System.Drawing.Point(12, 257);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 24);
            this.label1.TabIndex = 5;
            this.label1.Text = "세션 시간: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(248, 257);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 24);
            this.label2.TabIndex = 6;
            this.label2.Text = "시간";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(422, 257);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 24);
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
            this.cmbHour.Location = new System.Drawing.Point(148, 254);
            this.cmbHour.Name = "cmbHour";
            this.cmbHour.Size = new System.Drawing.Size(79, 32);
            this.cmbHour.TabIndex = 8;
            // 
            // cmbMin
            // 
            this.cmbMin.FormattingEnabled = true;
            this.cmbMin.Items.AddRange(new object[] {
            "10",
            "20",
            "30",
            "40",
            "50"});
            this.cmbMin.Location = new System.Drawing.Point(312, 254);
            this.cmbMin.Name = "cmbMin";
            this.cmbMin.Size = new System.Drawing.Size(79, 32);
            this.cmbMin.TabIndex = 9;
            // 
            // lblShowTimeLeft
            // 
            this.lblShowTimeLeft.AutoSize = true;
            this.lblShowTimeLeft.Font = new System.Drawing.Font("굴림", 22F);
            this.lblShowTimeLeft.Location = new System.Drawing.Point(99, 85);
            this.lblShowTimeLeft.Name = "lblShowTimeLeft";
            this.lblShowTimeLeft.Size = new System.Drawing.Size(505, 59);
            this.lblShowTimeLeft.TabIndex = 10;
            this.lblShowTimeLeft.Text = "00시간 00분 00초";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(714, 24);
            this.label4.TabIndex = 11;
            this.label4.Text = "집중모드 정지버튼은 개발 편의성을 위해서 일단 남겨두겠습니다.";
            // 
            // Prototype1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(722, 539);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblShowTimeLeft);
            this.Controls.Add(this.cmbMin);
            this.Controls.Add(this.cmbHour);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnStopBlocking);
            this.Controls.Add(this.btnActivateBlocking);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnBlockList);
            this.Name = "Prototype1";
            this.Text = "Prototype1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Prototype1_FormClosing);
            this.Load += new System.EventHandler(this.Prototype1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnBlockList;
        private System.Windows.Forms.Button btnExit;
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

