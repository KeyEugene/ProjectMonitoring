IF OBJECT_ID('[GetAccompliceFinancing]', 'P') IS NOT NULL
	DROP PROC [GetAccompliceFinancing]
GO

CREATE PROCEDURE [GetAccompliceFinancing]
	@id ObjID
AS
	SELECT [DS].[name] [sourceName],
		   [F].[name] [financingName],
		   [C].[name] [contractName],
		   [D].[name] [divisionName],
		   [AF].[typeID] [financingID],
		   [AF].[year],
		   [AF].[value],
		   [AF].[valueReal],
		   [AF].[sourceID]
	FROM [_AccompliceFinancing] [AF]	  
	JOIN [_FinancingType] [F] ON [AF].[typeID] = [F].[objID]
	JOIN [_Contract] [C] ON [C].[objID] = [AF].[_contractID]
	JOIN [_Division] [D] ON [D].[objID] = [AF].[_divisionID]
	LEFT JOIN [_Division] [DS] ON [AF].[sourceID] = [DS].[objID]
	WHERE [AF].[objID] = @id 