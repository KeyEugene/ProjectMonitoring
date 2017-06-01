<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationFrame_EntityListAttributeView.ascx.cs" Inherits="Teleform.ProjectMonitoring.NavigationFrame.NavigationFrame_EntityListAttributeView" %>

<%@ Register Src="~/NavigationDialogView/NavigationDialog.ascx" TagPrefix="Project" TagName="NavigationDialog" %>

<style>
    .action_container input, .action_container a, .action_container select, .action_container label {
        margin-top: 10px;
    }

    .action_container select {
        display: inline !important;
    }

    .border_rigth {
        border-right: 1px solid lightgray;
    }
</style>

<div class="action_container row" id="action_container">
    <div class="col-lg-3 text-center border_rigth">
        <div class="margin-top ">
            <asp:LinkButton ID="InsertInstance" CssClass="btn btn-sm btn-default" runat="server" Text="Добавить" OnClick="InsertInstance_Click" />
            <asp:LinkButton ID="DeleteInstance" CssClass="btn btn-sm btn-danger" runat="server" Text="Удалить" OnClick="DeleteInstance_Click" />
            <br />
            <div>
                <asp:CheckBox runat="server" ID="IsEditModeCheckBox" OnCheckedChanged="IsEditModeCheckBox_CheckedChanged" AutoPostBack="true" ToolTip="Включить/Отключить редактирование объектов" />
                <asp:LinkButton ID="SaveObjects" CssClass="btn btn-sm btn-default" runat="server" Text="Сохранить измененные объекты" OnClick="SaveObjects_OnClick" ToolTip="Сохранить объекты" />
                <script>
                    //Дизеблим кнопку, что бы пользователь нажимал ее только когда включен режим редактирования таблицы
                    var checkbox = $("[id*=IsEditModeCheckBox]").is(':checked');
                    if (checkbox) {
                        $("[id*=SaveObjects]").removeClass("disabled");
                    } else {
                        $("[id*=SaveObjects]").addClass("disabled");
                    }
                </script>
            </div>
        </div>
    </div>
    <div class="col-lg-3 text-center border_rigth">
        <asp:SqlDataSource ID="TemplateListSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
            SelectCommand="EXEC [model].[R$TemplateGetTableBased] @entityID">
            <SelectParameters>
                <asp:QueryStringParameter Name="entityID" DbType="Int32" QueryStringField="entity" DefaultValue="-1" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:DropDownList ID="TemplateList" runat="server" AutoPostBack="true" DataValueField="Value" DataTextField="Text"
            OnSelectedIndexChanged="TemplateList_SelectedIndexChanged" AppendDataBoundItems="true" SkinID="List" />
        <div class="form-inline">
            <asp:Button ID="TemplateConstructorButton" CssClass="btn btn-sm btn-default" runat="server" Text="Редактировать шаблон" OnLoad="TemplateConstructorButton_Load" OnClick="TemplateConstructorButton_Click" />
            <asp:LinkButton ID="ToGroupReportButton" CssClass="btn btn-sm btn-default" runat="server" Text="Создать шаблон" OnClick="ToGroupReportButton_Click" />
            <script>
                //Из-за SkinID="List" не получается применить css class form-control
                $("[id*=TemplateList]").addClass("form-control btn-sm");
            </script>
        </div>
    </div>

    <div class="col-lg-3 text-center border_rigth">
        <div class="form-inline">
            <asp:SqlDataSource ID="FilterSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
                SelectCommand="SELECT [objID], [name] FROM [model].[R$EntityFilter] WHERE [entityID] = @entityID AND [userID] = @userID ORDER BY [name]">
                <SelectParameters>
                    <asp:QueryStringParameter Name="entityID" DbType="Int32" QueryStringField="entity" />
                    <asp:SessionParameter Name="userID" DbType="String" SessionField="SystemUser.objID" DefaultValue="0" />
                </SelectParameters>
            </asp:SqlDataSource>
            <asp:DropDownList ID="FilterList" runat="server" AutoPostBack="true" DataTextField="name" class="form-control" SkinID="List" DataValueField="objID"
                OnSelectedIndexChanged="FilterList_SelectedIndexChanged" DataSourceID="FilterSource" AppendDataBoundItems="true" />
            <asp:Button ID="GoToFilterDesignerButton" CssClass="btn btn-sm btn-default" runat="server" Text="Редактировать фильтр" OnClick="GoToFilterDesignerButton_Click" />
            <script>
                //Из-за SkinID="List" не получается применить css class form-control
                $("[id*=FilterList]").addClass("form-control btn-sm");
            </script>
        </div>
        <div class="btn-group">
            <asp:LinkButton ID="ResetAllFilters" CssClass="btn btn-sm btn-warning" runat="server" Text="Сброс фильтров" OnClick="ResetAllFilters_OnClick" ToolTip="Сбрасывает все фильтры." />
            <asp:LinkButton ID="ResetAllSortings" CssClass="btn btn-sm btn-warning" runat="server" Text="Сброс сортировок" OnClick="ResetAllSortings_OnClick" ToolTip="Сбрасывает все сортировки." />
        </div>
    </div>
    <div class="col-lg-3 text-center  border_rigth">
        <asp:CheckBox runat="server" Title="создать отчет Excel, без учета оперативных фильтров" ID="IsNeedAllInstances" Style="position: relative; top: 4px; left: 0px" />
        <asp:LinkButton ID="ExcelReportButton" CssClass="btn btn-sm btn-default" runat="server" Text="Экспорт в Excel" EnableViewState="false" OnClick="CreateExcelReportButton_Click" />
        <%--<label runat="server" style="cursor: pointer; color: blue;" onclick="ShowImportDialog()" id="ImportReportButton">Импорт файл:</label>--%>
        <label class="btn btn-sm btn-default" for="<%=ImportFileUpload.ClientID%>">
            Импорт файл
            <asp:FileUpload ID="ImportFileUpload" runat="server" onchange="$('[id*=LoadImportFile]').click(); return false;" Style="display: none;" />
        </label>
        <asp:Button ID="LoadImportFile" runat="server" Text="Загрузить файл" OnClick="LoadImportFile_Click" Style="display: none;" />
    </div>
</div>

<div id="inner_left_menu" class="nav navbar-nav side-nav">
    <Project:NavigationDialog runat="server" ID="navigationDialog" EnableViewState="false" ViewStateMode="Disabled" />
</div>

