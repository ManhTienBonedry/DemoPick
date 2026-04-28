using System.Drawing;
using System.Windows.Forms;
using Sunny.UI;
using System.Windows.Forms.DataVisualization.Charting;

namespace DemoPick
{
    partial class UCKhoHang
    {
        private System.ComponentModel.IContainer components = null;

        // Top 4 Cards
        public UIPanel pnlCard1;
        public PictureBox pic1;
        public Label lblC1Title;
        public Label lblC1Value;
        public Label lblC1Badge;

        public UIPanel pnlCard2;
        public PictureBox pic2;
        public Label lblC2Title;
        public Label lblC2Value;
        public Label lblC2Badge;

        public UIPanel pnlCard3;
        public PictureBox pic3;
        public Label lblC3Title;
        public Label lblC3Value;
        public Label lblC3Badge;

        public UIPanel pnlCard4;
        public PictureBox pic4;
        public Label lblC4Title;
        public Label lblC4Value;
        public Label lblC4Badge;

        // Middle Section (List)
        public UIPanel pnlList;
        public Label lblListTitle;
        public UIButton btnXoaSP;
        public UIButton btnThemSP;
        public ListView lstKhoHang;

        // Bottom Section
        public UIPanel pnlBotLeft;
        public Label lblBotLeftTitle;
        public ListView lstGiaoDich;

