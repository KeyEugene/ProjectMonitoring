<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationFrame_6.ascx.cs" Inherits="Teleform.ProjectMonitoring.NavigationFrame.admin.NavigationFrame_6" %>

<style>
    .action_container input, .action_container a, .action_container select, .action_container label {
        margin-top: 5px;
    }

    .border_rigth {
        border-right: 1px solid lightgray;
    }

    .action_container select {
        display: inline !important;
    }

    .green_border {
        border: 3px solid lightgreen;
    }

    .default_border {
        border: 1px solid #ccc;
        border-radius: 4px;
    }

    #button_show {
        margin-top: 5px;
    }
</style>

<div class="action_container row" id="action_container">
    <div class="col-md-3"></div>
    <div class="column_left col-md-8">

        <table>
            <tr>
                <th>Типы пользователей 
                </th>
                <th>Пользователи
                </th>
                <th>Тип объектов /
                    <asp:LinkButton Text="Шаблоны" runat="server" ID="LinkBtnTemplate" OnClick="LinkBtnTemplate_Click" CssClass="linkBtn" />
                    <asp:LinkButton runat="server" CssClass="CloseTemplate_LinkButton" ID="CloseTemplate_LinkButton" OnClick="CloseTemplate_Click">
                        <asp:Image ImageUrl="~/images/clear.png" runat="server" ID="ImageCloseTemplate" Visible="false"
                            CssClass="closeTemplateImg" ToolTip="Закрыть шаблоны ?" />
                    </asp:LinkButton>
                </th>
            </tr>
            <tr>
                <td>
                    <asp:DropDownList runat="server" ID="UserTypeList" AutoPostBack="true" OnSelectedIndexChanged="UserTypeList_IndexChanged" CssClass="form-control" />
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="UserList" AutoPostBack="true" OnSelectedIndexChanged="UserList_IndexChanged" CssClass="form-control" />
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="EntityList" AutoPostBack="true" OnSelectedIndexChanged="EntityList_IndexChanged" CssClass="form-control" />
                </td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <asp:Button Text="Сохранить" runat="server" ID="SaveButton" OnClick="SaveButton_OnClick"
                        CssClass="btn btn-default" Visible="true" />
                    <asp:Button Text="Сохранить шаблоны" runat="server" ID="SaveTemplate" OnClick="SaveTemplate_Click"
                        CssClass="btn btn-default" Visible="false" />
                    <span class="btn btn-danger" id="button_show" onclick="ShowButton();">Сброс</span>
                </td>
                <td></td>
            </tr>
        </table>

    </div>
    <div class="col-md-1"></div>
</div>
