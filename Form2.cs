using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp15
{
    public partial class Form2 : Form
    {

        public Form2()
        {
            InitializeComponent();
            PopulateBlockedLists();
        }

        private void PopulateBlockedLists()
        {
            lbStudentBlockedItems.Items.Add("넷플릭스");
            lbStudentBlockedItems.Items.Add("네이버웹툰");

            lbDeveloperBlockedItems.Items.Add("유튜브");
            lbDeveloperBlockedItems.Items.Add("메모장");
            lbDeveloperBlockedItems.Items.Add("멜론");

            lbEditorBlockedItems.Items.Add("인스타그램");
            lbEditorBlockedItems.Items.Add("엑셀");

            lbExamineeBlockedItems.Items.Add("카카오톡");
            lbExamineeBlockedItems.Items.Add("인스타그램");
            lbExamineeBlockedItems.Items.Add("틱톡");
        }
        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
