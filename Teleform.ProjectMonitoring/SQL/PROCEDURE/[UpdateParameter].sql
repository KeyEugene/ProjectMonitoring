IF OBJECT_ID('[UpdateParameter]','P') IS NOT NULL
DROP PROCEDURE [UpdateParameter]
GO

CREATE PROCEDURE [UpdateParameter]
	@id ObjID,
	@name NAME,
	@value varchar(1024),
	@valueReal varchar(1024)
	--@_stage INT
AS
	IF @name IS NULL
	BEGIN
		RAISERROR('Поле ''Название'' необходимо для заполнения.',18,1)
		RETURN
	END
	
	UPDATE [_Parameter] SET
			[value] = @value,
			[valueReal] = @valueReal,
			[name] = @name
			--[_stage] = @_stage
	WHERE [objID] = @id
GO