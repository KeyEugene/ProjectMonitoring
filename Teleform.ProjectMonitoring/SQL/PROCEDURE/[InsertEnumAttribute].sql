IF OBJECT_ID('[InsertEnumAttribute]', 'P') IS NOT NULL
	DROP PROCEDURE [InsertEnumAttribute]
GO

CREATE PROCEDURE [InsertEnumAttribute]
	@enumName VARCHAR(30),
	@attributeValue VARCHAR(100)
AS
	DECLARE @query VARCHAR(1000) = '
		SET IDENTITY_INSERT [' + @enumName + '] ON
		DECLARE @lastID INT
		SELECT @lastID = MAX([objID]) FROM [' + @enumName + ']
		SET @lastID = @lastID + 1
		INSERT [' + @enumName + '] ([objID], [name]) VALUES (@lastID, ''' + @attributeValue + ''')
		SET IDENTITY_INSERT [' + @enumName + '] OFF'

		print @query
		EXEC sp_sqlexec @query
GO