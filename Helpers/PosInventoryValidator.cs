using DemoPick.Helpers;
using DemoPick.Data;
using DemoPick.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;

namespace DemoPick.Helpers
{
    internal static class PosInventoryValidator
    {
        // Lay du lieu/ket qua cho Load Product Categories tu tang xu ly phu hop.
        internal static Dictionary<int, string> LoadProductCategories(SqlConnection conn, SqlTransaction tran, IReadOnlyList<CartLine> lines)
        {
            var result = new Dictionary<int, string>();
            if (lines == null || lines.Count == 0) return result;

            var ids = new List<int>();
            var seen = new HashSet<int>();

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line == null) continue;
                if (line.ProductId <= 0) continue;
                if (!string.IsNullOrWhiteSpace(line.Category)) continue;

                if (seen.Add(line.ProductId))
                {
                    ids.Add(line.ProductId);
                }
            }

            if (ids.Count == 0) return result;

            var sb = new StringBuilder();
            sb.Append("SELECT ProductID, Category FROM dbo.Products WHERE ProductID IN (");

            var parameters = new SqlParameter[ids.Count];
            for (int i = 0; i < ids.Count; i++)
            {
                if (i > 0) sb.Append(", ");
                string p = "@P" + i;
                sb.Append(p);
                parameters[i] = new SqlParameter(p, ids[i]);
            }
            sb.Append(");");

            DataTable dt = DatabaseHelper.ExecuteQuery(conn, tran, sb.ToString(), parameters);
            foreach (DataRow row in dt.Rows)
            {
                if (row["ProductID"] == DBNull.Value) continue;

                int id = Convert.ToInt32(row["ProductID"]);
                string cat = row["Category"] == DBNull.Value ? null : Convert.ToString(row["Category"]);
                result[id] = cat;
            }

            return result;
        }

        // Kiem tra dieu kien Is Service Category va tra ve ket qua dung/sai cho luong xu ly.
        internal static bool IsServiceCategory(string category)
        {
            return string.Equals(NormalizeCategoryKey(category), "dichvu", StringComparison.Ordinal);
        }

        // Dam bao dieu kien Ensure Stock And Maybe Reduce da san sang truoc khi chay buoc xu ly tiep theo.
        internal static void EnsureStockAndMaybeReduce(SqlConnection conn, SqlTransaction tran, int productId, int qty, string prodName, bool hasReduceStockTrigger)
        {
            if (qty <= 0) return;

            if (hasReduceStockTrigger)
            {
                object stockObj = DatabaseHelper.ExecuteScalar(
                    conn,
                    tran,
                    "SELECT StockQuantity FROM Products WITH (UPDLOCK, ROWLOCK) WHERE ProductID = @ProductID",
                    new SqlParameter("@ProductID", productId)
                );

                if (stockObj == null || stockObj == DBNull.Value)
                    throw new InvalidOperationException("Không tìm thấy sản phẩm: " + prodName);

                int stock = Convert.ToInt32(stockObj);
                if (stock < qty)
                    throw new InvalidOperationException("Sản phẩm '" + prodName + "' không đủ hàng (còn " + stock + ", cần " + qty + ").");

                return; // Trigger will reduce after InvoiceDetails insert
            }

            int affected = DatabaseHelper.ExecuteNonQuery(
                conn,
                tran,
                "UPDATE Products SET StockQuantity = StockQuantity - @Qty WHERE ProductID = @ProductID AND StockQuantity >= @Qty",
                new SqlParameter("@Qty", qty),
                new SqlParameter("@ProductID", productId)
            );

            if (affected == 0)
                throw new InvalidOperationException("Sản phẩm '" + prodName + "' không đủ hàng để trừ kho.");
        }

        // Ap dung hoac chuan hoa trang thai Normalize Category Key de du lieu/giao dien nhat quan.
        private static string NormalizeCategoryKey(string category)
        {
            if (string.IsNullOrWhiteSpace(category)) return string.Empty;

            string s = category.Trim();
            s = RemoveDiacritics(s);
            s = s.Replace(" ", string.Empty);
            return s.ToLowerInvariant();
        }

        // Xoa, huy hoac dat lai du lieu Remove Diacritics theo dung dieu kien nghiep vu.
        private static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            string normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(normalized.Length);

            for (int i = 0; i < normalized.Length; i++)
            {
                char c = normalized[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}


