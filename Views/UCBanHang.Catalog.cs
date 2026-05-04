using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;
using Sunny.UI;
using Panel = System.Windows.Forms.Panel;

namespace DemoPick
{
    public partial class UCBanHang
    {
        private static readonly Font _posProductNameFont = new Font("Segoe UI", 10F, FontStyle.Bold);
        private static readonly Font _posProductPriceFont = new Font("Segoe UI", 10F, FontStyle.Regular);
        private static readonly Font _posEmptyStateFont = new Font("Segoe UI", 11F, FontStyle.Italic);
        private static readonly Font _posCourtNameFont = new Font("Segoe UI", 11F, FontStyle.Bold);
        private static readonly Font _posCourtBadgeFont = new Font("Segoe UI", 9F, FontStyle.Bold);
        private static readonly Font _posCourtTimeFont = new Font("Segoe UI", 9F, FontStyle.Regular);

        // Xoa, huy hoac dat lai du lieu Clear And Dispose Child Controls theo dung dieu kien nghiep vu.
        private static void ClearAndDisposeChildControls(Control parent)
        {
            if (parent == null) return;
            if (parent.Controls == null) return;
            if (parent.Controls.Count == 0) return;

            var old = new Control[parent.Controls.Count];
            parent.Controls.CopyTo(old, 0);
            parent.Controls.Clear();

            for (int i = 0; i < old.Length; i++)
            {
                old[i].Dispose();
            }
        }

