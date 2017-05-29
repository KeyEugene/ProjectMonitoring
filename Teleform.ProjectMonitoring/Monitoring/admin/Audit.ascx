<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Audit.ascx.cs" Inherits="Teleform.ProjectMonitoring.admin.Audit" %>
<link href="../Styles/TableView.css" rel="Stylesheet" />
<link href="../Styles/PredicateControl.css" rel="Stylesheet" />
<script type="text/javascript" src="../Scripts/site.js"></script>
<script type="text/javascript" src="../Scripts/CompositePredicateControl.js"></script>
<style type="text/css">
    details[open]
    {
        position: static;
    }
    
    .templateMenu
    {
        padding-left: 10px;
        border-bottom: 1px silver solid;
        vertical-align: middle;
        box-shadow: 0 0.5px 1px #bbb;
    }
</style>
<table class="templateMenu">
    <tr>
        <td align="center">
            Типы объектов
        </td>
        <td align="center">
            Пользователи
        </td>
        <td align="center">
            Дата изменения от
        </td>
        <td align="center">
            Дата изменения до
        </td>
        <td align="center">
        </td>
    </tr>
    <tr>
        <td align="center">
            <asp:DropDownList ID="EntityListAudit" runat="server" AutoPostBack="false" OnLoad="EntityListAudit_Load" />
        </td>
        <td align="center">
            <asp:DropDownList ID="UserListAudit" runat="server" AutoPostBack="false" OnLoad="UserListAudit_Load" />
        </td>
        <td align="center">
            <asp:TextBox runat="server" ID="DateFrom" type="datetime-local"></asp:TextBox>
        </td>
        <td align="center">
            <asp:TextBox runat="server" ID="DateTo" type="datetime-local"></asp:TextBox>
        </td>
        <td align="center">
            <asp:Button ID="ViewButton" runat="server" Text="Показать" OnClick="ViewButton_Click">
            </asp:Button>
        </td>
    </tr>
</table>
<asp:SqlDataSource ID="AuditSqlDataSource" runat="server" ConnectionString='<%$Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
    SelectCommand="SELECT [entity], [instanceID], [creationDate], [modificationDate], [eliminationDate], [creatorID], [modifierID], [eliminatorID] from [Z_Instance]" />
<asp:GridView ID="ViewAudit" runat="server" OnPageIndexChanging="ViewAudit_OnPageIndexChanging"
    AllowPaging="true" OnRowDataBound="ViewAudit_RowDataBound">
</asp:GridView>
