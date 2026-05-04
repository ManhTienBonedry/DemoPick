using System;
using System.Drawing;
using System.Windows.Forms;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;
using Panel = System.Windows.Forms.Panel;

namespace DemoPick
{
    public partial class UCBanHang
    {
        // Nap du lieu cho Load Courts roi cap nhat lai trang thai hien thi tren man hinh.
        private void LoadCourts()
        {
            try
            {
                ClearAndDisposeChildControls(flpCourts);
                var bookCtrl = new DemoPick.Controllers.BookingController();
                var courts = bookCtrl.GetCourts();
                var bookings = bookCtrl.GetBookingsByDate(DateTime.Now);

                foreach (var c in courts)
                {
                    var currentBooking = bookings.Find(b =>
                        b.CourtID == c.CourtID &&
                        !string.Equals(b.Status, AppConstants.BookingStatus.Maintenance, StringComparison.OrdinalIgnoreCase) &&
                        DateTime.Now >= b.StartTime && DateTime.Now <= b.EndTime);
                    bool active = currentBooking != null;
                    string statusTxt = active ? "Đang chơi" : "Trống";
                    string timeTxt = active ? $"{(int)(currentBooking.EndTime - DateTime.Now).TotalMinutes} phút" : "-";
                    Color lineCol = active ? Color.FromArgb(76, 175, 80) : Color.LightGray;

                    Panel pnlCtx = new Panel { Size = new Size(240, 80), BackColor = Color.White, Margin = new Padding(0, 0, 0, 10) };
                    pnlCtx.Paint += (s, e) =>
                    {
                        using (var pen = new Pen(Color.FromArgb(229, 231, 235), 1))
                        using (var brush = new SolidBrush(lineCol))
                        {
                            e.Graphics.DrawRectangle(pen, 0, 0, pnlCtx.Width - 1, pnlCtx.Height - 1);
                            e.Graphics.FillRectangle(brush, 0, 10, 4, pnlCtx.Height - 20);
                        }
                    };

                    Label cName = new Label { Text = c.Name, Font = _posCourtNameFont, ForeColor = Color.FromArgb(26, 35, 50), Location = new Point(15, 15), AutoSize = true };
                    Label badge = new Label { Text = statusTxt, Font = _posCourtBadgeFont, ForeColor = active ? Color.White : Color.Gray, BackColor = active ? Color.FromArgb(76, 175, 80) : Color.FromArgb(243, 244, 246), Location = new Point(150, 17), AutoSize = true, Padding = new Padding(2) };
                    Label cTime = new Label { Text = "🕒 " + timeTxt, Font = _posCourtTimeFont, ForeColor = Color.Gray, Location = new Point(15, 45), AutoSize = true };

                    pnlCtx.Controls.AddRange(new Control[] { cName, badge, cTime });
                    UiTheme.NormalizeTextBackgrounds(pnlCtx);
                    pnlCtx.Cursor = Cursors.Hand;

                    EventHandler selectCourt = (s, e) =>
                    {
                        lblRightTitle.Text = "Sản phẩm chờ - " + c.Name;
                        _selectedCourtName = c.Name;
                        foreach (Control p in flpCourts.Controls)
                        {
                            if (p is Panel panel) panel.BackColor = Color.White;
                        }
                        pnlCtx.BackColor = Color.FromArgb(235, 248, 235);
                        LoadPendingOrderForCourt(c.Name);
                    };

                    pnlCtx.Click += selectCourt;
                    cName.Click += selectCourt;
                    badge.Click += selectCourt;
                    cTime.Click += selectCourt;

                    flpCourts.Controls.Add(pnlCtx);
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("POS Load Courts Error", ex, "UCBanHang.LoadCourts");
            }
        }

        // Nap du lieu cho Load Pending Order For Court roi cap nhat lai trang thai hien thi tren man hinh.
        private void LoadPendingOrderForCourt(string courtName)
        {
            lstCart.Items.Clear();
            var lines = PosService.GetPendingOrder(courtName);
            foreach (var line in lines)
            {
                var lvi = new ListViewItem(new[] { line.ProductName, line.Quantity.ToString(), (line.UnitPrice * line.Quantity).ToString("N0") + "đ" });
                lvi.Tag = new CartItemTag(line.ProductId, line.UnitPrice, line.Category);
                lstCart.Items.Add(lvi);
            }
        }
    }
}


