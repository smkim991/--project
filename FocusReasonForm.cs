using System.Windows.Forms;

namespace Prototype1.UI
{
    public partial class FocusReasonForm : Form
    {
        public FocusReasonForm()
        {
            InitializeComponent();
            ActiveControl = txtMessageInput;
        }

        public string EnteredMessage
        {
            get { return txtMessageInput.Text; }
        }
    }
}
