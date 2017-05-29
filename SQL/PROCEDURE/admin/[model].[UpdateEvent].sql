IF OBJECT_ID('[model].[UpdateEvent]', 'P') IS NOT NULL
	DROP PROCEDURE [model].[UpdateEvent]
GO

CREATE PROCEDURE [model].[UpdateEvent]
	@eventID BIGINT,
	@message VARCHAR(255)=NULL,
	@daysFrom INT=NULL,
	@daysTo INT=NULL
AS

	if isNull(ltrim(@message),'')='' and @daysFrom is NULL and @daysTo is NULL Return
	set @daysFrom=isNull(@daysFrom,0)
	set @daysTo=isNull(@daysTo,0)
	set @message=isNull(ltrim(@message),'')
	
	/*IF @message IS NULL
	BEGIN
		RAISERROR('Сообщение события не может быть пустым.', 16, 1)
		RETURN
	END*/
	
	IF @daysFrom < 0 OR @daysTo < 0
	BEGIN
		RAISERROR('Значения полей "Напомнить за" и "Напомнить после" не могут быть отрицательными.', 16, 1)
		RETURN
	END

declare @tbl varchar(255),@col varchar(255)
select @tbl=tbl,@col=col from model.Event where objID=@eventID
if @tbl is NULL 
	BEGIN
		RAISERROR('Такого события нет', 16, 1)
		RETURN
	END
if @daysFrom<>'' exec model.updateColumnUdp @tbl,@col,@daysFrom,'daysFrom'
exec model.updateColumnUdp @tbl,@col,@daysTo,'daysTo'
exec model.updateColumnUdp @tbl,@col,@message,'Message'