using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Panel = System.Windows.Forms.Panel;
using Sunny.UI;
using DemoPick.Services;

namespace DemoPick
{
    public partial class UCBanHang : UserControl
    {
        private string _selectedCourtName = "";
        private bool _shownSelectCourtHint;
        private readonly InventoryService _inventoryService;

        public UCBanHang()
        {
            InitializeComponent();

            if (DemoPick.Services.DesignModeUtil.IsDesignMode(this))
            {
                return;
            }

            _inventoryService = new InventoryService();

            pnlLeft.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Color.FromArgb(229, 231, 235), 1), pnlLeft.Width - 1, 0, pnlLeft.Width - 1, pnlLeft.Height);
            pnlRight.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Color.FromArgb(229, 231, 235), 1), 0, 0, 0, pnlRight.Height);
            pnlTotals.Paint += (s, e) =>
            {
                Pen dashPen = new Pen(Color.LightGray, 1) { DashStyle = DashStyle.Dash };
                e.Graphics.DrawLine(dashPen, 0, 75, pnlTotals.Width, 75);
            };

            SetupCartColumns();
            btnAddProduct.Click += BtnAddProduct_Click;
            btnCheckout.Click += BtnSaveOrder_Click;
            btnClearOrder.Click += BtnClearOrder_Click;

            LoadAllData(resetCart: true);
        }

        public void RefreshOnActivated()
        {
            // Called when user navigates back to POS, so it picks up changes from Inventory.
            LoadCatalog();
            LoadCourts();
        }

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
    }
}
