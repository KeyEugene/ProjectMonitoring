USE [Minprom]
GO
/****** Object:  StoredProcedure [dbo].[InsertCost]    Script Date: 04.12.2013 21:23:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[InsertCost]
	@contractID OBJID,
	@value MONEY,
	@year INT,
	@workName StringL
AS
	IF (SELECT COUNT(*) 
			FROM [_ContractCost] 
			WHERE [_contractID] = @contractID AND [year] = @year)  > 0
	BEGIN
		RAISERROR('В проекте уже установлена сумма для этого года',18,1)
		RETURN
	END
	
	IF @year <1990 OR @year > 2050
	BEGIN
		RAISERROR('Год должен быть больше 1990 и меньше 2050.',18,1)
		RETURN
	END
	
	IF @value IS NULL
	BEGIN
		RAISERROR('Должна быть указана сумма.',18,1)
		RETURN
	END

	IF @year IS NULL
	BEGIN
		RAISERROR('Должен быть указан год.',18,1)
		RETURN
	END

	INSERT INTO [_ContractCost]
	([_contractID], [year], [value], [workName])
	VALUES
	(@contractID, @year, @value, @workName)
GO