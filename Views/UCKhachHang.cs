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
        private ImageList _rowHeightImageList;

        public UCKhachHang()
        {
            InitializeComponent();

            if (DesignModeUtil.IsDesignMode(this))
            {
                return;
            }
            _customerService = new CustomerService();
            _allCustomersCache = new System.Collections.Generic.List<DemoPick.Models.CustomerModel>();
            EnsureCustomerListRowHeight();
            InitializeCustomerListColumns();

            this.Disposed += (s, e) =>
            {
                if (_rowHeightImageList != null)
                {
                    _rowHeightImageList.Dispose();
                    _rowHeightImageList = null;
                }
            };

            lblTabAll.Click += (s, e) => FilterList("Tất cả", lblTabAll);
            lblTabAll.Cursor = Cursors.Hand;
            lblTabFixed.Click += (s, e) => FilterList("Cố định", lblTabFixed);
            lblTabFixed.Cursor = Cursors.Hand;
            lblTabWalkin.Click += (s, e) => FilterList("Vãng lai", lblTabWalkin);
            lblTabWalkin.Cursor = Cursors.Hand;
            lblTabNew.Click += (s, e) => FilterList("Mới", lblTabNew);
            lblTabNew.Cursor = Cursors.Hand;

            this.picFilter.Paint += (s, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                using (var pen = new Pen(Color.FromArgb(107, 114, 128), 2))
                {
                    g.DrawLine(pen, 5, 10, 25, 10);
                    g.DrawLine(pen, 10, 16, 20, 16);
                    g.DrawLine(pen, 13, 22, 17, 22);
                }
            };

            lstKhachHang.DrawColumnHeader += LstKhachHang_DrawColumnHeader;
            lstKhachHang.DrawSubItem += LstKhachHang_DrawSubItem;

            LoadDataAsync();
        }

        private void EnsureCustomerListRowHeight()
        {
            if (lstKhachHang == null)
            {
                return;
            }

            if (_rowHeightImageList == null)
            {
                _rowHeightImageList = new ImageList
                {
                    ColorDepth = ColorDepth.Depth32Bit,
                    ImageSize = new Size(1, 36)
                };
                _rowHeightImageList.Images.Add(new Bitmap(1, 36));
            }

            lstKhachHang.SmallImageList = _rowHeightImageList;
        }

        private void InitializeCustomerListColumns()
        {
            if (lstKhachHang == null)
            {
                return;
            }

            lstKhachHang.BeginUpdate();
            lstKhachHang.Columns.Clear();
            lstKhachHang.Columns.Add("Khách hàng", 350, HorizontalAlignment.Left);
            lstKhachHang.Columns.Add("SĐT", 140, HorizontalAlignment.Left);
            lstKhachHang.Columns.Add("Phân loại", 120, HorizontalAlignment.Left);
            lstKhachHang.Columns.Add("Hạng", 100, HorizontalAlignment.Center);
            lstKhachHang.Columns.Add("Giờ tích lũy", 180, HorizontalAlignment.Left);
            lstKhachHang.Columns.Add("Chi tiêu", 160, HorizontalAlignment.Right);
            lstKhachHang.EndUpdate();
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
                    string customerIdentity = $"{c.Name} - ID: {c.Id}";
                    var item = new ListViewItem(new[] { 
                        $"   {customerIdentity}", 
                        c.Phone, 
                        c.CustomerType, 
                        c.Tier,
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
                lstKhachHang.Items.Add(new ListViewItem(new[] { "   ⚠ Chưa có Hội viên", "-", "-", "-", "-", "0đ" }));
            }
        }

        private void LstKhachHang_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawBackground();
            using (var brush = new SolidBrush(Color.FromArgb(156, 163, 175)))
            {
                using (var sf = new StringFormat { LineAlignment = StringAlignment.Center })
                {
                    e.Graphics.DrawString(e.Header.Text, e.Font, brush, e.Bounds, sf);
                }
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

                int pillHeight = Math.Max(18, Math.Min(22, e.Bounds.Height - 8));
                int pillWidth = Math.Min(86, Math.Max(72, e.Bounds.Width - 12));
                int pillY = e.Bounds.Y + (e.Bounds.Height - pillHeight) / 2;

                Rectangle rect = new Rectangle(e.Bounds.X + 6, pillY, pillWidth, pillHeight);
                
                using (GraphicsPath p = new GraphicsPath())
                {
                    int r = Math.Min(12, rect.Height / 2);
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
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    using (var font = new Font(e.Item.Font.FontFamily, 9F, FontStyle.Bold))
                    {
                        e.Graphics.DrawString(text, font, fgBrush, rect, sf);
                    }
                }
            }
            else if (e.ColumnIndex == 3) // Hạng (Tier)
            {
                Color badgeBgColor;
                Color badgeFgColor;
                string tierText = MembershipTierHelper.NormalizeTier(c.Tier);
                
                if (tierText == "Gold")
                {
                    badgeBgColor = Color.FromArgb(254, 243, 199); // Light yellow/amber
                    badgeFgColor = Color.FromArgb(217, 119, 6);   // Dark amber
                }
                else if (tierText == "Silver")
                {
                    badgeBgColor = Color.FromArgb(243, 244, 246); // Light gray
                    badgeFgColor = Color.FromArgb(75, 85, 99);    // Dark gray
                }
                else
                {
                    badgeBgColor = Color.FromArgb(224, 231, 255); // Light indigo
                    badgeFgColor = Color.FromArgb(67, 56, 202);   // Dark indigo
                }

                int pillHeight = Math.Max(18, Math.Min(22, e.Bounds.Height - 8));
                int pillWidth = Math.Min(86, Math.Max(72, e.Bounds.Width - 12));
                int pillY = e.Bounds.Y + (e.Bounds.Height - pillHeight) / 2;

                Rectangle rect = new Rectangle(e.Bounds.X + (e.Bounds.Width - pillWidth) / 2, pillY, pillWidth, pillHeight);
                
                using (GraphicsPath p = new GraphicsPath())
                {
                    int r = Math.Min(12, rect.Height / 2);
                    p.AddArc(rect.X, rect.Y, r, r, 180, 90);
                    p.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
                    p.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
                    p.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
                    p.CloseFigure();
                    
                    using (Brush bgBrush = new SolidBrush(badgeBgColor))
                    {
                        e.Graphics.FillPath(bgBrush, p);
                    }
                }
                
                using (Brush fgBrush = new SolidBrush(badgeFgColor))
                {
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    using (var font = new Font(e.Item.Font.FontFamily, 9F, FontStyle.Bold))
                    {
                        e.Graphics.DrawString(tierText, font, fgBrush, rect, sf);
                    }
                }
            }
            else if (e.ColumnIndex == 4) // Giờ tích lũy (Progress Bar)
            {
                using (var boldFont = new Font(e.Item.Font, FontStyle.Bold))
                {
                    e.Graphics.DrawString(e.SubItem.Text, boldFont, Brushes.Black, new Point(e.Bounds.X, e.Bounds.Y + 2));
                }
                
                Rectangle barRect = new Rectangle(e.Bounds.X, e.Bounds.Y + 25, 100, 6);
                using (GraphicsPath pBg = new GraphicsPath())
                {
                    int r = 6;
                    pBg.AddArc(barRect.X, barRect.Y, r, r, 180, 90);
                    pBg.AddArc(barRect.Right - r, barRect.Y, r, r, 270, 90);
                    pBg.AddArc(barRect.Right - r, barRect.Bottom - r, r, r, 0, 90);
                    pBg.AddArc(barRect.X, barRect.Bottom - r, r, r, 90, 90);
                    pBg.CloseFigure();
                    using (var bgBrush = new SolidBrush(Color.FromArgb(229, 231, 235)))
                    {
                        e.Graphics.FillPath(bgBrush, pBg);
                    }
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
                            using (var barBrush = new SolidBrush(barColor))
                            {
                                e.Graphics.FillPath(barBrush, pFg);
                            }
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
                var membershipTask = _customerService.GetMembershipSummaryAsync();

                await Task.WhenAll(customersTask, tierTask, revenueTask, courtTask, membershipTask);

                BindCustomers(customersTask.Result);
                BindTierCounts(tierTask.Result);
                BindRevenue(revenueTask.Result, tierTask.Result);
                BindTodayOccupancy(courtTask.Result);
                BindMembershipSummary(membershipTask.Result);
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("Customer KPI Sync Error", ex, "UCKhachHang.LoadDataAsync");
            }
        }

        public void RefreshOnActivated()
        {
            LoadDataAsync();
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

        private void BindMembershipSummary(DemoPick.Models.MembershipSummaryModel summary)
        {
            summary = summary ?? new DemoPick.Models.MembershipSummaryModel();

            lblFixedValue.Text = "Khách cố định";
            lblFixedDesc.Text = "Khách hàng có lịch sử dụng sân cố định thường xuyên.";

            int totalMembers = summary.BasicCount + summary.SilverCount + summary.GoldCount;
            lblWalkinValue.Text = "Khách vãng lai";
            lblWalkinDesc.Text = "Khách hàng đặt lịch vãng lai qua hệ thống.";

            lblBot1Desc.Text = $"Tổng số khách vãng lai: {totalMembers}";
            lblBot2Desc.Text = "Theo dõi chi tiêu và tần suất đặt sân";
            lblBot3Desc.Text = "Quản lý khách hàng hiệu quả";
        }
    }
}
