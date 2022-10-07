<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Factory_of_the_Future.Main" %>

<!DOCTYPE html>

<html lang="en" xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>USPS FOTF</title>
    <meta charset="utf-8" />
    <meta http-equiv="Cache-control" content="public" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <script type="text/javascript">
        //Access Code-Behind Data in Javascript
        localStorage.setItem('User', JSON.stringify(<%=Session_Info%>));
    </script>
    <style>
        .loadingFOTF {
            position: fixed;
            margin-left: -5%;
            margin-right: auto;
            width: 110%;
            top: -50px;
            height: 10000px;
            display: block;
            z-index: 2000000001;
            background-color: #f9f9f9;
        }

        .loadingFOTFBody {
            overflow: hidden;
        }
    </style>
    <link rel="preload" href="Content/bootstrap-icons.css" as="style"/>
    <link rel="preload" as="style" href="Content/bootstrap.min.css" />
    <link rel="preload" as="style" href="Content/selectize.bootstrap4.css" />
    <link rel="preload" as="style" href="Content/easy-button.css" />
    <link rel="preload" as="style" href="Content/_base.min.css" />
    <link rel="preload" as="style" href="Content/leaflet.css" />
    <link rel="preload" as="style" href="Content/leaflet-sidebar.css" />
    <link rel="preload" as="style" href="Content/leaflet-geoman.css" />
    <link rel="preload" as="style" href="Content/Site.min.css" />
    <link rel="preload" as="style" href="Content/Filter.css" />
    <link rel="stylesheet" href="Content/bootstrap-icons.css" />
    <link rel="stylesheet" href="Content/bootstrap.min.css" />
    <link rel="stylesheet" href="Content/selectize.bootstrap4.css" />
    <link rel="stylesheet" href="Content/easy-button.css" />
    <link rel="stylesheet" href="Content/_base.min.css" />
    <link rel="stylesheet" href="Content/leaflet.css" />
    <link rel="stylesheet" href="Content/leaflet-sidebar.css" />
    <link rel="stylesheet" href="Content/leaflet-geoman.css" />
    <link rel="stylesheet" href="Content/Site.min.css" />
    <link rel="stylesheet" href="Content/Filter.css" />
