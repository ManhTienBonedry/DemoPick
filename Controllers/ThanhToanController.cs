using System;
using System.Collections.Generic;
using DemoPick.Models;
using DemoPick.Services;

namespace DemoPick.Controllers
{
    public class ThanhToanController
    {
        public class CheckoutResult
        {
            public int InvoiceId { get; set; }
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
        }

        public class CheckoutBreakdownLine
        {
            public CartLine CartLine { get; set; }
            public string DisplayName { get; set; }
            public string DisplayQuantity { get; set; }
            public string DisplayTotal { get; set; }
            public bool IsCourt { get; set; }
        }

        public class CheckoutBreakdown
        {
            public List<CheckoutBreakdownLine> DisplayLines { get; set; } = new List<CheckoutBreakdownLine>();
            public decimal SubTotal { get; set; }
            public decimal DiscountAmount { get; set; }
            public decimal FinalTotal { get; set; }
        }

        private readonly PosService _posService;

        public ThanhToanController()
        {
            _posService = new PosService();
        }

        public decimal GetCourtPayableRatio(BookingModel booking)
        {
            if (booking == null)
                return 1m;

            string paymentState = (booking.PaymentState ?? string.Empty).Trim();
            if (string.Equals(paymentState, AppConstants.BookingPaymentState.BankTransferred, StringComparison.OrdinalIgnoreCase))
                return 0m;

            if (string.Equals(paymentState, AppConstants.BookingPaymentState.Deposit50, StringComparison.OrdinalIgnoreCase))
                return 0.5m;

            return 1m;
        }

        public CheckoutBreakdown CalculateBreakdown(
            string courtName, 
            BookingModel booking, 
            CourtModel court, 
            bool isFixedCustomer, 
            decimal discountPct)
        {
            var result = new CheckoutBreakdown();
            decimal fixedDiscountAmtAmount = 0;

            if (booking != null)
            {
                var services = new List<ServiceCharge>();
                var pendingLines = PosService.GetPendingOrder(courtName);

                foreach (var pl in pendingLines)
                {
                    string name = pl.ProductName ?? "";
                    string unit = PriceCalculator.GuessServiceUnit(name);

                    services.Add(new ServiceCharge
                    {
                        ProductID = pl.ProductId,
                        ServiceName = name,
                        Quantity = pl.Quantity,
                        UnitPrice = pl.UnitPrice,
                        Unit = unit
                    });
                }

                decimal courtMultiplier = PriceCalculator.GetCourtRateMultiplier(court?.CourtType, court?.Name);
                decimal courtPayableRatio = GetCourtPayableRatio(booking);

                var breakdown = PriceCalculator.CalculateTotal(booking.StartTime, booking.EndTime, isFixedCustomer, services, courtMultiplier);

                foreach (var ts in breakdown.TimeSlots)
                {
                    decimal payableSlotTotal = ts.Total * courtPayableRatio;
                    result.DisplayLines.Add(new CheckoutBreakdownLine 
                    { 
                        CartLine = new CartLine(-1, "Giờ sân " + ts.Description, 1, payableSlotTotal),
                        DisplayName = "Giờ sân " + ts.Description,
                        DisplayQuantity = $"{ts.Hours:0.##}h",
                        DisplayTotal = payableSlotTotal.ToString("N0") + "đ",
                        IsCourt = true 
                    });
                }

                foreach (var svc in breakdown.Services)
                {
                    result.DisplayLines.Add(new CheckoutBreakdownLine 
                    { 
                        CartLine = new CartLine(svc.ProductID, svc.ServiceName, svc.Quantity, svc.UnitPrice),
                        DisplayName = svc.ServiceName,
                        DisplayQuantity = svc.Quantity.ToString(),
                        DisplayTotal = svc.Total.ToString("N0") + "đ",
                        IsCourt = false 
                    });
                }

                result.SubTotal = (breakdown.SubtotalCourts * courtPayableRatio) + breakdown.SubtotalServices;
                fixedDiscountAmtAmount = breakdown.DiscountAmount * courtPayableRatio;
            }
            else
            {
                var pendingLines = PosService.GetPendingOrder(courtName);
                foreach (var line in pendingLines)
                {
                    decimal total = line.UnitPrice * line.Quantity;
                    result.DisplayLines.Add(new CheckoutBreakdownLine 
                    { 
                        CartLine = new CartLine(line.ProductId, line.ProductName, line.Quantity, line.UnitPrice),
                        DisplayName = line.ProductName,
                        DisplayQuantity = line.Quantity.ToString(),
                        DisplayTotal = total.ToString("N0") + "đ",
                        IsCourt = false
                    });
                    result.SubTotal += total;
                }
            }

            result.DiscountAmount = (result.SubTotal * discountPct) + fixedDiscountAmtAmount;
            result.FinalTotal = result.SubTotal - result.DiscountAmount;
            if (result.FinalTotal < 0) result.FinalTotal = 0;

            return result;
        }

        public CheckoutResult PerformCheckout(
            int customerId,
            List<CartLine> lines,
            decimal subTotal,
            decimal discountAmount,
            decimal finalTotal,
            string courtName,
            BookingModel booking)
        {
            try
            {
                int? preferredBookingId = (booking != null && booking.BookingID > 0)
                    ? (int?)booking.BookingID
                    : null;

                int invoiceId = _posService.Checkout(
                    customerId,
                    lines,
                    subTotal,
                    discountAmount,
                    finalTotal,
                    "Cash",
                    courtName,
                    preferredBookingId
                );

                PosService.ClearPendingOrder(courtName);

                return new CheckoutResult { Success = true, InvoiceId = invoiceId };
            }
            catch (Exception ex)
            {
                return new CheckoutResult { Success = false, ErrorMessage = ex.Message };
            }
        }

        public List<InvoiceService.InvoiceHistoryItem> GetPaymentHistory(int limit, string keyword, int? memberId = null)
        {
            return InvoiceService.GetInvoiceHistory(limit, keyword, memberId) ?? new List<InvoiceService.InvoiceHistoryItem>();
        }

        public InvoiceService.ShiftReconciliationSummary GetShiftSummary()
        {
            return InvoiceService.GetCurrentShiftSummary();
        }

        public async System.Threading.Tasks.Task<CheckoutCustomerModel> SearchCustomerAsync(string phone)
        {
            var svc = new CustomerService();
            return await svc.FindCheckoutCustomerAsync(phone);
        }
    }
}
