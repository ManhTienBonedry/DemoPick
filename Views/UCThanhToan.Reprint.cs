using System;
using System.Drawing;
using System.Windows.Forms;
using DemoPick.Services;
using Sunny.UI;

namespace DemoPick
{
    public partial class UCThanhToan
    {
        private int _lastCompletedInvoiceId = 0;
        private string _lastCompletedCourtName = string.Empty;
        private UIButton _btnReprintLastInvoice;
        private UITextBox _txtReprintInvoiceId;
        private UIButton _btnReprintById;

        private void InitializeReprintButton()
        {
            if (pnlTotals == null || _btnReprintLastInvoice != null)
            {
                return;
            }

            _txtReprintInvoiceId = new UITextBox
            {
                Cursor = Cursors.IBeam,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(300, 18),
                Margin = new Padding(4, 5, 4, 5),
                MinimumSize = new Size(1, 16),
                Name = "txtReprintInvoiceId",
                Padding = new Padding(8, 4, 4, 4),
                RectColor = Color.FromArgb(229, 231, 235),
                ShowText = false,
                Size = new Size(120, 32),
                TabIndex = 8,
                TextAlignment = ContentAlignment.MiddleLeft,
                Watermark = "Mã HĐ"
            };
            _txtReprintInvoiceId.KeyDown += TxtReprintInvoiceId_KeyDown;

            _btnReprintById = new UIButton
            {
                Cursor = Cursors.Hand,
                FillColor = Color.FromArgb(59, 130, 246),
                FillHoverColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(425, 18),
                MinimumSize = new Size(1, 1),
                Name = "btnReprintById",
                Radius = 8,
                RectColor = Color.Transparent,
                RectHoverColor = Color.Transparent,
                Size = new Size(80, 32),
                TabIndex = 9,
                Text = "IN MÃ",
                TipsFont = new Font("Segoe UI", 9F)
            };
            _btnReprintById.Click += BtnReprintById_Click;

            _btnReprintLastInvoice = new UIButton
            {
                Cursor = Cursors.Hand,
                FillColor = Color.FromArgb(229, 231, 235),
                FillHoverColor = Color.FromArgb(209, 213, 219),
                ForeColor = Color.FromArgb(75, 85, 99),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(150, 115),
                MinimumSize = new Size(1, 1),
                Name = "btnReprintLastInvoice",
                Radius = 15,
                RectColor = Color.Transparent,
                RectHoverColor = Color.Transparent,
                Size = new Size(140, 50),
                TabIndex = 10,
                Text = "IN LẠI HĐ",
                TipsFont = new Font("Segoe UI", 9F)
            };

            _btnReprintLastInvoice.Click += BtnReprintLastInvoice_Click;
            pnlTotals.Controls.Add(_txtReprintInvoiceId);
            pnlTotals.Controls.Add(_btnReprintById);
            pnlTotals.Controls.Add(_btnReprintLastInvoice);
            _btnReprintLastInvoice.BringToFront();
        }

        private void UpdateReprintButtonState()
        {
            if (_btnReprintLastInvoice == null)
            {
                return;
            }

            bool canReprint = _lastCompletedInvoiceId > 0;
            _btnReprintLastInvoice.Enabled = canReprint;

            if (canReprint)
            {
                _btnReprintLastInvoice.FillColor = Color.FromArgb(59, 130, 246);
                _btnReprintLastInvoice.FillHoverColor = Color.FromArgb(37, 99, 235);
                _btnReprintLastInvoice.ForeColor = Color.White;
            }
            else
            {
                _btnReprintLastInvoice.FillColor = Color.FromArgb(229, 231, 235);
                _btnReprintLastInvoice.FillHoverColor = Color.FromArgb(209, 213, 219);
                _btnReprintLastInvoice.ForeColor = Color.FromArgb(75, 85, 99);
            }
        }

        private void BtnReprintLastInvoice_Click(object sender, EventArgs e)
        {
            if (_lastCompletedInvoiceId <= 0)
            {
                new UIPage().ShowWarningTip("Chưa có hóa đơn nào để in lại.");
                return;
            }

            ShowInvoicePreview(_lastCompletedInvoiceId, _lastCompletedCourtName);
        }

        private void BtnReprintById_Click(object sender, EventArgs e)
        {
            if (_txtReprintInvoiceId == null)
            {
                return;
            }

            string raw = (_txtReprintInvoiceId.Text ?? string.Empty).Trim();
            if (!int.TryParse(raw, out int invoiceId) || invoiceId <= 0)
            {
                new UIPage().ShowWarningTip("Vui lòng nhập mã hóa đơn hợp lệ (số dương).");
                _txtReprintInvoiceId.Focus();
                _txtReprintInvoiceId.SelectAll();
                return;
            }

            string courtName = invoiceId == _lastCompletedInvoiceId
                ? (_lastCompletedCourtName ?? string.Empty)
                : string.Empty;

            ShowInvoicePreview(invoiceId, courtName);

            _lastCompletedInvoiceId = invoiceId;
            _lastCompletedCourtName = courtName;
            UpdateReprintButtonState();
            ReloadPaymentHistory();
        }

        private void TxtReprintInvoiceId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e == null || e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.Handled = true;
            e.SuppressKeyPress = true;
            BtnReprintById_Click(sender, EventArgs.Empty);
        }

        private void ShowInvoicePreview(int invoiceId, string courtName)
        {
            if (invoiceId <= 0)
            {
                return;
            }

            try
            {
                using (var frm = new FrmInvoicePreview(invoiceId, courtName ?? string.Empty))
                {
                    frm.ShowDialog(FindForm());
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("Print error", ex, "UCThanhToan.ShowInvoicePreview");
                new UIPage().ShowErrorTip("Không thể mở hóa đơn để in lại.");
            }
        }
    }
}
