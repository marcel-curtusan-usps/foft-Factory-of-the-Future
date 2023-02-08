<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DockDoorView1.aspx.cs" Inherits="Factory_of_the_Future.DockDoor.DockDoor" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head runat="server">
    <title>DockDoor</title>
    <link rel="stylesheet" href="Content/dockdoor.css" />
    <link rel="stylesheet" href="../Content/bootstrap-icons.css" />
    <link rel="stylesheet" href="../Content/bootstrap.min.css" />
 

</head>
<body>
    <div class="doorcontainer-fluid">
        <div style="display: flex">
            <div class="card w-25" style="background-color: #0067F4">
                <div class="text-center align-middle">
                    <div class="btn-warning" id="testmode_div" style="display:block;">
                        <label class="control-label" style="font-size: 3rem; color: white; font-weight: bolder;" id="testmodetxt">
                            Test Mode
                        </label>
                    </div>
                    <br />
                    <div>
                        <label class="control-label" style="font-size: 6.5rem; color: white; font-weight: bolder;">
                            DOOR NUMBER
                            <br />
                            <br />
                            <label style="font-size: 12rem; color: white; font-weight: bolder;" id="dockdoorNumber"></label>
                        </label>
                    </div>
                </div>
            </div>
            <div class="card w-50">
                <div class="card-header" style="padding: 0rem;">
                    <div class="container">
                        <div class="row">
                            <div class="col-4">
                                <label class="control-label" style="font-size: 3rem; font-weight: bolder;">Current Trip</label>
                            </div>
                            <div class="col align-self-center">
                                <button class="btn btn-outline-info btn-block" id="currentTripDirectionInd" style="height: 80px; margin-left: 15px;font-size: 3rem;font-weight: bolder;border: none;color: black;"></button>
                            </div>
                        </div>
                    </div>
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
                    <div class="container">
                        <div class="row">
                            <div class="col-4">
                                <label class="control-label" style="font-size: 3rem; font-weight: bolder;">Next Trip</label>
                            </div>
                            <div class="col align-self-center">
                                <button class="btn btn-outline-info btn-block" id="nextTripDirectionInd" style="height: 80px; margin-left: 15px;font-size: 3rem;font-weight: bolder;border: none;color: black;"></button>
                            </div>
                        </div>
                    </div>
                    <div class="table-responsive">
                        <table class="table table-bordered table-hover table-striped" id="nextTriptable">
                            <tbody>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <div class="card w-25">
                <div class="card-body text-center" style="padding: 0rem; display: none;" id="countdowndiv">
                    <label class="control-label" style="font-size: 4rem; font-weight: bolder;" id="countdowntext"></label>
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
    <script src="Scripts/DigitalDockDoor.js"></script>
</body>
</html>
