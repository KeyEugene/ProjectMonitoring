IF NOT OBJECT_ID('[UpdateContract]', 'P') IS NULL
	DROP PROCEDURE [UpdateContract]
GO
CREATE PROCEDURE [dbo].[UpdateContract]
	@objID ObjID,
	@name varchar(20),
    @number varchar(20),
    @workName varchar(1024),
    @start Date,
    @startYear Int,
    @finish Date,
    @_workTypeID tinyint,
    @_programID Int,
    --@divisionID ObjID,
    --@signerID ObjID,
    --@executiveID ObjID,
    @_statusID tinyint,
    @closedWorking Bit
AS
BEGIN
	IF @startYear < 1990 OR @startYear > 2050
	BEGIN
		RAISERROR('Год должен быть больше 1990 и меньше 2050.',18,1)
		RETURN
	END
	
	IF @_workTypeID is NULL
	BEGIN
		RAISERROR('Не указан тип работы.',18,1)
		RETURN
	END
	
	IF @name IS NULL
	begin
		raiserror('Впишите название проекта', 18, 1)
		return
	end
	
	/*DECLARE @oldDivisionID ObjID
	SET @oldDivisionID = (SELECT TOP 1 [_divisionID] FROM [_Accomplice] WHERE [_contractID] = @objID AND [_accompliceRoleID] = 1)
	
	IF @divisionID != @oldDivisionID
	BEGIN
		IF (SELECT COUNT(*) FROM [_AccompliceFinancing] WHERE [_divisionID] = @oldDivisionID AND [_contractID] = @objID) > 0
		BEGIN
			RAISERROR('С победителем связанно финансирование, не подлежит изменению',18,1)
		END
		ELSE
		BEGIN
			UPDATE [_Contract] SET [mainExecutorID] = @divisionID WHERE [objID] = @objID
			UPDATE [_Accomplice] SET [_accompliceRoleID] = 3 WHERE [_contractID] = @objID AND [_divisionID] = @oldDivisionID
			UPDATE [_Accomplice] 
			SET [_accompliceRoleID] = 1,
				[signerID] = @signerID,
				[executiveID] =  @executiveID
			WHERE [_contractID] = @objID AND [_divisionID] = @divisionID 
		END
	END
	ELSE
	BEGIN
		UPDATE [_Accomplice] 
		SET [signerID] = @signerID,
				[executiveID] =  @executiveID
		WHERE [_contractID] = @objID AND [_divisionID] = @divisionID 
	END*/
	
	UPDATE [_Contract] SET
           [name] = @name,
           [number] = @number,
           [workName] = @workName,
           [start] = @start,
           [startYear] = @startYear,
           [finish] = @finish,
           [_workTypeID] = @_workTypeID,
           [_programID] = @_programID,
           [_statusID] = @_statusID,
           [closedWorking] = @closedWorking
    WHERE [objID] = @objID
END