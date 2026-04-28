using System.Drawing;
using System.Windows.Forms;
using Sunny.UI;

namespace DemoPick
{
    partial class UCThanhToan
    {
        private System.ComponentModel.IContainer components = null;

        public System.Windows.Forms.Panel pnlLeft;
        public System.Windows.Forms.Label lblLeftTitle;
        public System.Windows.Forms.FlowLayoutPanel flpCourts;

        public System.Windows.Forms.Panel pnlCenter;
        public System.Windows.Forms.Label lblRightTitle;
        public UITextBox txtCustomerPhone;
        public UIButton btnSearchCustomer;
        public System.Windows.Forms.Label lblCustomerInfo;
        public System.Windows.Forms.Label lblPaymentStateHint;
        public System.Windows.Forms.ListView lstCart;
        
        public System.Windows.Forms.Panel pnlTotals;
        public System.Windows.Forms.Label lblSubTotalT; 
        public System.Windows.Forms.Label lblSubTotalV;
        public System.Windows.Forms.Label lblDiscountT; 
        public System.Windows.Forms.Label lblDiscountV;
        public System.Windows.Forms.Label lblTotalT; 
        public System.Windows.Forms.Label lblTotalV;
        public UIButton btnCheckout;
        public UIButton btnCancel;
        public UCInvoiceReprintPanel ucInvoiceReprintPanel;

