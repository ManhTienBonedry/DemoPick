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
VALUES (@Username, @Email, NULL, N'Quản trị viên', @Hash, @Salt, 'Admin', 1)";

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
    WHERE B.Status = 'Paid'
      AND NOT EXISTS (
          SELECT 1
          FROM InvoiceDetails D
          WHERE D.BookingID = B.BookingID
      )
), 0);
DECLARE @totalRev DECIMAL(18,2) = @posRev + @courtRev;
DECLARE @totalCust INT = (
    SELECT COUNT(*)
    FROM Members m
    WHERE NOT (
        ISNULL(m.IsFixed, 0) = 0
        AND ISNULL(m.TotalSpent, 0) = 0
        AND ISNULL(m.TotalHoursPurchased, 0) = 0
        AND NOT EXISTS (
            SELECT 1
            FROM Bookings b
            WHERE b.MemberID = m.MemberID
              AND b.Status <> 'Cancelled'
        )
        AND NOT EXISTS (
            SELECT 1
            FROM Invoices i
            WHERE i.MemberID = m.MemberID
        )
    )
);
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
    WHERE B.Status = 'Paid'
      AND NOT EXISTS (
          SELECT 1
          FROM InvoiceDetails D
          WHERE D.BookingID = B.BookingID
      )
    GROUP BY CAST(B.StartTime AS DATE)
) BR ON BR.Dt = D.Dt
GROUP BY D.Dt
ORDER BY D.Dt";

            internal const string TopCourtsRevenue = @"
SELECT TOP 4
    C.Name,
    ISNULL(SUM(CASE
        WHEN B.BookingID IS NULL THEN 0
        WHEN IB.BookingRevenue IS NOT NULL THEN IB.BookingRevenue
        ELSE (DATEDIFF(minute, B.StartTime, B.EndTime) / 60.0) * C.HourlyRate
    END), 0) as Rev
FROM Courts C
LEFT JOIN Bookings B ON C.CourtID = B.CourtID AND B.Status = 'Paid'
LEFT JOIN (
    SELECT
        D.BookingID,
        SUM(ISNULL(D.Quantity, 1) * ISNULL(D.UnitPrice, 0)) as BookingRevenue
    FROM InvoiceDetails D
    WHERE D.BookingID IS NOT NULL
    GROUP BY D.BookingID
) IB ON IB.BookingID = B.BookingID
GROUP BY C.Name
ORDER BY Rev DESC";

            internal const string RecentActivity = @"
;WITH ActivityFeed AS (
    SELECT
        '#BK' + CAST(B.BookingID as VARCHAR) as Code,
        C.Name as CourtName,
        COALESCE(M.FullName, NULLIF(LTRIM(RTRIM(B.GuestName)), ''), N'Khách lẻ') as CustomerName,
        FORMAT(B.StartTime, 'dd/MM/yyyy HH:mm') as TimeText,
        B.Status as Status,
        CAST(ISNULL(IB.BookingRevenue, (DATEDIFF(minute, B.StartTime, B.EndTime) / 60.0) * C.HourlyRate) AS DECIMAL(18,2)) as Amount,
        B.StartTime as ActivityAt
    FROM Bookings B
    JOIN Courts C ON B.CourtID = C.CourtID
    LEFT JOIN Members M ON B.MemberID = M.MemberID
    LEFT JOIN (
        SELECT
            D.BookingID,
            SUM(ISNULL(D.Quantity, 1) * ISNULL(D.UnitPrice, 0)) as BookingRevenue
        FROM InvoiceDetails D
        WHERE D.BookingID IS NOT NULL
        GROUP BY D.BookingID
    ) IB ON IB.BookingID = B.BookingID
    WHERE B.Status = 'Paid'
      AND ISNULL(LTRIM(RTRIM(B.GuestName)), '') NOT LIKE 'SMOKE%'
      AND ISNULL(LTRIM(RTRIM(M.FullName)), '') NOT LIKE 'SMOKE%'

    UNION ALL

    SELECT
        '#INV' as Code,
        N'Kho hàng' as CourtName,
        ISNULL(NULLIF(LTRIM(RTRIM(S.SubDesc)), ''), N'-') as CustomerName,
        FORMAT(S.CreatedAt, 'dd/MM/yyyy HH:mm') as TimeText,
        S.EventDesc as Status,
        CAST(0 AS DECIMAL(18,2)) as Amount,
        S.CreatedAt as ActivityAt
    FROM dbo.SystemLogs S
    WHERE S.EventDesc IN (N'Nhập Kho Trực Tiếp', N'Xóa Sản phẩm Kho')
)
SELECT TOP (@Take)
    Code,
    CourtName,
    CustomerName,
    TimeText,
    Status,
    Amount
