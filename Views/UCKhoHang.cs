// ==========================================================
// File: UCKhoHang.cs
// Role: View (MVC)
// Description: UserControl quản lý kho hàng và giao dịch kho.
// Lấy dữ liệu qua InventoryController.
// ==========================================================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using DemoPick.Controllers;
using DemoPick.Models;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;

namespace DemoPick
{
    public partial class UCKhoHang : UserControl
    {
        private InventoryController _inventoryController;
        private bool _listColumnsInitialized;

        // Khoi tao man hinh/control UCKhoHang va chuan bi trang thai ban dau can dung.
        public UCKhoHang()
        {
            InitializeComponent();

            if (DesignModeUtil.IsDesignMode(this))
            {
                return;
            }

            _inventoryController = new InventoryController();

            EnsureListColumns();

            btnThemSP.Click += BtnThemSP_Click;
            btnXoaSP.Click += BtnXoaSP_Click;

            LoadDataAsync();
        }

        // Dam bao dieu kien Ensure List Columns da san sang truoc khi chay buoc xu ly tiep theo.
        private void EnsureListColumns()
        {
            if (_listColumnsInitialized)
            {
                return;
            }

            _listColumnsInitialized = true;

            lstKhoHang.BeginUpdate();
            try
            {
                lstKhoHang.Columns.Clear();
                lstKhoHang.Columns.Add("Sản phẩm", 320);
                lstKhoHang.Columns.Add("Nhóm", 150);
                lstKhoHang.Columns.Add("Tồn", 90, HorizontalAlignment.Right);
                lstKhoHang.Columns.Add("Trạng thái", 130);
                lstKhoHang.Columns.Add("Giá", 110, HorizontalAlignment.Right);
                lstKhoHang.Columns.Add("Đề xuất", 280);
            }
            finally
            {
                lstKhoHang.EndUpdate();
            }

            lstGiaoDich.BeginUpdate();
            try
            {
                lstGiaoDich.Columns.Clear();
                lstGiaoDich.Columns.Add(string.Empty, 380);
                lstGiaoDich.Columns.Add(string.Empty, 110, HorizontalAlignment.Right);
            }
            finally
            {
                lstGiaoDich.EndUpdate();
            }
        }

