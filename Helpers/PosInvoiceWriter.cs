using DemoPick.Helpers;
using DemoPick.Data;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DemoPick.Helpers
{
    internal static class PosInvoiceWriter
    {
        // Luu hoac ghi nhan Insert Invoice vao trang thai he thong/CSDL khi nghiep vu yeu cau.
        internal static int InsertInvoice(SqlConnection conn, SqlTransaction tran, int memberId, decimal totalAmount, decimal discountAmount, decimal finalAmount, string paymentMethod)
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

        // Luu hoac ghi nhan Insert Invoice Detail vao trang thai he thong/CSDL khi nghiep vu yeu cau.
        internal static void InsertInvoiceDetail(SqlConnection conn, SqlTransaction tran, int invoiceId, int? productId, int? bookingId, int qty, decimal unitPrice)
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
    }
}


