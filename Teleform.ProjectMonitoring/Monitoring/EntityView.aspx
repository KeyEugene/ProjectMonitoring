<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EntityView.aspx.cs" Inherits="Teleform.ProjectMonitoring.EntityView" %>
<%@ Register TagPrefix="Report" TagName="View" Src="~/ReportView/ReportView.ascx"%>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="tyxt/css">    
    </style>
    <link type="text/css" href="./Styles/selectedRow.css" rel="Stylesheet" />

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
    <asp:Label runat="server" Text="Сформировать отчет по: " />

    <asp:DropDownList ID="EntityList"  runat="server" AutoPostBack="true" 
        onselectedindexchanged="EntityList_SelectedIndexChanged"/>

    <Report:View ID="report" runat="server" />
</asp:Content>
