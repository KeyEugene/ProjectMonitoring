USE [Minprom]
GO
/****** Object:  StoredProcedure [dbo].[InsertStage]    Script Date: 04.12.2013 21:37:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[InsertStage]
	@contractID ObjID,
	@number INT,
	@stageName varchar(255),
	@daysToEnd Int,
	@startReal Date,
	@finishReal Date,
	@actDate Date,
	@cost varchar(64),	
	@financing Money,
	@paymentUptodate Bit,
	@summary varchar(1024)
AS
	IF @contractID IS NULL
	BEGIN
		RAISERROR('Не указан проект.', 16, 1)
		RETURN
	END
	
	
	IF @number IS NULL
	BEGIN
		RAISERROR('Не указан номер этапа.', 16, 1)
		RETURN
	END

	IF (SELECT COUNT(*) FROM [_Stage] WHERE [_contractID] = @contractID AND [number] = @number) > 0
	BEGIN
		RAISERROR('Этап с таким номером уже существует.',18,1)
		RETURN
	END
	
	IF @number < 0 OR @number > 256
	BEGIN
		RAISERROR('Номер должен быть от 1 до 255.',18,1)
		RETURN
	END
		
	
	INSERT INTO [_Stage]
			([_contractID],
			[number], 
			[name0], 
			[daysToEnd], 
			[startReal], 
			[finishReal], 
			[actDate], 
			[cost], 
			[financing],
			[paymentUptodate], 
			[summary])
	VALUES
			(@contractID,
			CAST(@number AS TinyInt),
			@stageName, 
			@daysToEnd, 
			@startReal, 
			@finishReal, 
			@actDate, 
			CAST(@cost AS Money), 
			CAST(@financing AS Money), 
			@paymentUptodate, 
			@summary)
GO

