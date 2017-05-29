IF OBJECT_ID('[GetTemplateAttributes]','P') IS NOT NULL
	DROP PROC [GetTemplateAttributes]
GO

CREATE PROCEDURE [GetTemplateAttributes]
			@templateID ObjID
AS
	DECLARE @baseTable VARCHAR(255) = (SELECT [baseTable] FROM [R_Template] WHERE [objID] = @templateID)
DECLARE @xml XML = 
(SELECT
	(
		SELECT ISNULL([RT].[formatID],0) '@formatID',
               [RT].[hash] '@id',
                CASE WHEN [RT].[name] = [B].[lPath] + '/' + [B].[nameA] THEN '' ELSE [RT].[name] END '@alias'
        FROM [R_TemplateAttribute] [RT]
        JOIN model.BObjectMap(@baseTable) [B] ON [B].[hash] = [RT].[hash]
        WHERE [RT].[templateID] = @templateID
        FOR XML PATH('attribute'), TYPE
    )
FOR XML PATH('attributes'), TYPE)
SELECT @xml
GO