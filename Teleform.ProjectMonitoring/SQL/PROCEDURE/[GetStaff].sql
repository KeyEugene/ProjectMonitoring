IF OBJECT_ID('[GetStaff]', 'P') IS NOT NULL
	DROP PROCEDURE [GetStaff]
GO

CREATE PROCEDURE [GetStaff]
	@xml XML 
AS
	IF @xml IS NULL SET @xml = ''

	DECLARE @query VARCHAR(MAX) = '
		SELECT
			[DP].[objID],
			[P].[objID] [personID],
			[P].[name],
			[P].[genitive],
			[P].[ablative],
			[DP].[email] [divEmail],
			[DP].[phone] [divPhone],
			[POS].[name] [position],
			[DP].[head],
			[POS].[genitive] [role genitive]
		FROM [_Person] [P]
			JOIN [_DivisionPerson] [DP] ON [DP].[_personID] = [P].[objID]
			LEFT JOIN [_Position] [POS] ON [POS].[objID] = [DP].[_personRoleID]' +
		[Search].[CreateWhereStatement]('GetStaff', @xml)
			
	EXEC sp_SqlExec @query
GO
