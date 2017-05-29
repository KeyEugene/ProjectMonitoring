IF NOT OBJECT_ID('[ConnectCountElements]', 'P') IS NULL
	DROP PROCEDURE [ConnectCountElements]
GO

CREATE PROCEDURE [dbo].[ConnectCountElements](@initialTableName VARCHAR(64),@xml XML)
-- exec [ConnectCountElements] '_Contract',6
AS
	DECLARE @idTable TABLE([parameter] [STRING], [value] [STRING])
	INSERT @idTable
	SELECT
		[X].[f].[value]('local-name(.)', 'String') [parameter],
		[X].[f].[value]('string(.)', 'String') [value]
	FROM @xml.nodes('/original/@*') X([f])
	
	DECLARE @result TABLE
	(
			[table] VARCHAR(64),
			[text] VARCHAR(64)DEFAULT NULL,
			[url] [STRING],
			[param] XML,
			[amount] INT
	)
	
	DECLARE @c CURSOR,
			@conTableName VARCHAR(64),
			@conTableNameRus VARCHAR(64),
			@url [STRING],
			@initialTableKey SYSNAME,
			@conCompNumber INT,
			@query VARCHAR(MAX),
			@status INT,
			@where VARCHAR(MAX) = '',
			@ctr INT,
			@xmlParam XML
			
				   
	SET @c = CURSOR LOCAL READ_ONLY FORWARD_ONLY FOR
				SELECT [attr] FROM model.BObjectMap(@initialTableName) WHERE [UType] = 'Table' AND [attr]<>@initialTableName
	OPEN @c
	FETCH NEXT FROM @c INTO @conTableName

	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @c2 CURSOR
		SET @c2 = CURSOR LOCAL READ_ONLY FORWARD_ONLY FOR
				SELECT [col] FROM model.TableColumns WHERE tbl=@conTableName AND [refTbl]=@initialTableName
		OPEN @c2
		FETCH NEXT FROM @c2 INTO @initialTableKey
		SET @status = @@FETCH_STATUS
		IF @status = 0 SET @where += ' WHERE '
	
	
		IF (SELECT COUNT(*) FROM @idTable) > 1
		BEGIN
			WHILE @status = 0
			BEGIN
				--DECLARE @query1 VARCHAR(MAX)
				--SET @query1 = '' 
				
				--SET @where += @initialTableKey + '=' + (SELECT [value] FROM @idTable WHERE CONTAINS([parameter],@initialTableKey))
				FETCH NEXT FROM @c2 INTO @initialTableKey
				SET @status = @@FETCH_STATUS
				IF @status = 0 SET @where += ' AND '
			END
		END
		ELSE
		BEGIN
			SET @where += @initialTableKey + '=' + (SELECT [value] FROM @idTable)
		END
		CLOSE @c2
		DEALLOCATE @c2
				
				
		SELECT @ctr = COUNT(*) 
		FROM [M_Application] [MA] 
		JOIN [M_Entity] [ME] ON [MA].[objID] = [ME].[applicationID] AND [ME].[baseTable] = @initialTableName
		WHERE [MA].[name] = @conTableName
		
		IF @ctr != 0
		BEGIN
			SET @xmlParam = 
			(
				SELECT(
					SELECT 
						[MP].[name] '@name'
					FROM [M_Parameter] [MP]
					JOIN [M_Application] [MA] ON [MA].[name] = @conTableName AND [MP].[applicationID] = [MA].[objID]
					WHERE [MP].[baseTable] = @initialTableName
					FOR XML PATH('parameter'), TYPE
				)FOR XML PATH('parameters'), TYPE
			)
			
			SELECT @conTableNameRus = [nameN], @url = [url] FROM [M_Application] WHERE [name] = @conTableName 
			SET @query = 'SELECT ''' + @conTableName +''',''' + @conTableNameRus + ''',''' + @url + ''',''' + CAST(@xmlParam AS VARCHAR(128))+''', COUNT(*) FROM ' + @conTableName + @where
			INSERT @result EXEC(@query)
		END
		SET @where = ''
		FETCH NEXT FROM @c INTO @conTableName
	END
	CLOSE @c
	DEALLOCATE @c
	SELECT * FROM @result
GO


