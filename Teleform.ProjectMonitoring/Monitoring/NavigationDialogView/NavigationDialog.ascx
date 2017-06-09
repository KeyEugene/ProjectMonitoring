<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationDialog.ascx.cs" Inherits="Teleform.ProjectMonitoring.NavigationDialog" %>

<style>
    .treeview a {
        color: #999;
        padding: 7px;
    }

        .treeview a:hover {
            color: #fff;
            background-color: #000;
            text-decoration: none;
        }

    .item_selected {
        color: white !important;
    }
</style>
<asp:TreeView runat="server" ID="treeView" ViewStateMode="Disabled" EnableViewState="false" CssClass="treeview">
</asp:TreeView>
<div class="text-center" style="margin-top: 25px;">
    <asp:CheckBox ID="ShowAllNavigation" Text="Показать все" runat="server" AutoPostBack="true" />
</div>

<%--<div class="menuByObject">

</div>--%>

<script>


    //$(document).ready(function () {
    //    var navigation_object = $("[id*=NavigationObjects]");
    //    var navigation_object_container = $("#object_treeview_conteiner");
    //    navigation_object.appendTo(navigation_object_container);
    //});
</script>

<style>
    /*#IndexNodeTextBox {
        margin: 2px 1px 0px 50px;
        width: 50px;
    }*/
</style>

<script type="text/javascript">
    /* Navigation dialog */
    //function moveNavigationDialog(o) {
    //    var divPath = document.getElementsByClassName("navigationTreeDialog");

    //    var isPath = o["id"] !== "MoveButtonNavigationPath";
    //    if (isPath)
    //        divPath = document.getElementsByClassName("NavigationObjects");  //Move NavigationObjects
    //    o = $(".movedialog")[0];
    //    if (o.innerText === ">>>") {
    //        o.innerText = "<<<";
    //        divPath[0].style.transition = "all 0.4s ease-in-out 0.1s";
    //        if (!isPath) {
    //            divPath[0].style.left = "0px";
    //            divPath[0].style.height = '470px';
    //        } else {
    //            divPath[0].style.left = "35%";//"623px";
    //            divPath[0].style.height = '550px';
    //        }
    //        divPath[0].style["opacity"] = "1";
    //    } else {
    //        o.innerText = ">>>";
    //        divPath[0].style.transition = "all 0.4s ease-in-out 0.1s";
    //        if (!isPath) {
    //            divPath[0].style.left = "-470px";
    //            divPath[0].style.height = '150px';
    //        } else {
    //            divPath[0].style.left = "97.8%";
    //            divPath[0].style.height = '150px';
    //        }
    //        divPath[0].style["opacity"] = "1"; //".1";.05
    //    }
    //}

    //function onMouseOverNavigDialog(o) {
    //    o.style.opacity = "1";
    //}

    //function onMouseOutNavigDialog(o) {
    //    o.style["opacity"] = "1"; //".1"; 05 --- разкоментить
    //}

    //function showHideSettings(o) {
    //    var div = document.getElementsByClassName("SettingsNavigation");

    //    if (!o["draggable"]) {
    //        document.getElementById(div[0]["id"]).classList.add("NewTestCssClass");
    //        document.getElementById(div[0]["id"]).classList.remove("SettingsNavigation");

    //        o.style.color = "white";
    //        o["draggable"] = true;
    //    } else {
    //        div = document.getElementsByClassName("NewTestCssClass");
    //        document.getElementById(div[0]["id"]).classList.add("SettingsNavigation");
    //        document.getElementById(div[0]["id"]).classList.remove("NewTestCssClass");
    //        o.style.color = "black";
    //        o["draggable"] = false;
    //    }
    //}

    //function ShowWaitGifForIndexNodeTextBox() {
    //    if ($(".IndexNodeTextBox").val()[0] === "-") {
    //        $(".ErrorMessage").text("Число не может быть отрицательным.");
    //        return false;
    //    }
    //    $(".WaitForRightNaviControl").attr("style", "");
    //}
    //function ChangeIndexNodeTextBox(o) {
    //    if ($(o).val() < 1)
    //        $(o).val("1");
    //}
    ///* Navigation dialog */
</script>
