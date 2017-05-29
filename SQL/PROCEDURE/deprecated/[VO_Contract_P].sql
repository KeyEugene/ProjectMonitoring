IF OBJECT_ID('VO__Contract_P', 'V') IS NOT NULL
	DROP VIEW VO__Contract_P
GO
CREATE VIEW VO__Contract_P
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
 [C].[_programID],
 [T].[name] [tender],
 [A].[_divisionID] [divisionID],
 [EDiv].[uID] [divisionUID],
 [A].[signerID],
 [ESign].[uID] [signerUID],
 [A].[executiveID],
 [EExec].[uID] [executiveUID],
 [S].[name] [status],
 [P].[name] [program],
 [WT].[name] [workType],
 [D].[name] [division],
 [SP].[name] [signer],
 [SE].[name] [executive],
 [C].[_statusID]
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
--program name
LEFT JOIN [_Program] [P] ON [P].[objID] = [C].[_programID]
--workType name
LEFT JOIN [_WorkType] [WT] ON [WT].[objID] = [C].[_workTypeID]
--division Name
LEFT JOIN [_Division] [D] ON [D].[objID] = [A].[_divisionID]
--signer Name
LEFT JOIN [_Person] [SP] ON [SP].[objID] = [A].[signerID]
--executive Name
LEFT JOIN [_Person] [SE] ON [SE].[objID] = [A].[executiveID]
GO