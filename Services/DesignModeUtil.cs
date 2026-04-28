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
                if (string.Equals(processName, "devenv", StringComparison.OrdinalIgnoreCase) ||
                    processName.IndexOf("xdesproc", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    processName.IndexOf("designtoolsserver", StringComparison.OrdinalIgnoreCase) >= 0)
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

            // Out-of-proc WinForms designer can have non-null entry assembly but still be in design-time host.
            try
            {
                var domainName = AppDomain.CurrentDomain.FriendlyName;
                if (!string.IsNullOrWhiteSpace(domainName) &&
                    (domainName.IndexOf("designtools", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     domainName.IndexOf("xdesproc", StringComparison.OrdinalIgnoreCase) >= 0))
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
