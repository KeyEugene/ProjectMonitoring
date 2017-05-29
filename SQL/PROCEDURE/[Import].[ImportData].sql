IF OBJECT_ID('[Import].[ImportData]', 'P') IS NOT NULL
	DROP PROCEDURE [Import].[ImportData]
GO

CREATE PROCEDURE [Import].[ImportData]
	@userID INT,
	@filePath VARCHAR(1000),
	@addFlag BIT
AS
	DECLARE @time DATETIME = GETDATE()
	INSERT [Import].[History] ([_personID], [time], [stateID])
	VALUES (@userID, @time, 3)

	DECLARE @id INT = SCOPE_IDENTITY()

	DECLARE @status INT, @r_message VARCHAR(1000)
	EXEC [Import].[LoadData] @filePath, @status OUT, @r_message, @add = @addFlag

	DECLARE @isError BIT = 0
	IF @status = 2
		SET @isError = 1


	DECLARE @query VARCHAR(1000) = 
	'UPDATE [Import].[History] SET [stateID] = ' + CONVERT(VARCHAR(5), @status) + ' WHERE [id] = ' + CONVERT(VARCHAR(5), @id)
	EXEC sp_sqlexec @query

	--IF @isError = 1
	--	RAISERROR(@r_message, 16, 1)
GO

