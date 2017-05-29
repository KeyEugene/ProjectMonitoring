IF OBJECT_ID('[GetRoute]', 'P') IS NOT NULL
	DROP PROC [GetRoute]
GO

CREATE PROC [GetRoute]
	@id ObjID
AS
	SELECT
		[R].[objID],
		[R].[_applicationID],
		[A].[fileName],
		[R].[stateID],
		[AS].[name] [stateName],
		[R].[_divisionID],
		[D].[name] [divisionName],
		[R].[sent],
		[R].[expected],
		[R].[done]
	FROM [_Route] [R]
	JOIN [_Application] [A] ON [A].[objID] = [R].[_applicationID]
	JOIN [_Division] [D] ON [D].[objID] = [R].[_divisionID]
	JOIN [_ApplicationState] [AS] ON [AS].[objID] = [R].[stateID]
	WHERE [R].[objID] = @id
GO