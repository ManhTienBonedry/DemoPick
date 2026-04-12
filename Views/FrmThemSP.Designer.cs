using System.Drawing;
using System.Windows.Forms;

namespace DemoPick
{
    partial class FrmThemSP
    {
        private System.ComponentModel.IContainer components = null;
        private Sunny.UI.UILabel lblHeader, lblTen, lblSKU, lblGia, lblSoLuong, lblLoai;
        private Sunny.UI.UITextBox txtTen, txtSKU, txtGia, txtSoLuong;
        private Sunny.UI.UIComboBox cboLoai;
        private Sunny.UI.UIButton btnLuu, btnDong;

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
            this.lblHeader = new Sunny.UI.UILabel();
            this.lblTen = new Sunny.UI.UILabel();
            this.txtTen = new Sunny.UI.UITextBox();
            this.lblLoai = new Sunny.UI.UILabel();
            this.cboLoai = new Sunny.UI.UIComboBox();
            this.lblGia = new Sunny.UI.UILabel();
            this.txtGia = new Sunny.UI.UITextBox();
            this.lblSoLuong = new Sunny.UI.UILabel();
            this.txtSoLuong = new Sunny.UI.UITextBox();
            this.lblSKU = new Sunny.UI.UILabel();
            this.txtSKU = new Sunny.UI.UITextBox();
            this.btnLuu = new Sunny.UI.UIButton();
            this.btnDong = new Sunny.UI.UIButton();
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.lblHeader.Location = new System.Drawing.Point(30, 45);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(400, 30);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "THÊM MỚI SẢN PHẨM";
            // 
            // lblTen
            // 
            this.lblTen.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTen.ForeColor = System.Drawing.Color.Gray;
            this.lblTen.Location = new System.Drawing.Point(30, 95);
            this.lblTen.Name = "lblTen";
            this.lblTen.Size = new System.Drawing.Size(200, 25);
            this.lblTen.TabIndex = 1;
            this.lblTen.Text = "Tên mặt hàng/Dịch vụ (*):";
            // 
            // txtTen
            // 
            this.txtTen.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtTen.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtTen.Location = new System.Drawing.Point(30, 125);
            this.txtTen.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtTen.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtTen.Name = "txtTen";
            this.txtTen.Padding = new System.Windows.Forms.Padding(5);
            this.txtTen.ShowText = false;
            this.txtTen.Size = new System.Drawing.Size(390, 34);
            this.txtTen.TabIndex = 2;
            this.txtTen.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtTen.Watermark = "";
            // 
            // lblLoai
            // 
            this.lblLoai.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblLoai.ForeColor = System.Drawing.Color.Gray;
            this.lblLoai.Location = new System.Drawing.Point(30, 175);
            this.lblLoai.Name = "lblLoai";
            this.lblLoai.Size = new System.Drawing.Size(200, 25);
            this.lblLoai.TabIndex = 3;
            this.lblLoai.Text = "Danh mục Phân loại (*):";
            // 
            // cboLoai
            // 
            this.cboLoai.DataSource = null;
            this.cboLoai.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;
            this.cboLoai.FillColor = System.Drawing.Color.White;
            this.cboLoai.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cboLoai.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.cboLoai.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.cboLoai.Location = new System.Drawing.Point(30, 205);
            this.cboLoai.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cboLoai.MinimumSize = new System.Drawing.Size(63, 0);
            this.cboLoai.Name = "cboLoai";
            this.cboLoai.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.cboLoai.Size = new System.Drawing.Size(390, 34);
            this.cboLoai.SymbolSize = 24;
            this.cboLoai.TabIndex = 4;
            this.cboLoai.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.cboLoai.Watermark = "";
            // 
            // lblGia
            // 
            this.lblGia.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblGia.ForeColor = System.Drawing.Color.Gray;
            this.lblGia.Location = new System.Drawing.Point(30, 255);
            this.lblGia.Name = "lblGia";
            this.lblGia.Size = new System.Drawing.Size(180, 25);
            this.lblGia.TabIndex = 5;
            this.lblGia.Text = "Đơn Giá Bán Thực (VND):";
            // 
            // txtGia
            // 
            this.txtGia.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtGia.DoubleValue = 15000D;
            this.txtGia.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtGia.IntValue = 15000;
            this.txtGia.Location = new System.Drawing.Point(30, 285);
            this.txtGia.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtGia.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtGia.Name = "txtGia";
            this.txtGia.Padding = new System.Windows.Forms.Padding(5);
            this.txtGia.ShowText = false;
            this.txtGia.Size = new System.Drawing.Size(180, 34);
            this.txtGia.TabIndex = 6;
            this.txtGia.Text = "15000";
            this.txtGia.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtGia.Watermark = "";
            // 
            // lblSoLuong
            // 
            this.lblSoLuong.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSoLuong.ForeColor = System.Drawing.Color.Gray;
            this.lblSoLuong.Location = new System.Drawing.Point(240, 255);
            this.lblSoLuong.Name = "lblSoLuong";
            this.lblSoLuong.Size = new System.Drawing.Size(180, 25);
            this.lblSoLuong.TabIndex = 7;
            this.lblSoLuong.Text = "Tồn Kho Hiện Tại:";
            // 
            // txtSoLuong
            // 
            this.txtSoLuong.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSoLuong.DoubleValue = 50D;
            this.txtSoLuong.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtSoLuong.IntValue = 50;
            this.txtSoLuong.Location = new System.Drawing.Point(240, 285);
            this.txtSoLuong.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSoLuong.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtSoLuong.Name = "txtSoLuong";
            this.txtSoLuong.Padding = new System.Windows.Forms.Padding(5);
            this.txtSoLuong.ShowText = false;
            this.txtSoLuong.Size = new System.Drawing.Size(180, 34);
            this.txtSoLuong.TabIndex = 8;
            this.txtSoLuong.Text = "50";
            this.txtSoLuong.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtSoLuong.Watermark = "";
            // 
            // lblSKU
            // 
            this.lblSKU.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSKU.ForeColor = System.Drawing.Color.Gray;
            this.lblSKU.Location = new System.Drawing.Point(30, 335);
            this.lblSKU.Name = "lblSKU";
            this.lblSKU.Size = new System.Drawing.Size(390, 25);
            this.lblSKU.TabIndex = 9;
            this.lblSKU.Text = "Mã Quét Quản Lý Đầu Cuối (SKU):";
            this.lblSKU.Visible = false;
            // 
            // txtSKU
            // 
            this.txtSKU.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSKU.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtSKU.Location = new System.Drawing.Point(30, 365);
            this.txtSKU.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtSKU.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtSKU.Name = "txtSKU";
            this.txtSKU.Padding = new System.Windows.Forms.Padding(5);
            this.txtSKU.ShowText = false;
            this.txtSKU.Size = new System.Drawing.Size(390, 34);
            this.txtSKU.TabIndex = 10;
            this.txtSKU.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtSKU.Watermark = "";
            this.txtSKU.Visible = false;
            // 
            // btnLuu
            // 
            this.btnLuu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLuu.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(219)))), ((int)(((byte)(44)))));
            this.btnLuu.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnLuu.Location = new System.Drawing.Point(250, 345);
            this.btnLuu.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.Size = new System.Drawing.Size(170, 45);
            this.btnLuu.TabIndex = 11;
            this.btnLuu.Text = "LƯU VÀO KHO";
            this.btnLuu.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // btnDong
            // 
            this.btnDong.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDong.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(219)))), ((int)(((byte)(44)))));
            this.btnDong.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDong.Location = new System.Drawing.Point(130, 345);
            this.btnDong.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnDong.Name = "btnDong";
            this.btnDong.Size = new System.Drawing.Size(110, 45);
            this.btnDong.TabIndex = 12;
            this.btnDong.Text = "HỦY";
            this.btnDong.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // FrmThemSP
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(450, 420);
            this.ControlBoxFillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Controls.Add(this.btnDong);
            this.Controls.Add(this.btnLuu);
            this.Controls.Add(this.txtSKU);
            this.Controls.Add(this.lblSKU);
            this.Controls.Add(this.txtSoLuong);
            this.Controls.Add(this.lblSoLuong);
            this.Controls.Add(this.txtGia);
            this.Controls.Add(this.lblGia);
            this.Controls.Add(this.cboLoai);
            this.Controls.Add(this.lblLoai);
            this.Controls.Add(this.txtTen);
            this.Controls.Add(this.lblTen);
            this.Controls.Add(this.lblHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.Name = "FrmThemSP";
            this.RectColor = System.Drawing.Color.White;
            this.Text = "Nhập Kho";
            this.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(219)))), ((int)(((byte)(44)))));
            this.ZoomScaleRect = new System.Drawing.Rectangle(19, 19, 450, 420);
            this.ResumeLayout(false);

        }
    }
}
