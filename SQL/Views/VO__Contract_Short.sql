USE [Minprom]
GO

/****** Object:  View [dbo].[VO__Contract_Short]    Script Date: 14.08.2013 13:25:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER VIEW [dbo].[VO__Contract_Short]
AS
SELECT 
 C.objID,
 C.name,
 C.number,
 C.startYear,
 C.start,
 CAST(C.start + CAST(term AS datetime) AS date) [finish],
 C.cost,
 WT.name [workType],
 PT.name [programType],
 S.name [status],
 A._divisionID [divisionID],
 D.name [divisionName],
 C.tmpExecutive [tmpExecutive]
 --EDiv.uID divisionUID
FROM _Contract C
-- worktype 
JOIN _WorkType WT ON WT.objID = C._worktypeID
-- programType
JOIN _ProgramType PT ON PT.objID = C._programTypeID
-- accomplice
JOIN _Accomplice A ON A._contractID = C.objID AND A._accompliceRoleID = 1
-- division
JOIN _Division D ON D.objID=A._divisionID
--LEFT JOIN Z_Entity EDiv ON EDiv.typeID = (SELECT ET.objID FROM Z_EntityType ET WHERE ET.base='_Division') AND EDiv.objID = A._divisionID
-- status
LEFT JOIN _Status S ON S.objID = C._statusID


GO

