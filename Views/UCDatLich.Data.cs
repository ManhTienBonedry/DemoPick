using DemoPick.Helpers;
using DemoPick.Data;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoPick
{
    public partial class UCDatLich
    {
        // Nap du lieu cho Reload Timeline roi cap nhat lai trang thai hien thi tren man hinh.
        private void ReloadTimelineAsync(bool forceReload)
        {
            if (!forceReload && _cacheDate.Date == _currentDate.Date && _cachedCourts != null && _cachedBookings != null)
            {
                RefreshTimelineLayoutOnly();
                return;
            }

            int seq = ++_reloadSeq;
            DateTime date = _currentDate.Date;

            Task.Run(() =>
            {
                System.Collections.Generic.List<DemoPick.Models.CourtModel> courts;
                System.Collections.Generic.List<DemoPick.Models.BookingModel> bookings;

                try { courts = _controller.GetCourts() ?? new System.Collections.Generic.List<DemoPick.Models.CourtModel>(); }
                catch (Exception ex)
                {
                    DemoPick.Data.DatabaseHelper.TryLogThrottled(
                        throttleKey: "UCDatLich.ReloadCourts",
                        eventDesc: "Timeline Load Courts Error",
                        ex: ex,
                        context: "UCDatLich.ReloadTimelineAsync",
                        minSeconds: 60);
                    courts = new System.Collections.Generic.List<DemoPick.Models.CourtModel>();
                }

                try { bookings = _controller.GetBookingsByDate(date) ?? new System.Collections.Generic.List<DemoPick.Models.BookingModel>(); }
                catch (Exception ex)
                {
                    DemoPick.Data.DatabaseHelper.TryLogThrottled(
                        throttleKey: "UCDatLich.ReloadBookings",
                        eventDesc: "Timeline Load Bookings Error",
                        ex: ex,
                        context: "UCDatLich.ReloadTimelineAsync",
                        minSeconds: 60);
                    bookings = new System.Collections.Generic.List<DemoPick.Models.BookingModel>();
                }

                return new Tuple<System.Collections.Generic.List<DemoPick.Models.CourtModel>, System.Collections.Generic.List<DemoPick.Models.BookingModel>>(courts, bookings);
            }).ContinueWith(t =>
            {
                try
                {
                    if (IsDisposed) return;
                    if (!IsHandleCreated) return;

                    BeginInvoke((MethodInvoker)(() =>
                    {
                        try
                        {
                            if (IsDisposed) return;
                            if (seq != _reloadSeq) return;
                            if (_currentDate.Date != date) return;

                            if (t.IsFaulted || t.IsCanceled)
                            {
                                _cachedCourts = new System.Collections.Generic.List<DemoPick.Models.CourtModel>();
                                _cachedBookings = new System.Collections.Generic.List<DemoPick.Models.BookingModel>();
                            }
                            else
                            {
                                _cachedCourts = t.Result.Item1 ?? new System.Collections.Generic.List<DemoPick.Models.CourtModel>();
                                _cachedBookings = t.Result.Item2 ?? new System.Collections.Generic.List<DemoPick.Models.BookingModel>();
                            }

                            _cacheDate = date;

                            // If selected booking no longer exists, clear selection.
                            if (_selectedBooking != null)
                            {
                                bool stillExists = _cachedBookings.Exists(b => b.BookingID == _selectedBooking.BookingID);
                                if (!stillExists) _selectedBooking = null;
                            }

                            RefreshTimelineLayoutOnly();
                        }
                        catch
                        {
                            // ignore
                        }
                    }));
                }
                catch
                {
                    // ignore
                }
            }, TaskScheduler.Default);
        }
    }
}

