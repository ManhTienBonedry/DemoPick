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
                MessageBox.Show(
                    DbDiagnostics.BuildDbInitErrorMessage(ex),
                    "Lỗi CSDL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
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
            var auth = new FrmAuthHost();
            auth.FormClosed += (s, e) =>
            {
                if (auth.DialogResult == DialogResult.OK)
                {
                    ShowMain();
                }
                else
                {
                    ExitThread();
                }
                auth.Dispose();
            };
            auth.Show();
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
