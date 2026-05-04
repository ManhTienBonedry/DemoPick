// ==========================================================
// File: UCTongQuan.cs
// Role: View (MVC)
// Description: UserControl tổng quan bảng điều khiển.
// Hiển thị thông tin từ DashboardController và InventoryController.
// ==========================================================
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using DemoPick.Controllers;
using DemoPick.Models;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;

namespace DemoPick
{
    public partial class UCTongQuan : UserControl
    {
        private DashboardController _dashboardController;
        private InventoryController _inventoryController;
        private Label _lblInventoryWarning;

        private bool _scrollResetQueued;

        // Khoi tao man hinh/control UCTongQuan va chuan bi trang thai ban dau can dung.
        public UCTongQuan()
        {
            InitializeComponent();

            if (DesignModeUtil.IsDesignMode(this))
            {
                return;
            }
            SetupCharts();
            SetupTable();
            AttachBorders();

            _dashboardController = new DashboardController();
            _inventoryController = new InventoryController();

            _lblInventoryWarning = new Label();
            _lblInventoryWarning.AutoSize = true;
            _lblInventoryWarning.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            _lblInventoryWarning.ForeColor = Color.FromArgb(220, 38, 38);
            _lblInventoryWarning.BackColor = Color.FromArgb(254, 226, 226);
            _lblInventoryWarning.Padding = new Padding(10, 5, 10, 5);
            _lblInventoryWarning.Location = new Point(25, 475);
            _lblInventoryWarning.Visible = false;
            this.Controls.Add(_lblInventoryWarning);

            LoadRealDataAsync();
        }

        // Xu ly logic man hinh On Visible Changed va cap nhat control lien quan.
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (DesignModeUtil.IsDesignMode(this))
            {
                return;
            }

            if (Visible)
            {
                QueueResetScrollToTop();
            }
        }

        // Xoa, huy hoac dat lai du lieu Reset Scroll To Top theo dung dieu kien nghiep vu.
        public void ResetScrollToTop()
        {
            if (!AutoScroll) return;

            try
            {
                SuspendLayout();
                AutoScrollPosition = new Point(0, 0);

                ResetScrollValue(VerticalScroll, target: 0);
                ResetScrollValue(HorizontalScroll, target: 0);
            }
            finally
            {
                ResumeLayout(true);
            }
        }

        // Xoa, huy hoac dat lai du lieu Reset Scroll Value theo dung dieu kien nghiep vu.
        private static void ResetScrollValue(ScrollProperties scroll, int target)
        {
            if (scroll == null) return;

            int min = scroll.Minimum;
            int max = scroll.Maximum - scroll.LargeChange + 1;
            if (max < min) max = min;

            if (target < min) target = min;
            if (target > max) target = max;

            scroll.Value = target;
        }

        // Xu ly logic man hinh Queue Reset Scroll To Top va cap nhat control lien quan.
        private void QueueResetScrollToTop()
        {
            if (_scrollResetQueued) return;
            _scrollResetQueued = true;

            try
            {
                BeginInvoke((Action)(() =>
                {
                    _scrollResetQueued = false;
                    ResetScrollToTop();
                }));
            }
            catch
            {
                _scrollResetQueued = false;
            }
        }

        // Thiet lap Attach Borders va gan cac cau hinh/su kien can dung ban dau.
        private void AttachBorders()
        {
            PaintEventHandler drawBorder = (s, e) => {
                var pnl = s as Panel;
                if (pnl == null) return;
                using (var pen = new Pen(UiTheme.CardBorder, UiTheme.CardBorderWidth))
                {
                    pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                    e.Graphics.DrawRectangle(pen, 0, 0, pnl.Width - 1, pnl.Height - 1);
                }
            };
            pnlCard1.Paint += drawBorder;
            pnlCard2.Paint += drawBorder;
            pnlCard3.Paint += drawBorder;
            pnlCard4.Paint += drawBorder;
            pnlChartBorder.Paint += drawBorder;
            pnlPieBorder.Paint += drawBorder;
            pnlTable.Paint += drawBorder;
        }

