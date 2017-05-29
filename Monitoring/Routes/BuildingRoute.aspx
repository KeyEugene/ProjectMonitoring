<%@ Page Title="Построить маршрут" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="BuildingRoute.aspx.cs" Inherits="Teleform.ProjectMonitoring.Routes.BuildingRoute" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="../Scripts/jquery-2.1.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/routescripts.js"></script>
    <link href="../Styles/BuildingRouteStyle.css" rel="Stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <br />
        <%--<label>Раздел находится в разработке</label>--%>
        <br />
        <br />
        <label>Тип документа: </label>
        <asp:DropDownList runat="server" ID="TypesList" AutoPostBack="true" OnSelectedIndexChanged="TypesList_SelectedIndexChanged">
        </asp:DropDownList>
        <br />
        <br />
        <asp:CheckBox runat="server" ID="cb" Visible="false" />
        <asp:MultiView runat="server" ID="MView">
            <asp:View ID="View1" runat="server">
                <table class="container">
                    <tr>
                        <td align="center">
                            <br />
                            <label>Инстанции</label>
                            <br />
                            <label>Фильтр:</label>
                            <asp:TextBox runat="server" ID="InstanceFilterBox" onkeyup="keyup_FilterControl(this, 'MainContent_InstanceList')"></asp:TextBox>
                            <br />
                            <br />
                        </td>
                        <td align="center">
                            <asp:Button runat="server" ID="includeButton" Text="+" OnClick="includeButton_Click" />
                            <asp:Button runat="server" ID="excludeButton" Text="-" OnClick="excludeButton_Click" />
                            <asp:Button runat="server" ID="upButton" Text="↑" OnClick="upButton_Click" />
                            <asp:Button runat="server" ID="downButton" Text="↓" OnClick="downButton_Click" />
                            <asp:Button runat="server" ID="saveButton" Text="Сохранить" OnClick="saveButton_Click" />
                            <asp:Button runat="server" ID="cancelButton" Text="Отмена" OnClick="cancelButton_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:ListBox runat="server" ID="InstanceList" Width="450" Height="450" SelectionMode="Multiple">
                            </asp:ListBox>
                        </td>
                        <td align="center">
                            <asp:PlaceHolder runat="server" ID="WorkPlace"></asp:PlaceHolder>
                        </td>
                    </tr>
                </table>
            </asp:View>
        </asp:MultiView>
    </div>
</asp:Content>
