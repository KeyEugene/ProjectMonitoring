IF NOT OBJECT_ID('[tr_AccompliceInsteadOfInsert]','TR') IS NULL
DROP TRIGGER [tr_AccompliceInsteadOfInsert]
GO

CREATE TRIGGER [dbo].[tr_AccompliceInsteadOfInsert] ON [dbo].[_Accomplice] INSTEAD OF INSERT
AS 
	DECLARE @count INT,
			@oldRole tinyint,
			@winner OBJID,
			@message VARCHAR(512)
			
	SELECT @count = COUNT(*), @oldRole = [A].[_accompliceRoleID]
	FROM [_Accomplice] [A]
	JOIN [INSERTED] [I]
	ON [A].[_contractID] = [I].[_contractID] AND [A].[_divisionID] = [I].[_divisionID]
	GROUP BY [A].[_accompliceRoleID]
	
	IF(@count > 0)
	BEGIN
		SET @message = 'Данная организация уже участвует в проекте,ее роль на данный момент "' + (SELECT [name] FROM [_AccompliceRole] WHERE [objID] = @oldRole) + '"'
		RAISERROR(@message,18,1)
		RETURN
	END
		
	
	IF(@count > 0)
	BEGIN
		SET @message = 'Данная организация уже участвует в проекте,ее роль на данный момент "' + (SELECT [name] FROM [_AccompliceRole] WHERE [objID] = @oldRole) + '"'
		RAISERROR(@message,18,1)
		RETURN
	END
		
		SELECT @count = COUNT(*), @winner = [A].[_divisionID]
		FROM [_Accomplice] [A]
		JOIN INSERTED [I]
		ON [A].[_contractID] = [I].[_contractID] AND [A].[_accompliceRoleID] = 1 and [I].[_accompliceRoleID] = 1
		GROUP BY [A].[_divisionID]
		
		IF(@count > 0)
		BEGIN
			SET @message = 'У данного проекта уже есть победитель - "' + (SELECT [name] FROM [_Division] WHERE [objID] = @winner) + '"'
			RAISERROR(@message,18,1)
			RETURN
		END
	
	INSERT [_Accomplice] 
	([signerID],[executiveID],[signerBase],[_accompliceRoleID],[_divisionID],[_contractID])
	SELECT
	[signerID],[executiveID],[signerBase],[_accompliceRoleID],[_divisionID],[_contractID] FROM INSERTED
GO
