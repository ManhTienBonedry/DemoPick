using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;
using Sunny.UI;

namespace DemoPick
{
    public partial class UCLogin : UserControl
    {
        public event EventHandler Authenticated;
        public event EventHandler RequestRegister;
        public event EventHandler RequestExit;

        // Khoi tao man hinh/control UCLogin va chuan bi trang thai ban dau can dung.
        public UCLogin()
        {
            InitializeComponent();

            if (DesignModeUtil.IsDesignMode(this))
            {
                return;
            }

            // Use the normal window close button instead of a custom big 'X' on the card.
            if (btnClose != null)
            {
                btnClose.Visible = false;
            }

            try
            {
                // Match legacy login card look (rounded corners).
                IntPtr hRgn = CreateRoundRectRgn(0, 0, Width, Height, 20, 20);
                if (hRgn != IntPtr.Zero)
                {
                    this.Region = Region.FromHrgn(hRgn);
                    DeleteObject(hRgn);
                }
            }
            catch
            {
                // ignore
            }
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        // Tao hoac tinh ra du lieu Create Round Rect Rgn tu cac thong tin dau vao hien co.
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse);

        [System.Runtime.InteropServices.DllImport("gdi32.dll", SetLastError = true)]
        // Xoa, huy hoac dat lai du lieu Delete Object theo dung dieu kien nghiep vu.
        private static extern bool DeleteObject(IntPtr hObject);

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
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

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void btnRegister_Click(object sender, EventArgs e)
        {
            RequestRegister?.Invoke(this, EventArgs.Empty);
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void lblRegisterNow_Click(object sender, EventArgs e)
        {
            RequestRegister?.Invoke(this, EventArgs.Empty);
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void btnClose_Click(object sender, EventArgs e)
        {
            RequestExit?.Invoke(this, EventArgs.Empty);
        }

        // Ve lai phan giao dien tuy bien cho control lien quan.
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


