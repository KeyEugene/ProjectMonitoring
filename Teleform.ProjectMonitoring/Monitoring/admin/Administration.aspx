<%@ Page Title="Администрирование" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Administration.aspx.cs" Inherits="Monitoring.Administration"
    Async="true" AsyncTimeout="1200" %>

<%@ Register TagPrefix="View" TagName="Event" Src="~/admin/EventManagement.ascx" %>
<%@ Register TagPrefix="View" TagName="Import" Src="~/admin/ImportManagement.ascx" %>
<%@ Register TagPrefix="View" TagName="Objects" Src="~/admin/SettingTheTypesOfObjects.ascx" %>
<%@ Register TagPrefix="View" TagName="Alias" Src="~/admin/SettingAttributesOfEntities.ascx" %>
<%@ Register TagPrefix="View" TagName="User" Src="~/admin/UserManagement.ascx" %>
<%@ Register TagPrefix="View" TagName="Sharing" Src="~/admin/Sharing.ascx" %>
<%@ Register TagPrefix="View" TagName="Audit" Src="~/admin/Audit.ascx" %>

<%@ Register TagPrefix="Navigation" TagName="Frame" Src="~/NavigationFrame/admin/NavigationFrame_Administration.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="../Scripts/CompositePredicateControl.js"></script>
    <script type="text/javascript" src="../Scripts/TableView.js"></script>
    <link href="../Styles/adminPageStyle.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/AdminUserMenegment.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .alert .alert-shadow {
            position: fixed;
            top: 0;
            width: 100%;
            height: 100%;
            left: 0;
            background: #000;
            opacity: 0.5;
            filter: alpha(opacity=50);
        }

        .alert .alert-dialog {
            position: fixed;
            top: 30%;
            left: 50%;
            width: 430px;
            margin-left: -150px;
            padding: 10px 0;
            background: #fff;
            border: 1px solid #000;
        }

        .alert .alert-text {
            padding: 10px;
        }

        .alert .alert-controls {
            text-align: center;
        }
    </style>
    <script type="text/javascript">

        function ShowBusyMessage() {
            alert('Ожидайте окончания импорта данных!');
        }

        function Alert(text) {
            this._create(text);
        }
        Alert.prototype = {
            constructor: Alert,
            html: [
                '<div class="alert">',
                    '<div class="alert-shadow"></div>',
                    '<div class="alert-dialog">',
                        '<div class="alert-text">',
                            '{TEXT}',
                        '</div>',
                        '<div class="alert-controls">',
                            '<button style="display:none">OK</button>',
                        '</div>',
                        '<div class="alert-controls">',
                            '<img src="../images/loader.gif">',
                        '</div>',
                    '</div>',
                '</div>'
            ].join(""),
            _rootElement: null,
            _create: function (text) {
                var node = document.createElement("div");
                node.innerHTML = this.html.replace("{TEXT}", text);
                this._rootElement = node.firstChild;
                this._addEvents();
            },
            _addEvents: function () {
                var thisAlert = this;
                this._rootElement.getElementsByTagName("button")[0].onclick = function () {
                    thisAlert.close();
                };
            },
            show: function () {
                document.body.appendChild(this._rootElement);
            },
            close: function () {
                document.body.removeChild(this._rootElement);
            }
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <Navigation:Frame ID="Frame" runat="server"></Navigation:Frame>

    <asp:MultiView ID="AdministrationOptionsMulti" runat="server">
        <%-- EventManagement View --%>
        <asp:View ID="EventView" runat="server">
            <View:Event runat="server" ID="EventManagement" />
        </asp:View>
        <%-- ImportManagement View --%>
        <asp:View ID="ImportView" runat="server">
            <View:Import runat="server" ID="ImportManager" />
        </asp:View>
        <%--Setting The Types Of Objects--%>
        <asp:View ID="TypesOfObjectsView" runat="server" OnActivate="SettingTheTypesOfObjects.DataBindCheckBoxForTypesOfObjects">
            <View:Objects runat="server" ID="SettingTheTypesOfObjects" />
        </asp:View>
        <%-- Setting attributes of entities view --%>
        <asp:View ID="AliasView" runat="server">
            <View:Alias runat="server" />
        </asp:View>
        <%--Setting UserManagement --%>
        <asp:View runat="server" ID="userManagement">
            <View:User runat="server" ID="users" />
        </asp:View>
        <%-- Work very hard with table Permission(Sharing) and "read/create/update/delete" --%>
        <asp:View ID="SeparationOfAccessRightsView" runat="server">
            <View:Sharing runat="server" />
        </asp:View>
        <asp:View ID="AuditView" runat="server">
            <View:Audit runat="server" />
        </asp:View>
    </asp:MultiView>

    <script type="text/javascript" src="../Scripts/AdminUserManagement.js"> </script>
</asp:Content>
