﻿CREATE TABLE [dbo].[tbTagsPai]
(
	[ID_TAG] INT NOT NULL,
	[IDTAG_PAI] INT NOT NULL,

	FOREIGN KEY ([ID_TAG]) REFERENCES [tbTags]([ID]) ,
	FOREIGN KEY ([IDTAG_PAI]) REFERENCES [tbTags]([ID]) ,
)