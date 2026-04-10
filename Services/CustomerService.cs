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
            string query = "SELECT MemberID, FullName, Phone, TotalHoursPurchased, IsFixed, TotalSpent, CreatedAt FROM Members ORDER BY CreatedAt DESC";

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
                var dt = DatabaseHelper.ExecuteQuery(@"
                    SELECT 
                        SUM(CASE WHEN IsFixed = 1 THEN 1 ELSE 0 END) as CntFixed,
                        SUM(CASE WHEN IsFixed = 0 OR IsFixed IS NULL THEN 1 ELSE 0 END) as CntWalkin
                    FROM Members
                ");

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
                var dt = DatabaseHelper.ExecuteQuery("SELECT COUNT(*) AS Cnt, ISNULL(SUM(TotalSpent), 0) as Rev FROM Members");
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
                object occObj = DatabaseHelper.ExecuteScalar(@"
                    DECLARE @total INT = (SELECT COUNT(*) * 18 FROM Courts WHERE (Status = 'Active' OR Status IS NULL OR LTRIM(RTRIM(Status)) = ''));
                    DECLARE @booked DECIMAL(18,2) = (
                        SELECT ISNULL(SUM(DATEDIFF(minute, StartTime, EndTime)/60.0),0)
                        FROM Bookings
                        WHERE CAST(StartTime as DATE) = CAST(GETDATE() as DATE)
                          AND Status != 'Cancelled'
                          AND Status != 'Maintenance'
                    );
                    SELECT CASE WHEN @total = 0 THEN 0 ELSE CAST((@booked * 100.0 / @total) AS INT) END;
                ");

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
                    "SELECT TOP 1 MemberID, FullName, Tier, IsFixed FROM Members WHERE Phone = @Phone OR CAST(MemberID as VARCHAR(20)) = @Qid",
                    new SqlParameter("@Phone", phone),
                    new SqlParameter("@Qid", qid)
                );

                if (dt.Rows.Count <= 0) return null;

                var row = dt.Rows[0];
                bool isFixed = false;
                if (dt.Columns.Contains("IsFixed") && row["IsFixed"] != DBNull.Value)
                {
                    try { isFixed = Convert.ToBoolean(row["IsFixed"]); }
                    catch { isFixed = false; }
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
