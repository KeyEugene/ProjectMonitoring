IF OBJECT_ID('[UpdateUserInfo]', 'P') IS NOT NULL
	DROP PROCEDURE [UpdateUserInfo]
GO

CREATE PROCEDURE [UpdateUserInfo] 
	@personID INT,
	@name VARCHAR(255),
	@sname VARCHAR(255),
	@mname VARCHAR(255),
	@ablative VARCHAR(255),
	@genitive VARCHAR(255),
	@email VARCHAR(255),
	@phone VARCHAR(255),
	@login VARCHAR(100), 
	@pwd VARCHAR(100),
	@disable BIT
AS
	DECLARE @errorMessage VARCHAR(4000)
	DECLARE @minUser SMALLINT
	SELECT @minUser = COUNT([dp].[_personID]) 
	FROM [_DivisionPerson] [dp] 
	WHERE [dp].[_divisionID] = 0 AND [dp].[_personID] = @personID

	IF @minUser = 0
	BEGIN
		RAISERROR('Данный пользователь не является пользователем системы Минпромторг.', 16, 1)
		RETURN
	END

	BEGIN TRANSACTION

	BEGIN TRY
		EXEC [UpdatePerson] 
			@personID,
			@sname,
			@name,
			@mname,
			@ablative,
			@genitive,
			@email,
			@phone
	END TRY
	BEGIN CATCH
		SELECT @errorMessage = ERROR_MESSAGE()
		RAISERROR(@errorMessage, 16, 1)
		ROLLBACK TRANSACTION
		RETURN
	END CATCH

	BEGIN TRY
		EXEC [UpdateUser]
			@personID,
			@login,
			@pwd,
			@disable
	END TRY
	BEGIN CATCH
		SELECT @errorMessage = ERROR_MESSAGE()
		RAISERROR(@errorMessage, 16, 1)
		ROLLBACK TRANSACTION
		RETURN
	END CATCH

	COMMIT TRANSACTION
GO

/*

exec [UpdateMinPromUser]
	3,
	'Андрей',
	'Адамов',
	'Анатольевич',
	'',
	'',
	'',
	'(495) 531-30-80',
	'asd2',
	'dfwsadgqreadfgq346q34tqertq346twdfgq34',
	0

*/