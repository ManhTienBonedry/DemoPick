using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using DemoPick.Models;
using DemoPick.Services;

namespace DemoPick
{
    public partial class UCBaoCao : UserControl
    {
        private ReportService _reportService;
        private Font _topCourtsViewAllNormalFont;
        private Font _topCourtsViewAllUnderlineFont;

        private UCDateRangeFilter DateFilter => dateFilter;

        private static readonly Color KpiPositiveColor = Color.FromArgb(76, 175, 80);
        private static readonly Color KpiNegativeColor = Color.FromArgb(239, 68, 68);
        private static readonly Color KpiNeutralColor = Color.FromArgb(107, 114, 128);

        public UCBaoCao()
        {
            InitializeComponent();

            if (DesignModeUtil.IsDesignMode(this))
            {
                return;
            }

            _reportService = new ReportService();

            SetupTopCourtsTable();
            SetupChartStyle();

            try
            {
                var today = DateTime.Today;
                var defaultFrom = today.AddDays(-6);

                if (DateFilter != null)
                {
                    DateFilter.Mode = UCDateRangeFilter.DateFilterMode.Range;
                    DateFilter.FromDate = defaultFrom;
                    DateFilter.ToDate = today;
                    DateFilter.ApplyClicked += async (s, e) => await ApplyFilterAsync();
                }
            }
            catch
            {
            }

            if (lblTopCourtsViewAll != null)
            {
                lblTopCourtsViewAll.Click += (s, e) => NavigateToDatLich();

                _topCourtsViewAllNormalFont = lblTopCourtsViewAll.Font;
                if (_topCourtsViewAllNormalFont != null)
                {
                    _topCourtsViewAllUnderlineFont = (_topCourtsViewAllNormalFont.Style & FontStyle.Underline) != 0
                        ? _topCourtsViewAllNormalFont
                        : new Font(_topCourtsViewAllNormalFont, _topCourtsViewAllNormalFont.Style | FontStyle.Underline);
                }

                lblTopCourtsViewAll.MouseEnter += (s, e) =>
                {
                    if (_topCourtsViewAllUnderlineFont != null)
                    {
                        lblTopCourtsViewAll.Font = _topCourtsViewAllUnderlineFont;
                    }
                };
                lblTopCourtsViewAll.MouseLeave += (s, e) =>
                {
                    if (_topCourtsViewAllNormalFont != null)
                    {
                        lblTopCourtsViewAll.Font = _topCourtsViewAllNormalFont;
                    }
                };

                Disposed += (s, e) =>
                {
                    try
                    {
                        if (_topCourtsViewAllUnderlineFont != null && !ReferenceEquals(_topCourtsViewAllUnderlineFont, _topCourtsViewAllNormalFont))
                        {
                            _topCourtsViewAllUnderlineFont.Dispose();
                        }
                    }
                    catch
                    {
                    }
                };
            }

            if (lstTopCourts != null)
            {
                lstTopCourts.DoubleClick += (s, e) => NavigateToDatLich();
            }

            _ = ReloadAsync();
        }

        private void SetupTopCourtsTable()
        {
            try
            {
                if (lstTopCourts == null)
                {
                    return;
                }

                lstTopCourts.Columns.Clear();
                lstTopCourts.Columns.Add("San", 280);
                lstTopCourts.Columns.Add("Loai", 140);
                lstTopCourts.Columns.Add("Gio cao diem", 150);
                lstTopCourts.Columns.Add("Do kin lich", 140);
                lstTopCourts.Columns.Add("Ty le huy", 120);
                lstTopCourts.Columns.Add("Doanh thu", 180);
            }
            catch
            {
            }
        }

        private void NavigateToDatLich()
        {
            var main = FindForm() as FrmChinh;
            if (main != null)
            {
                main.NavigateToDatLich();
                return;
            }

            try
            {
                using (var frm = new FrmDatSanCoDinh(FrmDatSanCoDinh.BookingMode.Quick, null, DateTime.Today, DateTime.Now))
                {
                    frm.ShowDialog();
                }
            }
            catch
            {
            }
        }

