namespace DemoPick.Services
{
    internal static class AppConstants
    {
        internal static class BookingStatus
        {
            internal const string Cancelled = "Cancelled";
            internal const string Confirmed = "Confirmed";
            internal const string Maintenance = "Maintenance";
            internal const string Paid = "Paid";
            internal const string Pending = "Pending";
        }

        internal static class Roles
        {
            internal const string Admin = "Admin";
            internal const string Staff = "Staff";
        }

        internal static class CourtStatus
        {
            internal const string Active = "Active";
            internal const string Inactive = "Inactive";
        }

        internal static class ProductCategories
        {
            internal const string Service = "Dịch vụ";
        }

        internal static class Units
        {
            internal const string Hour = "Giờ";
            internal const string Basket = "Rổ";
            internal const string Piece = "Cái";
        }
    }
}
