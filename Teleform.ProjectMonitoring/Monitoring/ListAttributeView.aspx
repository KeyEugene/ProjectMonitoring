<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ListAttributeView.aspx.cs" Inherits="Teleform.ProjectMonitoring.ListAttributeView" %>

<%@ Register TagPrefix="Report" TagName="View" Src="~/ReportView/ReportView.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="Styles/TableView.css" rel="Stylesheet" />
    <link href="Styles/PredicateControl.css" rel="Stylesheet" />
    <link type="text/css" rel="Stylesheet" href="Styles/DynamicCardStyle.css" />
    <link type="text/css" rel="Stylesheet" href="Styles/DynamicCardPage.css" />
    <%-- <script type="text/javascript" src="./Scripts/DynamicCardScript.js"></script>--%>
    <script type="text/javascript" src="./Scripts/DynamicCardListSearch.js"></script>
    <script type="text/javascript" src="./Scripts/site.js"></script>
    <script type="text/javascript" src="./Scripts/TableView.js"></script>
    <script type="text/javascript" src="./Scripts/CompositePredicateControl.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <Report:View ID="report" runat="server" />
</asp:Content>
