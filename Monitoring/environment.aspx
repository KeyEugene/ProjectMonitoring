<%@ Page Title="Система" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="environment.aspx.cs" Inherits="Teleform.ProjectMonitoring.EnvironmentPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    
    
    <script type="text/javascript">
        $(document).ready(function () {
            (function (d) { d.each(["backgroundColor", "borderBottomColor", "borderLeftColor", "borderRightColor", "borderTopColor", "color", "outlineColor"], function (f, e) { d.fx.step[e] = function (g) { if (!g.colorInit) { g.start = c(g.elem, e); g.end = b(g.end); g.colorInit = true } g.elem.style[e] = "rgb(" + [Math.max(Math.min(parseInt((g.pos * (g.end[0] - g.start[0])) + g.start[0]), 255), 0), Math.max(Math.min(parseInt((g.pos * (g.end[1] - g.start[1])) + g.start[1]), 255), 0), Math.max(Math.min(parseInt((g.pos * (g.end[2] - g.start[2])) + g.start[2]), 255), 0)].join(",") + ")" } }); function b(f) { var e; if (f && f.constructor == Array && f.length == 3) { return f } if (e = /rgb\(\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*\)/.exec(f)) { return [parseInt(e[1]), parseInt(e[2]), parseInt(e[3])] } if (e = /rgb\(\s*([0-9]+(?:\.[0-9]+)?)\%\s*,\s*([0-9]+(?:\.[0-9]+)?)\%\s*,\s*([0-9]+(?:\.[0-9]+)?)\%\s*\)/.exec(f)) { return [parseFloat(e[1]) * 2.55, parseFloat(e[2]) * 2.55, parseFloat(e[3]) * 2.55] } if (e = /#([a-fA-F0-9]{2})([a-fA-F0-9]{2})([a-fA-F0-9]{2})/.exec(f)) { return [parseInt(e[1], 16), parseInt(e[2], 16), parseInt(e[3], 16)] } if (e = /#([a-fA-F0-9])([a-fA-F0-9])([a-fA-F0-9])/.exec(f)) { return [parseInt(e[1] + e[1], 16), parseInt(e[2] + e[2], 16), parseInt(e[3] + e[3], 16)] } if (e = /rgba\(0, 0, 0, 0\)/.exec(f)) { return a.transparent } return a[d.trim(f).toLowerCase()] } function c(g, e) { var f; do { f = d.css(g, e); if (f != "" && f != "transparent" || d.nodeName(g, "body")) { break } e = "backgroundColor" } while (g = g.parentNode); return b(f) } var a = { aqua: [0, 255, 255], azure: [240, 255, 255], beige: [245, 245, 220], black: [0, 0, 0], blue: [0, 0, 255], brown: [165, 42, 42], cyan: [0, 255, 255], darkblue: [0, 0, 139], darkcyan: [0, 139, 139], darkgrey: [169, 169, 169], darkgreen: [0, 100, 0], darkkhaki: [189, 183, 107], darkmagenta: [139, 0, 139], darkolivegreen: [85, 107, 47], darkorange: [255, 140, 0], darkorchid: [153, 50, 204], darkred: [139, 0, 0], darksalmon: [233, 150, 122], darkviolet: [148, 0, 211], fuchsia: [255, 0, 255], gold: [255, 215, 0], green: [0, 128, 0], indigo: [75, 0, 130], khaki: [240, 230, 140], lightblue: [173, 216, 230], lightcyan: [224, 255, 255], lightgreen: [144, 238, 144], lightgrey: [211, 211, 211], lightpink: [255, 182, 193], lightyellow: [255, 255, 224], lime: [0, 255, 0], magenta: [255, 0, 255], maroon: [128, 0, 0], navy: [0, 0, 128], olive: [128, 128, 0], orange: [255, 165, 0], pink: [255, 192, 203], purple: [128, 0, 128], violet: [128, 0, 128], red: [255, 0, 0], silver: [192, 192, 192], white: [255, 255, 255], yellow: [255, 255, 0], transparent: [255, 255, 255] } })(jQuery);

            var items = $("div.Card"), i = 0;

            /*function showMenuItem() {
            setTimeout(function () {
            if (i < items.length) {
            var item = items.eq(i++);
            item.css("visibility", "visible");
            item.fadeIn(300, showMenuItem);
            }
            }, 50)
            };
        
            showMenuItem();*/


            var timer = setInterval(function () {
                if (i < items.length) {
                    var item = items.eq(i++);
                    item.css("visibility", "visible");
                    item.fadeIn("Slow");
                    console.info(i);
                }
                else clearInterval(timer);
            }, 50);

            var defaultColor = [];

            $(".Card").hover(function () {
                defaultColor[this] = $(this).css("backgroundColor");
                $(this).animate({ "backgroundColor": "#87CEFA" }, 50);
            }, function () {
                $(this).animate({ "backgroundColor": defaultColor[this] })
            })
        })
    </script>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:LinkButton ID="A1" runat="server" EnableViewState="false" PostBackUrl="~/EntityListAttributeView.aspx" >
        <div class="Card" style="background-color: #ff9000">
            <h3>Функционал АРМ</h3>
            <asp:Image ID="Image1" runat="server" ImageUrl="~/images/monitoring.png" />
        </div>
    </asp:LinkButton>

    <%--<asp:LinkButton id="LinkButton1" runat="server" enableviewstate="false" PostBackUrl="~/templates.aspx">
<div class="Card" style="background-color: rgb(152, 163, 201)">
<h3>Шаблоны</h3>
<asp:Image ID="Image6" runat="server" ImageUrl="~/images/edit.png" />
</div>
</asp:LinkButton>--%>

   <%-- <asp:LinkButton ID="LinkButton2" runat="server" EnableViewState="false" PostBackUrl="~/Templates/TemplateManager.aspx">
        <div class="Card" style="background-color: rgb(152, 163, 201)">
            <h3>Шаблоны</h3>
            <asp:Image ID="Image7" runat="server" ImageUrl="~/images/edit.png" />
        </div>
    </asp:LinkButton>--%>

     <asp:LinkButton ID="Templates" runat="server" EnableViewState="false" PostBackUrl="~/Templates/TemplateManager.aspx">
        <div class="Card" style="background-color: rgb(152, 163, 201)">
            <h3>Шаблоны</h3>
            <asp:Image ID="Image7" runat="server" ImageUrl="~/images/edit.png" />
        </div>
    </asp:LinkButton>



       <asp:LinkButton ID="Reports" runat="server" EnableViewState="false"  PostBackUrl="~/Reporting/Reports.aspx">
        <div class="Card" style="background-color: rgb(209, 151, 126)">
            <h3>Специальные отчеты</h3>
            <asp:Image ID="Image6" runat="server" ImageUrl="~/images/crossTemplate.png" Width="52px" Height="52px" />
        </div>
    </asp:LinkButton>





    <asp:LinkButton ID="Routes" runat="server" EnableViewState="false" PostBackUrl="~/Routes/Routes.aspx">
        <div class="Card" style="background-color: rgb(120, 83, 228)">
            <h3>Маршруты документов</h3>
            <asp:Image ID="Image666" runat="server" ImageUrl="~/images/routes.gif" />
        </div>
    </asp:LinkButton>

    <asp:LinkButton ID="A2" runat="server" EnableViewState="false" PostBackUrl="~/events.aspx">
        <div class="Card" style="background-color: rgb(186, 88, 113)">
            <h3>Уведомления</h3>
            <asp:Image ID="Image2" runat="server" ImageUrl="~/images/trumpet.png" />
        </div>
    </asp:LinkButton>

    <asp:LinkButton ID="SettingButton" runat="server" EnableViewState="false" PostBackUrl="~/Settings.aspx">
        <div class="Card" style="background-color: #1e90ff">
            <h3>Личные настройки</h3>
            <asp:Image ID="Image3" runat="server" ImageUrl="~/images/user-new.png" />
        </div>
    </asp:LinkButton>

    <%--<div class="Card" style="background-color: #B26B87">
<h3>Избранное</h3>
<asp:Image ID="Image4" runat="server" ImageUrl="~/images/favorites.png" />
</div>--%>

    <asp:LinkButton ID="A5" runat="server" EnableViewState="false" PostBackUrl="~/admin/administration.aspx">
        <div class="Card" style="background-color: rgb(57, 171, 62)">
            <h3>Администрирование</h3>
            <asp:Image ID="Image5" runat="server" ImageUrl="~/images/tool.png" />
        </div>
    </asp:LinkButton>

    <asp:LinkButton ID="A6" runat="server" EnableViewState="false" >
        <div class="Card" style="background-color: rgb(215, 210, 63)">
            <h3>Управление занятиями</h3>
            <asp:Image ID="Image9" runat="server" ImageUrl="~/images/reports.png" />
        </div>
    </asp:LinkButton>

    <%--<asp:LinkButton ID="PreparedReportButton" runat="server" Enabledviewstate="false" PostBackUrl="~/PreparedReports.aspx">
<div class="Card" style="background-color: rgb(205, 127, 50)">
<h3>Подготовленные отчеты</h3>
<asp:Image ID="Image7" runat="server" ImageUrl="~/images/reports.png" />
</div>
</asp:LinkButton>--%>
</asp:Content>
