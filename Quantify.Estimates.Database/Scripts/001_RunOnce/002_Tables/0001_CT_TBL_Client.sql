CREATE TABLE [Jobs].[Client] (
    -- The Id is the same as the source table's primary key, but it is NOT an IDENTITY column.
    [Id] INT NOT NULL,

    -- Core business data columns replicated from the source.
    [Code] VARCHAR(20) NOT NULL,
    [Name] VARCHAR(100) NOT NULL,

    -- Stores the ROWVERSION from the source table. This is critical for
    -- optimistic concurrency and ensuring the replica is up-to-date.
    [SourceVersion] BINARY(8) NOT NULL,

    -- Metadata column to track when this record was last synchronized.
    [ReplicatedOn] DATETIME NOT NULL CONSTRAINT [DF_Jobs_Client_ReplicatedOn] DEFAULT (GETUTCDATE()),

    -- The primary key for efficient lookups.
    CONSTRAINT [PK_Jobs_Client] PRIMARY KEY CLUSTERED ([Id] ASC)
);
