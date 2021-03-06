IF NOT OBJECT_ID('[Template].[Update]', 'P') IS NULL
	DROP PROCEDURE [Template].[Update]
GO

CREATE PROCEDURE [Template].[Update]
	@xml XML
AS
	DECLARE @templateID INT,
		@entityID INT,
		@name [Name],
		@fileName [Name],
		@type [Name],
		@sheet VARCHAR(255),
		@body VARBINARY(MAX),
		@typeID [ObjID],
		@useFieldName BIT,
		@mimeTypeID [ObjID],
		@table SYSNAME
		
	SELECT
		@templateID = template.value('@id', 'INT'),
		@entityID = template.value('@entityID', 'INT'),
		@name = template.value('@name', '[Name]'),
		@fileName = template.value('@fileName', '[Name]'),
		@type = template.value('@type', '[Name]'),
		@sheet = template.value('@sheet', 'VARCHAR(255)'),
		@body = template.value('(/template/body/.)[1]', 'VARBINARY(MAX)')
	FROM @xml.nodes('/template') COL(template)	
	
	IF @name IS NULL OR LTRIM(@name) = ''
	BEGIN
		RAISERROR('Шаблон не имеет имени.', 18, 1)
		RETURN
	END
			
	IF EXISTS 
	(
		SELECT * FROM [R_Template]
		WHERE [name] = @name AND [objID] != @templateID
	)
	BEGIN
		RAISERROR('В данной категории уже существует шаблон с таким именем.', 18, 2)
		RETURN
	END
		
	SELECT
		@typeID = [T].[objID],
		@useFieldName = [T].[useFieldName],
		@mimeTypeID = [T].[mimeTypeID]
	FROM [R_TemplateType] [T]
	WHERE [T].[code] = @type
	
	IF @typeID IS NULL
	BEGIN
		RAISERROR('Нарушен протокол передачи шаблона. Не указан тип шаблона.', 18, 3)
		RETURN
	END
	
	IF NOT EXISTS (SELECT * FROM [R_TemplateType] [T] WHERE [T].[objID] = @typeID)
	BEGIN
		RAISERROR('Система не поддерживает тип переданного шаблона.', 18, 4)
		RETURN
	END
	
	SELECT @table = [name]
	FROM [SYS].[TABLES] [A]
	WHERE [A].[object_id] = @entityID
	
	BEGIN TRAN
		
	UPDATE [R_Template]
	SET [fileName] = @fileName,
		[name] = @name,
		[entityID] = @entityID,
		[baseTable] = @table,
		[sheet] = @sheet,
		[mimeTypeID] = @mimeTypeID,
		[body] = @body
	WHERE [objID] = @templateID;
	
	DELETE FROM [R_TemplateAttribute] WHERE [templateID] = @templateID;
	
	WITH [Temp] AS
	(
	SELECT 
		attr.value('@attributeID', 'varchar(40)') [id],
		attr.value('@formatID', 'objID') [formatID],
		attr.value('@alias', 'varchar(255)') [alias]
	FROM @xml.nodes('/template/fields/field') col(attr)
	)
	
	INSERT INTO [R_TemplateAttribute]
	(
		[templateID], 
		[hash], 
		[fpath], 
		[col], 
		[tbl], 
		[formatID],
		[name] 
	)
	(
		SELECT @templateID,
				[T].[id],
				[B].[fpath],
				[B].[attr],
				[B].[tbl],
				CASE WHEN [T].[formatID] = 0 THEN NULL ELSE [T].[formatID] END,
				CASE
					WHEN @useFieldName = 0 THEN CAST(NEWID() AS VARCHAR(64))
					ELSE
					CASE
						WHEN [T].[alias] IS NULL OR [T].[alias] = '' THEN [B].[lPath] + '/' + [B].[nameA]
						ELSE [T].[alias]
					END
				END
		FROM [Temp] [T]
		JOIN model.BObjectMap(@table) [B] ON [B].[hash] = [T].[id]
	);

	IF @@ERROR = 0
		COMMIT TRAN
	ELSE
		ROLLBACK TRAN
GO