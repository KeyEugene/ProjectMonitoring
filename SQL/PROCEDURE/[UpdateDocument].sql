IF NOT OBJECT_ID('[UpdateDocument]', 'P') IS NULL
	DROP PROCEDURE [UpdateDocument]
GO
CREATE PROCEDURE [dbo].[UpdateDocument]
	@objID ObjID,
	@applicationTypeID tinyint,
	@number INT,
	@date Date,
	@based varchar(255),
	@comment varchar(1024),
	@stage tinyint,
	@fileName Name,
	@body VARBINARY(MAX)
AS
BEGIN
	IF @number < 0 OR @number > 255
		BEGIN
			RAISERROR('Номер должен быть в пределах от 0 до 255.',16,1)
			RETURN
		END
	
	IF @applicationTypeID IS NULL
		BEGIN
			RAISERROR('Не указан тип документа.',16,1)
			RETURN
		END
			
	DECLARE @file VARCHAR(256), @extension VARCHAR(32), @mimeTypeID [ObjID]
	
	IF @fileName IS NULL SET @body = NULL
	ELSE BEGIN
		SELECT @file = [item] FROM [$split](@fileName, '.') WHERE [id] = 1
		SELECT @extension = [item] FROM [$split](@fileName, '.') WHERE [id] = 2
	
		SET @extension = '.' + @extension
	
		SELECT @mimeTypeID = [MT].[objID]
		FROM [MimeType] [MT]
		WHERE [MT].[extension] = @extension
		
		IF @extension IS NOT NULL AND @mimeTypeID IS NULL
		BEGIN
			RAISERROR('Нельзя загружать файлы с указанным расширением.', 16, 10);
			RETURN
		END
		
	END
	
	UPDATE [_Application]
	SET [_ApplicationTypeID] = @applicationTypeID,
		[number] = @number,
		[date] = @date,
		[based] = @based,
		[comment] = @comment,
		[stage] = @stage,
		[fileName] = ISNULL(@file, [fileName]),
		[mimeTypeID] = ISNULL(@mimeTypeID, [mimeTypeID]),
		[body] = ISNULL(@body, [body])
	WHERE [objID] = @objID
END
GO
