using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace DemoPick.Services
{
    internal static class DesignModeUtil
    {
        internal static bool IsDesignMode(Control control)
        {
            // Most reliable early check for designer host.
            try
            {
                var processName = Process.GetCurrentProcess().ProcessName;
                if (string.Equals(processName, "devenv", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            catch
            {
                // ignore
            }

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return true;
            }

            // Designer loads control types without real app entry assembly.
            try
            {
                Assembly entry = Assembly.GetEntryAssembly();
                if (entry == null)
                {
                    return true;
                }
            }
            catch
            {
                // ignore
            }

            try
            {
                return control?.Site?.DesignMode == true;
            }
            catch
            {
                return false;
            }
        }
    }
}
