IF OBJECT_ID('[GetProgram]','P') IS NOT NULL
DROP PROCEDURE [GetProgram]
GO

CREATE PROCEDURE [GetProgram]
	@id OBJID
AS
	SELECT [P].[name],
			[P].[code],
			[PP].[objID] [parentID],
			[PP].[name] [parentName]
	FROM [_Program] [P]
	LEFT JOIN [_Program] [PP] ON [P].[parentID] = [PP].[objID]
	WHERE [P].[objID] = @id
GO
