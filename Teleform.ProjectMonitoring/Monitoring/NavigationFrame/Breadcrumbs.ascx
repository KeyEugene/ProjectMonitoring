<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Breadcrumbs.ascx.cs" Inherits="Teleform.ProjectMonitoring.NavigationFrame.Breadcrumb" %>

<%--<link href="../Styles/Header_style/breadcrumbs.css" rel="stylesheet" />--%>

<asp:PlaceHolder runat="server">
    <div id="breadcrumb" class="breadcrumb" style="margin-bottom: 3px; box-shadow: 0 2px 3px lightgrey;">
        <asp:PlaceHolder runat="server" ID="breadcrumbs"></asp:PlaceHolder>
    </div>
</asp:PlaceHolder>
