using DemoPick.Helpers;
using DemoPick.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using DemoPick.Models;

namespace DemoPick.Services
{
    internal static class BookingCourtQueryService
    {
        // Lay du lieu/ket qua cho Get Active Courts tu tang xu ly phu hop.
        internal static List<CourtModel> GetActiveCourts()
        {
            var list = new List<CourtModel>();
            var seenCourtKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            string query = $@"
SELECT CourtID, Name, CourtType, HourlyRate
FROM Courts
WHERE (Status = '{AppConstants.CourtStatus.Active}' OR Status IS NULL OR LTRIM(RTRIM(Status)) = '')
ORDER BY
    CASE
        WHEN CourtType IN (N'Trong nhà', N'Trong Nha', N'Indoor') OR Name LIKE N'%(Trong nhà)%' THEN 1
        WHEN CourtType IN (N'Ngoài trời', N'Ngoai Troi', N'Outdoor') OR Name LIKE N'%(Ngoài trời)%' THEN 2
        WHEN CourtType IN (N'Sân tập', N'San tap', N'Practice', N'Training') OR Name LIKE N'Sân Tập%' OR Name LIKE N'San Tap%' THEN 3
        ELSE 9
    END,
    CASE
        WHEN PATINDEX('%[0-9]%', Name) > 0
        THEN TRY_CONVERT(int,
            SUBSTRING(
                Name,
                PATINDEX('%[0-9]%', Name),
                PATINDEX('%[^0-9]%', SUBSTRING(Name, PATINDEX('%[0-9]%', Name), 100) + 'X') - 1
            )
        )
        ELSE 9999
    END,
    Name,
    CourtID";

            var dt = DatabaseHelper.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                string rawName = row["Name"]?.ToString() ?? string.Empty;
                string courtKey = NormalizeCourtNameKey(rawName);
                if (!string.IsNullOrWhiteSpace(courtKey) && !seenCourtKeys.Add(courtKey))
                {
                    continue;
                }

                list.Add(new CourtModel
                {
                    CourtID = Convert.ToInt32(row["CourtID"]),
                    Name = rawName,
                    CourtType = row["CourtType"].ToString(),
                    HourlyRate = Convert.ToDecimal(row["HourlyRate"])
                });
            }

            return list;
        }

        // Ap dung hoac chuan hoa trang thai Normalize Court Name Key de du lieu/giao dien nhat quan.
        private static string NormalizeCourtNameKey(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;

            string s = name.Replace('\u00A0', ' ');
            s = Regex.Replace(s, "\\s+", " ").Trim();
            return s;
        }
    }
}

