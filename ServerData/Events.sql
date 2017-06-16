CREATE TABLE [dbo].[Events]
(
	[Id] INT NOT NULL PRIMARY KEY Identity, 
    [EventName] NVARCHAR(50) NOT NULL, 
    [EventDescription] NVARCHAR(50) NOT NULL, 
    [DateAdded] DATETIME NULL, 
    [PixelLotID] NVARCHAR(50) NOT NULL, 
    [Duration] FLOAT NOT NULL, 
    [IsActive] BIT NOT NULL
)
