CREATE TABLE [dbo].[Game]
(
	[Id] INT NOT NULL IDENTITY PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Owner] NVARCHAR(50) NOT NULL,
    [Console] NVARCHAR(50) NOT NULL,
    [Price] INT NOT NULL,
    [MaxPlayers] INT NOT NULL,
    [Commentary] NVARCHAR(MAX)
)
