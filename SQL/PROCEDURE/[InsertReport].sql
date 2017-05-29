IF OBJECT_ID('[InsertReport]', 'P') IS NOT NULL
DROP PROCEDURE [InsertReport]
GO

CREATE PROCEDURE [InsertReport]
	@templateID OBJID,
	@created SMALLDATETIME,
	@userID OBJID,
	@link STRING,
	@name STRING
AS
	DECLARE @typeID INT,
			@typeName NAME
			--@name NAME
			
	SELECT @typeID = [RT].[typeID],
			@typeName = [RTT].[name]
	FROM [R_Template] [RT]
	JOIN [R_TemplateType] [RTT] ON [RTT].[objID] = [RT].[typeID]
	WHERE [RT].[objID] = @templateID
	
	IF @name = '' OR @name IS NULL
	BEGIN
		IF @typeName = 'Word' SET @name = 'מעקוע_' + CONVERT(VARCHAR(64), @created)
		ELSE IF @typeName = 'Excel' SET @name = (SELECT [name] FROM [R_Template] WHERE [objID] = @templateID)
	END
	
	INSERT INTO [R_Report] 
	([typeID], [created], [name], [userID], [link])
	VALUES
	(@typeID, @created, @name, @userID, @link)
GO