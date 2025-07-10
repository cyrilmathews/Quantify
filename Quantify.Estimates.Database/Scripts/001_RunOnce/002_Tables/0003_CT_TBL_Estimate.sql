CREATE TABLE [dbo].[Estimate] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [JobId] INT NOT NULL,
    [Amount] DECIMAL(18, 2) NOT NULL,
    [CreatedBy] INT NOT NULL CONSTRAINT [DF_Estimate_CreatedBy] DEFAULT 1,
    [CreatedOn] DATETIME NOT NULL CONSTRAINT [DF_Estimate_CreatedOn] DEFAULT GETUTCDATE(),
    [UpdatedBy] INT NULL,
    [UpdatedOn] DATETIME NULL,
    [Version] ROWVERSION NOT NULL,

    CONSTRAINT [PK_Estimate] PRIMARY KEY CLUSTERED ([Id] ASC)
);

ALTER TABLE [dbo].[Estimate] WITH CHECK
ADD CONSTRAINT [FK_Estimate_Job] FOREIGN KEY([JobId])
REFERENCES [Jobs].[Job] ([Id]);

ALTER TABLE [dbo].[Estimate] CHECK CONSTRAINT [FK_Estimate_Job];

CREATE TABLE [dbo].[EstimateAudit] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [EstimateId] INT NOT NULL,
    [JobId] INT NOT NULL,
    [Amount] DECIMAL(18, 2) NOT NULL,
    [CreatedBy] INT NOT NULL,
    [CreatedOn] DATETIME NOT NULL,
    [UpdatedBy] INT NULL,
    [UpdatedOn] DATETIME NULL,
    [AuditAction] VARCHAR(3) NOT NULL,
    [AuditTimestamp] DATETIME NOT NULL DEFAULT GETUTCDATE()
);
