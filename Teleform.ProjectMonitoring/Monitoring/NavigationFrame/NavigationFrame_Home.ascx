<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationFrame_Home.ascx.cs" Inherits="Teleform.ProjectMonitoring.NavigationFrame.NavigationFrame_Home" %>
<link href="../Styles/Header_style/NavigationFrame_Home.css" rel="stylesheet" />


<script src="../Scripts/bootstrap_treeview/bootstrap-treeview.js"></script>
<%--<link href="../Scripts/bootstrap_treeview/bootstrap-treeview.css" rel="stylesheet" />--%>

<div class="action_container row" id="action_container">
    <div class="column_left col-md-4 text-center">
    </div>
    <div class="column_center col-md-4 text-center">
    </div>
    <div class="column_right col-md-4 text-center">
    </div>
</div>

<div id="inner_left_menu" class="nav navbar-nav side-nav">
    <script>
              var tree = [
        {
            text: "<a href='http://google.com'>Node 1</a>",
            nodes: [
              {
                  text: "<a href='http://google.com'>Node 1</a>",
                  icon: "glyphicon glyphicon-stop",
                  selectedIcon: "glyphicon glyphicon-stop",
                  color: "#000000",
                  backColor: "#555555",
                  href: "http://google.com",
                  selectable: true,
                  state: {
                      
                     
                      expanded: true,
                      
                  },
                  tags: ['available'],
                  nodes: [
                    {
                        text: "Grandchild 1"
                    },
                    {
                        text: "Grandchild 2"
                    }
                  ]
              },
              {
                  text: "Child 2"
              }
            ]
        },
        {
            text: "Node 1",
            icon: "glyphicon glyphicon-stop",
            selectedIcon: "glyphicon glyphicon-stop",
            color: "#000000",
            backColor: "#FFFFFF",
            href: "http://google.com",
            selectable: true,
            state: {
               
                
                expanded: true,
               
            },
            tags: ['available'],
            nodes: [
              {
                  text: "Parent 3"
              },
                {
                    text: "Parent 4"
                }
            ]

        },
              {
                  text: "Parent 3"
              },
              {
                  text: "Parent 4"
              },
              {
                  text: "Parent 5"
              }
              ];

  //      var tree = [
  //{
  //    text: "Parent 1",
  //    nodes: [
  //      {
  //          text: "Child 1",
  //          nodes: [
  //            {
  //                text: "Grandchild 1"
  //            },
  //            {
  //                text: "Grandchild 2"
  //            }
  //          ]
  //      },
  //      {
  //          text: "Child 2"
  //      }
  //    ]
  //},
  //{
  //    text: "Parent 2"
  //},
  //{
  //    text: "Parent 3"
  //},
  //{
  //    text: "Parent 4"
  //},
  //{
  //    text: "Parent 5"
  //}
  //      ];
        $(document).ready(function () {
            $('#tree').treeview({ data: tree });
        });
    </script>
    <div id="tree"></div>
</div>
