namespace DemoPick
{
    partial class UCDateRangeFilter
    {
        private System.ComponentModel.IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCDateRangeFilter));
            this.lblFrom = new System.Windows.Forms.Label();
            this.lblTo = new System.Windows.Forms.Label();
            this.btnApply = new Sunny.UI.UIButton();
            this.btnPrevDay = new Sunny.UI.UIButton();
            this.btnNextDay = new Sunny.UI.UIButton();
            this.dtFrom = new CuoreUI.Controls.cuiCalendarDatePicker();
            this.dtTo = new CuoreUI.Controls.cuiCalendarDatePicker();
            this.SuspendLayout();
            // 
            // lblFrom
            // 
            this.lblFrom.AutoSize = true;
            this.lblFrom.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblFrom.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblFrom.Location = new System.Drawing.Point(20, 12);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(28, 20);
            this.lblFrom.TabIndex = 100;
            this.lblFrom.Text = "Từ";
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblTo.Location = new System.Drawing.Point(215, 12);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(37, 20);
            this.lblTo.TabIndex = 102;
            this.lblTo.Text = "Đến";
            // 
            // btnApply
            // 
            this.btnApply.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnApply.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.btnApply.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(86)))), ((int)(((byte)(185)))), ((int)(((byte)(90)))));
            this.btnApply.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnApply.Location = new System.Drawing.Point(425, 5);
            this.btnApply.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnApply.Name = "btnApply";
            this.btnApply.Radius = 8;
            this.btnApply.RectColor = System.Drawing.Color.Transparent;
            this.btnApply.Size = new System.Drawing.Size(110, 32);
            this.btnApply.TabIndex = 104;
            this.btnApply.Text = "Áp dụng";
            this.btnApply.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // btnPrevDay
            // 
            this.btnPrevDay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPrevDay.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.btnPrevDay.FillHoverColor = System.Drawing.Color.LightGray;
            this.btnPrevDay.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnPrevDay.ForeColor = System.Drawing.Color.DimGray;
            this.btnPrevDay.Location = new System.Drawing.Point(0, 2);
            this.btnPrevDay.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnPrevDay.Name = "btnPrevDay";
            this.btnPrevDay.Radius = 18;
            this.btnPrevDay.RectColor = System.Drawing.Color.Transparent;
            this.btnPrevDay.RectHoverColor = System.Drawing.Color.Transparent;
            this.btnPrevDay.Size = new System.Drawing.Size(40, 35);
            this.btnPrevDay.TabIndex = 1;
            this.btnPrevDay.Text = "<";
            this.btnPrevDay.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // btnNextDay
            // 
            this.btnNextDay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNextDay.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.btnNextDay.FillHoverColor = System.Drawing.Color.LightGray;
            this.btnNextDay.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnNextDay.ForeColor = System.Drawing.Color.DimGray;
            this.btnNextDay.Location = new System.Drawing.Point(46, 2);
            this.btnNextDay.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnNextDay.Name = "btnNextDay";
            this.btnNextDay.Radius = 18;
            this.btnNextDay.RectColor = System.Drawing.Color.Transparent;
            this.btnNextDay.RectHoverColor = System.Drawing.Color.Transparent;
            this.btnNextDay.Size = new System.Drawing.Size(40, 35);
            this.btnNextDay.TabIndex = 2;
            this.btnNextDay.Text = ">";
            this.btnNextDay.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            // 
            // dtFrom
            // 
            this.dtFrom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dtFrom.Content = new System.DateTime(2026, 4, 9, 0, 0, 0, 0);
            this.dtFrom.EnableThemeChangeButton = true;
            this.dtFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.dtFrom.ForeColor = System.Drawing.Color.Gray;
            this.dtFrom.HoverBackground = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dtFrom.HoverOutline = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dtFrom.Icon = ((System.Drawing.Image)(resources.GetObject("dtFrom.Icon")));
            this.dtFrom.IconTint = System.Drawing.Color.Gray;
            this.dtFrom.Location = new System.Drawing.Point(55, 5);
            this.dtFrom.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.NormalBackground = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dtFrom.NormalOutline = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dtFrom.OutlineThickness = 1.5F;
            this.dtFrom.PickerPosition = CuoreUI.Controls.cuiCalendarDatePicker.Position.Bottom;
            this.dtFrom.PressedBackground = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dtFrom.PressedOutline = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dtFrom.Rounding = 8;
            this.dtFrom.ShowIcon = true;
            this.dtFrom.Size = new System.Drawing.Size(152, 32);
            this.dtFrom.TabIndex = 3;
            this.dtFrom.Theme = CuoreUI.Controls.Forms.DatePicker.Themes.Light;
            // 
            // dtTo
            // 
            this.dtTo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dtTo.Content = new System.DateTime(2026, 4, 9, 0, 0, 0, 0);
            this.dtTo.EnableThemeChangeButton = true;
            this.dtTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.dtTo.ForeColor = System.Drawing.Color.Gray;
            this.dtTo.HoverBackground = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dtTo.HoverOutline = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dtTo.Icon = ((System.Drawing.Image)(resources.GetObject("dtTo.Icon")));
            this.dtTo.IconTint = System.Drawing.Color.Gray;
            this.dtTo.Location = new System.Drawing.Point(259, 5);
            this.dtTo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dtTo.Name = "dtTo";
            this.dtTo.NormalBackground = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dtTo.NormalOutline = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dtTo.OutlineThickness = 1.5F;
            this.dtTo.PickerPosition = CuoreUI.Controls.cuiCalendarDatePicker.Position.Bottom;
            this.dtTo.PressedBackground = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dtTo.PressedOutline = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dtTo.Rounding = 8;
            this.dtTo.ShowIcon = true;
            this.dtTo.Size = new System.Drawing.Size(152, 32);
            this.dtTo.TabIndex = 105;
            this.dtTo.Theme = CuoreUI.Controls.Forms.DatePicker.Themes.Light;
            // 
            // UCDateRangeFilter
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.dtTo);
            this.Controls.Add(this.dtFrom);
            this.Controls.Add(this.btnNextDay);
            this.Controls.Add(this.btnPrevDay);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.lblTo);
            this.Controls.Add(this.lblFrom);
            this.Name = "UCDateRangeFilter";
            this.Size = new System.Drawing.Size(540, 39);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.Label lblTo;
        private Sunny.UI.UIButton btnApply;
        private Sunny.UI.UIButton btnPrevDay;
        private Sunny.UI.UIButton btnNextDay;
        private CuoreUI.Controls.cuiCalendarDatePicker dtFrom;
        private CuoreUI.Controls.cuiCalendarDatePicker dtTo;
    }
}
