IF OBJECT_ID('[GetPayment]', 'P') IS NOT NULL
	DROP PROC [GetPayment]
GO

CREATE PROCEDURE [dbo].[GetPayment]
	@id OBJID
	--@stageID tinyInt, 
	--@contractID OBJID
AS
BEGIN
	SELECT 
	   [P].[objID],
       [P].[value],
       [P].[valueReal],
       [P].[date],
       [P].[dateReal],
       [P].[comment],
       [P].[typeID],
       [P].[purposeID],
       [PP].[name] [purposeName],
       [P].[_divisionID] [divisionID],
       [PT].[name] [paymentType],
       [D].[name] [division]
	FROM [_Payment] [P]
	LEFT JOIN [_PaymentType] [PT] ON [PT].[objID] = [P].[typeID]
	LEFT JOIN [_Division] [D] ON [D].[objID] = [P].[_divisionID]
	LEFT JOIN [_PaymentPurpose] [PP] ON [PP].[objID] = [P].[purposeID]
	WHERE [P].[objID] = @id --AND [P].[_stage] = @stageID AND [P].[_contractID] = @contractID
END
GO
