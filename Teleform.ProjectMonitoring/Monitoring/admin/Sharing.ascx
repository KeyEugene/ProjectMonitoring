<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Sharing.ascx.cs" Inherits="Teleform.ProjectMonitoring.admin.Sharing" %>
<link href="../Styles/TableView.css" rel="Stylesheet" />
<link href="../Styles/PredicateControl.css" rel="Stylesheet" />
<style type="text/css">
    details[open]
    {
        position: static;
    }
    
    .templateMenu
    {
        padding-left: 10px;
        border-bottom: 1px silver solid;
        vertical-align: middle;
        box-shadow: 0 0.5px 1px #bbb;
    }
    
    .templateMenu tbody tr td
    {
        padding: 5px;
    }
    
    /*.templateMenu tbody tr td:last-child {
                padding: 15px;
            }*/
    
    .linkBtn
    {
        color: black;
    }
    
    .button4
    {
        font-size: 15px;
        font-weight: bold;
        color: black;
        cursor: pointer;
    }
    
    .button4:hover
    {
        text-decoration: underline;
    }
    
    
    .closeTemplateImg
    {
        width: 15px;
        cursor: pointer; /*position: absolute;*/
        margin-top: -8px;
    }
    
    .closeTemplateImg:hover
    {
        transition: all ease-in-out .3s;
        -webkit-transition: all ease-in-out .3s;
        -moz-transition: all ease-in-out .3s;
        -o-transition: all ease-in-out .3s;
        width: 17px;
    }
    
    .pointer
    {
        content: '';
        position: absolute;
        top: 50%;
        right: 100%;
        margin-top: -8px;
        width: 0;
        height: 0;
        border-right: 8px solid lightgrey;
        border-top: 8px solid transparent;
        border-bottom: 8px solid transparent;
    }
    
    .RemoveObjects
    {
        width: 200px;
        background-color: lightgrey;
        height: 60px;
        border-radius: 5px;
        display: block;
        padding: 15px;
        position: absolute;
        top: 130px;
        left: 825px;
    }
    
    .cursorPointer
    {
        cursor: pointer;
    }
    
    .Button_Save
    {
        background-color: #CDCDCD;
        color: white;
        width: 171px;
        height: 25px;
        font-weight: 900;
    }
</style>

<script type="text/javascript">
    var filtered = false;

    function OnOffBoxes(cb) {
        var index = cb.parentElement.cellIndex;
        var rows = cbxR.parentElement.parentElement.parentElement.rows;

        for (var i = 1; i < rows.length; i++) {
            if (!filtered || rows[i].style.display == "table-row")
                rows[i].cells[index].children[0].checked = cb.checked;
        }
    }

    function NameFilter(txt, itemID) {
        var table = document.getElementById(itemID.toString());
        for (var i = 1; i < table.rows.length; i++) {
            var row = table.rows[i];
            if (row.cells[2].innerText.toString().toLowerCase().indexOf(txt.value.toLowerCase()) == -1)
                row.style.display = "none";
            else
                row.style.display = "table-row";
        }
        
        filtered = true;
    }
</script>

