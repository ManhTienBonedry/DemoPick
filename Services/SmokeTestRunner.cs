using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using DemoPick.Controllers;

namespace DemoPick.Services
{
    internal static class SmokeTestRunner
    {
        private sealed class StepResult
        {
            public string Name;
            public bool Success;
            public TimeSpan Duration;
            public string Details;
            public Exception Exception;
        }

        internal static int Run(string[] args)
        {
            EnsureConsole();

            var startedAt = DateTime.Now;
            var swTotal = Stopwatch.StartNew();
            var steps = new List<StepResult>();

            string identifier = GetArg(args, "--id") ?? GetArg(args, "-id");
            string password = GetArg(args, "--pw") ?? GetArg(args, "-pw");

            string credentialSource = (!string.IsNullOrWhiteSpace(identifier) && !string.IsNullOrWhiteSpace(password))
                ? "args"
                : null;

            bool createdTempAccount = false;
            string tempUsername = null;

            try
            {
                // Step 1: DB init
                steps.Add(RunStep("DB init (schema + migrations)", () =>
                {
                    SchemaInstaller.EnsureDatabaseAndSchema();
                    MigrationsRunner.ApplyPendingMigrations();
                    return "OK";
                }));

                // Step 2: Obtain credentials (prefer user args)
                if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(password))
                {
                    // If DB is empty, seed admin (DEBUG only inside SchemaInstaller); we avoid UI there.
                    // Otherwise, register a unique temp user we can always login with.
                    steps.Add(RunStep("Obtain test credentials", () =>
                    {
                        // If there are zero accounts, try seeding admin here (works in both DEBUG/RELEASE).
                        if (AuthService.TrySeedAdminIfEmpty(out var seededUser, out var seededPass))
                        {
                            identifier = seededUser;
                            password = seededPass;
                            credentialSource = "seeded-admin";
                            return $"Seeded admin: {seededUser}";
                        }

                        string uniqueEmail = $"smoke_{Guid.NewGuid():N}@local";
                        string pw = "Smoke#" + Guid.NewGuid().ToString("N").Substring(0, 10) + "!";
                        if (!AuthService.TryRegister(
                                fullName: "Smoke Test",
                                email: uniqueEmail,
                                phone: null,
                                password: pw,
                                confirmPassword: pw,
                                out var err))
                        {
                            throw new InvalidOperationException(err ?? "Register failed");
                        }

                        identifier = uniqueEmail;
                        password = pw;
                        createdTempAccount = true;
                        tempUsername = uniqueEmail;
                        credentialSource = "temp-user";
                        return $"Registered temp user: {uniqueEmail}";
                    }));
                }

                // Step 3: Login
                steps.Add(RunStep("Login", () =>
                {
                    if (!AuthService.TryLogin(identifier, password, out var user, out var err))
                        throw new InvalidOperationException(err ?? "Login failed");

                    AppSession.SignIn(user);
                    if (AppSession.CurrentUser == null)
                        throw new InvalidOperationException("Session not set");

                    return $"Signed in as {AppSession.CurrentUser.Username} ({AppSession.CurrentUser.Role})";
                }));

                // Step 4: Load courts
                List<Models.CourtModel> courts = null;
                steps.Add(RunStep("Load courts", () =>
                {
                    var controller = new BookingController();
                    courts = controller.GetCourts();
                    if (courts == null || courts.Count == 0)
                        throw new InvalidOperationException("No active courts found");
                    return $"Courts: {courts.Count}";
                }));

                // Step 5: Create & cancel a booking (best-effort cleanup)
                steps.Add(RunStep("Create + cancel test booking", () =>
                {
                    var controller = new BookingController();

                    int courtId = courts[0].CourtID;
                    string guest = $"SMOKE_{Guid.NewGuid():N}";
                    string note = "SMOKE TEST (auto)";

                    DateTime startBase = DateTime.Now.AddDays(3).Date.AddHours(17);
                    int[] durations = { 90, 60, 120 };

                    Exception last = null;
                    for (int dayOffset = 0; dayOffset <= 14; dayOffset++)
                    {
                        foreach (var dur in durations)
                        {
                            for (int hour = 6; hour <= 22; hour++)
                            {
                                DateTime start = startBase.AddDays(dayOffset).Date.AddHours(hour);
                                DateTime end = start.AddMinutes(dur);
                                if (start <= DateTime.Now) continue;

                                try
                                {
                                    controller.SubmitBooking(courtId, guest, note, start, end, status: "Confirmed");
                                    int bookingId = TryFindBookingId(courtId, guest, start, end);
                                    bool cancelled = TryCancelBookingById(bookingId);
                                    return $"Created booking on CourtID={courtId} {start:yyyy-MM-dd HH:mm} ({dur}m). Cancelled={cancelled}";
                                }
                                catch (Exception ex)
                                {
                                    last = ex;
                                    // Conflict messages come from RAISERROR.
                                    if ((ex.Message ?? "").IndexOf("already booked", StringComparison.OrdinalIgnoreCase) >= 0)
                                        continue;
                                }
                            }
                        }
                    }

                    throw new InvalidOperationException("Unable to create a non-conflicting booking", last);
                }));

                // Step 6: Logout
                steps.Add(RunStep("Logout", () =>
                {
                    AppSession.SignOut();
                    if (AppSession.CurrentUser != null)
                        throw new InvalidOperationException("Session not cleared");
                    return "OK";
                }));

                // Step 7: Cleanup temp account (best-effort)
                if (createdTempAccount && !string.IsNullOrWhiteSpace(tempUsername))
                {
                    steps.Add(RunStep("Cleanup temp account", () =>
                    {
                        int rows = DatabaseHelper.ExecuteNonQuery(
                            "DELETE FROM dbo.StaffAccounts WHERE Username = @U OR (Email IS NOT NULL AND Email = @U)",
                            new SqlParameter("@U", tempUsername));
                        return $"Deleted rows: {rows}";
                    }));
                }

                swTotal.Stop();

                string reportPath = WriteMarkdownReport(startedAt, swTotal.Elapsed, steps, identifier, credentialSource);
                Console.WriteLine("\nSMOKE TEST: SUCCESS");
                Console.WriteLine("Report: " + reportPath);
                return 0;
            }
            catch (Exception fatal)
            {
                swTotal.Stop();
                steps.Add(new StepResult
                {
                    Name = "Fatal",
                    Success = false,
                    Duration = TimeSpan.Zero,
                    Details = fatal.Message,
                    Exception = fatal
                });

                string reportPath = WriteMarkdownReport(startedAt, swTotal.Elapsed, steps, identifier, credentialSource);
                Console.WriteLine("\nSMOKE TEST: FAILED");
                Console.WriteLine(fatal.ToString());
                Console.WriteLine("Report: " + reportPath);
                return 1;
            }
        }

