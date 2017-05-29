USE [Minprom]
GO
/****** Object:  StoredProcedure [dbo].[SelectStage]    Script Date: 26.08.2013 14:47:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[ApplicationList]
	@contractID ObjID
AS

	SELECT
                      A.[objID],
                      A.[_contractID],
                      A.[_applicationTypeID],
                      T.[name],
                      A.[number],
                      A.[date],
                      A.[based],
                      A.[comment],
                      A.[fileName],
					  S.[name] as StateName,
					  R.[sent],
					  R.[expected],
					  R.[done],
					  D.[name] as DivisionName
                      FROM [_Application] A 
                      JOIN [_ApplicationType] T ON T.[objID]=A.[_applicationTypeID]
                      LEFT JOIN [_Route] R ON R.[_applicationID]=A.[objID]
					  LEFT JOIN [_ApplicationState] S ON S.[objID] = R.[stateID]
                      LEFT JOIN [_Division] D ON D.[objID]=R.[_divisionID]
                      WHERE [_contractID]=@contractID AND (R.[sent] IN 
					  (SELECT MAX([sent]) FROM _Route WHERE _applicationID=R._applicationID)
					  OR (SELECT MAX([sent]) FROM _Route WHERE _applicationID=R._applicationID) is NULL)
RETURN