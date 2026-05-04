using DemoPick.Helpers;
using DemoPick.Data;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DemoPick
{
    public partial class FrmDatSanCoDinh
    {
        // Nap du lieu va chuan bi trang thai khi man hinh/control vua duoc mo.
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

            RefreshConflictHintUi();
        }

        // Cap nhat trang thai khi checkbox/toggle thay doi.
        private void RbMode_CheckedChanged(object sender, EventArgs e)
        {
            // Only handle when a radio becomes checked (avoid double-fire)
            var rb = sender as Sunny.UI.UIRadioButton;
            if (rb != null && !rb.Checked) return;

            SuspendLayout();
            ApplyModeLayout();
            RefreshConflictHintUi();
            ResumeLayout(true);
        }
    }
}

