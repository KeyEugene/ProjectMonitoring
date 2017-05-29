IF OBJECT_ID('[DeleteContract]','P') IS NOT NULL
	DROP PROC [DeleteContract]
GO

CREATE PROCEDURE [DeleteContract] 
	@objID ObjID
AS
	IF (SELECT COUNT(*) FROM [_Stage] WHERE [_contractID] = @objID) > 0
	BEGIN
		RAISERROR('≈сть св€занные с этим контрактом этапы',18,1);
		RETURN
	END
	
	IF (SELECT COUNT(*) FROM [_Application] WHERE [_contractID] = @objID) > 0
	BEGIN
		RAISERROR('≈сть св€занные с этим контрактом документы',18,1);
		RETURN
	END
	
	IF (SELECT COUNT(*) FROM [_TenderContract] WHERE [_contractID] = @objID) > 0
	BEGIN
		RAISERROR('≈сть св€занные с этим контрактом лоты',18,1);
		RETURN
	END
	
	IF (SELECT COUNT(*) FROM [_ContractCost] WHERE [_contractID] = @objID) > 0
	BEGIN
		RAISERROR('≈сть св€занные с этим контрактом оплаты',18,1);
		RETURN
	END
	
	IF (SELECT COUNT(*) FROM [_Parameter] WHERE [_contractID] = @objID) > 0
	BEGIN
		RAISERROR('≈сть св€занные с этим контрактом параметры',18,1);
		RETURN
	END
	
	IF (SELECT COUNT(*) FROM [_Accomplice] WHERE [_contractID] = @objID) > 0
	BEGIN
		RAISERROR('≈сть св€занные с этим контрактом соисполнители',18,1);
		RETURN
	END
	
	--DELETE FROM [_Parameter] WHERE [_contractID] = @objID;
	--DELETE FROM [_Payment] WHERE [_contractID] = @objID;
	--DELETE FROM [_Stage] WHERE [_contractID] = @objID;
	
	--DELETE FROM [_TenderContract] WHERE [_contractID] = @objID;
	
	--DELETE FROM [_ContractCost] WHERE [_contractID] = @objID;
	
	--DELETE FROM [_Route]  WHERE [_applicationID] IN (SELECT [objID] FROM [_Application] WHERE [_contractID] = @objID)
	
	--DELETE FROM [_Application] WHERE [_contractID] = @objID;
	
	--DELETE FROM [_AccompliceFinancing] WHERE [_contractID] = @objID;
	--UPDATE [_Contract] SET [mainExecutorID] = NULL
	--DELETE FROM [_Accomplice] WHERE [_contractID] = @objID;
	
	--DELETE FROM [_Financing] WHERE [_contractID] = @objID;
	DELETE FROM [_Contract] WHERE [objID] = @objID;
GO