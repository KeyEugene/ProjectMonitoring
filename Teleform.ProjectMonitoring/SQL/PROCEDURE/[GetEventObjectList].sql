IF OBJECT_ID('[GetEventObjectList]', 'P') IS NOT NULL
	DROP PROCEDURE [GetEventObjectList]
GO

CREATE PROCEDURE [GetEventObjectList]
	@templateID INT,
	@table VARCHAR(30),
	@column VARCHAR(30),
	@daysFrom INT,
	@daysTo INT
AS
	DECLARE @ids VARCHAR(1000)
	DECLARE @query VARCHAR(1000) = '
		DECLARE @currentDate DATETIME = GETDATE()
		DECLARE @result VARCHAR(MAX) = ''''
		SELECT @result = @result + CONVERT(VARCHAR(5), [objID]) + N'','' FROM [' + @table + '] WHERE CONVERT(DATETIME, [' + @table + '].[' + @column + '])
		BETWEEN @currentDate - ' + CONVERT(VARCHAR(5), @daysFrom) + ' AND @currentDate + ' + CONVERT(VARCHAR(5), @daysTo) + '
		IF @result IS NOT NULL
			SET @result = LEFT(@result, LEN(@result) - 1)
		EXEC [report].[getReportData2] ' + CONVERT(varchar(5), @templateID) + ', @result, @cyr=1'

	--select @query
	EXEC sp_sqlexec @query
GO

--EXEC [GetEventObjectList] 258, '_Payment', 'dateReal', 100, 100