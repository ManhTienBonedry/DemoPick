-- Migration 0011: Add TotalSpent and Tier to Members

-- Thêm cột TotalSpent và Tier
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Members') AND name = 'TotalSpent')
BEGIN
    ALTER TABLE Members ADD TotalSpent DECIMAL(18,2) NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Members') AND name = 'Tier')
BEGIN
    ALTER TABLE Members ADD Tier NVARCHAR(50) NOT NULL DEFAULT 'Basic';
END

GO

-- Tạo trigger để tự động cập nhật TotalSpent và Tier khi một hóa đơn được Insert
IF OBJECT_ID('dbo.TRG_UpdateMemberTier', 'TR') IS NOT NULL
    DROP TRIGGER dbo.TRG_UpdateMemberTier;
GO

CREATE TRIGGER TRG_UpdateMemberTier
ON Invoices
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Tạm thời biến chứa các MemberID bị ảnh hưởng
    DECLARE @AffectedMembers TABLE (MemberID INT);
    
    INSERT INTO @AffectedMembers (MemberID)
    SELECT DISTINCT MemberID FROM inserted WHERE MemberID IS NOT NULL
    UNION
    SELECT DISTINCT MemberID FROM deleted WHERE MemberID IS NOT NULL;

    -- Cập nhật TotalSpent
    UPDATE KH
    SET KH.TotalSpent = ISNULL((SELECT SUM(FinalAmount) FROM Invoices H WHERE H.MemberID = KH.MemberID), 0)
    FROM Members KH
    INNER JOIN @AffectedMembers AC ON KH.MemberID = AC.MemberID;

    -- Cập nhật Tier
    UPDATE KH
    SET KH.Tier = CASE 
        WHEN KH.TotalSpent >= 5000000 THEN 'Gold'     -- 5 triệu -> Gold
        WHEN KH.TotalSpent >= 2000000 THEN 'Silver'   -- 2 triệu -> Silver
        ELSE 'Basic' 
    END
    FROM Members KH
    INNER JOIN @AffectedMembers AC ON KH.MemberID = AC.MemberID;
END
GO
