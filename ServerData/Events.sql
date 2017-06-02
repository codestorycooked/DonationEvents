CREATE TABLE [dbo].[Events]
(
	[Id] INT NOT NULL PRIMARY KEY Identity, 
    [EventName] NVARCHAR(50) NULL, 
    [EventDescription] NVARCHAR(50) NULL, 
    [DonatedValue] MONEY NULL, 
    [DateAdded] DATETIME NULL
)
