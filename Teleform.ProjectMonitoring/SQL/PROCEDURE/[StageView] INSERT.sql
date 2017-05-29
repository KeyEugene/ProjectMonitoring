IF NOT OBJECT_ID('RDocInsertAction', 'TR') IS NULL
	DROP TRIGGER RDocInsertAction
GO
CREATE TRIGGER RDocInsertAction ON [RDoc]
INSTEAD OF INSERT
AS
BEGIN
	DECLARE @parentID ObjID,
			@name name,
			@typeID ObjID,
			@extension code,
			@created DATETIME,
			@modified DATETIME,
			@comment comment,
			@icon link,
			@authorID ObjID,			
			@private flag,
			@parent2ID ObjID
			
	SELECT
		@parentID = [parentID],
		@name = [name],
		@typeID = [typeID],
		@extension = [extension],
		@created = [created],
		@modified = [modified],
		@comment = [comment],
		@icon = [icon],
		@authorID = [authorID],
		@private = [private],
		@parent2ID = [parent2ID]
	FROM INSERTED
	
	IF @parentID = 0 SET @parentID = NULL
	
	INSERT INTO [Doc] (
		[Doc].[parentID],
		[Doc].[name],
		[Doc].[typeID],
		[Doc].[extension],
		[Doc].[created],
		[Doc].[modified],
		[Doc].[comment],
		[Doc].[icon],
		[Doc].[authorID],
		[Doc].[private],
		[Doc].[parent2ID]
	)
	VALUES (
		@parentID,
		@name,
		@typeID,
		@extension,
		@created,
		@modified,
		@comment,
		@icon,
		@authorID,
		@private,
		@parent2ID
	)
END
GO