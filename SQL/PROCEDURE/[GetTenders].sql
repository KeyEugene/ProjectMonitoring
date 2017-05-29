IF OBJECT_ID('[GetTenders]', 'P') IS NOT NULL
	DROP PROCEDURE [GetTenders]
GO

CREATE PROCEDURE [GetTenders] 
	@xml XML
AS
	IF @xml IS NULL SET @xml = ''
	
	DECLARE @query VARCHAR(MAX) =
	'SELECT
		[T].[objID],
		[T].[purchaseNumber],
		[T].[year],
		[T].[number],
		[T].[cost],
		[T].[name],
		[T].[dateToOpen],
		[T].[dateToExamination],
		[T].[dateToSolution],
		[T].[dateToOpen2],
        [T].[dateToExamination2],
        [T].[dateToExamination3],
        [T].[dateToExamination4],
        [T].[dateToSolution2],
        [T].[dateToSolution3],
        [T].[dateToSolution4],
        [P].[name] [program]
	FROM [_Tender] [T]
	LEFT JOIN [_Program] [P] ON [P].[objID]=[T].[_programID]' + 
	[Search].[CreateWhereStatement]('GetTenders', @xml) 
	
	EXEC sp_SqlExec @query
GO