using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Panel = System.Windows.Forms.Panel;
using System.Collections.Generic;
using DemoPick.Services;

namespace DemoPick
{
    public partial class FrmChinh : Form
    {
        public UCTongQuan tongQuan;
        public UCDatLich datLich;
        public UCBanHang banHang;
        public UCKhachHang khachHang;
        public UCKhoHang khoHang;
        public UCBaoCao baoCao;
        public UCThanhToan thanhToan;
        
        private List<Sunny.UI.UIPanel> menuButtons;

        public FrmChinh()
        {
            InitializeComponent();

            if (DesignModeUtil.IsDesignMode(this))
            {
                return;
            }

            new DatabaseMaintenanceService().TryHealCorruptedCourtNames();

            InitModules();

            // Consistent inner page background across all modules (better contrast with white cards).
            UiTheme.ApplyPageBackground(this);
            UiTheme.ApplyPageBackground(pnlContent);

            // Ensure the 1px sidebar border isn't covered by full-width child panels (e.g., admin footer).
            // Padding reserves the last pixel column for the border line we draw in Paint.
            if (pnlSidebar != null)
            {
                var p = pnlSidebar.Padding;
                pnlSidebar.Padding = new Padding(p.Left, p.Top, Math.Max(p.Right, 1), p.Bottom);
            }
            if (pnlLogo != null)
            {
                pnlLogo.Dock = DockStyle.Top;
            }

            ApplyCurrentUserUI();
            ApplyRolePermissions();
            
            // Draw clean subtle 1px borders per Design System #e5e7eb
            pnlSidebar.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(229, 231, 235), 1))
                {
                    e.Graphics.DrawLine(pen, pnlSidebar.Width - 1, 0, pnlSidebar.Width - 1, pnlSidebar.Height);
                }
            };
            pnlHeader.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(229, 231, 235), 1))
                {
                    e.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
                }
            };

        }

        private void InitModules()
        {
            tongQuan = new UCTongQuan() { Dock = DockStyle.Fill };
            datLich = new UCDatLich() { Dock = DockStyle.Fill };
            banHang = new UCBanHang() { Dock = DockStyle.Fill };
            khachHang = new UCKhachHang() { Dock = DockStyle.Fill };
            khoHang = new UCKhoHang() { Dock = DockStyle.Fill };
            baoCao = new UCBaoCao() { Dock = DockStyle.Fill };
            thanhToan = new UCThanhToan() { Dock = DockStyle.Fill };

            menuButtons = new List<Sunny.UI.UIPanel> { btnNavDashboard, btnNavDatLich, btnNavBanHang, btnNavKhachHang, btnNavKhoHang, btnNavBaoCao, btnNavThanhToan };

            // Start with Dashboard
            SwitchModule(tongQuan, btnNavDashboard, "Dashboard", "Tổng quan hoạt động");

            BindClick(btnNavDashboard, tongQuan, "Dashboard", "Tổng quan hoạt động");
            BindClick(btnNavDatLich, datLich, "Quản lý Sơ đồ && Đặt lịch", "");
            BindClick(btnNavBanHang, banHang, "Tính tiền && Bán hàng POS", "Chi nhánh: Sân Cầu Lông Green Court | Ca sáng: Admin");
            BindClick(btnNavKhachHang, khachHang, "Quản lý Khách hàng", "Theo dõi hồ sơ người chơi, cấp bậc thành viên và chi tiêu.");
            BindClick(btnNavKhoHang, khoHang, "Kho hàng", "Quản lý xuất nhập tồn báo cáo vận hành");
            BindClick(btnNavBaoCao, baoCao, "Báo cáo & Phân tích", "Theo dõi và phân tích.");
            BindClick(btnNavThanhToan, thanhToan, "Thanh Toán Hóa Đơn", "Kiểm tra bill, thu tiền và in hóa đơn cho khách.");

            System.EventHandler logoClick = (s, e) => SwitchModule(tongQuan, btnNavDashboard, "Dashboard", "Tổng quan hoạt động");
            pnlLogo.Cursor = Cursors.Hand;
            pnlLogo.Click += logoClick;
            foreach (Control c in pnlLogo.Controls)
            {
                c.Cursor = Cursors.Hand;
                c.Click += logoClick;
            }

            pnlAdminAvatar.Cursor = Cursors.Hand;
            pnlAdminAvatar.Click += UserMenu_Click;
            foreach (Control c in pnlAdminAvatar.Controls)
            {
                c.Cursor = Cursors.Hand;
                c.Click += UserMenu_Click;
            }
        }
        
        private void BindClick(Sunny.UI.UIPanel p, UserControl uc, string title, string subtitle)
        {
            System.EventHandler h = (s, e) => {
                pnlContent.Controls.Clear();
                pnlContent.Controls.Add(uc);
                if(uc != null) SwitchModule(uc, p, title, subtitle);
            };
            p.Click += h;

            // Fix dark hover issue
            System.EventHandler hoverIn = (s, e) => {
                if (p.FillColor == Color.White)
                    p.FillColor = Color.FromArgb(235, 245, 235); // Light mint green instead of gray
            };
            System.EventHandler hoverOut = (s, e) => {
                if (p.FillColor == Color.FromArgb(235, 245, 235))
                    p.FillColor = Color.White;
            };

            p.MouseEnter += hoverIn;
            p.MouseLeave += hoverOut;

            foreach(Control c in p.Controls)
            {
                c.Click += h;
                c.Cursor = Cursors.Hand;
                c.MouseEnter += hoverIn;
                c.MouseLeave += hoverOut;
            }
        }

        public void SwitchModule(UserControl uc, Sunny.UI.UIPanel activeBtn, string title, string subtitle)
        {
            pnlContent.Controls.Clear();

            UiTheme.ApplyPageBackground(pnlContent);
            UiTheme.ApplyModuleTheme(uc);

            pnlContent.Controls.Add(uc);

            // Refresh data-driven modules when navigating between pages.
            try
            {
                if (uc is UCBanHang bh) bh.RefreshOnActivated();
                else if (uc is UCKhoHang kh) kh.RefreshOnActivated();
                else if (uc is UCThanhToan tt) tt.RefreshOnActivated();
            }
            catch (Exception ex)
            {
                try { DemoPick.Services.DatabaseHelper.TryLog("SwitchModule Refresh Error", ex, "FrmChinh.SwitchModule"); } catch { }
            }
            
            lblPageTitle.Text = title;
            lblPageSubtitle.Text = subtitle;

            foreach (var btn in menuButtons)
            {
                btn.FillColor = Color.White;
                SetLabelStyle(btn, false);
            }
            // Primary Color #4CAF50
            activeBtn.FillColor = Color.FromArgb(76, 175, 80);
            
            SetLabelStyle(activeBtn, true);
        }

        public void NavigateToDatLich()
        {
            SwitchModule(datLich, btnNavDatLich, "Quản lý Sơ đồ && Đặt lịch", "");
        }

        private void SetLabelStyle(Sunny.UI.UIPanel p, bool active)
        {
            foreach(Control c in p.Controls)
            {
                if (c is Label lbl)
                {
                    lbl.BackColor = Color.Transparent;
                    lbl.ForeColor = active ? Color.White : Color.FromArgb(107, 114, 128);
                    lbl.Font = new Font("Segoe UI", 11F, active ? FontStyle.Bold : FontStyle.Regular);
                }
            }
        }

        private void ApplyCurrentUserUI()
        {
            var user = AppSession.CurrentUser;
            if (user == null)
            {
                if (lblAdminName != null) lblAdminName.Text = "Chưa đăng nhập";
                if (lblAdminStatus != null) lblAdminStatus.Text = "";
                if (lblAdminAvatarText != null) lblAdminAvatarText.Text = "?";
                return;
            }

            string displayName = !string.IsNullOrWhiteSpace(user.FullName) ? user.FullName : user.Username;
            if (lblAdminName != null) lblAdminName.Text = displayName;
            if (lblAdminStatus != null) lblAdminStatus.Text = string.IsNullOrWhiteSpace(user.Role) ? "" : user.Role;

            if (lblAdminAvatarText != null)
            {
                string first = string.IsNullOrWhiteSpace(displayName) ? "?" : displayName.Trim().Substring(0, 1);
                lblAdminAvatarText.Text = first.ToUpperInvariant();
            }
        }

        private void ApplyRolePermissions()
        {
            bool isAdmin = AppSession.IsInRole("Admin");
            bool isStaff = AppSession.IsInRole("Staff");

            // Allow Staff to use Inventory & Reports too (Admin keeps full access).
            bool canAccessOps = isAdmin || isStaff;
            SetMenuEnabled(btnNavKhoHang, canAccessOps);
            SetMenuEnabled(btnNavBaoCao, canAccessOps);

            if (!canAccessOps)
            {
                // Ensure we don't land on a disabled module.
                SwitchModule(tongQuan, btnNavDashboard, "Dashboard", "Tổng quan hoạt động");
            }
        }

        private void SetMenuEnabled(Sunny.UI.UIPanel panel, bool enabled)
        {
            if (panel == null) return;

            panel.Enabled = enabled;
            foreach (Control c in panel.Controls)
            {
                c.Enabled = enabled;
            }

            if (!enabled)
            {
                panel.FillColor = Color.White;
                SetLabelStyle(panel, false);
                panel.Cursor = Cursors.Default;
                foreach (Control c in panel.Controls)
                {
                    c.Cursor = Cursors.Default;
                }
            }
        }

        private void pnlAdminAvatar_Paint(object sender, PaintEventArgs e)
        {
            if (sender is Panel p)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddEllipse(0, 0, p.Width - 1, p.Height - 1);
                    p.Region = new Region(gp);
                }
            }
        }

        private void UserMenu_Click(object sender, EventArgs e)
        {
            var frm = new FrmUserMenu();
            // Nằm ngay trên đầu Avatar, vì Avatar đang ở góc dưới cùng bên trái màn hình
            Point p = pnlAdminAvatar.PointToScreen(new Point(0, -frm.Height - 5));
            frm.Location = p;
            
            frm.FormClosed += (s, args) => 
            {
                if (frm.DialogResult == DialogResult.Retry) 
                {
                    // Logout triggered
                    this.DialogResult = DialogResult.Retry;
                    this.BeginInvoke(new Action(() => this.Close())); // Fix StackOverflow
                }
            };
            
            frm.Show(this); // Không dùng ShowDialog vòng kín, dùng Show tự do để nhận diện Click Outside
        }
    }
}
