IF OBJECT_ID('GetUIDForObject', 'FN') IS NOT NULL
	DROP FUNCTION GetUIDForObject
GO
CREATE FUNCTION GetUIDForObject (@objID int, @base varchar(50))
RETURNS varchar(50)
AS
BEGIN
	DECLARE @uid varchar(100),
			@c int

	SELECT @uid = [e].[uID] 
	FROM [Z_Entity] [e] 
	JOIN [Z_EntityType] [et] ON [e].[typeID] = [et].[objID] AND [e].[objID] = @objID AND [et].[base] = @base

	RETURN LTRIM(@uid)
END
GO