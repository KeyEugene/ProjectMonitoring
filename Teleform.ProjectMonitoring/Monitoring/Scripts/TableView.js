$(document).ready(function () {
});

function selectDefaultRow() {
    if ($("[id*='SelectedRowIDBox']").val() == "") {

        var rows = $('tr.AlternativeRow');
        
        var arrRowsID = [];
    
        rows.each(function () { arrRowsID[arrRowsID.length] = $(this).attr('id') })

        var defaultRowID = arrRowsID[0];
                
        var defaultRow = $('tr[id=' + defaultRowID + ']');

        defaultRow.attr('class', 'SelectedRow');

        $("[id*=SelectedRowIDBox]").val(defaultRowID);
    }
    else {
        var selectedRow = $('tr[id=' + $("[id*=SelectedRowIDBox]").val() + ']');
        selectedRow.attr('class', 'SelectedRow');
    }


    //при всех запросах к серверу выделяется верхняя строка
    //    var rows = $('[id*= CRZ] tr.AlternativeRow, tr.SelectedRow');

    //    var arrRowsID = [];
    //    rows.each(function () { arrRowsID[arrRowsID.length] = $(this).attr('id') })

    //    var defaultRowID = arrRowsID[0];

    //    var defaultRow = $('[id*= CRZ] tr[id=' + defaultRowID + ']');

    //    defaultRow.attr('class', 'SelectedRow');

    //    getTableCellControlsData();

    //    $("[id*=SelectedRowIDBox]").val(defaultRowID);
}

function informUser() {
    alert("You must save the object");
    //alert("Что бы открыть карточку, объект надо сохранить");
}

function switchSelectedRow(rowID, entityID, urlIsRelative) {

    var row = $('tr.SelectedRow')
    row.attr("class", "AlternativeRow");
    row.removeAttr("ondblclick");
    row = $('tr.AlternativeRow[id=' + rowID + ']');
    row.attr('class', 'SelectedRow');

    var url = getUrlDynamicCard(rowID, entityID, urlIsRelative);

    if (rowID != -1) {
        row.attr('ondblclick', 'javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions("", "", false, "", ' + url + ', false, true))');
    }
    else {
        row.attr('ondblclick', 'informUser()');        
    }

    $("[id*=SelectedRowIDBox]").val(rowID);
    
    var isEditMode = $("[id*=IsEditModeCheckBox]").is(':checked');
    if (isEditMode == true) {
        getTableCellControlsData();
    }     

    ChangeNavigationHrefID(rowID);
}

var InstanceCollection = [];

var Instances = { Instances: InstanceCollection };

function addEntityInstance(fieldsValue) {

    var id = $("[id*=SelectedRowIDBox]").val();

    var Instance = { id: id, data: fieldsValue };
  

    if (InstanceCollection.length == 0) {
        InstanceCollection[0] = Instance;
    }
    else {
        var isFound = false;
        for (var i = 0; i <InstanceCollection.length; i++) {
        
            if (InstanceCollection[i].id == Instance.id) {
                isFound = true;
                InstanceCollection[i] = Instance;
            }
        }

        if (!isFound) {
            InstanceCollection[InstanceCollection.length] = Instance;
        }
    }


    var s = JSON.stringify(Instances);

    $("[id*=SaveObjectsJeysonBox]").val(s.replace(/null,/g, ""));
}


function getTableCellControlsData() {

    var row = $('tr.SelectedRow');
    var columnsValue = "";

    var InputDateList = row.find('input[type = date]');
    InputDateList.each(function () {
        columnsValue += $(this).attr("attributeName") + "=" + $(this).val() + ";"
    });

    var TextAreaList = row.find('textarea');
    TextAreaList.each(function () {
        columnsValue += $(this).attr("attributeName") + "=" + $(this).val() + ";"
    });

    var InputCheckBoxList = row.find('input[type = checkbox]');
    InputCheckBoxList.each(function () {
        columnsValue += $(this).attr("attributeName") + "=" + $(this).is(':checked') + ";"
    });

    var InputNumberList = row.find('input[type = number]');
    InputNumberList.each(function () {
        columnsValue += $(this).attr("attributeName") + "=" + $(this).val() + ";"
    });
    if (columnsValue != "") {
        addEntityInstance(columnsValue);
    }

}



