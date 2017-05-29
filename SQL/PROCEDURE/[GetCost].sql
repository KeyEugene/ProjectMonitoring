IF OBJECT_ID('[GetCost]','P') IS NOT NULL
	DROP PROC [GetCost]
GO 

CREATE PROCEDURE [GetCost]
	@id OBJID
AS
	SELECT
	[CC].[objID],
	[CC].[value],
	[CC].[year],
	[CC].[workName]
	FROM [_ContractCost] [CC]
	WHERE [CC].[objID] = @id
GO
