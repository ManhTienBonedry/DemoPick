using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DemoPick.Services;
using Sunny.UI;

namespace DemoPick
{
    public partial class UCLogin : UserControl
    {
        public event EventHandler Authenticated;
        public event EventHandler RequestRegister;
        public event EventHandler RequestExit;

        public UCLogin()
        {
            InitializeComponent();

            if (DesignModeUtil.IsDesignMode(this))
            {
                return;
            }

            try
            {
                // Match legacy login card look (rounded corners).
                this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            }
            catch
            {
                // ignore
            }
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse);

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string id = txtEmail?.Text?.Trim() ?? "";
            string pw = txtPass?.Text ?? "";

            if (AuthService.TryLogin(id, pw, out var user, out var err))
            {
                AppSession.SignIn(user);
                Authenticated?.Invoke(this, EventArgs.Empty);
                return;
            }

            UIMessageBox.ShowError(err ?? "Đăng nhập thất bại.");
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            RequestRegister?.Invoke(this, EventArgs.Empty);
        }

        private void lblRegisterNow_Click(object sender, EventArgs e)
        {
            RequestRegister?.Invoke(this, EventArgs.Empty);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            RequestExit?.Invoke(this, EventArgs.Empty);
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
