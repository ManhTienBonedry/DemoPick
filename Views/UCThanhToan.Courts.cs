using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;
using Panel = System.Windows.Forms.Panel;

namespace DemoPick
{
    public partial class UCThanhToan
    {
        private enum BookingDisplayState
        {
            Upcoming,
            Active,
            Overdue,
            OrderOnly
        }

        private sealed class CheckoutCourtRow
        {
            public DemoPick.Models.CourtModel Court;
            public DemoPick.Models.BookingModel Booking;
            public BookingDisplayState State;
            public int PendingOrderCount;
        }

        private static readonly Font _checkoutCourtNameFont = new Font("Segoe UI", 11F, FontStyle.Bold);
        private static readonly Font _checkoutCourtBadgeFont = new Font("Segoe UI", 9F, FontStyle.Bold);
        private static readonly Font _checkoutCourtInfoFont = new Font("Segoe UI", 9F, FontStyle.Italic);
        private static readonly Font _checkoutEmptyStateFont = new Font("Segoe UI", 11F, FontStyle.Italic);

        // Xoa, huy hoac dat lai du lieu Clear And Dispose Child Controls theo dung dieu kien nghiep vu.
        private static void ClearAndDisposeChildControls(Control parent)
        {
            if (parent == null) return;
            if (parent.Controls == null) return;
            if (parent.Controls.Count == 0) return;

            var old = new Control[parent.Controls.Count];
            parent.Controls.CopyTo(old, 0);
            parent.Controls.Clear();

            for (int i = 0; i < old.Length; i++)
            {
                old[i].Dispose();
            }
        }

        // Nap du lieu cho Load Courts roi cap nhat lai trang thai hien thi tren man hinh.
        private void LoadCourts()
        {
            try
            {
                ClearAndDisposeChildControls(flpCourts);

                var bookCtrl = new DemoPick.Controllers.BookingController();
                DateTime now = DateTime.Now;

                var courts = bookCtrl.GetCourts();
                var courtsById = new Dictionary<int, DemoPick.Models.CourtModel>();
                foreach (var c in courts)
                {
                    if (!courtsById.ContainsKey(c.CourtID))
                        courtsById.Add(c.CourtID, c);
                }

                var unpaidBookings = bookCtrl.GetUnpaidBookingsUntil(now);

                var rows = new List<CheckoutCourtRow>();
                var courtsWithBookingRow = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var booking in unpaidBookings)
                {
                    if (!courtsById.TryGetValue(booking.CourtID, out var court))
                        continue;

                    int pendingOrderCount = PosService.GetPendingOrder(court.Name).Count;
                    rows.Add(new CheckoutCourtRow
                    {
                        Court = court,
                        Booking = booking,
                        State = ResolveBookingState(booking, now),
                        PendingOrderCount = pendingOrderCount
                    });

                    if (!string.IsNullOrWhiteSpace(court.Name))
                        courtsWithBookingRow.Add(court.Name.Trim());
                }

                // Keep compatibility: still show pending POS orders even when no booking debt exists until today.
                foreach (var court in courts)
                {
                    string name = (court.Name ?? string.Empty).Trim();
                    int pendingOrderCount = PosService.GetPendingOrder(court.Name).Count;
                    if (pendingOrderCount <= 0) continue;
                    if (courtsWithBookingRow.Contains(name)) continue;

                    rows.Add(new CheckoutCourtRow
                    {
                        Court = court,
                        Booking = null,
                        State = BookingDisplayState.OrderOnly,
                        PendingOrderCount = pendingOrderCount
                    });
                }

                foreach (var row in rows)
                {
                    AddCheckoutCourtPanel(row, unpaidBookings, now);
                }

                if (flpCourts.Controls.Count == 0)
                {
                    flpCourts.Controls.Add(new Label
                    {
                        Text = "Không có booking chưa thanh toán đến hôm nay",
                        Font = _checkoutEmptyStateFont,
                        AutoSize = true,
                        Margin = new Padding(20),
                        ForeColor = Color.Gray
                    });
                }

                UiTheme.NormalizeTextBackgrounds(flpCourts);
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("ThanhToan Load Courts Error", ex, "UCThanhToan.LoadCourts");
            }
        }

