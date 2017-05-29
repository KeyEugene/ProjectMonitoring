IF OBJECT_ID('[GetDocuments]', 'P') IS NOT NULL
	DROP PROCEDURE [GetDocuments]
GO

CREATE PROCEDURE [GetDocuments]
	@xml XML 
AS
	IF @xml IS NULL SET @xml = ''
	
	DECLARE @query VARCHAR(MAX) =
	'SELECT
		[A].[objID],
		[A].[name],
		[A].[_contractID],
		[C].[name] [contractName],
		[AT].[name] [applicationTypeName],
		[A].[number],
		[A].[based],
		[A].[comment],
		[A].[stage],
		[A].[name] [documentName]
	FROM [_Application] [A]
	JOIN [_ApplicationType] [AT] ON [AT].[objID] = [A].[_applicationTypeID]
	JOIN [_Contract] [C] ON [C].[objID] = [A].[_contractID]' +
	[Search].[CreateWhereStatement]('GetDocuments', @xml) 
	
	EXEC sp_SqlExec @query
GO
