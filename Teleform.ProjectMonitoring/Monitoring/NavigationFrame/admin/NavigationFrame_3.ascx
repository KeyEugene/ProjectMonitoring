<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationFrame_3.ascx.cs" Inherits="Teleform.ProjectMonitoring.NavigationFrame.admin.NavigationFrame_3" %>

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
        <asp:Button ID="Button1" Text="Сохранить" runat="server" OnClick="SaveObjectsViewNew_Click" CssClass="btn btn-default" />
    </div>
    
</div>
