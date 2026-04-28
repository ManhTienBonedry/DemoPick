using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DemoPick.Services;

namespace DemoPick
{
    public partial class FrmDatSanCoDinh : Form
    {
        public enum BookingMode
        {
            Quick = 0,
            Fixed = 1,
        }

        private readonly DemoPick.Controllers.BookingController _controller = new DemoPick.Controllers.BookingController();
        private readonly BookingMode _mode;
        private readonly int? _preselectedCourtId;
        private readonly DateTime? _preselectedDate;
        private readonly DateTime? _preselectedStartTime;

        private DateTimePicker _dtStartClock;
        private Label _lblConflict;
        private Timer _conflictDebounceTimer;
        private Label _lblSuggestions;
        private FlowLayoutPanel _pnlSuggestions;
        private readonly List<LinkLabel> _suggestionLinks = new List<LinkLabel>();
        private bool _smartSuggestionLayoutApplied;

        public FrmDatSanCoDinh()
            : this(BookingMode.Fixed, null, null, null)
        {
        }

        public FrmDatSanCoDinh(BookingMode mode, int? preselectedCourtId, DateTime? preselectedDate, DateTime? preselectedStartTime)
        {
            _mode = mode;
            _preselectedCourtId = preselectedCourtId;
            _preselectedDate = preselectedDate;
            _preselectedStartTime = preselectedStartTime;

            InitializeComponent();

            if (DesignModeUtil.IsDesignMode(this))
            {
                return;
            }

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            BuildStartTimePicker();
            BuildRealtimeConflictHint();
            BuildSmartSuggestionUi();
            BuildConflictDebounceTimer();
            ApplyModeLayout();

            btnCancel.Click += (s, e) => Close();
            btnCancelTop.Click += (s, e) => Close();

            rbDatNhanh.CheckedChanged += RbMode_CheckedChanged;
            rbKhachThue.CheckedChanged += RbMode_CheckedChanged;
            rbBaoTri.CheckedChanged += RbMode_CheckedChanged;

            if (txtPhone != null)
            {
                txtPhone.TextChanged += (s, e) =>
                {
                    UpdatePhoneValidationUi();
                    QueueConflictHintRefresh();
                };
            }

            if (txtName != null)
            {
                txtName.TextChanged += (s, e) => QueueConflictHintRefresh();
            }

            if (cbCourt != null)
            {
                cbCourt.SelectedIndexChanged += (s, e) => QueueConflictHintRefresh();
            }

            if (cbDuration != null)
            {
                cbDuration.SelectedIndexChanged += (s, e) => QueueConflictHintRefresh();
            }

            if (ucDateRange != null)
            {
                ucDateRange.SelectedDateChanged += (s, e) => QueueConflictHintRefresh();
                ucDateRange.RangeChanged += (s, e) => QueueConflictHintRefresh();
            }

            btnConfirm.Click += BtnConfirm_Click;

            Load += FrmDatSanCoDinh_Load;
            cbTime.SelectedIndex = 22;
            cbDuration.SelectedIndex = 1;

            if (ucDateRange != null)
            {
                ucDateRange.Mode = UCDateRangeFilter.DateFilterMode.Range;
                ucDateRange.ShowApplyButton = false;
                ucDateRange.FromDate = DateTime.Now;
                ucDateRange.ToDate = DateTime.Now.AddMonths(1);
            }

            if (lblTo != null) lblTo.Visible = false;

            MouseDown += Form_MouseDown;
            pnlHeader.MouseDown += Form_MouseDown;
            lblTitle.MouseDown += Form_MouseDown;

            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;

            using (var path = RoundedRect(new Rectangle(0, 0, Width, Height), 20))
            {
                var old = Region;
                Region = new Region(path);
                if (old != null) old.Dispose();
            }

            Paint += Frm_Paint;
            UpdatePhoneValidationUi();
            UiTheme.NormalizeTextBackgrounds(this);
        }

