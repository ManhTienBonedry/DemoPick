using System;
using System.Drawing;
using System.Windows.Forms;
using DemoPick.Services;

namespace DemoPick
{
    public partial class UCAuditLog : UserControl
    {
        public UCAuditLog()
        {
            InitializeComponent();
            if (DesignModeUtil.IsDesignMode(this)) return;
            LoadLogs();
        }

        public void RefreshOnActivated()
        {
            LoadLogs();
        }

        private async void LoadLogs()
        {
            try
            {
                var dt = await System.Threading.Tasks.Task.Run(() =>
                    DatabaseHelper.ExecuteQuery("SELECT TOP 200 LogID, EventDesc, SubDesc, CreatedAt FROM SystemLogs ORDER BY CreatedAt DESC"));
                if (dgvLogs != null)
                {
                    dgvLogs.DataSource = dt;
                    
                    if (dgvLogs.Columns.Contains("LogID")) { dgvLogs.Columns["LogID"].HeaderText = "ID"; dgvLogs.Columns["LogID"].Width = 80; }
                    if (dgvLogs.Columns.Contains("EventDesc")) { dgvLogs.Columns["EventDesc"].HeaderText = "Sự kiện"; dgvLogs.Columns["EventDesc"].Width = 200; }
                    if (dgvLogs.Columns.Contains("SubDesc")) { dgvLogs.Columns["SubDesc"].HeaderText = "Chi tiết"; dgvLogs.Columns["SubDesc"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; }
                    if (dgvLogs.Columns.Contains("CreatedAt")) { dgvLogs.Columns["CreatedAt"].HeaderText = "Thời gian"; dgvLogs.Columns["CreatedAt"].Width = 150; dgvLogs.Columns["CreatedAt"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss"; }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải nhật ký: " + ex.Message);
            }
        }
    }
}
