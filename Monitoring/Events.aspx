<%@ Page Title="Уведомления" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Events.aspx.cs" Inherits="Monitoring.Events" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="./Scripts/CompositePredicateControl.js"></script>
    <script type="text/javascript" src="./Scripts/TableView.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%-- Данные для списка событий. --%>
    <asp:SqlDataSource ID="EventListSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
        SelectCommand="EXEC [Report].[GetCountedEventList] 0" />
    <%-- Данные для списка шаблонов, выбранного события. --%>
    <%--( [mimeTypeID] IS NULL OR [mimeTypeID] = 666 )--%>
    <asp:SqlDataSource ID="TemplateListSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
        SelectCommand="SELECT t.objID,t.name FROM model.R$Template t join model.R$TemplateType y on y.objID=t.typeID and y.code='TableBased' and entityID = @entityID ORDER BY [name]">
        <SelectParameters>
            <asp:ControlParameter Name="entityID" ControlID="EventListGridView" PropertyName='SelectedDataKey["tableID"]' />
        </SelectParameters>
    </asp:SqlDataSource>
    <%-- Данные для отчетов. --%>
    <asp:SqlDataSource ID="EventObjectsSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
        SelectCommand="EXEC [report].[GetEventObjectList] @templateID, @tableName, @columnName, @daysFrom, @daysTo">
        <SelectParameters>
            <asp:ControlParameter Name="templateID" DbType="Int32" ControlID="TemplateList" PropertyName="SelectedValue" />
            <asp:ControlParameter Name="tableName" DbType="String" ControlID="EventListGridView"
                PropertyName='SelectedDataKey["tableName"]' />
            <asp:ControlParameter Name="columnName" DbType="String" ControlID="EventListGridView"
                PropertyName='SelectedDataKey["columnName"]' />
            <asp:ControlParameter Name="daysFrom" DbType="Int32" ControlID="EventListGridView"
                PropertyName='SelectedDataKey["daysFrom"]' />
            <asp:ControlParameter Name="daysTo" DbType="Int32" ControlID="EventListGridView"
                PropertyName='SelectedDataKey["daysTo"]' />
        </SelectParameters>
    </asp:SqlDataSource>
    <div style="width: 95%">
        <div style="position: relative; margin-bottom: 10px; background-color: #e7e7e7; padding: 4px 0">
            <asp:Label runat="server" Text="Вид шаблона:" />

            <%--<asp:DropDownList ID="TemplateList" runat="server" DataSourceID="TemplateListSource"
                DataValueField="objID" DataTextField="name" Style="min-width: 200px" AutoPostBack="true" />--%>

                <asp:DropDownList ID="TemplateList" runat="server" 
                DataValueField="objID" DataTextField="name" Style="min-width: 200px" AutoPostBack="true" />

            <asp:Button ID="FormReportButton" runat="server" Text="Сформировать отчет" OnClick="CreateExcelReportButton_Click" />
            <%--<asp:Literal runat="server" Text="В лист №: " />
          <asp:TextBox ID="SheetNumberBox" runat="server" Text="1" style="width:30px" />--%>
        </div>
        <table border="0" cellpadding="0" cellspacing="5" style="float: left; margin: -12px;
            height: 100%">
            <tr>
                <td colspan="2" style="text-align: center">
                </td>
            </tr>
            <tr>
                <td style="vertical-align: top; min-width: 300px">
                    <%--DataSourceID="EventListSource"--%>
                    <Phoenix:GridView ID="EventListGridView" runat="server" 
                        DataKeyNames="tableID,columnID,rTableName,tableName,columnName,daysFrom,daysTo"
                        EnableViewState="false" OnDataBound="EventListGridView_DataBound" OnSelectedIndexChanged="EventListGridView_SelectedIndexChanged">
                        <Columns>
                            <asp:TemplateField HeaderText="Список событий">
                                <ItemTemplate>
                                    <asp:Label runat="server" Text='<%# 
                         string.Format("{0} ({1})", 
                                Eval("eventname"),
                                Eval("countObjects"))
                         %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </Phoenix:GridView>
                </td>
                <td style="vertical-align: top;">
                    <asp:GridView OnRowDataBound="EventObjectsGridView__RowDataBound" ID="EventObjectsGridView"
                        runat="server" ShowHeader="true" DataSourceID="EventObjectsSource" DataKeyNames="objID" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
