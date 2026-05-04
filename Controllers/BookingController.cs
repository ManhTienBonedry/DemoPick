using System;
using System.Collections.Generic;
using DemoPick.Models;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;

namespace DemoPick.Controllers
{
    public class BookingController
    {
        // Lay du lieu/ket qua cho Get Or Create Member Id tu tang xu ly phu hop.
        public int? GetOrCreateMemberId(string fullName, string phone)
        {
            return BookingMemberService.GetOrCreateMemberId(fullName, phone);
        }

        // Lay du lieu/ket qua cho Get Courts tu tang xu ly phu hop.
        public List<CourtModel> GetCourts()
        {
            return BookingCourtQueryService.GetActiveCourts();
        }

        // Lay du lieu/ket qua cho Get Bookings By Date tu tang xu ly phu hop.
        public List<BookingModel> GetBookingsByDate(DateTime date)
        {
            return BookingQueryService.GetBookingsByDate(date);
        }

        // Lay du lieu/ket qua cho Get Unpaid Bookings Until tu tang xu ly phu hop.
        public List<BookingModel> GetUnpaidBookingsUntil(DateTime toDateInclusive)
        {
            return BookingQueryService.GetUnpaidBookingsUntil(toDateInclusive);
        }

        // Luu hoac ghi nhan Submit Booking vao trang thai he thong/CSDL khi nghiep vu yeu cau.
        public void SubmitBooking(int courtId, string guestName, DateTime startTime, DateTime endTime)
        {
            SubmitBooking(courtId, guestName, note: null, startTime: startTime, endTime: endTime, status: AppConstants.BookingStatus.Confirmed, paymentState: null);
        }

        // Luu hoac ghi nhan Submit Booking vao trang thai he thong/CSDL khi nghiep vu yeu cau.
        public void SubmitBooking(int courtId, string guestName, DateTime startTime, DateTime endTime, string status)
        {
            SubmitBooking(courtId, guestName, note: null, startTime: startTime, endTime: endTime, status: status, paymentState: null);
        }

        // Luu hoac ghi nhan Submit Booking vao trang thai he thong/CSDL khi nghiep vu yeu cau.
        public void SubmitBooking(int courtId, string guestName, string note, DateTime startTime, DateTime endTime, string status, string paymentState = null)
        {
            SubmitBooking(courtId, memberId: null, guestName: guestName, note: note, startTime: startTime, endTime: endTime, status: status, paymentState: paymentState);
        }

        // Luu hoac ghi nhan Submit Booking vao trang thai he thong/CSDL khi nghiep vu yeu cau.
        public void SubmitBooking(int courtId, int? memberId, string guestName, string note, DateTime startTime, DateTime endTime, string status, string paymentState = null)
        {
            BookingWriteService.SubmitBooking(courtId, memberId, guestName, note, startTime, endTime, status, paymentState);
        }

        // Cap nhat du lieu hoac trang thai Update Booking Time theo quy tac nghiep vu.
        public void UpdateBookingTime(int bookingId, DateTime newStartTime, DateTime newEndTime)
        {
            BookingWriteService.UpdateBookingTime(bookingId, newStartTime, newEndTime);
        }

        // Cap nhat du lieu hoac trang thai Update Booking Time And Note theo quy tac nghiep vu.
        public void UpdateBookingTimeAndNote(int bookingId, DateTime newStartTime, DateTime newEndTime, string note)
        {
            BookingWriteService.UpdateBookingTimeAndNote(bookingId, newStartTime, newEndTime, note);
        }

        // Xoa, huy hoac dat lai du lieu Deactivate Court theo dung dieu kien nghiep vu.
        public void DeactivateCourt(int courtId)
        {
            BookingCourtCommandService.DeactivateCourt(courtId);
        }

        // Luu hoac ghi nhan Mark Booking As Pending vao trang thai he thong/CSDL khi nghiep vu yeu cau.
        public void MarkBookingAsPending(int bookingId)
        {
            BookingWriteService.MarkBookingAsPending(bookingId);
        }

        // Xoa, huy hoac dat lai du lieu Cancel Booking theo dung dieu kien nghiep vu.
        public void CancelBooking(int bookingId)
        {
            BookingWriteService.CancelBooking(bookingId);
        }
    }
}

