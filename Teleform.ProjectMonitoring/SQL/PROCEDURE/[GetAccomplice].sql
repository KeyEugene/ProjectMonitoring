IF OBJECT_ID('[GetAccomplice]','P') IS NOT NULL
DROP PROCEDURE [GetAccomplice]
GO


CREATE PROCEDURE [dbo].[GetAccomplice]
	@id ObjID
AS
	SELECT 
		[S].[name] [signerName],
		[A].[signerID], 
		[E].[name] [executiveName],
		[A].[executiveID],
		[A].[signerBase] [reason],
		[A].[_accompliceRoleID] [roleID],
		[AR].[name] [roleName],
		[D].[name] [divisionName],
		[C].[name] [contractName]
	FROM [_Accomplice] [A]
		LEFT JOIN [_Person] [S] ON [S].[objID] = [A].[signerID]
        LEFT JOIN [_Person] [E] ON [E].[objID] = [A].[executiveID]
        JOIN [_AccompliceRole] [AR] ON [AR].[objID] = [A].[_accompliceRoleID]
        JOIN [_Division] [D] ON [D].[objID] = [A].[_divisionID]
        JOIN [_Contract] [C] ON [C].[objID] = [A].[_contractID]
	WHERE [A].[objID] = @id
GO


