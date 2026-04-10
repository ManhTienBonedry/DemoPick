using System;
using System.Drawing;
using System.Windows.Forms;
using Sunny.UI;
using DemoPick.Services;

namespace DemoPick
{
    public partial class FrmUserMenu : Form
    {
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
            
            pnlBorder.Paint += (s, e) => {
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(220, 220, 220)), 0, 0, Width - 1, Height - 1);
            };
            
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
            this.Deactivate += FrmUserMenu_Deactivate;
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private bool isOpeningSubForm = false;

        private void FrmUserMenu_Deactivate(object sender, EventArgs e)
        {
            if (!isOpeningSubForm)
            {
                this.Close();
            }
        }

        private void btnDoiMatKhau_Click(object sender, EventArgs e)
        {
            isOpeningSubForm = true;
            this.Hide();
            var frm = new FrmDoiMatKhau();
            frm.ShowDialog(this.Owner);
            this.Close();
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            AppSession.SignOut();
            // Use Retry to indicate logout to Program.cs
            this.DialogResult = DialogResult.Retry;
            this.Close();
        }

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
