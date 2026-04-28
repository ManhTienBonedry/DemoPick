/*
TesterData_Seed.sql
Mục tiêu: Bơm dữ liệu (Mock Data) chuẩn xác vào CSDL PickleBall phục vụ mục đích Testing (Sản phẩm, Hội viên, Lịch đặt sân, Giao dịch).
Yêu cầu: Dữ liệu này tuân thủ cấu trúc của phiên bản mới nhất, đặc biệt là bảng Members dùng "IsFixed" và "TotalHoursPurchased" thay vì Tier cũ.
Hướng dẫn chạy: Tester mở tab query trong SSMS và chạy file này vào DB "PickleBallDB", hoặc chạy bằng sqlcmd.
*/

USE [PickleBallDB];
GO

SET NOCOUNT ON;
PRINT '------ START SEEDING TESTER DATA ------';

/* 1. SEED MEMBERS (Hội viên: Cố định & Vãng lai) */
IF NOT EXISTS (SELECT 1 FROM dbo.Members WHERE Phone = '0900000001')
BEGIN
    INSERT INTO dbo.Members (FullName, Phone, Level, Tier, TotalSpent, TotalHoursPurchased, IsFixed, CreatedAt)
    VALUES 
    (N'Test Fixed Member 1', '0900000001', 'PRO', 'Silver', 2500000, 30.5, 1, DATEADD(day, -10, GETDATE())),
    (N'Test Walk-in Member 1', '0900000002', 'NEWBIE', 'Bronze', 500000, 0, 0, DATEADD(day, -5, GETDATE())),
    (N'Test Fixed Member 2', '0900000003', 'INTERMEDIATE', 'Gold', 8000000, 50, 1, DATEADD(day, -20, GETDATE())),
    (N'Test Walk-in Member 2', '0900000004', 'NEWBIE', 'Bronze', 180000, 0, 0, DATEADD(day, -2, GETDATE())),
    (N'Test VIP Member 1', '0900000005', 'PRO', 'Gold', 15000000, 100, 1, DATEADD(day, -30, GETDATE()));
    PRINT 'Inserted 5 mock members.';
END

/* 2. SEED PRODUCTS (Hàng hóa tiêu dùng cơ bản) */
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE SKU = 'DRK-001')
BEGIN
    INSERT INTO dbo.Products (SKU, Name, Category, Price, StockQuantity, MinThreshold)
    VALUES 
    ('DRK-001', N'Nước suối Aquafina', N'Thức uống', 10000, 100, 20),
    ('DRK-002', N'Bò húc Thái', N'Thức uống', 20000, 50, 10),
    ('DRK-003', N'Revive chanh muối', N'Thức uống', 15000, 80, 15),
    ('SNK-001', N'Xúc xích Đức', N'Đồ ăn nhẹ', 25000, 30, 5),
    ('SNK-002', N'Mì tôm trứng', N'Đồ ăn nhẹ', 30000, 20, 5);
    PRINT 'Inserted 5 mock consumable products.';
END

/* 3. SEED BOOKINGS & INVOICES (Giao dịch tạo Chart + Báo cáo) */
/* Cần lấy CourtID thực hành tĩnh, giả định 1=Sân 1, 2=Sân 2, 7=Sân 7 (Ngoài trời) */
DECLARE @Court1 INT = (SELECT TOP 1 CourtID FROM Courts WHERE CourtType = N'Trong nhà' ORDER BY CourtID);
DECLARE @Court2 INT = (SELECT TOP 1 CourtID FROM Courts WHERE CourtType = N'Ngoài trời' ORDER BY CourtID);
DECLARE @Mem1 INT = (SELECT TOP 1 MemberID FROM Members WHERE Phone = '0900000001');
DECLARE @Mem2 INT = (SELECT TOP 1 MemberID FROM Members WHERE Phone = '0900000002');

