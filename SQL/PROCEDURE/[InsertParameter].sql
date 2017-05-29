IF OBJECT_ID('[InsertParameter]','P') IS NOT NULL
DROP PROCEDURE [InsertParameter]
GO

CREATE PROCEDURE [InsertParameter]
	--@contractID ObjID,
	@id OBJID,
	@constraint INT,
	@number INT,
	@name NAME,
	--@_stage varchar(3),
	@value varchar(1024),
	@valueReal varchar(1024)
AS
	/*IF @contractID IS NULL
	BEGIN 
		RAISERROR('���� ''������'' ����������� ��� ����������',18,1)
		RETURN
	END*/
	
	IF @number IS NULL
	BEGIN 
		RAISERROR('���� ''�����'' ����������� ��� ����������',18,1)
		RETURN
	END
	
	IF @number < 0 OR @number > 255 
	BEGIN 
		RAISERROR('���� ''�����'' ������ ���� � �������� �� 0 �� 255',18,1)
		RETURN
	END
	
	IF @name IS NULL
	BEGIN 
		RAISERROR('���� ''��������'' ����������� ��� ����������',18,1)
		RETURN
	END
	
	DECLARE @tbl SYSNAME,
			@contractID OBJID,
			@_stage TINY = NULL,
			@msg VARCHAR(128),
			@tblR VARCHAR(255)
			
	SELECT @tbl = [RefTBL], @tblR = [nameR] FROM model.ForeignKeys WHERE [objID] = @constraint
	
	IF @tbl = '_Contract'
		SET @contractID = @id
	ELSE
	BEGIN
		IF @tbl = '_Stage'
			SELECT @contractID = [_contractID], @_stage = [number] FROM [_Stage] WHERE [objID] = @id
	END
	
	IF (SELECT COUNT(*) FROM [_Parameter] WHERE [_contractID] = @contractID AND [stage] = ISNULL(@_stage,0) AND [number] = @number) > 0
	BEGIN 
		SET @msg = '������ ''' + @tblR + ''' ��� �������� ������� ���������� � ����� �������'
		RAISERROR(@msg,18,1)
		RETURN
	END
	
	INSERT INTO [_Parameter]
	([_contractID], [number], [name],[_stage], [value], [valueReal])
	VALUES
	(@contractID, @number, @name, @_stage, @value, @valueReal)
GO