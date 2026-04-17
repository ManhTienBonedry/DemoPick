using DemoPick.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;

namespace DemoPick.Services
{
    public class PosService
    {
        // --- PENDING ORDER STATE MANAGEMENT ---
        public static Dictionary<string, List<CartLine>> PendingOrders { get; } = new Dictionary<string, List<CartLine>>(StringComparer.OrdinalIgnoreCase);

        public static void SavePendingOrder(string courtName, List<CartLine> lines)
        {
            if (string.IsNullOrWhiteSpace(courtName)) return;
            PendingOrders[courtName] = lines ?? new List<CartLine>();
        }

        public static List<CartLine> GetPendingOrder(string courtName)
        {
            if (string.IsNullOrWhiteSpace(courtName)) return new List<CartLine>();
            if (PendingOrders.TryGetValue(courtName, out var lines)) return new List<CartLine>(lines);
            return new List<CartLine>();
        }

        public static void ClearPendingOrder(string courtName)
        {
            if (string.IsNullOrWhiteSpace(courtName)) return;
            PendingOrders.Remove(courtName);
        }
        // --------------------------------------

        public int Checkout(
            int memberId,
            IReadOnlyList<CartLine> lines,
            decimal totalAmount,
            decimal discountAmount,
            decimal finalAmount,
            string paymentMethod,
            string courtNameForLog,
            int? preferredBookingId = null)
        {
            // Allow court-only checkouts (no POS products) so we can close bookings and issue invoices.
            if (lines == null)
                lines = Array.Empty<CartLine>();

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (var tran = conn.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        bool hasReduceStockTrigger = TriggerExists(conn, tran, "trg_ReduceStock");
                        bool hasUpdateMemberTrigger = TriggerExists(conn, tran, "trg_UpdateMemberTier");

                        int closedBookingId = 0;
                        int creditedBookingId = 0;

                        if (preferredBookingId.HasValue && preferredBookingId.Value > 0)
                        {
                            creditedBookingId = TrySetSpecificBookingPaid(conn, tran, preferredBookingId.Value);
                            closedBookingId = creditedBookingId;

                            if (creditedBookingId == 0)
                            {
                                throw new InvalidOperationException("Booking đã chọn chưa thể thanh toán hoặc không còn hợp lệ.");
                            }
                        }
                        else
                        {
                            closedBookingId = TryCloseActiveBookingByCourtName(conn, tran, courtNameForLog);
                            creditedBookingId = closedBookingId;

                            if (creditedBookingId == 0)
                            {
                                // Fallback: if booking already ended (EndTime <= now), still credit the member's hours
                                // using the most recent booking for this court (typically "khách chơi trễ" scenario).
                                creditedBookingId = TryMarkMostRecentEndedBookingPaidByCourtName(conn, tran, courtNameForLog);
                            }
                        }

                        int effectiveMemberId = ResolveMemberForCheckout(conn, tran, memberId, creditedBookingId);
                        int invoiceId = InsertInvoice(conn, tran, effectiveMemberId, totalAmount, discountAmount, finalAmount, paymentMethod);

                        var productCategories = LoadProductCategories(conn, tran, lines);

                        // 1) Insert POS product lines only (ProductId > 0)
                        //    Note: Category "Dịch vụ" should NOT reduce stock.
                        foreach (var line in lines)
                        {
                            if (line == null) continue;
                            if (line.ProductId <= 0) continue;
                            if (line.Quantity <= 0) continue;

                            string category = line.Category;
                            if (string.IsNullOrWhiteSpace(category) && productCategories.TryGetValue(line.ProductId, out var dbCat))
                            {
                                category = dbCat;
                            }

                            if (!IsServiceCategory(category))
                            {
                                EnsureStockAndMaybeReduce(conn, tran, line.ProductId, line.Quantity, line.ProductName, hasReduceStockTrigger);
                            }

                            InsertInvoiceDetail(conn, tran, invoiceId, productId: line.ProductId, bookingId: null, qty: line.Quantity, unitPrice: line.UnitPrice);
                        }

                        // 2) Insert court/service lines after we know which booking got credited.
                        // These lines should NOT reduce stock, so ProductID must be NULL.
                        foreach (var line in lines)
                        {
                            if (line == null) continue;
                            if (line.ProductId > 0) continue;
                            if (line.Quantity <= 0) continue;

                            int? bookingId = creditedBookingId > 0 ? (int?)creditedBookingId : null;
                            InsertInvoiceDetail(conn, tran, invoiceId, productId: null, bookingId: bookingId, qty: line.Quantity, unitPrice: line.UnitPrice);
                        }

                        // Reject accidental empty invoices.
                        if (lines.Count == 0 && finalAmount <= 0 && creditedBookingId == 0)
                            throw new InvalidOperationException("Không có dữ liệu thanh toán.");

                        if (effectiveMemberId > 0)
                        {
                            if (!hasUpdateMemberTrigger)
                            {
                                DatabaseHelper.ExecuteNonQuery(
                                    conn,
                                    tran,
                                    "UPDATE Members SET TotalSpent = ISNULL(TotalSpent, 0) + @FinalAmount WHERE MemberID = @MemberID",
                                    new SqlParameter("@FinalAmount", finalAmount),
                                    new SqlParameter("@MemberID", effectiveMemberId)
                                );
                            }

                            if (closedBookingId > 0)
                            {
                                object durationObj = DatabaseHelper.ExecuteScalar(
                                    conn, tran,
                                    "SELECT DATEDIFF(minute, StartTime, EndTime) / 60.0 FROM Bookings WHERE BookingID = @BookingID",
                                    new SqlParameter("@BookingID", closedBookingId)
                                );
                                decimal addedHours = (durationObj != null && durationObj != DBNull.Value) ? Convert.ToDecimal(durationObj) : 0m;
                                
                                if (addedHours > 0)
                                {
                                    DatabaseHelper.ExecuteNonQuery(
                                        conn, tran,
                                        "UPDATE Members SET TotalHoursPurchased = ISNULL(TotalHoursPurchased, 0) + @Hrs WHERE MemberID = @MemberID",
                                        new SqlParameter("@Hrs", addedHours),
                                        new SqlParameter("@MemberID", effectiveMemberId)
                                    );

                                    // Auto-upgrade to Fixed customer if TotalHoursPurchased >= 30
                                    DatabaseHelper.ExecuteNonQuery(
                                        conn, tran,
                                        "UPDATE Members SET IsFixed = 1 WHERE MemberID = @MemberID AND TotalHoursPurchased >= 30 AND IsFixed = 0",
                                        new SqlParameter("@MemberID", effectiveMemberId)
                                    );
                                }
                            }
                            else if (creditedBookingId > 0)
                            {
                                object durationObj = DatabaseHelper.ExecuteScalar(
                                    conn, tran,
                                    "SELECT DATEDIFF(minute, StartTime, EndTime) / 60.0 FROM Bookings WHERE BookingID = @BookingID",
                                    new SqlParameter("@BookingID", creditedBookingId)
                                );
                                decimal addedHours = (durationObj != null && durationObj != DBNull.Value) ? Convert.ToDecimal(durationObj) : 0m;

                                if (addedHours > 0)
                                {
                                    DatabaseHelper.ExecuteNonQuery(
                                        conn, tran,
                                        "UPDATE Members SET TotalHoursPurchased = ISNULL(TotalHoursPurchased, 0) + @Hrs WHERE MemberID = @MemberID",
                                        new SqlParameter("@Hrs", addedHours),
                                        new SqlParameter("@MemberID", effectiveMemberId)
                                    );

                                    DatabaseHelper.ExecuteNonQuery(
                                        conn, tran,
                                        "UPDATE Members SET IsFixed = 1 WHERE MemberID = @MemberID AND TotalHoursPurchased >= 30 AND IsFixed = 0",
                                        new SqlParameter("@MemberID", effectiveMemberId)
                                    );
                                }
                            }
                        }

                        if (!IsTestMode())
                        {
                            DatabaseHelper.ExecuteNonQuery(
                                conn,
                                tran,
                                "INSERT INTO SystemLogs (EventDesc, SubDesc) VALUES (@EventDesc, @SubDesc)",
                                new SqlParameter("@EventDesc", "POS Checkout"),
                                new SqlParameter("@SubDesc", BuildPosCheckoutLogSubDesc(invoiceId, courtNameForLog, finalAmount, paymentMethod, lines))
                            );
                        }

                        tran.Commit();
                        return invoiceId;
                    }
                    catch
                    {
                        try { tran.Rollback(); }
                        catch (Exception rbEx)
                        {
                            DatabaseHelper.TryLog("POS Rollback Error", rbEx, "PosService.Checkout");
                        }
                        throw;
                    }
                }
            }
        }

        private static bool IsTestMode()
        {
            try
            {
                string v = Environment.GetEnvironmentVariable("DEMOPICK_TEST_MODE");
                if (string.IsNullOrWhiteSpace(v)) return false;
                v = v.Trim();
                return v == "1" || v.Equals("true", StringComparison.OrdinalIgnoreCase) || v.Equals("yes", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private static string BuildPosCheckoutLogSubDesc(
            int invoiceId,
            string courtName,
            decimal finalAmount,
            string paymentMethod,
            IReadOnlyList<CartLine> lines)
        {
            var parts = new List<string>();
            if (invoiceId > 0) parts.Add($"HĐ #{invoiceId}");

            string court = (courtName ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(court))
            {
                parts.Add(court);
            }

            string items = BuildPosProductSummary(lines);
            if (!string.IsNullOrWhiteSpace(items))
            {
                parts.Add(items);
            }

            if (finalAmount > 0)
            {
                parts.Add(finalAmount.ToString("N0") + "đ");
            }

            string methodText = ToPaymentMethodDisplay(paymentMethod);
            if (!string.IsNullOrWhiteSpace(methodText))
            {
                parts.Add(methodText);
            }

            return parts.Count == 0 ? string.Empty : string.Join(" • ", parts);
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

        private static string BuildPosProductSummary(IReadOnlyList<CartLine> lines)
        {
            if (lines == null || lines.Count == 0) return string.Empty;

            var byName = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line == null) continue;
                if (line.ProductId <= 0) continue; // only stock items
                if (line.Quantity <= 0) continue;

                string name = (line.ProductName ?? "").Trim();
                if (string.IsNullOrWhiteSpace(name)) continue;

                if (byName.ContainsKey(name)) byName[name] += line.Quantity;
                else byName.Add(name, line.Quantity);
            }

            if (byName.Count == 0) return string.Empty;

            var items = new List<KeyValuePair<string, int>>(byName);
            items.Sort((a, b) => b.Value.CompareTo(a.Value));

            const int maxItemsToShow = 2;
            var sb = new StringBuilder();
            int shown = 0;

            for (int i = 0; i < items.Count && shown < maxItemsToShow; i++)
            {
                string name = items[i].Key ?? "";
                int qty = items[i].Value;
                if (qty <= 0) continue;

                if (sb.Length > 0) sb.Append(", ");
                sb.Append(TruncateForUi(name, 18));
                sb.Append(" x");
                sb.Append(qty);
                shown++;
            }

            int remaining = items.Count - shown;
            if (remaining > 0)
            {
                sb.Append(" +");
                sb.Append(remaining);
            }

            return sb.ToString();
        }

        private static string TruncateForUi(string text, int maxLen)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            if (maxLen <= 0) return string.Empty;
            if (text.Length <= maxLen) return text;
            if (maxLen == 1) return "…";
            return text.Substring(0, maxLen - 1) + "…";
        }

        private static int ResolveMemberForCheckout(SqlConnection conn, SqlTransaction tran, int memberId, int creditedBookingId)
        {
            if (memberId > 0) return memberId;
            if (creditedBookingId <= 0) return 0;

            object existingMemberObj = DatabaseHelper.ExecuteScalar(
                conn,
                tran,
                "SELECT MemberID FROM dbo.Bookings WHERE BookingID = @BookingID",
                new SqlParameter("@BookingID", creditedBookingId)
            );

            if (existingMemberObj != null && existingMemberObj != DBNull.Value)
            {
                int bookingMemberId = Convert.ToInt32(existingMemberObj);
                if (bookingMemberId > 0) return bookingMemberId;
            }

            string guestNameRaw = Convert.ToString(
                DatabaseHelper.ExecuteScalar(
                    conn,
                    tran,
                    "SELECT GuestName FROM dbo.Bookings WHERE BookingID = @BookingID",
                    new SqlParameter("@BookingID", creditedBookingId)
                )
            ) ?? "";

            ParseGuestInfo(guestNameRaw, out var fullName, out var phone);
            if (string.IsNullOrWhiteSpace(phone)) return 0;

            object existingByPhoneObj = DatabaseHelper.ExecuteScalar(
                conn,
                tran,
                "SELECT TOP (1) MemberID FROM dbo.Members WITH (UPDLOCK, HOLDLOCK) WHERE Phone = @Phone ORDER BY MemberID DESC",
                new SqlParameter("@Phone", phone)
            );

            int resolvedMemberId;

            if (existingByPhoneObj != null && existingByPhoneObj != DBNull.Value)
            {
                resolvedMemberId = Convert.ToInt32(existingByPhoneObj);

                if (!string.IsNullOrWhiteSpace(fullName))
                {
                    DatabaseHelper.ExecuteNonQuery(
                        conn,
                        tran,
                        "UPDATE dbo.Members SET FullName = @FullName WHERE MemberID = @MemberID AND (FullName IS NULL OR LTRIM(RTRIM(FullName)) = '')",
                        new SqlParameter("@FullName", fullName),
                        new SqlParameter("@MemberID", resolvedMemberId)
                    );
                }
            }
            else
            {
                object insertedObj = DatabaseHelper.ExecuteScalar(
                    conn,
                    tran,
                                        SqlQueries.Pos.InsertWalkinMemberReturnId,
                    new SqlParameter("@FullName", string.IsNullOrWhiteSpace(fullName) ? "Khach le" : fullName),
                    new SqlParameter("@Phone", phone)
                );

                if (insertedObj == null || insertedObj == DBNull.Value)
                    return 0;

                resolvedMemberId = Convert.ToInt32(insertedObj);
            }

            DatabaseHelper.ExecuteNonQuery(
                conn,
                tran,
                "UPDATE dbo.Bookings SET MemberID = @MemberID WHERE BookingID = @BookingID AND (MemberID IS NULL OR MemberID = 0)",
                new SqlParameter("@MemberID", resolvedMemberId),
                new SqlParameter("@BookingID", creditedBookingId)
            );

            return resolvedMemberId;
        }

        private static void ParseGuestInfo(string guestNameRaw, out string fullName, out string phone)
        {
            fullName = "";
            phone = "";

            string raw = (guestNameRaw ?? "").Trim();
            if (raw.Length == 0) return;

            int sep = raw.LastIndexOf(" - ", StringComparison.Ordinal);
            if (sep >= 0)
            {
                fullName = raw.Substring(0, sep).Trim();
                phone = raw.Substring(sep + 3).Trim();
            }
            else
            {
                int dash = raw.LastIndexOf('-');
                if (dash > 0 && dash < raw.Length - 1)
                {
                    fullName = raw.Substring(0, dash).Trim();
                    phone = raw.Substring(dash + 1).Trim();
                }
                else
                {
                    fullName = raw;
                }
            }

            phone = NormalizePhone(phone);

            if (string.IsNullOrWhiteSpace(fullName) && !string.IsNullOrWhiteSpace(phone))
                fullName = "Khach " + phone;
        }

        private static string NormalizePhone(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";

            var sb = new StringBuilder(input.Length);
            foreach (char c in input)
            {
                if (char.IsDigit(c)) sb.Append(c);
            }

            return sb.ToString();
        }

        private static Dictionary<int, string> LoadProductCategories(SqlConnection conn, SqlTransaction tran, IReadOnlyList<CartLine> lines)
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

        private static bool IsServiceCategory(string category)
        {
            return string.Equals(NormalizeCategoryKey(category), "dichvu", StringComparison.Ordinal);
        }

        private static string NormalizeCategoryKey(string category)
        {
            if (string.IsNullOrWhiteSpace(category)) return "";

            string s = category.Trim();
            s = RemoveDiacritics(s);
            s = s.Replace(" ", "");
            return s.ToLowerInvariant();
        }

        private static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";

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

        private static bool TriggerExists(SqlConnection conn, SqlTransaction tran, string triggerName)
        {
            if (string.IsNullOrWhiteSpace(triggerName)) return false;

            try
            {
                object result = DatabaseHelper.ExecuteScalar(
                    conn,
                    tran,
                    "SELECT 1 FROM sys.triggers WHERE name = @Name AND is_disabled = 0",
                    new SqlParameter("@Name", triggerName)
                );
                if (result != null && result != DBNull.Value) return true;

                // Fallback: OBJECT_ID can succeed in some environments where sys.* metadata visibility is limited.
                object objId = DatabaseHelper.ExecuteScalar(
                    conn,
                    tran,
                    "SELECT OBJECT_ID(QUOTENAME('dbo') + '.' + QUOTENAME(@Name), 'TR')",
                    new SqlParameter("@Name", triggerName)
                );
                return objId != null && objId != DBNull.Value;
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLogThrottled(
                    throttleKey: "PosService.TriggerExists",
                    eventDesc: "Trigger Exists Check Error",
                    ex: ex,
                    context: "PosService.TriggerExists(" + triggerName + ")",
                    minSeconds: 300);
                throw;
            }
        }

        private static int InsertInvoice(SqlConnection conn, SqlTransaction tran, int memberId, decimal totalAmount, decimal discountAmount, decimal finalAmount, string paymentMethod)
        {
            var memberParam = new SqlParameter("@MemberID", SqlDbType.Int);
            memberParam.Value = memberId > 0 ? (object)memberId : DBNull.Value;

            object idObj = DatabaseHelper.ExecuteScalar(
                conn,
                tran,
                                SqlQueries.Pos.InsertInvoiceReturnId,
                memberParam,
                new SqlParameter("@TotalAmount", totalAmount),
                new SqlParameter("@DiscountAmount", discountAmount),
                new SqlParameter("@FinalAmount", finalAmount),
                new SqlParameter("@PaymentMethod", paymentMethod)
            );

            if (idObj == null || idObj == DBNull.Value)
                throw new InvalidOperationException("Không lấy được InvoiceID sau khi tạo hóa đơn.");

            return Convert.ToInt32(idObj);
        }

        private static void InsertInvoiceDetail(SqlConnection conn, SqlTransaction tran, int invoiceId, int productId, int qty, decimal unitPrice)
        {
            InsertInvoiceDetail(conn, tran, invoiceId, productId: productId, bookingId: null, qty: qty, unitPrice: unitPrice);
        }

        private static void InsertInvoiceDetail(SqlConnection conn, SqlTransaction tran, int invoiceId, int? productId, int? bookingId, int qty, decimal unitPrice)
        {
            var pProduct = new SqlParameter("@ProductID", SqlDbType.Int);
            pProduct.Value = productId.HasValue && productId.Value > 0 ? (object)productId.Value : DBNull.Value;

            var pBooking = new SqlParameter("@BookingID", SqlDbType.Int);
            pBooking.Value = bookingId.HasValue && bookingId.Value > 0 ? (object)bookingId.Value : DBNull.Value;

            DatabaseHelper.ExecuteNonQuery(
                conn,
                tran,
                "INSERT INTO InvoiceDetails (InvoiceID, ProductID, BookingID, Quantity, UnitPrice) VALUES (@InvoiceID, @ProductID, @BookingID, @Quantity, @UnitPrice)",
                new SqlParameter("@InvoiceID", invoiceId),
                pProduct,
                pBooking,
                new SqlParameter("@Quantity", qty),
                new SqlParameter("@UnitPrice", unitPrice)
            );
        }

        private static void EnsureStockAndMaybeReduce(SqlConnection conn, SqlTransaction tran, int productId, int qty, string prodName, bool hasReduceStockTrigger)
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
                    throw new InvalidOperationException($"Không tìm thấy sản phẩm: {prodName}");

                int stock = Convert.ToInt32(stockObj);
                if (stock < qty)
                    throw new InvalidOperationException($"Sản phẩm '{prodName}' không đủ hàng (còn {stock}, cần {qty}).");

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
                throw new InvalidOperationException($"Sản phẩm '{prodName}' không đủ hàng để trừ kho.");
        }

        private static int TrySetSpecificBookingPaid(SqlConnection conn, SqlTransaction tran, int bookingId)
        {
            if (bookingId <= 0)
                return 0;

            var dt = DatabaseHelper.ExecuteQuery(
                conn,
                tran,
                "SELECT TOP (1) BookingID, StartTime, EndTime, Status FROM dbo.Bookings WHERE BookingID = @BookingID",
                new SqlParameter("@BookingID", bookingId)
            );

            if (dt.Rows.Count <= 0)
                return 0;

            var row = dt.Rows[0];
            string status = row["Status"] == DBNull.Value ? string.Empty : Convert.ToString(row["Status"]);

            if (string.Equals(status, AppConstants.BookingStatus.Cancelled, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, AppConstants.BookingStatus.Maintenance, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, AppConstants.BookingStatus.Paid, StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            DateTime start = Convert.ToDateTime(row["StartTime"]);
            DateTime end = Convert.ToDateTime(row["EndTime"]);
            DateTime now = DateTime.Now;

            // Do not pay a booking that has not started yet.
            if (start > now)
                return 0;

            int affected;
            if (end > now)
            {
                affected = DatabaseHelper.ExecuteNonQuery(
                    conn,
                    tran,
                    $@"UPDATE dbo.Bookings
SET EndTime = @Now,
    Status = @Status
WHERE BookingID = @BookingID
  AND Status <> '{AppConstants.BookingStatus.Cancelled}'
  AND Status <> '{AppConstants.BookingStatus.Maintenance}'
  AND Status <> '{AppConstants.BookingStatus.Paid}'",
                    new SqlParameter("@Now", now),
                    new SqlParameter("@Status", AppConstants.BookingStatus.Paid),
                    new SqlParameter("@BookingID", bookingId)
                );
            }
            else
            {
                affected = DatabaseHelper.ExecuteNonQuery(
                    conn,
                    tran,
                    $@"UPDATE dbo.Bookings
SET Status = @Status
WHERE BookingID = @BookingID
  AND Status <> '{AppConstants.BookingStatus.Cancelled}'
  AND Status <> '{AppConstants.BookingStatus.Maintenance}'
  AND Status <> '{AppConstants.BookingStatus.Paid}'",
                    new SqlParameter("@Status", AppConstants.BookingStatus.Paid),
                    new SqlParameter("@BookingID", bookingId)
                );
            }

            return affected > 0 ? bookingId : 0;
        }

        private static int TryCloseActiveBookingByCourtName(SqlConnection conn, SqlTransaction tran, string courtName)
        {
            string trimmed = (courtName ?? "").Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                return 0;

            object bookingIdObj = DatabaseHelper.ExecuteScalar(
                conn,
                tran,
                                $@"DECLARE @Now DATETIME = GETDATE();
SELECT TOP (1) b.BookingID
FROM dbo.Bookings b
INNER JOIN dbo.Courts c ON c.CourtID = b.CourtID
WHERE c.Name = @CourtName
    AND b.Status <> '{AppConstants.BookingStatus.Cancelled}'
        AND b.Status <> '{AppConstants.BookingStatus.Maintenance}'
  AND b.StartTime <= @Now
  AND b.EndTime > @Now
ORDER BY b.StartTime DESC, b.BookingID DESC;",
                new SqlParameter("@CourtName", trimmed)
            );

            if (bookingIdObj == null || bookingIdObj == DBNull.Value)
                return 0;

            int bookingId = Convert.ToInt32(bookingIdObj);

            int affected = DatabaseHelper.ExecuteNonQuery(
                conn,
                tran,
                                $@"DECLARE @Now DATETIME = GETDATE();
UPDATE dbo.Bookings
SET EndTime = @Now,
    Status = @Status
WHERE BookingID = @BookingID
    AND Status <> '{AppConstants.BookingStatus.Cancelled}'
        AND Status <> '{AppConstants.BookingStatus.Maintenance}'
  AND StartTime <= @Now
  AND EndTime > @Now;",
                                new SqlParameter("@Status", AppConstants.BookingStatus.Paid),
                new SqlParameter("@BookingID", bookingId)
            );

            return affected > 0 ? bookingId : 0;
        }

        private static int TryMarkMostRecentEndedBookingPaidByCourtName(SqlConnection conn, SqlTransaction tran, string courtName)
        {
            string trimmed = (courtName ?? "").Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                return 0;

            // Pick the most recent booking that has already ended (EndTime <= now).
            // We constrain to a reasonable time window to avoid picking a very old booking.
            object bookingIdObj = DatabaseHelper.ExecuteScalar(
                conn,
                tran,
                                $@"DECLARE @Now DATETIME = GETDATE();
SELECT TOP (1) b.BookingID
FROM dbo.Bookings b
INNER JOIN dbo.Courts c ON c.CourtID = b.CourtID
WHERE c.Name = @CourtName
    AND b.Status <> '{AppConstants.BookingStatus.Cancelled}'
        AND b.Status <> '{AppConstants.BookingStatus.Maintenance}'
  AND b.StartTime <= @Now
  AND b.EndTime <= @Now
  AND b.EndTime >= DATEADD(HOUR, -12, @Now)
ORDER BY b.EndTime DESC, b.BookingID DESC;",
                new SqlParameter("@CourtName", trimmed)
            );

            if (bookingIdObj == null || bookingIdObj == DBNull.Value)
                return 0;

            int bookingId = Convert.ToInt32(bookingIdObj);

            // Mark it as paid if not already.
            DatabaseHelper.ExecuteNonQuery(
                conn,
                tran,
                $"UPDATE dbo.Bookings SET Status = @Status WHERE BookingID = @BookingID AND Status <> '{AppConstants.BookingStatus.Cancelled}' AND Status <> '{AppConstants.BookingStatus.Maintenance}'",
                new SqlParameter("@Status", AppConstants.BookingStatus.Paid),
                new SqlParameter("@BookingID", bookingId)
            );

            return bookingId;
        }
    }
}
