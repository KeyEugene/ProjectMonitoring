<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EntityDropDownList.ascx.cs"
    Inherits="Teleform.ProjectMonitoring.EntityDropDownList" %>


<style>
    .openChild {
        display: inline-block;
        background: #999;
        margin-left: 5px;
        font-size: 1.2em;
        position: absolute;
        left: 85%;
        cursor: pointer;
        width: 24px;
        color: blue;
        height: 19px;
        line-height: 19px;
        z-index: 1;
    }

    .containerEntityDropDownList {
        position: absolute;
        left: 54px;
        top: 62px;
        z-index: 999;
        /*height: 400px;
        width: 100px;*/
        /*height: 150px;
    overflow-y: scroll;*/
    }

    .entityListOpen {
        position: absolute;
        left: 250px;
        top: 44px;
        cursor: pointer;
        /*height: 50%;
        overflow-y: auto;*/
    }

    .entityNavi li:hover {
        cursor: pointer;
        font-size: 1.1em;
        background: #bbb;
    }


    .entityNavi > li {
        display: none;
    }

    .entityNavi li {
        list-style: none;
        margin-left: 10px;
        width: 190px;
        height: 20px;
        line-height: 19px;
        text-align: center;
        border: solid 1px #5b79ef;
        position: relative;
        margin: 1px;
        background-color: #999;
        transition: all ease-in-out 0.2s;
        -o-transition: all ease-in-out 0.2s;
        -moz-transition: all ease-in-out 0.2s;
        -webkit-transition: all ease-in-out 0.2s;
    }

        .entityNavi li ul li {
            height: 35px;
            padding: 4px;
        }

    .entityNavi {
        margin: 0;
        padding: 0;
        height: inherit;
        /*width: 400px;*/
        overflow-y: auto;
        overflow-x: hidden;
    }

        .entityNavi > li {
            display: none;
        }

        .entityNavi li a {
            font: 1.0em Tahoma, Arial;
            color: white;
            cursor: pointer;
            text-decoration: none;
        }

        .entityNavi ul {
            margin-left: 160px;
            /*margin-top: -18px;*/
            width: 210px;
            position: fixed;
        }

    .childContainer {
        display: none;
        max-height: 300px;
        /*height: 300px;*/
        overflow-y: auto;
    }

    .first_element {
    }

    /*Scroll style start */
.EntityDropDownList_scroll::-webkit-scrollbar-track {
    -webkit-box-shadow: inset 0 0 6px rgba(0,0,0,0.3);
    background-color: #F5F5F5;
}

.EntityDropDownList_scroll::-webkit-scrollbar{
    width: 6px;
    background-color: #F5F5F5;
}

.EntityDropDownList_scroll::-webkit-scrollbar-thumb {
    background-color: #999;
}
/*Scroll style start */
</style>

<script>
    $(document).ready(function () {
        var a = $('.entityNavi');
        a.css("overflow-y", "hidden");
        CloseChildUlElement();

    });
    function ShowChild(o) {
        $(".childContainer").css("display", "none");

        var plusDiv = $(o);

        if (plusDiv.text() === "+") {
            $(".openChild").text("+"); //All <div> +</div> set + 
            plusDiv.text("->");
            var ul = $(o).next();
            
            //что бы не уезжали дочерние элементы
            var offTop = plusDiv.offset().top;

            if (Math.round(offTop) + parseInt(ul.css("height"), 10) > document.documentElement.clientHeight) {
                
                var pageHeight = document.documentElement.clientHeight;
                
                var footer = $(".footer");
                var footerHeight = parseInt(footer.css("height"), 10);

                var height = pageHeight - footerHeight - parseInt(ul.css("height"), 10) - 5;

                ul.css("top", height);

            }
            else {
                ul.css("top", Math.round(offTop));
            }

            ul.css("display", "block");
            
        } else {
            plusDiv.text("+");
            var ul = $(o).next();
            ul.css("display", "none");
        }
    }

    function openEntityNavi() {
        var plusDiv = $(".entityListOpen");


        if (plusDiv.text() === "+") {
            var a = $('.entityNavi');
            a.css("overflow-y", "auto");

            //подстройка высоты EntityDropDownList под размеры страницы
            var pageHeight = document.documentElement.clientHeight;
            var footer = $(".footer");
            var footerHeight = parseInt(footer.css("height"),10);
            var aTop = Math.round(a.offset().top);
            var height = pageHeight - footerHeight - aTop - 5;
            
            a.css("height", height);
            
            
            plusDiv.text("<-");
            $(".entityNavi > li").css("display", "block");

        } else {
            plusDiv.text("+");
            var a = $('.entityNavi');
            a.css("overflow-y", "hidden");
            a.css("height", "inherit");
            CloseChildUlElement();
        }
    }

    function CloseChildUlElement() {
        $(".entityNavi > li").css("display", "none");
        $(".openChild").text("+");

        var url = document.URL.replace("~", "");

        if (url.indexOf("entity") === -1)
            $(".entityNavi > li > a").each(function () {
                if ($(this).attr("id") === undefined) {
                    $(this).css("color", "yellow");
                    $(this).parent().css("display", "block");
                }
            });
        else
            $(".entityNavi > li > a").each(function () {
                if (url.indexOf($(this).attr("id")) != -1) {
                    $(this).css("color", "yellow");
                    $(this).parent().css("display", "block");
                }
            });
    }
</script>

<div>
    <asp:Literal ID="literal" runat="server" />
</div>
