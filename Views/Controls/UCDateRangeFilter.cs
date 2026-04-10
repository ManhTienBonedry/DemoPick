using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DemoPick.Services;

namespace DemoPick
{
    public partial class UCDateRangeFilter : UserControl
    {
        public enum DateFilterMode
        {
            SingleDate = 0,
            Range = 1,
        }

        private DateFilterMode _mode = DateFilterMode.SingleDate;
        private bool _suppressEvents;
        private bool _showApplyButton = true;

        private DateTime _lastFromContentDate;
        private DateTime _lastToContentDate;

        private Timer _watchFromTimer;
        private Timer _watchToTimer;
        private int _watchFromTicks;
        private int _watchToTicks;

        public event EventHandler SelectedDateChanged;
        public event EventHandler RangeChanged;
        public event EventHandler ApplyClicked;

        public UCDateRangeFilter()
        {
            InitializeComponent();

            if (!DesignModeUtil.IsDesignMode(this))
            {
                ApplyPickerVisual(dtFrom);
                ApplyPickerVisual(dtTo);

                AttachSinglePopupBehavior(dtFrom);
                AttachSinglePopupBehavior(dtTo);
            }

            AttachReadOnlyDatePickerBehavior(dtFrom);
            AttachReadOnlyDatePickerBehavior(dtTo);

            _lastFromContentDate = ReadPickerDate(dtFrom, DateTime.Today);
            _lastToContentDate = ReadPickerDate(dtTo, DateTime.Today);

            AttachPickerChangeDetection(dtFrom, isFrom: true);
            AttachPickerChangeDetection(dtTo, isFrom: false);

            MouseButtons prevDown = MouseButtons.None;
            MouseButtons nextDown = MouseButtons.None;
            MouseButtons applyDown = MouseButtons.None;

            btnPrevDay.MouseDown += (s, e) => prevDown = e.Button;
            btnNextDay.MouseDown += (s, e) => nextDown = e.Button;
            btnApply.MouseDown += (s, e) => applyDown = e.Button;

            btnPrevDay.Click += (s, e) =>
            {
                if (prevDown != MouseButtons.Left) return;
                SelectedDate = SelectedDate.AddDays(-1);
            };
            btnNextDay.Click += (s, e) =>
            {
                if (nextDown != MouseButtons.Left) return;
                SelectedDate = SelectedDate.AddDays(1);
            };
            btnApply.Click += (s, e) =>
            {
                if (applyDown != MouseButtons.Left) return;
                ApplyClicked?.Invoke(this, EventArgs.Empty);
            };

            Disposed += (s, e) =>
            {
                try { _watchFromTimer?.Stop(); } catch { }
                try { _watchToTimer?.Stop(); } catch { }
                try { _watchFromTimer?.Dispose(); } catch { }
                try { _watchToTimer?.Dispose(); } catch { }
                _watchFromTimer = null;
                _watchToTimer = null;
            };

            ApplyModeLayout();
        }

        [Browsable(true)]
        [DefaultValue(true)]
        public bool ShowApplyButton
        {
            get => _showApplyButton;
            set
            {
                if (_showApplyButton == value) return;
                _showApplyButton = value;
                ApplyModeLayout();
            }
        }

        [Browsable(true)]
        [DefaultValue(DateFilterMode.SingleDate)]
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

        [Browsable(false)]
        public DateTime SelectedDate
        {
            get => FromDate;
            set => FromDate = value;
        }

        [Browsable(false)]
        public DateTime FromDate
        {
            get => (dtFrom?.Content ?? DateTime.Today).Date;
            set
            {
                SetPickerDate(dtFrom, value);

                if (_mode == DateFilterMode.SingleDate)
                {
                    SelectedDateChanged?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    RangeChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        [Browsable(false)]
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

        [Browsable(false)]
        public bool ApplyEnabled
        {
            get => btnApply?.Enabled ?? true;
            set
            {
                if (btnApply != null) btnApply.Enabled = value;
            }
        }

        public bool ValidateRange(out string error)
        {
            error = null;
            if (_mode != DateFilterMode.Range) return true;

            if (FromDate > ToDate)
            {
                error = "Ngày 'Từ' phải nhỏ hơn hoặc bằng ngày 'Đến'.";
                return false;
            }

            return true;
        }
    }
}
