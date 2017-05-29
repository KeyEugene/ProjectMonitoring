IF OBJECT_ID('[InsertTableFieldName]', 'P') IS NOT NULL
	DROP PROCEDURE [InsertTableFieldName]
GO

CREATE PROCEDURE [InsertTableFieldName] 
	@table VARCHAR(50),
	@name VARCHAR(256)
AS
	DECLARE @query VARCHAR(MAX) = 
		'INSERT ' + '[' + @table + '] ([name]) VALUES (''' + @name + ''')'

	EXEC sp_SqlExec @query
GO