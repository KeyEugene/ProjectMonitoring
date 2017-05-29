IF OBJECT_ID('[GetParameter]','P') IS NOT NULL
DROP PROCEDURE [GetParameter]
GO

CREATE PROCEDURE [GetParameter]
	@id ObjID
AS
	SELECT [P].[value],
			[P].[valueReal],
			[P].[name],
			[P].[number],
			[P].[_contractID] [contractID],
			[C].[name] [contractName],
			[S].[number] [stageNumber],
			[S].[name] [stageName]
	FROM [_Parameter] [P]
	JOIN [_Contract] [C] ON [C].[objID] = [P].[_contractID]
	LEFT JOIN [_Stage] [S] ON [S].[_contractID] = [P].[_contractID] AND [S].[number] = [P].[_stage]
	WHERE [P].[objID] = @id
GO
