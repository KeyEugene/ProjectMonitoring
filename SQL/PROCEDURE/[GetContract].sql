IF NOT OBJECT_ID('[GetContract]', 'P') IS NULL
	DROP PROCEDURE [GetContract]
GO

CREATE PROCEDURE [dbo].[GetContract]
	@id INT, @flag INT
AS
	SELECT
		[C].[objID],
		[C].[name],
		[C].[number],
		[C].[workName],
		[C].[start],
		[C].[startYear],
		[C].[finish],
		[dbo].[FormatMoney]([C].[cost], @flag) [cost],
		[C].[_workTypeID],
		[WT].[objID] [workType],
		[C].[_programID],
		[P].[name] [program],
		[A].[_divisionID] [divisionID],
		[D].[name] [division],
		[A].[signerID],
		[PS].[name] [signer],
		[A].[executiveID],
		[PE].[name] [executive],
		[S].[name] [status],
		[C].[_statusID],
		[C].[closedWorking],
		[A].[objID] [accompliceID]
	FROM [_Contract] [C]
	JOIN [_WorkType] [WT] ON [WT].[objID] = [C].[_workTypeID]
	LEFT JOIN [_Status] [S] ON [S].[objID] = [C].[_statusID]
	LEFT JOIN [_Program] [P] ON [P].[objID] = [C].[_programID]
	LEFT JOIN [_Accomplice] [A] ON [A].[_contractID] = [C].[objID] AND [A].[_accompliceRoleID] = 1
	LEFT JOIN [_Division] [D] ON [D].[objID] = [A].[_divisionID]
	LEFT JOIN [_Person] [PS] ON [PS].[objID] = [A].[signerID]
	LEFT JOIN [_Person] [PE] ON [PE].[objID] = [A].[executiveID]
	WHERE [C].[objID]=@id