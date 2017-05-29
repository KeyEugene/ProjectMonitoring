USE [MinProm]
GO

/****** Object:  View [dbo].[VO__Division_P]    Script Date: 23.07.2013 13:14:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[VO__Division_P]
AS
SELECT 
	D.objID,
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
LEFT JOIN Z_Entity EPar ON EPar.typeID = (SELECT ET.objID FROM Z_EntityType ET WHERE ET.base='_Division') AND EPar.objID = D.parentID
GO


