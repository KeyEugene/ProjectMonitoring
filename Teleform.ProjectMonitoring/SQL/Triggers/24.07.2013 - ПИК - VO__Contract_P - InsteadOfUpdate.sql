USE [Minprom]
GO

/****** Object:  Trigger [dbo].[trVO__Contract_P_insteadOfUpdate]    Script Date: 07.08.2013 16:06:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER trigger [dbo].[trVO__Contract_P_insteadOfUpdate] on [dbo].[VO__Contract_P]
instead of update
as
begin
	declare
	@objID int,
	@name varchar(20),
    @number varchar(20),
    @workName varchar(1024),
    @start date,
	@startYear int, 
    @finish date,
    @cost money,
    @_worktypeID tinyint,
    @_programTypeID tinyint,
    @divisionID int,
    @signerID int,
    @executiveID int

	DECLARE @c CURSOR
	SET @c = CURSOR FORWARD_ONLY FOR
		SELECT
			[objID],
			[name],
			[number],
			[workName],
			[start],
			[startYear],
			[finish],
			[cost],
			[_worktypeID],
			[_programTypeID],
			[divisionID],
			[signerID],
			[executiveID]
		FROM [INSERTED]

	OPEN @c
	FETCH NEXT FROM @c INTO
		@objID,
		@name,
		@number,
		@workName,
		@start,
		@startYear,
		@finish,
		@cost,
		@_worktypeID,
		@_programTypeID,
		@divisionID,
		@signerID,
		@executiveID

	BEGIN TRANSACTION

	BEGIN TRY
		WHILE @@FETCH_STATUS = 0
		BEGIN
			PRINT @finish

			UPDATE [_Accomplice] SET
				[_divisionID] = @divisionID,
				[signerID] = @signerID,
				[executiveID] = @executiveID
			where [_contractID] = @objID AND [_accompliceRoleID]=1

			update [_Contract] set
				[name] = @name,
				[number] = @number,
				[WorkName] = @workName,
				[start] = @start,
				[startYear] = @startYear,
				[term] = CAST((@finish - CAST(@start as datetime)) as int),
				[cost] = @cost,
				[_worktypeID]=@_worktypeID,
				[_programTypeID]=@_programTypeID
			where [objID] = @objID

			

			FETCH NEXT FROM @c INTO
				@objID,
				@name,
				@number,
				@workName,
				@start,
				@startYear,
				@finish,
				@cost,
				@_worktypeID,
				@_programTypeID,
				@divisionID,
				@signerID,
				@executiveID
		END
	END TRY

	BEGIN CATCH
		PRINT ERROR_MESSAGE()
		ROLLBACK TRAN
		RETURN
	END CATCH

	COMMIT TRAN

END

GO


