IF OBJECT_ID('[DeleteEmployee]','P') IS NOT NULL
	DROP PROC [DeleteEmployee]
GO

CREATE PROCEDURE [DeleteEmployee] 
	@divisionID ObjID,
	@personID ObjID
AS
	DELETE FROM [_DivisionPerson]  WHERE [_divisionID] = @divisionID AND [_personID] = @personID
	
	/*IF (SELECT COUNT(*) FROM [_DivisionPerson] WHERE [_personID] = @personID) = 0
	BEGIN
		DELETE FROM [_Person]  WHERE [objID] = @personID
	END*/
GO