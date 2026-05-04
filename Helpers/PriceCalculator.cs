using DemoPick.Helpers;
using DemoPick.Data;
using System;
using System.Collections.Generic;
using DemoPick.Models;

namespace DemoPick.Helpers
{
    public class TimeSlotCharge
    {
        public DateTime StartBoundary { get; set; }
        public DateTime EndBoundary { get; set; }
        public decimal Hours { get; set; }
        public decimal RatePerHour { get; set; }
        public decimal Total { get; set; }
        public string Description { get; set; }
    }

    public class ServiceCharge
    {
        public int ProductID { get; set; }
        public string ServiceName { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }

    public class PriceBreakdown
    {
        public List<TimeSlotCharge> TimeSlots { get; set; } = new List<TimeSlotCharge>();
        public List<ServiceCharge> Services { get; set; } = new List<ServiceCharge>();
        public decimal SubtotalCourts { get; set; }
        public decimal SubtotalServices { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get; set; }
    }

    public static class PriceCalculator
    {
        private class RateBlock
        {
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public decimal RateWeekday { get; set; }
            public decimal RateWeekend { get; set; }
            public decimal FixedDiscountAmount { get; set; }
            public string Name { get; set; }
        }

        private static readonly List<RateBlock> Blocks = new List<RateBlock>
        {
            new RateBlock { StartTime = new TimeSpan(5, 0, 0), EndTime = new TimeSpan(9, 0, 0), RateWeekday = 130000, RateWeekend = 130000, FixedDiscountAmount = 30000, Name = "Ca Sáng (5h-9h)" },
            new RateBlock { StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), RateWeekday = 130000, RateWeekend = 150000, FixedDiscountAmount = 20000, Name = "Ca Bất Thường (9h-17h)" },
            new RateBlock { StartTime = new TimeSpan(17, 0, 0), EndTime = new TimeSpan(21, 0, 0), RateWeekday = 200000, RateWeekend = 200000, FixedDiscountAmount = 20000, Name = "Ca Vàng (17h-21h)" },
            new RateBlock { StartTime = new TimeSpan(21, 0, 0), EndTime = new TimeSpan(23, 0, 0), RateWeekday = 180000, RateWeekend = 180000, FixedDiscountAmount = 30000, Name = "Ca Tối (21h-23h)" }
        };

        // Ho tro xu ly Guess Service Unit de cac noi khac dung lai cung mot quy tac.
        public static string GuessServiceUnit(string serviceName)
        {
            string name = (serviceName ?? string.Empty).Trim();
            if (name.Length == 0) return AppConstants.Units.Piece;

            // Per-hour add-ons
            if (Contains(name, "máy bắn bóng") || Contains(name, "nhặt bóng"))
                return AppConstants.Units.Hour;

            // Basket of balls
            if (Contains(name, "bóng") && Contains(name, "rổ"))
                return AppConstants.Units.Basket;

            return AppConstants.Units.Piece;
        }

        // Lay du lieu/ket qua cho Get Court Rate Multiplier tu tang xu ly phu hop.
        public static decimal GetCourtRateMultiplier(string courtType, string courtName)
        {
            string t = (courtType ?? string.Empty).Trim();
            string n = (courtName ?? string.Empty).Trim();

            // Practice courts: 50% rate
            if (Contains(t, "tập") || Contains(t, "practice") || Contains(n, "tập"))
                return 0.5m;

            return 1m;
        }

        // Ho tro xu ly Contains de cac noi khac dung lai cung mot quy tac.
        private static bool Contains(string haystack, string needle)
        {
            if (string.IsNullOrEmpty(haystack) || string.IsNullOrEmpty(needle)) return false;
            return haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        // Tinh tong tien san va dich vu theo khung gio, loai khach va he so gia san.
        public static PriceBreakdown CalculateTotal(DateTime start, DateTime end, bool isFixedCustomer, List<ServiceCharge> services = null, decimal courtRateMultiplier = 1m)
        {
            var breakdown = new PriceBreakdown();

            if (start >= end) return breakdown;

            if (courtRateMultiplier <= 0) courtRateMultiplier = 1m;

            DateTime current = start;
            while (current < end)
            {
                // Advance day by day if spanning across midnight
                DateTime nextMidnight = current.Date.AddDays(1);
                DateTime blockEnd = end < nextMidnight ? end : nextMidnight;

                bool isWeekend = current.DayOfWeek == DayOfWeek.Saturday || current.DayOfWeek == DayOfWeek.Sunday;

                // For the current day day, find intersecting rate blocks
                foreach (var block in Blocks)
                {
                    DateTime blockStartDateTime = current.Date.Add(block.StartTime);
                    DateTime blockEndDateTime = current.Date.Add(block.EndTime);

                    // Find intersection between [current, blockEnd] and [blockStartDateTime, blockEndDateTime]
                    DateTime overlapStart = current > blockStartDateTime ? current : blockStartDateTime;
                    DateTime overlapEnd = blockEnd < blockEndDateTime ? blockEnd : blockEndDateTime;

                    if (overlapStart < overlapEnd)
                    {
                        decimal hours = (decimal)(overlapEnd - overlapStart).TotalHours;
                        decimal rate = (isWeekend ? block.RateWeekend : block.RateWeekday) * courtRateMultiplier;

                        var tsc = new TimeSlotCharge
                        {
                            StartBoundary = overlapStart,
                            EndBoundary = overlapEnd,
                            Hours = hours,
                            RatePerHour = rate,
                            Total = hours * rate,
                            Description = block.Name
                        };
                        breakdown.TimeSlots.Add(tsc);
                        breakdown.SubtotalCourts += tsc.Total;

                        if (isFixedCustomer)
                        {
                            breakdown.DiscountAmount += (hours * block.FixedDiscountAmount * courtRateMultiplier);
                        }
                    }
                }

                current = blockEnd;
            }

            // Calculate services
            decimal totalDuration = (decimal)(end - start).TotalHours;
            if (services != null)
            {
                foreach (var svc in services)
                {
                    decimal svcTotal = 0;
                    if (string.Equals(svc.Unit, AppConstants.Units.Hour, StringComparison.OrdinalIgnoreCase))
                    {
                        svcTotal = svc.UnitPrice * svc.Quantity * totalDuration;
                    }
                    else
                    {
                        // "Cái", "Rổ", etc.
                        svcTotal = svc.UnitPrice * svc.Quantity;
                    }
                    
                    svc.Total = svcTotal;
                    breakdown.Services.Add(svc);
                    breakdown.SubtotalServices += svcTotal;
                }
            }

            breakdown.GrandTotal = breakdown.SubtotalCourts + breakdown.SubtotalServices - breakdown.DiscountAmount;
            if (breakdown.GrandTotal < 0) breakdown.GrandTotal = 0;

            return breakdown;
        }
    }
}


