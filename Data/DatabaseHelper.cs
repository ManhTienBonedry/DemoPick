using DemoPick.Helpers;
using DemoPick.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DemoPick.Data
{
    public static class DatabaseHelper
    {
        private static readonly object _logThrottleLock = new object();
        private static readonly Dictionary<string, DateTime> _lastLogUtcByKey = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

        // Lay hoac nap du lieu cho Get Connection tu CSDL/nguon cau hinh.
        public static SqlConnection GetConnection()
        {
            return Db.CreateConnection();
        }

        // Thuc thi Execute Query, thuong la cau lenh hoac script thao tac voi CSDL.
        public static DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                var dt = new DataTable();
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        // Thuc thi Execute Query, thuong la cau lenh hoac script thao tac voi CSDL.
        public static DataTable ExecuteQuery(SqlConnection conn, SqlTransaction tran, string query, params SqlParameter[] parameters)
        {
            using (var cmd = new SqlCommand(query, conn, tran))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                var dt = new DataTable();
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        // Thuc thi Execute Non Query, thuong la cau lenh hoac script thao tac voi CSDL.
        public static int ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        // Thuc thi Execute Non Query, thuong la cau lenh hoac script thao tac voi CSDL.
        public static int ExecuteNonQuery(SqlConnection conn, SqlTransaction tran, string query, params SqlParameter[] parameters)
        {
            using (var cmd = new SqlCommand(query, conn, tran))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery();
            }
        }

        // Thuc thi Execute Scalar, thuong la cau lenh hoac script thao tac voi CSDL.
        public static object ExecuteScalar(string query, params SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        // Thuc thi Execute Scalar, thuong la cau lenh hoac script thao tac voi CSDL.
        public static object ExecuteScalar(SqlConnection conn, SqlTransaction tran, string query, params SqlParameter[] parameters)
        {
            using (var cmd = new SqlCommand(query, conn, tran))
            {
                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteScalar();
            }
        }

        // Ghi log theo kieu best-effort, neu ghi log loi thi khong lam dung luong chinh.
        public static void TryLog(string eventDesc, string subDesc)
        {
            try
            {
                ExecuteNonQuery(
                    "INSERT INTO SystemLogs (EventDesc, SubDesc) VALUES (@EventDesc, @SubDesc)",
                    new SqlParameter("@EventDesc", eventDesc ?? ""),
                    new SqlParameter("@SubDesc", (object)subDesc ?? DBNull.Value)
                );
            }
            catch
            {
                // Intentionally swallow: logging must never break the app.
            }
        }

        // Ghi log theo kieu best-effort, neu ghi log loi thi khong lam dung luong chinh.
        public static void TryLog(string eventDesc, Exception ex, string context = null)
        {
            string sub = ex == null ? (context ?? "") : $"{context}\n{ex.GetType().Name}: {ex.Message}";
            TryLog(eventDesc, sub);
        }

        // Best-effort throttled logging to SystemLogs. Never throws.
        // Ghi log co gioi han tan suat de tranh spam nhieu ban ghi loi trung nhau.
        public static void TryLogThrottled(string throttleKey, string eventDesc, Exception ex, string context = null, int minSeconds = 60)
        {
            try
            {
                if (minSeconds < 0) minSeconds = 0;

                string key = string.IsNullOrWhiteSpace(throttleKey)
                    ? (string.IsNullOrWhiteSpace(eventDesc) ? "log" : eventDesc.Trim())
                    : throttleKey.Trim();

                DateTime now = DateTime.UtcNow;
                bool shouldLog = false;
                lock (_logThrottleLock)
                {
                    if (_lastLogUtcByKey.Count > 2048)
                    {
                        _lastLogUtcByKey.Clear();
                    }

                    if (!_lastLogUtcByKey.TryGetValue(key, out DateTime last) || (now - last).TotalSeconds >= minSeconds)
                    {
                        _lastLogUtcByKey[key] = now;
                        shouldLog = true;
                    }
                }

                if (!shouldLog) return;

                TryLog(eventDesc, ex, context);
            }
            catch
            {
                // Intentionally swallow: logging must never break the app.
            }
        }
    }
}


