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
            category = (category ?? "").Trim();

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
                    SqlQueries.Inventory.ProductCategories);
                foreach (DataRow row in dt.Rows)
                {
                    string cat = row[0]?.ToString();
                    if (string.IsNullOrWhiteSpace(cat)) continue;
                    list.Add(cat.Trim());
                }
                return list;
            });
        }

        public async Task<ProductDeleteResult> DeleteProductAsync(int productId)
        {
            if (productId <= 0)
            {
                return new ProductDeleteResult { Success = false, Message = "Mã sản phẩm không hợp lệ." };
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
                        return new ProductDeleteResult { Success = false, Message = "Không tìm thấy sản phẩm để xóa." };
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
                            Message = "Sản phẩm đã có lịch sử bán hàng nên không thể xóa."
                        };
                    }

                    int rows = DatabaseHelper.ExecuteNonQuery(
                        SqlQueries.Inventory.DeleteProductById,
                        new SqlParameter("@ProductID", productId));

                    if (rows <= 0)
                    {
                        return new ProductDeleteResult { Success = false, Message = "Xóa sản phẩm thất bại." };
                    }

                    DatabaseHelper.TryLog(
                        "Xóa Sản phẩm Kho",
                        $"-{productName} (ID: {productId})");

                    return new ProductDeleteResult { Success = true, Message = "Đã xóa sản phẩm thành công." };
                }
                catch (Exception ex)
                {
                    DatabaseHelper.TryLog("Delete Product Error", ex, "InventoryService.DeleteProductAsync");
                    return new ProductDeleteResult { Success = false, Message = "Không thể xóa sản phẩm lúc này." };
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
            string query = SqlQueries.Inventory.InventoryItems;

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
                        ProductId = row["ProductID"] == DBNull.Value ? 0 : Convert.ToInt32(row["ProductID"]),
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
            string query = SqlQueries.Inventory.RecentTransactions;

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

                    string eventRaw = row["EventDesc"]?.ToString() ?? "";
                    string subRaw = row["SubDesc"] != DBNull.Value ? (row["SubDesc"]?.ToString() ?? "") : "";

                    // Skip orphan POS logs (typically from automated tests) that reference an invoice
                    // which no longer exists. These confuse the Inventory UI.
                    if (string.Equals(eventRaw?.Trim(), "POS Checkout", StringComparison.OrdinalIgnoreCase))
                    {
                        int invoiceId = TryExtractInvoiceIdFromPosCheckoutSubDesc(subRaw);
                        if (invoiceId > 0 && !InvoiceExists(invoiceId))
                        {
                            continue;
                        }
                    }

                    string eventUi = MapEventForUi(eventRaw);
                    string subUi = FormatSubDescForUi(eventRaw, subRaw);

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

        private static bool InvoiceExists(int invoiceId)
        {
            if (invoiceId <= 0) return false;
            try
            {
                object obj = DatabaseHelper.ExecuteScalar(
                    SqlQueries.Inventory.InvoiceExistsCount,
                    new SqlParameter("@Id", invoiceId));
                int count = obj == null || obj == DBNull.Value ? 0 : Convert.ToInt32(obj);
                return count > 0;
            }
            catch
            {
                return true; // Fail open: do not hide log if we cannot verify.
            }
        }

        private static int TryExtractInvoiceIdFromPosCheckoutSubDesc(string subDesc)
        {
            if (string.IsNullOrWhiteSpace(subDesc)) return 0;
            string s = subDesc.Trim();

            // New format: "HĐ #12 • ..."
            int hash = s.IndexOf('#');
            if (hash >= 0)
            {
                int i = hash + 1;
                while (i < s.Length && char.IsWhiteSpace(s[i])) i++;
                int j = i;
                while (j < s.Length && char.IsDigit(s[j])) j++;
                if (j > i && int.TryParse(s.Substring(i, j - i), out int idFromHash))
                    return idFromHash;
            }

            // Legacy debug format: "InvoiceID=12; ..."
            const string key = "InvoiceID=";
            int idx = s.IndexOf(key, StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                int i = idx + key.Length;
                while (i < s.Length && char.IsWhiteSpace(s[i])) i++;
                int j = i;
                while (j < s.Length && char.IsDigit(s[j])) j++;
                if (j > i && int.TryParse(s.Substring(i, j - i), out int idFromKey))
                    return idFromKey;
            }

            return 0;
        }

        private static string MapEventForUi(string eventDesc)
        {
            string e = (eventDesc ?? "").Trim();
            if (string.Equals(e, "POS Checkout", StringComparison.OrdinalIgnoreCase)) return "Bán hàng (POS)";
            if (string.Equals(e, "Nhập Kho Trực Tiếp", StringComparison.OrdinalIgnoreCase)) return "Nhập kho";
            return e;
        }

        private static string FormatSubDescForUi(string eventDesc, string subDesc)
        {
            string e = (eventDesc ?? "").Trim();
            string sub = (subDesc ?? "").Replace("\r", " ").Replace("\n", " ").Trim();
            if (sub.Length == 0) return string.Empty;

            if (string.Equals(e, "POS Checkout", StringComparison.OrdinalIgnoreCase))
            {
                string formatted = TryFormatPosCheckoutSubDesc(sub);
                return string.IsNullOrWhiteSpace(formatted) ? sub : formatted;
            }

            return sub;
        }

        private static string TryFormatPosCheckoutSubDesc(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;

            // New logs are already user-friendly (e.g., "HĐ #10 • ...").
            if (raw.IndexOf("InvoiceID=", StringComparison.OrdinalIgnoreCase) < 0)
            {
                return raw;
            }

            var kv = ParseKeyValuePairs(raw);

            int invoiceId = TryGetInt(kv, "InvoiceID");
            string court = TryGetString(kv, "Court");
            string total = TryGetString(kv, "Total");
            string method = TryGetString(kv, "Method");
            string methodUi = ToPaymentMethodDisplay(method);

            var parts = new List<string>();
            if (invoiceId > 0) parts.Add($"HĐ #{invoiceId}");

            if (!string.IsNullOrWhiteSpace(court) && court != "-") parts.Add(court);
            if (!string.IsNullOrWhiteSpace(total) && total != "-") parts.Add(total);
            if (!string.IsNullOrWhiteSpace(methodUi)) parts.Add(methodUi);

            return parts.Count == 0 ? raw : string.Join(" • ", parts);
        }

        private static Dictionary<string, string> ParseKeyValuePairs(string raw)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(raw)) return dict;

            string[] parts = raw.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                string p = (parts[i] ?? "").Trim();
                if (p.Length == 0) continue;

                int eq = p.IndexOf('=');
                if (eq <= 0) continue;

                string key = p.Substring(0, eq).Trim();
                string val = p.Substring(eq + 1).Trim();
                if (key.Length == 0) continue;

                dict[key] = val;
            }

            return dict;
        }

        private static int TryGetInt(Dictionary<string, string> kv, string key)
        {
            if (kv == null || string.IsNullOrWhiteSpace(key)) return 0;
            if (!kv.TryGetValue(key, out var raw)) return 0;
            if (string.IsNullOrWhiteSpace(raw)) return 0;
            return int.TryParse(raw.Trim(), out int val) ? val : 0;
        }

        private static string TryGetString(Dictionary<string, string> kv, string key)
        {
            if (kv == null || string.IsNullOrWhiteSpace(key)) return string.Empty;
            if (!kv.TryGetValue(key, out var raw)) return string.Empty;
            return (raw ?? "").Trim();
        }

        private static string ToPaymentMethodDisplay(string paymentMethod)
        {
            string s = (paymentMethod ?? "").Trim();
            if (s.Length == 0) return string.Empty;
            if (string.Equals(s, "Cash", StringComparison.OrdinalIgnoreCase)) return "Tiền mặt";
            if (string.Equals(s, "Bank", StringComparison.OrdinalIgnoreCase)) return "Chuyển khoản";
            if (string.Equals(s, "Transfer", StringComparison.OrdinalIgnoreCase)) return "Chuyển khoản";
            return s;
        }
    }
}
