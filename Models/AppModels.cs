using System;
using System.Collections.Generic;

namespace DemoPick.Models
{
    public class CustomerModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string CustomerType { get; set; }
        public decimal TotalHours { get; set; }
        public string TotalSpent { get; set; }
        public string Tier { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class InventoryItemModel
    {
        public int ProductId { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Stock { get; set; }
        public string Status { get; set; }
        public string Price { get; set; }
        public int CurrentStock { get; set; }
        public int MinThreshold { get; set; }
        public int SoldLast14Days { get; set; }
        public decimal AvgDailySales { get; set; }
        public int SuggestedReorderQuantity { get; set; }
        public int TargetStockQuantity { get; set; }
        public int? DaysRemaining { get; set; }
        public string Recommendation { get; set; }
    }

    public class TransactionModel
    {
        public string EventDesc { get; set; }
        public string SubDesc { get; set; }
        public string Time { get; set; }
    }

    public class TopCourtModel
    {
        public string CourtId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Occupancy { get; set; }
        public string Revenue { get; set; }
        public string PeakSlot { get; set; }
        public string CancelRate { get; set; }
    }
    public class CourtModel
    {
        public int CourtID { get; set; }
        public string Name { get; set; }
        public string CourtType { get; set; }
        public decimal HourlyRate { get; set; }
    }

    public class BookingModel
    {
        public int BookingID { get; set; }
        public int CourtID { get; set; }
        public string GuestName { get; set; }
        public string Note { get; set; }
        public string PaymentState { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } 
    }

    public class TrendPointModel
    {
        public string Label { get; set; }
        public decimal Revenue { get; set; }
    }

    public class NamedRevenueModel
    {
        public string Name { get; set; }
        public decimal Revenue { get; set; }
    }

    public class ReportKpiModel
    {
        public decimal CurrRev { get; set; }
        public decimal PrevRev { get; set; }
        public decimal CurrOcc { get; set; }
        public decimal PrevOcc { get; set; }
        public int CurrNewCust { get; set; }
        public int PrevNewCust { get; set; }
    }

    public class ReportHeatmapPointModel
    {
        public int Hour { get; set; }
        public string Label { get; set; }
        public int BookingCount { get; set; }
    }

    public class ReportBookingOpsModel
    {
        public int TotalBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int ShiftedBookings { get; set; }
        public int ActiveBookings { get; set; }
    }

    public class DashboardMetricsModel
    {
        public decimal Revenue { get; set; }
        public int OccupancyPct { get; set; }
        public int CustomerCount { get; set; }
        public int PosCount { get; set; }
    }

    public class DashboardActivityModel
    {
        public string Code { get; set; }
        public string CourtName { get; set; }
        public string CustomerName { get; set; }
        public string TimeText { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
    }

    public class CustomerTierCountsModel
    {
        public int FixedCount { get; set; }
        public int WalkinCount { get; set; }
    }

    public class CustomerRevenueSummaryModel
    {
        public int MemberCount { get; set; }
        public decimal Revenue { get; set; }
    }

    public class MembershipSummaryModel
    {
        public int BasicCount { get; set; }
        public int SilverCount { get; set; }
        public int GoldCount { get; set; }
        public int NearSilverCount { get; set; }
        public int NearGoldCount { get; set; }
    }

    public class InventoryKpiModel
    {
        public decimal TotalValue { get; set; }
        public int CriticalItems { get; set; }
        public int Sales { get; set; }
        public int InvoicesCount { get; set; }
    }

    public class InventoryForecastPointModel
    {
        public string Label { get; set; }
        public int RiskItems { get; set; }
    }

    public class InventorySmartInsightsModel
    {
        public int OutOfStockCount { get; set; }
        public int WarningItemsCount { get; set; }
        public int SuggestedReorderProductCount { get; set; }
        public int SuggestedReorderUnits { get; set; }
        public int SoldLast14Days { get; set; }
        public decimal AvgDailySales { get; set; }
        public List<InventoryForecastPointModel> ForecastPoints { get; set; } = new List<InventoryForecastPointModel>();
    }

    public class ProductCatalogItemModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }

    public class ProductDeleteListItemModel
    {
        public int ProductId { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class CheckoutCustomerModel
    {
        public int MemberId { get; set; }
        public string FullName { get; set; }
        public string Tier { get; set; }
        public bool IsFixed { get; set; }
    }
}
