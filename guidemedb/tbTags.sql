﻿CREATE TABLE [tbTags]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[TAG] VARCHAR(50) UNIQUE NOT NULL,
	[ID_ESTABELECIMENTO] INT NOT NULL,
    [ID_TIPO_TAG] INT NOT NULL,

    FOREIGN KEY ([ID_TIPO_TAG]) REFERENCES [tbTipoTag]([ID]),
	FOREIGN KEY ([ID_ESTABELECIMENTO]) REFERENCES [tbEstabelecimento]([ID]),
)
GO
CREATE  INDEX [IX_tbTags_Column] ON [tbTags] ([TAG])
GO
CREATE TRIGGER tr_DeleteTag
ON [tbTags]
INSTEAD OF DELETE
AS
BEGIN

    DELETE FROM [tbTagsPai]
    WHERE [ID_TAG] IN (SELECT [ID] FROM DELETED);

    DELETE FROM [tbTagsPai]
    WHERE [IDTAG_PAI] IN (SELECT [ID] FROM DELETED);

    -- Perform the actual DELETE operation on [tbTags]
    DELETE FROM [tbTags]
    WHERE [ID] IN (SELECT [ID] FROM DELETED);
END;
