<%@ Page Title="Функционал АРМ" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="EntityListAttributeView.aspx.cs" Inherits="Teleform.ProjectMonitoring.EntityListAttributeView" %>

<%@ Register TagPrefix="Report" TagName="View" Src="~/ReportView/ReportView.ascx" %>
<%@ Register TagPrefix="Entity" TagName="DropDownList" Src="~/ServiceControls/EntityDropDownList.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
        
    <link href="Styles/TableView.css" rel="Stylesheet" />
    <link href="Styles/Site.css" rel="Stylesheet" />
    <link href="Styles/PredicateControl.css" rel="Stylesheet" />
    <link type="text/css" rel="Stylesheet" href="Styles/DynamicCardStyle.css" />
    <link type="text/css" rel="Stylesheet" href="Styles/DynamicCardPage.css" />


    <script type="text/javascript" src="./Scripts/site.js"></script>    
    <script type="text/javascript" src="./Scripts/TableView.js"></script>
    <script type="text/javascript" src="./Scripts/CompositePredicateControl.js"></script>

    <script type="text/javascript" src="./Scripts/DynamicCardListSearch.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {
           
            $('.filter-wrap').hide();


            $('.filter-container > .filter-header').click(function () {
                $('.filter-wrap').slideToggle(30);
            });


            $('.filter-container').mouseenter(function (i) {
                $(this).fadeTo(200, 1.0);
            });

            $('.filter-container').mouseleave(function (i) {
                $(this).fadeTo(200, 0.6);
            });

            $('.filter-container').fadeTo(0, 0.6);
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:LinkButton ID="reCreateEntityTriggers" runat="server" Text="reCreateEntityTriggers"
        OnClick="reCreateEntityTriggersButton_Click" Visible="false"></asp:LinkButton>
    <%--<asp:Label ID="EntitiListLabel" runat="server" Text="Основные типы объектов:" />--%>
    <div>
        <Entity:DropDownList runat="server" ID="NewEntityList" />
        
    </div>
    <%-- <asp:DropDownList ID="EntityList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="EntityList_SelectedIndexChanged"
        AppendDataBoundItems="true" SkinID="List" DataTextField="name" DataValueField="objID" />--%>
    <%--<asp:SqlDataSource ID="EntityListDataSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
        SelectCommand="SELECT tblID objID, tblAlias name FROM model.Entity('base') WHERE udpValue = 1 ORDER BY NAME">
    </asp:SqlDataSource>
    <asp:DropDownList ID="EntityList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="EntityList_SelectedIndexChanged"
        DataSourceID="EntityListDataSource" AppendDataBoundItems="true" SkinID="List"
        DataTextField="name" DataValueField="objID" />
    --%>
    <Report:View ID="report" runat="server" />


</asp:Content>
