IF NOT OBJECT_ID('[GetStagePayment]', 'P') IS NULL
	DROP PROCEDURE [GetStagePayment]
GO
CREATE PROCEDURE [dbo].[GetStagePayment]
	@xml XML
AS
BEGIN
	IF @xml IS NULL SET @xml = ''
	
	DECLARE @query VARCHAR(MAX) =
	'SELECT 
		[P].[objID],
		[P].[date],
		[P].[value],
		[P].[comment],
		[P].[valueReal],
		[P].[dateReal],
		[PT].[name] [paymentType],
		[PP].[name] [purpose],
		[D].[name] [division]
	FROM [_Payment] [P]
	JOIN [_Division] [D] ON [D].[objID] = [P].[_divisionID]
	LEFT JOIN [_PaymentType] [PT] ON [PT].[objID] = [P].[typeID]
	LEFT JOIN [_PaymentPurpose] [PP] ON [PP].[objID] = [P].[purposeID]' +
	[Search].[CreateWhereStatement]('GetStagePayment', @xml) 
	
	EXEC sp_SqlExec @query
END

GO