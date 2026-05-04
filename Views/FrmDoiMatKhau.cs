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
    public partial class FrmDoiMatKhau : Form
    {
        // Khoi tao man hinh/control FrmDoiMatKhau va chuan bi trang thai ban dau can dung.
        public FrmDoiMatKhau()
        {
            InitializeComponent();

            if (DesignModeUtil.IsDesignMode(this))
            {
                return;
            }
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;

            // Rounded corners logic
            using (var path = RoundedRect(new Rectangle(0, 0, this.Width, this.Height), 20))
            {
                var old = this.Region;
                this.Region = new Region(path);
                if (old != null) old.Dispose();
            }
            this.Paint += Frm_Paint;
        }

        // Ve lai phan giao dien tuy bien cho control lien quan.
        private void Frm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (Pen p = new Pen(Color.FromArgb(220, 220, 220), 1))
            using (GraphicsPath path = RoundedRect(new Rectangle(0, 0, this.Width - 1, this.Height - 1), 20))
            {
                e.Graphics.DrawPath(p, path);
            }
        }

        // Xu ly logic man hinh Rounded Rect va cap nhat control lien quan.
        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void btnSave_Click(object sender, EventArgs e)
        {
            var user = AppSession.CurrentUser;
            if (user == null)
            {
                UIMessageBox.ShowError("Bạn cần đăng nhập để đổi mật khẩu.");
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return;
            }

            string oldPw = txtOldPass?.Text ?? "";
            string newPw = txtNewPass?.Text ?? "";
            string confirm = txtConfirm?.Text ?? "";

            // UX: validate early and focus the offending field.
            if (string.IsNullOrWhiteSpace(oldPw))
            {
                UIMessageBox.ShowError("Vui lòng nhập mật khẩu cũ.");
                FocusField(txtOldPass, selectAll: true);
                return;
            }
            if (string.IsNullOrWhiteSpace(newPw))
            {
                UIMessageBox.ShowError("Vui lòng nhập mật khẩu mới.");
                FocusField(txtNewPass, selectAll: true);
                return;
            }
            if (string.IsNullOrWhiteSpace(confirm))
            {
                UIMessageBox.ShowError("Vui lòng nhập xác nhận mật khẩu.");
                FocusField(txtConfirm, selectAll: true);
                return;
            }
            if (!string.Equals(newPw, confirm, StringComparison.Ordinal))
            {
                UIMessageBox.ShowError("Mật khẩu xác nhận không khớp.");
                FocusField(txtConfirm, selectAll: true);
                return;
            }

            if (AuthService.TryChangePassword(user.AccountId, oldPw, newPw, confirm, out var err))
            {
                UIMessageBox.ShowSuccess("Đổi mật khẩu thành công!");
                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }

            // UX: if old password is wrong, clear it to reduce repeated failed attempts.
            if (string.Equals(err, "Mật khẩu cũ không đúng.", StringComparison.Ordinal))
            {
                if (txtOldPass != null) txtOldPass.Text = "";
                UIMessageBox.ShowError(err);
                FocusField(txtOldPass, selectAll: false);
                return;
            }

            UIMessageBox.ShowError(err ?? "Đổi mật khẩu thất bại.");
            // Best-effort focus based on error text.
            if (!string.IsNullOrWhiteSpace(err))
            {
                if (err.IndexOf("xác nhận", StringComparison.OrdinalIgnoreCase) >= 0) FocusField(txtConfirm, selectAll: true);
                else if (err.IndexOf("mật khẩu mới", StringComparison.OrdinalIgnoreCase) >= 0) FocusField(txtNewPass, selectAll: true);
                else if (err.IndexOf("mật khẩu cũ", StringComparison.OrdinalIgnoreCase) >= 0) FocusField(txtOldPass, selectAll: true);
            }
        }

        // Xu ly logic man hinh Focus Field va cap nhat control lien quan.
        private static void FocusField(Control field, bool selectAll)
        {
            if (field == null) return;

            if (!field.IsDisposed && field.CanFocus)
            {
                field.Focus();
            }
            if (!selectAll) return;

            try
            {
                var selectAllMethod = field.GetType().GetMethod("SelectAll", Type.EmptyTypes);
                selectAllMethod?.Invoke(field, null);
            }
            catch
            {
                // Ignore: some custom textbox implementations may not support SelectAll.
            }
        }
    }
}


