IF OBJECT_ID('[UpdateCost]', 'P') IS NOT NULL
	DROP PROC [UpdateCost]
GO

CREATE PROCEDURE [UpdateCost]
	@id OBJID,
	@value VARCHAR(128),
	@workName VARCHAR(1024)
AS
	IF @value IS NULL
	BEGIN
		RAISERROR('Должна быть указана сумма.',18,1)
		RETURN
	END
	
	UPDATE [_ContractCost]
	SET
	[value] = CAST(@value AS Money),
	[workName] = @workName
	WHERE [objID] = @id
GO
