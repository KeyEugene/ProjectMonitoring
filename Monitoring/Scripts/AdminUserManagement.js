function GetObjOnRow(o) {
    var fillid = o["id"];
    var lastIndex = fillid.lastIndexOf('_');
    var bredId = fillid.substring(0, lastIndex + 1);
    var objID = fillid.substring(lastIndex + 4, fillid.length);

    var currentID = bredId + "ObjIDTB";
    var tb = document.getElementById(currentID);
    tb.value = objID;

    tryRepaintRows(objID);
}
function tryRepaintRows(objID) {

    allTDchangBackgroundColor();

    var name = "td" + objID;
    var arr = document.getElementsByName(name);

    for (var i = 0; i < arr.length; i++) {
        arr[i].style.backgroundColor = "#FFCC00";
    }
}

function allTDchangBackgroundColor() {
    var ar = document.getElementsByClassName("userRow");
    console.log("userRow length " + ar.length);

    for (var i = 0; i < ar.length; i++) {
        ar[i].style.backgroundColor = "white"; //"#BEBEBE"
    }
}
$(document).ready(function () {
    var text = document.getElementsByClassName("textBoxPwdn");

    if (text.length > 0)
        text[0].type = "password";
})

function showPwd(o) {
    var text = document.getElementsByClassName("textBoxPwdn");

    if (text[0].type != "text") {
        text[0].type = "text";
        o.innerHTML = "◙";
    }
    else {
        text[0].type = "password";
        o.innerHTML = "○";
    }
}

function validationDialogTable() {
    var validText = document.getElementById("validationText");
    var pwd = document.getElementsByClassName("textBoxPwdn");
    var login = document.getElementsByClassName("textBoxLoginN");
    var types = document.getElementById("validdl");

    if (types["selected"]) {
        validText.innerHTML = " Поле тип обязательно к заполнению.";
        return false;
    }

    if (login[0]["value"] == "") {
        validText.innerHTML = " Поле логин обязательно к заполнению.";
        return false;
    }

    if (pwd[0]["value"] == "") {
        validText.innerHTML = " Поле пароль обязательно к заполнению.";
        return false;
    }

    return true;
}
