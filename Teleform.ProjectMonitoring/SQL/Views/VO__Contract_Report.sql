USE [MinProm]
GO

/****** Object:  View [dbo].[VO__Contract_Report]    Script Date: 25.07.2013 18:54:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER VIEW [dbo].[VO__Contract_Report]
AS
SELECT 
 C.objID,
 C.name,
 C.number,
 C.WorkName,
 C.startYear,
 C.start,
 CAST(C.start + CAST(term AS datetime) AS date) [finish],
 C.cost,
 WT.name WorkType,
 PT.name ProgramType,
 S.name Status,
 D.name division,
 P.name signer,
 C.tmpExecutive
FROM _Contract C
JOIN _WorkType WT ON WT.[objID] = C._worktypeID
JOIN _ProgramType PT ON PT.[objID] = C._programTypeID
JOIN _Accomplice A ON A._contractID = C.objID AND A._accompliceRoleID = 1
JOIN _Division D ON D.[objID] = A._divisionID
JOIN _Person P ON P.[objID] = A.signerID
LEFT JOIN _Status S ON S.[objID] = C._statusID


GO


