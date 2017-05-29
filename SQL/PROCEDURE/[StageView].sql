IF OBJECT_ID('[StageView]', 'V') IS NOT NULL
	DROP VIEW [StageView]
GO
CREATE VIEW [StageView]
AS
	SELECT
		[S].[objID],
		[S].[_contractID] [contractID],
		[S].[number],
		[S].[name] [fullName],
		[S].[name0] [name],
		[S].[daysToEnd],
		[S].[start],
		[S].[finish],
		[S].[startReal],
		[S].[finishReal],
		[S].[actDate],
		[S].[cost],
		[S].[financing],
		[S].[paymentUptodate],
		[S].[summary]
	FROM [_Stage] [S]
GO