        private static StepResult RunStep(string name, Func<string> action)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                string details = action?.Invoke();
                sw.Stop();
                return new StepResult { Name = name, Success = true, Duration = sw.Elapsed, Details = details };
            }
            catch (Exception ex)
            {
                sw.Stop();
                return new StepResult { Name = name, Success = false, Duration = sw.Elapsed, Details = ex.Message, Exception = ex };
            }
        }

        private static int TryFindBookingId(int courtId, string guest, DateTime start, DateTime end)
        {
            try
            {
                object obj = DatabaseHelper.ExecuteScalar(
                    "SELECT TOP 1 BookingID FROM dbo.Bookings WHERE CourtID=@C AND GuestName=@G AND StartTime=@S AND EndTime=@E ORDER BY BookingID DESC",
                    new SqlParameter("@C", courtId),
                    new SqlParameter("@G", guest),
                    new SqlParameter("@S", start),
                    new SqlParameter("@E", end));

                if (obj == null || obj == DBNull.Value) return 0;
                return Convert.ToInt32(obj);
            }
            catch
            {
                return 0;
            }
        }

        private static bool TryCancelBookingById(int bookingId)
        {
            if (bookingId <= 0) return false;

            try
            {
                int rows = DatabaseHelper.ExecuteNonQuery(
                    "UPDATE dbo.Bookings SET Status = 'Cancelled' WHERE BookingID = @Id",
                    new SqlParameter("@Id", bookingId));
                return rows > 0;
            }
            catch
            {
                return false;
            }
        }

        private static string WriteMarkdownReport(DateTime startedAt, TimeSpan total, List<StepResult> steps, string identifier, string credentialSource)
        {
            string root = FindWorkspaceRoot();
            string docsDir = Path.Combine(root, "Docs");
            Directory.CreateDirectory(docsDir);

            string path = Path.Combine(docsDir, "SMOKE_RUN.md");

            var sb = new StringBuilder();
            sb.AppendLine("# DemoPick Smoke Test Report");
            sb.AppendLine();
            sb.AppendLine($"- Started: {startedAt:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"- Duration: {total.TotalSeconds:F2}s");
            sb.AppendLine($"- Machine: {Environment.MachineName}");
            sb.AppendLine($"- User: {Environment.UserName}");
            sb.AppendLine($"- App: {AppDomain.CurrentDomain.FriendlyName}");
            sb.AppendLine();

            if (!string.IsNullOrWhiteSpace(identifier))
            {
                sb.AppendLine("## Login Used");
                sb.AppendLine();
                sb.AppendLine($"- Identifier: {identifier}");
                if (!string.IsNullOrWhiteSpace(credentialSource)) sb.AppendLine($"- Source: {credentialSource}");
                sb.AppendLine();
            }

            sb.AppendLine("## Steps");
            sb.AppendLine();
            sb.AppendLine("| Step | Result | Duration | Details |");
            sb.AppendLine("|---|---|---:|---|");

            foreach (var s in steps)
            {
                string res = s.Success ? "SUCCESS" : "FAIL";
                string dur = s.Duration.TotalMilliseconds.ToString("0") + "ms";
                string details = (s.Details ?? "").Replace("\r", " ").Replace("\n", " ");
                if (details.Length > 200) details = details.Substring(0, 200) + "…";
                sb.AppendLine($"| {EscapePipe(s.Name)} | {res} | {dur} | {EscapePipe(details)} |");
            }

            sb.AppendLine();
            sb.AppendLine("## Failures");
            sb.AppendLine();
            bool anyFail = false;
            foreach (var s in steps)
            {
                if (s.Success) continue;
                anyFail = true;
                sb.AppendLine($"### {s.Name}");
                sb.AppendLine();
                sb.AppendLine("```text");
                sb.AppendLine((s.Exception ?? new Exception(s.Details ?? "Unknown error")).ToString());
                sb.AppendLine("```");
                sb.AppendLine();
            }
            if (!anyFail)
            {
                sb.AppendLine("No failures.");
                sb.AppendLine();
            }

            File.WriteAllText(path, sb.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            return path;
        }

        private static string EscapePipe(string s)
        {
            return (s ?? "").Replace("|", "\\|");
        }

        private static string FindWorkspaceRoot()
        {
            try
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                for (int i = 0; i < 8 && !string.IsNullOrWhiteSpace(dir); i++)
                {
                    if (File.Exists(Path.Combine(dir, "DemoPick.sln")) || Directory.Exists(Path.Combine(dir, "Docs")))
                        return dir;

                    var parent = Directory.GetParent(dir);
                    if (parent == null) break;
                    dir = parent.FullName;
                }
            }
            catch
            {
                // ignore
            }

            return Environment.CurrentDirectory;
        }

        private static string GetArg(string[] args, string key)
        {
            if (args == null || args.Length == 0) return null;
            for (int i = 0; i < args.Length; i++)
            {
                if (!string.Equals(args[i], key, StringComparison.OrdinalIgnoreCase))
                    continue;
                if (i + 1 < args.Length)
                    return args[i + 1];
                return null;
            }
            return null;
        }

        private static void EnsureConsole()
        {
            try
            {
                // WinExe doesn't have a console by default. Attach to parent console when launched from cmd.
                if (!AttachConsole(ATTACH_PARENT_PROCESS))
                {
                    AllocConsole();
                }

                try { Console.OutputEncoding = Encoding.UTF8; } catch { }
            }
            catch
            {
                // ignore
            }
        }

        private const int ATTACH_PARENT_PROCESS = -1;

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AttachConsole(int dwProcessId);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();
    }
}
