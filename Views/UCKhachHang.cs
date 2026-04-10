using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using DemoPick.Services;

namespace DemoPick
{
    public partial class UCKhachHang : UserControl
    {
        private CustomerService _customerService;
        private System.Collections.Generic.List<DemoPick.Models.CustomerModel> _allCustomersCache;

        public UCKhachHang()
        {
            InitializeComponent();

            if (DesignModeUtil.IsDesignMode(this))
            {
                return;
            }
            _customerService = new CustomerService();
            _allCustomersCache = new System.Collections.Generic.List<DemoPick.Models.CustomerModel>();

            lblTabAll.Click += (s, e) => FilterList("Tất cả", lblTabAll);
            lblTabAll.Cursor = Cursors.Hand;
            lblTabFixed.Click += (s, e) => FilterList("Cố định", lblTabFixed);
            lblTabFixed.Cursor = Cursors.Hand;
            lblTabWalkin.Click += (s, e) => FilterList("Vãng lai", lblTabWalkin);
            lblTabWalkin.Cursor = Cursors.Hand;
            lblTabNew.Click += (s, e) => FilterList("Mới", lblTabNew);
            lblTabNew.Cursor = Cursors.Hand;

            this.picFilter.Paint += (s, e) => {
                Graphics g = e.Graphics; g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.DrawLine(new Pen(Color.FromArgb(107, 114, 128), 2), 5, 10, 25, 10);
                g.DrawLine(new Pen(Color.FromArgb(107, 114, 128), 2), 10, 16, 20, 16);
                g.DrawLine(new Pen(Color.FromArgb(107, 114, 128), 2), 13, 22, 17, 22);
            };

            lstKhachHang.DrawColumnHeader += LstKhachHang_DrawColumnHeader;
            lstKhachHang.DrawSubItem += LstKhachHang_DrawSubItem;

            LoadDataAsync();
        }

        private void FilterList(string filterMode, Label activeLabel)
        {
            lblTabAll.ForeColor = Color.FromArgb(107, 114, 128);
            lblTabFixed.ForeColor = Color.FromArgb(107, 114, 128);
            lblTabWalkin.ForeColor = Color.FromArgb(107, 114, 128);
            lblTabNew.ForeColor = Color.FromArgb(107, 114, 128);
            activeLabel.ForeColor = Color.FromArgb(22, 163, 74); // Green
            
            pnlTabIndicator.Location = new Point(activeLabel.Left, 45);
            pnlTabIndicator.Width = activeLabel.Width;

            lstKhachHang.Items.Clear();
            foreach (var c in _allCustomersCache)
            {
                bool show = true;
                
                if (filterMode == "Cố định" && c.CustomerType != "Cố định") show = false;
                if (filterMode == "Vãng lai" && c.CustomerType != "Vãng lai") show = false;
                if (filterMode == "Mới" && (DateTime.Now - c.CreatedAt).TotalHours > 24) show = false;
                
                if (show)
                {
                    var item = new ListViewItem(new[] { 
                        $"   {c.Name}\n   ID: {c.Id}", 
                        c.Phone, 
                        c.CustomerType, 
                        c.TotalHours.ToString("0.##") + "h", 
                        c.TotalSpent 
                    });
                    item.Tag = c; // Pass full object to custom draw
                    lstKhachHang.Items.Add(item);
                }
            }

            // Show Empty Notification if blank
            if (lstKhachHang.Items.Count == 0 && _allCustomersCache.Count == 0)
            {
                lstKhachHang.Items.Add(new ListViewItem(new[] { "   ⚠ Chưa có Hội viên", "-", "-", "-", "0đ" }));
            }
        }

        private void LstKhachHang_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawBackground();
            using (var brush = new SolidBrush(Color.FromArgb(156, 163, 175)))
            {
                e.Graphics.DrawString(e.Header.Text, e.Font, brush, e.Bounds, new StringFormat { LineAlignment = StringAlignment.Center });
            }
        }

