<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="preview.aspx.cs" Inherits="Monitoring.documents.preview" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function load() {
            var o = document.getElementsByTagName('iframe')[0];
            o.style.height = document.body.scrollHeight + 'px';
        }
    </script>
</head>
<body style="padding: 0; margin: 0" onload="load()">
    <iframe style="width: 100%; height: 100%; border: none" src="<%: Request["id"] %>.document"></iframe>
</body>
</html>
