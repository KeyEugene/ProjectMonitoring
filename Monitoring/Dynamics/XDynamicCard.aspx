<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="XDynamicCard.aspx.cs" Inherits="Teleform.ProjectMonitoring.Dynamics.XDynamicCard" %>

<%@ Register TagPrefix="Dynamics" Namespace="Teleform.Reporting.DynamicCard" Assembly="DynamicCardControl" %>
<%@ Register TagPrefix="np" TagName="NavigationPanel" Src="~/NavigationPanel.ascx" %>


<%@ Register  TagPrefix="Documnet" TagName="Preview" Src="~/Dynamics/DocumentPreview.ascx" %>



<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link type="text/css" rel="Stylesheet" href="../Styles/DynamicCardStyle.css" />
    <link type="text/css" rel="Stylesheet" href="../Styles/DynamicCardPage.css" />
    <link href="../Styles/TableView.css" rel="Stylesheet" />
    <link href="../Styles/Site.css" rel="Stylesheet" />
    <link href="../Styles/PredicateControl.css" rel="Stylesheet" />
    <link href="../Styles/gridviewStyle.css" rel="Stylesheet" />
    <script type="text/javascript" src="../Scripts/DynamicCardScript.js"></script>
    <script type="text/javascript" src="../Scripts/DynamicCardListSearch.js"></script>
    <script type="text/javascript" src="../Scripts/site.js"></script>
    <script type="text/javascript" src="../Scripts/TableView.js"></script>
    <script type="text/javascript" src="../Scripts/CompositePredicateControl.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {


            colResize();
            colResizeCard();
            colResizeCardContainer();
            //SetResizableCellsWidth();


        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<Dialog:MessageBox ID="ErrorMessageBox" runat="server" Icon="Error">
    <ContentTemplate>
        <asp:Label ID="ErrorLabel" runat="server" />
    </ContentTemplate>
</Dialog:MessageBox>

    <Dialog:MessageBox ID="DeleteMessage" Icon="Question" Buttons="YesNo" runat="server"
        Caption="Внимание" OnClosed="DeleteMessage_Closed">
        <ContentTemplate>
            Объект будет безвозвратно удалён. Продолжить?
        </ContentTemplate>
    </Dialog:MessageBox>
    <Dialog:MessageBox runat="server" ID="DenayPermissionDialog" Buttons="OK">
        <ContentTemplate>
            Недостаточно прав для совершения данного действия.
        </ContentTemplate>
    </Dialog:MessageBox>

     <Dialog:MessageBox runat="server" ID="CreateErroreDialog" Caption="Внимание!" Buttons="OK" >
        <ContentTemplate>
            Объект создать не удалось.
        </ContentTemplate>
    </Dialog:MessageBox>

    <table>
        <tr>
            <td>
                <%= DCControl.DateCalculatorHyperText()%>
            </td>
            <td>
                <details>
 <summary style="color: gray;">
Шаблоны, фильтры:
</summary>
<table id="TeplatesFilters" runat="server" visible="false">
<tr><td>     <asp:SqlDataSource ID="TemplateListSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
                    SelectCommand="EXEC [model].[R$TemplateGetTableBased] @entityID">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="DCControl" PropertyName="EntityName" Name="entityID" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:DropDownList ID="TemplateList" runat="server" AutoPostBack="true" DataTextField="name"
                                DataValueField="objID" OnSelectedIndexChanged="TemplateList_SelectedIndexChanged"
                                DataSourceID="TemplateListSource" AppendDataBoundItems="true" SkinID="List" />
</td></tr>

<tr><td>
<asp:LinkButton  ID="ResetAllFilters" runat="server" Text="Сброс фильтров:" OnClick="ResetAllFilters_OnClick"
            ToolTip="Сбрасывает все фильтры." />
</td></tr>

<tr><td>
<asp:LinkButton  ID="ResetAllSortings" runat="server" Text="Сброс сортировок:" OnClick="ResetAllSortings_OnClick"
            ToolTip="Сбрасывает все сортировки." />
</td></tr>


</table>
</details>
            </td>
            <td>
                <details>
 <summary style="color: gray;">
Отчеты, объекты:
</summary>


<table >

<tr><td><Project:FastReportControl ID="frc" runat="server" /></td></tr>

<tr><td>        <asp:Image Visible="false"  ID="DeleteImage" runat="server" Style="position: relative;
                    top: -4px; left: 1px" ImageUrl="~/images/delete.png" Width="20" Height="20" ImageAlign="Middle" />
                <asp:LinkButton Visible="false"   ID="DeleteInstance"
                    runat="server" Text="Удалить объект:" ToolTip="Удалить объект" OnClick="DeleteInstance_Click" />
