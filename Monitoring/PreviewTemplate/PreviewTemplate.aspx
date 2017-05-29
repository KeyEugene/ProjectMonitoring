<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PreviewTemplate.aspx.cs" Inherits="Monitoring.Reporting.PreviewTemplate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Button ID="ShowPreviewTemplateButton" runat="server" OnClick="ShowPreviewTemplateButton_Click" Text="Предпросмотр"/>
    <asp:LinkButton ID="ExampleButton" Text="Тест" runat="server" PostBackUrl="~/58.preview"/>
</asp:Content>
