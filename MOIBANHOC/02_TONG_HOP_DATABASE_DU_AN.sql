/*
====================================================================
02_TONG_HOP_DATABASE_DU_AN.sql
Mục tiêu:
- Gộp toàn bộ SQL quan trọng của dự án DemoPick vào 1 file duy nhất.
- Bao gồm: schema chính + migration + seed test + legacy compatibility.

Nguồn gốc script đã gộp:
1) Database/PickleProDB_Complete.sql
2) Database/Migrations/0001__Create_MigrationProof.sql
3) Database/Migrations/0002__StaffAccounts_Lockout.sql
4) Database/Migrations/0003__Members_CreatedAt.sql
5) Database/Migrations/0004__Bookings_Note.sql
6) Database/Migrations/0005__Remove_Seed_Service_Products.sql
7) Database/Migrations/0006__Trigger_ReduceStock_ExcludeServices.sql
8) Database/Legacy/migration.sql
9) Database/TesterData_Seed.sql

Lưu ý:
- Script mang tính idempotent ở mức cao, có thể chạy lại nhiều lần.
- Ưu tiên chạy trong môi trường DEV/TEST trước khi dùng ở môi trường chính thức.
====================================================================
*/

USE [PickleProDB];
GO

/* ================================================================
   A) SCHEMA CHÍNH (từ PickleProDB_Complete.sql)
   ================================================================ */

IF OBJECT_ID('dbo.Courts','U') IS NULL
BEGIN
    CREATE TABLE dbo.Courts (
        CourtID INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        CourtType NVARCHAR(50) NOT NULL,
        Status NVARCHAR(50) NOT NULL CONSTRAINT DF_Courts_Status DEFAULT 'Active',
        HourlyRate DECIMAL(18,2) NOT NULL
    );
END
GO

IF OBJECT_ID('dbo.Members','U') IS NULL
BEGIN
    CREATE TABLE dbo.Members (
        MemberID INT IDENTITY(1,1) PRIMARY KEY,
        FullName NVARCHAR(100) NOT NULL,
        Phone NVARCHAR(20) NOT NULL,
        Level NVARCHAR(50) NOT NULL CONSTRAINT DF_Members_Level DEFAULT 'NEWBIE',
        Tier NVARCHAR(50) NOT NULL CONSTRAINT DF_Members_Tier DEFAULT 'Bronze',
        TotalSpent DECIMAL(18,2) NOT NULL CONSTRAINT DF_Members_TotalSpent DEFAULT 0,
        TotalHoursPurchased DECIMAL(18,2) NOT NULL CONSTRAINT DF_Members_TotalHoursPurchased DEFAULT 0,
        IsFixed BIT NOT NULL CONSTRAINT DF_Members_IsFixed DEFAULT 0,
        CreatedAt DATETIME NOT NULL CONSTRAINT DF_Members_CreatedAt DEFAULT GETDATE()
    );
END
GO

IF OBJECT_ID('dbo.Bookings','U') IS NULL
BEGIN
    CREATE TABLE dbo.Bookings (
        BookingID INT IDENTITY(1,1) PRIMARY KEY,
        CourtID INT NOT NULL,
        MemberID INT NULL,
        GuestName NVARCHAR(100) NULL,
        Note NVARCHAR(200) NULL,
        StartTime DATETIME NOT NULL,
        EndTime DATETIME NOT NULL,
        Status NVARCHAR(50) NOT NULL CONSTRAINT DF_Bookings_Status DEFAULT 'Confirmed'
    );
END
GO

IF OBJECT_ID('dbo.Products','U') IS NULL
BEGIN
    CREATE TABLE dbo.Products (
        ProductID INT IDENTITY(1,1) PRIMARY KEY,
        SKU NVARCHAR(50) NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        Category NVARCHAR(50) NOT NULL,
        Price DECIMAL(18,2) NOT NULL,
        StockQuantity INT NOT NULL CONSTRAINT DF_Products_Stock DEFAULT 0,
        MinThreshold INT NOT NULL CONSTRAINT DF_Products_MinThreshold DEFAULT 5
    );
END
GO

IF OBJECT_ID('dbo.Invoices','U') IS NULL
BEGIN
    CREATE TABLE dbo.Invoices (
        InvoiceID INT IDENTITY(1,1) PRIMARY KEY,
        MemberID INT NULL,
        TotalAmount DECIMAL(18,2) NOT NULL,
        DiscountAmount DECIMAL(18,2) NOT NULL CONSTRAINT DF_Invoices_Discount DEFAULT 0,
        FinalAmount DECIMAL(18,2) NOT NULL,
        PaymentMethod NVARCHAR(50) NOT NULL,
        CreatedAt DATETIME NOT NULL CONSTRAINT DF_Invoices_CreatedAt DEFAULT GETDATE()
    );
