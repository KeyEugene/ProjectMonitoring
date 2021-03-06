USE [Minprom]
GO
/****** Object:  StoredProcedure [dbo].[InsertPerson]    Script Date: 04.12.2013 21:34:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[InsertPerson]
	@name0 VARCHAR(255),
	@name1 VARCHAR(255),
	@name2 VARCHAR(255),
    @genitive VARCHAR(255),
    @ablative VARCHAR(255),
    @phone VARCHAR(255),
    @email VARCHAR(255),
	@personID INT = NULL OUTPUT
AS
	IF @name0 IS NULL
	BEGIN
		RAISERROR('Необходимо указать фамилию.', 16, 1)
		RETURN
	END

	IF @name1 IS NULL
	BEGIN
		RAISERROR('Необходимо указать имя.', 16, 1)
		RETURN
	END

	INSERT INTO [_Person] 
		([name0], [name1], [name2], [genitive], [ablative],[phone] ,[email])
	VALUES
		(@name0, @name1, @name2, @genitive, @ablative, @phone, @email)
		
	SET @personID = @@IDENTITY
	
	DECLARE @name VARCHAR(255)
	
	
	IF @name1 IS NOT NULL
		SET @name = SUBSTRING(@name1,1,1) + '.'
	IF @name2 IS NOT NULL
		SET @name += SUBSTRING(@name2,1,1) + '.'
	IF @name0 IS NOT NULL
		SET @name +=  ' ' + @name0 
		
	UPDATE [_Person] SET [name] = @name WHERE [objID] = @personID
GO
