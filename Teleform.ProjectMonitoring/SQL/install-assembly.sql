IF OBJECT_ID('FormatExecutor', 'FS') IS NOT NULL
	DROP FUNCTION [FormatExecutor]
GO

IF EXISTS(SELECT * FROM [SYS].[ASSEMBLIES] [A] WHERE [A].[name] = 'Teleform.SqlServer.Formatting')
	DROP ASSEMBLY [Teleform.SqlServer.Formatting]
GO

CREATE ASSEMBLY [Teleform.SqlServer.Formatting]
FROM 'C:\Projects\libraries\Teleform.SqlServer.Formatting.dll'
WITH permission_set = Safe
GO

CREATE FUNCTION [FormatExecutor](@provider NVARCHAR(1024), @format NVARCHAR(32), @value SQL_VARIANT)
RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME [Teleform.SqlServer.Formatting].[Teleform.SqlServer.Formatting.FormatProvider].[FormatExecutor]
GO