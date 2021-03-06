USE [Minprom]
GO
/****** Object:  StoredProcedure [dbo].[InsertEmployee]    Script Date: 04.12.2013 21:27:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[InsertEmployee]
	@divisionID ObjID,
	@personID ObjID,
	@personRoleID ObjID,
	@head ObjID,
	@signerBase VARCHAR(1024),
	@email VARCHAR(50),
	@phone VARCHAR(50)
AS
	IF @personID IS NULL
	BEGIN
		RAISERROR('Не указан сотрудник.',18,1)
		RETURN
	END
	
	INSERT INTO [_DivisionPerson] 
	([_divisionID],[_personID],[_personRoleID],[head],[signerBase],[email],[phone])
	VALUES
	(@divisionID,@personID,@personRoleID,@head,@signerBase, @email, @phone)
GO