using DemoPick.Helpers;
using DemoPick.Data;
using System;
using System.Runtime.InteropServices;

namespace DemoPick
{
    /// <summary>
    /// Helper to drag frameless WinForms windows natively.
    /// Kept central so current and legacy forms can share the same native helpers.
    /// </summary>
    public static class NativeMethods
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        // Ho tro xu ly Send Message de cac noi khac dung lai cung mot quy tac.
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        // Ho tro xu ly Release Capture de cac noi khac dung lai cung mot quy tac.
        public static extern bool ReleaseCapture();
    }
}


