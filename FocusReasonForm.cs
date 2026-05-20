using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Prototype1.UI
{
    public partial class FocusReasonForm : Form
    {
        public FocusReasonForm()
        {
            InitializeComponent();
        }

        public string EnteredMessage
        {
            get { return label1.Text; }
        }

        public string InitialMessage
        {
            set { label1.Text = value; }
        }
        private void FocusReasonForm_Load(object sender, EventArgs e)
        {
            label1.Focus();
        }
    }
}
