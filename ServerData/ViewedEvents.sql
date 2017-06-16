CREATE TABLE [dbo].[ViewedEvents]
(
	[Id] INT NOT NULL PRIMARY KEY Identity, 
    [EventID] INT NULL , 
    [Viewed] BIT NULL, 
    [UserID] UNIQUEIDENTIFIER NULL, 
    CONSTRAINT [FK_ViewedEvents_Evens] FOREIGN KEY (EventID) REFERENCES Events(Id)  ON DELETE CASCADE
)
