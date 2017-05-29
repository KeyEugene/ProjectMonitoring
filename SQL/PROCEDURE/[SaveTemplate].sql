IF OBJECT_ID('[SaveTemplate]', 'P') IS NOT NULL
DROP PROCEDURE [SaveTemplate]
GO

CREATE PROCEDURE [SaveTemplate]
	@xml XML
AS
	DECLARE @baseTable VARCHAR(255) = (SELECT [tbl] FROM model.Entity('base') WHERE [tblID] = @xml.value('(/template/@entityID)[1]', 'int')),
			@sheet VARCHAR(255) = @xml.value('(/template/@sheet)[1]', 'varchar(255)'),
			@name VARCHAR(255) = @xml.value('(/template/@name)[1]', 'varchar(255)')
			
	IF (SELECT COUNT(*) FROM [R_Template] WHERE [name] = @name) > 0
	BEGIN
		RAISERROR('Уже существует шаблон с таким именем.', 18, 1)
		RETURN
	END
	
	
	INSERT INTO [R_Template] 
	(
		[fileName], 
		[entityID],
		[mimeTypeID],
		[typeID],
		[body],
		[name],
		[baseTable],
		[sheet]
	)
	(
		SELECT @xml.value('(/template/@fileName)[1]', 'varchar(255)'),
			   @xml.value('(/template/@entityID)[1]', 'int'),
			   (SELECT [objID] FROM [MimeType] WHERE [mime] = @xml.value('(/template/@mimeType)[1]', 'varchar(255)')),
				(SELECT [objID] FROM [R_TemplateType] WHERE [name] = @xml.value('(/template/@type)[1]', 'varchar(255)')),
			   @xml.value('(/template/content/.)[1]', 'varbinary(max)'),
			   @name,
			   @baseTable,
			   CASE WHEN @sheet = '' THEN NULL ELSE @sheet END
	)
	
	
	
	DECLARE @templateID ObjID = @@IDENTITY;
	
	WITH [Temp] AS
	(
	SELECT 
		attr.value('@id', 'varchar(40)') [id],
		attr.value('@formatID', 'objID') [formatID],
		attr.value('@alias', 'varchar(255)') [alias]
	FROM @xml.nodes('/template/attributes/attribute') col(attr)
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
				CASE WHEN [T].[alias] = '' THEN [B].[lPath] + '/' + [B].[nameA] ELSE [T].[alias] END
		FROM [Temp] [T]
		JOIN model.BObjectMap(@baseTable) [B] ON [B].[hash] = [T].[id]
	)

GO