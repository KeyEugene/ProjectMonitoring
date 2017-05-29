IF OBJECT_ID('[InsertEnumAttribute]', 'P') IS NOT NULL
	DROP PROCEDURE [InsertEnumAttribute]
GO

CREATE PROCEDURE [InsertEnumAttribute]
	@enumName VARCHAR(30),
	@attributeValue VARCHAR(100)
AS
	DECLARE @query VARCHAR(1000)
	
	IF (SELECT [isIdentity] from model.TableColumns where tbl=@enumName and [col]='objID') = 1
	BEGIN
		IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=@enumName AND column_name='code')
			SET @query = 'INSERT [' + @enumName + '] ([name],[code]) VALUES (''' + @attributeValue + ''', ''' + @attributeValue + ''')'
        ELSE   
			SET @query = 'INSERT INTO [' + @enumName + '] (name) VALUES (''' + @attributeValue + ''')'
	END
	ELSE
	BEGIN
		SET @query = '
		DECLARE @lastID INT
		SELECT @lastID = MAX([objID]) FROM [' + @enumName + ']
		SET @lastID = @lastID + 1'
		
		IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=@enumName AND column_name='code')
			SET @query += 'INSERT [' + @enumName + '] ([objID], [name],[code]) VALUES (@lastID, ''' + @attributeValue + ''', ''' + @attributeValue + ''')'
        ELSE   
			SET @query += 'INSERT [' + @enumName + '] ([objID], [name]) VALUES (@lastID, ''' + @attributeValue + ''')'
	END

	EXEC (@query)
GO