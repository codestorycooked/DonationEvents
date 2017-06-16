CREATE TABLE [dbo].[ViewedEvents]
(
	[Id] INT NOT NULL PRIMARY KEY Identity, 
    [EventID] INT NULL , 
    [Viewed] BIT NULL, 
    CONSTRAINT [FK_ViewedEvents_Evens] FOREIGN KEY (eventId) REFERENCES Events(Id)
)
