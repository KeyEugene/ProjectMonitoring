<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImportManagement.ascx.cs" Inherits="Teleform.ProjectMonitoring.admin.ImportManagement" %>

<%@ Register TagPrefix="Navigation" TagName="Frame" Src="~/NavigationFrame/admin/NavigationFrame_2.ascx" %>

<Navigation:Frame ID="Frame" runat="server"></Navigation:Frame>



<asp:SqlDataSource ID="ImportSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
    SelectCommand="	SELECT 
                            [h].[start],
		                    [h].[finish],
		                    [s].[name] [status],
		                    [h].msg,
		                    [p].[name] 
	                    FROM [Import].[History] [h]
	                    LEFT JOIN [Import].[State] [s] ON [s].[objID] = [h].[stateID]
	                    LEFT JOIN [_Person] [p] ON [p].[objID] = [h].[_userID]
	                    ORDER BY [h].[start] DESC"></asp:SqlDataSource>
<asp:UpdatePanel ID="HistoryUpdate" runat="server" UpdateMode="Conditional">
    <%--  <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="HistoryTimer" EventName="Tick" />
                            </Triggers>--%>
    <ContentTemplate>
        <Phoenix:GridView ID="ImportGridView" runat="server" AutoGenerateColumns="false"
            DataSourceID="ImportSource" EnableViewState="false">
            <Columns>
                <asp:BoundField DataField="name" HeaderText="Пользователь" />
                <asp:BoundField DataField="start" HeaderText="Время начала" />
                <asp:BoundField DataField="finish" HeaderText="Время окончания" />
                <asp:BoundField DataField="status" HeaderText="Состояние" />
                <asp:BoundField DataField="msg" HeaderText="Сообщение" />
            </Columns>
        </Phoenix:GridView>
    </ContentTemplate>
</asp:UpdatePanel>
<%-- <asp:Timer ID="HistoryTimer" runat="server" Interval="1000" OnTick="HistoryTimer_Tick" />--%>

<Dialog:MessageBox runat="server" Icon="Notification" ID="SuccessfulImportMessageBox">
    <ContentTemplate>
        Загрузка данных выполнена успешно
    </ContentTemplate>
</Dialog:MessageBox>
<Dialog:MessageBox ID="NoFileMessageBox" runat="server" Icon="Error">
    <ContentTemplate>
        Импорт данных невозожен. Выберите файл.
    </ContentTemplate>
</Dialog:MessageBox>
<Dialog:MessageBox runat="server" Icon="Error" ID="UnSuccessfulImportMessageBox">
    <ContentTemplate>
        Не удалось выполнить загрузку данных.
    </ContentTemplate>
</Dialog:MessageBox>
<Dialog:MessageBox ID="ImportBusyMessageBox" runat="server" Icon="Error">
    <ContentTemplate>
        В текущий момент уже происходит импорт данных.
    </ContentTemplate>
</Dialog:MessageBox>
