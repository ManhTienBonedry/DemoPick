using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using DemoPick.Models;
using DemoPick.Services;
using Sunny.UI;

namespace DemoPick
{
    public class FrmXoaSP : UIForm
    {
        private readonly InventoryService _inventoryService;

        private readonly ListView _lstProducts;
        private readonly UIButton _btnDelete;
        private readonly UIButton _btnClose;

        private bool _hasDeleted;

        public FrmXoaSP()
        {
            _inventoryService = new InventoryService();

            Text = "Xóa sản phẩm";
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(900, 520);
            Size = new Size(1024, 600);

            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 48,
                Padding = new Padding(12, 10, 12, 0)
            };

            var lblTitle = new Label
            {
                AutoSize = true,
                Text = "Danh sách sản phẩm để xóa",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold)
            };
            pnlTop.Controls.Add(lblTitle);

            _lstProducts = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                HideSelection = false,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F)
            };
            _lstProducts.Columns.Add("ID", 70, HorizontalAlignment.Right);
            _lstProducts.Columns.Add("Tên", 320, HorizontalAlignment.Left);
            _lstProducts.Columns.Add("Danh mục", 180, HorizontalAlignment.Left);
            _lstProducts.Columns.Add("Giá", 120, HorizontalAlignment.Right);
            _lstProducts.Columns.Add("Tồn", 80, HorizontalAlignment.Right);
            _lstProducts.SelectedIndexChanged += (s, e) => UpdateDeleteButtonState();

            var pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 64,
                Padding = new Padding(12, 10, 12, 10)
            };

            _btnClose = new UIButton
            {
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Radius = 14,
                Size = new Size(120, 35),
                Text = "Đóng",
                FillColor = Color.FromArgb(107, 114, 128),
                FillHoverColor = Color.FromArgb(127, 134, 148),
                RectColor = Color.Transparent
            };

            _btnDelete = new UIButton
            {
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Radius = 14,
                Size = new Size(180, 35),
                Text = "Xóa sản phẩm",
                FillColor = Color.FromArgb(231, 76, 60),
                FillHoverColor = Color.FromArgb(241, 86, 70),
                RectColor = Color.FromArgb(231, 76, 60),
                RectHoverColor = Color.FromArgb(241, 86, 70)
            };

            pnlBottom.Controls.Add(_btnClose);
            pnlBottom.Controls.Add(_btnDelete);

            pnlBottom.Resize += (s, e) =>
            {
                _btnClose.Left = pnlBottom.Width - _btnClose.Width;
                _btnClose.Top = 10;

                _btnDelete.Left = _btnClose.Left - 10 - _btnDelete.Width;
                _btnDelete.Top = 10;
            };

            Controls.Add(_lstProducts);
            Controls.Add(pnlBottom);
            Controls.Add(pnlTop);

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

            UpdateDeleteButtonState();
        }

        private void UpdateDeleteButtonState()
        {
            _btnDelete.Enabled = _lstProducts.SelectedItems.Count > 0;
        }

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

        private ProductDeleteListItemModel GetSelectedProduct()
        {
            if (_lstProducts.SelectedItems.Count <= 0) return null;
            return _lstProducts.SelectedItems[0].Tag as ProductDeleteListItemModel;
        }

        private async Task DeleteSelectedAsync()
        {
            var p = GetSelectedProduct();
            if (p == null || p.ProductId <= 0)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