        // Ap dung hoac chuan hoa trang thai Normalize Pos Category de du lieu/giao dien nhat quan.
        private static string NormalizePosCategory(string category)
        {
            string c = (category ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(c)) return string.Empty;

            if (string.Equals(c, "Thuê Dụng cụ", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(c, "Thuê dụng cụ", StringComparison.OrdinalIgnoreCase))
            {
                return "Dịch vụ";
            }

            return c;
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void BtnAddProduct_Click(object sender, EventArgs e)
        {
            try
            {
                using (var f = new FrmThemSP())
                {
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        LoadCatalog();
                        new UIPage().ShowSuccessTip("Đã cập nhật danh sách hàng hóa.");
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("POS Add Product Error", ex, "UCBanHang.BtnAddProduct_Click");
                new UIPage().ShowErrorTip("Không thể mở form thêm sản phẩm: " + ex.Message);
            }
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void BtnDeleteProduct_Click(object sender, EventArgs e)
        {
            try
            {
                using (var f = new FrmXoaSP())
                {
                    if (f.ShowDialog(this) == DialogResult.OK)
                    {
                        if (!string.IsNullOrWhiteSpace(_selectedCourtName))
                        {
                            LoadPendingOrderForCourt(_selectedCourtName);
                        }

                        LoadCatalog();

                        try
                        {
                            var main = FindForm() as FrmChinh;
                            if (main?.khoHang != null)
                            {
                                main.khoHang.RefreshOnActivated();
                            }
                        }
                        catch
                        {
                            // Best effort cross-module refresh.
                        }

                        new UIPage().ShowSuccessTip("Đã cập nhật danh sách hàng hóa.");
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("POS Delete Product Page Error", ex, "UCBanHang.BtnDeleteProduct_Click");
                new UIPage().ShowErrorTip("Không thể mở trang xóa sản phẩm: " + ex.Message);
            }
        }

        // Nap du lieu cho Load Catalog roi cap nhat lai trang thai hien thi tren man hinh.
        private async void LoadCatalog()
        {
            try
            {
                var chips = flpCategories.Controls.OfType<UCCategoryChip>().ToList();

                // Use Designer chip texts as defaults so UI still shows categories even when DB is empty.
                var baseCats = chips
                    .Select(c => (c.Tag ?? c.Text)?.ToString())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => NormalizePosCategory(s))
                    .ToList();

                var catList = new System.Collections.Generic.List<string> { "Tất cả" };
                foreach (var c in baseCats)
                {
                    if (string.Equals(c, "Tất cả", StringComparison.OrdinalIgnoreCase)) continue;
                    if (!catList.Contains(c)) catList.Add(c);
                }

                var dbCats = await _inventoryService.GetProductCategoriesAsync();
                foreach (var catRaw in dbCats)
                {
                    string cat = NormalizePosCategory(catRaw);
                    if (string.IsNullOrWhiteSpace(cat)) continue;
                    if (string.Equals(cat, "Tất cả", StringComparison.OrdinalIgnoreCase)) continue;
                    if (!catList.Contains(cat)) catList.Add(cat);
                }

                int bindCount = Math.Min(catList.Count, chips.Count);
                for (int i = 0; i < chips.Count; i++)
                {
                    var chip = chips[i];

                    if (i < bindCount)
                    {
                        chip.Visible = true;
                        chip.Tag = catList[i];
                        chip.Text = catList[i];
                        chip.IsActive = chip.Text == "Tất cả";

                        chip.Click -= CategoryChip_Click;
                        chip.Click += CategoryChip_Click;
                    }
                    else
                    {
                        // Hide unused placeholder chips (if any were added in Designer).
                        chip.Visible = false;
                    }
                }

                if (catList.Count > chips.Count)
                {
                    DatabaseHelper.TryLog(
                        "POS Category Chips Insufficient",
                        new InvalidOperationException($"Need {catList.Count} chips, but only {chips.Count} exist in Designer."),
                        "UCBanHang.LoadCatalog");
                }

                ClearAndDisposeChildControls(flpProducts);
                var products = await _inventoryService.GetProductsAsync();
                int totalProds = 0;

                foreach (var prod in products)
                {
                    totalProds++;
                    int prodId = prod.ProductId;
                    string nameTxt = prod.Name ?? "";
                    decimal priceVal = prod.Price;
                    string catNorm = NormalizePosCategory(prod.Category);
                    string priceTxt = priceVal.ToString("N0") + "đ";

                    Panel pnlProd = new Panel { Size = new Size(150, 180), BackColor = Color.White, Margin = new Padding(10), Cursor = Cursors.Hand, Tag = catNorm };
                    pnlProd.Paint += (s, e) =>
                    {
                        using (var pen = new Pen(Color.FromArgb(229, 231, 235), 1))
                        {
                            e.Graphics.DrawRectangle(pen, 0, 0, pnlProd.Width - 1, pnlProd.Height - 1);
                        }
                    };

                    PictureBox pic = new PictureBox { Size = new Size(130, 100), Location = new Point(10, 10), BackColor = Color.FromArgb(243, 244, 246), Cursor = Cursors.Hand };
                    Label nameLbl = new Label { Text = nameTxt, Font = _posProductNameFont, ForeColor = Color.FromArgb(26, 35, 50), Location = new Point(10, 120), AutoSize = true, Cursor = Cursors.Hand };
                    Label priceLbl = new Label { Text = priceTxt, Font = _posProductPriceFont, ForeColor = Color.FromArgb(76, 175, 80), Location = new Point(10, 145), AutoSize = true, Cursor = Cursors.Hand };

                    pnlProd.Controls.AddRange(new Control[] { pic, nameLbl, priceLbl });
                    UiTheme.NormalizeTextBackgrounds(pnlProd);
                    
                    EventHandler onClick = (s, e) => AddToCart(prodId, nameTxt, priceVal, catNorm);
                    pnlProd.Click += onClick;
                    pic.Click += onClick;
                    nameLbl.Click += onClick;
                    priceLbl.Click += onClick;

                    flpProducts.Controls.Add(pnlProd);
                }

                if (totalProds == 0)
                {
                    flpProducts.Controls.Add(new Label { Name = "lblEmptyProd", Text = "Toàn bộ Máy tính tiền đang trống vãn. Sếp nhập hàng vào Kho trước nhé!", Font = _posEmptyStateFont, AutoSize = true, Margin = new Padding(20), ForeColor = Color.Gray });
                }

                UiTheme.NormalizeTextBackgrounds(flpProducts);
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("POS Load Catalog Error", ex, "UCBanHang.LoadCatalog");
            }
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void CategoryChip_Click(object sender, EventArgs e)
        {
            if (!(sender is UCCategoryChip chip)) return;
            string filterCat = (chip.Tag ?? chip.Text ?? "").ToString();

            foreach (var cb in flpCategories.Controls.OfType<UCCategoryChip>())
            {
                cb.SetActive(cb == chip);
            }

            int hitCount = 0;
            foreach (Control pCtrl in flpProducts.Controls)
            {
                if (pCtrl.Name == "lblEmptyProd") continue;
                if (filterCat == "Tất cả" || pCtrl.Tag?.ToString() == filterCat) { pCtrl.Visible = true; hitCount++; }
                else pCtrl.Visible = false;
            }

            if (hitCount == 0)
            {
                if (!flpProducts.Controls.ContainsKey("lblEmptyProd"))
                {
                    flpProducts.Controls.Add(new Label { Name = "lblEmptyProd", Text = "Chưa có món hàng nào thuộc mục này.", Font = _posEmptyStateFont, AutoSize = true, Margin = new Padding(20), ForeColor = Color.Gray });
                }
                flpProducts.Controls["lblEmptyProd"].Visible = true;
                UiTheme.NormalizeTextBackgrounds(flpProducts);
            }
            else
            {
                if (flpProducts.Controls.ContainsKey("lblEmptyProd")) flpProducts.Controls["lblEmptyProd"].Visible = false;
            }
        }
    }
}
