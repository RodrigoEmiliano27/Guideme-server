﻿CREATE TABLE [dbo].[tbTagDetalhe]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[ID_TAG] INT NOT NULL,
	[ID_TIPO_TAG] INT NOT NULL,

	FOREIGN KEY ([ID_TAG]) REFERENCES [tbTags]([ID]) ,
	FOREIGN KEY ([ID_TIPO_TAG]) REFERENCES [tbTipoTag]([ID])
)
GO

