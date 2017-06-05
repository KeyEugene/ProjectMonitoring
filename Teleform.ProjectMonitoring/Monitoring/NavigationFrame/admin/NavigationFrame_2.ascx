<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationFrame_2.ascx.cs" Inherits="Teleform.ProjectMonitoring.NavigationFrame.admin.NavigationFrame_2" %>

<style>
     .action_container a, .action_container select, .action_container label {
        margin-top: 10px;
    }

    .border_rigth {
        border-right: 1px solid lightgray;
    }
     .action_container select {
        display: inline !important;
    }
</style>

<div class="action_container row" id="action_container">
    <div class="column_left col-md-12 text-center">
        <div style="display:inline-block;">
            <label class="btn btn-sm btn-default" for="<%=ImportUpload.ClientID%>">
                Импорт файл
            <asp:FileUpload ID="ImportUpload" runat="server" EnableViewState="true"
                onchange="$('#filename').text($('input[type=file]').val().split('\\').pop()); return false;" Style="display: none;" />
            </label>
            <label id="filename"></label>
        </div>
        <br />
        <asp:Label runat="server" Text="Тип загрузки" />
        <asp:DropDownList runat="server" ID="ImportModeList" AppendDataBoundItems="true" CssClass="form-control">
            <asp:ListItem Text="полная" Value="0" />
            <asp:ListItem Text="подгрузка" Value="1" />
            <asp:ListItem Text="инкор" Value="2" />
        </asp:DropDownList>
        <asp:Button ID="ImportButton" runat="server" Text="Импорт данных" OnClientClick="new Alert('Идет загрузка данных.\r\nЭто может занять некоторое время.').show();"
            OnClick="ImportButton_Click" CssClass="btn btn-default" />
    </div>
</div>
