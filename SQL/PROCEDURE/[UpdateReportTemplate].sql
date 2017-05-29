IF OBJECT_ID('[UpdateReportTemplate]','P') IS NOT NULL
DROP PROCEDURE [UpdateReportTemplate]
GO

CREATE PROCEDURE [UpdateReportTemplate]
	@xml XML
AS
	DECLARE @templateID OBJID = @xml.value('(/template/@id)[1]','objID'),
			@baseTable VARCHAR(255) = (SELECT [tbl] FROM model.Entity('base') WHERE [tblID] = @xml.value('(/template/@entityID)[1]','int'))
	
	/*UPDATE [R_Template] SET
		[name] = @xml.value('(/template/@name)[1]','varchar(255)'),
		[baseTable] = (SELECT [tbl] FROM model.Entity('base') WHERE [tblID] = @xml.value('(/template/@entityID)[1]','int')),
        [fileName] = @xml.value('(/template/@fileName)[1]','varchar(255)'),
        [body] = @xml.value('(/template/content/.)[1]','varbinary(max)'),
		[entityID] = @xml.value('(/template/@entityID)[1]','int')
	WHERE [objID] = @templateID;*/
	
	DECLARE @t TABLE (
						[attributeType] varchar(64),
						[attributeID] NAME,
						[formatID] NAME,
						[alias] varchar(255),
						[filter] varchar(1024),
						[operation] smallint,
						[order] tinyint,
						[aggregate] varchar(255)
					)
	INSERT @t
		SELECT 
			--[field].[value]('@id', 'name') [id],
			[field].[value]('@attributeType', 'varchar(64)') [attributeType],
			[field].[value]('@attributeID', 'name') [attributeID],
			[field].[value]('@formatID', 'name') [formatID],
			[field].[value]('@alias', 'varchar(255)') [alias],
			[field].[value]('@filter', 'varchar(1024)') [filter],
			[field].[value]('@operation', 'smallint') [operation],
			[field].[value]('@order', 'tinyInt') [order],
			[field].[value]('@aggregate', 'varchar(255)') [aggregate]
		FROM @xml.[nodes]('/template/fields/field') COL([field])
	
	--Начинается валидация
	DECLARE @count INT = 1,
			@msg varchar(1024),
			@precision INT,
			@maxLength INT,
			@query varchar(1024),
			@filter varchar(1024),
			@attributeType varchar(64),
			@c CURSOR
			
			SET @C = CURSOR LOCAL READ_ONLY FORWARD_ONLY FOR 
							SELECT [filter],[attributeType] FROM @t
	
	OPEN @c
	
	FETCH NEXT FROM @c INTO @filter,@attributeType
	WHILE @@FETCH_STATUS = 0
	BEGIN
	
		SELECT
			@maxLength = [UT].[max_length],
			@attributeType = [ST].[name],
			@precision = [ST].[precision]
		FROM [SYS].[TYPES] [UT] 
		JOIN [SYS].[TYPES] [ST] ON [UT].[system_type_id] = [ST].[system_type_id] AND [ST].[is_user_defined] = 0
		WHERE [UT].[name] = @attributeType
		
		IF @precision = 0
			IF LEN(@filter) > @maxLength
			BEGIN
				SET @msg = 'Максисальное допустимое количество символов в фильтре в строке ' + CONVERT(varchar(32),@count) + ' равно ' + CONVERT(varchar(32),@maxLength) + '.'
				RAISERROR(@msg,18,1)
				RETURN
			END
			
		BEGIN TRY
			SET @query = 'SELECT CONVERT(' + @attributeType + ',''' + @filter + ''')'
			EXEC(@query)
			
		END	TRY
		BEGIN CATCH
			SET @msg = 'Неправильно задан фильтр в строке ' + CONVERT(varchar(32),@count) + '.'
			RAISERROR(@msg,18,1)
			RETURN
		END CATCH
		
		SET @count = @count + 1
		FETCH NEXT FROM @c INTO @filter,@attributeType
	END
	
	CLOSE @c
	--заканчивается валидация
	
	DELETE FROM [R_TemplateAttribute] WHERE [templateID] = @templateID;
	
	INSERT INTO [R_TemplateAttribute]
	(
		[templateID],
		[formatID],
		[operatorID],
		[sequence],
		[aggregate],
		[filter],
		[hash],
		[tbl],
		[fpath], 
		[col],
		[name]
		
	)
	(
		SELECT @templateID,
				CASE WHEN [T].[formatID] = 0 THEN NULL ELSE [T].[formatID] END,
				CASE WHEN [T].[operation] = '' THEN NULL ELSE [T].[operation] END,
				CASE WHEN [T].[order] = 0 THEN NULL ELSE [T].[order] END,
				CASE WHEN [T].[aggregate] = '' THEN NULL ELSE [T].[aggregate] END,
				CASE WHEN [T].[filter] = '' THEN NULL ELSE [T].[filter]
											/*ELSE 
												CASE WHEN [T].[operation] IN ('7','8')
												THEN '%' + REPLACE(REPLACE([T].[filter],'_','[_]'),'%','[%]') + '%' ELSE [T].[filter] END*/
				END,
				[B].[hash],
				[B].[tbl],
				[B].[fpath],
				[B].[attr],
				CASE WHEN [T].[alias] = '' THEN [B].[lPath] + '/' + [B].[nameA] ELSE [T].[alias] END
		FROM @t [T]
		JOIN model.BObjectMap(@baseTable) [B] ON [B].[hash] = [T].[attributeID]
	)
GO