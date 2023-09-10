CREATE TABLE [dbo].[tbEstabelecimento]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[NOME] VARCHAR(50) NOT NULL
)
go

CREATE TRIGGER tr_DeleteEstabelecimento
ON [tbEstabelecimento]
INSTEAD OF DELETE
AS
BEGIN
    -- Delete tags associated with the deleted [tbEstabelecimento] from [tbTags]
    DELETE FROM [tbTags]
    WHERE [ID_ESTABELECIMENTO] IN (SELECT [Id] FROM DELETED);

    -- Perform the actual DELETE operation on [tbEstabelecimento]
    DELETE FROM [tbEstabelecimento]
    WHERE [Id] IN (SELECT [Id] FROM DELETED);
END;
