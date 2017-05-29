CREATE SCHEMA [Template]
GO
CREATE PROCEDURE [Template].[Delete]
	@id [objID]
AS
	DELETE FROM [R_Report] WHERE [templateID] = @id
    DELETE FROM [R_TemplateAttribute] WHERE [templateID] = @id
    DELETE FROM [R_Template] WHERE [objID] = @id
GO