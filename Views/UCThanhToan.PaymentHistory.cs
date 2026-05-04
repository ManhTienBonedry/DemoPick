using System;
using System.Collections.Generic;
using System.Drawing;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;
using Sunny.UI;

namespace DemoPick
{
    public partial class UCThanhToan
    {
        private List<InvoiceService.InvoiceHistoryItem> _historyItems = new List<InvoiceService.InvoiceHistoryItem>();

        // Thiet lap Initialize Payment History Panel va gan cac cau hinh/su kien can dung ban dau.
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

        // Xu ly su kien tu control lien quan tren man hinh.
        private void UcPaymentHistoryPanel_SearchRequested(object sender, EventArgs e)
        {
            ReloadPaymentHistory();
        }

        // Xu ly su kien tu control lien quan tren man hinh.
        private void UcPaymentHistoryPanel_OpenRequested(object sender, EventArgs e)
        {
            OpenSelectedHistoryInvoice();
        }

        // Nap du lieu cho Reload Payment History roi cap nhat lai trang thai hien thi tren man hinh.
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
                        InvoiceCode = "HD" + h.InvoiceID.ToString(),
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

        // Tao hoac tinh ra du lieu Build Shift Summary Text tu cac thong tin dau vao hien co.
        private static string BuildShiftSummaryText(InvoiceService.ShiftReconciliationSummary shift)
        {
            if (shift == null)
            {
                return string.Empty;
            }

            string FormatK(decimal val)
            {
                if (val == 0) return "0";
                if (val >= 1000000) return (val / 1000000m).ToString("0.#") + "M";
                if (val >= 1000) return (val / 1000m).ToString("0.#") + "k";
                return val.ToString("N0");
            }

            return string.Format(
                "Ca {0:HH:mm}-{1:HH:mm} | {2} HĐ | Tổng {3}\nTM {4} | CK {5} | Sân {6} | POS {7}",
                shift.ShiftStart,
                shift.ShiftEnd,
                shift.InvoiceCount,
                FormatK(shift.TotalAmount),
                FormatK(shift.CashAmount),
                FormatK(shift.BankAmount),
                shift.BookingLinkedInvoices,
                shift.PosOnlyInvoices);
        }

        // Thu thuc hien Try Get Selected History Item, neu du lieu khong hop le thi dung va tra thong bao phu hop.
        private bool TryGetSelectedHistoryItem(out InvoiceService.InvoiceHistoryItem selected)
        {
            selected = null;
            if (ucPaymentHistoryPanel == null)
            {
                return false;
            }

            return ucPaymentHistoryPanel.TryGetSelectedTag<InvoiceService.InvoiceHistoryItem>(out selected);
        }

        // Dieu huong hoac hien thi Open Selected History Invoice theo trang thai hien tai cua ung dung.
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


