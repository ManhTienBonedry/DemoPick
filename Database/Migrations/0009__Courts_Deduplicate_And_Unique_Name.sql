/*
Deduplicate duplicated courts by normalized Name.
Keep the smallest CourtID, repoint Bookings to the kept CourtID, then remove duplicates.
Also enforce unique court names to prevent future duplicates.
*/

SET NOCOUNT ON;

IF OBJECT_ID('dbo.Courts', 'U') IS NULL
    RETURN;

UPDATE dbo.Courts
SET Name = LTRIM(RTRIM(Name))
WHERE Name IS NOT NULL;

DECLARE @CourtMap TABLE
(
    DuplicateCourtID INT PRIMARY KEY,
    KeepCourtID INT NOT NULL
);

;WITH Ranked AS
(
    SELECT
        CourtID,
        LTRIM(RTRIM(Name)) AS NormalizedName,
        ROW_NUMBER() OVER (PARTITION BY LTRIM(RTRIM(Name)) ORDER BY CourtID ASC) AS RN,
        MIN(CourtID) OVER (PARTITION BY LTRIM(RTRIM(Name))) AS KeepCourtID
    FROM dbo.Courts
    WHERE Name IS NOT NULL
      AND LTRIM(RTRIM(Name)) <> N''
)
INSERT INTO @CourtMap (DuplicateCourtID, KeepCourtID)
SELECT CourtID, KeepCourtID
FROM Ranked
WHERE RN > 1;

IF EXISTS (SELECT 1 FROM @CourtMap)
BEGIN
    UPDATE b
    SET b.CourtID = m.KeepCourtID
    FROM dbo.Bookings b
    INNER JOIN @CourtMap m ON b.CourtID = m.DuplicateCourtID;

    DELETE c
    FROM dbo.Courts c
    INNER JOIN @CourtMap m ON c.CourtID = m.DuplicateCourtID
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.Bookings b
        WHERE b.CourtID = c.CourtID
    );
END

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.Courts')
      AND name = 'UX_Courts_Name'
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_Courts_Name
        ON dbo.Courts(Name)
        WHERE Name IS NOT NULL;
END

GO
