IF OBJECT_ID('[GetTender]', 'P') IS NOT NULL
	DROP PROC [GetTender]
GO

CREATE PROC [GetTender]
	@id ObjID
AS
	SELECT 
		[T].[objID],
		[T].[purchaseNumber],
		[T].[year],
		[T].[number],
		[T].[cost],
		[T].[name],
		[T].[dateToOpen],
		[T].[dateToExamination],
		[T].[dateToSolution],
		[T].[dateToOpen2],
        [T].[dateToExamination2],
        [T].[dateToExamination3],
        [T].[dateToExamination4],
        [T].[dateToSolution2],
        [T].[dateToSolution3],
        [T].[dateToSolution4],
        [T].[_programID],
        [P].[name] [program]
	FROM [_Tender] [T]
	LEFT JOIN [_Program] [P] ON [P].[objID] = [T].[_programID]
	WHERE [T].[objID]= @id
GO