<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DockDoor.aspx.cs" Inherits="Factory_of_the_Future.DockDoor.DockDoor1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>DockDoor</title>
    <link rel="stylesheet" href="Content/dockdoor.css" />
    <link rel="stylesheet" href="../Content/bootstrap-icons.css" />
    <link rel="stylesheet" href="../Content/bootstrap.min.css" />
</head>
<body>
     <div class="doorcontainer-fluid">
        <div style="display: flex">
            <div class="card w-75">
                <div class="card-header" style="padding: 0rem;">
                  
                    <div class="table-responsive">
                        <table class="table table-bordered table-hover table-striped" id="currentTripTable">
                            <tbody>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="card-body" style="padding: 0rem;">
                    <div class="table-responsive">
                        <table class="table table-bordered table-hover table-striped"  style="border-top-style: solid; border-top-width: thick; border-top-color: black;" id="containerLocationtable">
                            <tbody>
                            </tbody>
                        </table>
                    </div>
                </div>
             
            </div>
            <div class="card w-25">
                <div class="text-center">
                    <label class="control-label" style="font-size: 15rem; font-weight: bolder;" id="dockdoorNumber"></label>
                </div>
                <div class="card-body text-center" style="padding: 0rem; display: none;" id="countdowndiv">
                    <label class="control-label" style="font-size: 4rem;" id="countdowntext"></label>
                    <label class="timecounter" style="padding-top: 15rem;"></label>
                </div>

            </div>
        </div>
    </div>
    <script src="../Scripts/jquery-3.6.0.min.js"></script>
    <script src="../Scripts/jquery.dataTables.js"></script>
    <script src="../Scripts/moment.js"></script>
    <script src="../Scripts/moment-duration-format.js"></script>
    <script src="../Scripts/jquery.signalR-2.4.2.js"></script>
    <script src="../signalr/hubs/"></script>
    <script src="Scripts/digitalDockDoor.js"></script>
</body>
</html>
