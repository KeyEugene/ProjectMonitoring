IF OBJECT_ID('VO__Person_P', 'V') IS NOT NULL
	DROP VIEW VO__Person_P
GO

CREATE VIEW VO__Person_P 
AS
SELECT        
	P.objID AS ��, 
	P.name AS ���, 
	P.genitive AS ���_��, 
	P.ablative AS ���_��
FROM 
	_Person P
GO

