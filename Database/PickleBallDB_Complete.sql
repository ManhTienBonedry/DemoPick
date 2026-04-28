/*
PickleBallDB_Complete.sql
Mục tiêu: Tạo đầy đủ cấu trúc CSDL cho PickleBall, bao gồm bảng, ràng buộc, trigger, stored procedure và seed data cơ bản.
Hướng dẫn chạy: Mở file này trong SQL Server Management Studio (SSMS) và chạy vào database "PickleBallDB". File này được thiết kế để có thể chạy nhiều lần mà không gây lỗi (idempotent), nên bạn có thể yên tâm chạy lại sau khi đã có dữ liệu để cập nhật schema nếu cần.
*/

USE [PickleBallDB];
GO

/* 2) Tables */
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

/* 3) Constraints & Indexes (idempotent) */
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

/* 4) Stored procedure (create once if missing) */
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

/* 5) Triggers (create once if missing) */
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

/* 6) Base seed data (idempotent) */
BEGIN TRY
    /* Best effort: normalize older court names/types to the 15-court scheme */
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
    -- ignore
END CATCH

DECLARE @SeedCourts TABLE (
    Name NVARCHAR(100) NOT NULL,
    CourtType NVARCHAR(50) NOT NULL,
    HourlyRate DECIMAL(18,2) NOT NULL
);

/* 15 courts: 6 indoor, 6 outdoor, 3 practice */
INSERT INTO @SeedCourts (Name, CourtType, HourlyRate) VALUES
(N'Sân Pickleball 1 (Trong nhà)',  N'Trong nhà', 180000),
(N'Sân Pickleball 2 (Trong nhà)',  N'Trong nhà', 180000),
(N'Sân Pickleball 3 (Trong nhà)',  N'Trong nhà', 180000),
(N'Sân Pickleball 4 (Trong nhà)',  N'Trong nhà', 180000),
(N'Sân Pickleball 5 (Trong nhà)',  N'Trong nhà', 180000),
(N'Sân Pickleball 6 (Trong nhà)',  N'Trong nhà', 180000),

(N'Sân Pickleball 7 (Ngoài trời)',  N'Ngoài trời', 150000),
(N'Sân Pickleball 8 (Ngoài trời)',  N'Ngoài trời', 150000),
(N'Sân Pickleball 9 (Ngoài trời)',  N'Ngoài trời', 150000),
(N'Sân Pickleball 10 (Ngoài trời)', N'Ngoài trời', 150000),
(N'Sân Pickleball 11 (Ngoài trời)', N'Ngoài trời', 150000),
(N'Sân Pickleball 12 (Ngoài trời)', N'Ngoài trời', 150000),

/* Practice courts: half price */
(N'Sân Tập 1', N'Sân tập', 90000),
(N'Sân Tập 2', N'Sân tập', 90000),
(N'Sân Tập 3', N'Sân tập', 90000);

INSERT INTO dbo.Courts (Name, CourtType, HourlyRate)
SELECT s.Name, s.CourtType, s.HourlyRate
FROM @SeedCourts s
WHERE NOT EXISTS (SELECT 1 FROM dbo.Courts c WHERE c.Name = s.Name);
GO

PRINT N'Database initialized (schema + base seed)';
GO

/*
Optional: Seed admin account
- RELEASE: app KHÔNG tự seed admin để tránh mật khẩu mặc định bị lộ.
- DEBUG: app có thể seed admin (mật khẩu random hoặc từ env var DEMOPICK_BOOTSTRAP_ADMIN_PASSWORD).
- Nếu bạn muốn seed bằng SQL thuần, hãy tạo file riêng trong Database/Migrations (không để trong script này).
*/
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
