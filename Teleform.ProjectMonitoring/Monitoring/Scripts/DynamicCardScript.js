$(document).ready(function alignReferenceTabaleControl() {

    $('.ListAttributeContainer').width($(document).width());
    $('.ListAttributeContainer').height($(document).height());

    $('.ListAttributeContent').offset({ top: 200, left: 0 });

    var headerRow = $('#ListView tr:first').detach();
    $('#ListView').prepend('<thead></thead>');
    $('#ListView thead').prepend(headerRow);
    var left = ($(document).width() / 2) - ($('.ListAttributeContent').width() / 2);

//    if ($(document).width() >= 1366) {
//        $('.ListAttributeContent').css('left', left - 1250);
//    }
//    else {
//        $('.ListAttributeContent').css('left', left);
//    }


    var card = $('#Card');
    if (card != null && card != undefined) {
        var frame = $('.InfoContainer iframe:first');

        if (frame != null && frame != undefined && frame.outerWidth() != null) {
            frame.width($('.DynamicCardCommonContainer').outerWidth() - (card.outerWidth() + 10));
        }
        else {
            card.toggleClass("left");
        }
    }

});

function CalculateDate() {
    console.info($("#SelectedDate").val())
    var date = new Date($("#SelectedDate").val())
    var resultDate = new Date($("#SelectedDate").val());
    var number = parseInt($("#DayNumber").val());

    resultDate.setDate(date.getDate() + number);

    document.getElementById('resultDate').valueAsDate = resultDate;
}