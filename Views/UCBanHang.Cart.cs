using System;
using System.Windows.Forms;
using Sunny.UI;

namespace DemoPick
{
    public partial class UCBanHang
    {
        private sealed class CartItemTag
        {
            public int ProductId { get; }
            public decimal UnitPrice { get; }

            public CartItemTag(int productId, decimal unitPrice)
            {
                ProductId = productId;
                UnitPrice = unitPrice;
            }
        }

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

        private bool EnsureCourtSelectedForAdd()
        {
            if (!string.IsNullOrWhiteSpace(_selectedCourtName)) return true;

            ShowSelectCourtHintIfNeeded();
            MessageBox.Show("Bạn cần chọn sân trước thì mới được thêm hàng.", "Chưa chọn sân", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

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

        private void BtnSaveOrder_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_selectedCourtName))
            {
                ShowSelectCourtHintIfNeeded();
                return;
            }
            if (lstCart.Items.Count == 0)
            {
                Services.PosService.ClearPendingOrder(_selectedCourtName);
                new UIPage().ShowSuccessTip("Đã xóa trắng giỏ hàng.");
                return;
            }

            try
            {
                var lines = new System.Collections.Generic.List<Services.PosService.CartLine>();
                foreach (ListViewItem item in lstCart.Items)
                {
                    if (!(item.Tag is CartItemTag meta))
                        throw new InvalidOperationException("Lỗi: Không xác định được sản phẩm.");

                    if (!int.TryParse(item.SubItems[1].Text, out int qty))
                        throw new InvalidOperationException($"Số lượng không hợp lệ cho '{item.Text}'.");

                    lines.Add(new Services.PosService.CartLine(meta.ProductId, item.Text, qty, meta.UnitPrice));
                }

                Services.PosService.SavePendingOrder(_selectedCourtName, lines);
                new UIPage().ShowSuccessTip($"Đã LƯU order cho {_selectedCourtName} thành công!");
            }
            catch (Exception ex)
            {
                new UIPage().ShowErrorTip("Lỗi lưu đơn: " + ex.Message);
            }
        }

        private void SetupCartColumns()
        {
            lstCart.Columns.Add("Sản phẩm", 140);
            lstCart.Columns.Add("SL", 40);
            lstCart.Columns.Add("Thành tiền", 110);
        }

        private void AddToCart(string prodName, decimal unitPrice)
        {
            AddToCart(0, prodName, unitPrice);
        }

        private void AddToCart(int productId, string prodName, decimal unitPrice)
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
                if (productId > 0)
                    lvi.Tag = new CartItemTag(productId, unitPrice);
                lstCart.Items.Add(lvi);
            }
        }
    }
}
