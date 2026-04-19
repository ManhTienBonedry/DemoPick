using System.Drawing;
using System.Windows.Forms;
using Sunny.UI;

namespace DemoPick
{
    partial class UCBanHang
    {
        private System.ComponentModel.IContainer components = null;

        public System.Windows.Forms.Panel pnlLeft;
        public System.Windows.Forms.Label lblLeftTitle;
        public System.Windows.Forms.FlowLayoutPanel flpCourts;

        public System.Windows.Forms.Panel pnlCenter;
        public System.Windows.Forms.Panel pnlCategoryBar;
        public System.Windows.Forms.FlowLayoutPanel flpCategories;
        public System.Windows.Forms.FlowLayoutPanel flpProducts;
        public UIButton btnAddProduct;
        public UIButton btnDeleteProduct;
        public UCCategoryChip chipCatAll;
        public UCCategoryChip chipCatDrink;
        public UCCategoryChip chipCatSnack;
        public UCCategoryChip chipCatTool;
        public UCCategoryChip chipCatExtra1;
        public UCCategoryChip chipCatExtra2;
        public UCCategoryChip chipCatExtra3;
        public UCCategoryChip chipCatExtra4;
        public UCCategoryChip chipCatExtra5;
        public UCCategoryChip chipCatExtra6;

        public System.Windows.Forms.Panel pnlRight;
        public System.Windows.Forms.Label lblRightTitle;
        public System.Windows.Forms.ListView lstCart;
        public UIButton btnQtyPlus;
        public UIButton btnQtyMinus;
        
        public System.Windows.Forms.Panel pnlTotals;
        public UIButton btnClearOrder;
        public UIButton btnCheckout;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.lblLeftTitle = new System.Windows.Forms.Label();
            this.flpCourts = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlCenter = new System.Windows.Forms.Panel();
            this.pnlCategoryBar = new System.Windows.Forms.Panel();
            this.flpProducts = new System.Windows.Forms.FlowLayoutPanel();
            this.flpCategories = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAddProduct = new Sunny.UI.UIButton();
            this.btnDeleteProduct = new Sunny.UI.UIButton();
            this.chipCatAll = new DemoPick.UCCategoryChip();
            this.chipCatDrink = new DemoPick.UCCategoryChip();
            this.chipCatSnack = new DemoPick.UCCategoryChip();
            this.chipCatTool = new DemoPick.UCCategoryChip();
            this.chipCatExtra1 = new DemoPick.UCCategoryChip();
            this.chipCatExtra2 = new DemoPick.UCCategoryChip();
            this.chipCatExtra3 = new DemoPick.UCCategoryChip();
            this.chipCatExtra4 = new DemoPick.UCCategoryChip();
            this.chipCatExtra5 = new DemoPick.UCCategoryChip();
            this.chipCatExtra6 = new DemoPick.UCCategoryChip();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.lblRightTitle = new System.Windows.Forms.Label();
            this.btnQtyPlus = new Sunny.UI.UIButton();
            this.btnQtyMinus = new Sunny.UI.UIButton();
            this.lstCart = new System.Windows.Forms.ListView();
            this.pnlTotals = new System.Windows.Forms.Panel();
            this.btnCheckout = new Sunny.UI.UIButton();
            this.btnClearOrder = new Sunny.UI.UIButton();
            this.pnlLeft.SuspendLayout();
            this.pnlCenter.SuspendLayout();
            this.pnlCategoryBar.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlTotals.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLeft
            // 
            this.pnlLeft.BackColor = System.Drawing.Color.White;
            this.pnlLeft.Controls.Add(this.lblLeftTitle);
            this.pnlLeft.Controls.Add(this.flpCourts);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(300, 820);
            this.pnlLeft.TabIndex = 2;
            // 
            // lblLeftTitle
            // 
            this.lblLeftTitle.AutoSize = true;
            this.lblLeftTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblLeftTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblLeftTitle.Location = new System.Drawing.Point(20, 20);
            this.lblLeftTitle.Name = "lblLeftTitle";
            this.lblLeftTitle.Size = new System.Drawing.Size(175, 32);
            this.lblLeftTitle.TabIndex = 0;
            this.lblLeftTitle.Text = "Trạng thái sân";
            // 
            // flpCourts
            // 
            this.flpCourts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpCourts.AutoScroll = true;
            this.flpCourts.Location = new System.Drawing.Point(20, 60);
            this.flpCourts.Name = "flpCourts";
            this.flpCourts.Size = new System.Drawing.Size(260, 740);
            this.flpCourts.TabIndex = 1;

