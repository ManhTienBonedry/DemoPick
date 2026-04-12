using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DemoPick.Services;

namespace DemoPick
{
    public partial class FrmThemSP : Sunny.UI.UIForm
    {
        private readonly InventoryService _inventoryService;
        private static readonly string[] DefaultCategories =
        {
            "Thức uống",
            "Đồ ăn nhẹ",
            "Thuê Dụng cụ",
            "Khác"
        };

        public FrmThemSP()
        {
            InitializeComponent();
            _inventoryService = new InventoryService();
            txtSKU.Text = GenerateInternalSku();
            SetupForm();
        }

        private void SetupForm()
        {
            btnDong.Click += (s, e) => this.Close();
            btnLuu.Click += BtnLuu_Click;
            this.Shown += async (s, e) => await LoadCategoriesAsync();

            // SKU is an internal code (anti-duplication). We keep it in DB but hide it from UI.
            try
            {
                if (lblSKU != null) lblSKU.Visible = false;
                if (txtSKU != null) txtSKU.Visible = false;
            }
            catch
            {
                // Best effort.
            }
        }

        private static string GenerateInternalSku()
        {
            // Not a "scan code" for now; just a unique internal identifier.
            string ts = DateTime.Now.ToString("yyMMddHHmmss");
            string rand = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpperInvariant();
            return $"PD-{ts}-{rand}";
        }

        private async System.Threading.Tasks.Task LoadCategoriesAsync()
        {
            cboLoai.Items.Clear();

            var seen = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var cat in DefaultCategories)
            {
                if (string.IsNullOrWhiteSpace(cat)) continue;
                if (seen.Add(cat.Trim())) cboLoai.Items.Add(cat.Trim());
            }

            try
            {
                var categories = await _inventoryService.GetProductCategoriesAsync();
                if (categories != null)
                {
                    foreach (var cat in categories)
                    {
                        var normalized = (cat ?? string.Empty).Trim();
                        if (string.IsNullOrWhiteSpace(normalized)) continue;
                        if (seen.Add(normalized)) cboLoai.Items.Add(normalized);
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("Load Categories Error", ex, "FrmThemSP.LoadCategoriesAsync");
            }

            if (cboLoai.Items.Count > 0)
            {
                cboLoai.SelectedIndex = 0;
            }
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
                MessageBox.Show("Đơn giá không hợp lệ.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtSoLuong.Text.Trim(), out int qty) || qty <= 0)
            {
                MessageBox.Show("Số lượng không hợp lệ.", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string sku = (txtSKU.Text ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(sku) || sku.StartsWith("SVC-", StringComparison.OrdinalIgnoreCase))
                {
                    sku = GenerateInternalSku();
                    txtSKU.Text = sku;
                }

                const int maxAttempts = 3;
                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    try
                    {
                        await _inventoryService.AddProductAsync(
                            sku,
                            txtTen.Text.Trim(),
                            cboLoai.SelectedItem?.ToString() ?? "",
                            price,
                            qty,
                            minThreshold: 5
                        );
                        break;
                    }
                    catch (SqlException ex) when ((ex.Number == 2601 || ex.Number == 2627) && attempt < maxAttempts)
                    {
                        // SKU unique index collision - regenerate and retry.
                        sku = GenerateInternalSku();
                        txtSKU.Text = sku;
                    }
                }

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
