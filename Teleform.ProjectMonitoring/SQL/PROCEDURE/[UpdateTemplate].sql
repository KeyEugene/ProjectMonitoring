IF OBJECT_ID('[UpdateTemplate]', 'P') IS NOT NULL
DROP PROCEDURE [UpdateTemplate]
GO

CREATE PROCEDURE [UpdateTemplate]
	@xml XML,
	@templateID ObjID
AS
	DECLARE @baseTable VARCHAR(255) = (SELECT [tbl] FROM model.Entity('base') WHERE [tblID] = @xml.value('(/template/@entityID)[1]', 'int')),
			@sheet VARCHAR(255) = @xml.value('(/template/@sheet)[1]', 'varchar(255)'),
			@name VARCHAR(255) = @xml.value('(/template/@name)[1]', 'varchar(255)')
			
	IF (SELECT COUNT(*) FROM [R_Template] WHERE [name] = @name AND [objID] != @templateID) > 0
	BEGIN
		RAISERROR('Уже существует шаблон с таким именем.',18,1)
		RETURN
	END
			
			
	IF @xml.value('(/template/content/.)[1]', 'varbinary(max)') IS NOT NULL
		UPDATE [R_Template] 
		SET [body] = @xml.value('(/template/content/.)[1]', 'varbinary(max)'),
			[mimeTypeID] = (SELECT [objID] FROM [MimeType] WHERE [mime] = @xml.value('(/template/@mimeType)[1]', 'varchar(255)'))
		WHERE [objID] = @templateID
		
	UPDATE [R_Template]
	SET [fileName] = @xml.value('(/template/@fileName)[1]', 'varchar(255)'),
		[name] = @name,
		[entityID] = @xml.value('(/template/@entityID)[1]', 'int'),
		[baseTable] = @baseTable,
		[sheet] = CASE WHEN @sheet = '' THEN NULL ELSE @sheet END
	WHERE [objID] = @templateID;
	
	DELETE FROM [R_TemplateAttribute] WHERE [templateID] = @templateID;
	
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
	);
GO