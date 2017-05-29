IF NOT OBJECT_ID('[GetEmployee]', 'P') IS NULL
	DROP PROCEDURE [GetEmployee]
GO
CREATE PROCEDURE [GetEmployee]
	@id ObjID
AS
	SELECT
		[DP].[objID],
		[DP].[_personID] [personID],
		[DP].[_divisionID] [divisionID],
		[P].[name],
		[DP].[email],
		[DP].[phone],
		[DP].[head],
		[DP].[signerBase],
		[DP].[_personRoleID] [positionID],
		[POS].[name] [position]
	FROM [_DivisionPerson] [DP]
	JOIN [_Person] [P] ON [P].[objID] = [DP].[_personID]
	LEFT JOIN [_Position] [POS] ON [POS].[objID] = [DP].[_personRoleID]
	WHERE [DP].[objID] = @id
GO