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
                var dt = DatabaseHelper.ExecuteQuery(SqlQueries.Dashboard.Metrics);

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
                var dtTrend = DatabaseHelper.ExecuteQuery(SqlQueries.Dashboard.RevenueTrendLast7Days);

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
                var dtPie = DatabaseHelper.ExecuteQuery(SqlQueries.Dashboard.TopCourtsRevenue);

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
                var dt = DatabaseHelper.ExecuteQuery(SqlQueries.Dashboard.RecentActivity, new SqlParameter("@Take", take));

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
