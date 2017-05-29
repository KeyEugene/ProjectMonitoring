IF NOT OBJECT_ID('[GetTemplateAndAttributes]', 'P') IS NULL
	DROP PROCEDURE [GetTemplateAndAttributes]
GO

CREATE PROCEDURE [GetTemplateAndAttributes]
	@templateID ObjID
	
AS
	IF @templateID IS NOT NULL
	BEGIN
		DECLARE @baseTable Name = (SELECT [baseTable] FROM [R_Template] WHERE [objID] = @templateID)
		
		SELECT
			[RT].[name] '@templateName',
			[RT].[baseTable] '@baseTable',
			[RT].[fileName] '@fileName',
			[T].[name] '@aliasTable',
			[RT].[body] '@body',
			(
				SELECT 
						[BO].[lPath] + ':' + [BO].[nameA] '@attrRus', 
						[BO].[fPath] + ':' + [BO].[attr] '@attr',
						0 '@active'
				FROM [model].[BObjectMap](@baseTable) [BO]
				--UNION
					--SELECT [TA].[name] '@name'
					--FROM model.TemplateAttr(@templateID) [TA]
				FOR XML PATH('attribute'), TYPE
			)[attributes]
		FROM [R_Template] [RT]
		LEFT JOIN model.Tables [T] ON [RT].[baseTable] = [T].[tbl]
		WHERE [RT].[objID] = @templateID
		FOR XML PATH('template'), TYPE
	END
GO