            // 
            // pnlCenter
            // 
            this.pnlCenter.BackColor = System.Drawing.Color.Transparent;
            this.pnlCenter.Controls.Add(this.flpProducts);
            this.pnlCenter.Controls.Add(this.pnlCategoryBar);
            this.pnlCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCenter.Location = new System.Drawing.Point(300, 0);
            this.pnlCenter.Name = "pnlCenter";
            this.pnlCenter.Padding = new System.Windows.Forms.Padding(20);
            this.pnlCenter.Size = new System.Drawing.Size(510, 820);
            this.pnlCenter.TabIndex = 0;

            // 
            // pnlCategoryBar
            // 
            this.pnlCategoryBar.BackColor = System.Drawing.Color.Transparent;
            this.pnlCategoryBar.Controls.Add(this.flpCategories);
            this.pnlCategoryBar.Controls.Add(this.btnDeleteProduct);
            this.pnlCategoryBar.Controls.Add(this.btnAddProduct);
            this.pnlCategoryBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCategoryBar.Location = new System.Drawing.Point(20, 20);
            this.pnlCategoryBar.Name = "pnlCategoryBar";
            this.pnlCategoryBar.Size = new System.Drawing.Size(470, 60);
            this.pnlCategoryBar.TabIndex = 2;
            // 
            // flpProducts
            // 
            this.flpProducts.AutoScroll = true;
            this.flpProducts.BackColor = System.Drawing.Color.Transparent;
            this.flpProducts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpProducts.Location = new System.Drawing.Point(20, 80);
            this.flpProducts.Name = "flpProducts";
            this.flpProducts.Size = new System.Drawing.Size(470, 720);
            this.flpProducts.TabIndex = 0;
            // 
            // flpCategories
            // 
            this.flpCategories.AutoScroll = true;
            this.flpCategories.BackColor = System.Drawing.Color.Transparent;
            this.flpCategories.Controls.Add(this.chipCatAll);
            this.flpCategories.Controls.Add(this.chipCatDrink);
            this.flpCategories.Controls.Add(this.chipCatSnack);
            this.flpCategories.Controls.Add(this.chipCatTool);
            this.flpCategories.Controls.Add(this.chipCatExtra1);
            this.flpCategories.Controls.Add(this.chipCatExtra2);
            this.flpCategories.Controls.Add(this.chipCatExtra3);
            this.flpCategories.Controls.Add(this.chipCatExtra4);
            this.flpCategories.Controls.Add(this.chipCatExtra5);
            this.flpCategories.Controls.Add(this.chipCatExtra6);
            this.flpCategories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpCategories.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.flpCategories.Location = new System.Drawing.Point(0, 0);
            this.flpCategories.Name = "flpCategories";
            this.flpCategories.Size = new System.Drawing.Size(360, 60);
            this.flpCategories.TabIndex = 1;
            this.flpCategories.WrapContents = false;

            // 
            // chipCatAll
            // 
            this.chipCatAll.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.chipCatAll.Name = "chipCatAll";
            this.chipCatAll.Size = new System.Drawing.Size(100, 35);
            this.chipCatAll.TabIndex = 2;
            this.chipCatAll.Text = "Tất cả";

            // 
            // chipCatDrink
            // 
            this.chipCatDrink.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.chipCatDrink.Name = "chipCatDrink";
            this.chipCatDrink.Size = new System.Drawing.Size(110, 35);
            this.chipCatDrink.TabIndex = 3;
            this.chipCatDrink.Text = "Thức uống";

            // 
            // chipCatSnack
            // 
            this.chipCatSnack.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.chipCatSnack.Name = "chipCatSnack";
            this.chipCatSnack.Size = new System.Drawing.Size(120, 35);
            this.chipCatSnack.TabIndex = 4;
            this.chipCatSnack.Text = "Đồ ăn nhẹ";

            // 
            // chipCatTool
            // 
            this.chipCatTool.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.chipCatTool.Name = "chipCatTool";
            this.chipCatTool.Size = new System.Drawing.Size(140, 35);
            this.chipCatTool.TabIndex = 5;
            this.chipCatTool.Text = "Dịch vụ";

            // 
            // chipCatExtra1
            // 
            this.chipCatExtra1.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.chipCatExtra1.Name = "chipCatExtra1";
            this.chipCatExtra1.Size = new System.Drawing.Size(100, 35);
            this.chipCatExtra1.TabIndex = 6;
            this.chipCatExtra1.Text = "";
            this.chipCatExtra1.Visible = false;

