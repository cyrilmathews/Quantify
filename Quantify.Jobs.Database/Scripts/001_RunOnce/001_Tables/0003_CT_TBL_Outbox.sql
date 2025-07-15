CREATE TABLE [Outbox] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [EventType] NVARCHAR(255) NOT NULL,
    [EventData] NVARCHAR(MAX) NOT NULL, -- JSON serialized event payload
    [Timestamp] DATETIME2 DEFAULT GETUTCDATE(),
    [IsProcessed] BIT DEFAULT 0,
    [ProcessedDate] DATETIME2 NULL
);