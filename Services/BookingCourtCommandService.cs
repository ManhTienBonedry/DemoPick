using DemoPick.Helpers;
using DemoPick.Data;
using System;
using System.Data.SqlClient;

namespace DemoPick.Services
{
    internal static class BookingCourtCommandService
    {
        // Xoa, huy hoac dat lai du lieu Deactivate Court theo dung dieu kien nghiep vu.
        internal static void DeactivateCourt(int courtId)
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
    }
}
