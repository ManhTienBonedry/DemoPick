using System.Drawing;
using System.Windows.Forms;
using Sunny.UI;

namespace DemoPick
{
    partial class FrmDatSanCoDinh
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlHeader;
        private Label lblTitle;
        private Label btnCancelTop;
        
        private UIRadioButton rbKhachThue;
        private UIRadioButton rbBaoTri;
        
        private Label lblName;
        private UITextBox txtName;
        private Label lblPhone;
        private UITextBox txtPhone;

        private Label lblNote;
        private UITextBox txtNote;
        
        private Label lblCourt;
        private UIComboBox cbCourt;
        private Label lblTime;
        private UIComboBox cbTime;
        private Label lblDuration;
        private UIComboBox cbDuration;
        
        private Label lblDateRange;
        private Label lblTo;

        private UCDateRangeFilter ucDateRange;
        
        private Label lblRepeat;
        private UICheckBox chkMon;
        private UICheckBox chkTue;
        private UICheckBox chkWed;
        private UICheckBox chkThu;
        private UICheckBox chkFri;
        private UICheckBox chkSat;
        private UICheckBox chkSun;
        
        private UIButton btnCancel;
        private UIButton btnConfirm;

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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnCancelTop = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.rbKhachThue = new Sunny.UI.UIRadioButton();
            this.rbBaoTri = new Sunny.UI.UIRadioButton();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new Sunny.UI.UITextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.txtPhone = new Sunny.UI.UITextBox();
            this.lblNote = new System.Windows.Forms.Label();
            this.txtNote = new Sunny.UI.UITextBox();
            this.lblCourt = new System.Windows.Forms.Label();
            this.cbCourt = new Sunny.UI.UIComboBox();
            this.lblTime = new System.Windows.Forms.Label();
            this.cbTime = new Sunny.UI.UIComboBox();
            this.lblDuration = new System.Windows.Forms.Label();
            this.cbDuration = new Sunny.UI.UIComboBox();
            this.lblDateRange = new System.Windows.Forms.Label();
            this.lblTo = new System.Windows.Forms.Label();
            this.lblRepeat = new System.Windows.Forms.Label();
            this.chkMon = new Sunny.UI.UICheckBox();
            this.chkTue = new Sunny.UI.UICheckBox();
            this.chkWed = new Sunny.UI.UICheckBox();
            this.chkThu = new Sunny.UI.UICheckBox();
            this.chkFri = new Sunny.UI.UICheckBox();
            this.chkSat = new Sunny.UI.UICheckBox();
            this.chkSun = new Sunny.UI.UICheckBox();
            this.btnCancel = new Sunny.UI.UIButton();
            this.btnConfirm = new Sunny.UI.UIButton();
            this.ucDateRange = new DemoPick.UCDateRangeFilter();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(253)))), ((int)(((byte)(242)))));
            this.pnlHeader.Controls.Add(this.btnCancelTop);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(700, 70);
            this.pnlHeader.TabIndex = 26;
            // 
            // btnCancelTop
            // 
            this.btnCancelTop.AutoSize = true;
            this.btnCancelTop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelTop.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCancelTop.ForeColor = System.Drawing.Color.Silver;
            this.btnCancelTop.Location = new System.Drawing.Point(650, 20);
            this.btnCancelTop.Name = "btnCancelTop";
            this.btnCancelTop.Size = new System.Drawing.Size(25, 28);
            this.btnCancelTop.TabIndex = 0;
            this.btnCancelTop.Text = "X";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(402, 37);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Lập Lịch Bảo Trì & Thuê Cố Định";
            // 
            // rbKhachThue
            // 
            this.rbKhachThue.Checked = true;
            this.rbKhachThue.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbKhachThue.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.rbKhachThue.Location = new System.Drawing.Point(30, 90);
            this.rbKhachThue.MinimumSize = new System.Drawing.Size(1, 1);
            this.rbKhachThue.Name = "rbKhachThue";
            this.rbKhachThue.RadioButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.rbKhachThue.Size = new System.Drawing.Size(180, 29);
            this.rbKhachThue.TabIndex = 25;
            this.rbKhachThue.Text = "Khách thuê";
            // 
            // rbBaoTri
            // 
            this.rbBaoTri.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbBaoTri.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.rbBaoTri.Location = new System.Drawing.Point(220, 90);
            this.rbBaoTri.MinimumSize = new System.Drawing.Size(1, 1);
            this.rbBaoTri.Name = "rbBaoTri";
            this.rbBaoTri.RadioButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.rbBaoTri.Size = new System.Drawing.Size(150, 29);
            this.rbBaoTri.TabIndex = 24;
            this.rbBaoTri.Text = "Bảo trì sân";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblName.Location = new System.Drawing.Point(30, 140);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(127, 23);
            this.lblName.TabIndex = 23;
            this.lblName.Text = "Khách đại diện:";
            // 
            // txtName
            // 
            this.txtName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtName.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtName.Location = new System.Drawing.Point(30, 165);
            this.txtName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtName.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtName.Name = "txtName";
            this.txtName.Padding = new System.Windows.Forms.Padding(5);
            this.txtName.Radius = 6;
            this.txtName.ShowText = false;
            this.txtName.Size = new System.Drawing.Size(300, 36);
            this.txtName.TabIndex = 22;
            this.txtName.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtName.Watermark = "Nhập họ tên";
            // 
            // lblPhone
            // 
            this.lblPhone.AutoSize = true;
            this.lblPhone.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPhone.Location = new System.Drawing.Point(30, 215);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(115, 23);
            this.lblPhone.TabIndex = 21;
            this.lblPhone.Text = "Số điện thoại:";
            // 
            // txtPhone
            // 
            this.txtPhone.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPhone.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtPhone.Location = new System.Drawing.Point(30, 240);
            this.txtPhone.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPhone.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Padding = new System.Windows.Forms.Padding(5);
            this.txtPhone.Radius = 6;
            this.txtPhone.ShowText = false;
            this.txtPhone.Size = new System.Drawing.Size(300, 36);
            this.txtPhone.TabIndex = 20;
            this.txtPhone.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtPhone.Watermark = "Nhập SĐT";
            // 
            // lblNote
            // 
            this.lblNote.AutoSize = true;
            this.lblNote.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNote.Location = new System.Drawing.Point(30, 280);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(118, 23);
            this.lblNote.TabIndex = 27;
            this.lblNote.Text = "Ghi chú thêm:";
            // 
            // txtNote
            // 
            this.txtNote.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtNote.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtNote.Location = new System.Drawing.Point(30, 305);
            this.txtNote.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtNote.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtNote.Name = "txtNote";
            this.txtNote.Padding = new System.Windows.Forms.Padding(5);
            this.txtNote.Radius = 6;
            this.txtNote.ShowText = false;
            this.txtNote.Size = new System.Drawing.Size(300, 36);
            this.txtNote.TabIndex = 26;
            this.txtNote.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtNote.Watermark = "Ví dụ: cố định 3 buổi/tuần, mang theo nước...";
            // 
            // lblCourt
            // 
            this.lblCourt.AutoSize = true;
            this.lblCourt.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblCourt.Location = new System.Drawing.Point(370, 95);
            this.lblCourt.Name = "lblCourt";
            this.lblCourt.Size = new System.Drawing.Size(127, 23);
            this.lblCourt.TabIndex = 15;
            this.lblCourt.Text = "Chọn Mục tiêu:";
            // 
            // cbCourt
            // 
            this.cbCourt.DataSource = null;
            this.cbCourt.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;
            this.cbCourt.FillColor = System.Drawing.Color.White;
            this.cbCourt.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.cbCourt.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.cbCourt.Items.AddRange(new object[] {
            "Sân số 1",
            "Sân số 2",
            "Sân số 3",
            "Sân VIP 1",
            "Sân VIP 2",
            "Tất Cả Các Sân"});
            this.cbCourt.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.cbCourt.Location = new System.Drawing.Point(370, 120);
            this.cbCourt.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbCourt.MinimumSize = new System.Drawing.Size(63, 0);
            this.cbCourt.Name = "cbCourt";
            this.cbCourt.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.cbCourt.Size = new System.Drawing.Size(300, 36);
            this.cbCourt.SymbolSize = 24;
            this.cbCourt.TabIndex = 14;
            this.cbCourt.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbCourt.Watermark = "";
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTime.Location = new System.Drawing.Point(370, 170);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(137, 23);
            this.lblTime.TabIndex = 13;
            this.lblTime.Text = "Ca bắt đầu chốt:";
            // 
            // cbTime
            // 
            this.cbTime.DataSource = null;
            this.cbTime.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;
            this.cbTime.FillColor = System.Drawing.Color.White;
            this.cbTime.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.cbTime.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.cbTime.Items.AddRange(new object[] {
            "06:00",
            "06:30",
            "07:00",
            "07:30",
            "08:00",
            "08:30",
            "09:00",
            "09:30",
            "10:00",
            "10:30",
            "11:00",
            "11:30",
            "12:00",
            "12:30",
            "13:00",
            "13:30",
            "14:00",
            "14:30",
            "15:00",
            "15:30",
            "16:00",
            "16:30",
            "17:00",
            "17:30",
            "18:00",
            "18:30",
            "19:00",
            "19:30",
            "20:00",
            "20:30",
            "21:00",
            "21:30",
            "22:00",
            "22:30",
            "23:00"});
            this.cbTime.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.cbTime.Location = new System.Drawing.Point(370, 195);
            this.cbTime.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbTime.MinimumSize = new System.Drawing.Size(63, 0);
            this.cbTime.Name = "cbTime";
            this.cbTime.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.cbTime.Size = new System.Drawing.Size(140, 36);
            this.cbTime.SymbolSize = 24;
            this.cbTime.TabIndex = 12;
            this.cbTime.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbTime.Watermark = "";
            // 
            // lblDuration
            // 
            this.lblDuration.AutoSize = true;
            this.lblDuration.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDuration.Location = new System.Drawing.Point(530, 170);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(96, 23);
            this.lblDuration.TabIndex = 11;
            this.lblDuration.Text = "Thời lượng:";
            // 
            // cbDuration
            // 
            this.cbDuration.DataSource = null;
            this.cbDuration.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;
            this.cbDuration.FillColor = System.Drawing.Color.White;
            this.cbDuration.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.cbDuration.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.cbDuration.Items.AddRange(new object[] {
            "60 phút",
            "90 phút",
            "120 phút",
            "Nguyên ngày"});
            this.cbDuration.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.cbDuration.Location = new System.Drawing.Point(530, 195);
            this.cbDuration.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbDuration.MinimumSize = new System.Drawing.Size(63, 0);
            this.cbDuration.Name = "cbDuration";
            this.cbDuration.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.cbDuration.Size = new System.Drawing.Size(140, 36);
            this.cbDuration.SymbolSize = 24;
            this.cbDuration.TabIndex = 10;
            this.cbDuration.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbDuration.Watermark = "";
            // 
            // lblDateRange
            // 
            this.lblDateRange.AutoSize = true;
            this.lblDateRange.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDateRange.Location = new System.Drawing.Point(30, 350);
            this.lblDateRange.Name = "lblDateRange";
            this.lblDateRange.Size = new System.Drawing.Size(148, 23);
            this.lblDateRange.TabIndex = 19;
            this.lblDateRange.Text = "Chu kỳ (Từ - Đến):";
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTo.Location = new System.Drawing.Point(165, 380);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(29, 23);
            this.lblTo.TabIndex = 17;
            this.lblTo.Text = "->";
            // 
            // lblRepeat
            // 
            this.lblRepeat.AutoSize = true;
            this.lblRepeat.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblRepeat.Location = new System.Drawing.Point(370, 250);
            this.lblRepeat.Name = "lblRepeat";
            this.lblRepeat.Size = new System.Drawing.Size(155, 23);
            this.lblRepeat.TabIndex = 9;
            this.lblRepeat.Text = "Áp dụng cho ngày:";
            // 
            // chkMon
            // 
            this.chkMon.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkMon.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkMon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.chkMon.Location = new System.Drawing.Point(370, 280);
            this.chkMon.MinimumSize = new System.Drawing.Size(1, 1);
            this.chkMon.Name = "chkMon";
            this.chkMon.Size = new System.Drawing.Size(80, 29);
            this.chkMon.TabIndex = 8;
            this.chkMon.Text = "Thứ 2";
            // 
            // chkTue
            // 
            this.chkTue.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkTue.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkTue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.chkTue.Location = new System.Drawing.Point(460, 280);
            this.chkTue.MinimumSize = new System.Drawing.Size(1, 1);
            this.chkTue.Name = "chkTue";
            this.chkTue.Size = new System.Drawing.Size(80, 29);
            this.chkTue.TabIndex = 7;
            this.chkTue.Text = "Thứ 3";
            // 
            // chkWed
            // 
            this.chkWed.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkWed.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkWed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.chkWed.Location = new System.Drawing.Point(550, 280);
            this.chkWed.MinimumSize = new System.Drawing.Size(1, 1);
            this.chkWed.Name = "chkWed";
            this.chkWed.Size = new System.Drawing.Size(80, 29);
            this.chkWed.TabIndex = 6;
            this.chkWed.Text = "Thứ 4";
            // 
            // chkThu
            // 
            this.chkThu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkThu.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkThu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.chkThu.Location = new System.Drawing.Point(370, 315);
            this.chkThu.MinimumSize = new System.Drawing.Size(1, 1);
            this.chkThu.Name = "chkThu";
            this.chkThu.Size = new System.Drawing.Size(80, 29);
            this.chkThu.TabIndex = 5;
            this.chkThu.Text = "Thứ 5";
            // 
            // chkFri
            // 
            this.chkFri.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkFri.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkFri.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.chkFri.Location = new System.Drawing.Point(460, 315);
            this.chkFri.MinimumSize = new System.Drawing.Size(1, 1);
            this.chkFri.Name = "chkFri";
            this.chkFri.Size = new System.Drawing.Size(80, 29);
            this.chkFri.TabIndex = 4;
            this.chkFri.Text = "Thứ 6";
            // 
            // chkSat
            // 
            this.chkSat.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkSat.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkSat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.chkSat.Location = new System.Drawing.Point(550, 315);
            this.chkSat.MinimumSize = new System.Drawing.Size(1, 1);
            this.chkSat.Name = "chkSat";
            this.chkSat.Size = new System.Drawing.Size(80, 29);
            this.chkSat.TabIndex = 3;
            this.chkSat.Text = "Thứ 7";
            // 
            // chkSun
            // 
            this.chkSun.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkSun.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkSun.ForeColor = System.Drawing.Color.Red;
            this.chkSun.Location = new System.Drawing.Point(370, 350);
            this.chkSun.MinimumSize = new System.Drawing.Size(1, 1);
            this.chkSun.Name = "chkSun";
            this.chkSun.Size = new System.Drawing.Size(120, 29);
            this.chkSun.TabIndex = 2;
            this.chkSun.Text = "Chủ Nhật";
            // 
            // btnCancel
            // 
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FillColor = System.Drawing.Color.White;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.btnCancel.ForeHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.btnCancel.Location = new System.Drawing.Point(30, 420);
            this.btnCancel.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Radius = 8;
            this.btnCancel.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnCancel.Size = new System.Drawing.Size(200, 45);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Hủy bỏ";
            this.btnCancel.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirm.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(219)))), ((int)(((byte)(44)))));
            this.btnConfirm.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(214)))), ((int)(((byte)(123)))));
            this.btnConfirm.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnConfirm.Location = new System.Drawing.Point(250, 420);
            this.btnConfirm.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Radius = 8;
            this.btnConfirm.RectColor = System.Drawing.Color.Transparent;
            this.btnConfirm.Size = new System.Drawing.Size(420, 45);
            this.btnConfirm.TabIndex = 0;
            this.btnConfirm.Text = "Tạo Lịch Cố Định";
            this.btnConfirm.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // ucDateRange
            // 
            this.ucDateRange.Location = new System.Drawing.Point(30, 372);
            this.ucDateRange.Mode = DemoPick.UCDateRangeFilter.DateFilterMode.Range;
            this.ucDateRange.Name = "ucDateRange";
            this.ucDateRange.ShowApplyButton = false;
            this.ucDateRange.Size = new System.Drawing.Size(420, 39);
            this.ucDateRange.TabIndex = 28;
            // 
            // FrmDatSanCoDinh
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(700, 500);
            this.Controls.Add(this.ucDateRange);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.chkSun);
            this.Controls.Add(this.chkSat);
            this.Controls.Add(this.chkFri);
            this.Controls.Add(this.chkThu);
            this.Controls.Add(this.chkWed);
            this.Controls.Add(this.chkTue);
            this.Controls.Add(this.chkMon);
            this.Controls.Add(this.lblRepeat);
            this.Controls.Add(this.cbDuration);
            this.Controls.Add(this.lblDuration);
            this.Controls.Add(this.cbTime);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.cbCourt);
            this.Controls.Add(this.lblCourt);
            this.Controls.Add(this.lblTo);
            this.Controls.Add(this.lblDateRange);
            this.Controls.Add(this.txtNote);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(this.lblPhone);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.rbBaoTri);
            this.Controls.Add(this.rbKhachThue);
            this.Controls.Add(this.pnlHeader);
            this.Name = "FrmDatSanCoDinh";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        
    }
}
