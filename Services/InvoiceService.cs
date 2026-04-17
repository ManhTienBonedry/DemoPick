using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DemoPick.Services
{
    public static class InvoiceService
    {
        public sealed class InvoiceHeader
        {
            public int InvoiceID { get; set; }
            public DateTime CreatedAt { get; set; }
            public int? MemberID { get; set; }
            public string MemberName { get; set; }
            public string PaymentMethod { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal DiscountAmount { get; set; }
            public decimal FinalAmount { get; set; }
            public DateTime? BookingStartTime { get; set; }
            public DateTime? BookingEndTime { get; set; }
        }

        public sealed class InvoiceLine
        {
            public string ItemName { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal LineTotal { get; set; }
        }

        public sealed class InvoiceHistoryItem
        {
            public int InvoiceID { get; set; }
            public DateTime CreatedAt { get; set; }
            public string CustomerName { get; set; }
            public string CourtName { get; set; }
            public decimal FinalAmount { get; set; }
            public string PaymentMethod { get; set; }
        }

        public static InvoiceHeader GetInvoiceHeader(int invoiceId)
        {
            return GetInvoiceHeader(invoiceId, null);
        }

        public static InvoiceHeader GetInvoiceHeader(int invoiceId, string courtName)
        {
            var dt = DatabaseHelper.ExecuteQuery(
                SqlQueries.Invoice.InvoiceHeader,
                new SqlParameter("@InvoiceID", invoiceId),
                new SqlParameter("@CourtName", (object)(courtName ?? "") ?? DBNull.Value)
            );

            if (dt.Rows.Count == 0)
                throw new InvalidOperationException($"Không tìm thấy hóa đơn #{invoiceId}.");

            DataRow r = dt.Rows[0];

            return new InvoiceHeader
            {
                InvoiceID = Convert.ToInt32(r["InvoiceID"]),
                CreatedAt = Convert.ToDateTime(r["CreatedAt"]),
                MemberID = r["MemberID"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["MemberID"]),
                MemberName = r["MemberName"] == DBNull.Value ? "" : r["MemberName"].ToString(),
                PaymentMethod = r["PaymentMethod"] == DBNull.Value ? "" : r["PaymentMethod"].ToString(),
                TotalAmount = Convert.ToDecimal(r["TotalAmount"]),
                DiscountAmount = Convert.ToDecimal(r["DiscountAmount"]),
                FinalAmount = Convert.ToDecimal(r["FinalAmount"]),
                BookingStartTime = r.Table.Columns.Contains("BookingStartTime") && r["BookingStartTime"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(r["BookingStartTime"]) : null,
                BookingEndTime = r.Table.Columns.Contains("BookingEndTime") && r["BookingEndTime"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(r["BookingEndTime"]) : null
            };
        }

        public static List<InvoiceLine> GetInvoiceLines(int invoiceId)
        {
            var list = new List<InvoiceLine>();
            var dt = DatabaseHelper.ExecuteQuery(
                SqlQueries.Invoice.InvoiceLines,
                new SqlParameter("@InvoiceID", invoiceId)
            );

            foreach (DataRow r in dt.Rows)
            {
                list.Add(new InvoiceLine
                {
                    ItemName = r["ItemName"] == DBNull.Value ? "" : r["ItemName"].ToString(),
                    Quantity = Convert.ToInt32(r["Quantity"]),
                    UnitPrice = Convert.ToDecimal(r["UnitPrice"]),
                    LineTotal = Convert.ToDecimal(r["LineTotal"])
                });
            }

            return list;
        }

        public static List<InvoiceHistoryItem> GetInvoiceHistory(int take, string keyword)
        {
            if (take <= 0) take = 50;

            var list = new List<InvoiceHistoryItem>();
            var dt = DatabaseHelper.ExecuteQuery(
                SqlQueries.Invoice.InvoiceHistory,
                new SqlParameter("@Take", take),
                new SqlParameter("@Keyword", string.IsNullOrWhiteSpace(keyword) ? (object)DBNull.Value : keyword.Trim())
            );

            foreach (DataRow r in dt.Rows)
            {
                list.Add(new InvoiceHistoryItem
                {
                    InvoiceID = Convert.ToInt32(r["InvoiceID"]),
                    CreatedAt = Convert.ToDateTime(r["CreatedAt"]),
                    CustomerName = r["CustomerName"] == DBNull.Value ? "Khách lẻ" : r["CustomerName"].ToString(),
                    CourtName = r["CourtName"] == DBNull.Value ? string.Empty : r["CourtName"].ToString(),
                    FinalAmount = r["FinalAmount"] == DBNull.Value ? 0m : Convert.ToDecimal(r["FinalAmount"]),
                    PaymentMethod = r["PaymentMethod"] == DBNull.Value ? string.Empty : r["PaymentMethod"].ToString()
                });
            }

            return list;
        }
    }
}
