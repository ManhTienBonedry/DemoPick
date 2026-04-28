IF COL_LENGTH('dbo.Bookings', 'PaymentState') IS NULL
BEGIN
    ALTER TABLE dbo.Bookings
    ADD PaymentState NVARCHAR(50) NOT NULL
        CONSTRAINT DF_Bookings_PaymentState DEFAULT 'PayAtVenue';
END
GO

-- Separate batch: column now exists, safe to reference
IF COL_LENGTH('dbo.Bookings', 'PaymentState') IS NOT NULL
BEGIN
    UPDATE dbo.Bookings
    SET PaymentState = 'PayAtVenue'
    WHERE PaymentState IS NULL OR LTRIM(RTRIM(PaymentState)) = '';
END
GO

IF OBJECT_ID('dbo.sp_CreateBooking', 'P') IS NULL
BEGIN
    EXEC('CREATE PROCEDURE dbo.sp_CreateBooking AS BEGIN SET NOCOUNT ON; END');
END
GO

ALTER PROCEDURE dbo.sp_CreateBooking
    @CourtID INT,
    @MemberID INT = NULL,
    @GuestName NVARCHAR(100) = NULL,
    @Note NVARCHAR(200) = NULL,
    @PaymentState NVARCHAR(50) = 'PayAtVenue',
    @StartTime DATETIME,
    @EndTime DATETIME,
    @Status NVARCHAR(50) = 'Confirmed'
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM dbo.Bookings
        WHERE CourtID = @CourtID
          AND Status <> 'Cancelled'
          AND (StartTime < @EndTime AND EndTime > @StartTime)
    )
    BEGIN
        RAISERROR('Court is already booked for this time slot.', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.Bookings (CourtID, MemberID, GuestName, Note, PaymentState, StartTime, EndTime, Status)
    VALUES (@CourtID, @MemberID, @GuestName, @Note, @PaymentState, @StartTime, @EndTime, @Status);
END
GO
