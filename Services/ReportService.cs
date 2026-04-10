using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DemoPick.Models;

namespace DemoPick.Services
{
    public class ReportService
    {
        public async Task<List<TopCourtModel>> GetTopCourtsAsync()
        {
            // Backward-compatible: default to all time.
            return await GetTopCourtsAsync(null, null);
        }

        public async Task<List<TopCourtModel>> GetTopCourtsAsync(DateTime? fromDateInclusive, DateTime? toDateExclusive)
        {
            var list = new List<TopCourtModel>();
            string query = @"
                SELECT 
                    c.Name as CourtName, 
                    c.CourtType as CourtType,
                    ISNULL(SUM(DATEDIFF(minute, b.StartTime, b.EndTime)), 0) as BookedMinutes,
                    ISNULL(SUM(DATEDIFF(minute, b.StartTime, b.EndTime) / 60.0 * c.HourlyRate), 0) as Revenue
                FROM Courts c
                LEFT JOIN Bookings b 
                    ON c.CourtID = b.CourtID
                    AND b.Status != 'Cancelled'
                    AND b.Status != 'Maintenance'
                    AND (@From IS NULL OR b.StartTime >= @From)
                    AND (@To IS NULL OR b.StartTime < @To)
                GROUP BY c.CourtID, c.Name, c.CourtType
                ORDER BY Revenue DESC";

            await Task.Run(() => {
                var dt = DatabaseHelper.ExecuteQuery(
                    query,
                    new System.Data.SqlClient.SqlParameter("@From", (object)fromDateInclusive ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@To", (object)toDateExclusive ?? DBNull.Value)
                );
                
                // Find max booked minutes for relative scaling up to 100%
                decimal maxMins = 0;
                foreach (DataRow row in dt.Rows)
                {
                    decimal mins = Convert.ToDecimal(row["BookedMinutes"]);
                    if (mins > maxMins) maxMins = mins;
                }

                int rank = 1;
                foreach (DataRow row in dt.Rows)
                {
                    decimal rev = Convert.ToDecimal(row["Revenue"]);
                    decimal mins = Convert.ToDecimal(row["BookedMinutes"]);
                    string name = row["CourtName"].ToString();
                    
                    // Allow fallback to generic string if Type column isn't properly returned by some schemas
                    string type = dt.Columns.Contains("CourtType") ? row["CourtType"].ToString() : "Sân Pickleball";

                    // Relative occupancy: 0 to 100% logic
                    int occPct = maxMins > 0 ? (int)Math.Round((mins / maxMins) * 100) : 0;
                    string visualBar = occPct + "%";

                    list.Add(new TopCourtModel
                    {
                        CourtId = "T" + rank,
                        Name = name,
                        Type = type,
                        Occupancy = visualBar,
                        Revenue = rev == 0 ? "0đ" : rev.ToString("N0") + "đ"
                    });
                    rank++;
                }
            });
            return list;
        }

        public async Task<ReportKpiModel> GetKpisAsync(DateTime fromStart, DateTime toExclusive, int days)
        {
            return await Task.Run(() =>
            {
                var dt = DatabaseHelper.ExecuteQuery(@"
                    DECLARE @currStart DATETIME = @FromStart;
                    DECLARE @currEnd DATETIME = @ToExclusive;
                    DECLARE @prevStart DATETIME = DATEADD(DAY, -@Days, @currStart);
                    DECLARE @prevEnd DATETIME = @currStart;

                    DECLARE @activeCourts INT = (SELECT COUNT(*) FROM Courts WHERE (Status = 'Active' OR Status IS NULL OR LTRIM(RTRIM(Status)) = ''));
                    DECLARE @capacityHours DECIMAL(18,2) = @activeCourts * 18.0 * @Days;

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

                if (dt.Rows.Count <= 0)
                    return new ReportKpiModel();

                var row = dt.Rows[0];
                return new ReportKpiModel
                {
                    CurrRev = row["CurrRev"] == DBNull.Value ? 0m : Convert.ToDecimal(row["CurrRev"]),
                    PrevRev = row["PrevRev"] == DBNull.Value ? 0m : Convert.ToDecimal(row["PrevRev"]),
                    CurrOcc = row["CurrOcc"] == DBNull.Value ? 0m : Convert.ToDecimal(row["CurrOcc"]),
                    PrevOcc = row["PrevOcc"] == DBNull.Value ? 0m : Convert.ToDecimal(row["PrevOcc"]),
                    CurrNewCust = row["CurrNewCust"] == DBNull.Value ? 0 : Convert.ToInt32(row["CurrNewCust"]),
                    PrevNewCust = row["PrevNewCust"] == DBNull.Value ? 0 : Convert.ToInt32(row["PrevNewCust"])
                };
            });
        }

        public async Task<List<TrendPointModel>> GetTrendAsync(DateTime fromStart, DateTime toExclusive, DateTime fromDateInclusive, DateTime toDateInclusive)
        {
            return await Task.Run(() =>
            {
                var list = new List<TrendPointModel>();
                var dtTrend = DatabaseHelper.ExecuteQuery(@"
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
                    new SqlParameter("@FromDate", fromDateInclusive.Date),
                    new SqlParameter("@ToDate", toDateInclusive.Date),
                    new SqlParameter("@FromStart", fromStart),
                    new SqlParameter("@ToExclusive", toExclusive));

                foreach (DataRow r in dtTrend.Rows)
                {
                    list.Add(new TrendPointModel
                    {
                        Label = r["Label"].ToString(),
                        Revenue = r["Revenue"] == DBNull.Value ? 0m : Convert.ToDecimal(r["Revenue"])
                    });
                }

                return list;
            });
        }

        public async Task<List<NamedRevenueModel>> GetTopCourtsRevenueAsync(DateTime fromStart, DateTime toExclusive)
        {
            return await Task.Run(() =>
            {
                var list = new List<NamedRevenueModel>();
                var dtPie = DatabaseHelper.ExecuteQuery(@"
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

                foreach (DataRow r in dtPie.Rows)
                {
                    list.Add(new NamedRevenueModel
                    {
                        Name = r["Name"].ToString(),
                        Revenue = r["Rev"] == DBNull.Value ? 0m : Convert.ToDecimal(r["Rev"])
                    });
                }

                return list;
            });
        }
    }
}
