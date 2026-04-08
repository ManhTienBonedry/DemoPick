using System;
using System.Drawing;
using System.Windows.Forms;

namespace DemoPick
{
    public partial class UCDateRangeFilter : UserControl
    {
        public enum DateFilterMode
        {
            SingleDate = 0,
            Range = 1,
        }

        private DateFilterMode _mode = DateFilterMode.Range;
        private bool _suppressEvents;

        public event EventHandler SelectedDateChanged;
        public event EventHandler RangeChanged;
        public event EventHandler ApplyClicked;

        public UCDateRangeFilter()
        {
            InitializeComponent();

            // Prevent user from typing manually
            AttachReadOnlyDatePickerBehavior(dtFrom);
            AttachReadOnlyDatePickerBehavior(dtTo);

            // Value change detection (CuoreUI uses Content + TextChanged)
            dtFrom.TextChanged += (s, e) => OnPickerTextChanged(isFrom: true);
            dtTo.TextChanged += (s, e) => OnPickerTextChanged(isFrom: false);

            btnPrevDay.Click += (s, e) => SelectedDate = SelectedDate.AddDays(-1);
            btnNextDay.Click += (s, e) => SelectedDate = SelectedDate.AddDays(1);
            btnApply.Click += (s, e) => ApplyClicked?.Invoke(this, EventArgs.Empty);

            ApplyModeLayout();
        }

        public DateFilterMode Mode
        {
            get => _mode;
            set
            {
                if (_mode == value) return;
                _mode = value;
                ApplyModeLayout();
            }
        }

        public DateTime SelectedDate
        {
            get => (dtFrom?.Content ?? DateTime.Today).Date;
            set
            {
                SetPickerDate(dtFrom, value);
                if (_mode == DateFilterMode.SingleDate)
                {
                    SelectedDateChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public DateTime FromDate
        {
            get => (dtFrom?.Content ?? DateTime.Today).Date;
            set
            {
                SetPickerDate(dtFrom, value);
                if (_mode == DateFilterMode.Range)
                {
                    RangeChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public DateTime ToDate
        {
            get => (dtTo?.Content ?? DateTime.Today).Date;
            set
            {
                SetPickerDate(dtTo, value);
                if (_mode == DateFilterMode.Range)
                {
                    RangeChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool ApplyEnabled
        {
            get => btnApply?.Enabled ?? false;
            set
            {
                if (btnApply != null) btnApply.Enabled = value;
            }
        }

        public bool ValidateRange(out string error)
        {
            error = null;
            if (_mode != DateFilterMode.Range) return true;

            var from = FromDate;
            var to = ToDate;
            if (from > to)
            {
                error = "Ngày 'Từ' phải nhỏ hơn hoặc bằng ngày 'Đến'.";
                return false;
            }

            return true;
        }

        private void OnPickerTextChanged(bool isFrom)
        {
            if (_suppressEvents) return;

            if (_mode == DateFilterMode.SingleDate)
            {
                if (!isFrom) return;
                SelectedDateChanged?.Invoke(this, EventArgs.Empty);
                return;
            }

            // Range
            RangeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SetPickerDate(CuoreUI.Controls.cuiCalendarDatePicker picker, DateTime date)
        {
            if (picker == null) return;

            try
            {
                _suppressEvents = true;
                picker.Content = date.Date;
                picker.Text = date.ToString("yyyy-MM-dd");
            }
            finally
            {
                _suppressEvents = false;
            }
        }

        private static void AttachReadOnlyDatePickerBehavior(Control picker)
        {
            if (picker == null) return;

            picker.KeyPress += (s, e) => { e.Handled = true; };
            picker.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };
        }

        private void ApplyModeLayout()
        {
            bool isRange = _mode == DateFilterMode.Range;

            lblFrom.Visible = isRange;
            lblTo.Visible = isRange;
            dtTo.Visible = isRange;
            btnApply.Visible = isRange;

            btnPrevDay.Visible = !isRange;
            btnNextDay.Visible = !isRange;

            // Layout tweaks to keep consistent sizing with existing screens
            if (isRange)
            {
                dtFrom.Location = new Point(55, 5);
                dtFrom.Size = new Size(152, 32);

                lblFrom.Location = new Point(20, 12);
                lblTo.Location = new Point(215, 12);

                dtTo.Location = new Point(259, 5);
                dtTo.Size = new Size(152, 32);

                btnApply.Location = new Point(425, 5);
                btnApply.Size = new Size(110, 32);
            }
            else
            {
                // Prev/Next + single picker
                btnPrevDay.Location = new Point(0, 2);
                btnPrevDay.Size = new Size(40, 35);

                btnNextDay.Location = new Point(46, 2);
                btnNextDay.Size = new Size(40, 35);

                dtFrom.Location = new Point(100, 0);
                dtFrom.Size = new Size(152, 39);
            }
        }
    }
}
