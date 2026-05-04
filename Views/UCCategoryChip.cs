using DemoPick.Helpers;
using DemoPick.Data;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DemoPick
{
    public partial class UCCategoryChip : UserControl
    {
        private bool _isActive;

        // Khoi tao man hinh/control UCCategoryChip va chuan bi trang thai ban dau can dung.
        public UCCategoryChip()
        {
            InitializeComponent();
            UpdateVisual();

            btnChip.Click += (s, e) => OnClick(e);
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get => btnChip.Text;
            set
            {
                base.Text = value;
                btnChip.Text = value;
                UpdateSizeToContent();
            }
        }

        [Browsable(false)]
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                UpdateVisual();
            }
        }

        // Ap dung hoac chuan hoa trang thai Set Active de du lieu/giao dien nhat quan.
        public void SetActive(bool isActive) => IsActive = isActive;

        // Xu ly logic man hinh On Click va cap nhat control lien quan.
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
        }

        // Cap nhat lai du lieu/trang thai Update Visual tren man hinh hien tai.
        private void UpdateVisual()
        {
            UpdateSizeToContent();

            // Keep colors aligned with existing UI.
            if (_isActive)
            {
                btnChip.FillColor = Color.FromArgb(76, 175, 80);
                btnChip.FillHoverColor = Color.FromArgb(86, 185, 90);
                btnChip.ForeColor = Color.White;
                btnChip.ForeHoverColor = Color.White;
                btnChip.RectColor = Color.FromArgb(76, 175, 80);
            }
            else
            {
                btnChip.FillColor = Color.White;
                btnChip.FillHoverColor = Color.FromArgb(240, 240, 240);
                btnChip.ForeColor = Color.FromArgb(107, 114, 128);
                btnChip.ForeHoverColor = Color.FromArgb(107, 114, 128);
                btnChip.RectColor = Color.FromArgb(229, 231, 235);
            }
        }

        // Xu ly logic man hinh On Text Changed va cap nhat control lien quan.
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            // When base.Text changes (design-time or runtime), keep the inner button in sync.
            btnChip.Text = base.Text;
            UpdateSizeToContent();
        }

        // Xu ly logic man hinh On Font Changed va cap nhat control lien quan.
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            UpdateSizeToContent();
        }

        // Cap nhat lai du lieu/trang thai Update Size To Content tren man hinh hien tai.
        private void UpdateSizeToContent()
        {
            // Keep a minimum width similar to the original hardcoded 100.
            var size = TextRenderer.MeasureText(btnChip.Text ?? string.Empty, btnChip.Font);
            int width = Math.Max(100, size.Width + 28);
            btnChip.Width = width;
            this.Width = width;
        }
    }
}

