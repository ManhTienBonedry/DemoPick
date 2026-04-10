using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Sunny.UI;
using DemoPick.Services;

namespace DemoPick
{
    public partial class FrmRegister : Form
    {
        public FrmRegister()
        {
            InitializeComponent();

            if (DesignModeUtil.IsDesignMode(this))
            {
                return;
            }
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Set corner radius
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            // Attach drag event
            pnlLeft.MouseDown += Drag_MouseDown;
            pnlRight.MouseDown += Drag_MouseDown;
            lblTitle.MouseDown += Drag_MouseDown;

            // Ensure clickable UI elements are actually wired.
            // (Some of these are intentionally not wired in Designer.)
            if (btnClose != null) btnClose.Click += btnClose_Click;
            if (btnRegister != null) btnRegister.Click += btnRegister_Click;
            if (lblLoginNow != null) lblLoginNow.Click += lblLoginNow_Click;
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void Drag_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string fullName = txtName?.Text?.Trim() ?? "";
            string email = txtEmail?.Text?.Trim() ?? "";
            string phone = txtPhone?.Text?.Trim() ?? "";
            string pw = txtPass?.Text ?? "";
            string confirm = txtConfirm?.Text ?? "";

            if (AuthService.TryRegister(fullName, email, phone, pw, confirm, out var err))
            {
                // Auto sign-in after successful registration so the main UI doesn't show "Chưa đăng nhập".
                if (AuthService.TryLogin(email, pw, out var user, out var loginErr) && user != null)
                {
                    AppSession.SignIn(user);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return;
                }

                // Fallback: registration succeeded but auto-login failed (unexpected).
                UIMessageBox.ShowSuccess("Đăng ký thành công! Bạn có thể đăng nhập ngay.");
                if (!string.IsNullOrWhiteSpace(loginErr))
                {
                    DemoPick.Services.DatabaseHelper.TryLog("Auth AutoLogin After Register Failed", new InvalidOperationException(loginErr), "FrmRegister.btnRegister_Click");
                }
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return;
            }

            UIMessageBox.ShowError(err ?? "Đăng ký thất bại.");
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel; // Go back to Login without registering
            this.Close();
        }

        private void lblLoginNow_Click(object sender, EventArgs e)
        {
            btnLogin_Click(sender, e);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            // Close this dialog and return to login; AppFlowContext will handle app exit if needed.
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void pnlLeft_Paint(object sender, PaintEventArgs e)
        {
            // Draw dotted background effect
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(200, 230, 200)))
            {
                for (int x = 20; x < pnlLeft.Width; x += 40)
                {
                    for (int y = 20; y < pnlLeft.Height; y += 40)
                    {
                        g.FillEllipse(brush, x, y, 3, 3);
                    }
                }
            }
        }
    }
}
