CREATE OR ALTER TRIGGER [dbo].[trg_Client_Audit]
   ON  [dbo].[Client]
   AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- The main insert statement into the audit table.
    INSERT INTO [dbo].[ClientAudit]
    (
        [ClientId],
        [Code],
        [Name],
        [CreatedBy],
        [UpdatedBy],
        [AuditAction] -- The new column to log the action
    )
    -- Part 1: Handle INSERT and UPDATE operations from the 'inserted' table.
    SELECT
        i.[Id],
        i.[Code],
        i.[Name],
        i.[CreatedBy],
        i.[UpdatedBy],
        -- Use a CASE statement to determine the action type.
        -- If a matching row exists in 'deleted', it's an UPDATE.
        -- Otherwise, it's a new INSERT.
        CASE
            WHEN EXISTS (SELECT 1 FROM deleted d WHERE d.Id = i.Id) THEN 'UPD'
            ELSE 'INS'
        END AS AuditAction
    FROM
        inserted AS i

    UNION ALL

    -- Part 2: Handle DELETE operations from the 'deleted' table.
    SELECT
        d.[Id],
        d.[Code],
        d.[Name],
        d.[CreatedBy],
        d.[UpdatedBy],
        'DEL' AS AuditAction -- The action is always 'DEL' for this part.
    FROM
        deleted AS d
    WHERE
        -- This condition ensures we only select from 'deleted' for a true DELETE operation.
        -- For an UPDATE, the 'inserted' table will contain the matching row,
        -- so this condition will filter out the 'before' image of the update.
        NOT EXISTS (SELECT 1 FROM inserted i WHERE i.Id = d.Id);

END
GO