        private void LstKhachHang_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Item.Tag == null)
            {
                e.DrawText(TextFormatFlags.VerticalCenter);
                return;
            }

            var c = (DemoPick.Models.CustomerModel)e.Item.Tag;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (e.ColumnIndex == 2) // Phân Loại (Pills)
            {
                bool isFixed = c.CustomerType == "Cố định";
                Color bgColor = isFixed ? Color.FromArgb(236, 253, 245) : Color.FromArgb(243, 244, 246);
                Color fgColor = isFixed ? Color.FromArgb(16, 185, 129) : Color.FromArgb(107, 114, 128);
                string text = isFixed ? "Cố định" : "Vãng lai";

                Rectangle rect = new Rectangle(e.Bounds.X + 5, e.Bounds.Y + 10, 80, 24);
                
                using (GraphicsPath p = new GraphicsPath())
                {
                    int r = 12;
                    p.AddArc(rect.X, rect.Y, r, r, 180, 90);
                    p.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
                    p.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
                    p.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
                    p.CloseFigure();
                    
                    using (Brush bgBrush = new SolidBrush(bgColor))
                    {
                        e.Graphics.FillPath(bgBrush, p);
                    }
                    if (isFixed)
                    {
                        using (Pen pen = new Pen(fgColor))
                            e.Graphics.DrawPath(pen, p);
                    }
                }
                
                using (Brush fgBrush = new SolidBrush(fgColor))
                {
                    StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    e.Graphics.DrawString(text, new Font(e.Item.Font.FontFamily, 9F, FontStyle.Bold), fgBrush, rect, sf);
                }
            }
            else if (e.ColumnIndex == 3) // Giờ tích lũy (Progress Bar)
            {
                e.Graphics.DrawString(e.SubItem.Text, new Font(e.Item.Font, FontStyle.Bold), Brushes.Black, new Point(e.Bounds.X, e.Bounds.Y + 2));
                
                Rectangle barRect = new Rectangle(e.Bounds.X, e.Bounds.Y + 25, 100, 6);
                using (GraphicsPath pBg = new GraphicsPath())
                {
                    int r = 6;
                    pBg.AddArc(barRect.X, barRect.Y, r, r, 180, 90);
                    pBg.AddArc(barRect.Right - r, barRect.Y, r, r, 270, 90);
                    pBg.AddArc(barRect.Right - r, barRect.Bottom - r, r, r, 0, 90);
                    pBg.AddArc(barRect.X, barRect.Bottom - r, r, r, 90, 90);
                    pBg.CloseFigure();
                    e.Graphics.FillPath(new SolidBrush(Color.FromArgb(229, 231, 235)), pBg);
                }

                float pct = (float)c.TotalHours / 30f;
                if (pct > 1) pct = 1f;

                if (pct > 0)
                {
                    Rectangle fgRect = new Rectangle(e.Bounds.X, e.Bounds.Y + 25, (int)(100 * pct), 6);
                    if (fgRect.Width > 4)
                    {
                        using (GraphicsPath pFg = new GraphicsPath())
                        {
                            int r = 6;
                            pFg.AddArc(fgRect.X, fgRect.Y, r, r, 180, 90);
                            pFg.AddArc(fgRect.Right - r, fgRect.Y, r, r, 270, 90);
                            pFg.AddArc(fgRect.Right - r, fgRect.Bottom - r, r, r, 0, 90);
                            pFg.AddArc(fgRect.X, fgRect.Bottom - r, r, r, 90, 90);
                            pFg.CloseFigure();
                            
                            Color barColor = pct >= 1f ? Color.FromArgb(16, 185, 129) : Color.FromArgb(167, 139, 250);
                            e.Graphics.FillPath(new SolidBrush(barColor), pFg);
                        }
                    }
                }
            }
            else
            {
                e.DrawText(TextFormatFlags.VerticalCenter);
            }
        }

        private async void LoadDataAsync()
        {
            try
            {
                // Run the expensive queries in parallel to reduce screen-load latency.
                var customersTask = LoadCustomersAsync();
                var tierTask = _customerService.GetTierCountsAsync();
                var revenueTask = _customerService.GetRevenueSummaryAsync();
                var courtTask = _customerService.GetTodayOccupancyPctAsync();

                await Task.WhenAll(customersTask, tierTask, revenueTask, courtTask);

                BindCustomers(customersTask.Result);
                BindTierCounts(tierTask.Result);
                BindRevenue(revenueTask.Result, tierTask.Result);
                BindTodayOccupancy(courtTask.Result);
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("Customer KPI Sync Error", ex, "UCKhachHang.LoadDataAsync");
            }
        }

        private Task<System.Collections.Generic.List<DemoPick.Models.CustomerModel>> LoadCustomersAsync()
        {
            return _customerService.GetAllCustomersAsync();
        }

        private void BindCustomers(System.Collections.Generic.List<DemoPick.Models.CustomerModel> customers)
        {
            _allCustomersCache = customers ?? new System.Collections.Generic.List<DemoPick.Models.CustomerModel>();
            FilterList("Tất cả", lblTabAll);
        }

        private void BindTierCounts(DemoPick.Models.CustomerTierCountsModel tier)
        {
            if (tier == null)
            {
                tier = new DemoPick.Models.CustomerTierCountsModel();
            }
            lblFixedCount.Text = $"● {tier.FixedCount} Hội viên";
            lblWalkinCount.Text = $"● {tier.WalkinCount} Hội viên";
        }

        private void BindRevenue(DemoPick.Models.CustomerRevenueSummaryModel revenue, DemoPick.Models.CustomerTierCountsModel tier)
        {
            if (revenue == null)
            {
                revenue = new DemoPick.Models.CustomerRevenueSummaryModel();
            }

            if (tier == null)
            {
                tier = new DemoPick.Models.CustomerTierCountsModel();
            }

            lblBot1Value.Text = revenue.MemberCount.ToString();
            lblBot3Value.Text = revenue.Revenue == 0 ? "0đ" : revenue.Revenue.ToString("N0") + " đ";
            lblBot2Value.Text = tier.FixedCount.ToString();
        }

        private void BindTodayOccupancy(int occPct)
        {
            lblBot4Value.Text = $"{occPct}%";
        }
    }
}
