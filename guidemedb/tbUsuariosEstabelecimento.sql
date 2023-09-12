CREATE TABLE [dbo].[tbUsuariosEstabelecimento]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [LOGIN] VARCHAR(MAX) NOT NULL, 
    [SENHA] VARCHAR(MAX) NOT NULL, 
    [ADMINISTRADOR] BIT NOT NULL, 
    [Id_Estabelecimento] INT NOT NULL
)
