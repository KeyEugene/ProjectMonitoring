IF OBJECT_ID('[UpdateRoute]', 'P') IS NOT NULL
	DROP PROC [UpdateRoute]
GO

CREATE PROC [UpdateRoute]
	@objID ObjID,
	@sent SMALLDATETIME,
	@expected SMALLDATETIME,
	@done SMALLDATETIME
AS
	UPDATE [_Route] SET
		[sent] = @sent,
		[expected] = @expected,
		[done] = @done
	WHERE [objID] = @objID
GO