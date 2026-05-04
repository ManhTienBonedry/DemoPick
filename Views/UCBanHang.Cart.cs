using DemoPick.Helpers;
using DemoPick.Data;
using System;
using System.Windows.Forms;
using DemoPick.Models;
using Sunny.UI;

namespace DemoPick
{
    public partial class UCBanHang
    {
        private sealed class CartItemTag
        {
            public int ProductId { get; }
            public decimal UnitPrice { get; }
            public string Category { get; }

            // Khoi tao man hinh/control CartItemTag va chuan bi trang thai ban dau can dung.
            public CartItemTag(int productId, decimal unitPrice, string category)
            {
                ProductId = productId;
                UnitPrice = unitPrice;
                Category = category;
            }
        }

        // Dieu huong hoac hien thi Show Select Court Hint If Needed theo trang thai hien tai cua ung dung.
        private void ShowSelectCourtHintIfNeeded()
        {
            if (_shownSelectCourtHint) return;
            if (!string.IsNullOrWhiteSpace(_selectedCourtName)) return;

            _shownSelectCourtHint = true;
            try
            {
                lblRightTitle.Text = "Chưa chọn sân";
                new UIPage().ShowInfoTip("Bạn cần chọn sân (bên trái) rồi mới thêm hàng.");
            }
            catch
            {
                // Best effort.
            }
        }

        // Dam bao dieu kien Ensure Court Selected For Add da san sang truoc khi chay buoc xu ly tiep theo.
        private bool EnsureCourtSelectedForAdd()
        {
            if (!string.IsNullOrWhiteSpace(_selectedCourtName)) return true;

            ShowSelectCourtHintIfNeeded();
            MessageBox.Show("Bạn cần chọn sân trước thì mới được thêm hàng.", "Chưa chọn sân", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void BtnClearOrder_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_selectedCourtName))
            {
                ShowSelectCourtHintIfNeeded();
                return;
            }
            lstCart.Items.Clear();
            Services.PosService.ClearPendingOrder(_selectedCourtName);
            new UIPage().ShowSuccessTip($"Đã xóa đơn chờ của {_selectedCourtName}.");
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void BtnSaveOrder_Click(object sender, EventArgs e)
        {
            SavePendingOrderFromCart(showSuccessTip: true);
        }

        // Luu hoac ghi nhan Save Pending Order From Cart vao trang thai he thong/CSDL khi nghiep vu yeu cau.
        private bool SavePendingOrderFromCart(bool showSuccessTip)
        {
            if (string.IsNullOrWhiteSpace(_selectedCourtName))
            {
                if (showSuccessTip)
                {
                    ShowSelectCourtHintIfNeeded();
                }
                return false;
            }

            if (lstCart.Items.Count == 0)
            {
                Services.PosService.ClearPendingOrder(_selectedCourtName);
                if (showSuccessTip)
                {
                    new UIPage().ShowSuccessTip("Đã xóa trắng giỏ hàng.");
                }
                return true;
            }

            try
            {
                var lines = new System.Collections.Generic.List<CartLine>();
                foreach (ListViewItem item in lstCart.Items)
                {
                    if (!(item.Tag is CartItemTag meta))
                        throw new InvalidOperationException("Lỗi: Không xác định được sản phẩm.");

                    if (!int.TryParse(item.SubItems[1].Text, out int qty))
                        throw new InvalidOperationException($"Số lượng không hợp lệ cho '{item.Text}'.");

                    lines.Add(new CartLine(meta.ProductId, item.Text, qty, meta.UnitPrice, meta.Category));
                }

                Services.PosService.SavePendingOrder(_selectedCourtName, lines);
                if (showSuccessTip)
                {
                    new UIPage().ShowSuccessTip($"Đã LƯU order cho {_selectedCourtName} thành công!");
                }
                return true;
            }
            catch (Exception ex)
            {
                if (showSuccessTip)
                {
                    new UIPage().ShowErrorTip("Lỗi lưu đơn: " + ex.Message);
                }
                else
                {
                    DemoPick.Data.DatabaseHelper.TryLog("POS Save Pending Error", ex, "UCBanHang.SavePendingOrderFromCart");
                }
                return false;
            }
        }

        // Thiet lap Setup Cart Columns va gan cac cau hinh/su kien can dung ban dau.
        private void SetupCartColumns()
        {
            lstCart.Columns.Add("Sản phẩm", 140);
            lstCart.Columns.Add("SL", 40);
            lstCart.Columns.Add("Thành tiền", 110);
        }

        // Xu ly logic man hinh Add To Cart va cap nhat control lien quan.
        private void AddToCart(string prodName, decimal unitPrice)
        {
            AddToCart(0, prodName, unitPrice, category: null);
        }

        // Xu ly logic man hinh Add To Cart va cap nhat control lien quan.
        private void AddToCart(int productId, string prodName, decimal unitPrice, string category)
        {
            if (!EnsureCourtSelectedForAdd()) return;

            bool found = false;
            foreach (ListViewItem item in lstCart.Items)
            {
                if (item.Tag is CartItemTag tag && tag.ProductId == productId && productId > 0)
                {
                    int qty = int.Parse(item.SubItems[1].Text) + 1;
                    item.SubItems[1].Text = qty.ToString();
                    item.SubItems[2].Text = (qty * unitPrice).ToString("N0") + "đ";
                    found = true;
                    break;
                }

                if (productId <= 0 && item.Text == prodName)
                {
                    int qty = int.Parse(item.SubItems[1].Text) + 1;
                    item.SubItems[1].Text = qty.ToString();
                    item.SubItems[2].Text = (qty * unitPrice).ToString("N0") + "đ";
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                var lvi = new ListViewItem(new[] { prodName, "1", unitPrice.ToString("N0") + "đ" });
                lvi.Tag = new CartItemTag(productId, unitPrice, category);
                lstCart.Items.Add(lvi);
            }
        }
    }
}



