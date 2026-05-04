using System;
using System.Drawing;
using System.Windows.Forms;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;

namespace DemoPick
{
    public partial class UCDateRangeFilter
    {
        // Thiet lap Attach Picker Change Detection va gan cac cau hinh/su kien can dung ban dau.
        private void AttachPickerChangeDetection(CuoreUI.Controls.cuiCalendarDatePicker picker, bool isFrom)
        {
            if (picker == null) return;

            void handler(object sender, EventArgs e)
            {
                OnPickerChanged(isFrom);
            }

            // Fallback WinForms events
            picker.TextChanged += handler;
            picker.Leave += handler;
            picker.LostFocus += handler;
            picker.MouseUp += handler;
            picker.MouseCaptureChanged += handler;

            // Watchdog polling after user interaction (CuoreUI sometimes updates value after popup closes)
            picker.MouseDown += (s, e) => StartWatchdog(isFrom);
            picker.GotFocus += (s, e) => StartWatchdog(isFrom);

            // Best-effort CuoreUI-specific events (if they exist)
            TryAttachEventHandler(picker, "ContentChanged", handler);
            TryAttachEventHandler(picker, "ValueChanged", handler);
            TryAttachEventHandler(picker, "DateChanged", handler);
            TryAttachEventHandler(picker, "SelectedDateChanged", handler);
        }

        // Nap du lieu cho Read Picker Date roi cap nhat lai trang thai hien thi tren man hinh.
        private static DateTime ReadPickerDate(CuoreUI.Controls.cuiCalendarDatePicker picker, DateTime fallback)
        {
            try
            {
                if (picker != null)
                {
                    var content = picker.Content;
                    // CuoreUI default can be MinValue; guard it.
                    if (content.Year >= 1900 && content.Year <= 2100)
                        return content.Date;
                }
            }
            catch
            {
                // ignore
            }

            try
            {
                string text = picker?.Text;
                if (!string.IsNullOrWhiteSpace(text) && DateTime.TryParse(text, out var parsed))
                    return parsed.Date;
            }
            catch
            {
                // ignore
            }

            return fallback.Date;
        }

        // Xu ly logic man hinh Start Watchdog va cap nhat control lien quan.
        private void StartWatchdog(bool isFrom)
        {
            try
            {
                if (isFrom)
                {
                    if (_watchFromTimer == null)
                    {
                        _watchFromTimer = new Timer { Interval = 100 };
                        _watchFromTimer.Tick += (s, e) => WatchdogTick(isFrom: true);
                    }
                    _watchFromTicks = 0;
                    _watchFromTimer.Stop();
                    _watchFromTimer.Start();
                }
                else
                {
                    if (_watchToTimer == null)
                    {
                        _watchToTimer = new Timer { Interval = 100 };
                        _watchToTimer.Tick += (s, e) => WatchdogTick(isFrom: false);
                    }
                    _watchToTicks = 0;
                    _watchToTimer.Stop();
                    _watchToTimer.Start();
                }
            }
            catch
            {
                // ignore
            }
        }

        // Xu ly logic man hinh Watchdog Tick va cap nhat control lien quan.
        private void WatchdogTick(bool isFrom)
        {
            // Stop after ~3 seconds to avoid any lingering timers.
            const int maxTicks = 30;

            if (isFrom)
            {
                _watchFromTicks++;
                OnPickerChanged(isFrom: true);
                if (_watchFromTicks >= maxTicks)
                {
                    try { _watchFromTimer.Stop(); } catch { }
                }
            }
            else
            {
                _watchToTicks++;
                OnPickerChanged(isFrom: false);
                if (_watchToTicks >= maxTicks)
                {
                    try { _watchToTimer.Stop(); } catch { }
                }
            }
        }

        // Thu thuc hien Try Attach Event Handler, neu du lieu khong hop le thi dung va tra thong bao phu hop.
        private static void TryAttachEventHandler(object instance, string eventName, EventHandler handler)
        {
            try
            {
                if (instance == null) return;
                if (handler == null) return;

                var ev = instance.GetType().GetEvent(eventName);
                if (ev == null) return;

                // Only attach to EventHandler-like events.
                if (ev.EventHandlerType == typeof(EventHandler))
                {
                    ev.AddEventHandler(instance, handler);
                    return;
                }
            }
            catch
            {
                // ignore
            }
        }

        // Thiet lap Attach Single Popup Behavior va gan cac cau hinh/su kien can dung ban dau.
        private static void AttachSinglePopupBehavior(Control picker)
        {
            if (picker == null) return;

            // Close any currently open CuoreUI date-picker popup before opening another.
            // CuoreUI doesn't expose a public "DroppedDown" API on the picker itself.
            picker.MouseDown += (s, e) =>
            {
                CloseOpenCuoreDatePickerPopups();
            };
        }

        // Xu ly logic man hinh Close Open Cuore Date Picker Popups va cap nhat control lien quan.
        private static void CloseOpenCuoreDatePickerPopups()
        {
            try
            {
                // Copy first: closing forms will modify Application.OpenForms.
                var toClose = new System.Collections.Generic.List<Form>();
                for (int i = 0; i < Application.OpenForms.Count; i++)
                {
                    var f = Application.OpenForms[i];
                    if (f == null) continue;

                    var typeName = f.GetType().FullName;
                    if (string.Equals(typeName, "CuoreUI.Controls.Forms.DatePicker", StringComparison.Ordinal))
                    {
                        toClose.Add(f);
                    }
                }

                foreach (var f in toClose)
                {
                    try { f.Close(); }
                    catch { }
                }
            }
            catch
            {
                // ignore
            }
        }

