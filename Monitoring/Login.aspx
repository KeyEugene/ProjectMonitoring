<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Teleform.ProjectMonitoring.LoginPage" %>

<%@ Import Namespace="Teleform.ProjectMonitoring" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="~/Styles/Site.css" rel="stylesheet" type="text/css" />
    <link type="text/css" href="./Styles/selectedRow.css" rel="Stylesheet" />
    <link type="text/css" href="Styles/newObjectStyle.css" rel="Stylesheet" />
    <link type="text/css" href="Styles/detailsStyle.css" rel="Stylesheet" />
    <link type="text/css" href="Styles/buttonsStyle.css" rel="Stylesheet" />
    <link type="text/css" href="Styles/gridviewStyle.css" rel="Stylesheet" />
    <style type="text/css">
        .LoginForm {
            background: #ffffff; /* Old browsers */
            background: -moz-linear-gradient(top, #ffffff 0%, #f6f6f6 47%, #ededed 100%); /* FF3.6+ */
            background: -webkit-gradient(linear, left top, left bottom, color-stop(0%,#ffffff), color-stop(47%,#f6f6f6), color-stop(100%,#ededed)); /* Chrome,Safari4+ */
            background: -webkit-linear-gradient(top, #ffffff 0%,#f6f6f6 47%,#ededed 100%); /* Chrome10+,Safari5.1+ */
            background: -o-linear-gradient(top, #ffffff 0%,#f6f6f6 47%,#ededed 100%); /* Opera 11.10+ */
            background: -ms-linear-gradient(top, #ffffff 0%,#f6f6f6 47%,#ededed 100%); /* IE10+ */
            background: linear-gradient(to bottom, #ffffff 0%,#f6f6f6 47%,#ededed 100%); /* W3C */
            filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#ffffff', endColorstr='#ededed',GradientType=0 ); /* IE6-9 */
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div class="header" style="height: 65px">
                <table width="100%">
                    <tr>
                        <td>
                            <h1>Мониторинг проектов</h1>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="ErrorMessageBox" align="center" runat="server" style="font-size: 120%; margin-top: 50px; color: red; font-weight: 600"></div>
            <asp:Panel runat="server" align="center" Style="position: absolute; top: 30%; width: 100%">
                <fieldset style="background-color: aliceblue; width: 250px; border-radius: 10px" class="LoginForm">
                    <legend>Выполнить вход</legend>
                    <table cellspacing="8">
                        <caption></caption>
                        <tr>
                            <td rowspan="2" style="width: 66px;">
                                <img runat="server" src="~/images/user.png" width="64" height="64" />
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="LoginBox" placeholder="Логин" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox runat="server" ID="PasswordBox" TextMode="Password" placeholder="Пароль" />
                            </td>
                        </tr>
                    </table>
                    <hr />
                    <asp:Button runat="server" ID="TryLoginButton" SkinID="Default" Text="Войти"
                        OnClick="TryLoginButton_Click" />
                </fieldset>
            </asp:Panel>
        </div>
    </form>
</body>
</html>
