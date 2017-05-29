<%@ Page Title="Шаблоны" Language="C#" AutoEventWireup="true" CodeBehind="TemplateManager.aspx.cs"
    MasterPageFile="~/Site.Master" Inherits="Teleform.ProjectMonitoring.Templates.TemplateManager" %>

<%@ Register TagPrefix="Navigation" TagName="Breadcrumbs" Src="~/NavigationFrame/Breadcrumbs.ascx" %>
<%@ Register TagPrefix="Navigation" TagName="Frame" Src="~/NavigationFrame/NavigationFrame_Template.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <style type="text/css">
        .frame {
            width: 100%;
            height: 100%;
        }

            .frame tr:first-child {
                background-color: #F9F9F9;
            }

            .frame td:first-child {
                background-color: #F9F9F9;
                width: 300px;
                border-right: 2px silver solid;
                box-shadow: 1px 0 2px #bbb;
            }
    </style>
    <link href="../Styles/TableView.css" rel="Stylesheet" />
    <link href="../Styles/PredicateControl.css" rel="Stylesheet" />
    <link href="../Styles/HardTemplate.css" rel="Stylesheet" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server" EnableViewState="true"
    ViewStateMode="Enabled">

    <script type="text/javascript">
        function keyup_handlerFilterControl(o, itemID) {
            var c = $("#" + itemID).find("option");
            for (var i = 0; i < c.length; i++) {
                var option = c.eq(i);
                var label = option.valueOf("label");

                var html = label.html();

                if (html.toString().toLowerCase().indexOf(o.value.toLowerCase()) == -1)
                    option.css("display", "none");
                else
                    option.css("display", "block");
            }
        }
        $(document).ready(function () {
           //Чтобы диалог был на первом плане 
            $('[id$=TemplateDesignerDialog]').css("z-index", "1030");
        });
    </script>

    <Navigation:Breadcrumbs runat="server"></Navigation:Breadcrumbs>
    <Navigation:Frame ID="Frame" runat="server"></Navigation:Frame>

    <div class="content">
        <iframe runat="server" id="PreviewFrame" style="width: 100%; height: 100%; border: none; margin: 0; padding: 0"
            enableviewstate="false"></iframe>

        <Dialog:Form ID="TemplateDesignerDialog" runat="server" OnClosed="TemplateDialog_Closed"
            CancelButtonID="CancelButton" ButtonsAlign="Center">
            <ContentTemplate>
                <asp:SqlDataSource ID="TemplateTypeDataSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
                    SelectCommand="SELECT [name],[code] FROM [model].[R$TemplateType]" />
                <asp:RadioButtonList ID="RadioList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="TemplateTypeButton_Click"
                    DataSourceID="TemplateTypeDataSource" DataTextField="name" DataValueField="code" />
                <asp:PlaceHolder ID="PlaceHolder" runat="server" />
            </ContentTemplate>
            <Buttons>
                <Dialog:ButtonItem Text="Сохранить как" ControlID="SaveAsButton" OnClick="SaveAsButton_Click" />
                <Dialog:ButtonItem Text="Сохранить" ControlID="SaveButton" OnClick="SaveButton_Click" />
                <Dialog:ButtonItem Text="Отмена" ControlID="CancelButton" />
            </Buttons>
        </Dialog:Form>
    </div>

    <script type="text/javascript" src="../Scripts/site.js"></script>
    <script type="text/javascript" src="../Scripts/CompositePredicateControl.js"></script>
    <script type="text/javascript" src="../Scripts/HardTemplate.js"></script>
</asp:Content>
