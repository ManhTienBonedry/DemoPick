using System;
using System.Windows.Forms;
using DemoPick.Services;

namespace DemoPick
{
    public partial class FrmDatSanCoDinh
    {
        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            if (rbKhachThue.Checked && (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtPhone.Text)))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin khách thuê!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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

            int courtId = 0;
            object selected = cbCourt?.SelectedValue;
            if (selected is int id)
            {
                courtId = id;
            }
            else if (selected != null && int.TryParse(selected.ToString(), out int parsed))
            {
                courtId = parsed;
            }
            if (courtId <= 0)
            {
                MessageBox.Show("Không xác định được sân. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string timeStr = cbTime.SelectedItem?.ToString() ?? "17:00";
            if (!TryParseHourMinute(timeStr, out int hh, out int mm))
            {
                MessageBox.Show("Giờ bắt đầu không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int durationMins = ParseDurationMinutes(cbDuration.SelectedItem?.ToString());
            if (durationMins <= 0) durationMins = 90;

            bool dMon = chkMon.Checked;
            bool dTue = chkTue.Checked;
            bool dWed = chkWed.Checked;
            bool dThu = chkThu.Checked;
            bool dFri = chkFri.Checked;
            bool dSat = chkSat.Checked;
            bool dSun = chkSun.Checked;

            string status = rbBaoTri.Checked ? AppConstants.BookingStatus.Maintenance : AppConstants.BookingStatus.Confirmed;
            string guestName = rbBaoTri.Checked ? (txtName.Text ?? "Ban Quản Lý (Bảo Trì)") : (txtName.Text.Trim() + " - " + txtPhone.Text.Trim());
            string note = (txtNote.Text ?? "").Trim();

            int? memberId = null;
            if (!string.Equals(status, AppConstants.BookingStatus.Maintenance, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    memberId = _controller.GetOrCreateMemberId(txtName.Text, txtPhone.Text);
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

            for (DateTime d = from; d <= to; d = d.AddDays(1))
            {
                if (!IsSelectedDay(d.DayOfWeek, dMon, dTue, dWed, dThu, dFri, dSat, dSun))
                    continue;

                DateTime start = d.AddHours(hh).AddMinutes(mm);
                DateTime end = start.AddMinutes(durationMins);
                if (end <= start) continue;

                // Skip occurrences in the past to reduce accidental spam.
                if (end <= now)
                {
                    skippedPast++;
                    continue;
                }

                try
                {
                    _controller.SubmitBooking(courtId, memberId, guestName, note, start, end, status);
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

        private static int ParseDurationMinutes(string durationText)
        {
            string t = durationText ?? "";
            if (t.IndexOf("60", StringComparison.OrdinalIgnoreCase) >= 0) return 60;
            if (t.IndexOf("90", StringComparison.OrdinalIgnoreCase) >= 0) return 90;
            if (t.IndexOf("120", StringComparison.OrdinalIgnoreCase) >= 0) return 120;
            if (t.IndexOf("180", StringComparison.OrdinalIgnoreCase) >= 0) return 180;
            return 0;
        }

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