        private void BuildStartTimePicker()
        {
            _dtStartClock = new DateTimePicker();
            _dtStartClock.Format = DateTimePickerFormat.Custom;
            _dtStartClock.CustomFormat = "HH:mm";
            _dtStartClock.ShowUpDown = true;
            _dtStartClock.Font = cbTime.Font;
            _dtStartClock.Location = cbTime.Location;
            _dtStartClock.Size = cbTime.Size;
            _dtStartClock.ValueChanged += (s, e) => QueueConflictHintRefresh();

            Controls.Add(_dtStartClock);
            _dtStartClock.BringToFront();

            cbTime.Visible = false;
            cbTime.Enabled = false;
        }

        private void BuildRealtimeConflictHint()
        {
            _lblConflict = new Label();
            _lblConflict.AutoSize = true;
            _lblConflict.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            _lblConflict.ForeColor = Color.FromArgb(220, 38, 38);
            _lblConflict.BackColor = BackColor;
            _lblConflict.Location = new Point(_dtStartClock.Left, _dtStartClock.Bottom + 6);
            _lblConflict.Text = string.Empty;

            Controls.Add(_lblConflict);
            _lblConflict.BringToFront();
        }

        private void BuildSmartSuggestionUi()
        {
            _lblSuggestions = new Label();
            _lblSuggestions.AutoSize = true;
            _lblSuggestions.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _lblSuggestions.ForeColor = Color.FromArgb(15, 118, 110);
            _lblSuggestions.BackColor = BackColor;
            _lblSuggestions.Location = new Point(_dtStartClock.Left, _dtStartClock.Bottom + 30);
            _lblSuggestions.Text = "Goi y khung gio:";
            _lblSuggestions.Visible = false;

            _pnlSuggestions = new FlowLayoutPanel();
            _pnlSuggestions.Location = new Point(_dtStartClock.Left, _lblSuggestions.Bottom + 4);
            _pnlSuggestions.Size = new Size(300, 78);
            _pnlSuggestions.BackColor = BackColor;
            _pnlSuggestions.FlowDirection = FlowDirection.TopDown;
            _pnlSuggestions.WrapContents = false;
            _pnlSuggestions.AutoScroll = true;
            _pnlSuggestions.Visible = false;

            for (int i = 0; i < 3; i++)
            {
                var link = new LinkLabel();
                link.AutoSize = false;
                link.Width = 286;
                link.Height = 22;
                link.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
                link.LinkColor = Color.FromArgb(22, 101, 52);
                link.ActiveLinkColor = Color.FromArgb(21, 128, 61);
                link.VisitedLinkColor = Color.FromArgb(22, 101, 52);
                link.BackColor = BackColor;
                link.Visible = false;
                link.Click += SuggestionLink_Click;
                _suggestionLinks.Add(link);
                _pnlSuggestions.Controls.Add(link);
            }

            Controls.Add(_lblSuggestions);
            Controls.Add(_pnlSuggestions);
            _lblSuggestions.BringToFront();
            _pnlSuggestions.BringToFront();
        }

        private void BuildConflictDebounceTimer()
        {
            _conflictDebounceTimer = new Timer();
            _conflictDebounceTimer.Interval = 180;
            _conflictDebounceTimer.Tick += (s, e) =>
            {
                _conflictDebounceTimer.Stop();
                RefreshConflictHintUi();
            };

            Disposed += (s, e) =>
            {
                try
                {
                    _conflictDebounceTimer.Stop();
                    _conflictDebounceTimer.Dispose();
                }
                catch
                {
                }
            };
        }

        private BookingMode CurrentMode
        {
            get
            {
                if (rbBaoTri != null && rbBaoTri.Checked) return BookingMode.Fixed; // Bảo trì uses Fixed date logic
                if (rbKhachThue != null && rbKhachThue.Checked) return BookingMode.Fixed;
                return BookingMode.Quick;
            }
        }