        // Ap dung hoac chuan hoa trang thai Apply Picker Visual de du lieu/giao dien nhat quan.
        private static void ApplyPickerVisual(CuoreUI.Controls.cuiCalendarDatePicker picker)
        {
            if (picker == null) return;

            try
            {
                // Softer rounded input: reduce sharp outer edges and match the page background.
                picker.BackColor = UiTheme.PageBackground;
                picker.Rounding = 12;
                picker.OutlineThickness = 1.0f;
                picker.NormalOutline = Color.FromArgb(229, 231, 235);
                picker.HoverOutline = Color.FromArgb(229, 231, 235);
                picker.PressedOutline = Color.FromArgb(229, 231, 235);

                // Use an opaque background to avoid harsh alpha edges.
                picker.NormalBackground = Color.FromArgb(224, 224, 224);
                picker.HoverBackground = Color.FromArgb(224, 224, 224);
                picker.PressedBackground = Color.FromArgb(224, 224, 224);

                // Enforce real rounded corners (clip region) so the outer border isn't sharp.
                ApplyRoundedRegion(picker, 12);
                picker.SizeChanged -= Picker_SizeChanged_ApplyRegion;
                picker.SizeChanged += Picker_SizeChanged_ApplyRegion;
            }
            catch
            {
                // ignore
            }
        }

        // Xu ly logic man hinh Picker_Size Changed_Apply Region va cap nhat control lien quan.
        private static void Picker_SizeChanged_ApplyRegion(object sender, EventArgs e)
        {
            if (sender is CuoreUI.Controls.cuiCalendarDatePicker picker)
            {
                ApplyRoundedRegion(picker, 12);
            }
        }

        // Ap dung hoac chuan hoa trang thai Apply Rounded Region de du lieu/giao dien nhat quan.
        private static void ApplyRoundedRegion(Control c, int radius)
        {
            if (c == null) return;
            if (c.Width <= 0 || c.Height <= 0) return;

            try
            {
                int r = Math.Max(0, radius);
                int d = r * 2;
                var rect = new Rectangle(0, 0, c.Width, c.Height);

                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    if (r == 0)
                    {
                        path.AddRectangle(rect);
                    }
                    else
                    {
                        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
                        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
                        path.CloseFigure();
                    }

                    var old = c.Region;
                    c.Region = new Region(path);
                    if (old != null) old.Dispose();
                }
            }
            catch
            {
                // ignore
            }
        }

        // Xu ly logic man hinh On Picker Changed va cap nhat control lien quan.
        private void OnPickerChanged(bool isFrom)
        {
            if (_suppressEvents) return;

            if (_mode == DateFilterMode.SingleDate)
            {
                if (!isFrom) return;

                var cur = ReadPickerDate(dtFrom, DateTime.Today);
                if (cur == _lastFromContentDate) return;
                _lastFromContentDate = cur;

                if (IsHandleCreated)
                {
                    BeginInvoke((MethodInvoker)(() => SelectedDateChanged?.Invoke(this, EventArgs.Empty)));
                }
                else
                {
                    SelectedDateChanged?.Invoke(this, EventArgs.Empty);
                }
                return;
            }

            // Range
            var from = ReadPickerDate(dtFrom, DateTime.Today);
            var to = ReadPickerDate(dtTo, DateTime.Today);

            bool changed = false;
            if (from != _lastFromContentDate) { _lastFromContentDate = from; changed = true; }
            if (to != _lastToContentDate) { _lastToContentDate = to; changed = true; }
            if (!changed) return;

            if (IsHandleCreated)
            {
                BeginInvoke((MethodInvoker)(() => RangeChanged?.Invoke(this, EventArgs.Empty)));
            }
            else
            {
                RangeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        // Ap dung hoac chuan hoa trang thai Set Picker Date de du lieu/giao dien nhat quan.
        private void SetPickerDate(CuoreUI.Controls.cuiCalendarDatePicker picker, DateTime date)
        {
            if (picker == null) return;

            try
            {
                _suppressEvents = true;
                picker.Content = date.Date;
                picker.Text = date.ToString("yyyy-MM-dd");

                if (ReferenceEquals(picker, dtFrom)) _lastFromContentDate = date.Date;
                if (ReferenceEquals(picker, dtTo)) _lastToContentDate = date.Date;
            }
            finally
            {
                _suppressEvents = false;
            }
        }

        // Thiet lap Attach Read Only Date Picker Behavior va gan cac cau hinh/su kien can dung ban dau.
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

        // Ap dung hoac chuan hoa trang thai Apply Mode Layout de du lieu/giao dien nhat quan.
        private void ApplyModeLayout()
        {
            bool isRange = _mode == DateFilterMode.Range;

            lblFrom.Visible = isRange;
            lblTo.Visible = isRange;
            dtTo.Visible = isRange;
            btnApply.Visible = isRange && _showApplyButton;

            btnPrevDay.Visible = !isRange;
            btnNextDay.Visible = !isRange;

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
                btnPrevDay.Location = new Point(0, 2);
                btnPrevDay.Size = new Size(40, 35);

                btnNextDay.Location = new Point(46, 2);
                btnNextDay.Size = new Size(40, 35);

                dtFrom.Location = new Point(100, 0);
                dtFrom.Size = new Size(152, 39);
            }

            BackColor = Color.Transparent;
        }
    }
}


