IF OBJECT_ID('[DeleteDivision]','P') IS NOT NULL
	DROP PROC [DeleteDivision]
GO

CREATE PROCEDURE [DeleteDivision]
	@objID ObjID
AS
	IF (SELECT COUNT(*) FROM [_Contract]  WHERE [mainExecutorID] = @objID) > 0
	BEGIN
		RAISERROR('Организация является победителем проектов и не может быть удалена',18,1);
		RETURN
	END
	
	IF (SELECT COUNT(*) FROM [_Accomplice]  WHERE [_divisionID] = @objID) > 0
	BEGIN
		RAISERROR('Организация является участником проектов и не может быть удалена',18,1);
		RETURN
	END
	
	IF (SELECT COUNT(*) FROM [_DivisionPerson] WHERE [_divisionID] = @objID) > 0
	BEGIN
		RAISERROR('В данной организации работают сотрудники, она не может быть удалена',18,1);
		RETURN
	END
	
	IF (SELECT COUNT(*) FROM [_Route] WHERE [_divisionID] = @objID) > 0
	BEGIN
		RAISERROR('Через данную организацию проходили маршруты документов, она не может быть удалена',18,1);
		RETURN
	END
	
	IF (SELECT COUNT(*) FROM [_Division] WHERE [parentID] = @objID) > 0
	BEGIN
		RAISERROR('Данная организация является концерном для организаций, она не может быть удалена',18,1);
		RETURN
	END
	--SELECT COUNT(*) 
	--FROM [_Accomplice] [A]
	--JOIN  [_DivisionPerson] [DP] ON [A].[signerID] = [DP].[_personID] OR [A].[executiveID] = [DP].[_personID]
	--WHERE [_divisionID] = @objID
	
	--DELETE FROM [_AccompliceFinancing]  WHERE [_divisionID] = @objID
	--DELETE FROM [_Accomplice]  WHERE [_divisionID] = @objID
	
	--UPDATE [Division] SET [parentID] = NULL WHERE [parentID] = objID
	
	--DELETE FROM [_Route]  WHERE [_divisionID] = @objID
	
	--DELETE FROM [_DivisionPerson]  WHERE [_divisionID] = @objID
	
	DELETE FROM [_Division]  WHERE [objID] = @objID
GO