using System.Drawing;
using System.Windows.Forms;

namespace DemoPick.Services
{
    internal static class UiTheme
    {
        public static bool IsDarkMode => false;

        // ── Background tokens ──
        public static Color PageBackground => IsDarkMode
            ? Color.FromArgb(25, 30, 35)
            : Color.FromArgb(236, 239, 243);

        public static Color CardBackground => IsDarkMode
            ? Color.FromArgb(38, 44, 52)
            : Color.White;

        // ── Text tokens ──
        public static Color TextPrimary => IsDarkMode
            ? Color.FromArgb(230, 235, 240)
            : Color.FromArgb(26, 35, 50);

        public static Color TextSecondary => IsDarkMode
            ? Color.FromArgb(160, 170, 180)
            : Color.FromArgb(107, 114, 128);

        // ── Border tokens ──
        public static Color CardBorder => IsDarkMode
            ? Color.FromArgb(78, 89, 98)
            : Color.FromArgb(196, 200, 206);

        public static Color FrameDivider => IsDarkMode
            ? Color.FromArgb(67, 78, 86)
            : Color.FromArgb(196, 200, 206);

        public const float CardBorderWidth = 2f;
        private static readonly Color LegacyLightBorder = Color.FromArgb(229, 231, 235);

        // ── Grid tokens ──
        public static Color GridBackground => IsDarkMode
            ? Color.FromArgb(32, 38, 46)
            : Color.White;

        public static Color GridAltRow => IsDarkMode
            ? Color.FromArgb(40, 47, 55)
            : Color.FromArgb(248, 249, 250);

        public static Color GridHeaderBack => IsDarkMode
            ? Color.FromArgb(45, 52, 62)
            : Color.FromArgb(240, 242, 245);

        public static void ToggleMode()
        {
            // Dark mode disabled
        }

        public static void ApplyPageBackground(Control root)
        {
            if (root == null || root.IsDisposed) return;
            root.BackColor = PageBackground;
        }

        /// <summary>
        /// Applies page theme plus deep color propagation for dark mode.
        /// </summary>
        public static void ApplyModuleTheme(Control moduleRoot)
        {
            if (moduleRoot == null) return;

            ApplyPageBackground(moduleRoot);
            NormalizeTextBackgrounds(moduleRoot);
            ApplyDeepTheme(moduleRoot, 0);
        }

        public static void NormalizeTextBackgrounds(Control root)
        {
            if (root == null || root.IsDisposed) return;
            NormalizeTextBackgroundsRecursive(root);
        }

        /// <summary>
        /// Recursively apply theme colors to all nested controls.
        /// </summary>
        private static void ApplyDeepTheme(Control root, int depth)
        {
            // Theming disabled by user request. Colors will be handled by the Designer.
        }

        private static void NormalizeTextBackgroundsRecursive(Control root)
        {
            if (root == null || root.IsDisposed) return;

            if (root is Label label)
            {
                NormalizeLabelBackground(label);
            }

            foreach (Control child in root.Controls)
            {
                NormalizeTextBackgroundsRecursive(child);
            }
        }

        private static void NormalizeLabelBackground(Label label)
        {
            if (label == null || label.IsDisposed) return;
            if (label.Padding != Padding.Empty) return;
            if (label.BorderStyle != BorderStyle.None) return;
            if (label.BackColor == Color.Transparent) return;

            if (label.BackColor.IsEmpty)
            {
                label.BackColor = Color.Transparent;
                return;
            }

            if (label.Parent != null && label.BackColor.ToArgb() == label.Parent.BackColor.ToArgb())
            {
                label.BackColor = Color.Transparent;
                return;
            }

            if (IsNeutralLabelBackground(label.BackColor))
            {
                label.BackColor = Color.Transparent;
            }
        }

        private static bool IsNeutralLabelBackground(Color color)
        {
            int argb = color.ToArgb();

            return argb == SystemColors.Control.ToArgb()
                || argb == SystemColors.ControlLight.ToArgb()
                || argb == SystemColors.ControlLightLight.ToArgb()
                || argb == Color.White.ToArgb()
                || argb == Color.WhiteSmoke.ToArgb()
                || argb == Color.Gainsboro.ToArgb()
                || argb == PageBackground.ToArgb()
                || argb == CardBackground.ToArgb()
                || argb == LegacyLightBorder.ToArgb()
                || argb == Color.FromArgb(243, 244, 246).ToArgb()
                || argb == Color.FromArgb(250, 250, 250).ToArgb();
        }
    }
}
