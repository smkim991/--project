using System;
using System.Drawing;
using System.Windows.Forms;

namespace Prototype1.UI
{
    internal static class AlertDialog
    {
        internal static readonly Color AppBackColor = Color.FromArgb(18, 18, 18);
        internal static readonly Color SurfaceColor = Color.FromArgb(24, 24, 24);
        internal static readonly Color FieldColor = Color.FromArgb(35, 35, 35);
        internal static readonly Color BorderColor = Color.FromArgb(60, 60, 60);
        internal static readonly Color TextColor = Color.White;
        internal static readonly Color SubtleTextColor = Color.FromArgb(210, 210, 215);
        internal static readonly Color AccentColor = Color.FromArgb(139, 92, 246);
        internal static readonly Color AccentHoverColor = Color.FromArgb(124, 58, 237);
        internal static readonly Color WarningColor = Color.FromArgb(245, 158, 11);
        internal static readonly Color ErrorColor = Color.FromArgb(239, 68, 68);

        public static DialogResult Show(IWin32Window owner, string message)
        {
            return Show(owner, message, "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static DialogResult Show(IWin32Window owner, string message, string title)
        {
            return Show(owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static DialogResult Show(IWin32Window owner, string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            using (Form dialog = new Form())
            using (TableLayoutPanel root = new TableLayoutPanel())
            using (Panel iconPanel = new Panel())
            using (Label iconLabel = new Label())
            using (Label titleLabel = new Label())
            using (Label messageLabel = new Label())
            using (FlowLayoutPanel buttonPanel = new FlowLayoutPanel())
            {
                ApplyDialogTheme(dialog);
                dialog.Text = title;
                dialog.ClientSize = CalculateDialogSize(message);
                dialog.Padding = new Padding(22, 20, 22, 18);

                root.Dock = DockStyle.Fill;
                root.BackColor = AppBackColor;
                root.ColumnCount = 2;
                root.RowCount = 3;
                root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48));
                root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
                root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));

                iconPanel.Size = new Size(36, 36);
                iconPanel.Margin = new Padding(0, 0, 12, 0);
                iconPanel.BackColor = GetIconColor(icon);

                iconLabel.Dock = DockStyle.Fill;
                iconLabel.Text = GetIconText(icon);
                iconLabel.TextAlign = ContentAlignment.MiddleCenter;
                iconLabel.Font = new Font("맑은 고딕", 13F, FontStyle.Bold);
                iconLabel.ForeColor = TextColor;
                iconPanel.Controls.Add(iconLabel);

                titleLabel.Dock = DockStyle.Fill;
                titleLabel.Text = string.IsNullOrWhiteSpace(title) ? "알림" : title;
                titleLabel.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
                titleLabel.ForeColor = TextColor;
                titleLabel.TextAlign = ContentAlignment.MiddleLeft;
                titleLabel.Margin = new Padding(0);

                messageLabel.Dock = DockStyle.Fill;
                messageLabel.Text = message ?? string.Empty;
                messageLabel.Font = new Font("맑은 고딕", 10F, FontStyle.Regular);
                messageLabel.ForeColor = SubtleTextColor;
                messageLabel.TextAlign = ContentAlignment.TopLeft;
                messageLabel.Margin = new Padding(0, 8, 0, 0);

                buttonPanel.Dock = DockStyle.Fill;
                buttonPanel.FlowDirection = FlowDirection.RightToLeft;
                buttonPanel.WrapContents = false;
                buttonPanel.BackColor = AppBackColor;
                buttonPanel.Margin = new Padding(0, 14, 0, 0);

                AddButtons(buttonPanel, dialog, buttons, icon);

                root.Controls.Add(iconPanel, 0, 0);
                root.SetRowSpan(iconPanel, 2);
                root.Controls.Add(titleLabel, 1, 0);
                root.Controls.Add(messageLabel, 1, 1);
                root.Controls.Add(buttonPanel, 0, 2);
                root.SetColumnSpan(buttonPanel, 2);
                dialog.Controls.Add(root);

                return owner == null ? dialog.ShowDialog() : dialog.ShowDialog(owner);
            }
        }

