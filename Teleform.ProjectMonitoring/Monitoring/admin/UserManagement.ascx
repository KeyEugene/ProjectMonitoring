<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserManagement.ascx.cs" Inherits="Teleform.ProjectMonitoring.admin.UserManagement" %>

<%@ Register TagPrefix="Navigation" TagName="Frame" Src="~/NavigationFrame/admin/NavigationFrame_5.ascx" %>

<Navigation:Frame ID="Frame" runat="server"></Navigation:Frame>

<style>
   .dialog_div {
        width: 30%;
    }

    #button_margin {
        margin-bottom: 10px;
    }
</style>


<script type="text/javascript">
    function filterPersons(txt) {
        var list = document.getElementById('DDLperson');
        for (var i = 1; i < list.length; i++) {
            var option = list.options[i];
            if (option.innerText.toString().toLowerCase().indexOf(txt.value.toLowerCase()) == -1)
                option.style.display = "none";
            else
                option.style.display = "block";
        }
    }
</script>

<asp:TextBox runat="server" ID="ObjIDTB" Style="display: none;" />
<div class="table-responsive">
    <asp:PlaceHolder runat="server" ID="ph" />
</div>
<div id="dialog_div" class="panel panel-primary dialog_div" runat="server" >
    <div class="panel-heading">
        <h3 class="panel-title"><i class="fa fa-long-arrow-right"></i>Создание\Редактирование пользователя</h3>
    </div>
    <span id="validationText" style="color: red;"></span>
    <asp:Label ID="validText" runat="server" Style="color: Red;" />
    <asp:Table runat="server" ID="UMdialogTable" CssClass="table table-hover">
        <asp:TableRow>
            <asp:TableHeaderCell> Персоналия</asp:TableHeaderCell>
            <asp:TableCell>
                <label>Фильтр: </label>
                <input type="text" onkeyup="filterPersons(this)" />
                <br />
                <asp:DropDownList runat="server" ID="DDLperson" ClientIDMode="Static" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableHeaderCell> Тип</asp:TableHeaderCell>
            <asp:TableCell>
                <asp:DropDownList runat="server" ID="DDLtype" CssClass="DDLtypeN" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableHeaderCell> Логин</asp:TableHeaderCell>
            <asp:TableCell>

                <asp:TextBox runat="server" ID="textBoxLogin" placeholder="Введите логин..." CssClass="textBoxLoginN" ViewStateMode="Disabled" EnableViewState="false" />
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableHeaderCell> Пароль</asp:TableHeaderCell><asp:TableCell>
                <asp:TextBox runat="server" ID="textBoxPwd" placeholder="Введите пароль..." CssClass="textBoxPwdn" ViewStateMode="Disabled" EnableViewState="false" />
                <span id="showPassword" onclick="showPwd(this);">○</span>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableHeaderCell> Блокирован</asp:TableHeaderCell><asp:TableCell>
                <asp:CheckBox ID="checkBoxDisable" runat="server" />
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <div id="button_margin">
        <asp:Button Text="Сохранить" runat="server" ID="Save" OnClick="Save_Click" OnClientClick="return validationDialogTable();" ViewStateMode="Disabled"
            CssClass="btn btn-default" EnableViewState="false" />
    </div>
</div>

