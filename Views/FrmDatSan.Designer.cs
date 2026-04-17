using System.Drawing;
using System.Windows.Forms;
using Sunny.UI;

namespace DemoPick
{
    partial class FrmDatSan
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel pnlTop;
        private Label lblHeaderTop;
        private Label btnCancelTop;

        private Label lblCustomerInfo;
        private Label lblCustomerName;
        private UITextBox txtName;
        private Label lblCustomerPhone;
        private UITextBox txtPhone;

        private Label lblCourtInfo;
        private Label lblCourt;
        private ComboBox cbCourt;
        private Label lblDate;
        private UCDateRangeFilter ucDate;
        private Label lblTime;
        private ComboBox cbTime;
        private Label lblDuration;
        private ComboBox cbDuration;
        private Label lblPayment;
        private ComboBox cbPayment;

        private Label lblNote;
        private UITextBox txtNote;

        private UIButton btnConfirm;
        private UIButton btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnCancelTop = new System.Windows.Forms.Label();
            this.lblHeaderTop = new System.Windows.Forms.Label();
            this.lblCustomerInfo = new System.Windows.Forms.Label();
            this.lblCustomerName = new System.Windows.Forms.Label();
            this.txtName = new Sunny.UI.UITextBox();
            this.lblCustomerPhone = new System.Windows.Forms.Label();
            this.txtPhone = new Sunny.UI.UITextBox();
            this.lblCourtInfo = new System.Windows.Forms.Label();
            this.lblCourt = new System.Windows.Forms.Label();
            this.cbCourt = new System.Windows.Forms.ComboBox();
            this.lblDate = new System.Windows.Forms.Label();
            this.ucDate = new DemoPick.UCDateRangeFilter();
            this.lblTime = new System.Windows.Forms.Label();
            this.cbTime = new System.Windows.Forms.ComboBox();
            this.lblDuration = new System.Windows.Forms.Label();
            this.cbDuration = new System.Windows.Forms.ComboBox();
            this.lblPayment = new System.Windows.Forms.Label();
            this.cbPayment = new System.Windows.Forms.ComboBox();
            this.lblNote = new System.Windows.Forms.Label();
            this.txtNote = new Sunny.UI.UITextBox();
            this.btnConfirm = new Sunny.UI.UIButton();
            this.btnCancel = new Sunny.UI.UIButton();
            this.pnlTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(219)))), ((int)(((byte)(44)))));
            this.pnlTop.Controls.Add(this.btnCancelTop);
            this.pnlTop.Controls.Add(this.lblHeaderTop);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(480, 50);
            this.pnlTop.TabIndex = 21;
            // 
            // btnCancelTop
            // 
            this.btnCancelTop.AutoSize = true;
            this.btnCancelTop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelTop.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCancelTop.ForeColor = System.Drawing.Color.White;
            this.btnCancelTop.Location = new System.Drawing.Point(445, 12);
            this.btnCancelTop.Name = "btnCancelTop";
            this.btnCancelTop.Size = new System.Drawing.Size(25, 28);
            this.btnCancelTop.TabIndex = 1;
            this.btnCancelTop.Text = "X";
            // 
            // lblHeaderTop
            // 
            this.lblHeaderTop.AutoSize = true;
            this.lblHeaderTop.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblHeaderTop.Location = new System.Drawing.Point(20, 12);
            this.lblHeaderTop.Name = "lblHeaderTop";
            this.lblHeaderTop.Size = new System.Drawing.Size(182, 28);
            this.lblHeaderTop.TabIndex = 0;
            this.lblHeaderTop.Text = "ĐẶT SÂN NHANH";
            // 
            // lblCustomerInfo
            // 
            this.lblCustomerInfo.AutoSize = true;
            this.lblCustomerInfo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblCustomerInfo.ForeColor = System.Drawing.Color.DimGray;
            this.lblCustomerInfo.Location = new System.Drawing.Point(20, 70);
            this.lblCustomerInfo.Name = "lblCustomerInfo";
            this.lblCustomerInfo.Size = new System.Drawing.Size(264, 28);
            this.lblCustomerInfo.TabIndex = 20;
            this.lblCustomerInfo.Text = "THÔNG TIN KHÁCH HÀNG";
            // 
            // lblCustomerName
            // 
            this.lblCustomerName.AutoSize = true;
            this.lblCustomerName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblCustomerName.Location = new System.Drawing.Point(20, 110);
            this.lblCustomerName.Name = "lblCustomerName";
            this.lblCustomerName.Size = new System.Drawing.Size(116, 23);
            this.lblCustomerName.TabIndex = 19;
            this.lblCustomerName.Text = "Họ tên khách:";
            // 
            // txtName
            // 
            this.txtName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtName.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtName.Location = new System.Drawing.Point(20, 135);
            this.txtName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtName.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtName.Name = "txtName";
            this.txtName.Padding = new System.Windows.Forms.Padding(5);
            this.txtName.Radius = 18;
            this.txtName.RectColor = System.Drawing.Color.LightGray;
            this.txtName.ShowText = false;
            this.txtName.Size = new System.Drawing.Size(210, 41);
            this.txtName.TabIndex = 0;
            this.txtName.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtName.Watermark = "Nhập tên khách";
            // 
            // lblCustomerPhone
            // 
            this.lblCustomerPhone.AutoSize = true;
            this.lblCustomerPhone.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblCustomerPhone.Location = new System.Drawing.Point(250, 110);
            this.lblCustomerPhone.Name = "lblCustomerPhone";
            this.lblCustomerPhone.Size = new System.Drawing.Size(115, 23);
            this.lblCustomerPhone.TabIndex = 18;
            this.lblCustomerPhone.Text = "Số điện thoại:";
            // 
            // txtPhone
            // 
            this.txtPhone.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPhone.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtPhone.Location = new System.Drawing.Point(250, 135);
            this.txtPhone.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPhone.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Padding = new System.Windows.Forms.Padding(5);
            this.txtPhone.Radius = 18;
            this.txtPhone.RectColor = System.Drawing.Color.LightGray;
            this.txtPhone.ShowText = false;
            this.txtPhone.Size = new System.Drawing.Size(210, 41);
            this.txtPhone.TabIndex = 1;
            this.txtPhone.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtPhone.Watermark = "Số đt khách hàng";
            // 
            // lblCourtInfo
            // 
            this.lblCourtInfo.AutoSize = true;
            this.lblCourtInfo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblCourtInfo.ForeColor = System.Drawing.Color.DimGray;
            this.lblCourtInfo.Location = new System.Drawing.Point(20, 195);
            this.lblCourtInfo.Name = "lblCourtInfo";
            this.lblCourtInfo.Size = new System.Drawing.Size(218, 28);
            this.lblCourtInfo.TabIndex = 17;
            this.lblCourtInfo.Text = "THÔNG TIN ĐẶT SÂN";
            // 
            // lblCourt
            // 
            this.lblCourt.AutoSize = true;
            this.lblCourt.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblCourt.Location = new System.Drawing.Point(20, 230);
            this.lblCourt.Name = "lblCourt";
            this.lblCourt.Size = new System.Drawing.Size(86, 23);
            this.lblCourt.TabIndex = 16;
            this.lblCourt.Text = "Chọn sân:";
            // 
            // cbCourt
            // 
            this.cbCourt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCourt.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.cbCourt.FormattingEnabled = true;
            this.cbCourt.Items.AddRange(new object[] {
            "Sân số 1 (Tiêu chuẩn)",
            "Sân số 2 (Tiêu chuẩn)",
            "Sân số 3 (Tiêu chuẩn)",
            "Sân VIP 1 (Có mái che)",
            "Sân VIP 2 (Có mái che)"});
            this.cbCourt.Location = new System.Drawing.Point(20, 255);
            this.cbCourt.Name = "cbCourt";
            this.cbCourt.Size = new System.Drawing.Size(440, 36);
            this.cbCourt.TabIndex = 2;
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDate.Location = new System.Drawing.Point(20, 310);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(91, 23);
            this.lblDate.TabIndex = 15;
            this.lblDate.Text = "Ngày chơi:";
            // 
            // ucDate
            // 
            this.ucDate.Location = new System.Drawing.Point(20, 332);
            this.ucDate.Mode = DemoPick.UCDateRangeFilter.DateFilterMode.SingleDate;
            this.ucDate.Name = "ucDate";
            this.ucDate.ShowApplyButton = true;
            this.ucDate.Size = new System.Drawing.Size(260, 39);
            this.ucDate.TabIndex = 3;
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTime.Location = new System.Drawing.Point(290, 310);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(104, 23);
            this.lblTime.TabIndex = 14;
            this.lblTime.Text = "Giờ bắt đầu:";
            // 
            // cbTime
            // 
            this.cbTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTime.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.cbTime.FormattingEnabled = true;
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
            this.cbTime.Location = new System.Drawing.Point(290, 335);
            this.cbTime.Name = "cbTime";
            this.cbTime.Size = new System.Drawing.Size(170, 36);
            this.cbTime.TabIndex = 4;
            // 
            // lblDuration
            // 
            this.lblDuration.AutoSize = true;
            this.lblDuration.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDuration.Location = new System.Drawing.Point(20, 390);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(96, 23);
            this.lblDuration.TabIndex = 13;
            this.lblDuration.Text = "Thời lượng:";
            // 
            // cbDuration
            // 
            this.cbDuration.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDuration.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.cbDuration.FormattingEnabled = true;
            this.cbDuration.Items.AddRange(new object[] {
            "60 phút",
            "90 phút",
            "120 phút",
            "150 phút",
            "180 phút"});
            this.cbDuration.Location = new System.Drawing.Point(20, 415);
            this.cbDuration.Name = "cbDuration";
            this.cbDuration.Size = new System.Drawing.Size(210, 36);
            this.cbDuration.TabIndex = 5;
            // 
            // lblPayment
            // 
            this.lblPayment.AutoSize = true;
            this.lblPayment.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPayment.Location = new System.Drawing.Point(250, 390);
            this.lblPayment.Name = "lblPayment";
            this.lblPayment.Size = new System.Drawing.Size(183, 23);
            this.lblPayment.TabIndex = 12;
            this.lblPayment.Text = "Tình trạng thanh toán:";
            // 
            // cbPayment
            // 
            this.cbPayment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPayment.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.cbPayment.FormattingEnabled = true;
            this.cbPayment.Items.AddRange(new object[] {
            "Thu trực tiếp tại sân",
            "Đã chuyển khoản",
            "Đã đặt cọc 50%"});
            this.cbPayment.Location = new System.Drawing.Point(250, 415);
            this.cbPayment.Name = "cbPayment";
            this.cbPayment.Size = new System.Drawing.Size(210, 36);
            this.cbPayment.TabIndex = 6;
            // 
            // lblNote
            // 
            this.lblNote.AutoSize = true;
            this.lblNote.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblNote.Location = new System.Drawing.Point(20, 470);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(118, 23);
            this.lblNote.TabIndex = 11;
            this.lblNote.Text = "Ghi chú thêm:";
            // 
            // txtNote
            // 
            this.txtNote.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtNote.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtNote.Location = new System.Drawing.Point(20, 495);
            this.txtNote.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtNote.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtNote.Name = "txtNote";
            this.txtNote.Padding = new System.Windows.Forms.Padding(5);
            this.txtNote.Radius = 18;
            this.txtNote.RectColor = System.Drawing.Color.LightGray;
            this.txtNote.ShowText = false;
            this.txtNote.Size = new System.Drawing.Size(440, 41);
            this.txtNote.TabIndex = 8;
            this.txtNote.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtNote.Watermark = "Ví dụ: Lấy 2 bình trà đá, thuê 4 vợt...";
            // 
            // btnConfirm
            // 
            this.btnConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirm.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(219)))), ((int)(((byte)(44)))));
            this.btnConfirm.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(214)))), ((int)(((byte)(123)))));
            this.btnConfirm.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnConfirm.Location = new System.Drawing.Point(250, 560);
            this.btnConfirm.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Radius = 18;
            this.btnConfirm.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnConfirm.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(214)))), ((int)(((byte)(123)))));
            this.btnConfirm.Size = new System.Drawing.Size(210, 45);
            this.btnConfirm.TabIndex = 10;
            this.btnConfirm.Text = "CHỐT SÂN";
            this.btnConfirm.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // btnCancel
            // 
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FillColor = System.Drawing.Color.White;
            this.btnCancel.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.Gray;
            this.btnCancel.Location = new System.Drawing.Point(20, 560);
            this.btnCancel.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Radius = 18;
            this.btnCancel.RectColor = System.Drawing.Color.LightGray;
            this.btnCancel.RectHoverColor = System.Drawing.Color.Gray;
            this.btnCancel.Size = new System.Drawing.Size(210, 45);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "ĐÓNG LẠI";
            this.btnCancel.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // FrmDatSan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(480, 630);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.txtNote);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.cbPayment);
            this.Controls.Add(this.lblPayment);
            this.Controls.Add(this.cbDuration);
            this.Controls.Add(this.lblDuration);
            this.Controls.Add(this.cbTime);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.ucDate);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.cbCourt);
            this.Controls.Add(this.lblCourt);
            this.Controls.Add(this.lblCourtInfo);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(this.lblCustomerPhone);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblCustomerName);
            this.Controls.Add(this.lblCustomerInfo);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmDatSan";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Đặt Sân Nhanh";
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
