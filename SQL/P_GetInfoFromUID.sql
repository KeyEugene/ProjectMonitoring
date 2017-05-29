IF OBJECT_ID('GetInfoFromUID', 'P') IS NOT NULL
	DROP PROCEDURE [GetInfoFromUID]
GO
CREATE PROCEDURE [GetInfoFromUID] @uid varchar(100) 
AS
DECLARE 
	--@uid VARCHAR (255) = '0271BE87-7C39-4059-B7F0-F62357648E6A',
	--@uid VARCHAR (255) = '4C8AC5B0-B018-42C3-9327-17EEA1D6A5D9',
	@objID INT,
	@view VARCHAR(100)
SELECT 
	@objID = [ZE].[objID],
	@view = [ZET].[userView]
FROM [Z_Entity] [ZE] 
JOIN [Z_EntityType] [ZET] ON [ZE].[typeID] = [ZET].[objID] AND [ZE].[uID] = @uid

DECLARE @query varchar(1000)
SET @query = 'SELECT * FROM ' + @view + ' WHERE [objID] = ' + CAST(@objID as varchar(15))
EXEC sp_sqlexec @query
