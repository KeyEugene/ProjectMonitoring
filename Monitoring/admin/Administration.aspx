<%@ Page Title="Администрирование" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Administration.aspx.cs" Inherits="Monitoring.Administration"
    Async="true" AsyncTimeout="1200" %>

<%@ Register TagPrefix="UserSharing" Src="~/admin/Sharing.ascx" TagName="ViewSharing" %>
<%@ Register TagPrefix="Audit" Src="~/admin/Audit.ascx" TagName="ViewAudit" %>
<%@ Register TagPrefix="User" Src="~/admin/UserManagement.ascx" TagName="ViewUser" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="../Scripts/CompositePredicateControl.js"></script>
    <script type="text/javascript" src="../Scripts/TableView.js"></script>
    <link href="../Styles/adminPageStyle.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/AdminUserMenegment.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .alert .alert-shadow {
            position: fixed;
            top: 0;
            width: 100%;
            height: 100%;
            left: 0;
            background: #000;
            opacity: 0.5;
            filter: alpha(opacity=50);
        }

        .alert .alert-dialog {
            position: fixed;
            top: 30%;
            left: 50%;
            width: 430px;
            margin-left: -150px;
            padding: 10px 0;
            background: #fff;
            border: 1px solid #000;
        }

        .alert .alert-text {
            padding: 10px;
        }

        .alert .alert-controls {
            text-align: center;
        }
    </style>
    <script type="text/javascript">

        function ShowBusyMessage() {
            alert('Ожидайте окончания импорта данных!');
        }

        function Alert(text) {
            this._create(text);
        }
        Alert.prototype = {
            constructor: Alert,
            html: [
                '<div class="alert">',
                    '<div class="alert-shadow"></div>',
                    '<div class="alert-dialog">',
                        '<div class="alert-text">',
                            '{TEXT}',
                        '</div>',
                        '<div class="alert-controls">',
                            '<button style="display:none">OK</button>',
                        '</div>',
                        '<div class="alert-controls">',
                            '<img src="../images/loader.gif">',
                        '</div>',
                    '</div>',
                '</div>'
            ].join(""),
            _rootElement: null,
            _create: function (text) {
                var node = document.createElement("div");
                node.innerHTML = this.html.replace("{TEXT}", text);
                this._rootElement = node.firstChild;
                this._addEvents();
            },
            _addEvents: function () {
                var thisAlert = this;
                this._rootElement.getElementsByTagName("button")[0].onclick = function () {
                    thisAlert.close();
                };
            },
            show: function () {
                document.body.appendChild(this._rootElement);
            },
            close: function () {
                document.body.removeChild(this._rootElement);
            }
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <Dialog:MessageBox ID="DeleteWarningDialog" runat="server" Icon="Warning" Buttons="YesNo"
        OnClosed="DeleteWarningDialog_Close">
        <ContentTemplate>
            Объект будет удалён. Продолжить?
        </ContentTemplate>
    </Dialog:MessageBox>
    <table border="0" cellpadding="0" cellspacing="0" style="float: left;">
        <tr>
            <td style="vertical-align: top; padding-top: 10px">
                <div id="OptionsDiv" runat="server" class="optionDiv">
                    <asp:Button ID="EventManagementButton" runat="server" Text="Управление событиями"
                        OnClick="AdminManagementButton_Click" CommandArgument="0" CssClass="optionButton" />
                    <asp:Button ID="ImportManagementBunnon" runat="server" Text="Импорт данных" OnClick="AdminManagementButton_Click"
                        CommandArgument="1" CssClass="optionButton" />
                    <asp:Button ID="SettingTheTypesOfObjects" Text="Настройка типов объектов" runat="server"
                        OnClick="AdminManagementButton_Click" CommandArgument="2" CssClass="optionButton" />
                    <asp:Button ID="AliasManagementButton" runat="server" Text="Настройка атрибутов объектов"
                        OnClick="AdminManagementButton_Click" CommandArgument="3" CssClass="optionButton" />
                    <asp:Button ID="EnumerationManagement" runat="server" Text="Управление классификаторами"
                        CssClass="optionButton" />
                    <asp:Button ID="UserManagementButton" runat="server" Text="Управление пользователями"
                        CssClass="optionButton" CommandArgument="4" OnClick="AdminManagementButton_Click" />
                    <asp:Button ID="SeparationOfAccessRights" runat="server" OnClick="AdminManagementButton_Click"
                        CommandArgument="5" CssClass="optionButton" Text="Разделение прав доступа" />
                    <asp:Button ID="AuditButton" runat="server" OnClick="AdminManagementButton_Click"
                        CommandArgument="6" CssClass="optionButton" Text="Аудит" />
                </div>
            </td>
            <td style="padding-top: 10px; padding-left: 20px">
                <asp:MultiView ID="AdministrationOptionsMulti" runat="server">
                    <%-- Event View --%>
                    <asp:View ID="EventView" runat="server">
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
                                        <table>
                                            <tr>
                                                <td>Имя аттрибута
                                                </td>
                                                <td>
                                                    <asp:SqlDataSource ID="EventAttributeDataSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
                                                        SelectCommand="SELECT name Attribute, objID eventID FROM [model].[vo_Events] where message is NULL" />
                                                    <asp:DropDownList ID="EventAttributeList" runat="server" DataSourceID="EventAttributeDataSource"
                                                        DataTextField="attribute" DataValueField="eventID" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Сообщение события:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="AddEventMessageBox" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Предупредить за (дней):
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="AddEventDaysFromBox" runat="server" Text='<%# Bind("daysFrom") %>' />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Предупредить после (дней):
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="AddEventDaysToBox" runat="server" Text='<%# Bind("daysTo") %>' />
                                                </td>
                                            </tr>
                                        </table>
                                    </InsertItemTemplate>
                                    <EditItemTemplate>
                                        <table>
                                            <tr>
                                                <td>Сообщение события:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="EditMessageBox" runat="server" Text='<%# Bind("message") %>' />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Предупредить за (дней):
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="EditDaysFromBox" runat="server" Text='<%# Bind("daysFrom") %>' />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Предупредить после (дней):
                                                </td>
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
                        <asp:Button ID="EventAddButton" runat="server" Text="Добавить событие" OnClick="EventAddButton_Click" />
                        <asp:Button ID="EventEditButton" runat="server" Text="Изменить событие" OnClick="EventEditButton_Click" />
                        <asp:Button ID="EventDeleteButton" runat="server" Text="Удалить событие" OnClick="EventDeleteButton_Click" />
                        <Phoenix:GridView ID="EventGridView" runat="server" DataKeyNames="objID" AutoGenerateColumns="false"
                            DataSourceID="EventListSource" EnableViewState="false">
                            <Columns>
                                <asp:BoundField DataField="nameT" HeaderText="Объект" />
                                <asp:BoundField DataField="nameC" HeaderText="Поле" />
                                <asp:BoundField DataField="name" HeaderText="Наименование события" />
                                <asp:BoundField DataField="message" HeaderText="Сообщение события" />
                                <asp:BoundField DataField="daysFrom" HeaderText="Напомнить за" />
                                <asp:BoundField DataField="daysTo" HeaderText="Напомнить после" />
                            </Columns>
                        </Phoenix:GridView>
                    </asp:View>
                    <%-- Import View --%>
                    <asp:View ID="ImportView" runat="server">
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
                        <asp:FileUpload ID="ImportUpload" runat="server" EnableViewState="true" />
                        <%--<asp:CheckBox ID="FullImportCheck" runat="server" Checked="true" Text="Полная загрузка" />--%>
                        <asp:Label runat="server" Text="Тип загрузки" />
                        <asp:DropDownList runat="server" ID="ImportModeList" AppendDataBoundItems="true">
                            <asp:ListItem Text="полная" Value="0" />
                            <asp:ListItem Text="подгрузка" Value="1" />
                            <asp:ListItem Text="инкор" Value="2" />
                        </asp:DropDownList>
                        <asp:Button ID="ImportButton" runat="server" Text="Импорт данных" OnClientClick="new Alert('Идет загрузка данных.\r\nЭто может занять некоторое время.').show();"
                            OnClick="ImportButton_Click" />
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
                    </asp:View>
                    <%--Setting The Types Of Objects--%>
                    <asp:View ID="TypesOfObjectsView" runat="server" OnActivate="DataBindCheckBoxForTypesOfObjects">
                        <asp:Table runat="server" ID="TableObjects">
                            <asp:TableHeaderRow>
                                <asp:TableHeaderCell>
                                   <span style="font: italic bold 16px/1px Georgia, serif;"> 
                        Вкл.
                        Тип объекта
                                        </span>
                                </asp:TableHeaderCell>
                                <asp:TableHeaderCell>
                                    <span style="font: italic bold 16px/1px Georgia, serif;"> 
                        Шаблоны
                                        </span>
                                </asp:TableHeaderCell>
                            </asp:TableHeaderRow>
                        </asp:Table>

                        <asp:Button ID="Button1" Text="Сохранить" runat="server" OnClick="SaveObjectsViewNew_Click" />
                    </asp:View>
                    <%-- Setting attributes of entities view --%>
                    <asp:View ID="AliasView" runat="server">
                        <%--'<%# string.Concat(" Добавление в таблицу : ", EntityList.SelectedItem.Text) %>'--%>
                        <Dialog:Form ID="AddAttriabute" runat="server" Caption="Добавление атрибута">
                            <ContentTemplate>
                                <table cellpadding="7">
                                    <tr>
                                        <td>Псевдоним :
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="NameColumn" ToolTip="A-z, А-я, 0-9, - _" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Код :
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="NameAttribute" ToolTip="Введите название латинскими буквами, возможно добавление специальных знаков: - или _" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Тип столбца :
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ListType" runat="server" DataSourceID="ListTypeSource" DataTextField="name"
                                                DataValueField="sysName" Width="155" SkinID="List" ViewStateMode="Disabled" />
                                            <asp:SqlDataSource ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
                                                ID="ListTypeSource" runat="server" SelectCommand="SELECT alias name,name sysName FROM model.Types WHERE name != sName and alias is not NULL ORDER BY alias" />
                                        </td>
                                    </tr>
                                </table>
                                <div align="center" style="padding: 3px">
                                    <asp:RegularExpressionValidator ID="ValidatorNameAttribute" ErrorMessage="Поле «Код» содержит недопустимые символы.<br />"
                                        ControlToValidate="NameAttribute" runat="server" ValidationExpression="[A-Za-z\s_\-\0-9]{1,32}$"
                                        ForeColor="Tomato" Font-Bold="true" ValidationGroup="xyz" />
                                    <asp:RegularExpressionValidator ID="ValidatorForNameColumn" ErrorMessage="Поле «Псевдоним» содержит недопустимые символы.<br />"
                                        ControlToValidate="NameColumn" runat="server" ValidationExpression="([_]?)([A-Za-zА-Яа-я0-9\s]+)([_]|[-])?([A-Za-zА-Яа-я0-9\s]+)"
                                        ForeColor="Tomato" Font-Bold="true" ValidationGroup="xyz" />
                                </div>
                            </ContentTemplate>
                            <Buttons>
                                <Dialog:ButtonItem Text="Добавить" OnClick="ButtonAdd_Attribute" />
                                <Dialog:ButtonItem Text="Отмена" OnClick="ButtonAdd_Close" />
                            </Buttons>
                        </Dialog:Form>
                        <asp:DropDownList ID="EntityList" runat="server" AutoPostBack="true" DataSourceID="EntitySource"
                            DataTextField="name" DataValueField="entity" />
                        <asp:Button Text="Добавление атрибута" ID="ButtonAdd" OnClick="AddAttriabute.Show"
                            runat="server" />
                        <asp:Button Text="Синхронизировать" runat="server" OnClick="Synchronize" />
                        <asp:SqlDataSource ID="EntitySource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
                            SelectCommand="SELECT alias name, name entity FROM model.BTables ORDER BY [alias] " />
                        <asp:SqlDataSource ID="AliasSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
                            SelectCommand="SELECT a.name attribute, a.alias udpValue, a.isTitle title, a.isUserMade flUserMade FROM  model.vo_TableColumns a where tbl=@entity and a.isOwn=1 and a.name<>a.alias order by a.alias"
                            UpdateCommand="UPDATE c set alias=@udpValue, isTitle=@title from model.Columns c join model.BTables t on t.name=@entity and c.object_id=t.object_id"
                            DeleteCommand="EXEC [model].[UserAttributeDrop]  @entity , @attribute">
                            <UpdateParameters>
                                <asp:Parameter Name="attribute" Type="String" />
                                <asp:Parameter Name="udpValue" Type="String" />
                                <asp:Parameter Name="title" Type="Boolean" />
                                <asp:ControlParameter Name="entity" ControlID="EntityList" PropertyName="SelectedValue" />
                            </UpdateParameters>
                            <SelectParameters>
                                <asp:ControlParameter ControlID="EntityList" Name="entity" PropertyName="SelectedValue"
                                    Type="String" />
                            </SelectParameters>
                            <DeleteParameters>
                                <asp:ControlParameter Name="entity" ControlID="EntityList" Type="String" PropertyName="SelectedValue" />
                                <asp:ControlParameter Name="attribute" ControlID="AliasGridView" Type="String" PropertyName="SelectedValue" />
                            </DeleteParameters>
                        </asp:SqlDataSource>
                        <asp:SqlDataSource runat="server" ID="ChangeAliasSource" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
                            SelectCommand="SELECT alias tblAlias, name tbl FROM model.BTables ORDER BY [alias]"
                            UpdateCommand="UPDATE [model].[BTables] SET alias = @tblAlias WHERE name = @tbl">
                            <UpdateParameters>
                                <asp:ControlParameter Name="tbl" ControlID="ChangeAlias" PropertyName="SelectedValue" />
                                <asp:Parameter Name="tblAlias" Type="String" />
                            </UpdateParameters>
                        </asp:SqlDataSource>
                        <table>
                            <tr>
                                <td valign="top">
                                    <asp:GridView ID="AliasGridView" runat="server" DataSourceID="AliasSource" AutoGenerateColumns="false"
                                        DataKeyNames="attribute" AutoGenerateEditButton="true" ToolTip="Атрибут является пользовательским элементом."
                                        Caption='<h3 style="font: italic bold 17px/30px Georgia, serif;">Список атрибутов:</h3>'
                                        OnRowDeleted="RowDeleted_OnClick">
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:LinkButton Text="Удалить" CommandName="Delete" OnClientClick="return confirm('Если вы удалите атрибут, все ваши данные, связанные с ним, также будут удалены.\nВы уверены, что хотите продолжить ?');"
                                                        runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="udpValue" HeaderText="Псевдоним" />
                                            <asp:BoundField DataField="attribute" HeaderText="Код" ReadOnly="true" />
                                            <asp:CheckBoxField DataField="title" HeaderText="Title" />
                                            <asp:CheckBoxField DataField="flUserMade" HeaderText="Пользовательский" ReadOnly="true"
                                                ItemStyle-HorizontalAlign="Center" />
                                        </Columns>
                                    </asp:GridView>
                                </td>
                                <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                </td>
                                <td valign="top">
                                    <asp:UpdatePanel runat="server">
                                        <ContentTemplate>
                                            <asp:GridView runat="server" ID="ChangeAlias" DataSourceID="ChangeAliasSource" AutoGenerateColumns="false"
                                                AutoGenerateEditButton="true" Caption='<h3 style="font: italic bold 17px/30px Georgia, serif;">Список псевдонимов:</h3>'
                                                DataKeyNames="tbl" AllowPaging="true" PageSize="15">
                                                <Columns>
                                                    <asp:BoundField DataField="tblAlias" HeaderText="Псевдоним" />
                                                    <asp:BoundField DataField="tbl" HeaderText="Код" ReadOnly="true" />
                                                </Columns>
                                            </asp:GridView>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                        </table>
                    </asp:View>
                    <%--Setting UserManagement --%>
                    <asp:View runat="server" ID="userManagement">
                        <User:ViewUser runat="server" ID="users" />
                    </asp:View>
                    <%-- Work very hard with table Permission(Sharing) and "read/create/update/delete" --%>
                    <asp:View ID="SeparationOfAccessRightsView" runat="server">
                        <div style="vertical-align: top">
                            <UserSharing:ViewSharing runat="server" />
                        </div>
                    </asp:View>
                    <asp:View ID="AuditView" runat="server">
                        <div style="vertical-align: top">
                            <Audit:ViewAudit runat="server" />
                        </div>
                    </asp:View>
                </asp:MultiView>
            </td>
        </tr>
    </table>
    <script type="text/javascript" src="../Scripts/AdminUserManagement.js"> </script>
</asp:Content>
