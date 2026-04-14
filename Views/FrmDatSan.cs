using System;
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
            
            pnlTop.MouseDown += Form_MouseDown;
            lblHeaderTop.MouseDown += Form_MouseDown;
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

            cbTime.SelectedIndex = 11; // 17:00
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

            DateTime selectedDate = ucDate.SelectedDate;
            string timeStr = cbTime.SelectedItem?.ToString() ?? "17:00"; 
            string[] timeParts = timeStr.Split(':');
            int hours = int.Parse(timeParts[0]);
            int mins = int.Parse(timeParts[1]);
            DateTime start = selectedDate.AddHours(hours).AddMinutes(mins);
            
            if (start < DateTime.Now)
            {
                MessageBox.Show("Thời gian bắt đầu đã qua hãy chọn lại thời gian.", "Lỗi chọn giờ", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    memberId = _controller.GetOrCreateMemberId(txtName.Text, txtPhone.Text);
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

                _controller.SubmitBooking(courtId, memberId, txtName.Text + " - " + txtPhone.Text, note, start, end, status: AppConstants.BookingStatus.Confirmed);
                MessageBox.Show($"Đã chốt sân thành công!\n- {txtName.Text}\n- Mốc: Từ {start:HH:mm} đến {end:HH:mm} ngày {start:dd/MM/yyyy}", " Đặt sân hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK; 
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("LỖI ĐẶT SÂN: " + ex.Message, "Cảnh báo trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
