IF OBJECT_ID('[UpdateReportTemplate(old)]','P') IS NOT NULL
DROP PROCEDURE [UpdateReportTemplate(old)]
GO

CREATE PROCEDURE [UpdateReportTemplate(old)]
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
	
	DELETE FROM [R_TemplateAttribute] WHERE [templateID] = @templateID;
	
	WITH [Temp] AS
	(
		SELECT 
			--[field].[value]('@id', 'name') [id],
			[field].[value]('@attributeID', 'name') [attributeID],
			[field].[value]('@formatID', 'name') [formatID],
			[field].[value]('@alias', 'varchar(255)') [alias],
			[field].[value]('@filter', 'varchar(1024)') [filter],
			[field].[value]('@operation', 'smallint') [operation],
			[field].[value]('@order', 'tinyInt') [order],
			[field].[value]('@aggregate', 'varchar(255)') [aggregate]
		FROM @xml.[nodes]('/template/fields/field') COL([field])
	)
	
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
				CASE WHEN [T].[filter] = '' THEN NULL 
											ELSE 
												CASE WHEN [T].[operation] IN ('7','8')
												THEN '%' + REPLACE(REPLACE([T].[filter],'_','[_]'),'%','[%]') + '%' ELSE [T].[filter] END
				END,
				[B].[hash],
				[B].[tbl],
				[B].[fpath],
				[B].[attr],
				CASE WHEN [T].[alias] = '' THEN [B].[lPath] + '/' + [B].[nameA] ELSE [T].[alias] END
		FROM [Temp] [T]
		JOIN model.BObjectMap(@baseTable) [B] ON [B].[hash] = [T].[attributeID]
	)
GO
