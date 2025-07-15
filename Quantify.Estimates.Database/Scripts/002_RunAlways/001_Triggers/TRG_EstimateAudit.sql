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
        [AuditTimestamp]
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
        GETUTCDATE() AS AuditTimestamp
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
        GETUTCDATE() AS AuditTimestamp
    FROM
        deleted AS d
    WHERE
        -- This condition ensures we only select from 'deleted' for a true DELETE operation.
        -- For an UPDATE, the 'inserted' table will contain the matching row,
        -- so this condition will filter out the 'before' image of the update.
        NOT EXISTS (SELECT 1 FROM inserted i WHERE i.Id = d.Id);

END
GO
