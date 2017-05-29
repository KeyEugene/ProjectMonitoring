IF OBJECT_ID('[GetEnumAttributes]', 'P') IS NOT NULL
	DROP PROCEDURE [GetEnumAttributes]
GO

CREATE PROCEDURE [GetEnumAttributes] 
	@table VARCHAR(50),
	@id INT = NULL
AS
	DECLARE @query VARCHAR(500) = 'SELECT [objID], [name] FROM [' + @table + ']'
	IF @id IS NOT NULL
		SET @query = @query + ' WHERE [objID] = ' + CONVERT(VARCHAR(5), @id)

	EXEC sp_sqlexec @query
GO