        private void SetupChartStyle()
        {
            try
            {
                if (chartTrend != null && chartTrend.ChartAreas.Count > 0)
                {
                    var area = chartTrend.ChartAreas[0];
                    area.BackColor = Color.Transparent;
                    area.AxisX.MajorGrid.Enabled = false;
                    area.AxisY.MajorGrid.LineColor = Color.FromArgb(232, 234, 237);
                    area.AxisX.LabelStyle.ForeColor = Color.FromArgb(107, 114, 128);
                    area.AxisY.LabelStyle.ForeColor = Color.FromArgb(107, 114, 128);
                    area.AxisY.LabelStyle.Format = "#,0";
                    area.AxisY.Minimum = 0;
                }

                if (chartTrend != null && chartTrend.Series.Count > 0)
                {
                    var series = chartTrend.Series[0];
                    series.ChartType = SeriesChartType.Column;
                    series.BorderWidth = 1;
                    series.BorderColor = Color.FromArgb(34, 94, 32);
                    series.Color = Color.FromArgb(76, 175, 80);
                    series.IsValueShownAsLabel = true;
                    series.LabelForeColor = Color.FromArgb(75, 85, 99);
                    series["PointWidth"] = "0.62";
                }

                if (chartPie != null && chartPie.Series.Count > 0)
                {
                    chartPie.BackColor = Color.Transparent;
                    chartPie.Series[0]["DoughnutRadius"] = "68";
                    chartPie.Series[0].IsValueShownAsLabel = true;
                    chartPie.Series[0].LabelForeColor = Color.FromArgb(55, 65, 81);
                }
            }
            catch
            {
            }
        }

