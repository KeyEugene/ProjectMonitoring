<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportView.ascx.cs" Inherits="Teleform.ProjectMonitoring.ReportView" %>
<%@ Register Src="~/project/FilterDesigner.ascx" TagPrefix="Project" TagName="FilterDesigner" %>

<%@ Register TagPrefix="Navigation" TagName="Frame" Src="~/NavigationFrame/NavigationFrame_EntityListAttributeView.ascx" %>

<Navigation:Frame ID="Frame" runat="server"></Navigation:Frame>
<%--
<%@ Register TagPrefix="Dynamic" Namespace="DynamicCardControl.Controls" Assembly="DynamicCardControl" %>
--%>
<script type="text/javascript">
    var afterPost = 0;

    var isMouseOnAgrDiv = false;
    var isDropOn = false;

    var currentMouseX;
    var currentMouseY;
    var prevMouseX;
    var prevMouseY;


    $(document).ready(function () {

        //Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandler);

        //window_resize();

        //colResize();

        $('div.main').attr('onscroll', 'SetDivPosition();');
        if (afterPost == 1) {
            var top = $('#hfScrollPosition').attr('Value');
            $('div.main').scrollTop(top);
            afterPost = 0;
        }

        $('#agrDiv').attr('onselectstart', 'return false');
        $('#ReportGridView').attr('onselectstart', 'return false');
        $('.main').attr('onselectstart', 'return false');

        var wHeight = $(window).height();
        var tHeight = $('#AgregateView').height();
        var pWidth = $('#agrDiv').width();
        var tWidth = $('#AgregateView').width();

        if (tWidth > pWidth)
            $('#agrDiv').height(tHeight + 18);
        else
            $('#agrDiv').height(tHeight + 2);

        $('#agrDiv').fadeTo(0, 0.5);

        //-------------- Перемещение таблицы агрегации -----------------//

        currentMouseX = 0; prevMouseX = 0;
        currentMouseY = 0; prevMouseY = 0;

        $('#agrDiv').mouseenter(function (e, t) { agrMouseEnter(e, this); });
        $('#agrDiv').mouseleave(function (e, t) { agrMouseLeave(e, this); });
        $(document).mousemove(function (e) { TrackMouseMove(e); });
        $(document).mousedown(function (e) { SetMoveFlag(e); });
        //$(document).mouseup(function (e) { CheckFlags(e); });

        //--------------------------------------------------------------//


        //---------------- Перенос таблицы агрегации в основную таблицу. -----------------//
        if (document.getElementById("agrDiv") != undefined) {

            var pageCount = $('#PageCountList').val();

            var mainTable = $('#ReportGridView > tbody');
            var allRows = mainTable.children('tr');

            var headerRow = allRows.eq(0);
            var colCount = headerRow.children('th').length;
            var div = $('#agrDiv').remove();

            if (allRows.length > (parseInt(pageCount) + 1)) {
                allRows.eq(allRows.length - 2).after('<tr><td colspan="' + colCount + '">' + div.html() + '</td></tr>');
            }
            else {
                allRows.eq(allRows.length - 1).after('<tr><td colspan="' + colCount + '">' + div.html() + '</td></tr>');
            }

        }
        //-----------------------------------------------------------------------------------//


    });

    function agrMouseEnter(e, elem) {
        isMouseOnAgrDiv = true;
        $(elem).fadeTo(300, 1);
    }

    function agrMouseLeave(e, elem) {
        isMouseOnAgrDiv = false;
        $(elem).fadeTo(300, 0.5);
    }

    function SetMoveFlag(e) {
        if (e.which != 1)
            return;
        if (isMouseOnAgrDiv == true)
            isDropOn = true;
        else
            isDropOn = false;
    }

    function TrackMouseMove(e) {
        prevMouseX = currentMouseX;
        prevMouseY = currentMouseY;
        currentMouseX = e.pageX;
        currentMouseY = e.pageY;

        if (isDropOn == true)
            MoveAgrDiv();
    }

    function MoveAgrDiv() {
        if (isDropOn == true && isMouseOnAgrDiv == true) {
            var curAgrPosX = $('#agrDiv').offset().left;
            var curAgrPosY = $('#agrDiv').offset().top;

            var offX = currentMouseX - (prevMouseX - curAgrPosX);
            var offY = currentMouseY - (prevMouseY - curAgrPosY);

            $('#agrDiv').offset({ top: offY, left: offX });

        }
    }

    function CheckFlags(e) {
        var agr = $('#agrDiv');
        var left = agr.offset().left;
        var right = left + agr.width();
        var top = agr.offset().top;
        var bottom = top + agr.height();

        if ((e.pageX > left && e.pageX < right) && (e.pageY > top && e.pageY < bottom)) {
            isMouseOnAgrDiv = true;
            isDropOn = false;
        }
        else {
            isMouseOnAgrDiv = false;
            isDropOn = false;
        }
    }

    function SetDivPosition() {
        if (afterPost != 1) {
            var intY = $('div.main').scrollTop();
            $('#hfScrollPosition').attr('Value', intY);
        }
    }

    function afterpostback() {
        afterPost = 1;
    }

    //for new Constructor
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
</script>
<Dialog:MessageBox runat="server" ID="TemplateSavedMessageBox" Caption="Шаблон создан"
    Icon="Notification">
    <ContentTemplate>
        Шаблон успешно сохранен.
    </ContentTemplate>
