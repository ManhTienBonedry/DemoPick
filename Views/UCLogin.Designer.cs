using System.Drawing;
using System.Windows.Forms;
using Sunny.UI;

namespace DemoPick
{
    partial class UCLogin
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlLeft;
        private Panel pnlRight;
        private PictureBox picCourt;
        private Label lblHeroTitle;
        private Label lblHeroSub;
        private Label lblTitle;
        private Label lblSubTitle;
        private Label lblEmail;
        private UITextBox txtEmail;
        private Label lblPass;
        private UITextBox txtPass;
        private UICheckBox chkRemember;
        private LinkLabel lnkForgot;
        private UIButton btnLogin;
        private UIButton btnRegister;
        private Label lblNoAcc;
        private Label lblRegisterNow;
        private Label btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.lblHeroTitle = new System.Windows.Forms.Label();
            this.lblHeroSub = new System.Windows.Forms.Label();
            this.picCourt = new System.Windows.Forms.PictureBox();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubTitle = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new Sunny.UI.UITextBox();
            this.lblPass = new System.Windows.Forms.Label();
            this.txtPass = new Sunny.UI.UITextBox();
            this.chkRemember = new Sunny.UI.UICheckBox();
            this.lnkForgot = new System.Windows.Forms.LinkLabel();
            this.btnLogin = new Sunny.UI.UIButton();
            this.btnRegister = new Sunny.UI.UIButton();
            this.lblNoAcc = new System.Windows.Forms.Label();
            this.lblRegisterNow = new System.Windows.Forms.Label();
            
