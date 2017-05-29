IF OBJECT_ID('[InsertAccompliceFinancing]', 'P') IS NOT NULL
	DROP PROC [InsertAccompliceFinancing]
GO
	
CREATE PROCEDURE [dbo].[InsertAccompliceFinancing]
		@id OBJID,
		@typeID TINY,
		@year SMALLINT,
		@value MONEY,
		@valueReal MONEY,
		@sourceID OBJID
AS 
	IF (SELECT COUNT(*)
		FROM [_AccompliceFinancing] [AF]
		JOIN [_Accomplice] [A] ON [A].[_divisionID] = [AF].[_divisionID] AND [A].[_contractID] = [AF].[_contractID]
		WHERE [A].[objID] = @id AND [AF].[typeID] = @typeID AND [AF].[year] = @year) >0
	BEGIN
		RAISERROR('� ������� ��������� ��� ���� �������������� � ������ ����� � �����.',18,1)
		RETURN
	END
	
IF @typeID IS NULL
	BEGIN
		RAISERROR('���� "��� ��������������", ����������� ��� ����������.', 18,1)
		RETURN
	END
	
IF @year < 1990 AND @year > 2050
	BEGIN
		RAISERROR('��� ������ ���� ������ 1990 � ������ 2050.', 18,1)
		RETURN
	END

IF @year IS NULL
	BEGIN	
		RAISERROR('������ ���� ������ ���.', 18, 1)
		RETURN
	END
	
IF @value IS NULL
	BEGIN
		RAISERROR ('������ ���� ������� �����.',18,1)
		RETURN
	END
	
	INSERT INTO [_AccompliceFinancing]
	([typeID],[year],[value],[valueReal],[sourceID],[_divisionID],[_contractID])
	SELECT 
		@typeID,
		@year, 
		@value, 
		@valueReal,
		@sourceID,
		[A].[_divisionID],
		[A].[_contractID]
	FROM [_Accomplice] [A]
	WHERE [objID] = @id
GO