</Dialog:MessageBox>
<Dialog:MessageBox ID="DeleteMessage" Icon="Question" Buttons="YesNo" runat="server"
    Caption="Внимание" OnClosed="DeleteMessage_Closed">
    <ContentTemplate>
        Объект будет безвозвратно удалён. Продолжить?
    </ContentTemplate>
</Dialog:MessageBox>
<Dialog:Form ID="CreateDialog" runat="server" Caption="Добавить элемент" CancelButtonID="CancelButton">
    <ContentTemplate>
        <asp:Table ID="DialogTable" runat="server">
        </asp:Table>
    </ContentTemplate>
    <Buttons>
        <Dialog:ButtonItem ControlID="AcceptButton" Text="Применить" />
        <Dialog:ButtonItem ControlID="CancelButton" Text="Отмена" />
    </Buttons>
</Dialog:Form>
<asp:HiddenField ID="hfScrollPosition" runat="server" Value="0" ClientIDMode="Static" />
<asp:UpdatePanel ID="RecordsNumberUpPanel" runat="server">
    <ContentTemplate>
        <asp:Label ID="Wcf_summator" runat="server" />
        <asp:Label ID="RecordsNumberLabel" runat="server" />
    </ContentTemplate>
</asp:UpdatePanel>
<div>
    Отображать по:
        <asp:DropDownList runat="server" ID="PageCountList" AutoPostBack="true" ClientIDMode="Static"
            OnSelectedIndexChanged="PageCountList_SelectedIndexChanged">
            <asp:ListItem Text="10" Value="10" Selected="True" />
            <asp:ListItem Text="20" Value="20" />
            <asp:ListItem Text="50" Value="50" />
            <asp:ListItem Text="Все" Value="all" />
        </asp:DropDownList>
</div>
<asp:Button Visible="false" ID="FilterReNameButton" runat="server" Text="Переименовать"
    OnClick="FilterReNameButton_Click" />
<asp:Button Visible="false" ID="FilterDeleteButton" runat="server" Text="Удалить"
    OnClick="FilterDeleteButton_Click" BackColor="#DAE2F5" />
<asp:SqlDataSource ID="NavigationDataSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
    SelectCommand="SELECT * FROM [model].[ListAttribute](@entity)  where isIndexed = 1">
    <SelectParameters>
        <asp:QueryStringParameter Name="entity" ConvertEmptyStringToNull="true" QueryStringField="entity" />
    </SelectParameters>
</asp:SqlDataSource>
<input type="hidden" id="IsNavigationListSelectedIndexChanged" runat="server" value="false"
    clientidmode="Static" />
<asp:LinkButton ID="navigationLink" runat="server" Text="ClientIDMode=Static" Style="display: none"
    ClientIDMode="Static" PostBackUrl=""></asp:LinkButton>

