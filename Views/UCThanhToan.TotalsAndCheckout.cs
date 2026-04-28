using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DemoPick.Models;
using DemoPick.Services;

using Sunny.UI;

namespace DemoPick
{
    public partial class UCThanhToan
    {
        private decimal GetCourtPayableRatio()
        {
            if (_currentBooking == null)
                return 1m;

            string paymentState = (_currentBooking.PaymentState ?? string.Empty).Trim();
            if (string.Equals(paymentState, AppConstants.BookingPaymentState.BankTransferred, StringComparison.OrdinalIgnoreCase))
                return 0m;

            if (string.Equals(paymentState, AppConstants.BookingPaymentState.Deposit50, StringComparison.OrdinalIgnoreCase))
                return 0.5m;

            return 1m;
        }

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

            var result = _controller.CalculateBreakdown(
                _selectedCourtName, 
                _currentBooking, 
                _selectedCourt, 
                _isFixedCustomer, 
                _currentDiscountPct
            );

            foreach (var line in result.DisplayLines)
            {
                var lvi = new ListViewItem(new[] { line.DisplayName, line.DisplayQuantity, line.DisplayTotal });
                lvi.Tag = line.CartLine;
                if (line.IsCourt)
                {
                    lvi.ForeColor = Color.DarkBlue;
                }
                lstCart.Items.Add(lvi);
            }

            _cartTotal = result.SubTotal;
            lblSubTotalV.Text = result.SubTotal.ToString("N0") + "đ";
            lblDiscountV.Text = "-" + result.DiscountAmount.ToString("N0") + "đ";

            _lastDiscountAmount = result.DiscountAmount;
            _lastFinalTotal = result.FinalTotal;

            lblTotalV.Text = result.FinalTotal.ToString("N0") + "đ";
            UpdateMockInvoicePreview(result.SubTotal, result.DiscountAmount, result.FinalTotal);
        }

        private void UpdateMockInvoicePreview(decimal subtotal, decimal discountAmount, decimal finalTotal)
        {
            if (lblPreviewDesc == null || lblPreviewTotal == null || pnlMockInvoice == null)
            {
                return;
            }

            lblPreviewDesc.AutoSize = false;
            lblPreviewDesc.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblPreviewTotal.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;

            lblPreviewDesc.Width = Math.Max(120, pnlMockInvoice.Width - 30);
            int usableHeight = Math.Max(120, lblPreviewTotal.Top - lblPreviewDesc.Top - 10);
            lblPreviewDesc.Height = usableHeight;

            var sb = new StringBuilder();
            sb.AppendLine("   GREEN COURT");
            sb.AppendLine("----------------------------");
            sb.AppendLine("HÓA ĐƠN THANH TOÁN");
            sb.AppendLine(" ");

            string courtName = string.IsNullOrWhiteSpace(_selectedCourtName) ? "-" : _selectedCourtName;
            sb.AppendLine("Sân  : " + TrimPreview(courtName, 18));

            if (_currentBooking != null)
            {
                sb.AppendLine($"Ca   : {_currentBooking.StartTime:HH:mm}-{_currentBooking.EndTime:HH:mm}");
            }

            sb.AppendLine("Khách: " + TrimPreview(GetPreviewCustomerName(), 18));
            sb.AppendLine("----------------------------");

            int maxRows = 7;
            if (lstCart.Items.Count == 0)
            {
                sb.AppendLine("(Chưa có dòng tính tiền)");
            }
            else
            {
                int visibleRows = Math.Min(maxRows, lstCart.Items.Count);
                for (int i = 0; i < visibleRows; i++)
                {
                    ListViewItem item = lstCart.Items[i];
                    string name = TrimPreview(item.SubItems.Count > 0 ? item.SubItems[0].Text : "-", 12);
                    string qty = TrimPreview(item.SubItems.Count > 1 ? item.SubItems[1].Text : "1", 4);
                    string amount = TrimPreview(item.SubItems.Count > 2 ? item.SubItems[2].Text : "0đ", 9);
                    sb.AppendLine($"{name,-12} x{qty,-4} {amount}");
                }

                int hiddenRows = lstCart.Items.Count - visibleRows;
                if (hiddenRows > 0)
                {
                    sb.AppendLine($"+ {hiddenRows} dòng khác...");
                }
            }

            sb.AppendLine("----------------------------");
            sb.AppendLine($"Tạm tính: {subtotal,10:N0}đ");
            sb.AppendLine($"Giảm giá: -{discountAmount,9:N0}đ");

            lblPreviewDesc.Text = sb.ToString();
            lblPreviewTotal.Text = finalTotal.ToString("N0") + "đ";
        }

        private string GetPreviewCustomerName()
        {
            if (_currentCustomerId <= 0)
            {
                return "Khách lẻ";
            }

            string infoText = (lblCustomerInfo == null ? string.Empty : lblCustomerInfo.Text) ?? string.Empty;
            infoText = infoText.Trim();
            if (string.IsNullOrEmpty(infoText))
            {
                return "Khách thành viên";
            }

            if (infoText.StartsWith("✓ "))
            {
                infoText = infoText.Substring(2).Trim();
            }

            int tierStart = infoText.IndexOf(" (", StringComparison.Ordinal);
            if (tierStart > 0)
            {
                infoText = infoText.Substring(0, tierStart).Trim();
            }

            return string.IsNullOrWhiteSpace(infoText) ? "Khách thành viên" : infoText;
        }

        private static string TrimPreview(string input, int maxLen)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "-";
            }

            string value = input.Trim().Replace("\r", " ").Replace("\n", " ");
            if (value.Length <= maxLen)
            {
                return value;
            }

            if (maxLen <= 1)
            {
                return value.Substring(0, 1);
            }

            return value.Substring(0, maxLen - 1) + "~";
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            ResetCheckoutPane();
        }

        private void BtnCheckout_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedCourtName))
            {
                new UIPage().ShowWarningTip("Vui lòng chọn sân cần thanh toán.");
                return;
            }

            if (_currentBooking != null && DateTime.Now < _currentBooking.StartTime)
            {
                MessageBox.Show(
                    "Booking chưa tới giờ chơi. Hãy bấm 'Nhận sân' trước, rồi thanh toán khi ca đã bắt đầu.",
                    "Chưa tới giờ thanh toán",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

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

                var result = _controller.PerformCheckout(
                    _currentCustomerId,
                    lines,
                    _cartTotal,
                    discountAmt,
                    finalTotal,
                    _selectedCourtName,
                    _currentBooking
                );

                if (result.Success)
                {
                    _lastCompletedInvoiceId = result.InvoiceId;
                    _lastCompletedCourtName = _selectedCourtName ?? string.Empty;
                    if (ucInvoiceReprintPanel != null)
                    {
                        ucInvoiceReprintPanel.InvoiceIdText = result.InvoiceId.ToString();
                    }
                    UpdateReprintButtonState();

                    ShowInvoicePreview(result.InvoiceId, _lastCompletedCourtName);

                    new UIPage().ShowSuccessTip($"Thanh toán hoàn tất! HĐ #{result.InvoiceId}");
                    RefreshOnActivated();
                }
                else
                {
                    new UIPage().ShowErrorTip("Lỗi: " + result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                new UIPage().ShowErrorTip("Lỗi: " + ex.Message);
            }
        }
    }
}