</td></tr>
<tr><td> 
                <asp:Image ID="SaveImage" runat="server" ImageUrl="~/images/save-icon.png" Width="20" Height="20" ImageAlign="Middle"
                           Style="position: relative; top: -4px; left: 1px" />
                <asp:LinkButton Visible="false"  ID="SaveObjects" runat="server" Text="Сохранить объекты:"
                    OnClick="SaveObjects_OnClick" ToolTip="Сохранить объекты" /></td></tr>

</table>

         </details>
            </td>
        </tr>
        <asp:UpdatePanel style="float: right;" runat="server" ID="RecordsNumberUpPanel">
            <ContentTemplate>
                <asp:Label ID="ListEntity" runat="server" Visible="false" />
                <asp:Label ID="RecordsNumberLabel" runat="server" Visible="false" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </table>
    <asp:TextBox Style="display: none" ID="ResizableCardBox" Width="700" runat="server" />
    <asp:TextBox Style="display: none" ID="ResizableCardContainerBox" Width="700" runat="server" />
    <table id="TableContainer" class="TableContainer">
        <tr>
            <td id="CellContainer1">
             <%--OnDocumentDisplaying="DisplayDocumentHandler"--%>
             
                <Dynamics:DynamicCardControl runat="server"  ID="DCControl" ClientTableID="CardTable" 
                    OnListRelationClicked="ListRelationClickedHandler" OnEditionCanceled="DynamicCard_EditionCanceled"
                    OnRelationClicked="RelationClickedHandler" OnSelfFieldClicked="SelfFieldClickedHandler"
                    OnModeChanged="ModeChangedHandler" OnCardSaving="DynamicCardCommonContainer_CardSaving" OnCardNotSaving="DynamicCardCommonContainer_CardNotSaving"
                    OnControlReturning="DynamicCard_ControlReturning">
                    <downloadbuttonstyle cssclass="downloadButton" />
                    <openrelatedlistbuttonstyle cssclass="changeButton" />
                    <clearreferencebuttonstyle cssclass="resetButton" />
                </Dynamics:DynamicCardControl>
            </td>
            <td id="CellContainer2">
                <%--<div class="InfoContainer">--%>
                <asp:MultiView ActiveViewIndex="0" runat="server" ID="AdditionalViews">
                    <asp:View ID="DefaultView" runat="server">
                    </asp:View>
                    <asp:View ID="DocumentPreviewView" runat="server">

                        <Documnet:Preview ID="DocumentPreview" runat="server" />

                    </asp:View>
                    <asp:View ID="EnumerationView" runat="server">
                        <asp:GridView runat="server" ID="EnumerationGridView" AutoGenerateColumns="true">
                        </asp:GridView>
                    </asp:View>
                    <asp:View ID="ObjectView" runat="server">
                        <Dynamics:DynamicCardControl runat="server" ID="HelpCard" class="left" AllowManagement="false"
                            ClientTableID="CardTable">
                            <downloadbuttonstyle cssclass="downloadButton" />
                        </Dynamics:DynamicCardControl>
                    </asp:View>
                    <asp:View ID="ReportView" runat="server">
                        <asp:UpdatePanel ID="ReportViewUpdatePanel" runat="server">
                            <ContentTemplate>
                                <Report:TableViewControl CssClass="gridviewStyle" runat="server" ID="ReportViewControl"
                                    PageSize="10" OnSelectedItemCreating="ReportViewControl_SelectedIndexChanged"
                                    OnDataReady="ReportViewControl_DataReady" Width="100%">
                                    <filterstyle cssclass="FilterBox" />
                                    <predicatecontrolstyle cssclass="PredicateControl" />
                                    <activestyle cssclass="Active" />
                                    <pagestyle cssclass="pageStyle" />
                                    <activepagestyle cssclass="activePageStyle" />
                                </Report:TableViewControl>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:TextBox Style="display: none" ID="ResizableTableControlBox" Width="1000" runat="server" />
                        <asp:TextBox Style="display: none" ID="ColResizableTempIDBox" Width="300" runat="server" />
                        <asp:TextBox Style="display: none" ID="SelectedRowIDBox" runat="server" Width="200"></asp:TextBox>
                        <asp:TextBox Style="display: none" ID="SaveObjectsJeysonBox" runat="server" Width="1800" />
                    </asp:View>
                </asp:MultiView>
                <%--</div>--%>
            </td>
        </tr>
    </table>
</asp:Content>
