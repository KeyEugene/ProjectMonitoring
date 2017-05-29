IF OBJECT_ID('[DeleteEntityItem]','P') IS NOT NULL
DROP PROCEDURE [DeleteEntityItem]
GO

CREATE PROCEDURE [DeleteEntityItem]
	@objID ObjiD,
	@tblID Int 
AS
	DECLARE @tableName NVARCHAR(128),
			@conTbl VARCHAR(255),
			@c CURSOR
	
	SET @tableName = (SELECT [tbl] FROM model.Entity('base') WHERE [tblID] = @tblID)
	
	SET @c = CURSOR LOCAL READ_ONLY FORWARD_ONLY FOR 
				SELECT [attr] FROM model.BObjectMap(@tableName) WHERE UType = 'Table'
	
	OPEN @c
	
	FETCH NEXT FROM @c INTO @conTbl
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SELECT [pCol],[rCol] from model.ForeignKeysColumns WHERE refTbl=@tableName AND [parentTbl]=@conTbl
		FETCH NEXT FROM @c INTO @conTbl
	END
GO
