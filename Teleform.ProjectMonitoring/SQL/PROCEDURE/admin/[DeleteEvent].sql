IF OBJECT_ID('[DeleteEvent]', 'P') IS NOT NULL
	DROP PROCEDURE [DeleteEvent]
GO

CREATE PROCEDURE [dbo].[DeleteEvent] @eventID INT
AS
	UPDATE [infrastructure].[Event] SET
		[message] = NULL,
		[daysFrom] = NULL,
		[daysTo] = NUll
	WHERE [objID] = @eventID

GO


