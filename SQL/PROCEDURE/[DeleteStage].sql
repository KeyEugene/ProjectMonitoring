IF OBJECT_ID('[DeleteStage]','P') IS NOT NULL
	DROP PROC [DeleteStage]
GO

CREATE PROCEDURE [DeleteStage] 
	@contractID ObjID,
	@stageID ObjID
AS
	IF (SELECT COUNT(*) FROM [_Parameter] WHERE [stage] = @stageID AND [_contractID] = @contractID) > 0
	BEGIN
		RAISERROR('Есть связанные с данным этапом параметры, эта запись не может быть удалена',18,1);
		RETURN
	END
	
	IF (SELECT COUNT(*) FROM [_Payment] WHERE [_stage] = @stageID AND [_contractID] = @contractID) > 0
	BEGIN
		RAISERROR('Есть связанные с данным этапом оплаты, эта запись не может быть удалена',18,1);
		RETURN
	END
	
	--DELETE FROM [_Parameter] WHERE [stage] = @stageID AND [_contractID] = @contractID;
	--DELETE FROM [_Payment] WHERE [_stage] = @stageID AND [_contractID] = @contractID;
	DELETE FROM [_Stage] WHERE [number] = @stageID AND [_contractID] = @contractID;
GO