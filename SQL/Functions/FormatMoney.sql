IF OBJECT_ID('FormatMoney', 'FN') IS NOT NULL
	DROP FUNCTION FormatMoney
GO
CREATE FUNCTION FormatMoney(@amount MONEY, @format BIT)
RETURNS MONEY
AS
BEGIN
	IF @format = 0 RETURN @amount
	
	RETURN @amount / 1000;
END
GO