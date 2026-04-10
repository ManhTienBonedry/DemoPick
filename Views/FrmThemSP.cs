using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DemoPick.Services;

namespace DemoPick
{
    public partial class FrmThemSP : Sunny.UI.UIForm
    {
        private readonly InventoryService _inventoryService;

        public FrmThemSP()
        {
            InitializeComponent();
            _inventoryService = new InventoryService();
            txtSKU.Text = "PD-" + DateTime.Now.ToString("yyMMddHHmm");
            SetupForm();
        }

        private void SetupForm()
        {
            btnDong.Click += (s, e) => this.Close();
            btnLuu.Click += BtnLuu_Click;
        }

        private async void BtnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTen.Text) || string.IsNullOrWhiteSpace(txtGia.Text))
            {
                MessageBox.Show("Sếp vui lòng nhập đủ Tên Món và Đơn Giá nhé!", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtGia.Text.Trim(), NumberStyles.Any, CultureInfo.CurrentCulture, out decimal price) || price <= 0)
            {
                MessageBox.Show("Đơn giá không hợp lệ (phải là số > 0).", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtSoLuong.Text.Trim(), out int qty) || qty <= 0)
            {
                MessageBox.Show("Số lượng không hợp lệ (phải là số nguyên > 0).", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                await _inventoryService.AddProductAsync(
                    txtSKU.Text.Trim(),
                    txtTen.Text.Trim(),
                    cboLoai.SelectedItem?.ToString() ?? "",
                    price,
                    qty,
                    minThreshold: 5
                );

                MessageBox.Show("Bơm hàng thành công rực rỡ! Đội POS vỗ tay!", "Cập Nhật SQL", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mẻ nạp hàng lỗi CSDL: " + ex.Message, "Phản Lệnh", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
