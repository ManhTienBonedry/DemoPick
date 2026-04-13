using System.Drawing;
using System.Windows.Forms;
using Sunny.UI;

namespace DemoPick
{
    partial class UCRegister
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlLeft;
        private Panel pnlRight;
        private PictureBox picCourt;
        private Label lblHeroTitle;
        private Label lblHeroSub;
        
        private Label lblTitle;
        
        private Label lblName;
        private UITextBox txtName;
        
        private Label lblEmail;
        private UITextBox txtEmail;
        
        private Label lblPhone;
        private UITextBox txtPhone;
        
        private Label lblPass;
        private UITextBox txtPass;
        
        private Label lblConfirm;
        private UITextBox txtConfirm;
        
        private UIButton btnRegister;
        private Label lblHasAcc;
        private Label lblLoginNow;
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
            this.pnlRight = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new Sunny.UI.UITextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new Sunny.UI.UITextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.txtPhone = new Sunny.UI.UITextBox();
            this.lblPass = new System.Windows.Forms.Label();
            this.txtPass = new Sunny.UI.UITextBox();
            this.lblConfirm = new System.Windows.Forms.Label();
            this.txtConfirm = new Sunny.UI.UITextBox();
            this.btnRegister = new Sunny.UI.UIButton();
            this.lblHasAcc = new System.Windows.Forms.Label();
            this.lblLoginNow = new System.Windows.Forms.Label();
            this.picCourt = new System.Windows.Forms.PictureBox();
            this.pnlLeft.SuspendLayout();
            this.pnlRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCourt)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlLeft
            // 
            this.pnlLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(253)))), ((int)(((byte)(242)))));
            this.pnlLeft.Controls.Add(this.picCourt);
            this.pnlLeft.Controls.Add(this.lblHeroTitle);
            this.pnlLeft.Controls.Add(this.lblHeroSub);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(420, 620);
            this.pnlLeft.TabIndex = 1;
            // 
            // lblHeroTitle
            // 
            this.lblHeroTitle.AutoSize = true;
            this.lblHeroTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblHeroTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblHeroTitle.Location = new System.Drawing.Point(40, 390);
            this.lblHeroTitle.Name = "lblHeroTitle";
            this.lblHeroTitle.Size = new System.Drawing.Size(361, 37);
            this.lblHeroTitle.TabIndex = 1;
            this.lblHeroTitle.Text = "Nâng tầm sân chơi của bạn";
            this.lblHeroTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblHeroSub
            // 
            this.lblHeroSub.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblHeroSub.ForeColor = System.Drawing.Color.Gray;
            this.lblHeroSub.Location = new System.Drawing.Point(45, 430);
            this.lblHeroSub.Name = "lblHeroSub";
            this.lblHeroSub.Size = new System.Drawing.Size(330, 45);
            this.lblHeroSub.TabIndex = 2;
            this.lblHeroSub.Text = "Giải pháp quản lý sân Pickleball chuyên nghiệp, hiện đại và tối ưu nhất.";
            this.lblHeroSub.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlRight
            // 
            this.pnlRight.BackColor = System.Drawing.Color.White;
            this.pnlRight.Controls.Add(this.btnClose);
            this.pnlRight.Controls.Add(this.lblTitle);
            this.pnlRight.Controls.Add(this.lblName);
            this.pnlRight.Controls.Add(this.txtName);
            this.pnlRight.Controls.Add(this.lblEmail);
            this.pnlRight.Controls.Add(this.txtEmail);
            this.pnlRight.Controls.Add(this.lblPhone);
            this.pnlRight.Controls.Add(this.txtPhone);
            this.pnlRight.Controls.Add(this.lblPass);
            this.pnlRight.Controls.Add(this.txtPass);
            this.pnlRight.Controls.Add(this.lblConfirm);
            this.pnlRight.Controls.Add(this.txtConfirm);
            this.pnlRight.Controls.Add(this.btnRegister);
            this.pnlRight.Controls.Add(this.lblHasAcc);
            this.pnlRight.Controls.Add(this.lblLoginNow);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Location = new System.Drawing.Point(420, 0);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(500, 620);
            this.pnlRight.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.AutoSize = true;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.Silver;
            this.btnClose.Location = new System.Drawing.Point(440, 15);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(30, 32);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "X";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblTitle.Location = new System.Drawing.Point(40, 50);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(338, 50);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Đăng ký tài khoản";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.lblName.Location = new System.Drawing.Point(45, 140);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(102, 20);
            this.lblName.TabIndex = 3;
            this.lblName.Text = "👤 Họ và tên";
            // 
            // txtName
            // 
            this.txtName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtName.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.txtName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtName.Location = new System.Drawing.Point(45, 165);
            this.txtName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtName.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtName.Name = "txtName";
            this.txtName.Padding = new System.Windows.Forms.Padding(5);
            this.txtName.Radius = 8;
            this.txtName.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.txtName.ShowText = false;
            this.txtName.Size = new System.Drawing.Size(380, 40);
            this.txtName.TabIndex = 4;
            this.txtName.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtName.Watermark = " Nhập họ và tên của bạn";
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEmail.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.lblEmail.Location = new System.Drawing.Point(45, 220);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(73, 20);
            this.lblEmail.TabIndex = 5;
            this.lblEmail.Text = "✉ Email";
            // 
            // txtEmail
            // 
            this.txtEmail.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtEmail.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.txtEmail.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtEmail.Location = new System.Drawing.Point(45, 245);
            this.txtEmail.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtEmail.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Padding = new System.Windows.Forms.Padding(5);
            this.txtEmail.Radius = 8;
            this.txtEmail.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.txtEmail.ShowText = false;
            this.txtEmail.Size = new System.Drawing.Size(380, 40);
            this.txtEmail.TabIndex = 6;
            this.txtEmail.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtEmail.Watermark = " example@email.com";
            // 
            // lblPhone
            // 
            this.lblPhone.AutoSize = true;
            this.lblPhone.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblPhone.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.lblPhone.Location = new System.Drawing.Point(45, 300);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(126, 20);
            this.lblPhone.TabIndex = 7;
            this.lblPhone.Text = "📞 Số điện thoại";
            // 
            // txtPhone
            // 
            this.txtPhone.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPhone.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.txtPhone.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPhone.Location = new System.Drawing.Point(45, 325);
            this.txtPhone.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPhone.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Padding = new System.Windows.Forms.Padding(5);
            this.txtPhone.Radius = 8;
            this.txtPhone.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.txtPhone.ShowText = false;
            this.txtPhone.Size = new System.Drawing.Size(380, 40);
            this.txtPhone.TabIndex = 8;
            this.txtPhone.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtPhone.Watermark = " Nhập số điện thoại";
            // 
            // lblPass
            // 
            this.lblPass.AutoSize = true;
            this.lblPass.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblPass.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.lblPass.Location = new System.Drawing.Point(45, 380);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(101, 20);
            this.lblPass.TabIndex = 9;
            this.lblPass.Text = "🔒 Mật khẩu";
            // 
            // txtPass
            // 
            this.txtPass.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPass.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.txtPass.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtPass.Location = new System.Drawing.Point(45, 405);
            this.txtPass.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPass.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtPass.Name = "txtPass";
            this.txtPass.Padding = new System.Windows.Forms.Padding(5);
            this.txtPass.PasswordChar = '●';
            this.txtPass.Radius = 8;
            this.txtPass.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.txtPass.ShowText = false;
            this.txtPass.Size = new System.Drawing.Size(180, 40);
            this.txtPass.TabIndex = 10;
            this.txtPass.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtPass.Watermark = " ********";
            // 
            // lblConfirm
            // 
            this.lblConfirm.AutoSize = true;
            this.lblConfirm.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblConfirm.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.lblConfirm.Location = new System.Drawing.Point(245, 380);
            this.lblConfirm.Name = "lblConfirm";
            this.lblConfirm.Size = new System.Drawing.Size(169, 20);
            this.lblConfirm.TabIndex = 11;
            this.lblConfirm.Text = "✔️ Xác nhận mật khẩu";
            // 
            // txtConfirm
            // 
            this.txtConfirm.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtConfirm.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.txtConfirm.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtConfirm.Location = new System.Drawing.Point(245, 405);
            this.txtConfirm.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtConfirm.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtConfirm.Name = "txtConfirm";
            this.txtConfirm.Padding = new System.Windows.Forms.Padding(5);
            this.txtConfirm.PasswordChar = '●';
            this.txtConfirm.Radius = 8;
            this.txtConfirm.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.txtConfirm.ShowText = false;
            this.txtConfirm.Size = new System.Drawing.Size(180, 40);
            this.txtConfirm.TabIndex = 12;
            this.txtConfirm.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtConfirm.Watermark = " ********";
            // 
            // btnRegister
            // 
            this.btnRegister.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRegister.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(219)))), ((int)(((byte)(44)))));
            this.btnRegister.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(229)))), ((int)(((byte)(54)))));
            this.btnRegister.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnRegister.Location = new System.Drawing.Point(45, 480);
            this.btnRegister.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Radius = 8;
            this.btnRegister.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(219)))), ((int)(((byte)(44)))));
            this.btnRegister.Size = new System.Drawing.Size(380, 45);
            this.btnRegister.TabIndex = 13;
            this.btnRegister.Text = "👤 Đăng ký ngay";
            this.btnRegister.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // lblHasAcc
            // 
            this.lblHasAcc.AutoSize = true;
            this.lblHasAcc.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblHasAcc.ForeColor = System.Drawing.Color.Gray;
            this.lblHasAcc.Location = new System.Drawing.Point(69, 560);
            this.lblHasAcc.Name = "lblHasAcc";
            this.lblHasAcc.Size = new System.Drawing.Size(170, 23);
            this.lblHasAcc.TabIndex = 14;
            this.lblHasAcc.Text = "Bạn đã có tài khoản?";
            // 
            // lblLoginNow
            // 
            this.lblLoginNow.AutoSize = true;
            this.lblLoginNow.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblLoginNow.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblLoginNow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.lblLoginNow.Location = new System.Drawing.Point(245, 560);
            this.lblLoginNow.Name = "lblLoginNow";
            this.lblLoginNow.Size = new System.Drawing.Size(212, 23);
            this.lblLoginNow.TabIndex = 15;
            this.lblLoginNow.Text = "Quay lại Đăng nhập ngay";
            // 
            // picCourt
            // 
            this.picCourt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(253)))), ((int)(((byte)(242)))));
            this.picCourt.Image = global::DemoPick.Properties.Resources.pick;
            this.picCourt.Location = new System.Drawing.Point(50, 160);
            this.picCourt.Name = "picCourt";
            this.picCourt.Size = new System.Drawing.Size(320, 200);
            this.picCourt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picCourt.TabIndex = 0;
            this.picCourt.TabStop = false;
            // 
            // UCRegister
            // 
            this.Controls.Add(this.pnlRight);
            this.Controls.Add(this.pnlLeft);
            this.Name = "UCRegister";
            this.Size = new System.Drawing.Size(920, 620);
            this.pnlLeft.ResumeLayout(false);
            this.pnlLeft.PerformLayout();
            this.pnlRight.ResumeLayout(false);
            this.pnlRight.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCourt)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
