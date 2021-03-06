USE [Minprom]
GO
/****** Object:  StoredProcedure [dbo].[InsertLot]    Script Date: 04.12.2013 21:29:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [dbo].[InsertLot]
	@tenderID ObjID,
	@contractID ObjID,
	@lotNumber INT,
	@comment VARCHAR(1024),
	@appAmount tinyInt,
	@appAmountR tinyInt
AS
	IF @contractID IS NULL
	BEGIN
		RAISERROR('Необходимо указать проект.',18,1)
		RETURN
	END
	
	IF (SELECT COUNT(*) FROM [_TenderContract] WHERE [_tenderID] = @tenderID AND [_contractID] = @contractID) > 0
	BEGIN
		RAISERROR('Данный номер уже присутствует в этом проекте.',18,1)
		RETURN
	END
			
	IF @tenderID IS NULL
	BEGIN
		RAISERROR('Необходимо указать тендер',18,1)
		RETURN
	END

	IF @lotNumber IS NULL
	BEGIN
		RAISERROR('Необходимо указать номер лота.',18,1)
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

	INSERT INTO [_TenderContract]
	(
		[_tenderID],
		[_contractID],
		[lotNumber],
		[comment],
		[appAmount],
		[appAmountReceived]
	)
	VALUES
	(
		@tenderID,
		@contractID,
		@lotNumber,
		@comment,
		@appAmount,
		@appAmountR
	)
	
	UPDATE [_Contract]
	SET [_tenderID] = @tenderID
	WHERE [objID] = @contractID
GO
