using System;
using System.Windows.Forms;

namespace DemoPick.Views
{
    public partial class FrmDoiCaBooking : Form
    {
        private DateTime _date;
        private DateTime? _currentStart;
        private DateTime? _currentEnd;

        public DateTime NewStart { get; private set; }
        public DateTime NewEnd { get; private set; }
        public string NewNote { get; private set; }

        // Parameterless ctor for Visual Studio Designer.
        public FrmDoiCaBooking()
        {
            InitializeComponent();

            if (DemoPick.Services.DesignModeUtil.IsDesignMode(this))
            {
                return;
            }

            _date = DateTime.Today;
            WireChrome();
            WireButtons();

            // Default placeholders
            lblBookingIdValue.Text = "-";
            lblCourtValue.Text = "-";
            lblGuestValue.Text = "-";
            lblStatusValue.Text = "-";
            lblCurrent.Text = "Hiện tại: -";

            NewNote = string.Empty;
            if (txtNote != null) txtNote.Text = string.Empty;

            if (cbTime.Items.Count > 0 && cbTime.SelectedIndex < 0) cbTime.SelectedIndex = Math.Min(11, cbTime.Items.Count - 1);
            if (cbDuration.Items.Count > 0 && cbDuration.SelectedIndex < 0) cbDuration.SelectedIndex = 1;
        }

        // Backward compatible ctor
        public FrmDoiCaBooking(DateTime date, DateTime currentStart, DateTime currentEnd)
            : this()
        {
            _date = date.Date;
            _currentStart = currentStart;
            _currentEnd = currentEnd;

            lblCurrent.Text = $"Hiện tại: {currentStart:dd/MM/yyyy HH:mm} - {currentEnd:HH:mm}";

            // Preselect current values
            string startStr = currentStart.ToString("HH:00");
            int idxTime = cbTime.Items.IndexOf(startStr);
            cbTime.SelectedIndex = idxTime >= 0 ? idxTime : Math.Min(11, cbTime.Items.Count - 1);

            int durMins = (int)Math.Round((currentEnd - currentStart).TotalMinutes);
            if (durMins <= 60) cbDuration.SelectedIndex = 0;
            else if (durMins <= 90) cbDuration.SelectedIndex = 1;
            else if (durMins <= 120) cbDuration.SelectedIndex = 2;
            else cbDuration.SelectedIndex = 3;

            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        public FrmDoiCaBooking(DateTime date, int bookingId, string courtName, string guestName, string status, DateTime currentStart, DateTime currentEnd)
            : this(date, currentStart, currentEnd)
        {
            lblBookingIdValue.Text = bookingId > 0 ? "#BK" + bookingId : "-";
            lblCourtValue.Text = string.IsNullOrWhiteSpace(courtName) ? "-" : courtName;
            lblGuestValue.Text = string.IsNullOrWhiteSpace(guestName) ? "-" : guestName;
            lblStatusValue.Text = string.IsNullOrWhiteSpace(status) ? "-" : status;
        }

        public FrmDoiCaBooking(DateTime date, int bookingId, string courtName, string guestName, string status, DateTime currentStart, DateTime currentEnd, string currentNote)
            : this(date, bookingId, courtName, guestName, status, currentStart, currentEnd)
        {
            if (txtNote != null)
            {
                txtNote.Text = currentNote ?? string.Empty;
            }
        }

        private void WireChrome()
        {
            // Close button
            btnCloseTop.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            // Drag borderless form like other forms
            pnlTop.MouseDown += Form_MouseDown;
            lblHeaderTop.MouseDown += Form_MouseDown;
        }

        private void WireButtons()
        {
            btnOk.Click += (s, e) => TryConfirm();
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                try
                {
                    DemoPick.NativeMethods.ReleaseCapture();
                    DemoPick.NativeMethods.SendMessage(Handle, DemoPick.NativeMethods.WM_NCLBUTTONDOWN, DemoPick.NativeMethods.HT_CAPTION, 0);
                }
                catch
                {
                    // best effort
                }
            }
        }

        private void TryConfirm()
        {
            string timeStr = cbTime.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(timeStr))
            {
                MessageBox.Show("Vui lòng chọn giờ bắt đầu.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int durationMins = ParseDurationMinutes(cbDuration.SelectedItem?.ToString());
            if (durationMins <= 0)
            {
                MessageBox.Show("Vui lòng chọn thời lượng.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string[] parts = timeStr.Split(':');
            int hh = int.Parse(parts[0]);
            int mm = int.Parse(parts[1]);

            DateTime start = _date.AddHours(hh).AddMinutes(mm);
            DateTime end = start.AddMinutes(durationMins);

            bool isTimeUnchanged = _currentStart.HasValue && _currentEnd.HasValue && start == _currentStart.Value && end == _currentEnd.Value;

            if (start < DateTime.Now && !isTimeUnchanged)
            {
                MessageBox.Show("Giờ bắt đầu đã qua, vui lòng chọn lại.", "Giờ không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string note = (txtNote?.Text ?? string.Empty).Trim();
            if (note.Length > 200)
            {
                MessageBox.Show("Ghi chú tối đa 200 ký tự.", "Ghi chú quá dài", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            NewStart = start;
            NewEnd = end;
            NewNote = note;
            DialogResult = DialogResult.OK;
            Close();
        }

        private static int ParseDurationMinutes(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            if (text.Contains("60")) return 60;
            if (text.Contains("90")) return 90;
            if (text.Contains("120")) return 120;
            if (text.Contains("180")) return 180;
            return 0;
        }
    }
}
