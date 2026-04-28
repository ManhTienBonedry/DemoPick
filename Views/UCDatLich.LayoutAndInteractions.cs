using System;
using System.Drawing;
using System.Windows.Forms;

namespace DemoPick
{
    public partial class UCDatLich
    {
        private void SetZoom(float zoom)
        {
            float clamped = zoom;
            if (clamped < ZoomMin) clamped = ZoomMin;
            if (clamped > ZoomMax) clamped = ZoomMax;
            if (Math.Abs(clamped - _zoom) < 0.0001f) return;

            _zoom = clamped;
            UpdateZoomButtons();
            RefreshTimelineLayoutOnly();
        }

        private void UpdateZoomButtons()
        {
            if (btnZoomOut != null)
            {
                btnZoomOut.Enabled = _zoom > ZoomMin + 0.0001f;
            }
            if (btnZoomIn != null)
            {
                btnZoomIn.Enabled = _zoom < ZoomMax - 0.0001f;
            }
        }

        private void RefreshTimelineLayoutOnly()
        {
            try
            {
                // Resize canvas so we can scroll vertically when there are many courts.
                int courtCount = _cachedCourts == null ? 0 : _cachedCourts.Count;
                int requiredHeight = TimeHeaderHeight + (courtCount * CourtRowHeight) + 2;
                int visibleWidth = pnlTimelineContainer.ClientSize.Width;

                // Predict vertical scrollbar: it may not be reported as Visible until after we resize.
                bool willShowVScroll = requiredHeight > pnlTimelineContainer.ClientSize.Height;
                if (willShowVScroll)
                    visibleWidth = Math.Max(0, visibleWidth - SystemInformation.VerticalScrollBarWidth);

                // Fit-to-width baseline hour width, then apply zoom.
                float baseHourWidth = (float)Math.Max(1, visibleWidth - CourtColWidth) / GridHoursToDraw;
                float hourWidth = baseHourWidth * _zoom;
                int zoomedWidth = (int)Math.Ceiling(CourtColWidth + (GridHoursToDraw * hourWidth));
                int requiredWidth = Math.Max(300, Math.Max(visibleWidth, zoomedWidth));

                if (pnlCanvas.Width != requiredWidth)
                    pnlCanvas.Width = requiredWidth;
                if (pnlCanvas.Height != requiredHeight)
                    pnlCanvas.Height = requiredHeight;
            }
            catch
            {
                // Best effort.
            }

            pnlCanvas.Invalidate();
        }

        private void PnlCanvas_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (TryShowBookingContextMenu(e.Location))
                    return;

                TryShowCourtContextMenu(e.Location);
                return;
            }

            if (e.Button != MouseButtons.Left) return;

            // Select booking under cursor (if any)
            for (int i = _bookingHits.Count - 1; i >= 0; i--)
            {
                var hit = _bookingHits[i];
                if (hit?.Booking == null) continue;
                if (!hit.Rect.Contains(e.Location)) continue;
                _selectedBooking = hit.Booking;
                pnlCanvas.Invalidate();
                return;
            }

            // Single click on timeline row: first click prompts, second click opens "Đặt Sân Chơi".
            if (TryGetCourtAtPoint(e.Location, requireTimelineArea: true, out var clickedCourt))
            {
                if (_selectedBooking != null)
                {
                    _selectedBooking = null;
                    pnlCanvas.Invalidate();
                }

                HandleCourtSingleClick(clickedCourt);
                return;
            }

