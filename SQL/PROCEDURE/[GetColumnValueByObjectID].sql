IF OBJECT_ID('GetColumnValueByObjectID','P') IS NOT NULL
DROP PROCEDURE GetColumnValueByObjectID
GO

CREATE PROCEDURE GetColumnValueByObjectID
	@tableName VARCHAR(128),
	@id OBJID,
	@columnName VARCHAR(128)
AS
	--проверка, на то существует ли такой @columnName в заданной таблице
	IF (SELECT COUNT(*) FROM model.TableColumns WHERE [tbl] = @tableName AND [col] = @columnName) = 0
		RETURN -1
	
	DECLARE @query VARCHAR(MAX)
	
	SET @query = 'SELECT ' + @columnName + ' FROM ' + @tableName + ' WHERE [objID] = ' + CONVERT(VARCHAR(64),@id)
	EXEC (@query) 
GO
