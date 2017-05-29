<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ListManagement.aspx.cs" Inherits="Monitoring.code_main.ListManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:SqlDataSource ID="TableAttributesDataSource" runat="server" ConnectionString='<%# Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
        SelectCommand="EXEC [GetTableFieldName] @table" UpdateCommand="EXEC [UpdateTableFieldName] @table,@objID,@name"
        InsertCommand="EXEC [InsertTableFieldName] @table,@name">
        <SelectParameters>
            <asp:ControlParameter Name="table" Type="String" ControlID="ListTablesList" PropertyName="SelectedValue" />
        </SelectParameters>
        <UpdateParameters>
            <asp:ControlParameter Name="table" Type="String" ControlID="ListTablesList" PropertyName="SelectedValue" />
        </UpdateParameters>
        <InsertParameters>
            <asp:ControlParameter Name="table" Type="String" ControlID="ListTablesList" PropertyName="SelectedValue" />
        </InsertParameters>
    </asp:SqlDataSource>
    <Dialog:EditorForm runat="server" ID="TableAttributesEditorForm" TargetControlID="TableAttributesGridView"
        AcceptButtonID="AcceptButton" CancelButtonID="CancelButton"
        SkinID="ObjectEditorForm" DataKeyNames="objID,name" Caption="Редактирование объекта">
        <ContentTemplate>
            <table>
                <tr>
                    <td>
                        Наименование
                    </td>
                    <td>
                        <asp:TextBox ID="NameBox" runat="server" Text='<%# Container.Bind("name") %>' />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Buttons>
            <Dialog:ButtonItem ControlID="AcceptButton" Text="Accept" />
            <Dialog:ButtonItem ControlID="CancelButton" Text="Cancel" />
        </Buttons>
    </Dialog:EditorForm>
    <asp:DropDownList ID="ListTablesList" runat="server" AutoPostBack="true">
        <asp:ListItem Text="Статус проекта" Value="_Status" Selected="True" />
        <asp:ListItem Text="Тип работы" Value="_WorkType" />
        <asp:ListItem Text="Роль участника" Value="_AccompliceRole" />
        <asp:ListItem Text="Позиция" Value="_Position" />
        <asp:ListItem Text="Тип документа" Value="_ApplicationType" />
    </asp:DropDownList>
    <asp:Button ID="EditTableAttributeButton" runat="server" Text="Редактировать" CommandName="Edit"
        OnClick="TableAttributesEditorForm.Show" />
    <asp:Button ID="InsertTableAttributeButton" runat="server" Text="Добавить" CommandName="Insert"
        OnClick="TableAttributesEditorForm.Show" />
    <Phoenix:GridView ID="TableAttributesGridView" runat="server" DataSourceID="TableAttributesDataSource" EnableViewState="false"
        DataKeyNames="objID,name" AutoGenerateColumns="false">
        <Columns>
            <asp:BoundField DataField="objID" HeaderText="Номер" />
            <asp:BoundField DataField="name" HeaderText="Наименование" />
        </Columns>
    </Phoenix:GridView>
</asp:Content>
