<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SettingTheTypesOfObjects.ascx.cs" Inherits="Teleform.ProjectMonitoring.admin.SettingTheTypesOfObjects" %>

<%@ Register TagPrefix="Navigation" TagName="Frame" Src="~/NavigationFrame/admin/NavigationFrame_3.ascx" %>

<Navigation:Frame ID="Frame" runat="server"></Navigation:Frame>

<div class="jumbotron">
    <asp:Table runat="server" ID="TableObjects" CssClass="table table-hover table-striped">
        <asp:TableHeaderRow>
            <asp:TableHeaderCell>
                <span style="font: italic bold 16px/1px Georgia, serif;">Вкл.Тип объекта</span>
            </asp:TableHeaderCell>
            <asp:TableHeaderCell>
                <span style="font: italic bold 16px/1px Georgia, serif;">Шаблоны</span>
            </asp:TableHeaderCell>
        </asp:TableHeaderRow>
    </asp:Table>
</div>
