using System;
using System.Drawing;
using System.Windows.Forms;
using Sunny.UI;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;

namespace DemoPick
{
    public partial class FrmUserMenu : Form
    {
        // Khoi tao man hinh/control FrmUserMenu va chuan bi trang thai ban dau can dung.
        public FrmUserMenu()
        {
            InitializeComponent();

            if (DesignModeUtil.IsDesignMode(this))
            {
                return;
            }
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            
            pnlBorder.Paint += (s, e) => 
            {
                using (var pen = new Pen(Color.FromArgb(220, 220, 220)))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                }
            };
            
            IntPtr hRgn = CreateRoundRectRgn(0, 0, Width, Height, 15, 15);
            if (hRgn != IntPtr.Zero)
            {
                this.Region = Region.FromHrgn(hRgn);
                DeleteObject(hRgn);
            }
            this.Deactivate += FrmUserMenu_Deactivate;
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        // Tao hoac tinh ra du lieu Create Round Rect Rgn tu cac thong tin dau vao hien co.
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        [System.Runtime.InteropServices.DllImport("gdi32.dll", SetLastError = true)]
        // Xoa, huy hoac dat lai du lieu Delete Object theo dung dieu kien nghiep vu.
        private static extern bool DeleteObject(IntPtr hObject);

        private bool isOpeningSubForm = false;

        // Xu ly logic man hinh Frm User Menu_Deactivate va cap nhat control lien quan.
        private void FrmUserMenu_Deactivate(object sender, EventArgs e)
        {
            if (!isOpeningSubForm)
            {
                this.Close();
            }
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void btnDoiMatKhau_Click(object sender, EventArgs e)
        {
            isOpeningSubForm = true;
            this.Hide();
            using (var frm = new FrmDoiMatKhau())
            {
                frm.ShowDialog(this.Owner);
            }
            this.Close();
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            AppSession.SignOut();
            // Use Retry to indicate logout to Program.cs
            this.DialogResult = DialogResult.Retry;
            this.Close();
        }

        // Xu ly su kien nguoi dung bam vao control lien quan va goi luong nghiep vu phu hop.
        private void btnThoat_Click(object sender, EventArgs e)
        {
            // Exit the app via the main form so AppFlowContext can end the message loop safely.
            try
            {
                if (this.Owner is Form owner)
                {
                    owner.DialogResult = DialogResult.Cancel;
                    owner.Close();
                }
                else
                {
                    Application.ExitThread();
                }
            }
            catch
            {
                // Best effort.
                try { Application.ExitThread(); } catch { }
            }
        }
    }
}