</head>
<body class="loadingFOTFBody">
      <div id="loadWrapper" class="loadingFOTF">
    </div>
    
    <div id="sparkline-message">
        <div id="sparkline-message-fixed">
            <div id="sparkline-message-inner">
                Zoom in to see spark - lines
            </div>
        </div>
    </div>
    <canvas id="sparkline-canvas" width="300" height="150" style="width: 300px; height: 150px; position: absolute; margin-top: -1000000px; "></canvas>
    <div id="map"></div>
      <!-- Modal Section -->
    <div class="modal fade" id="API_Connection_Modal" tabindex="-1" role="dialog" data-backdrop="static" aria-labelledby="API_Connection_Modal_Label">
        <div class="modal-dialog modal-xl">
            <div class="modal-content bg-white">
                <div class="modal-header1">
                    <div class="col-11">
                        <h4 class="modal-title1" id="modalHeader_ID"><span aria-hidden="true"></span></h4>
                    </div>
                    <button class="col-1 close" type="button" data-dismiss="modal" aria-label="Close"><i class="pi-iconExit float-right mt-2 mr-2"></i></button>
                </div>
                <div class="modal-body1">
                    <div class="row mb-3">
                        <div class="col">
                            <label class="control-label">Connection Name</label>
                            <input id="connection_name" type="text" class="form-control" name="connection_name">
                            <span id="error_connection_name" class="text-danger"></span>
                        </div>
                        <div class="col">
                            <label class="control-label">Connection Options</label>
                            <div class="custom-control custom-switch">
                                <div class="custom-control custom-switch">
                                    <input id="active_connection" type="checkbox" class="custom-control-input" name="active_connection">
                                    <label class="custom-control-label" for="active_connection">Active Connection</label>
                                </div>
                            </div>
                            <div class="custom-control custom-switch">

                            </div>
                        </div>
                        <div class="col mt-3">
                            <div class="custom-control custom-switch">
                                <input id="udp_connection" type="checkbox" class="custom-control-input" name="udp_connection">
                                <label class="custom-control-label" for="udp_connection">UDP Connection</label>
                            </div>
                            <div class="custom-control custom-switch">
                                <input id="ws_connection" type="checkbox" class="custom-control-input" name="ws_connection">
                                <label class="custom-control-label" for="ws_connection">WS Connection</label>
                            </div>
                            <div class="custom-control custom-switch">
                                <input id="hour_range" type="checkbox" class="custom-control-input" name="hour_range">
                                <label class="custom-control-label" for="hour_range">Hour Range</label>

                            </div>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="form-group col-4">
                            <label class="control-label">Host Name</label><input id="hostanme" type="text" class="form-control" name="hostanme"><span id="error_hostanme" class="text-danger"></span>
                        </div>
                        <div class="form-group col-4">
                            <label class="control-label">IP Address</label><input id="ip_address" type="text" class="form-control" name="ip_address"><span id="error_ip_address" class="text-danger"></span>
                        </div>
                        <div class="form-group col-4">
                            <label class="control-label">Port Number</label><input id="port_number" type="text" class="form-control" name="port_number"><span id="error_port_number" class="text-danger"></span>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="form-group col">
                            <label class="control-label">URL</label><input id="url" type="text" class="form-control" name="url"><span id="error_url" class="text-danger"></span>
                        </div>
                        <!--<div class="form-group col-6">
                            <label class="control-label">API Key</label><input id="outgoingapikey" type="text" class="form-control" name="outgoingapikey"><span id="error_outgoingapikey" class="text-danger"></span>
                        </div>-->
                    </div>
                    <div class="row hours_range_row" style="display:none">
                        <div class="col-6">
                            <div class="d-flex justify-content-center my-2">
                                <div class="w-100">
                                    <label class="control-label">Hours Backwards: <span class="font-weight-bold text-primary ml-2 hoursbackvalue"></span></label>
                                    <input type="range" class="custom-range" id="hoursback_range" min="0" max="60" value="0">
                                </div>
                            </div>
                        </div>

                        <div class="col-6">
                            <div class="d-flex justify-content-center my-2">
                                <div class="w-100">
                                    <label class="control-label">Hours Forward: <span class="font-weight-bold text-primary ml-2 hoursforwardvalue"></span></label>
                                    <input type="range" class="custom-range" id="hoursforward_range" min="0" max="60" value="0">
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="form-group col">
                            <label class="control-label">Message Type</label><input id="message_type" type="text" class="form-control" name="message_type"><span id="error_message_type" class="text-danger"></span>
                        </div>
                        <div class="form-group col">
                            <label class="control-label">Retrieve Occurrences</label>
                            <select id="data_retrieve" class="form-control pb-1" name="data_retrieve">
                                <option value=""></option>
                                <option value="250">250 Millisecond</option>
                                <option value="500">500 Millisecond</option>
                                <option value="1000">1 Second</option>
                                <option value="3000">3 Second</option>
                                <option value="5000">5 Second</option>
                                <option value="30000">30 Second</option>
                                <option value="60000">1 Minute</option>
                                <option value="180000">3 Minute</option>
                                <option value="300000">5 Minute</option>
                                <option value="1800000">30 Minute</option>
                                <option value="3600000">1 Hour</option>
                                <option value="86400000">1 Day</option>
                            </select>
                            <span id="error_data_retrieve" class="text-danger"></span>
                        </div>
                    </div>
                </div>
                <div class="modal-footer1">
                    <div class="col">
                        <button class="btn btn-outline-secondary float-left" type="button" data-dismiss="modal">Close</button>
                    </div>
                    <div class="col text-center"><span class="text-info" id="error_apisubmitBtn"></span></div>
                    <div class="col">
                        <button class="btn btn-primary float-right" type="button" id="apisubmitBtn">Save</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="Notification_Setup_Modal" tabindex="-1" role="dialog" data-backdrop="static" aria-labelledby="Notification_Setup_Modal_Label">
        <div class="modal-dialog modal-xl">
            <div class="modal-content bg-white">
                <div class="modal-header1">
                    <div class="col-11">
                        <h4 class="modal-title1" id="notification_SetupHeader">Add New Notification</h4>
                    </div>
                    <button class="col-1 close" type="button" data-dismiss="modal" aria-label="Close"><i class="pi-iconExit float-right mt-2 mr-2"></i></button>
                </div>
                <div class="modal-body1">
                    <div class="row mb-3">
                        <div class="col-6">
                            <label class="control-label">Name</label>
                            <input id="condition_name" type="text" class="form-control" name="condition_name"><span id="error_condition_name" class="text-danger"></span>
                        </div>
                        <div class="col-6">
                            <label class="control-label">Condition Options</label><div class="custom-control custom-switch">
                                <div class="custom-control custom-switch">
                                    <input id="condition_active" type="checkbox" class="custom-control-input" name="condition_active">
                                    <label class="custom-control-label" for="condition_active">Active Condition</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="form-group col-6">
                            <label class="control-label">Condition</label><input id="condition" type="text" class="form-control" name="condition"><span id="error_condition" class="text-danger"></span>
                        </div>
                        <div class="form-group col-6">
                            <label class="control-label">Condition Type</label>
                            <select id="condition_type" class="form-control" name="condition_type">
                                <option value=""></option>
                                <!--<option value="automation">Automation</option>-->
                                <option value="apiconnection">API Connection</option>
                                <option value="dockdoor">Dock Door</option>
                                <option value="mpe">MPE</option>
                                <option value="vehicle">Vehicle</option>
                                <option value="routetrip">RouteTrip</option>
                            </select>
                            <span id="error_condition_type" class="text-danger"></span>
                        </div>
                    </div>
                    <div class="row">
                        <div class="form-group col-6">
                            <div class="d-flex justify-content-center my-2">
                                <div class="w-75">
                                    <label class="control-label">Warning Condition: <span class="font-weight-bold text-primary ml-2 warning_conditionpickvalue"></span></label>
                                    <input type="range" class="custom-range" id="warning_condition" min="0" max="60" value="0">
                                </div>
                            </div>

                            <div class="col">
                                <label class="control-label">Warning Action</label><textarea id="warning_action" type="text" rows="3" class="form-control"></textarea><span id="error_warning_action" class="text-danger"></span>
                            </div>
                        </div>

                        <div class="col-6 form-group">
                            <div class="d-flex justify-content-center my-2">
                                <div class="w-75">
                                    <label class="control-label">Critical Condition: <span class="font-weight-bold text-primary ml-2 critical_conditionpickvalue"></span></label>
                                    <input type="range" class="custom-range" id="critical_condition" min="0" max="60" value="0">
                                </div>
                            </div>
                            <div class="col">
                                <label class="control-label">Critical Action</label><textarea id="critical_action" type="text" rows="3" class="form-control"></textarea><span id="error_critical_action" class="text-danger"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer1">
                    <div class="col">
                        <button class="btn btn-outline-secondary float-left" type="button" data-dismiss="modal">Close</button>
                    </div>
                    <div class="col text-center"><span class="text-info" id="error_notificationsubmitBtn"></span></div>
                    <div class="col">
                        <button class="btn btn-primary float-right" type="button" style="display: block;" id="notificationsubmitBtn">Save</button>
                        <button class="btn btn-primary float-right" type="button" style="display: none" id="editnotificationsubmitBtn">Save</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="RemoveNotificationModal" tabindex="-1" role="dialog" data-backdrop="static" aria-labelledby="RemoveNotificationModal_Label">
        <div class="modal-dialog modal-xl">
            <div class="modal-content bg-white">
                <div class="modal-header1">
                    <div class="col-11">
                        <h4 class="modal-title1" id="removeNotificationHeader"><span aria-hidden="true"></span></h4>
                    </div>
                    <button class="col-1 close" type="button" data-dismiss="modal" aria-label="Close"><i class="pi-iconExit float-right mt-2 mr-2"></i></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-12 text-center">
                            <h4 class="remove-body" style="color: red"></h4>
                            <h4 style="color: red">Do you want to proceed ?</h4>
                        </div>
                    </div>
                </div>
                <div class="modal-footer1">
                    <div class="col">
                        <button class="btn btn-outline-secondary float-left" type="button" data-dismiss="modal">Close</button>
                    </div>
                    <div class="col text-center"><span class="text-info " id="error_removeNotification"></span></div>
                    <div class="col">
                        <button class=" btn btn-primary float-right" type="button" id="removeNotification">Remove</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="AppSetting_value_Modal" tabindex="-1" role="dialog" style="z-index: 12000;" data-backdrop="static">
        <div class="modal-dialog" style="max-width: 50%;">
            <div class="modal-content bg-white">
                <div class="modal-header1">
                    <div class="col-11">
                        <h4 class="modal-title1" id="appsettingvaluemodalHeader"><span aria-hidden="true"></span></h4>
                    </div>
                    <button class="col-1 close" type="button" data-dismiss="modal" aria-label="Close"><i class="pi-iconExit float-right mt-2 mr-2"></i></button>
                </div>
                <div class="modal-body">
                    <div class="card">
                        <div class="card-body">
                            <div class="form-group row">
                                <div class="col">
                                    <label class="col-form-label">Attribute:</label><input class="form-control" disabled="disabled" id="modalKeyID">
                                </div>
                            </div>
                            <div class="form-group row valuediv" style="display:block">
                                <div class="col">
                                    <label class="col-form-label">Value:</label><input class="form-control" id="modalValueID"><span class="text-info" id="error_modalValueID"></span>
                                </div>
                            </div>
                            <div class="form-group row timezonediv" style="display:none">
                                <div class="col">
                                    <label class="col-form-label">Value:</label>
                                    <select class="form-control" id="timezoneValueID"></select>
                                    <span class="text-info" id="error_timezoneValueID"></span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer1">
                    <div class="col">
                        <button class="btn btn-outline-secondary float-left" type="button" data-dismiss="modal">Close</button>
                    </div>
                    <div class="col text-center"><span class="text-info" id="error_appsettingvalue"></span></div>
                    <div class="col">
                        <button class=" btn btn-primary float-right" type="button" id="appsettingvalue">Save</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="RemoveConfirmationModal" tabindex="-1" role="dialog" data-backdrop="static" aria-labelledby="RemoveConfirmationModal_Label">
        <div class="modal-dialog modal-xl">
            <div class="modal-content bg-white">
                <div class="modal-header1">
                    <div class="coll-11">
                        <h4 class="modal-title1" id="removeAPImodalHeader_ID"><span aria-hidden="true"></span></h4>
                    </div>
                    <button class="col-1 close" type="button" data-dismiss="modal" aria-label="Close"><i class="pi-iconExit float-right mt-2 mr-2"></i></button>
                </div>
                <div class="modal-body" id="modal_Add_AppSettingInfo_modalbody">
                    <div class="row">
                        <div class="col-12 text-center">
                            <h4 class="remove-body" style="color: red"></h4>
                            <h4 style="color: red">Do you want to proceed ?</h4>
                        </div>
                    </div>
                </div>
                <div class="modal-footer1">
                    <div class="col">
                        <button class="btn btn-outline-secondary float-left" type="button" data-dismiss="modal">Close</button>
                    </div>
                    <div class="col text-center"><span class="text-info " id="error_remove_server_connection"></span></div>
                    <div class="col">
                        <button class=" btn btn-primary float-right" type="button" id="remove_server_connection">Remove</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="Remove_Layer_Modal" tabindex="-1" role="dialog" data-backdrop="static" aria-labelledby="Remove_Layer_Modal_Label">
        <div class="modal-dialog modal-xl">
            <div class="modal-content bg-white">
                <div class="modal-header1">
                    <div class="coll-11">
                        <h4 class="modal-title1" id="removeLayermodalHeader_ID"><span aria-hidden="true"></span></h4>
                    </div>
                    <button class="col-1 close" type="button" data-dismiss="modal" aria-label="Close">
                        <i class="pi-iconExit float-right mt-2 mr-2"></i>
                    </button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-12 text-center">
                            <h4 class="remove-body" style="color: red"></h4>
                            <h4 style="color: red">Do you want to proceed ?</h4>
                        </div>
                    </div>
                </div>
                <div class="modal-footer1">
                    <div class="col">
                        <button class="btn btn-outline-secondary float-left" type="button" data-dismiss="modal">Close</button>
                    </div>
                    <div class="col text-center"><span class="text-info " id="error_remove_layer"></span></div>
                    <div class="col">
                        <button class=" btn btn-primary float-right" type="button" id="remove_layer">Remove</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="CTS_Details_Modal" tabindex="-1" role="dialog" style="z-index: 12000;" data-backdrop="static">
        <div class="modal-dialog" style="max-width: 97%;">
            <div class="modal-content bg-white">

                <div class="modal-header1">
                    <div class="col-11">
                        <h4 class="modal-title1" id="ctsdetailsmodalHeader"><span aria-hidden="true"></span></h4>
                    </div>
                    <button class="col-1 close" type="button" data-dismiss="modal" aria-label="Close"><i class="pi-iconExit float-right mt-2 mr-2"></i></button>
                </div>
                <div class="modal-body pt-0">
                    <!-- Preloader -->
                    <div id="modal-preloader">
                        <div class="modal-preloader_status">
                            <div class="modal-preloader_spinner">
                                <div class="d-flex justify-content-center">
                                    <div class="spinner-border" role="status"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!-- End Preloader -->
                    <div class="table-responsive">
                        <table class="table table-striped table-sm table-hover" id="ctsdetailstable">
                            <thead class="thead-dark"></thead>
                            <tbody></tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="col">
                        <button class="btn btn-outline-secondary float-left" type="button" data-dismiss="modal">Close</button>
                    </div>
                    <div class="col text-center"><span class="text-info" id="error_ctsdetails"></span></div>
                </div>
            </div>
        </div>
    </div>
    <!-- Camera Modal-->
    <div class="modal fade" id="Camera_Modal" tabindex="-1" role="dialog" style="z-index: 12000;" data-backdrop="static">
        <div class="modal-dialog" style="max-width: 75%;">
            <div class="modal-content bg-white">
                <div class="modal-header">
                    <div class="col-4 float-left">
                        <h4 class="modal-title1" id="cameramodalHeader"><span aria-hidden="true"></span></h4>
                    </div>
                    <div class="col-6 text-center">
                        <h4 id="cameradescription"><span aria-hidden="true"></span></h4>
                    </div>
                    <button class="col-1 close" type="button" data-dismiss="modal" aria-label="Close"><i class="pi-iconExit float-right mt-2 mr-2"></i></button>
                </div>
                <div class="modal-body pt-0 text-center" id="camera_modalbody">
                </div>
                <div class="modal-footer">
                    <div class="col">
                        <button class="btn btn-outline-secondary float-left" type="button" data-dismiss="modal">Close</button>
                    </div>
                    <div class="col text-center"><span class="text-info" id="error_camera"></span></div>
                </div>
            </div>
        </div>
    </div>
    <!--Add Camera Modal-->
    <div class="modal fade" id="AddEdit_Camera_Modal" tabindex="-1" role="dialog" style="z-index: 12000;" data-backdrop="static">
        <div class="modal-dialog" style="max-width: 75%;">
            <div class="modal-content bg-white">
                <div class="modal-header">
                    <div class="col-4 float-left">
                        <h4 class="modal-title1" id="addedit_cameramodalHeader"><span aria-hidden="true"></span></h4>
                    </div>
                    <div class="col-6 text-center">
                        <h4 id="cameradescription"><span aria-hidden="true"></span></h4>
                    </div>
                    <button class="col-1 close" type="button" data-dismiss="modal" aria-label="Close"><i class="pi-iconExit float-right mt-2 mr-2"></i></button>
                </div>
                <div class="modal-body pt-0 text-center" id="addedit_camera_modalbody">
                    <div class="row mb-3">
                        <div class="form-group col-4">
                            <label class="control-label">Camera Name</label>
                            <input id="camera_name" type="text" class="form-control" name="camera_name">
                            <span id="error_camera_name" class="text-danger"></span>
                        </div>
                        <div class="form-group col-8">
                            <label class="control-label">Description</label>
                            <input id="camera_description" type="text" class="form-control" name="camera_description">
                            <span id="error_camera_description" class="text-danger"></span>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="form-group col-4">
                            <label class="control-label">Camera Model</label>
                            <input id="camera_model" type="text" class="form-control" name="camera_model">
                            <span id="error_camera_model" class="text-danger"></span>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="col">
                        <button class="btn btn-outline-secondary float-left" type="button" data-dismiss="modal">Close</button>
                    </div>
                    <div class="col text-center"><span class="text-info" id="error_camera_submitBtn"></span></div>
                    <div class="col">
                        <button class="btn btn-primary float-right" type="button" id="camera_submitBtn">Save</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="UserTag_Modal" tabindex="-1" role="dialog" data-backdrop="static" aria-labelledby="API_Connection_Modal_Label">
        <div class="modal-dialog modal-xl">
            <div class="modal-content bg-white">
                <div class="modal-header1">
                    <div class="col-11">
                        <h4 class="modal-title1" id="modaluserHeader_ID"><span aria-hidden="true"></span></h4>
                    </div>
                    <button class="col-1 close" type="button" data-dismiss="modal" aria-label="Close"><i class="pi-iconExit float-right mt-2 mr-2"></i></button>
                </div>
                <div class="modal-body">
                    <div class="row mr-0 ml-0">
                        <div class="form-group col-3">
                            <label class="control-label">Name</label><input id="employee" type="text" class="form-control" name="employee_name"><span id="error_employee_name" class="text-danger"></span>
                        </div>
                        <div class="form-group col-3">
                            <label class="control-label">EIN</label><input id="employee_ein" type="text" class="form-control" name="employee_ein"><span id="error_employee_ein" class="text-danger"></span>
                        </div>
                        <div class="form-group col-4">
                            <label class="control-label">TAG Name</label><input id="tag_name" type="text" class="form-control" name="tag_name" disabled><span id="error_tag_name" class="text-danger"></span>
                        </div>
                    </div>
                    <div class="row mr-0 ml-0">
                        <div class="form-group col-6">
                            <label class="control-label">Tag MAC</label><input id="tag_id" type="text" class="form-control" name="tag_id" disabled><span id="error_tag_id" class="text-danger"></span>
                        </div>
                    </div>
                </div>
                <div class="modal-footer1">
                    <div class="col">
                        <button class="btn btn-outline-secondary float-left" type="button" data-dismiss="modal">Close</button>
                    </div>
                    <div class="col text-center"><span class="text-info" id="error_usertagsubmitBtn"></span></div>
                    <div class="col">
                        <button class="btn btn-primary float-right" type="button" id="usertagsubmitBtn">Save</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="Zone_Modal" tabindex="-1" role="dialog" data-backdrop="static" aria-labelledby="Zone_Modal_Label">
        <div class="modal-dialog modal-xl">
            <div class="modal-content bg-white">
                <div class="modal-header1">
                    <div class="col-11">
                        <h4 class="modal-title1" id="modalZoneHeader_ID"><span aria-hidden="true"></span></h4>
                    </div>
                    <button class="col-1 close" type="button" data-dismiss="modal" aria-label="Close"><i class="pi-iconExit float-right mt-2 mr-2"></i></button>
                </div>
                <div class="modal-body">
                    <div class="row mr-0 ml-0">
                        <div class="form-group col">
                            <label class="control-label">Zone ID</label><input id="machine_id" type="text" class="form-control" name="machine_id" disabled><span id="error_machine_id" class="text-danger"></span>
                        </div>
                    </div>
                    <div class="row mr-0 ml-0">
                        <div class="form-group col-4">
                            <label class="control-label">Name</label>
                            <%--<input id="machine_zone_name" type="text" class="form-control" name="machine_zone_name" style="display:block;">--%>
                            <select id="machine_zone_select_name" title="Zone Select Name" class="form-control" name="machine_zone_select_name">
                                <option value=""></option>
                            </select>
                            <span id="error_machine_zone_name" class="text-danger"></span>
                        </div>
                    </div>
                    <div class="row mr-0 ml-0" id="machine_manual_row">
                        <div class="form-group col">
                            <label class="control-label">Name</label><input id="machine_name" type="text" class="form-control" name="machine_name"/><span id="errormachine_name" class="text-danger"></span>
                        </div>
                        <div class="form-group col-4">
                            <label class="control-label">Number</label><input id="machine_number" type="text" class="form-control" name="machine_number"/><span id="error_machine_number" class="text-danger"></span>
                        </div>
                    </div>
                    <div class="row mr-0 ml-0">
                        <div class="form-group col-4">
                            <label class="control-label">Zone LDC</label><input id="zone_ldc" type="text" class="form-control" name="zone_ldc"><span id="errorzone_ldc" class="text-danger"></span>
                        </div>
                    </div>

                </div>
                <div class="modal-footer1">
                    <div class="col">
                        <button class="btn btn-outline-secondary float-left" type="button" data-dismiss="modal">Close</button>
                    </div>
                    <div class="col text-center"><span class="text-info" id="error_machinesubmitBtn"></span></div>
                    <div class="col">
                        <button class="btn btn-primary float-right" type="button" id="machinesubmitBtn">Save</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="ContainerDetails_Modal" tabindex="-1" style="z-index: 12000;" role="dialog" data-backdrop="static" aria-labelledby="ContainerDetails_Modal_Label">
        <div class="modal-dialog" style="max-width: 85%;">
            <div class="modal-content bg-white">
                <div class="modal-header1">
                    <div class="col-11">
                        <h4 class="modal-title1" id="modalContainerDetailsHeader_ID"><span aria-hidden="true"></span></h4>
                    </div>
                    <button class="col-1 close" type="button" data-dismiss="modal" aria-label="Close"><i class="pi-iconExit float-right mt-2 mr-2"></i></button>
                </div>
                <div class="modal-body">
                    <div class="card-body pb-0">
                        <div class="table-responsive">
                            <table class="table table-striped table-sm table-hover mb-0 border-bottom" id="containerdetailstable">
                                <thead class="thead-dark">
                                    <tr>
                                        <th style="width: 4%;">Site</th>
                                        <th style="width: 10%;">Site Name</th>
                                        <th style="width: 9%;">Site Type</th>
                                        <th style="width: 7%;">Event</th>
                                        <th style="width: 15%;">Event Time</th>
                                        <th style="width: 10%;">Location</th>
                                        <th style="width: 5%;">Bin</th>
                                        <th style="width: 10%; ">Trailer</th>
                                        <th style="width: 5%;">Route</th>
                                        <th style="width: 5%;">Trip</th>
                                        <th style="width: 10%;">Source</th>
                                        <th style="width: 10%;">User</th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>
                    </div>
                </div>
                <div class="modal-footer1">
                    <div class="col">
                        <button class="btn btn-outline-secondary float-left" type="button" data-dismiss="modal">Close</button>
                    </div>
                    <div class="col text-center"><span class="text-info" id="error_containerdetailsubmitBtn"></span></div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="RouteTripDetails_Modal" tabindex="-1" style="z-index: 12000;" role="dialog" data-backdrop="static" aria-labelledby="RouteTrip_Modal_Label">
        <div class="modal-dialog" style="max-width: 85%;">
            <div class="modal-content bg-white">
                <div class="modal-header1">
                    <div class="col-11">
                        <h3 class="modal-title1" id="modalRouteTripDetailsHeader_ID"><span aria-hidden="true"></span></h3>
                    </div>
                    <button  class="col-1 close" type="button" data-dismiss="modal" aria-label="Close"><i  class="pi-iconExit float-right mt-2 mr-2"></i></button>
                </div>
                <div class="modal-body" id="RouteTripDetailscard">
                    <div class="card-body pb-0">

                    </div>
                </div>
                <div class="modal-footer1">
                    <div class="col">
                        <button class="btn btn-outline-secondary float-left" type="button" data-dismiss="modal">Close</button>
                    </div>
                    <div class="col text-center"><span class="text-info" id="error_containerdetailsubmitBtn"></span></div>
                </div>
            </div>
        </div>
    </div>

    <!-- Sidebar panel content -->
    <div class="leaflet-sidebar leaflet-control leaflet-sidebar-left leaflet-touch collapsed" id="sidebar">
        <div class="leaflet-sidebar-content">
            <h1 id="fotf-site-facility-name" class="leaflet-sidebar-header d-flex">
                <span class="slantedLogoDivider"></span>
                <i id="fotf-sidebar-close" class="leaflet-sidebar-close pi-iconExit"></i>
            </h1>

            <!-- Sidebar panel Home - Zone Selector -->
            <div class="leaflet-sidebar-pane overflow-auto" id="home">
                <div id="div_zoneSelect">
                    <div id="zoneselectlist_div" class="d-flex mt-1" style="display: block;">
                        <h4 class="ml-p5rem">Zone Status</h4>
                        <div class="form-group flex-fill pl-3 mb-0 mr-p5rem">
                            <!-- <label class="mb-1">Select a Zone</label> -->
                            <select class="form-control" id="zoneselect" placeholder="Begin by selecting a zone">
                                <option value=""></option>
                            </select>
                            <span class="text-danger" name="eroor_zoneselect"></span>
                        </div>
                    </div>

                    <div id="area_div" data-id="" class="card bg-white mt-2 pb-1" style="display: none;">
                        <div class="card-header pl-1">
                            <h6 class="control-label sectionHeader mb-1">Area Info</h6>
                        </div>
                        <div id="div_area">
                            <div class="card-body pb-0">
                                <div class="table-responsive">
                                    <table class="table table-striped table-sm table-hover mb-0 border-bottom" id="areazonetoptable">
                                        <tbody></tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="bullpen_div" data-id="" class="card bg-white mt-2 pb-1" style="display: none;">
                        <div class="card-header pl-1">
                            <h6 class="control-label sectionHeader mb-1">Staging Bullpen Area</h6>
                        </div>
                        <div id="div_area">
                            <div class="card-body pb-0">
                                <div class="table-responsive">
                                    <table class="table table-striped table-sm table-hover mb-0 border-bottom" id="bullpenzonetoptable">
                                        <tbody></tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="layer_div" data-id="" class="card bg-white mt-2 pb-1" style="display: none;">
                        <div class="card-header pl-1">
                            <h6 class="control-label sectionHeader mb-1">Create Layer</h6>
                        </div>
                        <div id="div_layerarea">
                            <div class="card">
                                <div class="card-body pb-0">
                                    <div class="row mb-3">
                                        <div class="form-group col-6">
                                            <label class="control-label">Type</label>
                                            <select id="zone_type" title="Zone Type" class="form-control" name="zone_type">
                                                <option value=""></option>
                                                <option value="AGVLocation">AGV Location Zone</option>
                                                <option value="Area">Area Zone</option>
                                                <option value="Bin">BIN Zone</option>
                                                <option value="Camera">Camera</option>
                                                <option value="DockDoor">Dock Door Zone</option>
                                                <option value="Machine">Machine Zone</option>
                                                <option value="Bullpen">Bullpen Zone</option>
                                                <option value="ViewPorts">View Ports</option>

                                            </select>
                                            <span id="error_zone_type" class="text-danger"></span>
                                        </div>
                                        <div class="col-6">
                                            <label class="control-label">Name</label>
                                            <input id="zone_name" type="text" class="form-control" name="zone_name" style="display:block;">
                                            <select id="zone_select_name" type="text" title="Zone Select Name" class="form-control" name="zone_select_name" style="display:none;">
                                                <option value=""></option>
                                            </select>
                                            <span id="error_zone_name" class="text-danger"></span>
                                        </div>
                                        <div class="row mr-0 ml-0" id="new_machine_manual_row" style="display:none;">
                                            <div class="form-group col">
                                                <label class="control-label">Name</label><input id="new_machine_name" type="text" class="form-control" name="new_machine_name"/><span id="error_new_machine_name" class="text-danger"></span>
                                            </div>
                                            <div class="form-group col-4">
                                                <label class="control-label">Number</label><input id="new_machine_number" type="text" class="form-control" name="new_machine_number"/><span id="error_new_machine_number" class="text-danger"></span>
                                            </div>
                                        </div>

                                    </div>
                                    <div id="camerainfo" style="display:none;">
                                        <div class="row mb-3">
                                            <div class="form-group col">
                                                <label class="control-label">Camera URL</label>
                                                <select id="cameraLocation" class="form-control" name="cameraLocation">
                                                    <option value=""></option>
                                                </select>
                                                <span id="error_cameraLocation" class="text-danger"></span>
                                            </div>
                                        </div>
                                    </div>
                                    <div id="binzoneinfo" style="display:none;">
                                        <div class="row mb-3">
                                            <div class="col">
                                                <label class="control-label">Bins</label>
                                                <textarea id="bin_bins" type="text" rows="3" class="form-control"></textarea>
                                                <span id="error_bin_bins" class="text-danger"></span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="card-footer">
                                    <div class="col">
                                        <button class="btn btn-outline-secondary float-left" type="button" data-dismiss="modal" id="zonecloseBtn">Cancel</button>
                                    </div>
                                    <div class="col text-center"><span class="text-info" id="error_zonesubmitBtn"></span></div>
                                    <div class="col">
                                        <button class="btn btn-primary float-right" type="button" style="display: block;" id="zonesubmitBtn">Save</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="machine_div" data-id="" class="card bg-white mt-2 pb-1" style="display: none; ">
                        <div class="card-header pl-1">
                            <h6 class="control-label sectionHeader ml-1 mb-1 d-flex justify-content-between">
                                Machine Info
                                <button type="button" class="btn btn-secondary border-0 badge-info badge machineinfoedit" name="machineinfoedit" style="display:none" data-id="0" data-toggle="tooltip" data-container="body" data-placement="top">Edit</button>
                            </h6>
                        </div>
                        <div id="div_machine">
                            <div class="card-body pb-0">
                                <div class="table-responsive">
                                    <table class="table table-striped table-sm table-hover mb-0 border-bottom" id="machinetable">
                                        <tbody></tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="dps_div" class="card bg-white mt-2 pb-1" style="display: none; ">
                        <div class="card-header pl-1">
                            <h6 class="control-label sectionHeader ml-1 mb-1">DPS Info</h6>
                        </div>
                        <div id="div_dps">
                            <div class="card-body overflow-auto pb-0">
                                <div class="table-responsive">
                                    <table class="table table-sm table-hover mb-0 border-bottom" id="dpstable">
                                        <tbody></tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="agvlocation_div" data-id="" class="card bg-white mt-2 pb-1" style="display: none; ">
                        <div class="card-header pl-1 border-bottom">
                            <h6 class="control-label sectionHeader ml-1 mb-1 d-flex justify-content-between">
                                AGV Location Info
                                <span class="btn btn-secondary border-0 badge-info badge" name="locationid"></span>
                            </h6>
                        </div>
                        <div id="div_location_mission">
                            <div class="table-responsive">
                                <table class="table table-sm table-hover mb-0 border-bottom" id="locationinmissiontable">
                                    <thead class="thead-dark"></thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>
                    </div>

                    <div id="dockdoor_div" data-id="" class="card bg-white mt-2 pb-1" style="display: none; ">
                        <div class="card-header pl-1 border-bottom">
                            <h6 class="control-label sectionHeader ml-1 mb-1 d-flex justify-content-between">
                                Dock Door
                                <span class="btn btn-secondary border-0 badge-info badge" name="doornumberid"></span>
                                <span class="d-flex justify-content-between">
                                    Status:
                                    <span class="btn btn-secondary border-0 badge-info badge" name="doorstatus"></span>
                                </span>
                            </h6>
                        </div>
                        <div class="card-body pb-0" id="div_dockdoor">
                            <div class="table-responsive">
                                <table class="table table-sm table-hover mb-0 border-bottom" id="dockdoortable">
                                    <thead class="thead-dark"></thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>
                    </div>

                    <div id="vehicle_div" data-id="" class="card bg-white mt-2 pb-1" style="display: none; ">
                        <div class="card-header pl-1 d-flex flex-row">
                            <h6 class="control-label sectionHeader ml-1">Vehicle Info</h6>
                            <div class="custom-control custom-switch ml-3">
                                <input id="followvehicle" type="checkbox" class="custom-control-input" name="followvehicle">
                                <label class="vehicleInfo custom-control-label" for="followvehicle">Follow Vehicle</label>
                            </div>
                        </div>

                        <div id="div_vehicle">
                            <div class="table-responsive">
                                <table class="table table-sm table-hover mb-0 border-bottom" id="vehicletable">
                                    <thead class="thead-dark"></thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>
                        <div id="div_vehicle_mission">
                            <div class="table-responsive">
                                <table class="table table-sm table-hover mb-0 border-bottom" id="vehiclemissiontable">
                                    <thead class="thead-dark"></thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>
                    </div>

                    <!-- Trailer Content table template -->
                    <div id="trailer_div" class="card bg-white mt-2 pb-1" style="display: block; ">
                        <div class="card-header pl-1">
                            <h6 class="control-label sectionHeader ml-1 mb-1 d-flex justify-content-between">
                                Trailer Content
                                <button type="button" class="btn btn-secondary border-0 badge-info badge" data-toggle="tooltip" data-container="body" data-placement="top" title="Loaded vs. Not Loaded" name="container_counts">0/0</button>
                            </h6>
                        </div>
                        <div>
                            <div class="card-body pb-0">
                                <div class="table-responsive fixedHeader variableHeight filterable">
                                    <table class="table table-striped table-sm table-hover mb-0 border-bottom" id="containertable">
                                        <thead class="thead-dark">
                                            <tr>
                                                <th style="width: 15%; "><span class="ml-p25rem">Dest</span></th>
                                                <th style="width: 20%; ">Location</th>
                                                <th style="width: 5%; "></th>
                                                <th style="width: 47%; ">Placard</th>
                                                <th style="width: 12%; ">Status</th>
                                            </tr>
                                            <tr class="filters">
                                                <td><input class="frow-comp-1" id="dest" type="text" placeholder="Dest"></td>
                                                <td><input class="frow-comp-2" id="location" type="text" placeholder="Location"></td>
                                                <td></td>
                                                <td><input class="frow-comp-3" id="placard" type="text" placeholder="Placard"></td>
                                                <td><input class="frow-comp-4" id="status" type="text" placeholder="Status"></td>
                                            </tr>
                                        </thead>
                                        <tbody></tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!-- END -->

                    <div id="staff_div" class="card bg-white mt-2 pb-1" style="display: none; ">
                        <div class="card-header pl-1">
                            <h6 class="control-label sectionHeader ml-1 mb-1">Staffing Info</h6>
                        </div>
                        <div id="div_staff">
                            <div class="card-body pb-0">
                                <div class="table-responsive">
                                    <table class="table table-sm table-hover mb-0 border-bottom" id="stafftable">
                                        <thead class="thead-dark">
                                            <tr>
                                                <th>F1 Craft</th>
                                                <th>Current</th>
                                                <th>Planned</th>
                                            </tr>
                                        </thead>
                                        <tbody></tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="ctstabs_div" class="card bg-white" style="display: block;">
                        <div class="card-header pl-1 border-bottom">
                            <h6 class="control-label sectionHeader ml-1 mb-1">Arrive/Depart Trips</h6>
                        </div>
                        <ul id="cts_tabs" class="nav nav-tabs flex-column flex-sm-row card-header pt-0 px-1 mb-1">
                            <li class="nav-item"><a href="#cts_1" class="nav-link active" data-toggle="tab">OB Network</a></li>
                            <li class="nav-item"><i class="pi-iconDividerSlanted"></i></li>
                            <li class="nav-item"><a href="#cts_2" class="nav-link" data-toggle="tab">OB Local</a></li>
                            <li class="nav-item"><i class="pi-iconDividerSlanted"></i></li>
                            <li class="nav-item"><a href="#cts_3" class="nav-link" data-toggle="tab">Out Scheduled</a></li>
                            <li class="nav-item"><i class="pi-iconDividerSlanted"></i></li>
                            <li class="nav-item"><a href="#cts_4" class="nav-link" data-toggle="tab">In Scheduled</a></li>
                        </ul>
                        <div class="tab-content">
                            <div class="tab-pane fade show active" id="cts_1">
                                <div id="div_cts_dockdeparted">
                                    <div class="card-body">
                                        <div class="table-responsive fixedHeader variableHeight filterable">
                                            <table class="table table-striped table-sm table-hover mb-1 border-bottom" id="ctsdockdepartedtable">
                                                <thead class="thead-dark">
                                                    <tr>
                                                        <th class="row-cts-schd"><span class="ml-p25rem">Schd</span></th>
                                                        <th class="row-cts-depart">Deprt</th>
                                                        <th class="row-cts-rt">Route-Trip</th>
                                                        <th class="row-cts-door">Door</th>
                                                        <th class="row-cts-leg">Leg</th>
                                                        <th class="row-cts-dest">Dest</th>
                                                        <!--<th class="row-cts-close">Close</th>
                                        <th class="row-cts-load">Load</th>
                                        <th>Load%</th>-->
                                                    </tr>
                                                </thead>
                                                <tbody></tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="tab-pane fade" id="cts_2">
                                <div id="div_cts_localdeparted">

                                    <div class="card-body">
                                        <div class="table-responsive fixedHeader variableHeight">
                                            <table class="table table-striped table-sm table-hover mb-1 border-bottom" id="ctslocaldockdepartedtable">
                                                <thead class="thead-dark">
                                                    <tr>
                                                        <th class="row-cts-schd"><span class="ml-p25rem">Schd</span></th>
                                                        <th class="row-cts-depart">Deprt</th>
                                                        <th class="row-cts-rt">Route-Trip</th>
                                                        <th class="row-cts-door">Door</th>
                                                        <th class="row-cts-leg">Leg</th>
                                                        <th class="row-cts-dest">Dest</th>
                                                        <!--<th class="row-cts-close">Close</th>
                                        <th class="row-cts-load">Load</th>
                                        <th>Load%</th>-->
                                                    </tr>
                                                </thead>
                                                <tbody></tbody>
                                            </table>
                                        </div>
                                    </div>

                                </div>
                            </div>
                            <div class="tab-pane fade" id="cts_3">
                                <div id="div_cts_outbound">
                                    <div class="bg-white px-1">
                                        <div class="card-body">
                                            <div class="table-responsive fixedHeader variableHeight">
                                                <table class="table table-striped table-sm table-hover mb-1 border-bottom" id="ctsouttoptable">
                                                    <thead class="thead-dark">
                                                        <tr>
                                                            <th class="row-cts-schd"><span class="ml-p25rem">Schd</span></th>
                                                            <th class="row-cts-depart">Departed</th>
                                                            <th class="row-cts-rt">Route-Trip</th>
                                                            <th class="row-cts-door">Door</th>
                                                            <th class="row-cts-leg">Dest</th>
                                                            <th class="row-cts-dest">Site Name</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody></tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="tab-pane fade" id="cts_4">
                                <div id="div_cts_inbound">
                                    <div class="bg-white px-1">
                                        <div class="card-body">
                                            <div class="table-responsive fixedHeader variableHeight">
                                                <table class="table table-striped table-sm table-hover mb-1 border-bottom" id="ctsintoptable">
                                                    <thead class="thead-dark">
                                                        <tr>
                                                            <th class="row-cts-schd"><span class="ml-p25rem">Schd</span></th>
                                                            <th class="row-cts-depart">Arrived</th>
                                                            <th class="row-cts-rt">Route-Trip</th>
                                                            <th class="row-cts-door">Door</th>
                                                            <th class="row-cts-leg">Leg Origin</th>
                                                            <th class="row-cts-dest">Site Name</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody></tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!-- Sidebar panel Report Information -->
            <div class="leaflet-sidebar-pane overflow-auto" id="reports">
                <h4 class="ml-p5rem">Report Information</span></h4>
                <div class="btn-toolbar" role="toolbar" id="userinfo_div">
                    <div id="div_userinfo" class="container-fluid">
                        <div class="card w-100">
                            <div class="card-header pl-1">
                                <h6 class="sectionHeader ml-1 mb-1">Craft Type</h6>
                            </div>
                            <div class="card-body overflow-auto">
                                <div class="table-responsive">
                                    <table class="table table-sm table-hover mb-1 border-bottom" id="userstoptable">
                                        <thead class="thead-dark">
                                            <tr><th><span class="ml-p25rem">F1 Craft</span></th><th>In Work Zone</th></tr>
                                        </thead>
                                        <tbody></tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>


                    <div id="div_overtimeinfo" class="container-fluid ">
                        <div class="card w-100">
                            <div class="card-header pl-1">
                                <h6 class="sectionHeader ml-1 mb-1">Overtime Badges <span class="badge-info badge" name="overtime_count">0</span></h6>
                                <p class="smaller text-secondary mb-0 ml-2">Overtime List</p>
                            </div>
                            <div class="card-body">
                                <div class="table-responsive fixedHeader variableHeight filterable">
                                    <table class="table table-striped table-sm table-hover mb-1 border-bottom" id="overtimebagestable">
                                        <thead class="thead-dark">
                                            <tr>
                                                <th class="row-comp-1"><span class="ml-p25rem">Badge</span></th>
                                                <th class="row-comp-2">Overtime</th>
                                            </tr>
                                            <tr class="filters">
                                                <td><input class="frow-comp-1" id="badge" type="text" placeholder="Badge"></td>
                                                <td><input class="frow-comp-2" id="overtimebage_time" type="text" placeholder="Overtime"></td>
                                            </tr>
                                        </thead>
                                        <tbody></tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="div_complianceinfo" class="container-fluid ">
                        <div class="card w-100">
                            <div class="card-header pl-1">
                                <h6 class="sectionHeader ml-1 mb-1">Undetected Badges <span class="badge-info badge" name="undetected_count">0</span></h6>
                                <p class="smaller text-secondary mb-0 ml-2">Verify badges for these users are being worn and are functioning</p>
                            </div>
                            <div class="card-body">
                                <div class="table-responsive fixedHeader variableHeight filterable">
                                    <table class="table table-striped table-sm table-hover mb-1 border-bottom" id="compliancetable">
                                        <thead class="thead-dark">
                                            <tr>
                                                <th class="row-comp-1"><span class="ml-p25rem">Badge</span></th>
                                                <th class="row-comp-2">LDC</th>
                                                <th class="row-comp-3">OP Code</th>
                                                <th class="row-comp-4">Pay Location</th>
                                            </tr>
                                            <tr class="filters">
                                                <td><input class="frow-comp-1" id="badge" type="text" placeholder="Badge"></td>
                                                <td><input class="frow-comp-2" id="ldc" type="text" placeholder="LDC"></td>
                                                <td><input class="frow-comp-3" id="opcode" type="text" placeholder="Op Code"></td>
                                                <td><input class="frow-comp-4" id="paylocation" type="text" placeholder="Pay Location"></td>
                                            </tr>
                                        </thead>
                                        <tbody></tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="div_notvaildldcinfo" class="container-fluid">
                        <div class="card w-100">
                            <div class="card-header pl-1">
                                <h6 class="sectionHeader ml-1 mb-1">LDC to Work Zone Anomaly <span class="badge-info badge" name="ldcaler_count">0</span></h6>
                                <p class="smaller text-secondary mb-0 ml-2">Check employees’ TACS LDC. SELS Badge is physically present in a different LDC.</p>
                            </div>

                            <div class="card-body">
                                <div class="table-responsive fixedHeader variableHeight filterable">
                                    <table class="table table-sm table-hover mb-1 border-bottom" id="ldcalerttable">
                                        <thead class="thead-dark">
                                            <tr>
                                                <th class="row-comp-1"><span class="ml-p25rem">Badge</span></th>
                                                <th class="row-comp-2">LDC</th>
                                                <th class="row-comp-3">TACS LDC</th>
                                                <th class="row-comp-2">OPN</th>
                                                <th class="row-comp-2">P/L</th>
                                            </tr>
                                            <tr class="filters">
                                                <td><input class="frow-comp-1" id="badge" type="text" placeholder="Badge"></td>
                                                <td><input class="frow-comp-2" id="ldc" type="text" placeholder="LDC"></td>
                                                <td><input class="frow-comp-3" id="tacsldc" type="text" placeholder="TACS LDC"></td>
                                                <td><input class="frow-comp-3" id="opcode" type="text" placeholder="Op Code"></td>
                                                <td><input class="frow-comp-4" id="paylocation" type="text" placeholder="P/L"></td>
                                            </tr>
                                        </thead>
                                        <tbody></tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!-- Sidebar panel Trips Notification -->
            <div class="leaflet-sidebar-pane overflow-auto" id="tripsnotificationinfo">
                <h4 class="ml-p5rem">Trips Notification</h4>
                <div class="btn-toolbar" role="toolbar" id="tripsnotificationinfo_div">
                    <div id="div_tripsnotification" class="container-fluid">

                        <div class="accordion" id="tripsaccordion">

                            <div class="card w-100">
                                <div class="card-header pl-1">
                                    <h6 class="sectionHeader ml-1 mb-1">
                                        Late Trips
                                        <button class="btn btn-link" data-toggle="collapse" data-target="#latetrips" onclick="">
                                            <i class="bi bi-arrows-collapse"></i>
                                        </button>
                                        <!--<a href="#latetrips" data-toggle="collapse" class="bi-arrows-collapse"></a>-->
                                    </h6>

                                </div>
                                <div class="card-body collapse show" id="latetrips">
                                    <div class="table-responsive fixedHeader">
                                        <table class="table table-sm table-hover table-condensed mb-1 border-bottom" id="tripsnotificationtable">
                                            <thead class="thead-dark">
                                                <tr>
                                                    <th class="row-cts-schd"><span class="ml-p25rem">Schd</span></th>
                                                    <th style="width:min-content">Duration</th>
                                                    <th style="width:30px">DIR</th>
                                                    <th style="width:min-content">Route-Trip</th>
                                                    <th style="width:min-content" colspan="2">Site Name</th>
                                                </tr>
                                            </thead>
                                            <tbody></tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>

                            <div class="card w-100">
                                <div class="card-header pl-1">
                                    <h6 class="sectionHeader ml-1 mb-1">
                                        Load Scan After Departure
                                        <button class="btn btn-link" data-toggle="collapse" data-target="#loadafterdepart">
                                            <i class="bi bi-arrows-collapse"></i>
                                        </button>
                                    </h6>
                                </div>
                                <div class="card-body collapse show" id="loadafterdepart">
                                    <div class="table-responsive fixedHeader">
                                        <table class="table table-sm table-hover table-condensed mb-1 border-bottom" id="loadafterdeparttable">
                                            <thead class="thead-dark">
                                                <tr>
                                                    <th colspan="2">Placard</th>
                                                    <th>Load Scan</th>
                                                    <th colspan="2">Trailer</th>
                                                    <th colspan="2">Depart Scan</th>
                                                </tr>
                                            </thead>
                                            <tbody></tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>

                            <div class="card w-100">
                                <div class="card-header pl-1">
                                    <h6 class="sectionHeader ml-1 mb-1">
                                        Missing Closed Scan
                                        <button class="btn btn-link" data-toggle="collapse" data-target="#missingclosed">
                                            <i class="bi bi-arrows-collapse"></i>
                                        </button>
                                    </h6>
                                </div>
                                <div class="card-body collapse show" id="missingclosed">
                                    <div class="table-responsive fixedHeader">
                                        <table class="table table-sm table-hover table-condensed mb-1 border-bottom" id="missingclosedtable">
                                            <thead class="thead-dark">
                                                <tr>
                                                    <th colspan="8">Placard</th>
                                                </tr>
                                            </thead>
                                            <tbody></tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                            <div class="card w-100">
                                <div class="card-header pl-1">
                                    <h6 class="sectionHeader ml-1 mb-1">
                                        Missing Assigned Scan
                                        <button class="btn btn-link" data-toggle="collapse" data-target="#missingassigned">
                                            <i class="bi bi-arrows-collapse"></i>
                                        </button>
                                    </h6>
                                </div>
                                <div class="card-body collapse show" id="missingassigned">
                                    <div class="table-responsive fixedHeader">
                                        <table class="table table-sm table-hover table-condensed mb-1 border-bottom" id="missingassignedtable">
                                            <thead class="thead-dark">
                                                <tr>
                                                    <th colspan="8">Placard</th>
                                                </tr>
                                            </thead>
                                            <tbody></tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                            <div class="card w-100">
                                <div class="card-header pl-1">
                                    <h6 class="sectionHeader ml-1 mb-1">
                                        Missing Arrival Scan
                                        <button class="btn btn-link" data-toggle="collapse" data-target="#missingarrival">
                                            <i class="bi bi-arrows-collapse"></i>
                                        </button>
                                    </h6>
                                </div>
                                <div class="card-body collapse show" id="missingarrival">
                                    <div class="table-responsive fixedHeader">
                                        <table class="table table-sm table-hover table-condensed mb-1 border-bottom" id="missingarrivaltable">
                                            <thead class="thead-dark">
                                                <tr>
                                                    <th colspan="2">Dock Door</th>
                                                    <th colspan="7">Trailer</th>
                                                </tr>
                                            </thead>
                                            <tbody></tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!-- Sidebar panel AGV Notification -->
            <div class="leaflet-sidebar-pane overflow-auto" id="agvnotificationinfo">
                <h4 class="ml-p5rem">AGV Notification</h4>
                <div class="btn-toolbar" role="toolbar" id="agvnotificationinfo_div">
                    <div id="div_agvnotification" class="container-fluid">
                        <div class="card w-100">
                            <div class="card-header pl-1">
                                <h6 class="sectionHeader ml-1 mb-1">AGV Details and Status</h6>
                            </div>
                            <div class="card-body">
                                <div class="table-responsive fixedHeader variableHeight">
                                    <table class="table table-sm table-hover table-condensed mb-1 border-bottom" id="agvnotificationtable">
                                        <thead class="thead-dark">
                                            <tr>
                                                <th><span class="ml-p25rem">Condition Name</span></th>
                                                <th>Name</th>
                                                <th>Duration</th>
                                            </tr>
                                        </thead>
                                        <tbody></tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!-- Sidebar panel Notification Setup -->
            <div class="leaflet-sidebar-pane overflow-auto" id="notificationsetup">
                <h4 class="ml-p5rem">Notification Setup</h4>
                <div class="btn-toolbar" role="toolbar" id="notificationsetup_div">
                    <div id="div_notificationsetup" class="container-fluid">
                        <button type="button" class="btn btn-primary mb-sm-2 ml-p5rem" name="addnotificationsetup">Add</button><div class="card w-100">
                            <div class="card-body overflow-auto">
                                <div class="table-responsive">
                                    <table class="table table-sm table-hover mb-1 border-bottom" id="notificationsetuptable">
                                        <thead class="thead-dark">
                                            <tr>
                                                <th class="row-n-name"><span class="ml-p5rem">Name</span></th>
                                                <th class="row-n-wmin">Warning</th>
                                                <th class="row-n-cmin">Critical</th>
                                                <th class="row-n-status">Status</th>
                                                <th class="row-n-action">Action</th>
                                            </tr>
                                        </thead>
                                        <tbody></tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!-- Sidebar panel User Profile -->
            <div class="leaflet-sidebar-pane overflow-auto" id="userprofile">
                <h4 class="ml-p5rem">User Profile</h4>
                <div class="btn-toolbar ml-p5rem" role="toolbar" id="userprofile_div">
                    <div class="w-100 mb-3">
                        <div class="user-card-full bg-white mr-p65rem">
                            <div class="row m-l-0 m-r-0">
                                <div class="col-sm-3 user-profile bg-USPSBlue">
                                    <div class="text-center text-white largeIcon">
                                        <i class="pi-iconUserProfile"></i>
                                        <h6 class="pt-2" id="userfullname"></h6>
                                        <div id="usertitel" class="pb-2"></div>
                                    </div>
                                    <div class="col" style="display: none; ">
                                        <button class="btn btn-sm btn-outline-light btn-block" type="button" id="export_useage_report">Report</button>
                                    </div>
                                </div>
                                <div class="col-sm-9 user-info bg-white pb-0">
                                    <div class="px-0 py-1">
                                        <h5 class="m-b-10 p-b-5 b-b-default sectionHeader">Information</h5>
                                        <div class="row">
                                            <div class="col-sm-2">
                                                <p class="mb-2 f-w-600">Email</p>
                                                <p class="mb-2 f-w-600">Phone</p>
                                            </div>
                                            <div class="col-10">
                                                <p class="f-w-400 mb-2" id="useremail"></p>
                                                <p class="f-w-400 mb-0" id="userphone"></p>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="card w-100 mb-3">
                        <h6 class="card-header sectionHeader ml-2 mb-1 px-0">
                            User Survey
                        </h6>
                        <div class="card-body ml-np5rem">
                            <div class="input-group mb-2 mx-3">
                                <!-- FOTF Survey Buttons -->
                                <div role="group" class="d-flex btn-block" aria-label="Factory of the Future Surveys">
                                    <a type="button" class="btn btn-light flex-fill mr-3" href="https://forms.office.com/g/X1RWUniffh" target="_blank">Feedback</a>
                                    <a type="button" class="btn btn-light flex-fill mr-np5rem" href="https://forms.office.com/g/8e3MgW7yt5" target="_blank">Suggestions</a>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="card w-100 mb-3">
                        <h6 class="card-header sectionHeader ml-2 mb-1 px-0">
                            User Guide
                        </h6>
                        <div class="card-body mr-np5rem">
                            <div class="input-group mb-2 mx-3">
                                <!-- User Guide Buttons -->
                                <div role="group" class="d-flex btn-block mr-np5rem" aria-label="User Guide">
                                    <a type="button" class="btn btn-light flex-fill" href="https://usps365.sharepoint.com/:b:/s/ConnectedFacility/EVHZopcQdWxJruSDxjIqnkcBA1Gt4i8VyZv2ia1yYmK2KQ?e=KEfx3W&xsdata=MDV8MDF8fGY2YTMwOWNlZTljMjQwZGJlZGFkMDhkYTJlYmM3MGY5fGY5YWE1Nzg4ZWIzMzRhNDk4YWQwNzYxMDE5MTBjYWMzfDB8MHw2Mzc4NzM2ODQxMTA1MDMzNDF8R29vZHxWR1ZoYlhOVFpXTjFjbWwwZVZObGNuWnBZMlY4ZXlKV0lqb2lNQzR3TGpBd01EQWlMQ0pRSWpvaVYybHVNeklpTENKQlRpSTZJazkwYUdWeUlpd2lWMVFpT2pFeGZRPT18MXxNVGs2TnpnNFlURTVZV1V0TnpNMVpDMDBNakV4TFdGbU1HWXRNbVJtWTJNeFkyVm1OVEV4WDJGbU4yTXpPV1JrTFRCaU4yVXRORGN4TXkxaU9UWmtMV016WXpoa09EVTNaREptT0VCMWJuRXVaMkpzTG5Od1lXTmxjdz09fHw%3D&sdata=VW82Z1dXdXpNMkVHdVMyUmp0L1hOYXg1bU9BVVZ1bzA5SVJzZmxLaWhJcz0%3D&ovuser=f9aa5788-eb33-4a49-8ad0-76101910cac3%2CLori.J.Gavin%40usps.gov" target="_blank">User Guide</a>

                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="card w-100 mb-3">
                        <div class="row mx-0 mb-2">
                            <h6 class="card-header sectionHeader ml-2 px-0 py-0 mt-3">Badge / Equipment Search</h6>
                            <div class="input-group flex-fill px-0 mt-2 mx-3 mr-p5rem">
                                <input type="text" id="inputsearchbtn" class="form-control input-border-light" placeholder="Enter badge or equipment ID" aria-label="Tag / Equipment Name" aria-describedby="searchbtn" style="font-size: 14px; ">
                                <!--<div class="input-group-append">
                                    <button class="btn btn-outline-secondary" type="button" id="searchbtn">Search</button>
                                </div>-->
                            </div>
                            <p class="smaller text-secondary mb-0 mt-1 ml-2">Start search by entering any letter or number of a badge or vehicle ID</p>
                        </div>
                        <div class="card-body">
                            <div class="table-responsive fixedHeader variableHeight">
                                <table class="table table-sm table-hover mb-1 border-bottom" id="tagresulttable">
                                    <thead class="thead-dark">
                                        <tr><th><span class="ml-p25rem">Badge / Equipment Name</span></th><th>Action</th></tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!--Sidebar panel MPE Notification-->
            <div class="leaflet-sidebar-pane overflow-auto" id="mpenotificationinfo">
                <h4 class="ml-p5rem">MPE Notification</h4>
                <div class="btn-toolbar" role="toolbar" id="mpenotificationinfo_div">
                    <div id="div_mpenotification" class="container-fluid">
                        <div class="card w-100">
                            <div class="card-header pl-1">
                                <h6 class="sectionHeader ml-1 mb-1">MPE Details and Status</h6>
                            </div>
                            <div class="card-body">
                                <div class="table-responsive fixedHeader">
                                    <table class="table table-sm table-hover table-condensed mb-1 border-bottom" id="mpenotificationtable">
                                        <thead class="thead-dark">
                                            <tr>
                                                <th><span class="ml-p25rem">Condition Name</span></th>
                                                <th>Name</th>
                                                <th>Duration</th>
                                            </tr>
                                        </thead>
                                        <tbody></tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Sidebar panel Vertical Navigation Tab -->
        <div class="leaflet-sidebar-tabs" role="tablist">
            <!-- Top tab group -->
            <ul class="">
                <li class="" title="USPS"><a class="iconCenter" href="#home" role="tab"><i class="pi-logoUspsShort"></i></a></li>
                <li class="" title="Notification Setup"><a class="iconCenter" href="#notificationsetup" role="tab"><i class="pi-iconNotification"></i></a></li>
            </ul>
            <!-- Bottom tab group -->
            <ul class="mb-3">
                <li class="" title="User Profile"><a class="iconCenter" href="#userprofile" role="tab"><i class="pi-iconUserProfile"></i></a></li>
                <li class="" title="AGV Notification"><a class="iconCenter" href="#agvnotificationinfo" role="tab"><i id="agvnotificaion" class="pi-iconVehicle" title="AGV Notification"><span class="badge badge-notify bell-notification" id="agvnotificaion_number"></span></i></a></li>
                <li class="" title="Trips Notification"><a class="iconCenter" href="#tripsnotificationinfo" role="tab"><i id="tripsnotificaion" class="pi-iconDockTruck"><span class="badge badge-notify bell-notification" id="tripsnotificaion_number"></span></i></a></li>
                <li class="" title="MPE Notification"><a class="iconCenter" href="#mpenotificationinfo" role="tab"><i id="mpenotification" class="pi-iconMachine"><span class="badge badge-notify bell-notification" id="mpenotification_number"></span></i></a></li>
                <li class="" title="Report Information"><a class="iconCenter" href="#reports" role="tab"><i class="pi-iconStaffing"></i></a></li>
                <li class="" title=""><a class="iconCenter" href="#home" role="tab"><i class="pi-iconMenuOpen"></i></a></li>
                <!--<li class="" title="View Ports"><a class="" href="#viewports" role="tab"><i class="pi-iconViewport"></i></a></li>-->
            </ul>
        </div>
    </div>

    <!--Hidden content for js-->
    <div class="popover noCorners" role="complementary" style="display: none;">
        <!--Legend Popover-->
        <div id="legendContent" class="px-2 pt-0">
            <div class="row">
                <div class="col-5 pr-0">
                    <h4 class="popover-subheader mb-2 ml-p25rem">Vehicles</h4>
                    <ul class="noBullets">
                        <!-- PIV icons -->
                        <li class="mb-3"><i class="pi-iconLoader_forklift iconMedium pl-2 mr-4 pr-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span><span class="path6"></span></i> PIV Forklift</li>
                        <li class="mb-3"><i class="pi-iconLoader_tugger iconMedium ml-3 mr-4 pr-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span></i>PIV Tugger</li>
                        <li class="mb-3"><i class="pi-iconLoader_wr iconMedium mr-4 pr-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span></i>PIV Walking Rider</li>
                        <li class="mb-3"><i class="pi-iconSurfboard iconSmall mr-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span><span class="path6"></span><span class="path7"></span><span class="path8"></span></i>PIV Surfboard</li>
                        <li class="mb-3"><i class="pi-iconVh_bss iconMedium mr-4 pr-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span></i>PIV Cart</li>
                        <li class="mb-3"><i class="pi-iconMule iconMedium mr-4 pr-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span><span class="path6"></span><span class="path7"></span></i>PIV Mule</li>
                        <li class="mb-3"><i class="pi-iconTricycle iconMedium mr-4 pr-2"> <span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span></i>PIV Tricycle</li>
                        <!-- AGV icons -->
                        <li class="mb-3"><i class="pi-iconLoader_avg_pj iconMedium ml-2 mr-4 pr-2"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span><span class="path5"></span><span class="path6"></span><span class="path7"></span><span class="path8"></span><span class="path9"></span><span class="path10"></span></i>AGV Pallet Jack</li>
                        <li class="mb-3"><i class="pi-iconLoader_avg_t iconMedium ml-3 mr-4 pr-3"><span class="path1"></span><span class="path2"></span><span class="path3"></span><span class="path4"></span></i>AGV Tugger</li>
                    </ul>
                </div>
                <div class="col-7">
                    <h4 class="popover-subheader">Dock Status</h4>
                    <ul class="noBullets mb-2">
                        <li class="alert alert-info px-1 py-3 mb-sm-2 noCorners">Ready for departure - Load is full</li>
                        <li class="alert alert-warning p-sm-1 mb-sm-2 noCorners">15-30 minutes before departure - Load NOT full</li>
                        <li class="alert alert-danger p-sm-1 mb-sm-2 noCorners">&lt; 15 minutes before departure - Load NOT full</li>
                        <li class="alert alert-dangerX p-sm-1 mb-sm-3 noCorners">Trip Departed - Containers in Close, Stage or XDock</li>
                    </ul>
                    <h4 class="popover-subheader mb-2">MPE Performance</h4>
                    <ul class="noBullets">
                        <li class="alert alert-info p-sm-1 mb-2 noCorners">At or above expected throughput</li>
                        <li class="alert alert-warning p-sm-1 mb-2 noCorners">1-10% below expected throughput</li>
                        <li class="alert alert-danger p-sm-1 mb-2 noCorners">>10% below expected throughput</li>
                        <li class="alert alert-dark py-2 px-2 mb-2 noCorners">Idle/No Data</li>
                    </ul>
                </div>
            </div>
        </div>

        <!-- FAQ PopOver -->
        <div id="faqContent" class="px-3">
            <a type="button" class="btn btn-light flex-fill mr-3" href="https://usps365.sharepoint.com/:b:/s/ConnectedFacility/EVHZopcQdWxJruSDxjIqnkcBA1Gt4i8VyZv2ia1yYmK2KQ?e=KEfx3W&xsdata=MDV8MDF8fGY2YTMwOWNlZTljMjQwZGJlZGFkMDhkYTJlYmM3MGY5fGY5YWE1Nzg4ZWIzMzRhNDk4YWQwNzYxMDE5MTBjYWMzfDB8MHw2Mzc4NzM2ODQxMTA1MDMzNDF8R29vZHxWR1ZoYlhOVFpXTjFjbWwwZVZObGNuWnBZMlY4ZXlKV0lqb2lNQzR3TGpBd01EQWlMQ0pRSWpvaVYybHVNeklpTENKQlRpSTZJazkwYUdWeUlpd2lWMVFpT2pFeGZRPT18MXxNVGs2TnpnNFlURTVZV1V0TnpNMVpDMDBNakV4TFdGbU1HWXRNbVJtWTJNeFkyVm1OVEV4WDJGbU4yTXpPV1JrTFRCaU4yVXRORGN4TXkxaU9UWmtMV016WXpoa09EVTNaREptT0VCMWJuRXVaMkpzTG5Od1lXTmxjdz09fHw%3D&sdata=VW82Z1dXdXpNMkVHdVMyUmp0L1hOYXg1bU9BVVZ1bzA5SVJzZmxLaWhJcz0%3D&ovuser=f9aa5788-eb33-4a49-8ad0-76101910cac3%2CLori.J.Gavin%40usps.gov" target="_blank">User Guide</a>
            <div class="">
                <table class="faqTable faqHeader-dark table-striped bg-white">
                    <thead class="">
                        <tr>
                            <th class="">Questions</th>
                            <th class="">Answers</th>
                            <th class="pr-2">Details</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td class="">How do I get the new FOTF updates on my iPad/Tablet?</td>
                            <td class="">On the tablet, press CTRL + F5 to push the updates .  For iPads, relaunch the browser to refresh the view.</td>
                            <td class=""></td>
                        </tr>
                        <tr>
                            <td class="">Why won't Mobil Iron download to my device? </td>
                            <td class="">This should be done during the initial tablet/iPad setup.  Please contact the FOTF team if you have questions.</td>
                            <td class=""></td>
                        </tr>
                        <tr>
                            <td class="">Why won't my emails download to my device?</td>
                            <td class="">Access to email is not a primary function of FOTF.  It can take 2-3 days for the emails to appear on your device.  </td>
                            <td class=""></td>
                        </tr>
                        <tr>
                            <td class="">Do I have to login every time I use the device?</td>
                            <td class="">You will need to login after your session expires, which is set for daily time-out.</td>
                            <td class=""></td>
                        </tr>
                        <tr>
                            <td class="">Why does the "plug" icon change color?</td>
                            <td class="">This plug icon displays the level of Wi-Fi, for that specific tablet/iPad, at that location and time.</td>
                            <td class="text-center bg-USPSPurple text-white border-bottom"><i class="pi-iconPlugOutline"></i></td>
                        </tr>
                        <tr>
                            <td class="">What do the icons and colors on the map represent?</td>
                            <td class="">Please refer to the legend, located on the lower right of the screen for info about icons and color indicators.</td>
                            <td class="text-center bg-USPSPurple text-white border-bottom"><i class="pi-iconLegend"></i></td>
                        </tr>
                        <tr>
                            <td class="">Where can I view my user profile?</td>
                            <td class="">The Person icon on the left navigation is where you can view your user profile, that displays the account info of the current logged-in user.</td>
                            <td class="text-center bg-USPSPurple text-white border-bottom"><i class="pi-iconUserProfile"></i></td>
                        </tr>
                        <tr>
                            <td class="">Where can I see specific information about the employee type and number of personnel in my work zone?</td>
                            <td class="">The Report icon displays info related to craft type and number of badges in a work zone.</td>
                            <td class="text-center bg-USPSPurple text-white border-bottom"><i class="pi-iconStaffing"></i></td>
                        </tr>
                        <tr>
                            <td class="">Where can I view floor status by zone?  What about staff info and machine info?</td>
                            <td class="">The Floor Status icon allows you to check status by zone, as well as displays information about current machine and staffing.</td>
                            <td class="text-center bg-USPSPurple text-white border-bottom"><i class="pi-iconMenuOpen"></i></td>
                        </tr>
                        <tr>
                            <td class="">How do I expand my view to Full Screen?</td>
                            <td class="">Click the Expand Screen icon to view the screen in full screen mode.  Clicking the icon again will return to original view.</td>
                            <td class="text-center bg-USPSPurple text-white border-bottom px-0"><div class="border-bottom pb-2 mb-2"><i class="pi-iconFullScreen"></i></div><div><i class="pi-iconFullScreenExit"></i></div></td>
                        </tr>
                        <tr>
                            <td class="">What is the difference between the Feedback and Suggestions options on the right navigation?  </td>
                            <td class="">The Feedback option is intended as a forum for users to express feedback about current functionality and the user experience. By contrast, the Suggestions option is where users can submit recommendations or requests for future features and enhancements.</td>
                            <td class=""></td>
                        </tr>
                        <tr>
                            <td class="">Can I search for equipment location?</td>
                            <td class="">Yes.  From the Profile button, click in the search bar and start typing the name of the equipment (i.e. Mule). Items will change color and halo will appear around them.</td>
                            <td class=""></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

        <!--view ports Popover-->
        <div id="viewportsContent" class="w-100 d-flex justify-content-between">
        </div>

        <!--24 Hour Clock Popover-->
        <div id="tfhcContent" class="px-3">
            <div class="">
                <div id="clockContainer">
                    <div id="hour"></div>
                    <div id="minute"></div>
                    <div id="second"></div>
                </div>
            </div>
        </div>
    </div>

    <script src="Scripts/jquery-3.6.0.min.js"></script>
    <script>

        $("#loadWrapper").removeClass("loadingFOTF");
        $("body").removeClass("loadingFOTFBody");
    </script>
    <script src="Scripts/bootstrap.bundle.min.js"></script>
    <script src="Scripts/moment.min.js"></script>
    <script src="Scripts/moment-timezone.min.js"></script>
    <script src="Scripts/moment-duration-format.js"></script>
    <script src="Scripts/moment-timezone-with-data-2012-2022.min.js"></script>
    <%--<script src="Scripts/selectize.min.js"></script>--%>
    <script src="Scripts/selectize.js"></script>
    <script src="Scripts/jquery.signalR-2.4.2.min.js"></script>
    <!--<script src="Scripts/jquery.signalR-2.4.2.js"></script>-->
    <script src="signalr/hubs/"></script>
    <script src="Scripts/leaflet/leaflet.js"></script>
    <script src="Scripts/leaflet/Plugins/leaflet-sidebar.min.js"></script>
    <script src="Scripts/leaflet/Plugins/easy-button.js"></script>
    <script src="Scripts/leaflet/Plugins/leaflet-marker-slideto.js"></script>
    <script src="Scripts/leaflet/Plugins/leaflet-indoor.js"></script>
    <script src="Scripts/Filter.min.js"></script>
    <!--custom coded event listeners -->
    <script src="Scripts/View/eventListener.js"></script>
    <!--Load default settings for the site-->
    <script src="Scripts/Default.js"></script>
    <!--Load application settings for the site-->
    <script src="Scripts/View/application_setting.js"></script>
    <!--Notification-->
    <script src="Scripts/View/notification.js"></script>
    <!--Views for API Connection-->
    <script src="Scripts/View/connection.js"></script>
    <!--Extra utility functions-->
    <script src="Scripts/View/utils.js"></script>
    <!--Views for bin zone details-->
    <script src="Scripts/View/bin_zone.js"></script>
    <!--Views for sv zone details-->
    <script src="Scripts/View/staging_bullpen.js"></script>
    <!--Views for view ports zone details-->
    <script src="Scripts/View/viewports_zone.js"></script>
    <!--Views for machine processing equipment details-->
    <script src="Scripts/View/machine_zone.js"></script>
    <!--Views for sparklines/graphs for machine processing equipment details-->
    <!--<script src="Scripts/View/machine_sparkline.js"></script>-->
    <!--Views for vehicle details-->
    <script src="Scripts/View/vehicle.js"></script>
    <!--Views for tags details-->
    <script src="Scripts/View/person_tag.js"></script>
    <!--Views for agv location zone details-->
    <script src="Scripts/View/agvlocation_zone.js"></script>
    <!--Views for dock door zone details-->
    <script src="Scripts/View/dockdoor_zone.js"></script>
    <!--Views for stage location zone details-->
    <script src="Scripts/View/stange_zone.js"></script>
    <!--Views for locator's-->
    <script src="Scripts/View/locators.js"></script>
    <!--Views for trips-->
    <script src="Scripts/View/trips.js"></script>
    <!--QR Code js-->
    <script src="Scripts/QRCode/qrcode.min.js"></script>
    <!--tag search js-->
    <script src="Scripts/View/tag_search.js"></script>
    <!--camera js-->
    <script src="Scripts/View/camera_video.js"></script>
    <!--geometry editing -->
    <script src="Scripts/leaflet/Plugins/leaflet-geoman.min.js"></script>
    <script src="Scripts/View/geometry_editing.js"></script>
    <!--Views for sparklines/graphs for machine processing equipment details-->
    <script src="Scripts/View/machine_sparkline.js"></script>
    <!--Load Indoor Map settings for the site-->
    <script src="Scripts/View/indoorMap.js"></script>
    <!--24 Hour Clock js-->
    <script defer src="Scripts/View/twenty_four_hour_clock.js"></script>
    <script defer src="Scripts/Chart.min.js"></script>

    <script>
        $(document).ready(function () {
            $(".bi").on("click", function () {
                $(this).toggleClass("bi-arrows-expand");
                $(this).toggleClass("bi-arrows-collapse");
            });
        });
    </script>
</body>
</html>
