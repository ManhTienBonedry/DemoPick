using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DemoPick.Models;

namespace DemoPick.Services
{
    public class CustomerService
    {
        public async Task<List<CustomerModel>> GetAllCustomersAsync()
        {
            var list = new List<CustomerModel>();
            string query = SqlQueries.Customer.AllCustomers;

            await Task.Run(() => {
                var dt = DatabaseHelper.ExecuteQuery(query);
                foreach (DataRow row in dt.Rows)
                {
                    bool isFixed = row.Table.Columns.Contains("IsFixed") && row["IsFixed"] != DBNull.Value && Convert.ToBoolean(row["IsFixed"]);
                    decimal hours = row.Table.Columns.Contains("TotalHoursPurchased") && row["TotalHoursPurchased"] != DBNull.Value ? Convert.ToDecimal(row["TotalHoursPurchased"]) : 0m;
                    string type = isFixed ? "Cố định" : "Vãng lai";
                    DateTime created = row.Table.Columns.Contains("CreatedAt") && row["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(row["CreatedAt"]) : DateTime.MinValue;

                    list.Add(new CustomerModel
                    {
                        Id = "#PB" + row["MemberID"].ToString(),
                        Name = row["FullName"].ToString(),
                        Phone = row["Phone"].ToString(),
                        CustomerType = type,
                        TotalHours = hours,
                        TotalSpent = Convert.ToDecimal(row["TotalSpent"]).ToString("N0") + "đ",
                        CreatedAt = created
                    });
                }
            });
            
            return list;
        }

        public async Task<CustomerTierCountsModel> GetTierCountsAsync()
        {
            return await Task.Run(() =>
            {
                var dt = DatabaseHelper.ExecuteQuery(SqlQueries.Customer.TierCounts);

                if (dt.Rows.Count <= 0)
                    return new CustomerTierCountsModel();

                var row = dt.Rows[0];
                return new CustomerTierCountsModel
                {
                    FixedCount = row["CntFixed"] == DBNull.Value ? 0 : Convert.ToInt32(row["CntFixed"]),
                    WalkinCount = row["CntWalkin"] == DBNull.Value ? 0 : Convert.ToInt32(row["CntWalkin"])
                };
            });
        }

        public async Task<CustomerRevenueSummaryModel> GetRevenueSummaryAsync()
        {
            return await Task.Run(() =>
            {
                var dt = DatabaseHelper.ExecuteQuery(SqlQueries.Customer.RevenueSummary);
                if (dt.Rows.Count <= 0)
                    return new CustomerRevenueSummaryModel();

                var row = dt.Rows[0];
                return new CustomerRevenueSummaryModel
                {
                    MemberCount = row["Cnt"] == DBNull.Value ? 0 : Convert.ToInt32(row["Cnt"]),
                    Revenue = row["Rev"] == DBNull.Value ? 0m : Convert.ToDecimal(row["Rev"])
                };
            });
        }

        public async Task<int> GetTodayOccupancyPctAsync()
        {
            return await Task.Run(() =>
            {
                object occObj = DatabaseHelper.ExecuteScalar(SqlQueries.Customer.TodayOccupancyPct);

                if (occObj == null || occObj == DBNull.Value) return 0;
                return Convert.ToInt32(occObj);
            });
        }

        public async Task<CheckoutCustomerModel> FindCheckoutCustomerAsync(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return null;

            string phone = search.Trim();
            string qid = phone.Replace("#PB", "").Trim();

            return await Task.Run(() =>
            {
                var dt = DatabaseHelper.ExecuteQuery(
                    SqlQueries.Customer.FindCheckoutCustomer,
                    new SqlParameter("@Phone", phone),
                    new SqlParameter("@Qid", qid)
                );

                if (dt.Rows.Count <= 0) return null;

                var row = dt.Rows[0];
                bool isFixed = false;
                if (dt.Columns.Contains("IsFixed") && row["IsFixed"] != DBNull.Value)
                {
                    object raw = row["IsFixed"];
                    if (raw is bool b)
                    {
                        isFixed = b;
                    }
                    else if (raw is byte by)
                    {
                        isFixed = by != 0;
                    }
                    else if (raw is short sh)
                    {
                        isFixed = sh != 0;
                    }
                    else if (raw is int i)
                    {
                        isFixed = i != 0;
                    }
                    else if (raw is long l)
                    {
                        isFixed = l != 0;
                    }
                    else if (raw is decimal dec)
                    {
                        isFixed = dec != 0m;
                    }
                    else if (raw != null)
                    {
                        string s = raw.ToString();
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            s = s.Trim();
                            if (bool.TryParse(s, out bool parsedBool))
                                isFixed = parsedBool;
                            else if (int.TryParse(s, out int parsedInt))
                                isFixed = parsedInt != 0;
                        }
                    }
                }

                return new CheckoutCustomerModel
                {
                    MemberId = row["MemberID"] == DBNull.Value ? 0 : Convert.ToInt32(row["MemberID"]),
                    FullName = row["FullName"]?.ToString() ?? "",
                    Tier = row["Tier"] == DBNull.Value ? "" : (row["Tier"]?.ToString() ?? ""),
                    IsFixed = isFixed
                };
            });
        }
    }
}
