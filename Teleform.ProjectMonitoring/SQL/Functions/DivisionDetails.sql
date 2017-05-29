CREATE PROCEDURE [dbo].[DivisionDetails]
	@divisionID ObjID
AS
	SELECT 
		D.[objID],
		D.name,
		D.parentID,
		D.nameF,
		D.genitive,
		D.ablative,
		D.place,
		D.INN,
		D.KPP,
		D.comment,
		D.OGRN,
		D.OKPO,
		D.OKUD,
		EPar.uID parentUID
	FROM dbo._Division AS D
	-- parent
	LEFT JOIN Z_Entity EPar ON EPar.typeID = (SELECT ET.[objID] FROM Z_EntityType ET WHERE ET.base='_Division') AND EPar.[objID] = D.parentID
	WHERE D.[objID]=@divisionID
