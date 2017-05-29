IF NOT OBJECT_ID('[UpdateStage]', 'P') IS NULL
	DROP PROCEDURE [UpdateStage]
GO

CREATE PROCEDURE [dbo].[UpdateStage]
	@id objID,
	--@contractID ObjID,
	@stageName varchar(255),
	@daysToEnd Int,
	@startReal Date,
	@finishReal Date,
	@actDate Date,
	@cost varchar(64),
	@paymentUptodate Bit,
	@summary varchar(1024)
AS
BEGIN
	UPDATE [_Stage]
	SET
		[name0] = @stageName,
		[daysToEnd] = @daysToEnd,
		[startReal] = @startReal,
		[finishReal] = @finishReal,
		[actDate] = @actDate,
		[cost] = CAST(@cost AS Money),
		[paymentUptodate] = @paymentUptodate,
		[summary] = @summary
	WHERE [objID] = @id--[number] = @id AND [_contractID] = @contractID
END

GO

