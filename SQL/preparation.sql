--
SELECT * FROM [Model].[UIAttribute]('_Contract') [T]
WHERE [T].[enum] = 0

--SELECT * FROM [Model].[TableKey]('_Contract')

SELECT
	[T].[attribute] '@x',
	CASE [T].[base]
		WHEN 1 THEN 'true'
		ELSE 'false'
	END '@base',
	[T].[synonym] '@synonym',
	(
		CASE [T].[enum]
			WHEN 1 THEN
			(
				SELECT [WT].[name] '@x'
				FROM [_WorkType] [WT]
				FOR XML PATH('xyz'), TYPE
			) 
		END
	) [enumeration]
FROM [Model].[UIAttribute]('_Contract') [T]
FOR XML PATH('attribute'), TYPE