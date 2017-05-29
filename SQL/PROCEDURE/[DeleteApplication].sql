IF OBJECT_ID('[DeleteApplication]','P') IS NOT NULL
	DROP PROC [DeleteApplication]
GO

CREATE PROCEDURE [DeleteApplication] 
	@objID ObjID
AS
	IF (SELECT COUNT(*) FROM [_Route] WHERE [_applicationID] = @objID) > 0
	BEGIN
		RAISERROR('Есть связанные с этим документом маршруты',18,1);
		RETURN
	END
	
	--DELETE FROM [_Route]  WHERE [_applicationID] = @objID
	
	DELETE FROM [_Application] WHERE [objID] = @objID;
GO