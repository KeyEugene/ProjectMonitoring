IF OBJECT_ID('[UpdateUser]', 'P') IS NOT NULL
	DROP PROCEDURE [UpdateUser]
GO

CREATE PROCEDURE [UpdateUser]
	@personID INT,
	@login VARCHAR(100),
	@pwd VARCHAR(100),
	@disable BIT
AS

	DECLARE @errorMessage VARCHAR(4000)
	IF LEN(@pwd) > 20
	BEGIN
		RAISERROR('Пароль не должен превышать 20 символов.', 16, 1)
		RETURN
	END

	IF (SELECT COUNT([_personID]) FROM [_User] WHERE [_personID] = @personID) = 0
		BEGIN TRY
			EXEC [InsertUser] @personID, @login, @pwd, @disable
			RETURN
		END TRY
		BEGIN CATCH
			SELECT @errorMessage = ERROR_MESSAGE()
			RAISERROR(@errorMessage, 16, 1)
			RETURN
		END CATCH
	
	DECLARE @query VARCHAR(1000)
	BEGIN TRY
		SET @query = 'UPDATE [_User] SET [login]=''' + @login + ''', [disable]=' + CONVERT(VARCHAR(1), @disable)
		IF @pwd IS NOT NULL AND @pwd <> ''
			SET @query = @query + ', [pwd]=''' + @pwd + ''''
		SET @query = @query + ' WHERE [_personID]=' + CONVERT(VARCHAR(5), @personID)

		--select @query
		EXEC sp_sqlexec @query
	END TRY
	BEGIN CATCH
		RAISERROR('Не удалось обновить данные пользователя.', 16, 1)
		PRINT @query
	END CATCH
GO