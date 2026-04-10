using System.Drawing;
using System.Windows.Forms;
using Panel = System.Windows.Forms.Panel;
using Sunny.UI;

namespace DemoPick
{
    partial class FrmChinh
    {
        private System.ComponentModel.IContainer components = null;
        public Panel pnlSidebar;
        public Panel pnlHeader;
        public Panel pnlContent;
        
        public Sunny.UI.UIPanel btnNavDashboard;
        public Sunny.UI.UIPanel btnNavDatLich;
        public Sunny.UI.UIPanel btnNavBanHang;
        public Sunny.UI.UIPanel btnNavKhachHang;
        public Sunny.UI.UIPanel btnNavKhoHang;
        public Sunny.UI.UIPanel btnNavBaoCao;
        public Sunny.UI.UIPanel btnNavThanhToan;

        private Label lblNav1;
        private Label lblNav2;
        private Label lblNav3;
        private Label lblNav4;
        private Label lblNav5;
        private Label lblNav6;
        private Label lblNav7;

        private Panel pnlAdmin;
        private Panel pnlAdminAvatar;
        private Label lblAdminAvatarText;
        private Label lblAdminName;
        private Label lblAdminStatus;

        public Panel pnlLogo;
        public System.Windows.Forms.PictureBox Piccachu;
        public Label lblLogo1;
        public Label lblLogo2;
        public Label lblPageTitle;
        public Label lblPageSubtitle;
        public UITextBox txtSearch;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmChinh));
            this.pnlSidebar = new System.Windows.Forms.Panel();
            this.pnlLogo = new System.Windows.Forms.Panel();
            this.Piccachu = new System.Windows.Forms.PictureBox();
            this.lblLogo1 = new System.Windows.Forms.Label();
            this.lblLogo2 = new System.Windows.Forms.Label();
            this.btnNavDashboard = new Sunny.UI.UIPanel();
            this.lblNav1 = new System.Windows.Forms.Label();
            this.btnNavDatLich = new Sunny.UI.UIPanel();
            this.lblNav2 = new System.Windows.Forms.Label();
            this.btnNavBanHang = new Sunny.UI.UIPanel();
            this.lblNav3 = new System.Windows.Forms.Label();
            this.btnNavKhachHang = new Sunny.UI.UIPanel();
            this.lblNav4 = new System.Windows.Forms.Label();
            this.btnNavKhoHang = new Sunny.UI.UIPanel();
            this.lblNav5 = new System.Windows.Forms.Label();
            this.btnNavBaoCao = new Sunny.UI.UIPanel();
            this.lblNav6 = new System.Windows.Forms.Label();
            this.btnNavThanhToan = new Sunny.UI.UIPanel();
            this.lblNav7 = new System.Windows.Forms.Label();
            this.pnlAdmin = new System.Windows.Forms.Panel();
            this.pnlAdminAvatar = new System.Windows.Forms.Panel();
            this.lblAdminAvatarText = new System.Windows.Forms.Label();
            this.lblAdminName = new System.Windows.Forms.Label();
            this.lblAdminStatus = new System.Windows.Forms.Label();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblPageTitle = new System.Windows.Forms.Label();
            this.lblPageSubtitle = new System.Windows.Forms.Label();
            this.txtSearch = new Sunny.UI.UITextBox();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.pnlSidebar.SuspendLayout();
            this.pnlLogo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Piccachu)).BeginInit();
            this.btnNavDashboard.SuspendLayout();
            this.btnNavDatLich.SuspendLayout();
            this.btnNavBanHang.SuspendLayout();
            this.btnNavKhachHang.SuspendLayout();
            this.btnNavKhoHang.SuspendLayout();
            this.btnNavBaoCao.SuspendLayout();
            this.btnNavThanhToan.SuspendLayout();
            this.pnlAdmin.SuspendLayout();
            this.pnlAdminAvatar.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSidebar
            // 
            this.pnlSidebar.BackColor = System.Drawing.Color.White;
            this.pnlSidebar.Controls.Add(this.pnlLogo);
            this.pnlSidebar.Controls.Add(this.btnNavDashboard);
            this.pnlSidebar.Controls.Add(this.btnNavDatLich);
            this.pnlSidebar.Controls.Add(this.btnNavBanHang);
            this.pnlSidebar.Controls.Add(this.btnNavKhachHang);
            this.pnlSidebar.Controls.Add(this.btnNavKhoHang);
            this.pnlSidebar.Controls.Add(this.btnNavBaoCao);
            this.pnlSidebar.Controls.Add(this.btnNavThanhToan);
            this.pnlSidebar.Controls.Add(this.pnlAdmin);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.Location = new System.Drawing.Point(0, 0);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Size = new System.Drawing.Size(240, 900);
            this.pnlSidebar.TabIndex = 2;
            // 
            // pnlLogo
            // 
            this.pnlLogo.BackColor = System.Drawing.Color.White;
            this.pnlLogo.Controls.Add(this.Piccachu);
            this.pnlLogo.Controls.Add(this.lblLogo1);
            this.pnlLogo.Controls.Add(this.lblLogo2);
            this.pnlLogo.Location = new System.Drawing.Point(0, 0);
            this.pnlLogo.Name = "pnlLogo";
            this.pnlLogo.Size = new System.Drawing.Size(240, 90);
            this.pnlLogo.TabIndex = 0;
            // 
            // Piccachu
            // 
            this.Piccachu.BackColor = System.Drawing.Color.Transparent;
            this.Piccachu.Image = ((System.Drawing.Image)(resources.GetObject("Piccachu.Image")));
            this.Piccachu.Location = new System.Drawing.Point(20, 25);
            this.Piccachu.Name = "Piccachu";
            this.Piccachu.Size = new System.Drawing.Size(40, 40);
            this.Piccachu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Piccachu.TabIndex = 0;
            this.Piccachu.TabStop = false;
            // 
            // lblLogo1
            // 
            this.lblLogo1.AutoSize = true;
            this.lblLogo1.Font = new System.Drawing.Font("Segoe UI", 13.5F, System.Drawing.FontStyle.Bold);
            this.lblLogo1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblLogo1.Location = new System.Drawing.Point(62, 17);
            this.lblLogo1.Name = "lblLogo1";
            this.lblLogo1.Size = new System.Drawing.Size(112, 31);
            this.lblLogo1.TabIndex = 1;
            this.lblLogo1.Text = "CourtPro";
            // 
            // lblLogo2
            // 
            this.lblLogo2.AutoSize = true;
            this.lblLogo2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLogo2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblLogo2.Location = new System.Drawing.Point(67, 47);
            this.lblLogo2.Name = "lblLogo2";
            this.lblLogo2.Size = new System.Drawing.Size(97, 20);
            this.lblLogo2.TabIndex = 2;
            this.lblLogo2.Text = "Management";
            // 
            // btnNavDashboard
            // 
            this.btnNavDashboard.BackColor = System.Drawing.Color.White;
            this.btnNavDashboard.Controls.Add(this.lblNav1);
            this.btnNavDashboard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavDashboard.FillColor = System.Drawing.Color.White;
            this.btnNavDashboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnNavDashboard.Location = new System.Drawing.Point(15, 110);
            this.btnNavDashboard.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnNavDashboard.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnNavDashboard.Name = "btnNavDashboard";
            this.btnNavDashboard.Radius = 18;
            this.btnNavDashboard.RectColor = System.Drawing.Color.Transparent;
            this.btnNavDashboard.RectDisableColor = System.Drawing.Color.Transparent;
            this.btnNavDashboard.Size = new System.Drawing.Size(210, 48);
            this.btnNavDashboard.TabIndex = 1;
            this.btnNavDashboard.Text = null;
            this.btnNavDashboard.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNav1
            // 
            this.lblNav1.AutoSize = true;
            this.lblNav1.Font = new System.Drawing.Font("Segoe UI", 10.8F);
            this.lblNav1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblNav1.Location = new System.Drawing.Point(40, 11);
            this.lblNav1.Name = "lblNav1";
            this.lblNav1.Size = new System.Drawing.Size(98, 25);
            this.lblNav1.TabIndex = 0;
            this.lblNav1.Text = "Tổng quan";
            // 
            // btnNavDatLich
            // 
            this.btnNavDatLich.BackColor = System.Drawing.Color.White;
            this.btnNavDatLich.Controls.Add(this.lblNav2);
            this.btnNavDatLich.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavDatLich.FillColor = System.Drawing.Color.White;
            this.btnNavDatLich.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnNavDatLich.Location = new System.Drawing.Point(15, 165);
            this.btnNavDatLich.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnNavDatLich.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnNavDatLich.Name = "btnNavDatLich";
            this.btnNavDatLich.Radius = 18;
            this.btnNavDatLich.RectColor = System.Drawing.Color.Transparent;
            this.btnNavDatLich.RectDisableColor = System.Drawing.Color.Transparent;
            this.btnNavDatLich.Size = new System.Drawing.Size(210, 48);
            this.btnNavDatLich.TabIndex = 2;
            this.btnNavDatLich.Text = null;
            this.btnNavDatLich.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNav2
            // 
            this.lblNav2.AutoSize = true;
            this.lblNav2.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblNav2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblNav2.Location = new System.Drawing.Point(40, 11);
            this.lblNav2.Name = "lblNav2";
            this.lblNav2.Size = new System.Drawing.Size(149, 25);
            this.lblNav2.TabIndex = 0;
            this.lblNav2.Text = "Sơ đồ && Đặt lịch";
            // 
            // btnNavBanHang
            // 
            this.btnNavBanHang.BackColor = System.Drawing.Color.White;
            this.btnNavBanHang.Controls.Add(this.lblNav3);
            this.btnNavBanHang.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavBanHang.FillColor = System.Drawing.Color.White;
            this.btnNavBanHang.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnNavBanHang.Location = new System.Drawing.Point(15, 220);
            this.btnNavBanHang.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnNavBanHang.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnNavBanHang.Name = "btnNavBanHang";
            this.btnNavBanHang.Radius = 18;
            this.btnNavBanHang.RectColor = System.Drawing.Color.Transparent;
            this.btnNavBanHang.RectDisableColor = System.Drawing.Color.Transparent;
            this.btnNavBanHang.Size = new System.Drawing.Size(210, 48);
            this.btnNavBanHang.TabIndex = 3;
            this.btnNavBanHang.Text = null;
            this.btnNavBanHang.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNav3
            // 
            this.lblNav3.AutoSize = true;
            this.lblNav3.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblNav3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblNav3.Location = new System.Drawing.Point(40, 11);
            this.lblNav3.Name = "lblNav3";
            this.lblNav3.Size = new System.Drawing.Size(92, 25);
            this.lblNav3.TabIndex = 0;
            this.lblNav3.Text = "Bán hàng";
            // 
            // btnNavKhachHang
            // 
            this.btnNavKhachHang.BackColor = System.Drawing.Color.White;
            this.btnNavKhachHang.Controls.Add(this.lblNav4);
            this.btnNavKhachHang.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavKhachHang.FillColor = System.Drawing.Color.White;
            this.btnNavKhachHang.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnNavKhachHang.Location = new System.Drawing.Point(15, 275);
            this.btnNavKhachHang.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnNavKhachHang.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnNavKhachHang.Name = "btnNavKhachHang";
            this.btnNavKhachHang.Radius = 18;
            this.btnNavKhachHang.RectColor = System.Drawing.Color.Transparent;
            this.btnNavKhachHang.RectDisableColor = System.Drawing.Color.Transparent;
            this.btnNavKhachHang.Size = new System.Drawing.Size(210, 48);
            this.btnNavKhachHang.TabIndex = 4;
            this.btnNavKhachHang.Text = null;
            this.btnNavKhachHang.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNav4
            // 
            this.lblNav4.AutoSize = true;
            this.lblNav4.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblNav4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblNav4.Location = new System.Drawing.Point(40, 11);
            this.lblNav4.Name = "lblNav4";
            this.lblNav4.Size = new System.Drawing.Size(112, 25);
            this.lblNav4.TabIndex = 0;
            this.lblNav4.Text = "Khách hàng";
            // 
            // btnNavKhoHang
            // 
            this.btnNavKhoHang.BackColor = System.Drawing.Color.White;
            this.btnNavKhoHang.Controls.Add(this.lblNav5);
            this.btnNavKhoHang.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavKhoHang.FillColor = System.Drawing.Color.White;
            this.btnNavKhoHang.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnNavKhoHang.Location = new System.Drawing.Point(15, 330);
            this.btnNavKhoHang.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnNavKhoHang.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnNavKhoHang.Name = "btnNavKhoHang";
            this.btnNavKhoHang.Radius = 18;
            this.btnNavKhoHang.RectColor = System.Drawing.Color.Transparent;
            this.btnNavKhoHang.RectDisableColor = System.Drawing.Color.Transparent;
            this.btnNavKhoHang.Size = new System.Drawing.Size(210, 48);
            this.btnNavKhoHang.TabIndex = 5;
            this.btnNavKhoHang.Text = null;
            this.btnNavKhoHang.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNav5
            // 
            this.lblNav5.AutoSize = true;
            this.lblNav5.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblNav5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblNav5.Location = new System.Drawing.Point(40, 11);
            this.lblNav5.Name = "lblNav5";
            this.lblNav5.Size = new System.Drawing.Size(93, 25);
            this.lblNav5.TabIndex = 0;
            this.lblNav5.Text = "Kho hàng";
            // 
            // btnNavBaoCao
            // 
            this.btnNavBaoCao.BackColor = System.Drawing.Color.White;
            this.btnNavBaoCao.Controls.Add(this.lblNav6);
            this.btnNavBaoCao.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavBaoCao.FillColor = System.Drawing.Color.White;
            this.btnNavBaoCao.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnNavBaoCao.Location = new System.Drawing.Point(15, 385);
            this.btnNavBaoCao.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnNavBaoCao.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnNavBaoCao.Name = "btnNavBaoCao";
            this.btnNavBaoCao.Radius = 18;
            this.btnNavBaoCao.RectColor = System.Drawing.Color.Transparent;
            this.btnNavBaoCao.RectDisableColor = System.Drawing.Color.Transparent;
            this.btnNavBaoCao.Size = new System.Drawing.Size(210, 48);
            this.btnNavBaoCao.TabIndex = 6;
            this.btnNavBaoCao.Text = null;
            this.btnNavBaoCao.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNav6
            // 
            this.lblNav6.AutoSize = true;
            this.lblNav6.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblNav6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblNav6.Location = new System.Drawing.Point(40, 11);
            this.lblNav6.Name = "lblNav6";
            this.lblNav6.Size = new System.Drawing.Size(79, 25);
            this.lblNav6.TabIndex = 0;
            this.lblNav6.Text = "Báo cáo";
            // 
            // btnNavThanhToan
            // 
            this.btnNavThanhToan.BackColor = System.Drawing.Color.White;
            this.btnNavThanhToan.Controls.Add(this.lblNav7);
            this.btnNavThanhToan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavThanhToan.FillColor = System.Drawing.Color.White;
            this.btnNavThanhToan.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnNavThanhToan.Location = new System.Drawing.Point(15, 440);
            this.btnNavThanhToan.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnNavThanhToan.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnNavThanhToan.Name = "btnNavThanhToan";
            this.btnNavThanhToan.Radius = 18;
            this.btnNavThanhToan.RectColor = System.Drawing.Color.Transparent;
            this.btnNavThanhToan.RectDisableColor = System.Drawing.Color.Transparent;
            this.btnNavThanhToan.Size = new System.Drawing.Size(210, 48);
            this.btnNavThanhToan.TabIndex = 7;
            this.btnNavThanhToan.Text = null;
            this.btnNavThanhToan.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNav7
            // 
            this.lblNav7.AutoSize = true;
            this.lblNav7.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblNav7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblNav7.Location = new System.Drawing.Point(40, 11);
            this.lblNav7.Name = "lblNav7";
            this.lblNav7.Size = new System.Drawing.Size(108, 25);
            this.lblNav7.TabIndex = 0;
            this.lblNav7.Text = "Thanh toán";
            // 
            // pnlAdmin
            // 
            this.pnlAdmin.BackColor = System.Drawing.Color.White;
            this.pnlAdmin.Controls.Add(this.pnlAdminAvatar);
            this.pnlAdmin.Controls.Add(this.lblAdminName);
            this.pnlAdmin.Controls.Add(this.lblAdminStatus);
            this.pnlAdmin.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlAdmin.Location = new System.Drawing.Point(0, 825);
            this.pnlAdmin.Name = "pnlAdmin";
            this.pnlAdmin.Size = new System.Drawing.Size(240, 75);
            this.pnlAdmin.TabIndex = 7;
            // 
            // pnlAdminAvatar
            // 
            this.pnlAdminAvatar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(252)))), ((int)(((byte)(231)))));
            this.pnlAdminAvatar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnlAdminAvatar.BackgroundImage")));
            this.pnlAdminAvatar.Controls.Add(this.lblAdminAvatarText);
            this.pnlAdminAvatar.Location = new System.Drawing.Point(20, 18);
            this.pnlAdminAvatar.Name = "pnlAdminAvatar";
            this.pnlAdminAvatar.Size = new System.Drawing.Size(40, 40);
            this.pnlAdminAvatar.TabIndex = 0;
            this.pnlAdminAvatar.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlAdminAvatar_Paint);
            // 
            // lblAdminAvatarText
            // 
            this.lblAdminAvatarText.AutoSize = true;
            this.lblAdminAvatarText.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblAdminAvatarText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.lblAdminAvatarText.Location = new System.Drawing.Point(12, 9);
            this.lblAdminAvatarText.Name = "lblAdminAvatarText";
            this.lblAdminAvatarText.Size = new System.Drawing.Size(22, 23);
            this.lblAdminAvatarText.TabIndex = 0;
            this.lblAdminAvatarText.Text = "A";
            // 
            // lblAdminName
            // 
            this.lblAdminName.AutoSize = true;
            this.lblAdminName.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblAdminName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblAdminName.Location = new System.Drawing.Point(70, 20);
            this.lblAdminName.Name = "lblAdminName";
            this.lblAdminName.Size = new System.Drawing.Size(64, 23);
            this.lblAdminName.TabIndex = 1;
            this.lblAdminName.Text = "Admin";
            // 
            // lblAdminStatus
            // 
            this.lblAdminStatus.AutoSize = true;
            this.lblAdminStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblAdminStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblAdminStatus.Location = new System.Drawing.Point(70, 44);
            this.lblAdminStatus.Name = "lblAdminStatus";
            this.lblAdminStatus.Size = new System.Drawing.Size(61, 20);
            this.lblAdminStatus.TabIndex = 2;
            this.lblAdminStatus.Text = "Ca sáng";
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.Controls.Add(this.lblPageTitle);
            this.pnlHeader.Controls.Add(this.lblPageSubtitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(240, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1200, 90);
            this.pnlHeader.TabIndex = 1;
            // 
            // lblPageTitle
            // 
            this.lblPageTitle.AutoSize = true;
            this.lblPageTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblPageTitle.Location = new System.Drawing.Point(25, 12);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Size = new System.Drawing.Size(228, 37);
            this.lblPageTitle.TabIndex = 1;
            this.lblPageTitle.Text = "Bảng Điều Khiển";
            // 
            // lblPageSubtitle
            // 
            this.lblPageSubtitle.AutoSize = true;
            this.lblPageSubtitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPageSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblPageSubtitle.Location = new System.Drawing.Point(30, 48);
            this.lblPageSubtitle.Name = "lblPageSubtitle";
            this.lblPageSubtitle.Size = new System.Drawing.Size(178, 23);
            this.lblPageSubtitle.TabIndex = 2;
            this.lblPageSubtitle.Text = "Tổng quan hoạt động";
            // 
            // txtSearch
            // 
            this.txtSearch.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtSearch.Location = new System.Drawing.Point(0, 0);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSearch.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Padding = new System.Windows.Forms.Padding(5);
            this.txtSearch.ShowText = false;
            this.txtSearch.Size = new System.Drawing.Size(150, 29);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtSearch.Watermark = "";
            // 
            // pnlContent
            // 
            this.pnlContent.BackColor = System.Drawing.Color.Transparent;
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(240, 90);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(20);
            this.pnlContent.Size = new System.Drawing.Size(1200, 810);
            this.pnlContent.TabIndex = 0;
            // 
            // FrmChinh
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1440, 900);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlSidebar);
            this.MinimumSize = new System.Drawing.Size(1280, 800);
            this.Name = "FrmChinh";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CourtPro Management";
            this.pnlSidebar.ResumeLayout(false);
            this.pnlLogo.ResumeLayout(false);
            this.pnlLogo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Piccachu)).EndInit();
            this.btnNavDashboard.ResumeLayout(false);
            this.btnNavDashboard.PerformLayout();
            this.btnNavDatLich.ResumeLayout(false);
            this.btnNavDatLich.PerformLayout();
            this.btnNavBanHang.ResumeLayout(false);
            this.btnNavBanHang.PerformLayout();
            this.btnNavKhachHang.ResumeLayout(false);
            this.btnNavKhachHang.PerformLayout();
            this.btnNavKhoHang.ResumeLayout(false);
            this.btnNavKhoHang.PerformLayout();
            this.btnNavBaoCao.ResumeLayout(false);
            this.btnNavBaoCao.PerformLayout();
            this.btnNavThanhToan.ResumeLayout(false);
            this.btnNavThanhToan.PerformLayout();
            this.pnlAdmin.ResumeLayout(false);
            this.pnlAdmin.PerformLayout();
            this.pnlAdminAvatar.ResumeLayout(false);
            this.pnlAdminAvatar.PerformLayout();
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
