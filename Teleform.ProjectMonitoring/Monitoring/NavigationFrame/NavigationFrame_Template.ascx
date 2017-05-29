<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationFrame_Template.ascx.cs" Inherits="Teleform.ProjectMonitoring.NavigationFrame.NavigationFrame_Template" %>

<link href="../Styles/Header_style/NavigationFrame_Template.css" rel="stylesheet" />

<div class="action_container row" id="action_container">
    <div class="column_left col-md-4 text-center">
        <div class="text-center margin_top">
            <asp:LinqDataSource runat="server" ID="EntityListSource" OnSelecting="GetEntities" />
            <asp:Label ID="Label1" runat="server" Text="Тип объекта" Style="font-size: 16px;" />
            <asp:DropDownList runat="server" ID="EntityList" DataSourceID="EntityListSource" CssClass="form-control"
                DataTextField="Name" DataValueField="ID" AutoPostBack="true" OnSelectedIndexChanged="EntityList_SelectedIndexChanged" />
        </div>
    </div>
    <div class="column_center col-md-4 text-center">
        <Dialog:MessageBox runat="server" ID="WarningMessageBox" Caption="Внимание !" Icon="Warning" Buttons="OK">
            <ContentTemplate>
                У вас нет прав для совершение данного действия.
            </ContentTemplate>
        </Dialog:MessageBox>
        <Dialog:MessageBox runat="server" ID="DeleteMessageBox" Caption="Удаление шаблона"
            Icon="Question" Buttons="YesNo" DefaultButton="No" OnClosed="DeteleButton_Click">
            <ContentTemplate>
                Шаблон будет удалён. Продолжить?
            </ContentTemplate>
        </Dialog:MessageBox>
        <Dialog:MessageBox runat="server" ID="TemplateSavedMessageBox" Caption="Шаблон создан"
            Icon="Notification">
            <ContentTemplate>
                Шаблон успешно создан.
            </ContentTemplate>
        </Dialog:MessageBox>
        <div class="buttons_template">
            <asp:LinkButton runat="server" ID="CreateButton" Text="Создать" ToolTip="Создать шаблон."
                OnClick="CreateButton_Click" CssClass="btn btn-sm btn-default" />
            <asp:LinkButton runat="server" ID="EditButton" Text="Изменить" ToolTip="Изменить текущий шаблон."
                OnClick="EditButton_Click" CssClass="btn btn-sm btn-default" />
            <asp:LinkButton runat="server" ID="DownloadButton" Text="Скачать" ToolTip="Скачать тело текущего шаблона."
                OnClick="DownloadButton_Click" CssClass="btn btn-sm btn-default" />
            <asp:LinkButton runat="server" ID="DeleteButton" Text="Удалить" ToolTip="Удалить текущий шаблон."
                OnClick="DeleteMessageBox.Show" CssClass="btn btn-sm btn-default" />
            <asp:LinkButton runat="server" ID="PreviewButton" Text="Показать" ToolTip="Показать текущий шаблон."
                OnClick="ShowPreview_Click" OnClientClick="ShowIgame();" CssClass="btn btn-sm btn-default" />
            <asp:Image ID="Image2" runat="server" Style="visibility: hidden;" ImageUrl="~/images/ajax-loader.gif" />
        </div>
        <script type="text/javascript">
            function ShowIgame() {
                document.getElementById("<%=Image2.ClientID%>").style.visibility = "visible";
            }
        </script>
    </div>

    <div class="column_right col-md-4 text-center">
    </div>
</div>

<div id="inner_left_menu" class="nav navbar-nav side-nav">
    <asp:SqlDataSource runat="server" ID="TemplateListSource" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
        SelectCommand="SELECT
                                [T].[objID],
                                [T].[name],
                                [A].[icon],
                                [A].[code],
                                CASE
		                            WHEN [T].[body] IS NULL THEN 0
		                            ELSE 1
	                            END [body]
                            FROM [model].[R$Template] [T] JOIN [model].[R$TemplateType] [A] ON [A].[objID] = [T].[typeID]
                            WHERE [T].[entityID] = @entityID"
        DeleteCommand="EXEC [model].[R$TemplateDelete] @id">
        <SelectParameters>
            <asp:ControlParameter ControlID="EntityList" PropertyName="SelectedValue" Name="entityID" />
        </SelectParameters>
        <DeleteParameters>
            <asp:ControlParameter ControlID="TemplateList" PropertyName='SelectedDataKey["objID"]'
                Name="id" />
        </DeleteParameters>
    </asp:SqlDataSource>
    <div id="templateList">
        <asp:ListView runat="server" ID="TemplateList" DataSourceID="TemplateListSource"
            OnSelectedIndexChanged="TemplateList_SelectedIndexChanged" ClientIDMode="Predictable"
            DataKeyNames="objID,body,code">
            <LayoutTemplate>
                <ul id="inner_left_menu" class="nav navbar-nav side-nav">
                    <asp:PlaceHolder runat="server" ID="itemPlaceholder" />
                </ul>
            </LayoutTemplate>
            <ItemTemplate>
                <li>
                    <asp:LinkButton runat="server" ID="SelectButton" CommandName="Select" CssClass="templateButton"> <%# Eval("name") %>
                                   <%-- <div class="templateItem" title='<%# Eval("name") %>'>
    <img src='<%# ResolveUrl(Eval("icon").ToString()) %>' />
                                        <%# Eval("name") %></div>--%>
                    </asp:LinkButton>
                </li>
            </ItemTemplate>
            <SelectedItemTemplate>
                <%--<img src="#" alt="Alternate Text" />--%>
                <%--  <div class="templateItem" style="background-color: #CCCCCC; color: #000" title='<%# Eval("name") %>'>

                    <img src='<%# ResolveUrl(Eval("icon").ToString()) %>' />
                    <%# Eval("name") %>
                </div>--%>
                <li class="active">
                    <asp:LinkButton runat="server" ID="SelectButton" CommandName="Select" CssClass="active"> <%# Eval("name") %>
                      <%--<img src='<%# ResolveUrl(Eval("icon").ToString()) %>' />--%>
                    </asp:LinkButton>
                </li>
            </SelectedItemTemplate>
        </asp:ListView>
    </div>
</div>

