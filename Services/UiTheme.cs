using System.Drawing;
using System.Windows.Forms;

namespace DemoPick.Services
{
    internal static class UiTheme
    {
        // Slightly darker gray for the inner page background (better contrast with white cards).
        public static readonly Color PageBackground = Color.FromArgb(243, 244, 246);

        public static void ApplyPageBackground(Control root)
        {
            if (root == null || root.IsDisposed) return;
            root.BackColor = PageBackground;
        }

        /// <summary>
        /// Applies page theme plus small WinForms/SunnyUI rendering fixups.
        /// </summary>
        public static void ApplyModuleTheme(Control moduleRoot)
        {
            if (moduleRoot == null) return;

            ApplyPageBackground(moduleRoot);
            FixSunnyUiLabelBackColor(moduleRoot);
        }

        private static void FixSunnyUiLabelBackColor(Control root)
        {
            if (root == null || root.IsDisposed) return;

            var children = new Control[root.Controls.Count];
            root.Controls.CopyTo(children, 0);

            foreach (Control child in children)
            {
                if (child == null || child.IsDisposed) continue;

                if (child is Label lbl && lbl.Parent is Sunny.UI.UIPanel uiPanel)
                {
                    // Some SunnyUI containers don't blend WinForms label transparency well,
                    // causing the label to render as a gray block. Match the panel FillColor.
                    lbl.BackColor = uiPanel.FillColor;
                }

                if (child.HasChildren)
                {
                    FixSunnyUiLabelBackColor(child);
                }
            }
        }
    }
}
