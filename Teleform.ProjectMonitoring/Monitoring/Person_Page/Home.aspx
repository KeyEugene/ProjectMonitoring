<%@ Page Title="Моя страница" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Teleform.ProjectMonitoring.Person_Page.Home" %>

<%@ Register TagPrefix="Navigation" TagName="Breadcrumbs" Src="~/NavigationFrame/Breadcrumbs.ascx" %>
<%@ Register TagPrefix="Navigation" TagName="Frame" Src="~/NavigationFrame/NavigationFrame_Home.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <Navigation:Breadcrumbs runat="server"></Navigation:Breadcrumbs>
    <Navigation:Frame ID="Frame" runat="server"></Navigation:Frame>

</asp:Content>
