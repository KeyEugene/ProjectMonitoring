IF OBJECT_ID('[ChangeUserPassord]', 'P') IS NOT NULL
DROP PROC [ChangeUserPassword]
GO

CREATE PROCEDURE [ChangeUserPassword]
	@personID OBJID,
	@pwd VARCHAR(128),
	@pwdDupl VARCHAR(128) 
AS
	IF @pwd IS NULL
	BEGIN
		RAISERROR('”кажите новый пароль.',18,1)
		RETURN
	END
	
	IF @pwd <> @pwdDupl
	BEGIN
		RAISERROR('ѕароли не совпадают.',18,1)
		RETURN
	END
	
	IF LEN(@pwd) > 20
	BEGIN
		RAISERROR('ѕароль должен быть не длиннее 20 символов.',18,1)
		RETURN
	END
	
	UPDATE [_User] SET [pwd] = @pwd WHERE [_personID] = @personID
GO
