using System.Windows.Forms;

namespace Prototype1.UI
{
    public partial class FocusReasonForm : Form
    {
        public FocusReasonForm()
        {
            InitializeComponent();
            ApplyTheme();
            ActiveControl = txtMessageInput;
        }

        public string EnteredMessage
        {
            get { return txtMessageInput.Text; }
        }

        private void ApplyTheme()
        {
            BackColor = AlertDialog.AppBackColor;
            ForeColor = AlertDialog.TextColor;

            label1.ForeColor = AlertDialog.TextColor;
            txtMessageInput.BackColor = AlertDialog.FieldColor;
            txtMessageInput.BorderStyle = BorderStyle.FixedSingle;
            txtMessageInput.ForeColor = AlertDialog.TextColor;

            AlertDialog.StyleButton(btnOk, true);
            AlertDialog.StyleButton(btnCancel, false, true);
        }
    }
}
