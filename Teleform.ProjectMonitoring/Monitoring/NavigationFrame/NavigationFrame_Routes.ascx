<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationFrame_Routes.ascx.cs" Inherits="Teleform.ProjectMonitoring.NavigationFrame.NavigationFrame_Routes" %>
    


    <div class="action_container row" id="action_container">
        <div class="buttons_template">
            <asp:LinkButton runat="server" ID="BuildRouteButton" PostBackUrl="~/Routes/BuildingRoute.aspx" Text="Построить маршрут" ToolTip="Построить маршрут документа."
                OnClick="BuildRouteButton_Click" CssClass="btn btn-sm btn-default" />
        </div>
    </div>

    <div id="inner_left_menu" class="nav navbar-nav side-nav">
        <div id="docType">
            <asp:SqlDataSource runat="server" ID="DocTypeSource" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
                SelectCommand="SELECT [objID], [name] FROM [_ApplicationType]"></asp:SqlDataSource>            
            <asp:ListView runat="server" ID="DocTypeList" DataSourceID="DocTypeSource"
            OnSelectedIndexChanged="DocTypeList_SelectedIndexChanged" ClientIDMode="Predictable" DataKeyNames="objID" >
            <LayoutTemplate>
                <ul  class="nav navbar-nav side-nav">
                    <asp:PlaceHolder runat="server" ID="itemPlaceholder" />
                </ul>
            </LayoutTemplate>
            <ItemTemplate>
                <li>
                    <asp:LinkButton  runat="server" ID="SelectButton" CommandName="Select" CssClass="templateButton"> <%# Eval("name") %>
                    </asp:LinkButton>
                </li>
            </ItemTemplate>
               <SelectedItemTemplate>

                <li class="active">
                    <asp:LinkButton runat="server" ID="SelectButton" CommandName="Select" CssClass="active"> <%# Eval("name") %>
                    </asp:LinkButton>
                </li>
            </SelectedItemTemplate>
        </asp:ListView>
        </div>
    </div>