IF NOT EXISTS (SELECT 1 FROM dbo.Bookings WHERE GuestName = 'Test Data Mock')
BEGIN
    /* Tạo booking hằng ngày trong 3 ngày qua để vẽ Trend Line */
    DECLARE @Date1 DATETIME = DATEADD(day, -1, GETDATE());
    DECLARE @Date2 DATETIME = DATEADD(day, -2, GETDATE());
    DECLARE @Date3 DATETIME = DATEADD(day, -3, GETDATE());

    /* Bơm Bookings */
    INSERT INTO dbo.Bookings (CourtID, MemberID, GuestName, StartTime, EndTime, Status)
    VALUES 
    (@Court1, @Mem1, 'Test Data Mock', DATEADD(hour, 8, CAST(CAST(@Date1 AS DATE) AS DATETIME)), DATEADD(hour, 10, CAST(CAST(@Date1 AS DATE) AS DATETIME)), 'Confirmed'),
    (@Court2, @Mem2, 'Test Data Mock', DATEADD(hour, 14, CAST(CAST(@Date1 AS DATE) AS DATETIME)), DATEADD(hour, 16, CAST(CAST(@Date1 AS DATE) AS DATETIME)), 'Confirmed'),
    (@Court1, NULL,  'Khách Vãng Lai A', DATEADD(hour, 7, CAST(CAST(@Date2 AS DATE) AS DATETIME)), DATEADD(hour, 9, CAST(CAST(@Date2 AS DATE) AS DATETIME)), 'Confirmed'),
    (@Court2, @Mem1, 'Test Data Mock', DATEADD(hour, 17, CAST(CAST(@Date3 AS DATE) AS DATETIME)), DATEADD(hour, 19, CAST(CAST(@Date3 AS DATE) AS DATETIME)), 'Confirmed');
    
    DECLARE @BookingId1 INT = SCOPE_IDENTITY() - 3;
    DECLARE @BookingId2 INT = SCOPE_IDENTITY() - 2;

    /* Bơm Invoices cho POS (Nước, Đồ ăn) */
    DECLARE @Prod1 INT = (SELECT TOP 1 ProductID FROM Products WHERE SKU = 'DRK-001');
    DECLARE @Prod2 INT = (SELECT TOP 1 ProductID FROM Products WHERE SKU = 'SNK-001');

    -- Invoice 1
    INSERT INTO dbo.Invoices (MemberID, TotalAmount, DiscountAmount, FinalAmount, PaymentMethod, CreatedAt)
    VALUES (@Mem1, 40000, 0, 40000, 'Banking', @Date1);
    DECLARE @Inv1 INT = SCOPE_IDENTITY();
    
    INSERT INTO dbo.InvoiceDetails (InvoiceID, ProductID, BookingID, Quantity, UnitPrice)
    VALUES 
    (@Inv1, @Prod1, NULL, 4, 10000);

    -- Invoice 2
    INSERT INTO dbo.Invoices (MemberID, TotalAmount, DiscountAmount, FinalAmount, PaymentMethod, CreatedAt)
    VALUES (NULL, 50000, 0, 50000, 'Cash', @Date2);
    DECLARE @Inv2 INT = SCOPE_IDENTITY();

    INSERT INTO dbo.InvoiceDetails (InvoiceID, ProductID, BookingID, Quantity, UnitPrice)
    VALUES 
    (@Inv2, @Prod2, NULL, 2, 25000);

    PRINT 'Inserted mock Bookings, Invoices, and Details.';
END

/* 4. Thêm System Logs */
INSERT INTO dbo.SystemLogs (EventDesc, SubDesc, CreatedAt) 
VALUES (N'Cài đặt Mock Data nội bộ hoàn tất', N'TesterData_Seed', GETDATE());

PRINT '------ END SEEDING TESTER DATA ------';
GO

-- Cập nhật hoặc Thêm mới 4 dịch vụ cố định (Ghim bằng SKU)
MERGE INTO dbo.Products AS Target
USING (VALUES 
    (N'DV_THUE_VOT', N'Thuê vợt', N'Dịch vụ', 40000, 9999, 0),
    (N'DV_BONG_RO', N'Bóng tập (rổ)', N'Dịch vụ', 40000, 9999, 0),
    (N'DV_MAY_BAN', N'Máy bắn bóng', N'Dịch vụ', 80000, 9999, 0),
    (N'DV_NHAT_BONG', N'Nhặt bóng', N'Dịch vụ', 40000, 9999, 0)
) AS Source (SKU, Name, Category, Price, StockQuantity, MinThreshold)
ON Target.SKU = Source.SKU
WHEN MATCHED THEN 
    UPDATE SET 
        Target.Name = Source.Name,
        Target.Category = Source.Category,
        Target.Price = Source.Price
WHEN NOT MATCHED THEN 
    INSERT (SKU, Name, Category, Price, StockQuantity, MinThreshold)
    VALUES (Source.SKU, Source.Name, Source.Category, Source.Price, Source.StockQuantity, Source.MinThreshold);
PRINT 'Upserted 4 fixed service products.';
