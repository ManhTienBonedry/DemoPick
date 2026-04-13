using System.Drawing;
using System.Windows.Forms;

namespace DemoPick
{
    partial class FrmAuthHost
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlRoot;

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
            this.pnlRoot = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // pnlRoot
            // 
            this.pnlRoot.BackgroundImage = global::DemoPick.Properties.Resources.PickleBall;
            this.pnlRoot.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pnlRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRoot.Location = new System.Drawing.Point(0, 0);
            this.pnlRoot.Name = "pnlRoot";
            this.pnlRoot.Size = new System.Drawing.Size(1153, 720);
            this.pnlRoot.TabIndex = 0;
            // 
            // FrmAuthHost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1153, 720);
            this.Controls.Add(this.pnlRoot);
            this.Name = "FrmAuthHost";
            this.Text = "DemoPick";
            this.ResumeLayout(false);

        }
    }
}
