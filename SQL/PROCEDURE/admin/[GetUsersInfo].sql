IF OBJECT_ID('[GetUsersInfo]', 'P') IS NOT NULL
	DROP PROCEDURE [GetUsersInfo]
GO

CREATE PROCEDURE [GetUsersInfo] @personID INT = NULL
AS
	DECLARE @query VARCHAR(2000) = '
	SELECT
		[p].[nameF] [userName],
		[p].[name0] [sname],
		[p].[name1] [name],
		[p].[name2] [mname],
		[p].[ablative],
		[p].[genitive],
		[p].[email],
		[p].[phone],
		[pos].[name] [position],
		[u].[login],
		[u].[pwd],
		[dp].[_personID] [personID],
		[u].[disable]
	FROM [_DivisionPerson] [dp]
	JOIN [_Person] [p] ON [p].[objID] = [dp].[_personID]
	JOIN [_Position] [pos] ON [pos].[objID] = [dp].[_personRoleID]
	JOIN [_Division] [d] ON [d].[objID] = [dp].[_divisionID] AND [d].[objID] = 0
	LEFT JOIN [_User] [u] ON [u].[_personID] = [dp].[_personID]'

	IF @personID IS NOT NULL AND @personID <> 0
		SET @query = @query + ' WHERE [p].[objID] = ' + CONVERT(VARCHAR(5), @personID)

	EXEC sp_sqlexec @query

GO