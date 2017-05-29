IF OBJECT_ID('[UpdateEmployee]','P') IS NOT NULL
	DROP PROC [UpdateEmployee]
GO

CREATE PROCEDURE [UpdateEmployee]
	@id ObjID,
	@position ObjID,
	@head Bit,
	@email VARCHAR(50),
	@phone VARCHAR(50),
	@signerBase VARCHAR(1024)
AS
	/*IF @position IS NULL
	BEGIN 
		RAISERROR('Не указана должность.', 16, 1) 
		RETURN 
	END*/
	
	UPDATE [_DivisionPerson] 
	SET 
		[_personRoleID] = @position,
		[head] = @head,
		[email] = @email,
		[phone] = @phone,
		[signerBase] = @signerBase
	WHERE [objID] = @id
GO
