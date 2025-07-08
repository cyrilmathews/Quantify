-- Create Client table
CREATE TABLE [dbo].[Client] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Code] VARCHAR(20) NOT NULL,
    [Name] VARCHAR(100) NOT NULL,
    [CreatedBy] INT NOT NULL,
    [CreatedOn] DATETIME NOT NULL,
    [UpdatedBy] INT NULL,
    [UpdatedOn] DATETIME NULL,
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
    [AuditAction] VARCHAR(3) NOT NULL
);