        // Thiet lap Setup Charts va gan cac cau hinh/su kien can dung ban dau.
        private void SetupCharts()
        {
            // Trend chart: clean spline-area style for clear up/down reading.
            chartTrend.ChartAreas.Clear();
            chartTrend.Series.Clear();
            chartTrend.BackColor = Color.Transparent;

            ChartArea trendArea = new ChartArea("MainArea");
            trendArea.BackColor = Color.Transparent;
            trendArea.AxisX.MajorGrid.Enabled = false;
            trendArea.AxisY.MajorGrid.LineColor = Color.FromArgb(232, 234, 237);
            trendArea.AxisX.LineColor = Color.FromArgb(220, 225, 229);
            trendArea.AxisY.LineColor = Color.FromArgb(220, 225, 229);
            trendArea.AxisX.LabelStyle.ForeColor = Color.FromArgb(107, 114, 128);
            trendArea.AxisY.LabelStyle.ForeColor = Color.FromArgb(107, 114, 128);
            trendArea.AxisY.LabelStyle.Format = "#,0";
            chartTrend.ChartAreas.Add(trendArea);

            Series s = new Series("Revenue");
            s.ChartType = SeriesChartType.SplineArea;
            s.Color = Color.FromArgb(90, 76, 175, 80);
            s.BorderColor = Color.FromArgb(40, 140, 60);
            s.BorderWidth = 3;
            s.MarkerStyle = MarkerStyle.Circle;
            s.MarkerSize = 5;
            s.MarkerColor = Color.FromArgb(40, 140, 60);
            s["LineTension"] = "0.35";
            chartTrend.Series.Add(s);
            chartTrend.Legends.Clear();

            // Doughnut chart.
            chartPie.ChartAreas.Clear();
            chartPie.Series.Clear();
            chartPie.Legends.Clear();

            chartPie.ChartAreas.Add(new ChartArea("PieArea"));
            chartPie.ChartAreas[0].BackColor = Color.Transparent;
            Series sp = new Series("Tỉ trọng");
            sp.ChartType = SeriesChartType.Doughnut;
            sp.BorderWidth = 2;
            sp.BorderColor = Color.White;
            sp["DoughnutRadius"] = "68";
            
            chartPie.Series.Add(sp);
            
            Legend l = new Legend("Legend");
            l.Docking = Docking.Bottom;
            l.Alignment = StringAlignment.Center;
            l.BackColor = Color.Transparent;
            chartPie.Legends.Add(l);
        }

        // Thiet lap Setup Table va gan cac cau hinh/su kien can dung ban dau.
        private void SetupTable()
        {
            lstBookings.Columns.Add("Mã Đặt", 100);
            lstBookings.Columns.Add("Sân", 200);
            lstBookings.Columns.Add("Khách hàng", 250);
            lstBookings.Columns.Add("Thời gian", 200);
            lstBookings.Columns.Add("Trạng thái", 150);
            lstBookings.Columns.Add("Doanh thu", 150);
            lstBookings.Items.Clear();
        }

        // Nap du lieu cho Load Real Data roi cap nhat lai trang thai hien thi tren man hinh.
        private async void LoadRealDataAsync()
        {
            try
            {
                var metricsTask = _dashboardController.GetMetricsAsync();
                var trendTask = _dashboardController.GetRevenueTrendLast7DaysAsync();
                var pieTask = _dashboardController.GetTopCourtsRevenueAsync();
                var activityTask = _dashboardController.GetRecentActivityAsync(10);
                var inventoryTask = _inventoryController.GetInventoryKpisAsync();

                await System.Threading.Tasks.Task.WhenAll(metricsTask, trendTask, pieTask, activityTask, inventoryTask);

                BindTopCards(metricsTask.Result);
                BindTrendChart(trendTask.Result);
                BindPieChart(pieTask.Result);
                BindRecentActivity(activityTask.Result);
                
                var kpi = inventoryTask.Result ?? new DemoPick.Models.InventoryKpiModel();
                if (kpi.CriticalItems > 0)
                {
                    _lblInventoryWarning.Text = $"⚠ CẢNH BÁO: Có {kpi.CriticalItems} sản phẩm sắp hết hàng trong kho. Vui lòng kiểm tra mục Kho hàng.";
                    _lblInventoryWarning.Visible = true;
                }
                else
                {
                    _lblInventoryWarning.Visible = false;
                }
            }
            catch (Exception ex)
            {
                DemoPick.Data.DatabaseHelper.TryLog("Dashboard Load Error", ex, "UCTongQuan.LoadRealDataAsync");
            }
        }

        // Cap nhat lai du lieu/trang thai Refresh On Activated tren man hinh hien tai.
        public void RefreshOnActivated()
        {
            LoadRealDataAsync();
            QueueResetScrollToTop();
        }

