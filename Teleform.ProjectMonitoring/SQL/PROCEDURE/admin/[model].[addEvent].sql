IF OBJECT_ID('[model].[addEvent]', 'P') IS NOT NULL
	DROP PROCEDURE [model].[addEvent]
GO

CREATE PROCEDURE [model].[addEvent]
	@eventID BIGINT,
	@message VARCHAR(255),
	@daysFrom INT=0,
	@daysTo INT=0
AS

	if isNull(ltrim(@message),'')=''
	BEGIN
		RAISERROR('Сообщение события не может быть пустым.', 16, 1)
		RETURN
	END

	set @daysFrom=isNull(@daysFrom,0)
	set @daysTo=isNull(@daysTo,0)

	IF @daysFrom < 0 OR @daysTo < 0
	BEGIN
		RAISERROR('Значения полей "Напомнить за" и "Напомнить после" не могут быть отрицательными.', 16, 1)
		RETURN
	END

declare @tbl varchar(255),@col varchar(255)
select @tbl=tbl,@col=col from model.EventCandidate where eventID=@eventID
if @tbl is NULL 
	BEGIN
		RAISERROR('Такого кандидата нет', 16, 1)
		RETURN
	END
exec model.addColumnsUdp @tbl,@col,@daysFrom,'daysFrom'
exec model.addColumnsUdp @tbl,@col,@daysTo,'daysTo'
exec model.addColumnsUdp @tbl,@col,@message,'Message'
--