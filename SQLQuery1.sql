IF OBJECT_ID(N'dbo.Requests', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Requests
    (
        Id       UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [Time]   DATETIME2        NOT NULL,
        [Path]   NVARCHAR(256)    NOT NULL,
        [Login]  NVARCHAR(50)     NULL,
        [Answer] INT              NOT NULL
    );
END