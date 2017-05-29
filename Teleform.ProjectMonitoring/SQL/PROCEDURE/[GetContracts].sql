IF NOT OBJECT_ID('[GetContracts]', 'P') IS NULL
	DROP PROCEDURE [GetContracts]
GO
CREATE PROCEDURE [GetContracts]
	@xml XML
AS
	IF @xml IS NULL SET @xml = ''
	DECLARE @query VARCHAR(MAX) = '
	SELECT
		[C].[objID],
		[C].[name],
		[C].[number],
		[C].[startYear],
		[C].[finishYear],
		[C].[start],
		[C].[finish],
		[C].[cost],
		[C].[_workTypeID],
		[C].[_programID],
		[C].[_statusID],
		[C].[closedWorking],
        [D].[name] [division],
        [A].[_divisionID] [divisionID],
        [WT].[name] [workType],
        [P].[name] [programType],
        [S].[name] [status]
	FROM [_Contract] [C]
		LEFT JOIN [_Accomplice] [A] ON [C].[objID] = [A].[_contractID] AND [A].[_accompliceRoleID] = 1
		LEFT JOIN [_Division] [D] ON [D].[objID] = [A].[_divisionID]
		LEFT JOIN [_WorkType] [WT] ON [C].[_workTypeID] = [WT].[objID]
		LEFT JOIN [_Program] [P] ON [C].[_programID] = [P].[objID]
		LEFT JOIN [_Status] [S] ON [C].[_statusID] = [S].[objID]' +
		[Search].[CreateWhereStatement]('GetContracts', @xml)

	EXEC sp_SqlExec @query
GO