        public UIPanel pnlBotRight;
        public Label lblBotRightTitle;
        public Chart chartDuBao;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.pnlCard1 = new Sunny.UI.UIPanel();
            this.lblC1Badge = new System.Windows.Forms.Label();
            this.lblC1Title = new System.Windows.Forms.Label();
            this.lblC1Value = new System.Windows.Forms.Label();
            this.pnlCard2 = new Sunny.UI.UIPanel();
            this.lblC2Badge = new System.Windows.Forms.Label();
            this.lblC2Title = new System.Windows.Forms.Label();
            this.lblC2Value = new System.Windows.Forms.Label();
            this.pnlCard3 = new Sunny.UI.UIPanel();
            this.lblC3Badge = new System.Windows.Forms.Label();
            this.lblC3Title = new System.Windows.Forms.Label();
            this.lblC3Value = new System.Windows.Forms.Label();
            this.pnlCard4 = new Sunny.UI.UIPanel();
            this.lblC4Badge = new System.Windows.Forms.Label();
            this.lblC4Title = new System.Windows.Forms.Label();
            this.lblC4Value = new System.Windows.Forms.Label();
            this.pnlList = new Sunny.UI.UIPanel();
            this.lblListTitle = new System.Windows.Forms.Label();
            this.btnXoaSP = new Sunny.UI.UIButton();
            this.btnThemSP = new Sunny.UI.UIButton();
            this.lstKhoHang = new System.Windows.Forms.ListView();
            this.pnlBotLeft = new Sunny.UI.UIPanel();
            this.lblBotLeftTitle = new System.Windows.Forms.Label();
            this.lstGiaoDich = new System.Windows.Forms.ListView();
            this.pnlBotRight = new Sunny.UI.UIPanel();
            this.lblBotRightTitle = new System.Windows.Forms.Label();
            this.chartDuBao = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.pic1 = new System.Windows.Forms.PictureBox();
            this.pic2 = new System.Windows.Forms.PictureBox();
            this.pic3 = new System.Windows.Forms.PictureBox();
            this.pic4 = new System.Windows.Forms.PictureBox();
            this.pnlCard1.SuspendLayout();
            this.pnlCard2.SuspendLayout();
            this.pnlCard3.SuspendLayout();
            this.pnlCard4.SuspendLayout();
            this.pnlList.SuspendLayout();
            this.pnlBotLeft.SuspendLayout();
            this.pnlBotRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartDuBao)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic4)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlCard1
            // 
            this.pnlCard1.Controls.Add(this.pic1);
            this.pnlCard1.Controls.Add(this.lblC1Badge);
            this.pnlCard1.Controls.Add(this.lblC1Title);
            this.pnlCard1.Controls.Add(this.lblC1Value);
            this.pnlCard1.FillColor = System.Drawing.Color.White;
            this.pnlCard1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlCard1.Location = new System.Drawing.Point(20, 20);
            this.pnlCard1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlCard1.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlCard1.Name = "pnlCard1";
            this.pnlCard1.Radius = 16;
            this.pnlCard1.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.pnlCard1.Size = new System.Drawing.Size(265, 120);
            this.pnlCard1.TabIndex = 0;
            this.pnlCard1.Text = null;
            this.pnlCard1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblC1Badge
            // 
            this.lblC1Badge.AutoSize = false;
            this.lblC1Badge.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblC1Badge.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblC1Badge.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.lblC1Badge.Location = new System.Drawing.Point(115, 20);
            this.lblC1Badge.Name = "lblC1Badge";
            this.lblC1Badge.Size = new System.Drawing.Size(130, 20);
            this.lblC1Badge.TabIndex = 1;
            // 
            // lblC1Title
            // 
            this.lblC1Title.AutoSize = true;
            this.lblC1Title.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblC1Title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblC1Title.Location = new System.Drawing.Point(15, 65);
            this.lblC1Title.Name = "lblC1Title";
            this.lblC1Title.Size = new System.Drawing.Size(119, 21);
            this.lblC1Title.TabIndex = 2;
            this.lblC1Title.Text = "Tổng giá trị kho";
            // 
            // lblC1Value
            // 
            this.lblC1Value.AutoSize = true;
            this.lblC1Value.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblC1Value.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblC1Value.Location = new System.Drawing.Point(10, 85);
            this.lblC1Value.Name = "lblC1Value";
            this.lblC1Value.Size = new System.Drawing.Size(57, 37);
            this.lblC1Value.TabIndex = 3;
            this.lblC1Value.Text = "0 đ";
            // 
            // pnlCard2
            // 
            this.pnlCard2.Controls.Add(this.pic2);
            this.pnlCard2.Controls.Add(this.lblC2Badge);
            this.pnlCard2.Controls.Add(this.lblC2Title);
            this.pnlCard2.Controls.Add(this.lblC2Value);
            this.pnlCard2.FillColor = System.Drawing.Color.White;
            this.pnlCard2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlCard2.Location = new System.Drawing.Point(305, 20);
            this.pnlCard2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlCard2.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlCard2.Name = "pnlCard2";
            this.pnlCard2.Radius = 16;
            this.pnlCard2.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.pnlCard2.Size = new System.Drawing.Size(265, 120);
            this.pnlCard2.TabIndex = 1;
            this.pnlCard2.Text = null;
            this.pnlCard2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblC2Badge
            // 
            this.lblC2Badge.AutoSize = false;
            this.lblC2Badge.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblC2Badge.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblC2Badge.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(108)))), ((int)(((byte)(108)))));
            this.lblC2Badge.Location = new System.Drawing.Point(115, 20);
            this.lblC2Badge.Name = "lblC2Badge";
            this.lblC2Badge.Size = new System.Drawing.Size(130, 20);
            this.lblC2Badge.TabIndex = 1;
            // 
            // lblC2Title
            // 
            this.lblC2Title.AutoSize = true;
            this.lblC2Title.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblC2Title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblC2Title.Location = new System.Drawing.Point(15, 65);
            this.lblC2Title.Name = "lblC2Title";
            this.lblC2Title.Size = new System.Drawing.Size(141, 21);
            this.lblC2Title.TabIndex = 2;
            this.lblC2Title.Text = "Cảnh báo hết hàng";
            // 
            // lblC2Value
            // 
            this.lblC2Value.AutoSize = true;
            this.lblC2Value.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblC2Value.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblC2Value.Location = new System.Drawing.Point(10, 85);
            this.lblC2Value.Name = "lblC2Value";
            this.lblC2Value.Size = new System.Drawing.Size(166, 37);
            this.lblC2Value.TabIndex = 3;
            this.lblC2Value.Text = "0 Sản phẩm";
            // 
            // pnlCard3
            // 
            this.pnlCard3.Controls.Add(this.pic3);
            this.pnlCard3.Controls.Add(this.lblC3Badge);
            this.pnlCard3.Controls.Add(this.lblC3Title);
            this.pnlCard3.Controls.Add(this.lblC3Value);
            this.pnlCard3.FillColor = System.Drawing.Color.White;
            this.pnlCard3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlCard3.Location = new System.Drawing.Point(590, 20);
            this.pnlCard3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlCard3.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlCard3.Name = "pnlCard3";
            this.pnlCard3.Radius = 16;
            this.pnlCard3.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.pnlCard3.Size = new System.Drawing.Size(265, 120);
            this.pnlCard3.TabIndex = 2;
            this.pnlCard3.Text = null;
            this.pnlCard3.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblC3Badge
            // 
            this.lblC3Badge.AutoSize = false;
            this.lblC3Badge.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblC3Badge.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblC3Badge.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblC3Badge.Location = new System.Drawing.Point(115, 20);
            this.lblC3Badge.Name = "lblC3Badge";
            this.lblC3Badge.Size = new System.Drawing.Size(130, 20);
            this.lblC3Badge.TabIndex = 1;
            // 
            // lblC3Title
            // 
            this.lblC3Title.AutoSize = true;
            this.lblC3Title.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblC3Title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblC3Title.Location = new System.Drawing.Point(15, 65);
            this.lblC3Title.Name = "lblC3Title";
            this.lblC3Title.Size = new System.Drawing.Size(131, 21);
            this.lblC3Title.TabIndex = 2;
            this.lblC3Title.Text = "Sản phẩm đã bán";
            // 
            // lblC3Value
            // 
            this.lblC3Value.AutoSize = true;
            this.lblC3Value.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblC3Value.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblC3Value.Location = new System.Drawing.Point(10, 85);
            this.lblC3Value.Name = "lblC3Value";
            this.lblC3Value.Size = new System.Drawing.Size(160, 37);
            this.lblC3Value.TabIndex = 3;
            this.lblC3Value.Text = "0 Giao dịch";
            // 
            // pnlCard4
            // 
            this.pnlCard4.Controls.Add(this.pic4);
            this.pnlCard4.Controls.Add(this.lblC4Badge);
            this.pnlCard4.Controls.Add(this.lblC4Title);
            this.pnlCard4.Controls.Add(this.lblC4Value);
            this.pnlCard4.FillColor = System.Drawing.Color.White;
            this.pnlCard4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlCard4.Location = new System.Drawing.Point(875, 20);
            this.pnlCard4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlCard4.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlCard4.Name = "pnlCard4";
            this.pnlCard4.Radius = 16;
            this.pnlCard4.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.pnlCard4.Size = new System.Drawing.Size(265, 120);
            this.pnlCard4.TabIndex = 3;
            this.pnlCard4.Text = null;
            this.pnlCard4.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblC4Badge
            // 
            this.lblC4Badge.AutoSize = false;
            this.lblC4Badge.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblC4Badge.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblC4Badge.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblC4Badge.Location = new System.Drawing.Point(115, 20);
            this.lblC4Badge.Name = "lblC4Badge";
            this.lblC4Badge.Size = new System.Drawing.Size(130, 20);
            this.lblC4Badge.TabIndex = 1;
            // 
            // lblC4Title
            // 
            this.lblC4Title.AutoSize = true;
            this.lblC4Title.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblC4Title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblC4Title.Location = new System.Drawing.Point(15, 65);
            this.lblC4Title.Name = "lblC4Title";
            this.lblC4Title.Size = new System.Drawing.Size(103, 21);
            this.lblC4Title.TabIndex = 2;
            this.lblC4Title.Text = "Hóa đơn xuất";
            // 
            // lblC4Value
            // 
            this.lblC4Value.AutoSize = true;
            this.lblC4Value.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblC4Value.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblC4Value.Location = new System.Drawing.Point(10, 85);
            this.lblC4Value.Name = "lblC4Value";
            this.lblC4Value.Size = new System.Drawing.Size(93, 37);
            this.lblC4Value.TabIndex = 3;
            this.lblC4Value.Text = "0 Đơn";
            // 
            // pnlList
            // 
            this.pnlList.Controls.Add(this.lblListTitle);
            this.pnlList.Controls.Add(this.btnXoaSP);
            this.pnlList.Controls.Add(this.btnThemSP);
            this.pnlList.Controls.Add(this.lstKhoHang);
            this.pnlList.FillColor = System.Drawing.Color.White;
            this.pnlList.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlList.Location = new System.Drawing.Point(20, 160);
            this.pnlList.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlList.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlList.Name = "pnlList";
            this.pnlList.Radius = 16;
            this.pnlList.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.pnlList.Size = new System.Drawing.Size(1120, 260);
            this.pnlList.TabIndex = 4;
            this.pnlList.Text = null;
            this.pnlList.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblListTitle
            // 
            this.lblListTitle.AutoSize = true;
            this.lblListTitle.BackColor = System.Drawing.Color.White;
            this.lblListTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblListTitle.Location = new System.Drawing.Point(20, 20);
            this.lblListTitle.Name = "lblListTitle";
            this.lblListTitle.Size = new System.Drawing.Size(189, 28);
            this.lblListTitle.TabIndex = 0;
            this.lblListTitle.Text = "Danh sách tồn kho";
            // 
            // btnXoaSP
            // 
            this.btnXoaSP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnXoaSP.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnXoaSP.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnXoaSP.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(86)))), ((int)(((byte)(70)))));
            this.btnXoaSP.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnXoaSP.Location = new System.Drawing.Point(690, 15);
            this.btnXoaSP.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnXoaSP.Name = "btnXoaSP";
            this.btnXoaSP.Radius = 14;
            this.btnXoaSP.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnXoaSP.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(86)))), ((int)(((byte)(70)))));
            this.btnXoaSP.Size = new System.Drawing.Size(200, 35);
            this.btnXoaSP.TabIndex = 2;
            this.btnXoaSP.Text = "[X] XÓA SẢN PHẨM";
            this.btnXoaSP.TipsFont = new System.Drawing.Font("Segoe UI", 9F);
            // 
            // btnThemSP
            // 
            this.btnThemSP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnThemSP.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnThemSP.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnThemSP.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(86)))), ((int)(((byte)(185)))), ((int)(((byte)(90)))));
            this.btnThemSP.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnThemSP.Location = new System.Drawing.Point(900, 15);
            this.btnThemSP.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnThemSP.Name = "btnThemSP";
            this.btnThemSP.Radius = 14;
            this.btnThemSP.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnThemSP.Size = new System.Drawing.Size(200, 35);
            this.btnThemSP.TabIndex = 0;
            this.btnThemSP.Text = "[+] NHẬP HÀNG HÓA";
            this.btnThemSP.TipsFont = new System.Drawing.Font("Segoe UI", 9F);
            // 
            // lstKhoHang
            // 
            this.lstKhoHang.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstKhoHang.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lstKhoHang.FullRowSelect = true;
            this.lstKhoHang.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstKhoHang.HideSelection = false;
            this.lstKhoHang.Location = new System.Drawing.Point(20, 60);
            this.lstKhoHang.Name = "lstKhoHang";
            this.lstKhoHang.Size = new System.Drawing.Size(1080, 180);
            this.lstKhoHang.TabIndex = 1;
            this.lstKhoHang.UseCompatibleStateImageBehavior = false;
            this.lstKhoHang.View = System.Windows.Forms.View.Details;
            // 
            // pnlBotLeft
            // 
            this.pnlBotLeft.Controls.Add(this.lblBotLeftTitle);
            this.pnlBotLeft.Controls.Add(this.lstGiaoDich);
            this.pnlBotLeft.FillColor = System.Drawing.Color.White;
            this.pnlBotLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlBotLeft.Location = new System.Drawing.Point(20, 440);
            this.pnlBotLeft.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlBotLeft.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlBotLeft.Name = "pnlBotLeft";
            this.pnlBotLeft.Radius = 16;
            this.pnlBotLeft.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.pnlBotLeft.Size = new System.Drawing.Size(540, 260);
            this.pnlBotLeft.TabIndex = 5;
            this.pnlBotLeft.Text = null;
            this.pnlBotLeft.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBotLeftTitle
            // 
            this.lblBotLeftTitle.AutoSize = true;
            this.lblBotLeftTitle.BackColor = System.Drawing.Color.White;
            this.lblBotLeftTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblBotLeftTitle.Location = new System.Drawing.Point(20, 20);
            this.lblBotLeftTitle.Name = "lblBotLeftTitle";
            this.lblBotLeftTitle.Size = new System.Drawing.Size(183, 28);
            this.lblBotLeftTitle.TabIndex = 0;
            this.lblBotLeftTitle.Text = "Giao dịch gần đây";
            // 
            // lstGiaoDich
            // 
            this.lstGiaoDich.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstGiaoDich.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lstGiaoDich.FullRowSelect = true;
            this.lstGiaoDich.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstGiaoDich.HideSelection = false;
            this.lstGiaoDich.Location = new System.Drawing.Point(20, 60);
            this.lstGiaoDich.Name = "lstGiaoDich";
            this.lstGiaoDich.Size = new System.Drawing.Size(500, 180);
            this.lstGiaoDich.TabIndex = 1;
            this.lstGiaoDich.UseCompatibleStateImageBehavior = false;
            this.lstGiaoDich.View = System.Windows.Forms.View.Details;
            // 
            // pnlBotRight
            // 
            this.pnlBotRight.Controls.Add(this.lblBotRightTitle);
            this.pnlBotRight.Controls.Add(this.chartDuBao);
            this.pnlBotRight.FillColor = System.Drawing.Color.White;
            this.pnlBotRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlBotRight.Location = new System.Drawing.Point(580, 440);
            this.pnlBotRight.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlBotRight.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlBotRight.Name = "pnlBotRight";
            this.pnlBotRight.Radius = 16;
            this.pnlBotRight.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.pnlBotRight.Size = new System.Drawing.Size(560, 260);
            this.pnlBotRight.TabIndex = 6;
            this.pnlBotRight.Text = null;
            this.pnlBotRight.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBotRightTitle
            // 
            this.lblBotRightTitle.AutoSize = true;
            this.lblBotRightTitle.BackColor = System.Drawing.Color.White;
            this.lblBotRightTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblBotRightTitle.Location = new System.Drawing.Point(20, 20);
            this.lblBotRightTitle.Name = "lblBotRightTitle";
            this.lblBotRightTitle.Size = new System.Drawing.Size(160, 28);
            this.lblBotRightTitle.TabIndex = 0;
            this.lblBotRightTitle.Text = "Dự báo tồn kho";
            // 
            // chartDuBao
            // 
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            chartArea1.Name = "MainArea";
            this.chartDuBao.ChartAreas.Add(chartArea1);
            this.chartDuBao.Location = new System.Drawing.Point(20, 60);
            this.chartDuBao.Name = "chartDuBao";
            series1.ChartArea = "MainArea";
            series1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            series1.Name = "Forecast";
            this.chartDuBao.Series.Add(series1);
            this.chartDuBao.Size = new System.Drawing.Size(520, 180);
            this.chartDuBao.TabIndex = 1;
            // 
            // pic1
            // 
            this.pic1.BackColor = System.Drawing.Color.White;
            this.pic1.Image = global::DemoPick.Properties.Resources.kho_4;
            this.pic1.Location = new System.Drawing.Point(15, 15);
            this.pic1.Name = "pic1";
            this.pic1.Size = new System.Drawing.Size(40, 40);
            this.pic1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pic1.TabIndex = 0;
            this.pic1.TabStop = false;
            // 
            // pic2
            // 
            this.pic2.BackColor = System.Drawing.Color.White;
            this.pic2.Image = global::DemoPick.Properties.Resources.hết_hàng;
            this.pic2.Location = new System.Drawing.Point(15, 15);
            this.pic2.Name = "pic2";
            this.pic2.Size = new System.Drawing.Size(40, 40);
            this.pic2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pic2.TabIndex = 0;
            this.pic2.TabStop = false;
            // 
            // pic3
            // 
            this.pic3.BackColor = System.Drawing.Color.White;
            this.pic3.Image = global::DemoPick.Properties.Resources.Kho_1;
            this.pic3.Location = new System.Drawing.Point(15, 15);
            this.pic3.Name = "pic3";
            this.pic3.Size = new System.Drawing.Size(40, 40);
            this.pic3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pic3.TabIndex = 0;
            this.pic3.TabStop = false;
            // 
            // pic4
            // 
            this.pic4.BackColor = System.Drawing.Color.White;
            this.pic4.Image = global::DemoPick.Properties.Resources.Kho_2;
            this.pic4.Location = new System.Drawing.Point(15, 15);
            this.pic4.Name = "pic4";
            this.pic4.Size = new System.Drawing.Size(40, 40);
            this.pic4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pic4.TabIndex = 0;
            this.pic4.TabStop = false;
            // 
            // UCKhoHang
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.pnlCard1);
            this.Controls.Add(this.pnlCard2);
            this.Controls.Add(this.pnlCard3);
            this.Controls.Add(this.pnlCard4);
            this.Controls.Add(this.pnlList);
            this.Controls.Add(this.pnlBotLeft);
            this.Controls.Add(this.pnlBotRight);
            this.Name = "UCKhoHang";
            this.Size = new System.Drawing.Size(1160, 820);
            this.pnlCard1.ResumeLayout(false);
            this.pnlCard1.PerformLayout();
            this.pnlCard2.ResumeLayout(false);
            this.pnlCard2.PerformLayout();
            this.pnlCard3.ResumeLayout(false);
            this.pnlCard3.PerformLayout();
            this.pnlCard4.ResumeLayout(false);
            this.pnlCard4.PerformLayout();
            this.pnlList.ResumeLayout(false);
            this.pnlList.PerformLayout();
            this.pnlBotLeft.ResumeLayout(false);
            this.pnlBotLeft.PerformLayout();
            this.pnlBotRight.ResumeLayout(false);
            this.pnlBotRight.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartDuBao)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic4)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
