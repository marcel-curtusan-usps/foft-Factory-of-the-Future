<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DockDoor.aspx.cs" Inherits="Factory_of_the_Future.DockDoor.DockDoor" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head runat="server">
    <title>DockDoor</title>
     <link rel="preload" href="../Content/bootstrap-icons.css" as="style"/>
    <link rel="preload" as="style" href="../Content/bootstrap.min.css" />
    <link rel="stylesheet" href="../Content/bootstrap-icons.css" />
    <link rel="stylesheet" href="../Content/bootstrap.min.css" />
    <link rel="stylesheet" href="Content/dockdoor.css" />

</head>
<body>
    <div class="doorcontainer-fluid">
        <div style="display: flex">
            <div class="card w-25" style="background-color: #0067F4">
                <div class="text-center align-middle">
                    <br/><br/>
                    <label class="control-label" style="font-size: 6.5rem; color: white; font-weight: bolder;">
                        DOOR NUMBER <br/><br/>
                        <label style="font-size: 12rem; color: white; font-weight: bolder;" id="dockdoorNumber"></label>
                    </label>
                </div>
            </div>
            <div class="card w-50">
                <div class="card-header" style="padding: 0rem;">
                    <label class="control-label" style="font-size: 3rem; font-weight: bolder;">Current Trip</label>
                    <div class="table-responsive">
                        <table class="table table-bordered table-hover table-striped" id="currentTripTable">
                            <tbody>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="card-body" style="padding: 0rem;">
                    <div class="table-responsive">
                        <table class="table table-bordered table-hover table-striped" id="containerLocationtable">
                            <tbody>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="card-footer" style="padding: 0rem;">
                    <label class="control-label" style="font-size: 3rem; font-weight: bolder;">Next Trip</label>
                    <div class="table-responsive">
                        <table class="table table-bordered table-hover table-striped" id="nextTriptable">
                            <tbody>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <div class="card w-25">
                <div class="card-heade text-center" style="padding: 0rem;">
                    <div class="col ml-0">
                        <label class="control-label" style="font-size: 3rem; font-weight: bolder display:none;" id="totalIndc">Not Trip</label>
                    </div>
                    <div class="col ml-0" style="background-color: purple;">
                        <label class="control-label flex-fill" style="font-size: 4rem; color: white; font-weight: bolder; display:none;" id="containerLoaded"></label>
                    </div>
                </div>
                <div class="card-body text-center" style="padding: 0rem; background-color: yellow; display:none;" id="countdowndiv">
                    <label class="control-label" style="font-size: 4rem; font-weight: bolder;">
                        Countdown to Scheduled Departure <br/><br/>
                        <label class="control-label text-center" style="font-size: 6rem; font-weight: bolder;" id="timeCount">00:00</label>
                    </label>
                </div>

            </div>
        </div>
    </div>
    <script src="../Scripts/jquery-3.6.0.min.js"></script>
    <script src="../Scripts/jquery.dataTables.js"></script>
    <script src="../Scripts/moment.js"></script>
    <script src="../Scripts/jquery.signalR-2.4.2.js"></script>
    <script src="../signalr/hubs/"></script>
    <script src="Scripts/DigitalDockDoor.js"></script>
</body>
</html>
