<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Kanban.aspx.cs" Inherits="Factory_of_the_Future.MPEKanban.MPEKanban" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../Content/bootstrap.min.css" rel="stylesheet" />
    <link href="../Content/supportCSSBundle.css" rel="stylesheet" />
    <link href="Contents/kanban.css" rel="stylesheet" />
</head>
<body>
    <div id="divkanban" class="h-50 w-100">

    </div>
    <div id="divkanban_dispatch" class="h-50 w-100" >

    </div>
    <script src="../Scripts/jquery-3.6.0.js"></script>
    <script src="../Scripts/jquery.dataTables.js"></script>
    <script src="../Scripts/jquery-ui-1.13.1.js"></script>
    <script src="../Scripts/jquery.signalR-2.4.2.js"></script>
    <script src="../signalr/hubs/"></script>
    <script src="Scripts/kanban.js"></script>
</body>
</html>
