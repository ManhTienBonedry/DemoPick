using System;
using System.Drawing;
using System.Windows.Forms;
using DemoPick.Services;

namespace DemoPick
{
    public partial class FrmDatSanCoDinh : Form
    {
        private readonly DemoPick.Controllers.BookingController _controller = new DemoPick.Controllers.BookingController();

        public FrmDatSanCoDinh()
        {
            InitializeComponent();

            if (DesignModeUtil.IsDesignMode(this))
            {
                return;
            }
            
            // Logic for closing
            btnCancel.Click += (s, e) => this.Close();
            btnCancelTop.Click += (s, e) => this.Close();
            
            // Logic for switching modes
            rbKhachThue.CheckedChanged += RbMode_CheckedChanged;
            rbBaoTri.CheckedChanged += RbMode_CheckedChanged;

            // Save logic
            btnConfirm.Click += BtnConfirm_Click;

            this.Load += FrmDatSanCoDinh_Load;
            cbTime.SelectedIndex = 11; // 17:00
            cbDuration.SelectedIndex = 1; // 90 phút
            
            // Default dates
            if (ucDateRange != null)
            {
                ucDateRange.Mode = UCDateRangeFilter.DateFilterMode.Range;
                ucDateRange.ShowApplyButton = false;
                ucDateRange.FromDate = DateTime.Now;
                ucDateRange.ToDate = DateTime.Now.AddMonths(1);
            }

            // Legacy arrow label becomes redundant with the range control.
            if (lblTo != null) lblTo.Visible = false;

            // Drag form logic
            this.MouseDown += Form_MouseDown;
            pnlHeader.MouseDown += Form_MouseDown;
            lblTitle.MouseDown += Form_MouseDown;

            // Form styles
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Region = new Region(RoundedRect(new Rectangle(0, 0, this.Width, this.Height), 20));
            this.Paint += Frm_Paint;
        }
    }
}
