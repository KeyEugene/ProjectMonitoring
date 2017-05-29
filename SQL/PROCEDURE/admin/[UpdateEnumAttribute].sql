IF OBJECT_ID('[UpdateEnumAttribute]', 'P') IS NOT NULL
	DROP PROCEDURE [UpdateEnumAttribute]
GO

CREATE PROCEDURE [UpdateEnumAttribute]
	@table VARCHAR(50),
	@id objID,
	@value VARCHAR(256)
AS
	DECLARE @query VARCHAR(1000) = 
		'UPDATE ' + '[' + @table + '] SET [name]=''' + @value + ''' WHERE [objID]=' + CONVERT(VARCHAR(10), @id)
	EXEC sp_SqlExec @query
GO