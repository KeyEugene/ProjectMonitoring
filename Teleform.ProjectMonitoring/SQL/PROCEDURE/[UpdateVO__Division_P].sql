IF OBJECT_ID('[UpdateVO__Division_P]', 'P') IS NOT NULL
	DROP PROC [UpdateVO__Division_P]
GO
CREATE PROCEDURE [dbo].[UpdateVO__Division_P]
	@objID INT,
	@parentID [ObjID],
	@name [Name],
	@nameF [StringL],
	@genitive [Name],
	@ablative [Name],
	@placeID ObjID,
	@INN [Code],
	@KPP [Code],
	@comment [Comment],
	@OKPO [Code],
	@OGRN [Code],
	@OKUD [Code],
	@ownershipID tinyINT,
	@address VARCHAR(255),
	@fax VARCHAR(50),
	@email VARCHAR(50)
AS	
	DECLARE @newParentID ObjID = @parentID
	
	WHILE @newParentID IS NOT NULL
	BEGIN
		IF @newParentID = @objID
		BEGIN
			RAISERROR('Нельзя создавать связи типа «цикл» между подразделениями.', 18, 1) 
			RETURN
		END
		SELECT @newParentID = [D].[parentID] FROM [_Division] [D] WHERE [D].[objID] = @newParentID
		
		PRINT @newParentID
	END
	
	IF(SELECT COUNT(*) FROM [_Division] WHERE [name] = @name) > 0
	BEGIN
		RAISERROR('Организация с таким именем уже  существует', 16, 1)
		RETURN
	END
	
	IF @name IS NULL
	BEGIN
		RAISERROR('Необходимо вписать название организации.', 16, 1)
		RETURN
	END
	
	UPDATE [_Division] SET 
		[name] = @name,
		[nameF] = @nameF,
		[genitive] = @genitive,
		[ablative] = @ablative,
		[parentID] = @parentID,
		[_placeID] = @placeID,
		[INN] = @INN,
		[KPP] = @KPP,
		[comment] = @comment,
		[OKPO] = @OKPO,
		[OGRN] = @OGRN,
		[OKUD] = @OKUD,
		[_ownershipID] = @ownershipID,
		[address] = @address,
		[fax] = @fax,
		[email] = @email
	WHERE [objID] = @objID;
GO


