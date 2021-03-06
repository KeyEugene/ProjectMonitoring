USE [Minprom]
GO
/****** Object:  StoredProcedure [dbo].[InsertTender]    Script Date: 04.12.2013 21:40:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[InsertTender]
	@purchaseNumber VARCHAR(50), 
	@year INT, 
	@number INT,
	@cost MONEY,
	@dateToOpen SMALLDATETIME,
	@dateToOpen2 SMALLDATETIME,
	@dateToExamination SMALLDATETIME,
	@dateToExamination2 SMALLDATETIME,
	@dateToExamination3 SMALLDATETIME,
	@dateToExamination4 SMALLDATETIME,
	@dateToSolution SMALLDATETIME,
	@dateToSolution2 SMALLDATETIME,
	@dateToSolution3 SMALLDATETIME,
	@dateToSolution4 SMALLDATETIME,
	@programID INT
AS
	IF @number IS NULL
	BEGIN
		RAISERROR('Не указана "Часть" тендера.', 16, 1)
		RETURN
	END

	IF @number < 0 OR @number > 256
	BEGIN
		RAISERROR('Номер должен быть в диапазоне от 1 до 255',18,1);
		RETURN
	END
	
	IF @year IS NOT NULL AND (@year <1990 OR @year > 2050)
	BEGIN
		RAISERROR('Год должен быть больше 1990 и меньше 2050.',18,1)
		RETURN
	END
	
	INSERT INTO [_Tender] 
				([purchaseNumber],
				[year],
				[number],
				[cost],
				[dateToOpen],
				[dateToOpen2],
				[dateToExamination],
				[dateToExamination2],
				[dateToExamination3],
				[dateToExamination4],
				[dateToSolution],
				[dateToSolution2],
				[dateToSolution3],
				[dateToSolution4],
				[_programID])
	VALUES
				(@purchaseNumber, 
				@year, 
				CAST(@number AS TINYINT),
				@cost, 
				@dateToOpen, 
				@dateToOpen2, 
				@dateToExamination, 
				@dateToExamination2,
				@dateToExamination3,
				@dateToExamination4,
				@dateToSolution,
				@dateToSolution2,
				@dateToSolution3,
				@dateToSolution4,
				@programID)
GO
	
