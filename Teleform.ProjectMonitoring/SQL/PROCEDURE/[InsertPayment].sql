IF OBJECT_ID('[InsertPayment]','P') IS NOT NULL
	DROP PROC [InsertPayment]
GO

CREATE PROCEDURE [InsertPayment]
	 @stageID varchar(3),
	 @contractID OBJID,
	 @date Date,
	 @dateReal Date,
	 @value varchar(64),
	 @valueReal varchar(64),
	 @typeID OBJID,
	 @comment varchar(1024),
	 @divisionID OBJID,
	 @purposeID varchar(3)
AS
BEGIN
	/*IF @date IS NULL
	BEGIN
		RAISERROR('Поле "Дата-план" обязательно для заполнения',18,1)
		RETURN
	END*/
	
	IF @contractID IS NULL
	BEGIN
		RAISERROR('Поле "Проект" обязательно для заполнения',18,1)
		RETURN
	END
	
	IF @stageID IS NULL
	BEGIN
		RAISERROR('Поле "Этап" обязательно для заполнения',18,1)
		RETURN
	END
	
	IF @value IS NULL
	BEGIN
		RAISERROR('Поле "Сумма-план" обязательно для заполнения',18,1)
		RETURN
	END
	
	IF @divisionID IS NULL
	BEGIN
		RAISERROR('Поле Организация обязательно для заполнения',18,1)
		RETURN
	END
	
	INSERT INTO [_Payment]
	([_stage],[_contractID],[date],[dateReal],[value],[valueReal],[typeID],[comment],[_divisionID],[purposeID])
	VALUES
	(CONVERT(tinyint,@stageID),@contractID,@date,@dateReal,CAST(@value as Money),CAST(@valueReal as Money),@typeID,@comment,@divisionID,CONVERT(TinyInt,@purposeID))
END
GO