END
GO

IF OBJECT_ID('dbo.InvoiceDetails','U') IS NULL
BEGIN
    CREATE TABLE dbo.InvoiceDetails (
        DetailID INT IDENTITY(1,1) PRIMARY KEY,
        InvoiceID INT NOT NULL,
        ProductID INT NULL,
        BookingID INT NULL,
        Quantity INT NOT NULL CONSTRAINT DF_InvoiceDetails_Qty DEFAULT 1,
        UnitPrice DECIMAL(18,2) NOT NULL
    );
END
GO

IF OBJECT_ID('dbo.SystemLogs','U') IS NULL
BEGIN
    CREATE TABLE dbo.SystemLogs (
        LogID INT IDENTITY(1,1) PRIMARY KEY,
        EventDesc NVARCHAR(200) NOT NULL,
        SubDesc NVARCHAR(200) NULL,
        CreatedAt DATETIME NOT NULL CONSTRAINT DF_SystemLogs_CreatedAt DEFAULT GETDATE()
    );
END
GO

IF OBJECT_ID('dbo.StaffAccounts','U') IS NULL
BEGIN
    CREATE TABLE dbo.StaffAccounts (
        AccountID INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL,
        Email NVARCHAR(120) NULL,
        Phone NVARCHAR(30) NULL,
        FullName NVARCHAR(120) NULL,
        PasswordHash VARBINARY(32) NOT NULL,
        PasswordSalt VARBINARY(16) NOT NULL,
        Role NVARCHAR(30) NOT NULL CONSTRAINT DF_StaffAccounts_Role DEFAULT 'Staff',
        IsActive BIT NOT NULL CONSTRAINT DF_StaffAccounts_IsActive DEFAULT 1,
        FailedLoginCount INT NOT NULL CONSTRAINT DF_StaffAccounts_FailedLoginCount DEFAULT 0,
        LockoutUntil DATETIME NULL,
        LastFailedLoginAt DATETIME NULL,
        CreatedAt DATETIME NOT NULL CONSTRAINT DF_StaffAccounts_CreatedAt DEFAULT GETDATE()
    );
END
GO

IF OBJECT_ID('dbo.FK_Bookings_Courts','F') IS NULL
BEGIN
    ALTER TABLE dbo.Bookings
    ADD CONSTRAINT FK_Bookings_Courts FOREIGN KEY (CourtID) REFERENCES dbo.Courts(CourtID);
END
GO

IF OBJECT_ID('dbo.FK_Bookings_Members','F') IS NULL
BEGIN
    ALTER TABLE dbo.Bookings
    ADD CONSTRAINT FK_Bookings_Members FOREIGN KEY (MemberID) REFERENCES dbo.Members(MemberID);
END
GO

IF OBJECT_ID('dbo.FK_Invoices_Members','F') IS NULL
BEGIN
    ALTER TABLE dbo.Invoices
    ADD CONSTRAINT FK_Invoices_Members FOREIGN KEY (MemberID) REFERENCES dbo.Members(MemberID);
END
GO

IF OBJECT_ID('dbo.FK_InvoiceDetails_Invoices','F') IS NULL
BEGIN
    ALTER TABLE dbo.InvoiceDetails
    ADD CONSTRAINT FK_InvoiceDetails_Invoices FOREIGN KEY (InvoiceID) REFERENCES dbo.Invoices(InvoiceID);
END
GO

IF OBJECT_ID('dbo.FK_InvoiceDetails_Products','F') IS NULL
BEGIN
    ALTER TABLE dbo.InvoiceDetails
    ADD CONSTRAINT FK_InvoiceDetails_Products FOREIGN KEY (ProductID) REFERENCES dbo.Products(ProductID);
END
GO

IF OBJECT_ID('dbo.FK_InvoiceDetails_Bookings','F') IS NULL
BEGIN
    ALTER TABLE dbo.InvoiceDetails
    ADD CONSTRAINT FK_InvoiceDetails_Bookings FOREIGN KEY (BookingID) REFERENCES dbo.Bookings(BookingID);
END
GO

