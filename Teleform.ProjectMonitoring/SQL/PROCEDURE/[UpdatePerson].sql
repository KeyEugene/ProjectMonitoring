IF OBJECT_ID('[UpdatePerson]', 'P') IS NOT NULL
	DROP PROCEDURE [UpdatePerson]
GO

CREATE PROCEDURE [UpdatePerson] 
	@id INT,
	@sname VARCHAR(255),
	@name VARCHAR(255),
	@mname VARCHAR(255),
	@ablative VARCHAR(255),
	@genitive VARCHAR(255),
	@email VARCHAR(255),
	@phone VARCHAR(255)
AS
	IF @sname IS NULL
	BEGIN
		RAISERROR('Необходимо указать фамилию.', 16, 1)
		RETURN
	END
	IF @name IS NULL
	BEGIN
		RAISERROR('Необходимо указать имя.', 16, 1)
		RETURN
	END

	UPDATE [_Person] SET
		[name0] = @sname,
		[name1] = @name,
		[name2] = @mname,
		[ablative] = @ablative,
		[genitive] = @genitive,
		[email] = @email,
		[phone] = @phone
	WHERE [objID] = @id
GO
