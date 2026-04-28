using System;

namespace DemoPick.Services
{
    internal static class MembershipTierHelper
    {
        internal const decimal SilverThreshold = 2000000m;
        internal const decimal GoldThreshold = 5000000m;

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
