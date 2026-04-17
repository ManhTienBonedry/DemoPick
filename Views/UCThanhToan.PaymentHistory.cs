using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DemoPick.Services;
using Sunny.UI;

namespace DemoPick
{
    public partial class UCThanhToan
    {
        private Label _lblHistoryTitle;
        private UITextBox _txtHistorySearch;
        private UIButton _btnHistoryRefresh;
        private ListView _lstInvoiceHistory;
        private UIButton _btnHistoryOpen;
        private List<InvoiceService.InvoiceHistoryItem> _historyItems = new List<InvoiceService.InvoiceHistoryItem>();

        private void InitializePaymentHistoryPanel()
        {
            if (pnlRight == null || _lstInvoiceHistory != null)
            {
                return;
            }

            if (pnlMockInvoice != null)
            {
                pnlMockInvoice.Size = new Size(280, 420);
            }

            if (lblPreviewTotal != null)
            {
                lblPreviewTotal.Location = new Point(15, 370);
            }

            _lblHistoryTitle = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(75, 85, 99),
                Location = new Point(20, 500),
                Name = "lblHistoryTitle",
                Text = "Lịch sử thanh toán"
            };

            _txtHistorySearch = new UITextBox
            {
                Cursor = Cursors.IBeam,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(20, 530),
                Margin = new Padding(4, 5, 4, 5),
                MinimumSize = new Size(1, 16),
                Name = "txtHistorySearch",
                Padding = new Padding(8, 4, 4, 4),
                RectColor = Color.FromArgb(229, 231, 235),
                ShowText = false,
                Size = new Size(188, 32),
                TabIndex = 12,
                TextAlignment = ContentAlignment.MiddleLeft,
                Watermark = "Tìm mã/SĐT/khách"
            };
            _txtHistorySearch.KeyDown += TxtHistorySearch_KeyDown;

            _btnHistoryRefresh = new UIButton
            {
                Cursor = Cursors.Hand,
                FillColor = Color.FromArgb(59, 130, 246),
                FillHoverColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(214, 530),
                MinimumSize = new Size(1, 1),
                Name = "btnHistoryRefresh",
                Radius = 8,
                RectColor = Color.Transparent,
                RectHoverColor = Color.Transparent,
                Size = new Size(86, 32),
                TabIndex = 13,
                Text = "Lọc",
                TipsFont = new Font("Segoe UI", 9F)
            };
            _btnHistoryRefresh.Click += BtnHistoryRefresh_Click;

            _lstInvoiceHistory = new ListView
            {
                FullRowSelect = true,
                GridLines = false,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                HideSelection = false,
                Location = new Point(20, 568),
                MultiSelect = false,
                Name = "lstInvoiceHistory",
                ShowItemToolTips = true,
                Size = new Size(280, 190),
                TabIndex = 14,
                UseCompatibleStateImageBehavior = false,
                View = View.Details
            };
            _lstInvoiceHistory.Columns.Add("Mã", 48);
            _lstInvoiceHistory.Columns.Add("Giờ", 84);
            _lstInvoiceHistory.Columns.Add("Khách", 82);
            _lstInvoiceHistory.Columns.Add("Tổng", 64);
            _lstInvoiceHistory.DoubleClick += LstInvoiceHistory_DoubleClick;

            _btnHistoryOpen = new UIButton
            {
                Cursor = Cursors.Hand,
                FillColor = Color.FromArgb(16, 185, 129),
                FillHoverColor = Color.FromArgb(5, 150, 105),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(20, 764),
                MinimumSize = new Size(1, 1),
                Name = "btnHistoryOpen",
                Radius = 10,
                RectColor = Color.Transparent,
                RectHoverColor = Color.Transparent,
                Size = new Size(280, 36),
                TabIndex = 15,
                Text = "Mở / In hóa đơn đã chọn",
                TipsFont = new Font("Segoe UI", 9F)
            };
            _btnHistoryOpen.Click += BtnHistoryOpen_Click;

            pnlRight.Controls.Add(_lblHistoryTitle);
            pnlRight.Controls.Add(_txtHistorySearch);
            pnlRight.Controls.Add(_btnHistoryRefresh);
            pnlRight.Controls.Add(_lstInvoiceHistory);
            pnlRight.Controls.Add(_btnHistoryOpen);
            _btnHistoryOpen.BringToFront();
        }

        private void ReloadPaymentHistory()
        {
            if (_lstInvoiceHistory == null)
            {
                return;
            }

            try
            {
                string keyword = _txtHistorySearch == null ? string.Empty : _txtHistorySearch.Text;
                _historyItems = InvoiceService.GetInvoiceHistory(120, keyword) ?? new List<InvoiceService.InvoiceHistoryItem>();

                _lstInvoiceHistory.BeginUpdate();
                _lstInvoiceHistory.Items.Clear();

                for (int i = 0; i < _historyItems.Count; i++)
                {
                    var h = _historyItems[i];
                    var lvi = new ListViewItem(h.InvoiceID.ToString());
                    lvi.SubItems.Add(h.CreatedAt.ToString("dd/MM HH:mm"));
                    lvi.SubItems.Add(string.IsNullOrWhiteSpace(h.CustomerName) ? "Khách lẻ" : h.CustomerName);
                    lvi.SubItems.Add((h.FinalAmount <= 0 ? 0m : h.FinalAmount).ToString("N0") + "đ");

                    string court = string.IsNullOrWhiteSpace(h.CourtName) ? "-" : h.CourtName;
                    string payment = string.IsNullOrWhiteSpace(h.PaymentMethod) ? "-" : h.PaymentMethod;
                    lvi.ToolTipText = $"Sân: {court} | PTTT: {payment}";
                    lvi.Tag = h;

                    if (h.InvoiceID == _lastCompletedInvoiceId)
                    {
                        lvi.BackColor = Color.FromArgb(239, 246, 255);
                    }

                    _lstInvoiceHistory.Items.Add(lvi);
                }

                _lstInvoiceHistory.EndUpdate();
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("Load Payment History Error", ex, "UCThanhToan.ReloadPaymentHistory");
            }
        }

        private bool TryGetSelectedHistoryItem(out InvoiceService.InvoiceHistoryItem selected)
        {
            selected = null;
            if (_lstInvoiceHistory == null || _lstInvoiceHistory.SelectedItems.Count <= 0)
            {
                return false;
            }

            selected = _lstInvoiceHistory.SelectedItems[0].Tag as InvoiceService.InvoiceHistoryItem;
            return selected != null;
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
            if (_txtReprintInvoiceId != null)
            {
                _txtReprintInvoiceId.Text = selected.InvoiceID.ToString();
            }

            UpdateReprintButtonState();
            ReloadPaymentHistory();
            ShowInvoicePreview(selected.InvoiceID, _lastCompletedCourtName);
        }

        private void BtnHistoryRefresh_Click(object sender, EventArgs e)
        {
            ReloadPaymentHistory();
        }

        private void TxtHistorySearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e == null || e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.Handled = true;
            e.SuppressKeyPress = true;
            ReloadPaymentHistory();
        }

        private void LstInvoiceHistory_DoubleClick(object sender, EventArgs e)
        {
            OpenSelectedHistoryInvoice();
        }

        private void BtnHistoryOpen_Click(object sender, EventArgs e)
        {
            OpenSelectedHistoryInvoice();
        }
    }
}
