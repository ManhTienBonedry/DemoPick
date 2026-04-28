using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DemoPick.Services
{
    public static class InvoiceService
    {
        private const string InvoiceHistoryQuery = @"
SELECT TOP (@Take)
    i.InvoiceID,
    i.CreatedAt,
    ISNULL(NULLIF(LTRIM(RTRIM(m.FullName)), ''), N'Khách lẻ') AS CustomerName,
    ISNULL(ca.BookingID, 0) AS BookingID,
    ISNULL(ca.CourtName, N'') AS CourtName,
    i.FinalAmount,
    ISNULL(i.PaymentMethod, N'') AS PaymentMethod
FROM dbo.Invoices i
LEFT JOIN dbo.Members m ON m.MemberID = i.MemberID
OUTER APPLY (
    SELECT TOP (1)
        b.BookingID,
        c.Name AS CourtName
    FROM dbo.InvoiceDetails d
    LEFT JOIN dbo.Bookings b ON b.BookingID = d.BookingID
    LEFT JOIN dbo.Courts c ON c.CourtID = b.CourtID
    WHERE d.InvoiceID = i.InvoiceID
      AND d.BookingID IS NOT NULL
    ORDER BY d.DetailID ASC
) ca
WHERE (
    (@MemberID IS NULL OR i.MemberID = @MemberID)
    AND
    (
        @Keyword IS NULL
        OR LTRIM(RTRIM(@Keyword)) = ''
    OR CAST(i.InvoiceID AS NVARCHAR(20)) LIKE N'%' + @Keyword + N'%'
    OR ISNULL(m.FullName, N'') LIKE N'%' + @Keyword + N'%'
    OR ISNULL(m.Phone, N'') LIKE N'%' + @Keyword + N'%'
    OR ISNULL(ca.CourtName, N'') LIKE N'%' + @Keyword + N'%'
    OR CAST(ISNULL(ca.BookingID, 0) AS NVARCHAR(20)) LIKE N'%' + @Keyword + N'%'
    )
)
ORDER BY i.CreatedAt DESC, i.InvoiceID DESC";

        private const string ShiftReconciliationQuery = @"
SELECT
    COUNT(*) AS InvoiceCount,
    ISNULL(SUM(i.FinalAmount), 0) AS TotalAmount,
    ISNULL(SUM(CASE WHEN ISNULL(i.PaymentMethod, N'') = N'Cash' THEN i.FinalAmount ELSE 0 END), 0) AS CashAmount,
    ISNULL(SUM(CASE WHEN ISNULL(i.PaymentMethod, N'') IN (N'Bank', N'Banking', N'Transfer') THEN i.FinalAmount ELSE 0 END), 0) AS BankAmount,
    SUM(CASE WHEN EXISTS (
        SELECT 1
        FROM dbo.InvoiceDetails d
        WHERE d.InvoiceID = i.InvoiceID
          AND d.BookingID IS NOT NULL
    ) THEN 1 ELSE 0 END) AS BookingLinkedInvoices,
    SUM(CASE WHEN EXISTS (
        SELECT 1
        FROM dbo.InvoiceDetails d
        WHERE d.InvoiceID = i.InvoiceID
          AND d.BookingID IS NOT NULL
    ) THEN 0 ELSE 1 END) AS PosOnlyInvoices
FROM dbo.Invoices i
WHERE i.CreatedAt >= @From
  AND i.CreatedAt < @To";

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
            public int BookingID { get; set; }
            public string CourtName { get; set; }
            public decimal FinalAmount { get; set; }
            public string PaymentMethod { get; set; }
        }

        public sealed class ShiftReconciliationSummary
        {
            public DateTime ShiftStart { get; set; }
            public DateTime ShiftEnd { get; set; }
            public int InvoiceCount { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal CashAmount { get; set; }
            public decimal BankAmount { get; set; }
            public int BookingLinkedInvoices { get; set; }
            public int PosOnlyInvoices { get; set; }
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
                new SqlParameter("@CourtName", (object)(courtName ?? string.Empty) ?? DBNull.Value)
            );

            if (dt.Rows.Count == 0)
            {
                throw new InvalidOperationException("Khong tim thay hoa don #" + invoiceId + ".");
            }

            DataRow r = dt.Rows[0];

            return new InvoiceHeader
            {
                InvoiceID = Convert.ToInt32(r["InvoiceID"]),
                CreatedAt = Convert.ToDateTime(r["CreatedAt"]),
                MemberID = r["MemberID"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["MemberID"]),
                MemberName = r["MemberName"] == DBNull.Value ? string.Empty : r["MemberName"].ToString(),
                PaymentMethod = r["PaymentMethod"] == DBNull.Value ? string.Empty : r["PaymentMethod"].ToString(),
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
                    ItemName = r["ItemName"] == DBNull.Value ? string.Empty : r["ItemName"].ToString(),
                    Quantity = Convert.ToInt32(r["Quantity"]),
                    UnitPrice = Convert.ToDecimal(r["UnitPrice"]),
                    LineTotal = Convert.ToDecimal(r["LineTotal"])
                });
            }

            return list;
        }

        public static List<InvoiceHistoryItem> GetInvoiceHistory(int take, string keyword, int? memberId = null)
        {
            if (take <= 0)
            {
                take = 50;
            }

            var list = new List<InvoiceHistoryItem>();
            var dt = DatabaseHelper.ExecuteQuery(
                InvoiceHistoryQuery,
                new SqlParameter("@Take", take),
                new SqlParameter("@Keyword", string.IsNullOrWhiteSpace(keyword) ? (object)DBNull.Value : keyword.Trim()),
                new SqlParameter("@MemberID", memberId.HasValue && memberId.Value > 0 ? (object)memberId.Value : DBNull.Value)
            );

            foreach (DataRow r in dt.Rows)
            {
                list.Add(new InvoiceHistoryItem
                {
                    InvoiceID = Convert.ToInt32(r["InvoiceID"]),
                    CreatedAt = Convert.ToDateTime(r["CreatedAt"]),
                    CustomerName = r["CustomerName"] == DBNull.Value ? "Khach le" : r["CustomerName"].ToString(),
                    BookingID = r["BookingID"] == DBNull.Value ? 0 : Convert.ToInt32(r["BookingID"]),
                    CourtName = r["CourtName"] == DBNull.Value ? string.Empty : r["CourtName"].ToString(),
                    FinalAmount = r["FinalAmount"] == DBNull.Value ? 0m : Convert.ToDecimal(r["FinalAmount"]),
                    PaymentMethod = r["PaymentMethod"] == DBNull.Value ? string.Empty : r["PaymentMethod"].ToString()
                });
            }

            return list;
        }

        public static ShiftReconciliationSummary GetCurrentShiftSummary()
        {
            DateTime now = DateTime.Now;
            var window = GetShiftWindow(now);

            var dt = DatabaseHelper.ExecuteQuery(
                ShiftReconciliationQuery,
                new SqlParameter("@From", window.Item1),
                new SqlParameter("@To", now)
            );

            if (dt.Rows.Count == 0)
            {
                return new ShiftReconciliationSummary
                {
                    ShiftStart = window.Item1,
                    ShiftEnd = now
                };
            }

            var row = dt.Rows[0];
            return new ShiftReconciliationSummary
            {
                ShiftStart = window.Item1,
                ShiftEnd = now,
                InvoiceCount = row["InvoiceCount"] == DBNull.Value ? 0 : Convert.ToInt32(row["InvoiceCount"]),
                TotalAmount = row["TotalAmount"] == DBNull.Value ? 0m : Convert.ToDecimal(row["TotalAmount"]),
                CashAmount = row["CashAmount"] == DBNull.Value ? 0m : Convert.ToDecimal(row["CashAmount"]),
                BankAmount = row["BankAmount"] == DBNull.Value ? 0m : Convert.ToDecimal(row["BankAmount"]),
                BookingLinkedInvoices = row["BookingLinkedInvoices"] == DBNull.Value ? 0 : Convert.ToInt32(row["BookingLinkedInvoices"]),
                PosOnlyInvoices = row["PosOnlyInvoices"] == DBNull.Value ? 0 : Convert.ToInt32(row["PosOnlyInvoices"])
            };
        }

        private static Tuple<DateTime, DateTime> GetShiftWindow(DateTime now)
        {
            DateTime start = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0);
            if (now < start)
            {
                start = start.AddDays(-1);
            }

            return Tuple.Create(start, start.AddDays(1));
        }
    }
}
