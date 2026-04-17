using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using DemoPick.Services;
using Microsoft.Reporting.WinForms;
using Sunny.UI;

namespace DemoPick
{
    public partial class FrmInvoicePreview : Form
    {
        private readonly int _invoiceId;
        private readonly string _courtName;

        private static string TryGetExternalImageUriFromResources(string fileStem)
        {
            try
            {
                string resourcesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
                if (!Directory.Exists(resourcesDir))
                    return "";

                string[] preferredExts = { ".png", ".jpg", ".jpeg" };
                for (int i = 0; i < preferredExts.Length; i++)
                {
                    string preferredPath = Path.Combine(resourcesDir, fileStem + preferredExts[i]);
                    if (File.Exists(preferredPath))
                        return new Uri(preferredPath).AbsoluteUri;
                }

                string[] anyMatches = Directory.GetFiles(resourcesDir, fileStem + ".*");
                for (int i = 0; i < anyMatches.Length; i++)
                {
                    if (File.Exists(anyMatches[i]))
                        return new Uri(anyMatches[i]).AbsoluteUri;
                }

                return "";
            }
            catch
            {
                return "";
            }
        }

        public FrmInvoicePreview(int invoiceId, string courtName)
        {
            _invoiceId = invoiceId;
            _courtName = courtName ?? "";

            InitializeComponent();

            btnExportPdf.Click += (s, e) => Export("PDF", "pdf", "PDF (*.pdf)|*.pdf");
            btnExportExcel.Click += (s, e) => Export("EXCELOPENXML", "xlsx", "Excel (*.xlsx)|*.xlsx");
            btnClose.Click += (s, e) => Close();

            Load += (s, e) => LoadReport();
        }

        private void LoadReport()
        {
            try
            {
                var header = InvoiceService.GetInvoiceHeader(_invoiceId, _courtName);
                var lines = InvoiceService.GetInvoiceLines(_invoiceId);

                var headerDt = new DataTable("InvoiceHeader");
                headerDt.Columns.Add("InvoiceID", typeof(int));
                headerDt.Columns.Add("CreatedAt", typeof(DateTime));
                headerDt.Columns.Add("MemberName", typeof(string));
                headerDt.Columns.Add("PaymentMethod", typeof(string));
                headerDt.Columns.Add("TotalAmount", typeof(decimal));
                headerDt.Columns.Add("DiscountAmount", typeof(decimal));
                headerDt.Columns.Add("FinalAmount", typeof(decimal));
                headerDt.Columns.Add("BookingStartTime", typeof(DateTime));
                headerDt.Columns.Add("BookingEndTime", typeof(DateTime));

                headerDt.Rows.Add(
                    header.InvoiceID,
                    header.CreatedAt,
                    header.MemberName ?? "",
                    header.PaymentMethod ?? "",
                    header.TotalAmount,
                    header.DiscountAmount,
                    header.FinalAmount,
                    (object)header.BookingStartTime ?? DBNull.Value,
                    (object)header.BookingEndTime ?? DBNull.Value
                );

                var linesDt = new DataTable("InvoiceLines");
                linesDt.Columns.Add("ItemName", typeof(string));
                linesDt.Columns.Add("Quantity", typeof(int));
                linesDt.Columns.Add("UnitPrice", typeof(decimal));
                linesDt.Columns.Add("LineTotal", typeof(decimal));

                foreach (var l in lines)
                {
                    linesDt.Rows.Add(l.ItemName ?? "", l.Quantity, l.UnitPrice, l.LineTotal);
                }

                reportViewer.Reset();
                reportViewer.LocalReport.EnableExternalImages = true;
                reportViewer.LocalReport.ReportEmbeddedResource = "DemoPick.Reports.Bill.rdlc";
                reportViewer.LocalReport.DataSources.Clear();
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("InvoiceHeader", headerDt));
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("InvoiceLines", linesDt));

                string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "pick.png");
                string logoUri = File.Exists(logoPath) ? new Uri(logoPath).AbsoluteUri : "";

                string qrUri = TryGetExternalImageUriFromResources("qr");
                if (string.IsNullOrWhiteSpace(qrUri))
                {
                    // Fallback to logo when a dedicated QR image is not provisioned.
                    qrUri = logoUri;
                }

                reportViewer.LocalReport.SetParameters(new[]
                {
                    new ReportParameter("LogoPath", logoUri ?? string.Empty),
                    new ReportParameter("QrPath", qrUri ?? string.Empty),
                    new ReportParameter("CourtName", _courtName ?? string.Empty)
                });

                reportViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải hóa đơn để in/xem trước.\n\n" + ex.Message,
                    "Lỗi hóa đơn",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                Close();
            }
        }

        private void Export(string renderFormat, string fileExt, string filter)
        {
            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = filter;
                    sfd.FileName = $"Invoice_{_invoiceId}.{fileExt}";

                    if (sfd.ShowDialog(this) != DialogResult.OK)
                        return;

                    Warning[] warnings;
                    string[] streamIds;
                    string mimeType;
                    string encoding;
                    string extension;

                    byte[] bytes = reportViewer.LocalReport.Render(
                        renderFormat,
                        null,
                        out mimeType,
                        out encoding,
                        out extension,
                        out streamIds,
                        out warnings
                    );

                    File.WriteAllBytes(sfd.FileName, bytes);

                    new UIPage().ShowSuccessTip($"Đã xuất file: {sfd.FileName}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Xuất file thất bại: " + ex.Message,
                    "Lỗi xuất hóa đơn",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
