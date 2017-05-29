IF OBJECT_ID('[GetContractLots]','P') IS NOT NULL
	DROP PROC [GetContractLots]
GO

CREATE PROC [GetContractLots]
	@xml XML
AS
	IF @xml IS NULL SET @xml = ''

	DECLARE @query VARCHAR(MAX) = '
		SELECT
			[TC].[objID],
			[TC].[_tenderID] [tenderID],
			[T].[name] [tender],
			[T].[purchaseNumber],
			[TC].[lotNumber],
			[TC].[comment],
			[TC].[appAmount],
			[TC].[appAmountReceived],
			[C].[name] [contract]
		FROM [_TenderContract] [TC]
		JOIN [_Tender] [T] ON [TC].[_tenderID] = [T].[objID]
		JOIN [_Contract] [C] ON [TC].[_contractID] = [C].[objID]' +
		[Search].[CreateWhereStatement]('GetContractLots', @xml) 

	EXEC sp_SqlExec @query
GO