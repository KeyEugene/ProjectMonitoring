<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationDialog.ascx.cs" Inherits="Teleform.ProjectMonitoring.NavigationDialog" %>

<%--<asp:UpdatePanel runat="server" ID="NavigationTreeDialog">
    <ContentTemplate>
        <div runat="server" class="navigationTreeDialog" onmouseover="onMouseOverNavigDialog(this);" onmouseout="onMouseOutNavigDialog(this);">
            <div class="SettingsNavigation" id="SettingsNavig">
                <asp:CheckBox ID="ShowAllNavigation" Text="Показать все" runat="server" AutoPostBack="true" />
            </div>
            <div class="SettingsButtonNavigation" onclick="showHideSettings(this);">Настройки</div>
            <div class="movedialog" onclick="moveNavigationDialog(this);" id="MoveButtonNavigationPath">
                >>>
            </div>
            <asp:TreeView runat="server" ID="treeView" ViewStateMode="Disabled" EnableViewState="false">
            </asp:TreeView>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>--%>
<asp:TreeView runat="server" ID="treeView" ViewStateMode="Disabled" EnableViewState="false">
</asp:TreeView>
 <asp:CheckBox ID="ShowAllNavigation" Text="Показать все" runat="server" AutoPostBack="true" />

<asp:UpdatePanel runat="server" ID="NavigationObjects">
    <ContentTemplate>

        <div class="navigationTreeDialog NavigationObjects" onmouseover="onMouseOverNavigDialog(this);" onmouseout="onMouseOutNavigDialog(this);" 
            onclick="moveNavigationDialog(this);">
            <div class="movedialog" onclick="moveNavigationDialog(this);" id="MoveButtonNavigationObjects">
                >>>
            </div>
            <asp:PlaceHolder runat="server" ID="LevelContainer">
                <asp:TextBox ID="IndexNodeTextBox" CssClass="IndexNodeTextBox" runat="server"
                    Style="margin: 2px 1px 0px 50px; width: 50px;" ToolTip="Уровень раскрытия." type="number" onchange="ChangeIndexNodeTextBox(this)" />
                <asp:Button Text="Обновить" runat="server" OnClientClick="ShowWaitGifForIndexNodeTextBox();" />
                <asp:Image CssClass="WaitForRightNaviControl" ImageUrl="~/images/WaitForRightNaviControl.gif" runat="server" Style="display: none;" />
                <asp:Label CssClass="ErrorMessage" runat="server" Style="color: red;" />
            </asp:PlaceHolder>
            <asp:TreeView runat="server" ID="objectTreeView" ViewStateMode="Disabled" EnableViewState="false" />
        </div>
    </ContentTemplate>
</asp:UpdatePanel>

 <script>
        $(document).ready(function () {
            var navigation_object = $("[id*=NavigationObjects]");
            var navigation_object_container = $("#object_treeview_conteiner");
            navigation_object.appendTo(navigation_object_container);
        });
    </script>

<style>
    #IndexNodeTextBox {
        margin: 2px 1px 0px 50px;
        width: 50px;
    }
</style>

<script type="text/javascript">
    /* Navigation dialog */
    function moveNavigationDialog(o) {
        var divPath = document.getElementsByClassName("navigationTreeDialog");

        var isPath = o["id"] !== "MoveButtonNavigationPath";
        if (isPath)
            divPath = document.getElementsByClassName("NavigationObjects");  //Move NavigationObjects
        o = $(".movedialog")[0];
        if (o.innerText === ">>>") {
            o.innerText = "<<<";
            divPath[0].style.transition = "all 0.4s ease-in-out 0.1s";
            if (!isPath) {
                divPath[0].style.left = "0px";
                divPath[0].style.height = '470px';
            } else {
                divPath[0].style.left = "35%";//"623px";
                divPath[0].style.height = '550px';
            }
            divPath[0].style["opacity"] = "1";
        } else {
            o.innerText = ">>>";
            divPath[0].style.transition = "all 0.4s ease-in-out 0.1s";
            if (!isPath) {
                divPath[0].style.left = "-470px";
                divPath[0].style.height = '150px';
            } else {
                divPath[0].style.left = "97.8%";
                divPath[0].style.height = '150px';
            }
            divPath[0].style["opacity"] = "1"; //".1";.05
        }
    }

    function onMouseOverNavigDialog(o) {
        o.style.opacity = "1";
    }

    function onMouseOutNavigDialog(o) {
        o.style["opacity"] = "1"; //".1"; 05 --- разкоментить
    }

    function showHideSettings(o) {
        var div = document.getElementsByClassName("SettingsNavigation");

        if (!o["draggable"]) {
            document.getElementById(div[0]["id"]).classList.add("NewTestCssClass");
            document.getElementById(div[0]["id"]).classList.remove("SettingsNavigation");

            o.style.color = "white";
            o["draggable"] = true;
        } else {
            div = document.getElementsByClassName("NewTestCssClass");
            document.getElementById(div[0]["id"]).classList.add("SettingsNavigation");
            document.getElementById(div[0]["id"]).classList.remove("NewTestCssClass");
            o.style.color = "black";
            o["draggable"] = false;
        }
    }

    function ShowWaitGifForIndexNodeTextBox() {
        if ($(".IndexNodeTextBox").val()[0] === "-") {
            $(".ErrorMessage").text("Число не может быть отрицательным.");
            return false;
        }
        $(".WaitForRightNaviControl").attr("style", "");
    }
    function ChangeIndexNodeTextBox(o) {
        if ($(o).val() < 1)
            $(o).val("1");
    }
    /* Navigation dialog */
</script>
