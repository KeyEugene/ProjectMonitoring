IF OBJECT_ID('[DeleteEnumAttribute]', 'P') IS NOT NULL
	DROP PROCEDURE [DeleteEnumAttribute]
GO

CREATE PROCEDURE [DeleteEnumAttribute] 
	@table VARCHAR(50),
	@id INT
AS
	DECLARE @query VARCHAR(500) = 'DELETE FROM [' + @table + '] WHERE [objID] = ' + CONVERT(VARCHAR(5), @id)
	BEGIN TRY
		EXEC sp_sqlexec @query
	END TRY
	BEGIN CATCH
		DECLARE @errMessage VARCHAR(MAX) = ERROR_MESSAGE()
		RAISERROR(@errMessage, 16, 1)
		RETURN
	END CATCH
GO