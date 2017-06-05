<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SettingAttributesOfEntities.ascx.cs" Inherits="Teleform.ProjectMonitoring.admin.SettingAttributesOfEntities" %>

<%@ Register TagPrefix="Navigation" TagName="Frame" Src="~/NavigationFrame/admin/NavigationFrame_4.ascx" %>

<Navigation:Frame ID="Frame" runat="server"></Navigation:Frame>

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

<div class="column_left col-md-12 text-center" id="entity_controls">
    <asp:DropDownList ID="EntityList" runat="server" AutoPostBack="true" DataSourceID="EntitySource"
        DataTextField="name" DataValueField="entity" CssClass="form-control" />
    <br />
    <asp:Button Text="Добавление атрибута" ID="ButtonAdd" OnClick="AddAttriabute.Show"
        runat="server" CssClass="btn btn-default" />
    <asp:Button Text="Синхронизировать" runat="server" OnClick="Synchronize" CssClass="btn btn-default" />
</div>
<script>
    //переносим вручную тк. приходится много тащить с собой контролов не относящихся к панеле действия
    var action_container = $("#action_container");
    var entity_controls = $("#entity_controls");
    entity_controls.appendTo(action_container);
</script>
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
                OnRowDeleted="RowDeleted_OnClick" CssClass="table table-bordered table-hover">
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
                        DataKeyNames="tbl" AllowPaging="true" PageSize="15" CssClass="table table-bordered table-hover">
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
