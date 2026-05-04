// ==========================================================
// File: UCBanHang.cs
// Role: View (MVC)
// Description: Giao diện POS bán hàng (nước uống, dịch vụ).
// ==========================================================
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Panel = System.Windows.Forms.Panel;
using Sunny.UI;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;

namespace DemoPick
{
    public partial class UCBanHang : UserControl
    {
        private string _selectedCourtName = "";
        private bool _shownSelectCourtHint;
        private readonly InventoryService _inventoryService;
        private ContextMenuStrip _cartContextMenu;

        // Khoi tao man hinh/control UCBanHang va chuan bi trang thai ban dau can dung.
        public UCBanHang()
        {
            InitializeComponent();

            if (DemoPick.Helpers.DesignModeUtil.IsDesignMode(this))
            {
                return;
            }

            _inventoryService = new InventoryService();

            pnlLeft.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(229, 231, 235), 1))
                {
                    e.Graphics.DrawLine(pen, pnlLeft.Width - 1, 0, pnlLeft.Width - 1, pnlLeft.Height);
                }
            };
            pnlRight.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(229, 231, 235), 1))
                {
                    e.Graphics.DrawLine(pen, 0, 0, 0, pnlRight.Height);
                }
            };
            pnlTotals.Paint += (s, e) =>
            {
                using (var dashPen = new Pen(Color.LightGray, 1) { DashStyle = DashStyle.Dash })
                {
                    e.Graphics.DrawLine(dashPen, 0, 75, pnlTotals.Width, 75);
                }
            };

            SetupCartColumns();
            SetupContextMenus();
            btnAddProduct.Click += BtnAddProduct_Click;
            btnDeleteProduct.Click += BtnDeleteProduct_Click;
            btnCheckout.Click += BtnSaveOrder_Click;
            btnClearOrder.Click += BtnClearOrder_Click;
            btnQtyPlus.Click += BtnQtyPlus_Click;
            btnQtyMinus.Click += BtnQtyMinus_Click;
            lstCart.KeyDown += LstCart_KeyDown;

            LoadAllData(resetCart: true);
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void BtnQtyPlus_Click(object sender, EventArgs e)
        {
            if (lstCart.SelectedItems.Count <= 0)
            {
                new UIPage().ShowInfoTip("Chọn sản phẩm trong giỏ để tăng số lượng.");
                return;
            }

            AdjustSelectedCartItemQuantity(+1);
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void BtnQtyMinus_Click(object sender, EventArgs e)
        {
            if (lstCart.SelectedItems.Count <= 0)
            {
                new UIPage().ShowInfoTip("Chọn sản phẩm trong giỏ để giảm số lượng.");
                return;
            }

            AdjustSelectedCartItemQuantity(-1);
        }

        // Xu ly phim bam de kich hoat thao tac nhanh hoac dieu huong tren man hinh.
        private void LstCart_KeyDown(object sender, KeyEventArgs e)
        {
            if (lstCart.SelectedItems.Count <= 0) return;

            bool plusPressed = e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus;
            bool minusPressed = e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus;

            if (plusPressed)
            {
                AdjustSelectedCartItemQuantity(+1);
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (minusPressed)
            {
                AdjustSelectedCartItemQuantity(-1);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        // Cap nhat lai du lieu/trang thai Refresh On Activated tren man hinh hien tai.
        public void RefreshOnActivated()
        {
            _selectedCourtName = "";
            _shownSelectCourtHint = false;
            LoadAllData(resetCart: true);
        }

        // Nap du lieu cho Load All Data roi cap nhat lai trang thai hien thi tren man hinh.
        private void LoadAllData(bool resetCart)
        {
            LoadCatalog();
            LoadCourts();

            if (resetCart)
            {
                lstCart.Items.Clear();
                if (string.IsNullOrWhiteSpace(_selectedCourtName))
                {
                    lblRightTitle.Text = "Chọn sân để thêm hàng";
                }
            }
        }

        // Thiet lap Setup Context Menus va gan cac cau hinh/su kien can dung ban dau.
        private void SetupContextMenus()
        {
            _cartContextMenu = new ContextMenuStrip();
            var mnuSetQty = new ToolStripMenuItem("Nhập số lượng...");
            mnuSetQty.Click += (s, e) => SetSelectedCartItemQuantity();
            _cartContextMenu.Items.Add(mnuSetQty);

            _cartContextMenu.Items.Add(new ToolStripSeparator());

            var mnuIncreaseQty = new ToolStripMenuItem("Tăng số lượng (+1)");
            mnuIncreaseQty.Click += (s, e) => AdjustSelectedCartItemQuantity(+1);
            _cartContextMenu.Items.Add(mnuIncreaseQty);

            var mnuDecreaseQty = new ToolStripMenuItem("Giảm số lượng (-1)");
            mnuDecreaseQty.Click += (s, e) => AdjustSelectedCartItemQuantity(-1);
            _cartContextMenu.Items.Add(mnuDecreaseQty);

            _cartContextMenu.Items.Add(new ToolStripSeparator());

            var mnuRemoveCart = new ToolStripMenuItem("Xóa món khỏi giỏ");
            mnuRemoveCart.Click += (s, e) => RemoveSelectedCartItem();
            _cartContextMenu.Items.Add(mnuRemoveCart);

            lstCart.MouseDown += (s, e) =>
            {
                if (e.Button != MouseButtons.Right) return;

                var hit = lstCart.HitTest(e.Location);
                if (hit?.Item == null) return;

                hit.Item.Selected = true;
                _cartContextMenu.Show(lstCart, e.Location);
            };

            lstCart.DoubleClick += (s, e) => SetSelectedCartItemQuantity();
        }

        // Xoa, huy hoac dat lai du lieu Remove Selected Cart Item theo dung dieu kien nghiep vu.
        private void RemoveSelectedCartItem()
        {
            if (lstCart.SelectedItems.Count <= 0) return;

            var item = lstCart.SelectedItems[0];
            var itemName = item.Text;
            lstCart.Items.Remove(item);
            SavePendingOrderFromCart(showSuccessTip: false);

            try
            {
                new UIPage().ShowSuccessTip($"Đã xóa '{itemName}' khỏi giỏ.");
            }
            catch
            {
                // Best effort.
            }
        }

        // Xu ly logic man hinh Adjust Selected Cart Item Quantity va cap nhat control lien quan.
        private void AdjustSelectedCartItemQuantity(int delta)
        {
            if (lstCart.SelectedItems.Count <= 0) return;

            var item = lstCart.SelectedItems[0];
            if (!(item.Tag is CartItemTag meta))
            {
                new UIPage().ShowWarningTip("Không xác định được dữ liệu món hàng.");
                return;
            }

            if (!int.TryParse(item.SubItems[1].Text, out int qty))
            {
                new UIPage().ShowWarningTip("Số lượng hiện tại không hợp lệ.");
                return;
            }

            qty += delta;
            if (qty <= 0)
            {
                lstCart.Items.Remove(item);
            }
            else
            {
                item.SubItems[1].Text = qty.ToString();
                item.SubItems[2].Text = (qty * meta.UnitPrice).ToString("N0") + "đ";
            }

            SavePendingOrderFromCart(showSuccessTip: false);
        }

        // Ap dung hoac chuan hoa trang thai Set Selected Cart Item Quantity de du lieu/giao dien nhat quan.
        private void SetSelectedCartItemQuantity()
        {
            if (lstCart.SelectedItems.Count <= 0) return;

            var item = lstCart.SelectedItems[0];
            if (!(item.Tag is CartItemTag meta))
            {
                new UIPage().ShowWarningTip("Không xác định được dữ liệu món hàng.");
                return;
            }

            if (!int.TryParse(item.SubItems[1].Text, out int currentQty))
            {
                new UIPage().ShowWarningTip("Số lượng hiện tại không hợp lệ.");
                return;
            }

            int? newQty = PromptQuantity(item.Text, currentQty);
            if (!newQty.HasValue) return;

            if (newQty.Value <= 0)
            {
                lstCart.Items.Remove(item);
            }
            else
            {
                item.SubItems[1].Text = newQty.Value.ToString();
                item.SubItems[2].Text = (newQty.Value * meta.UnitPrice).ToString("N0") + "đ";
            }

            SavePendingOrderFromCart(showSuccessTip: false);
        }

        // Xu ly logic man hinh Prompt Quantity va cap nhat control lien quan.
        private int? PromptQuantity(string itemName, int currentQty)
        {
            using (var dlg = new Form())
            {
                dlg.Text = "Sửa số lượng";
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;
                dlg.ClientSize = new Size(320, 150);

                var lbl = new Label
                {
                    AutoSize = false,
                    Left = 12,
                    Top = 12,
                    Width = 296,
                    Height = 40,
                    Text = $"Nhập số lượng cho '{itemName}' (0 để xóa):"
                };

                var nud = new NumericUpDown
                {
                    Left = 12,
                    Top = 58,
                    Width = 296,
                    Minimum = 0,
                    Maximum = 9999,
                    Value = currentQty,
                    TextAlign = HorizontalAlignment.Right
                };

                var btnOk = new Button
                {
                    Text = "OK",
                    Left = 152,
                    Width = 75,
                    Top = 105,
                    DialogResult = DialogResult.OK
                };

                var btnCancel = new Button
                {
                    Text = "Hủy",
                    Left = 233,
                    Width = 75,
                    Top = 105,
                    DialogResult = DialogResult.Cancel
                };

                dlg.Controls.Add(lbl);
                dlg.Controls.Add(nud);
                dlg.Controls.Add(btnOk);
                dlg.Controls.Add(btnCancel);
                dlg.AcceptButton = btnOk;
                dlg.CancelButton = btnCancel;

                return dlg.ShowDialog(this) == DialogResult.OK
                    ? (int?)Convert.ToInt32(nud.Value)
                    : null;
            }
        }

    }
}


