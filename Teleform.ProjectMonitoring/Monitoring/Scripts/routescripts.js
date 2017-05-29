//фильтрация атрибутов 
function keyup_FilterControl(o, itemID) {
    var c = $("#" + itemID).find("option");
    for (var i = 0; i < c.length; i++) {
        var option = c.eq(i);
        var label = option.valueOf("label");

        var html = label.html();

        if (html.toString().toLowerCase().indexOf(o.value.toLowerCase()) == -1)
            option.css("display", "none");
        else
            option.css("display", "block");
    }
}

//фильтрация документов в таблице маршрута
function keyup_RouteTableFilter(o, itemID) {
    var c = $("#" + itemID).find("tr");
    for (var i = 1; i < c.length; i++) {
        var row = c.eq(i);
        var tds = row.find("td.headerCell");
        var td = tds.eq(0);
        var tdtext = td.valueOf("innerText").html();
        if (tdtext.toString().toLowerCase().indexOf(o.value.toLowerCase()) == -1)
            row.css("display", "none");
        else
            row.css("display", "block");
    }

}

//drag'n'drop события
$(function () {

    $('.imgDoc').draggable({
        axis: "x",
        containment: ".tblRoute"
    });

    $('.docCell').droppable({
        drop: function () {
            var s = document.getElementById(this.id);
            var index = s.cellIndex;
            var par = s.parentNode;
            if (s.bgColor = 'limegreen') {
                var nextCell = par.cells[index + 1];
                if (nextCell.bgColor != 'white' && nextCell.bgColor != '') {
                    nextCell.childNodes[0].defaultValue = 'tomato';
                    nextCell.bgColor = 'tomato';
                }
            }

            var l = this.id + '_lbl';
            var m = document.getElementById(l);
            m.defaultValue = 'yellow';
            s.bgColor = 'yellow';

        },
        out: function () {
            var s = document.getElementById(this.id);
            var l = this.id + '_lbl';
            var m = document.getElementById(l);
            m.defaultValue = 'limegreen';
            s.bgColor = 'limegreen';
        }
    });

    $('.headerCell').droppable({
        drop: function () {
            var index = this.cellIndex;
            var par = this.parentNode;
            var nextCell = par.cells[index + 1];
            if (nextCell.bgColor = 'limegreen') {
                nextCell.bgColor = 'tomato';
                nextCell.childNodes[0].defaultValue = 'tomato';
            }
        }
    });

});

//выделить выбранную строку
function selectThisRow(o) {
    clearSelection(o);
    var row = o.parentNode;
    var cell = row.cells[0];
    row.style.color = 'red';
    var cb = cell.children[0].children[0];
    cb.checked = true;
}

//очистить выделение
function clearSelection(o) {
    var table = o.parentNode.parentNode.parentNode;
    for (var i = 1; i < table.rows.length; i++) {
        var row = table.rows[i];
        row.style.color = '';
        var cb = row.cells[0].children[0].children[0];
        cb.checked = false;
    }
}

$(document).ready(function () {
    var datebox = document.getElementById('MainContent_DateBox');
    var daterbox = document.getElementById('MainContent_DateRBox');
    datebox.value = datebox.defaultValue;
    daterbox.value = daterbox.defaultValue;
});