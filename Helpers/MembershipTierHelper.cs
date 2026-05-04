using DemoPick.Helpers;
using DemoPick.Data;
using System;

namespace DemoPick.Helpers
{
    internal static class MembershipTierHelper
    {
        internal const decimal SilverThreshold = 2000000m;
        internal const decimal GoldThreshold = 5000000m;

        // Ap dung hoac chuan hoa trang thai Normalize Tier de du lieu/giao dien nhat quan.
        internal static string NormalizeTier(string rawTier)
        {
            string tier = (rawTier ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(tier))
                return "Basic";

            if (tier.Equals("Gold", StringComparison.OrdinalIgnoreCase))
                return "Gold";

            if (tier.Equals("Silver", StringComparison.OrdinalIgnoreCase))
                return "Silver";

            if (tier.Equals("Bronze", StringComparison.OrdinalIgnoreCase) ||
                tier.Equals("Basic", StringComparison.OrdinalIgnoreCase))
                return "Basic";

            return tier;
        }

        // Lay du lieu/ket qua cho Get Discount Percent tu tang xu ly phu hop.
        internal static decimal GetDiscountPercent(string rawTier)
        {
            switch (NormalizeTier(rawTier))
            {
                case "Gold":
                    return 0.10m;
                case "Silver":
                    return 0.05m;
                default:
                    return 0m;
            }
        }

        // Tao hoac tinh ra du lieu Build Upgrade Hint tu cac thong tin dau vao hien co.
        internal static string BuildUpgradeHint(decimal totalSpent)
        {
            if (totalSpent >= GoldThreshold)
                return "Da dat hang Gold";

            if (totalSpent >= SilverThreshold)
            {
                decimal remainingGold = GoldThreshold - totalSpent;
                return "Con " + remainingGold.ToString("N0") + "d de len Gold";
            }

            decimal remainingSilver = SilverThreshold - totalSpent;
            return "Con " + remainingSilver.ToString("N0") + "d de len Silver";
        }
    }
}


