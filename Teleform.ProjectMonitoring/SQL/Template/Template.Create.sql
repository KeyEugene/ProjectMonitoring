IF NOT OBJECT_ID('[Template].[Create]', 'P') IS NULL
	DROP PROCEDURE [Template].[Create]
GO
CREATE PROCEDURE [Template].[Create]
	@xml XML
AS		
	DECLARE @table SYSNAME,
		@entityID INT,
		@name [Name],
		@fileName [Name],
		@type [Name],
		@typeID [ObjID],
		@body VARBINARY(MAX),
		@useFieldName BIT,
		@useBody BIT,
		@mimeTypeID [ObjID],
		@parameters XML
			
	SELECT
		@entityID = template.value('@entityID', 'INT'),
		@name = template.value('@name', '[Name]'),
		@fileName = template.value('@fileName', '[Name]'),
		@type = template.value('@type', '[Name]'),
		@body = template.value('(/template/body/.)[1]', 'VARBINARY(MAX)'),
		@parameters = template.query('/parameters')
	FROM @xml.nodes('/template') COL(template)
	
	IF @name IS NULL OR LTRIM(@name) = ''
	BEGIN
		RAISERROR('Шаблон не имеет имени.', 18, 1)
		RETURN
	END

-- В дальнейшем условие может быть ослаблено.
	IF EXISTS
	(
		SELECT *
		FROM [R_Template] [T]
		WHERE [T].[name] = @name AND [T].[entityID] = @entityID
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
	
	IF @useBody = 1 AND @body IS NULL
	BEGIN
		RAISERROR('Шаблоны данного типа должны иметь тело документа.', 18, 5)
		RETURN
	END
	
	IF @useBody = 0 AND NOT @body IS NULL
	BEGIN
		RAISERROR('Шаблоны данного типа не могут иметь тело документа.', 18, 5)
		RETURN
	END
	
	SELECT @table = [name]
	FROM [SYS].[TABLES] [A]
	WHERE [A].[object_id] = @entityID

-- mimeTypeID пишется для совместимости.

	BEGIN TRAN

	INSERT INTO [R_Template] 
	(
		[fileName], 
		[entityID],
		[mimeTypeID],
		[typeID],
		[body],
		[name],
		[baseTable],
		[parameters]
	)
	SELECT
		@fileName,
		@entityID,
		@mimeTypeID,
		@typeID,
		@body,
		@name,
		@table,
		@parameters
	
	DECLARE @templateID ObjID = @@IDENTITY;
	
	WITH [Temp] AS
	(
		SELECT 
			field.value('@attributeID', 'varchar(40)') [id],
			field.value('@formatID', 'objID') [formatID],
			field.value('@alias', 'varchar(255)') [alias]
		FROM @xml.nodes('/template/fields/field') col(field)
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
		SELECT
			@templateID,
			[T].[id],
			[B].[fpath],
			[B].[attr],
			[B].[tbl],
			CASE
				WHEN [T].[formatID] IS NULL OR [T].[formatID] = 0 THEN NULL
				ELSE [T].[formatID]
			END,
			CASE
-- Возможно, следует использовать последовательноси? Потокобезопасность.
				WHEN @useFieldName = 0 THEN CAST(NEWID() AS VARCHAR(64))
				ELSE
				CASE
					WHEN [T].[alias] IS NULL OR [T].[alias] = '' THEN [B].[lPath] + '/' + [B].[nameA]
					ELSE [T].[alias]
				END
			END
		FROM [Temp] [T] JOIN
			model.BObjectMap(@table) [B] ON [B].[hash] = [T].[id]
	)

	IF @@ERROR = 0
		COMMIT TRAN
	ELSE
		ROLLBACK TRAN
GO