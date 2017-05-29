SELECT
	[T].[name],
	[T].[body],
	[T].[typeID]
FROM [R_Template] [T] JOIN [R_TemplateType] [TT] ON [T].[typeID] = [TT].[objID] 
WHERE ([T].[body] IS NULL AND [TT].[useBody] = 1) OR ([T].[body] IS NOT NULL AND [TT].[useBody] = 0)