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
            string query = SqlQueries.Report.TopCourts;

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
                var dt = DatabaseHelper.ExecuteQuery(
                    SqlQueries.Report.Kpis,
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
                var dtTrend = DatabaseHelper.ExecuteQuery(
                    SqlQueries.Report.Trend,
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
                var dtPie = DatabaseHelper.ExecuteQuery(
                    SqlQueries.Report.TopCourtsRevenue,
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
