sp_configure 'show advanced options', 1;
GO
RECONFIGURE;
GO
sp_configure 'clr enabled', 1;
GO
RECONFIGURE;
GO
--------------------------------------------------------------------

IF OBJECT_ID('FormatExecutor', 'FS') IS NOT NULL
	DROP FUNCTION [FormatExecutor]
GO

IF EXISTS(SELECT * FROM [SYS].[ASSEMBLIES] [A] WHERE [A].[name] = 'Teleform.SqlServer.Formatting')
	DROP ASSEMBLY [Teleform.SqlServer.Formatting]
GO

-----------------
CREATE ASSEMBLY [Teleform.SqlServer.Formatting]
FROM 'c:\Projects\Pmonitor\libraries\Teleform.SqlServer.Formatting\Teleform.SqlServer.Formatting\bin\Debug\Teleform.SqlServer.Formatting.dll'
WITH permission_set = Safe
GO

CREATE FUNCTION [FormatExecutor](@provider NVARCHAR(256), @format NVARCHAR(32), @value SQL_VARIANT)
RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME [Teleform.SqlServer.Formatting].[Teleform.SqlServer.Formatting.FormatProvider].[FormatExecutor]
go

/*
create FUNCTION [dbo].[FormatExecutor](@provider [nvarchar](256), @format [nvarchar](32), @value [sql_variant])
RETURNS [nvarchar](max) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [Teleform.SqlServer.Formatting].[Teleform.SqlServer.Formatting.FormatProvider].[FormatExecutor]
*/