            // Clicked empty area
            if (_selectedBooking != null)
            {
                _selectedBooking = null;
                pnlCanvas.Invalidate();
            }
        }

        private bool TryShowBookingContextMenu(Point canvasPoint)
        {
            try
            {
                for (int i = _bookingHits.Count - 1; i >= 0; i--)
                {
                    var hit = _bookingHits[i];
                    if (hit?.Booking == null) continue;
                    if (!hit.Rect.Contains(canvasPoint)) continue;

                    var booking = hit.Booking;

                    var menu = new ContextMenuStrip();
                    var miDeleteBooking = new ToolStripMenuItem("Xóa đặt sân");
                    miDeleteBooking.Click += (s, e) =>
                    {
                        if (!CanReschedule()) return;

                        string customer = string.IsNullOrWhiteSpace(booking.GuestName) ? "Khách lẻ" : booking.GuestName;
                        var confirm = MessageBox.Show(
                            $"Bạn chắc chắn muốn xóa booking của '{customer}' lúc {booking.StartTime:HH:mm} - {booking.EndTime:HH:mm}?",
                            "Xác nhận xóa booking",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning
                        );

                        if (confirm != DialogResult.Yes) return;

                        try
                        {
                            _controller.CancelBooking(booking.BookingID);
                            _selectedBooking = null;
                            ReloadTimelineAsync(forceReload: true);
                            MessageBox.Show("Đã xóa booking thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            try { DemoPick.Services.DatabaseHelper.TryLog("CancelBooking Error", ex, "UCDatLich"); } catch { }
                            MessageBox.Show(ex.Message, "Không thể xóa booking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    };

                    menu.Items.Add(miDeleteBooking);
                    menu.Show(pnlCanvas, canvasPoint);
                    return true;
                }
            }
            catch
            {
                // ignore
            }

            return false;
        }

        private bool CanManageCourts()
        {
            try
            {
                bool ok = DemoPick.Services.AppSession.IsInRole(DemoPick.Services.AppConstants.Roles.Admin);
                if (!ok)
                {
                    MessageBox.Show("Chỉ Admin mới có quyền xóa sân.", "Không có quyền", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                DemoPick.Services.DatabaseHelper.TryLogThrottled(
                    throttleKey: "UCDatLich.CanManageCourts",
                    eventDesc: "Role Check Error",
                    ex: ex,
                    context: "UCDatLich.CanManageCourts",
                    minSeconds: 300);
                MessageBox.Show("Không thể xác định quyền người dùng. Vui lòng đăng nhập lại.", "Không có quyền", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }

        private void TryShowCourtContextMenu(Point canvasPoint)
        {
            try
            {
                if (!TryGetCourtAtPoint(canvasPoint, requireTimelineArea: false, out var court)) return;

                var menu = new ContextMenuStrip();
                var miDelete = new ToolStripMenuItem("Xóa sân");
                miDelete.Click += (s, e) => DeleteCourtWithConfirmation(court);

                menu.Items.Add(miDelete);
                menu.Show(pnlCanvas, canvasPoint);
            }
            catch
            {
                // ignore
            }
        }

        private bool CanReschedule()
        {
            try
            {
                // Only Staff/Admin can reschedule.
                bool ok = DemoPick.Services.AppSession.IsInRole(DemoPick.Services.AppConstants.Roles.Admin) || DemoPick.Services.AppSession.IsInRole(DemoPick.Services.AppConstants.Roles.Staff);
                if (!ok)
                {
                    MessageBox.Show("Tài khoản của bạn không có quyền đổi ca.", "Không có quyền", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                DemoPick.Services.DatabaseHelper.TryLogThrottled(
                    throttleKey: "UCDatLich.CanReschedule",
                    eventDesc: "Role Check Error",
                    ex: ex,
                    context: "UCDatLich.CanReschedule",
                    minSeconds: 300);
                MessageBox.Show("Không thể xác định quyền người dùng. Vui lòng đăng nhập lại.", "Không có quyền", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }

        private void OpenRescheduleForSelected()
        {
            if (!CanReschedule()) return;

            if (_selectedBooking == null)
            {
                MessageBox.Show("Hãy click chọn 1 booking trên lịch trước, rồi bấm 'Đổi ca'.\n(hoặc double-click trực tiếp vào booking)", "Chưa chọn booking", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var b = _selectedBooking;
            if (string.Equals(b.Status, DemoPick.Services.AppConstants.BookingStatus.Paid, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Booking đã thanh toán, không thể đổi ca.", "Không thể đổi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.Equals(b.Status, DemoPick.Services.AppConstants.BookingStatus.Cancelled, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Booking đã bị hủy.", "Không thể đổi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string courtName = _cachedCourts?.Find(x => x.CourtID == b.CourtID)?.Name ?? "";

                using (var frm = new DemoPick.Views.FrmDoiCaBooking(_currentDate.Date, b.BookingID, courtName, b.GuestName, b.Status, b.StartTime, b.EndTime, b.Note))
                {
                    if (frm.ShowDialog() != DialogResult.OK) return;

                    _controller.UpdateBookingTimeAndNote(b.BookingID, frm.NewStart, frm.NewEnd, frm.NewNote);
                    ReloadTimelineAsync(forceReload: true);
                    MessageBox.Show($"Đổi ca thành công!\n{frm.NewStart:HH:mm} - {frm.NewEnd:HH:mm}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                try { DemoPick.Services.DatabaseHelper.TryLog("DoiCa Error", ex, "UCDatLich.OpenRescheduleForSelected"); } catch { }
                MessageBox.Show("Không thể đổi ca: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PnlCanvas_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Left) return;

                // Find booking under cursor
                for (int i = _bookingHits.Count - 1; i >= 0; i--)
                {
                    var hit = _bookingHits[i];
                    if (hit?.Booking == null) continue;
                    if (!hit.Rect.Contains(e.Location)) continue;

                    if (!CanReschedule()) return;

                    var b = hit.Booking;
                    if (string.Equals(b.Status, DemoPick.Services.AppConstants.BookingStatus.Paid, StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Booking đã thanh toán, không thể đổi ca.", "Không thể đổi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (string.Equals(b.Status, DemoPick.Services.AppConstants.BookingStatus.Cancelled, StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Booking đã bị hủy.", "Không thể đổi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    string courtName = "";
                    courtName = _cachedCourts?.Find(x => x.CourtID == b.CourtID)?.Name ?? "";

                    using (var frm = new DemoPick.Views.FrmDoiCaBooking(_currentDate.Date, b.BookingID, courtName, b.GuestName, b.Status, b.StartTime, b.EndTime, b.Note))
                    {
                        if (frm.ShowDialog() != DialogResult.OK) return;

                        _controller.UpdateBookingTimeAndNote(b.BookingID, frm.NewStart, frm.NewEnd, frm.NewNote);
                        _selectedBooking = b;
                        ReloadTimelineAsync(forceReload: true);
                        MessageBox.Show($"Đổi ca thành công!\n{frm.NewStart:HH:mm} - {frm.NewEnd:HH:mm}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                // Double-click court name area to delete court quickly.
                if (TryGetCourtAtPoint(e.Location, requireTimelineArea: false, out var courtToDelete))
                {
                    DeleteCourtWithConfirmation(courtToDelete);
                }
            }
            catch (Exception ex)
            {
                try { DemoPick.Services.DatabaseHelper.TryLog("DoiCa Error", ex, "UCDatLich.PnlCanvas_MouseDoubleClick"); } catch { }
                MessageBox.Show("Không thể đổi ca: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool TryGetCourtAtPoint(Point canvasPoint, bool requireTimelineArea, out DemoPick.Models.CourtModel court)
        {
            court = null;

            try
            {
                if (_cachedCourts == null || _cachedCourts.Count == 0) return false;
                if (canvasPoint.Y < TimeHeaderHeight) return false;
                if (requireTimelineArea && canvasPoint.X <= CourtColWidth) return false;
                if (!requireTimelineArea && canvasPoint.X > CourtColWidth) return false;

                int idx = (canvasPoint.Y - TimeHeaderHeight) / CourtRowHeight;
                if (idx < 0 || idx >= _cachedCourts.Count) return false;

                court = _cachedCourts[idx];
                return court != null;
            }
            catch
            {
                return false;
            }
        }

        private void HandleCourtSingleClick(DemoPick.Models.CourtModel court)
        {
            if (court == null) return;

            DateTime now = DateTime.Now;
            DateTime suggestedStart = _currentDate.Date.Add(now.TimeOfDay);

            using (var frm = new FrmDatSanCoDinh(FrmDatSanCoDinh.BookingMode.Quick, court.CourtID, _currentDate.Date, suggestedStart))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    ReloadTimelineAsync(forceReload: true);
                }
            }
        }

        private void DeleteCourtWithConfirmation(DemoPick.Models.CourtModel court)
        {
            if (court == null) return;
            if (!CanManageCourts()) return;

            var name = string.IsNullOrWhiteSpace(court.Name) ? "(Không tên)" : court.Name;
            var confirm = MessageBox.Show(
                $"Bạn chắc chắn muốn xóa sân: {name}?\n(Sân sẽ bị ẩn khỏi danh sách và không thể đặt mới.)",
                "Xác nhận xóa sân",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm != DialogResult.Yes) return;

            try
            {
                _controller.DeactivateCourt(court.CourtID);
                _selectedBooking = null;
                ReloadTimelineAsync(forceReload: true);
                MessageBox.Show("Đã xóa sân thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                try { DemoPick.Services.DatabaseHelper.TryLog("DeactivateCourt Error", ex, "UCDatLich"); } catch { }
                MessageBox.Show(ex.Message, "Không thể xóa sân", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateDateLabel()
        {
            string dateStr = "Hôm nay";
            if (_currentDate.Date == DateTime.Now.Date.AddDays(-1))
                dateStr = "Hôm qua";
            else if (_currentDate.Date == DateTime.Now.Date.AddDays(1))
                dateStr = "Ngày mai";
            else if (_currentDate.Date != DateTime.Now.Date)
            {
                var dict = new System.Collections.Generic.Dictionary<DayOfWeek, string>
                {
                    {DayOfWeek.Monday, "Thứ Hai"}, {DayOfWeek.Tuesday, "Thứ Ba"},
                    {DayOfWeek.Wednesday, "Thứ Tư"}, {DayOfWeek.Thursday, "Thứ Năm"},
                    {DayOfWeek.Friday, "Thứ Sáu"}, {DayOfWeek.Saturday, "Thứ Bảy"},
                    {DayOfWeek.Sunday, "Chủ Nhật"}
                };
                dateStr = dict[_currentDate.DayOfWeek];
            }

            lblDate.Text = $"{dateStr}, {_currentDate:dd} Thg {_currentDate:MM}";
        }
    }
}
