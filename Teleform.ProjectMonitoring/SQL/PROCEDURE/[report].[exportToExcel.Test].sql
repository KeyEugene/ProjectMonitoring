USE [Minprom]
GO
/****** Object:  StoredProcedure [report].[exportToExcel.Test]    Script Date: 12/26/2013 13:24:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [report].[exportToExcel.Test]
	
	 @templateID int,
	 @file varchar(1024),
	 @sheet varchar (20)
	 
	
AS
BEGIN
	DECLARE	@db_directory NVARCHAR (1024) = 
	(
		SELECT
			SUBSTRING(physical_name,1, CHARINDEX(dbo.[$lastItem](physical_name,'\'), LOWER(physical_name))-1)
		FROM sys.database_files
		WHERE physical_name LIKE '%.mdf'
    )
    , @i INT, @fileFound int, @colCount INT = 20
	, @sql NVARCHAR(1000), @stmt NVARCHAR(MAX) = ''
    , @colNames VARCHAR(MAX)
    , @outputList VARCHAR(MAX)					 
	, @spName VARCHAR(100) = 'EXEC [report].[getReportData]'
	, @cName VARCHAR(1024) = ''
	, @cType VARCHAR(255) = ''
	, @cmd VARCHAR(3000)	
	, @eCol VARCHAR(20)

	--подготовить список полей и их типов для временной таблици
	EXEC [report].[getReportData] @templateID, @outputList = @colNames  OUTPUT
	SET @colNames = ISNULL(RTRIM(LTRIM(@colNames)), '')
	SET @colNames = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(@colNames,'[','' ), '/', ''), ']', ''),'_',''),':','')
	SET @cType = 'VARCHAR(1024)' 
	IF @colNames <> '' 	
	BEGIN
		SET @colCount = (SELECT COUNT(*) FROM [$Split](@colNames, ','))
		SET @i = 1	
		WHILE @i < @colCount + 1
		BEGIN		  
			IF @stmt <> '' SET @stmt += ','
			SET @cName =  (SELECT item FROM [$Split](@colNames, ',') WHERE ID = @i)
			SET @stmt += '[' + @cName + ']' + @cType
			SET @i += 1
		 END			
		--создать временную таблицу	
		 SET @stmt = 'CREATE TABLE ##report(' + @stmt + ')'
		 EXEC SP_EXECUTESQL @stmt
	 END
	 --SET @Error = ''
	 ELSE 
	 BEGIN
		 PRINT 'Нет Полей'
		 RETURN
	 END	  
	 
	--вставить записи во временную таблицу 
	SET @stmt = 'INSERT INTO ##report (' + @colNames +') '+ CONVERT(NVARCHAR(1024),@spName)+' '+ CONVERT(NVARCHAR(1024),@templateID)+''
	EXEC SP_EXECUTESQL @stmt
	
	--подготовить файл Excel и диапазон ячеек в которые будут вставлены записи	 
	SET @eCol=char(ASCII('a')+(@colCount-1))+'64000'	
	EXECUTE XP_FILEEXIST @file, @fileFound OUTPUT

	IF @fileFound = 0
	BEGIN
		SET  @cmd = 'copy "'+ @db_directory+'report.xlsx" '+ @file
		EXECUTE XP_CMDSHELL @cmd,NO_OUTPUT REVERT
	END
			
	--вставить заглоки полей в Excel		
	SET @stmt = 'INSERT INTO OPENROWSET (''Microsoft.ACE.OLEDB.12.0'',
                               ''Excel 12.0;Database='+@file+';HDR=NO; Mode=Write;Trusted_Connection=yes;'',
                               ''SELECT * FROM ['+@sheet+'$a1:'+@eCol+']'') ' + 'values('''+REPLACE(@colNames,',',''',''')+''')'
		
	EXEC SP_EXECUTESQL @stmt
	
	--вставить данные  Excel
	SET @sql='SELECT * FROM ##report'	 
    SET @stmt = 'INSERT INTO OPENROWSET (''Microsoft.ACE.OLEDB.12.0'',
                               ''Excel 12.0;Database='+@file+';HDR=NO; Mode=Write;Trusted_Connection=yes;'',
                               ''SELECT * FROM ['+@sheet+'$a1:'+@eCol+']'') '  + @sql 	              
		
	EXEC SP_EXECUTESQL @stmt
	DROP TABLE ##report
	
END
/*
exec [report].[exportToExcel.Test] 255, 'C:\TestDataBase\test.xlsx', 'Лист4' 

drop table ##report
select * from ##report

EXEC [report].[getReportData] 265, '1'
*/
