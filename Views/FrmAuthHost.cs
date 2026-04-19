using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using DemoPick.Services;

namespace DemoPick
{
    public partial class FrmAuthHost : Form
    {
        private UCLogin _ucLogin;
        private UCRegister _ucRegister;
        private UCBasicLogin _ucBasicLogin;
        private Control _currentCard;

        private Image _rootBackgroundImage;

        // Helps eliminate WinForms flicker when using background images + lots of controls.
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                try
                {
                    cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                }
                catch
                {
                    // ignore
                }
                return cp;
            }
        }

        public FrmAuthHost()
        {
            InitializeComponent();

            if (DesignModeUtil.IsDesignMode(this))
            {
                return;
            }

            // Full-screen borderless host.
            // Use standard window chrome so users can close/minimize normally.
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;

            // Reduce initial paint flicker.
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            EnableDoubleBuffer(pnlRoot);

            // Paint background as "cover" (fill without distortion).
            InitializeRootBackgroundCover();

            _ucLogin = new UCLogin();
            _ucRegister = new UCRegister();
            _ucBasicLogin = new UCBasicLogin();

            _ucLogin.Authenticated += (s, e) => CompleteOk();
            _ucLogin.RequestRegister += (s, e) => ShowRegister();
            _ucLogin.RequestExit += (s, e) => CompleteCancel();

            _ucRegister.Authenticated += (s, e) => CompleteOk();
            _ucRegister.RequestLogin += (s, e) => ShowLogin();

            _ucBasicLogin.Authenticated += (s, e) => CompleteOk();

            ShowLogin();
        }

        private void InitializeRootBackgroundCover()
        {
            if (pnlRoot == null)
            {
                return;
            }

            _rootBackgroundImage = pnlRoot.BackgroundImage;
            if (_rootBackgroundImage == null)
            {
                return;
            }

            // Prevent WinForms from drawing the image with its built-in layouts (Zoom/Stretch/etc).
            // We'll draw it ourselves as "cover".
            pnlRoot.BackgroundImage = null;

            pnlRoot.Paint += PnlRoot_PaintCoverBackground;
            pnlRoot.Resize += (s, e) => pnlRoot.Invalidate();
        }

        private void PnlRoot_PaintCoverBackground(object sender, PaintEventArgs e)
        {
            Image img = _rootBackgroundImage;
            if (img == null || pnlRoot == null)
            {
                return;
            }

            Rectangle bounds = pnlRoot.ClientRectangle;
            if (bounds.Width <= 0 || bounds.Height <= 0 || img.Width <= 0 || img.Height <= 0)
            {
                return;
            }

            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            DrawImageCover(e.Graphics, img, bounds);
        }

        private static void DrawImageCover(Graphics graphics, Image image, Rectangle bounds)
        {
            float scaleX = (float)bounds.Width / image.Width;
            float scaleY = (float)bounds.Height / image.Height;
            float scale = Math.Max(scaleX, scaleY);

            int drawWidth = (int)Math.Ceiling(image.Width * scale);
            int drawHeight = (int)Math.Ceiling(image.Height * scale);

            int x = bounds.X + (bounds.Width - drawWidth) / 2;
            int y = bounds.Y + (bounds.Height - drawHeight) / 2;

            graphics.DrawImage(image, new Rectangle(x, y, drawWidth, drawHeight));
        }

        private void ShowLogin()
        {
            ShowCard(_ucBasicLogin);
        }

        private void ShowRegister()
        {
            ShowCard(_ucRegister);
        }

        private void ShowCard(Control card)
        {
            if (card == null)
            {
                return;
            }

            pnlRoot.SuspendLayout();
            try
            {
                EnsureCardAdded(_ucLogin);
                EnsureCardAdded(_ucRegister);
                EnsureCardAdded(_ucBasicLogin);

                _currentCard = card;

                if (_ucLogin != null) _ucLogin.Visible = ReferenceEquals(card, _ucLogin);
                if (_ucRegister != null) _ucRegister.Visible = ReferenceEquals(card, _ucRegister);
                if (_ucBasicLogin != null) _ucBasicLogin.Visible = ReferenceEquals(card, _ucBasicLogin);

                card.Visible = true;
                card.BringToFront();
            }
            finally
            {
                pnlRoot.ResumeLayout(true);
            }

            CenterCard();
        }

        private void CenterCard()
        {
            if (pnlRoot == null)
            {
                return;
            }

            Control card = _currentCard;
            if (card == null)
            {
                return;
            }

            int x = (pnlRoot.ClientSize.Width - card.Width) / 2;
            int y = (pnlRoot.ClientSize.Height - card.Height) / 2;

            if (x < 0) x = 0;
            if (y < 0) y = 0;

            card.Location = new Point(x, y);
        }

        private void EnsureCardAdded(Control card)
        {
            if (card == null || pnlRoot == null)
            {
                return;
            }

            if (!pnlRoot.Controls.Contains(card))
            {
                card.Dock = DockStyle.None;
                card.Anchor = AnchorStyles.None;
                card.Visible = false;
                pnlRoot.Controls.Add(card);
            }
        }

        private static void EnableDoubleBuffer(Control control)
        {
            if (control == null)
            {
                return;
            }

            try
            {
                typeof(Control)
                    .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.SetValue(control, true, null);
            }
            catch
            {
                // ignore
            }
        }

        private void CompleteOk()
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CompleteCancel()
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            CenterCard();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CenterCard();
        }
    }
}
