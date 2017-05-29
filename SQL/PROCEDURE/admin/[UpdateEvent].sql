IF OBJECT_ID('[UpdateEvent]', 'P') IS NOT NULL
	DROP PROCEDURE [UpdateEvent]
GO

CREATE PROCEDURE [UpdateEvent]
	@eventID INT,
	@name VARCHAR(255),
	@message VARCHAR(255),
	@daysFrom INT,
	@daysTo INT
AS

	IF @message IS NULL OR @message = ''
	BEGIN
		RAISERROR('��������� ������� �� ����� ���� ������.', 16, 1)
		RETURN
	END

	/*IF @daysFrom IS NULL OR @daysFrom = 0
	BEGIN
		RAISERROR('���� "��������� ��" �� ����� ���� ������ ��� ������ ����.', 16, 1)
		RETURN
	END

	IF @daysTo IS NULL OR @daysTo = 0
	BEGIN
		RAISERROR('���� "��������� �����" �� ����� ���� ������ ��� ������ ����.', 16, 1)
		RETURN
	END*/


	IF @daysFrom < 0 OR @daysTo < 0
	BEGIN
		RAISERROR('�������� ����� "��������� ��" � "��������� �����" �� ����� ���� ��������������.', 16, 1)
		RETURN
	END


	UPDATE [infrastructure].[Event] SET 
		[name] = @name,
		[message] = @message,
		[daysFrom] = @daysFrom,
		[daysTo] = @daysTo
	WHERE [objID] = @eventID
GO