FROM ActivityFeed
ORDER BY ActivityAt DESC";
        }

        internal static class Customer
        {
            internal const string AllCustomers = @"
SELECT MemberID, FullName, Phone, TotalHoursPurchased, IsFixed, TotalSpent, Tier, CreatedAt
FROM Members m
WHERE NOT (
    ISNULL(m.IsFixed, 0) = 0
    AND ISNULL(m.TotalSpent, 0) = 0
    AND ISNULL(m.TotalHoursPurchased, 0) = 0
    AND NOT EXISTS (
        SELECT 1
        FROM Bookings b
        WHERE b.MemberID = m.MemberID
          AND b.Status <> 'Cancelled'
    )
    AND NOT EXISTS (
        SELECT 1
        FROM Invoices i
        WHERE i.MemberID = m.MemberID
    )
)
ORDER BY CreatedAt DESC";

            internal const string RevenueSummary = @"
SELECT COUNT(*) AS Cnt, ISNULL(SUM(m.TotalSpent), 0) as Rev
FROM Members m
WHERE NOT (
    ISNULL(m.IsFixed, 0) = 0
    AND ISNULL(m.TotalSpent, 0) = 0
    AND ISNULL(m.TotalHoursPurchased, 0) = 0
    AND NOT EXISTS (
        SELECT 1
        FROM Bookings b
        WHERE b.MemberID = m.MemberID
          AND b.Status <> 'Cancelled'
    )
    AND NOT EXISTS (
        SELECT 1
        FROM Invoices i
        WHERE i.MemberID = m.MemberID
    )
)";

            internal const string FindCheckoutCustomer = "SELECT TOP 1 MemberID, FullName, Tier, IsFixed FROM Members WHERE Phone = @Phone OR CAST(MemberID as VARCHAR(20)) = @Qid";

            internal const string TierCounts = @"
SELECT 
    SUM(CASE WHEN m.IsFixed = 1 THEN 1 ELSE 0 END) as CntFixed,
    SUM(CASE WHEN m.IsFixed = 0 OR m.IsFixed IS NULL THEN 1 ELSE 0 END) as CntWalkin
FROM Members m
WHERE NOT (
    ISNULL(m.IsFixed, 0) = 0
    AND ISNULL(m.TotalSpent, 0) = 0
    AND ISNULL(m.TotalHoursPurchased, 0) = 0
    AND NOT EXISTS (
        SELECT 1
        FROM Bookings b
        WHERE b.MemberID = m.MemberID
          AND b.Status <> 'Cancelled'
    )
    AND NOT EXISTS (
        SELECT 1
        FROM Invoices i
        WHERE i.MemberID = m.MemberID
    )
)";

            internal const string MembershipSummary = @"
SELECT
    SUM(CASE WHEN ISNULL(LTRIM(RTRIM(m.Tier)), '') IN (N'', N'Basic', N'Bronze') THEN 1 ELSE 0 END) AS BasicCount,
    SUM(CASE WHEN ISNULL(LTRIM(RTRIM(m.Tier)), '') = N'Silver' THEN 1 ELSE 0 END) AS SilverCount,
    SUM(CASE WHEN ISNULL(LTRIM(RTRIM(m.Tier)), '') = N'Gold' THEN 1 ELSE 0 END) AS GoldCount,
    SUM(CASE WHEN ISNULL(m.TotalSpent, 0) >= 1500000 AND ISNULL(m.TotalSpent, 0) < 2000000 THEN 1 ELSE 0 END) AS NearSilverCount,
    SUM(CASE WHEN ISNULL(m.TotalSpent, 0) >= 4000000 AND ISNULL(m.TotalSpent, 0) < 5000000 THEN 1 ELSE 0 END) AS NearGoldCount
FROM Members m
WHERE NOT (
    ISNULL(m.IsFixed, 0) = 0
    AND ISNULL(m.TotalSpent, 0) = 0
    AND ISNULL(m.TotalHoursPurchased, 0) = 0
    AND NOT EXISTS (
        SELECT 1
        FROM Bookings b
        WHERE b.MemberID = m.MemberID
          AND b.Status <> 'Cancelled'
    )
    AND NOT EXISTS (
        SELECT 1
        FROM Invoices i
        WHERE i.MemberID = m.MemberID
    )
)";

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
FROM Products
WHERE Category != N'Dịch vụ đi kèm'
    AND SKU NOT LIKE N'SVC-%'
    AND SKU NOT LIKE N'DV_%'";

            internal const string InventoryItems = @"
;WITH Sales14 AS (
    SELECT
        d.ProductID,
        SUM(ISNULL(d.Quantity, 1)) AS SoldLast14Days
    FROM dbo.InvoiceDetails d
    INNER JOIN dbo.Invoices i ON i.InvoiceID = d.InvoiceID
    WHERE d.ProductID IS NOT NULL
      AND i.CreatedAt >= DATEADD(DAY, -14, GETDATE())
    GROUP BY d.ProductID
)
SELECT
    p.ProductID,
    p.SKU,
    p.Name,
    p.Category,
    p.StockQuantity,
    p.MinThreshold,
    p.Price,
    ISNULL(s.SoldLast14Days, 0) AS SoldLast14Days
FROM dbo.Products p
LEFT JOIN Sales14 s ON s.ProductID = p.ProductID
WHERE p.Category != N'Dịch vụ đi kèm'
  AND p.Category != N'Dịch vụ'
  AND p.SKU NOT LIKE N'SVC-%'
  AND p.SKU NOT LIKE N'DV[_]%'
ORDER BY
    CASE
        WHEN p.StockQuantity <= 0 THEN 0
        WHEN p.StockQuantity <= p.MinThreshold THEN 1
        WHEN p.StockQuantity <= p.MinThreshold * 2 THEN 2
        ELSE 3
    END,
    p.StockQuantity ASC,
    p.Name ASC";

            internal const string RecentTransactions = @"
SELECT TOP 10 EventDesc, SubDesc, CreatedAt
FROM dbo.SystemLogs
WHERE EventDesc IN (N'Nhập Kho Trực Tiếp', N'Xóa Sản phẩm Kho')
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
;WITH BookingScope AS (
    SELECT
        c.CourtID,
        c.Name AS CourtName,
        c.CourtType,
        c.HourlyRate,
        b.BookingID,
        b.Status,
        b.StartTime,
        b.EndTime,
        ISNULL(ib.BookingRevenue, CASE WHEN b.Status = 'Paid' THEN (DATEDIFF(minute, b.StartTime, b.EndTime) / 60.0) * c.HourlyRate ELSE 0 END) AS Revenue
    FROM Courts c
    LEFT JOIN Bookings b
        ON c.CourtID = b.CourtID
        AND (@From IS NULL OR b.StartTime >= @From)
        AND (@To IS NULL OR b.StartTime < @To)
    LEFT JOIN (
        SELECT
            D.BookingID,
            SUM(ISNULL(D.Quantity, 1) * ISNULL(D.UnitPrice, 0)) as BookingRevenue
        FROM InvoiceDetails D
        WHERE D.BookingID IS NOT NULL
        GROUP BY D.BookingID
    ) ib ON ib.BookingID = b.BookingID
),
PeakHour AS (
    SELECT
        CourtID,
        DATEPART(HOUR, StartTime) AS PeakHour,
        COUNT(*) AS BookingCount,
        ROW_NUMBER() OVER (PARTITION BY CourtID ORDER BY COUNT(*) DESC, DATEPART(HOUR, StartTime) ASC) AS rn
    FROM BookingScope
    WHERE BookingID IS NOT NULL
      AND Status != 'Cancelled'
      AND Status != 'Maintenance'
    GROUP BY CourtID, DATEPART(HOUR, StartTime)
)
SELECT
    bs.CourtName,
    bs.CourtType,
    ISNULL(SUM(CASE
        WHEN bs.BookingID IS NULL THEN 0
        WHEN bs.Status = 'Cancelled' OR bs.Status = 'Maintenance' THEN 0
        ELSE DATEDIFF(minute, bs.StartTime, bs.EndTime)
    END), 0) AS BookedMinutes,
    ISNULL(SUM(CASE
        WHEN bs.BookingID IS NULL THEN 0
        WHEN bs.Status = 'Cancelled' OR bs.Status = 'Maintenance' THEN 0
        ELSE bs.Revenue
    END), 0) AS Revenue,
    ISNULL(ph.PeakHour, -1) AS PeakHour,
    CAST(CASE
        WHEN SUM(CASE WHEN bs.BookingID IS NOT NULL AND bs.Status != 'Maintenance' THEN 1 ELSE 0 END) = 0 THEN 0
        ELSE SUM(CASE WHEN bs.Status = 'Cancelled' THEN 1 ELSE 0 END) * 100.0
            / SUM(CASE WHEN bs.BookingID IS NOT NULL AND bs.Status != 'Maintenance' THEN 1 ELSE 0 END)
    END AS DECIMAL(18,2)) AS CancelRate
FROM BookingScope bs
LEFT JOIN PeakHour ph ON ph.CourtID = bs.CourtID AND ph.rn = 1
GROUP BY bs.CourtID, bs.CourtName, bs.CourtType, ph.PeakHour
ORDER BY Revenue DESC, BookedMinutes DESC";

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
        WHERE B.Status = 'Paid'
      AND B.StartTime >= @currStart AND B.StartTime < @currEnd
            AND NOT EXISTS (
                    SELECT 1
                    FROM InvoiceDetails D
                    WHERE D.BookingID = B.BookingID
            )
), 0);
DECLARE @prevCourtRev DECIMAL(18,2) = ISNULL((
    SELECT SUM((DATEDIFF(minute, B.StartTime, B.EndTime) / 60.0) * C.HourlyRate)
    FROM Bookings B
    JOIN Courts C ON B.CourtID = C.CourtID
        WHERE B.Status = 'Paid'
      AND B.StartTime >= @prevStart AND B.StartTime < @prevEnd
            AND NOT EXISTS (
                    SELECT 1
                    FROM InvoiceDetails D
                    WHERE D.BookingID = B.BookingID
            )
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
    WHERE B.Status = 'Paid'
      AND B.StartTime >= @FromStart AND B.StartTime < @ToExclusive
      AND NOT EXISTS (
          SELECT 1
          FROM InvoiceDetails D
          WHERE D.BookingID = B.BookingID
      )
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
        WHEN IB.BookingRevenue IS NOT NULL THEN IB.BookingRevenue
        WHEN B.Status = 'Paid' THEN (DATEDIFF(minute, B.StartTime, B.EndTime) / 60.0) * C.HourlyRate
        ELSE 0
    END), 0) as Rev
