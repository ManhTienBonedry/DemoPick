using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DemoPick.Services;
using Sunny.UI;
using Panel = System.Windows.Forms.Panel;

namespace DemoPick
{
    public partial class UCBanHang
    {
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

        private async void LoadCatalog()
        {
            try
            {
                var chips = flpCategories.Controls.OfType<UCCategoryChip>().ToList();

                // Use Designer chip texts as defaults so UI still shows categories even when DB is empty.
                var baseCats = chips
                    .Select(c => (c.Tag ?? c.Text)?.ToString())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Trim())
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
                    string cat = (catRaw ?? "").Trim();
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

                flpProducts.Controls.Clear();
                var products = await _inventoryService.GetProductsAsync();
                int totalProds = 0;

                foreach (var prod in products)
                {
                    totalProds++;
                    int prodId = prod.ProductId;
                    string nameTxt = prod.Name ?? "";
                    decimal priceVal = prod.Price;
                    string priceTxt = priceVal.ToString("N0") + "đ";

                    Panel pnlProd = new Panel { Size = new Size(150, 180), BackColor = Color.White, Margin = new Padding(10), Cursor = Cursors.Hand, Tag = prod.Category ?? "" };
                    pnlProd.Paint += (s, e) => e.Graphics.DrawRectangle(new Pen(Color.FromArgb(229, 231, 235), 1), 0, 0, pnlProd.Width - 1, pnlProd.Height - 1);

                    PictureBox pic = new PictureBox { Size = new Size(130, 100), Location = new Point(10, 10), BackColor = Color.FromArgb(243, 244, 246) };
                    pic.Enabled = false;
                    Label nameLbl = new Label { Text = nameTxt, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(26, 35, 50), Location = new Point(10, 120), AutoSize = true };
                    nameLbl.Enabled = false;
                    Label priceLbl = new Label { Text = priceTxt, Font = new Font("Segoe UI", 10F, FontStyle.Regular), ForeColor = Color.FromArgb(76, 175, 80), Location = new Point(10, 145), AutoSize = true };
                    priceLbl.Enabled = false;

                    pnlProd.Controls.AddRange(new Control[] { pic, nameLbl, priceLbl });
                    pnlProd.Click += (s, e) => AddToCart(prodId, nameTxt, priceVal);

                    flpProducts.Controls.Add(pnlProd);
                }

                if (totalProds == 0)
                {
                    flpProducts.Controls.Add(new Label { Name = "lblEmptyProd", Text = "Toàn bộ Máy tính tiền đang trống vãn. Sếp nhập hàng vào Kho trước nhé!", Font = new Font("Segoe UI", 11F, FontStyle.Italic), AutoSize = true, Margin = new Padding(20), ForeColor = Color.Gray });
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("POS Load Catalog Error", ex, "UCBanHang.LoadCatalog");
            }
        }

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
                    flpProducts.Controls.Add(new Label { Name = "lblEmptyProd", Text = "Chưa có món hàng nào thuộc mục này.", Font = new Font("Segoe UI", 11F, FontStyle.Italic), AutoSize = true, Margin = new Padding(20), ForeColor = Color.Gray });
                }
                flpProducts.Controls["lblEmptyProd"].Visible = true;
            }
            else
            {
                if (flpProducts.Controls.ContainsKey("lblEmptyProd")) flpProducts.Controls["lblEmptyProd"].Visible = false;
            }
        }
    }
}
