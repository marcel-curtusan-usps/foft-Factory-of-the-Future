/*start SV Var*/
var SV_ScannerSN = "";
var SV_User = "";
var SV_IP = "";
var SV_SiteId = "";
/*end SV Var*/
var UserName = "Mobile_Scan_User";
var CLIENT_IP = "";
var bodytype = "";
var uri = URLconstructor(window.location);
var CONTINER = {};
var Location = {};
var timer_interval = 15000;
var timer = 0;
var statusTimer = 0;
var width = window.innerWidth;
var height = window.innerHeight;
var agvmVersion= "1.0.0.0";
var numOfConnection= 0;
var numOfEnabledConnection = 0;
var TESTMODE = false;
var APIAuthorization = "YmFzaWMgUVVkV1VFOVNWRUZNVlhObGNqcEJaM1pRYjNKMFlXeDFKR1Z5TURFPQ==";
let materialMoveRequest = {
    OBJECT_TYPE: "COMMAND_REQUEST",
    ACTION: "MOVEREQUEST",
    VENDOR_NAME: SV_ScannerSN === "" ? "CN51" : SV_ScannerSN,
    CONNECTION_ID: "",
    TYPE: "",
    PICKBCR: "",
    BARCODE_TYPE: "",
    DROPBCR: "",
    BARCODE: "",
    BODYTYPE: "",
    NASS_CODE: "",
    DOCKBAYNUMBER: "",
    PICKNAME: "",
    DROPNAME: "",
    ENDNAME: "",
    USERNAME: SV_User === "" ? UserName : SV_User,
    CLIENT_IP: SV_IP === "" ? CLIENT_IP : SV_IP,
    SERVER_IP: "",
    TESTMODE: false,
}
let ConnectionMessageObject = {
    OBJECT_TYPE: "STATUS_UPDATE",
    ACTION: "HEARTBEATSTATUS",
    CLIENT_IP: localStorage['ClientIP'],
    VENDOR_NAME: "Scanner",
    USERNAME: localStorage['UserEIN']
}
//setup before functions
let conncetion_refresh = {
    interval: 15000,
    interval_id: 0,
    refresh: function () {
        $.ajax({
            url: uri + "DeviceHeartbeat",
            headers:
            {
                'APIAuthorization': APIAuthorization
            },
            dataType: "json",
            data: JSON.stringify(ConnectionMessageObject),
            type: "Post",
            contentType: "application/json",
            success: function (connectionData) {
                try {
                    if (connectionData.length) {
                        numOfConnection = connectionData.length;
                        numOfEnabledConnection = 0;
                        for (const element of connectionData) {
                           agvmVersion = element.AGVM_VERSION;
                            if (element.ACTIVE_CONNECTION) {
                                if (element.VENDOR_CONNECTED) {
                                    numOfEnabledConnection = (parseInt(numOfEnabledConnection) + 1)
                                }
                            }
                        }
                        if (numOfConnection === 0) {
                            $('.agvactive').text('AGV Off-Line').removeClass("btn-success").addClass('btn-danger')
                        }
                        if (numOfConnection === numOfEnabledConnection) {
                            $('.agvactive').text('AGV On-Line').removeClass("btn-danger").addClass('btn-success')
                            
                        }
                        if (numOfConnection > 0) {
                            if (numOfEnabledConnection === numOfConnection) {
                                $("#connection_button").text("AGV On-line");
                                $("#connection_button").removeClass("btn-danger").removeClass("btn-warning").addClass("btn-success");
                            }
                            else if (numOfEnabledConnection < numOfConnection) {
                                $("#connection_button").text("AGV Off-line");
                                $("#connection_button").removeClass("btn-success").removeClass("btn-warning").addClass("btn-danger");
                            }
                        }
                        if (numOfEnabledConnection > 0) {
                            if (numOfEnabledConnection < numOfConnection) {
                                $("#connection_button").text("AGV " + numOfEnabledConnection + " of " + numOfConnection + " Servers On-line");
                                $("#connection_button").removeClass("btn-success").removeClass("btn-danger").addClass("btn-warning");
                            }

                        }
                    }
                    else {
                        $("#connection_button").text("AGV Off-line");
                        $("#connection_button").removeClass("btn-success").removeClass("btn-warning").addClass("btn-danger");
                    }

                } catch (e) {
                    $("#error_vehicle_queue").text("Error Unable to Parse Data");
                }
            },
            error: function (response) {
                $("#connection_button").text("AGV OFF-line");
                $("#connection_button").removeClass("btn-success").removeClass("btn-warning").addClass("btn-danger");
                $("#MainContent_text_barcode").val("");
                $("#MainContent_text_barcode").css({ "border-color": "#D3D3D3" });
                $('#MainContent_lblError').css("display", "block");
                $('#MainContent_lblError').css('color', 'red');
                $('#MainContent_lblError').text("" + window.location.origin + "/" + window.location.pathname + "/" + "RequestHandler/Scanner_RequestHandler.ashx" + " : " + "");
                $("#MainContent_text_barcode").focus();
                $('label[id=status]').text(response);
            },
            failure: function (response) {
                $("#connection_button").text("AGV OFF-line");
                $("#connection_button").removeClass("btn-success").removeClass("btn-warning").addClass("btn-danger");
                $("#MainContent_text_barcode").val("");
                $("#MainContent_text_barcode").css({ "border-color": "#D3D3D3" });
                $('#MainContent_lblError').css("display", "block");
                $('#MainContent_lblError').css('color', 'red');
                $('#MainContent_lblError').text("" + response.responseText + " : " + "");
                $("#MainContent_text_barcode").focus();
                $('label[id=status]').text(response);
            },
            // Focus
            complete: function () {
                $('#scannerVersion').text(agvmVersion);
                $('#MainContent_text_barcode').val('').focus();
               
            }

        });
    }
};
var scanRequestObject = {
    BARCODE: "",
    BARCODE_TYPE: "",
    CLIENT_IP: SV_IP === "" ? localStorage['ClientIP'] : SV_IP,
    USER_ID: SV_User === "" ? localStorage['UserEIN'] : SV_User,
    SCANNER_SN: SV_ScannerSN,
    SITE_ID: SV_SiteId
};
$(function () {
    $("form").submit(function () { return false; });
    localStorage['UserEIN'] = UserName;
    localStorage['ClientIP'] = CLIENT_IP;
    //this sets the focus on the input box if user click any where on screen 
    $("body").on("click", function () {
        if (!$('input[name=barcode_input]').is(":focus")) {
            $('input[name=barcode_input]').focus().val("");
        }
    });

    $('input[type=text][name=barcode_input]').scannerDetection({
        
        timeBeforeScanTest: 200, // wait for the next character for upto 200ms
        avgTimeByChar: 40, // it's not a barcode if a character takes longer than 100ms
        preventDefault: true,
        endChar: [13],
        onComplete: function (barcode, qty) {
            $('input[type=text][name=barcode_input]').val(barcode);
            //this the function to call after barcode has been scanned.
            Scanned(barcode);
        },
        onError: function (string, qty) {
            $('input[type=text][name=barcode_input]').val($('input[type=text][name=barcode_input]').val() + string);
            if (checkValue(string)) {
                Scanned(string);
            }
        }
    });
    $('input[type=text][name=barcode_input]').on('keypress', function (e) {
        if (e.which == 13) {
            Scanned(e.target.value);
        }
    });
    //check connection state
    conncetion_refresh.refresh();
    conncetion_refresh.interval_id = setInterval(function () { conncetion_refresh.refresh(); }, conncetion_refresh.interval);

});
function Scanned(barcode) {
    if (barcode.length > 5) {
        var tempBarcode = "";
        var barcodetype = "";
        var scanRequest = $.extend(true, {}, scanRequestObject);
        if (/(^(\w{1})(AG\w+))|(^(AG\w+))/ig.test(barcode)) {

            if (/^AGP/ig.test(barcode)) {
                bodytype = "pallet";
            }
            else if (/^AGT/ig.test(barcode)) {
                bodytype = "tugger";
            }
            if (/PIC$/ig.test(barcode)) {
                barcodetype = "Pickup";
            }
            if (/DRO$/ig.test(barcode)) {
                barcodetype = "Drop-Off";
            }
            scanRequest.BARCODE = barcode;
            scanRequest.BARCODE_TYPE = barcodetype;
        }
        else {
            tempBarcode = barcode.replace(/[\u0011]/gi, '').replace(/^\]C1/gi, '');
            if (/(^(\w{1}\d{2}[A-Z]))|(^\d{2}[A-Z])/ig.test(tempBarcode)) {
                barcodetype = "MTEL";
            
            }
            if (/(^(\w{1}\d{24}))|(^(\d{24}))/ig.test(barcode)) {
                barcodetype = "TrayLabel";
             
            }
            scanRequest.BARCODE = tempBarcode;
            scanRequest.BARCODE_TYPE = barcodetype;
        }
        $('input[type=text][name=barcode_input]').val(scanRequest.hasOwnProperty("BARCODE") ? scanRequest.BARCODE : '');
        if (!/Pickup|Drop-Off/i.test(scanRequest.BARCODE_TYPE) && $.isEmptyObject(Location)) {
            $('label[id=status]').css('color', 'Red');
            $('label[id=status]').text("Please Scan Location Barcode First");
            $("#agv_status_button").removeClass("btn-success").removeClass("btn-primary").removeClass("btn-outline-light").addClass("btn-danger");
            $("#agv_status_button").text("Scan Pickup First");

            LogScan(scanRequest);
       
        }
        else {
            if (barcodetype === "MTEL") {
                $('#mtelinfo').css('display', 'flex');
            }
            if (barcodetype === "TrayLabel") {
                $('#traylabelinfo').css('display', 'flex');
            }
            $("#agv_status_button").text("");
            $("#agv_status_button").removeClass("btn-danger").removeClass("btn-success").addClass("btn-outline-light");
            RequestScan(scanRequest, barcodetype);
        }
    }
}
function LogScan(scanRequest) {
    $.ajax({
        url: uri + "DeviceScan",
        headers:
        {
            'APIAuthorization': APIAuthorization
        },
        data: JSON.stringify(scanRequest),
        dataType: "json",
        type: "POST",
        contentType: "application/json",
        success: function (return_barcode) {
        },
        error: function (response) {

        },
        beforeSend: function () {
        },
        failure: function (response) {

        },
        complete: function () {
            $('input[name=barcode_input]').val('').focus();
            Location = {};
            clearTimeout(timer);
            timer = setTimeout(function () { restForm(); }, 2000);
        }
    });
}
function RequestScan(scanRequest, barcodetype) {
   
        $.ajax({
            url: uri + "DeviceScan",
            headers:
            {
                'APIAuthorization': APIAuthorization
            },
            data: JSON.stringify(scanRequest),
            dataType: "json",
            type: "POST",
            contentType: "application/json",
            success: function (return_barcode) {

                if (return_barcode.hasOwnProperty("RESPONSE_CODE") && return_barcode.RESPONSE_CODE === "0") {
                    if (return_barcode.hasOwnProperty("TestMode") && return_barcode.TestMode) {
                        TESTMODE = true;
                    }
                    if (return_barcode.hasOwnProperty("Tray_Label") ) {
                        CONTINER = return_barcode;
                        CONTINER.BARCODE_TYPE = "TrayLabel";
                        $('#traylabelinfo').css('display', 'flex');

                        $('#mtelinfo').css('display', 'none');
                        $('#dropdetails').css('display', 'none');
                        $('label[id=status]').text(barcodetype + " Barcode found: " + return_barcode.Barcode);
                        $('label[id=status]').css('color', 'green');
                        $('input[name=mail_class]').css({ "border-color": "#2eb82e" });
                        $('input[name=mail_class]').val(return_barcode.Mail_Class);
                        $('input[name=priority_level]').css({ "border-color": "#2eb82e" });
                        $('input[name=priority_level]').val(getPriorityText(return_barcode.Priority_Level));
                    }
                    if (!return_barcode.hasOwnProperty("Tray_Label") && return_barcode.BARCODE_TYPE == "MTEL") {
                        CONTINER = return_barcode;
                        var dropoff_location = lacation_formating(return_barcode.hasOwnProperty("palletdropLocation") ? return_barcode.palletdropLocation : "");
                        $('#mtelinfo').css('display', 'flex');
                        $('#traylabelinfo').css('display', 'none');
                        $('label[id=status]').text(barcodetype + " Barcode Scanned: " + scanRequest.BARCODE);

                        //.OriginCode
                        //.doorNumber
                        //.destLegSiteName
                        $('input[name=dest_location]').val(return_barcode.hasOwnProperty("doorNumber") ? return_barcode.doorNumber : "");
                        $('input[name=dest_location]').css({ "border-color": "#2eb82e" });
                        $('input[name=dest_nasscode]').val(return_barcode.hasOwnProperty("OriginCode") ? return_barcode.OriginCode : "");
                        $('input[name=dest_nasscode]').css({ "border-color": "#2eb82e" });
                        $('input[name=destination_location]').val(return_barcode.hasOwnProperty("destLegSiteName") ? return_barcode.destLegSiteName : "");
                        $('input[name=destination_location]').css({ "border-color": "#2eb82e" });
                        //
                        $('input[name=dropoff_column]').val(dropoff_location.hasOwnProperty("column") ? dropoff_location.column : "");
                        $('input[name=dropoff_column]').css({ "border-color": "#2eb82e" });
                        $('input[name=dropoff_location]').val(dropoff_location.hasOwnProperty("location") ? dropoff_location.location : "");
                        $('input[name=dropoff_location]').css({ "border-color": "#2eb82e" });
                        $('input[name=dropoff_dropoffcode]').val(dropoff_location.hasOwnProperty("fulllocation") ? dropoff_location.fulllocation : "");
                        $('input[name=dropoff_dropoffcode]').css({ "border-color": "#2eb82e" });
                    }
                    if (return_barcode.hasOwnProperty("BARCODE_TYPE") && /(Pickup|Drop-Off)/.test(return_barcode.BARCODE_TYPE)) {
                        bodytype = return_barcode.Body_Type;
                        if (return_barcode.Type === "drop") {
                            $('label[id=status]').text(barcodetype + " Barcode Scanned: " + return_barcode.Barcode);
                            $('label[id=status]').css('color', 'green');
                            $('input[name=dropoff_column]').css({ "border-color": "#2eb82e" });
                            $('input[name=dropoff_dropoffcode]').css({ "border-color": "#2eb82e" });
                            $('input[name=dropoff_location]').css({ "border-color": "#2eb82e" });
                            var scan_location = lacation_formating(return_barcode.hasOwnProperty("Name") ? return_barcode.Name : "");
                            $('input[name=dropoff_column]').val(scan_location.hasOwnProperty("column") ? scan_location.column : "");
                            $('input[name=dropoff_location]').val(scan_location.hasOwnProperty("location") ? scan_location.location : "");
                            $('input[name=dropoff_dropoffcode]').val(return_barcode.hasOwnProperty("Name") ? return_barcode.Name : "");
                        }
                        if (return_barcode.Type === "pickup") {
                            $('label[id=status]').text(barcodetype + " Barcode Scanned: " + return_barcode.Barcode);
                            $('label[id=status]').css('color', 'green');
                            $('input[name=pickup_column]').css({ "border-color": "#2eb82e" });
                            $('input[name=pickup_pickupcode]').css({ "border-color": "#2eb82e" });
                            $('input[name=pickup_location]').css({ "border-color": "#2eb82e" });
                            var scan_location = lacation_formating(return_barcode.hasOwnProperty("Name") ? return_barcode.Name : "");
                            $('input[name=pickup_column]').val(scan_location.hasOwnProperty("column") ? scan_location.column : "");
                            $('input[name=pickup_location]').val(scan_location.hasOwnProperty("location") ? scan_location.location : "");
                            $('input[name=pickup_pickupcode]').val(return_barcode.hasOwnProperty("Name") ? return_barcode.Name : "");
                        }

                        Location = return_barcode;
                    }

                }
                else {
                    $('label[id=status]').text(barcodetype + " Barcode NOT found: " + return_barcode.RESPONSE_MSG);
                    $('label[id=status]').css('color', 'red');
                    Location = {};
                    CONTINER = {};
                    // restForm();
                }
                $('input[name=barcode_input]').val("").focus();
            },
            error: function (response) {

            },
            beforeSend: function () {
                clearTimeout(timer);
                if (!$.isEmptyObject(Location) && !$.isEmptyObject(CONTINER)) {
                    restForm();
                }
            },
            failure: function (response) {

            },
            complete: function () {
                $('input[name=barcode_input]').val('').focus();
                if (!$.isEmptyObject(Location) && !$.isEmptyObject(CONTINER)) {
                    CallforAGV();
                }
                timer = setTimeout(function () { restForm(); }, timer_interval);
            }
        });
    
}
function getPriorityText(data) {
    switch (data) {
        case "1":
            return "Low (" + data + ")";
        case "2":
            return "High (" + data + ")";
    default:
           return "No Priority";
    }
}
function LoadPick_loacation(pickdata) {
    vaildorigin = true;
    origin = true;
    zone = pickdata.ZONE
    bodytype = pickdata.BODY_TYPE
    var Post = "";
    var Lane = "";
    if (pickdata.PICKUP_LOCATION !== "") {
        var arr = pickdata.PICKUP_LOCATION.match(/.{1,3}/g);
        if (arr.length === 4) {
            Post = arr[0].replace(/^0+/, '') + arr[1].replace(/^0+/, '');
            Lane = arr[2].replace(/^0+/, '') + " , " + arr[3].replace(/^0+/, '');
        }
    }

    $("#text_origincolumn").val(Post);
    $("#text_originlane").val(Lane);
    $("#text_origin").val(pickdata.PICKUP_LOCATION);
    $('#MainContent_lblpickupstatus').html("Picking-up From Column: " + Post + ", Location: " + Lane + "<br/> Bin #: " + pickdata.BIN_NUMBER);
    $("#text_origin").css({ "border-color": "#2eb82e" });
    $("#text_origincolumn").css({ "border-color": "#2eb82e" });
    $("#text_originlane").css({ "border-color": "#2eb82e" });
    $("#MainContent_text_barcode").val("");
    $("#MainContent_text_barcode").css({ "border-color": "#D3D3D3" });
    $("#MainContent_text_barcode").focus();
    return pickdata;

}
function LoadDrop_loacation(dropdata) {
    try {

        destination = true;

        var dropPost = "";
        var dropLane = "";
        if (dropdata.DROP_LOCATION !== "") {
            var droparr = dropdata.DROP_LOCATION.match(/.{1,3}/g);
            if (droparr.length === 4) {
                dropPost = droparr[0].replace(/^0+/, '') + droparr[1].replace(/^0+/, '');
                dropLane = droparr[2].replace(/^0+/, '') + " , " + droparr[3].replace(/^0+/, '');
            }
        }

        var endPost = "";
        var endLane = "";
        if (dropdata.END_LOCATION !== "") {
            var endarr = dropdata.END_LOCATION.match(/.{1,3}/g);
            if (endarr.length === 4) {
                endPost = endarr[0].replace(/^0+/, '') + endarr[1].replace(/^0+/, '');
                endLane = endarr[2].replace(/^0+/, '') + " , " + endarr[3].replace(/^0+/, '');
            }
        }


        $("#text_destinationnass").val(dropdata.NASS_CODE);
        $("#text_destinationdoor").val(dropdata.DOCKBAYNUMBER);
        $('#MainContent_lbldropstatus').css('color', 'green');
        $("#text_destination").val(dropdata.DROP_LOCATION);
        $("#text_destinationcolumn").val(dropPost);
        $("#text_destinationlane").val(dropLane);
        $("#text_endlocation").val(dropdata.END_LOCATION);
        $("#text_endlocationcolumn").val(endPost);
        $("#MainContent_text_dropofflocation").val(dropdata.END_LOCATION);
        $("#MainContent_lbldropstatus").html("Destination Column: " + dropPost + ", Location: " + dropLane + " <br/> Door #: " + dropdata.DOCKBAYNUMBER);
        $("#MainContent_text_dropofflocation").css({ "border-color": "#2eb82e" });
        $("#text_destination").css({ "border-color": "#2eb82e" });
        $("#text_endlocation").css({ "border-color": "#2eb82e" });
        $("#text_destinationdoor").css({ "border-color": "#2eb82e" });
        $("#text_destinationnass").css({ "border-color": "#2eb82e" });
        $("#text_destinationcolumn").css({ "border-color": "#2eb82e" });
        $("#text_destinationlane").css({ "border-color": "#2eb82e" });
        $("#text_endlocationcolumn").css({ "border-color": "#2eb82e" });
        $("#MainContent_text_barcode").css({ "border-color": "#D3D3D3" });
        $("#MainContent_text_barcode").val("");
        $("#MainContent_text_barcode").focus();
        return dropdata;
    }
    catch (e) {
        $("#MainContent_text_barcode").val("");
        $("#MainContent_text_barcode").css({ "border-color": "#D3D3D3" });
        $('#MainContent_lblError').css('color', 'red');
        $('#MainContent_lblError').text("" + e + "Barcode Not Found : " + parsbcr_value);
        $("#MainContent_text_barcode").focus();
    }
}
function Vaild_barcode(bc) {
    if (/^j|^i/i.test(bc)) {
        bc = bc.substr(1, bc.length);

        if (bc.length === 12) {
            if (bc.substr(0, 3) === "000") {
                localStorage['UserEIN'] = bc.substr(3, bc, length - 1);
                localStorage['Last_Scan_date_time'] = moment().format();
                CheckLastScan();
                return true;
            }
            else {
                return false;
            }
        }
    }
    else {
        if (bc.length === 12) {
            if (bc.substr(0, 3) === "000") {
                localStorage['UserEIN'] = bc.substr(3, bc, length - 1);
                localStorage['Last_Scan_date_time'] = moment().format();
                CheckLastScan();
                return true;
            }
            else {
                return false;
            }
        }
    }
}
function CheckLastScan() {
    try {
        var diff_time = moment(moment().format()).diff(localStorage['Last_Scan_date_time']);
        var duration_time = moment.duration(diff_time);

        if (duration_time._isValid) {

            //log out user after 15 minutes...
            if (duration_time.asSeconds() > 900) {
                $(".usernameloggedin").text("");
                $(".logouttimer").text("");
                localStorage['UserEIN'] = "CN51";
            }
            else {
                //display timer
                $(".logouttimer").text(countdowntimeFormat(duration_time.asSeconds()));
            }
        }
    } catch (e) {

    }
}
function countdowntimeFormat(interval) {
    var duration = moment.duration({
        'minutes': 15,
        'seconds': 00

    });
    duration = moment.duration(duration.asSeconds() - interval, 'seconds');
    var min = duration.minutes();
    var sec = duration.seconds();
    sec -= 1;
    if (min < 0) return clearInterval(timer);
    if (min < 10 && min.length != 2) min = '0' + min;
    if (sec < 0 && min != 0) {
        min -= 1;
        sec = 59;
    }
    else if (sec < 10 && sec.length != 2) sec = '0' + sec;

    return min + ':' + sec;
}
//call AGV --old way to call for vehicle 
function CallforAGV() {

    var material_Move_Request = $.extend(true, {}, materialMoveRequest);
    if (Location.Type === "drop") {
        material_Move_Request.DROPBCR = Location.Barcode;
        material_Move_Request.DROPNAME = Location.Name;
    }
    if (Location.Type === "pickup") {
        material_Move_Request.PICKBCR = Location.Barcode;
        material_Move_Request.PICKNAME = Location.Name;
        if (bodytype === "PALLET") {
            material_Move_Request.DROPBCR = CONTINER.Barcode;
            material_Move_Request.DROPNAME = CONTINER.hasOwnProperty("palletdropLocation") ? CONTINER.palletdropLocation : "";
            material_Move_Request.ENDNAME = CONTINER.hasOwnProperty("palletendLocation") ? CONTINER.palletendLocation : "";
        }
        else {
            material_Move_Request.DROPBCR = CONTINER.Barcode;
            material_Move_Request.DROPNAME = CONTINER.hasOwnProperty("tuggerdropLocation") ? CONTINER.tuggerdropLocation : "";
            material_Move_Request.ENDNAME = CONTINER.hasOwnProperty("tuggerendLocation") ? CONTINER.tuggerendLocation : "";
        }
    }
    material_Move_Request.BARCODE_TYPE = CONTINER.BARCODE_TYPE;
    material_Move_Request.NASS_CODE = CONTINER.DestinationCode;
    material_Move_Request.CONNECTION_ID = Location.Connection_Id;
    material_Move_Request.TYPE = Location.Type;
    material_Move_Request.LABEL_DAY = CONTINER.Label_day;
    material_Move_Request.MAIL_CLASS = CONTINER.Mail_Class;
    material_Move_Request.PRIORITY_LEVEL = CONTINER.Priority_Level;
    material_Move_Request.BARCODE = CONTINER.Barcode;
    material_Move_Request.BODYTYPE = Location.Body_Type;
    material_Move_Request.USERNAME = SV_User === "" ? localStorage['UserEIN'] : SV_User;
    material_Move_Request.CLIENT_IP = SV_IP === "" ? localStorage['ClientIP'] : SV_IP;
    material_Move_Request.SERVER_IP = "";
    if (TESTMODE) {
        material_Move_Request.TESTMODE = true;
    }
    $('label[id=status]').css('color', 'black');
    $('label[id=status]').text("Calling Vehicle Please wait");
    $("#agv_status_button").text("Calling Vehicle");
    $("#agv_status_button").removeClass("btn-danger").removeClass("btn-outline-light").removeClass("btn-success").addClass("btn-primary");
    $.ajax({
        url: uri + "DeviceMissionRequest",
        headers:
        {
            'APIAuthorization': APIAuthorization
        },
        data: JSON.stringify(material_Move_Request),
        dataType: "json",
        type: "POST",
        contentType: "application/json",
        success: function (return_info) {
            timer = 0;
            $('label[id=status]').text("Calling AGV Please");
            if (return_info.hasOwnProperty("RESPONSE_CODE") && return_info.RESPONSE_CODE === "0") {
                $('label[id=status]').text(return_info.hasOwnProperty("RESPONSE_MSG") ? return_info.RESPONSE_MSG : "");
                $('label[id=status]').text("Vehicle has been Called");
                $('label[id=status]').css('color', 'green');
                $("#agv_status_button").text("Vehicle Called");
                $("#agv_status_button").removeClass("btn-danger").removeClass("btn-primary").removeClass("btn-outline-light").addClass("btn-success");

                if (Location.Type === "drop") {
                    $('input[name=dropoff_column]').css({ "border-color": "#2eb82e" });
                    $('input[name=dropoff_dropoffcode]').css({ "border-color": "#2eb82e" });
                    $('input[name=dropoff_location]').css({ "border-color": "#2eb82e" });
                    var scan_location = lacation_formating(return_info.hasOwnProperty("Name") ? return_info.Name : "");
                    $('input[name=dropoff_column]').val(scan_location.hasOwnProperty("column") ? scan_location.column : "");
                    $('input[name=dropoff_location]').val(scan_location.hasOwnProperty("location") ? scan_location.location : "");
                    $('input[name=dropoff_dropoffcode]').val(return_info.hasOwnProperty("Name") ? return_info.NAME : "");
                }
                if (Location.Type === "pickup") {
                    $('input[name=pickup_column]').css({ "border-color": "#2eb82e" });
                    $('input[name=dropoff_dropoffcode]').css({ "border-color": "#2eb82e" });
                    $('input[name=pickup_pickupcode]').css({ "border-color": "#2eb82e" });
                    var scan_location = lacation_formating(return_info.hasOwnProperty("Name") ? return_info.Name : "");
                    $('input[name=pickup_column]').val(scan_location.hasOwnProperty("column") ? scan_location.column : "");
                    $('input[name=pickup_location]').val(scan_location.hasOwnProperty("location") ? scan_location.location : "");
                    $('input[name=pickup_pickupcode]').val(return_info.hasOwnProperty("Name") ? return_info.Name : "");
                }
            }
            else {
                $('label[id=status]').css('color', 'red');
                $('label[id=status]').text(return_info.hasOwnProperty("RESPONSE_MSG") ? return_info.RESPONSE_MSG : "");
                $("#agv_status_button").removeClass("btn-success").removeClass("btn-primary").removeClass("btn-outline-light").addClass("btn-danger");
                $("#agv_status_button").text("No Vehicle Called");

            }
        },
        error: function (response) {

        },
        beforeSend: function () {
            clearTimeout(timer);
        },
        failure: function (response) {

        },
        // Focus
        complete: function () {
            CONTINER = {};
            Location = {};
            $('#MainContent_text_barcode').val('').focus();
            timer = setTimeout(function () { restForm(); }, timer_interval);
        }
    });
}
//rest the screen
function restForm() {
    //tray Label
    $('input[name=mail_class]').val("");
    $('input[name=mail_class]').css({ "border-color": "#D3D3D3" });
    $('input[name=priority_level]').val("");
    $('input[name=priority_level]').css({ "border-color": "#D3D3D3" });
    //pickup
    $('input[name=pickup_column]').val("");
    $('input[name=pickup_location]').val("");
    $('input[name=pickup_pickupcode]').val("");
    $('input[name=pickup_column]').css({ "border-color": "#D3D3D3" });
    $('input[name=pickup_pickupcode]').css({ "border-color": "#D3D3D3" });
    $('input[name=pickup_location]').css({ "border-color": "#D3D3D3" });
    //dropoff
    $('input[name=dropoff_dropoffcode]').val("");
    $('input[name=destination_location]').val("");
    $('input[name=dropoff_location]').val("");
    $('input[name=dropoff_column]').val("");
    $('input[name=dest_nasscode]').val("");
    $('input[name=dest_location]').val("");
    $('input[name=destination_location]').css({ "border-color": "#D3D3D3" });
    $('input[name=dropoff_dropoffcode]').css({ "border-color": "#D3D3D3" });
    $('input[name=dropoff_location]').css({ "border-color": "#D3D3D3" });
    $('input[name=dropoff_column]').css({ "border-color": "#D3D3D3" });
    $('input[name=dest_nasscode]').css({ "border-color": "#D3D3D3" });
    $('input[name=dest_location]').css({ "border-color": "#D3D3D3" });
    //status
    $('label[id=status]').text("");
    $('input[name=barcode_input]').val("");
    $('#mtelinfo').css('display', 'none');
    $('#traylabelinfo').css('display', 'none');
    $('#dropdetails').css('display', 'flex');
    $("#agv_status_button").text("");
    $("#agv_status_button").removeClass("btn-danger").removeClass("btn-success").addClass("btn-outline-light");
    CONTINER = {};
    Location = {};
}
function lacation_formating(location) {
    var Post = {
        column: "",
        location: "",
        fulllocation: ""
    };
    if (checkValue(location)) {
        var arr = location.match(/.{1,3}/g);
        if (arr.length === 4) {
            Post.column = (checkValue(arr[0].replace(/^0+/, '')) ? arr[0].replace(/^0+/, '') : "0") + (checkValue(arr[1].replace(/^0+/, '')) ? arr[1].replace(/^0+/, '') : "0");
            Post.location = (checkValue(arr[2].replace(/^0+/, '')) ? arr[2].replace(/^0+/, '') : "0") + "," + (checkValue(arr[3].replace(/^0+/, '')) ? arr[3].replace(/^0+/, '') : "0");
            Post.fulllocation = location;
            return Post;
        }
    }
    return Post;
}
function checkValue(value) {
    switch (value) {
        case "": return false;
        case null: return false;
        case "undefined": return false;
        case undefined: return false;
        default: return true;
    }
}

function URLconstructor(winLoc)
{
    if (/^(.AGVM)/i.test(winLoc.pathname)) {
        return winLoc.origin + "/AGVM/api/";
    }
    else {
        return winLoc.origin + "/api/";
    }
}
