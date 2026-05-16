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
            this.SuspendLayout();
            // 
            // btnBlockList
            // 
            this.btnBlockList.Location = new System.Drawing.Point(536, 12);
            this.btnBlockList.Name = "btnBlockList";
            this.btnBlockList.Size = new System.Drawing.Size(199, 132);
            this.btnBlockList.TabIndex = 1;
            this.btnBlockList.Text = "차단목록관리";
            this.btnBlockList.UseVisualStyleBackColor = true;
            this.btnBlockList.Click += new System.EventHandler(this.btnBlockList_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(543, 202);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(192, 71);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnActivateBlocking
            // 
            this.btnActivateBlocking.Location = new System.Drawing.Point(12, 12);
            this.btnActivateBlocking.Name = "btnActivateBlocking";
            this.btnActivateBlocking.Size = new System.Drawing.Size(426, 132);
            this.btnActivateBlocking.TabIndex = 3;
            this.btnActivateBlocking.Text = "집중모드 활성화";
            this.btnActivateBlocking.UseVisualStyleBackColor = true;
            this.btnActivateBlocking.Click += new System.EventHandler(this.btnActivateBlocking_Click);
            // 
            // btnStopBlocking
            // 
            this.btnStopBlocking.Location = new System.Drawing.Point(12, 170);
            this.btnStopBlocking.Name = "btnStopBlocking";
            this.btnStopBlocking.Size = new System.Drawing.Size(428, 103);
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
            // Prototype1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 303);
            this.Controls.Add(this.btnStopBlocking);
            this.Controls.Add(this.btnActivateBlocking);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnBlockList);
            this.Name = "Prototype1";
            this.Text = "Prototype1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Prototype1_FormClosing);
            this.Load += new System.EventHandler(this.Prototype1_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnBlockList;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnActivateBlocking;
        private System.Windows.Forms.Button btnStopBlocking;
        private System.Windows.Forms.Timer blockingtimer;
    }
}