        // Xu ly logic man hinh To Ui Status va cap nhat control lien quan.
        private static string ToUiStatus(string raw)
        {
            string s = (raw ?? string.Empty).Trim();
            if (s.Length == 0)
            {
                return string.Empty;
            }

            if (string.Equals(s, "Out of Stock", StringComparison.OrdinalIgnoreCase)) return "Hết hàng";
            if (string.Equals(s, "Critical Low", StringComparison.OrdinalIgnoreCase)) return "Cần nhập gấp";
            if (string.Equals(s, "Warning", StringComparison.OrdinalIgnoreCase)) return "Sắp hết";
            if (string.Equals(s, "Healthy", StringComparison.OrdinalIgnoreCase)) return "Ổn";
            return s;
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void BtnXoaSP_Click(object sender, EventArgs e)
        {
            try
            {
                using (var f = new FrmXoaSP())
                {
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        LoadDataAsync();

                        try
                        {
                            var main = FindForm() as FrmChinh;
                            if (main?.banHang != null)
                            {
                                main.banHang.RefreshOnActivated();
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("Inventory Delete Product Page Error", ex, "UCKhoHang.BtnXoaSP_Click");
                MessageBox.Show("Khong the mo trang xoa san pham: " + ex.Message, "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void BtnThemSP_Click(object sender, EventArgs e)
        {
            try
            {
                using (var f = new FrmThemSP())
                {
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        LoadDataAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("Inventory Add Product Error", ex, "UCKhoHang.BtnThemSP_Click");
                MessageBox.Show("Khong the mo form nhap hang: " + ex.Message, "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Nap du lieu cho Load Data roi cap nhat lai trang thai hien thi tren man hinh.
        private async void LoadDataAsync()
        {
            try
            {
                var itemsTask = _inventoryController.GetInventoryItemsAsync();
                var txsTask = _inventoryController.GetRecentTransactionsAsync();
                var kpiTask = _inventoryController.GetInventoryKpisAsync();

                await Task.WhenAll(itemsTask, txsTask, kpiTask);

                var items = itemsTask.Result ?? new List<InventoryItemModel>();
                var insight = _inventoryController.BuildSmartInsights(items);

                BindInventoryItems(items);
                BindTransactions(txsTask.Result);
                BindSmartKpis(kpiTask.Result ?? new InventoryKpiModel(), insight);
                BindForecast(insight);
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("Inventory Load Error", ex, "UCKhoHang.LoadDataAsync");
                BindForecast(new InventorySmartInsightsModel());
            }
        }

        // Dua du lieu Bind Inventory Items len giao dien hoac ve lai phan hien thi lien quan.
        private void BindInventoryItems(IReadOnlyCollection<InventoryItemModel> items)
        {
            lblListTitle.Text = "Tồn kho thông minh";

            lstKhoHang.BeginUpdate();
            try
            {
                lstKhoHang.Items.Clear();
                foreach (var item in items)
                {
                    var lvi = new ListViewItem(new[]
                    {
                        item.Name ?? string.Empty,
                        item.Category ?? string.Empty,
                        item.Stock ?? "0",
                        ToUiStatus(item.Status),
                        item.Price ?? "0d",
                        item.Recommendation ?? string.Empty
                    });

                    lvi.Tag = new ProductCatalogItemModel
                    {
                        ProductId = item.ProductId,
                        Name = item.Name,
                        Category = item.Category
                    };

                    ApplyRowStyle(lvi, item);
                    lstKhoHang.Items.Add(lvi);
                }
            }
            finally
            {
                lstKhoHang.EndUpdate();
            }
        }

        // Dua du lieu Bind Transactions len giao dien hoac ve lai phan hien thi lien quan.
        private void BindTransactions(IReadOnlyCollection<TransactionModel> txs)
        {
            lblBotLeftTitle.Text = "Giao dịch kho gần đây";
            var safeTransactions = txs ?? Array.Empty<TransactionModel>();

            lstGiaoDich.BeginUpdate();
            try
            {
                lstGiaoDich.Items.Clear();
                foreach (var tx in safeTransactions)
                {
                    string sub = (tx.SubDesc ?? string.Empty).Replace("\r", " ").Replace("\n", " ").Trim();
                    string eventText = (tx.EventDesc ?? string.Empty).Trim();

                    string line;
                    if (string.Equals(eventText, "POS", StringComparison.OrdinalIgnoreCase))
                    {
                        line = string.IsNullOrWhiteSpace(sub) ? "POS" : sub;
                    }
                    else
                    {
                        line = string.IsNullOrWhiteSpace(sub) ? eventText : eventText + " - " + sub;
                    }

                    lstGiaoDich.Items.Add(new ListViewItem(new[] { line, tx.Time ?? string.Empty }));
                }
            }
            finally
            {
                lstGiaoDich.EndUpdate();
            }
        }

        // Dua du lieu Bind Smart Kpis len giao dien hoac ve lai phan hien thi lien quan.
        private void BindSmartKpis(InventoryKpiModel kpi, InventorySmartInsightsModel insight)
        {
            lblC1Title.Text = "Tổng giá trị kho";
            lblC1Value.Text = kpi.TotalValue <= 0m ? "0d" : kpi.TotalValue.ToString("N0") + " d";
            lblC1Badge.Text = insight.WarningItemsCount > 0 ? insight.WarningItemsCount + " mục cần theo dõi" : "Ổn định";

            lblC2Title.Text = "Cảnh báo sắp hết";
            lblC2Value.Text = insight.WarningItemsCount + " SP";
            lblC2Badge.Text = insight.OutOfStockCount > 0 ? insight.OutOfStockCount + " hết hàng" : "Không có mục hết";

            lblC3Title.Text = "Bán 14 ngày";
            lblC3Value.Text = insight.SoldLast14Days + " SP";
            lblC3Badge.Text = insight.AvgDailySales > 0m ? insight.AvgDailySales.ToString("0.0") + "/ngày" : "Chưa có tốc độ bán";

            lblC4Title.Text = "Đề xuất nhập hàng";
            lblC4Value.Text = insight.SuggestedReorderProductCount + " SP";
            lblC4Badge.Text = insight.SuggestedReorderUnits > 0 ? "+" + insight.SuggestedReorderUnits.ToString("N0") + " đơn vị" : "Tạm đủ";
        }

        // Dua du lieu Bind Forecast len giao dien hoac ve lai phan hien thi lien quan.
        private void BindForecast(InventorySmartInsightsModel insight)
        {
            lblBotRightTitle.Text = "Dự báo tồn kho 7 ngày tới";

            if (chartDuBao.Series.Count == 0)
            {
                return;
            }

            var area = chartDuBao.ChartAreas[0];
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(229, 231, 235);
            area.AxisX.LabelStyle.ForeColor = Color.FromArgb(107, 114, 128);
            area.AxisY.LabelStyle.ForeColor = Color.FromArgb(107, 114, 128);
            area.AxisY.Minimum = 0;

            var series = chartDuBao.Series[0];
            series.Points.Clear();
            series.ChartType = SeriesChartType.SplineArea;
            series.BorderWidth = 3;
            series.Color = Color.FromArgb(239, 68, 68);
            series.BackSecondaryColor = Color.FromArgb(254, 226, 226);
            series.BackGradientStyle = GradientStyle.TopBottom;
            series.MarkerStyle = MarkerStyle.Circle;
            series.MarkerSize = 8;
            series.MarkerColor = Color.White;
            series.MarkerBorderColor = Color.FromArgb(239, 68, 68);
            series.MarkerBorderWidth = 2;
            series.ToolTip = "Dự kiến #VAL mặt hàng cạn kho vào #AXISLABEL";

            if (insight?.ForecastPoints == null || insight.ForecastPoints.Count == 0)
            {
                for (int day = 1; day <= 7; day++)
                {
                    string label = day == 1 ? "Hôm nay" : (day == 2 ? "Ngày mai" : "Ngày " + day);
                    series.Points.AddXY(label, 0);
                }

                return;
            }

            foreach (var point in insight.ForecastPoints)
            {
                series.Points.AddXY(point.Label, point.RiskItems);
            }
        }

        // Ap dung hoac chuan hoa trang thai Apply Row Style de du lieu/giao dien nhat quan.
        private static void ApplyRowStyle(ListViewItem item, InventoryItemModel model)
        {
            if (item == null || model == null)
            {
                return;
            }

            if (string.Equals(model.Status, "Out of Stock", StringComparison.OrdinalIgnoreCase))
            {
                item.ForeColor = Color.FromArgb(220, 38, 38);
                return;
            }

            if (string.Equals(model.Status, "Critical Low", StringComparison.OrdinalIgnoreCase))
            {
                item.ForeColor = Color.FromArgb(234, 88, 12);
                return;
            }

            if (string.Equals(model.Status, "Warning", StringComparison.OrdinalIgnoreCase))
            {
                item.ForeColor = Color.FromArgb(202, 138, 4);
            }
        }

        // Cap nhat lai du lieu/trang thai Refresh On Activated tren man hinh hien tai.
        public void RefreshOnActivated()
        {
            LoadDataAsync();
        }
    }
}


