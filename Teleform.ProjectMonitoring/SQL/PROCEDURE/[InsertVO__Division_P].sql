USE [Minprom]
GO
/****** Object:  StoredProcedure [dbo].[InsertVO__Division_P]    Script Date: 04.12.2013 21:41:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[InsertVO__Division_P]
	@parentID ObjID,
	@name VARCHAR(255),
	@nameF VARCHAR(1024),
	@genitive VARCHAR(255),
	@ablative VARCHAR(255),
	@place INT,
	@INN VARCHAR(20),
	@KPP VARCHAR(20),
	@comment VARCHAR(1024),
	@okpo VARCHAR(20),
	@ogrn VARCHAR(20),
	@okud VARCHAR(20),
	@ownershipID tinyINT,
	@address VARCHAR(255),
	@fax VARCHAR(50),
	@email VARCHAR(50)
AS
BEGIN

	IF @name IS NULL or @name = ''
	BEGIN
		RAISERROR('Необходимо вписать название организации.', 16, 1)
		RETURN
	END
	
	IF (SELECT COUNT(*) FROM [_Division] WHERE [name] = @name)>0
	BEGIN
		RAISERROR('Организация с таким именем уже  существует', 16, 1)
		RETURN
	END
	
	
	INSERT INTO [_Division]
	(
		[parentID],
		[name],
		[nameF],
		[genitive],
		[ablative],
		[_placeID],
		[INN],
		[KPP],
		[comment],
		[OKPO],
		[OGRN],
		[OKUD],
		[_ownershipID],
		[address],
		[fax],
		[email]
	)
	VALUES
	(
		@parentID,
		@name,
		@nameF,
		@genitive,
		@ablative,
		@place,
		@INN,
		@KPP,
		@comment,
		@okpo,
		@ogrn,
		@okud,
		@ownershipID,
		@address,
		@fax,
		@email
	)
END

