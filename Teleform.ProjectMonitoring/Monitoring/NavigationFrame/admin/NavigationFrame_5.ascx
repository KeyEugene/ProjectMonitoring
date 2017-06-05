<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationFrame_5.ascx.cs" Inherits="Teleform.ProjectMonitoring.NavigationFrame.admin.NavigationFrame_5" %>

<style>
    .action_container input, .action_container a, .action_container select, .action_container label {
        margin-top: 30px;
    }

    .border_rigth {
        border-right: 1px solid lightgray;
    }

    .action_container select {
        display: inline !important;
    }
</style>

<div class="action_container row" id="action_container">
    <div class="column_left col-md-12 text-center">
        <asp:Button Text="Создание" runat="server" ID="ButtonNew" OnClick="ButtonNew_Click" ViewStateMode="Disabled" EnableViewState="false" CssClass="btn btn-default" />
        <asp:Button Text="Изминение" runat="server" ID="ButtonUpdate" OnClick="ButtonUpdate_Click" ViewStateMode="Disabled" EnableViewState="false" CssClass="btn btn-default" />
        <asp:Button Text="Удаление" runat="server" ID="ButtonDelete" OnClick="ButtonDelete_Click" ViewStateMode="Disabled" EnableViewState="false" CssClass="btn btn-danger" />
    </div>
</div>
