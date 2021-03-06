USE [Minprom]
GO
/****** Object:  StoredProcedure [dbo].[SelectStage]    Script Date: 27.08.2013 16:21:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SelectStage]
	@_contractID ObjID,
	@number int
AS
	DECLARE @summaryT varchar(1024)

	SELECT @summaryT = [summary] FROM [_Stage] WHERE [_contractID] = @_contractID AND [number]=@number

	IF @summaryT IS NOT NULL
	BEGIN
		SELECT  summary,
				number,
				daysToEnd,
				cost,
				payment,
				actDate,
				paymentUptodate,
				start,
				finish,
				_summaryID
		FROM [_Stage]
			WHERE _contractID=@_contractID AND number=@number
	END
	ELSE
	BEGIN
		SELECT  name as summary,
				number,
				daysToEnd,
				cost,
				payment,
				actDate,
				paymentUptodate,
				start,
				finish,
				_summaryID
		FROM [_Stage] S LEFT JOIN [_StageSummary] SS ON SS.objID=S._summaryID
		WHERE _contractID=@_contractID AND number=@number
	END
RETURN