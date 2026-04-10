-- Add booking note support (idempotent)

/* 1) Add Note column */
IF COL_LENGTH('dbo.Bookings', 'Note') IS NULL
BEGIN
    ALTER TABLE dbo.Bookings
    ADD Note NVARCHAR(200) NULL;
END
GO

/* 2) Ensure sp_CreateBooking supports @Note and writes to Bookings.Note */
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
