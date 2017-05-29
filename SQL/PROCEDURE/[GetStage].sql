IF NOT OBJECT_ID('[GetStage]', 'P') IS NULL
	DROP PROCEDURE [GetStage]
GO

CREATE PROCEDURE [dbo].[GetStage]
	--@id tinyInt,@contractID ObjID
	@id objID
AS
BEGIN
	SELECT
		[S].[objID],
		[S].[name] [stagename],
		[S].[daysToEnd],
		[S].[start],
		[S].[finish],
		[S].[startReal],
		[S].[finishReal],
		[S].[actDate],
		[S].[cost],
		[S].[paymentUptodate],
		[S].[summary],
		[C].[name] [contract]
	FROM [StageView] [S]
	LEFT JOIN [_Contract] [C] ON [C].[objID] = [S].[contractID]
	WHERE [S].[objID] = @id--[S].[number] = @id AND [S].[contractID] = @contractID
	
	
END



GO


