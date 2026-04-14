using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DemoPick.Services;

namespace DemoPick
{
    public partial class UCDatLich
    {
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

            Pen gridPen = null;
            Font axisFont = null;
            Brush axisBrush = null;
            Brush headerBgBrush = null;
            Pen headerBorderPen = null;
            Brush pastHourBrush = null;
            Brush courtNameBrush = null;

            StringFormat headerFormat = null;
            StringFormat courtFormat = null;
            try
            {
                gridPen = new Pen(Color.FromArgb(243, 244, 246), 1);
                axisFont = new Font("Segoe UI", 9F, FontStyle.Bold);
                axisBrush = new SolidBrush(Color.FromArgb(107, 114, 128));
                headerBgBrush = new SolidBrush(Color.FromArgb(249, 250, 251));
                headerBorderPen = new Pen(Color.FromArgb(229, 231, 235), 1);
                pastHourBrush = new SolidBrush(Color.FromArgb(235, 235, 235));
                courtNameBrush = new SolidBrush(Color.FromArgb(26, 35, 50));

                headerFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                courtFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap };

                // Draw header background
                g.FillRectangle(headerBgBrush, 0, 0, width, timeRowHeight);
                g.DrawLine(headerBorderPen, 0, timeRowHeight, width, timeRowHeight);
                g.DrawLine(headerBorderPen, courtColWidth, 0, courtColWidth, height);

                // Draw Time Headers
                for (int i = 0; i < hoursToDraw; i++)
                {
                    int h = GridStartHour + i;
                    float x = courtColWidth + (i * hourWidth);

                    // Nếu là quá khứ, tô nền đục
                    if (_currentDate.Date < DateTime.Now.Date ||
                       (_currentDate.Date == DateTime.Now.Date && h < DateTime.Now.Hour))
                    {
                        g.FillRectangle(pastHourBrush, x, timeRowHeight, hourWidth, height - timeRowHeight);
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
                var courts = _cachedCourts ?? new System.Collections.Generic.List<DemoPick.Models.CourtModel>();
                int rowHeight = CourtRowHeight;

                for (int i = 0; i < courts.Count; i++)
                {
                    float y = timeRowHeight + (i * rowHeight);
                    // Horizontal grid lines
                    g.DrawLine(gridPen, 0, y + rowHeight, width, y + rowHeight);

                    // Court string
                    var rect = new RectangleF(12, y, courtColWidth - 18, rowHeight);
                    g.DrawString(courts[i].Name, axisFont, courtNameBrush, rect, courtFormat);
                }

                // DRAW REAL BOOKINGS
                var bookings = _cachedBookings ?? new System.Collections.Generic.List<DemoPick.Models.BookingModel>();
                foreach (var b in bookings)
                {
                    int cIdx = courts.FindIndex(c => c.CourtID == b.CourtID);
                    if (cIdx < 0) continue;

                    float startHour = b.StartTime.Hour + (b.StartTime.Minute / 60.0f);
                    float durHours = (float)(b.EndTime - b.StartTime).TotalHours;

                    Color bookingColor = Color.FromArgb(59, 130, 246); // Blue (default)
                    if (string.Equals(b.Status, AppConstants.BookingStatus.Confirmed, StringComparison.OrdinalIgnoreCase)) bookingColor = Color.FromArgb(76, 175, 80); // Green
                    else if (string.Equals(b.Status, AppConstants.BookingStatus.Pending, StringComparison.OrdinalIgnoreCase)) bookingColor = Color.FromArgb(245, 158, 11); // Orange
                    else if (string.Equals(b.Status, AppConstants.BookingStatus.Maintenance, StringComparison.OrdinalIgnoreCase)) bookingColor = Color.FromArgb(239, 68, 68); // Red

                    bool selected = _selectedBooking != null && _selectedBooking.BookingID == b.BookingID;
                    bool showNote = !string.Equals(b.Status, AppConstants.BookingStatus.Paid, StringComparison.OrdinalIgnoreCase);
                    RectangleF rect = DrawBooking(g, courtColWidth, timeRowHeight, hourWidth, rowHeight, cIdx, startHour, durHours, b.GuestName, bookingColor, b.StartTime, b.EndTime, selected, b.Note, showNote);
                    _bookingHits.Add(new BookingHitInfo { Rect = rect, Booking = b });
                }
            }
            finally
            {
                if (headerFormat != null) headerFormat.Dispose();
                if (courtFormat != null) courtFormat.Dispose();
                if (gridPen != null) gridPen.Dispose();
                if (axisFont != null) axisFont.Dispose();
                if (axisBrush != null) axisBrush.Dispose();
                if (headerBgBrush != null) headerBgBrush.Dispose();
                if (headerBorderPen != null) headerBorderPen.Dispose();
                if (pastHourBrush != null) pastHourBrush.Dispose();
                if (courtNameBrush != null) courtNameBrush.Dispose();
            }
        }

        private RectangleF DrawBooking(Graphics g, float offsetX, float offsetY, float hourWidth, float rowHeight, int courtIndex, float startHour, float durationHours, string title, Color color, DateTime startTime, DateTime endTime, bool selected, string note, bool showNote)
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
                using (var bgBrush = new SolidBrush(Color.FromArgb(40, color)))
                using (var borderPen = new Pen(color, 2))
                {
                    g.FillPath(bgBrush, path); // 15% opacity bg
                    g.DrawPath(borderPen, path); // Solid Border
                }

                if (selected)
                {
                    using (var selPen = new Pen(Color.FromArgb(26, 35, 50), 2))
                    {
                        g.DrawPath(selPen, path);
                    }
                }
            }

            // Draw vertical solid indicator line on the left
            using (var indicatorBrush = new SolidBrush(color))
            {
                g.FillRectangle(indicatorBrush, x + 2, y + 4, 3, h - 8);
            }

            // Draw Title inside block
            string safeTitle = string.IsNullOrWhiteSpace(title) ? "(Không tên)" : title;
            string timeText = $"{startTime:HH:mm} - {endTime:HH:mm}";

            // Draw 2-3 lines: name + time range + (optional) note
            using (Font f = new Font("Segoe UI", 9F, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(Color.FromArgb(26, 35, 50)))
            {
                g.DrawString(safeTitle, f, titleBrush, x + 10, y + 6);
            }

            using (Font tf = new Font("Segoe UI", 8.5F, FontStyle.Regular))
            using (var timeBrush = new SolidBrush(Color.FromArgb(75, 85, 99)))
            {
                g.DrawString(timeText, tf, timeBrush, x + 10, y + 26);
            }

            if (showNote && !string.IsNullOrWhiteSpace(note) && h >= 62)
            {
                using (Font nf = new Font("Segoe UI", 8.25F, FontStyle.Italic))
                using (var noteBrush = new SolidBrush(Color.FromArgb(107, 114, 128)))
                using (var noteFormat = new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap })
                {
                    var noteRect = new RectangleF(x + 10, y + 44, Math.Max(0, w - 14), 18);
                    g.DrawString(note.Trim(), nf, noteBrush, noteRect, noteFormat);
                }
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
