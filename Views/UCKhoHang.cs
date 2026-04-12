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

        private bool _listColumnsInitialized;

        public UCKhoHang()
        {
            InitializeComponent();

            if (DesignModeUtil.IsDesignMode(this))
            {
                return;
            }
            _inventoryService = new InventoryService();

            EnsureListColumns();

            btnThemSP.Click += BtnThemSP_Click;
            btnXoaSP.Click += BtnXoaSP_Click;

            LoadDataAsync();
        }

        private void EnsureListColumns()
        {
            if (_listColumnsInitialized) return;
            _listColumnsInitialized = true;

            // Inventory list
            lstKhoHang.BeginUpdate();
            try
            {
                lstKhoHang.Columns.Clear();
                lstKhoHang.Columns.Add("Sản phẩm", 360);
                lstKhoHang.Columns.Add("Nhóm", 160);
                lstKhoHang.Columns.Add("Tồn", 110, HorizontalAlignment.Right);
                lstKhoHang.Columns.Add("Trạng thái", 150);
                lstKhoHang.Columns.Add("Giá", 110, HorizontalAlignment.Right);
            }
            finally
            {
                lstKhoHang.EndUpdate();
            }

            // Recent transactions
            lstGiaoDich.BeginUpdate();
            try
            {
                lstGiaoDich.Columns.Clear();
                // HeaderStyle=None in Designer, but Columns are still required to show content.
                lstGiaoDich.Columns.Add("", 380);
                lstGiaoDich.Columns.Add("", 110, HorizontalAlignment.Right);
            }
            finally
            {
                lstGiaoDich.EndUpdate();
            }
        }

        private static string ToUiStatus(string raw)
        {
            string s = (raw ?? string.Empty).Trim();
            if (s.Length == 0) return string.Empty;

            if (string.Equals(s, "Out of Stock", StringComparison.OrdinalIgnoreCase)) return "Hết hàng";
            if (string.Equals(s, "Critical Low", StringComparison.OrdinalIgnoreCase)) return "Cảnh báo";
            if (string.Equals(s, "Warning", StringComparison.OrdinalIgnoreCase)) return "Sắp hết";
            if (string.Equals(s, "Healthy", StringComparison.OrdinalIgnoreCase)) return "Ổn";

            return s;
        }

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
                            // Best effort cross-module refresh.
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("Inventory Delete Product Page Error", ex, "UCKhoHang.BtnXoaSP_Click");
                MessageBox.Show("Không thể mở trang xóa sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                lstKhoHang.BeginUpdate();
                try
                {
                    lstKhoHang.Items.Clear();
                    foreach (var item in items)
                    {
                        string nameCell = item.Name ?? string.Empty;

                        var lvi = new ListViewItem(new[] { nameCell, item.Category, item.Stock, ToUiStatus(item.Status), item.Price });
                        lvi.Tag = new DemoPick.Models.ProductCatalogItemModel
                        {
                            ProductId = item.ProductId,
                            Name = item.Name,
                            Category = item.Category
                        };
                        lstKhoHang.Items.Add(lvi);
                    }
                }
                finally
                {
                    lstKhoHang.EndUpdate();
                }

                var txs = txsTask.Result;
                lstGiaoDich.BeginUpdate();
                try
                {
                    lstGiaoDich.Items.Clear();
                    foreach (var tx in txs)
                    {
                        string sub = (tx.SubDesc ?? "").Replace("\r", " ").Replace("\n", " ").Trim();
                        string line = string.IsNullOrWhiteSpace(sub) ? (tx.EventDesc ?? "") : $"{tx.EventDesc} — {sub}";
                        lstGiaoDich.Items.Add(new ListViewItem(new[] { line, tx.Time }));
                    }
                }
                finally
                {
                    lstGiaoDich.EndUpdate();
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
