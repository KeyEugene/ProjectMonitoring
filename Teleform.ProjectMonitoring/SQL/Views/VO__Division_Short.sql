USE [Minprom]
GO

/****** Object:  View [dbo].[VO__Division_Short]    Script Date: 14.08.2013 16:25:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER VIEW [dbo].[VO__Division_Short]
AS
SELECT 
 D.objID,
 D.name,
 D.place,
 D.INN,
 D.KPP,
 D.parentID [parentID],
 DN.name [parentName]
FROM _Division D
-- parentUID
LEFT JOIN _Division DN ON DN.objID = D.parentID


GO


