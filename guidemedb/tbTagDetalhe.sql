CREATE TABLE [dbo].[tbTagDetalhe]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[ID_TAG] INT NOT NULL,
	[ID_TIPO_TAG] INT NOT NULL,
	[TAG_PAI] INT NULL,

	FOREIGN KEY ([ID_TAG]) REFERENCES [tbTags]([ID]) ,
	FOREIGN KEY ([ID_TIPO_TAG]) REFERENCES [tbTipoTag]([ID]) ,
	FOREIGN KEY ([TAG_PAI]) REFERENCES [tbTags]([ID])
)
GO

CREATE TRIGGER tr_DeleteTag
ON [tbTags]
INSTEAD OF DELETE
AS
BEGIN
    -- Delete records from [tbTagDetalhe] where ID_TAG matches the deleted tag ID
    DELETE FROM [tbTagDetalhe]
    WHERE [ID_TAG] IN (SELECT [ID] FROM DELETED);

    -- Set [TAG_PAI] to NULL in [tbTagDetalhe] for rows referencing the deleted tag
    UPDATE [tbTagDetalhe]
    SET [TAG_PAI] = NULL
    WHERE [TAG_PAI] IN (SELECT [ID] FROM DELETED);

    -- Perform the actual DELETE operation on [tbTags]
    DELETE FROM [tbTags]
    WHERE [ID] IN (SELECT [ID] FROM DELETED);
END;
