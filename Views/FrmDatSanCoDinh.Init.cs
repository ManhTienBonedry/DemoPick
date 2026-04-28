using System;
using System.Drawing;
using System.Windows.Forms;

namespace DemoPick
{
    public partial class FrmDatSanCoDinh
    {
        private void FrmDatSanCoDinh_Load(object sender, EventArgs e)
        {
            try
            {
                var courts = _controller.GetCourts();
                cbCourt.DataSource = courts;
                cbCourt.DisplayMember = "Name";
                cbCourt.ValueMember = "CourtID";

                if (courts.Count > 0)
                    cbCourt.SelectedIndex = 0;

                if (_preselectedCourtId.HasValue)
                {
                    for (int i = 0; i < courts.Count; i++)
                    {
                        if (courts[i].CourtID == _preselectedCourtId.Value)
                        {
                            cbCourt.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Data Sân: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            DateTime selectedDate = _preselectedDate?.Date ?? DateTime.Today;
            if (ucDateRange != null)
            {
                ucDateRange.FromDate = selectedDate;
                ucDateRange.ToDate = CurrentMode == BookingMode.Fixed ? selectedDate.AddMonths(1) : selectedDate;
            }

            if (_dtStartClock != null)
            {
                DateTime now = DateTime.Now;
                DateTime seed = _preselectedStartTime ?? now;
                _dtStartClock.Value = now.Date.Add(seed.TimeOfDay);
            }

            if (CurrentMode == BookingMode.Quick)
            {
                cbDuration.SelectedItem = "90 phút";
            }

            // Select the correct radio based on constructor mode
            if (_mode == BookingMode.Quick)
            {
                rbDatNhanh.Checked = true;
            }
            else
            {
                rbKhachThue.Checked = true;
            }

            QueueConflictHintRefresh();
        }

        private void RbMode_CheckedChanged(object sender, EventArgs e)
        {
            // Only handle when a radio becomes checked (avoid double-fire)
            var rb = sender as Sunny.UI.UIRadioButton;
            if (rb != null && !rb.Checked) return;

            ApplyModeLayout();
            QueueConflictHintRefresh();
        }
    }
}
