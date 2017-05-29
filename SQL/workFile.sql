declare @t table(val int, txt varchar(20))
insert @t
select 10, 'раз' union all
select 20, 'два' union all
select 30, 'три' union all
select null, 'четыре' union all
select 50, 'пять'
SELECT
	CASE
		WHEN val is not null THEN 'not null'
		ELSE null
	END [val],
	txt
FROM @t t



-- Stage
select
	c.name,
	s.number,
	s.cost,
	s.payment,
	s.daysToEnd,
	s.actDate,
	s.start,
	s.finish,
	s.paymentDelay
from _Contract c
join _Stage s on c.objID = s._contractID and s._contractID = 1

-- Accomplice
select 
	c.name [cname],
	d.name,
	ar.name [arname],
	af.value,
	af.year
from _Accomplice a
join _Contract c on a._contractID = c.objID and c.objID=1
join _AccompliceRole ar on a._accompliceRoleID = ar.objID
join _AccompliceFinancing af on af._contractID = a._contractID and af._divisionID = a._divisionID
join _Division d on d.objID = a._divisionID


select * from Z_Entity

SELECT [d].[objID], [d].[name] FROM [_Division] [d]
JOIN [Z_Entity] [e] ON [e].[objID] = [d].[objID] AND [e].[uID] = 'BB33A2E0-812B-4F4F-97AF-DD2B0D75C3F8'
JOIN [Z_EntityType] [et] ON [et].[objID] = [e].[typeID] AND [et].[base] = '_Division'

select d.objID, d.name, e.uID from Z_Entity e
join _Division d on d.objID = e.objID and e.uID = 'BB33A2E0-812B-4F4F-97AF-DD2B0D75C3F8'
