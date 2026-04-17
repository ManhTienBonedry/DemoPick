using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DemoPick.Models;
using DemoPick.Services;

namespace DemoPick.Controllers
{
    public class BookingController
    {
        private static readonly object _noteSchemaLock = new object();
        private static bool _noteSchemaChecked;
        private static bool _noteSchemaOk;

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

    INSERT INTO dbo.Bookings (CourtID, MemberID, GuestName, Note, StartTime, EndTime, Status)
    VALUES (@CourtID, @MemberID, @GuestName, @Note, @StartTime, @EndTime, @Status);
END");

                    _noteSchemaOk = true;
                }
                catch (Exception ex)
                {
                    // Best-effort: do not crash the app if the database is not up to date.
                    _noteSchemaOk = false;
                    DatabaseHelper.TryLogThrottled(
                        throttleKey: "BookingController.TryEnsureBookingNoteSchema",
                        eventDesc: "Booking Note Schema Ensure Failed",
                        ex: ex,
                        context: "BookingController.TryEnsureBookingNoteSchema",
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
                    throttleKey: "BookingController.HasBookingNoteColumn",
                    eventDesc: "Booking Note Schema Check Failed",
                    ex: ex,
                    context: "BookingController.HasBookingNoteColumn",
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
                    throttleKey: "BookingController.SpCreateBookingHasNoteParam",
                    eventDesc: "Booking Proc Metadata Check Failed",
                    ex: ex,
                    context: "BookingController.SpCreateBookingHasNoteParam",
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
                    throttleKey: "BookingController.SpCreateBookingHasMemberIdParam",
                    eventDesc: "Booking Proc Metadata Check Failed",
                    ex: ex,
                    context: "BookingController.SpCreateBookingHasMemberIdParam",
                    minSeconds: 300);
                return false;
            }
        }

        public int? GetOrCreateMemberId(string fullName, string phone)
        {
            try
            {
                string p = (phone ?? "").Trim();
                string name = (fullName ?? "").Trim();

                // Members.FullName is NOT NULL in the default schema; ensure a non-empty value.
                string safeName = string.IsNullOrWhiteSpace(name) ? p : name;

                if (string.IsNullOrWhiteSpace(p))
                    return null;

                // Find existing member by phone
                object existingObj = DatabaseHelper.ExecuteScalar(
                    "SELECT TOP 1 MemberID FROM dbo.Members WHERE Phone = @Phone ORDER BY MemberID DESC",
                    new SqlParameter("@Phone", p)
                );

                if (existingObj != null && existingObj != DBNull.Value)
                {
                    int id = Convert.ToInt32(existingObj);
                    // Best-effort: update name if the record has empty name.
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        DatabaseHelper.ExecuteNonQuery(
                            "UPDATE dbo.Members SET FullName = COALESCE(NULLIF(LTRIM(RTRIM(FullName)), ''), @Name) WHERE MemberID = @Id",
                            new SqlParameter("@Name", name),
                            new SqlParameter("@Id", id)
                        );
                    }
                    return id;
                }

                // Create new member
                object newIdObj = DatabaseHelper.ExecuteScalar(@"
INSERT INTO dbo.Members (FullName, Phone)
VALUES (@Name, @Phone);
SELECT CAST(SCOPE_IDENTITY() AS INT);",
                    new SqlParameter("@Name", safeName),
                    new SqlParameter("@Phone", p)
                );

                if (newIdObj == null || newIdObj == DBNull.Value) return null;
                return Convert.ToInt32(newIdObj);
            }
            catch (Exception ex)
            {
                try { DatabaseHelper.TryLog("Member Upsert Error", ex, "BookingController.GetOrCreateMemberId"); } catch { }
                return null;
            }
        }

        public List<CourtModel> GetCourts()
        {
            var list = new List<CourtModel>();
            string query = $@"
SELECT CourtID, Name, CourtType, HourlyRate
FROM Courts
WHERE (Status = '{AppConstants.CourtStatus.Active}' OR Status IS NULL OR LTRIM(RTRIM(Status)) = '')
ORDER BY
    CASE
        WHEN CourtType IN (N'Trong nhà', N'Trong Nha', N'Indoor') OR Name LIKE N'%(Trong nhà)%' THEN 1
        WHEN CourtType IN (N'Ngoài trời', N'Ngoai Troi', N'Outdoor') OR Name LIKE N'%(Ngoài trời)%' THEN 2
        WHEN CourtType IN (N'Sân tập', N'San tap', N'Practice', N'Training') OR Name LIKE N'Sân Tập%' OR Name LIKE N'San Tap%' THEN 3
        ELSE 9
    END,
    CASE
        WHEN PATINDEX('%[0-9]%', Name) > 0
        THEN TRY_CONVERT(int,
            SUBSTRING(
                Name,
                PATINDEX('%[0-9]%', Name),
                PATINDEX('%[^0-9]%', SUBSTRING(Name, PATINDEX('%[0-9]%', Name), 100) + 'X') - 1
            )
        )
        ELSE 9999
    END,
    Name,
    CourtID";
            var dt = DatabaseHelper.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new CourtModel
                {
                    CourtID = Convert.ToInt32(row["CourtID"]),
                    Name = row["Name"].ToString(),
                    CourtType = row["CourtType"].ToString(),
                    HourlyRate = Convert.ToDecimal(row["HourlyRate"])
                });
            }
            return list;
        }

        public List<BookingModel> GetBookingsByDate(DateTime date)
        {
            var list = new List<BookingModel>();
                        // Best-effort ensure schema once. Even if it fails, we must not crash the UI.
                        TryEnsureBookingNoteSchema();

                        bool hasNote = HasBookingNoteColumn();
                        string query = hasNote
                                ? $@"
                                SELECT BookingID, CourtID, GuestName, Note, StartTime, EndTime, Status
                                FROM Bookings
                                WHERE CAST(StartTime AS DATE) = @Date
                                    AND Status != '{AppConstants.BookingStatus.Cancelled}'"
                                : $@"
                                SELECT BookingID, CourtID, GuestName, CAST(NULL AS NVARCHAR(200)) AS Note, StartTime, EndTime, Status
                                FROM Bookings
                                WHERE CAST(StartTime AS DATE) = @Date
                                    AND Status != '{AppConstants.BookingStatus.Cancelled}'";
                  
            var dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@Date", date.Date));
            foreach (DataRow row in dt.Rows)
            {
                string guest = row["GuestName"] != DBNull.Value ? row["GuestName"].ToString() : null;
                if (string.IsNullOrWhiteSpace(guest))
                {
                    guest = "Khách lẻ";
                }

                list.Add(new BookingModel
                {
                    BookingID = Convert.ToInt32(row["BookingID"]),
                    CourtID = Convert.ToInt32(row["CourtID"]),
                    GuestName = guest,
                    Note = row["Note"] != DBNull.Value ? row["Note"].ToString() : "",
                    StartTime = Convert.ToDateTime(row["StartTime"]),
                    EndTime = Convert.ToDateTime(row["EndTime"]),
                    Status = row["Status"].ToString()
                });
            }
            return list;
        }

        public void SubmitBooking(int courtId, string guestName, DateTime startTime, DateTime endTime)
        {
            SubmitBooking(courtId, guestName, note: null, startTime: startTime, endTime: endTime, status: AppConstants.BookingStatus.Confirmed);
        }

        public void SubmitBooking(int courtId, string guestName, DateTime startTime, DateTime endTime, string status)
        {
            SubmitBooking(courtId, guestName, note: null, startTime: startTime, endTime: endTime, status: status);
        }

        public void SubmitBooking(int courtId, string guestName, string note, DateTime startTime, DateTime endTime, string status)
        {
            SubmitBooking(courtId, memberId: null, guestName: guestName, note: note, startTime: startTime, endTime: endTime, status: status);
        }

        public void SubmitBooking(int courtId, int? memberId, string guestName, string note, DateTime startTime, DateTime endTime, string status)
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

                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime", endTime);
                cmd.Parameters.AddWithValue("@Status", string.IsNullOrWhiteSpace(status) ? AppConstants.BookingStatus.Confirmed : status);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateBookingTime(int bookingId, DateTime newStartTime, DateTime newEndTime)
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

        public void UpdateBookingTimeAndNote(int bookingId, DateTime newStartTime, DateTime newEndTime, string note)
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

        public void DeactivateCourt(int courtId)
        {
            if (courtId <= 0) throw new ArgumentException("CourtID không hợp lệ.");

            // Safety: do not allow deactivating a court that still has active/future bookings.
            object cntObj = DatabaseHelper.ExecuteScalar(
                $"SELECT COUNT(*) FROM dbo.Bookings WHERE CourtID = @CourtID AND Status <> '{AppConstants.BookingStatus.Cancelled}' AND EndTime > GETDATE()",
                new SqlParameter("@CourtID", courtId)
            );

            int cnt = cntObj == null || cntObj == DBNull.Value ? 0 : Convert.ToInt32(cntObj);
            if (cnt > 0)
            {
                throw new InvalidOperationException("Sân đang có booking hiện tại/ tương lai. Vui lòng hủy hoặc xử lý booking trước khi xóa sân.");
            }

            int affected = DatabaseHelper.ExecuteNonQuery(
                $"UPDATE dbo.Courts SET Status = '{AppConstants.CourtStatus.Inactive}' WHERE CourtID = @CourtID AND (Status = '{AppConstants.CourtStatus.Active}' OR Status IS NULL OR LTRIM(RTRIM(Status)) = '')",
                new SqlParameter("@CourtID", courtId)
            );

            if (affected <= 0)
            {
                throw new InvalidOperationException("Không tìm thấy sân để xóa (hoặc sân đã bị vô hiệu hóa).");
            }
        }

        public void MarkBookingAsPending(int bookingId)
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

        public void CancelBooking(int bookingId)
        {
            if (bookingId <= 0) throw new ArgumentException("BookingID không hợp lệ.");

            var dt = DatabaseHelper.ExecuteQuery(
                "SELECT Status FROM dbo.Bookings WHERE BookingID = @Id",
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

            int affected = DatabaseHelper.ExecuteNonQuery(
                $"UPDATE dbo.Bookings SET Status = '{AppConstants.BookingStatus.Cancelled}' WHERE BookingID = @Id AND Status <> '{AppConstants.BookingStatus.Paid}' AND Status <> '{AppConstants.BookingStatus.Cancelled}'",
                new SqlParameter("@Id", bookingId)
            );

            if (affected <= 0)
            {
                throw new InvalidOperationException("Không thể xóa booking ở trạng thái hiện tại.");
            }
        }
    }
}