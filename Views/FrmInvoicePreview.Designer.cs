namespace DemoPick
{
    partial class FrmInvoicePreview
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlTopBar = new Sunny.UI.UIPanel();
            this.btnClose = new Sunny.UI.UIButton();
            this.btnExportExcel = new Sunny.UI.UIButton();
            this.btnExportPdf = new Sunny.UI.UIButton();
            this.reportViewer = new Microsoft.Reporting.WinForms.ReportViewer();
            this.pnlTopBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTopBar
            // 
            this.pnlTopBar.Controls.Add(this.btnClose);
            this.pnlTopBar.Controls.Add(this.btnExportExcel);
            this.pnlTopBar.Controls.Add(this.btnExportPdf);
            this.pnlTopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopBar.FillColor = System.Drawing.Color.White;
            this.pnlTopBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlTopBar.Location = new System.Drawing.Point(0, 0);
            this.pnlTopBar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlTopBar.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlTopBar.Name = "pnlTopBar";
            this.pnlTopBar.Radius = 0;
            this.pnlTopBar.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.pnlTopBar.Size = new System.Drawing.Size(950, 48);
            this.pnlTopBar.TabIndex = 0;
            this.pnlTopBar.Text = null;
            this.pnlTopBar.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.btnClose.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(134)))), ((int)(((byte)(148)))));
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnClose.Location = new System.Drawing.Point(818, 8);
            this.btnClose.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnClose.Name = "btnClose";
            this.btnClose.Radius = 8;
            this.btnClose.RectColor = System.Drawing.Color.Transparent;
            this.btnClose.Size = new System.Drawing.Size(120, 32);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Đóng";
            this.btnClose.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExportExcel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnExportExcel.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.btnExportExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnExportExcel.Location = new System.Drawing.Point(140, 8);
            this.btnExportExcel.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Radius = 8;
            this.btnExportExcel.RectColor = System.Drawing.Color.Transparent;
            this.btnExportExcel.Size = new System.Drawing.Size(120, 32);
            this.btnExportExcel.TabIndex = 1;
            this.btnExportExcel.Text = "Xuất Excel";
            this.btnExportExcel.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // btnExportPdf
            // 
            this.btnExportPdf.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExportPdf.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(219)))), ((int)(((byte)(44)))));
            this.btnExportPdf.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(86)))), ((int)(((byte)(185)))), ((int)(((byte)(90)))));
            this.btnExportPdf.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btnExportPdf.Location = new System.Drawing.Point(12, 8);
            this.btnExportPdf.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnExportPdf.Name = "btnExportPdf";
            this.btnExportPdf.Radius = 8;
            this.btnExportPdf.RectColor = System.Drawing.Color.Transparent;
            this.btnExportPdf.Size = new System.Drawing.Size(120, 32);
            this.btnExportPdf.TabIndex = 0;
            this.btnExportPdf.Text = "Xuất PDF";
            this.btnExportPdf.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // reportViewer
            // 
            this.reportViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportViewer.Location = new System.Drawing.Point(0, 48);
            this.reportViewer.Name = "reportViewer";
            this.reportViewer.ServerReport.BearerToken = null;
            this.reportViewer.Size = new System.Drawing.Size(950, 652);
            this.reportViewer.TabIndex = 1;
            // 
            // FrmInvoicePreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 700);
            this.Controls.Add(this.reportViewer);
            this.Controls.Add(this.pnlTopBar);
            this.Name = "FrmInvoicePreview";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Hóa đơn";
            this.pnlTopBar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer;
        private Sunny.UI.UIPanel pnlTopBar;
        private Sunny.UI.UIButton btnExportPdf;
        private Sunny.UI.UIButton btnExportExcel;
        private Sunny.UI.UIButton btnClose;
    }
}
