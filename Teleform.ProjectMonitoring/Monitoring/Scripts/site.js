function endRequestHandler() {
    //alert("endRequestHandler");
    PredicateReportViewExecutor();
    colResize();
    //manageDetails();
    selectDefaultRow();
    alignReferenceTabaleControl();
    //setDefaultData();
}



function alignReferenceTabaleControl() {

    $('.ListAttributeContainer').width($(document).width());
    //$('.ListAttributeContainer').height($(document).height());
    $('.ListAttributeContainer').offset({ top: 200, left: 100 });

    $('.ListAttributeContent').width($(document).width());
    //$('.ListAttributeContent').height($(document).height()); 
    $('.ListAttributeContent').offset({ top: 200, left: 100 });
    //$('.ListAttributeContent').offset({ top: 200, left: 0 });

    //    var left = ($(document).width() / 2) - ($('.ListAttributeContent').width() / 2);

    //    if ($(document).width() >= 1366) {
    //        $('.ListAttributeContent').css('left', left - 1250);
    //    }
    //    else {
    //        $('.ListAttributeContent').css('left', left);
    //    }

};



function colResize() {
    var onSampleResized = function (e) {
        var columns = $(e.currentTarget).find("th");
        var msg = "";
        columns.each(function () { msg += $(this).attr("HeaderColumnAlias") + "=" + $(this).width() + ";" })

        $("[id*='ResizableTableControlBox']").val(msg);
        var templateID = $("select[id*='TemplateList'] option:selected").val();

        $("[id*='ColResizableTempIDBox']").val(templateID);
    };
    $("table.gridviewStyle").colResizable({ onResize: onSampleResized });
};

function colResizeCard() {
    var onSampleResized = function (e) {
        var columns = $(e.currentTarget).find("th.CardCells");

        var msg = "";

        columns.each(function () { msg += $(this).attr("ColumnName") + "=" + $(this).width() + ";" })

        $("[id*='ResizableCardBox']").val(msg);
    };
    $("#CardTable").colResizable({ onResize: onSampleResized });
};

function colResizeCardContainer() {
    //alert();
    var onSampleResized = function (e) {
        var columns = $(e.currentTarget).find("td[id*='CellContainer']");
        var msg = "";

        var stack = {};

        columns.each(function () {
            stack[$(this).attr("id")] = $(this).width();
        })

        msg = JSON.stringify(stack);

        $("[id*='ResizableCardContainerBox']").val(msg);
    };

    $("#TableContainer").colResizable({ onResize: onSampleResized });
};


function window_resize() {

    alert($("html").width());
    //return $("html").width();
    var page_w = $("html").width();

    //$("[id*='HtmlWidth']").val(page_w);
}

$(document).ready(function () {
    //Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequestHandler) 
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandler);

    //Чтобы не вылетала ошибка "PredicateReportViewExecutor is not defined"
    if (typeof  PredicateReportViewExecutor  === "function") {
        PredicateReportViewExecutor();
    }

    manageDetails();

    //Чтобы не вылетала ошибка "selectDefaultRow is not defined"
    if (typeof selectDefaultRow === "function") {
        selectDefaultRow();
    }

    colResize();


    //setDefaultData();

    var navigationElement = $('#navigationLink');
    var isIndexChanged = $('#IsNavigationListSelectedIndexChanged');
    if ($(isIndexChanged).val() == "true") {
        eval($(navigationElement).attr('href'));
    }
});


function ChangeNavigationHrefID(rowID) {
    var entityID = QueryString["entity"];
    var listNodes = $(".ent" + entityID);

    for (var i = 0; i < listNodes.length; i++) {
        var href = listNodes[i]["href"];

        if (href !== null) {
            var oldID = href.substring(href.indexOf("&id=") + 4, href.indexOf("&level"));
            href = href.replace(("&id=" + oldID + "&"), ("&id=" + rowID + "&"));

            listNodes[i]["href"] = href;
        }
    }

}


function manageDetails() {
    //alert("Запустил managerDeatails");
    $('details').click(function () {
        var index = $('details').index(this);
        var len = $("details").length;
        for (var i = 0; i < len; i++)
            if (i != index) {

                var o = $("details")[i];
                o.removeAttribute("open");
                //o.style.left = "";
            }
    });
};


//актуальна для FilterControl in TableViewControl
function keyup_handlerFilterControl2(sender, itemID) {

    //moveElement(itemID);

    // alert(itemID)
    var c = $("#" + itemID).find("td")
    for (var i = 0; i < c.length; i++) {
        var td = c.eq(i);
        var checkbox = td.find("input")
        var label = td.find("label");

        console.info(label.html());

        var html = label.html();

        if (checkbox.attr("checked") != "checked" && html != undefined && html.toString().toLowerCase().indexOf(sender.value) == -1)
            td.parent().css("display", "none");
        else
            td.parent().css("display", "block");
    }
}
//актуальна
function keyup_handlerFilterControl(o, itemID) {
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




function getRequests() {
    var s1 = location.search.substring(1, location.search.length).split('&'),
        r = {}, s2, i;
    for (i = 0; i < s1.length; i += 1) {
        s2 = s1[i].split('=');
        r[decodeURIComponent(s2[0]).toLowerCase()] = decodeURIComponent(s2[1]);
    }
    return r;
};

var QueryString = getRequests();

function getUrlDynamicCard(rowID, entityID, urlIsRelative) {

    var id = '&id=' + rowID;

    //var entityID = QueryString['entity'];    

    var constraint = QueryString['contraint'] == null ? "" : '&constraint=' + QueryString['constraint'];
    var checker = QueryString['checker'] == null ? "" : '&checker=' + QueryString['checker'];
    var url = '';
    if (urlIsRelative == true) {
        url = '"' + '../Dynamics/XDynamicCard.aspx?entity=' + entityID + constraint + id + checker + '' + '"';
    }
    else if (urlIsRelative == false) {
        url = '"' + 'Dynamics/XDynamicCard.aspx?entity=' + entityID + constraint + id + checker + '' + '"';
    }
    else {
        url = '"' + 'XDynamicCard.aspx?entity=' + entityID + constraint + id + checker + '' + '"';
    }

    return url;
}






function removeAllCheckedAndReturnColor() {
    var array = document.getElementsByClassName("SelectedFieldAlex");
    var name = '';
    for (var i = 0; i < array.length; i++) {
        name = array[i]["id"];

        var el = document.getElementById(name);
        el.style.color = "gray";

        var currentIDlbl = name + "lblAlex";
        var lbl = document.getElementById(currentIDlbl);
        lbl.style.color = "gray";

        var currentIDch = name + "chAlex";
        var checkBox = document.getElementById(currentIDch);
        checkBox["checked"] = false;
    }
};

//Alex selected field for TableBased
function SelectedTheField(o) {
    removeAllCheckedAndReturnColor();

    o.style.color = "red";
    var id = o["id"];

    var currentIDlbl = id + "lblAlex";
    var lbl = document.getElementById(currentIDlbl);
    lbl.style.color = "red";


    var currentIDch = id + "chAlex";
    var ch = document.getElementById(currentIDch);
    ch["checked"] = true;
}



