<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationFrame_7.ascx.cs" Inherits="Teleform.ProjectMonitoring.NavigationFrame.admin.NavigationFrame_7" %>



<div class="action_container row" id="action_container">
    <div class="col-md-1"></div>
    <div class="column_left col-md-10">

        <table>
            <tr>
                <td align="center">Типы объектов
                </td>
                <td align="center">Пользователи
                </td>
                <td align="center">Дата изменения от
                </td>
                <td align="center">Дата изменения до
                </td>
                <td align="center"></td>
            </tr>
            <tr>
                <td align="center">
                    <asp:DropDownList ID="EntityListAudit" runat="server" AutoPostBack="false" OnLoad="EntityListAudit_Load" CssClass="form-control" />
                </td>
                <td align="center">
                    <asp:DropDownList ID="UserListAudit" runat="server" AutoPostBack="false" OnLoad="UserListAudit_Load" CssClass="form-control" />
                </td>
                <td align="center">
                    <asp:TextBox runat="server" ID="DateFrom" type="datetime-local" CssClass="form-control"></asp:TextBox>
                </td>
                <td align="center">
                    <asp:TextBox runat="server" ID="DateTo" type="datetime-local" CssClass="form-control"></asp:TextBox>
                </td>
                <td align="center">
                    <asp:Button ID="ViewButton" runat="server" Text="Показать" OnClick="ViewButton_Click" CssClass="btn btn-default"></asp:Button>
                </td>
            </tr>
        </table>
    </div>
    <div class="col-md-1"></div>
</div>
