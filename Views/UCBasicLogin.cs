using System;
using System.Drawing;
using System.Windows.Forms;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;
using Sunny.UI;

namespace DemoPick
{
    public partial class UCBasicLogin : UserControl
    {
        public event EventHandler Authenticated;

        // Khoi tao man hinh/control UCBasicLogin va chuan bi trang thai ban dau can dung.
        public UCBasicLogin()
        {
            InitializeComponent();

            if (DesignModeUtil.IsDesignMode(this))
            {
                return;
            }

            // Make it rounded
            IntPtr hRgn = CreateRoundRectRgn(0, 0, Width, Height, 20, 20);
            if (hRgn != IntPtr.Zero)
            {
                this.Region = Region.FromHrgn(hRgn);
                DeleteObject(hRgn);
            }

            btnLogin.Click += BtnLogin_Click;
            txtPass.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnLogin.PerformClick(); };
            txtEmail.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) txtPass.Focus(); };
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        // Tao hoac tinh ra du lieu Create Round Rect Rgn tu cac thong tin dau vao hien co.
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        [System.Runtime.InteropServices.DllImport("gdi32.dll", SetLastError = true)]
        // Xoa, huy hoac dat lai du lieu Delete Object theo dung dieu kien nghiep vu.
        private static extern bool DeleteObject(IntPtr hObject);

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string id = txtEmail.Text.Trim();
            string pw = txtPass.Text;

            if (AuthService.TryLogin(id, pw, out var user, out var err))
            {
                AppSession.SignIn(user);
                Authenticated?.Invoke(this, EventArgs.Empty);
                return;
            }

            UIMessageBox.ShowError(err ?? "Sai tài khoản hoặc mật khẩu.");
        }
    }
}


