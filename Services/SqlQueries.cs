namespace DemoPick.Services
{
    internal static class SqlQueries
    {
        internal static class Auth
        {
            internal const string LoginCandidatesByIdentifier = @"
SELECT AccountID, Username, FullName, Role, PasswordHash, PasswordSalt, IsActive,
       FailedLoginCount, LockoutUntil
FROM dbo.StaffAccounts
WHERE (Username = @Id
   OR (Email IS NOT NULL AND Email = @Id)
   OR (Phone IS NOT NULL AND Phone = @Id)
   OR (FullName IS NOT NULL AND LTRIM(RTRIM(FullName)) = @Id)) ";

            internal const string RecordFailedLoginAttempt = @"
UPDATE dbo.StaffAccounts
SET FailedLoginCount = ISNULL(FailedLoginCount, 0) + 1,
    LockoutUntil = CASE WHEN ISNULL(FailedLoginCount, 0) + 1 >= @Max THEN DATEADD(MINUTE, @Minutes, GETDATE()) ELSE LockoutUntil END,
    LastFailedLoginAt = GETDATE()
WHERE AccountID = @Id;";

            internal const string ResetFailedLogin = @"
UPDATE dbo.StaffAccounts
SET FailedLoginCount = 0,
    LockoutUntil = NULL
WHERE AccountID = @Id;";

            internal const string RegisterStaffAccount = @"
INSERT INTO dbo.StaffAccounts (Username, Email, Phone, FullName, PasswordHash, PasswordSalt, Role, IsActive)
VALUES (@Username, @Email, @Phone, @FullName, @Hash, @Salt, @Role, 1)";

            internal const string SeedAdmin = @"
INSERT INTO dbo.StaffAccounts (Username, Email, Phone, FullName, PasswordHash, PasswordSalt, Role, IsActive)
VALUES (@Username, NULL, NULL, N'Quản trị viên', @Hash, @Salt, 'Admin', 1)";

            internal const string ChangePasswordLoadHashSalt = @"
SELECT TOP 1 PasswordHash, PasswordSalt
FROM dbo.StaffAccounts
WHERE AccountID = @Id AND IsActive = 1";

            internal const string ChangePasswordUpdateHashSalt = @"
UPDATE dbo.StaffAccounts
SET PasswordHash = @Hash,
    PasswordSalt = @Salt
WHERE AccountID = @Id";

            internal const string UsernameOrEmailExists = "SELECT COUNT(1) FROM dbo.StaffAccounts WHERE Username = @U OR (Email IS NOT NULL AND Email = @E)";

            internal const string StaffAccountsCount = "SELECT COUNT(1) FROM dbo.StaffAccounts";
        }

        internal static class Dashboard
        {
            internal const string Metrics = @"
DECLARE @posRev DECIMAL(18,2) = ISNULL((SELECT SUM(FinalAmount) FROM Invoices), 0);
DECLARE @courtRev DECIMAL(18,2) = ISNULL((
    SELECT SUM((DATEDIFF(minute, B.StartTime, B.EndTime) / 60.0) * C.HourlyRate)
    FROM Bookings B
    JOIN Courts C ON B.CourtID = C.CourtID
    WHERE B.Status != 'Cancelled'
      AND B.Status != 'Maintenance'
), 0);
DECLARE @totalRev DECIMAL(18,2) = @posRev + @courtRev;
DECLARE @totalCust INT = (SELECT COUNT(*) FROM Members);
DECLARE @total INT = (SELECT COUNT(*) * 18 FROM Courts WHERE (Status = 'Active' OR Status IS NULL OR LTRIM(RTRIM(Status)) = ''));
DECLARE @booked DECIMAL(18,2) = (
    SELECT ISNULL(SUM(DATEDIFF(minute, StartTime, EndTime) / 60.0), 0)
    FROM Bookings
    WHERE CAST(StartTime AS DATE) = CAST(GETDATE() AS DATE)
      AND Status != 'Cancelled'
      AND Status != 'Maintenance'
);
DECLARE @occ INT = CASE WHEN @total = 0 THEN 0 ELSE CAST((@booked * 100.0 / @total) AS INT) END;
DECLARE @pos INT = (SELECT COUNT(*) FROM Invoices);

SELECT @totalRev as Rev, @totalCust as Cust, @occ as Occ, @pos as POS;";

            internal const string RevenueTrendLast7Days = @"
WITH Last7Days AS (
    SELECT CAST(GETDATE() - 6 AS DATE) as Dt UNION ALL
    SELECT CAST(GETDATE() - 5 AS DATE) UNION ALL
    SELECT CAST(GETDATE() - 4 AS DATE) UNION ALL
    SELECT CAST(GETDATE() - 3 AS DATE) UNION ALL
    SELECT CAST(GETDATE() - 2 AS DATE) UNION ALL
    SELECT CAST(GETDATE() - 1 AS DATE) UNION ALL
    SELECT CAST(GETDATE() AS DATE)
)
SELECT 
    FORMAT(D.Dt, 'dd/MM') as Label,
    ISNULL(SUM(I.FinalAmount), 0) + ISNULL(SUM(BR.CourtRevenue), 0) as Revenue
FROM Last7Days D
LEFT JOIN Invoices I ON CAST(I.CreatedAt AS DATE) = D.Dt
LEFT JOIN (
    SELECT
        CAST(B.StartTime AS DATE) as Dt,
        SUM((DATEDIFF(minute, B.StartTime, B.EndTime) / 60.0) * C.HourlyRate) as CourtRevenue
    FROM Bookings B
    JOIN Courts C ON B.CourtID = C.CourtID
    WHERE B.Status != 'Cancelled'
      AND B.Status != 'Maintenance'
    GROUP BY CAST(B.StartTime AS DATE)
) BR ON BR.Dt = D.Dt
GROUP BY D.Dt
ORDER BY D.Dt";

            internal const string TopCourtsRevenue = @"
SELECT TOP 4
    C.Name,
    ISNULL(SUM(CASE
        WHEN B.BookingID IS NULL THEN 0
        ELSE (DATEDIFF(minute, B.StartTime, B.EndTime) / 60.0) * C.HourlyRate
    END), 0) as Rev
FROM Courts C
LEFT JOIN Bookings B ON C.CourtID = B.CourtID AND B.Status != 'Cancelled' AND B.Status != 'Maintenance'
GROUP BY C.Name
ORDER BY Rev DESC";

            internal const string RecentActivity = @"
SELECT TOP (@Take)
    '#BK' + CAST(B.BookingID as VARCHAR) as Code,
    C.Name as CourtName,
    COALESCE(M.FullName, NULLIF(LTRIM(RTRIM(B.GuestName)), ''), N'Khách lẻ') as CustomerName,
    FORMAT(B.StartTime, 'dd/MM/yyyy HH:mm') as TimeText,
    B.Status as Status,
    CAST(ISNULL((DATEDIFF(minute, B.StartTime, B.EndTime) / 60.0) * C.HourlyRate, 0) AS DECIMAL(18,2)) as Amount
FROM Bookings B
JOIN Courts C ON B.CourtID = C.CourtID
LEFT JOIN Members M ON B.MemberID = M.MemberID
WHERE B.Status = 'Paid'
  AND ISNULL(LTRIM(RTRIM(B.GuestName)), '') NOT LIKE 'SMOKE%'
  AND ISNULL(LTRIM(RTRIM(M.FullName)), '') NOT LIKE 'SMOKE%'
ORDER BY B.StartTime DESC";
        }

        internal static class Customer
        {
            internal const string AllCustomers = "SELECT MemberID, FullName, Phone, TotalHoursPurchased, IsFixed, TotalSpent, CreatedAt FROM Members ORDER BY CreatedAt DESC";

            internal const string RevenueSummary = "SELECT COUNT(*) AS Cnt, ISNULL(SUM(TotalSpent), 0) as Rev FROM Members";

            internal const string FindCheckoutCustomer = "SELECT TOP 1 MemberID, FullName, Tier, IsFixed FROM Members WHERE Phone = @Phone OR CAST(MemberID as VARCHAR(20)) = @Qid";

            internal const string TierCounts = @"
SELECT 
    SUM(CASE WHEN IsFixed = 1 THEN 1 ELSE 0 END) as CntFixed,
    SUM(CASE WHEN IsFixed = 0 OR IsFixed IS NULL THEN 1 ELSE 0 END) as CntWalkin
FROM Members";

            internal const string TodayOccupancyPct = @"
DECLARE @total INT = (SELECT COUNT(*) * 18 FROM Courts WHERE (Status = 'Active' OR Status IS NULL OR LTRIM(RTRIM(Status)) = ''));
DECLARE @booked DECIMAL(18,2) = (
    SELECT ISNULL(SUM(DATEDIFF(minute, StartTime, EndTime)/60.0),0)
    FROM Bookings
    WHERE CAST(StartTime as DATE) = CAST(GETDATE() as DATE)
      AND Status != 'Cancelled'
      AND Status != 'Maintenance'
);
SELECT CASE WHEN @total = 0 THEN 0 ELSE CAST((@booked * 100.0 / @total) AS INT) END;";
        }

        internal static class Inventory
        {
            internal const string InsertProduct = @"
INSERT INTO Products (SKU, Name, Category, Price, StockQuantity, MinThreshold)
VALUES (@SKU, @Name, @Category, @Price, @StockQuantity, @MinThreshold)";

            internal const string ProductCategories = @"
SELECT DISTINCT Category
FROM Products
WHERE Category IS NOT NULL
    AND LTRIM(RTRIM(Category)) <> ''
    AND Category <> N'Dịch vụ đi kèm'
    AND SKU NOT LIKE N'SVC-%'
ORDER BY Category";

            internal const string ProductsCatalog = @"
SELECT ProductID, Name, Price, Category
FROM Products
WHERE Category <> N'Dịch vụ đi kèm'
    AND SKU NOT LIKE N'SVC-%'
ORDER BY ProductID DESC";

            internal const string ProductsForDeletion = @"
SELECT ProductID, SKU, Name, Category, Price, StockQuantity
FROM Products
ORDER BY ProductID DESC";

            internal const string InventoryKpis = @"
SELECT 
    ISNULL(SUM(Price * StockQuantity), 0) as TotalVal,
    (SELECT COUNT(*) FROM Products WHERE StockQuantity <= MinThreshold AND Category != N'Dịch vụ đi kèm') as CriticalItems,
    (SELECT ISNULL(SUM(Quantity), 0) FROM InvoiceDetails) as Sales,
    (SELECT COUNT(*) FROM Invoices) as InvoicesCount
FROM Products WHERE Category != N'Dịch vụ đi kèm'";

            internal const string InventoryItems = "SELECT ProductID, SKU, Name, Category, StockQuantity, MinThreshold, Price FROM Products WHERE Category != N'Dịch vụ đi kèm'";

            internal const string RecentTransactions = @"
SELECT TOP 10 EventDesc, SubDesc, CreatedAt
FROM dbo.SystemLogs
WHERE EventDesc IN (N'Nhập Kho Trực Tiếp', N'POS Checkout')
ORDER BY CreatedAt DESC";

                internal const string InsertSystemLog = "INSERT INTO SystemLogs (EventDesc, SubDesc) VALUES (@EventDesc, @SubDesc)";

                internal const string ProductNameById = "SELECT TOP 1 Name FROM Products WHERE ProductID = @ProductID";

                internal const string InvoiceDetailsCountByProductId = "SELECT COUNT(1) FROM InvoiceDetails WHERE ProductID = @ProductID";

                internal const string DeleteProductById = "DELETE FROM Products WHERE ProductID = @ProductID";

                internal const string InvoiceExistsCount = "SELECT COUNT(1) FROM dbo.Invoices WHERE InvoiceID = @Id";
        }

            internal static class Pos
            {
                internal const string InsertWalkinMemberReturnId = @"
        INSERT INTO dbo.Members (FullName, Phone, IsFixed)
        VALUES (@FullName, @Phone, 0);
        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                internal const string InsertInvoiceReturnId = @"
        INSERT INTO Invoices (MemberID, TotalAmount, DiscountAmount, FinalAmount, PaymentMethod)
        VALUES (@MemberID, @TotalAmount, @DiscountAmount, @FinalAmount, @PaymentMethod);
        SELECT CAST(SCOPE_IDENTITY() AS INT);";
            }

        internal static class Report
        {
            internal const string TopCourts = @"
SELECT 
    c.Name as CourtName, 
    c.CourtType as CourtType,
    ISNULL(SUM(DATEDIFF(minute, b.StartTime, b.EndTime)), 0) as BookedMinutes,
    ISNULL(SUM(DATEDIFF(minute, b.StartTime, b.EndTime) / 60.0 * c.HourlyRate), 0) as Revenue
FROM Courts c
LEFT JOIN Bookings b 
    ON c.CourtID = b.CourtID
    AND b.Status != 'Cancelled'
    AND b.Status != 'Maintenance'
    AND (@From IS NULL OR b.StartTime >= @From)
    AND (@To IS NULL OR b.StartTime < @To)
GROUP BY c.CourtID, c.Name, c.CourtType
ORDER BY Revenue DESC";

            internal const string Kpis = @"
DECLARE @currStart DATETIME = @FromStart;
DECLARE @currEnd DATETIME = @ToExclusive;
DECLARE @prevStart DATETIME = DATEADD(DAY, -@Days, @currStart);
DECLARE @prevEnd DATETIME = @currStart;

DECLARE @activeCourts INT = (SELECT COUNT(*) FROM Courts WHERE (Status = 'Active' OR Status IS NULL OR LTRIM(RTRIM(Status)) = ''));
DECLARE @capacityHours DECIMAL(18,2) = @activeCourts * 18.0 * @Days;

DECLARE @currPosRev DECIMAL(18,2) = ISNULL((SELECT SUM(FinalAmount) FROM Invoices WHERE CreatedAt >= @currStart AND CreatedAt < @currEnd), 0);
DECLARE @prevPosRev DECIMAL(18,2) = ISNULL((SELECT SUM(FinalAmount) FROM Invoices WHERE CreatedAt >= @prevStart AND CreatedAt < @prevEnd), 0);

DECLARE @currCourtRev DECIMAL(18,2) = ISNULL((
    SELECT SUM((DATEDIFF(minute, B.StartTime, B.EndTime) / 60.0) * C.HourlyRate)
    FROM Bookings B
    JOIN Courts C ON B.CourtID = C.CourtID
    WHERE B.Status != 'Cancelled'
      AND B.Status != 'Maintenance'
      AND B.StartTime >= @currStart AND B.StartTime < @currEnd
), 0);
DECLARE @prevCourtRev DECIMAL(18,2) = ISNULL((
    SELECT SUM((DATEDIFF(minute, B.StartTime, B.EndTime) / 60.0) * C.HourlyRate)
    FROM Bookings B
    JOIN Courts C ON B.CourtID = C.CourtID
    WHERE B.Status != 'Cancelled'
      AND B.Status != 'Maintenance'
      AND B.StartTime >= @prevStart AND B.StartTime < @prevEnd
), 0);

DECLARE @currRev DECIMAL(18,2) = @currPosRev + @currCourtRev;
DECLARE @prevRev DECIMAL(18,2) = @prevPosRev + @prevCourtRev;

DECLARE @currBookedHours DECIMAL(18,2) = ISNULL((
    SELECT SUM(DATEDIFF(minute, StartTime, EndTime) / 60.0)
    FROM Bookings
    WHERE Status != 'Cancelled'
      AND Status != 'Maintenance'
      AND StartTime >= @currStart AND StartTime < @currEnd
), 0);
DECLARE @prevBookedHours DECIMAL(18,2) = ISNULL((
    SELECT SUM(DATEDIFF(minute, StartTime, EndTime) / 60.0)
    FROM Bookings
    WHERE Status != 'Cancelled'
      AND Status != 'Maintenance'
      AND StartTime >= @prevStart AND StartTime < @prevEnd
), 0);

DECLARE @currOcc DECIMAL(18,2) = CASE WHEN @capacityHours = 0 THEN 0 ELSE (@currBookedHours * 100.0 / @capacityHours) END;
DECLARE @prevOcc DECIMAL(18,2) = CASE WHEN @capacityHours = 0 THEN 0 ELSE (@prevBookedHours * 100.0 / @capacityHours) END;

;WITH Activity AS (
    SELECT MemberID, CreatedAt AS At
    FROM Invoices
    WHERE MemberID IS NOT NULL AND CreatedAt < @currEnd
    UNION ALL
    SELECT MemberID, StartTime AS At
    FROM Bookings
    WHERE MemberID IS NOT NULL AND Status != 'Cancelled' AND Status != 'Maintenance' AND StartTime < @currEnd
), FirstActivity AS (
    SELECT MemberID, MIN(At) AS FirstAt
    FROM Activity
    GROUP BY MemberID
)
SELECT
    @currRev AS CurrRev,
    @prevRev AS PrevRev,
    @currOcc AS CurrOcc,
    @prevOcc AS PrevOcc,
    (SELECT COUNT(*) FROM FirstActivity WHERE FirstAt >= @currStart AND FirstAt < @currEnd) AS CurrNewCust,
    (SELECT COUNT(*) FROM FirstActivity WHERE FirstAt >= @prevStart AND FirstAt < @prevEnd) AS PrevNewCust;";

            internal const string Trend = @"
;WITH Dates AS (
    SELECT CAST(@FromDate AS DATE) AS Dt
    UNION ALL
    SELECT DATEADD(DAY, 1, Dt)
    FROM Dates
    WHERE Dt < CAST(@ToDate AS DATE)
)
SELECT
    FORMAT(D.Dt, 'dd/MM') as Label,
    ISNULL(SUM(I.FinalAmount), 0) + ISNULL(SUM(BR.CourtRevenue), 0) as Revenue
FROM Dates D
LEFT JOIN Invoices I 
    ON CAST(I.CreatedAt AS DATE) = D.Dt
    AND I.CreatedAt >= @FromStart AND I.CreatedAt < @ToExclusive
LEFT JOIN (
    SELECT
        CAST(B.StartTime AS DATE) as Dt,
        SUM((DATEDIFF(minute, B.StartTime, B.EndTime) / 60.0) * C.HourlyRate) as CourtRevenue
    FROM Bookings B
    JOIN Courts C ON B.CourtID = C.CourtID
    WHERE B.Status != 'Cancelled'
      AND B.Status != 'Maintenance'
      AND B.StartTime >= @FromStart AND B.StartTime < @ToExclusive
    GROUP BY CAST(B.StartTime AS DATE)
) BR ON BR.Dt = D.Dt
GROUP BY D.Dt
ORDER BY D.Dt
OPTION (MAXRECURSION 0);";

            internal const string TopCourtsRevenue = @"
SELECT TOP 4
    C.Name,
    ISNULL(SUM(CASE
        WHEN B.BookingID IS NULL THEN 0
        ELSE (DATEDIFF(minute, B.StartTime, B.EndTime) / 60.0) * C.HourlyRate
    END), 0) as Rev
FROM Courts C
LEFT JOIN Bookings B 
    ON C.CourtID = B.CourtID
    AND B.Status != 'Cancelled'
    AND B.Status != 'Maintenance'
    AND B.StartTime >= @FromStart AND B.StartTime < @ToExclusive
GROUP BY C.Name
ORDER BY Rev DESC";
        }

        internal static class Invoice
        {
            internal const string InvoiceHeader = @";WITH Inv AS (
    SELECT TOP (1)
        i.InvoiceID,
        i.CreatedAt,
        i.MemberID,
        m.FullName AS MemberName,
        i.PaymentMethod,
        i.TotalAmount,
        i.DiscountAmount,
        i.FinalAmount
    FROM dbo.Invoices i
    LEFT JOIN dbo.Members m ON m.MemberID = i.MemberID
    WHERE i.InvoiceID = @InvoiceID
), Bk AS (
    SELECT TOP (1)
        b.StartTime AS BookingStartTime,
        b.EndTime AS BookingEndTime
    FROM dbo.Bookings b
    INNER JOIN dbo.Courts c ON c.CourtID = b.CourtID
    CROSS JOIN Inv
    WHERE @CourtName IS NOT NULL
      AND LTRIM(RTRIM(@CourtName)) <> ''
      AND c.Name = @CourtName
      AND b.Status = 'Paid'
      AND b.EndTime >= DATEADD(MINUTE, -10, Inv.CreatedAt)
      AND b.EndTime <= DATEADD(MINUTE,  10, Inv.CreatedAt)
    ORDER BY ABS(DATEDIFF(SECOND, b.EndTime, Inv.CreatedAt))
)
SELECT
    Inv.*,
    Bk.BookingStartTime,
    Bk.BookingEndTime
FROM Inv
LEFT JOIN Bk ON 1 = 1;";

            internal const string InvoiceLines = @"
SELECT
      CASE
          WHEN d.ProductID IS NULL AND d.BookingID IS NOT NULL THEN N'Tiền sân'
          WHEN d.ProductID IS NULL THEN N'(Dịch vụ)'
          ELSE ISNULL(p.Name, N'(Dịch vụ)')
      END AS ItemName,
      d.Quantity,
      d.UnitPrice,
      CAST(d.Quantity * d.UnitPrice AS DECIMAL(18,2)) AS LineTotal
  FROM dbo.InvoiceDetails d
  LEFT JOIN dbo.Products p ON p.ProductID = d.ProductID
  WHERE d.InvoiceID = @InvoiceID
  ORDER BY d.DetailID ASC";
        }

        internal static class Migrations
        {
            internal const string EnsureMigrationsTableExists = @"
IF OBJECT_ID('dbo.__Migrations','U') IS NULL
BEGIN
    CREATE TABLE dbo.__Migrations (
        MigrationId NVARCHAR(260) NOT NULL,
        AppliedAt DATETIME NOT NULL CONSTRAINT DF___Migrations_AppliedAt DEFAULT(GETDATE()),
        Checksum VARBINARY(32) NULL,
        CONSTRAINT PK___Migrations PRIMARY KEY (MigrationId)
    );
END
";

            internal const string LoadAppliedMigrations = "SELECT MigrationId, Checksum FROM dbo.__Migrations";

            internal const string MarkApplied = "INSERT INTO dbo.__Migrations (MigrationId, Checksum) VALUES (@Id, @Checksum)";
        }
    }
}
