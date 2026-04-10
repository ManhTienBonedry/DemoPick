using System;
using System.Drawing;
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

        private const float ZoomMin = 1.0f;   // Fit-to-width baseline
        private const float ZoomMax = 2.5f;   // Allow horizontal scroll when zoomed
        private const float ZoomStep = 0.25f;
        private float _zoom = ZoomMin;

        private DateTime _currentDate = DateTime.Now;
        private readonly DemoPick.Controllers.BookingController _controller = new DemoPick.Controllers.BookingController();

        private System.Collections.Generic.List<DemoPick.Models.CourtModel> _cachedCourts = new System.Collections.Generic.List<DemoPick.Models.CourtModel>();
        private System.Collections.Generic.List<DemoPick.Models.BookingModel> _cachedBookings = new System.Collections.Generic.List<DemoPick.Models.BookingModel>();
        private DateTime _cacheDate = DateTime.MinValue;

        private int _reloadSeq;

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

            _currentDate = DateTime.Today;

            try
            {
                if (DateFilter != null)
                {
                    DateFilter.Mode = UCDateRangeFilter.DateFilterMode.SingleDate;
                    DateFilter.SelectedDate = _currentDate;
                    DateFilter.SelectedDateChanged += (s, e) =>
                    {
                        try
                        {
                            var selected = DateFilter.SelectedDate.Date;
                            if (selected == _currentDate.Date) return;
                            _currentDate = selected;
                            UpdateDateLabel();
                            ReloadTimelineAsync(forceReload: true);
                        }
                        catch
                        {
                            // Ignore control exceptions
                        }
                    };
                }
            }
            catch
            {
                // Ignore control initialization differences
            }

            UpdateDateLabel();

            // Double buffered to prevent flickering when scrolling/drawing GDI grid
            typeof(Panel).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(pnlCanvas, true, null);

            pnlTimelineContainer.Paint += (s, e) =>
            {
                var p = s as Panel;
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(229, 231, 235), 1), 0, 0, p.Width - 1, p.Height - 1);
            };

            pnlCanvas.Paint += RenderTimelineGrid;
            pnlCanvas.MouseDoubleClick += PnlCanvas_MouseDoubleClick;
            pnlCanvas.MouseClick += PnlCanvas_MouseClick;

            pnlTimelineContainer.Resize += (s, e) => RefreshTimelineLayoutOnly();
            pnlTimelineContainer.Scroll += (s, e) => pnlCanvas.Invalidate();

            if (btnZoomIn != null)
            {
                btnZoomIn.Click += (s, e) => SetZoom(_zoom + ZoomStep);
            }
            if (btnZoomOut != null)
            {
                btnZoomOut.Click += (s, e) => SetZoom(_zoom - ZoomStep);
            }
            UpdateZoomButtons();

            // Bind Quick Booking button
            if (btnDatNhanh != null)
            {
                btnDatNhanh.Click += (s, e) =>
                {
                    using (var frm = new FrmDatSan())
                    {
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            ReloadTimelineAsync(forceReload: true);
                        }
                    }
                };
            }

            // Bind Fixed Booking & Maintenance button
            if (btnDatCoDinh != null)
            {
                btnDatCoDinh.Click += (s, e) =>
                {
                    using (var frm = new FrmDatSanCoDinh())
                    {
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            ReloadTimelineAsync(forceReload: true);
                        }
                    }
                };
            }

            // Đổi ca button (select booking then click)
            if (btnDoiCa != null)
            {
                btnDoiCa.Click += (s, e) => OpenRescheduleForSelected();
            }

            if (IsHandleCreated)
            {
                ReloadTimelineAsync(forceReload: true);
            }
            else
            {
                EventHandler onHandleCreated = null;
                onHandleCreated = (s, e) =>
                {
                    HandleCreated -= onHandleCreated;
                    ReloadTimelineAsync(forceReload: true);
                };
                HandleCreated += onHandleCreated;
            }
        }
    }
}