FROM Courts C
LEFT JOIN Bookings B 
    ON C.CourtID = B.CourtID
    AND B.Status != 'Cancelled'
    AND B.Status != 'Maintenance'
    AND B.StartTime >= @FromStart AND B.StartTime < @ToExclusive
LEFT JOIN (
    SELECT
        D.BookingID,
        SUM(ISNULL(D.Quantity, 1) * ISNULL(D.UnitPrice, 0)) as BookingRevenue
    FROM InvoiceDetails D
    WHERE D.BookingID IS NOT NULL
    GROUP BY D.BookingID
) IB ON IB.BookingID = B.BookingID
GROUP BY C.Name
ORDER BY Rev DESC";

            internal const string BookingHourHeatmap = @"
;WITH Hours AS (
    SELECT 6 AS Hr UNION ALL
    SELECT 7 UNION ALL
    SELECT 8 UNION ALL
    SELECT 9 UNION ALL
    SELECT 10 UNION ALL
    SELECT 11 UNION ALL
    SELECT 12 UNION ALL
    SELECT 13 UNION ALL
    SELECT 14 UNION ALL
    SELECT 15 UNION ALL
    SELECT 16 UNION ALL
    SELECT 17 UNION ALL
    SELECT 18 UNION ALL
    SELECT 19 UNION ALL
    SELECT 20 UNION ALL
    SELECT 21 UNION ALL
    SELECT 22 UNION ALL
    SELECT 23
)
SELECT
    Hr,
    RIGHT('0' + CAST(Hr AS VARCHAR(2)), 2) + ':00' AS Label,
    COUNT(b.BookingID) AS BookingCount
