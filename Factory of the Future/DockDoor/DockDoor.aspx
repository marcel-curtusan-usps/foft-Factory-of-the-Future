<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DockDoor.aspx.cs" Inherits="Factory_of_the_Future.DockDoor.DockDoor" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head runat="server">
    <title>DockDoor</title>
     <link rel="preload" href="../Content/bootstrap-icons.css" as="style"/>
    <link rel="preload" as="style" href="../Content/bootstrap.min.css" />
    <link rel="stylesheet" href="../Content/bootstrap-icons.css" />
    <link rel="stylesheet" href="../Content/bootstrap.min.css" />

</head>
<body>
    <div class="card">
        <div class="card-header">   
            <div class="row">
                    <div class="col text-center">
                       <label class="control-label" style="font-size: 8rem;font-weight: bolder;margin-top: -64px;margin-bottom: -36px;">DockDoor <button class="btn btn-lg btn-dark" id="dockdoorNumber" style="font-size: 4rem;font-weight: bolder;margin-bottom: -20px;margin-top: -36px;"></button> </label>
                    </div>
                  <div class="col text-center">
                         <label class="control-label" style="font-size: 4rem; font-weight: bolder;margin-bottom: -7px;margin-top: -37px">Trip Direction</label>
                        <button class="btn btn-lg btn-block btn-light" id="tripDirectionInd" style="margin-bottom: -12px;"></button>
                    </div>
            </div>
        </div>
        <div class="card-body">
                  <div class="row">
                    <div class="col">
                        1 of 3
                    </div>
                    <div class="col">
                      <label class="container-lable" id="destSiteId"></label>
                    </div>
                   
                </div>
                <div class="row">
                    <div class="col">
                        1 of 3
                    </div>
                    <div class="col">
                        2 of 3
                    </div>
                    <div class="col">
                        3 of 3
                    </div>
                </div>
        </div>
</div>
    <script src="../Scripts/jquery-3.6.0.min.js"></script>
    <script src="../Scripts/jquery.signalR-2.4.2.js"></script>
    <script src="../signalr/hubs/"></script>
    <script src="Scripts/DigitalDockDoor.js"></script>
</body>
</html>
