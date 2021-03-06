IF NOT OBJECT_ID('[Template].[Save]', 'P') IS NULL
	DROP PROCEDURE [Template].[Save]
GO
CREATE PROCEDURE [Template].[Save]
	@xml XML
AS
	DECLARE @templateID [objID] = @xml.value('(/template/@id)[1]', 'objID')

	IF @templateID IS NULL
		EXEC [Template].[Create] @xml
	ELSE
		EXEC [Template].[Update] @xml
GO