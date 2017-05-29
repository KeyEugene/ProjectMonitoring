IF OBJECT_ID('[DeleteCost]','P') IS NOT NULL
	DROP PROC [DeleteCost]
GO

CREATE PROCEDURE [DeleteCost]
	@objID ObjID
AS
	DELETE FROM [_ContractCost]  WHERE [objID] = @objID
GO