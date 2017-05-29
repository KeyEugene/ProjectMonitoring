IF OBJECT_ID('[UpdateProgram]','P') IS NOT NULL
DROP PROCEDURE [UpdateProgram]
GO

CREATE PROCEDURE [UpdateProgram]
	@id OBJID,
	@name VARCHAR(1024),
	@parentID INT,
	@code VARCHAR(20)
AS
	IF @name IS NULL
	BEGIN
		RAISERROR('Поле "Название" программы и мероприятия не может быть пустым.',18,1)
		RETURN
	END
	
	IF @code IS NULL
	BEGIN
		RAISERROR('Поле "Код" программы и мероприятия не может быть пустым.',18,1)
		RETURN
	END
	
	DECLARE @parent OBJID = @parentID
	
	WHILE @parent IS NOT NULL
	BEGIN
		IF @parent != @id
			SET @parent = (SELECT [parentID] FROM [_Program] WHERE [objID] = @parent)
		ELSE
		BEGIN
			RAISERROR('Выбранный родитель имеет в предках данную программу.',18,1)
			RETURN
		END
	END
	
	UPDATE [_Program] SET
			[name] = @name,
			[code] = @code,
			[parentID] = @parentID
	WHERE [objID] = @id
GO
