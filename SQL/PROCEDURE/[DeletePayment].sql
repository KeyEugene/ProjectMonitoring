IF OBJECT_ID('[DeletePayment]','P') IS NOT NULL
	DROP PROC [DeletePayment]
GO

CREATE PROCEDURE [DeletePayment] 
	@objID ObjID
AS
	DELETE FROM [_Payment] WHERE [objID] = @objID;
GO
