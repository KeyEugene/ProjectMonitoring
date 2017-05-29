IF OBJECT_ID('[DeleteAccomplice]','P') IS NOT NULL
	DROP PROC [DeleteAccomplice]
GO

CREATE PROCEDURE [DeleteAccomplice]
	@divisionID ObjID,
	@contractID ObjID
AS
	IF (SELECT COUNT(*) FROM [_AccompliceFinancing] WHERE [_divisionID] = @divisionID AND [_contractID] = @contractID) > 0
	BEGIN
		RAISERROR('Есть данные о финансировании, эта запись не может быть удалена',18,1);
		RETURN
	END
	
	IF (SELECT COUNT(*) FROM [_Payment] WHERE [_divisionID] = @divisionID AND [_contractID] = @contractID) > 0
	BEGIN
		RAISERROR('Есть данные об оплате, эта запись не может быть удалена',18,1);
		RETURN
	END
	
	--DELETE FROM [_AccompliceFinancing]  WHERE [_divisionID] = @divisionID AND [_contractID] = @contractID
	DELETE FROM [_Accomplice]  WHERE [_divisionID] = @divisionID AND [_contractID] = @contractID
GO