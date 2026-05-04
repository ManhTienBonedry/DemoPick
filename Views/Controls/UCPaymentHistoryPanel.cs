using DemoPick.Helpers;
using DemoPick.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DemoPick
{
    public partial class UCPaymentHistoryPanel : UserControl
    {
        private Label _summaryLabel;

        public sealed class HistoryRow
        {
            public string InvoiceCode { get; set; }
            public string TimeText { get; set; }
            public string CustomerText { get; set; }
            public string TotalText { get; set; }
            public string ToolTipText { get; set; }
            public bool IsHighlighted { get; set; }
            public object Tag { get; set; }
        }

        public event EventHandler SearchRequested;
        public event EventHandler OpenRequested;

        // Khoi tao man hinh/control UCPaymentHistoryPanel va chuan bi trang thai ban dau can dung.
        public UCPaymentHistoryPanel()
        {
            InitializeComponent();

            EnsureSummaryLabel();

            txtSearch.KeyDown += TxtSearch_KeyDown;
            btnSearch.Click += BtnSearch_Click;
            btnOpen.Click += BtnOpen_Click;
            gridHistory.DoubleClick += GridHistory_DoubleClick;
        }

        public string SearchKeyword
        {
            get
            {
                if (txtSearch == null) return string.Empty;
                string val = txtSearch.Text;
                if (val == txtSearch.Watermark) return string.Empty;
                return val;
            }
            set
            {
                if (txtSearch != null)
                {
                    txtSearch.Text = value ?? string.Empty;
                }
            }
        }

        // Ap dung hoac chuan hoa trang thai Set Shift Summary de du lieu/giao dien nhat quan.
        public void SetShiftSummary(string summaryText)
        {
            EnsureSummaryLabel();
            if (_summaryLabel == null)
            {
                return;
            }

            _summaryLabel.Text = summaryText ?? string.Empty;
        }

        // Dua du lieu Bind Rows len giao dien hoac ve lai phan hien thi lien quan.
        public void BindRows(IReadOnlyList<HistoryRow> rows)
        {
            if (gridHistory == null)
            {
                return;
            }

            gridHistory.Rows.Clear();

            if (rows != null)
            {
                for (int i = 0; i < rows.Count; i++)
                {
                    var row = rows[i];
                    if (row == null)
                    {
                        continue;
                    }

                    int rowIndex = gridHistory.Rows.Add(
                        row.InvoiceCode ?? string.Empty,
                        row.TimeText ?? string.Empty,
                        row.CustomerText ?? string.Empty,
                        row.TotalText ?? string.Empty
                    );
                    
                    var gridRow = gridHistory.Rows[rowIndex];
                    gridRow.Tag = row.Tag;

                    if (row.IsHighlighted)
                    {
                        gridRow.DefaultCellStyle.BackColor = Color.FromArgb(239, 246, 255);
                    }
                    
                    // Tooltip is not standard supported on row without specific cell events, but let's skip for simple UI
                }
            }
        }

        public bool TryGetSelectedTag<T>(out T selected) where T : class
        {
            selected = null;

            if (gridHistory == null || gridHistory.SelectedRows.Count <= 0)
            {
                return false;
            }

            selected = gridHistory.SelectedRows[0].Tag as T;
            return selected != null;
        }

        // Dam bao dieu kien Ensure Summary Label da san sang truoc khi chay buoc xu ly tiep theo.
        private void EnsureSummaryLabel()
        {
            if (_summaryLabel != null)
            {
                return;
            }

            _summaryLabel = new Label
            {
                AutoSize = false,
                Location = new Point(0, 24),
                Size = new Size(280, 30),
                ForeColor = Color.FromArgb(107, 114, 128),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                Text = string.Empty
            };

            Controls.Add(_summaryLabel);
            _summaryLabel.BringToFront();
            DemoPick.Helpers.UiTheme.NormalizeTextBackgrounds(this);

            // Push controls down to make room for the summary label
            txtSearch.Location = new Point(0, 58);
            btnSearch.Location = new Point(194, 58);
            gridHistory.Location = new Point(0, 96);
            gridHistory.Size = new Size(280, 156);
            btnOpen.Location = new Point(0, 258);
            Size = new Size(280, 296);
        }

        // Xu ly logic man hinh Raise Search Requested va cap nhat control lien quan.
        private void RaiseSearchRequested()
        {
            var handler = SearchRequested;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        // Xu ly logic man hinh Raise Open Requested va cap nhat control lien quan.
        private void RaiseOpenRequested()
        {
            var handler = OpenRequested;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        // Xu ly phim bam de kich hoat thao tac nhanh hoac dieu huong tren man hinh.
        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e == null || e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.Handled = true;
            e.SuppressKeyPress = true;
            RaiseSearchRequested();
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            RaiseSearchRequested();
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void BtnOpen_Click(object sender, EventArgs e)
        {
            RaiseOpenRequested();
        }

        // Xu ly logic man hinh Grid History_Double Click va cap nhat control lien quan.
        private void GridHistory_DoubleClick(object sender, EventArgs e)
        {
            RaiseOpenRequested();
        }
    }
}

