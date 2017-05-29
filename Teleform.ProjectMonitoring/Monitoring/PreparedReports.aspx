<%@ Page Title="Подготовленные отчеты" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="PreparedReports.aspx.cs" Inherits="Teleform.ProjectMonitoring.PreparedReports" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var __control, __t;

        function update2() { __control.onchange() }

        function keyup_handler2(o) {
            if (__t != undefined) {
                clearTimeout(__t);
                __t = undefined
            }
            __control = o;
            __t = setTimeout(update2, 150)
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:LinkButton ID="DeleteButon" runat="server" Text="Удалить" OnClick="DeleteButon_Click" />
   <%-- <asp:UpdatePanel runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <ContentTemplate>
            <table>
                <tr>
                    <td>
                        Тип отчета
                    </td>
                    <td>
                        <asp:SqlDataSource ID="TypeDataSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
                            SelectCommand="SELECT [objID], [name] FROM [MimeType] WHERE [enable] = 1 ORDER BY [name]" />
                        <asp:DropDownList ID="TypeList" runat="server" DataSourceID="TypeDataSource" DataTextField="name" OnSelectedIndexChanged="List_SelectedIndexChanged"
                            DataValueField="objID" AutoPostBack="true" AppendDataBoundItems="true" SkinID="List"
                            Style="max-width: 200px"/>
                    </td>
                    <td>
                        Имя сущности
                    </td>
                    <td>
                        <asp:SqlDataSource ID="EntityDataSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
                            SelectCommand="SELECT [tblAlias], [tblID] FROM model.Entity('base') ORDER BY [tblAlias]" />
                        <asp:DropDownList ID="EntityList" runat="server" AppendDataBoundItems="true" SkinID="List" OnSelectedIndexChanged="List_SelectedIndexChanged"
                            AutoPostBack="true" DataSourceID="EntityDataSource" DataTextField="tblAlias"
                            DataValueField="tblID" />
                    </td>
                    <td>
                        Создатель
                    </td>
                    <td>
                        <asp:TextBox ID="PersonBox" runat="server" AutoPostBack="true" OnTextChanged="FilterBox_TextChanged" onkeyup="keyup_handler2(this)" />
                    </td>
                    <td>
                        Имя отчета
                    </td>
                    <td>
                        <asp:TextBox ID="ReportNameBox" runat="server" AutoPostBack="true" OnTextChanged="FilterBox_TextChanged" onkeyup="keyup_handler2(this)" />
                    </td>
                   
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>--%>
    <asp:SqlDataSource ID="PreparedReportsDataSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
      SelectCommand="EXEC [Report].[GetPreparedReports] ''"
    
        DeleteCommand="DELETE FROM [R_Report] WHERE [created] = @created AND [userID] = @userID">
        <DeleteParameters>
            <asp:ControlParameter Name="created" ControlID="PreparedReportsView" PropertyName='SelectedDataKey["created"]' />
            <asp:ControlParameter Name="userID" ControlID="PreparedReportsView" PropertyName='SelectedDataKey["userID"]' />
        </DeleteParameters>
    </asp:SqlDataSource>
    <Dialog:MessageBox ID="DeleteMessageBox" runat="server" SkinID="DeleteObjectMessageBox"
        OnClosed="DeleteMessageBox_Close" />
   <%--<asp:UpdatePanel ID="PreparedReportsPanel" runat="server" ClientIDMode="Static">
        <ContentTemplate>--%>
            <Phoenix:GridView ID="PreparedReportsView" runat="server" DataSourceID="PreparedReportsDataSource"
                DataKeyNames="userID,created,reportName,link">
                <Columns>
                    <asp:TemplateField HeaderText="Имя отчета" SortExpression="reportName">
                        <ItemTemplate>
                            <table class="xxx">
                                <tr>
                                    <td>
                                        <asp:Image ID="Image1" ImageUrl='<%# Eval("mimeIcon") %>' runat="server" Width="24"
                                            Height="24" />
                                    </td>
                                    <td>
                                        <%#Eval("reportName") %>
                                    </td>
                                </tr>
                            </table>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="entityName" HeaderText="Имя сущности" SortExpression="entityName" />
                    <asp:BoundField DataField="created" HeaderText="Время создания" SortExpression="created" />
                    <asp:BoundField DataField="personName" HeaderText="Создатель" SortExpression="personName" />
                    <asp:TemplateField HeaderText="Ссылка">
                        <ItemTemplate>
                            <asp:LinkButton ID="ReportLinkButton" runat="server" Text="Загрузить" CommandArgument='<%#Eval("link") %>'
                                OnClick="ReportLinkButton_Click" />
                           <%-- <asp:LinkButton ID="ReportLinkButton" runat="server" Text="Загрузить" PostBackUrl='<%#GetLoadUrl(Eval("created"),  Eval("userID")) %>'/>--%>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </Phoenix:GridView>
       <%--</ContentTemplate>
    </asp:UpdatePanel>--%>
</asp:Content>
