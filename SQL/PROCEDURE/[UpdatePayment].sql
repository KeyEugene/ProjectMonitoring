IF OBJECT_ID('[UpdatePayment]','P') IS NOT NULL
	DROP PROC [UpdatePayment]
GO

CREATE PROCEDURE [UpdatePayment]
	 @id OBJID,
	 --@stageID tinyInt,
	 --@contractID OBJID,
	 @date Date,
	 @dateReal Date,
	 @value varchar(64),
	 @valueReal varchar(64),
	 @typeID OBJID,
	 @comment varchar(1024),
	 @division OBJID,
	 @purposeID Tiny
AS
BEGIN
	/*IF @date IS NULL
	BEGIN
		RAISERROR('Поле "Дата-план" обязательно для заполнения',18,1)
		RETURN
	END*/
	
	IF @division IS NULL
	BEGIN
		RAISERROR('Поле "Организация" обязательно для заполнения',18,1)
		RETURN
	END
	
	IF @value IS NULL
	BEGIN
		RAISERROR('Поле "Сумма-план" обязательно для заполнения',18,1)
		RETURN
	END
	
	UPDATE [_Payment]
	SET
		[date] = @date,
		[dateReal] = @dateReal,
		[typeID] = @typeID,
		[value] = CAST (@value AS Money),
		[valueReal] = CAST (@valueReal AS Money),
		[comment] = @comment,
		[_divisionID] = @division,
		[purposeID] = @purposeID
	WHERE [objID] = @id--[_stage] = @stageID AND [_contractID] = @contractID AND [objID] = @id
END
GO
