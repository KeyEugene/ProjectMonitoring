IF OBJECT_ID('[DeleteRoute]','P') IS NOT NULL
	DROP PROC [DeleteRoute]
GO

CREATE PROCEDURE [DeleteRoute] 
	@objID ObjID
AS
	DELETE FROM [_Route]  WHERE [objID] = @objID
GO