<table class="templateMenu">
    <tr>
        <td align="center">
            <asp:Image ID="imageFirst" ImageUrl="~/images/84_lightbulb.png" runat="server" Visible="false" />
        </td>
        <td align="center">
            <asp:Image ID="imageSecond" ImageUrl="~/images/84_lightbulb.png" runat="server" Visible="false" />
        </td>
        <td align="center">
            <asp:Image ImageUrl="~/images/84_lightbulb.png" runat="server" ID="imageThird" Visible="false" />
        </td>
    </tr>
    <tr>
        <th>
            Типы пользователей &nbsp;&nbsp;&nbsp;
        </th>
        <th>
            Пользователи
        </th>
        <th>
            Тип объектов /
            <asp:LinkButton Text="Шаблоны" runat="server" ID="LinkBtnTemplate" OnClick="LinkBtnTemplate_Click"
                CssClass="linkBtn" />
            <asp:LinkButton runat="server" CssClass="CloseTemplate_LinkButton" ID="CloseTemplate_LinkButton"
                OnClick="CloseTemplate_Click">
                <asp:Image ImageUrl="~/images/clear.png" runat="server" ID="ImageCloseTemplate" Visible="false"
                    CssClass="closeTemplateImg" ToolTip="Закрыть шаблоны ?" />
            </asp:LinkButton>
        </th>
    </tr>
    <tr>
        <td align="center">
            <asp:DropDownList runat="server" ID="UserTypeList" AutoPostBack="true" OnSelectedIndexChanged="UserTypeList_IndexChanged" />
        </td>
        <td align="center">
            <asp:DropDownList runat="server" ID="UserList" AutoPostBack="true" OnSelectedIndexChanged="UserList_IndexChanged" />
        </td>
        <td align="center">
            <asp:DropDownList runat="server" ID="EntityList" AutoPostBack="true" OnSelectedIndexChanged="EntityList_IndexChanged" />
        </td>
    </tr>
    <tr>
        <td>
        </td>
        <td>
            <asp:Button Text="Сохранить" runat="server" ID="SaveButton" OnClick="SaveButton_OnClick"
                CssClass="Button_Save" Visible="true" />
            <asp:Button Text="Сохранить шаблоны" runat="server" ID="SaveTemplate" OnClick="SaveTemplate_Click"
                CssClass="Button_Save"  BackColor="#CDCDCD" Visible="false" />
        </td>
        <td>
            <span class="button4" onclick="ShowButton();">Сброс</span>
        </td>
    </tr>
</table>
<div id="containerBtnRemove" class="RemoveObjects" style="display: none;">
    <span class="pointer"></span>
    <asp:Button Text="Сброс объекта" runat="server" ID="BtnResetOneObject" OnClick="BtnResetOneObject_Click"
        OnClientClick="return confirm('Вы уверены, что хотите сбросить настройки для типа объекта?');" />
    <asp:Button Text="Сброс для типов объектов" runat="server" ID="BtnResetALotOfObjects"
        OnClick="BtnResetALotOfObjects_Click" OnClientClick="return confirm('Вы уверены, что хотите сбросить настройки для типы объектов ?');" />
    <asp:Button Text="Сброс шаблонов" runat="server" ID="BtnResetTemplates" OnClick="BtnResetTemplates_Click"
        OnClientClick="return confirm('Вы уверены, что хотите сбросить настройки для шаблонов?');" />
