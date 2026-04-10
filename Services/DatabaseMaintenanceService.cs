using System;

namespace DemoPick.Services
{
    public class DatabaseMaintenanceService
    {
        public void TryHealCorruptedCourtNames()
        {
            // Self-heal corrupted database text (tr?i instead of trời)
            try
            {
                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE Courts SET Name = REPLACE(Name, 'tr?i', N'trời') WHERE Name LIKE '%tr?i%';"
                );
            }
            catch (Exception ex)
            {
                DatabaseHelper.TryLog("DB Self-heal Skipped", ex, "DatabaseMaintenanceService.TryHealCorruptedCourtNames");
            }
        }
    }
}
