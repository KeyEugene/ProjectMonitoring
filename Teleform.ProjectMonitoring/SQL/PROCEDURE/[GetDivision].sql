IF NOT OBJECT_ID('[GetDivision]', 'P') IS NULL
	DROP PROCEDURE [GetDivision]
GO
CREATE PROCEDURE [GetDivision]
	@id ObjID
AS
	SELECT 
		[D].[objID],
		[D].[name],
		[D].[parentID],
		[PD].[name] [parentName],
		[D].[nameF] [fullName],
		[D].[genitive],
		[D].[ablative],
		[D].[_placeID] [placeID],
		[P].[name] [place],
		[D].[INN],
		[D].[KPP],
		[D].[comment],
		[D].[OGRN],
		[D].[OKPO],
		[D].[OKUD],
		[D].[address],
		[D].[fax],
		[D].[email],
		[D].[_ownershipID] [ownershipID],
		[O].[name] [ownershipName]
	FROM [_Division] [D] LEFT JOIN [_Division] [PD] ON [PD].[objID] = [D].[parentID]
	LEFT JOIN [_Place] [P] ON [P].[objID] = [D].[_placeID]
	LEFT JOIN [_Ownership] [O] ON [o].[objID] = [D].[_ownershipID]
	WHERE [D].[objID]= @id
GO