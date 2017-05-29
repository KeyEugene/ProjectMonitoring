IF OBJECT_ID('VO__Person_P', 'V') IS NOT NULL
	DROP VIEW VO__Person_P
GO

CREATE VIEW VO__Person_P 
AS
SELECT        
	P.objID AS ศฤ, 
	P.name AS ิศฮ, 
	P.genitive AS ิศฮ_๐๏, 
	P.ablative AS ิศฮ_๒๏
FROM 
	_Person P
GO

