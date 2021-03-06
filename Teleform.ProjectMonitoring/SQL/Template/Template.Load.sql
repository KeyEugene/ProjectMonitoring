IF NOT OBJECT_ID('[Template].[Load]', 'FN') IS NULL
	DROP FUNCTION [Template].[Load]
GO
CREATE FUNCTION [Template].[Load](@id [ObjID])
RETURNS XML
AS
BEGIN
	IF @id IS NULL OR @id < 1 RETURN NULL
	
	RETURN
	(
		SELECT
			[A].[objID] '@id',
			[A].[name] '@name',
			[A].[fileName] '@fileName',
			[A].[entityID] '@entityID',
			[B].[code] '@type',
			(
				SELECT
					[C].[objID] '@id',
					[C].[hash] '@attributeID',
					[C].[name] '@name',
					[C].[formatID] '@formatID',
					[C].[operatorID] '@operation',
					[C].[filter] '@filter',
					[C].[aggregate] '@aggregate',
					[C].[sequence] '@order'
				FROM [R_TemplateAttribute] [C]
				WHERE [C].[templateID] = @id
				FOR XML PATH('field'), TYPE
			) [fields],
			CAST('' AS XML).value('xs:base64Binary(sql:column("body"))', 'VARCHAR(MAX)') [content]
		FROM [R_Template] [A] JOIN [R_TemplateType] [B] ON [A].[typeID] = [B].[objID]
		WHERE [A].[objID] = @id
		FOR XML PATH('template'), TYPE
	)
END
