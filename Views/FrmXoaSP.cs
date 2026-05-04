using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using DemoPick.Models;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;
using Sunny.UI;

namespace DemoPick
{
    public partial class FrmXoaSP : UIForm
    {
        private readonly InventoryService _inventoryService;
        private bool _hasDeleted;

        // Khoi tao man hinh/control FrmXoaSP va chuan bi trang thai ban dau can dung.
        public FrmXoaSP()
        {
            InitializeComponent();
            _inventoryService = new InventoryService();
            SetupForm();
        }

        // Thiet lap Setup Form va gan cac cau hinh/su kien can dung ban dau.
        private void SetupForm()
        {
            _lstProducts.SelectedIndexChanged += (s, e) => UpdateDeleteButtonState();
            _pnlBottom.Resize += PnlBottom_Resize;

            Shown += async (s, e) => await LoadProductsAsync();
            FormClosing += (s, e) =>
            {
                if (_hasDeleted && DialogResult == DialogResult.None)
                {
                    DialogResult = DialogResult.OK;
                }
            };

            _btnClose.Click += (s, e) =>
            {
                DialogResult = _hasDeleted ? DialogResult.OK : DialogResult.Cancel;
                Close();
            };

            _btnDelete.Click += async (s, e) => await DeleteSelectedAsync();

            PnlBottom_Resize(_pnlBottom, EventArgs.Empty);
            UpdateDeleteButtonState();
        }

        // Cap nhat bo cuc khi kich thuoc man hinh/control thay doi.
        private void PnlBottom_Resize(object sender, EventArgs e)
        {
            if (_pnlBottom == null || _btnClose == null || _btnDelete == null)
            {
                return;
            }

            _btnClose.Left = _pnlBottom.Width - _btnClose.Width;
            _btnClose.Top = 10;

            _btnDelete.Left = _btnClose.Left - 10 - _btnDelete.Width;
            _btnDelete.Top = 10;
        }

        // Cap nhat lai du lieu/trang thai Update Delete Button State tren man hinh hien tai.
        private void UpdateDeleteButtonState()
        {
            _btnDelete.Enabled = _lstProducts.SelectedItems.Count > 0;
        }

        // Nap du lieu cho Load Products roi cap nhat lai trang thai hien thi tren man hinh.
        private async Task LoadProductsAsync()
        {
            try
            {
                _btnDelete.Enabled = false;
                _lstProducts.BeginUpdate();
                _lstProducts.Items.Clear();

                var products = await _inventoryService.GetProductsForDeletionAsync();
                foreach (var p in products)
                {
                    if (p == null || p.ProductId <= 0) continue;

                    var lvi = new ListViewItem(p.ProductId.ToString())
                    {
                        Tag = p
                    };

                    lvi.SubItems.Add((p.Name ?? string.Empty).Trim());
                    lvi.SubItems.Add((p.Category ?? string.Empty).Trim());
                    lvi.SubItems.Add(p.Price.ToString("N0") + "đ");
                    lvi.SubItems.Add(p.StockQuantity.ToString("N0"));

                    _lstProducts.Items.Add(lvi);
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("Load Delete Products Error", ex, "FrmXoaSP.LoadProductsAsync");
                MessageBox.Show("Không thể tải danh sách sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _lstProducts.EndUpdate();
                UpdateDeleteButtonState();
            }
        }

        // Nap du lieu cho Get Selected Product roi cap nhat lai trang thai hien thi tren man hinh.
        private ProductDeleteListItemModel GetSelectedProduct()
        {
            if (_lstProducts.SelectedItems.Count <= 0) return null;
            return _lstProducts.SelectedItems[0].Tag as ProductDeleteListItemModel;
        }

        // Xoa, huy hoac dat lai du lieu Delete Selected theo dung dieu kien nghiep vu.
        private async Task DeleteSelectedAsync()
        {
            var p = GetSelectedProduct();
            if (p == null || p.ProductId <= 0)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (p.Category != null && p.Category.Trim().Equals(AppConstants.ProductCategories.Service, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    "Không thể xóa các sản phẩm thuộc danh mục 'Dịch vụ' mặc định.", 
                    "Không thể xóa", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn xóa '{p.Name}'?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            var result = await _inventoryService.DeleteProductAsync(p.ProductId);
            if (!result.Success)
            {
                MessageBox.Show(result.Message ?? "Xóa sản phẩm thất bại.", "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _hasDeleted = true;
            await LoadProductsAsync();

            try
            {
                new UIPage().ShowSuccessTip("Đã xóa sản phẩm.");
            }
            catch
            {
                // Best effort.
            }
        }
    }
}


