using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;

namespace DemoPick
{
    public partial class UCDatLich
    {
        // --- Cached GDI+ Objects ---
        private bool _cachedDarkTheme = false;
        private Pen _gridPen;
        private Font _axisFont;
        private Brush _axisBrush;
        private Brush _headerBgBrush;
        private Pen _headerBorderPen;
        private Brush _pastHourBrush;
        private Brush _courtNameBrush;
        private Brush _canvasBgBrush;
        private StringFormat _headerFormat;
        private StringFormat _courtFormat;

        private Font _titleFont;
        private Brush _titleBrush;
        private Font _timeFont;
        private Brush _timeBrush;
        private Font _noteFont;
        private Brush _noteBrush;
        private Font _payStateFont;
        private Brush _payStatePaidBrush;
        private Brush _payStateTransferBrush;
        private Brush _payStateDepositBrush;
        private Brush _payStateDefaultBrush;

        // Dam bao dieu kien Ensure Cached Gdi Objects da san sang truoc khi chay buoc xu ly tiep theo.
        private void EnsureCachedGdiObjects()
        {
            bool isDark = false;
            if (_gridPen != null && _cachedDarkTheme == isDark) return;

            DisposeGdiObjects();
            _cachedDarkTheme = isDark;

            _gridPen = new Pen(isDark ? Color.FromArgb(55, 63, 72) : Color.FromArgb(243, 244, 246), 1);
            _axisFont = new Font("Segoe UI", 9F, FontStyle.Bold);
            _axisBrush = new SolidBrush(isDark ? Color.FromArgb(170, 180, 190) : Color.FromArgb(107, 114, 128));
            _headerBgBrush = new SolidBrush(isDark ? Color.FromArgb(35, 42, 50) : Color.FromArgb(249, 250, 251));
            _headerBorderPen = new Pen(isDark ? Color.FromArgb(55, 63, 72) : Color.FromArgb(229, 231, 235), 1);
            _pastHourBrush = new SolidBrush(isDark ? Color.FromArgb(28, 33, 40) : Color.FromArgb(235, 235, 235));
            _courtNameBrush = new SolidBrush(isDark ? Color.FromArgb(220, 228, 235) : Color.FromArgb(26, 35, 50));
            _canvasBgBrush = new SolidBrush(isDark ? Color.FromArgb(30, 36, 44) : Color.White);

            _headerFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            _courtFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap };

            _titleFont = new Font("Segoe UI", 9F, FontStyle.Bold);
            _titleBrush = new SolidBrush(DemoPick.Helpers.UiTheme.TextPrimary);
            _timeFont = new Font("Segoe UI", 8.5F, FontStyle.Regular);
            _timeBrush = new SolidBrush(DemoPick.Helpers.UiTheme.TextSecondary);
            _noteFont = new Font("Segoe UI", 8.25F, FontStyle.Italic);
            _noteBrush = new SolidBrush(DemoPick.Helpers.UiTheme.TextSecondary);
            _payStateFont = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            _payStatePaidBrush = new SolidBrush(Color.FromArgb(22, 163, 74));
            _payStateTransferBrush = new SolidBrush(Color.FromArgb(37, 99, 235));
            _payStateDepositBrush = new SolidBrush(Color.FromArgb(234, 88, 12));
            _payStateDefaultBrush = new SolidBrush(Color.FromArgb(107, 114, 128));
        }

        // Xu ly logic man hinh Dispose Gdi Objects va cap nhat control lien quan.
        private void DisposeGdiObjects()
        {
            _gridPen?.Dispose();
            _axisFont?.Dispose();
            _axisBrush?.Dispose();
            _headerBgBrush?.Dispose();
            _headerBorderPen?.Dispose();
            _pastHourBrush?.Dispose();
            _courtNameBrush?.Dispose();
            _canvasBgBrush?.Dispose();
            _headerFormat?.Dispose();
            _courtFormat?.Dispose();
            _titleFont?.Dispose();
            _titleBrush?.Dispose();
            _timeFont?.Dispose();
            _timeBrush?.Dispose();
            _noteFont?.Dispose();
            _noteBrush?.Dispose();
            _payStateFont?.Dispose();
            _payStatePaidBrush?.Dispose();
            _payStateTransferBrush?.Dispose();
            _payStateDepositBrush?.Dispose();
            _payStateDefaultBrush?.Dispose();
        }

