CREATE TABLE [Jobs].[Job] (
    -- The Id is the same as the source table's primary key, but it is NOT an IDENTITY column.
    [Id] INT NOT NULL,

    -- Foreign key to the replica Client table.
    [ClientId] INT NOT NULL,

    -- Core business data columns replicated from the source.
    [Code] VARCHAR(20) NOT NULL,
    [Name] VARCHAR(100) NOT NULL,

    -- Stores the ROWVERSION from the source table for concurrency checks.
    [SourceVersion] BINARY(8) NOT NULL,

    -- Metadata column to track when this record was last synchronized.
    [ReplicatedOn] DATETIME NOT NULL CONSTRAINT [DF_Jobs_Job_ReplicatedOn] DEFAULT (GETUTCDATE()),

    -- The primary key for efficient lookups.
    CONSTRAINT [PK_Jobs_Job] PRIMARY KEY CLUSTERED ([Id] ASC)
);

ALTER TABLE [Jobs].[Job] WITH CHECK
ADD CONSTRAINT [FK_Jobs_Job_Client] FOREIGN KEY([ClientId])
REFERENCES [Jobs].[Client] ([Id]);

ALTER TABLE [Jobs].[Job] CHECK CONSTRAINT [FK_Jobs_Job_Client];
