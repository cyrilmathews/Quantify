-- Create Job table
CREATE TABLE [dbo].[Job] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [ClientId] INT NOT NULL,
    [Code] VARCHAR(20) NOT NULL,
    [Name] VARCHAR(100) NOT NULL,
    [CreatedBy] INT NOT NULL,
    [CreatedOn] DATETIME NOT NULL,
    [UpdatedBy] INT NULL,
    [UpdatedOn] DATETIME NULL,
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
    [AuditAction] VARCHAR(3) NOT NULL
);