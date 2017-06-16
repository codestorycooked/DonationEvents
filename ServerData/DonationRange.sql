CREATE TABLE [dbo].[DonationRange]
(
	[Id] INT NOT NULL PRIMARY KEY IDentity, 
    [EventID] INT NULL, 
    [DonationAmount] MONEY NULL, 
    CONSTRAINT [FK_DonationRange_Events] FOREIGN KEY (EventID) REFERENCES Events(Id)  ON DELETE CASCADE
)
