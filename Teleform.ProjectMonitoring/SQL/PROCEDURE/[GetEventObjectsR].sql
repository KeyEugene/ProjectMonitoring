USE [Minprom]
GO
/****** Object:  StoredProcedure [dbo].[GetEventObjectsR]    Script Date: 25.12.2013 20:20:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetEventObjectsR] 
	@table VARCHAR(30),
	@column VARCHAR(30),
	@daysFrom INT,
	@daysTo INT
AS
	DECLARE @tableID INT
	DECLARE @tableColumns TABLE (rname VARCHAR(30), name VARCHAR(30), [type] SYSNAME, [length] INT, colID INT)
	
	SELECT @tableID = [t].[object_id] FROM [sys].[tables] [t] WHERE [t].[name] = @table
	
	INSERT @tableColumns (rname, name, [type], [length], colID) 
		SELECT 
			CONVERT(VARCHAR(30), [c].[nameC]),
			CONVERT(VARCHAR(30), [c].[col]),
			[c].[sType],
			[c].[colLength],
			[c].[colID] 
		FROM [model].[TableColumns] [c] 
		WHERE [c].[tblID] = @tableID AND [c].[nameC] <> [c].[col] AND [c].[constr] IS NULL


	DECLARE @createTempTableQuery VARCHAR(MAX) = 'DECLARE @tt TABLE ('
	DECLARE @insert_query VARCHAR(MAX) = 'INSERT @tt ('
	DECLARE @select_query VARCHAR(MAX) = 'SELECT '
	DECLARE 
		@columnForSelect VARCHAR(30),
		@creationColumnName VARCHAR(30), 
		@creationColumnType SYSNAME,
		@creationColumnLength INT
	
	DECLARE Creation_cursor CURSOR
		FOR SELECT name, rname, [type], [length] FROM @tableColumns
	OPEN Creation_cursor
	FETCH NEXT FROM Creation_cursor
	INTO @columnForSelect, @creationColumnName, @creationColumnType, @creationColumnLength
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @insert_query = @insert_query + ' [' + @creationColumnName + '],'
		SET @select_query = @select_query + ' [' + @columnForSelect + '],'
	
		SET @createTempTableQuery = @createTempTableQuery + '[' + @creationColumnName + '] ' + @creationColumnType 
		IF @creationColumnType = 'varchar' SET @createTempTableQuery = @createTempTableQuery + '(' + CONVERT(VARCHAR(5), @creationColumnLength) + ')'
		SET @createTempTableQuery = @createTempTableQuery + ' NULL, '
	
		FETCH NEXT FROM Creation_cursor
		INTO @columnForSelect, @creationColumnName, @creationColumnType, @creationColumnLength
	END
	
	SET @createTempTableQuery = RTRIM(@createTempTableQuery)
	SET @createTempTableQuery = LEFT(@createTempTableQuery, LEN(@createTempTableQuery) - 1)
	SET @createTempTableQuery = @createTempTableQuery + ')
	
	'
	
	SET @insert_query = RTRIM(@insert_query)
	SET @insert_query = LEFT(@insert_query, LEN(@insert_query) - 1)
	SET @insert_query = @insert_query + ')
	'
	
	SET @select_query = RTRIM(@select_query)
	SET @select_query = LEFT(@select_query, LEN(@select_query) - 1)
	SET @select_query = @select_query + ' FROM [' + @table + ']
	WHERE CONVERT(DATETIME, [' + @table + '].[' + @column + ']) 
	BETWEEN @currentDate - ' + CONVERT(VARCHAR(10), @daysFrom) + ' AND @currentDate + ' + CONVERT(VARCHAR(10), @daysTo)
	
	CLOSE Creation_cursor
	DEALLOCATE Creation_cursor
	
	DECLARE @mainQuery VARCHAR(MAX) = 'DECLARE @currentDate DATETIME = GETDATE(); 
	'
	SET @mainQuery = @mainQuery + @createTempTableQuery + @insert_query + @select_query + '
	SELECT * FROM @tt'
	
	--SELECT @mainQuery
	EXEC sp_sqlexec @mainQuery

GO