        // Dua du lieu Bind Top Cards len giao dien hoac ve lai phan hien thi lien quan.
        private void BindTopCards(DemoPick.Models.DashboardMetricsModel metrics)
        {
            metrics = metrics ?? new DemoPick.Models.DashboardMetricsModel();

            lblC1V.Text = metrics.Revenue == 0 ? "0 đ" : metrics.Revenue.ToString("N0") + "đ";
            lblC2V.Text = metrics.OccupancyPct.ToString() + "%";
            lblC3V.Text = metrics.CustomerCount.ToString();
            lblC4V.Text = metrics.PosCount.ToString();

            lblC1S.Text = "+0% ↗"; // placeholder for delta
            lblC2S.Text = "+0% ↗";
            lblC3S.Text = "+0.0% ↗";
            lblC4S.Text = "+0% ↗";
        }

        // Dua du lieu Bind Trend Chart len giao dien hoac ve lai phan hien thi lien quan.
        private void BindTrendChart(System.Collections.Generic.List<DemoPick.Models.TrendPointModel> points)
        {
            chartTrend.Series[0].Points.Clear();

            points = points ?? new System.Collections.Generic.List<DemoPick.Models.TrendPointModel>();
            foreach (var p in points)
            {
                chartTrend.Series[0].Points.AddXY(p.Label, p.Revenue);
            }

            if (points.Count >= 2)
            {
                decimal first = points[0].Revenue;
                decimal last = points[points.Count - 1].Revenue;
                if (last > first)
                {
                    lblChartT.Text = "Xu hướng doanh thu ▲";
                    lblChartT.ForeColor = Color.FromArgb(22, 163, 74);
                }
                else if (last < first)
                {
                    lblChartT.Text = "Xu hướng doanh thu ▼";
                    lblChartT.ForeColor = Color.FromArgb(220, 38, 38);
                }
                else
                {
                    lblChartT.Text = "Xu hướng doanh thu";
                    lblChartT.ForeColor = Color.FromArgb(26, 35, 50);
                }
            }
            else
            {
                lblChartT.Text = "Xu hướng doanh thu";
                lblChartT.ForeColor = Color.FromArgb(26, 35, 50);
            }
        }

        // Dua du lieu Bind Pie Chart len giao dien hoac ve lai phan hien thi lien quan.
        private void BindPieChart(System.Collections.Generic.List<DemoPick.Models.NamedRevenueModel> slices)
        {
            chartPie.Series[0].Points.Clear();

            slices = slices ?? new System.Collections.Generic.List<DemoPick.Models.NamedRevenueModel>();
            if (slices.Count == 0 || (slices.Count > 0 && slices[0].Revenue == 0))
            {
                chartPie.Series[0].Points.AddXY("Chưa có D.Thu", 100);
                chartPie.Series[0].Points[0].Color = Color.LightGray;
                return;
            }

            Color[] colors = { Color.FromArgb(76, 175, 80), Color.FromArgb(129, 199, 132), Color.FromArgb(165, 214, 167), Color.FromArgb(232, 245, 233) };
            int i = 0;
            foreach (var s in slices)
            {
                if (s.Revenue > 0)
                {
                    chartPie.Series[0].Points.AddXY(s.Name, s.Revenue);
                    chartPie.Series[0].Points[chartPie.Series[0].Points.Count - 1].Color = colors[i % colors.Length];
                    i++;
                }
            }
        }

        // Dua du lieu Bind Recent Activity len giao dien hoac ve lai phan hien thi lien quan.
        private void BindRecentActivity(System.Collections.Generic.List<DemoPick.Models.DashboardActivityModel> items)
        {
            lstBookings.Items.Clear();

            items = items ?? new System.Collections.Generic.List<DemoPick.Models.DashboardActivityModel>();
            foreach (var a in items)
            {
                string amt = a.Amount == 0 ? "0đ" : a.Amount.ToString("N0") + "đ";
                lstBookings.Items.Add(new ListViewItem(new[] { a.Code, a.CourtName, a.CustomerName, a.TimeText, a.Status, amt }));
            }
            if (lstBookings.Items.Count == 0)
            {
                lstBookings.Items.Add(new ListViewItem(new[] { "-", "-", "Không có hoạt động nào", "-", "-", "0đ" }));
            }
        }
    }
}


