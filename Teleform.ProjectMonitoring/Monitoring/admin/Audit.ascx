<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Audit.ascx.cs" Inherits="Teleform.ProjectMonitoring.admin.Audit" %>

<%@ Register TagPrefix="Navigation" TagName="Frame" Src="~/NavigationFrame/admin/NavigationFrame_7.ascx" %>

<link href="../Styles/TableView.css" rel="Stylesheet" />
<link href="../Styles/PredicateControl.css" rel="Stylesheet" />
<script type="text/javascript" src="../Scripts/site.js"></script>
<script type="text/javascript" src="../Scripts/CompositePredicateControl.js"></script>


<Navigation:Frame ID="Frame" runat="server"></Navigation:Frame>

<style type="text/css">
    details[open] {
        position: static;
    }

    .templateMenu {
        padding-left: 10px;
        border-bottom: 1px silver solid;
        vertical-align: middle;
        box-shadow: 0 0.5px 1px #bbb;
    }
</style>

<asp:SqlDataSource ID="AuditSqlDataSource" runat="server" ConnectionString='<%$Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
    SelectCommand="SELECT [entity], [instanceID], [creationDate], [modificationDate], [eliminationDate], [creatorID], [modifierID], [eliminatorID] from [Z_Instance]" />
<asp:GridView ID="ViewAudit" runat="server" OnPageIndexChanging="ViewAudit_OnPageIndexChanging"
    AllowPaging="true" OnRowDataBound="ViewAudit_RowDataBound" CssClass="table table-bordered table-hover">
</asp:GridView>
