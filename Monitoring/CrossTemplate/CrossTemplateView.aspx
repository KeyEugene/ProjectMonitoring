<%@ Page Title="Перекрестное представление" Language="C#" AutoEventWireup="true" CodeBehind="CrossTemplateView.aspx.cs" MasterPageFile="~/Site.Master"
    Inherits="Teleform.ProjectMonitoring.CrossTemplate.CrossTemplateView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../Styles/PredicateControl.css" rel="Stylesheet" />
    <script src="../Scripts/jquery-2.1.3.min.js"></script>
    <style>
        .crossTable td:first-child {
            background: #424242;
            height: 20px;
            color: White;
            padding: 5px;
            font-weight: 600;
            text-align: center;
        }

        .crossTable td:hover {
            cursor: pointer;
            border: 1px solid black;
            padding: 4px 2px;
            box-shadow: 0px 0px 30px 0px rgba(0,0,0,0.95);
        }

        .HeaderStyle tr th {
            background: #424242;
            height: 20px;
            color: White;
            padding: 5px;
            font-weight: 600;
        }

        .ImageStyleCross {
            position: absolute;
            left: 800px;
            top: 300px;
            width: 100px;
        }

        .PositionButton {
            margin: 15px;
        }

        .headerFixed {
            /*position: absolute;*/
            top: 360px;
        }

        .containerDiv {
            background: #424242;
            color: white;
            position: fixed;
            padding: 5px 0px 5px 0px;
            font-weight: 600;
            text-align: center;
        }
    </style>
    <script type="text/javascript">
        ; $(document).ready(function () {

            $("[id*=gv]").addClass("crossTable");;

            /*Fixed header*/
            //$(".main").scroll(function () {
            //    var top = $(this).scrollTop();

            //    if (top >= 90) {

            //        var haveFixedTable = document.getElementById("fixedHeader");
            //        if (haveFixedTable !== null) {
            //            //$('#fixedHeader').css({
            //            //    'left': $(this).scrollLeft() + 15
            //            //    //Why this 15, because in the CSS, we have set left 15, so as we scroll, we would want this to remain at 15px left
            //            //});
            //            return false;
            //        }

            //        var table = $("[id*=gv]");
            //        var cloneTable = table.clone();
            //        cloneTable.attr("id", "fixedHeader");
            //        var tmpRow = cloneTable.find("tr");
            //        var headRow = tmpRow[0];

            //        var newTableWithHeaderRow = cloneTable.find("tr").remove().end().append(headRow);
            //        newTableWithHeaderRow.addClass("headerFixed");
            //        newTableWithHeaderRow.insertBefore("[id*=gv]");

            //        SetWidthTh();
            //    } else {
            //        $("#fixedHeader").remove();
            //    }

            //    function SetWidthATh() {
            //        newTableWithHeaderRow.find("th").each(function (index) {
            //            $(this).css("width", table.find("th").eq(index).outerWidth() + "px");
            //        });
            //    }
            //});
            /* End, fixed header */

            /*Click to the cell and show name column*/
            $(".crossTable td").click(function (index) {
                var mainTd = $(this);

                var td = mainTd.parent().find("td").eq(0);
                var th = $("[id*=gv]").find("th").eq(td.parent().find("td").index(mainTd));
                var widthHorizintalCell = td.width();
                var heightVerticalCell = th.height();

                var offset = $(this).offset();
                var width = $(this).width();
                var height = $(this).height();

                var styleTop =
                    "style = 'top:" + (offset.top - heightVerticalCell - 10) + "px; left: " + offset.left + "px; width:" + (width + 5) + "px; height: " + heightVerticalCell + "px;'";
                var styleLeft =
                    "style=' top:" + offset.top + "px; left: " + (offset.left - widthHorizintalCell) + "px; width:" + widthHorizintalCell + "px; height: " + height + "px;'";
                $("<div class='containerDiv'" + styleTop + " >" + th.html() + "</div>").prependTo(this);
                $("<div class='containerDiv'" + styleLeft + ">" + td.html() + "</div>").prependTo(this);
            });

            $(".crossTable td").mouseout(function () {
                var arrayObject = document.getElementsByClassName("containerDiv");
                if (arrayObject !== null)
                    for (var i = 0; i < arrayObject.length; i++)
                        arrayObject[i].remove();
            });
            /*End, show cell*/
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Literal ID="MainEntityLiteral" Text="Типы объектов:" runat="server" />
    <asp:DropDownList runat="server" ID="EntityList" AutoPostBack="true" OnSelectedIndexChanged="EntityList_OnSelectedIndex" />
    &nbsp;&nbsp;
    <asp:Literal ID="Literal1" Text="Шаблоны :" runat="server" />
    <asp:DropDownList AutoPostBack="true" ID="TemplateList" runat="server" OnSelectedIndexChanged="TemplateList_SelectedIndexChanged" />

    <br />
    <div class="PositionButton">
        <asp:Button Text="Отчет" runat="server" ID="ReportButton" OnClick="ReportButton_Click" />
        <asp:Button runat="server" Text="Экспорт в Excel" ID="toExcelButton" OnClick="toExcelButton_Click"
            BackColor="#CDCDCD" />
        <asp:Button Text="Конструктор" runat="server" ID="ConstructorButton" OnClick="ConstructorButton_Click" />
    </div>
    <asp:MultiView ActiveViewIndex="0" runat="server" ID="multiView" OnActiveViewChanged="multiView_ActiveViewChanged">
        <asp:View runat="server">
            <asp:Image ID="Image4" runat="server" ImageUrl="~/images/crossTemplate.png" CssClass="ImageStyleCross" />
        </asp:View>
        <asp:View runat="server">
            <Template:CrossReportTemplateDesigner ID="CrossTemplate" runat="server" ShowCloseButton="true" ShowSaveButton="true" />
        </asp:View>
        <asp:View runat="server">
            <asp:GridView runat="server" ID="gv" ViewStateMode="Disabled"></asp:GridView>
        </asp:View>
    </asp:MultiView>

    <Dialog:MessageBox runat="server" ID="WarningMessageBox" Caption="Внимание !" Icon="Warning" Buttons="OK">
        <ContentTemplate>
            У вас нет прав для совершение данного действия.
        </ContentTemplate>
    </Dialog:MessageBox>

    <script type="text/javascript" src="../Scripts/CompositePredicateControl.js"></script>
</asp:Content>

