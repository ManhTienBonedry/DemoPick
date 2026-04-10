using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DemoPick
{
    partial class UCTongQuan
    {
        private System.ComponentModel.IContainer components = null;

        public Chart chartTrend;
        public Chart chartPie;
        public System.Windows.Forms.ListView lstBookings;

        public System.Windows.Forms.Panel pnlCard1;
        public System.Windows.Forms.Label lblC1T;
        public System.Windows.Forms.Label lblC1V;
        public System.Windows.Forms.Label lblC1S;
        public System.Windows.Forms.PictureBox picIcon1;

        public System.Windows.Forms.Panel pnlCard2;
        public System.Windows.Forms.Label lblC2T;
        public System.Windows.Forms.Label lblC2V;
        public System.Windows.Forms.Label lblC2S;
        public System.Windows.Forms.PictureBox picIcon2;

        public System.Windows.Forms.Panel pnlCard3;
        public System.Windows.Forms.Label lblC3T;
        public System.Windows.Forms.Label lblC3V;
        public System.Windows.Forms.Label lblC3S;
        public System.Windows.Forms.PictureBox picIcon3;

        public System.Windows.Forms.Panel pnlCard4;
        public System.Windows.Forms.Label lblC4T;
        public System.Windows.Forms.Label lblC4V;
        public System.Windows.Forms.Label lblC4S;
        public System.Windows.Forms.PictureBox picIcon4;

        public System.Windows.Forms.Panel pnlChartBorder;
        public System.Windows.Forms.Label lblChartT;
        public System.Windows.Forms.ComboBox cmbFilter;

        public System.Windows.Forms.Panel pnlPieBorder;
        public System.Windows.Forms.Label lblPieT;

        public System.Windows.Forms.Panel pnlTable;
        public System.Windows.Forms.Label lblTableTitle;

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
            this.chartTrend = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartPie = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lstBookings = new System.Windows.Forms.ListView();
            this.pnlCard1 = new System.Windows.Forms.Panel();
            this.lblC1T = new System.Windows.Forms.Label();
            this.lblC1V = new System.Windows.Forms.Label();
            this.lblC1S = new System.Windows.Forms.Label();
            this.pnlCard2 = new System.Windows.Forms.Panel();
            this.lblC2T = new System.Windows.Forms.Label();
            this.lblC2V = new System.Windows.Forms.Label();
            this.lblC2S = new System.Windows.Forms.Label();
            this.pnlCard3 = new System.Windows.Forms.Panel();
            this.lblC3T = new System.Windows.Forms.Label();
            this.lblC3V = new System.Windows.Forms.Label();
            this.lblC3S = new System.Windows.Forms.Label();
            this.pnlCard4 = new System.Windows.Forms.Panel();
            this.lblC4T = new System.Windows.Forms.Label();
            this.lblC4V = new System.Windows.Forms.Label();
            this.lblC4S = new System.Windows.Forms.Label();
            this.pnlChartBorder = new System.Windows.Forms.Panel();
            this.lblChartT = new System.Windows.Forms.Label();
            this.cmbFilter = new System.Windows.Forms.ComboBox();
            this.pnlPieBorder = new System.Windows.Forms.Panel();
            this.lblPieT = new System.Windows.Forms.Label();
            this.pnlTable = new System.Windows.Forms.Panel();
            this.lblTableTitle = new System.Windows.Forms.Label();
            this.picIcon4 = new System.Windows.Forms.PictureBox();
            this.picIcon3 = new System.Windows.Forms.PictureBox();
            this.picIcon2 = new System.Windows.Forms.PictureBox();
            this.picIcon1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.chartTrend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartPie)).BeginInit();
            this.pnlCard1.SuspendLayout();
            this.pnlCard2.SuspendLayout();
            this.pnlCard3.SuspendLayout();
            this.pnlCard4.SuspendLayout();
            this.pnlChartBorder.SuspendLayout();
            this.pnlPieBorder.SuspendLayout();
            this.pnlTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon1)).BeginInit();
            this.SuspendLayout();
            // 
            // chartTrend
            // 
            this.chartTrend.Location = new System.Drawing.Point(20, 60);
            this.chartTrend.Name = "chartTrend";
            this.chartTrend.Size = new System.Drawing.Size(700, 230);
            this.chartTrend.TabIndex = 2;
            // 
            // chartPie
            // 
            this.chartPie.Location = new System.Drawing.Point(20, 60);
            this.chartPie.Name = "chartPie";
            this.chartPie.Size = new System.Drawing.Size(310, 230);
            this.chartPie.TabIndex = 1;
            // 
            // lstBookings
            // 
            this.lstBookings.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstBookings.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lstBookings.FullRowSelect = true;
            this.lstBookings.HideSelection = false;
            this.lstBookings.Location = new System.Drawing.Point(20, 60);
            this.lstBookings.Name = "lstBookings";
            this.lstBookings.Size = new System.Drawing.Size(1075, 200);
            this.lstBookings.TabIndex = 0;
            this.lstBookings.UseCompatibleStateImageBehavior = false;
            this.lstBookings.View = System.Windows.Forms.View.Details;
            // 
            // pnlCard1
            // 
            this.pnlCard1.BackColor = System.Drawing.Color.White;
            this.pnlCard1.Controls.Add(this.lblC1T);
            this.pnlCard1.Controls.Add(this.lblC1V);
            this.pnlCard1.Controls.Add(this.lblC1S);
            this.pnlCard1.Controls.Add(this.picIcon1);
            this.pnlCard1.Location = new System.Drawing.Point(25, 15);
            this.pnlCard1.Name = "pnlCard1";
            this.pnlCard1.Size = new System.Drawing.Size(260, 140);
            this.pnlCard1.TabIndex = 6;
            // 
            // lblC1T
            // 
            this.lblC1T.AutoSize = true;
            this.lblC1T.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblC1T.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblC1T.Location = new System.Drawing.Point(20, 75);
            this.lblC1T.Name = "lblC1T";
            this.lblC1T.Size = new System.Drawing.Size(134, 23);
            this.lblC1T.TabIndex = 0;
            this.lblC1T.Text = "Tổng doanh thu";
            // 
            // lblC1V
            // 
            this.lblC1V.AutoSize = true;
            this.lblC1V.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblC1V.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblC1V.Location = new System.Drawing.Point(15, 95);
            this.lblC1V.Name = "lblC1V";
            this.lblC1V.Size = new System.Drawing.Size(54, 41);
            this.lblC1V.TabIndex = 1;
            this.lblC1V.Text = "0đ";
            // 
            // lblC1S
            // 
            this.lblC1S.AutoSize = true;
            this.lblC1S.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblC1S.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.lblC1S.Location = new System.Drawing.Point(160, 25);
            this.lblC1S.Name = "lblC1S";
            this.lblC1S.Size = new System.Drawing.Size(65, 23);
            this.lblC1S.TabIndex = 2;
            this.lblC1S.Text = "+0% ↗";
            // 
            // pnlCard2
            // 
            this.pnlCard2.BackColor = System.Drawing.Color.White;
            this.pnlCard2.Controls.Add(this.lblC2T);
            this.pnlCard2.Controls.Add(this.lblC2V);
            this.pnlCard2.Controls.Add(this.lblC2S);
            this.pnlCard2.Controls.Add(this.picIcon2);
            this.pnlCard2.Location = new System.Drawing.Point(310, 15);
            this.pnlCard2.Name = "pnlCard2";
            this.pnlCard2.Size = new System.Drawing.Size(260, 140);
            this.pnlCard2.TabIndex = 5;
            // 
            // lblC2T
            // 
            this.lblC2T.AutoSize = true;
            this.lblC2T.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblC2T.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblC2T.Location = new System.Drawing.Point(20, 75);
            this.lblC2T.Name = "lblC2T";
            this.lblC2T.Size = new System.Drawing.Size(119, 23);
            this.lblC2T.TabIndex = 0;
            this.lblC2T.Text = "Công suất sân";
            // 
            // lblC2V
            // 
            this.lblC2V.AutoSize = true;
            this.lblC2V.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblC2V.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblC2V.Location = new System.Drawing.Point(15, 95);
            this.lblC2V.Name = "lblC2V";
            this.lblC2V.Size = new System.Drawing.Size(61, 41);
            this.lblC2V.TabIndex = 1;
            this.lblC2V.Text = "0%";
            // 
            // lblC2S
            // 
            this.lblC2S.AutoSize = true;
            this.lblC2S.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblC2S.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.lblC2S.Location = new System.Drawing.Point(170, 25);
            this.lblC2S.Name = "lblC2S";
            this.lblC2S.Size = new System.Drawing.Size(65, 23);
            this.lblC2S.TabIndex = 2;
            this.lblC2S.Text = "+0% ↗";
            // 
            // pnlCard3
            // 
            this.pnlCard3.BackColor = System.Drawing.Color.White;
            this.pnlCard3.Controls.Add(this.lblC3T);
            this.pnlCard3.Controls.Add(this.lblC3V);
            this.pnlCard3.Controls.Add(this.lblC3S);
            this.pnlCard3.Controls.Add(this.picIcon3);
            this.pnlCard3.Location = new System.Drawing.Point(595, 15);
            this.pnlCard3.Name = "pnlCard3";
            this.pnlCard3.Size = new System.Drawing.Size(260, 140);
            this.pnlCard3.TabIndex = 4;
            // 
            // lblC3T
            // 
            this.lblC3T.AutoSize = true;
            this.lblC3T.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblC3T.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblC3T.Location = new System.Drawing.Point(20, 75);
            this.lblC3T.Name = "lblC3T";
            this.lblC3T.Size = new System.Drawing.Size(91, 23);
            this.lblC3T.TabIndex = 0;
            this.lblC3T.Text = "Khách mới";
            // 
            // lblC3V
            // 
            this.lblC3V.AutoSize = true;
            this.lblC3V.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblC3V.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblC3V.Location = new System.Drawing.Point(15, 95);
            this.lblC3V.Name = "lblC3V";
            this.lblC3V.Size = new System.Drawing.Size(35, 41);
            this.lblC3V.TabIndex = 1;
            this.lblC3V.Text = "0";
            // 
            // lblC3S
            // 
            this.lblC3S.AutoSize = true;
            this.lblC3S.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblC3S.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.lblC3S.Location = new System.Drawing.Point(160, 25);
            this.lblC3S.Name = "lblC3S";
            this.lblC3S.Size = new System.Drawing.Size(65, 23);
            this.lblC3S.TabIndex = 2;
            this.lblC3S.Text = "+0% ↗";
            // 
            // pnlCard4
            // 
            this.pnlCard4.BackColor = System.Drawing.Color.White;
            this.pnlCard4.Controls.Add(this.lblC4T);
            this.pnlCard4.Controls.Add(this.lblC4V);
            this.pnlCard4.Controls.Add(this.lblC4S);
            this.pnlCard4.Controls.Add(this.picIcon4);
            this.pnlCard4.Location = new System.Drawing.Point(880, 15);
            this.pnlCard4.Name = "pnlCard4";
            this.pnlCard4.Size = new System.Drawing.Size(260, 140);
            this.pnlCard4.TabIndex = 3;
            // 
            // lblC4T
            // 
            this.lblC4T.AutoSize = true;
            this.lblC4T.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblC4T.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblC4T.Location = new System.Drawing.Point(20, 75);
            this.lblC4T.Name = "lblC4T";
            this.lblC4T.Size = new System.Drawing.Size(129, 23);
            this.lblC4T.TabIndex = 0;
            this.lblC4T.Text = "Giao dịch (POS)";
            // 
            // lblC4V
            // 
            this.lblC4V.AutoSize = true;
            this.lblC4V.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblC4V.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblC4V.Location = new System.Drawing.Point(15, 95);
            this.lblC4V.Name = "lblC4V";
            this.lblC4V.Size = new System.Drawing.Size(35, 41);
            this.lblC4V.TabIndex = 1;
            this.lblC4V.Text = "0";
            // 
            // lblC4S
            // 
            this.lblC4S.AutoSize = true;
            this.lblC4S.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblC4S.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.lblC4S.Location = new System.Drawing.Point(180, 25);
            this.lblC4S.Name = "lblC4S";
            this.lblC4S.Size = new System.Drawing.Size(65, 23);
            this.lblC4S.TabIndex = 2;
            this.lblC4S.Text = "+0% ↗";
            // 
            // pnlChartBorder
            // 
            this.pnlChartBorder.BackColor = System.Drawing.Color.White;
            this.pnlChartBorder.Controls.Add(this.lblChartT);
            this.pnlChartBorder.Controls.Add(this.cmbFilter);
            this.pnlChartBorder.Controls.Add(this.chartTrend);
            this.pnlChartBorder.Location = new System.Drawing.Point(25, 175);
            this.pnlChartBorder.Name = "pnlChartBorder";
            this.pnlChartBorder.Size = new System.Drawing.Size(740, 310);
            this.pnlChartBorder.TabIndex = 2;
            // 
            // lblChartT
            // 
            this.lblChartT.AutoSize = true;
            this.lblChartT.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblChartT.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblChartT.Location = new System.Drawing.Point(20, 20);
            this.lblChartT.Name = "lblChartT";
            this.lblChartT.Size = new System.Drawing.Size(251, 32);
            this.lblChartT.TabIndex = 0;
            this.lblChartT.Text = "Xu hướng doanh thu";
            // 
            // cmbFilter
            // 
            this.cmbFilter.FormattingEnabled = true;
            this.cmbFilter.Items.AddRange(new object[] {
            "7 ngày qua",
            "Tháng này"});
            this.cmbFilter.Location = new System.Drawing.Point(580, 20);
            this.cmbFilter.Name = "cmbFilter";
            this.cmbFilter.Size = new System.Drawing.Size(130, 24);
            this.cmbFilter.TabIndex = 1;
            // 
            // pnlPieBorder
            // 
            this.pnlPieBorder.BackColor = System.Drawing.Color.White;
            this.pnlPieBorder.Controls.Add(this.lblPieT);
            this.pnlPieBorder.Controls.Add(this.chartPie);
            this.pnlPieBorder.Location = new System.Drawing.Point(790, 175);
            this.pnlPieBorder.Name = "pnlPieBorder";
            this.pnlPieBorder.Size = new System.Drawing.Size(350, 310);
            this.pnlPieBorder.TabIndex = 1;
            // 
            // lblPieT
            // 
            this.lblPieT.AutoSize = true;
            this.lblPieT.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblPieT.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblPieT.Location = new System.Drawing.Point(20, 20);
            this.lblPieT.Name = "lblPieT";
            this.lblPieT.Size = new System.Drawing.Size(238, 32);
            this.lblPieT.TabIndex = 0;
            this.lblPieT.Text = "Doanh thu theo sân";
            // 
            // pnlTable
            // 
            this.pnlTable.BackColor = System.Drawing.Color.White;
            this.pnlTable.Controls.Add(this.lblTableTitle);
            this.pnlTable.Controls.Add(this.lstBookings);
            this.pnlTable.Location = new System.Drawing.Point(25, 510);
            this.pnlTable.Name = "pnlTable";
            this.pnlTable.Size = new System.Drawing.Size(1115, 280);
            this.pnlTable.TabIndex = 0;
            // 
            // lblTableTitle
            // 
            this.lblTableTitle.AutoSize = true;
            this.lblTableTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTableTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblTableTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTableTitle.Name = "lblTableTitle";
            this.lblTableTitle.Size = new System.Drawing.Size(234, 32);
            this.lblTableTitle.TabIndex = 0;
            this.lblTableTitle.Text = "Hoạt động gần đây";
            // 
            // picIcon4
            // 
            this.picIcon4.BackColor = System.Drawing.Color.White;
            this.picIcon4.Image = global::DemoPick.Properties.Resources.giaodich;
            this.picIcon4.Location = new System.Drawing.Point(20, 20);
            this.picIcon4.Name = "picIcon4";
            this.picIcon4.Size = new System.Drawing.Size(36, 36);
            this.picIcon4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picIcon4.TabIndex = 3;
            this.picIcon4.TabStop = false;
            // 
            // picIcon3
            // 
            this.picIcon3.BackColor = System.Drawing.Color.White;
            this.picIcon3.Image = global::DemoPick.Properties.Resources.khach_moi;
            this.picIcon3.Location = new System.Drawing.Point(20, 20);
            this.picIcon3.Name = "picIcon3";
            this.picIcon3.Size = new System.Drawing.Size(36, 36);
            this.picIcon3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picIcon3.TabIndex = 3;
            this.picIcon3.TabStop = false;
            // 
            // picIcon2
            // 
            this.picIcon2.BackColor = System.Drawing.Color.White;
            this.picIcon2.Image = global::DemoPick.Properties.Resources.Kho_2;
            this.picIcon2.Location = new System.Drawing.Point(20, 20);
            this.picIcon2.Name = "picIcon2";
            this.picIcon2.Size = new System.Drawing.Size(36, 36);
            this.picIcon2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picIcon2.TabIndex = 3;
            this.picIcon2.TabStop = false;
            // 
            // picIcon1
            // 
            this.picIcon1.BackColor = System.Drawing.Color.White;
            this.picIcon1.Image = global::DemoPick.Properties.Resources.tổng_doanh_thu;
            this.picIcon1.Location = new System.Drawing.Point(20, 20);
            this.picIcon1.Name = "picIcon1";
            this.picIcon1.Size = new System.Drawing.Size(36, 36);
            this.picIcon1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picIcon1.TabIndex = 3;
            this.picIcon1.TabStop = false;
            // 
            // UCTongQuan
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.pnlTable);
            this.Controls.Add(this.pnlPieBorder);
            this.Controls.Add(this.pnlChartBorder);
            this.Controls.Add(this.pnlCard4);
            this.Controls.Add(this.pnlCard3);
            this.Controls.Add(this.pnlCard2);
            this.Controls.Add(this.pnlCard1);
            this.Name = "UCTongQuan";
            this.Size = new System.Drawing.Size(1160, 820);
            ((System.ComponentModel.ISupportInitialize)(this.chartTrend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartPie)).EndInit();
            this.pnlCard1.ResumeLayout(false);
            this.pnlCard1.PerformLayout();
            this.pnlCard2.ResumeLayout(false);
            this.pnlCard2.PerformLayout();
            this.pnlCard3.ResumeLayout(false);
            this.pnlCard3.PerformLayout();
            this.pnlCard4.ResumeLayout(false);
            this.pnlCard4.PerformLayout();
            this.pnlChartBorder.ResumeLayout(false);
            this.pnlChartBorder.PerformLayout();
            this.pnlPieBorder.ResumeLayout(false);
            this.pnlPieBorder.PerformLayout();
            this.pnlTable.ResumeLayout(false);
            this.pnlTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon1)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
