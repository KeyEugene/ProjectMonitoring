USE [Minprom]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[ContractDetails]
	@objID ObjID
AS
	SELECT 
		 [C].[objID],
		 [C].[name],
		 [C].[number],
		 [C].[WorkName] [workName],
		 [C].[startYear],
		 [C].[start],
		 CAST([C].[start] + CAST([term] AS datetime) AS date) [finish],
		 [C].[cost],
		 [C].[_worktypeID],
		 [C].[_programTypeID],
		 [S].[name] [status],
		 [T].[name] [tender],
		 [A].[_divisionID] [divisionID],
		 [EDiv].[uID] [divisionUID],
		 [A].[signerID],
		 [ESign].[uID] [signerUID],
		 [A].[executiveID],
		 [EExec].[uID] [executiveUID]
	FROM [_Contract] [C]
	-- accomplice
	LEFT JOIN [_Accomplice] [A] ON [A].[_contractID] = [C].[objID] AND [A].[_accompliceRoleID] = 1
	-- division
	LEFT JOIN [Z_Entity] [EDiv] ON [EDiv].[typeID] = (SELECT [ET].[objID] FROM [Z_EntityType] [ET] WHERE [ET].[base]='_Division') AND [EDiv].[objID] = [A].[_divisionID]
	-- signer
	LEFT JOIN [Z_Entity] [ESign] ON [ESign].[typeID] = (SELECT [ET].[objID] FROM [Z_EntityType] [ET] WHERE [ET].[base] = '_Person') AND [ESign].[objID] = [A].[signerID]
	-- executive
	LEFT JOIN [Z_Entity] [EExec] ON [EExec].[typeID] = (SELECT [ET].[objID] FROM [Z_EntityType] [ET] WHERE [ET].[base] = '_Person') AND [EExec].[objID] = [A].[executiveID]
	--status
	LEFT JOIN [_Status] [S] ON [S].[objID] = [C].[_statusID]
	--tender
	LEFT JOIN [_Tender] [T] ON [T].[objID] = [C].[_tenderID]
	WHERE [C].[objID]=@objID
