USE [Minprom]
GO
/****** Object:  StoredProcedure [dbo].[InsertAccomplice]    Script Date: 04.12.2013 20:46:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[InsertAccomplice] 
	@divisionID ObjID,
    @contractID ObjID,
	@signer ObjID,
	@executive ObjID,
	@roleID tinyint,
    @reason varchar(1024)
AS
BEGIN

	IF @divisionID IS NULL
	BEGIN RAISERROR('Не указана организация.', 16, 1) RETURN END

	IF @contractID IS NULL
	BEGIN RAISERROR('Не указан проект.', 16, 1) RETURN END

	--IF @signer IS NULL
	--BEGIN RAISERROR('Не указан подписант.', 16, 1) RETURN END

	--IF @executive IS NULL
	--BEGIN RAISERROR('Не указан исполнитель.', 16, 1) RETURN END

	
	INSERT INTO [_Accomplice] 
	([_divisionID],[_contractID],[signerID],[executiveID],[_accompliceRoleID],[signerBase])
	VALUES
	(@divisionID, @contractID, @signer, @executive, @roleID, @reason)
END