            // 
            // chipCatExtra2
            // 
            this.chipCatExtra2.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.chipCatExtra2.Name = "chipCatExtra2";
            this.chipCatExtra2.Size = new System.Drawing.Size(100, 35);
            this.chipCatExtra2.TabIndex = 7;
            this.chipCatExtra2.Text = "";
            this.chipCatExtra2.Visible = false;

            // 
            // chipCatExtra3
            // 
            this.chipCatExtra3.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.chipCatExtra3.Name = "chipCatExtra3";
            this.chipCatExtra3.Size = new System.Drawing.Size(100, 35);
            this.chipCatExtra3.TabIndex = 8;
            this.chipCatExtra3.Text = "";
            this.chipCatExtra3.Visible = false;

            // 
            // chipCatExtra4
            // 
            this.chipCatExtra4.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.chipCatExtra4.Name = "chipCatExtra4";
            this.chipCatExtra4.Size = new System.Drawing.Size(100, 35);
            this.chipCatExtra4.TabIndex = 9;
            this.chipCatExtra4.Text = "";
            this.chipCatExtra4.Visible = false;

            // 
            // chipCatExtra5
            // 
            this.chipCatExtra5.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.chipCatExtra5.Name = "chipCatExtra5";
            this.chipCatExtra5.Size = new System.Drawing.Size(100, 35);
            this.chipCatExtra5.TabIndex = 10;
            this.chipCatExtra5.Text = "";
            this.chipCatExtra5.Visible = false;

            // 
            // chipCatExtra6
            // 
            this.chipCatExtra6.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.chipCatExtra6.Name = "chipCatExtra6";
            this.chipCatExtra6.Size = new System.Drawing.Size(100, 35);
            this.chipCatExtra6.TabIndex = 11;
            this.chipCatExtra6.Text = "";
            this.chipCatExtra6.Visible = false;

