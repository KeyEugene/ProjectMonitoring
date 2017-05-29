IF OBJECT_ID('[GetLots]','P') IS NOT NULL
	DROP PROC [GetLots]
GO

CREATE PROC [GetLots]
	@tenderID ObjID
AS
	SELECT
		[TC].[objID],
		[TC].[_contractID] [contractID],
		[C].[name] [contract],
		[TC].[_tenderID] [tenderID],
		[T].[name] [tender],
		[TC].[lotNumber],
		[TC].[comment],
		[TC].[appAmount],
		[TC].[appAmountReceived]
	FROM [_TenderContract] [TC]
	JOIN [_Contract] [C] ON [TC].[_contractID] = [C].[objID]
	JOIN [_Tender] [T] ON [TC].[_tenderID] = [T].[objID]
	WHERE [TC].[_tenderID] = @tenderID
GO