IF OBJECT_ID('[GetHistory]', 'P') IS NOT NULL
	DROP PROCEDURE [GetHistory]
GO

CREATE PROCEDURE [GetHistory]
AS
	SELECT TOP 10 
		[h].[time],
		[s].[name] [status],
		[p].[nameF] [name]
	FROM [Import].[History] [h]
	LEFT JOIN [Import].[State] [s] ON [s].[objID] = [h].[stateID]
	LEFT JOIN [_Person] [p] ON [p].[objID] = [h].[_personID]
	ORDER BY [h].[time] DESC
GO

