IF OBJECT_ID('[GetContractDocuments]', 'P') IS NOT NULL
	DROP PROCEDURE [GetContractDocuments]
GO

CREATE PROCEDURE [GetContractDocuments]
	@xml XML
AS
	IF @xml IS NULL SET @xml = ''
	
	DECLARE @query VARCHAR(MAX) =
	'SELECT
		[A].[objID],
		[A].[_contractID],
		[C].[name] [contractName],
		[AT].[name] [applicationTypeName],
		[A].[number],
		[A].[based],
		[A].[comment],
		[A].[stage],
		[A].[name]
	FROM [_Application] [A]
	JOIN [_ApplicationType] [AT] ON [AT].[objID] = [A].[_applicationTypeID]
	JOIN [_Contract] [C] ON [C].[objID] = [A].[_contractID]' +
	[Search].[CreateWhereStatement]('GetContractDocuments', @xml) 
	
	EXEC sp_SqlExec @query
