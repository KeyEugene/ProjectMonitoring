
ALTER PROCEDURE [report].[exportToExcel1]
	@spName NVARCHAR(100),
	@xml XML,
	@colNames NVARCHAR(1000),
	@fileName NVARCHAR(500),
	@reportDirectory  NVARCHAR (1000)= NULL,
	@errorMessage NVARCHAR(4000) = NULL OUTPUT
AS
BEGIN
	DECLARE	@db_directory NVARCHAR (1000) = 
	(
		SELECT
			SUBSTRING(physical_name,1, CHARINDEX(dbo.[$lastItem](physical_name,'\'), LOWER (physical_name))-1)
		FROM sys.database_files
		WHERE physical_name LIKE '%.mdf'
    )
    , @sql NVARCHAR(1000)					 
	, @file NVARCHAR(500)
	, @fName NVARCHAR(500)
	, @cmd VARCHAR(3000)
	, @stmt NVARCHAR(max) = ''
	, @colCount INT = 20
	, @i INT
	, @eCol VARCHAR(10)
	, @cName VARCHAR(1000) = ''
	, @cType VARCHAR(255) = ''
		
	--
	--подготовить список полей и их типов для временной таблици
	SET @colNames = isNULL(rtrim(ltrim(@colNames)), '')	
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
		  --prINT @stmt		  
		  --prINT @i
		  SET @i += 1
		 END			
		--создать временную таблицу	
		SET @stmt = 'create table ##report(' + @stmt + ')'
		--prINT @stmt
		EXEC sp_executesql @stmt
	 END		 
	 else
		 BEGIN
			 SET @errorMessage = 'Нет списка колонок'
			 return
		 END
	--вставить записи во временную таблицу 
	SET @stmt = 'INSERT INTO ##report (' + @colNames +') '+ convert(NVARCHAR(1024),@spName)+' ''+ convert(NVARCHAR(1024),@xml)+'''
	EXEC dbo.sp_executesql @stmt
	
	--подготовить файл Excel и диапазон ячеек в которые будут вставлены записи	 
	SET @eCol=char(ascii('a')+(@colCount-1))+'64000'	
	IF @reportDirectory is NULL SET  @reportDirectory = @db_directory +'report\'
	SET	 @fName = ''+ replace(@fileName, ' ','_')+'_'+replace(replace(replace(convert(VARCHAR,getdate()),' ', '_'),'-','_'),':','_')+'.xls'+''
	SET	 @file = ''+@reportDirectory+ @fName
	SET  @cmd = 'copy "'+ @db_directory+'report.xls" '+ @file
	EXECUTE master..XP_CMDSHELL @cmd,NO_OUTPUT REVERT
		
	--вставить заглоки полей в Excel		
	SET @stmt = 'INSERT INTO OPENROWSET (''Microsoft.ACE.OLEDB.12.0'',
                               ''Excel 12.0;Database='+@file+';HDR=NO; Mode=Write;Trusted_Connection=yes;'',
                               ''SELECT * FROM [Лист1$a1:'+@eCol+']'') ' + 'values('''+replace(@colNames,',',''',''')+''')'
	--prINT @stmt	
	EXEC dbo.sp_executesql @stmt
	
	--вставить данные  Excel
	SET @sql='SELECT * FROM ##report'	 
    SET @stmt = 'INSERT INTO OPENROWSET (''Microsoft.ACE.OLEDB.12.0'',
                               ''Excel 12.0;Database='+@file+';HDR=NO; Mode=Write;Trusted_Connection=yes;'',
                               ''SELECT * FROM [Лист1$a1:'+@eCol+']'') '  + @sql 	              
	--prINT @stmt	
	EXEC dbo.sp_executesql @stmt
	
	--удалить временную таблицу
	drop table ##report

    SET @errorMessage = 'Отчет сохранен:' + @stmt
    prINT @errorMessage    
     
END
--exec report.exportToExcel 
--'EXEC GetDocuments' 
--,'<filter documentName="Допсоглашение-01" />'
--,'ИД,Имя,IDПроекта,НазваниеДокумента,НазваниеПроекта,ТипДокумента,Номер,Основание,Комментарий,Этап'
--,'ReportExcel'
--,'\\admin\c$\TestDataBase\'