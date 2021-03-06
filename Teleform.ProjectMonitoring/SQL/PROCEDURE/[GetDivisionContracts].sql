USE [Minprom]
GO
/****** Object:  StoredProcedure [dbo].[GetDivisionContracts]    Script Date: 12/25/2013 19:35:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetDivisionContracts]
	@xml XML
AS
	IF @xml IS NULL SET @xml = ''
	
	DECLARE @divisionID objID
	
	--SELECT @divisionID = [action].[r].[value]('@division', 'INT') FROM @xml.[nodes]('/filter') [action]([r])
	SELECT @divisionID = @xml.[value]('(action/filter/group/@division)[1]', 'INT')
	
	PRINT @divisionID

	DECLARE @query VARCHAR(MAX) = '
		DECLARE @divisionID objID = ' + LTRIM(STR(@divisionID)) + '
		SELECT
			[C].[objID],
			[C].[name],
			[C].[number],
			[C].[startYear],
			[C].[finishYear],
			[C].[start],
			[C].[finish],
			[C].[cost],
			[C].[closedWorking],
			[D].[name] [division],
			[D].[objID] [divisionID],
			[WT].[name] [workType],
			[PT].[name] [program],
			[S].[name] [status]
		FROM 
			[_Division] as [D]
			JOIN [_Accomplice] as [A] ON [D].[objID] = [A].[_divisionID] AND [A].[_divisionID] = @divisionID
			JOIN [_Contract] as [C] ON [A].[_contractID] = [C].[objID] AND A._divisionID = C.mainExecutorID 
			LEFT JOIN [_WorkType] [WT] ON [C].[_workTypeID] = [WT].[objID]
			LEFT JOIN [_Program] [PT] ON [C].[_programID] = [PT].[objID]
			LEFT JOIN [_Status] [S] ON [C].[_statusID] = [S].[objID]' +
		[Search].[CreateWhereStatement]('GetDivisionContracts', @xml)
			
	EXEC sp_SqlExec @query

--exec GetDivisionContracts '<action userID="1">  <filter>    <group division="2" />  </filter></action>'
