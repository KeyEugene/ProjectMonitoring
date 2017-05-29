IF OBJECT_ID('[DeleteMinPromUser]', 'P') IS NOT NULL
	DROP PROCEDURE [DeleteMinPromUser]
GO

CREATE PROCEDURE [DeleteMinPromUser] @id INT
AS
	DELETE FROM [_User] WHERE [_personID] = @id
GO