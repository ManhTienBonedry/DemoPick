using System;
using System.Windows.Forms;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;
using Sunny.UI;

namespace DemoPick
{
    public partial class UCThanhToan
    {
        private int _lastCompletedInvoiceId = 0;
        private string _lastCompletedCourtName = string.Empty;

        // Thiet lap Initialize Reprint Button va gan cac cau hinh/su kien can dung ban dau.
        private void InitializeReprintButton()
        {
            if (ucInvoiceReprintPanel == null)
            {
                return;
            }

            ucInvoiceReprintPanel.ReprintByIdRequested -= UcInvoiceReprintPanel_ReprintByIdRequested;
            ucInvoiceReprintPanel.ReprintLastRequested -= UcInvoiceReprintPanel_ReprintLastRequested;

            ucInvoiceReprintPanel.ReprintByIdRequested += UcInvoiceReprintPanel_ReprintByIdRequested;
            ucInvoiceReprintPanel.ReprintLastRequested += UcInvoiceReprintPanel_ReprintLastRequested;
        }

        // Xu ly su kien tu control lien quan tren man hinh.
        private void UcInvoiceReprintPanel_ReprintByIdRequested(object sender, EventArgs e)
        {
            BtnReprintById_Click(sender, EventArgs.Empty);
        }

        // Xu ly su kien tu control lien quan tren man hinh.
        private void UcInvoiceReprintPanel_ReprintLastRequested(object sender, EventArgs e)
        {
            BtnReprintLastInvoice_Click(sender, EventArgs.Empty);
        }

        // Cap nhat lai du lieu/trang thai Update Reprint Button State tren man hinh hien tai.
        private void UpdateReprintButtonState()
        {
            if (ucInvoiceReprintPanel == null)
            {
                return;
            }

            bool canReprint = _lastCompletedInvoiceId > 0;
            ucInvoiceReprintPanel.SetLastInvoiceEnabled(canReprint);
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void BtnReprintLastInvoice_Click(object sender, EventArgs e)
        {
            if (_lastCompletedInvoiceId <= 0)
            {
                new UIPage().ShowWarningTip("Chưa có hóa đơn nào để in lại.");
                return;
            }

            ShowInvoicePreview(_lastCompletedInvoiceId, _lastCompletedCourtName);
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void BtnReprintById_Click(object sender, EventArgs e)
        {
            if (ucInvoiceReprintPanel == null)
            {
                return;
            }

            string raw = (ucInvoiceReprintPanel.InvoiceIdText ?? string.Empty).Trim();
            if (!int.TryParse(raw, out int invoiceId) || invoiceId <= 0)
            {
                new UIPage().ShowWarningTip("Vui lòng nhập mã hóa đơn hợp lệ (số dương).");
                ucInvoiceReprintPanel.FocusInvoiceInput();
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

        // Dieu huong hoac hien thi Show Invoice Preview theo trang thai hien tai cua ung dung.
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