        public static void ApplyDialogTheme(Form dialog)
        {
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            dialog.MaximizeBox = false;
            dialog.MinimizeBox = false;
            dialog.ShowInTaskbar = false;
            dialog.BackColor = AppBackColor;
            dialog.ForeColor = TextColor;
            dialog.Font = new Font("맑은 고딕", 10F, FontStyle.Regular);
        }

        public static void StyleButton(Button button, bool primary)
        {
            StyleButton(button, primary, false);
        }

        public static void StyleButton(Button button, bool primary, bool danger)
        {
            Color fill = danger ? ErrorColor : primary ? AccentColor : FieldColor;
            Color hover = danger ? Color.FromArgb(220, 38, 38) : primary ? AccentHoverColor : Color.FromArgb(48, 48, 54);

            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = primary || danger ? fill : BorderColor;
            button.FlatAppearance.MouseOverBackColor = hover;
            button.FlatAppearance.MouseDownBackColor = primary || danger ? hover : AccentColor;
            button.BackColor = fill;
            button.ForeColor = TextColor;
            button.UseVisualStyleBackColor = false;
            button.Font = new Font("맑은 고딕", 9F, FontStyle.Bold);
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.Cursor = Cursors.Hand;
        }

        private static Size CalculateDialogSize(string message)
        {
            Size measured = TextRenderer.MeasureText(
                message ?? string.Empty,
                new Font("맑은 고딕", 10F, FontStyle.Regular),
                new Size(390, 0),
                TextFormatFlags.WordBreak);

            int width = Math.Max(420, Math.Min(540, measured.Width + 120));
            int height = Math.Max(172, Math.Min(320, measured.Height + 128));
            return new Size(width, height);
        }

        private static void AddButtons(FlowLayoutPanel buttonPanel, Form dialog, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            if (buttons == MessageBoxButtons.OKCancel)
            {
                Button cancelButton = CreateDialogButton("취소", DialogResult.Cancel, false, true);
                Button okButton = CreateDialogButton("확인", DialogResult.OK, true, false);
                buttonPanel.Controls.Add(cancelButton);
                buttonPanel.Controls.Add(okButton);
                dialog.AcceptButton = okButton;
                dialog.CancelButton = cancelButton;
                return;
            }

            if (buttons == MessageBoxButtons.YesNo)
            {
                Button noButton = CreateDialogButton("아니오", DialogResult.No, false, false);
                Button yesButton = CreateDialogButton("예", DialogResult.Yes, true, false);
                buttonPanel.Controls.Add(yesButton);
                buttonPanel.Controls.Add(noButton);
                dialog.AcceptButton = yesButton;
                dialog.CancelButton = noButton;
                return;
            }

            if (buttons == MessageBoxButtons.YesNoCancel)
            {
                Button cancelButton = CreateDialogButton("취소", DialogResult.Cancel, false, true);
                Button noButton = CreateDialogButton("아니오", DialogResult.No, false, false);
                Button yesButton = CreateDialogButton("예", DialogResult.Yes, true, false);
                buttonPanel.Controls.Add(cancelButton);
                buttonPanel.Controls.Add(noButton);
                buttonPanel.Controls.Add(yesButton);
                dialog.AcceptButton = yesButton;
                dialog.CancelButton = cancelButton;
                return;
            }

            Button okOnlyButton = CreateDialogButton("확인", DialogResult.OK, true, false);
            buttonPanel.Controls.Add(okOnlyButton);
            dialog.AcceptButton = okOnlyButton;
            dialog.CancelButton = okOnlyButton;
        }

        private static Button CreateDialogButton(string text, DialogResult dialogResult, bool primary, bool danger)
        {
            Button button = new Button();
            button.Text = text;
            button.DialogResult = dialogResult;
            button.Size = new Size(96, 34);
            button.Margin = new Padding(8, 0, 0, 0);
            StyleButton(button, primary, danger);
            return button;
        }

        private static string GetIconText(MessageBoxIcon icon)
        {
            if (icon == MessageBoxIcon.Error)
            {
                return "!";
            }

            if (icon == MessageBoxIcon.Warning)
            {
                return "!";
            }

            return "i";
        }

        private static Color GetIconColor(MessageBoxIcon icon)
        {
            if (icon == MessageBoxIcon.Error)
            {
                return ErrorColor;
            }

            if (icon == MessageBoxIcon.Warning)
            {
                return WarningColor;
            }

            return AccentColor;
        }
    }
}
