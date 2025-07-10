-- Create Client table
CREATE TABLE [dbo].[Client] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Code] VARCHAR(20) NOT NULL,
    [Name] VARCHAR(100) NOT NULL,
    [CreatedBy] INT NOT NULL CONSTRAINT DF_Client_CreatedBy DEFAULT 1,
    [CreatedOn] DATETIME NOT NULL CONSTRAINT DF_Client_CreatedOn DEFAULT GETUTCDATE(),
    [UpdatedBy] INT NULL CONSTRAINT DF_Client_UpdatedBy DEFAULT 1,
    [UpdatedOn] DATETIME NULL CONSTRAINT DF_Client_UpdatedOn DEFAULT GETUTCDATE(),
    [Version] ROWVERSION NOT NULL
);

-- Create ClientAudit table
CREATE TABLE [dbo].[ClientAudit] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
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