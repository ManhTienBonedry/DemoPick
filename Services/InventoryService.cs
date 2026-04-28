using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DemoPick.Models;

namespace DemoPick.Services
{
    public class InventoryService
    {
        public sealed class ProductDeleteResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }

        public async Task AddProductAsync(string sku, string name, string category, decimal price, int stockQuantity, int minThreshold)
        {
            if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("SKU is required.", nameof(sku));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
            if (price <= 0) throw new ArgumentOutOfRangeException(nameof(price), "Price must be > 0.");
            if (stockQuantity <= 0) throw new ArgumentOutOfRangeException(nameof(stockQuantity), "StockQuantity must be > 0.");
            if (minThreshold < 0) minThreshold = 0;

            sku = sku.Trim();
            name = name.Trim();
            category = (category ?? string.Empty).Trim();

            await Task.Run(() =>
            {
                DatabaseHelper.ExecuteNonQuery(
                    SqlQueries.Inventory.InsertProduct,
                    new SqlParameter("@SKU", sku),
                    new SqlParameter("@Name", name),
                    new SqlParameter("@Category", category),
                    new SqlParameter("@Price", price),
                    new SqlParameter("@StockQuantity", stockQuantity),
                    new SqlParameter("@MinThreshold", minThreshold)
                );

                DatabaseHelper.ExecuteNonQuery(
                    SqlQueries.Inventory.InsertSystemLog,
                    new SqlParameter("@EventDesc", "Nhập Kho Trực Tiếp"),
                    new SqlParameter("@SubDesc", "+" + stockQuantity + " " + name)
                );
            });
        }

        public async Task<List<string>> GetProductCategoriesAsync()
        {
            return await Task.Run(() =>
            {
                var list = new List<string>();
                var dt = DatabaseHelper.ExecuteQuery(SqlQueries.Inventory.ProductCategories);
                foreach (DataRow row in dt.Rows)
                {
                    string cat = row[0]?.ToString();
                    if (string.IsNullOrWhiteSpace(cat))
                    {
                        continue;
                    }

                    list.Add(cat.Trim());
                }

                return list;
            });
        }

        public async Task<ProductDeleteResult> DeleteProductAsync(int productId)
        {
            if (productId <= 0)
            {
                return new ProductDeleteResult { Success = false, Message = "Ma san pham khong hop le." };
            }

            return await Task.Run(() =>
            {
                try
                {
                    var nameObj = DatabaseHelper.ExecuteScalar(
                        SqlQueries.Inventory.ProductNameById,
                        new SqlParameter("@ProductID", productId));

                    var productName = (nameObj == null || nameObj == DBNull.Value)
                        ? string.Empty
                        : (nameObj.ToString() ?? string.Empty).Trim();

                    if (string.IsNullOrWhiteSpace(productName))
                    {
                        return new ProductDeleteResult { Success = false, Message = "Khong tim thay san pham de xoa." };
                    }

                    var usedObj = DatabaseHelper.ExecuteScalar(
                        SqlQueries.Inventory.InvoiceDetailsCountByProductId,
                        new SqlParameter("@ProductID", productId));
                    int usedCount = usedObj == null || usedObj == DBNull.Value ? 0 : Convert.ToInt32(usedObj);

                    if (usedCount > 0)
                    {
                        return new ProductDeleteResult
                        {
                            Success = false,
                            Message = "San pham da co lich su ban hang nen khong the xoa."
                        };
                    }

                    int rows = DatabaseHelper.ExecuteNonQuery(
                        SqlQueries.Inventory.DeleteProductById,
                        new SqlParameter("@ProductID", productId));

                    if (rows <= 0)
                    {
                        return new ProductDeleteResult { Success = false, Message = "Xoa san pham that bai." };
                    }

                    DatabaseHelper.TryLog("Xóa Sản phẩm Kho", "-" + productName + " (ID: " + productId + ")");
                    return new ProductDeleteResult { Success = true, Message = "Da xoa san pham thanh cong." };
                }
                catch (Exception ex)
                {
                    DatabaseHelper.TryLog("Delete Product Error", ex, "InventoryService.DeleteProductAsync");
                    return new ProductDeleteResult { Success = false, Message = "Khong the xoa san pham luc nay." };
                }
            });
        }

