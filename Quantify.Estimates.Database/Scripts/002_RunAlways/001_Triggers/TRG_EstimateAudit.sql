CREATE TABLE [dbo].[EstimateAudit] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [EstimateId] INT NOT NULL, -- Renamed from Id to EstimateId to avoid conflict and clearly reference the original table's Id
    [JobId] INT NOT NULL,
    [Amount] DECIMAL(18, 2) NOT NULL,
    [CreatedBy] INT NOT NULL,
    [CreatedOn] DATETIME NOT NULL,
    [UpdatedBy] INT NULL,
    [UpdatedOn] DATETIME NULL,
    -- [Version] ROWVERSION NOT NULL, -- ROWVERSION is not typically audited directly as it's a changing internal value.
                                    -- If you need to track the version, you might store its value as a BIGINT.
                                    -- For audit purposes, the state before/after the change is usually sufficient.
    [AuditAction] VARCHAR(3) NOT NULL, -- 'INS' for Insert, 'UPD' for Update, 'DEL' for Delete
    [AuditTimestamp] DATETIME NOT NULL DEFAULT GETUTCDATE(), -- When the audit record was created
    [AuditUser] INT NULL -- User who performed the audit action (optional, but good practice)
);

GO

CREATE OR ALTER TRIGGER [dbo].[trg_Estimate_Audit]
   ON  [dbo].[Estimate]
   AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- The main insert statement into the audit table.
    INSERT INTO [dbo].[EstimateAudit]
    (
        [EstimateId],
        [JobId],
        [Amount],
        [CreatedBy],
        [CreatedOn],
        [UpdatedBy],
        [UpdatedOn],
        [AuditAction], -- The column to log the action
        [AuditTimestamp],
        [AuditUser]
    )
    -- Part 1: Handle INSERT and UPDATE operations from the 'inserted' table.
    SELECT
        i.[Id],
        i.[JobId],
        i.[Amount],
        i.[CreatedBy],
        i.[CreatedOn],
        i.[UpdatedBy],
        i.[UpdatedOn],
        -- Use a CASE statement to determine the action type.
        -- If a matching row exists in 'deleted', it's an UPDATE.
        -- Otherwise, it's a new INSERT.
        CASE
            WHEN EXISTS (SELECT 1 FROM deleted d WHERE d.Id = i.Id) THEN 'UPD'
            ELSE 'INS'
        END AS AuditAction,
        GETUTCDATE() AS AuditTimestamp,
        -- For AuditUser, you might want to get the actual user performing the action.
        -- If your application sets UpdatedBy/CreatedBy on the original table for the current action,
        -- you could use COALESCE(i.UpdatedBy, i.CreatedBy). Otherwise, it might be NULL or SUSER_SNAME().
        -- For this example, we'll use the UpdatedBy from 'inserted' if available, otherwise CreatedBy.
        COALESCE(i.UpdatedBy, i.CreatedBy) AS AuditUser
    FROM
        inserted AS i

    UNION ALL

    -- Part 2: Handle DELETE operations from the 'deleted' table.
    SELECT
        d.[Id],
        d.[JobId],
        d.[Amount],
        d.[CreatedBy],
        d.[CreatedOn],
        d.[UpdatedBy],
        d.[UpdatedOn],
        'DEL' AS AuditAction, -- The action is always 'DEL' for this part.
        GETUTCDATE() AS AuditTimestamp,
        -- For deleted records, use UpdatedBy if available, otherwise CreatedBy.
        COALESCE(d.UpdatedBy, d.CreatedBy) AS AuditUser
    FROM
        deleted AS d
    WHERE
        -- This condition ensures we only select from 'deleted' for a true DELETE operation.
        -- For an UPDATE, the 'inserted' table will contain the matching row,
        -- so this condition will filter out the 'before' image of the update.
        NOT EXISTS (SELECT 1 FROM inserted i WHERE i.Id = d.Id);

END
GO
