using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DemoPick.Models;

namespace DemoPick.Services
{
    internal static class BookingQueryService
    {
        private static readonly object _noteSchemaLock = new object();
        private static bool _noteSchemaChecked;
        private static bool _noteSchemaOk;

        internal static List<BookingModel> GetBookingsByDate(DateTime date)
        {
            var list = new List<BookingModel>();

            // Best-effort ensure schema once. Even if it fails, we must not crash the UI.
            TryEnsureBookingNoteSchema();

            bool hasNote = HasBookingNoteColumn();
            bool hasPaymentState = HasBookingPaymentStateColumn();
            string selectNote = hasNote ? "Note" : "CAST(NULL AS NVARCHAR(200)) AS Note";
            string selectPaymentState = hasPaymentState
                ? "PaymentState"
                : $"CAST('{AppConstants.BookingPaymentState.PayAtVenue}' AS NVARCHAR(50)) AS PaymentState";

            string query = $@"
                                SELECT BookingID, CourtID, GuestName, {selectNote}, {selectPaymentState}, StartTime, EndTime, Status
                                FROM Bookings
                                WHERE CAST(StartTime AS DATE) = @Date
                                    AND Status != '{AppConstants.BookingStatus.Cancelled}'";

            var dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@Date", date.Date));
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapBookingRow(row));
            }

            return list;
        }

        internal static List<BookingModel> GetUnpaidBookingsUntil(DateTime toDateInclusive)
        {
            var list = new List<BookingModel>();

            // Best-effort ensure schema once. Even if it fails, we must not crash the UI.
            TryEnsureBookingNoteSchema();

            bool hasNote = HasBookingNoteColumn();
            bool hasPaymentState = HasBookingPaymentStateColumn();
            DateTime toDateExclusive = toDateInclusive.Date.AddDays(1);
            string selectNote = hasNote ? "Note" : "CAST(NULL AS NVARCHAR(200)) AS Note";
            string selectPaymentState = hasPaymentState
                ? "PaymentState"
                : $"CAST('{AppConstants.BookingPaymentState.PayAtVenue}' AS NVARCHAR(50)) AS PaymentState";

            string query = $@"
                                SELECT BookingID, CourtID, GuestName, {selectNote}, {selectPaymentState}, StartTime, EndTime, Status
                                FROM Bookings
                                WHERE StartTime < @ToDateExclusive
                                    AND Status <> '{AppConstants.BookingStatus.Cancelled}'
                                    AND Status <> '{AppConstants.BookingStatus.Maintenance}'
                                    AND Status <> '{AppConstants.BookingStatus.Paid}'
                                ORDER BY StartTime ASC, CourtID ASC, BookingID ASC";

            var dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@ToDateExclusive", toDateExclusive));
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapBookingRow(row));
            }

            return list;
        }

        private static BookingModel MapBookingRow(DataRow row)
        {
            string guest = row["GuestName"] != DBNull.Value ? row["GuestName"].ToString() : null;
            if (string.IsNullOrWhiteSpace(guest))
            {
                guest = "Khách lẻ";
            }

            return new BookingModel
            {
                BookingID = Convert.ToInt32(row["BookingID"]),
                CourtID = Convert.ToInt32(row["CourtID"]),
                GuestName = guest,
                Note = row["Note"] != DBNull.Value ? row["Note"].ToString() : string.Empty,
                PaymentState = row["PaymentState"] != DBNull.Value ? row["PaymentState"].ToString() : AppConstants.BookingPaymentState.PayAtVenue,
                StartTime = Convert.ToDateTime(row["StartTime"]),
                EndTime = Convert.ToDateTime(row["EndTime"]),
                Status = row["Status"].ToString()
            };
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

                    _noteSchemaOk = true;
                }
                catch (Exception ex)
                {
                    _noteSchemaOk = false;
                    DatabaseHelper.TryLogThrottled(
                        throttleKey: "BookingQueryService.TryEnsureBookingNoteSchema",
                        eventDesc: "Booking Note Schema Ensure Failed",
                        ex: ex,
                        context: "BookingQueryService.TryEnsureBookingNoteSchema",
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
            try
            {
                object col = DatabaseHelper.ExecuteScalar("SELECT COL_LENGTH('dbo.Bookings', 'Note')");
                return !(col == null || col == DBNull.Value);
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLogThrottled(
                    throttleKey: "BookingQueryService.HasBookingNoteColumn",
                    eventDesc: "Booking Note Schema Check Failed",
                    ex: ex,
                    context: "BookingQueryService.HasBookingNoteColumn",
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
                    throttleKey: "BookingQueryService.HasBookingPaymentStateColumn",
                    eventDesc: "Booking PaymentState Schema Check Failed",
                    ex: ex,
                    context: "BookingQueryService.HasBookingPaymentStateColumn",
                    minSeconds: 300);
                return false;
            }
        }
    }
}