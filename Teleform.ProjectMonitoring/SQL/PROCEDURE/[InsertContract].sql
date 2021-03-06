USE [Minprom]
GO
/****** Object:  StoredProcedure [dbo].[InsertContract]    Script Date: 04.12.2013 21:02:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[InsertContract]
	@name varchar(20),
    @number varchar(20),
    @workName varchar(1024),
    @start Date,
    @startYear Int,
    @term Int,
    @workTypeID tinyint,
    @programID Int,
    @statusID tinyint,
    --@divisionID ObjID,
    --@signerID ObjID,
    --@executiveID ObjID,
    @closedWorking Bit
AS
BEGIN
	IF @name = '' OR @name IS NULL
	BEGIN
		RAISERROR('Впишите название проекта.',18,1)
		RETURN
	END
	
	IF @startYear < 1990 OR @startYear > 2050
	BEGIN
		RAISERROR('Год должен быть больше 1990 и меньше 2050.',18,1)
		RETURN
	END
	
	IF LEN(@name) > 255
	BEGIN
		RAISERROR('Длина имени не должна превышать 255 символов.',18,1)
		RETURN
	END
		
	IF (SELECT COUNT(*) FROM [_Contract] WHERE [name] = @name) > 0
	BEGIN
		RAISERROR('Проект с таким название уже существует.',18,1)
		RETURN
	END
	
	IF @workTypeID IS NULL
	BEGIN
		RAISERROR('Выберите тип работы.',18,1)
		RETURN
	END
	
	IF @programID IS NULL
	BEGIN
		RAISERROR('Выберите мероприятие.',18,1)
		RETURN
	END
	
	INSERT INTO [_Contract]
		([name],
           [number],
           [workName],
           [start],
           [startYear],
           [term],
           [_workTypeID],
           [_programID],
           [_statusID],
           [closedWorking])
           
	VALUES
		(@name,
           @number,
           @workName,
           @start,
           @startYear,
           @term,
           @workTypeID,
           @programID,
           @statusID,
           @closedWorking)
          
    
    /*DECLARE @contractID ObjID = @@IDENTITY
    
    print @contractID
    IF @divisionID IS NOT NULL  
    BEGIN   
		INSERT INTO [_Accomplice] 
			([_contractID],
				[_divisionID],
				[_accompliceRoleID],
				[signerID],
				[executiveID])
		VALUES
			(
				@contractID,
				@divisionID,
				1,
				@signerID,
				@executiveID
			)
		UPDATE [_Contract] SET [mainExecutorID]  = @divisionID WHERE [objID] = @contractID
    END*/
	
END