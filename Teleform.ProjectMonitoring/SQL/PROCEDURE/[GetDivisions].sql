IF OBJECT_ID('[GetDivisions]', 'P') IS NOT NULL
	DROP PROCEDURE [GetDivisions]
GO

CREATE PROCEDURE [GetDivisions] 
	@xml XML 
AS
	IF @xml IS NULL SET @xml = ''
	
	DECLARE @query VARCHAR(MAX) =
	'SELECT
		[D].[objID],
		[D].[name],
		[D].[_placeID],
		[P].[name] [place],
		[D].[INN],
		[D].[KPP],
		[D].[parentID],
		[DN].[name] [parentName],
		[D].[address],
		[D].[email],
		[D].[fax]
	FROM [_Division] [D] LEFT JOIN [_Division] [DN] ON [DN].[objID] = [D].[parentID]
	LEFT JOIN [_Place] [P] ON [P].[objID] = [D].[_placeID]' +
	[Search].[CreateWhereStatement]('GetDivisions', @xml) 
	
	EXEC sp_SqlExec @query
GO