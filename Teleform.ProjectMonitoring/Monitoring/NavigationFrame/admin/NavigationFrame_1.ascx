<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationFrame_1.ascx.cs" Inherits="Teleform.ProjectMonitoring.NavigationFrame.admin.NavigationFrame_1" %>

<style>
    .action_container input, .action_container a, .action_container select, .action_container label {
        margin-top: 30px;
    }

    .border_rigth {
        border-right: 1px solid lightgray;
    }
</style>

<div class="action_container row" id="action_container">
    <div class="column_left col-md-12 text-center ">
            <asp:Button ID="EventAddButton" runat="server" Text="Добавить событие" OnClick="EventAddButton_Click" CssClass="btn btn-default" />
            <asp:Button ID="EventEditButton" runat="server" Text="Изменить событие" OnClick="EventEditButton_Click" CssClass="btn btn-default" />
            <asp:Button ID="EventDeleteButton" runat="server" Text="Удалить событие" OnClick="EventDeleteButton_Click" CssClass="btn btn-danger" />
    </div>
    
</div>
