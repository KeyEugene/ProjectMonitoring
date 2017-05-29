IF NOT OBJECT_ID('[UpdateAccomplice]','P') IS NULL
DROP PROCEDURE [UpdateAccomplice]
GO

CREATE PROCEDURE [dbo].[UpdateAccomplice] 
	@id ObjID,
    --@contractID ObjID,
	@signer ObjID,
	@executive ObjID,
	@roleID tinyint,
    @reason varchar(1024)
AS

	IF @roleID = 1
	BEGIN
		DECLARE @contractID objID = (SELECT [_contractID] FROM [_Accomplice] WHERE [objID] = @id)
		IF(SELECT COUNT(*) 
			FROM [_Accomplice] [A]
			WHERE [_contractID] = @contractID AND [_accompliceRoleID] = 1) > 0
		BEGIN
			RAISERROR('” данного проекта уже есть победитель',18,1)
			RETURN
		END
		
		
	END
	
	UPDATE [_Accomplice] SET 
                   [signerID]=@signer,
                   [executiveID]=@executive, 
                   [_accompliceRoleID]=@roleID,
                   [signerBase]=@reason
    WHERE [objID]=@id


GO


