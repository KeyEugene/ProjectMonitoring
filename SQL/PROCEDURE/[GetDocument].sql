IF NOT OBJECT_ID('[GetDocument]', 'P') IS NULL
	DROP PROCEDURE [GetDocument]
GO
CREATE PROCEDURE [dbo].[GetDocument]
	@id INT 
AS
	SELECT
		[A].[objID],
		[A].[_contractID],
		[C].[name] [contractName],
		[A].[_applicationTypeID],
		[AT].[name] [applicationTypeName],
		[A].[number],
		[A].[date],
		[A].[based],
		[A].[comment],
		[A].[stage] [stageID],
		[A].[fileName],
		[A].[body],
		[S].[name] [stageName]
	FROM [_Application] [A]
	LEFT JOIN [_ApplicationType] [AT] ON [AT].[objID] = [A].[_applicationTypeID]
	LEFT JOIN [_Contract] [C] ON [C].[objID] = [A].[_contractID]
	LEFT JOIN [_Stage] [S] ON [A].[stage] = [S].[number] AND [A].[_contractID] = [S].[_contractID]
	WHERE [A].[objID] = @id
GO