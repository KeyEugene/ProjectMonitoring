IF OBJECT_ID('[GetRoutes]', 'P') IS NOT NULL
	DROP PROC [GetRoutes]
GO

CREATE PROC [GetRoutes]
	@xml XML
AS
	IF @xml IS NULL SET @xml = ''

	DECLARE @query VARCHAR(MAX) = '
	SELECT
		[R].[objID],
		[R].[_applicationID],
		[A].[fileName],
		[AS].[name] [stateName],
		[R].[_divisionID],
		[D].[name] [divisionName],
		[R].[sent],
		[R].[expected],
		[R].[done]
	FROM [_Route] [R]
	JOIN [_Application] [A] ON [A].[objID] = [R].[_applicationID]
	JOIN [_Division] [D] ON [D].[objID] = [R].[_divisionID]
	JOIN [_ApplicationState] [AS] ON [AS].[objID] = [R].[stateID]' +
	[Search].[CreateWhereStatement]('GetRoutes', @xml)

	EXEC sp_SqlExec @query
GO

