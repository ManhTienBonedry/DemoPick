using DemoPick.Helpers;
using DemoPick.Data;
using DemoPick.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DemoPick.Services
{
    public class PosService
    {
        // --- PENDING ORDER STATE MANAGEMENT ---
        public static Dictionary<string, List<CartLine>> PendingOrders { get; } = new Dictionary<string, List<CartLine>>(StringComparer.OrdinalIgnoreCase);

        // Luu hoac ghi nhan Save Pending Order vao trang thai he thong/CSDL khi nghiep vu yeu cau.
        public static void SavePendingOrder(string courtName, List<CartLine> lines)
        {
            if (string.IsNullOrWhiteSpace(courtName)) return;
            PendingOrders[courtName] = lines ?? new List<CartLine>();
        }

        // Lay du lieu/ket qua cho Get Pending Order tu tang xu ly phu hop.
        public static List<CartLine> GetPendingOrder(string courtName)
        {
            if (string.IsNullOrWhiteSpace(courtName)) return new List<CartLine>();
            if (PendingOrders.TryGetValue(courtName, out var lines)) return new List<CartLine>(lines);
            return new List<CartLine>();
        }

        // Xoa, huy hoac dat lai du lieu Clear Pending Order theo dung dieu kien nghiep vu.
        public static void ClearPendingOrder(string courtName)
        {
            if (string.IsNullOrWhiteSpace(courtName)) return;
            PendingOrders.Remove(courtName);
        }
        // Thuc hien thanh toan POS, ghi hoa don, chi tiet hoa don va cap nhat trang thai booking/kho.

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
                            creditedBookingId = PosBookingPaymentStateService.TrySetSpecificBookingPaid(conn, tran, preferredBookingId.Value);
                            closedBookingId = creditedBookingId;

                            if (creditedBookingId == 0)
                            {
                                throw new InvalidOperationException("Booking đã chọn chưa thể thanh toán hoặc không còn hợp lệ.");
                            }
                        }
                        else
                        {
                            closedBookingId = PosBookingPaymentStateService.TryCloseActiveBookingByCourtName(conn, tran, courtNameForLog);
                            creditedBookingId = closedBookingId;

                            if (creditedBookingId == 0)
                            {
                                // Fallback: if booking already ended (EndTime <= now), still credit the member's hours
                                // using the most recent booking for this court (typically "khách chơi trễ" scenario).
                                creditedBookingId = PosBookingPaymentStateService.TryMarkMostRecentEndedBookingPaidByCourtName(conn, tran, courtNameForLog);
                            }
                        }

                        int effectiveMemberId = PosMemberResolver.ResolveMemberForCheckout(conn, tran, memberId, creditedBookingId);
                        int invoiceId = PosInvoiceWriter.InsertInvoice(conn, tran, effectiveMemberId, totalAmount, discountAmount, finalAmount, paymentMethod);

                        var productCategories = PosInventoryValidator.LoadProductCategories(conn, tran, lines);

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

                            if (!PosInventoryValidator.IsServiceCategory(category))
                            {
                                PosInventoryValidator.EnsureStockAndMaybeReduce(conn, tran, line.ProductId, line.Quantity, line.ProductName, hasReduceStockTrigger);
                            }

                            PosInvoiceWriter.InsertInvoiceDetail(conn, tran, invoiceId, productId: line.ProductId, bookingId: null, qty: line.Quantity, unitPrice: line.UnitPrice);
                        }

                        // 2) Insert court/service lines after we know which booking got credited.
                        // These lines should NOT reduce stock, so ProductID must be NULL.
                        foreach (var line in lines)
                        {
                            if (line == null) continue;
                            if (line.ProductId > 0) continue;
                            if (line.Quantity <= 0) continue;

                            int? bookingId = creditedBookingId > 0 ? (int?)creditedBookingId : null;
                            PosInvoiceWriter.InsertInvoiceDetail(conn, tran, invoiceId, productId: null, bookingId: bookingId, qty: line.Quantity, unitPrice: line.UnitPrice);
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

                        if (!PosCheckoutLogFormatter.IsTestMode())
                        {
                            DatabaseHelper.ExecuteNonQuery(
                                conn,
                                tran,
                                "INSERT INTO SystemLogs (EventDesc, SubDesc) VALUES (@EventDesc, @SubDesc)",
                                new SqlParameter("@EventDesc", "POS Checkout"),
                                new SqlParameter("@SubDesc", PosCheckoutLogFormatter.BuildPosCheckoutLogSubDesc(invoiceId, courtNameForLog, finalAmount, paymentMethod, lines))
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

        // Kiem tra dieu kien Trigger Exists va tra ve ket qua dung/sai cho luong xu ly.
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

    }
}

