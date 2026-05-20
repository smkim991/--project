using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Prototype1.UI
{
    public partial class StudyPlanForm : Form
    {
        private const string MemoFileName = "StudyPlanMemo.txt";
        private string _memoFilePath;
        public StudyPlanForm()
        {
            InitializeComponent();
            _memoFilePath = Path.Combine(Application.UserAppDataPath, MemoFileName);
            Directory.CreateDirectory(Application.UserAppDataPath);
        }

        private void StudyPlanForm_Load(object sender, EventArgs e)
        {
            if (File.Exists(_memoFilePath)) // 파일이 존재하는지 확인
            {
                try
                {
                    txtMemo.Text = File.ReadAllText(_memoFilePath); // 파일의 모든 텍스트를 읽어 txtMemo에 설정
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"메모를 불러오는 데 실패했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void StudyPlanForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                File.WriteAllText(_memoFilePath, txtMemo.Text); // txtMemo의 모든 텍스트를 파일에 저장
            }
            catch (Exception ex)
            {
                MessageBox.Show($"메모를 저장하는 데 실패했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
