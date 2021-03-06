IF OBJECT_ID('GetObjectInfo', 'P') IS NOT NULL
	DROP PROCEDURE GetObjectInfo
GO

CREATE PROCEDURE [dbo].[GetObjectInfo] @uid VARCHAR(255)
AS
DECLARE 
	--@uid VARCHAR (255) = '0271BE87-7C39-4059-B7F0-F62357648E6A',
	--@uid VARCHAR (255) = '4C8AC5B0-B018-42C3-9327-17EEA1D6A5D9',
	@objID INT,
	@table VARCHAR(100)
SELECT 
	@objID = [ZE].[objID], 
	@table = [ZET].[base]
FROM [Z_Entity] [ZE] 
JOIN [Z_EntityType] [ZET] ON [ZE].[typeID] = [ZET].[objID] AND [ZE].[uID] = @uid

DECLARE @query VARCHAR(1000)
SET @query = 'SELECT name FROM ' + @table + ' WHERE objID = ' + CAST(@objID as varchar(10))

DECLARE @name TABLE(name VARCHAR(200))
INSERT @name
EXEC sp_sqlexec @query

SELECT * FROM @name