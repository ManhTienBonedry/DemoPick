using DemoPick.Helpers;
using DemoPick.Data;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DemoPick
{
    public partial class UCInvoiceReprintPanel : UserControl
    {
        public event EventHandler ReprintByIdRequested;
        public event EventHandler ReprintLastRequested;

        // Khoi tao man hinh/control UCInvoiceReprintPanel va chuan bi trang thai ban dau can dung.
        public UCInvoiceReprintPanel()
        {
            InitializeComponent();

            txtInvoiceId.KeyDown += TxtInvoiceId_KeyDown;
            btnReprintById.Click += BtnReprintById_Click;
            btnReprintLast.Click += BtnReprintLast_Click;

            SetLastInvoiceEnabled(false);
        }

        public string InvoiceIdText
        {
            get { return txtInvoiceId == null ? string.Empty : txtInvoiceId.Text; }
            set
            {
                if (txtInvoiceId != null)
                {
                    txtInvoiceId.Text = value ?? string.Empty;
                }
            }
        }

        // Xu ly logic man hinh Focus Invoice Input va cap nhat control lien quan.
        public void FocusInvoiceInput()
        {
            if (txtInvoiceId == null)
            {
                return;
            }

            txtInvoiceId.Focus();
            txtInvoiceId.SelectAll();
        }

        // Ap dung hoac chuan hoa trang thai Set Last Invoice Enabled de du lieu/giao dien nhat quan.
        public void SetLastInvoiceEnabled(bool canReprint)
        {
            if (btnReprintLast == null)
            {
                return;
            }

            btnReprintLast.Enabled = canReprint;

            if (canReprint)
            {
                btnReprintLast.FillColor = Color.FromArgb(59, 130, 246);
                btnReprintLast.FillHoverColor = Color.FromArgb(37, 99, 235);
                btnReprintLast.ForeColor = Color.White;
            }
            else
            {
                btnReprintLast.FillColor = Color.FromArgb(229, 231, 235);
                btnReprintLast.FillHoverColor = Color.FromArgb(209, 213, 219);
                btnReprintLast.ForeColor = Color.FromArgb(75, 85, 99);
            }
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void BtnReprintById_Click(object sender, EventArgs e)
        {
            var handler = ReprintByIdRequested;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void BtnReprintLast_Click(object sender, EventArgs e)
        {
            var handler = ReprintLastRequested;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        // Xu ly phim bam de kich hoat thao tac nhanh hoac dieu huong tren man hinh.
        private void TxtInvoiceId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e == null || e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.Handled = true;
            e.SuppressKeyPress = true;
            BtnReprintById_Click(this, EventArgs.Empty);
        }
    }
}
