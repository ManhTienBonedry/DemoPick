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
            return await GetTopCourtsAsync(null, null);
        }

        public async Task<List<TopCourtModel>> GetTopCourtsAsync(DateTime? fromDateInclusive, DateTime? toDateExclusive)
        {
            var list = new List<TopCourtModel>();

            await Task.Run(() =>
            {
                var dt = DatabaseHelper.ExecuteQuery(
                    SqlQueries.Report.TopCourts,
                    new SqlParameter("@From", (object)fromDateInclusive ?? DBNull.Value),
                    new SqlParameter("@To", (object)toDateExclusive ?? DBNull.Value)
                );

                decimal maxMins = 0m;
                foreach (DataRow row in dt.Rows)
                {
                    decimal mins = row["BookedMinutes"] == DBNull.Value ? 0m : Convert.ToDecimal(row["BookedMinutes"]);
                    if (mins > maxMins)
                    {
                        maxMins = mins;
                    }
                }

                int rank = 1;
                foreach (DataRow row in dt.Rows)
                {
                    decimal rev = row["Revenue"] == DBNull.Value ? 0m : Convert.ToDecimal(row["Revenue"]);
                    decimal mins = row["BookedMinutes"] == DBNull.Value ? 0m : Convert.ToDecimal(row["BookedMinutes"]);
                    string name = row["CourtName"]?.ToString() ?? string.Empty;
                    string type = dt.Columns.Contains("CourtType") ? row["CourtType"]?.ToString() ?? "San Pickleball" : "San Pickleball";
                    int peakHour = row["PeakHour"] == DBNull.Value ? -1 : Convert.ToInt32(row["PeakHour"]);
                    decimal cancelRate = row["CancelRate"] == DBNull.Value ? 0m : Convert.ToDecimal(row["CancelRate"]);

                    int occPct = maxMins > 0 ? (int)Math.Round((mins / maxMins) * 100m) : 0;

                    list.Add(new TopCourtModel
                    {
                        CourtId = "T" + rank,
                        Name = name,
                        Type = type,
                        Occupancy = occPct + "%",
                        Revenue = rev == 0m ? "0d" : rev.ToString("N0") + "d",
                        PeakSlot = peakHour < 0 ? "-" : peakHour.ToString("00") + ":00",
                        CancelRate = cancelRate.ToString("0.0") + "%"
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
                var dt = DatabaseHelper.ExecuteQuery(
                    SqlQueries.Report.Kpis,
                    new SqlParameter("@FromStart", fromStart),
                    new SqlParameter("@ToExclusive", toExclusive),
                    new SqlParameter("@Days", days));

                if (dt.Rows.Count <= 0)
                {
                    return new ReportKpiModel();
                }

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
                var dtTrend = DatabaseHelper.ExecuteQuery(
                    SqlQueries.Report.Trend,
                    new SqlParameter("@FromDate", fromDateInclusive.Date),
                    new SqlParameter("@ToDate", toDateInclusive.Date),
                    new SqlParameter("@FromStart", fromStart),
                    new SqlParameter("@ToExclusive", toExclusive));

                foreach (DataRow row in dtTrend.Rows)
                {
                    list.Add(new TrendPointModel
                    {
                        Label = row["Label"]?.ToString() ?? string.Empty,
                        Revenue = row["Revenue"] == DBNull.Value ? 0m : Convert.ToDecimal(row["Revenue"])
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
                var dtPie = DatabaseHelper.ExecuteQuery(
                    SqlQueries.Report.TopCourtsRevenue,
                    new SqlParameter("@FromStart", fromStart),
                    new SqlParameter("@ToExclusive", toExclusive));

                foreach (DataRow row in dtPie.Rows)
                {
                    list.Add(new NamedRevenueModel
                    {
                        Name = row["Name"]?.ToString() ?? string.Empty,
                        Revenue = row["Rev"] == DBNull.Value ? 0m : Convert.ToDecimal(row["Rev"])
                    });
                }

                return list;
            });
        }

        public async Task<List<ReportHeatmapPointModel>> GetBookingHourHeatmapAsync(DateTime fromStart, DateTime toExclusive)
        {
            return await Task.Run(() =>
            {
                var list = new List<ReportHeatmapPointModel>();
                var dt = DatabaseHelper.ExecuteQuery(
                    SqlQueries.Report.BookingHourHeatmap,
                    new SqlParameter("@FromStart", fromStart),
                    new SqlParameter("@ToExclusive", toExclusive));

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new ReportHeatmapPointModel
                    {
                        Hour = row["Hr"] == DBNull.Value ? 0 : Convert.ToInt32(row["Hr"]),
                        Label = row["Label"]?.ToString() ?? string.Empty,
                        BookingCount = row["BookingCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["BookingCount"])
                    });
                }

                return list;
            });
        }

        public async Task<ReportBookingOpsModel> GetBookingOpsAsync(DateTime fromStart, DateTime toExclusive)
        {
            return await Task.Run(() =>
            {
                var dt = DatabaseHelper.ExecuteQuery(
                    SqlQueries.Report.BookingOps,
                    new SqlParameter("@FromStart", fromStart),
                    new SqlParameter("@ToExclusive", toExclusive));

                if (dt.Rows.Count <= 0)
                {
                    return new ReportBookingOpsModel();
                }

                var row = dt.Rows[0];
                return new ReportBookingOpsModel
                {
                    TotalBookings = row["TotalBookings"] == DBNull.Value ? 0 : Convert.ToInt32(row["TotalBookings"]),
                    CancelledBookings = row["CancelledBookings"] == DBNull.Value ? 0 : Convert.ToInt32(row["CancelledBookings"]),
                    ShiftedBookings = row["ShiftedBookings"] == DBNull.Value ? 0 : Convert.ToInt32(row["ShiftedBookings"]),
                    ActiveBookings = row["ActiveBookings"] == DBNull.Value ? 0 : Convert.ToInt32(row["ActiveBookings"])
                };
            });
        }
    }
}
