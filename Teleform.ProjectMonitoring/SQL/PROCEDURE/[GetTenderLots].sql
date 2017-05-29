IF OBJECT_ID('[GetTenderLots]','P') IS NOT NULL
	DROP PROC [GetTenderLots]
GO

CREATE PROC [GetTenderLots] 
	@xml XML
AS
	IF @xml IS NULL SET @xml = ''

	DECLARE @query VARCHAR(MAX) = '
		SELECT
			[TC].[objID],
			[TC].[_contractID] [contractID],
			[C].[name] [contract],
			[TC].[lotNumber],
			[TC].[comment],
			[TC].[appAmount],
			[TC].[appAmountReceived]
		FROM [_TenderContract] [TC]
		JOIN [_Contract] [C] ON [TC].[_contractID] = [C].[objID]' +
		[Search].[CreateWhereStatement]('GetTenderLots', @xml)

	EXEC sp_SqlExec @query
GO