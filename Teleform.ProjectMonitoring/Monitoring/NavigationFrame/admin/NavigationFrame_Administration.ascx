<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationFrame_Administration.ascx.cs" Inherits="Teleform.ProjectMonitoring.NavigationFrame.admin.NavigationFrame_Administration" %>


<div id="inner_left_menu" class="nav navbar-nav side-nav">
    <ul id="admin_menu" runat="server" class="nav navbar-nav side-nav">
        <li>
            <asp:LinkButton ID="EventManagementButton" runat="server" Text="Управление событиями"
                OnClick="AdminManagementButton_Click" CommandArgument="0" /></li>
        <li>
            <asp:LinkButton ID="ImportManagementBunnon" runat="server" Text="Импорт данных" OnClick="AdminManagementButton_Click"
                CommandArgument="1" /></li>
        <li>
            <asp:LinkButton ID="SettingTheTypesOfObjects" Text="Настройка типов объектов" runat="server"
                OnClick="AdminManagementButton_Click" CommandArgument="2" /></li>
        <li>
            <asp:LinkButton ID="AliasManagementButton" runat="server" Text="Настройка атрибутов объектов"
                OnClick="AdminManagementButton_Click" CommandArgument="3" /></li>
        <li>
            <asp:LinkButton ID="UserManagementButton" runat="server" Text="Управление пользователями"
                CommandArgument="4" OnClick="AdminManagementButton_Click" /></li>
        <li>
            <asp:LinkButton ID="SeparationOfAccessRights" runat="server" OnClick="AdminManagementButton_Click"
                CommandArgument="5" Text="Разделение прав доступа" /></li>
        <li>
            <asp:LinkButton ID="AuditButton" runat="server" OnClick="AdminManagementButton_Click"
                CommandArgument="6" Text="Аудит" /></li>
        <li>
            <asp:LinkButton ID="EnumerationManagement" runat="server" Text="Управление классификаторами"
                OnClick="EnumerationManagement_Click"  /></li>
    </ul>
    <script>
        //Делаем активной кнопку на которую нажали
        $("li.active").removeClass("active");
        $(".button_active").removeClass("button_active").parent().addClass("active");
    </script>
</div>