            this.pnlLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCourt)).BeginInit();
            this.pnlRight.SuspendLayout();
            this.SuspendLayout();
            
            // pnlLeft
            this.pnlLeft.BackColor = System.Drawing.Color.FromArgb(238, 253, 242);
            this.pnlLeft.Controls.Add(this.picCourt);
            this.pnlLeft.Controls.Add(this.lblHeroTitle);
            this.pnlLeft.Controls.Add(this.lblHeroSub);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(420, 620);
            this.pnlLeft.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlLeft_Paint);
            
            // lblHeroTitle
            this.lblHeroTitle.AutoSize = true;
            this.lblHeroTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblHeroTitle.ForeColor = System.Drawing.Color.FromArgb(26, 35, 50);
            this.lblHeroTitle.Location = new System.Drawing.Point(40, 390);
            this.lblHeroTitle.Name = "lblHeroTitle";
            this.lblHeroTitle.Size = new System.Drawing.Size(361, 37);
            this.lblHeroTitle.Text = "Nâng tầm sân chơi của bạn";
            this.lblHeroTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            
            // lblHeroSub
            this.lblHeroSub.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblHeroSub.ForeColor = System.Drawing.Color.Gray;
            this.lblHeroSub.Location = new System.Drawing.Point(45, 430);
            this.lblHeroSub.Name = "lblHeroSub";
            this.lblHeroSub.Size = new System.Drawing.Size(330, 45);
            this.lblHeroSub.Text = "Giải pháp quản lý sân Pickleball chuyên nghiệp, hiện đại và tối ưu nhất.";
            this.lblHeroSub.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            
            // picCourt
            this.picCourt.BackColor = System.Drawing.Color.FromArgb(238, 253, 242);
            this.picCourt.Image = global::DemoPick.Properties.Resources.pick;
            this.picCourt.Location = new System.Drawing.Point(50, 160);
            this.picCourt.Name = "picCourt";
            this.picCourt.Size = new System.Drawing.Size(320, 200);
            this.picCourt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            
            // pnlRight
            this.pnlRight.BackColor = System.Drawing.Color.White;
            this.pnlRight.Controls.Add(this.btnClose);
            this.pnlRight.Controls.Add(this.lblTitle);
            this.pnlRight.Controls.Add(this.lblSubTitle);
            this.pnlRight.Controls.Add(this.lblEmail);
            this.pnlRight.Controls.Add(this.txtEmail);
            this.pnlRight.Controls.Add(this.lblPass);
            this.pnlRight.Controls.Add(this.txtPass);
            this.pnlRight.Controls.Add(this.chkRemember);
            this.pnlRight.Controls.Add(this.lnkForgot);
            this.pnlRight.Controls.Add(this.btnLogin);
            this.pnlRight.Controls.Add(this.btnRegister);
            this.pnlRight.Controls.Add(this.lblNoAcc);
            this.pnlRight.Controls.Add(this.lblRegisterNow);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Location = new System.Drawing.Point(420, 0);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(480, 620);
            
            // btnClose
            this.btnClose.AutoSize = true;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.Silver;
            this.btnClose.Location = new System.Drawing.Point(440, 15);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(30, 32);
            this.btnClose.Text = "X";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            
            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(26, 35, 50);
            this.lblTitle.Location = new System.Drawing.Point(50, 70);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(338, 50);
            this.lblTitle.Text = "Chào mừng trở lại";
            
            // lblSubTitle
            this.lblSubTitle.AutoSize = true;
            this.lblSubTitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSubTitle.ForeColor = System.Drawing.Color.Gray;
            this.lblSubTitle.Location = new System.Drawing.Point(55, 120);
            this.lblSubTitle.Name = "lblSubTitle";
            this.lblSubTitle.Size = new System.Drawing.Size(381, 23);
            this.lblSubTitle.Text = "Vui lòng đăng nhập để quản lý sân chơi của bạn";
            
            // lblEmail
            this.lblEmail.AutoSize = true;
            this.lblEmail.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblEmail.ForeColor = System.Drawing.Color.FromArgb(50, 60, 70);
            this.lblEmail.Location = new System.Drawing.Point(55, 180);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(248, 23);
            this.lblEmail.Text = "👤 Tên đăng nhập hoặc Email";
            
            // txtEmail
            this.txtEmail.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtEmail.FillColor = System.Drawing.Color.FromArgb(250, 252, 253);
            this.txtEmail.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtEmail.Location = new System.Drawing.Point(55, 210);
            this.txtEmail.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Padding = new System.Windows.Forms.Padding(5);
            this.txtEmail.Radius = 8;
            this.txtEmail.RectColor = System.Drawing.Color.FromArgb(229, 231, 235);
            this.txtEmail.ShowText = false;
            this.txtEmail.Size = new System.Drawing.Size(380, 45);
            this.txtEmail.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtEmail.Watermark = " Nhập tài khoản của bạn";
            
            // lblPass
            this.lblPass.AutoSize = true;
            this.lblPass.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPass.ForeColor = System.Drawing.Color.FromArgb(50, 60, 70);
            this.lblPass.Location = new System.Drawing.Point(55, 280);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(115, 23);
            this.lblPass.Text = "🔒 Mật khẩu";
            
            // txtPass
            this.txtPass.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPass.FillColor = System.Drawing.Color.FromArgb(250, 252, 253);
            this.txtPass.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtPass.Location = new System.Drawing.Point(55, 310);
            this.txtPass.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtPass.Name = "txtPass";
            this.txtPass.Padding = new System.Windows.Forms.Padding(5);
            this.txtPass.PasswordChar = '●';
            this.txtPass.Radius = 8;
            this.txtPass.RectColor = System.Drawing.Color.FromArgb(229, 231, 235);
            this.txtPass.ShowText = false;
            this.txtPass.Size = new System.Drawing.Size(380, 45);
            this.txtPass.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtPass.Watermark = " Nhập mật khẩu";
            
            // chkRemember
            this.chkRemember.CheckBoxColor = System.Drawing.Color.LightGray;
            this.chkRemember.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkRemember.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkRemember.ForeColor = System.Drawing.Color.FromArgb(100, 100, 100);
            this.chkRemember.Location = new System.Drawing.Point(55, 370);
            this.chkRemember.MinimumSize = new System.Drawing.Size(1, 1);
            this.chkRemember.Name = "chkRemember";
            this.chkRemember.Size = new System.Drawing.Size(186, 29);
            this.chkRemember.Text = "Ghi nhớ đăng nhập";
            
            // lnkForgot
            this.lnkForgot.AutoSize = true;
            this.lnkForgot.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lnkForgot.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkForgot.LinkColor = System.Drawing.Color.FromArgb(119, 219, 44);
            this.lnkForgot.Location = new System.Drawing.Point(325, 373);
            this.lnkForgot.Name = "lnkForgot";
            this.lnkForgot.Size = new System.Drawing.Size(140, 23);
            this.lnkForgot.TabStop = true;
            this.lnkForgot.Text = "Quên mật khẩu?";
            
            // btnLogin
            this.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogin.FillColor = System.Drawing.Color.FromArgb(119, 219, 44);
            this.btnLogin.FillHoverColor = System.Drawing.Color.FromArgb(129, 229, 54);
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnLogin.Location = new System.Drawing.Point(55, 420);
            this.btnLogin.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Radius = 8;
            this.btnLogin.RectColor = System.Drawing.Color.FromArgb(119, 219, 44);
            this.btnLogin.Size = new System.Drawing.Size(380, 45);
            this.btnLogin.Text = "➜  Đăng nhập ngay";
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            
            // btnRegister
            this.btnRegister.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRegister.FillColor = System.Drawing.Color.White;
            this.btnRegister.FillHoverColor = System.Drawing.Color.WhiteSmoke;
            this.btnRegister.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnRegister.ForeColor = System.Drawing.Color.FromArgb(50, 60, 70);
            this.btnRegister.ForeHoverColor = System.Drawing.Color.FromArgb(50, 60, 70);
            this.btnRegister.Location = new System.Drawing.Point(55, 480);
            this.btnRegister.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Radius = 8;
            this.btnRegister.RectColor = System.Drawing.Color.FromArgb(220, 220, 220);
            this.btnRegister.RectHoverColor = System.Drawing.Color.FromArgb(200, 200, 200);
            this.btnRegister.Size = new System.Drawing.Size(380, 45);
            this.btnRegister.Text = "👤 Tạo tài khoản mới";
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            
            // lblNoAcc
            this.lblNoAcc.AutoSize = true;
            this.lblNoAcc.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNoAcc.ForeColor = System.Drawing.Color.Gray;
            this.lblNoAcc.Location = new System.Drawing.Point(120, 560);
            this.lblNoAcc.Name = "lblNoAcc";
            this.lblNoAcc.Size = new System.Drawing.Size(188, 23);
            this.lblNoAcc.Text = "Bạn chưa có tài khoản?";
            
            // lblRegisterNow
            this.lblRegisterNow.AutoSize = true;
            this.lblRegisterNow.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblRegisterNow.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblRegisterNow.ForeColor = System.Drawing.Color.FromArgb(119, 219, 44);
            this.lblRegisterNow.Location = new System.Drawing.Point(315, 560);
            this.lblRegisterNow.Name = "lblRegisterNow";
            this.lblRegisterNow.Size = new System.Drawing.Size(121, 23);
            this.lblRegisterNow.Text = "Đăng ký ngay";
            this.lblRegisterNow.Click += new System.EventHandler(this.lblRegisterNow_Click);
            
            // UCLogin
            this.Controls.Add(this.pnlRight);
            this.Controls.Add(this.pnlLeft);
            this.Name = "UCLogin";
            this.Size = new System.Drawing.Size(900, 620);
            
            this.pnlLeft.ResumeLayout(false);
            this.pnlLeft.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCourt)).EndInit();
            this.pnlRight.ResumeLayout(false);
            this.pnlRight.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