IF OBJECT_ID('dbo.Products','U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Products_SKU' AND object_id = OBJECT_ID('dbo.Products','U'))
BEGIN
    CREATE UNIQUE INDEX UX_Products_SKU ON dbo.Products(SKU);
END
GO

IF OBJECT_ID('dbo.StaffAccounts','U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_StaffAccounts_Username' AND object_id = OBJECT_ID('dbo.StaffAccounts','U'))
BEGIN
    CREATE UNIQUE INDEX UX_StaffAccounts_Username ON dbo.StaffAccounts(Username);
END
GO

IF OBJECT_ID('dbo.sp_CreateBooking','P') IS NULL
BEGIN
    EXEC(N'
CREATE PROCEDURE dbo.sp_CreateBooking
    @CourtID INT,
    @MemberID INT = NULL,
    @GuestName NVARCHAR(100) = NULL,
    @Note NVARCHAR(200) = NULL,
    @StartTime DATETIME,
    @EndTime DATETIME,
    @Status NVARCHAR(50) = ''Confirmed''
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM dbo.Bookings
        WHERE CourtID = @CourtID
          AND Status != ''Cancelled''
          AND (StartTime < @EndTime AND EndTime > @StartTime)
    )
    BEGIN
        RAISERROR(''Court is already booked for this time slot.'', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.Bookings (CourtID, MemberID, GuestName, Note, StartTime, EndTime, Status)
    VALUES (@CourtID, @MemberID, @GuestName, @Note, @StartTime, @EndTime, @Status);
END
');
END
GO

IF OBJECT_ID('dbo.trg_ReduceStock','TR') IS NULL
BEGIN
    EXEC(N'
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
      AND ISNULL(p.Category, N'''') <> N''Dịch vụ'';
END
');
END
GO

IF OBJECT_ID('dbo.trg_UpdateMemberTier','TR') IS NULL
BEGIN
    EXEC(N'
CREATE TRIGGER dbo.trg_UpdateMemberTier
ON dbo.Invoices
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE m
    SET m.TotalSpent = m.TotalSpent + i.FinalAmount
    FROM dbo.Members m
    INNER JOIN inserted i ON m.MemberID = i.MemberID
    WHERE i.MemberID IS NOT NULL;

    UPDATE dbo.Members
    SET Tier = CASE
        WHEN TotalSpent >= 10000000 THEN ''Gold''
        WHEN TotalSpent >= 5000000 THEN ''Silver''
        ELSE ''Bronze''
    END
    WHERE MemberID IN (SELECT MemberID FROM inserted WHERE MemberID IS NOT NULL);
END
');
END
GO

BEGIN TRY
    ;WITH CourtParsed AS (
        SELECT
            CourtID,
            CourtNo = TRY_CONVERT(int,
                SUBSTRING(
                    Name,
                    PATINDEX('%[0-9]%', Name),
                    PATINDEX('%[^0-9]%', SUBSTRING(Name, PATINDEX('%[0-9]%', Name), 100) + 'X') - 1
                )
            )
        FROM dbo.Courts
        WHERE Name IS NOT NULL
          AND Name LIKE N'%Pickleball%'
          AND PATINDEX('%[0-9]%', Name) > 0
    )
    UPDATE c
    SET
        Name = CASE
            WHEN p.CourtNo BETWEEN 1 AND 6
                THEN N'Sân Pickleball ' + CAST(p.CourtNo AS NVARCHAR(10)) + N' (Trong nhà)'
            WHEN p.CourtNo BETWEEN 7 AND 12
                THEN N'Sân Pickleball ' + CAST(p.CourtNo AS NVARCHAR(10)) + N' (Ngoài trời)'
            ELSE c.Name
        END,
        CourtType = CASE
            WHEN p.CourtNo BETWEEN 1 AND 6 THEN N'Trong nhà'
            WHEN p.CourtNo BETWEEN 7 AND 12 THEN N'Ngoài trời'
            ELSE c.CourtType
        END,
        HourlyRate = CASE
            WHEN p.CourtNo BETWEEN 1 AND 6 THEN 180000
            WHEN p.CourtNo BETWEEN 7 AND 12 THEN 150000
            ELSE c.HourlyRate
        END
    FROM dbo.Courts c
    INNER JOIN CourtParsed p ON p.CourtID = c.CourtID
    WHERE p.CourtNo BETWEEN 1 AND 12;

    ;WITH PracticeParsed AS (
        SELECT
            CourtID,
            CourtNo = TRY_CONVERT(int,
                SUBSTRING(
                    Name,
                    PATINDEX('%[0-9]%', Name),
                    PATINDEX('%[^0-9]%', SUBSTRING(Name, PATINDEX('%[0-9]%', Name), 100) + 'X') - 1
                )
            )
        FROM dbo.Courts
        WHERE Name IS NOT NULL
          AND (Name LIKE N'Sân Tập%' OR Name LIKE N'San Tap%')
          AND PATINDEX('%[0-9]%', Name) > 0
    )
    UPDATE c
    SET
        Name = N'Sân Tập ' + CAST(p.CourtNo AS NVARCHAR(10)),
        CourtType = N'Sân tập',
        HourlyRate = 90000
    FROM dbo.Courts c
    INNER JOIN PracticeParsed p ON p.CourtID = c.CourtID
    WHERE p.CourtNo BETWEEN 1 AND 3;
END TRY
BEGIN CATCH
END CATCH
GO

DECLARE @SeedCourts TABLE (
    Name NVARCHAR(100) NOT NULL,
    CourtType NVARCHAR(50) NOT NULL,
    HourlyRate DECIMAL(18,2) NOT NULL
);

INSERT INTO @SeedCourts (Name, CourtType, HourlyRate) VALUES
(N'Sân Pickleball 1 (Trong nhà)',  N'Trong nhà', 180000),
(N'Sân Pickleball 2 (Trong nhà)',  N'Trong nhà', 180000),
(N'Sân Pickleball 3 (Trong nhà)',  N'Trong nhà', 180000),
(N'Sân Pickleball 4 (Trong nhà)',  N'Trong nhà', 180000),
(N'Sân Pickleball 5 (Trong nhà)',  N'Trong nhà', 180000),
(N'Sân Pickleball 6 (Trong nhà)',  N'Trong nhà', 180000),
(N'Sân Pickleball 7 (Ngoài trời)', N'Ngoài trời', 150000),
(N'Sân Pickleball 8 (Ngoài trời)', N'Ngoài trời', 150000),
(N'Sân Pickleball 9 (Ngoài trời)', N'Ngoài trời', 150000),
(N'Sân Pickleball 10 (Ngoài trời)', N'Ngoài trời', 150000),
(N'Sân Pickleball 11 (Ngoài trời)', N'Ngoài trời', 150000),
(N'Sân Pickleball 12 (Ngoài trời)', N'Ngoài trời', 150000),
(N'Sân Tập 1', N'Sân tập', 90000),
(N'Sân Tập 2', N'Sân tập', 90000),
(N'Sân Tập 3', N'Sân tập', 90000);

INSERT INTO dbo.Courts (Name, CourtType, HourlyRate)
SELECT s.Name, s.CourtType, s.HourlyRate
FROM @SeedCourts s
WHERE NOT EXISTS (SELECT 1 FROM dbo.Courts c WHERE c.Name = s.Name);
GO

IF COL_LENGTH('dbo.Members', 'TotalHoursPurchased') IS NULL
BEGIN
    ALTER TABLE dbo.Members ADD TotalHoursPurchased DECIMAL(18,2) NOT NULL DEFAULT 0;
END
GO
IF COL_LENGTH('dbo.Members', 'IsFixed') IS NULL
BEGIN
    ALTER TABLE dbo.Members ADD IsFixed BIT NOT NULL DEFAULT 0;
END
GO
IF COL_LENGTH('dbo.Members', 'CreatedAt') IS NULL
BEGIN
    ALTER TABLE dbo.Members ADD CreatedAt DATETIME NOT NULL CONSTRAINT DF_Members_CreatedAt DEFAULT GETDATE();
END
GO
IF OBJECT_ID('dbo.BookingDetails', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.BookingDetails (
        DetailID INT IDENTITY(1,1) PRIMARY KEY,
        BookingID INT NOT NULL,
        ProductID INT NOT NULL,
        Unit NVARCHAR(50) NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18,2) NOT NULL,
        Total DECIMAL(18,2) NOT NULL
    );
END
GO

/* ================================================================
   B) MIGRATIONS (từ Database/Migrations)
   ================================================================ */

/* 0001__Create_MigrationProof.sql */
IF OBJECT_ID('dbo.MigrationProof','U') IS NULL
BEGIN
    CREATE TABLE dbo.MigrationProof (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_MigrationProof PRIMARY KEY,
        Note NVARCHAR(200) NOT NULL,
        CreatedAt DATETIME NOT NULL CONSTRAINT DF_MigrationProof_CreatedAt DEFAULT(GETDATE())
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.MigrationProof WHERE Note = N'Migration 0001 applied')
BEGIN
    INSERT INTO dbo.MigrationProof (Note)
    VALUES (N'Migration 0001 applied');
END
GO

/* 0002__StaffAccounts_Lockout.sql */
IF COL_LENGTH('dbo.StaffAccounts', 'FailedLoginCount') IS NULL
BEGIN
    ALTER TABLE dbo.StaffAccounts
    ADD FailedLoginCount INT NOT NULL
        CONSTRAINT DF_StaffAccounts_FailedLoginCount_Mig DEFAULT(0);
END
GO

IF COL_LENGTH('dbo.StaffAccounts', 'LockoutUntil') IS NULL
BEGIN
    ALTER TABLE dbo.StaffAccounts
    ADD LockoutUntil DATETIME NULL;
END
GO

IF COL_LENGTH('dbo.StaffAccounts', 'LastFailedLoginAt') IS NULL
BEGIN
    ALTER TABLE dbo.StaffAccounts
    ADD LastFailedLoginAt DATETIME NULL;
END
GO

/* 0003__Members_CreatedAt.sql */
IF COL_LENGTH('dbo.Members', 'CreatedAt') IS NULL
BEGIN
    ALTER TABLE dbo.Members
    ADD CreatedAt DATETIME NOT NULL
        CONSTRAINT DF_Members_CreatedAt_Mig DEFAULT(GETDATE());
END
GO

/* 0004__Bookings_Note.sql */
IF COL_LENGTH('dbo.Bookings', 'Note') IS NULL
BEGIN
    ALTER TABLE dbo.Bookings
    ADD Note NVARCHAR(200) NULL;
END
GO

IF OBJECT_ID('dbo.sp_CreateBooking','P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_CreateBooking AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.sp_CreateBooking
    @CourtID INT,
    @MemberID INT = NULL,
    @GuestName NVARCHAR(100) = NULL,
    @Note NVARCHAR(200) = NULL,
    @StartTime DATETIME,
    @EndTime DATETIME,
    @Status NVARCHAR(50) = 'Confirmed'
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM dbo.Bookings
        WHERE CourtID = @CourtID
          AND Status != 'Cancelled'
          AND (StartTime < @EndTime AND EndTime > @StartTime)
    )
    BEGIN
        RAISERROR('Court is already booked for this time slot.', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.Bookings (CourtID, MemberID, GuestName, Note, StartTime, EndTime, Status)
    VALUES (@CourtID, @MemberID, @GuestName, @Note, @StartTime, @EndTime, @Status);
END
GO

/* 0005__Remove_Seed_Service_Products.sql */
DELETE p
FROM dbo.Products p
WHERE p.SKU IN (N'SVC-RACKET', N'SVC-BALL-BASKET', N'SVC-BALL-MACHINE', N'SVC-BALL-PICK')
  AND NOT EXISTS (
      SELECT 1
      FROM dbo.InvoiceDetails d
      WHERE d.ProductID = p.ProductID
  );
GO

/* 0006__Trigger_ReduceStock_ExcludeServices.sql */
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

/* ================================================================
   C) LEGACY COMPATIBILITY (từ Database/Legacy/migration.sql)
   ================================================================ */

IF COL_LENGTH('dbo.Members', 'TotalHoursPurchased') IS NULL
BEGIN
    ALTER TABLE dbo.Members ADD TotalHoursPurchased DECIMAL(18,2) NOT NULL DEFAULT 0;
END
GO
IF COL_LENGTH('dbo.Members', 'IsFixed') IS NULL
BEGIN
    ALTER TABLE dbo.Members ADD IsFixed BIT NOT NULL DEFAULT 0;
END
GO
IF OBJECT_ID('dbo.BookingDetails', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.BookingDetails (
        DetailID INT IDENTITY(1,1) PRIMARY KEY,
        BookingID INT NOT NULL,
        ProductID INT NOT NULL,
        Unit NVARCHAR(50) NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18,2) NOT NULL,
        Total DECIMAL(18,2) NOT NULL
    );
END
GO

/* ================================================================
   D) TESTER SEED DATA (từ TesterData_Seed.sql)
   ================================================================ */

SET NOCOUNT ON;
PRINT '------ START SEEDING TESTER DATA ------';

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
GO

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
GO

DECLARE @Court1 INT = (SELECT TOP 1 CourtID FROM Courts WHERE CourtType = N'Trong nhà' ORDER BY CourtID);
DECLARE @Court2 INT = (SELECT TOP 1 CourtID FROM Courts WHERE CourtType = N'Ngoài trời' ORDER BY CourtID);
DECLARE @Mem1 INT = (SELECT TOP 1 MemberID FROM Members WHERE Phone = '0900000001');
DECLARE @Mem2 INT = (SELECT TOP 1 MemberID FROM Members WHERE Phone = '0900000002');

IF NOT EXISTS (SELECT 1 FROM dbo.Bookings WHERE GuestName = 'Test Data Mock')
BEGIN
    DECLARE @Date1 DATETIME = DATEADD(day, -1, GETDATE());
    DECLARE @Date2 DATETIME = DATEADD(day, -2, GETDATE());
    DECLARE @Date3 DATETIME = DATEADD(day, -3, GETDATE());

    INSERT INTO dbo.Bookings (CourtID, MemberID, GuestName, StartTime, EndTime, Status)
    VALUES
    (@Court1, @Mem1, 'Test Data Mock', DATEADD(hour, 8, CAST(CAST(@Date1 AS DATE) AS DATETIME)), DATEADD(hour, 10, CAST(CAST(@Date1 AS DATE) AS DATETIME)), 'Confirmed'),
    (@Court2, @Mem2, 'Test Data Mock', DATEADD(hour, 14, CAST(CAST(@Date1 AS DATE) AS DATETIME)), DATEADD(hour, 16, CAST(CAST(@Date1 AS DATE) AS DATETIME)), 'Confirmed'),
    (@Court1, NULL,  'Khách Vãng Lai A', DATEADD(hour, 7, CAST(CAST(@Date2 AS DATE) AS DATETIME)), DATEADD(hour, 9, CAST(CAST(@Date2 AS DATE) AS DATETIME)), 'Confirmed'),
    (@Court2, @Mem1, 'Test Data Mock', DATEADD(hour, 17, CAST(CAST(@Date3 AS DATE) AS DATETIME)), DATEADD(hour, 19, CAST(CAST(@Date3 AS DATE) AS DATETIME)), 'Confirmed');

    DECLARE @Prod1 INT = (SELECT TOP 1 ProductID FROM Products WHERE SKU = 'DRK-001');
    DECLARE @Prod2 INT = (SELECT TOP 1 ProductID FROM Products WHERE SKU = 'SNK-001');

    INSERT INTO dbo.Invoices (MemberID, TotalAmount, DiscountAmount, FinalAmount, PaymentMethod, CreatedAt)
    VALUES (@Mem1, 40000, 0, 40000, 'Banking', @Date1);
    DECLARE @Inv1 INT = SCOPE_IDENTITY();

    INSERT INTO dbo.InvoiceDetails (InvoiceID, ProductID, BookingID, Quantity, UnitPrice)
    VALUES (@Inv1, @Prod1, NULL, 4, 10000);

    INSERT INTO dbo.Invoices (MemberID, TotalAmount, DiscountAmount, FinalAmount, PaymentMethod, CreatedAt)
    VALUES (NULL, 50000, 0, 50000, 'Cash', @Date2);
    DECLARE @Inv2 INT = SCOPE_IDENTITY();

    INSERT INTO dbo.InvoiceDetails (InvoiceID, ProductID, BookingID, Quantity, UnitPrice)
    VALUES (@Inv2, @Prod2, NULL, 2, 25000);

    PRINT 'Inserted mock Bookings, Invoices, and Details.';
END
GO

INSERT INTO dbo.SystemLogs (EventDesc, SubDesc, CreatedAt)
VALUES (N'Cài đặt Mock Data nội bộ hoàn tất', N'TesterData_Seed', GETDATE());
GO

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
GO

PRINT '------ END ALL-IN-ONE DATABASE SCRIPT ------';
GO

/* ================================================================
   E) QUICK CHECK QUERIES (tuỳ chọn)
   ================================================================ */
-- SELECT COUNT(*) AS Courts FROM dbo.Courts;
-- SELECT COUNT(*) AS Members FROM dbo.Members;
-- SELECT COUNT(*) AS Bookings FROM dbo.Bookings;
-- SELECT COUNT(*) AS Products FROM dbo.Products;
-- SELECT COUNT(*) AS Invoices FROM dbo.Invoices;
-- SELECT COUNT(*) AS InvoiceDetails FROM dbo.InvoiceDetails;
-- SELECT COUNT(*) AS StaffAccounts FROM dbo.StaffAccounts;
