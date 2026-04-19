using System;
using System.Collections.Generic;
using DemoPick.Models;
using DemoPick.Services;

namespace DemoPick.Controllers
{
    public class BookingController
    {
        public int? GetOrCreateMemberId(string fullName, string phone)
        {
            return BookingMemberService.GetOrCreateMemberId(fullName, phone);
        }

        public List<CourtModel> GetCourts()
        {
            return BookingCourtQueryService.GetActiveCourts();
        }

        public List<BookingModel> GetBookingsByDate(DateTime date)
        {
            return BookingQueryService.GetBookingsByDate(date);
        }

        public List<BookingModel> GetUnpaidBookingsUntil(DateTime toDateInclusive)
        {
            return BookingQueryService.GetUnpaidBookingsUntil(toDateInclusive);
        }

        public void SubmitBooking(int courtId, string guestName, DateTime startTime, DateTime endTime)
        {
            SubmitBooking(courtId, guestName, note: null, startTime: startTime, endTime: endTime, status: AppConstants.BookingStatus.Confirmed, paymentState: null);
        }

        public void SubmitBooking(int courtId, string guestName, DateTime startTime, DateTime endTime, string status)
        {
            SubmitBooking(courtId, guestName, note: null, startTime: startTime, endTime: endTime, status: status, paymentState: null);
        }

        public void SubmitBooking(int courtId, string guestName, string note, DateTime startTime, DateTime endTime, string status, string paymentState = null)
        {
            SubmitBooking(courtId, memberId: null, guestName: guestName, note: note, startTime: startTime, endTime: endTime, status: status, paymentState: paymentState);
        }

        public void SubmitBooking(int courtId, int? memberId, string guestName, string note, DateTime startTime, DateTime endTime, string status, string paymentState = null)
        {
            BookingWriteService.SubmitBooking(courtId, memberId, guestName, note, startTime, endTime, status, paymentState);
        }

        public void UpdateBookingTime(int bookingId, DateTime newStartTime, DateTime newEndTime)
        {
            BookingWriteService.UpdateBookingTime(bookingId, newStartTime, newEndTime);
        }

        public void UpdateBookingTimeAndNote(int bookingId, DateTime newStartTime, DateTime newEndTime, string note)
        {
            BookingWriteService.UpdateBookingTimeAndNote(bookingId, newStartTime, newEndTime, note);
        }

        public void DeactivateCourt(int courtId)
        {
            BookingCourtCommandService.DeactivateCourt(courtId);
        }

        public void MarkBookingAsPending(int bookingId)
        {
            BookingWriteService.MarkBookingAsPending(bookingId);
        }

        public void CancelBooking(int bookingId)
        {
            BookingWriteService.CancelBooking(bookingId);
        }
    }
}