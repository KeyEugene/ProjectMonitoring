IF NOT OBJECT_ID('[GetAccomplices]', 'P') IS NULL
	DROP PROCEDURE [GetAccomplices]
GO
CREATE PROCEDURE [dbo].[GetAccomplices]
	@xml XML
AS
	IF @xml IS NULL SET @xml = ''
	
	DECLARE @query VARCHAR(MAX) =
	'SELECT
		[A].[objID],
		[A].[_divisionID] [id],
		[D].[name] [name],
		[A].[signerBase] [reason],
		[S].[name] [signer],
		[A].[signerID],
		[E].[name] [executive],
		[A].[executiveID],
		[AR].[name] [roleName]
	FROM [_Accomplice] [A] 
        JOIN [_AccompliceRole] [AR] ON [AR].[objID] = [A].[_accompliceRoleID]
        JOIN [_Division] [D] ON [D].[objID] = [A].[_divisionID]
        LEFT JOIN [_Person] [S] ON [S].[objID] = [A].[signerID]
        LEFT JOIN [_Person] [E] ON [E].[objID] = [A].[executiveID]' +
	[Search].[CreateWhereStatement]('GetAccomplices', @xml) 
	
	EXEC sp_SqlExec @query
GO