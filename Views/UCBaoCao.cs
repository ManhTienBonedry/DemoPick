using System;
using System.Data.SqlClient;
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

            try
            {
                var today = DateTime.Today;
                var defaultFrom = today.AddDays(-6);

                if (DateFilter != null)
                {
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
                lblTopCourtsViewAll.MouseEnter += (s, e) => lblTopCourtsViewAll.Font = new Font(lblTopCourtsViewAll.Font, FontStyle.Underline);
                lblTopCourtsViewAll.MouseLeave += (s, e) => lblTopCourtsViewAll.Font = new Font(lblTopCourtsViewAll.Font, FontStyle.Regular);
            }

            if (lstTopCourts != null)
            {
                lstTopCourts.DoubleClick += (s, e) => NavigateToDatLich();
            }

            _ = ReloadAsync();
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
            var from = DateFilter?.FromDate ?? DateTime.Today.AddDays(-6);
            var to = DateFilter?.ToDate ?? DateTime.Today;

            if (DateFilter != null && !DateFilter.ValidateRange(out var err))
            {
                MessageBox.Show(err, "Khoảng thời gian không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await ReloadAsync(from, to);
        }

        private async System.Threading.Tasks.Task ReloadAsync()
        {
            var from = DateFilter?.FromDate ?? DateTime.Today.AddDays(-6);
            var to = DateFilter?.ToDate ?? DateTime.Today;
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

            // KPI (selected range) + delta vs previous period (same length)
            try
            {
                var dt = DemoPick.Services.DatabaseHelper.ExecuteQuery(@"
                    DECLARE @currStart DATETIME = @FromStart;
                    DECLARE @currEnd DATETIME = @ToExclusive;
                    DECLARE @days INT = @Days;
                    DECLARE @prevStart DATETIME = DATEADD(DAY, -@days, @currStart);
                    DECLARE @prevEnd DATETIME = @currStart;

                    DECLARE @activeCourts INT = (SELECT COUNT(*) FROM Courts WHERE Status='Active');
                    DECLARE @capacityHours DECIMAL(18,2) = @activeCourts * 16.0 * @days;

                    DECLARE @currPosRev DECIMAL(18,2) = ISNULL((SELECT SUM(FinalAmount) FROM Invoices WHERE CreatedAt >= @currStart AND CreatedAt < @currEnd), 0);
                    DECLARE @prevPosRev DECIMAL(18,2) = ISNULL((SELECT SUM(FinalAmount) FROM Invoices WHERE CreatedAt >= @prevStart AND CreatedAt < @prevEnd), 0);

                    DECLARE @currCourtRev DECIMAL(18,2) = ISNULL((
                        SELECT SUM((DATEDIFF(minute, B.StartTime, B.EndTime) / 60.0) * C.HourlyRate)
                        FROM Bookings B
                        JOIN Courts C ON B.CourtID = C.CourtID
                        WHERE B.Status != 'Cancelled'
                          AND B.Status != 'Maintenance'
                          AND B.StartTime >= @currStart AND B.StartTime < @currEnd
                    ), 0);
                    DECLARE @prevCourtRev DECIMAL(18,2) = ISNULL((
                        SELECT SUM((DATEDIFF(minute, B.StartTime, B.EndTime) / 60.0) * C.HourlyRate)
                        FROM Bookings B
                        JOIN Courts C ON B.CourtID = C.CourtID
                        WHERE B.Status != 'Cancelled'
                          AND B.Status != 'Maintenance'
                          AND B.StartTime >= @prevStart AND B.StartTime < @prevEnd
                    ), 0);

                    DECLARE @currRev DECIMAL(18,2) = @currPosRev + @currCourtRev;
                    DECLARE @prevRev DECIMAL(18,2) = @prevPosRev + @prevCourtRev;

                    DECLARE @currBookedHours DECIMAL(18,2) = ISNULL((
                        SELECT SUM(DATEDIFF(minute, StartTime, EndTime) / 60.0)
                        FROM Bookings
                        WHERE Status != 'Cancelled'
                          AND Status != 'Maintenance'
                          AND StartTime >= @currStart AND StartTime < @currEnd
                    ), 0);
                    DECLARE @prevBookedHours DECIMAL(18,2) = ISNULL((
                        SELECT SUM(DATEDIFF(minute, StartTime, EndTime) / 60.0)
                        FROM Bookings
                        WHERE Status != 'Cancelled'
                          AND Status != 'Maintenance'
                          AND StartTime >= @prevStart AND StartTime < @prevEnd
                    ), 0);

                    DECLARE @currOcc DECIMAL(18,2) = CASE WHEN @capacityHours = 0 THEN 0 ELSE (@currBookedHours * 100.0 / @capacityHours) END;
                    DECLARE @prevOcc DECIMAL(18,2) = CASE WHEN @capacityHours = 0 THEN 0 ELSE (@prevBookedHours * 100.0 / @capacityHours) END;

                    ;WITH Activity AS (
                        SELECT MemberID, CreatedAt AS At
                        FROM Invoices
                        WHERE MemberID IS NOT NULL AND CreatedAt < @currEnd
                        UNION ALL
                        SELECT MemberID, StartTime AS At
                        FROM Bookings
                        WHERE MemberID IS NOT NULL AND Status != 'Cancelled' AND Status != 'Maintenance' AND StartTime < @currEnd
                    ), FirstActivity AS (
                        SELECT MemberID, MIN(At) AS FirstAt
                        FROM Activity
                        GROUP BY MemberID
                    )
                    SELECT
                        @currRev AS CurrRev,
                        @prevRev AS PrevRev,
                        @currOcc AS CurrOcc,
                        @prevOcc AS PrevOcc,
                        (SELECT COUNT(*) FROM FirstActivity WHERE FirstAt >= @currStart AND FirstAt < @currEnd) AS CurrNewCust,
                        (SELECT COUNT(*) FROM FirstActivity WHERE FirstAt >= @prevStart AND FirstAt < @prevEnd) AS PrevNewCust;
                ",
                new SqlParameter("@FromStart", fromStart),
                new SqlParameter("@ToExclusive", toExclusive),
                new SqlParameter("@Days", days));
                
                if (dt.Rows.Count > 0)
                {
                    decimal currRev = Convert.ToDecimal(dt.Rows[0]["CurrRev"]);
                    decimal prevRev = Convert.ToDecimal(dt.Rows[0]["PrevRev"]);
                    decimal currOcc = Convert.ToDecimal(dt.Rows[0]["CurrOcc"]);
                    decimal prevOcc = Convert.ToDecimal(dt.Rows[0]["PrevOcc"]);
                    int currNewCust = Convert.ToInt32(dt.Rows[0]["CurrNewCust"]);
                    int prevNewCust = Convert.ToInt32(dt.Rows[0]["PrevNewCust"]);

                    // Values (current 7 days)
                    lblC1Value.Text = currRev == 0 ? "0đ" : currRev.ToString("N0", CultureInfo.CurrentCulture) + "đ";
                    lblC2Value.Text = Math.Round(currOcc, 0).ToString(CultureInfo.CurrentCulture) + "%";
                    lblC3Value.Text = currNewCust.ToString(CultureInfo.CurrentCulture);

                    // Badges (delta vs previous 7 days)
                    ApplyBadgePercent(lblC1Badge, currRev, prevRev);
                    ApplyBadgeDeltaPoints(lblC2Badge, currOcc, prevOcc);
                    ApplyBadgePercent(lblC3Badge, currNewCust, prevNewCust);
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("Report KPI Error", ex, "UCBaoCao.LoadDataAsync KPI");
            }

            // Trend Data - Dynamic (selected range)
            chartTrend.Series[0].Points.Clear();
            try {
                var dtTrend = DemoPick.Services.DatabaseHelper.ExecuteQuery(@"
                    ;WITH Dates AS (
                        SELECT CAST(@FromDate AS DATE) AS Dt
                        UNION ALL
                        SELECT DATEADD(DAY, 1, Dt)
                        FROM Dates
                        WHERE Dt < CAST(@ToDate AS DATE)
                    )
                    SELECT
                        FORMAT(D.Dt, 'dd/MM') as Label,
                        ISNULL(SUM(I.FinalAmount), 0) + ISNULL(SUM(BR.CourtRevenue), 0) as Revenue
                    FROM Dates D
                    LEFT JOIN Invoices I 
                        ON CAST(I.CreatedAt AS DATE) = D.Dt
                        AND I.CreatedAt >= @FromStart AND I.CreatedAt < @ToExclusive
                    LEFT JOIN (
                        SELECT
                            CAST(B.StartTime AS DATE) as Dt,
                            SUM((DATEDIFF(minute, B.StartTime, B.EndTime) / 60.0) * C.HourlyRate) as CourtRevenue
                        FROM Bookings B
                        JOIN Courts C ON B.CourtID = C.CourtID
                        WHERE B.Status != 'Cancelled'
                                                    AND B.Status != 'Maintenance'
                          AND B.StartTime >= @FromStart AND B.StartTime < @ToExclusive
                        GROUP BY CAST(B.StartTime AS DATE)
                    ) BR ON BR.Dt = D.Dt
                    GROUP BY D.Dt
                    ORDER BY D.Dt
                    OPTION (MAXRECURSION 0);
                ",
                new SqlParameter("@FromDate", fromStart.Date),
                new SqlParameter("@ToDate", toDateInclusive.Date),
                new SqlParameter("@FromStart", fromStart),
                new SqlParameter("@ToExclusive", toExclusive));
                foreach (System.Data.DataRow r in dtTrend.Rows)
                {
                    chartTrend.Series[0].Points.AddXY(r["Label"].ToString(), Convert.ToDecimal(r["Revenue"]));
                }

                // Pie Data - Dynamic courts revenue (selected range)
                chartPie.Series[0].Points.Clear();
                var dtPie = DemoPick.Services.DatabaseHelper.ExecuteQuery(@"
                    SELECT TOP 4
                        C.Name,
                        ISNULL(SUM(CASE
                            WHEN B.BookingID IS NULL THEN 0
                            ELSE (DATEDIFF(minute, B.StartTime, B.EndTime) / 60.0) * C.HourlyRate
                        END), 0) as Rev
                    FROM Courts C
                    LEFT JOIN Bookings B 
                        ON C.CourtID = B.CourtID
                        AND B.Status != 'Cancelled'
                        AND B.Status != 'Maintenance'
                        AND B.StartTime >= @FromStart AND B.StartTime < @ToExclusive
                    GROUP BY C.Name
                    ORDER BY Rev DESC
                ",
                new SqlParameter("@FromStart", fromStart),
                new SqlParameter("@ToExclusive", toExclusive));
                
                if (dtPie.Rows.Count == 0 || (dtPie.Rows.Count > 0 && Convert.ToDecimal(dtPie.Rows[0]["Rev"]) == 0))
                {
                    chartPie.Series[0].Points.AddXY("Chưa có D.Thu", 100);
                    chartPie.Series[0].Points[0].Color = Color.LightGray;
                }
                else
                {
                    Color[] colors = { Color.FromArgb(76, 175, 80), Color.FromArgb(129, 199, 132), Color.FromArgb(165, 214, 167), Color.FromArgb(232, 245, 233) };
                    int i = 0;
                    foreach (System.Data.DataRow r in dtPie.Rows)
                    {
                        if (Convert.ToDecimal(r["Rev"]) > 0)
                        {
                            chartPie.Series[0].Points.AddXY(r["Name"].ToString(), Convert.ToDecimal(r["Rev"]));
                            chartPie.Series[0].Points[chartPie.Series[0].Points.Count - 1].Color = colors[i % colors.Length];
                            i++;
                        }
                    }
                }
            } catch (Exception ex)
            {
                DatabaseHelper.TryLog("Report Charts Error", ex, "UCBaoCao.LoadDataAsync Trend/Pie");
            }

            }
            finally
            {
                if (DateFilter != null) DateFilter.ApplyEnabled = true;
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
