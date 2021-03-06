USE [Minprom]
GO
/****** Object:  StoredProcedure [dbo].[GetCountedEventList]    Script Date: 25.12.2013 20:15:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetCountedEventList] @showEmpty BIT = 1, @showNotCreated BIT = 0, @showEventsOnlyFor VARCHAR(30) = NULL
AS
	DECLARE @tables Table(name VARCHAR(30))
	DECLARE @nowDate DATETIME = GETDATE()
	INSERT @tables (name) 
		SELECT [tbl] FROM [model].[Entity]('base')

	DECLARE @tbl TABLE
		([tableID] BIGINT,
		 [tableName] VARCHAR(30),
		 [rTableName] VARCHAR(30),
		 [columnID] BIGINT,
		 [columnName] VARCHAR(30),
		 [rColumnName] VARCHAR(30),
		 [daysFrom] INT,
		 [daysTo] INT,
		 [message] VARCHAR(200),
		 [eventname] VARCHAR(2000),
		 [countObjects] INT NULL)

	INSERT @tbl
		SELECT 
			CONVERT(BIGINT, [table].[object_id]),
			[table].[name],
			CONVERT(VARCHAR(30), [rT].[name]),
			CONVERT(BIGINT, [column].[column_id]),
			[column].[name],
			CONVERT(VARCHAR(30), [rC].[nameC]),
			CAST([property].[daysFrom] AS INT),
			CAST([property].[daysTo] AS INT),
			CAST([property].[Message] AS VARCHAR(200)),
			CAST([property].[NameN] AS VARCHAR(2000)),
			-1
		FROM 
			[sys].[tables] [table]
			JOIN [sys].[columns] [column] ON [column].[object_id] = [table].[object_id] AND [table].[name] IN (SELECT * FROM @tables)
			JOIN [sys].[types] [type] ON [column].[system_type_id] = [type].[system_type_id] AND [type].[name] IN ('date', 'smalldatetime', 'datetime')
			CROSS APPLY [dbo].[fn_GetUDPEvents]([table].[object_id], [column].[column_id], (CASE @showNotCreated WHEN 0 THEN 0 ELSE 1 END)) [property]
			JOIN [model].[Tables] [rT] ON [rT].[objID] = [table].[object_id]
			JOIN [model].[TableColumns] [rC] ON [rC].[colID] = [column].[column_id] AND [rC].[tblID] = [table].[object_id]

	--ВЫБОР КОЛИЧЕСТВА ОБЪЕКТОВ ПО СОБЫТИЯМ.
	DECLARE @t VARCHAR(30), @c VARCHAR(30), @from INT, @to INT, @count INT
	DECLARE @countTable TABLE([count] INT)
	DECLARE [tCursor] CURSOR FOR SELECT [tableName], [columnName], [daysFrom], [daysTo] FROM @tbl
	OPEN [tCursor]
	FETCH NEXT FROM [tCursor]
	INTO @t, @c, @from, @to

	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @query VARCHAR(2000) =
		'DECLARE @curDate DATETIME = GETDATE()
		SELECT COUNT(*) FROM [' + @t + '] WHERE CONVERT(DATETIME, [' + @c + ']) BETWEEN @curDate - ' + 
			CONVERT(VARCHAR(10), @from) + ' AND @curDate + ' + CONVERT(VARCHAR(10), @to)
		INSERT @countTable EXEC sp_SqlExec @query
		SELECT @count = [count] FROM @countTable

		UPDATE @tbl SET [countObjects] = @count WHERE [tableName] = @t AND [columnName] = @c

		FETCH NEXT FROM [tCursor]
		INTO @t, @c, @from, @to
	END
	CLOSE [tCursor]
	DEALLOCATE [tCursor]
	-----------------------------------------

	IF @showEmpty = 1
		SELECT * FROM @tbl
	ELSE
		SELECT * FROM @tbl WHERE [countObjects] <> 0

