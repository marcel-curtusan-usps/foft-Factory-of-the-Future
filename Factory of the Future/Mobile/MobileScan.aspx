﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MobileScan.aspx.cs" Inherits="Factory_of_the_Future.Mobile.MobileScan" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="Content/scan_site.css" rel="stylesheet" />
    <link href="../Content/bootstrap.css" rel="stylesheet" />

    <style>
        .input_label {
            font-weight: bold;
        }
    </style>
    <title>Scan</title>
</head>
<body>
    <div class="vertical-center">
        <div id="ScanPanel" class="panel panel-info">
            <div class="panel-heading">
                <div class="row" style="margin-bottom: -12px; margin-top: -1px;">
                    <div class="text-center" style="width: 100%;">
                        <label>Version Number:
                            <label id="scannerVersion"></label>
                        </label>
                    </div>
                </div>
                <!--<div class="row">
                    <div class="col">
                        <label class="text-responsive input_label">Width:<span class="col" id="widthsz"></span></label>
                    </div>
                    <div class="col">
                        <label class="text-responsive input_label">Height:<span class="col" id="heightsz"></span></label>
                    </div>
                </div>-->
                <div class="form-group row">
                    <div class="col-sm-12 text-center">
                        <button title="agv_status_button" type="button" class="btn btn-outline-light btn-lg center-block agv_status_button" style="width: 190px;" disabled="disabled" id="agv_status_button">
                            <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                        </button>
                        <button title="connection_button" type="button" class="btn btn-danger btn-lg center-block agvactive" disabled="disabled" id="connection_button"></button>
                    </div>
                </div>
            </div>
            <div class="form-group row" id="scan_card_header" style="display: none">
                <div class="col text-center">
                    <span class="text-danger" id="scan_card_header_info"></span>
                </div>
            </div>

            <div class="panel-body ">
                <div class="col-xs-12">

                    <div class="panel panel-info" style="border-color: white;">
                        <div class="panel-body">


                            <div class="row">
                                <div class="col-xs-2">
                                    <label class="control-label pull-left top-label" style="color: white">-</label>
                                    <div class="right"><span>Input:</span></div>
                                </div>
                                <div class="col-xs-10">
                                    <input title="Input" placeholder="Input" type="text" id="barcode_input" autofocus name="barcode_input" class="form-control clearable" />

                                </div>

                            </div>

                            <div class="row">
                                <div class="col-xs-2">
                                    <label class="control-label pull-left top-label" style="color: white">-</label>
                                    <div class="right"><span>From:</span></div>
                                </div>
                                <div class="col-xs-2">
                                    <label class="control-label pull-left top-label ">Column</label>
                                    <input title="Column" placeholder="Column" name="pickup_column" disabled="disabled" class="form-control clearable" id="text_origincolumn" type="text">
                                </div>
                                <div class="col-xs-3">
                                    <label class="control-label pull-left top-label ">Location</label>
                                    <input title="Location" placeholder="Location" name="pickup_location" disabled="disabled" class="form-control clearable" id="text_originlane" type="text">
                                </div>
                                <div class="col-xs-5">
                                    <label class="control-label pull-left top-label">Pickup Code</label>
                                    <input title="Pickup Code" placeholder="Pickup Code" name="pickup_pickupcode" disabled="disabled" class="form-control clearable" id="text_origin" type="text">
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xs-2">
                                    <label class="control-label pull-left top-label" style="color: white">-</label>
                                    <div class="right"><span>To:</span></div>
                                </div>
                                <div class="col-xs-6">
                                    <label class="control-label pull-left top-label">Door #</label>
                                    <input title="Door #" placeholder="Door #" name="dest_location" disabled="disabled" class="form-control clearable" id="text_destinationdoor" type="text">
                                </div>
                                <div class="col-xs-4">
                                    <label class="control-label pull-left top-label">NASS Code</label>
                                    <input title="NASS Code" placeholder="NASS Code" name="dest_nasscode" disabled="disabled" class="form-control clearable" id="text_destinationnass" type="text">
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-xs-2">
                                </div>
                                <div class="col-xs-2">
                                    <label class="control-label pull-left top-label">Column</label>
                                    <input title="Column" placeholder="Column" name="dropoff_column" disabled="disabled" class="form-control clearable" id="text_destinationcolumn" type="text">
                                </div>
                                <div class="col-xs-3">
                                    <label class="control-label pull-left top-label">Location</label>
                                    <input title="Location" placeholder="Location" name="dropoff_location" disabled="disabled" class="form-control clearable" id="text_destinationlane" type="text">
                                </div>
                                <div class="col-xs-5">
                                    <label class="control-label pull-left top-label">Destination Code</label>
                                    <input title="Destination Code" placeholder="Destination Code" name="dropoff_dropoffcode" disabled="disabled" class="form-control clearable" id="text_destination" type="text">
                                </div>
                            </div>

                            <div class="row" style="display: none">
                                <div class="col-xs-2">
                                    <label class="control-label pull-left top-label" style="color: white">-</label>
                                    <div class="right"><span>End:</span></div>
                                </div>
                                <div class="col-xs-2">
                                    <label class="control-label pull-left top-label">Column</label>
                                    <input title="Column" placeholder="Column" name="dropoff_column" disabled="disabled" class="form-control clearable" id="text_endlocationcolumn" type="text">
                                </div>
                                <div class="col-xs-7">
                                    <label class="control-label pull-left top-label">End Code</label>
                                    <input title="End Code" placeholder="End Code" name="dropoff_location" disabled="disabled" class="form-control clearable" id="text_endlocation" type="text">
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="panel-footer">
                <div class="text-center header-label">
                    <label class="control-label text-center">Scan Status</label>
                </div>
                <div class="row">
                    <div align="center" class="col-sm-12 col-xs-12">
                        <label class="control-label" id="status" style="font-weight: bolder;"></label>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <footer align="center">
        <label class="control-label text-center header-label" runat="server">Scan Location Barcode and Placard Barcode</label>
    </footer>
    <script src="../Scripts/jquery-3.6.0.js"></script>
    <script src="../Scripts/bootstrap.js"></script>
    <script src="Script/Scan.js"></script>
    <script src="Script/ScannerDectection.js"></script>
</body>
</html>