FROM Hours h
LEFT JOIN Bookings b
    ON DATEPART(HOUR, b.StartTime) = h.Hr
    AND b.Status != 'Cancelled'
    AND b.Status != 'Maintenance'
    AND b.StartTime >= @FromStart
    AND b.StartTime < @ToExclusive
GROUP BY Hr
ORDER BY Hr";

            internal const string BookingOps = @"
SELECT
    SUM(CASE WHEN b.Status != 'Maintenance' THEN 1 ELSE 0 END) AS TotalBookings,
    SUM(CASE WHEN b.Status = 'Cancelled' THEN 1 ELSE 0 END) AS CancelledBookings,
    SUM(CASE WHEN b.Status != 'Cancelled' AND b.Status != 'Maintenance' THEN 1 ELSE 0 END) AS ActiveBookings,
    (
        SELECT COUNT(*)
        FROM dbo.SystemLogs s
        WHERE s.EventDesc = N'Doi Ca Booking'
          AND s.CreatedAt >= @FromStart
          AND s.CreatedAt < @ToExclusive
    ) AS ShiftedBookings
FROM dbo.Bookings b
WHERE b.StartTime >= @FromStart
  AND b.StartTime < @ToExclusive";
        }

        internal static class Invoice
        {
            internal const string InvoiceHistory = @"
SELECT TOP (@Take)
    i.InvoiceID,
    i.CreatedAt,
    ISNULL(NULLIF(LTRIM(RTRIM(m.FullName)), ''), N'Khách lẻ') AS CustomerName,
    ISNULL(ca.CourtName, N'') AS CourtName,
    i.FinalAmount,
    ISNULL(i.PaymentMethod, N'') AS PaymentMethod
FROM dbo.Invoices i
LEFT JOIN dbo.Members m ON m.MemberID = i.MemberID
OUTER APPLY (
    SELECT TOP (1) c.Name AS CourtName
    FROM dbo.InvoiceDetails d
    LEFT JOIN dbo.Bookings b ON b.BookingID = d.BookingID
    LEFT JOIN dbo.Courts c ON c.CourtID = b.CourtID
    WHERE d.InvoiceID = i.InvoiceID
      AND d.BookingID IS NOT NULL
    ORDER BY d.DetailID ASC
) ca
WHERE (
    @Keyword IS NULL
    OR LTRIM(RTRIM(@Keyword)) = ''
    OR CAST(i.InvoiceID AS NVARCHAR(20)) LIKE N'%' + @Keyword + N'%'
    OR ISNULL(m.FullName, N'') LIKE N'%' + @Keyword + N'%'
    OR ISNULL(m.Phone, N'') LIKE N'%' + @Keyword + N'%'
    OR ISNULL(ca.CourtName, N'') LIKE N'%' + @Keyword + N'%'
)
ORDER BY i.CreatedAt DESC, i.InvoiceID DESC";

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
