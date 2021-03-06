SELECT [P].*
FROM [Search].[Parameter] [P]
	JOIN [Search].[ParameterGroup] [PG] ON [P].[groupID] = [PG].[id]
	JOIN [Search].[Condition] [C] ON [PG].[conditionID] = [C].[id]
	JOIN [Search].[Query] [Q] ON [C].[queryID] = [Q].[id]
WHERE [Q].[name] = 'GetDivisionContracts'