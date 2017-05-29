IF OBJECT_ID('[InsertUserInfo]', 'P') IS NOT NULL
	DROP PROCEDURE [InsertUserInfo]
GO

CREATE PROCEDURE [InsertUserInfo]
	@name VARCHAR(255),
	@sname VARCHAR(255),
	@mname VARCHAR(255),
	@ablative VARCHAR(255),
	@genitive VARCHAR(255),
	@positionID INT,
	@email VARCHAR(255),
	@phone VARCHAR(255),
	@login VARCHAR(100) = NULL, 
	@pwd VARCHAR(100) = NULL,
	@disable BIT = NULL
AS

	BEGIN TRAN InsertMinUser
	
	DECLARE @errorMessage VARCHAR(4000)
	DECLARE @personID INT

	BEGIN TRY
		EXEC [InsertPerson]
			@sname,
			@name,
			@mname,
			@genitive,
			@ablative,
			@phone,
			@email,
			@personID OUT
	END TRY
	BEGIN CATCH
		SELECT @errorMessage = ERROR_MESSAGE()
		RAISERROR(@errorMessage, 16, 1)
		ROLLBACK TRAN InsertMinUser
		RETURN
	END CATCH

	IF @personID IS NULL
	BEGIN
		RAISERROR('Не удалось получить идентификатор нового пользователя.', 16, 1)
		ROLLBACK TRAN InsertMinUser
		RETURN
	END 


	BEGIN TRY
		INSERT [_DivisionPerson] ([_divisionID], [_personID], [_personRoleID])
			SELECT 0, @personID, @positionID
	END TRY
	BEGIN CATCH
		RAISERROR('Не удалось добавить пользователя в организацию Минпромторг.', 16, 1)
		ROLLBACK TRAN InsertMinUser
		RETURN
	END CATCH

	BEGIN TRY
		EXEC [InsertUser]
			@personID,
			@login,
			@pwd,
			@disable
	END TRY
	BEGIN CATCH
		SELECT @errorMessage = ERROR_MESSAGE()
		RAISERROR(@errorMessage, 16, 1)
		ROLLBACK TRAN InsertMinUser
		RETURN
	END CATCH

	COMMIT TRAN InsertMinUser
GO