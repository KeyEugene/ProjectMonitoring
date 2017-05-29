IF OBJECT_ID('[UpdateLot]','P') IS NOT NULL
	DROP PROC [UpdateLot]
GO

CREATE PROC [UpdateLot]
		@id ObjID,
		@lotNumber INT,
		@comment VARCHAR(1000),
		@appAmount INT,
		@appAmountR INT
AS

	IF @lotNumber IS NULL
	BEGIN
		RAISERROR('Необходимо выбрать лот.',18,1)
		RETURN
	END
	
	IF @appAmount IS NULL
	BEGIN
		RAISERROR('Необходимо выбрать количество поданных заявок.',18,1)
		RETURN
	END
	
	IF @appAmountR IS NULL
	BEGIN
		RAISERROR('Необходимо выбрать количество принятых заявок.',18,1)
		RETURN
	END
	
	UPDATE [_TenderContract] SET
		[lotNumber] = @lotNumber,
		[comment] = @comment,
		[appAmount] = @appAmount,
		[appAmountReceived] = @appAmountR
	WHERE [objID] = @id
GO