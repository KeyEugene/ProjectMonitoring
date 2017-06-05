<%@ Page Title="Личные настройки" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Settings.aspx.cs" Inherits="Teleform.ProjectMonitoring.Settings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="Styles/adminPageStyle.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table style="float: left">
        <tr style="vertical-align: top">
            <td>
                <div class="optionDiv">
                    <asp:Button ID="ChangePasswordButton" Text="Сменить пароль" runat="server" OnClick="ChangePasswordButton_Click"
                       />
                    <%--<asp:Button ID="MainEntityButton" Text="Основные сущности и шаблоны" runat="server"
                        OnClick="MainEntityButton_Click"/>--%>
                    <asp:Button ID="OnOffNavigation" Text="Вкл./Откл. навигацию" runat="server" OnClick="OnOffNavigationButton_Click"
                       />
                </div>
            </td>
            <td>
                <asp:MultiView ID="SettingView" runat="server" ActiveViewIndex="1">
                    <asp:View ID="MainEntityView" runat="server">
                  <!--
                         <asp:SqlDataSource ID="MainEntityDataSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'   
                               SelectCommand="SELECT [isLogicMain], b.alias tblAlias, b.object_id tblID FROM model.BTables b join model.AppTypes at on at.name='Base' and b.appTypeID=at.object_ID" />
                    -->
                        <asp:ListView runat="server"  ID="listView">
                        <%--<asp:ListView runat="server" ID="listView">--%>
                            <LayoutTemplate>
                                <table>
                                    <tr>
                                        <th>
                                            Вкл.
                                        </th>
                                        <th>
                                            Сущности
                                        </th>
                                        <th>
                                            Шаблоны
                                        </th>
                                    </tr>
                                    <tr runat="server" id="itemPlaceholder" />
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <asp:CheckBox ID="IncludeEntityBox" runat="server" Checked='<%# Eval("isLogicMain")%>' />
                                    </td>
                                    <td>
                                        <asp:HiddenField runat="server" Value='<%# Eval("tblID")%>' />
                                        <asp:Label ID="EntityLabel" runat="server" Text='<%# Eval("tblAlias")%>' />
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="EntityTemplatesList" runat="server" DataSource='<%# GetEntityTemplates(Eval("tblID")) %>'
                                            DataTextField="name" DataValueField="objID" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:ListView>
                    </asp:View>
                    <asp:View ID="ChangePasswordView" runat="server">
                        <Dialog:MessageBox ID="PasswordChangeMessage" runat="server" Icon="Notification">
                            <ContentTemplate>
                                Пароль успешно изменен.
                                <asp:LinkButton Text="Залогиниться заново" runat="server" PostBackUrl="~/Login.aspx" />
                            </ContentTemplate>
                        </Dialog:MessageBox>
                        <div style="float: left">
                            <asp:Label runat="server" Text="Новый пароль:" />
                            <asp:TextBox ID="PasswordBox" runat="server" TextMode="Password" />
                            <asp:Label runat="server" Text="Подтвердите новый пароль:" />
                            <asp:TextBox ID="DublicatePasswordBox" runat="server" TextMode="Password" />
                        </div>
                        <asp:Button ID="ChangeUserPasswordButton" runat="server" OnClick="ChangeUserPasswordButton_Click"
                            Text="Сменить пароль" />
                    </asp:View>
                    <asp:View ID="NavigationSettingsView" runat="server">
                        <asp:CheckBox ID="checkBoxMainNavigation" runat="server" Checked='true' Text="Вкл./Откл. навигацию."
                            AutoPostBack="true" />
                        <br />
                        <asp:CheckBox ID="checkBoxObjectsNavigation" runat="server" Checked='true' Text="Вкл./Откл. навигацию  по объектам."
                            AutoPostBack="true" />
                    </asp:View>
                </asp:MultiView>
            </td>
        </tr>
    </table>
</asp:Content>
