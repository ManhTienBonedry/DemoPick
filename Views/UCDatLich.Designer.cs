using System.Windows.Forms;
using Sunny.UI;

namespace DemoPick
{
    partial class UCDatLich
    {
        private System.ComponentModel.IContainer components = null;

        public System.Windows.Forms.Panel pnlControlBar;
        public System.Windows.Forms.Panel pnlTimelineContainer;
        public System.Windows.Forms.Panel pnlCanvas;
        public System.Windows.Forms.Panel pnlLegend;

        public System.Windows.Forms.Label lblDate;

        public UIButton btnDatNhanh;
        public UIButton btnDatCoDinh;
        public UIButton btnDoiCa;

        // Custom legend labels
        public System.Windows.Forms.Label l1C;
        public System.Windows.Forms.Label l1T;
        public System.Windows.Forms.Label l2C;
        public System.Windows.Forms.Label l2T;
        public System.Windows.Forms.Label l3C;
        public System.Windows.Forms.Label l3T;
        public System.Windows.Forms.Label l4C;
        public System.Windows.Forms.Label l4T;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlControlBar = new System.Windows.Forms.Panel();
            this.lblDate = new System.Windows.Forms.Label();
            this.dateFilter = new DemoPick.UCDateRangeFilter();
            this.btnDatNhanh = new Sunny.UI.UIButton();
            this.btnDatCoDinh = new Sunny.UI.UIButton();
            this.btnDoiCa = new Sunny.UI.UIButton();
            this.pnlTimelineContainer = new System.Windows.Forms.Panel();
            this.pnlCanvas = new System.Windows.Forms.Panel();
            this.pnlLegend = new System.Windows.Forms.Panel();
            this.l1C = new System.Windows.Forms.Label();
            this.l1T = new System.Windows.Forms.Label();
            this.l2C = new System.Windows.Forms.Label();
            this.l2T = new System.Windows.Forms.Label();
            this.l3C = new System.Windows.Forms.Label();
            this.l3T = new System.Windows.Forms.Label();
            this.l4C = new System.Windows.Forms.Label();
            this.l4T = new System.Windows.Forms.Label();
            this.pnlControlBar.SuspendLayout();
            this.pnlTimelineContainer.SuspendLayout();
            this.pnlLegend.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlControlBar
            // 
            this.pnlControlBar.BackColor = System.Drawing.Color.Transparent;
            this.pnlControlBar.Controls.Add(this.dateFilter);
            this.pnlControlBar.Controls.Add(this.lblDate);
            this.pnlControlBar.Controls.Add(this.btnDatNhanh);
            this.pnlControlBar.Controls.Add(this.btnDatCoDinh);
            this.pnlControlBar.Controls.Add(this.btnDoiCa);
            this.pnlControlBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlControlBar.Location = new System.Drawing.Point(0, 0);
            this.pnlControlBar.Name = "pnlControlBar";
            this.pnlControlBar.Size = new System.Drawing.Size(1324, 80);
            this.pnlControlBar.TabIndex = 1;
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblDate.Location = new System.Drawing.Point(25, 28);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(199, 37);
            this.lblDate.TabIndex = 0;
            this.lblDate.Text = "Đang tải lịch...";
            // 
            // dateFilter
            // 
            this.dateFilter.BackColor = System.Drawing.Color.Transparent;
            this.dateFilter.Location = new System.Drawing.Point(302, 28);
            this.dateFilter.Mode = DemoPick.UCDateRangeFilter.DateFilterMode.SingleDate;
            this.dateFilter.Name = "dateFilter";
            this.dateFilter.Size = new System.Drawing.Size(260, 39);
            this.dateFilter.TabIndex = 1;
            // 
            // btnDatNhanh
            // 
            this.btnDatNhanh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDatNhanh.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnDatNhanh.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(214)))), ((int)(((byte)(123)))));
            this.btnDatNhanh.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(194)))), ((int)(((byte)(103)))));
            this.btnDatNhanh.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDatNhanh.Location = new System.Drawing.Point(1000, 22);
            this.btnDatNhanh.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnDatNhanh.Name = "btnDatNhanh";
            this.btnDatNhanh.Radius = 18;
            this.btnDatNhanh.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnDatNhanh.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(214)))), ((int)(((byte)(123)))));
            this.btnDatNhanh.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(194)))), ((int)(((byte)(103)))));
            this.btnDatNhanh.Size = new System.Drawing.Size(140, 40);
            this.btnDatNhanh.Style = Sunny.UI.UIStyle.Custom;
            this.btnDatNhanh.TabIndex = 4;
            this.btnDatNhanh.Text = "+ Đặt sân nhanh";
            this.btnDatNhanh.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // btnDatCoDinh
            // 
            this.btnDatCoDinh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDatCoDinh.FillColor = System.Drawing.Color.White;
            this.btnDatCoDinh.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnDatCoDinh.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.btnDatCoDinh.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDatCoDinh.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.btnDatCoDinh.ForeHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.btnDatCoDinh.ForePressColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.btnDatCoDinh.Location = new System.Drawing.Point(680, 22);
            this.btnDatCoDinh.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnDatCoDinh.Name = "btnDatCoDinh";
            this.btnDatCoDinh.Radius = 18;
            this.btnDatCoDinh.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.btnDatCoDinh.RectHoverColor = System.Drawing.Color.Gray;
            this.btnDatCoDinh.RectPressColor = System.Drawing.Color.DimGray;
            this.btnDatCoDinh.Size = new System.Drawing.Size(150, 40);
            this.btnDatCoDinh.Style = Sunny.UI.UIStyle.Custom;
            this.btnDatCoDinh.TabIndex = 5;
            this.btnDatCoDinh.Text = "📅 Đặt sân cố định";
            this.btnDatCoDinh.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // btnDoiCa
            // 
            this.btnDoiCa.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDoiCa.FillColor = System.Drawing.Color.White;
            this.btnDoiCa.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnDoiCa.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.btnDoiCa.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDoiCa.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.btnDoiCa.ForeHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.btnDoiCa.ForePressColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.btnDoiCa.Location = new System.Drawing.Point(840, 22);
            this.btnDoiCa.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnDoiCa.Name = "btnDoiCa";
            this.btnDoiCa.Radius = 18;
            this.btnDoiCa.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.btnDoiCa.RectHoverColor = System.Drawing.Color.Gray;
            this.btnDoiCa.RectPressColor = System.Drawing.Color.DimGray;
            this.btnDoiCa.Size = new System.Drawing.Size(150, 40);
            this.btnDoiCa.Style = Sunny.UI.UIStyle.Custom;
            this.btnDoiCa.TabIndex = 6;
            this.btnDoiCa.Text = "✏ Đổi ca";
            this.btnDoiCa.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // pnlTimelineContainer
            // 
            this.pnlTimelineContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTimelineContainer.AutoScroll = true;
            this.pnlTimelineContainer.BackColor = System.Drawing.Color.White;
            this.pnlTimelineContainer.Controls.Add(this.pnlCanvas);
            this.pnlTimelineContainer.Location = new System.Drawing.Point(25, 80);
            this.pnlTimelineContainer.Name = "pnlTimelineContainer";
            this.pnlTimelineContainer.Size = new System.Drawing.Size(1274, 563);
            this.pnlTimelineContainer.TabIndex = 0;
            // 
            // pnlCanvas
            // 
            this.pnlCanvas.BackColor = System.Drawing.Color.White;
            this.pnlCanvas.Location = new System.Drawing.Point(0, 0);
            this.pnlCanvas.Name = "pnlCanvas";
            this.pnlCanvas.Size = new System.Drawing.Size(1274, 563);
            this.pnlCanvas.TabIndex = 0;
            // 
            // pnlLegend
            // 
            this.pnlLegend.BackColor = System.Drawing.Color.Transparent;
            this.pnlLegend.Controls.Add(this.l1C);
            this.pnlLegend.Controls.Add(this.l1T);
            this.pnlLegend.Controls.Add(this.l2C);
            this.pnlLegend.Controls.Add(this.l2T);
            this.pnlLegend.Controls.Add(this.l3C);
            this.pnlLegend.Controls.Add(this.l3T);
            this.pnlLegend.Controls.Add(this.l4C);
            this.pnlLegend.Controls.Add(this.l4T);
            this.pnlLegend.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlLegend.Location = new System.Drawing.Point(0, 653);
            this.pnlLegend.Name = "pnlLegend";
            this.pnlLegend.Size = new System.Drawing.Size(1324, 50);
            this.pnlLegend.TabIndex = 2;
            // 
            // l1C
            // 
            this.l1C.AutoSize = true;
            this.l1C.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.l1C.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.l1C.Location = new System.Drawing.Point(25, 10);
            this.l1C.Name = "l1C";
            this.l1C.Size = new System.Drawing.Size(33, 37);
            this.l1C.TabIndex = 0;
            this.l1C.Text = "●";
            // 
            // l1T
            // 
            this.l1T.AutoSize = true;
            this.l1T.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.l1T.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.l1T.Location = new System.Drawing.Point(55, 15);
            this.l1T.Name = "l1T";
            this.l1T.Size = new System.Drawing.Size(54, 23);
            this.l1T.TabIndex = 1;
            this.l1T.Text = "Trống";
            // 
            // l2C
            // 
            this.l2C.AutoSize = true;
            this.l2C.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.l2C.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.l2C.Location = new System.Drawing.Point(125, 10);
            this.l2C.Name = "l2C";
            this.l2C.Size = new System.Drawing.Size(33, 37);
            this.l2C.TabIndex = 2;
            this.l2C.Text = "●";
            // 
            // l2T
            // 
            this.l2T.AutoSize = true;
            this.l2T.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.l2T.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.l2T.Location = new System.Drawing.Point(155, 15);
            this.l2T.Name = "l2T";
            this.l2T.Size = new System.Drawing.Size(88, 23);
            this.l2T.TabIndex = 3;
            this.l2T.Text = "Đang chơi";
            // 
            // l3C
            // 
            this.l3C.AutoSize = true;
            this.l3C.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.l3C.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(158)))), ((int)(((byte)(11)))));
            this.l3C.Location = new System.Drawing.Point(245, 10);
            this.l3C.Name = "l3C";
            this.l3C.Size = new System.Drawing.Size(33, 37);
            this.l3C.TabIndex = 4;
            this.l3C.Text = "●";
            // 
            // l3T
            // 
            this.l3T.AutoSize = true;
            this.l3T.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.l3T.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.l3T.Location = new System.Drawing.Point(275, 15);
            this.l3T.Name = "l3T";
            this.l3T.Size = new System.Drawing.Size(106, 23);
            this.l3T.TabIndex = 5;
            this.l3T.Text = "Đã đặt trước";
            // 
            // l4C
            // 
            this.l4C.AutoSize = true;
            this.l4C.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.l4C.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.l4C.Location = new System.Drawing.Point(385, 10);
            this.l4C.Name = "l4C";
            this.l4C.Size = new System.Drawing.Size(33, 37);
            this.l4C.TabIndex = 6;
            this.l4C.Text = "●";
            // 
            // l4T
            // 
            this.l4T.AutoSize = true;
            this.l4T.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.l4T.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.l4T.Location = new System.Drawing.Point(415, 15);
            this.l4T.Name = "l4T";
            this.l4T.Size = new System.Drawing.Size(121, 23);
            this.l4T.TabIndex = 7;
            this.l4T.Text = "Đã thanh toán";
            // 
            // UCDatLich
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.pnlTimelineContainer);
            this.Controls.Add(this.pnlControlBar);
            this.Controls.Add(this.pnlLegend);
            this.Name = "UCDatLich";
            this.Size = new System.Drawing.Size(1324, 703);
            this.pnlControlBar.ResumeLayout(false);
            this.pnlControlBar.PerformLayout();
            this.pnlTimelineContainer.ResumeLayout(false);
            this.pnlLegend.ResumeLayout(false);
            this.pnlLegend.PerformLayout();
            this.ResumeLayout(false);

        }

        private UCDateRangeFilter dateFilter;
    }
}
