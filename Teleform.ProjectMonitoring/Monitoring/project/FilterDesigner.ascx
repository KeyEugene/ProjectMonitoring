<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FilterDesigner.ascx.cs"
    Inherits="Teleform.ProjectMonitoring.FilterDesigner" %>
<%@ Import Namespace="Teleform.Reporting" %>
<%@ Register TagPrefix="Predicate" Namespace="Teleform.Reporting.Web" Assembly="Teleform.Reporting.Web" %>
<script type="text/javascript">
    var __control, __t;

    $(document).ready(function () {
        PredicateDesignerExecutor();
    });

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
<Dialog:MessageBox ID="ErrorMessage" runat="server" Icon="Error">
    <ContentTemplate>
        <asp:Label ID="ErrorLabel" runat="server" />
    </ContentTemplate>
</Dialog:MessageBox>
<Dialog:MessageBox ID="TemplateSavedMessageBox" runat="server" Icon="Notification">
    <ContentTemplate>
        <asp:Label ID="LabelShowNameTemplate" runat="server" />
    </ContentTemplate>
</Dialog:MessageBox>
<asp:LinqDataSource runat="server" ID="AttributeDataSource" OnSelecting="AttributeSource_OnSelecting">
</asp:LinqDataSource>
<asp:Panel ClientIDMode="Static" class="scrollArea" runat="server" onscroll="SetDivPosition()">
    <table width="100%">
        <tr>
            <td style="width: 100px">
                <table width="100%">
                    <tr>
                        <td>
                            <asp:Button runat="server" ID="BackwardButton" Text="Вернуться" OnClick="BackwardButton_Click" />
                        </td>
                        <td>
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                <ContentTemplate>
                                    <asp:TextBox placeholder="Фильтр атрибутов" runat="server" ID="AttributeFilterBox" Width="250px"
                                        AutoPostBack="true" OnTextChanged="AttributeFilterBox_TextChanged" onkeyup="keyup_handler2(this)" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <table width="100%">
                    <tr>
                        <td align="left">
                            <asp:Button runat="server" ID="IncludeAttributeButton" Text="+" OnClick="IncludeAttributeButton_Click" />
                            <asp:Button runat="server" ID="ExcludeAttributeButton" Text="-" OnClick="ExcludeAttributeButton_Click" />
                            <asp:Button runat="server" ID="CopyFilterButton" Text="Копировть" OnClick="CopyFilterButton_Click" BackColor="#DAE2F5" />
                            <asp:Button runat="server" ID="UpButton" Text="↑" OnClick="UpButton_Click" />
                            <asp:Button runat="server" ID="DownButton" Text="↓" OnClick="DownButton_Click" />
                            <asp:Button runat="server" ID="ResetFiltersButton" Text="Сброс" OnClick="ResetFiltersButton_Click" />
                        </td>
                        <td align="right">
                            <asp:Button runat="server" ID="UpdateEntityFilterButton" Text="Сохранить" OnClick="UpdateEntityFilterButton_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td valign="top">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:ListBox runat="server" ID="AttributeList" DataSourceID="AttributeDataSource"
                            DataTextField="Name" DataValueField="ID" SelectionMode="Multiple" Height="100%"
                            Style="min-height: 600px" Width="450" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td valign="top">

                <asp:LinqDataSource runat="server" ID="FieldDataSource" OnSelecting="FieldSource_OnSelecting">
                </asp:LinqDataSource>
                <asp:ListView runat="server" ID="FieldList" DataSourceID="FieldDataSource" OnItemCreated="FieldList_ItemCreated">
                    <LayoutTemplate>
                        <table class="gridviewStyle">
                            <tr>
                                <th style="display: none">
                                </th>
                                <th>
                                    Атрибут
                                </th>
                                <th>
                                    Предикат
                                </th>
                            </tr>
                            <asp:PlaceHolder runat="server" ID="itemPlaceholder" />
                        </table>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr>
                            <td style="display: none">
                                <asp:LinkButton ID="SelectButton" Text="Выбрать" CommandName="Select" runat="server"
                                    BackColor="Green" />
                            </td>
                            <td style="cursor: pointer" onclick="eval($(this).parent().children().eq(0).children().eq(0).attr('href'))">
                                <asp:Label ID="NameLabel" runat="server" Text='<%# (Eval("Attribute") as Teleform.Reporting.Attribute).Name %>' />
                            </td>
                            <td>
                                <asp:TextBox ID="UserPredicateBox" runat="server" Width="200px" TextMode="MultiLine"></asp:TextBox>
                                <Predicate:CompositePredicateControl ID="CompositePredicateControl" Column="#a" CssClass="PredicateControl"
                                    runat="server" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <SelectedItemTemplate>
                        <tr>
                            <td style="background-color: Yellow">
                                <asp:Label ID="NameLabel" runat="server" Text='<%# (Eval("Attribute") as Teleform.Reporting.Attribute).Name %>' />
                            </td>
                            <td style="background-color: Yellow">
                                <asp:TextBox ID="userPredicateBox" runat="server" Width="200px" TextMode="MultiLine"></asp:TextBox>
                                <%-- <Predicate:PredicateControl ID="PredicateControl" Column="#a" CssClass="PredicateControl" runat="server" />--%>
                                <Predicate:CompositePredicateControl ID="CompositePredicateControl" Column="#a" CssClass="PredicateControl"
                                    runat="server" />
                            </td>
                        </tr>
                    </SelectedItemTemplate>
                </asp:ListView>
            </td>
        </tr>
    </table>
</asp:Panel>
<Dialog:MessageBox ID="WarningMessageDialog" runat="server" Icon="Notification">
    <ContentTemplate>
        <asp:Label ID="WarningLabel" runat="server" />
    </ContentTemplate>
</Dialog:MessageBox>
<Dialog:MessageBox ID="ErrorMessageDialog" runat="server" Icon="Error">
    <ContentTemplate>
        <asp:Label ID="ErrorLabel1" runat="server" />
    </ContentTemplate>
</Dialog:MessageBox>
<asp:SqlDataSource ID="TemplateType" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
    SelectCommand="SELECT [name], [code] FROM [model].[R$TemplateType] WHERE [objID] <= 3 ORDER BY [name] DESC">
</asp:SqlDataSource>
<Dialog:Form ID="CopyFilterDialog" runat="server" OnClosed="CopyFilterDialogClose">
    <ContentTemplate>
        <table>
            <tr>
                <td>
                    Имя фильтра
                </td>
                <td>
                    <asp:TextBox ID="InsertNameBox" Width="250px" runat="server" placeholder="обязательно для заполнения" />
                </td>
            </tr>
        </table>
    </ContentTemplate>
    <Buttons>
        <Dialog:ButtonItem Text="Создать" ControlID="ApplyCopyFilterButton" OnClick="ApplyCopyFilterButton_Click" />
        <Dialog:ButtonItem Text="Отмена" ControlID="CancelButton" OnClick="CopyFilterDialog.Close" />
    </Buttons>
</Dialog:Form>
