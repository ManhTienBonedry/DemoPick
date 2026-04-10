using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DemoPick.Models;

namespace DemoPick.Services
{
    public class DashboardService
    {
        public async Task<DashboardMetricsModel> GetMetricsAsync()
        {
            return await Task.Run(() =>
            {
                var dt = DatabaseHelper.ExecuteQuery(@"
                    DECLARE @posRev DECIMAL(18,2) = ISNULL((SELECT SUM(FinalAmount) FROM Invoices), 0);
                    DECLARE @courtRev DECIMAL(18,2) = ISNULL((
                        SELECT SUM((DATEDIFF(minute, B.StartTime, B.EndTime) / 60.0) * C.HourlyRate)
                        FROM Bookings B
                        JOIN Courts C ON B.CourtID = C.CourtID
                        WHERE B.Status != 'Cancelled'
                          AND B.Status != 'Maintenance'
                    ), 0);
                    DECLARE @totalRev DECIMAL(18,2) = @posRev + @courtRev;
                    DECLARE @totalCust INT = (SELECT COUNT(*) FROM Members);
                    DECLARE @total INT = (SELECT COUNT(*) * 18 FROM Courts WHERE (Status = 'Active' OR Status IS NULL OR LTRIM(RTRIM(Status)) = ''));
                    DECLARE @booked DECIMAL(18,2) = (
                        SELECT ISNULL(SUM(DATEDIFF(minute, StartTime, EndTime) / 60.0), 0)
                        FROM Bookings
                        WHERE CAST(StartTime AS DATE) = CAST(GETDATE() AS DATE)
                          AND Status != 'Cancelled'
                          AND Status != 'Maintenance'
                    );
                    DECLARE @occ INT = CASE WHEN @total = 0 THEN 0 ELSE CAST((@booked * 100.0 / @total) AS INT) END;
                    DECLARE @pos INT = (SELECT COUNT(*) FROM Invoices);
                    
                    SELECT @totalRev as Rev, @totalCust as Cust, @occ as Occ, @pos as POS;
                ");

                if (dt.Rows.Count <= 0)
                    return new DashboardMetricsModel();

                var row = dt.Rows[0];
                return new DashboardMetricsModel
                {
                    Revenue = row["Rev"] == DBNull.Value ? 0m : Convert.ToDecimal(row["Rev"]),
                    CustomerCount = row["Cust"] == DBNull.Value ? 0 : Convert.ToInt32(row["Cust"]),
                    OccupancyPct = row["Occ"] == DBNull.Value ? 0 : Convert.ToInt32(row["Occ"]),
                    PosCount = row["POS"] == DBNull.Value ? 0 : Convert.ToInt32(row["POS"])
                };
            });
        }

        public async Task<List<TrendPointModel>> GetRevenueTrendLast7DaysAsync()
        {
            return await Task.Run(() =>
            {
                var list = new List<TrendPointModel>();
                var dtTrend = DatabaseHelper.ExecuteQuery(@"
                    WITH Last7Days AS (
                        SELECT CAST(GETDATE() - 6 AS DATE) as Dt UNION ALL
                        SELECT CAST(GETDATE() - 5 AS DATE) UNION ALL
                        SELECT CAST(GETDATE() - 4 AS DATE) UNION ALL
                        SELECT CAST(GETDATE() - 3 AS DATE) UNION ALL
                        SELECT CAST(GETDATE() - 2 AS DATE) UNION ALL
                        SELECT CAST(GETDATE() - 1 AS DATE) UNION ALL
                        SELECT CAST(GETDATE() AS DATE)
                    )
                    SELECT 
                        FORMAT(D.Dt, 'dd/MM') as Label,
                        ISNULL(SUM(I.FinalAmount), 0) + ISNULL(SUM(BR.CourtRevenue), 0) as Revenue
                    FROM Last7Days D
                    LEFT JOIN Invoices I ON CAST(I.CreatedAt AS DATE) = D.Dt
                    LEFT JOIN (
                        SELECT
                            CAST(B.StartTime AS DATE) as Dt,
                            SUM((DATEDIFF(minute, B.StartTime, B.EndTime) / 60.0) * C.HourlyRate) as CourtRevenue
                        FROM Bookings B
                        JOIN Courts C ON B.CourtID = C.CourtID
                        WHERE B.Status != 'Cancelled'
                          AND B.Status != 'Maintenance'
                        GROUP BY CAST(B.StartTime AS DATE)
                    ) BR ON BR.Dt = D.Dt
                    GROUP BY D.Dt
                    ORDER BY D.Dt
                ");

                foreach (DataRow row in dtTrend.Rows)
                {
                    list.Add(new TrendPointModel
                    {
                        Label = row["Label"].ToString(),
                        Revenue = row["Revenue"] == DBNull.Value ? 0m : Convert.ToDecimal(row["Revenue"])
                    });
                }

                return list;
            });
        }

        public async Task<List<NamedRevenueModel>> GetTopCourtsRevenueAsync()
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
                    LEFT JOIN Bookings B ON C.CourtID = B.CourtID AND B.Status != 'Cancelled' AND B.Status != 'Maintenance'
                    GROUP BY C.Name
                    ORDER BY Rev DESC
                ");

                foreach (DataRow row in dtPie.Rows)
                {
                    list.Add(new NamedRevenueModel
                    {
                        Name = row["Name"].ToString(),
                        Revenue = row["Rev"] == DBNull.Value ? 0m : Convert.ToDecimal(row["Rev"])
                    });
                }

                return list;
            });
        }

        public async Task<List<DashboardActivityModel>> GetRecentActivityAsync(int take)
        {
            return await Task.Run(() =>
            {
                var list = new List<DashboardActivityModel>();
                var dt = DatabaseHelper.ExecuteQuery(@"
                    SELECT TOP (@Take)
                        '#BK' + CAST(B.BookingID as VARCHAR) as Code,
                        C.Name as CourtName,
                        COALESCE(M.FullName, NULLIF(LTRIM(RTRIM(B.GuestName)), ''), N'Khách lẻ') as CustomerName,
                        FORMAT(B.StartTime, 'dd/MM/yyyy HH:mm') as TimeText,
                        B.Status as Status,
                        CAST(CASE WHEN B.Status = 'Maintenance' THEN 0 ELSE ISNULL((DATEDIFF(minute, B.StartTime, B.EndTime) / 60.0) * C.HourlyRate, 0) END AS DECIMAL(18,2)) as Amount
                    FROM Bookings B
                    JOIN Courts C ON B.CourtID = C.CourtID
                    LEFT JOIN Members M ON B.MemberID = M.MemberID
                    ORDER BY B.StartTime DESC
                ", new SqlParameter("@Take", take));

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new DashboardActivityModel
                    {
                        Code = row["Code"].ToString(),
                        CourtName = row["CourtName"].ToString(),
                        CustomerName = row["CustomerName"].ToString(),
                        TimeText = row["TimeText"].ToString(),
                        Status = row["Status"].ToString(),
                        Amount = row["Amount"] == DBNull.Value ? 0m : Convert.ToDecimal(row["Amount"])
                    });
                }

                return list;
            });
        }
    }
}