        private void ApplyModeLayout()
        {
            bool quickMode = rbDatNhanh != null && rbDatNhanh.Checked;
            bool fixedMode = rbKhachThue != null && rbKhachThue.Checked;
            bool maintenanceMode = rbBaoTri != null && rbBaoTri.Checked;

            if (quickMode)
            {
                Text = "Đặt Sân Nhanh";
                lblTitle.Text = "Đặt Sân Nhanh";
            }
            else if (maintenanceMode)
            {
                Text = "Bảo Trì & Khóa Sân";
                lblTitle.Text = "Bảo Trì & Khóa Sân";
            }
            else
            {
                Text = "Đặt Sân Cố Định";
                lblTitle.Text = "Đặt Sân Cố Định";
            }

            if (quickMode)
            {
                lblDateRange.Text = "Ngày chơi:";

                lblRepeat.Visible = false;
                chkMon.Visible = false;
                chkTue.Visible = false;
                chkWed.Visible = false;
                chkThu.Visible = false;
                chkFri.Visible = false;
                chkSat.Visible = false;
                chkSun.Visible = false;
                lblTo.Visible = false;

                if (ucDateRange != null)
                {
                    ucDateRange.Mode = UCDateRangeFilter.DateFilterMode.SingleDate;
                    ucDateRange.ShowApplyButton = false;
                }

                btnConfirm.FillColor = Color.FromArgb(46, 204, 113);
                btnConfirm.FillHoverColor = Color.FromArgb(56, 214, 123);
                btnConfirm.Text = "Xác nhận đặt sân";
            }
            else if (maintenanceMode)
            {
                lblDateRange.Text = "Chu kỳ (Từ - Đến):";

                lblRepeat.Visible = true;
                chkMon.Visible = true;
                chkTue.Visible = true;
                chkWed.Visible = true;
                chkThu.Visible = true;
                chkFri.Visible = true;
                chkSat.Visible = true;
                chkSun.Visible = true;

                if (ucDateRange != null)
                {
                    ucDateRange.Mode = UCDateRangeFilter.DateFilterMode.Range;
                    ucDateRange.ShowApplyButton = false;
                }

                txtName.Enabled = false;
                txtPhone.Enabled = false;
                txtName.Text = "Ban Quản Lý (Bảo Trì)";
                txtPhone.Text = "N/A";

                btnConfirm.FillColor = Color.FromArgb(231, 76, 60);
                btnConfirm.FillHoverColor = Color.FromArgb(241, 86, 70);
                btnConfirm.Text = "Xác nhận Khóa Sân";
            }
            else // Fixed
            {
                lblDateRange.Text = "Chu kỳ (Từ - Đến):";

                lblRepeat.Visible = true;
                chkMon.Visible = true;
                chkTue.Visible = true;
                chkWed.Visible = true;
                chkThu.Visible = true;
                chkFri.Visible = true;
                chkSat.Visible = true;
                chkSun.Visible = true;

                if (ucDateRange != null)
                {
                    ucDateRange.Mode = UCDateRangeFilter.DateFilterMode.Range;
                    ucDateRange.ShowApplyButton = false;
                }

                txtName.Enabled = true;
                txtPhone.Enabled = true;
                if (txtName.Text == "Ban Quản Lý (Bảo Trì)")
                {
                    txtName.Text = "";
                    txtPhone.Text = "";
                }

                btnConfirm.FillColor = Color.FromArgb(119, 219, 44);
                btnConfirm.FillHoverColor = Color.FromArgb(56, 214, 123);
                btnConfirm.Text = "Tạo Lịch Cố Định";
            }

            UpdatePhoneValidationUi();
            ApplySmartSuggestionLayout(showSuggestions: false);
        }

        internal TimeSpan GetSelectedStartTimeOfDay()
        {
            if (_dtStartClock == null) return new TimeSpan(17, 0, 0);
            return _dtStartClock.Value.TimeOfDay;
        }

        internal int ParseDurationMinutesFromUi()
        {
            string text = cbDuration == null ? string.Empty : cbDuration.SelectedItem?.ToString() ?? string.Empty;
            return ParseDurationMinutesCore(text);
        }

        internal int ResolveSelectedCourtId()
        {
            object selected = cbCourt == null ? null : cbCourt.SelectedValue;
            if (selected is int intId) return intId;

            int parsed;
            if (selected != null && int.TryParse(selected.ToString(), out parsed))
            {
                return parsed;
            }

            return 0;
        }

