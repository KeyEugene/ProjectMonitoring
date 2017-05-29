<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorPage2.aspx.cs" Inherits="Teleform.ProjectMonitoring.ErrorPage2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .styleImage
        {
            position: absolute;
            left: 50%;
            top: 50%;
            margin-left: -285px;
            margin-top: -190px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <br />
        <br />
        <h4>
            <p>
                <asp:Label ForeColor="Red" Font-Bold="true" ID="MessageLabel" runat="server" TextMode="MultiLine"></asp:Label>
                ..</p>
            <p>
                Для продолжения работы с сайтом Вы можете перейти на:</p>
        </h4>
        <ul>
            <asp:LinkButton ID="MainPage" Text="Главную страницу сайта." runat="server" OnClick="MainPage_OnClick" />
            <br />
            <asp:LinkButton ID="Monitoring" Text="Мониторинг" runat="server" OnClick="Monitoring_OnClick" />
            <h4>
                <p>
                    ... или вернуться
                </p>
            </h4>
            <input type="button" onclick="window.history.back();" value="Назад" />
            <br /><br />
            <asp:Button ID="LogoutButton" runat="server" Text="Выход" OnClick="LogoutButton_Click" />
            <h4>
                <p>
                    ... если есть затруднения, обратитесь к разработчикам в ООО «Телеформ»
                </p>
            </h4>
        </ul>
        <%-- <img src="404.jpg" alt="Page Not Found (404)." style="position: absolute; left: 50%;
            top: 50%; margin-left: -285px; margin-top: -190px;">
        <asp:Image ImageUrl="404.jpg" runat="server" CssClass="styleImage" />--%>
        <asp:TextBox ID="ErrorTextBox" runat="server" TextMode="MultiLine"></asp:TextBox>
    </div>
    </form>
</body>
</html>