<asp:MultiView ID="ReportMultiView" runat="server" ActiveViewIndex="0">

    <asp:View ID="TemplateView" runat="server">

        <Dialog:Form runat="server" ID="GroupReportForm2" Caption="Подготовка отчета по шаблону"
            CancelButtonID="CancelButton">
            <ContentTemplate>
                <table cellspacing="10">
                    <tr>
                        <td>
                            <asp:Button ID="CreateTemplateButton" runat="server" Text="Создать шаблон" OnClick="CreateTemplateButton_Click" />
                            <asp:Button ID="EditTemplateButton" runat="server" Text="Редактировать шаблон" OnClick="EditTemplateButton_Click" />
                            <asp:DropDownList OnSelectedIndexChanged="ReportsTemplatesList_OnSelectedIndexChanged"
                                OnLoad="ReportsTemplatesList_OnLoad" AutoPostBack="true" runat="server" ID="ReportsTemplatesList"
                                DataValueField="Value" DataTextField="Text" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label Visible="false" runat="server" ID="ArchiveNameLabel">Имя архива:</asp:Label>
                            <asp:TextBox Visible="false" runat="server" ID="ArchiveNameBox" placeholder="" />
                            <asp:Button Visible="false" ID="DownloadButton" runat="server" Text="Загрузить отчет"
                                OnClick="DownloadButton_Click" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Buttons>
                <%--      <Dialog:ButtonItem Text="Загрузить" OnClick="DownloadButton_Click" />--%>
                <Dialog:ButtonItem Text="Отмена" ControlID="CancelButton" OnClick="CencelButton_Click" />
            </Buttons>
        </Dialog:Form>
        <Dialog:Form ID="TemplateDesignerDialog" runat="server" CancelButtonID="CancelButton"
            ButtonsAlign="Center" OnClosed="TemplateDialog_Closed">
            <ContentTemplate>
                <asp:SqlDataSource ID="TemplateTypeDataSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
                    SelectCommand="SELECT [name],[code] FROM [model].[R$TemplateType]" />
                <asp:RadioButtonList ID="RadioList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="TemplateTypeRadioButton_Click"
                    DataSourceID="TemplateTypeDataSource" DataTextField="name" DataValueField="code" />
                <asp:PlaceHolder ID="PlaceHolder1" runat="server" />
            </ContentTemplate>
            <Buttons>
                <Dialog:ButtonItem Text="Сохранить" ControlID="SaveButton" OnClick="SaveButton_Click" />
                <Dialog:ButtonItem Text="Отмена" ControlID="CancelButton" />
            </Buttons>
        </Dialog:Form>

        <asp:UpdatePanel ID="ReportViewUpdatePanel" runat="server">
            <ContentTemplate>
                <Report:TableViewControl runat="server" ID="ReportViewControl" PageSize="10" CssClass="gridviewStyle"
                    OnSelectedItemCreating="ReportViewControl_SelectedIndexChanged" OnDataReady="ReportViewControl_DataReady"
                    Width="100%">
                    <filterstyle cssclass="FilterBox" />
                    <predicatecontrolstyle cssclass="PredicateControl" />
                    <activestyle cssclass="Active" />
                    <pagestyle cssclass="pageStyle" />
                    <activepagestyle cssclass="activePageStyle" />
                </Report:TableViewControl>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:TextBox Style="display: none" ID="ResizableTableControlBox" Width="1000" runat="server" />
        <asp:TextBox Style="display: none" ID="ColResizableTempIDBox" Width="100" runat="server" />
        <asp:TextBox Style="display: none" ID="SelectedRowIDBox" runat="server"></asp:TextBox>
        <asp:TextBox Style="display: none" ID="SaveObjectsJeysonBox" runat="server" Width="1800" />
    </asp:View>
    <asp:View ID="TemplateDesignerView" runat="server">
        <Template:TableTemplateDesigner ID="TemplateDesigner" runat="server" OnCloseButtonClick="CloseButtonClick_Click"
            ShowCloseButton="true" ShowSaveButton="true" ShowSaveAsButton="true" ShowCreateNewTemplateButton="true" />
    </asp:View>
    <asp:View ID="FilterDesignerView" runat="server">
        <Project:FilterDesigner ID="FilterDesigner" runat="server" OnDesigningFinished="FilterDesigner_DesigningFinished" />
    </asp:View>
</asp:MultiView>
<Dialog:Form ID="FilterDialog" runat="server">
    <ContentTemplate>
        <table>
            <tr>
                <td>Имя фильтра
                </td>
                <td>
                    <asp:TextBox ID="InsertNameFilter" Width="250px" runat="server" placeholder="обязательно для заполнения" />
                </td>
            </tr>
        </table>
    </ContentTemplate>
    <Buttons>
        <Dialog:ButtonItem Text="Применить" ControlID="ApplyNewFilterButton" OnClick="ApplyNewFilterButton_Click" />
        <Dialog:ButtonItem Text="Отмена" ControlID="CancelButton" OnClick="FilterDialog.Close" />
    </Buttons>
</Dialog:Form>
<Dialog:Form ID="ReNameFilterDialog" runat="server" OnClosed="ReNameFilterDialogCloseHandler">
    <ContentTemplate>
        <table>
            <tr>
                <td>Имя фильтра
                </td>
                <td>
                    <asp:TextBox ID="RenameNameBox" Width="250px" runat="server" placeholder="обязательно для заполнения" />
                </td>
            </tr>
        </table>
    </ContentTemplate>
    <Buttons>
        <Dialog:ButtonItem Text="Применить" ControlID="ApplyReNameFilterButton" OnClick="ApplyReNameFilterButton_Click" />
        <Dialog:ButtonItem Text="Отмена" ControlID="CancelButton" OnClick="ReNameFilterDialog.Close" />
    </Buttons>
</Dialog:Form>
<Dialog:MessageBox ID="DeleteWarningDialog" runat="server" Icon="Warning" Buttons="YesNo"
    OnClosed="DeleteWarningDialog_Close">
    <ContentTemplate>
        Объект будет удалён. Продолжить?
    </ContentTemplate>
</Dialog:MessageBox>
<Dialog:MessageBox ID="WarningMessageBox" runat="server" Icon="Notification">
    <ContentTemplate>
        <asp:Label ID="WarningLabel" runat="server" />
    </ContentTemplate>
</Dialog:MessageBox>
<Dialog:MessageBox ID="ErrorMessageBox" runat="server" Icon="Error">
    <ContentTemplate>
        <asp:Label ID="ErrorLabel" runat="server" />
    </ContentTemplate>
</Dialog:MessageBox>
<Dialog:Form ID="ReferenceTableControlDialog" runat="server">
    <ContentTemplate>
        <asp:PlaceHolder ID="PlaceHolder" runat="server" />
    </ContentTemplate>
</Dialog:Form>
<Dialog:MessageBox runat="server" ID="WarningMessageBoxAuthorization" Caption="Внимание !"
    Icon="Warning" Buttons="OK">
    <ContentTemplate>
        У вас нет прав для совершение данного действия.
    </ContentTemplate>
</Dialog:MessageBox>
