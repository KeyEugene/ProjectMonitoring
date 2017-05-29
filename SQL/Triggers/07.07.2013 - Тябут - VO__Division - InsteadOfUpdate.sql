IF OBJECT_ID('InsteadOfUpdate', 'TR') IS NOT NULL
	DROP TRIGGER InsteadOfUpdate
GO
CREATE TRIGGER InsteadOfUpdate ON [VO__Division_P]
INSTEAD OF UPDATE
AS
BEGIN
	DECLARE
		@objID int,
		@name varchar(255),
		@nameF varchar(1024),
		@parentID int,
		@genitive varchar(255),
		@ablative varchar(255),
		@place varchar(50),
		@inn varchar(20),
		@kpp varchar(20)

	DECLARE @c CURSOR
	SET @c = CURSOR FORWARD_ONLY FOR
		SELECT
			[objID],
			[name],
			[nameF],
			[parentID],
			[genitive],
			[ablative],
			[place],
			[inn],
			[kpp]
		FROM [INSERTED]


	OPEN @c
	FETCH NEXT FROM @c INTO
		@objID,
		@name,
		@nameF,
		@parentID,
		@genitive,
		@ablative,
		@place,
		@inn,
		@kpp


	DECLARE @tranUpdate varchar(30) = 'UpdateVO__Division_P'
	BEGIN TRANSACTION @tranUpdate

	BEGIN TRY
		WHILE @@FETCH_STATUS = 0
		BEGIN
			UPDATE [_Division] SET
				[name] = @name,
				[nameF] = @nameF,
				[parentID] = @parentID,
				[genitive] = @genitive,
				[ablative] = @ablative,
				[place] = @place,
				[INN] = @inn,
				[KPP] = @kpp
			WHERE [objID] = @objID

			FETCH NEXT FROM @c INTO
				@objID,
				@name,
				@nameF,
				@parentID,
				@genitive,
				@ablative,
				@place,
				@inn,
				@kpp
		END
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN @tranUpdate
		RETURN
	END CATCH

	COMMIT TRAN @tranUpdate
END
GO