using DemoPick.Helpers;
using DemoPick.Data;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DemoPick.Services
{
    internal static class PosBookingPaymentStateService
    {
        // Thu thuc hien Try Set Specific Booking Paid, neu du lieu khong hop le thi dung va tra thong bao phu hop.
        internal static int TrySetSpecificBookingPaid(SqlConnection conn, SqlTransaction tran, int bookingId)
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

        // Thu thuc hien Try Close Active Booking By Court Name, neu du lieu khong hop le thi dung va tra thong bao phu hop.
        internal static int TryCloseActiveBookingByCourtName(SqlConnection conn, SqlTransaction tran, string courtName)
        {
            string trimmed = (courtName ?? string.Empty).Trim();
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

        // Thu thuc hien Try Mark Most Recent Ended Booking Paid By Court Name, neu du lieu khong hop le thi dung va tra thong bao phu hop.
        internal static int TryMarkMostRecentEndedBookingPaidByCourtName(SqlConnection conn, SqlTransaction tran, string courtName)
        {
            string trimmed = (courtName ?? string.Empty).Trim();
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