        // Dua du lieu Render Timeline Grid len giao dien hoac ve lai phan hien thi lien quan.
        private void RenderTimelineGrid(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            _bookingHits.Clear();
            EnsureCachedGdiObjects();

            int width = pnlCanvas.Width;
            int height = pnlCanvas.Height;
            int courtColWidth = CourtColWidth;
            int timeRowHeight = TimeHeaderHeight;
            int hoursToDraw = GridHoursToDraw;
            float hourWidth = (float)(width - courtColWidth) / hoursToDraw;

            // Clear canvas with theme-aware background
            g.FillRectangle(_canvasBgBrush, 0, 0, width, height);

            // Draw header background
            g.FillRectangle(_headerBgBrush, 0, 0, width, timeRowHeight);
            g.DrawLine(_headerBorderPen, 0, timeRowHeight, width, timeRowHeight);
            g.DrawLine(_headerBorderPen, courtColWidth, 0, courtColWidth, height);

            // Draw Time Headers
            for (int i = 0; i < hoursToDraw; i++)
            {
                int h = GridStartHour + i;
                float x = courtColWidth + (i * hourWidth);

                if (_currentDate.Date < DateTime.Now.Date ||
                   (_currentDate.Date == DateTime.Now.Date && h < DateTime.Now.Hour))
                {
                    g.FillRectangle(_pastHourBrush, x, timeRowHeight, hourWidth, height - timeRowHeight);
                }

                g.DrawLine(_gridPen, x, timeRowHeight, x, height);
                string timeStr = string.Format("{0:D2}:00", h);
                g.DrawString(timeStr, _axisFont, _axisBrush, new RectangleF(x, 0, hourWidth, timeRowHeight), _headerFormat);
            }

            try
            {
                string endLabel = "24:00";
                SizeF sEnd = g.MeasureString(endLabel, _axisFont);
                g.DrawString(endLabel, _axisFont, _axisBrush, width - sEnd.Width - 6, (timeRowHeight / 2f) - (sEnd.Height / 2f));
            }
            catch { }

            // Draw Court Rows
            var courts = _cachedCourts ?? new System.Collections.Generic.List<DemoPick.Models.CourtModel>();
            int rowHeight = CourtRowHeight;

            for (int i = 0; i < courts.Count; i++)
            {
                float y = timeRowHeight + (i * rowHeight);
                g.DrawLine(_gridPen, 0, y + rowHeight, width, y + rowHeight);

                var rect = new RectangleF(12, y, courtColWidth - 18, rowHeight);
                g.DrawString(courts[i].Name, _axisFont, _courtNameBrush, rect, _courtFormat);
            }

            // DRAW REAL BOOKINGS
            var bookings = _cachedBookings ?? new System.Collections.Generic.List<DemoPick.Models.BookingModel>();
            foreach (var b in bookings)
            {
                int cIdx = courts.FindIndex(c => c.CourtID == b.CourtID);
                if (cIdx < 0) continue;

                float startHour = b.StartTime.Hour + (b.StartTime.Minute / 60.0f);
                float durHours = (float)(b.EndTime - b.StartTime).TotalHours;

                Color bookingColor = Color.FromArgb(59, 130, 246); // Blue
                if (string.Equals(b.Status, AppConstants.BookingStatus.Confirmed, StringComparison.OrdinalIgnoreCase)) bookingColor = Color.FromArgb(76, 175, 80); // Green
                else if (string.Equals(b.Status, AppConstants.BookingStatus.Pending, StringComparison.OrdinalIgnoreCase)) bookingColor = Color.FromArgb(245, 158, 11); // Orange
                else if (string.Equals(b.Status, AppConstants.BookingStatus.Maintenance, StringComparison.OrdinalIgnoreCase)) bookingColor = Color.FromArgb(239, 68, 68); // Red

                bool selected = _selectedBooking != null && _selectedBooking.BookingID == b.BookingID;
                bool pendingSoon = IsPendingSoon(b);
                bool showNote = !string.Equals(b.Status, AppConstants.BookingStatus.Paid, StringComparison.OrdinalIgnoreCase);
                
                RectangleF rect = DrawBooking(g, courtColWidth, timeRowHeight, hourWidth, rowHeight, cIdx, startHour, durHours, b.GuestName, bookingColor, b.StartTime, b.EndTime, selected, pendingSoon, b.Note, showNote, b.PaymentState, b.Status);
                _bookingHits.Add(new BookingHitInfo { Rect = rect, Booking = b });
            }
        }