        public async Task<List<ProductCatalogItemModel>> GetProductsAsync()
        {
            return await Task.Run(() =>
            {
                var list = new List<ProductCatalogItemModel>();
                var dt = DatabaseHelper.ExecuteQuery(SqlQueries.Inventory.ProductsCatalog);
                foreach (DataRow row in dt.Rows)
                {
                    int productId = row["ProductID"] == DBNull.Value ? 0 : Convert.ToInt32(row["ProductID"]);
                    string name = row["Name"]?.ToString() ?? string.Empty;
                    decimal price = row["Price"] == DBNull.Value ? 0m : Convert.ToDecimal(row["Price"]);
                    string category = row["Category"]?.ToString() ?? string.Empty;

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

        public async Task<List<ProductDeleteListItemModel>> GetProductsForDeletionAsync()
        {
            return await Task.Run(() =>
            {
                var list = new List<ProductDeleteListItemModel>();
                var dt = DatabaseHelper.ExecuteQuery(SqlQueries.Inventory.ProductsForDeletion);

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new ProductDeleteListItemModel
                    {
                        ProductId = row["ProductID"] == DBNull.Value ? 0 : Convert.ToInt32(row["ProductID"]),
                        Sku = row["SKU"]?.ToString() ?? string.Empty,
                        Name = row["Name"]?.ToString() ?? string.Empty,
                        Category = row["Category"]?.ToString() ?? string.Empty,
                        Price = row["Price"] == DBNull.Value ? 0m : Convert.ToDecimal(row["Price"]),
                        StockQuantity = row["StockQuantity"] == DBNull.Value ? 0 : Convert.ToInt32(row["StockQuantity"])
                    });
                }

                return list;
            });
        }

        public async Task<InventoryKpiModel> GetInventoryKpisAsync()
        {
            return await Task.Run(() =>
            {
                var dt = DatabaseHelper.ExecuteQuery(SqlQueries.Inventory.InventoryKpis);
                if (dt.Rows.Count <= 0)
                {
                    return new InventoryKpiModel();
                }

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

            await Task.Run(() =>
            {
                var dt = DatabaseHelper.ExecuteQuery(SqlQueries.Inventory.InventoryItems);
                foreach (DataRow row in dt.Rows)
                {
                    int stock = row["StockQuantity"] == DBNull.Value ? 0 : Convert.ToInt32(row["StockQuantity"]);
                    int min = row["MinThreshold"] == DBNull.Value ? 0 : Convert.ToInt32(row["MinThreshold"]);
                    int soldLast14Days = row.Table.Columns.Contains("SoldLast14Days") && row["SoldLast14Days"] != DBNull.Value
                        ? Convert.ToInt32(row["SoldLast14Days"])
                        : 0;
                    decimal avgDailySales = Math.Round(soldLast14Days / 14m, 2);
                    int targetStock = BuildTargetStock(min, avgDailySales);
                    int? daysRemaining = avgDailySales > 0m
                        ? Math.Max(0, (int)Math.Floor(stock / avgDailySales))
                        : (int?)null;
                    string status = GetInventoryStatus(stock, min, daysRemaining);
                    int suggestedReorder = Math.Max(0, targetStock - stock);

                    list.Add(new InventoryItemModel
                    {
                        ProductId = row["ProductID"] == DBNull.Value ? 0 : Convert.ToInt32(row["ProductID"]),
                        Sku = row["SKU"]?.ToString() ?? string.Empty,
                        Name = row["Name"]?.ToString() ?? string.Empty,
                        Category = row["Category"]?.ToString() ?? string.Empty,
                        Stock = stock.ToString("N0"),
                        Status = status,
                        Price = (row["Price"] == DBNull.Value ? 0m : Convert.ToDecimal(row["Price"])).ToString("N0") + "d",
                        CurrentStock = stock,
                        MinThreshold = min,
                        SoldLast14Days = soldLast14Days,
                        AvgDailySales = avgDailySales,
                        SuggestedReorderQuantity = suggestedReorder,
                        TargetStockQuantity = targetStock,
                        DaysRemaining = daysRemaining,
                        Recommendation = BuildRecommendation(stock, min, avgDailySales, daysRemaining, suggestedReorder)
                    });
                }
            });

            return list;
        }

        public InventorySmartInsightsModel BuildSmartInsights(IReadOnlyCollection<InventoryItemModel> items)
        {
            var safeItems = items ?? Array.Empty<InventoryItemModel>();
            var insight = new InventorySmartInsightsModel
            {
                OutOfStockCount = safeItems.Count(x => x.CurrentStock <= 0),
                WarningItemsCount = safeItems.Count(x => !string.Equals(x.Status, "Healthy", StringComparison.OrdinalIgnoreCase)),
                SuggestedReorderProductCount = safeItems.Count(x => x.SuggestedReorderQuantity > 0),
                SuggestedReorderUnits = safeItems.Sum(x => x.SuggestedReorderQuantity),
                SoldLast14Days = safeItems.Sum(x => x.SoldLast14Days),
                AvgDailySales = Math.Round(safeItems.Sum(x => x.AvgDailySales), 1)
            };

            for (int day = 1; day <= 7; day++)
            {
                int riskItems = 0;
                foreach (var item in safeItems)
                {
                    if (item.AvgDailySales <= 0m)
                    {
                        if (item.CurrentStock <= item.MinThreshold)
                        {
                            riskItems++;
                        }

                        continue;
                    }

                    decimal projectedStock = item.CurrentStock - (item.AvgDailySales * day);
                    if (projectedStock <= item.MinThreshold)
                    {
                        riskItems++;
                    }
                }

                insight.ForecastPoints.Add(new InventoryForecastPointModel
                {
                    Label = "Ngay " + day,
                    RiskItems = riskItems
                });
            }

            return insight;
        }

        public async Task<List<TransactionModel>> GetRecentTransactionsAsync()
        {
            var list = new List<TransactionModel>();

            await Task.Run(() =>
            {
                var dt = DatabaseHelper.ExecuteQuery(SqlQueries.Inventory.RecentTransactions);
                foreach (DataRow row in dt.Rows)
                {
                    DateTime time = Convert.ToDateTime(row["CreatedAt"]);
                    string timeStr;
                    var span = DateTime.Now - time;
                    if (span.TotalMinutes < 60)
                    {
                        timeStr = ((int)span.TotalMinutes) + " phut truoc";
                    }
                    else if (span.TotalHours < 24)
                    {
                        timeStr = ((int)span.TotalHours) + " gio truoc";
                    }
                    else
                    {
                        timeStr = "Hom qua";
                    }

                    string eventRaw = row["EventDesc"]?.ToString() ?? string.Empty;
                    string subRaw = row["SubDesc"] != DBNull.Value ? (row["SubDesc"]?.ToString() ?? string.Empty) : string.Empty;

                    string eventUi = InventoryTransactionFormatter.MapEventForUi(eventRaw);
                    string subUi = InventoryTransactionFormatter.FormatSubDescForUi(eventRaw, subRaw);

                    list.Add(new TransactionModel
                    {
                        EventDesc = eventUi,
                        SubDesc = subUi,
                        Time = timeStr
                    });
                }
            });

            return list;
        }

        private static int BuildTargetStock(int minThreshold, decimal avgDailySales)
        {
            int forecastDemand = avgDailySales <= 0m ? 0 : (int)Math.Ceiling(avgDailySales * 14m);
            return Math.Max(minThreshold * 2, forecastDemand + minThreshold);
        }

        private static string GetInventoryStatus(int stock, int minThreshold, int? daysRemaining)
        {
            if (stock <= 0)
            {
                return "Out of Stock";
            }

            if (stock <= minThreshold)
            {
                return "Critical Low";
            }

            if (daysRemaining.HasValue && daysRemaining.Value <= 3)
            {
                return "Critical Low";
            }

            if (stock <= minThreshold * 2)
            {
                return "Warning";
            }

            if (daysRemaining.HasValue && daysRemaining.Value <= 7)
            {
                return "Warning";
            }

            return "Healthy";
        }

        private static string BuildRecommendation(int stock, int minThreshold, decimal avgDailySales, int? daysRemaining, int suggestedReorder)
        {
            if (stock <= 0)
            {
                return suggestedReorder > 0 ? "Nhap gap +" + suggestedReorder.ToString("N0") : "Nhap gap";
            }

            bool lowByThreshold = stock <= minThreshold;
            bool lowByVelocity = avgDailySales > 0m && daysRemaining.HasValue && daysRemaining.Value <= 7;
            if (lowByThreshold || lowByVelocity)
            {
                string coverage = daysRemaining.HasValue ? daysRemaining.Value + " ngay" : "ban cham";
                return "Nhap +" + suggestedReorder.ToString("N0") + " / " + coverage;
            }

            if (avgDailySales <= 0m)
            {
                return "Ban cham";
            }

            return "On dinh ~" + Math.Max(1, daysRemaining ?? 0) + " ngay";
        }
    }
}
