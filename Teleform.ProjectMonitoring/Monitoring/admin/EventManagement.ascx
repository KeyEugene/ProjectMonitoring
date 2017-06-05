<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EventManagement.ascx.cs" Inherits="Teleform.ProjectMonitoring.admin.EventManagement" %>

<%@ Register TagPrefix="Navigation" TagName="Frame" Src="~/NavigationFrame/admin/NavigationFrame_1.ascx" %>

<Navigation:Frame ID="Frame" runat="server"></Navigation:Frame>


<Dialog:MessageBox ID="DeleteWarningDialog" runat="server" Icon="Warning" Buttons="YesNo"
    OnClosed="DeleteWarningDialog_Close">
    <ContentTemplate>
        Объект будет удалён. Продолжить?
    </ContentTemplate>
</Dialog:MessageBox>

<asp:SqlDataSource ID="EventListSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
    SelectCommand="SELECT [objID],[nameT],[nameC],[name],[daysFrom],[daysTo],[message] FROM [model].[vo_Events] where message is not NULL" />
<Dialog:Form ID="EventDialog" runat="server">
    <ContentTemplate>
        <asp:SqlDataSource ID="EventSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
            SelectCommand="SELECT [name],[message],[daysTo],[daysFrom],[objID] FROM [model].[vo_Events] WHERE [objID] = @eventID"
            UpdateCommand="EXEC [model].[EventUpdate] @eventID, @message, @daysFrom, @daysTo"
            InsertCommand="EXEC [model].[EventUpdate] @eventID, @message, @daysFrom, @daysTo">
            <SelectParameters>
                <asp:ControlParameter Name="eventID" Type="Int64" ControlID="EventGridView" PropertyName="SelectedValue"
                    ConvertEmptyStringToNull="true" DefaultValue="null" />
            </SelectParameters>
            <UpdateParameters>
                <asp:ControlParameter Name="eventID" Type="Int64" ControlID="EventGridView" PropertyName="SelectedValue"
                    ConvertEmptyStringToNull="true" DefaultValue="null" />
                <asp:ControlParameter Name="message" Type="String" ControlID="EventForm$EditMessageBox"
                    PropertyName="Text" ConvertEmptyStringToNull="true" />
            </UpdateParameters>
            <InsertParameters>
                <asp:ControlParameter Name="eventID" Type="Int64" ControlID="EventForm$EventAttributeList"
                    PropertyName="SelectedValue" />
                <asp:ControlParameter Name="message" Type="String" ControlID="EventForm$AddEventMessageBox"
                    PropertyName="Text" ConvertEmptyStringToNull="true" />
            </InsertParameters>
        </asp:SqlDataSource>
        <asp:FormView ID="EventForm" runat="server" DataKeyNames="objID" DataSourceID="EventSource">
            <InsertItemTemplate>
                <table class="table">
                    <tr>
                        <th>Имя аттрибута
                        </th>
                        <td>
                            <asp:SqlDataSource ID="EventAttributeDataSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
                                SelectCommand="SELECT name Attribute, objID eventID FROM [model].[vo_Events] where message is NULL" />
                            <asp:DropDownList ID="EventAttributeList" runat="server" DataSourceID="EventAttributeDataSource"
                                DataTextField="attribute" DataValueField="eventID" />
                        </td>
                    </tr>
                    <tr>
                        <th>Сообщение события:
                        </th>
                        <td>
                            <asp:TextBox ID="AddEventMessageBox" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <th>Предупредить за (дней):
                        </th>
                        <td>
                            <asp:TextBox ID="AddEventDaysFromBox" runat="server" Text='<%# Bind("daysFrom") %>' />
                        </td>
                    </tr>
                    <tr>
                        <th>Предупредить после (дней):
                        </th>
                        <td>
                            <asp:TextBox ID="AddEventDaysToBox" runat="server" Text='<%# Bind("daysTo") %>' />
                        </td>
                    </tr>
                </table>
            </InsertItemTemplate>
            <EditItemTemplate>
                <table class="table">
                    <tr>
                        <th>Сообщение события:
                        </th>
                        <td>
                            <asp:TextBox ID="EditMessageBox" runat="server" Text='<%# Bind("message") %>' />
                        </td>
                    </tr>
                    <tr>
                        <th>Предупредить за (дней):
                        </th>
                        <td>
                            <asp:TextBox ID="EditDaysFromBox" runat="server" Text='<%# Bind("daysFrom") %>' />
                        </td>
                    </tr>
                    <tr>
                        <th>Предупредить после (дней):
                        </th>
                        <td>
                            <asp:TextBox ID="EditDaysToBox" runat="server" Text='<%# Bind("daysTo") %>' />
                        </td>
                    </tr>
                </table>
            </EditItemTemplate>
        </asp:FormView>
    </ContentTemplate>
    <Buttons>
        <Dialog:ButtonItem Text="Сохранить" ControlID="EnumSaveButton" OnClick="EventSaveHandler" />
        <Dialog:ButtonItem Text="Отмена" ControlID="EnumCancelButton" OnClick="EventDialog.Close" />
    </Buttons>
</Dialog:Form>
<div class="table-responsive">
    <Phoenix:GridView ID="EventGridView" runat="server" DataKeyNames="objID" AutoGenerateColumns="false"
        DataSourceID="EventListSource" EnableViewState="false" CssClass="table table-bordered table-hover table-striped">
        <Columns>
            <asp:BoundField DataField="nameT" HeaderText="Объект" />
            <asp:BoundField DataField="nameC" HeaderText="Поле" />
            <asp:BoundField DataField="name" HeaderText="Наименование события" />
            <asp:BoundField DataField="message" HeaderText="Сообщение события" />
            <asp:BoundField DataField="daysFrom" HeaderText="Напомнить за" />
            <asp:BoundField DataField="daysTo" HeaderText="Напомнить после" />
        </Columns>
    </Phoenix:GridView>
</div>
