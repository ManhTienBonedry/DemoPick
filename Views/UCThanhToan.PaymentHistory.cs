using System;
using System.Collections.Generic;
using System.Drawing;
using DemoPick.Services;
using Sunny.UI;

namespace DemoPick
{
    public partial class UCThanhToan
    {
        private List<InvoiceService.InvoiceHistoryItem> _historyItems = new List<InvoiceService.InvoiceHistoryItem>();

        private void InitializePaymentHistoryPanel()
        {
            if (ucPaymentHistoryPanel == null)
            {
                return;
            }

            if (pnlMockInvoice != null)
            {
                pnlMockInvoice.Size = new Size(280, 410);
            }

            if (lblPreviewTotal != null)
            {
                lblPreviewTotal.Location = new Point(15, 360);
            }

            ucPaymentHistoryPanel.Location = new Point(20, 480);
            ucPaymentHistoryPanel.Size = new Size(280, 296);
            pnlRight.AutoScrollMinSize = new Size(0, ucPaymentHistoryPanel.Bottom + 20);

            ucPaymentHistoryPanel.SearchRequested -= UcPaymentHistoryPanel_SearchRequested;
            ucPaymentHistoryPanel.OpenRequested -= UcPaymentHistoryPanel_OpenRequested;

            ucPaymentHistoryPanel.SearchRequested += UcPaymentHistoryPanel_SearchRequested;
            ucPaymentHistoryPanel.OpenRequested += UcPaymentHistoryPanel_OpenRequested;
            ucPaymentHistoryPanel.BringToFront();
        }

        private void UcPaymentHistoryPanel_SearchRequested(object sender, EventArgs e)
        {
            ReloadPaymentHistory();
        }

        private void UcPaymentHistoryPanel_OpenRequested(object sender, EventArgs e)
        {
            OpenSelectedHistoryInvoice();
        }

        private void ReloadPaymentHistory()
        {
            if (ucPaymentHistoryPanel == null)
            {
                return;
            }

            try
            {
                string keyword = ucPaymentHistoryPanel.SearchKeyword;
                int? filterMemberId = _currentCustomerId > 0 ? (int?)_currentCustomerId : null;
                var history = _controller.GetPaymentHistory(120, keyword, filterMemberId);
                _historyItems = history ?? new List<InvoiceService.InvoiceHistoryItem>();
                var shift = _controller.GetShiftSummary();

                var rows = new List<UCPaymentHistoryPanel.HistoryRow>(_historyItems.Count);
                for (int i = 0; i < _historyItems.Count; i++)
                {
                    var h = _historyItems[i];

                    string court = string.IsNullOrWhiteSpace(h.CourtName) ? "-" : h.CourtName;
                    string payment = string.IsNullOrWhiteSpace(h.PaymentMethod) ? "-" : h.PaymentMethod;
                    string booking = h.BookingID > 0 ? "BK#" + h.BookingID : "POS";

                    rows.Add(new UCPaymentHistoryPanel.HistoryRow
                    {
                        InvoiceCode = h.InvoiceID.ToString(),
                        TimeText = h.CreatedAt.ToString("dd/MM HH:mm"),
                        CustomerText = string.IsNullOrWhiteSpace(h.CustomerName) ? "Khách lẻ" : h.CustomerName,
                        TotalText = (h.FinalAmount <= 0 ? 0m : h.FinalAmount).ToString("N0") + "đ",
                        ToolTipText = "Booking: " + booking + " | Sân: " + court + " | PTTT: " + payment,
                        IsHighlighted = h.InvoiceID == _lastCompletedInvoiceId,
                        Tag = h
                    });
                }

                ucPaymentHistoryPanel.BindRows(rows);
                ucPaymentHistoryPanel.SetShiftSummary(BuildShiftSummaryText(shift));
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("Load Payment History Error", ex, "UCThanhToan.ReloadPaymentHistory");
            }
        }

        private static string BuildShiftSummaryText(InvoiceService.ShiftReconciliationSummary shift)
        {
            if (shift == null)
            {
                return string.Empty;
            }

            return string.Format(
                "Ca {0:HH:mm}-{1:HH:mm} | {2} HĐ | {3:N0}đ | TM {4:N0}đ | CK {5:N0}đ | Sân {6} | POS {7}",
                shift.ShiftStart,
                shift.ShiftEnd,
                shift.InvoiceCount,
                shift.TotalAmount,
                shift.CashAmount,
                shift.BankAmount,
                shift.BookingLinkedInvoices,
                shift.PosOnlyInvoices);
        }

        private bool TryGetSelectedHistoryItem(out InvoiceService.InvoiceHistoryItem selected)
        {
            selected = null;
            if (ucPaymentHistoryPanel == null)
            {
                return false;
            }

            return ucPaymentHistoryPanel.TryGetSelectedTag<InvoiceService.InvoiceHistoryItem>(out selected);
        }

        private void OpenSelectedHistoryInvoice()
        {
            if (!TryGetSelectedHistoryItem(out var selected))
            {
                new UIPage().ShowWarningTip("Vui lòng chọn hóa đơn trong lịch sử.");
                return;
            }

            _lastCompletedInvoiceId = selected.InvoiceID;
            _lastCompletedCourtName = selected.CourtName ?? string.Empty;
            if (ucInvoiceReprintPanel != null)
            {
                ucInvoiceReprintPanel.InvoiceIdText = selected.InvoiceID.ToString();
            }

            UpdateReprintButtonState();
            ReloadPaymentHistory();
            ShowInvoicePreview(selected.InvoiceID, _lastCompletedCourtName);
        }
    }
}