        // Dua du lieu Draw Booking len giao dien hoac ve lai phan hien thi lien quan.
        private RectangleF DrawBooking(Graphics g, float offsetX, float offsetY, float hourWidth, float rowHeight, int courtIndex, float startHour, float durationHours, string title, Color color, DateTime startTime, DateTime endTime, bool selected, bool pendingSoon, string note, bool showNote, string paymentState, string bookingStatus)
        {
            float gridStartHour = GridStartHour;
            float x = offsetX + ((startHour - gridStartHour) * hourWidth);
            float y = offsetY + (courtIndex * rowHeight) + 10;
            float w = (durationHours * hourWidth) - 4;
            float h = rowHeight - 20;

            var rect = new RectangleF(x, y, w, h);

            using (GraphicsPath path = GetRoundedRect(new RectangleF(x, y, w, h), 6))
            {
                int bgAlpha = pendingSoon && _pendingBlinkOn ? 70 : 40;
                int borderWidth = pendingSoon && _pendingBlinkOn ? 3 : 2;
                using (var bgBrush = new SolidBrush(Color.FromArgb(bgAlpha, color)))
                using (var borderPen = new Pen(color, borderWidth))
                {
                    g.FillPath(bgBrush, path);
                    g.DrawPath(borderPen, path);
                }

                if (selected)
                {
                    using (var selPen = new Pen(Color.FromArgb(26, 35, 50), 2))
                    {
                        g.DrawPath(selPen, path);
                    }
                }
            }

            using (var indicatorBrush = new SolidBrush(color))
            {
                g.FillRectangle(indicatorBrush, x + 2, y + 4, 3, h - 8);
            }

            string safeTitle = string.IsNullOrWhiteSpace(title) ? "(Không tên)" : title;
            string timeText = $"{startTime:HH:mm} - {endTime:HH:mm}";
            string safeNote = string.IsNullOrWhiteSpace(note) ? string.Empty : note.Trim();

            g.DrawString(safeTitle, _titleFont, _titleBrush, x + 10, y + 6);

            bool canShowNote = showNote && !string.IsNullOrWhiteSpace(safeNote);
            bool canShowThreeLines = h >= 56;

            if (canShowNote && !canShowThreeLines)
            {
                using (var noteFormat = new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap })
                {
                    var noteRect = new RectangleF(x + 10, y + 24, Math.Max(0, w - 14), 16);
                    g.DrawString(safeNote, _noteFont, _noteBrush, noteRect, noteFormat);
                }
            }
            else
            {
                g.DrawString(timeText, _timeFont, _timeBrush, x + 10, y + 26);

                if (canShowNote && canShowThreeLines)
                {
                    using (var noteFormat = new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap })
                    {
                        var noteRect = new RectangleF(x + 10, y + 40, Math.Max(0, w - 14), Math.Max(12, h - 42));
                        g.DrawString(safeNote, _noteFont, _noteBrush, noteRect, noteFormat);
                    }
                }
            }

            // Draw payment state badge (right-aligned inside the booking block)
            if (!string.Equals(bookingStatus, AppConstants.BookingStatus.Cancelled, StringComparison.OrdinalIgnoreCase))
            {
                string payLabel = GetPaymentStateLabel(paymentState, bookingStatus);
                Brush payBrush = GetPaymentStateBrush(paymentState, bookingStatus);
                SizeF paySize = g.MeasureString(payLabel, _payStateFont);
                float payX = x + w - paySize.Width - 8;
                float payY = y + 6;
                if (payX > x + 10 && paySize.Width < w - 20)
                {
                    g.DrawString(payLabel, _payStateFont, payBrush, payX, payY);
                }
            }

            return rect;
        }

        // Nap du lieu cho Get Payment State Label roi cap nhat lai trang thai hien thi tren man hinh.
        private static string GetPaymentStateLabel(string paymentState, string bookingStatus)
        {
            if (string.Equals(bookingStatus, AppConstants.BookingStatus.Paid, StringComparison.OrdinalIgnoreCase))
                return "Đã thanh toán";
            if (string.Equals(paymentState, AppConstants.BookingPaymentState.BankTransferred, StringComparison.OrdinalIgnoreCase))
                return "Đã CK";
            if (string.Equals(paymentState, AppConstants.BookingPaymentState.Deposit50, StringComparison.OrdinalIgnoreCase))
                return "Cọc 50%";
            return "Thu tại sân";
        }

        // Nap du lieu cho Get Payment State Brush roi cap nhat lai trang thai hien thi tren man hinh.
        private Brush GetPaymentStateBrush(string paymentState, string bookingStatus)
        {
            if (string.Equals(bookingStatus, AppConstants.BookingStatus.Paid, StringComparison.OrdinalIgnoreCase))
                return _payStatePaidBrush;
            if (string.Equals(paymentState, AppConstants.BookingPaymentState.BankTransferred, StringComparison.OrdinalIgnoreCase))
                return _payStateTransferBrush;
            if (string.Equals(paymentState, AppConstants.BookingPaymentState.Deposit50, StringComparison.OrdinalIgnoreCase))
                return _payStateDepositBrush;
            return _payStateDefaultBrush;
        }

        // Kiem tra dieu kien Is Pending Soon va tra ve ket qua dung/sai cho luong xu ly.
        private bool IsPendingSoon(DemoPick.Models.BookingModel booking)
        {
            if (booking == null) return false;
            if (!string.Equals(booking.Status, AppConstants.BookingStatus.Pending, StringComparison.OrdinalIgnoreCase)) return false;

            DateTime now = DateTime.Now;
            if (booking.StartTime < now) return false;

            return (booking.StartTime - now).TotalMinutes <= PendingBlinkWindowMinutes;
        }

        // Nap du lieu cho Get Rounded Rect roi cap nhat lai trang thai hien thi tren man hinh.
        private GraphicsPath GetRoundedRect(RectangleF bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            RectangleF arc = new RectangleF(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}


