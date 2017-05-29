<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UDPSetting.aspx.cs" Inherits="Monitoring.UDPSetting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:SqlDataSource ID="TableDataSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
         SelectCommand="SELECT DISTINCT [tbl],[nameT] FROM model.TableColumns WHERE [tbl] LIKE '[_]%' AND [sType] IN ('date', 'smalldatetime', 'datetime')"/>
    <asp:DropDownList ID="TableList" runat="server" SkinID="List" AppendDataBoundItems="true" AutoPostBack="true" 
                    DataSourceID="TableDataSource" DataTextField="nameT" DataValueField="tbl"/>

    <asp:SqlDataSource ID="DateColumnsDataSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
         SelectCommand="SELECT [col],[nameC] FROM model.TableColumns WHERE [tbl]=@tblName AND [sType] IN ('date', 'smalldatetime', 'datetime')">
         <SelectParameters>
            <asp:ControlParameter Name="tblName" ControlID="TableList" PropertyName="SelectedValue"/>
         </SelectParameters>
    </asp:SqlDataSource>
    <asp:DropDownList ID="DateColumnsList" runat="server" SkinID="List" AppendDataBoundItems="true" AutoPostBack="true" OnDataBinding="DateColumnsList_DataBinding"
                    DataSourceID="DateColumnsDataSource" DataTextField="nameC" DataValueField="col"/>

    <asp:SqlDataSource ID="UDPDataSource" runat="server" ConnectionString='<%$ Connection:Teleform.ProjectMonitoring.HttpApplication.Global.ConnectionString %>'
         SelectCommand="SELECT [tbl],[col],[udp],[udpValue] FROM [model].[TableColumnsUdp] WHERE [tbl] = @tblName AND [col]=@colName"
         InsertCommand="EXEC model.addColumnsUdp @tblName, @colName, @udpValue, @udp"
         UpdateCommand="EXEC model.updateColumnUdp @tblName, @colName, @udpValue, @udp"
         DeleteCommand="EXEC model.dropColumnUdp @tblName, @colName, @udp">
         <SelectParameters>
            <asp:ControlParameter Name="tblName" ControlID="TableList" PropertyName="SelectedValue"/>
            <asp:ControlParameter Name="colName" ControlID="DateColumnsList" PropertyName="SelectedValue"/>
         </SelectParameters>
         <InsertParameters>
            <asp:ControlParameter Name="tblName" ControlID="TableList" PropertyName="SelectedValue"/>
            <asp:ControlParameter Name="colName" ControlID="DateColumnsList" PropertyName="SelectedValue"/>
         </InsertParameters>
         <UpdateParameters>
            <asp:ControlParameter Name="tblName" ControlID="TableList" PropertyName="SelectedValue"/>
            <asp:ControlParameter Name="colName" ControlID="DateColumnsList" PropertyName="SelectedValue"/>
         </UpdateParameters>
         <DeleteParameters>
            <asp:ControlParameter Name="tblName" ControlID="TableList" PropertyName="SelectedValue"/>
            <asp:ControlParameter Name="colName" ControlID="DateColumnsList" PropertyName="SelectedValue"/>
         </DeleteParameters>
    </asp:SqlDataSource>


    <Dialog:EditorForm runat="server" ID="UDPEditorForm" TargetControlID="UDPView"
        AcceptButtonID="AcceptButton" CancelButtonID="CancelButton"
        SkinID="ObjectEditorForm" DataKeyNames="udp,udpValue" Caption="Редактирование свойста">
        <ContentTemplate>
            <table>
                <tr>
                    <td>
                        Наименование
                    </td>
                    <td>
                        <asp:TextBox ID="NameBox" runat="server" Text='<%# Container.Bind("udp") %>' />
                    </td>
                </tr>
                <tr>
                    <td>
                        Значение
                    </td>
                    <td>
                        <asp:TextBox ID="ValueBox" runat="server" Text='<%# Container.Bind("udpValue") %>' />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Buttons>
            <Dialog:ButtonItem ControlID="AcceptButton" Text="Accept" />
            <Dialog:ButtonItem ControlID="CancelButton" Text="Cancel" />
        </Buttons>
    </Dialog:EditorForm>

    <Phoenix:GridView ID="UDPView" runat="server" DataSourceID="UDPDataSource" DataKeyNames="udp,udpValue" EnableViewState="false">
        <Columns>
            <asp:BoundField DataField="udp" HeaderText="Имя свойства"/>
            <asp:BoundField DataField="udpValue" HeaderText="Значение свойства"/>
        </Columns>
    </Phoenix:GridView>
    <asp:LinkButton ID="InsertUDPButton" runat="server" Text="Добавить" OnClick="InsertUDPButton_Click"/>
    <asp:LinkButton ID="EditUDPButton" runat="server" Text="Редактировать" OnClick="EditUDPButton_Click"/>
    <asp:LinkButton ID="DeleteUDPButton" runat="server" Text="Удалить" OnClick="DeleteUDPButton_Click"/>
</asp:Content>
