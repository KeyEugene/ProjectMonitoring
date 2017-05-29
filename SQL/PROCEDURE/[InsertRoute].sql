IF OBJECT_ID('[InsertRoute]', 'P') IS NOT NULL
	DROP PROC [InsertRoute]
GO

CREATE PROC [InsertRoute]
	@documentID ObjID,
	@divisionID ObjID,
	@stateID tinyInt,
	@sent SMALLDATETIME,
	@expected SMALLDATETIME,
	@done SMALLDATETIME
AS
	IF @divisionID IS NULL
	BEGIN
		RAISERROR('Необходимо выбрать исполнителя.',18,1)
		RETURN
	END
	
	IF @stateID IS NULL
	BEGIN
		RAISERROR('Необходимо выбрать состояние документа.',18,1)
		RETURN
	END
	
	IF @documentID IS NULL
	BEGIN	
		RAISERROR('Необходимо выбрать документ',18,1)
		RETURN
	END
	
	DECLARE @contractID INT
	SELECT @contractID = [_contractID] FROM [_Application] WHERE [objID] = @documentID


	INSERT INTO [_Route] 
		([_applicationID],[_divisionID],[stateID],[sent],[expected],[done], [_contractID])
	VALUES
		(@documentID,@divisionID,@stateID,@sent,@expected,@done, @contractID)
	
GO