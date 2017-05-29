IF OBJECT_ID('trVO__Person_PInsteadUpdate', 'TR') IS NOT NULL
	DROP TRIGGER trVO__Person_PInsteadUpdate
GO

CREATE TRIGGER trVO__Person_PInsteadUpdate ON [VO__Person_P] 
INSTEAD OF UPDATE
AS
BEGIN
	DECLARE
		@objID int,
		@name varchar(200),
		@genitive varchar(1000),
		@ablative varchar(1000),
		@roleID int,
		@divisionID int

	DECLARE @c CURSOR 
	SET @c = CURSOR FORWARD_ONLY FOR
		SELECT 
			[ИД],
			[ФИО],
			[ФИО_рп],
			[ФИО_тп],
			[Должность],
			[Организация]
		FROM [INSERTED]

	OPEN @c
	FETCH NEXT FROM @c INTO
		@objID,
		@name,
		@genitive,
		@ablative,
		@roleID,
		@divisionID

	DECLARE @tranUpdate varchar(30) = 'UpdateVO__Person_P'
	BEGIN TRANSACTION @tranUpdate
		
	BEGIN TRY
		WHILE @@FETCH_STATUS = 0
		BEGIN
			UPDATE [_Person] SET
				[name] = @name,
				[genitive] = @genitive,
				[ablative] = @ablative
			WHERE [objID] = @objID
			UPDATE [_DivisionPerson] SET
				[_divisionID] = @divisionID,
				[_personRoleID] = @roleID
			WHERE [_personID] = @objID

			FETCH NEXT FROM @c INTO
				@objID,
				@name,
				@genitive,
				@ablative,
				@roleID,
				@divisionID
		END
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN @tranUpdate
		RETURN
	END CATCH
	COMMIT TRAN @tranUpdate
END

GO