</div>
<br />
<asp:MultiView ActiveViewIndex="0" runat="server" ID="MView">
    <asp:View runat="server" ID="GridV">
        <%--<label>Фильтр по названию: </label>
        <input type="text" id="NameEntityFilter" onkeyup="NameFilter(this, 'GVPermission')" />
        <br />--%>
        <asp:GridView runat="server" CssClass="GVPermission" ID="GVPermission" AutoGenerateColumns="false"
            DataKeyNames="entity" SelectedRowStyle-BackColor="Red" ClientIDMode="Static">
          <%--  <asp:GridView runat="server" CssClass="GVPermission" ID="GVPermission" AutoGenerateColumns="false"
            DataKeyNames="entity" ClientIDMode="Static">--%>
            <Columns>
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <div style="display: none">
                            <asp:Literal Text='<%#  Eval("entity") %>' runat="server" ID="entity" />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="typeAlias" HeaderText="Раздел" ReadOnly="true" />
                <asp:BoundField DataField="entityAlias" HeaderText="Тип объекта" ReadOnly="true" />
                <asp:TemplateField HeaderText="Чтение  <input type='checkbox' id='cbxR' onchange='OnOffBoxes(this)' title='Вкл/Выкл все' />">
                    <ItemTemplate>
                        <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Eval("read") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Создание  <input type='checkbox' id='cbxC' onchange='OnOffBoxes(this)' title='Вкл/Выкл все' />">
                    <ItemTemplate>
                        <asp:CheckBox ID="CheckBox2" runat="server" Checked='<%# Eval("create") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Изменение  <input type='checkbox' id='cbxM' onchange='OnOffBoxes(this)' title='Вкл/Выкл все' />">
                    <ItemTemplate>
                        <asp:CheckBox ID="CheckBox3" runat="server" Checked='<%# Eval("update") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Удаление  <input type='checkbox' id='cbxD' onchange='OnOffBoxes(this)' title='Вкл/Выкл все' />">
                    <ItemTemplate>
                        <asp:CheckBox ID="CheckBox4" runat="server" Checked='<%# Eval("delete") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </asp:View>
    <asp:View runat="server" ID="TableV">
        <div class="templateMenu">
            <Predicate:PredicateBuilder runat="server" ID="PredicateBuilder1" />
        </div>
    </asp:View>
    <asp:View runat="server" ID="TemplateTableView">
        <label>Фильтр по названию: </label>
        <input type="text" id="NameTemplateFilter" onkeyup="NameFilter(this, 'GVTemplate')" />
        <br />
        <asp:GridView runat="server" ID="GVTemplate" CssClass="GVTemplate" AutoGenerateColumns="false"
            DataKeyNames="objID" AlternatingRowStyle-BackColor="#DFE1E1" ClientIDMode="Static" >
            <Columns>
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <div style="display: none">
                            <asp:Literal Text='<%#  Eval("objID") %>' runat="server" ID="objID" />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="entityAlias" HeaderText="Тип объекта" ReadOnly="true" />
                <asp:BoundField DataField="name" HeaderText="Название шаблона" ReadOnly="true" />
                <asp:BoundField DataField="type" HeaderText="Тип" ReadOnly="true" />
                <asp:TemplateField HeaderText="Чтение  <input type='checkbox' id='cbxR' onchange='OnOffBoxes(this)' title='Вкл/Выкл все' />" >
                    <ItemTemplate>
                        <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Eval("read") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:TemplateField HeaderText="Создание  <input type='checkbox' id='cbxC' onchange='OnOffBoxes(this)' title='Вкл/Выкл все' />" >--%>
                <asp:TemplateField HeaderText="Создание  <input type='checkbox' id='cbxC' onchange='OnOffBoxes(this)' title='Вкл/Выкл все' />" Visible = "false" >
                    <ItemTemplate>
                        <asp:CheckBox ID="CheckBox2" runat="server" Checked="false" />
                        <%--<asp:CheckBox ID="CheckBox2" runat="server" Checked='<%# Eval("create") %>' />--%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Изменение  <input type='checkbox' id='cbxM' onchange='OnOffBoxes(this)' title='Вкл/Выкл все' />">
                    <ItemTemplate>
                        <asp:CheckBox ID="CheckBox3" runat="server" Checked='<%# Eval("update") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Удаление   <input type='checkbox' id='cbxD' onchange='OnOffBoxes(this)' title='Вкл/Выкл все' />">
                    <ItemTemplate>
                        <asp:CheckBox ID="CheckBox4" runat="server" Checked='<%# Eval("delete") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </asp:View>
</asp:MultiView>
<script type="text/javascript" src="../Scripts/site.js"></script>
<script type="text/javascript" src="../Scripts/CompositePredicateControl.js"></script>
<script>
    $(document).ready(function () {
        //$(".GVPermission tr").on("mouseover", function () {
        //    $(".GVPermission tr").attr("style", "");

        //    $(this).attr("style", "background: #dfdfdf");
        //});


        $(".GVPermission tr").addClass("cursorPointer");
        $(".GVTemplate tr").addClass("cursorPointer");

        $(".GVPermission tr").on("click", function () {
            $(".GVPermission tr").attr("style", "");

            $(this).attr("style", "background: #FFCC00");
        });

        //$(".GVTemplate tr").on("click", function () {
        //    $(".GVTemplate tr").attr("style", "");

        //    $(this).attr("style", "background: #FFCC00");
        //});
    });

    function ShowButton() {
        console.log("onfocus");
        var divStyle = $("#containerBtnRemove").attr("style");

        if (divStyle == "") {
            $("#containerBtnRemove").attr("style", "display:none;")
            $(".button4").attr("style", "color: black;");
        }
        else {
            $("#containerBtnRemove").attr("style", "")
            $(".button4").attr("style", "color: gray;");
        }
    }

</script>
