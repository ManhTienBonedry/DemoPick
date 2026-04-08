using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DemoPick
{
    public partial class UCDatLich : UserControl
    {
        private const int GridStartHour = 6;
        private const int GridHoursToDraw = 18; // 06:00 to 24:00
        private const int CourtColWidth = 220;
        private const int TimeHeaderHeight = 46;
        private const int CourtRowHeight = 64;

        private DateTime _currentDate = DateTime.Now;
        private readonly DemoPick.Controllers.BookingController _controller = new DemoPick.Controllers.BookingController();

        private UCDateRangeFilter DateFilter => dateFilter;

        private DemoPick.Models.BookingModel _selectedBooking;

        private sealed class BookingHitInfo
        {
            public RectangleF Rect;
            public DemoPick.Models.BookingModel Booking;
        }

        private readonly System.Collections.Generic.List<BookingHitInfo> _bookingHits = new System.Collections.Generic.List<BookingHitInfo>();

        public UCDatLich()
        {
            InitializeComponent();

            _currentDate = _currentDate.Date;

            // Setup shared date filter (SingleDate mode)
            DateFilter.SelectedDate = _currentDate;
            UpdateDateLabel();

            DateFilter.SelectedDateChanged += (s, e) =>
            {
                try
                {
                    var selected = DateFilter.SelectedDate;
                    if (selected == _currentDate.Date) return;
                    _currentDate = selected;
                    UpdateDateLabel();
                    RefreshTimeline();
                }
                catch
                {
                    // Ignore control exceptions
                }
            };

            // Double buffered to prevent flickering when scrolling/drawing GDI grid
            typeof(Panel).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(pnlCanvas, true, null);

            pnlTimelineContainer.Paint += (s, e) => {
                var p = s as Panel;
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(229, 231, 235), 1), 0, 0, p.Width - 1, p.Height - 1);
            };

            pnlCanvas.Paint += RenderTimelineGrid;
            pnlCanvas.MouseDoubleClick += PnlCanvas_MouseDoubleClick;
            pnlCanvas.MouseClick += PnlCanvas_MouseClick;

            pnlTimelineContainer.Resize += (s, e) => RefreshTimeline();
            pnlTimelineContainer.Scroll += (s, e) => pnlCanvas.Invalidate();

            // Bind Quick Booking button
            btnDatNhanh.Click += (s, e) => {
                using (var frm = new FrmDatSan())
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        RefreshTimeline();
                    }
                }
            };
            
            // Bind Fixed Booking & Maintenance button
            btnDatCoDinh.Click += (s, e) => {
                using (var frm = new FrmDatSanCoDinh())
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        RefreshTimeline();
                    }
                }
            };

            // Đổi ca button (select booking then click)
            btnDoiCa.Click += (s, e) => OpenRescheduleForSelected();

            RefreshTimeline();
        }

        private void RefreshTimeline()
        {
            try
            {
                // Resize canvas so we can scroll vertically when there are many courts.
                var courts = _controller.GetCourts();
                int requiredHeight = TimeHeaderHeight + (courts.Count * CourtRowHeight) + 2;
                int visibleWidth = pnlTimelineContainer.ClientSize.Width;
                // If vertical scrollbar becomes visible, client width is reduced but sometimes not accounted for immediately.
                if (pnlTimelineContainer.VerticalScroll != null && pnlTimelineContainer.VerticalScroll.Visible)
                    visibleWidth = Math.Max(0, visibleWidth - SystemInformation.VerticalScrollBarWidth);

                int requiredWidth = Math.Max(300, visibleWidth);

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

            // Clicked empty area
            if (_selectedBooking != null)
            {
                _selectedBooking = null;
                pnlCanvas.Invalidate();
            }
        }

        private bool CanReschedule()
        {
            try
            {
                // Only Staff/Admin can reschedule.
                bool ok = DemoPick.Services.AppSession.IsInRole("Admin") || DemoPick.Services.AppSession.IsInRole("Staff");
                if (!ok)
                {
                    MessageBox.Show("Tài khoản của bạn không có quyền đổi ca.", "Không có quyền", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                return true;
            }
            catch
            {
                // If roles are not available for some reason, default to allow.
                return true;
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
            if (string.Equals(b.Status, "Paid", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Booking đã thanh toán, không thể đổi ca.", "Không thể đổi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.Equals(b.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Booking đã bị hủy.", "Không thể đổi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string courtName = "";
                try
                {
                    var courts = _controller.GetCourts();
                    var c = courts.Find(x => x.CourtID == b.CourtID);
                    courtName = c?.Name ?? "";
                }
                catch { }

                using (var frm = new DemoPick.Views.FrmDoiCaBooking(_currentDate.Date, b.BookingID, courtName, b.GuestName, b.Status, b.StartTime, b.EndTime))
                {
                    if (frm.ShowDialog() != DialogResult.OK) return;

                    _controller.UpdateBookingTime(b.BookingID, frm.NewStart, frm.NewEnd);
                    pnlCanvas.Invalidate();
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
                if (!CanReschedule()) return;

                // Find booking under cursor
                for (int i = _bookingHits.Count - 1; i >= 0; i--)
                {
                    var hit = _bookingHits[i];
                    if (hit?.Booking == null) continue;
                    if (!hit.Rect.Contains(e.Location)) continue;

                    var b = hit.Booking;
                    if (string.Equals(b.Status, "Paid", StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Booking đã thanh toán, không thể đổi ca.", "Không thể đổi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (string.Equals(b.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Booking đã bị hủy.", "Không thể đổi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    string courtName = "";
                    try
                    {
                        var courts = _controller.GetCourts();
                        var c = courts.Find(x => x.CourtID == b.CourtID);
                        courtName = c?.Name ?? "";
                    }
                    catch { }

                    using (var frm = new DemoPick.Views.FrmDoiCaBooking(_currentDate.Date, b.BookingID, courtName, b.GuestName, b.Status, b.StartTime, b.EndTime))
                    {
                        if (frm.ShowDialog() != DialogResult.OK) return;

                        _controller.UpdateBookingTime(b.BookingID, frm.NewStart, frm.NewEnd);
                        _selectedBooking = b;
                        pnlCanvas.Invalidate();
                        MessageBox.Show($"Đổi ca thành công!\n{frm.NewStart:HH:mm} - {frm.NewEnd:HH:mm}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                try { DemoPick.Services.DatabaseHelper.TryLog("DoiCa Error", ex, "UCDatLich.PnlCanvas_MouseDoubleClick"); } catch { }
                MessageBox.Show("Không thể đổi ca: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void RenderTimelineGrid(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            _bookingHits.Clear();

            int width = pnlCanvas.Width;
            int height = pnlCanvas.Height;

            int courtColWidth = CourtColWidth;
            int timeRowHeight = TimeHeaderHeight;
            int hoursToDraw = GridHoursToDraw;
            float hourWidth = (float)(width - courtColWidth) / hoursToDraw;

            Pen gridPen = new Pen(Color.FromArgb(243, 244, 246), 1);
            Font axisFont = new Font("Segoe UI", 9F, FontStyle.Bold);
            Brush axisBrush = new SolidBrush(Color.FromArgb(107, 114, 128));

            StringFormat headerFormat = null;
            StringFormat courtFormat = null;
            try
            {
                headerFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                courtFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap };

            // Draw header background
            g.FillRectangle(new SolidBrush(Color.FromArgb(249, 250, 251)), 0, 0, width, timeRowHeight);
            g.DrawLine(new Pen(Color.FromArgb(229, 231, 235), 1), 0, timeRowHeight, width, timeRowHeight);
            g.DrawLine(new Pen(Color.FromArgb(229, 231, 235), 1), courtColWidth, 0, courtColWidth, height);

                // Draw Time Headers
                for (int i = 0; i < hoursToDraw; i++)
                {
                    int h = GridStartHour + i;
                    float x = courtColWidth + (i * hourWidth);

                    // Nếu là quá khứ, tô nền đục
                    if (_currentDate.Date < DateTime.Now.Date ||
                       (_currentDate.Date == DateTime.Now.Date && h < DateTime.Now.Hour))
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(235, 235, 235)), x, timeRowHeight, hourWidth, height - timeRowHeight);
                    }

                    // Vertical grid lines
                    g.DrawLine(gridPen, x, timeRowHeight, x, height);

                    // Header string
                    string timeStr = string.Format("{0:D2}:00", h);
                    g.DrawString(timeStr, axisFont, axisBrush, new RectangleF(x, 0, hourWidth, timeRowHeight), headerFormat);
                }

                // Explicit end label
                try
                {
                    string endLabel = "24:00";
                    SizeF sEnd = g.MeasureString(endLabel, axisFont);
                    g.DrawString(endLabel, axisFont, axisBrush, width - sEnd.Width - 6, (timeRowHeight / 2f) - (sEnd.Height / 2f));
                }
                catch { }

            // Draw Court Rows
            var courts = _controller.GetCourts();
            int rowHeight = CourtRowHeight;

                for (int i = 0; i < courts.Count; i++)
                {
                    float y = timeRowHeight + (i * rowHeight);
                    // Horizontal grid lines
                    g.DrawLine(gridPen, 0, y + rowHeight, width, y + rowHeight);

                    // Court string
                    using (var courtBrush = new SolidBrush(Color.FromArgb(26, 35, 50)))
                    {
                        var rect = new RectangleF(12, y, courtColWidth - 18, rowHeight);
                        g.DrawString(courts[i].Name, axisFont, courtBrush, rect, courtFormat);
                    }
                }

                // DRAW REAL BOOKINGS
                var bookings = _controller.GetBookingsByDate(_currentDate);
                foreach (var b in bookings)
                {
                    int cIdx = courts.FindIndex(c => c.CourtID == b.CourtID);
                    if (cIdx < 0) continue;

                    float startHour = b.StartTime.Hour + (b.StartTime.Minute / 60.0f);
                    float durHours = (float)(b.EndTime - b.StartTime).TotalHours;

                    Color bookingColor = Color.FromArgb(59, 130, 246); // Blue (default)
                    if (string.Equals(b.Status, "Confirmed", StringComparison.OrdinalIgnoreCase)) bookingColor = Color.FromArgb(76, 175, 80); // Green
                    else if (string.Equals(b.Status, "Pending", StringComparison.OrdinalIgnoreCase)) bookingColor = Color.FromArgb(245, 158, 11); // Orange
                    else if (string.Equals(b.Status, "Maintenance", StringComparison.OrdinalIgnoreCase)) bookingColor = Color.FromArgb(239, 68, 68); // Red

                    bool selected = _selectedBooking != null && _selectedBooking.BookingID == b.BookingID;
                    RectangleF rect = DrawBooking(g, courtColWidth, timeRowHeight, hourWidth, rowHeight, cIdx, startHour, durHours, b.GuestName, bookingColor, b.StartTime, b.EndTime, selected);
                    _bookingHits.Add(new BookingHitInfo { Rect = rect, Booking = b });
                }
            }
            finally
            {
                if (headerFormat != null) headerFormat.Dispose();
                if (courtFormat != null) courtFormat.Dispose();
            }
        }

        private RectangleF DrawBooking(Graphics g, float offsetX, float offsetY, float hourWidth, float rowHeight, int courtIndex, float startHour, float durationHours, string title, Color color, DateTime startTime, DateTime endTime, bool selected)
        {
            float gridStartHour = GridStartHour;
            float x = offsetX + ((startHour - gridStartHour) * hourWidth);
            float y = offsetY + (courtIndex * rowHeight) + 10;
            float w = (durationHours * hourWidth) - 4;
            float h = rowHeight - 20;

            var rect = new RectangleF(x, y, w, h);

            // Draw Block
            using (GraphicsPath path = GetRoundedRect(new RectangleF(x, y, w, h), 6))
            {
                g.FillPath(new SolidBrush(Color.FromArgb(40, color)), path); // 15% opacity bg
                g.DrawPath(new Pen(color, 2), path); // Solid Border

                if (selected)
                {
                    g.DrawPath(new Pen(Color.FromArgb(26, 35, 50), 2), path);
                }
            }

            // Draw vertical solid indicator line on the left
            g.FillRectangle(new SolidBrush(color), x + 2, y + 4, 3, h - 8);

            // Draw Title inside block
            Font f = new Font("Segoe UI", 9F, FontStyle.Bold);

            string safeTitle = string.IsNullOrWhiteSpace(title) ? "(Không tên)" : title;
            string timeText = $"{startTime:HH:mm} - {endTime:HH:mm}";

            // Draw 2 lines: name + time range
            g.DrawString(safeTitle, f, new SolidBrush(Color.FromArgb(26, 35, 50)), x + 10, y + 6);
            using (Font tf = new Font("Segoe UI", 8.5F, FontStyle.Regular))
            {
                g.DrawString(timeText, tf, new SolidBrush(Color.FromArgb(75, 85, 99)), x + 10, y + 26);
            }

            return rect;
        }

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
