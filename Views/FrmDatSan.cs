using System;
using System.Drawing;
using System.Windows.Forms;
using DemoPick.Services;

namespace DemoPick
{
    public partial class FrmDatSan : Form
    {
        private DemoPick.Controllers.BookingController _controller = new DemoPick.Controllers.BookingController();

        public FrmDatSan()
        {
            InitializeComponent();

            try
            {
                if (ucDate != null)
                {
                    ucDate.Mode = UCDateRangeFilter.DateFilterMode.SingleDate;
                    ucDate.SelectedDate = DateTime.Today;
                }
            }
            catch
            {
                // ignore
            }
            
            this.Load += FrmDatSan_Load;

            TryClampComboDropDownToScreen(cbTime, maxHeight: 260, minHeight: 120);

            btnCancel.Click += (s, e) => this.Close();
            btnCancelTop.Click += (s, e) => this.Close();
            
            btnConfirm.Click += BtnConfirm_Click;

            if (txtPhone != null)
            {
                txtPhone.TextChanged += (s, e) => UpdatePhoneValidationUi();
            }
            
            pnlTop.MouseDown += Form_MouseDown;
            lblHeaderTop.MouseDown += Form_MouseDown;
        }

        private void UpdatePhoneValidationUi()
        {
            if (txtPhone == null) return;

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

        private static void TryClampComboDropDownToScreen(ComboBox combo, int maxHeight, int minHeight)
        {
            if (combo == null) return;

            try
            {
                combo.IntegralHeight = false;
                combo.DropDown += (s, e) =>
                {
                    try
                    {
                        var working = Screen.FromControl(combo).WorkingArea;
                        var below = combo.PointToScreen(new System.Drawing.Point(0, combo.Height));
                        var top = combo.PointToScreen(System.Drawing.Point.Empty);

                        int spaceBelow = working.Bottom - below.Y - 8;
                        int spaceAbove = top.Y - working.Top - 8;

                        int available = Math.Max(spaceBelow, spaceAbove);
                        int h = Math.Min(maxHeight, available);
                        h = Math.Max(minHeight, h);

                        combo.DropDownHeight = h;
                    }
                    catch
                    {
                        // ignore
                    }
                };
            }
            catch
            {
                // ignore
            }
        }

        private void FrmDatSan_Load(object sender, EventArgs e)
        {
            try
            {
                var courts = _controller.GetCourts();
                cbCourt.DataSource = courts;
                cbCourt.DisplayMember = "Name";
                cbCourt.ValueMember = "CourtID";

                if (courts.Count > 0) cbCourt.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Data Sân: " + ex.Message);
            }

            cbTime.SelectedIndex = 22; // 17:00 (30-minute slots)
            cbDuration.SelectedIndex = 1; // 90 phút
            cbPayment.SelectedIndex = 0; // Trực tiếp
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin khách hàng!", "Cảnh báo thiết sót", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string phoneDigits = PhoneNumberValidator.NormalizeDigits(txtPhone.Text);
            if (phoneDigits.Length != 10)
            {
                MessageBox.Show("Số điện thoại phải đúng 10 chữ số.", "Sai số điện thoại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhone.Focus();
                return;
            }

            DateTime selectedDate = ucDate.SelectedDate;
            string timeStr = cbTime.SelectedItem?.ToString() ?? "17:00"; 
            string[] timeParts = timeStr.Split(':');
            int hours = int.Parse(timeParts[0]);
            int mins = int.Parse(timeParts[1]);
            DateTime start = selectedDate.AddHours(hours).AddMinutes(mins);
            
            bool isToday = start.Date == DateTime.Today;
            if (isToday && start < DateTime.Now)
            {
                MessageBox.Show("Không thể đặt sân trong quá khứ. Vui lòng chọn giờ khác.", "Lỗi chọn giờ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int durationMins = 90;
            string selDur = cbDuration.SelectedItem?.ToString() ?? "90";
            if (selDur.Contains("60")) durationMins = 60;
            else if (selDur.Contains("120")) durationMins = 120;
            else if (selDur.Contains("180")) durationMins = 180;
            
            DateTime end = start.AddMinutes(durationMins);
            int courtId = cbCourt.SelectedValue != null ? (int)cbCourt.SelectedValue : 1;

            try
            {
                string note = (txtNote.Text ?? "").Trim();
                int? memberId = null;
                try
                {
                    memberId = _controller.GetOrCreateMemberId(txtName.Text, phoneDigits);
                }
                catch (Exception ex)
                {
                    DatabaseHelper.TryLogThrottled(
                        throttleKey: "FrmDatSan.GetOrCreateMemberId",
                        eventDesc: "Member Upsert Error",
                        ex: ex,
                        context: "FrmDatSan.BtnConfirm_Click",
                        minSeconds: 300);
                }

                    string paymentState = MapPaymentSelectionToState();
                    _controller.SubmitBooking(courtId, memberId, txtName.Text + " - " + phoneDigits, note, start, end, status: AppConstants.BookingStatus.Confirmed, paymentState: paymentState);
                MessageBox.Show($"Đã chốt sân thành công!\n- {txtName.Text}\n- Mốc: Từ {start:HH:mm} đến {end:HH:mm} ngày {start:dd/MM/yyyy}", " Đặt sân hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK; 
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("LỖI ĐẶT SÂN: " + ex.Message, "Cảnh báo trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string MapPaymentSelectionToState()
        {
            string selected = (cbPayment?.SelectedItem?.ToString() ?? string.Empty).Trim();

            if (string.Equals(selected, "Đã chuyển khoản", StringComparison.OrdinalIgnoreCase))
                return AppConstants.BookingPaymentState.BankTransferred;

            if (string.Equals(selected, "Đã đặt cọc 50%", StringComparison.OrdinalIgnoreCase))
                return AppConstants.BookingPaymentState.Deposit50;

            return AppConstants.BookingPaymentState.PayAtVenue;
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(Handle, NativeMethods.WM_NCLBUTTONDOWN, NativeMethods.HT_CAPTION, 0);
            }
        }
    }
    
    // Helper to drag frameless winforms natively
    public static class NativeMethods
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(System.IntPtr hWnd, int Msg, int wParam, int lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
    }
}