        internal DateTime ResolveSelectedDate()
        {
            if (ucDateRange == null) return DateTime.Today;
            return ucDateRange.FromDate.Date;
        }

        internal bool HasConflictAtStart(int courtId, DateTime start, DateTime end)
        {
            if (courtId <= 0) return false;
            if (end <= start) return false;

            try
            {
                var bookings = _controller.GetBookingsByDate(start.Date);
                if (bookings == null || bookings.Count == 0) return false;

                foreach (var b in bookings)
                {
                    if (b == null) continue;
                    if (b.CourtID != courtId) continue;
                    if (string.Equals(b.Status, AppConstants.BookingStatus.Cancelled, StringComparison.OrdinalIgnoreCase)) continue;

                    bool overlap = start < b.EndTime && end > b.StartTime;
                    if (overlap) return true;
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLogThrottled(
                    throttleKey: "FrmDatSanCoDinh.HasConflictAtStart",
                    eventDesc: "Conflict Check Error",
                    ex: ex,
                    context: "FrmDatSanCoDinh.HasConflictAtStart",
                    minSeconds: 60);
            }

            return false;
        }

        private void QueueConflictHintRefresh()
        {
            if (_conflictDebounceTimer == null) return;
            _conflictDebounceTimer.Stop();
            _conflictDebounceTimer.Start();
        }

        private void RefreshConflictHintUi()
        {
            if (_lblConflict == null || _lblConflict.IsDisposed) return;

            int courtId = ResolveSelectedCourtId();
            if (courtId <= 0)
            {
                _lblConflict.Text = string.Empty;
                RefreshSmartSuggestionUi(0, DateTime.Today, DateTime.Today, DateTime.Today);
                return;
            }

            int durationMins = ParseDurationMinutesFromUi();
            if (durationMins <= 0) durationMins = 90;

            DateTime date = ResolveSelectedDate();
            DateTime start = date.Add(GetSelectedStartTimeOfDay());
            DateTime end = start.AddMinutes(durationMins);

            if (HasConflictAtStart(courtId, start, end))
            {
                _lblConflict.ForeColor = Color.FromArgb(220, 38, 38);
                _lblConflict.Text = "Trung lich: khung gio nay da co booking.";
            }
            else
            {
                _lblConflict.ForeColor = Color.FromArgb(22, 163, 74);
                _lblConflict.Text = "Khung gio dang chon dang trong.";
            }

            RefreshSmartSuggestionUi(courtId, date, start, end);
        }

        internal static int ParseDurationMinutesCore(string durationText)
        {
            string t = durationText ?? string.Empty;
            if (t.IndexOf("60", StringComparison.OrdinalIgnoreCase) >= 0) return 60;
            if (t.IndexOf("90", StringComparison.OrdinalIgnoreCase) >= 0) return 90;
            if (t.IndexOf("120", StringComparison.OrdinalIgnoreCase) >= 0) return 120;
            if (t.IndexOf("150", StringComparison.OrdinalIgnoreCase) >= 0) return 150;
            if (t.IndexOf("180", StringComparison.OrdinalIgnoreCase) >= 0) return 180;
            if (t.IndexOf("nguyen ngay", StringComparison.OrdinalIgnoreCase) >= 0) return 720;
            if (t.IndexOf("Nguy", StringComparison.OrdinalIgnoreCase) >= 0) return 720;
            return 0;
        }

        private void UpdatePhoneValidationUi()
        {
            if (txtPhone == null) return;

            if (!txtPhone.Enabled)
            {
                txtPhone.RectColor = Color.LightGray;
                return;
            }

            string raw = txtPhone.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(raw))
            {
                txtPhone.RectColor = Color.LightGray;
                return;
            }

            txtPhone.RectColor = PhoneNumberValidator.IsValidTenDigits(raw)
                ? Color.LightGray
                : Color.FromArgb(231, 76, 60);
        }
    }
}
