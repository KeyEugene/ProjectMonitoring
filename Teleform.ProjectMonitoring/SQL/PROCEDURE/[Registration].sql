IF OBJECT_ID('[Registration]', 'P') IS NOT NULL
DROP PROCEDURE [Registration]
GO

CREATE PROCEDURE [Registration]
	@login VARCHAR(100),
	@password VARCHAR(20)
AS
	IF (SELECT COUNT(*) FROM [_User] WHERE [login] = @login) = 0
	BEGIN
		RAISERROR('Такого пользователя нет',18,1)
		RETURN
	END
	
	IF @password != (SELECT [pwd] FROM [_User] WHERE [login] = @login)
	BEGIN
		RAISERROR('Неверно введен пароль',18,1)
		RETURN
	END
GO
