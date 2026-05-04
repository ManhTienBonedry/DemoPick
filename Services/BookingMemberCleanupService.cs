using DemoPick.Helpers;
using DemoPick.Data;
using System;
using System.Data.SqlClient;

namespace DemoPick.Services
{
    internal static class BookingMemberCleanupService
    {
        // Thu thuc hien Try Cleanup Orphan Walkin Member, neu du lieu khong hop le thi dung va tra thong bao phu hop.
        internal static void TryCleanupOrphanWalkinMember(int memberId)
        {
            if (memberId <= 0) return;

            try
            {
                DatabaseHelper.ExecuteNonQuery(
                    $@"UPDATE dbo.Bookings
SET MemberID = NULL
WHERE MemberID = @MemberID
  AND Status = '{AppConstants.BookingStatus.Cancelled}'",
                    new SqlParameter("@MemberID", memberId)
                );

                DatabaseHelper.ExecuteNonQuery(
                    $@"DELETE FROM dbo.Members
WHERE MemberID = @MemberID
  AND ISNULL(IsFixed, 0) = 0
  AND ISNULL(TotalSpent, 0) = 0
  AND ISNULL(TotalHoursPurchased, 0) = 0
  AND NOT EXISTS (
      SELECT 1
      FROM dbo.Bookings b
      WHERE b.MemberID = @MemberID
        AND b.Status <> '{AppConstants.BookingStatus.Cancelled}'
  )
  AND NOT EXISTS (
      SELECT 1
      FROM dbo.Invoices i
      WHERE i.MemberID = @MemberID
  )",
                    new SqlParameter("@MemberID", memberId)
                );
            }
            catch (Exception ex)
            {
                try { DatabaseHelper.TryLog("Cleanup Walkin Member Error", ex, "BookingMemberCleanupService.TryCleanupOrphanWalkinMember"); } catch { }
            }
        }
    }
}

