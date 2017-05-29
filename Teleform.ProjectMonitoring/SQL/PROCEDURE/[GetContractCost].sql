IF NOT OBJECT_ID('[GetContractCost]', 'P') IS NULL
	DROP PROCEDURE [GetContractCost]
GO
CREATE PROCEDURE [dbo].[GetContractCost]
	@xml XML
AS
	IF @xml IS NULL SET @xml = ''

	DECLARE @query VARCHAR(MAX) = 
	'SELECT
		[CC].[objID], 
		[CC].[year],
		[CC].[value],
		[CC].[workName] 
	FROM [_ContractCost] [CC]'+
	[Search].[CreateWhereStatement]('GetContractCost', @xml)

	EXEC sp_SqlExec @query
GO