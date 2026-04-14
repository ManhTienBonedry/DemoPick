/*
Exclude service-category products ("Dịch vụ") from stock reduction.

Rationale:
- POS service items are represented as products so they can appear on invoices/reports.
- They should not participate in inventory stock checks or stock decrements.
*/

IF OBJECT_ID('dbo.trg_ReduceStock','TR') IS NOT NULL
    DROP TRIGGER dbo.trg_ReduceStock;
GO

CREATE TRIGGER dbo.trg_ReduceStock
ON dbo.InvoiceDetails
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE p
    SET p.StockQuantity = p.StockQuantity - i.Quantity
    FROM dbo.Products p
    INNER JOIN inserted i ON p.ProductID = i.ProductID
    WHERE i.ProductID IS NOT NULL
      AND ISNULL(p.Category, N'') <> N'Dịch vụ';
END
GO
