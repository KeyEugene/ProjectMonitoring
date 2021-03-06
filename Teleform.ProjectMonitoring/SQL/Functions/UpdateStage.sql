USE [Minprom]
GO
/****** Object:  StoredProcedure [dbo].[UpdateStage]    Script Date: 13.08.2013 15:27:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[UpdateStage]
	@_contractID ObjID,
	@number int,
	@daysToEnd int Output,
	@cost money Output,
	@payment money Output,
	@actDate date Output,
	@paymentUptodate bit Output,
	@summary varchar(1024) Output
AS
	DECLARE @summaryID ObjID

	SELECT @summaryID = [objID] FROM [_StageSummary] WHERE [name] = @summary

	IF @summaryID IS NOT NULL
	BEGIN
		UPDATE [_Stage] SET
				_summaryID=@summaryID,
				summary=null,
				daysToEnd=@daysToEnd,
				cost=@cost,
				payment=@payment,
				actDate=@actDate,
				paymentUptodate=@paymentUptodate
			WHERE _contractID=@_contractID AND number=@number
	END
	ELSE
	BEGIN
		UPDATE [_Stage] SET _summaryID=null, summary=@summary,
				daysToEnd=@daysToEnd,
				cost=@cost,
				payment=@payment,
				actDate=@actDate,
				paymentUptodate=@paymentUptodate
		 WHERE _contractID=@_contractID AND number=@number
	END
RETURN