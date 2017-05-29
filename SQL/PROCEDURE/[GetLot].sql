IF OBJECT_ID('[GetLot]','P') IS NOT NULL
	DROP PROC [GetLot]
GO

CREATE PROC [GetLot]
	@lotID ObjID
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
	WHERE [TC].[objID] = @lotID
GO