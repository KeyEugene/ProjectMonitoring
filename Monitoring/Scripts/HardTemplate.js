function afterpostback() {
    afterPost = 1;
}
//Visible/hidde wait.gif
function ShowIgame() {
    document.getElementById("<%=MainImage.ClientID%>").style.visibility = "hidden";
    document.getElementById("<%=Image1.ClientID%>").style.visibility = "visible";
}

//Сортировка controls Designer в коснтрукторе
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
//Alex Selected for HardTemplate
function SelectedFieldHardTemplate(o) {
    removeAllColorHardTemplate();

    if (typeof (o) == "string") {
        var obj = o;
        var t = document.getElementsByName(obj);
        o = t[0];
    } else
        o.style.color = "red";

    var id = o["id"] + "hardCheck";
    var checkbox = document.getElementById(id);
    checkbox["checked"] = true;
}
function removeAllColorHardTemplate() {
    var nameClass = document.getElementsByClassName("HardTemplateAlex");

    for (var i = 0; i < nameClass.length; i++) {
        nameClass[i].style.color = "black";

        var id = nameClass[i]["id"] + "hardCheck";
        var checkbox = document.getElementById(id);
        checkbox["checked"] = false;
    }
}
//Alex 


//Сортировка в конструкторе Up
function clickUp(o, tbID, fieldID) {
    var bredID = o["id"].substr(0, o["id"].lastIndexOf("_"));
    var currentTbID = bredID + "_" + tbID;

    var tb = document.getElementById(currentTbID);
    var imgDown = document.getElementById(bredID + "_arrowDown" + fieldID);

    if (tb.value == "" || tb.value == "ASC") {
        tb.value = "DESC";
        o.style.opacity = "1";
        imgDown.style.opacity = ".3";
    } else if (tb.value == "DESC") {
        tb.value = "";
        o.style.opacity = ".3";
    }

}
//Сортировка в конструкторе Down
function clickDown(o, tbID, fieldID) {
    var bredID = o["id"].substr(0, o["id"].lastIndexOf("_"));
    var currentTbID = bredID + "_" + tbID;

    var tb = document.getElementById(currentTbID);
    var imgUp = document.getElementById(bredID + "_arrowUp" + fieldID);

    if (tb.value == "" || tb.value == "DESC") {
        tb.value = "ASC";
        o.style.opacity = "1";
        imgUp.style.opacity = ".3";
    } else if (tb.value == "ASC") {
        tb.value = "";
        o.style.opacity = ".3";
    }
}

//Наведение курсора мыши на TreeCell
function onmouseoverTreeCell(o) {
    var btn = document.getElementsByName(o);
    btn[0].style.transition = "all .2s ease-out 0.1s";
    btn[0].style.opacity = "1";
}

// курсор мыши выходит за пределы элемента  TreeCell
function onmouseoutTreeCell(o) {
    var btn = document.getElementsByName(o);
    btn[0].style.transition = "all 1s ease-out 0.1s";
    btn[0].style.opacity = "0";
}