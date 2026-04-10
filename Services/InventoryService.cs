using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DemoPick.Models;

namespace DemoPick.Services
{
    public class InventoryService
    {
        public async Task AddProductAsync(string sku, string name, string category, decimal price, int stockQuantity, int minThreshold)
        {
            if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("SKU is required.", nameof(sku));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
            if (price <= 0) throw new ArgumentOutOfRangeException(nameof(price), "Price must be > 0.");
            if (stockQuantity <= 0) throw new ArgumentOutOfRangeException(nameof(stockQuantity), "StockQuantity must be > 0.");
            if (minThreshold < 0) minThreshold = 0;

            sku = sku.Trim();
            name = name.Trim();
            category = (category ?? "").Trim();

            await Task.Run(() =>
            {
                const string insertSql = @"
INSERT INTO Products (SKU, Name, Category, Price, StockQuantity, MinThreshold)
VALUES (@SKU, @Name, @Category, @Price, @StockQuantity, @MinThreshold)";

                DatabaseHelper.ExecuteNonQuery(
                    insertSql,
                    new SqlParameter("@SKU", sku),
                    new SqlParameter("@Name", name),
                    new SqlParameter("@Category", category),
                    new SqlParameter("@Price", price),
                    new SqlParameter("@StockQuantity", stockQuantity),
                    new SqlParameter("@MinThreshold", minThreshold)
                );

                DatabaseHelper.ExecuteNonQuery(
                    "INSERT INTO SystemLogs (EventDesc, SubDesc) VALUES (@EventDesc, @SubDesc)",
                    new SqlParameter("@EventDesc", "Nhập Kho Trực Tiếp"),
                    new SqlParameter("@SubDesc", $"+{stockQuantity} {name}")
                );
            });
        }

        public async Task<List<string>> GetProductCategoriesAsync()
        {
            return await Task.Run(() =>
            {
                var list = new List<string>();
                var dt = DatabaseHelper.ExecuteQuery(
                    "SELECT DISTINCT Category FROM Products WHERE Category IS NOT NULL AND LTRIM(RTRIM(Category)) <> '' ORDER BY Category");
                foreach (DataRow row in dt.Rows)
                {
                    string cat = row[0]?.ToString();
                    if (string.IsNullOrWhiteSpace(cat)) continue;
                    list.Add(cat.Trim());
                }
                return list;
            });
        }

        public async Task<List<ProductCatalogItemModel>> GetProductsAsync()
        {
            return await Task.Run(() =>
            {
                var list = new List<ProductCatalogItemModel>();
                var dt = DatabaseHelper.ExecuteQuery("SELECT ProductID, Name, Price, Category FROM Products");
                foreach (DataRow row in dt.Rows)
                {
                    int productId = row["ProductID"] == DBNull.Value ? 0 : Convert.ToInt32(row["ProductID"]);
                    string name = row["Name"]?.ToString() ?? "";
                    decimal price = row["Price"] == DBNull.Value ? 0m : Convert.ToDecimal(row["Price"]);
                    string category = row["Category"]?.ToString() ?? "";

                    list.Add(new ProductCatalogItemModel
                    {
                        ProductId = productId,
                        Name = name,
                        Price = price,
                        Category = category
                    });
                }
                return list;
            });
        }

        public async Task<InventoryKpiModel> GetInventoryKpisAsync()
        {
            return await Task.Run(() =>
            {
                var dt = DatabaseHelper.ExecuteQuery(@"
                    SELECT 
                        ISNULL(SUM(Price * StockQuantity), 0) as TotalVal,
                        (SELECT COUNT(*) FROM Products WHERE StockQuantity <= MinThreshold AND Category != N'Dịch vụ đi kèm') as CriticalItems,
                        (SELECT ISNULL(SUM(Quantity), 0) FROM InvoiceDetails) as Sales,
                        (SELECT COUNT(*) FROM Invoices) as InvoicesCount
                    FROM Products WHERE Category != N'Dịch vụ đi kèm'
                ");

                if (dt.Rows.Count <= 0)
                    return new InventoryKpiModel();

                var row = dt.Rows[0];
                return new InventoryKpiModel
                {
                    TotalValue = row["TotalVal"] == DBNull.Value ? 0m : Convert.ToDecimal(row["TotalVal"]),
                    CriticalItems = row["CriticalItems"] == DBNull.Value ? 0 : Convert.ToInt32(row["CriticalItems"]),
                    Sales = row["Sales"] == DBNull.Value ? 0 : Convert.ToInt32(row["Sales"]),
                    InvoicesCount = row["InvoicesCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["InvoicesCount"])
                };
            });
        }

        public async Task<List<InventoryItemModel>> GetInventoryItemsAsync()
        {
            var list = new List<InventoryItemModel>();
            string query = "SELECT SKU, Name, Category, StockQuantity, MinThreshold, Price FROM Products WHERE Category != N'Dịch vụ đi kèm'";

            await Task.Run(() => {
                var dt = DatabaseHelper.ExecuteQuery(query);
                foreach (DataRow row in dt.Rows)
                {
                    int stock = Convert.ToInt32(row["StockQuantity"]);
                    int min = Convert.ToInt32(row["MinThreshold"]);
                    string status = "Healthy";
                    if (stock <= 0) status = "Out of Stock";
                    else if (stock <= min) status = "Critical Low";
                    else if (stock <= min * 2) status = "Warning";

                    list.Add(new InventoryItemModel
                    {
                        Sku = row["SKU"].ToString(),
                        Name = row["Name"].ToString(),
                        Category = row["Category"].ToString(),
                        Stock = $"{stock} / {min * 10}", // Giổ hàng max giả định
                        Status = status,
                        Price = Convert.ToDecimal(row["Price"]).ToString("N0") + "đ"
                    });
                }
            });
            return list;
        }

        public async Task<List<TransactionModel>> GetRecentTransactionsAsync()
        {
            var list = new List<TransactionModel>();
            // Inventory screen should show actual inventory/sales transactions,
            // not generic system error logs from other modules.
            string query = @"
SELECT TOP 10 EventDesc, SubDesc, CreatedAt
FROM dbo.SystemLogs
WHERE EventDesc IN (N'Nhập Kho Trực Tiếp', N'POS Checkout')
ORDER BY CreatedAt DESC";

            await Task.Run(() => {
                var dt = DatabaseHelper.ExecuteQuery(query);
                foreach (DataRow row in dt.Rows)
                {
                    DateTime time = Convert.ToDateTime(row["CreatedAt"]);
                    string timeStr;
                    var span = DateTime.Now - time;
                    if (span.TotalMinutes < 60) timeStr = $"{(int)span.TotalMinutes} phút trước";
                    else if (span.TotalHours < 24) timeStr = $"{(int)span.TotalHours} giờ trước";
                    else timeStr = "Hôm qua";

                    list.Add(new TransactionModel
                    {
                        EventDesc = row["EventDesc"].ToString(),
                        SubDesc = row["SubDesc"] != DBNull.Value ? (row["SubDesc"].ToString() ?? "") : "",
                        Time = timeStr
                    });
                }
            });
            return list;
        }
    }
}
