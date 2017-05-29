<%@ Page Title="Древовидное представление" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="HardTemplateView.aspx.cs" Inherits="Teleform.ProjectMonitoring.HardTemplate.HardTemplateView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../Styles/PredicateControl.css" rel="Stylesheet" />
    <link href="../Styles/HardTemplate.css" rel="Stylesheet" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Literal ID="MainEntityLiteral" Text="Типы объектов:" runat="server" />
    <asp:DropDownList runat="server" ID="EntityList" AutoPostBack="true" OnSelectedIndexChanged="EntityList_OnSelectedIndex" />
    &nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Literal ID="Literal1" Text="Шаблоны :" runat="server" />
    <asp:DropDownList ID="TemplateList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="TemplateList_SelectedIndexChanged" />
    <br />
    <br />
    <asp:Button Text="Отчет" runat="server" ID="ButtonShowTemplate" OnClick="ShowTemplateView_OnClick"
        OnClientClick="ShowIgame();" />
    <asp:Button runat="server" Text="Экспорт в Excel" ID="toExcelButton" OnClick="toExcelButton_Click"
        BackColor="#CDCDCD" />
    <asp:Button Text="Редактор" runat="server" ID="ButtonConsctuctor" OnClick="BtnConstructor_OnClick" />
    <asp:MultiView ActiveViewIndex="0" runat="server" ID="MainView" OnActiveViewChanged="Multiview_AciveViewChanged">
        <asp:View runat="server" ID="PreviewHardTemplate">
            <asp:Image ID="Image1" CssClass="imagePage" Style="visibility: hidden;" runat="server" ImageUrl="~/images/wait.gif" />
            <asp:Image ID="MainImage" CssClass="imagePage" ImageUrl="~/images/tree_structure.png" runat="server" />
        </asp:View>
        <asp:View runat="server" ID="ConstructorHardTemplate">
            <div class="ConstructorCss">
                <Template:TreeBasedTemplateDesigner ID="treeDesigner" runat="server" />
            </div>
        </asp:View>
        <asp:View runat="server" ID="ShowTreeView">
            <div align="left" class="treeView">
                <asp:TreeView ID="tree" runat="server" ViewStateMode="Disabled" EnableViewState="false" />
            </div>
        </asp:View>
    </asp:MultiView>
    <Dialog:MessageBox runat="server" ID="WarningMessageBox" Caption="Внимание !" Icon="Warning" Buttons="OK">
        <ContentTemplate>
            У вас нет прав для совершение данного действия.
        </ContentTemplate>
    </Dialog:MessageBox>

    <script type="text/javascript" src="../Scripts/site.js"></script>
    <script type="text/javascript" src="../Scripts/CompositePredicateControl.js"></script>
    <script type="text/javascript" src="../Scripts/HardTemplate.js"></script>
</asp:Content>
