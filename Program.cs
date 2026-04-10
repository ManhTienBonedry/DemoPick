using System;
using System.Windows.Forms;
using DemoPick.Services;

namespace DemoPick
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool smoke = args != null && Array.Exists(args, a => string.Equals(a, "--smoke", StringComparison.OrdinalIgnoreCase));
            if (smoke)
            {
                // Best-effort: suppress bootstrap UI prompts during automated runs.
                try { Environment.SetEnvironmentVariable("DEMOPICK_SUPPRESS_UI", "1"); } catch { }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                SchemaInstaller.EnsureDatabaseAndSchema();
                MigrationsRunner.ApplyPendingMigrations();
            }
            catch (Exception ex)
            {
                try { DatabaseHelper.TryLog("Schema Ensure Failed", ex, "Program.Main"); } catch { }
                if (smoke)
                {
                    // No UI in smoke mode.
                    try { Console.Error.WriteLine(DbDiagnostics.BuildDbInitErrorMessage(ex)); } catch { }
                    Environment.ExitCode = 1;
                    return;
                }

                MessageBox.Show(
                    DbDiagnostics.BuildDbInitErrorMessage(ex),
                    "Lỗi CSDL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            if (smoke)
            {
                int code = SmokeTestRunner.Run(args ?? Array.Empty<string>());
                Environment.ExitCode = code;
                return;
            }

            Application.Run(new AppFlowContext());
        }
    }

    internal sealed class AppFlowContext : ApplicationContext
    {
        public AppFlowContext()
        {
            ShowLogin();
        }

        private void ShowLogin()
        {
            var login = new FrmLogin();
            login.FormClosed += (s, e) =>
            {
                if (login.DialogResult == DialogResult.OK)
                {
                    ShowMain();
                }
                else
                {
                    ExitThread();
                }
                login.Dispose();
            };
            login.Show();
        }

        private void ShowMain()
        {
            var main = new FrmChinh();

            main.FormClosed += (s, e) =>
            {
                if (main.DialogResult == DialogResult.Retry)
                {
                    ShowLogin();
                }
                else
                {
                    ExitThread();
                }
                main.Dispose();
            };
            main.Show();
        }
    }
}
