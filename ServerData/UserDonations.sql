CREATE TABLE [dbo].[UserDonations]
(
	[Id] INT NOT NULL PRIMARY KEY IDentity, 
    [UserID] UNIQUEIDENTIFIER NULL, 
    [EventID] INT NULL, 
    [PayPalResponse] NVARCHAR(50) NULL, 
    [TransactionID] NVARCHAR(50) NULL, 
    [status] NVARCHAR(50) NULL, 
    CONSTRAINT [FK_UserDonations_Events] FOREIGN KEY (EventID) REFERENCES Events(Id)  ON DELETE CASCADE
)