        private async Task ApplyFilterAsync()
        {
            if (DateFilter != null && !DateFilter.ValidateRange(out var err))
            {
                MessageBox.Show(err ?? "Khoang thoi gian khong hop le.", "Khoang thoi gian khong hop le", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await ReloadAsync(GetFilterFromDate(), GetFilterToDate());
        }

        public void RefreshOnActivated()
        {
            _ = ReloadAsync();
        }

        private async Task ReloadAsync()
        {
            await ReloadAsync(GetFilterFromDate(), GetFilterToDate());
        }

        private async Task ReloadAsync(DateTime fromDateInclusive, DateTime toDateInclusive)
        {
            DateTime fromStart = fromDateInclusive.Date;
            DateTime toExclusive = toDateInclusive.Date.AddDays(1);
            int days = (int)(toDateInclusive.Date - fromDateInclusive.Date).TotalDays + 1;

            if (DateFilter != null)
            {
                DateFilter.ApplyEnabled = false;
            }

            try
            {
                var topTask = _reportService.GetTopCourtsAsync(fromStart, toExclusive);
                var kpiTask = _reportService.GetKpisAsync(fromStart, toExclusive, days);
                var heatmapTask = _reportService.GetBookingHourHeatmapAsync(fromStart, toExclusive);
                var opsTask = _reportService.GetBookingOpsAsync(fromStart, toExclusive);
                var pieTask = _reportService.GetTopCourtsRevenueAsync(fromStart, toExclusive);

                await Task.WhenAll(topTask, kpiTask, heatmapTask, opsTask, pieTask);

                BindTopCourts(topTask.Result ?? new List<TopCourtModel>());
                BindRevenueCard(kpiTask.Result ?? new ReportKpiModel());
                BindOperations(heatmapTask.Result ?? new List<ReportHeatmapPointModel>(), opsTask.Result ?? new ReportBookingOpsModel());
                BindOutcomePie(opsTask.Result ?? new ReportBookingOpsModel(), pieTask.Result ?? new List<NamedRevenueModel>());
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("Report Dashboard Error", ex, "UCBaoCao.ReloadAsync");
            }
            finally
            {
                if (DateFilter != null)
                {
                    DateFilter.ApplyEnabled = true;
                }
            }
        }

        private void BindTopCourts(IReadOnlyCollection<TopCourtModel> topCourts)
        {
            lblTopCourtsTitle.Text = "Top san theo khung gio";

            if (lstTopCourts == null)
            {
                return;
            }

            lstTopCourts.Items.Clear();
            foreach (var court in topCourts)
            {
                var item = new ListViewItem(new[]
                {
                    "   " + (court.CourtId ?? string.Empty) + "   " + (court.Name ?? string.Empty),
                    court.Type ?? string.Empty,
                    court.PeakSlot ?? "-",
                    court.Occupancy ?? "0%",
                    court.CancelRate ?? "0.0%",
                    court.Revenue ?? "0d"
                });

                if (!string.IsNullOrWhiteSpace(court.CancelRate))
                {
                    decimal cancelRate;
                    if (decimal.TryParse(court.CancelRate.Replace("%", string.Empty), NumberStyles.Any, CultureInfo.InvariantCulture, out cancelRate) && cancelRate >= 20m)
                    {
                        item.ForeColor = Color.FromArgb(220, 38, 38);
                    }
                }

                lstTopCourts.Items.Add(item);
            }
        }

        private void BindRevenueCard(ReportKpiModel kpi)
        {
            decimal currRev = kpi?.CurrRev ?? 0m;
            decimal prevRev = kpi?.PrevRev ?? 0m;

            lblC1Title.Text = "Tổng doanh thu";
            lblC1Value.Text = currRev == 0m ? "0d" : currRev.ToString("N0", CultureInfo.CurrentCulture) + "d";
            ApplyBadgePercent(lblC1Badge, currRev, prevRev);
        }

        private void BindOperations(IReadOnlyCollection<ReportHeatmapPointModel> heatmap, ReportBookingOpsModel ops)
        {
            var points = heatmap ?? Array.Empty<ReportHeatmapPointModel>();
            var peakPoint = points.OrderByDescending(x => x.BookingCount).ThenBy(x => x.Hour).FirstOrDefault();

            lblC2Title.Text = "Khung giờ cao điểm";
            lblC2Value.Text = peakPoint == null || peakPoint.BookingCount <= 0 ? "-" : peakPoint.Label;
            lblC2Badge.Text = peakPoint == null ? "0 lượt đặt" : peakPoint.BookingCount.ToString("N0") + " lượt đặt";
            lblC2Badge.ForeColor = KpiPositiveColor;

            decimal riskRate = 0m;
            if (ops != null && ops.TotalBookings > 0)
            {
                riskRate = ((ops.CancelledBookings + ops.ShiftedBookings) * 100m) / ops.TotalBookings;
            }

            lblC3Title.Text = "Hủy / Đổi ca";
            lblC3Value.Text = riskRate.ToString("0.0", CultureInfo.CurrentCulture) + "%";
            lblC3Badge.Text = "Hủy " + (ops?.CancelledBookings ?? 0) + " | Đổi " + (ops?.ShiftedBookings ?? 0);
            lblC3Badge.ForeColor = riskRate >= 15m ? KpiNegativeColor : KpiNeutralColor;

            BindHeatmap(points);
        }

        private void BindHeatmap(IReadOnlyCollection<ReportHeatmapPointModel> heatmap)
        {
            lblTrendTitle.Text = "Heatmap gio dat san";

            if (chartTrend == null || chartTrend.Series.Count == 0)
            {
                return;
            }

            var points = heatmap ?? Array.Empty<ReportHeatmapPointModel>();
            int maxValue = points.Any() ? points.Max(x => x.BookingCount) : 0;
            var series = chartTrend.Series[0];
            series.Points.Clear();

            foreach (var point in points)
            {
                int pointIndex = series.Points.AddXY(point.Label, point.BookingCount);
                var chartPoint = series.Points[pointIndex];
                chartPoint.Color = BuildHeatColor(point.BookingCount, maxValue);
                chartPoint.Label = point.BookingCount > 0 ? point.BookingCount.ToString(CultureInfo.CurrentCulture) : string.Empty;
            }
        }

        private void BindOutcomePie(ReportBookingOpsModel ops, IReadOnlyCollection<NamedRevenueModel> pie)
        {
            lblPieTitle.Text = "Ty le huy / doi ca";

            if (chartPie == null || chartPie.Series.Count == 0)
            {
                return;
            }

            var series = chartPie.Series[0];
            series.Points.Clear();

            int active = ops?.ActiveBookings ?? 0;
            int cancelled = ops?.CancelledBookings ?? 0;
            int shifted = ops?.ShiftedBookings ?? 0;

            if (active == 0 && cancelled == 0 && shifted == 0)
            {
                var emptyIndex = series.Points.AddXY("Chua co du lieu", 1);
                series.Points[emptyIndex].Color = Color.LightGray;
                return;
            }

            AddPiePoint(series, "Giu lich", active, Color.FromArgb(34, 197, 94));
            AddPiePoint(series, "Huy", cancelled, Color.FromArgb(239, 68, 68));
            AddPiePoint(series, "Doi ca", shifted, Color.FromArgb(245, 158, 11));
        }

        private static void AddPiePoint(Series series, string label, int value, Color color)
        {
            if (series == null || value <= 0)
            {
                return;
            }

            int pointIndex = series.Points.AddXY(label, value);
            series.Points[pointIndex].Color = color;
            series.Points[pointIndex].Label = value.ToString(CultureInfo.CurrentCulture);
        }

        private DateTime GetFilterFromDate()
        {
            return dateFilter == null ? DateTime.Today.AddDays(-6) : dateFilter.FromDate.Date;
        }

        private DateTime GetFilterToDate()
        {
            return dateFilter == null ? DateTime.Today : dateFilter.ToDate.Date;
        }

        private static Color BuildHeatColor(int value, int maxValue)
        {
            if (value <= 0 || maxValue <= 0)
            {
                return Color.FromArgb(209, 250, 229);
            }

            double ratio = Math.Max(0.15, Math.Min(1.0, (double)value / maxValue));
            int red = (int)(209 - (ratio * 120));
            int green = (int)(250 - (ratio * 70));
            int blue = (int)(229 - (ratio * 180));
            return Color.FromArgb(Math.Max(34, red), Math.Max(120, green), Math.Max(55, blue));
        }

        private static void ApplyBadgePercent(Label badgeLabel, decimal currentValue, decimal previousValue)
        {
            if (badgeLabel == null)
            {
                return;
            }

            decimal changePercent;
            if (previousValue == 0)
            {
                changePercent = currentValue == 0 ? 0 : 100;
            }
            else
            {
                changePercent = (currentValue - previousValue) * 100m / previousValue;
            }

            SetBadge(badgeLabel, changePercent);
        }

        private static void SetBadge(Label badgeLabel, decimal signedValue)
        {
            const string upArrow = "↗";
            const string downArrow = "↘";

            decimal rounded = Math.Round(signedValue, 1);
            if (rounded > 0)
            {
                badgeLabel.ForeColor = KpiPositiveColor;
                badgeLabel.Text = "+" + rounded.ToString("0.0", CultureInfo.CurrentCulture) + "% " + upArrow;
            }
            else if (rounded < 0)
            {
                badgeLabel.ForeColor = KpiNegativeColor;
                badgeLabel.Text = rounded.ToString("0.0", CultureInfo.CurrentCulture) + "% " + downArrow;
            }
            else
            {
                badgeLabel.ForeColor = KpiNeutralColor;
                badgeLabel.Text = "0.0%";
            }
        }
    }
}
