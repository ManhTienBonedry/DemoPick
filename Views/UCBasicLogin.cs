using System;
using System.Drawing;
using System.Windows.Forms;
using DemoPick.Services;
using Sunny.UI;

namespace DemoPick
{
    public partial class UCBasicLogin : UserControl
    {
        public event EventHandler Authenticated;

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
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        [System.Runtime.InteropServices.DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

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
