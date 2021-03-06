USE [Minprom]
GO
/****** Object:  StoredProcedure [dbo].[InsertDocument]    Script Date: 04.12.2013 21:25:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[InsertDocument]
	@contractID ObjID,
	@applicationTypeID tinyint,
	@number Int,
	@date Date,
	@based varchar(255),
	@comment varchar(1024),
	@stage tinyint,
	@fileName Name,
	@body VARBINARY(MAX)
AS
BEGIN

	DECLARE @file VARCHAR(256), @extension VARCHAR(32), @mimeTypeID [ObjID]

	IF @contractID IS NULL
	BEGIN
		 RAISERROR('Не выбран проект.',18,1)
		 RETURN
	END

	IF @applicationTypeID IS NULL
	BEGIN
		 RAISERROR('Не указан тип документа.',16,1)
		 RETURN
	END
	
	IF @number < 0 OR @number > 256
	BEGIN
		RAISERROR('Номер должен быть в диапазоне от 1 до 255',18,1);
		RETURN
	END
	
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
	
	INSERT INTO [_Application] 
		([_contractID], 
		[_ApplicationTypeID], 
		[number], 
		[date], 
		[based], 
		[comment], 
		[stage], 
		[fileName], 
		[mimeTypeID], 
		[body])
	VALUES
		(@contractID, 
		@applicationTypeID, 
		CAST(@number AS TINYINT),
		@date, 
		@based, 
		@comment, 
		@stage, 
		@file,
		@mimeTypeID,
		@body)
END
