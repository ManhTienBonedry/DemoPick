namespace DemoPick.Models
{
    public sealed class CartLine
    {
        public int ProductId { get; }
        public string ProductName { get; }
        public int Quantity { get; }
        public decimal UnitPrice { get; }

        // Optional: used for business rules like "Dịch vụ không trừ kho".
        public string Category { get; }

        public CartLine(int productId, string productName, int quantity, decimal unitPrice, string category = null)
        {
            ProductId = productId;
            ProductName = productName ?? "";
            Quantity = quantity;
            UnitPrice = unitPrice;
            Category = category;
        }
    }
}
