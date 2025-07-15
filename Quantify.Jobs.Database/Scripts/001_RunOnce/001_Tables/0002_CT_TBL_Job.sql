-- Create Job table
CREATE TABLE [dbo].[Job] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [ClientId] INT NOT NULL,
    [Code] VARCHAR(20) NOT NULL CONSTRAINT UQ_Job_Code UNIQUE ([Code]),
    [Name] VARCHAR(100) NOT NULL,
    [CreatedBy] INT NOT NULL CONSTRAINT DF_Job_CreatedBy DEFAULT 1,
    [CreatedOn] DATETIME NOT NULL CONSTRAINT DF_Job_CreatedOn DEFAULT GETUTCDATE(),
    [UpdatedBy] INT NULL CONSTRAINT DF_Job_UpdatedBy DEFAULT 1,
    [UpdatedOn] DATETIME NULL CONSTRAINT DF_Job_UpdatedOn DEFAULT GETUTCDATE(),
    [Version] ROWVERSION NOT NULL
    CONSTRAINT FK_Job_Client FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Client]([Id])
);

-- Create JobAudit table
CREATE TABLE [dbo].[JobAudit] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [JobId] INT NOT NULL,
    [ClientId] INT NOT NULL,
    [Code] VARCHAR(20) NOT NULL,
    [Name] VARCHAR(100) NOT NULL,
    [CreatedBy] INT NOT NULL,
    [CreatedOn] DATETIME NOT NULL,
    [UpdatedBy] INT NULL,
    [UpdatedOn] DATETIME NULL,
    [AuditAction] VARCHAR(3) NOT NULL,
    [AuditTimestamp] DATETIME NOT NULL DEFAULT GETUTCDATE()
);