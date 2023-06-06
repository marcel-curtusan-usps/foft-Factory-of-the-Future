<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MPESDO.aspx.cs" Inherits="Factory_of_the_Future.MPESDO.MPESDO" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <link rel="stylesheet" href="~/Content/bootstrap.min.css" type="text/css" media="screen" />
    <link href="Content/mpesdo.css" rel="stylesheet" />

    <title>MPE SDO</title>
</head>
<body>
    <div class="container-fluid">
       <%-- <div class="flex-container">
            <div class="item">
                <p class="card-title">MMP Mail Cleared</p>
            </div>
            <div class="item">
                <p class="card-title">004 Cleared</p>
            </div>
            <div class="item-grey">
                <p class="card-title">HSTS/RCS Cleared</p>
            </div>
            <div class="item-grey">
                <p class="card-title">OGP Cleared</p>
            </div>
            <div class="item-grey">
                <p class="card-title">DOV in [time]</p>
            </div> 
        </div>--%>
        <div class="flex-container">
            <div class="item2">
                <%--<p class="card-title">Operation #918</p>--%>
                <h1 id="machineType" class="card-text">Machine Type: DBCS</h1>
                <h1 id="scheduledStaff" class="card-text">Scheduled Staff: 5</h1>
                <h1 id="actualStaff" class="card-text">Actual Staff: 2</h1>
            </div>
            <div class="item2">
                <p id="totalVolume" class="card-title text-success">135,389</p>
                <h1 class="card-text">Total Volume</h1>
                <h1 id="plannedVolume" class="card-text">Plan: 95,000</h1>
            </div>
            <div class="item2">
                <p id="avgThroughput" class="card-title text-success">23,134/hr</p>
                <h1 class="card-text">Avg Throughput</h1>
                <h1 id="plannedThroughput" class="card-text">Plan: 21,000</h1>
            </div>
            <div class="item2">
                <h1 class="card-title">Projected End Time:</h1>
                <p id="projectedEndTime" class="card-text">06:30</p>
                <h1 class="card-text">Planed End Time:</h1>
                <p id="plannedEndTime" class="card-text">05:00</p>
            </div>
        </div>

        <div  class="item2 text-center">
            <p id="totalMPEs" class="card-text"></p>
            <h1 class="card-text">Total Machines</h1>
            <div id="mpeGroupList" class="card-body flex-container"></div>
        </div>
        <div class="item2 text-left">
            <p id="mpeAlertMsg" class="card-text text-danger"></p>
        </div>
    </div>
    <script src="../Scripts/jquery-3.6.0.js"></script>
     <script src="../Scripts/moment.js"></script>
    <script src="../Scripts/jquery.signalR-2.4.2.js"></script>
    <script src="../signalr/hubs/"></script>
    <script src="Scripts/mpesdo.js"></script>
</body>
</html>
