using DemoPick.Helpers;
using DemoPick.Data;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DemoPick.Helpers
{
    internal static class PosMemberResolver
    {
        // Dien giai hoac xac dinh Resolve Member For Checkout tu du lieu dau vao de cac buoc sau dung thong nhat.
        internal static int ResolveMemberForCheckout(SqlConnection conn, SqlTransaction tran, int memberId, int creditedBookingId)
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
            ) ?? string.Empty;

            PosGuestInfoParser.ParseGuestInfo(guestNameRaw, out var fullName, out var phone);
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
    }
}


