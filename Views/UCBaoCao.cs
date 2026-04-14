using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
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
                // Ignore: design-time / component differences
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

                this.Disposed += (s, e) =>
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
                        // Best effort.
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
                if (lstTopCourts == null) return;

                lstTopCourts.Columns.Clear();
                lstTopCourts.Columns.Add("Sân", 420);
                lstTopCourts.Columns.Add("Loại", 180);
                lstTopCourts.Columns.Add("Tỉ lệ", 180);
                lstTopCourts.Columns.Add("Doanh thu", 200);
            }
            catch
            {
                // ignore
            }
        }

        private void TrySetFilterDates(DateTime from, DateTime to)
        {
            if (dateFilter == null) return;
            dateFilter.FromDate = from.Date;
            dateFilter.ToDate = to.Date;
        }

        private DateTime GetFilterFromDate()
        {
            if (dateFilter == null) return DateTime.Today.AddDays(-6);
            return dateFilter.FromDate.Date;
        }

        private DateTime GetFilterToDate()
        {
            if (dateFilter == null) return DateTime.Today;
            return dateFilter.ToDate.Date;
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
                using (var frm = new FrmDatSan())
                {
                    frm.ShowDialog();
                }
            }
            catch
            {
                // ignore
            }
        }

        private async System.Threading.Tasks.Task ApplyFilterAsync()
        {
            if (DateFilter != null && !DateFilter.ValidateRange(out var err))
            {
                MessageBox.Show(err ?? "Khoảng thời gian không hợp lệ.", "Khoảng thời gian không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var from = GetFilterFromDate();
            var to = GetFilterToDate();

            await ReloadAsync(from, to);
        }

        private async System.Threading.Tasks.Task ReloadAsync()
        {
            var from = GetFilterFromDate();
            var to = GetFilterToDate();
            await ReloadAsync(from, to);
        }

        private async System.Threading.Tasks.Task ReloadAsync(DateTime fromDateInclusive, DateTime toDateInclusive)
        {
            DateTime fromStart = fromDateInclusive.Date;
            DateTime toExclusive = toDateInclusive.Date.AddDays(1);
            int days = (int)(toDateInclusive.Date - fromDateInclusive.Date).TotalDays + 1;

            if (DateFilter != null) DateFilter.ApplyEnabled = false;
            try
            {
                await LoadTopCourtsAsync(fromStart, toExclusive);
                await LoadKpisAsync(fromStart, toExclusive, days);
                await LoadChartsAsync(fromStart, toExclusive, fromStart.Date, toDateInclusive.Date);
            }
            finally
            {
                if (DateFilter != null) DateFilter.ApplyEnabled = true;
            }
        }

        private async System.Threading.Tasks.Task LoadTopCourtsAsync(DateTime fromStart, DateTime toExclusive)
        {
            try
            {
                var topCourts = await _reportService.GetTopCourtsAsync(fromStart, toExclusive);
                lstTopCourts.Items.Clear();
                foreach (var c in topCourts)
                {
                    lstTopCourts.Items.Add(new ListViewItem(new[] { $"   {c.CourtId}   {c.Name}", c.Type, c.Occupancy, c.Revenue }));
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("Report TopCourts Error", ex, "UCBaoCao.ReloadAsync TopCourts");
            }
        }

        private async System.Threading.Tasks.Task LoadKpisAsync(DateTime fromStart, DateTime toExclusive, int days)
        {
            // KPI (selected range) + delta vs previous period (same length)
            try
            {
                var kpi = await _reportService.GetKpisAsync(fromStart, toExclusive, days);

                decimal currRev = kpi?.CurrRev ?? 0m;
                decimal prevRev = kpi?.PrevRev ?? 0m;
                decimal currOcc = kpi?.CurrOcc ?? 0m;
                decimal prevOcc = kpi?.PrevOcc ?? 0m;
                int currNewCust = kpi?.CurrNewCust ?? 0;
                int prevNewCust = kpi?.PrevNewCust ?? 0;

                // Values (current range)
                lblC1Value.Text = currRev == 0 ? "0đ" : currRev.ToString("N0", CultureInfo.CurrentCulture) + "đ";
                lblC2Value.Text = Math.Round(currOcc, 0).ToString(CultureInfo.CurrentCulture) + "%";
                lblC3Value.Text = currNewCust.ToString(CultureInfo.CurrentCulture);

                // Badges (delta vs previous same-length period)
                ApplyBadgePercent(lblC1Badge, currRev, prevRev);
                ApplyBadgeDeltaPoints(lblC2Badge, currOcc, prevOcc);
                ApplyBadgePercent(lblC3Badge, currNewCust, prevNewCust);
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("Report KPI Error", ex, "UCBaoCao.LoadDataAsync KPI");
            }
        }

        private async System.Threading.Tasks.Task LoadChartsAsync(DateTime fromStart, DateTime toExclusive, DateTime fromDateInclusive, DateTime toDateInclusive)
        {
            // Trend + Pie (selected range)
            try
            {
                chartTrend.Series[0].Points.Clear();
                var trend = await _reportService.GetTrendAsync(fromStart, toExclusive, fromDateInclusive, toDateInclusive);
                foreach (var p in trend)
                {
                    chartTrend.Series[0].Points.AddXY(p.Label, p.Revenue);
                }

                chartPie.Series[0].Points.Clear();
                var pie = await _reportService.GetTopCourtsRevenueAsync(fromStart, toExclusive);

                if (pie.Count == 0 || (pie.Count > 0 && pie[0].Revenue == 0))
                {
                    chartPie.Series[0].Points.AddXY("Chưa có D.Thu", 100);
                    chartPie.Series[0].Points[0].Color = Color.LightGray;
                }
                else
                {
                    Color[] colors = { Color.FromArgb(76, 175, 80), Color.FromArgb(129, 199, 132), Color.FromArgb(165, 214, 167), Color.FromArgb(232, 245, 233) };
                    int i = 0;
                    foreach (var s in pie)
                    {
                        if (s.Revenue > 0)
                        {
                            chartPie.Series[0].Points.AddXY(s.Name, s.Revenue);
                            chartPie.Series[0].Points[chartPie.Series[0].Points.Count - 1].Color = colors[i % colors.Length];
                            i++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("Report Charts Error", ex, "UCBaoCao.LoadDataAsync Trend/Pie");
            }
        }

        private static void ApplyBadgePercent(Label badgeLabel, decimal currentValue, decimal previousValue)
        {
            if (badgeLabel == null) return;

            decimal changePercent;
            if (previousValue == 0)
            {
                changePercent = currentValue == 0 ? 0 : 100;
            }
            else
            {
                changePercent = (currentValue - previousValue) * 100 / previousValue;
            }

            SetBadge(badgeLabel, changePercent);
        }

        private static void ApplyBadgePercent(Label badgeLabel, int currentValue, int previousValue)
        {
            ApplyBadgePercent(badgeLabel, (decimal)currentValue, (decimal)previousValue);
        }

        private static void ApplyBadgeDeltaPoints(Label badgeLabel, decimal currentPercent, decimal previousPercent)
        {
            if (badgeLabel == null) return;
            var delta = currentPercent - previousPercent;
            SetBadge(badgeLabel, delta);
        }

        private static void SetBadge(Label badgeLabel, decimal signedValue)
        {
            const string upArrow = "↗";
            const string downArrow = "↘";

            var rounded = Math.Round(signedValue, 1);
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
