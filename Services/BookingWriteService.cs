using System;
using System.Data;
using System.Data.SqlClient;

namespace DemoPick.Services
{
    internal static class BookingWriteService
    {
        private static readonly object _noteSchemaLock = new object();
        private static bool _noteSchemaChecked;
        private static bool _noteSchemaOk;

        internal static void SubmitBooking(int courtId, int? memberId, string guestName, string note, DateTime startTime, DateTime endTime, string status, string paymentState = null)
        {
            // Ensure schema/proc if possible; if not, fall back to legacy proc call (without @Note).
            TryEnsureBookingNoteSchema();

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("sp_CreateBooking", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CourtID", courtId);

                if (SpCreateBookingHasMemberIdParam())
                {
                    object mid = (memberId.HasValue && memberId.Value > 0) ? (object)memberId.Value : DBNull.Value;
                    cmd.Parameters.AddWithValue("@MemberID", mid);
                }

                string normalizedGuest = guestName;
                if (!string.IsNullOrWhiteSpace(normalizedGuest))
                {
                    normalizedGuest = normalizedGuest.Trim();
                    if (normalizedGuest.Replace(" ", "") == "-")
                    {
                        normalizedGuest = null;
                    }
                }

                object guestDb = string.IsNullOrWhiteSpace(normalizedGuest) ? (object)DBNull.Value : normalizedGuest;
                cmd.Parameters.AddWithValue("@GuestName", guestDb);

                if (SpCreateBookingHasNoteParam())
                {
                    object noteDb = string.IsNullOrWhiteSpace(note) ? (object)DBNull.Value : note.Trim();
                    cmd.Parameters.AddWithValue("@Note", noteDb);
                }

                if (HasBookingPaymentStateColumn() && SpCreateBookingHasPaymentStateParam())
                {
                    cmd.Parameters.AddWithValue("@PaymentState", NormalizeBookingPaymentState(paymentState));
                }

                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime", endTime);
                cmd.Parameters.AddWithValue("@Status", string.IsNullOrWhiteSpace(status) ? AppConstants.BookingStatus.Confirmed : status);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        internal static void UpdateBookingTime(int bookingId, DateTime newStartTime, DateTime newEndTime)
        {
            if (bookingId <= 0) throw new ArgumentException("BookingID không hợp lệ.");
            if (newEndTime <= newStartTime) throw new ArgumentException("Giờ kết thúc phải sau giờ bắt đầu.");
            if (newStartTime < DateTime.Now) throw new ArgumentException("Giờ bắt đầu đã qua, vui lòng chọn lại.");

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                var dt = DatabaseHelper.ExecuteQuery(
                    "SELECT CourtID, Status FROM dbo.Bookings WHERE BookingID = @Id",
                    new SqlParameter("@Id", bookingId)
                );

                if (dt.Rows.Count == 0)
                    throw new InvalidOperationException("Không tìm thấy booking để đổi ca.");

                int courtId = Convert.ToInt32(dt.Rows[0]["CourtID"]);
                string status = dt.Rows[0]["Status"] == DBNull.Value ? "" : dt.Rows[0]["Status"].ToString();

                if (string.Equals(status, AppConstants.BookingStatus.Cancelled, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Booking đã bị hủy.");
                if (string.Equals(status, AppConstants.BookingStatus.Paid, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Booking đã thanh toán, không thể đổi ca.");

                object conflictObj = DatabaseHelper.ExecuteScalar(
                    $"SELECT COUNT(*) FROM dbo.Bookings WHERE CourtID = @CourtID AND BookingID <> @BookingID AND Status <> '{AppConstants.BookingStatus.Cancelled}' AND StartTime < @NewEnd AND EndTime > @NewStart",
                    new SqlParameter("@CourtID", courtId),
                    new SqlParameter("@BookingID", bookingId),
                    new SqlParameter("@NewStart", newStartTime),
                    new SqlParameter("@NewEnd", newEndTime)
                );

                int conflicts = conflictObj == null || conflictObj == DBNull.Value ? 0 : Convert.ToInt32(conflictObj);
                if (conflicts > 0)
                    throw new InvalidOperationException("Khung giờ mới bị trùng lịch. Vui lòng chọn ca khác.");

                DatabaseHelper.ExecuteNonQuery(
                    $"UPDATE dbo.Bookings SET StartTime = @Start, EndTime = @End WHERE BookingID = @Id AND Status <> '{AppConstants.BookingStatus.Cancelled}'",
                    new SqlParameter("@Start", newStartTime),
                    new SqlParameter("@End", newEndTime),
                    new SqlParameter("@Id", bookingId)
                );
            }
        }

        internal static void UpdateBookingTimeAndNote(int bookingId, DateTime newStartTime, DateTime newEndTime, string note)
        {
            if (bookingId <= 0) throw new ArgumentException("BookingID không hợp lệ.");

            // Ensure schema once. If not available, still allow time update (no crash), but cannot persist note.
            TryEnsureBookingNoteSchema();
            bool hasNote = HasBookingNoteColumn();

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                var dt = DatabaseHelper.ExecuteQuery(
                    "SELECT CourtID, Status, StartTime, EndTime FROM dbo.Bookings WHERE BookingID = @Id",
                    new SqlParameter("@Id", bookingId)
                );

                if (dt.Rows.Count == 0)
                    throw new InvalidOperationException("Không tìm thấy booking để cập nhật.");

                int courtId = Convert.ToInt32(dt.Rows[0]["CourtID"]);
                string status = dt.Rows[0]["Status"] == DBNull.Value ? "" : dt.Rows[0]["Status"].ToString();
                DateTime currentStart = Convert.ToDateTime(dt.Rows[0]["StartTime"]);
                DateTime currentEnd = Convert.ToDateTime(dt.Rows[0]["EndTime"]);

                if (string.Equals(status, AppConstants.BookingStatus.Cancelled, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Booking đã bị hủy.");
                if (string.Equals(status, AppConstants.BookingStatus.Paid, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Booking đã thanh toán, không thể đổi ca.");

                bool timeChanged = newStartTime != currentStart || newEndTime != currentEnd;
                if (timeChanged)
                {
                    if (newEndTime <= newStartTime) throw new ArgumentException("Giờ kết thúc phải sau giờ bắt đầu.");
                    if (newStartTime < DateTime.Now) throw new ArgumentException("Giờ bắt đầu đã qua, vui lòng chọn lại.");

                    object conflictObj = DatabaseHelper.ExecuteScalar(
                        $"SELECT COUNT(*) FROM dbo.Bookings WHERE CourtID = @CourtID AND BookingID <> @BookingID AND Status <> '{AppConstants.BookingStatus.Cancelled}' AND StartTime < @NewEnd AND EndTime > @NewStart",
                        new SqlParameter("@CourtID", courtId),
                        new SqlParameter("@BookingID", bookingId),
                        new SqlParameter("@NewStart", newStartTime),
                        new SqlParameter("@NewEnd", newEndTime)
                    );

                    int conflicts = conflictObj == null || conflictObj == DBNull.Value ? 0 : Convert.ToInt32(conflictObj);
                    if (conflicts > 0)
                        throw new InvalidOperationException("Khung giờ mới bị trùng lịch. Vui lòng chọn ca khác.");
                }

                if (hasNote)
                {
                    object noteDb = string.IsNullOrWhiteSpace(note) ? (object)DBNull.Value : note.Trim();
                    DatabaseHelper.ExecuteNonQuery(
                        $"UPDATE dbo.Bookings SET StartTime = @Start, EndTime = @End, Note = @Note WHERE BookingID = @Id AND Status <> '{AppConstants.BookingStatus.Cancelled}'",
                        new SqlParameter("@Start", timeChanged ? newStartTime : currentStart),
                        new SqlParameter("@End", timeChanged ? newEndTime : currentEnd),
                        new SqlParameter("@Note", noteDb),
                        new SqlParameter("@Id", bookingId)
                    );
                }
                else
                {
                    // No Note column in DB: keep app functional by updating only time.
                    DatabaseHelper.ExecuteNonQuery(
                        $"UPDATE dbo.Bookings SET StartTime = @Start, EndTime = @End WHERE BookingID = @Id AND Status <> '{AppConstants.BookingStatus.Cancelled}'",
                        new SqlParameter("@Start", timeChanged ? newStartTime : currentStart),
                        new SqlParameter("@End", timeChanged ? newEndTime : currentEnd),
                        new SqlParameter("@Id", bookingId)
                    );

                    if (!string.IsNullOrWhiteSpace(note))
                        throw new InvalidOperationException("CSDL chưa được cập nhật để lưu ghi chú (thiếu cột Bookings.Note). Vui lòng khởi động lại app để chạy migration hoặc rebuild CSDL.");
                }
            }
        }

        internal static void MarkBookingAsPending(int bookingId)
        {
            if (bookingId <= 0) throw new ArgumentException("BookingID không hợp lệ.");

            int affected = DatabaseHelper.ExecuteNonQuery(
                $@"UPDATE dbo.Bookings
SET Status = @Status
WHERE BookingID = @Id
  AND Status <> '{AppConstants.BookingStatus.Cancelled}'
  AND Status <> '{AppConstants.BookingStatus.Maintenance}'
  AND Status <> '{AppConstants.BookingStatus.Paid}'",
                new SqlParameter("@Status", AppConstants.BookingStatus.Pending),
                new SqlParameter("@Id", bookingId)
            );

            if (affected <= 0)
            {
                throw new InvalidOperationException("Không thể nhận sân cho booking ở trạng thái hiện tại.");
            }
        }

        internal static void CancelBooking(int bookingId)
        {
            if (bookingId <= 0) throw new ArgumentException("BookingID không hợp lệ.");

            var dt = DatabaseHelper.ExecuteQuery(
                "SELECT Status, MemberID FROM dbo.Bookings WHERE BookingID = @Id",
                new SqlParameter("@Id", bookingId)
            );

            if (dt.Rows.Count == 0)
            {
                throw new InvalidOperationException("Không tìm thấy booking để xóa.");
            }

            string status = dt.Rows[0]["Status"] == DBNull.Value ? "" : dt.Rows[0]["Status"].ToString();

            if (string.Equals(status, AppConstants.BookingStatus.Cancelled, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (string.Equals(status, AppConstants.BookingStatus.Paid, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Booking đã thanh toán, không thể xóa.");
            }

            int memberId = dt.Rows[0]["MemberID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["MemberID"]);

            int affected = DatabaseHelper.ExecuteNonQuery(
                $"UPDATE dbo.Bookings SET Status = '{AppConstants.BookingStatus.Cancelled}' WHERE BookingID = @Id AND Status <> '{AppConstants.BookingStatus.Paid}' AND Status <> '{AppConstants.BookingStatus.Cancelled}'",
                new SqlParameter("@Id", bookingId)
            );

            if (affected <= 0)
            {
                throw new InvalidOperationException("Không thể xóa booking ở trạng thái hiện tại.");
            }

            BookingMemberCleanupService.TryCleanupOrphanWalkinMember(memberId);
        }

        private static bool TryEnsureBookingNoteSchema()
        {
            if (_noteSchemaChecked)
                return _noteSchemaOk;

            lock (_noteSchemaLock)
            {
                if (_noteSchemaChecked)
                    return _noteSchemaOk;

                try
                {
                    // 1) Ensure column exists
                    object col = DatabaseHelper.ExecuteScalar("SELECT COL_LENGTH('dbo.Bookings', 'Note')");
                    if (col == null || col == DBNull.Value)
                    {
                        DatabaseHelper.ExecuteNonQuery("ALTER TABLE dbo.Bookings ADD Note NVARCHAR(200) NULL;");
                    }

                    object payCol = DatabaseHelper.ExecuteScalar("SELECT COL_LENGTH('dbo.Bookings', 'PaymentState')");
                    if (payCol == null || payCol == DBNull.Value)
                    {
                        DatabaseHelper.ExecuteNonQuery($@"ALTER TABLE dbo.Bookings
ADD PaymentState NVARCHAR(50) NOT NULL
CONSTRAINT DF_Bookings_PaymentState DEFAULT '{AppConstants.BookingPaymentState.PayAtVenue}';");
                    }
                    else
                    {
                        DatabaseHelper.ExecuteNonQuery($@"UPDATE dbo.Bookings
SET PaymentState = '{AppConstants.BookingPaymentState.PayAtVenue}'
WHERE PaymentState IS NULL OR LTRIM(RTRIM(PaymentState)) = '';");
                    }

                    // 2) Ensure procedure exists and supports @Note
                    DatabaseHelper.ExecuteNonQuery(@"IF OBJECT_ID('dbo.sp_CreateBooking','P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_CreateBooking AS BEGIN SET NOCOUNT ON; END');
END");

                    DatabaseHelper.ExecuteNonQuery($@"
ALTER PROCEDURE dbo.sp_CreateBooking
    @CourtID INT,
    @MemberID INT = NULL,
    @GuestName NVARCHAR(100) = NULL,
    @Note NVARCHAR(200) = NULL,
    @PaymentState NVARCHAR(50) = '{AppConstants.BookingPaymentState.PayAtVenue}',
    @StartTime DATETIME,
    @EndTime DATETIME,
    @Status NVARCHAR(50) = '{AppConstants.BookingStatus.Confirmed}'
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM dbo.Bookings
        WHERE CourtID = @CourtID
                    AND Status != '{AppConstants.BookingStatus.Cancelled}'
          AND (StartTime < @EndTime AND EndTime > @StartTime)
    )
    BEGIN
        RAISERROR('Court is already booked for this time slot.', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.Bookings (CourtID, MemberID, GuestName, Note, PaymentState, StartTime, EndTime, Status)
    VALUES (@CourtID, @MemberID, @GuestName, @Note, @PaymentState, @StartTime, @EndTime, @Status);
END");

                    _noteSchemaOk = true;
                }
                catch (Exception ex)
                {
                    // Best-effort: do not crash the app if the database is not up to date.
                    _noteSchemaOk = false;
                    DatabaseHelper.TryLogThrottled(
                        throttleKey: "BookingWriteService.TryEnsureBookingNoteSchema",
                        eventDesc: "Booking Note Schema Ensure Failed",
                        ex: ex,
                        context: "BookingWriteService.TryEnsureBookingNoteSchema",
                        minSeconds: 300);
                }
                finally
                {
                    _noteSchemaChecked = true;
                }

                return _noteSchemaOk;
            }
        }

        private static bool HasBookingNoteColumn()
        {
            // Avoid depending on TryEnsureBookingNoteSchema() because proc ALTER might fail even if column exists.
            try
            {
                object col = DatabaseHelper.ExecuteScalar("SELECT COL_LENGTH('dbo.Bookings', 'Note')");
                return !(col == null || col == DBNull.Value);
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLogThrottled(
                    throttleKey: "BookingWriteService.HasBookingNoteColumn",
                    eventDesc: "Booking Note Schema Check Failed",
                    ex: ex,
                    context: "BookingWriteService.HasBookingNoteColumn",
                    minSeconds: 300);
                return false;
            }
        }

        private static bool HasBookingPaymentStateColumn()
        {
            try
            {
                object col = DatabaseHelper.ExecuteScalar("SELECT COL_LENGTH('dbo.Bookings', 'PaymentState')");
                return !(col == null || col == DBNull.Value);
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLogThrottled(
                    throttleKey: "BookingWriteService.HasBookingPaymentStateColumn",
                    eventDesc: "Booking PaymentState Schema Check Failed",
                    ex: ex,
                    context: "BookingWriteService.HasBookingPaymentStateColumn",
                    minSeconds: 300);
                return false;
            }
        }

        private static bool SpCreateBookingHasNoteParam()
        {
            try
            {
                object obj = DatabaseHelper.ExecuteScalar(@"
SELECT COUNT(*)
FROM sys.parameters
WHERE object_id = OBJECT_ID('dbo.sp_CreateBooking')
  AND name = '@Note'");

                int count = obj == null || obj == DBNull.Value ? 0 : Convert.ToInt32(obj);
                return count > 0;
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLogThrottled(
                    throttleKey: "BookingWriteService.SpCreateBookingHasNoteParam",
                    eventDesc: "Booking Proc Metadata Check Failed",
                    ex: ex,
                    context: "BookingWriteService.SpCreateBookingHasNoteParam",
                    minSeconds: 300);
                return false;
            }
        }

        private static bool SpCreateBookingHasMemberIdParam()
        {
            try
            {
                object obj = DatabaseHelper.ExecuteScalar(@"
SELECT COUNT(*)
FROM sys.parameters
WHERE object_id = OBJECT_ID('dbo.sp_CreateBooking')
  AND name = '@MemberID'");

                int count = obj == null || obj == DBNull.Value ? 0 : Convert.ToInt32(obj);
                return count > 0;
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLogThrottled(
                    throttleKey: "BookingWriteService.SpCreateBookingHasMemberIdParam",
                    eventDesc: "Booking Proc Metadata Check Failed",
                    ex: ex,
                    context: "BookingWriteService.SpCreateBookingHasMemberIdParam",
                    minSeconds: 300);
                return false;
            }
        }

        private static bool SpCreateBookingHasPaymentStateParam()
        {
            try
            {
                object obj = DatabaseHelper.ExecuteScalar(@"
SELECT COUNT(*)
FROM sys.parameters
WHERE object_id = OBJECT_ID('dbo.sp_CreateBooking')
  AND name = '@PaymentState'");

                int count = obj == null || obj == DBNull.Value ? 0 : Convert.ToInt32(obj);
                return count > 0;
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLogThrottled(
                    throttleKey: "BookingWriteService.SpCreateBookingHasPaymentStateParam",
                    eventDesc: "Booking Proc Metadata Check Failed",
                    ex: ex,
                    context: "BookingWriteService.SpCreateBookingHasPaymentStateParam",
                    minSeconds: 300);
                return false;
            }
        }

        private static string NormalizeBookingPaymentState(string paymentState)
        {
            string p = (paymentState ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(p))
                return AppConstants.BookingPaymentState.PayAtVenue;

            if (string.Equals(p, AppConstants.BookingPaymentState.BankTransferred, StringComparison.OrdinalIgnoreCase))
                return AppConstants.BookingPaymentState.BankTransferred;

            if (string.Equals(p, AppConstants.BookingPaymentState.Deposit50, StringComparison.OrdinalIgnoreCase))
                return AppConstants.BookingPaymentState.Deposit50;

            return AppConstants.BookingPaymentState.PayAtVenue;
        }
    }
}