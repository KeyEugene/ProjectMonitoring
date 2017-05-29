IF OBJECT_ID('[UpdateTender]', 'P') IS NOT NULL
	DROP PROC [UpdateTender]
GO
CREATE PROC [UpdateTender]
	@id INT, 
	@purchaseNumber VARCHAR(50), 
	@year INT, 
	@number TINYINT,
	@cost MONEY,
	@dateToOpen SMALLDATETIME,
	@dateToExamination SMALLDATETIME,
	@dateToSolution SMALLDATETIME,
	
	@dateToOpen2 SMALLDATETIME,
	@dateToExamination2 SMALLDATETIME,
	@dateToExamination3 SMALLDATETIME,
	@dateToExamination4 SMALLDATETIME,
	@dateToSolution2 SMALLDATETIME,
	@dateToSolution3 SMALLDATETIME,
	@dateToSolution4 SMALLDATETIME,
	@programID INT
AS
	IF @number IS NULL
	BEGIN
		RAISERROR('Не указана "Часть" тендера',18,1)
		RETURN
	END
	
	IF @number < 0 OR @number > 256
	BEGIN
		RAISERROR('Номер должен быть в диапазоне от 1 до 255',18,1);
		RETURN
	END
	
	IF @year <1990 OR @year > 2050
	BEGIN
		RAISERROR('Год должен быть больше 1990 и меньше 2050.',18,1)
		RETURN
	END
	
	UPDATE [_Tender] SET
		[purchaseNumber] = @purchaseNumber,
		[year] = @year,
		[number] = @number,
		[cost] = @cost,
		[dateToOpen] = @dateToOpen,
		[dateToExamination] = @dateToExamination,
		[dateToSolution] = @dateToSolution,
		[dateToOpen2] = @dateToOpen2,
		[dateToExamination2] = @dateToExamination2,
		[dateToExamination3] = @dateToExamination3,
		[dateToExamination4] = @dateToExamination4,
		[dateToSolution2] = @dateToSolution2,
		[dateToSolution3] = @dateToSolution3,
		[dateToSolution4] = @dateToSolution4,
		[_programID] = @programID
	WHERE [objID] = @id
GO