IF NOT OBJECT_ID('[GetContractStages]', 'P') IS NULL
	DROP PROCEDURE [GetContractStages]
GO
CREATE PROCEDURE [GetContractStages]
	@xml XML
AS	
	IF @xml IS NULL SET @xml = ''
	
	DECLARE @query VARCHAR(MAX) =
	'SELECT
		[S].[objID],
		[S].[number],
		[S].[name],
		[S].[fullName],
		[S].[daysToEnd],
		[S].[start],
		[S].[finish],
		[S].[startReal],
		[S].[finishReal],
		[S].[actDate],
		[S].[cost],
		[S].[paymentUptodate],
		[S].[summary],
		[C].[name] [contract],
		[C].[objID] [contractID]
	FROM [StageView] [S]
	JOIN [_Contract] [C] ON [C].[objID] = [S].[contractID]' +
	[Search].[CreateWhereStatement]('GetContractStages', @xml) 
	
	EXEC sp_SqlExec @query
GO