function ListAttributeSearchTextChanged(elem) {
    var dInput = $(elem).val().toLowerCase();

    if (dInput == '') {
        ShowSelection(null);
        return;
    }
    
    $('#ListView tr[data-searched = true]').each(function () {
        $(this).attr('data-searched', 'false');
    });


    $('#ListView tr[data-row-content = false]').each(function () {
        $(this).attr('data-searched', 'true');
    });

    var collection = $('#ListView tr[data-row-content = true]').filter(function () {
        var o = $(this).find('td:not([style *= "display:none"])').html();
        return o != undefined && o != null && o.toLowerCase().indexOf(dInput) > -1
    });

    collection.each(function () {
        $(this).attr('data-searched', 'true');
    });

    ShowSelection(collection);
}


function ShowSelection(collection) {

    $('#ListView tr[data-row-content = true').css('display', 'table-row');

    if (collection == null)
        return;

    $('#ListView tr[data-searched != true]').css('display', 'none');
}