IF OBJECT_ID('[DeleteTender]','P') IS NOT NULL
	DROP PROC [DeleteTender]
GO

CREATE PROCEDURE [DeleteTender] 
	@objID ObjID
AS
	IF (SELECT COUNT(*) FROM [_TenderContract] WHERE [_tenderID] = @objID) > 0
	BEGIN
		RAISERROR('���� ��������� � ������ �������� ����, ��� ������ �� ����� ���� �������',18,1);
		RETURN
	END
	
	--DELETE FROM [_TenderContract] WHERE [_tenderID] = @objID;
	DELETE FROM [_Tender] WHERE [objID] = @objID;
GO
