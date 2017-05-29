IF OBJECT_ID('[DeleteLot]','P') IS NOT NULL
	DROP PROC [DeleteLot]
GO

CREATE PROCEDURE [DeleteLot] 
	--@tenderID ObjID,
	--@contractID ObjID
	@objID ObjID
AS
	DECLARE @contractID ObjID = (SELECT [_contractID] FROM [_TenderContract] WHERE [objID] = @objID)
	
	UPDATE [_Contract] SET [_tenderID] = NULL WHERE [objID] = @contractID
	
	DELETE FROM [_TenderContract]  WHERE [objID] = @objID--[_contractID] = @contractID AND [_tenderID] = @tenderID
GO