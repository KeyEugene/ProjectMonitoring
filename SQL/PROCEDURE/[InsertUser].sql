IF OBJECT_ID('[InsertUser]', 'P') IS NOT NULL
	DROP PROCEDURE [InsertUser]
GO

CREATE PROCEDURE [InsertUser]
	@personID INT,
	@login VARCHAR(100),
	@pwd VARCHAR(20),
	@disable BIT
AS
	IF (SELECT COUNT([_personID]) FROM [_User] WHERE [_personID] = @personID) > 0
	BEGIN
		RAISERROR('Такой пользователь уже зарегистрирован в системе.', 16, 1)
		RETURN
	END

	IF @login IS NULL AND @pwd IS NULL
		RETURN

	IF @login IS NULL OR @pwd IS NULL
	BEGIN
		RAISERROR('Необходимо указать логин и пароль пользователя.', 16, 1)
		RETURN
	END

	INSERT [_User] ([_personID], [login], [pwd], [disable])
	VALUES (@personID, @login, @pwd, @disable)
GO