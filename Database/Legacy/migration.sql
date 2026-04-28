USE PickleBallDB;
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