        // Xu ly logic man hinh Add Checkout Court Panel va cap nhat control lien quan.
        private void AddCheckoutCourtPanel(CheckoutCourtRow row, List<DemoPick.Models.BookingModel> unpaidBookings, DateTime now)
        {
            if (row == null || row.Court == null) return;

            Color lineColor = GetStateColor(row.State);
            string statusText = GetStateText(row);
            bool showReceiveButton = row.Booking != null && row.State == BookingDisplayState.Upcoming;

            Panel pnlCtx = new Panel
            {
                Size = new Size(240, showReceiveButton ? 112 : 96),
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 10),
                Cursor = Cursors.Hand
            };
            pnlCtx.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(229, 231, 235), 1))
                using (var brush = new SolidBrush(lineColor))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, pnlCtx.Width - 1, pnlCtx.Height - 1);
                    e.Graphics.FillRectangle(brush, 0, 10, 4, pnlCtx.Height - 20);
                }
            };

            string courtName = string.IsNullOrWhiteSpace(row.Court.Name) ? "(Không tên sân)" : row.Court.Name;
            Label lblCourt = new Label
            {
                Text = courtName,
                Font = _checkoutCourtNameFont,
                ForeColor = Color.FromArgb(26, 35, 50),
                Location = new Point(15, 10),
                AutoSize = true
            };

            Label lblBadge = new Label
            {
                Text = statusText,
                Font = _checkoutCourtBadgeFont,
                ForeColor = Color.White,
                BackColor = lineColor,
                Location = new Point(130, 12),
                AutoSize = true,
                Padding = new Padding(3, 1, 3, 1)
            };

            string timeText;
            if (row.Booking == null)
            {
                timeText = "Không có booking đến hôm nay";
            }
            else
            {
                string dayText = row.Booking.StartTime.Date == now.Date
                    ? "Hôm nay"
                    : row.Booking.StartTime.ToString("dd/MM");
                timeText = $"Ca: {dayText} {row.Booking.StartTime:HH:mm} - {row.Booking.EndTime:HH:mm}";
            }

            Label lblTime = new Label
            {
                Text = timeText,
                Font = _checkoutCourtInfoFont,
                ForeColor = Color.FromArgb(75, 85, 99),
                Location = new Point(15, 38),
                AutoSize = true
            };

            string guest = row.Booking == null
                ? ""
                : (string.IsNullOrWhiteSpace(row.Booking.GuestName) ? "Khách lẻ" : row.Booking.GuestName);

            string orderInfo = row.PendingOrderCount > 0
                ? $"Order chờ: {row.PendingOrderCount} món"
                : "Chưa có order";

            string detail = row.Booking == null
                ? orderInfo
                : $"{guest} • {orderInfo}";

            Label lblDetail = new Label
            {
                Text = detail,
                Font = _checkoutCourtInfoFont,
                ForeColor = Color.Gray,
                Location = new Point(15, 58),
                AutoSize = true
            };

            pnlCtx.Controls.Add(lblCourt);
            pnlCtx.Controls.Add(lblBadge);
            pnlCtx.Controls.Add(lblTime);
            pnlCtx.Controls.Add(lblDetail);
            UiTheme.NormalizeTextBackgrounds(pnlCtx);

            EventHandler selectCourt = (s, e) =>
            {
                foreach (Control p in flpCourts.Controls)
                {
                    if (p is Panel panel) panel.BackColor = Color.White;
                }

                pnlCtx.BackColor = Color.FromArgb(235, 248, 235);
                SelectCourtToCheckout(row.Court, row.Booking);
            };

            pnlCtx.Click += selectCourt;
            lblCourt.Click += selectCourt;
            lblBadge.Click += selectCourt;
            lblTime.Click += selectCourt;
            lblDetail.Click += selectCourt;

            if (showReceiveButton)
            {
                var btnReceive = new Button
                {
                    Text = "Nhận sân",
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(34, 197, 94),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    Size = new Size(90, 24),
                    Location = new Point(136, 82),
                    Cursor = Cursors.Hand
                };
                btnReceive.FlatAppearance.BorderSize = 0;

                btnReceive.Click += (s, e) =>
                {
                    TryReceiveCourt(row, unpaidBookings, now);
                };

                pnlCtx.Controls.Add(btnReceive);
            }

            flpCourts.Controls.Add(pnlCtx);
        }

        // Thu thuc hien Try Receive Court, neu du lieu khong hop le thi dung va tra thong bao phu hop.
        private void TryReceiveCourt(CheckoutCourtRow row, List<DemoPick.Models.BookingModel> unpaidBookings, DateTime now)
        {
            if (row == null || row.Court == null || row.Booking == null)
                return;

            bool hasOverdue = HasOverdueUnpaidBookingOnSameCourt(unpaidBookings, row.Court.CourtID, row.Booking.BookingID, now);
            if (hasOverdue)
            {
                var confirm = MessageBox.Show(
                    "Sân này còn booking chưa thanh toán. Xác nhận tiếp tục?",
                    "Xác nhận nhận sân",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (confirm != DialogResult.Yes)
                    return;
            }

            try
            {
                var ctrl = new DemoPick.Controllers.BookingController();
                ctrl.MarkBookingAsPending(row.Booking.BookingID);

                MessageBox.Show(
                    "Đã nhận sân thành công. Booking cũ chưa thu vẫn được giữ lại để thanh toán sau.",
                    "Nhận sân thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LoadCourts();

                var refreshed = ctrl.GetBookingsByDate(DateTime.Now)
                    .Find(b => b.BookingID == row.Booking.BookingID);
                SelectCourtToCheckout(row.Court, refreshed);
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("ThanhToan ReceiveCourt Error", ex, "UCThanhToan.TryReceiveCourt");
                MessageBox.Show("Không thể nhận sân: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Kiem tra dieu kien Has Overdue Unpaid Booking On Same Court va tra ve ket qua dung/sai cho luong xu ly.
        private static bool HasOverdueUnpaidBookingOnSameCourt(List<DemoPick.Models.BookingModel> bookings, int courtId, int exceptBookingId, DateTime now)
        {
            if (bookings == null || bookings.Count == 0) return false;

            for (int i = 0; i < bookings.Count; i++)
            {
                var b = bookings[i];
                if (b == null) continue;
                if (b.CourtID != courtId) continue;
                if (b.BookingID == exceptBookingId) continue;
                if (ShouldIgnoreForCheckout(b.Status)) continue;

                if (now > b.EndTime)
                    return true;
            }

            return false;
        }

        // Kiem tra dieu kien Should Ignore For Checkout va tra ve ket qua dung/sai cho luong xu ly.
        private static bool ShouldIgnoreForCheckout(string status)
        {
            return string.Equals(status, AppConstants.BookingStatus.Paid, StringComparison.OrdinalIgnoreCase)
                || string.Equals(status, AppConstants.BookingStatus.Cancelled, StringComparison.OrdinalIgnoreCase)
                || string.Equals(status, AppConstants.BookingStatus.Maintenance, StringComparison.OrdinalIgnoreCase);
        }

        // Dien giai hoac xac dinh Resolve Booking State tu du lieu dau vao de cac buoc sau dung thong nhat.
        private static BookingDisplayState ResolveBookingState(DemoPick.Models.BookingModel booking, DateTime now)
        {
            if (booking == null) return BookingDisplayState.OrderOnly;

            if (now > booking.EndTime)
                return BookingDisplayState.Overdue;

            string status = (booking.Status ?? string.Empty).Trim();
            if (string.Equals(status, AppConstants.BookingStatus.Pending, StringComparison.OrdinalIgnoreCase))
                return BookingDisplayState.Active;

            if (now < booking.StartTime)
                return BookingDisplayState.Upcoming;

            return BookingDisplayState.Active;
        }

        // Nap du lieu cho Get State Text roi cap nhat lai trang thai hien thi tren man hinh.
        private static string GetStateText(CheckoutCourtRow row)
        {
            switch (row.State)
            {
                case BookingDisplayState.Upcoming:
                    return "Chưa tới giờ";
                case BookingDisplayState.Active:
                    if (row.Booking != null && string.Equals(row.Booking.Status, AppConstants.BookingStatus.Pending, StringComparison.OrdinalIgnoreCase))
                        return "Đã nhận sân";
                    return "Đang chơi";
                case BookingDisplayState.Overdue:
                    return "Quá giờ chưa thu";
                case BookingDisplayState.OrderOnly:
                default:
                    return "Có order";
            }
        }

        // Nap du lieu cho Get State Color roi cap nhat lai trang thai hien thi tren man hinh.
        private static Color GetStateColor(BookingDisplayState state)
        {
            switch (state)
            {
                case BookingDisplayState.Upcoming:
                    return Color.FromArgb(34, 197, 94);   // Green
                case BookingDisplayState.Active:
                    return Color.FromArgb(245, 158, 11);  // Yellow/Amber
                case BookingDisplayState.Overdue:
                    return Color.FromArgb(239, 68, 68);   // Red
                case BookingDisplayState.OrderOnly:
                default:
                    return Color.FromArgb(59, 130, 246);  // Blue
            }
        }

        // Xu ly logic man hinh Select Court To Checkout va cap nhat control lien quan.
        private void SelectCourtToCheckout(DemoPick.Models.CourtModel court, DemoPick.Models.BookingModel booking)
        {
            string courtName = court.Name;
            _selectedCourtName = courtName;

            if (booking != null)
            {
                lblRightTitle.Text = $"Hóa đơn - {courtName} ({booking.StartTime:HH:mm}-{booking.EndTime:HH:mm})";
            }
            else
            {
                lblRightTitle.Text = "Hóa đơn - " + courtName;
            }

            _currentBooking = booking;
            _selectedCourt = court;
            _cartTotal = 0;

            UpdatePaymentStateHint();
            UpdateTotals();
            ReloadPaymentHistory();
        }

        // Cap nhat lai du lieu/trang thai Update Payment State Hint tren man hinh hien tai.
        private void UpdatePaymentStateHint()
        {
            if (lblPaymentStateHint == null) return;

            if (_currentBooking == null)
            {
                lblPaymentStateHint.Text = "Không có booking: chỉ thu order chờ/dịch vụ.";
                lblPaymentStateHint.ForeColor = Color.FromArgb(55, 65, 81);
                return;
            }

            string paymentState = (_currentBooking.PaymentState ?? string.Empty).Trim();
            if (string.Equals(paymentState, AppConstants.BookingPaymentState.BankTransferred, StringComparison.OrdinalIgnoreCase))
            {
                lblPaymentStateHint.Text = "Đã chuyển khoản sân: chỉ thu dịch vụ phát sinh.";
                lblPaymentStateHint.ForeColor = Color.FromArgb(22, 163, 74);
                return;
            }

            if (string.Equals(paymentState, AppConstants.BookingPaymentState.Deposit50, StringComparison.OrdinalIgnoreCase))
            {
                lblPaymentStateHint.Text = "Đã đặt cọc 50%: thu 50% tiền sân còn lại + dịch vụ.";
                lblPaymentStateHint.ForeColor = Color.FromArgb(234, 88, 12);
                return;
            }

            lblPaymentStateHint.Text = "Thu tại sân: thu đủ tiền sân + dịch vụ.";
            lblPaymentStateHint.ForeColor = Color.FromArgb(37, 99, 235);
        }

        // Xoa, huy hoac dat lai du lieu Reset Checkout Pane theo dung dieu kien nghiep vu.
        private void ResetCheckoutPane()
        {
            _cartTotal = 0;
            _currentDiscountPct = 0;
            _currentCustomerId = 0;
            _isFixedCustomer = false;
            _currentBooking = null;
            _selectedCourt = null;
            txtCustomerPhone.Text = "";
            lblCustomerInfo.Text = "Khách lẻ (Không áp dụng thẻ)";
            lblCustomerInfo.ForeColor = Color.Gray;
            lstCart.Items.Clear();
            lblRightTitle.Text = "Hóa đơn thanh toán";
            _selectedCourtName = "";
            _lastDiscountAmount = 0m;
            _lastFinalTotal = 0m;
            UpdatePaymentStateHint();
            UpdateTotals();
            ReloadPaymentHistory();
        }
    }
}


