// ==========================================================
// File: FrmDatSanCoDinh.cs
// Role: View (MVC)
// Description: Form xử lý nghiệp vụ đặt sân và bảo trì sân.
// ==========================================================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;

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

        // Khoi tao man hinh/control FrmDatSanCoDinh va chuan bi trang thai ban dau can dung.
        public FrmDatSanCoDinh()
            : this(BookingMode.Fixed, null, null, null)
        {
        }

        // Khoi tao man hinh/control FrmDatSanCoDinh va chuan bi trang thai ban dau can dung.
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

            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;

            // Wire Resize trước mọi thứ thay đổi size
            Resize += (s, e) => UpdateFormRegion();
            Paint += Frm_Paint;

            BuildStartTimePicker();
            BuildRealtimeConflictHint();
            BuildConflictDebounceTimer();

            // Set radio theo mode trước khi apply layout
            if (_mode == BookingMode.Quick)
                rbDatNhanh.Checked = true;
            else
                rbKhachThue.Checked = true;

            // Wire mode radio events
            rbDatNhanh.CheckedChanged += RbMode_CheckedChanged;
            rbKhachThue.CheckedChanged += RbMode_CheckedChanged;
            rbBaoTri.CheckedChanged += RbMode_CheckedChanged;

            ApplyModeLayout();

            // Sau ApplyModeLayout cập nhật region một lần nữa cho chắc
            UpdateFormRegion();

            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            btnCancelTop.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

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

            cbPayState.SelectedIndex = 0; // default: Thu tại sân

            if (ucDateRange != null)
            {
                ucDateRange.FromDate = DateTime.Now;
                ucDateRange.ToDate = DateTime.Now.AddMonths(1);
            }

            if (lblTo != null) lblTo.Visible = false;

            MouseDown += Form_MouseDown;
            pnlHeader.MouseDown += Form_MouseDown;
            lblTitle.MouseDown += Form_MouseDown;

            UpdatePhoneValidationUi();
            UiTheme.NormalizeTextBackgrounds(this);
        }

        // Cap nhat lai du lieu/trang thai Update Form Region tren man hinh hien tai.
        private void UpdateFormRegion()
        {
            using (var path = RoundedRect(new Rectangle(0, 0, Width, Height), 20))
            {
                var old = Region;
                Region = new Region(path);
                if (old != null) old.Dispose();
            }
        }

        // Tao hoac tinh ra du lieu Build Start Time Picker tu cac thong tin dau vao hien co.
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

        // Tao hoac tinh ra du lieu Build Realtime Conflict Hint tu cac thong tin dau vao hien co.
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



        // Tao hoac tinh ra du lieu Build Conflict Debounce Timer tu cac thong tin dau vao hien co.
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

        // Ap dung hoac chuan hoa trang thai Apply Mode Layout de du lieu/giao dien nhat quan.
        private void ApplyModeLayout()
        {
            bool quickMode = rbDatNhanh != null && rbDatNhanh.Checked;
            bool maintenanceMode = rbBaoTri != null && rbBaoTri.Checked;

            if (quickMode)
            {
                ApplyQuickModeLayout();
            }
            else if (maintenanceMode)
            {
                ApplyMaintenanceModeLayout();
            }
            else
            {
                ApplyFixedModeLayout();
            }

            UpdatePhoneValidationUi();
        }

        // Ap dung hoac chuan hoa trang thai Apply Quick Mode Layout de du lieu/giao dien nhat quan.
        private void ApplyQuickModeLayout()
        {
            Text = "Đặt Sân Nhanh";
            lblTitle.Text = "Đặt Sân Nhanh";
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

            txtName.Enabled = true;
            txtPhone.Enabled = true;
            if (txtName.Text == "Ban Quản Lý (Bảo Trì)")
            {
                txtName.Text = "";
                txtPhone.Text = "";
            }

            btnConfirm.FillColor = Color.FromArgb(46, 204, 113);
            btnConfirm.FillHoverColor = Color.FromArgb(56, 214, 123);
            btnConfirm.Text = "Xác nhận đặt sân";

            lblPayState.Visible = true;
            cbPayState.Visible = true;
            cbPayState.Enabled = true;
        }

        // Ap dung hoac chuan hoa trang thai Apply Maintenance Mode Layout de du lieu/giao dien nhat quan.
        private void ApplyMaintenanceModeLayout()
        {
            Text = "Bảo Trì & Khóa Sân";
            lblTitle.Text = "Bảo Trì & Khóa Sân";
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

            lblPayState.Visible = false;
            cbPayState.Visible = false;
        }

        // Ap dung hoac chuan hoa trang thai Apply Fixed Mode Layout de du lieu/giao dien nhat quan.
        private void ApplyFixedModeLayout()
        {
            Text = "Đặt Sân Cố Định";
            lblTitle.Text = "Đặt Sân Cố Định";
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

            lblPayState.Visible = true;
            cbPayState.Visible = true;
            cbPayState.Enabled = true;
        }

        // Nap du lieu cho Get Selected Start Time Of Day roi cap nhat lai trang thai hien thi tren man hinh.
        internal TimeSpan GetSelectedStartTimeOfDay()
        {
            if (_dtStartClock == null) return new TimeSpan(17, 0, 0);
            return _dtStartClock.Value.TimeOfDay;
        }

        // Dien giai hoac xac dinh Parse Duration Minutes From Ui tu du lieu dau vao de cac buoc sau dung thong nhat.
        internal int ParseDurationMinutesFromUi()
        {
            string text = cbDuration == null ? string.Empty : cbDuration.SelectedItem?.ToString() ?? string.Empty;
            return ParseDurationMinutesCore(text);
        }

        // Dien giai hoac xac dinh Resolve Selected Court Id tu du lieu dau vao de cac buoc sau dung thong nhat.
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

        // Dien giai hoac xac dinh Resolve Selected Date tu du lieu dau vao de cac buoc sau dung thong nhat.
        internal DateTime ResolveSelectedDate()
        {
            if (ucDateRange == null) return DateTime.Today;
            return ucDateRange.FromDate.Date;
        }

        // Kiem tra dieu kien Has Conflict At Start va tra ve ket qua dung/sai cho luong xu ly.
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

        // Xu ly logic man hinh Queue Conflict Hint Refresh va cap nhat control lien quan.
        private void QueueConflictHintRefresh()
        {
            if (_conflictDebounceTimer == null) return;
            _conflictDebounceTimer.Stop();
            _conflictDebounceTimer.Start();
        }

        // Cap nhat lai du lieu/trang thai Refresh Conflict Hint Ui tren man hinh hien tai.
        private void RefreshConflictHintUi()
        {
            if (_lblConflict == null || _lblConflict.IsDisposed) return;

            int courtId = ResolveSelectedCourtId();
            if (courtId <= 0)
            {
                _lblConflict.Text = string.Empty;
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
        }

        // Dien giai hoac xac dinh Parse Duration Minutes Core tu du lieu dau vao de cac buoc sau dung thong nhat.
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

        // Cap nhat lai du lieu/trang thai Update Phone Validation Ui tren man hinh hien tai.
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

        // Dien giai hoac xac dinh Resolve Payment State tu du lieu dau vao de cac buoc sau dung thong nhat.
        internal string ResolvePaymentState()
        {
            if (cbPayState == null) return AppConstants.BookingPaymentState.PayAtVenue;
            string selected = (cbPayState.SelectedItem ?? "").ToString().Trim();

            if (selected.IndexOf("chuyển khoản", StringComparison.OrdinalIgnoreCase) >= 0)
                return AppConstants.BookingPaymentState.BankTransferred;
            if (selected.IndexOf("50%", StringComparison.OrdinalIgnoreCase) >= 0)
                return AppConstants.BookingPaymentState.Deposit50;

            return AppConstants.BookingPaymentState.PayAtVenue;
        }
    }
}


