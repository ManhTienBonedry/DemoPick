using System;
using System.Drawing;
using System.Windows.Forms;
using DemoPick.Services;
using Panel = System.Windows.Forms.Panel;

namespace DemoPick
{
    public partial class UCThanhToan
    {
        private static readonly Font _checkoutCourtNameFont = new Font("Segoe UI", 11F, FontStyle.Bold);
        private static readonly Font _checkoutCourtBadgeFont = new Font("Segoe UI", 9F, FontStyle.Bold);
        private static readonly Font _checkoutCourtInfoFont = new Font("Segoe UI", 9F, FontStyle.Italic);
        private static readonly Font _checkoutEmptyStateFont = new Font("Segoe UI", 11F, FontStyle.Italic);

        private static void ClearAndDisposeChildControls(Control parent)
        {
            if (parent == null) return;
            if (parent.Controls == null) return;
            if (parent.Controls.Count == 0) return;

            var old = new Control[parent.Controls.Count];
            parent.Controls.CopyTo(old, 0);
            parent.Controls.Clear();

            for (int i = 0; i < old.Length; i++)
            {
                old[i].Dispose();
            }
        }

        private void LoadCourts()
        {
            try
            {
                ClearAndDisposeChildControls(flpCourts);
                var bookCtrl = new DemoPick.Controllers.BookingController();
                var courts = bookCtrl.GetCourts();

                foreach (var c in courts)
                {
                    // Check if court has pending order or is actively playing.
                    var lines = PosService.GetPendingOrder(c.Name);
                    bool hasOrder = lines.Count > 0;

                    var bookings = bookCtrl.GetBookingsByDate(DateTime.Now);
                    var currentBooking = bookings.Find(b =>
                        b.CourtID == c.CourtID &&
                        !string.Equals(b.Status, AppConstants.BookingStatus.Maintenance, StringComparison.OrdinalIgnoreCase) &&
                        DateTime.Now >= b.StartTime && DateTime.Now <= b.EndTime);
                    bool active = currentBooking != null;

                    if (!hasOrder && !active) continue; // Only show courts that need to be checked out

                    string statusTxt = hasOrder ? "Có order" : (active ? "Đang chơi" : "");
                    Color lineCol = hasOrder ? Color.FromArgb(231, 76, 60) : Color.FromArgb(76, 175, 80);

                    Panel pnlCtx = new Panel { Size = new Size(240, 80), BackColor = Color.White, Margin = new Padding(0, 0, 0, 10), Cursor = Cursors.Hand };
                    pnlCtx.Paint += (s, e) =>
                    {
                        using (var pen = new Pen(Color.FromArgb(229, 231, 235), 1))
                        using (var brush = new SolidBrush(lineCol))
                        {
                            e.Graphics.DrawRectangle(pen, 0, 0, pnlCtx.Width - 1, pnlCtx.Height - 1);
                            e.Graphics.FillRectangle(brush, 0, 10, 4, pnlCtx.Height - 20);
                        }
                    };

                    Label cName = new Label { Text = c.Name, Font = _checkoutCourtNameFont, ForeColor = Color.FromArgb(26, 35, 50), Location = new Point(15, 15), AutoSize = true };
                    Label badge = new Label { Text = statusTxt, Font = _checkoutCourtBadgeFont, ForeColor = Color.White, BackColor = lineCol, Location = new Point(150, 17), AutoSize = true, Padding = new Padding(2) };
                    Label cOrderInfo = new Label { Text = $"Đỏ: {lines.Count} món đang chờ.", Font = _checkoutCourtInfoFont, ForeColor = Color.Gray, Location = new Point(15, 45), AutoSize = true };

                    pnlCtx.Controls.AddRange(new Control[] { cName, badge, cOrderInfo });

                    EventHandler selectCourt = (s, e) =>
                    {
                        foreach (Control p in flpCourts.Controls) { if (p is Panel panel) panel.BackColor = Color.White; }
                        pnlCtx.BackColor = Color.FromArgb(235, 248, 235);
                        SelectCourtToCheckout(c, currentBooking);
                    };

                    pnlCtx.Click += selectCourt;
                    cName.Click += selectCourt;
                    badge.Click += selectCourt;
                    cOrderInfo.Click += selectCourt;

                    flpCourts.Controls.Add(pnlCtx);
                }

                if (flpCourts.Controls.Count == 0)
                {
                    flpCourts.Controls.Add(new Label { Text = "Không có sân nào đang cần thanh toán", Font = _checkoutEmptyStateFont, AutoSize = true, Margin = new Padding(20), ForeColor = Color.Gray });
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("ThanhToan Load Courts Error", ex, "UCThanhToan.LoadCourts");
            }
        }

        private void SelectCourtToCheckout(DemoPick.Models.CourtModel court, DemoPick.Models.BookingModel booking)
        {
            string courtName = court.Name;
            _selectedCourtName = courtName;
            lblRightTitle.Text = "Hóa đơn - " + courtName;

            _currentBooking = booking;
            _selectedCourt = court;
            _cartTotal = 0;

            UpdateTotals();
        }

        private void ResetCheckoutPane()
        {
            _cartTotal = 0;
            _currentDiscountPct = 0;
            _currentCustomerId = 0;
            txtCustomerPhone.Text = "";
            lblCustomerInfo.Text = "Khách lẻ (Không áp dụng thẻ)";
            lblCustomerInfo.ForeColor = Color.Gray;
            lstCart.Items.Clear();
            lblRightTitle.Text = "Hóa đơn thanh toán";
            _selectedCourtName = "";
            _lastDiscountAmount = 0m;
            _lastFinalTotal = 0m;
            UpdateTotals();
        }
    }
}
