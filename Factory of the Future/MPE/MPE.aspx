<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MPE.aspx.cs" Inherits="Factory_of_the_Future.MPE.MPE" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="../Content/bootstrap.min.css" rel="stylesheet" />
    <title>MPE</title>
</head>
<body>
    <div class="card w-100" style="padding: 0px;">
        <div class="card-header">
            <div class="container-fluid">
                <div class="row justify-content-md-center">
                    <div class="col-5 float-right" style="padding: 0rem;">
                        <label class="control-label" style="font-size: 4rem;" id="mpeName_text">MPE Name:</label>
                        <label class="control-label" style="font-size: 4rem; font-weight: bolder;" id="mpeName"></label>
                    </div>
                    <div class="col-6" style="padding: 0rem;">
                        <label class="control-label float-right" style="font-size: 4rem;" id="opn_text">Operation Number:</label>
                    </div>
                    <div class="col" style="padding: 0rem;">
                        <label class="control-label" style="font-size: 4rem; font-weight: bolder;" id="opn"></label>
                    </div>
                </div>
                <div class="row">
                    <div class="col-5 float-right" style="padding: 0rem;">
                    </div>
                    <div class="col-5" style="padding: 0rem;">
                        <label class="control-label float-right" style="font-size: 4rem;" id="sortprogram">Sortplan Name:</label>
                    </div>
                    <div class="col" style="padding: 0rem;">
                        <label class="control-label" style="font-size: 4rem; font-weight: bolder;" id="sortplan_name_text"></label>
                    </div>
                </div>
            </div>
        </div>
        <div class="card-body" style="padding: 0px;">
            <div class="table-responsive">
                <table class="table table-bordered table-hover table-striped" id="mpeStatustable">
                    <tbody>
                    </tbody>
                </table>
            </div>
        </div>
        <div class="card-footer">
            <div class="text-center" style="padding: 0rem;" id="countdowndiv">
                <label class="control-label" style="font-size: 4rem;" id="countdowntext">Planned Sort Program End Time Countdown:</label>
                <label class="control-label" style="font-size: 4rem; font-weight: bolder;" id="countdown"></label>
            </div>
            <div class="text-center">
                <label class="control-label" style="font-size: 4rem;" id="mpe_status_txt">Machine Status:</label>
                <label class="control-label" style="font-size: 4rem; font-weight: bolder;" id="mpe_status"></label>
            </div>

        </div>
    </div>
    <script src="../Scripts/jquery-3.6.0.js"></script>
    <script src="../Scripts/jquery.dataTables.js"></script>
    <script src="../Scripts/moment.js"></script>
    <script src="../Scripts/moment-duration-format.js"></script>
    <script src="../Scripts/jquery.signalR-2.4.2.js"></script>
    <script src="../signalr/hubs/"></script>
    <script src="Scripts/mpe.js"></script>
</body>
</html>
