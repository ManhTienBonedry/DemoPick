namespace DemoPick.Views
{
    partial class FrmDoiCaBooking
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblHeaderTop;
        private System.Windows.Forms.Label btnCloseTop;

        private System.Windows.Forms.Label lblInfoHeader;
        private System.Windows.Forms.Label lblBookingId;
        internal System.Windows.Forms.Label lblBookingIdValue;
        private System.Windows.Forms.Label lblCourt;
        internal System.Windows.Forms.Label lblCourtValue;
        private System.Windows.Forms.Label lblGuest;
        internal System.Windows.Forms.Label lblGuestValue;
        private System.Windows.Forms.Label lblStatus;
        internal System.Windows.Forms.Label lblStatusValue;
        internal System.Windows.Forms.Label lblCurrent;

        private System.Windows.Forms.Label lblChangeHeader;
        private System.Windows.Forms.Label lblTime;
        internal System.Windows.Forms.ComboBox cbTime;
        private System.Windows.Forms.Label lblDuration;
        internal System.Windows.Forms.ComboBox cbDuration;

        private System.Windows.Forms.Label lblNote;
        internal Sunny.UI.UITextBox txtNote;

        internal Sunny.UI.UIButton btnOk;
        internal Sunny.UI.UIButton btnCancel;

        private System.Windows.Forms.Label lblHint;

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
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnCloseTop = new System.Windows.Forms.Label();
            this.lblHeaderTop = new System.Windows.Forms.Label();
            this.lblInfoHeader = new System.Windows.Forms.Label();
            this.lblBookingId = new System.Windows.Forms.Label();
            this.lblBookingIdValue = new System.Windows.Forms.Label();
            this.lblCourt = new System.Windows.Forms.Label();
            this.lblCourtValue = new System.Windows.Forms.Label();
            this.lblGuest = new System.Windows.Forms.Label();
            this.lblGuestValue = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblStatusValue = new System.Windows.Forms.Label();
            this.lblCurrent = new System.Windows.Forms.Label();
            this.lblChangeHeader = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.cbTime = new System.Windows.Forms.ComboBox();
            this.lblDuration = new System.Windows.Forms.Label();
            this.cbDuration = new System.Windows.Forms.ComboBox();
            this.lblNote = new System.Windows.Forms.Label();
            this.txtNote = new Sunny.UI.UITextBox();
            this.btnOk = new Sunny.UI.UIButton();
            this.btnCancel = new Sunny.UI.UIButton();
            this.lblHint = new System.Windows.Forms.Label();
            this.pnlTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.Chartreuse;
            this.pnlTop.Controls.Add(this.btnCloseTop);
            this.pnlTop.Controls.Add(this.lblHeaderTop);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(520, 50);
            this.pnlTop.TabIndex = 0;
            // 
            // btnCloseTop
            // 
            this.btnCloseTop.AutoSize = true;
            this.btnCloseTop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCloseTop.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCloseTop.ForeColor = System.Drawing.Color.White;
            this.btnCloseTop.Location = new System.Drawing.Point(490, 11);
            this.btnCloseTop.Name = "btnCloseTop";
            this.btnCloseTop.Size = new System.Drawing.Size(25, 28);
            this.btnCloseTop.TabIndex = 1;
            this.btnCloseTop.Text = "X";
            // 
            // lblHeaderTop
            // 
            this.lblHeaderTop.AutoSize = true;
            this.lblHeaderTop.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblHeaderTop.ForeColor = System.Drawing.Color.White;
            this.lblHeaderTop.Location = new System.Drawing.Point(18, 11);
            this.lblHeaderTop.Name = "lblHeaderTop";
            this.lblHeaderTop.Size = new System.Drawing.Size(170, 28);
            this.lblHeaderTop.TabIndex = 0;
            this.lblHeaderTop.Text = "ĐỔI CA / GIỜ CHƠI";
            // 
            // lblInfoHeader
            // 
            this.lblInfoHeader.AutoSize = true;
            this.lblInfoHeader.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblInfoHeader.ForeColor = System.Drawing.Color.DimGray;
            this.lblInfoHeader.Location = new System.Drawing.Point(18, 65);
            this.lblInfoHeader.Name = "lblInfoHeader";
            this.lblInfoHeader.Size = new System.Drawing.Size(196, 28);
            this.lblInfoHeader.TabIndex = 1;
            this.lblInfoHeader.Text = "THÔNG TIN BOOKING";
            // 
            // lblBookingId
            // 
            this.lblBookingId.AutoSize = true;
            this.lblBookingId.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblBookingId.Location = new System.Drawing.Point(20, 102);
            this.lblBookingId.Name = "lblBookingId";
            this.lblBookingId.Size = new System.Drawing.Size(97, 23);
            this.lblBookingId.TabIndex = 2;
            this.lblBookingId.Text = "Mã booking:";
            // 
            // lblBookingIdValue
            // 
            this.lblBookingIdValue.AutoSize = true;
            this.lblBookingIdValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblBookingIdValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblBookingIdValue.Location = new System.Drawing.Point(140, 102);
            this.lblBookingIdValue.Name = "lblBookingIdValue";
            this.lblBookingIdValue.Size = new System.Drawing.Size(30, 23);
            this.lblBookingIdValue.TabIndex = 3;
            this.lblBookingIdValue.Text = "-";
            // 
            // lblCourt
            // 
            this.lblCourt.AutoSize = true;
            this.lblCourt.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblCourt.Location = new System.Drawing.Point(20, 132);
            this.lblCourt.Name = "lblCourt";
            this.lblCourt.Size = new System.Drawing.Size(43, 23);
            this.lblCourt.TabIndex = 4;
            this.lblCourt.Text = "Sân:";
            // 
            // lblCourtValue
            // 
            this.lblCourtValue.AutoEllipsis = true;
            this.lblCourtValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblCourtValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblCourtValue.Location = new System.Drawing.Point(140, 132);
            this.lblCourtValue.Name = "lblCourtValue";
            this.lblCourtValue.Size = new System.Drawing.Size(360, 23);
            this.lblCourtValue.TabIndex = 5;
            this.lblCourtValue.Text = "-";
            // 
            // lblGuest
            // 
            this.lblGuest.AutoSize = true;
            this.lblGuest.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblGuest.Location = new System.Drawing.Point(20, 162);
            this.lblGuest.Name = "lblGuest";
            this.lblGuest.Size = new System.Drawing.Size(55, 23);
            this.lblGuest.TabIndex = 6;
            this.lblGuest.Text = "Khách:";
            // 
            // lblGuestValue
            // 
            this.lblGuestValue.AutoEllipsis = true;
            this.lblGuestValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblGuestValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblGuestValue.Location = new System.Drawing.Point(140, 162);
            this.lblGuestValue.Name = "lblGuestValue";
            this.lblGuestValue.Size = new System.Drawing.Size(360, 23);
            this.lblGuestValue.TabIndex = 7;
            this.lblGuestValue.Text = "-";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStatus.Location = new System.Drawing.Point(20, 192);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(87, 23);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Trạng thái:";
            // 
            // lblStatusValue
            // 
            this.lblStatusValue.AutoSize = true;
            this.lblStatusValue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatusValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblStatusValue.Location = new System.Drawing.Point(140, 192);
            this.lblStatusValue.Name = "lblStatusValue";
            this.lblStatusValue.Size = new System.Drawing.Size(30, 23);
            this.lblStatusValue.TabIndex = 9;
            this.lblStatusValue.Text = "-";
            // 
            // lblCurrent
            // 
            this.lblCurrent.AutoSize = true;
            this.lblCurrent.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblCurrent.ForeColor = System.Drawing.Color.Gray;
            this.lblCurrent.Location = new System.Drawing.Point(20, 222);
            this.lblCurrent.Name = "lblCurrent";
            this.lblCurrent.Size = new System.Drawing.Size(70, 21);
            this.lblCurrent.TabIndex = 10;
            this.lblCurrent.Text = "Hiện tại:";
            // 
            // lblChangeHeader
            // 
            this.lblChangeHeader.AutoSize = true;
            this.lblChangeHeader.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblChangeHeader.ForeColor = System.Drawing.Color.DimGray;
            this.lblChangeHeader.Location = new System.Drawing.Point(18, 252);
            this.lblChangeHeader.Name = "lblChangeHeader";
            this.lblChangeHeader.Size = new System.Drawing.Size(150, 28);
            this.lblChangeHeader.TabIndex = 11;
            this.lblChangeHeader.Text = "CHỌN GIỜ MỚI";
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblTime.Location = new System.Drawing.Point(20, 292);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(93, 21);
            this.lblTime.TabIndex = 12;
            this.lblTime.Text = "Giờ bắt đầu:";
            // 
            // cbTime
            // 
            this.cbTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTime.Font = new System.Drawing.Font("Segoe UI", 10F);
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
            this.cbTime.Location = new System.Drawing.Point(140, 288);
            this.cbTime.Name = "cbTime";
            this.cbTime.Size = new System.Drawing.Size(210, 31);
            this.cbTime.TabIndex = 13;
            // 
            // lblDuration
            // 
            this.lblDuration.AutoSize = true;
            this.lblDuration.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblDuration.Location = new System.Drawing.Point(300, 292);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(77, 21);
            this.lblDuration.TabIndex = 14;
            this.lblDuration.Text = "Thời lượng:";
            // 
            // cbDuration
            // 
            this.cbDuration.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDuration.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbDuration.FormattingEnabled = true;
            this.cbDuration.Items.AddRange(new object[] {
            "60 phút",
            "90 phút",
            "120 phút",
            "180 phút"});
            this.cbDuration.Location = new System.Drawing.Point(384, 288);
            this.cbDuration.Name = "cbDuration";
            this.cbDuration.Size = new System.Drawing.Size(116, 31);
            this.cbDuration.TabIndex = 15;
            // 
            // lblNote
            // 
            this.lblNote.AutoSize = true;
            this.lblNote.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblNote.Location = new System.Drawing.Point(20, 330);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(105, 21);
            this.lblNote.TabIndex = 19;
            this.lblNote.Text = "Ghi chú thêm:";
            // 
            // txtNote
            // 
            this.txtNote.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtNote.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNote.Location = new System.Drawing.Point(140, 324);
            this.txtNote.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtNote.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtNote.Name = "txtNote";
            this.txtNote.Padding = new System.Windows.Forms.Padding(5);
            this.txtNote.Radius = 10;
            this.txtNote.ShowText = false;
            this.txtNote.Size = new System.Drawing.Size(360, 36);
            this.txtNote.TabIndex = 20;
            this.txtNote.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtNote.Watermark = "Nhập ghi chú (nếu có)";
            // 
            // btnOk
            // 
            this.btnOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOk.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnOk.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(214)))), ((int)(((byte)(123)))));
            this.btnOk.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnOk.Location = new System.Drawing.Point(280, 382);
            this.btnOk.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnOk.Name = "btnOk";
            this.btnOk.Radius = 18;
            this.btnOk.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnOk.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(214)))), ((int)(((byte)(123)))));
            this.btnOk.Size = new System.Drawing.Size(110, 40);
            this.btnOk.TabIndex = 16;
            this.btnOk.Text = "Xác nhận";
            this.btnOk.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // btnCancel
            // 
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FillColor = System.Drawing.Color.White;
            this.btnCancel.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.btnCancel.Location = new System.Drawing.Point(398, 382);
            this.btnCancel.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Radius = 18;
            this.btnCancel.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.btnCancel.RectHoverColor = System.Drawing.Color.Gray;
            this.btnCancel.Size = new System.Drawing.Size(102, 40);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // lblHint
            // 
            this.lblHint.AutoSize = true;
            this.lblHint.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblHint.ForeColor = System.Drawing.Color.Gray;
            this.lblHint.Location = new System.Drawing.Point(20, 392);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(239, 20);
            this.lblHint.TabIndex = 18;
            this.lblHint.Text = "Mẹo: double-click booking để đổi ca.";
            // 
            // FrmDoiCaBooking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(520, 440);
            this.Controls.Add(this.lblHint);
            this.Controls.Add(this.txtNote);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.cbDuration);
            this.Controls.Add(this.lblDuration);
            this.Controls.Add(this.cbTime);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.lblChangeHeader);
            this.Controls.Add(this.lblCurrent);
            this.Controls.Add(this.lblStatusValue);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblGuestValue);
            this.Controls.Add(this.lblGuest);
            this.Controls.Add(this.lblCourtValue);
            this.Controls.Add(this.lblCourt);
            this.Controls.Add(this.lblBookingIdValue);
            this.Controls.Add(this.lblBookingId);
            this.Controls.Add(this.lblInfoHeader);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmDoiCaBooking";
            this.Text = "Đổi ca";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
