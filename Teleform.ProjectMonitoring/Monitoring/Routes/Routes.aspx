<%@ Page Title="Маршруты документов" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
 CodeBehind="Routes.aspx.cs" Inherits="Teleform.ProjectMonitoring.Routes.Routes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    
    <script type="text/javascript" src="../Scripts/jquery-ui.min.js"></script> 
    <script type="text/javascript" src="../Scripts/routescripts.js"></script>
    <link href="../Styles/routestyles.css" rel="Stylesheet" />
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server" ViewStateMode="Enabled">
    <div>
        <br />
       <%-- <label>Раздел находится в разработке</label>--%>
        <br />
        <br />
        <asp:LinkButton runat="server" ID="BuildRoute" PostBackUrl="~/Routes/BuildingRoute.aspx" Text="Построить маршрут"></asp:LinkButton>
        <br />
        <br />
        <label>Тип документа: </label>
        <asp:DropDownList runat="server" ID="TypesList" AutoPostBack="true" OnSelectedIndexChanged="TypesList_SelectedIndexChanged"></asp:DropDownList>
        <br />
        <br />
        <asp:MultiView runat="server" ID="WorkPlaces">
            <asp:View ID="View1" runat="server">

                <table>
                    <tr>
                        <td>
                            <!--тут пусто-->
                        </td>
                        <td>
                            <asp:Button runat="server" ID="includeButton" Text="+" OnClick="includeButton_Click" />
                            <asp:Button runat="server" ID="saveButton" Text="Сохранить" OnClick="saveButton_Click" />
                            <asp:Button runat="server" ID="cancelButton" Text="Отмена" OnClick="cancelButton_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <label>Документы</label>
                            <br />
                            <label>Фильтр: </label> 
                            <asp:TextBox runat="server" ID="ApplicationFilterBox" onkeyup="keyup_FilterControl(this, 'MainContent_ApplicationList')"></asp:TextBox>
                            <br />
                            <br />
                            <asp:ListBox runat="server" ID="ApplicationList" SelectionMode="Multiple" Width="300" Height="300"></asp:ListBox>
                        </td>
                    </tr>
                </table>

                <br />
                <asp:PlaceHolder runat="server" ID="TablePlace"></asp:PlaceHolder>
            </asp:View>
            <asp:View ID="View2" runat="server">
                <br />
                <br />
                <asp:PlaceHolder runat="server" ID="EditEvent">
                    <table>
                        <tr>
                            <td>
                                <asp:Label runat="server" ID="lblAppNumber" Text="Номер документа"></asp:Label>
                            </td>
                            <td>
                                <asp:Label runat="server" ID="AppNumberBox"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ID="lblPointName" Text="Инстанция"></asp:Label>
                            </td>
                            <td>
                                <asp:Label runat="server" ID="PointNameBox"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ID="lblState" Text="Состояние"></asp:Label>
                            </td>
                            <td>
                                <asp:Label runat="server" ID="StateBox"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ID="lblDate" Text="Дата планируемая: "></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="DateBox" type="date"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label runat="server" ID="lblDateR" Text="Дата реальная: "></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="DateRBox" ReadOnly="true" type="date"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <br />
                                <asp:Button runat="server" ID="saveStateButton" Text="Сохранить" OnClick="saveStateButton_Click" />
                            </td>
                            <td>
                                <br />
                                <asp:Button runat="server" ID="cancelStateButton" Text="Отмена" OnClick="cancelStateButton_Click" />
                            </td>
                        </tr>
                    </table>
                </asp:PlaceHolder>

            </asp:View>
        </asp:MultiView>
    </div>
</asp:Content>
