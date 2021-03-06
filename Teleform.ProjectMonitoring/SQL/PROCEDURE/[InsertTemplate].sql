USE [Minprom]
GO
/****** Object:  StoredProcedure [dbo].[InsertTemplate]    Script Date: 04.12.2013 21:39:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[InsertTemplate]
	@name Name,
	@baseTable Name,
	@fileName Name,
	@body VARBINARY(MAX),
	@contentType VARCHAR(255),
	@word Bit,
	@attrXml XML
AS
	IF @baseTable IS NULL OR @baseTable=''
    BEGIN
         RAISERROR('Выберите тип объекта.',18,1)
         RETURN
    END
    
    IF @body IS NULL AND @word = 1
    BEGIN
         RAISERROR('Не выбран документ для загрузки.',18,1)
         RETURN
    END
    
    IF @body IS NULL
    BEGIN
		INSERT INTO [R_Template]([name], [baseTable]) VALUES (@name, @baseTable);
		
		DECLARE @templateID ObjID = @@IDENTITY,
				@nodesNum INT;
		
		SET @nodesNum = (SELECT COUNT(*) FROM @attrXml.[nodes]('/attributes/attribute') AS COL(attr));
		IF @nodesNum > 0
		BEGIN
			WITH [Temp] AS
			(
				SELECT 
					[attr].[value]('@name', 'name') [name],
					[attr].[value]('@tblColPair', 'name') [tblColPair]
				FROM @attrXml.[nodes]('/attributes/attribute') COL([attr])
			)
		
			INSERT [R_TemplateAttribute] ([templateID], [tbl], [name], [fpath], [col])
			SELECT @templateID,
				   @baseTable,
				   [T].[name],
				   (SELECT [item] FROM [$split]([T].[tblColPair],':') WHERE [id]=1),
				   (SELECT [item] FROM [$split]([T].[tblColPair],':') WHERE [id]=2)
			FROM [Temp][T]
		END
    END
    ELSE
    BEGIN
		INSERT INTO [R_Template]([name], [fileName], [body], [typeID], [baseTable]) 
		VALUES (@name, @fileName, @body, (SELECT TOP 1 [objID] FROM [MimeType] WHERE [mime]=@contentType), @baseTable);
	END
	
    SELECT CAST(SCOPE_IDENTITY() AS INT);
