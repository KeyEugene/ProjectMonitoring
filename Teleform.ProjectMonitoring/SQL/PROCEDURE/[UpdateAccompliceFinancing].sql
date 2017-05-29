IF OBJECT_ID('[UpdateAccompliceFinancing]', 'P') IS NOT NULL
DROP PROC [UpdateAccompliceFinancing]
GO

CREATE PROCEDURE [UpdateAccompliceFinancing]
	@id  OBJID,
	@value VARCHAR(128),
	@valueReal VARCHAR(128),
	@sourceID OBJID
AS
		
IF @value IS NULL
	BEGIN
		RAISERROR ('������ ���� ������� �����.',18,1)
		RETURN
	END
	
UPDATE [_AccompliceFinancing]
SET
	[value]= CONVERT(MONEY,@value),
	[valueReal] = CONVERT(MONEY, @valueReal),
	[sourceID] = @sourceID
WHERE objID = @id