        public System.Windows.Forms.Panel pnlRight;
        public System.Windows.Forms.Label lblPreviewTitle;
        public System.Windows.Forms.Panel pnlMockInvoice;
        public System.Windows.Forms.Label lblPreviewTotal;
        public System.Windows.Forms.Label lblPreviewDesc;
        public UCPaymentHistoryPanel ucPaymentHistoryPanel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.lblLeftTitle = new System.Windows.Forms.Label();
            this.flpCourts = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlCenter = new System.Windows.Forms.Panel();
            this.lblRightTitle = new System.Windows.Forms.Label();
            this.txtCustomerPhone = new Sunny.UI.UITextBox();
            this.btnSearchCustomer = new Sunny.UI.UIButton();
            this.lblCustomerInfo = new System.Windows.Forms.Label();
            this.lblPaymentStateHint = new System.Windows.Forms.Label();
            this.lstCart = new System.Windows.Forms.ListView();
            this.pnlTotals = new System.Windows.Forms.Panel();
            this.ucInvoiceReprintPanel = new DemoPick.UCInvoiceReprintPanel();
            this.lblSubTotalT = new System.Windows.Forms.Label();
            this.lblSubTotalV = new System.Windows.Forms.Label();
            this.lblDiscountT = new System.Windows.Forms.Label();
            this.lblDiscountV = new System.Windows.Forms.Label();
            this.lblTotalT = new System.Windows.Forms.Label();
            this.lblTotalV = new System.Windows.Forms.Label();
            this.btnCheckout = new Sunny.UI.UIButton();
            this.btnCancel = new Sunny.UI.UIButton();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.pnlMockInvoice = new System.Windows.Forms.Panel();
            this.lblPreviewTotal = new System.Windows.Forms.Label();
            this.lblPreviewDesc = new System.Windows.Forms.Label();
            this.lblPreviewTitle = new System.Windows.Forms.Label();
            this.ucPaymentHistoryPanel = new DemoPick.UCPaymentHistoryPanel();
            this.pnlLeft.SuspendLayout();
            this.pnlCenter.SuspendLayout();
            this.pnlTotals.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlMockInvoice.SuspendLayout();
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
            this.pnlLeft.Size = new System.Drawing.Size(280, 820);
            this.pnlLeft.TabIndex = 0;
            // 
            // lblLeftTitle
            // 
            this.lblLeftTitle.AutoSize = true;
            this.lblLeftTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblLeftTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblLeftTitle.Location = new System.Drawing.Point(20, 20);
            this.lblLeftTitle.Name = "lblLeftTitle";
            this.lblLeftTitle.Size = new System.Drawing.Size(222, 32);
            this.lblLeftTitle.TabIndex = 0;
            this.lblLeftTitle.Text = "Chọn sân tính tiền";
            // 
            // flpCourts
            // 
            this.flpCourts.AutoScroll = true;
            this.flpCourts.Location = new System.Drawing.Point(20, 70);
            this.flpCourts.Name = "flpCourts";
            this.flpCourts.Size = new System.Drawing.Size(260, 730);
            this.flpCourts.TabIndex = 1;
            // 
            // pnlCenter
            // 
            this.pnlCenter.BackColor = System.Drawing.Color.White;
            this.pnlCenter.Controls.Add(this.lblRightTitle);
            this.pnlCenter.Controls.Add(this.txtCustomerPhone);
            this.pnlCenter.Controls.Add(this.btnSearchCustomer);
            this.pnlCenter.Controls.Add(this.lblCustomerInfo);
            this.pnlCenter.Controls.Add(this.lblPaymentStateHint);
            this.pnlCenter.Controls.Add(this.lstCart);
            this.pnlCenter.Controls.Add(this.pnlTotals);
            this.pnlCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCenter.Location = new System.Drawing.Point(280, 0);
            this.pnlCenter.Name = "pnlCenter";
            this.pnlCenter.Padding = new System.Windows.Forms.Padding(20);
            this.pnlCenter.Size = new System.Drawing.Size(560, 820);
            this.pnlCenter.TabIndex = 1;
            // 
            // lblRightTitle
            // 
            this.lblRightTitle.AutoSize = true;
            this.lblRightTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblRightTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblRightTitle.Location = new System.Drawing.Point(20, 20);
            this.lblRightTitle.Name = "lblRightTitle";
            this.lblRightTitle.Size = new System.Drawing.Size(243, 32);
            this.lblRightTitle.TabIndex = 0;
            this.lblRightTitle.Text = "Hóa đơn thanh toán";
            // 
            // txtCustomerPhone
            // 
            this.txtCustomerPhone.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtCustomerPhone.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtCustomerPhone.Location = new System.Drawing.Point(20, 70);
            this.txtCustomerPhone.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtCustomerPhone.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtCustomerPhone.Name = "txtCustomerPhone";
            this.txtCustomerPhone.Padding = new System.Windows.Forms.Padding(10, 5, 5, 5);
            this.txtCustomerPhone.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.txtCustomerPhone.ShowText = false;
            this.txtCustomerPhone.Size = new System.Drawing.Size(260, 44);
            this.txtCustomerPhone.TabIndex = 1;
            this.txtCustomerPhone.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtCustomerPhone.Watermark = "SĐT Khách hàng / VIP ID...";
            // 
            // btnSearchCustomer
            // 
            this.btnSearchCustomer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearchCustomer.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnSearchCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnSearchCustomer.Location = new System.Drawing.Point(290, 70);
            this.btnSearchCustomer.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSearchCustomer.Name = "btnSearchCustomer";
            this.btnSearchCustomer.RectColor = System.Drawing.Color.Transparent;
            this.btnSearchCustomer.Size = new System.Drawing.Size(100, 44);
            this.btnSearchCustomer.TabIndex = 2;
            this.btnSearchCustomer.Text = "Check Số";
            this.btnSearchCustomer.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // lblCustomerInfo
            // 
            this.lblCustomerInfo.AutoSize = true;
            this.lblCustomerInfo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Italic);
            this.lblCustomerInfo.ForeColor = System.Drawing.Color.Gray;
            this.lblCustomerInfo.Location = new System.Drawing.Point(20, 125);
            this.lblCustomerInfo.Name = "lblCustomerInfo";
            this.lblCustomerInfo.Size = new System.Drawing.Size(226, 23);
            this.lblCustomerInfo.TabIndex = 3;
            this.lblCustomerInfo.Text = "Khách lẻ (Không áp dụng thẻ)";
            // 
            // lblPaymentStateHint
            // 
            this.lblPaymentStateHint.AutoSize = true;
            this.lblPaymentStateHint.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblPaymentStateHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.lblPaymentStateHint.Location = new System.Drawing.Point(20, 148);
            this.lblPaymentStateHint.Name = "lblPaymentStateHint";
            this.lblPaymentStateHint.Size = new System.Drawing.Size(216, 21);
            this.lblPaymentStateHint.TabIndex = 6;
            this.lblPaymentStateHint.Text = "Không có booking đến hạn";
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
            this.lstCart.Location = new System.Drawing.Point(20, 180);
            this.lstCart.Name = "lstCart";
            this.lstCart.Size = new System.Drawing.Size(520, 430);
            this.lstCart.TabIndex = 4;
            this.lstCart.UseCompatibleStateImageBehavior = false;
            this.lstCart.View = System.Windows.Forms.View.Details;
            // 
            // pnlTotals
            // 
            this.pnlTotals.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTotals.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.pnlTotals.Controls.Add(this.ucInvoiceReprintPanel);
            this.pnlTotals.Controls.Add(this.lblSubTotalT);
            this.pnlTotals.Controls.Add(this.lblSubTotalV);
            this.pnlTotals.Controls.Add(this.lblDiscountT);
            this.pnlTotals.Controls.Add(this.lblDiscountV);
            this.pnlTotals.Controls.Add(this.lblTotalT);
            this.pnlTotals.Controls.Add(this.lblTotalV);
            this.pnlTotals.Controls.Add(this.btnCheckout);
            this.pnlTotals.Controls.Add(this.btnCancel);
            this.pnlTotals.Location = new System.Drawing.Point(20, 620);
            this.pnlTotals.Name = "pnlTotals";
            this.pnlTotals.Size = new System.Drawing.Size(520, 180);
            this.pnlTotals.TabIndex = 5;
            // 
            // ucInvoiceReprintPanel
            // 
            this.ucInvoiceReprintPanel.BackColor = System.Drawing.Color.Transparent;
            this.ucInvoiceReprintPanel.InvoiceIdText = "";
            this.ucInvoiceReprintPanel.Location = new System.Drawing.Point(300, 15);
            this.ucInvoiceReprintPanel.Name = "ucInvoiceReprintPanel";
            this.ucInvoiceReprintPanel.Size = new System.Drawing.Size(205, 72);
            this.ucInvoiceReprintPanel.TabIndex = 8;
            // 
            // lblSubTotalT
            // 
            this.lblSubTotalT.AutoSize = true;
            this.lblSubTotalT.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblSubTotalT.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblSubTotalT.Location = new System.Drawing.Point(20, 15);
            this.lblSubTotalT.Name = "lblSubTotalT";
            this.lblSubTotalT.Size = new System.Drawing.Size(130, 25);
            this.lblSubTotalT.TabIndex = 0;
            this.lblSubTotalT.Text = "Tổng tạm tính";
            // 
            // lblSubTotalV
            // 
            this.lblSubTotalV.AutoSize = true;
            this.lblSubTotalV.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblSubTotalV.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblSubTotalV.Location = new System.Drawing.Point(180, 15);
            this.lblSubTotalV.Name = "lblSubTotalV";
            this.lblSubTotalV.Size = new System.Drawing.Size(35, 25);
            this.lblSubTotalV.TabIndex = 1;
            this.lblSubTotalV.Text = "0đ";
            // 
            // lblDiscountT
            // 
            this.lblDiscountT.AutoSize = true;
            this.lblDiscountT.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblDiscountT.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblDiscountT.Location = new System.Drawing.Point(20, 45);
            this.lblDiscountT.Name = "lblDiscountT";
            this.lblDiscountT.Size = new System.Drawing.Size(119, 25);
            this.lblDiscountT.TabIndex = 2;
            this.lblDiscountT.Text = "Giảm giá thẻ";
            // 
            // lblDiscountV
            // 
            this.lblDiscountV.AutoSize = true;
            this.lblDiscountV.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblDiscountV.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.lblDiscountV.Location = new System.Drawing.Point(180, 45);
            this.lblDiscountV.Name = "lblDiscountV";
            this.lblDiscountV.Size = new System.Drawing.Size(35, 25);
            this.lblDiscountV.TabIndex = 3;
            this.lblDiscountV.Text = "0đ";
            // 
            // lblTotalT
            // 
            this.lblTotalT.AutoSize = true;
            this.lblTotalT.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTotalT.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblTotalT.Location = new System.Drawing.Point(20, 85);
            this.lblTotalT.Name = "lblTotalT";
            this.lblTotalT.Size = new System.Drawing.Size(155, 30);
            this.lblTotalT.TabIndex = 4;
            this.lblTotalT.Text = "Thành tiền (*)";
            // 
            // lblTotalV
            // 
            this.lblTotalV.AutoSize = true;
            this.lblTotalV.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTotalV.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.lblTotalV.Location = new System.Drawing.Point(180, 80);
            this.lblTotalV.Name = "lblTotalV";
            this.lblTotalV.Size = new System.Drawing.Size(50, 37);
            this.lblTotalV.TabIndex = 5;
            this.lblTotalV.Text = "0đ";
            // 
            // btnCheckout
            // 
            this.btnCheckout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCheckout.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.btnCheckout.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.btnCheckout.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCheckout.Location = new System.Drawing.Point(300, 115);
            this.btnCheckout.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnCheckout.Name = "btnCheckout";
            this.btnCheckout.Radius = 15;
            this.btnCheckout.RectColor = System.Drawing.Color.Transparent;
            this.btnCheckout.RectHoverColor = System.Drawing.Color.Transparent;
            this.btnCheckout.Size = new System.Drawing.Size(200, 50);
            this.btnCheckout.TabIndex = 6;
            this.btnCheckout.Text = "🖨 THANH TOÁN";
            this.btnCheckout.TipsFont = new System.Drawing.Font("Segoe UI", 9F);
            // 
            // btnCancel
            // 
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.btnCancel.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.btnCancel.Location = new System.Drawing.Point(20, 115);
            this.btnCancel.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Radius = 15;
            this.btnCancel.RectColor = System.Drawing.Color.Transparent;
            this.btnCancel.RectHoverColor = System.Drawing.Color.Transparent;
            this.btnCancel.Size = new System.Drawing.Size(120, 50);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "HỦY";
            this.btnCancel.TipsFont = new System.Drawing.Font("Segoe UI", 9F);
            // 
            // pnlRight
            // 
            this.pnlRight.AutoScroll = true;
            this.pnlRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.pnlRight.Controls.Add(this.pnlMockInvoice);
            this.pnlRight.Controls.Add(this.lblPreviewTitle);
            this.pnlRight.Controls.Add(this.ucPaymentHistoryPanel);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlRight.Location = new System.Drawing.Point(840, 0);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(320, 820);
            this.pnlRight.TabIndex = 2;
            // 
            // pnlMockInvoice
            // 
            this.pnlMockInvoice.BackColor = System.Drawing.Color.White;
            this.pnlMockInvoice.Controls.Add(this.lblPreviewTotal);
            this.pnlMockInvoice.Controls.Add(this.lblPreviewDesc);
            this.pnlMockInvoice.Location = new System.Drawing.Point(20, 70);
            this.pnlMockInvoice.Name = "pnlMockInvoice";
            this.pnlMockInvoice.Size = new System.Drawing.Size(280, 487);
            this.pnlMockInvoice.TabIndex = 1;
            // 
            // lblPreviewTotal
            // 
            this.lblPreviewTotal.AutoSize = true;
            this.lblPreviewTotal.Font = new System.Drawing.Font("Courier New", 14F, System.Drawing.FontStyle.Bold);
            this.lblPreviewTotal.Location = new System.Drawing.Point(15, 450);
            this.lblPreviewTotal.Name = "lblPreviewTotal";
            this.lblPreviewTotal.Size = new System.Drawing.Size(40, 27);
            this.lblPreviewTotal.TabIndex = 1;
            this.lblPreviewTotal.Text = "0đ";
            // 
            // lblPreviewDesc
            // 
            this.lblPreviewDesc.AutoSize = true;
            this.lblPreviewDesc.Font = new System.Drawing.Font("Courier New", 9F);
            this.lblPreviewDesc.ForeColor = System.Drawing.Color.Gray;
            this.lblPreviewDesc.Location = new System.Drawing.Point(15, 15);
            this.lblPreviewDesc.Name = "lblPreviewDesc";
            this.lblPreviewDesc.Size = new System.Drawing.Size(215, 68);
            this.lblPreviewDesc.TabIndex = 0;
            this.lblPreviewDesc.Text = "   GREEN COURT\n-----------------------\nHÓA ĐƠN THANH TOÁN SÂN\n...";
            // 
            // lblPreviewTitle
            // 
            this.lblPreviewTitle.AutoSize = true;
            this.lblPreviewTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblPreviewTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblPreviewTitle.Location = new System.Drawing.Point(20, 20);
            this.lblPreviewTitle.Name = "lblPreviewTitle";
            this.lblPreviewTitle.Size = new System.Drawing.Size(177, 28);
            this.lblPreviewTitle.TabIndex = 0;
            this.lblPreviewTitle.Text = "Preview Hóa Đơn";
            // 
            // ucPaymentHistoryPanel
            // 
            this.ucPaymentHistoryPanel.BackColor = System.Drawing.Color.Transparent;
            this.ucPaymentHistoryPanel.Location = new System.Drawing.Point(20, 576);
            this.ucPaymentHistoryPanel.Name = "ucPaymentHistoryPanel";
            this.ucPaymentHistoryPanel.SearchKeyword = "";
            this.ucPaymentHistoryPanel.Size = new System.Drawing.Size(280, 224);
            this.ucPaymentHistoryPanel.TabIndex = 2;
            this.ucPaymentHistoryPanel.Load += new System.EventHandler(this.ucPaymentHistoryPanel_Load);
            // 
            // UCThanhToan
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.pnlCenter);
            this.Controls.Add(this.pnlRight);
            this.Controls.Add(this.pnlLeft);
            this.Name = "UCThanhToan";
            this.Size = new System.Drawing.Size(1160, 820);
            this.pnlLeft.ResumeLayout(false);
            this.pnlLeft.PerformLayout();
            this.pnlCenter.ResumeLayout(false);
            this.pnlCenter.PerformLayout();
            this.pnlTotals.ResumeLayout(false);
            this.pnlTotals.PerformLayout();
            this.pnlRight.ResumeLayout(false);
            this.pnlRight.PerformLayout();
            this.pnlMockInvoice.ResumeLayout(false);
            this.pnlMockInvoice.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
