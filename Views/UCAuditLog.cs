using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;

namespace DemoPick
{
    public partial class UCAuditLog : UserControl
    {
        private DataTable _fullData;
        private Timer _searchDebounce;

        // Khoi tao man hinh/control UCAuditLog va chuan bi trang thai ban dau can dung.
        public UCAuditLog()
        {
            InitializeComponent();
            if (DesignModeUtil.IsDesignMode(this)) return;

            _searchDebounce = new Timer { Interval = 300 };
            _searchDebounce.Tick += (s, e) =>
            {
                _searchDebounce.Stop();
                ApplyFilter();
            };

            if (txtSearch != null)
            {
                txtSearch.TextChanged += (s, e) =>
                {
                    _searchDebounce.Stop();
                    _searchDebounce.Start();
                };
            }

            if (dgvLogs != null)
            {
                dgvLogs.CellFormatting += DgvLogs_CellFormatting;
            }

            Disposed += (s, e) =>
            {
                try { _searchDebounce?.Stop(); _searchDebounce?.Dispose(); } catch { }
            };

            LoadLogs();
        }

        // Cap nhat lai du lieu/trang thai Refresh On Activated tren man hinh hien tai.
        public void RefreshOnActivated()
        {
            LoadLogs();
        }

        // Nap du lieu cho Load Logs roi cap nhat lai trang thai hien thi tren man hinh.
        private async void LoadLogs()
        {
            try
            {
                var dt = await System.Threading.Tasks.Task.Run(() =>
                    DatabaseHelper.ExecuteQuery("SELECT TOP 500 LogID, EventDesc, SubDesc, CreatedAt FROM SystemLogs ORDER BY CreatedAt DESC"));

                _fullData = dt;

                if (dgvLogs != null)
                {
                    dgvLogs.DataSource = dt;
                    FormatColumns();
                }

                UpdateKpiCards(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải nhật ký: " + ex.Message);
            }
        }

        // Xu ly logic man hinh Format Columns va cap nhat control lien quan.
        private void FormatColumns()
        {
            if (dgvLogs.Columns.Contains("LogID"))
            {
                dgvLogs.Columns["LogID"].HeaderText = "ID";
                dgvLogs.Columns["LogID"].Width = 70;
            }
            if (dgvLogs.Columns.Contains("EventDesc"))
            {
                dgvLogs.Columns["EventDesc"].HeaderText = "Sự kiện";
                dgvLogs.Columns["EventDesc"].Width = 200;
            }
            if (dgvLogs.Columns.Contains("SubDesc"))
            {
                dgvLogs.Columns["SubDesc"].HeaderText = "Chi tiết";
                dgvLogs.Columns["SubDesc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            if (dgvLogs.Columns.Contains("CreatedAt"))
            {
                dgvLogs.Columns["CreatedAt"].HeaderText = "Thời gian";
                dgvLogs.Columns["CreatedAt"].Width = 170;
                dgvLogs.Columns["CreatedAt"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            }
        }

        // Cap nhat lai du lieu/trang thai Update Kpi Cards tren man hinh hien tai.
        private void UpdateKpiCards(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                SetKpi(lblCardTotalValue, "0");
                SetKpi(lblCardPosValue, "0");
                SetKpi(lblCardInventoryValue, "0");
                SetKpi(lblCardErrorValue, "0");
                return;
            }

            DateTime today = DateTime.Today;
            int total = 0, pos = 0, inventory = 0, errors = 0;

            foreach (DataRow row in dt.Rows)
            {
                DateTime created;
                object rawDate = row["CreatedAt"];
                if (rawDate == null || rawDate == DBNull.Value) continue;
                if (rawDate is DateTime dtVal)
                    created = dtVal;
                else if (!DateTime.TryParse(rawDate.ToString(), out created))
                    continue;

                if (created.Date != today) continue;

                total++;

                string evt = (row["EventDesc"] ?? "").ToString().Trim();

                if (evt.IndexOf("POS", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    evt.IndexOf("Checkout", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    pos++;
                }
                else if (evt.IndexOf("Kho", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         evt.IndexOf("Inventory", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         evt.IndexOf("Nhập", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         evt.IndexOf("Xóa Sản phẩm", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    inventory++;
                }
                else if (evt.IndexOf("Error", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         evt.IndexOf("Lỗi", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         evt.IndexOf("Failed", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    errors++;
                }
            }

            SetKpi(lblCardTotalValue, total.ToString());
            SetKpi(lblCardPosValue, pos.ToString());
            SetKpi(lblCardInventoryValue, inventory.ToString());
            SetKpi(lblCardErrorValue, errors.ToString());
        }

        // Ap dung hoac chuan hoa trang thai Set Kpi de du lieu/giao dien nhat quan.
        private static void SetKpi(Label lbl, string value)
        {
            if (lbl != null) lbl.Text = value;
        }

        // Ap dung hoac chuan hoa trang thai Apply Filter de du lieu/giao dien nhat quan.
        private void ApplyFilter()
        {
            if (_fullData == null || dgvLogs == null) return;

            string keyword = (txtSearch?.Text ?? "").Trim();

            if (string.IsNullOrEmpty(keyword))
            {
                dgvLogs.DataSource = _fullData;
                FormatColumns();
                return;
            }

            // Safe filter: escape special characters for DataTable RowFilter
            string safe = keyword.Replace("'", "''").Replace("%", "[%]").Replace("*", "[*]");

            try
            {
                DataView dv = new DataView(_fullData);
                dv.RowFilter = string.Format(
                    "EventDesc LIKE '%{0}%' OR SubDesc LIKE '%{0}%'", safe);
                dgvLogs.DataSource = dv;
                FormatColumns();
            }
            catch
            {
                dgvLogs.DataSource = _fullData;
                FormatColumns();
            }
        }

        // Xu ly logic man hinh Dgv Logs_Cell Formatting va cap nhat control lien quan.
        private void DgvLogs_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvLogs == null || e.RowIndex < 0) return;
            if (!dgvLogs.Columns.Contains("EventDesc")) return;

            try
            {
                object val = dgvLogs.Rows[e.RowIndex].Cells["EventDesc"].Value;
                if (val == null || val == DBNull.Value) return;

                string evt = val.ToString();
                var row = dgvLogs.Rows[e.RowIndex];

                if (evt.IndexOf("POS", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    evt.IndexOf("Checkout", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(22, 101, 52);
                    if (e.ColumnIndex == dgvLogs.Columns["EventDesc"].Index)
                    {
                        e.CellStyle.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
                    }
                }
                else if (evt.IndexOf("Kho", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         evt.IndexOf("Inventory", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         evt.IndexOf("Nhập", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         evt.IndexOf("Xóa Sản phẩm", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(30, 64, 175);
                    if (e.ColumnIndex == dgvLogs.Columns["EventDesc"].Index)
                    {
                        e.CellStyle.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
                    }
                }
                else if (evt.IndexOf("Error", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         evt.IndexOf("Lỗi", StringComparison.OrdinalIgnoreCase) >= 0 ||
                         evt.IndexOf("Failed", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(185, 28, 28);
                    row.DefaultCellStyle.BackColor = Color.FromArgb(254, 242, 242);
                    if (e.ColumnIndex == dgvLogs.Columns["EventDesc"].Index)
                    {
                        e.CellStyle.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
                    }
                }
            }
            catch
            {
                // Ignore formatting errors silently
            }
        }
    }
}
