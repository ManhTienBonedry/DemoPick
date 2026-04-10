using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using DemoPick.Services;

namespace DemoPick
{
    public partial class UCKhoHang : UserControl
    {
        private InventoryService _inventoryService;

        public UCKhoHang()
        {
            InitializeComponent();

            if (DesignModeUtil.IsDesignMode(this))
            {
                return;
            }
            _inventoryService = new InventoryService();

            btnThemSP.Click += BtnThemSP_Click;

            LoadDataAsync();
        }

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
                MessageBox.Show("Không thể mở form nhập hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void LoadDataAsync()
        {
            try
            {
                var itemsTask = _inventoryService.GetInventoryItemsAsync();
                var txsTask = _inventoryService.GetRecentTransactionsAsync();
                var kpiTask = _inventoryService.GetInventoryKpisAsync();

                await Task.WhenAll(itemsTask, txsTask, kpiTask);

                var items = itemsTask.Result;
                lstKhoHang.Items.Clear();
                foreach (var item in items)
                {
                    lstKhoHang.Items.Add(new ListViewItem(new[] { $"{item.Name}\nSKU: {item.Sku}", item.Category, item.Stock, item.Status, item.Price }));
                }

                var txs = txsTask.Result;
                lstGiaoDich.Items.Clear();
                foreach (var tx in txs)
                {
                    string sub = (tx.SubDesc ?? "").Replace("\r", " ").Replace("\n", " ").Trim();
                    string line = string.IsNullOrWhiteSpace(sub) ? (tx.EventDesc ?? "") : $"{tx.EventDesc} — {sub}";
                    lstGiaoDich.Items.Add(new ListViewItem(new[] { line, tx.Time }));
                }

                var kpi = kpiTask.Result ?? new DemoPick.Models.InventoryKpiModel();
                lblC1Value.Text = kpi.TotalValue == 0 ? "0đ" : kpi.TotalValue.ToString("N0") + " đ";
                lblC2Value.Text = kpi.CriticalItems + " SP";
                lblC3Value.Text = kpi.Sales + " Xuất";
                lblC4Value.Text = kpi.InvoicesCount + " Đơn";

                lblC1Title.Text = "Tổng giá trị kho";
                lblC2Title.Text = "Cảnh báo hết hàng";
                lblC3Title.Text = "Sản phẩm đã bán";
                lblC4Title.Text = "Hóa đơn xuất";
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("Inventory Load Error", ex, "UCKhoHang.LoadDataAsync");
            }

            // Chart (Clear mock data since DB is clean)
            chartDuBao.Series[0].Points.Clear();
            chartDuBao.Series[0].Points.AddXY("T2", 0);
            chartDuBao.Series[0].Points.AddXY("T3", 0);
            chartDuBao.Series[0].Points.AddXY("T4", 0);
            chartDuBao.Series[0].Points.AddXY("T5", 0);
            chartDuBao.Series[0].Points.AddXY("T6", 0);
            chartDuBao.Series[0].Points.AddXY("T7", 0);
            chartDuBao.Series[0].Points.AddXY("CN", 0);
        }

        public void RefreshOnActivated()
        {
            LoadDataAsync();
        }
    }
}