            // 
            // btnAddProduct
            // 
            this.btnAddProduct.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddProduct.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnAddProduct.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnAddProduct.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(86)))), ((int)(((byte)(185)))), ((int)(((byte)(90)))));
            this.btnAddProduct.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAddProduct.Location = new System.Drawing.Point(360, 0);
            this.btnAddProduct.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnAddProduct.Name = "btnAddProduct";
            this.btnAddProduct.Radius = 18;
            this.btnAddProduct.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnAddProduct.Size = new System.Drawing.Size(110, 60);
            this.btnAddProduct.TabIndex = 0;
            this.btnAddProduct.Text = "+ Thêm SP";
            // 
            // btnDeleteProduct
            // 
            this.btnDeleteProduct.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDeleteProduct.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnDeleteProduct.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnDeleteProduct.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(86)))), ((int)(((byte)(70)))));
            this.btnDeleteProduct.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDeleteProduct.Location = new System.Drawing.Point(250, 0);
            this.btnDeleteProduct.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnDeleteProduct.Name = "btnDeleteProduct";
            this.btnDeleteProduct.Radius = 18;
            this.btnDeleteProduct.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnDeleteProduct.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(86)))), ((int)(((byte)(70)))));
            this.btnDeleteProduct.Size = new System.Drawing.Size(110, 60);
            this.btnDeleteProduct.TabIndex = 3;
            this.btnDeleteProduct.Text = "Xóa SP";
            this.btnDeleteProduct.TipsFont = new System.Drawing.Font("Segoe UI", 9F);
            this.btnAddProduct.TipsFont = new System.Drawing.Font("Segoe UI", 9F);
            //
            // pnlRight
            // 
            this.pnlRight.BackColor = System.Drawing.Color.White;
            this.pnlRight.Controls.Add(this.btnQtyPlus);
            this.pnlRight.Controls.Add(this.btnQtyMinus);
            this.pnlRight.Controls.Add(this.lblRightTitle);
            this.pnlRight.Controls.Add(this.lstCart);
            this.pnlRight.Controls.Add(this.pnlTotals);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlRight.Location = new System.Drawing.Point(810, 0);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(350, 820);
            this.pnlRight.TabIndex = 1;
            // 
            // lblRightTitle
            // 
            this.lblRightTitle.AutoSize = true;
            this.lblRightTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblRightTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblRightTitle.Location = new System.Drawing.Point(20, 20);
            this.lblRightTitle.Name = "lblRightTitle";
            this.lblRightTitle.Size = new System.Drawing.Size(244, 32);
            this.lblRightTitle.TabIndex = 0;
            this.lblRightTitle.Text = "Sản phẩm chờ - Sân VIP 1";
            // 
            // btnQtyPlus
            // 
            this.btnQtyPlus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnQtyPlus.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnQtyPlus.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(214)))), ((int)(((byte)(123)))));
            this.btnQtyPlus.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnQtyPlus.Location = new System.Drawing.Point(295, 20);
            this.btnQtyPlus.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnQtyPlus.Name = "btnQtyPlus";
            this.btnQtyPlus.Radius = 14;
            this.btnQtyPlus.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnQtyPlus.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(214)))), ((int)(((byte)(123)))));
            this.btnQtyPlus.Size = new System.Drawing.Size(35, 30);
            this.btnQtyPlus.TabIndex = 4;
            this.btnQtyPlus.Text = "+";
            this.btnQtyPlus.TipsFont = new System.Drawing.Font("Segoe UI", 9F);
            // 
            // btnQtyMinus
            // 
            this.btnQtyMinus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnQtyMinus.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnQtyMinus.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(86)))), ((int)(((byte)(70)))));
            this.btnQtyMinus.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnQtyMinus.Location = new System.Drawing.Point(254, 20);
            this.btnQtyMinus.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnQtyMinus.Name = "btnQtyMinus";
            this.btnQtyMinus.Radius = 14;
            this.btnQtyMinus.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnQtyMinus.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(86)))), ((int)(((byte)(70)))));
            this.btnQtyMinus.Size = new System.Drawing.Size(35, 30);
            this.btnQtyMinus.TabIndex = 5;
            this.btnQtyMinus.Text = "-";
            this.btnQtyMinus.TipsFont = new System.Drawing.Font("Segoe UI", 9F);
            // 
            // lstCart
            // 
            this.lstCart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstCart.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstCart.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lstCart.FullRowSelect = true;
            this.lstCart.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstCart.HideSelection = false;
            this.lstCart.Location = new System.Drawing.Point(20, 60);
            this.lstCart.Name = "lstCart";
            this.lstCart.Size = new System.Drawing.Size(310, 650);
            this.lstCart.TabIndex = 0;
            this.lstCart.UseCompatibleStateImageBehavior = false;
            this.lstCart.View = System.Windows.Forms.View.Details;
            // 
            // pnlTotals
            // 
            this.pnlTotals.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTotals.Controls.Add(this.btnCheckout);
            this.pnlTotals.Controls.Add(this.btnClearOrder);
            this.pnlTotals.Location = new System.Drawing.Point(20, 720);
            this.pnlTotals.Name = "pnlTotals";
            this.pnlTotals.Size = new System.Drawing.Size(310, 80);
            this.pnlTotals.TabIndex = 1;
            // 
            // btnCheckout
            // 
            this.btnCheckout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCheckout.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnCheckout.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(214)))), ((int)(((byte)(123)))));
            this.btnCheckout.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCheckout.Location = new System.Drawing.Point(165, 15);
            this.btnCheckout.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnCheckout.Name = "btnCheckout";
            this.btnCheckout.Radius = 18;
            this.btnCheckout.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnCheckout.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(214)))), ((int)(((byte)(123)))));
            this.btnCheckout.Size = new System.Drawing.Size(140, 50);
            this.btnCheckout.TabIndex = 1;
            this.btnCheckout.Text = "LƯU (F9)";
            this.btnCheckout.TipsFont = new System.Drawing.Font("Segoe UI", 9F);
            // 
            // btnClearOrder
            // 
            this.btnClearOrder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClearOrder.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnClearOrder.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(86)))), ((int)(((byte)(70)))));
            this.btnClearOrder.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnClearOrder.Location = new System.Drawing.Point(5, 15);
            this.btnClearOrder.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnClearOrder.Name = "btnClearOrder";
            this.btnClearOrder.Radius = 18;
            this.btnClearOrder.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnClearOrder.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(86)))), ((int)(((byte)(70)))));
            this.btnClearOrder.Size = new System.Drawing.Size(140, 50);
            this.btnClearOrder.TabIndex = 2;
            this.btnClearOrder.Text = "XÓA ĐƠN";
            this.btnClearOrder.TipsFont = new System.Drawing.Font("Segoe UI", 9F);
            // 
            // UCBanHang
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.pnlCenter);
            this.Controls.Add(this.pnlRight);
            this.Controls.Add(this.pnlLeft);
            this.Name = "UCBanHang";
            this.Size = new System.Drawing.Size(1160, 820);
            this.pnlLeft.ResumeLayout(false);
            this.pnlLeft.PerformLayout();
            this.pnlCenter.ResumeLayout(false);
            this.pnlCategoryBar.ResumeLayout(false);
            this.pnlRight.ResumeLayout(false);
            this.pnlRight.PerformLayout();
            this.pnlTotals.ResumeLayout(false);
            this.pnlTotals.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
