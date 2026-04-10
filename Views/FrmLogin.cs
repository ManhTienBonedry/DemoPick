using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Sunny.UI;
using DemoPick.Services;

namespace DemoPick
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
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

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string id = txtEmail?.Text?.Trim() ?? "";
            string pw = txtPass?.Text ?? "";

            if (AuthService.TryLogin(id, pw, out var user, out var err))
            {
                AppSession.SignIn(user);
                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }

            UIMessageBox.ShowError(err ?? "Đăng nhập thất bại.");
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Open register
            var frmReg = new FrmRegister();
            this.Hide();
            if (frmReg.ShowDialog() == DialogResult.OK)
            {
                // Proceed to main only if registration also resulted in a signed-in session.
                if (AppSession.CurrentUser != null)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    // Fallback: show login again.
                    this.Show();
                }
            }
            else
            {
                this.Show();
            }
        }

        private void lblRegisterNow_Click(object sender, EventArgs e)
        {
            btnRegister_Click(sender, e);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            // Let AppFlowContext decide whether to exit.
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
