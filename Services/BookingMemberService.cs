using System;
using System.Data.SqlClient;

namespace DemoPick.Services
{
    internal static class BookingMemberService
    {
        internal static int? GetOrCreateMemberId(string fullName, string phone)
        {
            try
            {
                string p = PhoneNumberValidator.NormalizeDigits(phone);
                string name = (fullName ?? string.Empty).Trim();

                // Members.FullName is NOT NULL in the default schema; ensure a non-empty value.
                string safeName = string.IsNullOrWhiteSpace(name) ? p : name;

                if (string.IsNullOrWhiteSpace(p) || p.Length != 10)
                    return null;

                // Find existing member by phone.
                object existingObj = DatabaseHelper.ExecuteScalar(
                    "SELECT TOP 1 MemberID FROM dbo.Members WHERE Phone = @Phone ORDER BY MemberID DESC",
                    new SqlParameter("@Phone", p)
                );

                if (existingObj != null && existingObj != DBNull.Value)
                {
                    int id = Convert.ToInt32(existingObj);

                    // Best-effort: update name if the record has empty name.
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        DatabaseHelper.ExecuteNonQuery(
                            "UPDATE dbo.Members SET FullName = COALESCE(NULLIF(LTRIM(RTRIM(FullName)), ''), @Name) WHERE MemberID = @Id",
                            new SqlParameter("@Name", name),
                            new SqlParameter("@Id", id)
                        );
                    }

                    return id;
                }

                // Create new member.
                object newIdObj = DatabaseHelper.ExecuteScalar(@"
INSERT INTO dbo.Members (FullName, Phone)
VALUES (@Name, @Phone);
SELECT CAST(SCOPE_IDENTITY() AS INT);",
                    new SqlParameter("@Name", safeName),
                    new SqlParameter("@Phone", p)
                );

                if (newIdObj == null || newIdObj == DBNull.Value) return null;
                return Convert.ToInt32(newIdObj);
            }
            catch (Exception ex)
            {
                try { DatabaseHelper.TryLog("Member Upsert Error", ex, "BookingMemberService.GetOrCreateMemberId"); } catch { }
                return null;
            }
        }
    }
}