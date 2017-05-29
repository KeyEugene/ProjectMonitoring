IF OBJECT_ID('[DeleteUserInfo]', 'P') IS NOT NULL
	DROP PROCEDURE [DeleteUserInfo]
GO

CREATE PROCEDURE [DeleteUserInfo] @id INT
AS
	DELETE FROM [_User] WHERE [_personID] = @id
	
	
	
	UPDATE [_Accomplice] SET [signerID] = NULL WHERE [signerID] = @id
	UPDATE [_Accomplice] SET [executiveID] = NULL WHERE [executiveID] = @id
	
	DELETE FROM [_DivisionPerson] WHERE [_personID] = @id
	
	DELETE FROM [Import].[History] WHERE [_personID] = @id
	
	DELETE FROM [_Person] WHERE [objID] = @id
	--DELETE FROM [_User] WHERE [_personID] = @id
GO