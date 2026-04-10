using System.Windows.Forms;
using Sunny.UI;
using System.Windows.Forms.DataVisualization.Charting;

namespace DemoPick
{
    partial class UCBaoCao
    {
        private System.ComponentModel.IContainer components = null;

        // Date Range Filter
        public UCDateRangeFilter dateFilter;

        // Top Cards
        public UIPanel pnlCard1;
        public Label lblC1Title;
        public Label lblC1Value;
        public Label lblC1Badge;

        public UIPanel pnlCard2;
        public Label lblC2Title;
        public Label lblC2Value;
        public Label lblC2Badge;

        public UIPanel pnlCard3;
        public Label lblC3Title;
        public Label lblC3Value;
        public Label lblC3Badge;

        // Charts
        public UIPanel pnlTrend;
        public Label lblTrendTitle;
        public Chart chartTrend;

        public UIPanel pnlPie;
        public Label lblPieTitle;
        public Chart chartPie;

        // Bottom List
        public UIPanel pnlTopCourts;
        public Label lblTopCourtsTitle;
        public Label lblTopCourtsViewAll;
        public ListView lstTopCourts;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.pnlCard1 = new Sunny.UI.UIPanel();
            this.lblC1Title = new System.Windows.Forms.Label();
            this.lblC1Badge = new System.Windows.Forms.Label();
            this.lblC1Value = new System.Windows.Forms.Label();
            this.dateFilter = new DemoPick.UCDateRangeFilter();
            this.pnlCard2 = new Sunny.UI.UIPanel();
            this.lblC2Title = new System.Windows.Forms.Label();
            this.lblC2Badge = new System.Windows.Forms.Label();
            this.lblC2Value = new System.Windows.Forms.Label();
            this.pnlCard3 = new Sunny.UI.UIPanel();
            this.lblC3Title = new System.Windows.Forms.Label();
            this.lblC3Badge = new System.Windows.Forms.Label();
            this.lblC3Value = new System.Windows.Forms.Label();
            this.pnlTrend = new Sunny.UI.UIPanel();
            this.lblTrendTitle = new System.Windows.Forms.Label();
            this.chartTrend = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.pnlPie = new Sunny.UI.UIPanel();
            this.lblPieTitle = new System.Windows.Forms.Label();
            this.chartPie = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.pnlTopCourts = new Sunny.UI.UIPanel();
            this.lblTopCourtsTitle = new System.Windows.Forms.Label();
            this.lblTopCourtsViewAll = new System.Windows.Forms.Label();
            this.lstTopCourts = new System.Windows.Forms.ListView();
            this.pnlCard1.SuspendLayout();
            this.pnlCard2.SuspendLayout();
            this.pnlCard3.SuspendLayout();
            this.pnlTrend.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartTrend)).BeginInit();
            this.pnlPie.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartPie)).BeginInit();
            this.pnlTopCourts.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCard1
            // 
            this.pnlCard1.Controls.Add(this.lblC1Title);
            this.pnlCard1.Controls.Add(this.lblC1Badge);
            this.pnlCard1.Controls.Add(this.lblC1Value);
            this.pnlCard1.FillColor = System.Drawing.Color.White;
            this.pnlCard1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlCard1.Location = new System.Drawing.Point(20, 60);
            this.pnlCard1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlCard1.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlCard1.Name = "pnlCard1";
            this.pnlCard1.Radius = 16;
            this.pnlCard1.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.pnlCard1.Size = new System.Drawing.Size(360, 110);
            this.pnlCard1.TabIndex = 0;
            this.pnlCard1.Text = null;
            this.pnlCard1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblC1Title
            // 
            this.lblC1Title.AutoSize = true;
            this.lblC1Title.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblC1Title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblC1Title.Location = new System.Drawing.Point(20, 20);
            this.lblC1Title.Name = "lblC1Title";
            this.lblC1Title.Size = new System.Drawing.Size(167, 23);
            this.lblC1Title.TabIndex = 0;
            this.lblC1Title.Text = "TỔNG DOANH THU";
            // 
            // lblC1Badge
            // 
            this.lblC1Badge.AutoSize = true;
            this.lblC1Badge.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblC1Badge.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblC1Badge.Location = new System.Drawing.Point(280, 20);
            this.lblC1Badge.Name = "lblC1Badge";
            this.lblC1Badge.Size = new System.Drawing.Size(44, 20);
            this.lblC1Badge.TabIndex = 1;
            this.lblC1Badge.Text = "0.0%";
            // 
            // lblC1Value
            // 
            this.lblC1Value.AutoSize = true;
            this.lblC1Value.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblC1Value.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblC1Value.Location = new System.Drawing.Point(15, 45);
            this.lblC1Value.Name = "lblC1Value";
            this.lblC1Value.Size = new System.Drawing.Size(62, 46);
            this.lblC1Value.TabIndex = 2;
            this.lblC1Value.Text = "0đ";
            // 
            // dateFilter
            // 
            this.dateFilter.BackColor = System.Drawing.Color.Transparent;
            this.dateFilter.Location = new System.Drawing.Point(20, 0);
            this.dateFilter.Mode = DemoPick.UCDateRangeFilter.DateFilterMode.Range;
            this.dateFilter.Name = "dateFilter";
            this.dateFilter.Size = new System.Drawing.Size(540, 39);
            this.dateFilter.TabIndex = 100;
            // 
            // pnlCard2
            // 
            this.pnlCard2.Controls.Add(this.lblC2Title);
            this.pnlCard2.Controls.Add(this.lblC2Badge);
            this.pnlCard2.Controls.Add(this.lblC2Value);
            this.pnlCard2.FillColor = System.Drawing.Color.White;
            this.pnlCard2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlCard2.Location = new System.Drawing.Point(400, 60);
            this.pnlCard2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlCard2.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlCard2.Name = "pnlCard2";
            this.pnlCard2.Radius = 16;
            this.pnlCard2.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.pnlCard2.Size = new System.Drawing.Size(360, 110);
            this.pnlCard2.TabIndex = 1;
            this.pnlCard2.Text = null;
            this.pnlCard2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblC2Title
            // 
            this.lblC2Title.AutoSize = true;
            this.lblC2Title.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblC2Title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblC2Title.Location = new System.Drawing.Point(20, 20);
            this.lblC2Title.Name = "lblC2Title";
            this.lblC2Title.Size = new System.Drawing.Size(108, 23);
            this.lblC2Title.TabIndex = 0;
            this.lblC2Title.Text = "CÔNG SUẤT";
            // 
            // lblC2Badge
            // 
            this.lblC2Badge.AutoSize = true;
            this.lblC2Badge.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblC2Badge.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblC2Badge.Location = new System.Drawing.Point(280, 20);
            this.lblC2Badge.Name = "lblC2Badge";
            this.lblC2Badge.Size = new System.Drawing.Size(44, 20);
            this.lblC2Badge.TabIndex = 1;
            this.lblC2Badge.Text = "0.0%";
            // 
            // lblC2Value
            // 
            this.lblC2Value.AutoSize = true;
            this.lblC2Value.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblC2Value.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblC2Value.Location = new System.Drawing.Point(15, 45);
            this.lblC2Value.Name = "lblC2Value";
            this.lblC2Value.Size = new System.Drawing.Size(69, 46);
            this.lblC2Value.TabIndex = 2;
            this.lblC2Value.Text = "0%";
            // 
            // pnlCard3
            // 
            this.pnlCard3.Controls.Add(this.lblC3Title);
            this.pnlCard3.Controls.Add(this.lblC3Badge);
            this.pnlCard3.Controls.Add(this.lblC3Value);
            this.pnlCard3.FillColor = System.Drawing.Color.White;
            this.pnlCard3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlCard3.Location = new System.Drawing.Point(780, 60);
            this.pnlCard3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlCard3.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlCard3.Name = "pnlCard3";
            this.pnlCard3.Radius = 16;
            this.pnlCard3.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.pnlCard3.Size = new System.Drawing.Size(350, 110);
            this.pnlCard3.TabIndex = 2;
            this.pnlCard3.Text = null;
            this.pnlCard3.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblC3Title
            // 
            this.lblC3Title.AutoSize = true;
            this.lblC3Title.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblC3Title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblC3Title.Location = new System.Drawing.Point(20, 20);
            this.lblC3Title.Name = "lblC3Title";
            this.lblC3Title.Size = new System.Drawing.Size(109, 23);
            this.lblC3Title.TabIndex = 0;
            this.lblC3Title.Text = "KHÁCH MỚI";
            // 
            // lblC3Badge
            // 
            this.lblC3Badge.AutoSize = true;
            this.lblC3Badge.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblC3Badge.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblC3Badge.Location = new System.Drawing.Point(270, 20);
            this.lblC3Badge.Name = "lblC3Badge";
            this.lblC3Badge.Size = new System.Drawing.Size(44, 20);
            this.lblC3Badge.TabIndex = 1;
            this.lblC3Badge.Text = "0.0%";
            // 
            // lblC3Value
            // 
            this.lblC3Value.AutoSize = true;
            this.lblC3Value.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblC3Value.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblC3Value.Location = new System.Drawing.Point(15, 45);
            this.lblC3Value.Name = "lblC3Value";
            this.lblC3Value.Size = new System.Drawing.Size(40, 46);
            this.lblC3Value.TabIndex = 2;
            this.lblC3Value.Text = "0";
            // 
            // pnlTrend
            // 
            this.pnlTrend.Controls.Add(this.lblTrendTitle);
            this.pnlTrend.Controls.Add(this.chartTrend);
            this.pnlTrend.FillColor = System.Drawing.Color.White;
            this.pnlTrend.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlTrend.Location = new System.Drawing.Point(20, 190);
            this.pnlTrend.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlTrend.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlTrend.Name = "pnlTrend";
            this.pnlTrend.Radius = 16;
            this.pnlTrend.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.pnlTrend.Size = new System.Drawing.Size(740, 300);
            this.pnlTrend.TabIndex = 3;
            this.pnlTrend.Text = null;
            this.pnlTrend.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTrendTitle
            // 
            this.lblTrendTitle.AutoSize = true;
            this.lblTrendTitle.BackColor = System.Drawing.Color.White;
            this.lblTrendTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTrendTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTrendTitle.Name = "lblTrendTitle";
            this.lblTrendTitle.Size = new System.Drawing.Size(208, 28);
            this.lblTrendTitle.TabIndex = 0;
            this.lblTrendTitle.Text = "Xu hướng doanh thu";
            // 
            // chartTrend
            // 
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            chartArea1.Name = "TrendArea";
            this.chartTrend.ChartAreas.Add(chartArea1);
            this.chartTrend.Location = new System.Drawing.Point(20, 60);
            this.chartTrend.Name = "chartTrend";
            series1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            series1.BorderWidth = 3;
            series1.ChartArea = "TrendArea";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.SplineArea;
            series1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            series1.Name = "Revenue";
            this.chartTrend.Series.Add(series1);
            this.chartTrend.Size = new System.Drawing.Size(700, 220);
            this.chartTrend.TabIndex = 1;
            // 
            // pnlPie
            // 
            this.pnlPie.Controls.Add(this.lblPieTitle);
            this.pnlPie.Controls.Add(this.chartPie);
            this.pnlPie.FillColor = System.Drawing.Color.White;
            this.pnlPie.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlPie.Location = new System.Drawing.Point(780, 190);
            this.pnlPie.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlPie.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlPie.Name = "pnlPie";
            this.pnlPie.Radius = 16;
            this.pnlPie.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.pnlPie.Size = new System.Drawing.Size(350, 300);
            this.pnlPie.TabIndex = 4;
            this.pnlPie.Text = null;
            this.pnlPie.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPieTitle
            // 
            this.lblPieTitle.AutoSize = true;
            this.lblPieTitle.BackColor = System.Drawing.Color.White;
            this.lblPieTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblPieTitle.Location = new System.Drawing.Point(20, 20);
            this.lblPieTitle.Name = "lblPieTitle";
            this.lblPieTitle.Size = new System.Drawing.Size(201, 28);
            this.lblPieTitle.TabIndex = 0;
            this.lblPieTitle.Text = "Doanh thu theo Sân";
            // 
            // chartPie
            // 
            chartArea2.Name = "PieArea";
            this.chartPie.ChartAreas.Add(chartArea2);
            legend1.Alignment = System.Drawing.StringAlignment.Center;
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend1.Name = "Legend1";
            this.chartPie.Legends.Add(legend1);
            this.chartPie.Location = new System.Drawing.Point(20, 60);
            this.chartPie.Name = "chartPie";
            series2.ChartArea = "PieArea";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Doughnut;
            series2.Legend = "Legend1";
            series2.Name = "PieSeries";
            this.chartPie.Series.Add(series2);
            this.chartPie.Size = new System.Drawing.Size(310, 220);
            this.chartPie.TabIndex = 1;
            // 
            // pnlTopCourts
            // 
            this.pnlTopCourts.Controls.Add(this.lblTopCourtsTitle);
            this.pnlTopCourts.Controls.Add(this.lblTopCourtsViewAll);
            this.pnlTopCourts.Controls.Add(this.lstTopCourts);
            this.pnlTopCourts.FillColor = System.Drawing.Color.White;
            this.pnlTopCourts.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlTopCourts.Location = new System.Drawing.Point(20, 510);
            this.pnlTopCourts.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlTopCourts.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlTopCourts.Name = "pnlTopCourts";
            this.pnlTopCourts.Radius = 16;
            this.pnlTopCourts.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.pnlTopCourts.Size = new System.Drawing.Size(1110, 280);
            this.pnlTopCourts.TabIndex = 5;
            this.pnlTopCourts.Text = null;
            this.pnlTopCourts.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTopCourtsTitle
            // 
            this.lblTopCourtsTitle.AutoSize = true;
            this.lblTopCourtsTitle.BackColor = System.Drawing.Color.White;
            this.lblTopCourtsTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTopCourtsTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTopCourtsTitle.Name = "lblTopCourtsTitle";
            this.lblTopCourtsTitle.Size = new System.Drawing.Size(161, 28);
            this.lblTopCourtsTitle.TabIndex = 0;
            this.lblTopCourtsTitle.Text = "Sân đấu nổi bật";
            // 
            // lblTopCourtsViewAll
            // 
            this.lblTopCourtsViewAll.AutoSize = true;
            this.lblTopCourtsViewAll.BackColor = System.Drawing.Color.White;
            this.lblTopCourtsViewAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblTopCourtsViewAll.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTopCourtsViewAll.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.lblTopCourtsViewAll.Location = new System.Drawing.Point(1020, 25);
            this.lblTopCourtsViewAll.Name = "lblTopCourtsViewAll";
            this.lblTopCourtsViewAll.Size = new System.Drawing.Size(80, 20);
            this.lblTopCourtsViewAll.TabIndex = 1;
            this.lblTopCourtsViewAll.Text = "Xem tất cả";
            // 
            // lstTopCourts
            // 
            this.lstTopCourts.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstTopCourts.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.lstTopCourts.FullRowSelect = true;
            this.lstTopCourts.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstTopCourts.HideSelection = false;
            this.lstTopCourts.Location = new System.Drawing.Point(20, 60);
            this.lstTopCourts.Name = "lstTopCourts";
            this.lstTopCourts.Size = new System.Drawing.Size(1070, 200);
            this.lstTopCourts.TabIndex = 2;
            this.lstTopCourts.UseCompatibleStateImageBehavior = false;
            this.lstTopCourts.View = System.Windows.Forms.View.Details;
            // 
            // UCBaoCao
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.dateFilter);
            this.Controls.Add(this.pnlCard1);
            this.Controls.Add(this.pnlCard2);
            this.Controls.Add(this.pnlCard3);
            this.Controls.Add(this.pnlTrend);
            this.Controls.Add(this.pnlPie);
            this.Controls.Add(this.pnlTopCourts);
            this.Name = "UCBaoCao";
            this.Size = new System.Drawing.Size(1160, 820);
            this.pnlCard1.ResumeLayout(false);
            this.pnlCard1.PerformLayout();
            this.pnlCard2.ResumeLayout(false);
            this.pnlCard2.PerformLayout();
            this.pnlCard3.ResumeLayout(false);
            this.pnlCard3.PerformLayout();
            this.pnlTrend.ResumeLayout(false);
            this.pnlTrend.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartTrend)).EndInit();
            this.pnlPie.ResumeLayout(false);
            this.pnlPie.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartPie)).EndInit();
            this.pnlTopCourts.ResumeLayout(false);
            this.pnlTopCourts.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    }
}
