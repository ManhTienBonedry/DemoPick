using System;
using System.Windows.Forms;
using DemoPick.Services;
using DemoPick.Data;
using DemoPick.Helpers;

namespace DemoPick
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        // Diem vao chinh cua ung dung, khoi tao CSDL roi mo luong dang nhap/main form.
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
        // Khoi tao AppFlowContext va thiet lap trang thai ban dau can dung.
        public AppFlowContext()
        {
            ShowLogin();
        }

        // Dieu huong hoac hien thi Show Login theo trang thai hien tai cua ung dung.
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

        // Dieu huong hoac hien thi Show Main theo trang thai hien tai cua ung dung.
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


