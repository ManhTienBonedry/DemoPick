using System;
using System.Drawing;
using System.Windows.Forms;

namespace DemoPick
{
    public partial class FrmDatSanCoDinh
    {
        private void FrmDatSanCoDinh_Load(object sender, EventArgs e)
        {
            try
            {
                var courts = _controller.GetCourts();
                cbCourt.DataSource = courts;
                cbCourt.DisplayMember = "Name";
                cbCourt.ValueMember = "CourtID";

                if (courts.Count > 0)
                    cbCourt.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Data Sân: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RbMode_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBaoTri.Checked)
            {
                txtName.Enabled = false;
                txtPhone.Enabled = false;
                txtName.Text = "Ban Quản Lý (Bảo Trì)";
                txtPhone.Text = "N/A";

                btnConfirm.FillColor = Color.FromArgb(231, 76, 60); // Red
                btnConfirm.FillHoverColor = Color.FromArgb(241, 86, 70);
                btnConfirm.Text = "Xác nhận Khóa Sân";
            }
            else
            {
                txtName.Enabled = true;
                txtPhone.Enabled = true;
                txtName.Text = "";
                txtPhone.Text = "";

                btnConfirm.FillColor = Color.FromArgb(46, 204, 113); // Green
                btnConfirm.FillHoverColor = Color.FromArgb(56, 214, 123);
                btnConfirm.Text = "Tạo Lịch Cố Định";
            }
        }
    }
}
