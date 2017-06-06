CREATE TABLE [dbo].[Events]
(
	[Id] INT NOT NULL PRIMARY KEY Identity, 
    [EventName] NVARCHAR(50) NULL, 
    [EventDescription] NVARCHAR(50) NULL, 
    [DateAdded] DATETIME NULL
)
