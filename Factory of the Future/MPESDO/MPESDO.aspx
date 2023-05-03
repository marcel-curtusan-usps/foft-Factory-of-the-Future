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
       
            <div class="flex-container">
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
        
        
        </div>
        <div class="flex-container">
            <div class="item2">
                <p class="card-title">Operation #918</p>
                <h1 class="card-text">Machine Type: DBCS</h1>
                <h1 class="card-text">Scheduled Staff: 5</h1>
                <h1 class="card-text">Actual Staff: 2</h1>
            </div>
            <div class="item2">
                <p class="card-title text-success">135,389</p>
                <h1 class="card-text">Total Volume</h1>
                <h1 class="card-text">Plan: 95,000</h1>
            </div>
            <div class="item2">
                <p class="card-title text-success">23,134/hr</p>
                <h1 class="card-text">Avg Throughput</h1>
                <h1 class="card-text">Plan: 21,000</h1>
            </div>
            <div class="item2">
                <h1 class="card-title">Projected End Time:</h1>
                <p class="card-text text-danger">06:30</p>
                <h1 class="card-text">Planed End Time:</h1>
                <p class="card-text">05:00</p>
            </div>
        </div>

        <div class="item2 text-center">
            <p class="card-text">6</p>
            <h1 class="card-text">Total Machines</h1>
            <div class="card-body flex-container">
                <div class="item2">
                    <h5 class="card-title">DBCS-001</h5>
                    <h6 class="card-text btn-success">On Schedule</h6>
                    <p class="card-text">420</p>
                </div>
                <div class="item2">
                    <h5 class="card-title">DBCS-033</h5>
                    <h6 class="card-text btn-danger">Behind Schedule</h6>
                    <p class="card-text">400</p>
                </div>
                <div class="item2">
                    <h5 class="card-title">DBCS-089</h5>
                    <h6 class="card-text btn-secondary">Not started</h6>
                    <p class="card-text">2612</p>
                </div>
                <div class="item2">
                    <h5 class="card-title">USS-001</h5>
                    <h6 class="card-text btn-warning">Maintenance</h6>
                    <p class="card-text">N/A</p>
                </div>
            </div>
        </div>
        <div class="item2 text-left">
            <p class="card-text text-danger">Alert Message: Wrong Sort Program on [DBCS #22] / Machine Jams</p>
        </div>
    </div>
</body>
</html>
