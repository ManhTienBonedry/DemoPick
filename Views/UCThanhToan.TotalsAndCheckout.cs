using System;
using System.Drawing;
using System.Windows.Forms;
using DemoPick.Models;
using DemoPick.Services;
using Sunny.UI;

namespace DemoPick
{
    public partial class UCThanhToan
    {
        private void SetupListView()
        {
            lstCart.Columns.Add("Sản phẩm", 140);
            lstCart.Columns.Add("SL", 40);
            lstCart.Columns.Add("Thành tiền", 110);
        }

        private void UpdateTotals()
        {
            lstCart.Items.Clear();
            _cartTotal = 0;

            decimal fixedDiscountAmtAmount = 0;

            if (_currentBooking != null)
            {
                // Run PriceCalculator
                var services = new System.Collections.Generic.List<DemoPick.Services.ServiceCharge>();
                var pendingLines = PosService.GetPendingOrder(_selectedCourtName);

                foreach (var pl in pendingLines)
                {
                    // By default: quantity-based services. Some add-ons are per-hour.
                    string name = pl.ProductName ?? "";
                    string unit = PriceCalculator.GuessServiceUnit(name);

                    services.Add(new DemoPick.Services.ServiceCharge
                    {
                        ProductID = pl.ProductId,
                        ServiceName = name,
                        Quantity = pl.Quantity,
                        UnitPrice = pl.UnitPrice,
                        Unit = unit
                    });
                }

                decimal courtMultiplier = PriceCalculator.GetCourtRateMultiplier(_selectedCourt?.CourtType, _selectedCourt?.Name);

                var breakdown = DemoPick.Services.PriceCalculator.CalculateTotal(_currentBooking.StartTime, _currentBooking.EndTime, _isFixedCustomer, services, courtMultiplier);

                foreach (var ts in breakdown.TimeSlots)
                {
                    var lviCourt = new ListViewItem(new[] { "Giờ sân " + ts.Description, $"{ts.Hours:0.##}h", ts.Total.ToString("N0") + "đ" });
                    lviCourt.Tag = new CartLine(-1, "Giờ sân " + ts.Description, 1, ts.Total);
                    lviCourt.ForeColor = Color.DarkBlue;
                    lstCart.Items.Add(lviCourt);
                }

                foreach (var svc in breakdown.Services)
                {
                    var lvi = new ListViewItem(new[] { svc.ServiceName, svc.Quantity.ToString(), svc.Total.ToString("N0") + "đ" });
                    lvi.Tag = new CartLine(svc.ProductID, svc.ServiceName, svc.Quantity, svc.UnitPrice);
                    lstCart.Items.Add(lvi);
                }

                _cartTotal = breakdown.SubtotalCourts + breakdown.SubtotalServices;
                fixedDiscountAmtAmount = breakdown.DiscountAmount;
            }
            else
            {
                var pendingLines = PosService.GetPendingOrder(_selectedCourtName);
                foreach (var line in pendingLines)
                {
                    var lvi = new ListViewItem(new[] { line.ProductName, line.Quantity.ToString(), (line.UnitPrice * line.Quantity).ToString("N0") + "đ" });
                    lvi.Tag = line;
                    lstCart.Items.Add(lvi);
                    _cartTotal += (line.UnitPrice * line.Quantity);
                }
            }

            lblSubTotalV.Text = _cartTotal.ToString("N0") + "đ";

            // Normal discount % + fixed amount discount
            decimal discountAmt = (_cartTotal * _currentDiscountPct) + fixedDiscountAmtAmount;
            lblDiscountV.Text = "-" + discountAmt.ToString("N0") + "đ";

            decimal finalTotal = _cartTotal - discountAmt;
            if (finalTotal < 0) finalTotal = 0;

            _lastDiscountAmount = discountAmt;
            _lastFinalTotal = finalTotal;

            lblTotalV.Text = finalTotal.ToString("N0") + "đ";
            lblPreviewTotal.Text = lblTotalV.Text; // update fake bill
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            ResetCheckoutPane();
        }

        private void BtnCheckout_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedCourtName)) return;

            // Prompt user with yes/no dialog equivalent.
            // In a pro UI, we'd use Sunny UI Form.
            var diagRet = MessageBox.Show($"Xác nhận Thu tiền và In Hóa Đơn cho {_selectedCourtName}?", "Thanh toán", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (diagRet == DialogResult.No) return;

            decimal discountAmt = _lastDiscountAmount;
            decimal finalTotal = _lastFinalTotal;

            try
            {
                var lines = new System.Collections.Generic.List<CartLine>();
                foreach (ListViewItem item in lstCart.Items)
                {
                    if (item.Tag is CartLine line)
                    {
                        lines.Add(line);
                    }
                }

                var pos = new PosService();
                int invoiceId = pos.Checkout(
                    _currentCustomerId,
                    lines,
                    _cartTotal,
                    discountAmt,
                    finalTotal,
                    "Cash",
                    _selectedCourtName
                );

                try
                {
                    using (var frm = new FrmInvoicePreview(invoiceId, _selectedCourtName))
                    {
                        frm.ShowDialog(FindForm());
                    }
                }
                catch (Exception ex) { DatabaseHelper.TryLog("Print error", ex, ""); }

                PosService.ClearPendingOrder(_selectedCourtName);
                new UIPage().ShowSuccessTip("Thanh toán hoàn tất!");
                RefreshOnActivated();

            }
            catch (Exception ex)
            {
                new UIPage().ShowErrorTip("Lỗi: " + ex.Message);
            }
        }
    }
}
