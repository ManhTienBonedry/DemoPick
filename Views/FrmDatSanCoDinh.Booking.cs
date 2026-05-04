using System;
using System.Windows.Forms;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;

namespace DemoPick
{
    public partial class FrmDatSanCoDinh
    {
        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            if (rbKhachThue.Checked && (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtPhone.Text)))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin khách thuê!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string phoneDigits = PhoneNumberValidator.NormalizeDigits(txtPhone.Text);
            if (rbKhachThue.Checked && phoneDigits.Length != 10)
            {
                MessageBox.Show("Số điện thoại phải đúng 10 chữ số.", "Sai số điện thoại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhone.Focus();
                return;
            }

            if (CurrentMode == BookingMode.Quick)
            {
                SubmitQuickBooking(phoneDigits);
                return;
            }

            SubmitRecurringBooking(phoneDigits);
        }

        // Luu hoac ghi nhan Submit Quick Booking vao trang thai he thong/CSDL khi nghiep vu yeu cau.
        private void SubmitQuickBooking(string phoneDigits)
        {
            int courtId = ResolveSelectedCourtId();
            if (courtId <= 0)
            {
                MessageBox.Show("Không xác định được sân. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int durationMins = ParseDurationMinutesFromUi();
            if (durationMins <= 0) durationMins = 90;

            DateTime selectedDate = ResolveSelectedDate();
            DateTime start = selectedDate.Date.Add(GetSelectedStartTimeOfDay());
            DateTime end = start.AddMinutes(durationMins);

            if (start.Date == DateTime.Today && start < DateTime.Now)
            {
                MessageBox.Show("Không thể đặt sân trong quá khứ. Vui lòng chọn giờ khác.", "Lỗi chọn giờ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (HasConflictAtStart(courtId, start, end))
            {
                MessageBox.Show("Khung giờ này đã có booking trên sân đã chọn.", "Trùng lịch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string status = rbBaoTri.Checked ? AppConstants.BookingStatus.Maintenance : AppConstants.BookingStatus.Confirmed;
            string guestName = rbBaoTri.Checked ? (txtName.Text ?? "Ban Quản Lý (Bảo Trì)") : (txtName.Text.Trim() + " - " + phoneDigits);
            string note = (txtNote.Text ?? "").Trim();

            int? memberId = null;
            if (!string.Equals(status, AppConstants.BookingStatus.Maintenance, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    memberId = _controller.GetOrCreateMemberId(txtName.Text, phoneDigits);
                }
                catch (Exception ex)
                {
                    DatabaseHelper.TryLogThrottled(
                        throttleKey: "FrmDatSanCoDinh.GetOrCreateMemberId.Quick",
                        eventDesc: "Member Upsert Error",
                        ex: ex,
                        context: "FrmDatSanCoDinh.SubmitQuickBooking",
                        minSeconds: 300);
                }
            }

            try
            {
                _controller.SubmitBooking(courtId, memberId, guestName, note, start, end, status, ResolvePaymentState());

                MessageBox.Show(
                    $"Đặt sân thành công!\n- {start:dd/MM/yyyy HH:mm} đến {end:HH:mm}",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể đặt sân: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Luu hoac ghi nhan Submit Recurring Booking vao trang thai he thong/CSDL khi nghiep vu yeu cau.
        private void SubmitRecurringBooking(string phoneDigits)
        {
            // Check if at least one day is checked
            if (!chkMon.Checked && !chkTue.Checked && !chkWed.Checked && !chkThu.Checked &&
                !chkFri.Checked && !chkSat.Checked && !chkSun.Checked)
            {
                MessageBox.Show("Vui lòng chọn ít nhất 1 ngày trong tuần để lặp lại!", "Lý lịch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime fromDate = ucDateRange?.FromDate ?? DateTime.Today;
            DateTime toDate = ucDateRange?.ToDate ?? DateTime.Today;

            if (toDate <= fromDate)
            {
                MessageBox.Show("Ngày kết thúc phải lớn hơn ngày bắt đầu!", "Lỗi ngày", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int courtId = ResolveSelectedCourtId();
            if (courtId <= 0)
            {
                MessageBox.Show("Không xác định được sân. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            TimeSpan startOfDay = GetSelectedStartTimeOfDay();
            int durationMins = ParseDurationMinutesFromUi();
            if (durationMins <= 0) durationMins = 90;

            bool dMon = chkMon.Checked;
            bool dTue = chkTue.Checked;
            bool dWed = chkWed.Checked;
            bool dThu = chkThu.Checked;
            bool dFri = chkFri.Checked;
            bool dSat = chkSat.Checked;
            bool dSun = chkSun.Checked;

            string status = rbBaoTri.Checked ? AppConstants.BookingStatus.Maintenance : AppConstants.BookingStatus.Confirmed;
            string guestName = rbBaoTri.Checked ? (txtName.Text ?? "Ban Quản Lý (Bảo Trì)") : (txtName.Text.Trim() + " - " + phoneDigits);
            string note = (txtNote.Text ?? "").Trim();

            int? memberId = null;
            if (!string.Equals(status, AppConstants.BookingStatus.Maintenance, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    memberId = _controller.GetOrCreateMemberId(txtName.Text, phoneDigits);
                }
                catch (Exception ex)
                {
                    DatabaseHelper.TryLogThrottled(
                        throttleKey: "FrmDatSanCoDinh.GetOrCreateMemberId",
                        eventDesc: "Member Upsert Error",
                        ex: ex,
                        context: "FrmDatSanCoDinh.BtnConfirm_Click",
                        minSeconds: 300);
                }
            }

            int created = 0;
            int conflicts = 0;
            int skippedPast = 0;
            int errors = 0;

            DateTime from = fromDate.Date;
            DateTime to = toDate.Date;
            DateTime now = DateTime.Now;
            DateTime today = now.Date;

            for (DateTime d = from; d <= to; d = d.AddDays(1))
            {
                if (!IsSelectedDay(d.DayOfWeek, dMon, dTue, dWed, dThu, dFri, dSat, dSun))
                    continue;

                DateTime start = d.Date.Add(startOfDay);
                DateTime end = start.AddMinutes(durationMins);
                if (end <= start) continue;

                // Past dates are never valid for recurring creation.
                if (d.Date < today)
                {
                    skippedPast++;
                    continue;
                }

                bool isToday = start.Date == today;
                if (isToday && start < now)
                {
                    skippedPast++;
                    continue;
                }

                try
                {
                    if (HasConflictAtStart(courtId, start, end))
                    {
                        conflicts++;
                        continue;
                    }

                    _controller.SubmitBooking(courtId, memberId, guestName, note, start, end, status, ResolvePaymentState());
                    created++;
                }
                catch (Exception ex)
                {
                    // Stored proc uses RAISERROR for conflicts.
                    if ((ex.Message ?? "").IndexOf("already booked", StringComparison.OrdinalIgnoreCase) >= 0)
                        conflicts++;
                    else
                        errors++;
                }
            }

            if (created <= 0)
            {
                MessageBox.Show("Không tạo được lịch nào (có thể do trùng lịch hoặc toàn bộ mốc thời gian đã qua).", "Không có thay đổi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.Cancel;
                return;
            }

            string modeStr = rbBaoTri.Checked ? "BẢO TRÌ SÂN" : "THUÊ CỐ ĐỊNH";
            MessageBox.Show(
                $"Đã tạo lịch {modeStr}!\n\n" +
                $"- Tạo mới: {created}\n" +
                (conflicts > 0 ? $"- Trùng lịch: {conflicts}\n" : "") +
                (skippedPast > 0 ? $"- Bỏ qua (quá khứ): {skippedPast}\n" : "") +
                (errors > 0 ? $"- Lỗi khác: {errors}\n" : ""),
                "Thành công",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // Kiem tra dieu kien Is Selected Day va tra ve ket qua dung/sai cho luong xu ly.
        private static bool IsSelectedDay(DayOfWeek day, bool mon, bool tue, bool wed, bool thu, bool fri, bool sat, bool sun)
        {
            switch (day)
            {
                case DayOfWeek.Monday: return mon;
                case DayOfWeek.Tuesday: return tue;
                case DayOfWeek.Wednesday: return wed;
                case DayOfWeek.Thursday: return thu;
                case DayOfWeek.Friday: return fri;
                case DayOfWeek.Saturday: return sat;
                case DayOfWeek.Sunday: return sun;
                default: return false;
            }
        }

        // Dien giai hoac xac dinh Parse Duration Minutes tu du lieu dau vao de cac buoc sau dung thong nhat.
        private static int ParseDurationMinutes(string durationText)
        {
            return ParseDurationMinutesCore(durationText);
        }

        // Thu thuc hien Try Parse Hour Minute, neu du lieu khong hop le thi dung va tra thong bao phu hop.
        private static bool TryParseHourMinute(string timeText, out int hours, out int minutes)
        {
            hours = 0;
            minutes = 0;

            string t = (timeText ?? "").Trim();
            string[] parts = t.Split(':');
            if (parts.Length < 2) return false;
            if (!int.TryParse(parts[0], out hours)) return false;
            if (!int.TryParse(parts[1], out minutes)) return false;
            if (hours < 0 || hours > 23) return false;
            if (minutes < 0 || minutes > 59) return false;
            return true;
        }
    }
}


