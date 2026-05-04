using System;
using System.Drawing;
using System.Windows.Forms;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;
using Sunny.UI;

namespace DemoPick
{
    public partial class UCRegister : UserControl
    {
        public event EventHandler Authenticated;
        public event EventHandler RequestLogin;

        // Khoi tao man hinh/control UCRegister va chuan bi trang thai ban dau can dung.
        public UCRegister()
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
                // Match legacy register card look (rounded corners).
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

            // Ensure clickable UI elements are actually wired.
            // (Some of these are intentionally not wired in Designer.)
            if (btnClose != null) btnClose.Click += btnClose_Click;
            if (btnRegister != null) btnRegister.Click += btnRegister_Click;
            if (lblLoginNow != null) lblLoginNow.Click += lblLoginNow_Click;
            if (txtPhone != null)
            {
                txtPhone.TextChanged += (s, e) => UpdatePhoneValidationUi();
            }
        }

        // Cap nhat lai du lieu/trang thai Update Phone Validation Ui tren man hinh hien tai.
        private void UpdatePhoneValidationUi()
        {
            if (txtPhone == null) return;

            string raw = txtPhone.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(raw))
            {
                txtPhone.RectColor = Color.LightGray;
                return;
            }

            txtPhone.RectColor = PhoneNumberValidator.IsValidTenDigits(raw)
                ? Color.LightGray
                : Color.FromArgb(231, 76, 60);
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
                    Authenticated?.Invoke(this, EventArgs.Empty);
                    return;
                }

                // Fallback: registration succeeded but auto-login failed (unexpected).
                UIMessageBox.ShowSuccess("Đăng ký thành công! Bạn có thể đăng nhập ngay.");
                if (!string.IsNullOrWhiteSpace(loginErr))
                {
                    DemoPick.Data.DatabaseHelper.TryLog("Auth AutoLogin After Register Failed", new InvalidOperationException(loginErr), "UCRegister.btnRegister_Click");
                }

                RequestLogin?.Invoke(this, EventArgs.Empty);
                return;
            }

            UIMessageBox.ShowError(err ?? "Đăng ký thất bại.");
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void btnLogin_Click(object sender, EventArgs e)
        {
            RequestLogin?.Invoke(this, EventArgs.Empty);
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void lblLoginNow_Click(object sender, EventArgs e)
        {
            btnLogin_Click(sender, e);
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void btnClose_Click(object sender, EventArgs e)
        {
            // Close this card and return to login.
            RequestLogin?.Invoke(this, EventArgs.Empty);
        }
    }
}


