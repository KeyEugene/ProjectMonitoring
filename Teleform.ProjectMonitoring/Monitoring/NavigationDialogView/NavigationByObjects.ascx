<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationByObjects.ascx.cs" Inherits="Teleform.ProjectMonitoring.NavigationDialogView.NavigationByObjects" %>

<div style="display: none;">
    <asp:Button Text="Показать меню по объектам" runat="server" OnClick="OpenMenuDialog_Click" ID="openMenuDialog"  />
</div>
<style>
    .objectTreeView {
        max-width: 1290px;
        overflow: auto;
        padding: 10px;
    }

    .DialogBackground {
        z-index: 1140 !important;
    }

    .object_item_selected {
        color: black;
        /*border: 1px solid lightgray;
        padding: 4px;
        background-color: lightgray;
        border-radius: 3px;*/
    }

    .navigationTagA {
        color: gray;
    }

        .navigationTagA:hover {
            color: black;
        }
</style>

<Dialog:Form ID="dialog_object" runat="server" Caption="Меню по объектам">
    <ContentTemplate>
        <div class="dialog_container"></div>
    </ContentTemplate> 
    <Buttons>
        <Dialog:ButtonItem Text="Отмена" OnClick="CloseDialog_Click" />
    </Buttons>
</Dialog:Form>


<%--ViewStateMode="Disabled" EnableViewState="false"--%>
<asp:UpdatePanel runat="server" ID="NavigationObjects" class="menuByObject" Visible="false">
    <ContentTemplate>
        <asp:PlaceHolder runat="server" ID="LevelContainer">
            <asp:TextBox ID="IndexNodeTextBox" CssClass="IndexNodeTextBox" runat="server"
                ToolTip="Уровень раскрытия." type="number" onchange="ChangeIndexNodeTextBox(this)" />
            <asp:Button Text="Обновить" runat="server" OnClientClick="ShowWaitGifForIndexNodeTextBox();" ID="button_update" />
            <asp:Image CssClass="WaitForRightNaviControl" ImageUrl="~/images/WaitForRightNaviControl.gif" runat="server" Style="display: none;" />
            <asp:Label CssClass="ErrorMessage" runat="server" Style="color: red;" />
        </asp:PlaceHolder>
        <asp:TreeView runat="server" ID="objectTreeView" CssClass="objectTreeView" />
    </ContentTemplate>
</asp:UpdatePanel>

<script>
    //Делаем так, потому что внутри диалога форм не видны контролы 
    var dialog_container = $(".dialog_container");
    var menuByObject = $(".menuByObject");
    menuByObject.appendTo(dialog_container);


</script>
