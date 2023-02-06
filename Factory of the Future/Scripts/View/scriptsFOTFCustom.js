/// A simple template method for replacing placeholders enclosed in curly braces.
if (!String.prototype.supplant) {
    String.prototype.supplant = function (o) {
        return this.replace(/{([^{}]*)}/g,
            function (a, b) {
                var r = o[b];
                return typeof r === 'string' || typeof r === 'number' ? r : a;
            }
        );
    };
}
let User = {};
let baselayerid = "";
let map = null;
let bounds = [];
let container = new L.FeatureGroup();
let connectattp = 0;
let connecttimer;
let fotfmanager = $.connection.FOTFManager;
let timezone = {};
let condition = false;

$(function () {
    $("form").submit(function () { return false; });
    
    $(window).resize(function () {
        setHeight();
    });
    User = $.parseJSON(localStorage.getItem('User'));  
    if (!$.isEmptyObject(User)) {
        $(document).prop('title', User.Facility_Name + ' ' + User.ApplicationAbbr);
        $('#fotf-site-facility-name').append(User.Facility_Name);
        $(document).prop('title', User.Facility_Name + ' ' + User.ApplicationFullName);
        map.attributionControl.setPrefix("USPS " + User.ApplicationFullName + " (" + User.SoftwareVersion + ") | " + User.Facility_Name);
        //add Connection list if user is Admin or OIE
        if (/^(Admin|OIE|OPERATOR)/i.test(User.Role)) {
            sidebar.addPanel({
                id: 'connections',
                tab: '<span class="iconCenter"><i class="pi-iconDiagramOutline"></i></span>',
                position: 'top',
                pane: '<div class="btn-toolbar" role="toolbar" id="connection_div">' +
                    '<div id="div_agvnotification" class="container-fluid">' +
                    '<h4 class="ml-p5rem" >Connections</h4>' +
                    '<button type="button" class="btn btn-primary float-left mb-2 ml-p5rem" name="addconnection">Add</button>' +
                    '<div class="card w-100 bg-white mt-2 pb-1">' +
                    '<div class="card-body">' +
                    '<div class="table-responsive">' +
                    '<table class="table table-sm table-hover table-condensed mb-1" id="connectiontable" style="border-collapse:collapse;">' +
                    '<thead class="thead-dark">' +
                    '<tr>' +
                    '<th class="row-connection-name"><span class="ml-p5rem">Name</span></th><th class="row-connection-type">Message Type</th>' +
                    '<th class="row-connection-port">Port</th>' +
                    '<th class="row-connection-status">Status</th>' +
                    '<th class="row-connection-action"><span class="ml-p25rem">Action</span></th>' +
                    '</tr>' +
                    '</thead>' +
                    '<tbody></tbody>' +
                    '</table>' +
                    '</div>' +
                    '</div>' +
                    '</div >' +
                    '</div></div>'
            });

            $('button[name=addconnection]').off().on('click', function () {
                /* close the sidebar */
                sidebar.close();
                Add_Connection();
            });
            $('button[name=machineinfoedit]').css('display', 'block');
            //setup connection list
            init_connection($.parseJSON(User.ConnectionList));
         
        }
        if (User.hasOwnProperty("FacilityTimeZone")) {
            if (checkValue(User.FacilityTimeZone)) {
                timezone = { Facility_TimeZone: User.FacilityTimeZone }
                cBlock();
            }
        }
        if (User.hasOwnProperty("Environment")) {
            //Environment Status Controls
            if (/(DEV|SIT|CAT)/i.test(User.Environment)) {
                let Environment = L.Control.extend({
                    options: {
                        position: 'topright'
                    },
                    onAdd: function () {
                        let Domcntainer = L.DomUtil.create('input');
                        Domcntainer.type = "button";
                        Domcntainer.id = "environment";
                        Domcntainer.className = getEnv(User.Environment);
                        Domcntainer.value = User.Environment;
                        return Domcntainer;
                    }
                });
                map.addControl(new Environment());
                function getEnv(env) {
                    if (/CAT/i.test(env)) {
                        return "btn btn-outline-primary btn-sm";
                    }
                    else if (/SIT/i.test(env)) {
                        return "btn btn-outline-warning btn-sm";
                    }
                    else if (/DEV/i.test(env)) {
                        return "btn btn-outline-danger btn-sm";
                    }
                }
            }
        }
        if (/^Admin/i.test(User.Role)) {
            sidebar.addPanel({
                id: 'setting',
                tab: '<span class="iconCenter"><i class="pi-iconGearFill"></i></span>',
                position: 'bottom',
                pane: '<div class="btn-toolbar" role="toolbar" id="app_setting">' +
                        '<div id="div_app_settingtable" class="container-fluid">' +
                            '<div class="card w-100">' +
                                '<div class="card-header pl-1">' +
                                    '<h6 class="control-label sectionHeader ml-1 mb-1 d-flex justify-content-between">Workroom/Floor Image</h6>' +
                                    '<button type="button" class="btn btn-primary float-left mb-2 ml-p5rem" name="addfloorpan">Add Image</button>' +
                                        '<div class="table-responsive fixedHeader" style="max-height: calc(100vh - 100px); ">' +
                                        '<h2 class="control-label sectionHeader ml-1 mb-1 d-flex justify-content-between">Loaded Workroom/Floor Image</h2>' +
                                        '<table class="table table-sm table-hover table-condensed" id="floorplantable" style="border-collapse:collapse;">' +
                                        '<thead class="thead-dark">' +
                                        '<tr>' +
                                        '<th class="row-name"><span class="ml-p25rem">Name</span></th><th class="row-value">Value</th><th class="row-action">Action</th>' +
                                        '</tr>' +
                                        '</thead>' +
                                        '<tbody></tbody>' +
                                        '</table>' +
                                    '</div>' +
                                '</div>' +
                            '</div>' +
                    '<div class="card w-100">' +
                        '<div class="card-header pl-1">' +
                        '<h6 class="control-label sectionHeader ml-1 mb-1 d-flex justify-content-between">Application Setting</h6>' +
                            '</div>' +
                                '<div class="card-body">' +
                                    '<div class="table-responsive fixedHeader" style="max-height: calc(100vh - 100px); ">' +
                                        '<table class="table table-sm table-hover table-condensed" id="app_settingtable" style="border-collapse:collapse;">' +
                                        '<thead class="thead-dark">' +
                                        '<tr>' +
                                        '<th class="row-name"><span class="ml-p25rem">Name</span></th><th class="row-value">ID</th><th class="row-action">Action</th>' +
                                        '</tr>' +
                                        '</thead>' +
                                        '<tbody></tbody>' +
                                        '</table>' +
                                    '</div>' +
                                '</div>' +
                            '</div >' +
                        '</div>' +
                    '</div>' +
                  '</div>'
            });
            $('button[name=addfloorpan]').off().on('click', function () {
                /* close the sidebar */
                sidebar.close();
                Add_Floorplan();
            });

        }
        if (/(^PMCCUser$)/i.test(User.UserId)) {
            //add QRCode
            let QRCodedisplay = L.Control.extend({
                options: {
                    position: 'topright'
                },
                onAdd: function () {
                    let Domcntainer = L.DomUtil.create('div');
                    Domcntainer.id = "qrcodeUrl";
                    return Domcntainer;
                }
            });
            map.addControl(new QRCodedisplay());

            let qrcode = new QRCode("qrcodeUrl", {
                text: window.location.href,
                width: 128,
                height: 128,
                colorDark: "#000000",
                colorLight: "#ffffff",
                correctLevel: QRCode.CorrectLevel.H
            });
        }
    }
 
    // Search Tag Name
    $('input[id=inputsearchbtn').keyup(function () {
        startSearch(this.value)
    }).keydown(function (event) {
        if (event.which == 13) {
            event.preventDefault();
        }
    });
    $('#UserTag_Modal').on('hidden.bs.modal', function () {
        $(this)
            .find("input[type=text],textarea,select")
            .css({ "border-color": "#D3D3D3" })
            .val('')
            .end()
            .find("span[class=text]")
            .css("border-color", "#FF0000")
            .val('')
            .text("")
            .end()
            .find('input[type=checkbox]')
            .prop('checked', false).change();

        if (!$('#UserTag_Modal').hasClass('in')) {
            $('#UserTag_Modal').addClass('modal-open');
        }
    });

    $.extend(fotfmanager.client, {
        floorImage: async (MapData) => {
            //init_mapSetup(MapData);
        }
    });
  
    // Start the connection
    $.connection.hub.qs = { 'page_type': "CF".toUpperCase() };
    $.connection.hub.start({ waitForPageLoad: false })
        .done(function () {

            if (/^(Admin|OIE)/i.test(User.Role)) {
                init_geometry_editing();
            }
            if (!/^(Admin|OIE)/i.test(User.Role)) {
                fotfmanager.server.leaveGroup("PeopleMarkers");
            }
            Promise.all([init_mapSetup()]);
            conntoggle.state('conn-on');
        }).catch(
            function (err) {
                console.log(err.toString());
            });
   



    Initiate24HourClock();
    //add connection status
    let conntoggle = L.easyButton({
        position: 'topright',
        states: [
            {
                stateName: 'conn-off',
                icon: '<i class="conectionOff pi-iconPlugOutline" width="18" height="18" title="Connection OFF"></i>',
                title: 'Connection Off'
            },
            {
                stateName: 'conn-on',
                icon: '<i class="conectionOn pi-iconPlugOutline" width="18" height="18" title="Connection On"></i>',
                title: 'Connection On'
            },
            {
                stateName: 'conn-low',
                icon: '<i class="conectionSlow pi-iconPlugOutline" width="18" height="18" title="Connection Slow"></i>',
                title: 'Connection On'
            }]
    });
    conntoggle.addTo(map);

    //add legend
    L.easyButton({
        position: 'bottomright',
        states: [{
            stateName: 'legend',
            icon: '<div id="legendToggle" data-toggle="popover" title="Legend"><i class="pi-iconLegend align-self-center"></i></div>',
            title: 'Legend'
        }]
    }).addTo(map);

    $('#legendToggle').popover({
        html: true,
        trigger: 'click',
        content: $('#legendContent'),
    });
    $('#legendToggle').on('click', function () {
        // close the sidebar
        sidebar.close();
    });

    //add FAQ
    L.easyButton({
        position: 'bottomright',
        states: [{
            stateName: 'faq',
            icon: '<div id="faqToggle"  data-toggle="popover"  title="Frequently Asked Questions" class="iconCenter" ><i class="pi-iconFAQ align-self-center"></i></div> ',
            title: 'faq'
        }]
    }).addTo(map);

    $('#faqToggle').popover({
        html: true,
        trigger: 'click',
        content: $('#faqContent'),
    });
    $('#faqToggle').on('click', function () {
        // close the sidebar
        sidebar.close();
    });
    // SelectizeJs Init for searching select boxes.
    let options = {
        create: false,
        sortField: "text",
    }
    $('.selectize-dropdown').click(function (e) {
        e.stopPropagation();        // To fix zone select scroll bug. May need to be revisited
    })
    $zoneSelect = $("#zoneselect").selectize(options);
    /**User  Modal***/
    function EditUserInfo(layerindex) {
        $('#modaluserHeader_ID').text('Edit User Info');

        if ($.isNumeric(layerindex)) {
            layerindex = parseInt(layerindex);
        }
        sidebar.close();
        try {
            if (map._layers[layerindex].hasOwnProperty("feature")) {
                if (map._layers[layerindex].feature.hasOwnProperty("properties")) {
                    $('input[type=text][name=empId]').val(map._layers[layerindex].feature.properties.Employee_EIN);
                    $('input[type=text][name=empName]').val(map._layers[layerindex].feature.properties.Employee_Name);
                    $('input[type=text][name=tag_name]').val(map._layers[layerindex].feature.properties.name);
                    $('input[type=text][name=tag_id]').val(map._layers[layerindex].feature.properties.id);
                    $('button[id=usertagsubmitBtn]').off().on('click', function () {
                        try {
                            $('button[id=usertagsubmitBtn]').prop('disabled', true);
                            let jsonObject = {};
                            if ($('input[type=text][name=empId]').val() !== map._layers[layerindex].feature.properties.Employee_EIN) {
                                jsonObject.Employee_EIN = $('input[type=text][name=employee_ein]').val();
                            }
                            else {
                                jsonObject.Employee_EIN = "";
                            };
                            if ($('input[type=text][name=empName]').val() !== map._layers[layerindex].feature.properties.Employee_Name) {
                                jsonObject.Employee_Name = $('input[type=text][name=employee_name]').val()
                            }
                            else jsonObject.Employee_Name = "";
                            if (!$.isEmptyObject(jsonObject)) {
                                jsonObject.id = map._layers[layerindex].feature.properties.id;
                                $.connection.FOTFManager.server.editTagInfo(JSON.stringify(jsonObject)).done(function (Data) {
                                    if (Data.hasOwnProperty(ERROR_MESSAGE)) {
                                        $('span[id=error_usertagsubmitBtn]').text(Data.ERROR_MESSAGE);
                                    }
                                    if (Data[0].hasOwnProperty("MESSAGE_TYPE")) {
                                        $('span[id=error_usertagsubmitBtn]').text(Data.MESSAGE_TYPE);
                                        setTimeout(function () { $("#UserTag_Modal").modal('hide'); }, 3000);
                                    }
                                });
                            }
                            else {
                                $('span[id=error_usertagsubmitBtn]').text("No Tag Data has been Updated");
                                setTimeout(function () { $("#UserTag_Modal").modal('hide'); }, 3000);
                            }
                        } catch (e) {
                            $('span[id=error_usertagsubmitBtn]').text(e);
                        }
                    });

                    $('#UserTag_Modal').modal();
                }
            }
        } catch (e) {
            console.log(e);
        }
    }


    // Raised when the connection state changes. Provides the old state and the new state (Connecting, Connected, Reconnecting, or Disconnected).
    $.connection.hub.stateChanged(function (state) {
        switch (state.newState) {
            case 1:
                conntoggle.state('conn-on');
                break;
            case 2:
                conntoggle.state('conn-low');
                break;
            case 0:
                conntoggle.state('conn-low');
                break;
            case 4:
                conntoggle.state('conn-off');
                break;
            default:
        }
    });
    //handling Disconnect
    $.connection.hub.disconnected(function () {
        connecttimer = setTimeout(function () {
            if (connectattp > 10) {
                clearTimeout(connecttimer);
            }
            connectattp += 1;
            conntoggle.state('conn-off');
            $.connection.hub.start().done(function () {
                console.log("Connected time" + new Date($.now()));
            }).catch(function (err) {
                console.log(err.toString());
            });
        }, 10000); // Restart connection after 10 seconds.
    });
    //Raised when the underlying transport has reconnected.
    $.connection.hub.reconnecting(function () {
        conntoggle.state('conn-low');
        clearTimeout(connecttimer);
    });
    //Raised when the client detects a slow or frequently dropping connection
    $.connection.hub.connectionSlow(function () {
        conntoggle.state('conn-low');
    });
    async function LoadtagDetails(tagid) {
        try {
            if (map.hasOwnProperty("_layers")) {
                $.map(map._layers, function (layer, i) {
                    if (layer.hasOwnProperty("feature")) {
                        if (layer.feature.properties.id === tagid) {
                            map.setView(layer.getLatLng(), 3);
                            sidebar.open('home');
                            LoadVehicleTable(layer.feature.properties);
                            return false;
                        }
                    }
                });
            }
        } catch (e) {
            console.log(e);
        }
    }
 
    $(document).on('click', '.ctsdetails', function () {
        var td = $(this);
        var tr = $(td).closest('tr'),
            route = tr.attr('data-route'),
            trip = tr.attr('data-trip');
        $('div[id=CTS_Details_Modal]').modal();
        $('#ctsdetailsmodalHeader').text('Retrieving Data from CTS Please Wait, The data shall be loaded upon retrieval');
        LoadCTSDetails(tr, route, trip, "ctsdetailstable");
    });
    $(document).on('click', '.containerdetails', function () {
        var td = $(this);
        var doorid = td.attr('data-doorid');
        var placardid = td.attr('data-placardid');
        if (checkValue(doorid)) {
            LoadContainerDetails(doorid, placardid);
        }
    });
    //search tag edit
    $(document).on('click', '.tagedit', function () {
        var td = $(this);
        var tr = $(td).closest('tr'),
            layer_id = tr.attr('data-id');

        EditUserInfo(layer_id);
    });
    //search tag delete
    $(document).on('click', 'tagdelete', function () {
        var td = $(this);
        var tr = $(td).closest('tr'),
            layer_id = tr.attr('data-id');

        DeleteUserInfo(layer_id);
    });
    //search tag details
    $(document).on('click', '.tagdetails', function (e) {
        var td = $(this);
        var tagid = td.attr('data-tag');
        if (checkValue(tagid)) {
            LoadtagDetails(tagid);
        }
    });
    //search machine details
    $(document).on('click', '.machinedetails', function (e) {
        var td = $(this);
        var machZoneid = td.attr('data-machine');
        if (checkValue(machZoneid)) {
            LoadMachineDetails(machZoneid);
            sidebar.open('home');
        }
    });
    $(document).on('click', '.viewportszones', function () {
        try {
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            var selcValue = this.id;
            updateURIParametersForLayer($("#" + selcValue).html());
            if (viewPortsAreas.hasOwnProperty("_layers")) {
                $.map(viewPortsAreas._layers, function (layer, i) {
                    if (layer.hasOwnProperty("feature")) {
                        if (layer.feature.properties.id === selcValue) {
                            var Center = new L.latLng(
                                (layer._bounds._southWest.lat + layer._bounds._northEast.lat) / 2,
                                (layer._bounds._southWest.lng + layer._bounds._northEast.lng) / 2);
                            map.setView(Center, 3);
                            Loadtable(layer.feature.properties.id, 'viewportstable');
                            return false;
                        }
                    }
                });
            }
        } catch (e) {
            console.log(e);
        }
    });
    //$(document).on('click', '.connectionedit', function () {
    //    var td = $(this);
    //    var tr = $(td).closest('tr');
    //    var id = tr.attr('data-id');
    //    Edit_Connection(id);
    //});
    $(document).on('click', '.connectiondelete', function () {
        var td = $(this);
        var tr = $(td).closest('tr'),
            id = tr.attr('data-id');
        Remove_Connection(id);
    });
    $(document).on('click', '.camera_view', function () {
        let td = $(this);
        let tr = $(td).closest('tr');
        let id = tr.attr('data-id');
        let model = tr.attr('data-model');
        let description = tr.attr('data-description');
        View_Web_Camera(id, model, description);
    });
    ////set the trip to a door
    //$(document).on('click', '.tripSelectorbtn', function () {
    //    let td = $(this);
    //    let tr = $(td).closest('tr');
    //    let id = tr.attr('data-id');
    //    let model = tr.attr('data-model');
    //    let description = tr.attr('data-description');
    //    View_Web_Camera(id, model, description);
    //});
});
function removeTrips(routetrip)
{
    let trname = $.find('tbody tr[data-id=' + routetrip + ']');

    if (trname.length > 0) {
        for (let tr_name of trname) {
            let tablename = $(tr_name).closest('table').attr('id');
            $('#' + tablename).find('tbody tr[data-id=' + routetrip + ']').remove();
        }
    }
    $('#tripSelector option[id=' + routetrip + ']').remove();
}

function Page_Update(data) {
    $('#fotf-site-facility-name').append(data.FACILITY_NAME);
    $(document).prop('title', data.FACILITY_NAME + ' ' + data.APPLICATION_NAME);
}
function Map_Update(data) {
    map.attributionControl.setPrefix("USPS " + data.APPLICATION_FULLNAME + " (" + User.SoftwareVersion + ") | " + data.FACILITY_NAME);
}
function Clear() {
    let progress = 0;
    $('#progresbarrow').css('display', 'none');
    $('#file_upload_progressbar').css('width', progress + '%');
    $("#metersPerPixel").val("");
    $('input[type=file]').val('');
    $('input[type=radio]').prop("checked", "");
    $('span[id=error_btnUpload]').text("");
    $('button[id=btnUpload]').prop("disabled", false);
}
//sort table header
$('th').click(function () {
    var $th = $(this).closest('th');
    $th.toggleClass('selected');
    var isSelected = $th.hasClass('selected');
    var isInput = $th.hasClass('input');
    var column = $th.index();
    var $table = $th.closest('table');
    var isNum = $table.find('tbody > tr').children('td').eq(column).hasClass('num');
    var rows = $table.find('tbody > tr').get();
    rows.sort(function (rowA, rowB) {
        if (isInput) {
            var keyA = $(rowA).children('td').eq(column).children('input').val().toUpperCase();
            var keyB = $(rowB).children('td').eq(column).children('input').val().toUpperCase();
        } else {
            var keyA = $(rowA).children('td').eq(column).text().toUpperCase();
            var keyB = $(rowB).children('td').eq(column).text().toUpperCase();
        }
        if (isSelected) return OrderBy(keyA, keyB, isNum);
        return OrderBy(keyB, keyA, isNum);
    });
    $.each(rows, function (index, row) {
        $table.children('tbody').append(row);
    });
    return false;
});
function setHeight() {
    let height = (this.window.innerHeight > 0 ? this.window.innerHeight : this.screen.height) - 1;
    $('div[id=map]').css("min-height", height + "px");
}
function SortByVehicleName(a, b) {
    if (a.VEHICLENAME < b.VEHICLENAME) {
        return -1;
    }
    else if (a.VEHICLENAME > b.VEHICLENAME) {
        return 1;
    } else {
        return 0;
    }
    /*return a.VEHICLENAME < b.VEHICLENAME ? -1 : a.VEHICLENAME > b.VEHICLENAME ? 1 : 0;*/
}
function formatTime(value_time) {
    try {
        if (!$.isEmptyObject(timezone)) {
            if (timezone.hasOwnProperty("Facility_TimeZone")) {
                let time = moment(value_time);
                if (time._isValid) {
                    return time.format("HH:mm");
                }
            }
        }
    } catch (e) {
        console.log(e);
    }
}
function formatDateTime(value_time) {
    try {

        let time = moment(value_time);
        if (time._isValid) {
            return time.format("M/D/YYYY h:mm a");
        }

    } catch (e) {
        console.log(e);
    }
}
function capitalize_Words(str) {
    try {
        return str.replace(/\w\S*/g, function (txt) {
            return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
        });
    } catch (e) {
        console.log(e);
    }
  
}
function IPAddress_validator(value) {
    let ipPattern = /^(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})$/;
    let ipArray = value.match(ipPattern);
    if (value === "0.0.0.0" || value === "255.255.255.255" || ipArray === null)
        return "Invalid IP Address";
    else {
        for (let i = 1; i < ipArray.length; i++) {
           let thisSegment = ipArray[i];
            if (thisSegment > 255) {
                return "Invalid IP Address";
            }
            if (i === 0 && thisSegment > 255) {
                return "Invalid IP Address";
            }
        }
    }
    return value;
}
//order
function OrderBy(a, b, n) {
    if (n) return a - b;
    if (a < b) return -1;
    if (a > b) return 1;
    return 0;
}
function SortByTagName(a, b) {
    if (a.name < b.name) {
        return -1;
    }
    else if (a.name > b.name) {
        return 1
    } else {
           return 0;
           }
    //return a.name < b.name ? -1 : a.name > b.name ? 1 : 0;
}
function SortByName(a, b) {
    let aName = a.toLowerCase();
    let bName = b.toLowerCase();
    if (aName < bName) {
        return -1;
    }
    else if (aName > bName) {
        return 1
    } else {
        return 0;
    }

    //return ((aName < bName) ? -1 : ((aName > bName) ? 1 : 0));
}
function SortBySiteName(a, b) {
    let aName = a.legSiteName.toLowerCase();
    let bName = b.legSiteName.toLowerCase();
    if (aName < bName) {
        return -1;
    }
    else if (aName > bName) {
        return 1
    } else {
        return 0;
    }
    //return ((aName < bName) ? -1 : ((aName > bName) ? 1 : 0));
}
function SortByLocationName(a, b) {
    let aName = a.toLowerCase();
    let bName = b.locationName.toLowerCase();
    if (aName < bName) {
        return -1;
    }
    else if (aName > bName) {
            return 1
         } else {
                  return 0;
         }
    //return ((aName < bName) ? -1 : ((aName > bName) ? 1 : 0));
}
function SortByNumber(a, b) {
    return a - b;
}
function GetPeopleInZone(zone, P2Pdata, staffarray) {
    //staffing
    let planstaffarray = [];
    let planstaffCounts = [];
    let plansumstaffCounts = {};
    let staffCounts = [];
    let sumstaffCounts = {};
    try {
        if (staffarray.length === 0) {
            if (tagsMarkersGroup.hasOwnProperty("_layers")) {
                $.map(tagsMarkersGroup._layers, function (layer, i) {
                    if (layer.hasOwnProperty("feature")) {
                        if (/person/i.test(layer.feature.properties.Tag_Type)) {
                            if (layer.feature.properties.hasOwnProperty("zones")) {
                                $.map(layer.feature.properties.zones, function (p_zone) {
                                    if (p_zone.id == zone) {
                                        if (layer.hasOwnProperty("_tooltip")) {
                                            if (layer._tooltip.hasOwnProperty("_container")) {
                                                if (!layer._tooltip._container.classList.contains('tooltip-hidden')) {
                                                    let name = "";
                                                    let nameId = "";
                                                    if (checkValue(layer.feature.properties.craftName)) {
                                                        name = layer.feature.properties.craftName;
                                                    }
                                                    else {
                                                        layer.feature.properties.id;
                                                    }
                                                    if (checkValue(layer.feature.properties.id) {
                                                        nameId = layer.feature.properties.id;
                                                    }
                                                    else {
                                                        nameId = layer.feature.properties.id;
                                                    }
                                                    staffarray.push({
                                                        name:  name,
                                                        nameId: nameId,
                                                        id: layer.feature.properties.id
                                                    })
                                                }
                                            }
                                        }
                                    }
                                });
                            }
                        }
                    }
                });
            }
        }

        if (checkValue(P2Pdata)) {
            if (P2Pdata.hasOwnProperty("clerk")) {
                if (P2Pdata.clerk >= 1) {
                    for (let i = 0; i < P2Pdata.clerk; i++) {
                        planstaffarray.push({
                            name: "Clerk",
                            value: i
                        })
                    }
                    //planstaffarray.push({
                    //    name: "Clerk",
                    //    value: P2Pdata.clerk
                    //})
                }
            }
            if (P2Pdata.hasOwnProperty("mh")) {
                if (P2Pdata.mh >= 1) {
                    for (let r = 0; r < P2Pdata.mh; r++) {
                        planstaffarray.push({
                            name: "MailHandler",
                            value: r
                        })
                    }
                    //planstaffarray.push({
                    //    name: "MailHandler",
                    //    value: P2Pdata.mh
                    //})
                }
            }
        }

        $.each(staffarray, function (i, e) {
            sumstaffCounts[this.name] = (sumstaffCounts[this.name] || 0) + 1;
        });
        $.each(sumstaffCounts, function (index, value) {
            staffCounts.push({
                name: index,
                currentstaff: value
            });
        });

        $.each(planstaffarray, function (i, e) {
            plansumstaffCounts[this.name] = (plansumstaffCounts[this.name] || 0) + 1;
        });
        $.each(plansumstaffCounts, function (index, value) {
            planstaffCounts.push({
                name: index,
                planstaff: value
            });
        });
        let staffing = $.extend(true, staffCounts, planstaffCounts);
        staffing.sort(SortByTagName);

        $('div[id=staff_div]').css('display', 'block');
        let stafftop_Table = $('table[id=stafftable]');
        let stafftop_Table_thead = stafftop_Table.find('thead');
        stafftop_Table_thead.empty();
        stafftop_Table_thead.append('<tr><th>F1 Craft</th><th>Current</th><th>Planned</th></tr>');
        let stafftop_Table_Body = stafftop_Table.find('tbody');
        stafftop_Table_Body.empty();
  
        
        staffCounts.push({
            name: 'Total',
            currentstaff: staffarray.length,
            planstaff: planstaffarray.length
        });
        $.each(staffCounts, function () {
            stafftop_Table_Body.append(stafftop_row_template.supplant(formatstafftoprow(this)));
        })
    } catch (e) {
        console.log(e);
    }
}
let stafftop_row_template = '<tr data-id={staffName}{style}>' +
    '<td class="text-center">{staffName}</td>' +
    '<td class="text-center">{staffNameId}</td>' +
    '<td class="text-center">{planstaff}</td>' +
    '</tr>"';
function formatstafftoprow(properties) {
    return $.extend(properties, {
        staffName: properties.name,
        staffId: properties.id,
        staffNameId: properties.hasOwnProperty("currentstaff") ? properties.currentstaff : 0,
        planstaff: properties.hasOwnProperty("planstaff") ? properties.planstaff : 0,
        style: GetStaffRowStyle(properties.hasOwnProperty("currentstaff") ? properties.currentstaff : 0, properties.hasOwnProperty("planstaff") ? properties.planstaff : 0),
    });
}
function GetStaffRowStyle(currStaff, planStaff) {
    let rowAlertColor = ' style="background-color:rgba(220, 53, 69, 0.5);"';
    let rowWarningColor = ' style="background-color:rgba(255, 193, 7, 0.5);"';
    if (planStaff > 4) {
        if (currStaff < planStaff) {
            if ((currStaff / planStaff) * 100 > 95) {
                return rowWarningColor
            }
            else {
                return rowAlertColor
            }
        }
    }
    return "";
}
function digits(num) {
    return num.replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
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
function pad(str, max) {
    str = str.toString();
    return str.length < max ? pad("0" + str, max) : str;
}
function objSVTime(t) {
    try {
        if (t !== null) {
            let time = moment().set({ 'year': t.year, 'month': t.month + 1, 'date': t.dayOfMonth, 'hour': t.hourOfDay, 'minute': t.minute, 'second': t.second });
            if (time._isValid) {
                if (time.year() === 1) {
                    return "";
                }
                return time.format("HH:mm");
            }
        }
        else {
            return "";
        }
    } catch (e) {
        console.log(e);
    }
}
async function updateTime(t) {
    $('#localTime').val(moment(t).format('H:mm'));
}
function formatSVmonthdayTime(t) {
    try {
        if (checkValue(t)) {
            let time = moment().set({ 'year': t.year, 'month': t.month, 'date': t.dayOfMonth, 'hour': t.hourOfDay, 'minute': t.minute, 'second': t.second });
            if (time._isValid) {
                if (time.year() === 1) {
                    return "";
                }
                return time.format("MM/DD/YYYY HH:mm");
            }
            else {
                return "";
            }
        }
        else {
            return "";
        }
    } catch (e) {
        console.log(e);
        return "";
    }
}
function cBlock() {
    let t = moment().tz(timezone.Facility_TimeZone)
    $('#localTime').val(moment(t).format('H:mm:ss'));
    $('#twentyfourmessage').text(GetTwentyFourMessage(t));
   
    if ($("#tfhcContent").length > 0) {
        SetClockHands(t);
    }
    let visible = sidebar._getTab("reports");
    if (visible) {
        if (visible.classList.length) {
            if (visible.classList.contains('active')) {
                GetUserInfo();
            }
        }
    }
    setTimeout(cBlock, 1000);
    Promise.all([zonecurrentStaff()]);
}
// current zone staff
//TODO: look in to this more.
async function zonecurrentStaff() {
    try {
        //clear Old tags 
        if (tagsMarkersGroup.hasOwnProperty("_layers")) {
            $.map(tagsMarkersGroup._layers, (layer) => {

                if (/person/i.test(layer.feature.properties.Tag_Type)) {
                    if (layer.feature.properties.tagVisible === true) {
                        if (layer.feature.properties.hasOwnProperty("positionTS")) {
                            let startTime = moment(layer.feature.properties.positionTS);
                            let endTime = moment();
                            let diffmillsec = endTime.diff(startTime, "milliseconds");

                            if (diffmillsec > layer.feature.properties.tagVisibleMils) {
                                layer.feature.properties.tagVisibleMils = diffmillsec;
                            }
                        }
                        if (layer.feature.properties.tagVisibleMils > 80000) {
                            layer.feature.properties.tagVisible = false;
                            //this to hide tooltip
                            if (layer.hasOwnProperty("_tooltip")) {
                                if (layer._tooltip.hasOwnProperty("_container")) {
                                    if (!layer._tooltip._container.classList.contains('tooltip-hidden')) {
                                        layer._tooltip._container.classList.add('tooltip-hidden');
                                    }
                                }
                            }
                        }
                    }
                }

            })
        }
        // Machine zone
        if (polygonMachine.hasOwnProperty("_layers")) {
            $.map(polygonMachine._layers, function (Machinelayer, i) {
                let MachineCurrentStaff = [];
                if (tagsMarkersGroup.hasOwnProperty("_layers")) {
                    $.map(tagsMarkersGroup._layers, function (layer, i) {
                        if (/person/i.test(layer.feature.properties.Tag_Type) && layer.feature.properties.zones != null) {
                            
                            $.map(layer.feature.properties.zones, function (p_zone) {

                                if (p_zone.id == Machinelayer.feature.properties.id) {
                                    
                                               MachineCurrentStaff.push({
                                                    name: checkValue(layer.feature.properties.craftName) ? layer.feature.properties.craftName : layer.feature.properties.id,
                                                    nameId: checkValue(layer.feature.properties.id) ? layer.feature.properties.id : layer.feature.properties.id,
                                                    id: layer.feature.properties.id
                                                })
                               
                                }
                            });
                        }
                    });
                }

                if (Machinelayer.hasOwnProperty("_tooltip")) {
                    if (MachineCurrentStaff.length !== Machinelayer.feature.properties.CurrentStaff) {
                        Machinelayer.setTooltipContent(Machinelayer.feature.properties.name + "<br/>" + "Staffing: " + MachineCurrentStaff.length);
                        Machinelayer.feature.properties.CurrentStaff = MachineCurrentStaff.length;
                        if ($('select[id=zoneselect] option:selected').val() === Machinelayer.feature.properties.id) {
                            let p2pdata = Machinelayer.feature.properties.hasOwnProperty("P2PData") ? Machinelayer.feature.properties.P2PData : "";
                            GetPeopleInZone(Machinelayer.feature.properties.id, p2pdata, MachineCurrentStaff);
                        }
                    }
                }
            });
        }

        // other zone
        if (stagingAreas.hasOwnProperty("_layers")) {
            $.map(stagingAreas._layers, function (stagelayer, i)
            {
                
                let CurrentStaff = [];
                if (tagsMarkersGroup.hasOwnProperty("_layers")) {
                    $.map(tagsMarkersGroup._layers, function (layer, i) {
                       
                        if (/person/i.test(layer.feature.properties.Tag_Type) && layer.feature.properties.zones != null) {
                           
                            $.map(layer.feature.properties.zones, function (p_zone) {
                                if (p_zone.id == stagelayer.feature.properties.id) {
                                    
                                              CurrentStaff.push({
                                                    name: checkValue(layer.feature.properties.craftName) ? layer.feature.properties.craftName : layer.feature.properties.id,
                                                    nameId: checkValue(layer.feature.properties.id) ? layer.feature.properties.id : layer.feature.properties.id,
                                                    id: layer.feature.properties.id
                                                })
                                       
                                }
                            });
                        }

                    });
                }

                if (stagelayer.hasOwnProperty("_tooltip")) {
                    if (CurrentStaff.length !== stagelayer.feature.properties.CurrentStaff) {
                        stagelayer.setTooltipContent(stagelayer.feature.properties.name + "<br/>" + "Staffing: " + CurrentStaff.length);
                        stagelayer.feature.properties.CurrentStaff = CurrentStaff.length;
                        if (stagingAreas.hasOwnProperty("feature")) {
                            if ($('select[id=zoneselect] option:selected').val() === stagingAreas.feature.properties.id) {
                                GetPeopleInZone(stagelayer.feature.properties.idp2pdata, CurrentStaff);
                            }
                        }
                    }
                }
            });
        }
    } catch (e) {
        console.log(e);
    }
}

function checkViewportLoad() {
    if ($.urlParam('viewport')) {
        viewportSelectedByName($.urlParam('viewport'));
    }
}

function viewportSelectedByName(name) {

    $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
    let selcValue = null;
    $('button').each(function () {
        if ($(this).text() === name && $(this).hasClass('viewportszones')) {
            selcValue = $(this).attr("id");
        }
    });
    if (selcValue === null) return;
    if (viewPortsAreas.hasOwnProperty("_layers")) {
        $.map(viewPortsAreas._layers, function (layer, i) {
            if (layer.hasOwnProperty("feature")) {
                if (layer.feature.properties.id === selcValue) {
                    let Center = new L.latLng(
                        (layer._bounds._southWest.lat + layer._bounds._northEast.lat) / 2,
                        (layer._bounds._southWest.lng + layer._bounds._northEast.lng) / 2);
                    map.setView(Center, 3);
                    Loadtable(layer.feature.properties.id, 'viewportstable');
                    return false;
                }
            }
        });
    }
}

function hideSidebarLayerDivs() {

    $('div[id=agvlocation_div]').css('display', 'none');
    $('div[id=area_div]').css('display', 'none');
    $('div[id=bullpen_div]').css('display', 'none');
    $('div[id=dockdoor_div]').css('display', 'none');
    $('div[id=trailer_div]').css('display', 'none');
    $('div[id=machine_div]').css('display', 'none');
    $('div[id=staff_div]').css('display', 'none');
    $('div[id=ctstabs_div]').css('display', 'none');
    $('div[id=vehicle_div]').css('display', 'none');
    $('div[id=dps_div]').css('display', 'none');
    $('div[id=layer_div]').css('display', 'none');
    $('div[id=dockdoor_tripdiv]').css('display', 'none');

}
$('#AppSetting_value_Modal').on('hidden.bs.modal', function () {
    $(this)
        .find("input[type=text],textarea,select")
        .css({ "border-color": "#D3D3D3" })
        .val('')
        .end()
        .find("span[class=text]")
        .css("border-color", "#FF0000")
        .val('')
        .text("")
        .end()
        .find('input[type=checkbox]')
        .prop('checked', false).change();

    if (!$('#AppSetting_Modal').hasClass('in')) {
        $('#AppSetting_Modal').addClass('modal-open');
    }
});
$('#AppSetting_value_Modal').on('shown.bs.modal', function () {
    $('span[id=error_appsettingvalue]').text("");
    $('button[id=appsettingvalue]').prop('disabled', false);
    //Connection name Validation
    if (!checkValue($('input[id=modalValueID]').val())) {
        $('input[id=modalValueID]').removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_modalValueID]').text("Please Enter Value");
    }
    else {
        $('input[id=modalValueID]').removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_modalValueID]').text("");
    }
});
//app setting
function Edit_AppSetting(table) {
    fotfmanager.server.getAppSettingdata().done(function (AppsettingData) {
        if (AppsettingData) {
          
            LoadappSettingTable(AppsettingData, table);
            Page_Update(AppsettingData);
            Map_Update(AppsettingData);
        }
    });
}
function Get_Action_State() {
    if (/^Admin/i.test(User.Role)) {
        return '<div class="btn-toolbar" role="toolbar">' +
            '<div class="btn-group mr-2" role="group" >' +
            '<button class="btn btn-light btn-sm mx-1 pi-iconEdit" name="editappsetting"></button>' +
            '</div>';
    }
    else {
        return '';
    }
}
function Edit_AppSetting_Value(id, value, table) {
    $('#appsettingvaluemodalHeader').text('Edit ' + id + ' Setting');
    $('input[id=modalKeyID]').val(id);
    $('input[id=modalValueID]').val(value);
    if (/TIMEZONE/i.test(id)) {
        fotfmanager.server.getTimeZone().done(function (data) {
            $('.valuediv').css("display", "none");
            $('.timezonediv').css("display", "block");
            $('#timezoneValueID').empty();
            $('<option/>').val("").appendTo('#timezoneValueID');
            $.each(data, function () {
                $('<option/>').val(this).html(this).appendTo('#timezoneValueID');
            })
            $('#timezoneValueID').val(value);
        })
    }
    else {
        $('.valuediv').css("display", "block");
        $('.timezonediv').css("display", "none");
    }
    $('button[id=appsettingvalue]').off().on('click', function () {
        $('button[id=appsettingvalue]').prop('disabled', true);
        var jsonObject = {};
        if (/TIMEZONE/i.test(id)) {
            jsonObject[id] = $("#timezoneValueID option:selected").val();
        }
        else {
            jsonObject[id] = $('input[id=modalValueID]').val();
        }
        if (!$.isEmptyObject(jsonObject)) {
            fotfmanager.server.editAppSettingdata(JSON.stringify(jsonObject)).done(function () {

                $('span[id=error_appsettingvalue]').text("Data has been updated");
                setTimeout(function () { $("#AppSetting_value_Modal").modal('hide'); }, 1500);
                Edit_AppSetting(table);

            });
        }
    });
    $('#AppSetting_value_Modal').modal();
}
async function LoadappSettingTable(AppsettingData, table) {
    let AppSettingTable = $('table[id=' + table + ']');
    let AppSettingTable_Body = AppSettingTable.find('tbody');
    let AppSettingTable_row_template = '<tr data-id="{id}" data-value="{value}">' +
        '<td ><span class="ml-p25rem">{id}</span></td>' +
        '<td >{value}</td>' +
        '<td>{action}</td>' +
        '</tr>';

    AppSettingTable_Body.empty();
    let index = 0;
    function formatAppSetting(key, value, index) {
        return $.extend(key, value, index, {
            number: index,
            id: key,
            value: value,
            action: Get_Action_State()
        });
    }
    $.each(AppsettingData, function (key, value) {
        if (!/REMOTEDB|SERVER_ACTIVE|SERVER_ACTIVE_HOSTNAME/i.test(key)) {
            AppSettingTable_Body.append(AppSettingTable_row_template.supplant(formatAppSetting(key, value, index++)));
        }
    });
    $('button[name=editappsetting]').on('click', function () {
        let td = $(this);
        let tr = $(td).closest('tr'),
            id = tr.attr('data-id'),
            value = tr.attr('data-value');
        Edit_AppSetting_Value(id, value, table);
    });
}
let nextSibling, fileName ="", checked;
$(function () {
  
    $("#fupload").keyup(function (e) {
        if (this.value.length === 0) {
            $("button[id=btnUpload]").prop("disabled", true);
        }
        else {
            $("button[id=btnUpload]").prop("disabled", false);
        }

    });
    $('button[id=btnUpload]').on('click', function () {
        let progress = 10;
        $('button[id=btnUpload]').prop("disabled", true);
        let fileUpload = $("#fupload").get(0);
        let files = fileUpload.files;
        if (files.length > 0) {
            let data = new FormData();
            for (const element of files) {
                data.append(element.name, element);
             
            }
            data.append("name", $('input[type=text][id=floorname]').val());
            data.append("metersPerPixel", $("#metersPerPixel option:selected").val());
            $.ajax({
                url: window.location.href + "/api/UploadFiles",
                type: "POST",
                data: data,
                cache: false,
                contentType: false,
                processData: false,
                beforeSend: function () {
                    $('button[id=btnUpload]').prop("disabled", true);
                    $('#progresbarrow').css('display', 'block');
                    $('span[id=error_btnUpload]').text("Loading File Please stand by");
              
                    $('#file_upload_progressbar').css('width', progress + '%');
                },
                xhr: function () {
                    let xhr = $.ajaxSettings.xhr();
                    if (xhr.upload) {
                        xhr.upload.addEventListener("progress", function (evt) {
                            if (evt.lengthComputable) {
                                let percentComplete = evt.loaded / evt.total;
                                percentComplete = parseInt(percentComplete * 100);
                                $('#file_upload_progressbar').attr('aria-valuenow', percentComplete).css('width', percentComplete + '%');
                                if (percentComplete === 100) {
                                    $('span[id=error_btnUpload]').text("File Transfer Complete -->> Processing File ");
                                }
                            }
                        }, false);
                    }
                    return xhr;
                },
                success: function (response) {
                    if (response !== "") {
                        try {
                            $('span[id=error_btnUpload]').text("File Processing Completed");
                            setTimeout(function () { Clear(); }, 500);
                        }
                        catch (e) {
                            $('span[id=error_btnUpload]').text(e);
                        }
                    }
                },
                error: function (response) {
                    $('span[id=error_btnUpload]').text(response.statusText);
                    $('#progresbarrow').css('display', 'none');
                    setTimeout(function () { Clear(); }, 10000);
                },
                failure: function (response) {
                    $('span[id=error_btnUpload]').text(response.statusText);
                    $('#progresbarrow').css('display', 'none');
                    setTimeout(function () { Clear(); }, 10000);
                }
            })

        }
    });
});
//on open set rules
$('#FloorPlan_Modal').on('shown.bs.modal', function () {
    if (fileName === "" && $('select[id=metersPerPixel] option:selected').val() === "") {
        $("button[id=btnUpload]").prop("disabled", true);
    }
    else {
        $("button[id=btnUpload]").prop("disabled", false );
    }
    $('select[name=metersPerPixel]').change(function () {
        if (fileName !== "" && $('select[id=metersPerPixel] option:selected').val() !== "") {
            $("button[id=btnUpload]").prop("disabled", false);
        }
        else {
            $("button[id=btnUpload]").prop("disabled", true);
        }
    });
    $("#fupload").change(function (e) {
        if (document.getElementById("fupload").files.length > 0) {
            fileName = document.getElementById("fupload").files[0].name;
            nextSibling = e.target.nextElementSibling
            nextSibling.innerText = fileName
        }
        else {
            fileName = "";
            nextSibling = e.target.nextElementSibling
            nextSibling.innerText = 'Choose file'
        }

        $('span[id=error_btnUpload]').text("");
        if (fileName === "" && $('select[id=metersPerPixel] option:selected').val() !== "") {
            $("button[id=btnUpload]").prop("disabled", false);
        }
        else {
            $("button[id=btnUpload]").prop("disabled", true);
        }
    });
});
$('#FloorPlan_Modal').on('hidden.bs.modal', function () {
    $(this)
        .find("input[type=text],textarea,select")
        .css({ "border-color": "#D3D3D3" })
        .val('')
        .end()
        .find("span[class=text]")
        .css("border-color", "#FF0000")
        .val('')
        .text("")
        .end()
        .find('input[type=checkbox]')
        .prop('checked', false).change();
    let progress = 0;
    $('#progresbarrow').css('display', 'none');
    $('#file_upload_progressbar').css('width', progress + '%');
    $('input[id=fupload][type=file]').val("");
    document.getElementById("fupload").nextElementSibling.innerText = 'Choose file';
    $('span[id=error_btnUpload]').text("");
    $('button[id=btnUpload]').prop("disabled", true);
    sidebar.open('setting');
});
function Add_Floorplan() {
    $('#floormodalHeader_ID').text('Add New Floor Plan');
    $('#FloorPlan_Modal').modal();
}
function RemoveFloorPlan(id, value, table) {
    var jsonObject = {
        Id: value
    };
    if (!$.isEmptyObject(jsonObject)) {
        fotfmanager.server.removeFloorplan(JSON.stringify(jsonObject)).done(function () {

            $('span[id=error_appsettingvalue]').text("Data has been updated");
            setTimeout(function () { $("#AppSetting_value_Modal").modal('hide'); }, 1500);
            Edit_AppSetting(table);

        });
    }
}
async function Load_Floorplan_Table(table) {
    let FloorplanTable = $('table[id=' + table + ']');
    let FloorplanTable_Body = FloorplanTable.find('tbody');
    let FloorplanTable_row_template = '<tr data-id="{id}" data-value="{value}">' +
        '<td ><span class="ml-p25rem">{id}</span></td>' +
        '<td >{value}</td>' +
        '<td>{action}</td>' +
        '</tr>';

    FloorplanTable_Body.empty();
    fotfmanager.server.getFloorPlanData().done(function (FloorplanData) {
        if (FloorplanData) {
            $.each(FloorplanData, function () {
                FloorplanTable_Body.append(FloorplanTable_row_template.supplant(formatFloorplan(this)));
            });
        }
    });
    $('button[name=removeFloorplan]').on('click', function () {
        let td = $(this);
        let tr = $(td).closest('tr'),
            id = tr.attr('data-id'),
            value = tr.attr('data-value');
        RemoveFloorPlan(id, value, table);
    });
}
function formatFloorplan(data) {
    return $.extend(data, {
        number: data.id,
        id: data.name,
        value: data.id,
        action: Get_Floor_Action_State()
    });
}
function Get_Floor_Action_State() {
    if (/^Admin/i.test(User.Role)) {
        return '<div class="btn-toolbar" role="toolbar">' +
            '<div class="btn-group mr-2" role="group" >' +
            '<button class="btn btn-light btn-sm mx-1 pi-trashFill" name="removeFloorplan" onclick="RemoveFloorPlan($(this))"></button>' +
            '</div>';
    }
    else {
        return '';
    }
}

/**
* this is use to setup a the Notification information and other function
*
* **/

$.extend(fotfmanager.client, {
    updateNotification: async (updatenotification) => { updateNotification(updatenotification) }
});
$('#Notification_Setup_Modal').on('hidden.bs.modal', function () {
    $(this)
        .find("input[type=text],textarea,select")
        .val('')
        .end()
        .find("span[class=text]")
        .val('')
        .text("")
        .end()
        .find('input[type=checkbox]')
        .prop('checked', false).change()
        .end();
});
$('#Notification_Setup_Modal').on('shown.bs.modal', function () {
    $('.warning_conditionpickvalue').html($('input[id=warning_condition]').val());
    $('input[id=warning_condition]').on('input change', () => {
        $('.warning_conditionpickvalue').html($('input[id=warning_condition]').val());
    });
    $('.critical_conditionpickvalue').html($('input[id=critical_condition]').val());
    $('input[id=critical_condition]').on('input change', () => {
        $('.critical_conditionpickvalue').html($('input[id=critical_condition]').val());
    });
    $('span[id=error_notificationsubmitBtn]').text("");
    //Condition name Validation
    if (!checkValue($('input[type=text][name=condition_name]').val())) {
        $('input[type=text][name=condition_name]').removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_condition_name]').text("Please enter Condition Name");
    }
    else {
        $('input[type=text][name=condition_name]').removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_condition_name]').text("");
    }
    // condition Validation
    if (!checkValue($('input[type=text][name=condition]').val())) {
        $('input[type=text][name=condition]').removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_condition]').text("Please Enter Condition");
    }
    else {
        $('input[type=text][name=condition]').removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_condition]').text("");
    }
    // condition_type type Validation
    //if (!checkValue($('select[name=condition_type]').val())) {
    //    $('select[name=condition_type]').removeClass('is-valid').addClass('is-invalid');
    //    $('span[id=error_condition_type]').text("Please Select Condition Type");
    //}
    //else {
    //    $('select[name=condition_type]').removeClass('is-invalid').addClass('is-valid');
    //    $('span[id=error_condition_type]').text("");
    //}
    //warning_condition Validation
    if (!checkValue($('input[type=text][name=warning_condition]').val())) {
        $('input[type=text][name=warning_condition]').removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_warning_condition]').text("Please Condition Time in Minutes");
    }
    else {
        $('input[type=text][name=warning_condition]').removeClass('is-invalid').addClass('is-valid');
        $('span[id=warning_condition]').text("");
    }
    //critical_condition Validation
    if (!checkValue($('input[type=text][name=critical_condition]').val())) {
        $('input[type=text][name=critical_condition]').removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_critical_condition]').text("Please Condition Time in Minutes");
    }
    else {
        $('input[type=text][name=critical_condition]').removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_critical_condition]').text("");
    }
    enableNotificationSubmit();

    $('input[type=text][name=critical_condition]').keyup(function () {
        if ($.isNumeric($('input[type=text][name=critical_condition]').val())) {
            if (!validateNum(parseInt($('input[type=text][name=critical_condition]').val()), 0, 60)) {
                $('input[type=text][name=critical_condition]').removeClass('is-valid').addClass('is-invalid');
                $('span[id=error_critical_condition]').text("Invalid Number");
            }
            else {
                $('input[type=text][name=critical_condition]').removeClass('is-invalid').addClass('is-valid');
                $('span[id=error_critical_condition]').text("");
            }
        }
        else if (checkValue($('input[type=text][name=critical_condition]').val())) {
            $('input[type=text][name=critical_condition]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_critical_condition]').text("Enter Number");
        }
        else if (!checkValue($('input[type=text][name=critical_condition]').val())) {
            $('input[type=text][name=critical_condition]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_critical_condition]').text("Enter Number");
        }
        enableNotificationSubmit();
    });
    $('input[type=text][name=warning_condition]').keyup(function () {
        if ($.isNumeric($('input[type=text][name=warning_condition]').val())) {
            if (!validateNum(parseInt($('input[type=text][name=warning_condition]').val()), 0, 60)) {
                $('input[type=text][name=warning_condition]').removeClass('is-valid').addClass('is-invalid');
                $('span[id=error_warning_condition]').text("Invalid Number");
            }
            else {
                $('input[type=text][name=warning_condition]').removeClass('is-invalid').addClass('is-valid');
                $('span[id=error_warning_condition]').text("");
            }
        }
        else if (checkValue($('input[type=text][name=warning_condition]').val())) {
            $('input[type=text][name=warning_condition]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_warning_condition]').text("Enter Number");
        }
        else if (!checkValue($('input[type=text][name=warning_condition]').val())) {
            $('input[type=text][name=warning_condition]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_warning_condition]').text("Enter Number");
        }
        enableNotificationSubmit();
    });
    $('input[type=text][name=condition]').keyup(function () {
        if (!checkValue($('input[type=text][name=condition]').val())) {
            $('input[type=text][name=condition]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_condition]').text("Please Enter Condition");
        }
        else {
            $('input[type=text][name=condition]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_condition]').text("");
        }
        enableNotificationSubmit();
    });
    $('input[type=text][name=condition_name]').keyup(function () {
        //hostname Validation

        if (!checkValue($('input[type=text][name=condition_name]').val())) {
            $('input[type=text][name=condition_name]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_condition_name]').text("Please Enter Condition Name");
        }
        else {
            $('input[type=text][name=condition_name]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_condition_name]').text("");
        }
        enableNotificationSubmit();
    });
    $('select[name=condition_type]').change(function () {
        if (!checkValue($('select[name=condition_type]').val())) {
            $('select[name=condition_type]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_condition_type]').text("Please Select Condition Type");
        }
        else {
            $('select[name=condition_type]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_condition_type]').text("");
        }
        enableNotificationSubmit();
    });
    if ($('select[name=condition_type]').val() === "") {
        $('select[name=condition_type]').removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_condition_type]').text("Please Select Condition Type");
    }
});
$('button[name=addnotificationsetup]').off().on('click', function () {
    /* close the sidebar */
    sidebar.close();
    AddNotification("notificationsetuptable");
});
let notification = [];
async function updateNotification(updatenotification) {
    try {
        let notificationindex = notification.filter(x => x.NotificationID === updatenotification.NotificationID).map(x => x).length;
        //add notification
        if (notificationindex === 0) {
            notification.push(updatenotification);
        }
        if (notificationindex > 0) {
            if (updatenotification.Delete) {
                    //delete notification 
                Promise.all([delete_notification(updatenotification.NotificationID)]);
            }
            else {
                //update notification
                Promise.all([update_notification(updatenotification.NotificationID)]);
            }
        }
       
        if (/vehicle/i.test(updatenotification.Type)) {
            Promise.all([updateagvTable(updatenotification)]);
        }
        if (/routetrip/i.test(updatenotification.Type)) {
            Promise.all([updatetripTable(updatenotification)]);
        }
        if (/mpe/i.test(updatenotification.Type)) {
            Promise.all([updateMPETable(updatenotification)]);
        }
        if (/dockdoor/i.test(updatenotification.Type)) {
            Promise.all([updateDockDoorTables(updatenotification)]);
        }
    }
    catch (e) {
        console.log(e);
    }
}
async function updateagvTable(updatenotification) {
    try {
        let Vehiclecount = notification.filter(x => x.Type === "vehicle").map(x => x).length;
        //AGV Counts
        if (Vehiclecount > 0) {
            if (parseInt($('#agvnotificaion_number').text()) !== Vehiclecount) {
                $('#agvnotificaion_number').text(Vehiclecount);
            }
        }
        else {
            $('#agvnotificaion_number').text("");
        }
        let findagvtrdataid = agvnotificationtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']');
        if (findagvtrdataid.length > 0) {
            if (updatenotification.Delete) {
                agvnotificationtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']').remove();
            }
            else {
                agvnotificationtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']').replaceWith(agv_row_template.supplant(formatagvnotifirow(updatenotification)));
            }
        }
        else {
            agvnotificationtable_Body.append(agv_row_template.supplant(formatagvnotifirow(updatenotification)));
        }
    } catch (e) {
        console.log(e);
    }
    return null;
}
async function updatetripTable(updatenotification) {
    try {
        //let routetripcount = notification.filter(x => x.Type === "routetrip").map(x => x).length;
        let routetripcount = notification.filter(x => x.Type === "routetrip" || x.Type === "dockdoor").map(x => x).length;
        // routetrip Counts
        if (routetripcount > 0) {
            if (parseInt($('#tripsnotificaion_number').text()) !== routetripcount) {
                $('#tripsnotificaion_number').text(routetripcount);
            }
            $('#ctsnotificaion_number').text(routetripcount);
        }
        else {
            $('#tripsnotificaion_number').text("");
        }
        let findtrdataid = tripsnotificationtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']');
        if (findtrdataid.length > 0) {
            if (updatenotification.Delete) {
                tripsnotificationtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']').remove();
                tripsnotificationtable_Body.find('tr[data-id=collapse_' + updatenotification.NotificationID + ']').remove();
            }
            else {
                tripsnotificationtable_Body.find('tr[data-id=collapse_' + updatenotification.NotificationID + ']').remove();
                tripsnotificationtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']').replaceWith(trip_row_template.supplant(formattripnotifirow(updatenotification)));
            }
        }
        else {
            tripsnotificationtable_Body.append(trip_row_template.supplant(formattripnotifirow(updatenotification)));
        }
    } catch (e) {
        console.log(e);
    }
    return null;
}
async function updateMPETable(updatenotification) {
    try {
        let mpeCounter = notification.filter(x => x.Type === "mpe" && x.Delete != true).map(x => x).map(x => x).length;
        if (mpeCounter > 0) {
            if (parseInt($('#mpenotification_number').text()) !== mpeCounter) {
                $('#mpenotification_number').text(mpeCounter);
            }
        }
        else {
            $('#mpenotification_number').text("");
        }
        let findmpedataid = mpenotificationtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']');
        if (findmpedataid.length > 0) {
            if (updatenotification.hasOwnProperty("DELETE")) {
                mpenotificationtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']').remove();
            }
            else {
                mpenotificationtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']').replaceWith(mpe_row_template.supplant(formatmpenotifirow(updatenotification)));
            }
        }
        else {
            mpenotificationtable_Body.append(mpe_row_template.supplant(formatmpenotifirow(updatenotification)));
        }
        sortMPETable();
    }
    catch (e) {
        console.log(e);
    }
    return null;
}
async function updateDockDoorTables(updatenotification) {
    if (updatenotification.Name === "Load Scan After Departure") {
        updatedockdoorloadafterdeparttable(updatenotification);
    }
    if (updatenotification.Name === "Missing Closed Scan") {
        updatemissinclosedscantable(updatenotification);
    }
    if (updatenotification.Name === "Missing Assigned Scan") {
        updatemissingassignedscantable(updatenotification);
    }
    if (updatenotification.Name === "Missing Arrived Scan") {
        updatemissingarrivedscantable(updatenotification);
    }
    return null;
}
async function updatemissingassignedscantable(updatenotification) {
    try {
        let dockdoorandtripscount = notification.filter(x => x.Type === "routetrip" || x.Type === "dockdoor").map(x => x).length;
        if (dockdoorandtripscount > 0) {
            if (parseInt($('#tripsnotificaion_number').text()) !== dockdoorandtripscount) {
                $('#tripsnotificaion_number').text(dockdoorandtripscount);
            }
            $('#tripsnotificaion_number').text(dockdoorandtripscount);
        }
        else {
            $('#tripsnotificaion_number').text("");
        }
        let findmissingassigneddataid = dockdoormissingassignedtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']');
        if (findmissingassigneddataid.length > 0) {
            if (updatenotification.hasOwnProperty("DELETE")) {
                dockdoormissingassignedtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']').remove();
            }
        }
        else {
            dockdoormissingassignedtable_Body.append(dockdoormissingassignedtable_row_template.supplant(formatdockdoormissingassignedtablerow(updatenotification)))
        }
    }
    catch (e) {
        console.log(e);
    }
    return null;
}
async function gettipsanddoornotificationcounts() {
    try {
        let dockdoorandtripscount = notification.filter(x => x.Type === "routetrip" || x.Type === "dockdoor").map(x => x).length;
        if (dockdoorandtripscount > 0) {
            if (parseInt($('#tripsnotificaion_number').text()) !== dockdoorandtripscount) {
                $('#tripsnotificaion_number').text(dockdoorandtripscount);
            }
            $('#tripsnotificaion_number').text(dockdoorandtripscount);
        }
        else {
            $('#tripsnotificaion_number').text("");
        }
    }
    catch (e) {
        console.log(e);
    }
    return null;
}
async function updatemissingarrivedscantable(updatenotification) {
    try {
        gettipsanddoornotificationcounts();
        let findmissingarriveddataid = dockdoormissingarrivedtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']');
        if (findmissingarriveddataid.length > 0) {
            if (updatenotification.hasOwnProperty("DELETE")) {
                dockdoormissingarrivedtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']').remove();
            }
        }
        else {
            dockdoormissingarrivedtable_Body.append(dockdoormissingarrivedtable_row_template.supplant(formatdockdoormissingarrivedtablerow(updatenotification)));
        }
    }
    catch (e) {
        console.log(e);
    }
    return null;
}
async function updatemissinclosedscantable(updatenotification) {
    try {
        gettipsanddoornotificationcounts();
        let findddmissingcloseddataid = dockdoormissingclosedtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']');
        if (findddmissingcloseddataid.length > 0) {
            if (updatenotification.hasOwnProperty("DELETE")) {
                dockdoormissingclosedtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']').remove();
            }
        }
        else {
            dockdoormissingclosedtable_Body.append(dockdoormissingclosedtable_row_template.supplant(formatdockdoormissingclosedtablerow(updatenotification)));
        }
    }
    catch (e) {
        console.log(e);
    }
    return null;
}
async function updatedockdoorloadafterdeparttable(updatenotification) {
    try {
        gettipsanddoornotificationcounts();
        let findddladdataid = dockdoorloadafterdeparttable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']');
        if (findddladdataid.length > 0) {
            if (updatenotification.hasOwnProperty("DELETE")) {
                dockdoorloadafterdeparttable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']').remove();
            }
            //else {
            //    dockdoorloadafterdeparttable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']').replaceWith(dockdoorloadafterdeparttable_row_template.supplant(formatdockdoorloadafterdeparttablerow(updatenotification)));
            //}
        }
        else {
            dockdoorloadafterdeparttable_Body.append(dockdoorloadafterdeparttable_row_template.supplant(formatdockdoorloadafterdeparttablerow(updatenotification)));
        }
    }
    catch (e) {
        console.log(e);
    }
    return null;
}
async function sortMPETable() {
    var table, rows, switching, i, x, y, shouldSwitch;
    table = document.getElementById("mpenotificationtable");
    switching = true;
    while (switching) {
        switching = false;
        rows = table.rows;
        for (i = 1; i < (rows.length - 1); i++) {
            shouldSwitch = false;

            var id = $(rows[i]).attr("data-id");
            var notificationrw = notification.filter(y => y.Type === "mpe" && y.id == id);

            if (notificationrw.length < 1 || notificationrw[0].hasOwnProperty("DELETE")) {
                mpenotificationtable_Body.find('tr[data-id=' + id + ']').remove();
            }
            else {
                x = rows[i].getElementsByTagName("TD")[3];
                y = rows[i + 1].getElementsByTagName("TD")[3];
                if (Number(x.innerHTML) < Number(y.innerHTML)) {
                    shouldSwitch = true;
                    break;
                }
            }
        }
        if (shouldSwitch) {
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;
        }
    }
}
let mpenotificationtable = $('table[id=mpenotificationtable]');
let mpenotificationtable_Body = mpenotificationtable.find('tbody');
async function delete_notification(id)
{
    try {
        notification.forEach(function (obj) {
            if (obj.NotificationID === id) {
                notification.splice(notification.indexOf(obj), 1);
            }
        });
        return null;
    } catch (e) {
        console.log(e);
    }
  
}
async function update_notification(id) {
    try {
        notification.forEach(function (obj) {
            if (obj.NotificationID === id) {
                let indexobj = notification.indexOf(obj);
            }
        });
        return null;
    } catch (e) {
        console.log(e);
    }

}
let agvnotificationtable = $('table[id=agvnotificationtable]');
let agvnotificationtable_Body = agvnotificationtable.find('tbody');

function LoadNotification(value) {
   
    try {
     
        $.connection.FOTFManager.server.getNotification(value).done(function (Data) {
            $.each(Data, function () {
                updateNotification(this);
            });
        });
    } catch (e) {
        console.log(e);
    }
}
function LoadNotificationsetup(Data, table) {
    $.connection.FOTFManager.server.getNotification_ConditionsList().done(function (NotificationData) {
        notificationTable_Body.empty();
        $.each(NotificationData, function () {
            notificationTable_Body.append(notificationTable_row_template.supplant(formatnotificationrow(this)));
        });
        $('button[name=notificationedit]').on('click', function () {
            var td = $(this);
            var tr = $(td).closest('tr'),
                id = tr.attr('data-id');
            EditNotification(id, table);
        });
        $('button[name=notificationdelete]').on('click', function () {
            var td = $(this);
            var tr = $(td).closest('tr'),
                id = tr.attr('data-id');
            RemoveNotification(id, table);
        });
    });
}
let agv_row_template =
    '<tr data-id={id} style=background-color:{conditioncolor} data-toggle=collapse data-target=#{action_text} class=accordion-toggle>' +
    '<td><span class="ml-p25rem">{name}</span></td>' +
    '<td><button class="btn btn-outline-info btn-sm btn-block tagdetails" data-tag="{tagid}" >{type}</button></td>' +
    '<td>{duration}</td>' +
    '</tr>'
    ;
function formatagvnotifirow(properties, indx) {
    return $.extend(properties, {
        id: properties.NotificationID,
        tagid: properties.TypeID,
        name: properties.Name,
        type: properties.TypeName,
        condition: properties.Conditions,
        duration: calculatevehicleDuration(properties.TypeTime),
        conditioncolor: conditioncolor(properties.TIME, parseInt(properties.Warning), parseInt(properties.Critical)),
        warning_action_text: properties.WarningAction,
        critical_action_text: properties.CriticalAction,
        action_text: conditionaction_text(properties.vehicleTime, parseInt(properties.Warning), parseInt(properties.Critical)) + "_" + properties.notificationId,
        indexobj: indx
    });
}
let trip_row_template =
    '<tr data-id={id} class="accordion-toggle collapsed" id={id} data-toggle=collapse data-parent=#{id} href="#collapse_{id}">' +
  
    '<td><span class="ml-p25rem">{schd}</span></td>' +
    '<td>{duration}</td>' +
    '<td style="text-align:center"">{direction}</td>' +
    '<td>' +
    '<button class="btn btn-outline-info btn-sm btn-block px-1 routetripdetails" data-routetrip="{routetripid}" style="font-size:12px;">{routedispaly}</button>' +
    '</td>' +
    '<td data-toggle="tooltip" title="{dest}" style="overflow: inherit;">{dest}</td> ' +
    '<td class="expand-button">' +
    '<a class="btn btn-link d-flex justify-content-end" data-toggle="collapse" href="#collapse_{id}" role="button" aria-expanded="false" aria-controls="collapseSection">' +
    '<div class="iconXSmall">' +
    ' <i class="pi-iconCaretDownFill" />' +
    '</div>' +
    '</a>' +
    '</td>' +
    '</tr>' +
    '<tr data-id=collapse_{id} class="hide-table-padding">' +
        '<td colspan="6">'+
            '<div class="collapse" id="collapse_{id}">' +
            '<div class="mt-1">' +
            '<ol class="pl-4 mb-0">' +
            '<p class="pb-1 text-wrap">{warning_action_text}</p> ' +
            '</ol>' +
            '</div>' +
            '</div>' +
        '</td>' +
    '</tr>'
    ;
let tripsnotificationtable = $('table[id=tripsnotificationtable]');
let tripsnotificationtable_Body = tripsnotificationtable.find('tbody');
function formattripnotifirow(properties, indx) {
    return $.extend(properties, {
        id: properties.NotificationID,
        tagid: properties.TypeID,
        schd: moment(properties.TypeTime).format("HH:mm"),
        routetripid: properties.TypeID,
        routedispaly: spitName(properties.TypeName, 0),
        trip: properties.trip,
        direction: properties.TypeID.substr(properties.TypeID.length - 1),
        leg: properties.legSiteId,
        dest: spitName(properties.TypeName, 1),
        condition: properties.Conditions,
        duration: calculatevehicleDuration(properties.TypeTime),
        conditioncolor: conditioncolor(properties.VEHICLETIME, parseInt(properties.Warning), parseInt(properties.Critical)),
        warning_action_text: properties.WarningAction,
        critical_action_text: properties.CriticalAction,
        indexobj: indx
    });
}
let mpe_row_template =
    '<tr data-id={id} style=background-color:{conditioncolor} data-toggle=collapse data-target=#{action_text} class=accordion-toggle>' +
    '<td><span class="ml-p25rem">{name}</span></td>' +
    '<td><button class="btn btn-outline-info btn-sm btn-block machinedetails" data-machine="{zoneid}" >{mpeName}</button></td>' +
    '<td style="text-align:center">{duration}</td>' +
    '<td style="display:none;">{durationtime}</td>' +
    '</tr>'
    ;
function formatmpenotifirow(properties, indx) {
    return $.extend(properties, {
        id: properties.NotificationID,
        mpeName: properties.TypeName,//properties.mpe_type + "-" + properties.mpe_number.padStart(3, "0"),
        zoneid: properties.TypeID,
        name: properties.Name,
        //type: properties.Type,
        //condition: properties.Conditions,
        duration: ConverMPENotificationTime(properties.TypeDuration),
        durationtime: properties.TypeDuration,
        conditioncolor: conditioncolor(GetMPENotificationTime(properties.TypeDuration), parseInt(properties.Warning), parseInt(properties.Critical)),
        warning_action_text: properties.WarningAction,
        critical_action_text: properties.CriticalAction,
        action_text: conditionaction_text(GetMPENotificationTime(properties.TypeDuration), parseInt(properties.Warning), parseInt(properties.Critical)) + "_" + properties.notificationId,
        indexobj: indx
    });
}
let dockdoormissingassignedtable = $('table[id=missingassignedtable]');
let dockdoormissingassignedtable_Body = dockdoormissingassignedtable.find('tbody');
let dockdoormissingassignedtable_row_template = '<tr data-id={id} style=background-color:{conditioncolor} class="accordion-toggle collapsed" id={id} data-toggle=collapse data-parent=#{id} href="#collapse_{id}">' +
    '<td colspan="7" data-input="placard" class="text-left">' +
    '<a data-doorid={zone_id} data-placardid={placard} class="containerdetails">{placard}</a>' +
    '</td>' +
    '<td class="expand-button">' +
    '<a class="btn btn-link d-flex justify-content-end" data-toggle="collapse" href="#collapse_{id}" role="button" aria-expanded="false" aria-controls="collapseSection">' +
    '<div class="iconXSmall">' +
    ' <i class="pi-iconCaretDownFill" />' +
    '</div>' +
    '</a>' +
    '</td>' +
    '</tr>' +
    '<tr data-id=collapse_{id} class="hide-table-padding">' +
    '<td colspan="8">' +
    '<div class="collapse" id="collapse_{id}">' +
    '<div class="mt-1">' +
    '<ol class="pl-4 mb-0">' +
    '<p class="pb-1">{action_text}</p> ' +
    '</ol>' +
    '</div>' +
    '</div>' +
    '</td>' +
    '</tr>';
function formatdockdoormissingassignedtablerow(properties, indx) {
    return $.extend(properties, {
        id: properties.NotificationID,
        placard: properties.TypeID,
        action_text: getLADactiontext(properties),
        conditioncolor: getLADconditioncolor(properties),
        indexobj: indx,
        zone_id: "0"
    });
}

let dockdoormissingarrivedtable = $('table[id=missingarrivaltable]');
let dockdoormissingarrivedtable_Body = dockdoormissingarrivedtable.find('tbody');
let dockdoormissingarrivedtable_row_template = '<tr data-id={id} style=background-color:{conditioncolor} class="accordion-toggle collapsed" id={id} data-toggle=collapse data-parent=#{id} href="#collapse_{id}">' +
    '<td colspan="2">{docknumber}</td>' +
    '<td colspan="6">{trailer}</td>' +
    //'<td colspan="7" data-input="trailer" class="text-left">' +
    //'<a data-doorid={zone_id} data-trailerid={trailer} class="containerdetails">{trailer}</a>' +
    '</td>' +
    '<td class="expand-button">' +
    '<a class="btn btn-link d-flex justify-content-end" data-toggle="collapse" href="#collapse_{id}" role="button" aria-expanded="false" aria-controls="collapseSection">' +
    '<div class="iconXSmall">' +
    ' <i class="pi-iconCaretDownFill" />' +
    '</div>' +
    '</a>' +
    '</td>' +
    '</tr>' +
    '<tr data-id=collapse_{id} class="hide-table-padding">' +
    '<td colspan="8">' +
    '<div class="collapse" id="collapse_{id}">' +
    '<div class="mt-1">' +
    '<ol class="pl-4 mb-0">' +
    '<p class="pb-1">{action_text}</p> ' +
    '</ol>' +
    '</div>' +
    '</div>' +
    '</td>' +
    '</tr>';
function formatdockdoormissingarrivedtablerow(properties, indx) {
    return $.extend(properties, {
        id: properties.NotificationID,
        trailer: parsemissingarrivedscanname(properties.TypeName, "trailer"),
        docknumber: parsemissingarrivedscanname(properties.TypeName, "docknumber"),
        zoneid: parsemissingarrivedscanname(properties.TypeName, "dockid"),
        action_text: getLADactiontext(properties),
        conditioncolor: getLADconditioncolor(properties),
        indexobj: indx,
        zone_id: "0"
    });
}

let dockdoormissingclosedtable = $('table[id=missingclosedtable]');
let dockdoormissingclosedtable_Body = dockdoormissingclosedtable.find('tbody');
let dockdoormissingclosedtable_row_template = '<tr data-id={id} style=background-color:{conditioncolor} class="accordion-toggle collapsed" id={id} data-toggle=collapse data-parent=#{id} href="#collapse_{id}">' +
    '<td colspan="7" data-input="placard" class="text-left">' +
    '<a data-doorid={zone_id} data-placardid={placard} class="containerdetails">{placard}</a>' +
    '</td>' +
    '<td class="expand-button">' +
    '<a class="btn btn-link d-flex justify-content-end" data-toggle="collapse" href="#collapse_{id}" role="button" aria-expanded="false" aria-controls="collapseSection">' +
    '<div class="iconXSmall">' +
    ' <i class="pi-iconCaretDownFill" />' +
    '</div>' +
    '</a>' +
    '</td>' +
    '</tr>' +
    '<tr data-id=collapse_{id} class="hide-table-padding">' +
    '<td colspan="8">' +
    '<div class="collapse" id="collapse_{id}">' +
    '<div class="mt-1">' +
    '<ol class="pl-4 mb-0">' +
    '<p class="pb-1 text-wrap">{action_text}</p> ' +
    '</ol>' +
    '</div>' +
    '</div>' +
    '</td>' +
    '</tr>';
function formatdockdoormissingclosedtablerow(properties, indx) {
    return $.extend(properties, {
        id: properties.NotificationID,
        placard: properties.TypeID,
        action_text: getLADactiontext(properties),
        conditioncolor: getLADconditioncolor(properties),
        indexobj: indx,
        zone_id: "0"
    });
}
let dockdoorloadafterdeparttable = $('table[id=loadafterdeparttable]');
let dockdoorloadafterdeparttable_Body = dockdoorloadafterdeparttable.find('tbody');
let dockdoorloadafterdeparttable_row_template = '<tr data-id={id} style=background-color:{conditioncolor} class="accordion-toggle collapsed" id={id} data-toggle=collapse data-parent=#{id} href="#collapse_{id}">' +
    //'<td><button class="btn btn-outline-info btn-sm btn-block px-1 containerdetails" data-placardid={placard}" style="font-size:12px;" >{placard}</button ></td>' +
    '<td colspan="2" data-input="placard" class="text-center">' +
    '<a data-doorid={zone_id} data-placardid={placard} class="containerdetails">{placard}</a>' +
    //'<a data-doorid={zone_id} data-placardid={placard} class="containerdetails" title={placard}>{placard}</a>' +
    '</td>' +
    '<td><a title={loadscan}>{loadscan}</a></td>' +
    '<td colspan="2"><a title={trailer}>{trailer}</a></td>' +
    '<td><a title={departscan}>{departscan}</a></td>' +
    '<td class="expand-button">' +
    '<a class="btn btn-link d-flex justify-content-end" data-toggle="collapse" href="#collapse_{id}" role="button" aria-expanded="false" aria-controls="collapseSection">' +
    '<div class="iconXSmall">' +
    ' <i class="pi-iconCaretDownFill" />' +
    '</div>' +
    '</a>' +
    '</td>' +
    '</tr>' +
    '<tr data-id=collapse_{id} class="hide-table-padding">' +
    '<td colspan="7">' +
    '<div class="collapse" id="collapse_{id}">' +
    '<div class="mt-1">' +
    '<ol class="pl-4 mb-0">' +
    '<p class="pb-1">Placard: {placard}</br>Load Scan: {loadscan}</br>Trailer: {trailer}</br>Depart Scan: {departscan}</p> ' +
    '<p class="pb-1 text-wrap">{action_text}</p> ' +
    '</ol>' +
    '</div>' +
    '</div>' +
    '</td>' +
    '</tr>';
function formatdockdoorloadafterdeparttablerow(properties, indx) {
    return $.extend(properties, {
        id: properties.NotificationID,
        placard: properties.TypeID,
        loadscan: parseloadafterdeparttypename(properties.TypeName, 'loadscan'),
        trailer: parseloadafterdeparttypename(properties.TypeName, 'trailer'),
        departscan: parseloadafterdeparttypename(properties.TypeName, 'departscan'),
        action_text: getLADactiontext(properties),
        conditioncolor: getLADconditioncolor(properties),
        indexobj: indx,
        zone_id: "0"
    });
}
function getLADactiontext(properties) {
    if (properties.TypeStatus == 'Warning') { return properties.WarningAction; }
    if (properties.TypeStatus == 'Critical') { return properties.CriticalAction; }
    return "";
}
function getLADconditioncolor(properties) {
    if (properties.status == 'Warning') { return "#ffff0080" }
    if (properties.status == 'Critical') { return "#bd213052"; }
    return "white";
}
function parseloadafterdeparttypename(typename, name) {
    var typenames = typename.split('|', 4);
    if (name == 'loadscan') {
        return typenames[2];
    }
    if (name == 'trailer') {
        return typenames[1];
    }
    if (name == 'departscan') {
        return typenames[3]
    }
}
function parsemissingarrivedscanname(typename, name) {
    var typenames = typename.split('|', 4);
    if (name == 'trailer') {
        return typenames[0];
    }
    if (name == 'dockid') {
        return typenames[1];
    }
    if (name == 'docknumber') {
        return typenames[2]
    }
}
let notificationTable = $('table[id=notificationsetuptable]');
let notificationTable_Body = notificationTable.find('tbody');
let notificationTable_row_template = '<tr data-id="{id}" class="{button_collor}">' +
    '<td><span class="ml-p5rem">{name}</span></td>' +
    '<td>{warning}</td>' +
    '<td>{critical}</td>' +
    '<td>{status}</td>' +
    '<td>' +
    '<button class="btn btn-light btn-sm mx-1 pi-iconEdit" name="notificationedit"></button>' +
    '<button class="btn btn-light btn-sm mx-1 pi-trashFill" name="notificationdelete"></button>' +
    '</td>' +
    '</tr>';

function formatnotificationrow(properties) {
    return $.extend(properties, {
        id: properties.Id,
        name: properties.Name,
        type: properties.Type,
        condition: properties.Conditions,
        warning: properties.Warning,
        warning_action: properties.WarningAction,
        warning_color: properties.WarningColor,
        critical: properties.Critical,
        critical_action: properties.CriticalAction,
        critical_color: properties.CriticalColor,
        status: GetnotificationStatus(properties),
        button_collor: Get_notificationColor(properties)
    })
}
async function AddNotification(table) {
    $('.warning_conditionpickvalue').html(0);
    $('input[id=warning_condition]').val(0);
    $('.critical_conditionpickvalue').html(0);
    $('input[id=critical_condition]').val(0);
    $('#notification_SetupHeader').text('Add New Notification');
    sidebar.close('notificationsetup');
    $('#notificationsubmitBtn').css("display", "block");
    $('#editnotificationsubmitBtn').css("display", "none");
    $('#Notification_Setup_Modal').modal();
    $('button[id=notificationsubmitBtn]').prop('disabled', false);
    $('button[id=notificationsubmitBtn]').off().on('click', function () {
        $('button[id=notificationsubmitBtn]').prop('disabled', true);
        var jsonObject = {
            CreatedByUsername: User.UserId,
            ActiveCondition: $('input[type=checkbox][name=condition_active]')[0].checked,
            Name: $('input[type=text][name=condition_name]').val(),
            Type: $('select[name=condition_type] option:selected').val(),
            Conditions: $('input[type=text][name=condition]').val(),
            Warning: $('input[id=warning_condition]').val(),
            Critical: $('input[id=critical_condition]').val(),
            WarningAction: $('textarea[id=warning_action]').val(),
            CriticalAction: $('textarea[id=critical_action]').val()
        };
        $.connection.FOTFManager.server.addNotification_Conditions(JSON.stringify(jsonObject)).done(function (Data) {
            if (Data.length === 0) {
                $('span[id=error_notificationsubmitBtn]').text("Error Adding Condition");
                setTimeout(function () { $("#Notification_Setup_Modal").modal('hide'); }, 1000);
            }
            else {
                $('span[id=error_notificationsubmitBtn]').text("Condition has been Added");
                LoadNotificationsetup(Data, table);
                setTimeout(function () {$("#Notification_Setup_Modal").modal('hide'); sidebar.open('notificationsetup');}, 1000);
            }
        });
    });
}
async function EditNotification(Id, table) {

    $('#notificationsubmitBtn').css("display", "none");
    $('#editnotificationsubmitBtn').css("display", "block");
    $('button[id=editnotificationsubmitBtn]').prop('disabled', false);
    $('#notification_SetupHeader').text('Edit Notification');
    sidebar.close('notificationsetup');
    try {
        $.connection.FOTFManager.server.getNotification_Conditions(Id).done(function (N_Data) {
            if (N_Data.length > 0) {
                var NotificationData = N_Data[0];
                if (!$.isEmptyObject(NotificationData)) {
                
                    if (NotificationData.ActiveCondition) {
                        if (!$('input[type=checkbox][name=condition_active]')[0].checked) {
                            $('input[type=checkbox][name=condition_active]').prop('checked', true).change();
                        }
                    }
                    else {
                        if ($('input[type=checkbox][name=condition_active]')[0].checked) {
                            $('input[type=checkbox][name=condition_active]').prop('checked', false).change();
                        }
                    }
                    $('input[type=text][name=condition_name]').val(NotificationData.Name);
                    $('select[name=condition_type]').val(NotificationData.Type)
                    $('input[type=text][name=condition]').val(NotificationData.Conditions);
                    $('.warning_conditionpickvalue').html(NotificationData.Warning);
                    $('input[id=warning_condition]').val(NotificationData.Warning);
                    $('.critical_conditionpickvalue').html(NotificationData.Critical);
                    $('input[id=critical_condition]').val(NotificationData.Critical);
                    $('textarea[id=warning_action]').val(NotificationData.WarningAction);
                    $('textarea[id=critical_action]').val(NotificationData.CriticalAction);
                }

                $('button[id=editnotificationsubmitBtn]').off().on('click', function () {
                    $('button[id=editnotificationsubmitBtn]').prop('disabled', true);
                    var jsonObject = {
                        Id: Id,
                        LastupdateByUsername: User.UserId,
                        ActiveCondition: $('input[type=checkbox][name=condition_active]')[0].checked,
                        Name: $('input[type=text][name=condition_name]').val(),
                        Type: $('select[name=condition_type] option:selected').val(),
                        Conditions: $('input[type=text][name=condition]').val(),
                        Warning: $('input[id=warning_condition]').val(),
                        Critical: $('input[id=critical_condition]').val(),
                        WarningAction: $('textarea[id=warning_action]').val(),
                        CriticalAction: $('textarea[id=critical_action]').val()
                    };
                    $.connection.FOTFManager.server.editNotification_Conditions(JSON.stringify(jsonObject)).done(function (Data) {
                        if (Data.length === 0) {
                            $('span[id=error_notificationsubmitBtn]').text("Unable to loaded Condition");
                            setTimeout(function () { $("#Notification_Setup_Modal").modal('hide'); }, 3000);
                        }
                        else {
                            $('span[id=error_notificationsubmitBtn]').text("Condition has been Edited");
                            LoadNotificationsetup(Data, table);
                            setTimeout(function () { $("#Notification_Setup_Modal").modal('hide'); sidebar.open('notificationsetup'); }, 1000);
                        }
                    });
                })
                $('#Notification_Setup_Modal').modal();
            }
            else {
               //Add error reason     
            }
        });
    } catch (e) {
        console.log(e);
    }
}
async function RemoveNotification(id, table) {
    //RemoveNotificationModal
    sidebar.close('notificationsetup');
    $('#removeNotificationHeader').text('Remove Notification');
    $('button[id=removeNotification]').off().on('click', function () {
        var jsonObject = { Id: id };
        $.connection.FOTFManager.server.deleteNotification_Conditions(JSON.stringify(jsonObject)).done(function (Data) {
            $('span[id=error_notificationsubmitBtn]').text("Condition has been Edited");
            if (Data.length === 0) {
                setTimeout(function () { $("#RemoveNotificationModal").modal('hide'); }, 3000);
            }
            else {
                LoadNotificationsetup(Data, table);
                setTimeout(function () { $("#RemoveNotificationModal").modal('hide'); sidebar.open('notificationsetup'); }, 1000);
         
            }
        })
    });
    $('#RemoveNotificationModal').modal();

}
function conditioncolor(time, war_min, crit_min) {
    if (checkValue(time)) {
        var conditiontime = moment(time);  // 5am PDT
        var curenttime = moment();
        if (conditiontime._isValid) {
            var d = moment.duration(curenttime.diff(conditiontime));
            var minutes = Math.floor(d._milliseconds / 60000);

            if (minutes < war_min && minutes < crit_min) {
                return "white";
            }
            else if (minutes >= war_min && minutes < crit_min) {
                return "#ffff0080";
            }
            else if (minutes >= crit_min) {
                return "#bd213052";
            }
        }
        else {
            return "";
        }
    }
}
function conditionaction_text(time, war_min, crit_min) {
    if (checkValue(time)) {
        var conditiontime = moment(time);  // 5am PDT
        var curenttime = moment();
        if (conditiontime._isValid) {
            var d = moment.duration(curenttime.diff(conditiontime));
            var minutes = Math.floor(d._milliseconds / 60000);
            if (minutes < war_min && minutes < crit_min) {
                return "W_";
            }
            if (minutes >= war_min && minutes < crit_min) {
                return "W_";
            }
            else if (minutes >= crit_min) {
                return "C_";
            }
        }
        else {
            return "NONE";
        }
    }
}
function calculateDuration(t) {
    if (checkValue(t)) {
        var conditiontime = moment().tz(timezone.Facility_TimeZone).set({ 'year': t.year, 'month': t.month, 'date': t.dayOfMonth, 'hour': t.hourOfDay, 'minute': t.minute, 'second': t.second });//moment(time);  // 5am PDT
        var curenttime = moment();
        if (conditiontime._isValid) {
            var d = moment.duration(curenttime.diff(conditiontime));
            return moment.duration(d._milliseconds, "milliseconds").format("d [days], h [hrs], m [min]", {
                useSignificantDigits: true,
                trunc: true,
                precision: 3
            });
        }
        else {
            return "";
        }
    }
}
function calculatevehicleDuration(t) {
    if (checkValue(t)) {
        var conditiontime = moment(t);//moment(time);  // 5am PDT
        var curenttime = moment().tz(timezone.Facility_TimeZone);
        if (conditiontime._isValid) {
            var d = moment.duration(curenttime.diff(conditiontime));
            return moment.duration(d._milliseconds, "milliseconds").format("d [days], h [hrs], m [min]", {
                useSignificantDigits: true,
                trunc: true,
                precision: 3
            });
        }
        else {
            return "";
        }
    }
}
function enableNotificationSubmit() {
    if ($('input[type=text][name=condition_name]').hasClass('is-valid') &&
        $('input[type=text][name=condition]').hasClass('is-valid') &&
        $('select[name=condition_type]').hasClass('is-valid')
    ) {
        $('button[id=notificationsubmitBtn]').prop('disabled', false);
    }
    else {
        $('button[id=notificationsubmitBtn]').prop('disabled', true);
    }
}
function GetnotificationStatus(data) {
    if (data.ActiveCondition) {
        return "Active";
    }
    else {
        return "Disabled";
    }
}
function Get_notificationColor(data) {
    if (data.ActiveCondition) {
        return "table-success";
    }
    else {
        return "table-warning";
    }
}
function spitName(name, index) {
    try {
        var tempName = name.split("|");
        return tempName[index];
    } catch (e) {
        console.log(e);
    }
}
function ConverMPENotificationTime(secs) {
    try {
        if (secs != 0) {
            var sec_num = parseInt(secs, 10)
            var hours = Math.floor(sec_num / 3600)
            var minutes = Math.floor(sec_num / 60) % 60
            var seconds = sec_num % 60

            return [hours, minutes, seconds]
                .map(v => v < 10 ? "0" + v : v)
                .filter((v, i) => v !== "00" || i > 0)
                .join(":")
        }
        return "";
    } catch (e) {
        console.log(e);
    }
}
function GetMPENotificationTime(secs) {
    if (secs != 0) {
        return moment().subtract(secs, 'seconds');
    }
    
    return moment().add(1, 'minutes');
}

$.extend(fotfmanager.client, {
    updateQSMStatus: async (Connectionupdate) => { updateConnection(Connectionupdate) }
});
//on close clear all inputs
$('#API_Connection_Modal').on('hidden.bs.modal', function () {
    $(this)
        .find("input[type=text],textarea,select")
        .css({ "border-color": "#D3D3D3" })
        .val('')
        .end()
        .find("span[class=text]")
        .css("border-color", "#FF0000")
        .val('')
        .text("")
        .end()
        .find('input[type=checkbox]')
        .prop('checked', false).change();
    sidebar.open('connections');
});
//on open set rules
$('#API_Connection_Modal').on('shown.bs.modal', function () {
    sidebar.close('connections');
    $('span[id=error_apisubmitBtn]').text("");
    $('button[id=apisubmitBtn]').prop('disabled', true);

    $('.hoursforwardvalue').html($('input[id=hoursforward_range]').val());
    $('input[id=hoursforward_range]').on('input change', () => {
        $('.hoursforwardvalue').html($('input[id=hoursforward_range]').val());
    });
    $('.hoursbackvalue').html($('input[id=hoursback_range]').val());
    $('input[id=hoursback_range]').on('input change', () => {
        $('.hoursbackvalue').html($('input[id=hoursback_range]').val());
    });
    //Connection name Validation
    if (!checkValue($('input[type=text][name=connection_name]').val())) {
        $('input[type=text][name=connection_name]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_connection_name]').text("Please Enter Connection Name");
    }
    else {
        $('input[type=text][name=connection_name]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_connection_name]').text("");
    }
    //ip address Keyup
    $('input[type=text][name=ip_address]').keyup(function () {
        if (IPAddress_validator($('input[type=text][name=ip_address]').val()) ===
            "Invalid IP Address") {
            $('input[type=text][name=ip_address]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=ip_address]').text("Please Enter Connection Name");
        }
        else {
            $('input[type=text][name=ip_address]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_ip_address]').text("");
        }
        if ($('input[type=checkbox][name=udp_connection]').is(':checked')) {

            enableudpSubmit();
        }
        else if ($('input[type=checkbox][name=tcpip_connection]').is(':checked')) {

            enabletcpipSubmit();
        }
        else if ($('input[type=checkbox][name=ws_connection]').is(':checked')) {
            enablewsSubmit();
        }
        else {
            enableConnectionSubmit();
        }
    });
    //Connection name Keyup
    $('input[type=text][name=connection_name]').keyup(function () {
        if (!checkValue($('input[type=text][name=connection_name]').val())) {
            $('input[type=text][name=connection_name]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_connection_name]').text("Please Enter Connection Name");
        }
        else {
            $('input[type=text][name=connection_name]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_connection_name]').text("");
        }
        if ($('input[type=checkbox][name=udp_connection]').is(':checked')) {

            enableudpSubmit();
        } else if ($('input[type=checkbox][name=tcpip_connection]').is(':checked')) {

            enabletcpipSubmit();
        }
        else if ($('input[type=checkbox][name=ws_connection]').is(':checked')) {
            enablewsSubmit();
        }
        else {
            enableConnectionSubmit();
        }
    });

    //message Type Validation
    if (!checkValue($('input[type=text][name=message_type]').val())) {
        $('input[type=text][name=message_type]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_message_type]').text("Please Enter Message Type");
    }
    else {
        $('input[type=text][name=message_type]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_message_type]').text("");
    }
    //message Type Keyup
    $('input[type=text][name=message_type]').keyup(function () {
        if (!checkValue($('input[type=text][name=message_type]').val())) {
            $('input[type=text][name=message_type]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_message_type]').text("Please Enter Message Type");
        }
        else {
            $('input[type=text][name=message_type]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_message_type]').text("");
        }
        if ($('input[type=checkbox][name=udp_connection]').is(':checked')) {
            enableudpSubmit();
        } else if ($('input[type=checkbox][name=tcpip_connection]').is(':checked')) {

            enabletcpipSubmit();
        }
        else if ($('input[type=checkbox][name=ws_connection]').is(':checked')) {

            enablewsSubmit();
        }
        else {
            enableConnectionSubmit();
        }
    });
    $('input[type=text][name=port_number]').keyup(function () {
        if (!checkValue($('input[type=text][name=port_number]').val())) {
            $('input[type=text][name=port_number]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=port_number]').text("Please Enter Message Type");
        }
        else {
            $('input[type=text][name=port_number]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=port_number]').text("");
        }
        if ($('input[type=checkbox][name=udp_connection]').is(':checked')) {
            enableudpSubmit();
        } else if ($('input[type=checkbox][name=tcpip_connection]').is(':checked')) {

            enabletcpipSubmit();
        }
        else if ($('input[type=checkbox][name=ws_connection]').is(':checked')) {

            enablewsSubmit();
        }
        else {
            enableConnectionSubmit();
        }
    });
    //Data Retrieve Occurrences Validation
    if ($('input[type=checkbox][name=ws_connection]').is(':checked')) {

    }
    if (!checkValue($('select[name=data_retrieve] option:selected').val())) {
        $('select[name=data_retrieve]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_data_retrieve]').text("Select Data Retrieve Occurrences");
    }
    else {
        $('select[name=data_retrieve]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_data_retrieve]').text("");
    }
    //Data Retrieve Occurrences Keyup
    $('select[name=data_retrieve]').change(function () {
        if ($('input[type=checkbox][name=ws_connection]').is(':checked')) {

        }
        if (!checkValue($('select[name=data_retrieve] option:selected').val())) {
            $('select[name=data_retrieve]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_data_retrieve]').text("Select Data Retrieve Occurrences");
        }
        else {
            $('select[name=data_retrieve]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_data_retrieve]').text("");
        }
        if ($('input[type=checkbox][name=udp_connection]').is(':checked')) {
            enableudpSubmit();
        } else if ($('input[type=checkbox][name=tcpip_connection]').is(':checked')) {

            enabletcpipSubmit();
        }
        else if ($('input[type=checkbox][name=ws_connection]').is(':checked')) {
            enablewsSubmit();
        }
        else {
            enableConnectionSubmit();
        }
    });
    // Address Validation
    if (!checkValue($('input[type=text][name=ip_address]').val())) {
        $('input[type=text][name=ip_address]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_ip_address]').text("Please Enter Valid IP address");
    }
    else {
        $('input[type=text][name=ip_address]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_ip_address]').text("");
    }
    //IP Address Keyup
    $('input[type=text][name=ip_address]').keyup(function () {
        if (IPAddress_validator($('input[type=text][name=ip_address]').val()) === 'Invalid IP Address') {
            $('input[type=text][name=ip_address]').css("border-color", "#FF0000");
            $('span[id=error_ip_address]').text("Please Enter Valid IP Address!");
        }
        else {
            $('input[type=text][name=ip_address]').css({ "border-color": "#2eb82e" }).removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_ip_address]').text("");
        }
        if ($('input[type=checkbox][name=udp_connection]').is(":checked")) {
            enableudpSubmit();
        } else if ($('input[type=checkbox][name=tcpip_connection]').is(':checked')) {

            enabletcpipSubmit();
        }
        else if ($('input[type=checkbox][name=ws_connection]').is(":checked")) {
            enablewsSubmit();
        }
        else {
            enableConnectionSubmit();
        }
    });
    //port Validation
    if (!checkValue($('input[type=text][name=port_number]').val())) {
        $('input[type=text][name=port_number]').css({ "border-color": "#FF0000" }).removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_port_number]').text("Please Enter Port Number");
    }
    else {
        $('input[type=text][name=port_number]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_port_number]').text("");
    }
    //Port Keyup
    $('input[type=text][name=port_number]').keyup(function () {
        if ($.isNumeric($('input[type=text][name=port_number]').val())) {
            if ($('input[type=text][name=port_number]').val().length > 65535) {
                $('input[type=text][name=port_number]').css({ "border-color": "#FF0000" }).removeClass('is-valid').addClass('is-invalid');
                $('span[id=error_port_number]').text("Please Enter Port Number!");
            }
            else if ($('input[type=text][name=port_number]').val().length === 0) {
                $('input[type=text][name=port_number]').css({ "border-color": "#FF0000" }).addClass('is-valid').removeClass('is-invalid');
                $('span[id=error_port_number]').text("Please Enter Port Number!");
            }
            else {
                $('input[type=text][name=port_number]').css({ "border-color": "#2eb82e" }).removeClass('is-invalid').addClass('is-valid');
                $('span[id=error_port_number]').text("");
            }
        }
        else {
            $('input[type=text][name=port_number]').css({ "border-color": "#FF0000" }).removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_port_number]').text("Please Enter Port Number!");
        }
        if ($('input[type=checkbox][name=udp_connection]').is(':checked')) {
            enableudpSubmit();
        } else if ($('input[type=checkbox][name=tcpip_connection]').is(':checked')) {

            enabletcpipSubmit();
        }
        else if ($('input[type=checkbox][name=ws_connection]').is(':checked')) {
            enablewsSubmit();
        }
        else {
            enableConnectionSubmit();
        }
    });
    //Vendor URL
    if (!checkValue($('input[type=text][name=url]').val())) {
        $('input[type=text][name=url]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_url]').text("Please Enter API URL");
    } else {
        $('input[type=text][name=url]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_url]').text("");
    }
    //URL Keyup
    $('input[type=text][name=url]').keyup(function () {
        if (!checkValue($('input[type=text][name=url]').val())) {
            $('input[type=text][name=url]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_url]').text("Please Enter API URL");
        }
        else {
            $('input[type=text][name=url]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_url]').text("");
        }
        if ($('input[type=checkbox][name=udp_connection]').is(':checked')) {
            enableudpSubmit();
        } else if ($('input[type=checkbox][name=tcpip_connection]').is(':checked')) {

            enabletcpipSubmit();
        }
        else if ($('input[type=checkbox][name=ws_connection]').is(':checked')) {
            enablewsSubmit();
        }
        else {
            enableConnectionSubmit();
        }
    });

    //Hour 
    $('input[type=checkbox][name=hour_range]').change(() => {
        if (!$('input[type=checkbox][name=hour_range]').is(':checked')) {
            $('.hours_range_row').css("display", "none");
            $('input[id=hoursback_range]').val(0);
            $('.hoursbackvalue').html(0);
            $('input[id=hoursforward_range]').val(0);
            $('.hoursforwardvalue').html(0);
        }
        else {
            $('.hours_range_row').css("display", "");
        }
    });
    //radio check
    if ($("input[type=radio][name='connectionType']").change(() => {

        if ($(this).id === "udp_connection") {
            $('input[type=checkbox][name=tcpip_connection]').prop("checked", false);
            $('input[type=checkbox][name=ws_connection]').prop("checked", false);
            $('input[type=text][name=url]').prop("disabled", true);
            $('input[type=text][name=url]').val('');
            $('input[type=text][name=url]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_url]').text("");
            $('input[type=text][name=ip_address]').prop("disabled", true);
            $('input[type=text][name=ip_address]').val('');
            $('input[type=text][name=ip_address]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_ip_address]').text("");
            $('input[type=text][name=hostanme]').prop("disabled", true);
            $('input[type=text][name=hostanme]').val('');
            $('input[type=text][name=hostanme]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_hostanme]').text("");
            $('select[name=data_retrieve]').prop("disabled", true);
            $('select[name=data_retrieve]').val(' ');
            $('select[name=data_retrieve]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_data_retrieve]').text("");
        }
    }))
        if ($('input[type=checkbox][name=udp_connection]').is(':checked')) {
            $('input[type=text][name=url]').prop("disabled", true);
            $('input[type=text][name=url]').val('');
            $('input[type=text][name=url]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_url]').text("");
            //$('input[type=text][name=outgoingapikey]').prop("disabled", true);
            //$('input[type=text][name=outgoingapikey]').val('');
            //$('input[type=text][name=outgoingapikey]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            //$('span[id=error_outgoingapikey]').text("");
            $('input[type=text][name=ip_address]').prop("disabled", true);
            $('input[type=text][name=ip_address]').val('');
            $('input[type=text][name=ip_address]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_ip_address]').text("");
            $('input[type=text][name=hostanme]').prop("disabled", true);
            $('input[type=text][name=hostanme]').val('');
            $('input[type=text][name=hostanme]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_hostanme]').text("");
            $('select[name=data_retrieve]').prop("disabled", true);
            $('select[name=data_retrieve]').val(' ');
            $('select[name=data_retrieve]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_data_retrieve]').text("");
        }
    if ($('input[type=checkbox][name=tcpip_connection]').is(':checked')) {
        $('input[type=text][name=url]').prop("disabled", true);
        $('input[type=text][name=url]').val('');
        $('input[type=text][name=url]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
        $('span[id=error_url]').text("");
        $('input[type=text][name=ip_address]').prop("disabled", true);
        $('input[type=text][name=ip_address]').val('');
        $('input[type=text][name=ip_address]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
        $('span[id=error_ip_address]').text("");
        $('input[type=text][name=hostanme]').prop("disabled", true);
        $('input[type=text][name=hostanme]').val('');
        $('input[type=text][name=hostanme]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
        $('span[id=error_hostanme]').text("");
        $('select[name=data_retrieve]').prop("disabled", true);
        $('select[name=data_retrieve]').val(' ');
        $('select[name=data_retrieve]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
        $('span[id=error_data_retrieve]').text("");
    }
    $('input[type=checkbox][name=udp_connection]').change(() => {
        if (!$('input[type=checkbox][name=udp_connection]').is(':checked')) {
            onRegularConnection();
        }
        else {
            $('input[type=checkbox][name=tcpip_connection]').prop("checked", false);
            $('input[type=checkbox][name=ws_connection]').prop("checked", false);
            $('input[type=text][name=url]').prop("disabled", true);
            $('input[type=text][name=url]').val('');
            $('input[type=text][name=url]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_url]').text("");
            //$('input[type=text][name=outgoingapikey]').prop("disabled", true);
            //$('input[type=text][name=outgoingapikey]').val('');
            //$('input[type=text][name=outgoingapikey]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            //$('span[id=error_outgoingapikey]').text("");
            $('input[type=text][name=ip_address]').prop("disabled", true);
            $('input[type=text][name=ip_address]').val('');
            $('input[type=text][name=ip_address]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_ip_address]').text("");
            $('input[type=text][name=hostanme]').prop("disabled", true);
            $('input[type=text][name=hostanme]').val('');
            $('input[type=text][name=hostanme]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_hostanme]').text("");
            $('select[name=data_retrieve]').prop("disabled", true);
            $('select[name=data_retrieve]').val(' ');
            $('select[name=data_retrieve]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_data_retrieve]').text("");
            // enableUdpSubmit();
        }
    });
    $('input[type=checkbox][name=tcpip_connection]').change(() => {
        if (!$('input[type=checkbox][name=tcpip_connection]').is(':checked')) {
            onRegularConnection();
        }
        else {
            $('input[type=checkbox][name=udp_connection]').prop("checked", false);
            $('input[type=checkbox][name=ws_connection]').prop("checked", false);
            $('input[type=text][name=url]').prop("disabled", true);
            $('input[type=text][name=url]').val('');
            $('input[type=text][name=url]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_url]').text("");
            //$('input[type=text][name=outgoingapikey]').prop("disabled", true);
            //$('input[type=text][name=outgoingapikey]').val('');
            //$('input[type=text][name=outgoingapikey]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            //$('span[id=error_outgoingapikey]').text("");
            $('input[type=text][name=ip_address]').prop("disabled", true);
            $('input[type=text][name=ip_address]').val('');
            $('input[type=text][name=ip_address]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_ip_address]').text("");
            $('input[type=text][name=hostanme]').prop("disabled", true);
            $('input[type=text][name=hostanme]').val('');
            $('input[type=text][name=hostanme]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_hostanme]').text("");
            $('select[name=data_retrieve]').prop("disabled", true);
            $('select[name=data_retrieve]').val(' ');
            $('select[name=data_retrieve]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_data_retrieve]').text("");
            enabletcpipSubmit();
        }
    });
    if ($('input[type=radio][id=udp_connection]').is(':checked')) {
        $('input[type=radio][id=hour_range]').prop('disabled', true);
        enableudpSubmit();
    }
    else if ($('input[type=radio][id=tcpip_connection]').is(':checked')) {
        $('input[type=radio][id=hour_range]').prop('disabled', true);
        enabletcpipSubmit();
    }
    else if ($('input[type=radio][id=ws_connection]').is(':checked')) {
        $('input[type=radio][id=hour_range]').prop('disabled', true);
        enablewsSubmit();
    }
    else {
        enableConnectionSubmit();
    }
    //$('input[type=checkbox][name=ws_connection]').change(() => {
    //    onUpdateWS();
    //});
});
$('#RemoveConfirmationModal').on('shown.bs.modal', function () {
    $('#removeAPImodalHeader_ID').text('Remove Connection');
    sidebar.close('connections');
});
$('#RemoveConfirmationModal').on('hidden.bs.modal', function () {
    sidebar.open('connections');
});
async function updateConnection(Connectionupdate) {
    try {
        updateConnectionDataTable(Connectionupdate, "connectiontable");
    } catch (e) {
        console.log(e);
    }
}
async function init_connection(ConnectionList) {
    try {
        createConnectionDataTable("connectiontable");
        if (ConnectionList.length > 0) {
            loadConnectionDatatable(ConnectionList.sort(SortByConnectionName), "connectiontable")
        }
    } catch (e) {
        console.log(e);
    }
}
//start table function
function createConnectionDataTable(table) {
    let arrayColums = [{
        "ConnectionName": "",
        "MessageType": "",
        "Port": "",
        "Status": "",
        "Action": ""
    }]
    var columns = [];
    var tempc = {};
    $.each(arrayColums[0], function (key, value) {
        tempc = {};
        if (/ConnectionName/i.test(key)) {
            tempc = {
                "title": 'Name',
                "mDataProp": key
            }
        }
        else if (/MessageType/i.test(key)) {
            tempc = {
                "title": "Message Type",
                "mDataProp": key
            }
        }
        else if (/Port/i.test(key)) {
            tempc = {
                "title": "Port",
                "mDataProp": key
            }
        }
        else if (/Status/i.test(key)) {
            tempc = {
                "title": "Status",
                "mDataProp": key

            }
        }
        else if (/Action/i.test(key)) {
            tempc = {
                "title": "Action",
                "mDataProp": key,
                "mRender": function (data, type, full) {
                    return '<button class="btn btn-light btn-sm mx-1 pi-iconEdit connectionedit" name="connectionedit"></button>' + '<button class="btn btn-light btn-sm mx-1 pi-trashFill connectiondelete" name="connectiondelete"></button>'
                }
            }
        }
        else {
            tempc = {
                "title": capitalize_Words(key.replace(/\_/, ' ')),
                "mDataProp": key
            }
        }
        columns.push(tempc);

    });
    $('#' + table).DataTable({
        dom: 'Bfrtip',
        buttons: {
            buttons:
                [
                    {
                        text: "Add",
                        action: function () { Add_Connection(); }
                    }
                ]
        },
        bFilter: false,
        bdeferRender: true,
        bpaging: false,
        bPaginate: false,
        bAutoWidth: true,
        bInfo: false,
        destroy: true,
        language: {
            zeroRecords: "No Data",
        },
        aoColumns: columns,
        columnDefs: [],
        sorting: [[0, "asc"]],
        rowCallback: function (row, data, index) {
            $(row).find('td:eq(0)').css('text-align', 'left');
            if (data.ActiveConnection) {
                if (data.ApiConnected) {
                    $(row).find('td:eq(3)').css('background-color', 'green');
                }
                else {
                    $(row).find('td:eq(3)').css('background-color', 'red');
                }
            }
            else {
                $(row).find('td:eq(3)').css('background-color', 'orange');
            }


        }
    });
    // Edit/remove record
    $('#' + table + ' tbody').on('click', 'button', function () {
        let td = $(this);
        let table = $(td).closest('table');
        let row = $(table).DataTable().row(td.closest('tr'));
        if (/connectionedit/ig.test(this.name)) {
            sidebar.close();
            Edit_Connection(row.data());
        }
        else if (/connectiondelete/ig.test(this.name)) {
            sidebar.close();
            Remove_Connection(row.data());
        }
    });
}
function loadConnectionDatatable(data, table) {
    if ($.fn.dataTable.isDataTable("#" + table)) {
        if (!$.isEmptyObject(data)) {
            $('#' + table).DataTable().rows.add(data).draw();
        }
    }
}
function updateConnectionDataTable(newdata, table) {
    let loadnew = true;
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows(function (idx, data, node) {
            loadnew = false;
            if (data.Id === newdata.Id) {
                $('#' + table).DataTable().row(node).data(newdata).draw().invalidate();
            }
        })
        if (loadnew) {
            loadConnectionDatatable(newdata, table);
        }
    }
}
function removeConnectionDataTable(table) {
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows(function (idx, data, node) {
            $('#' + table).DataTable().row(node).remove().draw();
        })
    }
}
//end table function 
function Add_Connection() {
    $('#modalHeader_ID').text('Add Connection');
    $('button[id=apisubmitBtn]').off().on('click', function () {
        $('button[id=apisubmitBtn]').prop('disabled', true);
        $('input[type=checkbox][name=ws_connection]').prop('disabled', false);
        $('input[type=checkbox][name=udp_connection]').prop('disabled', false);
        $('input[type=checkbox][name=tcpip_connection]').prop('disabled', false);
        let jsonObject = {
            ActiveConnection: $('input[type=checkbox][name=active_connection]').is(':checked'),
            ApiConnection: $('input[type=checkbox][name=api_connection]').is(':checked'),
            UdpConnection: $('input[type=checkbox][name=udp_connection]').is(':checked'),
            TcpIpConnection: $('input[type=checkbox][name=tcpip_connection]').is(':checked'),
            WsConnection: $('input[type=checkbox][name=ws_connection]').is(':checked'),
            HoursBack: parseInt($('input[id=hoursback_range]').val()),
            HoursForward: parseInt($('input[id=hoursforward_range]').val()),
            DataRetrieve: $('select[name=data_retrieve] option:selected').val(),
            ConnectionName: $('input[type=text][name=connection_name]').val(),
            IpAddress: $('input[type=text][name=ip_address]').val(),
            Port: $('input[type=text][name=port_number]').val(),
            Url: $('input[type=text][name=url]').val(),
            MessageType: $('input[type=text][name=message_type]').val(),
            /*AdminEmailRecepient: $('input[type=text][name=admin_email_recepient').val(),*/
            CreatedByUsername: User.UserId,
            NassCode: User.Facility_NASS_Code,
        };
        if (!$.isEmptyObject(jsonObject)) {
            fotfmanager.server.addAPI(JSON.stringify(jsonObject)).done(function (Data) {
                if (Data.length === 1) {
                    updateConnection(Data[0]);
                    $('span[id=error_apisubmitBtn]').text(Data[0].ConnectionName + " " + Data[0].MessageType + " Connection has been Added");
                    setTimeout(function () { $("#API_Connection_Modal").modal('hide'); sidebar.open('connections'); }, 1500);
                }
                else {
                    $('span[id=error_apisubmitBtn]').text("Error Adding Connection");
                }
            });
        }
    });
    $('#API_Connection_Modal').modal();
}
function Edit_Connection(Data) {
    $('#modalHeader_ID').text('Edit Connection');
    $('input[type=checkbox][id=active_connection]').prop('checked', Data.ActiveConnection);
    $('input[type=text][id=connection_name]').val(Data.ConnectionName);
    $('input[type=text][id=ip_address]').val(Data.IpAddress);
    $('input[type=text][id=hostname]').val(Data.Hostname);
    $('input[type=text][id=port_number]').val(Data.Port);
    $('input[type=text][id=url]').val(Data.Url);
    $('input[type=text][id=message_type]').val(Data.MessageType);
    $('select[name=data_retrieve]').val(Data.DataRetrieve);
    if (Data.ApiConnection) {
        $('input[type=radio][id=api_connection]').prop('checked', Data.ApiConnection);
    }
    if (Data.UdpConnection) {
        $('input[type=radio][id=udp_connection]').prop('checked', Data.UdpConnection);
    }
    if (Data.TcpIpConnection) {
        $('input[type=radio][id=tcpip_connection]').prop('checked', Data.TcpIpConnection);
    }
    if (Data.WsConnection) {
        $('input[type=radio][id=ws_connection]').prop('checked', Data.WsConnection);
    }
    if (Data.HoursBack > 0 || Data.HoursForward > 0) {

        $('.hoursbackvalue').html($.isNumeric(Data.HoursBack) ? parseInt(Data.HoursBack) : 0);
        $('input[id=hoursback_range]').val($.isNumeric(Data.HoursBack) ? parseInt(Data.HoursBack) : 0);
        $('.hours_range_row').css("display", "");
        $('.hoursforwardvalue').html($.isNumeric(Data.HoursForward) ? parseInt(Data.HoursForward) : 0);
        $('input[id=hoursforward_range]').val($.isNumeric(Data.HoursForward) ? parseInt(Data.HoursForward) : 0);
        $('.hours_range_row').css("display", "");

        $('input[type=checkbox][name=hour_range]').prop('checked', true).change();

    }
    else {
        $('.hoursbackvalue').html($.isNumeric(Data.HoursBack) ? parseInt(Data.HoursBack) : 0);
        $('input[id=hoursback_range]').val($.isNumeric(Data.HoursBack) ? parseInt(Data.HoursBack) : 0);
        $('.hours_range_row').css("display", "none");
        $('.hoursforwardvalue').html($.isNumeric(Data.HoursForward) ? parseInt(Data.HoursForward) : 0);
        $('input[id=hoursforward_range]').val($.isNumeric(Data.HoursForward) ? parseInt(Data.HoursForward) : 0);
        $('.hours_range_row').css("display", "none");
        $('input[type=checkbox][name=hour_range]').prop('checked', false).change();
    }
    $('button[id=apisubmitBtn]').prop('disabled', true);
    $('button[id=apisubmitBtn]').off().on('click', function () {
        try {
            $('button[id=apisubmitBtn]').prop('disabled', true);
            let jsonObject = {
                ActiveConnection: $('input[type=checkbox][name=active_connection]').is(':checked'),
                ApiConnection: $('input[type=radio][id=api_connection]').is(':checked'),
                UdpConnection: $('input[type=radio][id=udp_connection]').is(':checked'),
                TcpIpConnection: $('input[type=radio][id=tcpip_connection]').is(':checked'),
                WsConnection: $('input[type=radio][name=ws_connection]').is(':checked'),
                HoursBack: $('input[type=checkbox][name=hour_range]').is(':checked') ? parseInt($('input[id=hoursback_range]').val()) : 0,
                HoursForward: $('input[type=checkbox][name=hour_range]').is(':checked') ? parseInt($('input[id=hoursforward_range]').val()) : 0,
                DataRetrieve: $('select[name=data_retrieve] option:selected').val(),
                ConnectionName: $('input[type=text][name=connection_name]').val(),
                IpAddress: $('input[type=text][id=ip_address]').val(),
                Port: $('input[type=text][name=port_number]').val(),
                Url: $('input[type=text][name=url]').val(),
                MessageType: $('input[type=text][name=message_type]').val(),
                LastupdateByUsername: User.UserId,
                Id: Data.Id
            }
            if (!$.isEmptyObject(jsonObject)) {
                fotfmanager.server.editAPI(JSON.stringify(jsonObject)).done(function (rData) {
                    setTimeout(function () { $("#API_Connection_Modal").modal('hide'); sidebar.open('connections'); }, 500);
                });
            }

        } catch (e) {
            $('span[id=error_apisubmitBtn]').text(e);
        }
    });
    $('#API_Connection_Modal').modal();
}
function Remove_Connection(id) {
    try {
        $('button[id=remove_server_connection]').off().on('click', function () {
            let jsonObject = { Id: id };
            fotfmanager.server.removeAPI(JSON.stringify(jsonObject)).done(function (Data) {
                $("#api_" + id).remove();
                setTimeout(function () {
                    $("#RemoveConfirmationModal").modal('hide');
                    sidebar.open('connections');
                }, 1500);
            })
        });
        $('#RemoveConfirmationModal').modal();
    } catch (e) {
        console.log(e);
    }
}

function GetConnectionStatus(data) {
    if (data.ActiveConnection) {
        if (data.ApiConnected) {
            return "Online";
        }
        else {
            return "Off-Line";
        }
    }
    else {
        return "Disabled";
    }
}
function enableConnectionSubmit() {
    //AGV connections
    if ($('input[type=text][name=ip_address]').hasClass('is-valid') &&
        $('input[type=text][name=url]').hasClass('is-valid') &&
        $('input[type=text][name=message_type]').hasClass('is-valid') &&
        $('select[name=data_retrieve]').hasClass('is-valid') &&
        $('input[type=text][name=connection_name]').hasClass('is-valid')
    ) {
        $('button[id=apisubmitBtn]').prop('disabled', false);
    }
    else {
        $('button[id=apisubmitBtn]').prop('disabled', true);
    }
}
function SortByConnectionName(a, b) {
    return a.ConnectionName < b.ConnectionName ? -1 : a.ConnectionName > b.ConnectionName ? 1 : 0;
}
function getHHMMSSFromSeconds(sec) {
    var utcString = (new Date(sec * 1000)).toUTCString();
    var match = utcString.match(/(\d+:\d\d:\d\d)/)[0];
    return match;
}
$.extend(fotfmanager.client, {
    updateBinZoneStatus: async (binzoneupdate, id) => { updateBinZone(binzoneupdate, id) }
});

var binzonepoly = new L.GeoJSON(null, {
    style: function (feature) {
        return {
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillOpacity: 0.2,
            fillColor: '#989ea4',
            label: feature.properties.name,
            lastOpacity: 0.2
        };
    },
    onEachFeature: function (feature, layer) {
        var flash = "";
        if (feature.properties.MPE_Bins.length > 0)
        {
                flash = "doorflash";
        }
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        layer.on('click', function (e) {
            //set to the center of the polygon.
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng);
            if ((' ' + document.getElementById('sidebar').className + ' ').indexOf(' ' + 'collapsed' + ' ') <= -1) {
                if ($('#zoneselect').val() == feature.properties.id) {
                    sidebar.close('home');
                }
                else {
                    sidebar.open('home');
                }
            }
            else {
                sidebar.open('home');
            }
            LoadBinZoneTables(feature.properties);
        });
        layer.bindTooltip("", {
            permanent: true,
            interactive: true,
            direction: 'center',
            opacity: 1,
            className: 'dockdooknumber ' + flash
        }).openTooltip();
        binzonepoly.bringToFront();
    },
    filter: function (feature, layer) {
        return feature.properties.visible;
    }
});

async function LoadBinZoneTables(dataproperties) {
    try {
        $zoneSelect[0].selectize.setValue(dataproperties.id, true);
        hideSidebarLayerDivs();
        $('div[id=area_div]').attr("data-id", dataproperties.id);
        $('div[id=area_div]').css('display', 'block');
        czzonetop_Table_Body.empty();
        czzonetop_Table_Body.append(czzonetop_row_template.supplant(formatczzonetoprow(dataproperties)));
    } catch (e) {
        console.log(e)
    }
}

let czzonetop_Table = $('table[id=areazonetoptable]');
let czzonetop_Table_Body = czzonetop_Table.find('tbody');
let czzonetop_row_template = '<tr data-id="{zoneId}"><td style="width: 22%;">Bin Zone Name</td><td>{zoneName}</td></tr>' +
    '<tr><td>Bins Configured</td><td>{AssignedBins}</td></tr>' +
    '<tr><td>Full Bins</td><td>{fullbins}</td></tr>';
function formatczzonetoprow(properties) {
    return $.extend(properties, {
        zoneId: properties.id,
        zoneName: properties.name,
        AssignedBins: properties.bins,
        fullbins: properties.MPE_Bins.toString()
    });
}
async function updateBinZone(binzoneupdate, id) {
    try {
        if (id == baselayerid) {
            let layerindex = -0;
            if (binzonepoly.hasOwnProperty("_layers")) {
                $.map(binzonepoly._layers, function (layer, i) {
                    if (layer.feature.properties.id === binzoneupdate.properties.id) {
                        layer.feature.properties = binzoneupdate.properties;
                        layerindex = layer._leaflet_id;
                        Promise.all([updatebin(layerindex)]);
                        return false;
                    }
                });
                if (layerindex !== -0) {
                    if ($('div[id=area_div]').is(':visible') && $('div[id=area_div]').attr("data-id") === binzoneupdate.properties.id) {

                    }

                }
                else {
                    binzonepoly.addData(binzoneupdate);
                }
            }
        }
    } catch (e) {
        console.log(e);
    }
}
async function updatebin(layerindex) {
    //add flashing to the bind 
    if (binzonepoly._layers[layerindex].feature.properties.MPE_Bins.length > 0) {
        if (binzonepoly._layers[layerindex].hasOwnProperty("_tooltip")) {
            if (binzonepoly._layers[layerindex]._tooltip.hasOwnProperty("_container")) {
                if (!binzonepoly._layers[layerindex]._tooltip._container.classList.contains('doorflash')) {
                    binzonepoly._layers[layerindex]._tooltip._container.classList.add('doorflash');
                }
            }
        }
        binzonepoly._layers[layerindex].setStyle({
            weight: 1,
            opacity: 1,
            fillOpacity: 0.5,
            fillColor: "#ff8855",
            lastOpacity: 0.5
        });
    }
    else {
        if (binzonepoly._layers[layerindex].hasOwnProperty("_tooltip")) {
            if (binzonepoly._layers[layerindex]._tooltip.hasOwnProperty("_container")) {
                if (binzonepoly._layers[layerindex]._tooltip._container.classList.contains('doorflash')) {
                    binzonepoly._layers[layerindex]._tooltip._container.classList.remove('doorflash');
                }
            }
        }
        binzonepoly._layers[layerindex].setStyle({
            weight: 1,
            opacity: 1,
            fillOpacity: 0.2,
            fillColor: "#989ea4",
            lastOpacity: 0.2
        });
    }
    return true;
}



var stagingBullpenAreas = new L.GeoJSON(null, {
    style: function (feature) {
        return  {
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillOpacity: 0.2,
            fillColor: '#989ea4',//'gray'
            lastOpacity: 0.2
        };
    },
    onEachFeature: function (feature, layer) {
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        layer.on('click', function (e) {
            //set to the center of the polygon.
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng);
            if ((' ' + document.getElementById('sidebar').className + ' ').indexOf(' ' + 'collapsed' + ' ') <= -1) {
                if ($('#zoneselect').val() == feature.properties.id) {
                    sidebar.close('home');
                }
                else {
                    sidebar.open('home');
                }
            }
            else {
                sidebar.open('home');
            }
            LoadStagingBullpenTables(feature.properties);

        });

        layer.bindTooltip(feature.properties.name, {
            permanent: true,
            interactive: true,
            direction: 'center',
            opacity: 1,
            className: 'location'
        }).openTooltip();
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        stagingBullpenAreas.bringToBack();
    },
    filter: function (feature, layer) {
        return feature.properties.visible;
    }

});

// loads the sidebar for the staging bullpen area zone
async function LoadStagingBullpenTables(dataproperties) {
    try {
        hideSidebarLayerDivs();
        $zoneSelect[0].selectize.setValue(dataproperties.id, true);
        $('div[id=bullpen_div]').attr("data-id", dataproperties.id);
        $('div[id=bullpen_div]').css('display', 'block');
        zonetop_Table_Body.empty();
        zonetop_Table_Body.append(zonetop_row_template.supplant(formatzonetoprow(dataproperties)));
       
    } catch (e) {
        console.log(e.message + ", " + e.stack);
    }
}
/*
 this is for view ports
 */
$(function () {
    $.extend(fotfmanager.client, {
        updateViewPortStatus: async (viewportzoneupdate) => { updateviewportzone(viewportzoneupdate)      }
    });
    
});
async function updateviewportzone() {
    try {
    } catch (e) {
        console.log(e);
    }
}
var viewPortsAreas = new L.GeoJSON(null, {
    style: function (feature) {
        return {
            weight: 2,
            opacity: 1,
            color: '#3573b1',
            fillColor: 'pink',
            fillOpacity: 0.2,
            label: feature.properties.name
        };
    },
    onEachFeature: function (feature, layer) {
        var vp_name = feature.properties.name;
        var vp_namer = vp_name.replace(/^VP_/, '');
        $('<div/>', { id: 'div_' + feature.properties.id }).append($('<button/>', { class: 'btn btn-light btn-sm mx-1 py-2 viewportszones', id: feature.properties.id, text: vp_namer })).appendTo($('div[id=viewportsContent]'))
        layer.bindTooltip(feature.properties.name, {
            permanent: true,
            interactive: true,
            direction: 'center',
            opacity: 1
        }).openTooltip();
        layer.bringToBack();
    }
});
/**
 * this is use to setup a the machine information and other function
 * 
 * **/

const sparklineTooltipOpacity = .5;
//on close clear all inputs
$('#Zone_Modal').on('hidden.bs.modal', function () {
    $(this)
        .find("input[type=text],textarea,select")
        .css({ "border-color": "#D3D3D3" })
        .val('')
        .end()
        .find("span[class=text]")
        .css("border-color", "#FF0000")
        .val('')
        .text("")
        .end()
        .find('input[type=checkbox]')
        .prop('checked', false).change();
});
//on open set rules
$('#Zone_Modal').on('shown.bs.modal', function () {
    $('span[id=error_machinesubmitBtn]').text("");
    $('button[id=machinesubmitBtn]').prop('disabled', true);
    //Request Type Keyup
    $('input[type=text][name=machine_name]').keyup(function () {
        if (!checkValue($('input[type=text][name=machine_name]').val())) {
            $('input[type=text][name=machine_name]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_machine_name]').text("Please Enter Machine Name");
        }
        else {
            $('input[type=text][name=machine_name]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_machine_name]').text("");
        }

        enablezoneSubmit();
    });
    //Connection name Validation
    if (!checkValue($('input[type=text][name=machine_name]').val())) {
        $('input[type=text][name=machine_name]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_machine_name]').text("Please Enter Machine Name");
    }
    else {
        $('input[type=text][name=machine_name]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_machine_name]').text("");
    }
    //Request Type Keyup
    $('input[type=text][name=machine_number]').keyup(function () {
        if (!checkValue($('input[type=text][name=machine_number]').val())) {
            $('input[type=text][name=machine_number]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_machine_number]').text("Please Enter Machine Number");
        }
        else {
            $('input[type=text][name=machine_number]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_machine_number]').text("");
        }

        enablezoneSubmit();
    });
    //Request Type Validation
    if (!checkValue($('input[type=text][name=machine_number]').val())) {
        $('input[type=text][name=machine_number]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_machine_number]').text("Please Enter Machine Number");
    }
    else {
        $('input[type=text][name=machine_number]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_machine_number]').text("");
    }
    //Request Type Validation
    if (checkValue($('input[type=text][name=zone_ldc]').val())) {
        $('input[type=text][name=zone_ldc]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_zone_ldc]').text("");
    }
    else {
        $('input[type=text][name=zone_ldc]').css("border-color", "#D3D3D3").removeClass('is-invalid').removeClass('is-valid');
        $('span[id=error_zone_ldc]').text("");
    }
    //Request zone LDC Keyup
    $('input[type=text][name=zone_ldc]').keyup(function () {
        if (checkValue($('input[type=text][name=zone_ldc]').val())) {
            $('input[type=text][name=zone_ldc]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_zone_ldc]').text("");
        }
        else {
            $('input[type=text][name=zone_ldc]').css("border-color", "#D3D3D3").removeClass('is-invalid').removeClass('is-valid');
            $('span[id=error_zone_ldc]').text("");
        }
    });
});


var lastMachineStatuses = "";
$.extend(fotfmanager.client, {
    updateMachineStatus: async (machineStatuses) => {
        let machineStatusesString = JSON.stringify(machineStatuses);
        if (lastMachineStatuses != machineStatusesString) {
            lastMachineStatuses = machineStatusesString;

            let sparklineStatuses = convertToSparkline(machineStatusesString);
            for (var tuple of machineStatuses) {

                updateMachineZone(tuple.Item1, tuple.Item2);

            }
            // clears the sparkline graph cache if it is old data
            clearSparklineCache();
            updateAllMachineSparklines(sparklineStatuses, 0);
        }
    }
});

var sparklineMinZoom = 2;
async function updateMachineZone(machineupdate, id) {
    try {
        if (id == baselayerid) {
            if (polygonMachine.hasOwnProperty("_layers")) {
                var layerindex = -0;
                $.map(polygonMachine._layers, function (layer, i) {
                    if (layer.hasOwnProperty("feature")) {
                        if (layer.feature.properties.id === machineupdate.properties.id) {
                            
                            if (layer.feature.properties.name != machineupdate.properties.name) {
                                layer.setTooltipContent(machineupdate.properties.name + "<br/>" + "Staffing: " + machineupdate.properties.CurrentStaff);
                            }
                            layer.feature.properties = machineupdate.properties;
                            layerindex = layer._leaflet_id;
                            return false;
                        }
                    }
                });
                if (layerindex !== -0) {
                    if ($('div[id=machine_div]').is(':visible') && $('div[id=machine_div]').attr("data-id") === machineupdate.properties.id) {
                        LoadMachineTables(machineupdate.properties, 'machinetable');
                    }
                    updateMPEZone(machineupdate.properties, layerindex);
                }
                else {
                    polygonMachine.addData(machineupdate);
                }
            }
        }
    } catch (e) {
        console.log(e);
    }
}
$(function () {
    $('button[name=machineinfoedit]').off().on('click', function () {
        /* close the sidebar */
        sidebar.close();
        var id = $(this).attr('id');
        if (checkValue(id)) {
            Edit_Machine_Info(id);
        }
    });
});

function updateMPEZoneTooltipDirection() {

    let mpeWorkAreasChecked = $("#MPEWorkAreas").prop("checked");
    if (mpeWorkAreasChecked) {
        let keys = Object.keys(polygonMachine._layers);
        let tooltipDirection = getMPEZoneTooltipDirection();
        for (const key of keys) {
            let layer = polygonMachine._layers[key];
            if (layer && layer._tooltip !== null) {

                layer.getTooltip().options.direction = tooltipDirection;
                layer.closeTooltip();
                layer.openTooltip();
            }


        }
    }
}
function getMPEZoneTooltipDirection() {
    let mpeSparklinesChecked = $("#MPESparklines").prop("checked");
    if (Object.keys(machineSparklines._layers).length == 0 ||
        !mpeSparklinesChecked) {
        return "center";
    }
    return "left";
}

function updateSparklineTooltipDirection() {

    let mpeSparklinesChecked = $("#MPESparklines").prop("checked");

    if (mpeSparklinesChecked) {
        let keys = Object.keys(machineSparklines._layers);

        var tooltipDirection = getSparklineTooltipDirection();
        for (const key of keys) {
            let layer = machineSparklines._layers[key];
            if (layer && layer._tooltip !== null) {
                layer.getTooltip().options.direction = tooltipDirection;
                layer.closeTooltip();
                layer.openTooltip();
            }


        }
    }
}
function getSparklineTooltipDirection() {

    let mpeWorkAreasChecked = $("#MPEWorkAreas").prop("checked");
    if (Object.keys(polygonMachine._layers).length == 0 ||
        !mpeWorkAreasChecked) {
        return "center";
    }
    return "right";
}

function getPolygonMachineStyle(feature) {

    var style = {};
    var sortplan = feature.properties.hasOwnProperty("MPEWatchData") ? feature.properties.MPEWatchData.hasOwnProperty("cur_sortplan") ? feature.properties.MPEWatchData.cur_sortplan : "" : "";
    var endofrun = feature.properties.hasOwnProperty("MPEWatchData") ? feature.properties.MPEWatchData.hasOwnProperty("current_run_end") ? feature.properties.MPEWatchData.current_run_end != "0" ? feature.properties.MPEWatchData.current_run_end : "" : "" : "";
    var startofrun = feature.properties.hasOwnProperty("MPEWatchData") ? feature.properties.MPEWatchData.hasOwnProperty("current_run_start") ? feature.properties.MPEWatchData.current_run_start : "" : "";
    if (checkValue(sortplan) && !checkValue(endofrun)) {
        var thpCode = feature.properties.hasOwnProperty("MPEWatchData") ? feature.properties.MPEWatchData.hasOwnProperty("throughput_status") ? feature.properties.MPEWatchData.throughput_status : "0" : "0";
        var fillColor = GetMacineBackground
            (feature.properties.MPEWatchData, startofrun);
        style = {
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillOpacity: 0.5,
            fillColor: fillColor,
            lastOpacity: 0.5
        };
    }
    else {
        style = {
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillOpacity: 0.2,
            fillColor: '#989ea4',
            lastOpacity: 0.2
        };
    }
    return style;
}
const polyObj = {
    style: function (feature) {
        if (feature.properties.sparkline) {
            return {
                permanent: true,
                interactive: true,
                color: "transparent",
                fillColor: "transparent",
                fillOpacity: 0,
                opacity: 0,
                lastOpacity: 0
            };
        }
        if (feature.properties.visible) {
            return getPolygonMachineStyle(feature);
        
        }
            
    },
    onEachFeature: function (feature, layer) {
        
        if (feature.properties.sparkline) {
            updateMachineSparklineTooltip(feature, layer);
        }
        else {
            $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
            $zoneSelect[0].selectize.addItem(feature.properties.id);
            $zoneSelect[0].selectize.setValue(-1, true);
            layer.on('click', function (e) {
                $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
                map.setView(e.sourceTarget.getCenter(), 3);
                if ((' ' + document.getElementById('sidebar').className + ' ').indexOf(' ' + 'collapsed' + ' ') <= -1) {
                    if ($('#zoneselect').val() == feature.properties.id) {
                        sidebar.close('home');
                    }
                    else {
                        sidebar.open('home');
                    }
                }
                else {
                    sidebar.open('home');
                }
                LoadMachineTables(feature.properties, 'machinetable');
            });
            layer.bindTooltip(feature.properties.name + "<br/>" + "Staffing: " + (feature.properties.hasOwnProperty("CurrentStaff") ? feature.properties.CurrentStaff : "0"), {
                permanent: true,
                interactive: true,
                direction: getMPEZoneTooltipDirection(),
                opacity: 1,
                className: 'location'
            }).openTooltip();
        }
    },
    filter: function (feature, layer) {
        if (/(way)$/i.test(feature.properties.name)) {
            return false;
        }
        else {
            return feature.properties.visible;
        }
    }
};
var polygonMachine = new L.GeoJSON(null, polyObj);
async function updateMPEZone(properties, index) {
    var sortplan = properties.MPEWatchData.hasOwnProperty("cur_sortplan") ? properties.MPEWatchData.cur_sortplan : "";
    var endofrun = properties.MPEWatchData.hasOwnProperty("current_run_end") ? properties.MPEWatchData.current_run_end == "0" ? "" : properties.MPEWatchData.current_run_end : "";
    var startofrun = properties.MPEWatchData.hasOwnProperty("current_run_start") ? properties.MPEWatchData.current_run_start : "";
    var opacityValue = 1;
    var fillOpacityValue = .5;
    if (properties.transparent) {
        opacityValue = 0;
        fillOpacityValue = 0;
    }
    if (checkValue(sortplan) && !checkValue(endofrun)) {
        var thpCode = properties.MPEWatchData.hasOwnProperty("throughput_status") ? properties.MPEWatchData.throughput_status : "0";
        var fillColor = GetMacineBackground(properties.MPEWatchData, startofrun);
        polygonMachine._layers[index].setStyle({
            weight: 1,
            opacity: opacityValue,
            fillOpacity: fillOpacityValue,
            fillColor: fillColor,
            lastOpacity: fillOpacityValue
        });
    }
    else {
        if (polygonMachine._layers[index].options.fillColor !== '#989ea4') { //'gray'
            polygonMachine._layers[index].setStyle({
                weight: 1,
                opacity: 1,
                fillOpacity: 0.2,
                fillColor: '#989ea4',//'gray'
                lastOpacity: 0.2
            });
        }
    }
}
async function LoadMachineTables(dataproperties, table) {
    try {
        if (!$.isEmptyObject(dataproperties)) {
            $('div[id=machine_div]').attr("data-id", dataproperties.id);
            hideSidebarLayerDivs();
            $('div[id=machine_div]').css('display', 'block');
            $('div[id=ctstabs_div]').css('display', 'block');
            if (/machinetable/i.test(table)) {

                $zoneSelect[0].selectize.setValue(dataproperties.id, true);
                $('button[name=machineinfoedit]').attr('id', dataproperties.id)
                $('div[id=dps_div]').css('display', 'none');
                let machinetop_Table = $('table[id=' + table + ']');
                let machinetop_Table_Body = machinetop_Table.find('tbody');
                machinetop_Table_Body.empty();
                machinetop_Table_Body.append(machinetop_row_template.supplant(formatmachinetoprow(dataproperties)));

                if (dataproperties.MPEWatchData.hasOwnProperty("bin_full_bins")) {
                    if (dataproperties.MPEWatchData.bin_full_bins != "") {
                        var result_style = document.getElementById('fullbin_tr').style;
                        result_style.display = 'table-row';
                        $("tr:visible").each(function (index) {
                            var curcolor = $(this).css("background-color");
                            if (curcolor == "" || curcolor == "rgba(0, 0, 0, 0)" || curcolor == "rgba(0, 0, 0, 0.05)") {
                                $(this).css("background-color", !!(index & 1) ? "rgba(0, 0, 0, 0)" : "rgba(0, 0, 0, 0.05)");
                            }
                        });
                    }
                }

                if (dataproperties.MPEWatchData.cur_operation_id == "918" || dataproperties.MPEWatchData.cur_operation_id == "919") {
                    LoadMachineTables(dataproperties, "dpstable");
                }
                var staffdata = dataproperties.hasOwnProperty("staffingData") ? dataproperties.staffingData : "";
                var MachineCurrentStaff = [];
                GetPeopleInZone(dataproperties.id, staffdata, MachineCurrentStaff);
                if (dataproperties.MPEWatchData.hasOwnProperty("current_run_end")) {
                    if (dataproperties.MPEWatchData.current_run_end == "" || dataproperties.MPEWatchData.current_run_end == "0") {
                        var runEndTR = document.getElementById('endtime_tr').style;
                        runEndTR.display = 'none';

                        $("tr:visible").each(function (index) {
                            var curcolor = $(this).css("background-color");
                            if (curcolor == "" || curcolor == "rgba(0, 0, 0, 0)" || curcolor == "rgba(0, 0, 0, 0.05)") {
                                $(this).css("background-color", !!(index & 1) ? "rgba(0, 0, 0, 0)" : "rgba(0, 0, 0, 0.05)");
                            }
                        });

                    }
                }
                if (dataproperties.MPEWatchData.hasOwnProperty("ars_recrej3")) {
                    if (dataproperties.MPEWatchData.ars_recrej3 != "0" && dataproperties.MPEWatchData.ars_recrej3 != "") {
                        var arsrec_tr = document.getElementById('arsrec_tr').style;
                        arsrec_tr.display = 'table-row';
                        $("tr:visible").each(function (index) {
                            var curcolor = $(this).css("background-color");
                            if (curcolor == "" || curcolor == "rgba(0, 0, 0, 0)" || curcolor == "rgba(0, 0, 0, 0.05)") {
                                $(this).css("background-color", !!(index & 1) ? "rgba(0, 0, 0, 0)" : "rgba(0, 0, 0, 0.05)");
                            }
                        });
                    }
                }
                if (dataproperties.MPEWatchData.hasOwnProperty("sweep_recrej3")) {
                    if (dataproperties.MPEWatchData.sweep_recrej3 != "0" && dataproperties.MPEWatchData.sweep_recrej3 != "") {
                        var sweeprec_tr = document.getElementById('sweeprec_tr').style;
                        sweeprec_tr.display = 'table-row';
                        $("tr:visible").each(function (index) {
                            var curcolor = $(this).css("background-color");
                            if (curcolor == "" || curcolor == "rgba(0, 0, 0, 0)" || curcolor == "rgba(0, 0, 0, 0.05)") {
                                $(this).css("background-color", !!(index & 1) ? "rgba(0, 0, 0, 0)" : "rgba(0, 0, 0, 0.05)");
                            }
                        });
                    }
                }
                document.getElementById('machineChart_tr').style.backgroundColor = 'rgba(0,0,0,0)';
                if (dataproperties.MPEWatchData.hasOwnProperty("hourly_data")) {
                    GetMachinePerfGraph(dataproperties);
                }
                else {
                    var mpgtrStyle = document.getElementById('machineChart_tr').style;
                    mpgtrStyle.display = 'none';
                }
                var startofrun = dataproperties.hasOwnProperty("MPEWatchData") ? dataproperties.MPEWatchData.hasOwnProperty("current_run_start") ? dataproperties.MPEWatchData.current_run_start : "" : "";
                var expectedTP = dataproperties.hasOwnProperty("MPEWatchData") ? dataproperties.MPEWatchData.hasOwnProperty("expected_throughput") ? dataproperties.MPEWatchData.expected_throughput : "" : "";
                var throughput = dataproperties.hasOwnProperty("MPEWatchData") ? dataproperties.MPEWatchData.hasOwnProperty("cur_thruput_ophr") ? dataproperties.MPEWatchData.cur_thruput_ophr : "" : "";
                var thpCode = dataproperties.hasOwnProperty("MPEWatchData") ? dataproperties.MPEWatchData.hasOwnProperty("throughput_status") ? dataproperties.MPEWatchData.throughput_status : "0" : "0";
                FormatMachineRowColors(dataproperties.MPEWatchData, startofrun);
            }
            if (/dpstable/i.test(table)) {
                if (dataproperties.hasOwnProperty("DPSData")) {
                    if (checkValue(dataproperties.DPSData)) {
                        $zoneSelect[0].selectize.setValue(dataproperties.id, true);
                        $('div[id=dps_div]').css('display', 'block');
                        let dpstop_Table = $('table[id=' + table + ']');
                        let dpstop_Table_Body = dpstop_Table.find('tbody')
                        dpstop_Table_Body.empty();
                        dpstop_Table_Body.append(dpstop_row_template.supplant(formatdpstoprow(dataproperties.DPSData)));
                    }
                }
            }

        }
    } catch (e) {
        console.log(e);
    }
}
function formatmachinetoprow(properties) {
    return $.extend(properties, {
        zoneId: properties.id,
        zoneName: properties.name,
        zoneType: properties.Zone_Type,
        sortPlan: checkValue(properties.MPEWatchData.cur_sortplan) ? properties.MPEWatchData.cur_sortplan : "N/A",
        opNum: properties.MPEWatchData.cur_operation_id.padStart(3, "0"),
        sortPlanStart: properties.MPEWatchData.current_run_start,
        sortPlanEnd: properties.MPEWatchData.current_run_end,
        peicesFed: digits(properties.MPEWatchData.tot_sortplan_vol),
        throughput: digits(properties.MPEWatchData.cur_thruput_ophr),
        rpgVol: digits(properties.MPEWatchData.rpg_est_vol),
        stateBadge: getstatebadge(properties),
        stateText: getstateText(properties),
        estComp: checkValue(properties.MPEWatchData.rpg_est_comp_time) ? properties.MPEWatchData.rpg_est_comp_time : "Estimate Not Available",
        rpgStart: moment(properties.MPEWatchData.rpg_start_dtm, "MM/DD/YYYY hh:mm:ss A").format("YYYY-MM-DD HH:mm:ss"),
        rpgEnd: moment(properties.MPEWatchData.rpg_end_dtm, "MM/DD/YYYY hh:mm:ss A").format("YYYY-MM-DD HH:mm:ss"),
        expThroughput: digits(properties.MPEWatchData.expected_throughput),
        fullBins: properties.MPEWatchData.bin_full_bins,
        arsRecirc: properties.MPEWatchData.ars_recrej3,
        sweepRecirc: properties.MPEWatchData.sweep_recrej3,
    });
}
let machinetop_row_template =
    '<tr data-id="{zoneId}"><td>{zoneType}</td><td>{zoneName}</td><td><span class="badge badge-pill {stateBadge}" style="font-size: 12px;">{stateText}</span></td></tr>' +
    '<tr id="SortPlan_tr"><td>OPN / Sort Plan</td><td colspan="2">{opNum} / {sortPlan}</td></tr>' +
    '<tr id="StartTime_tr"><td>Start</td><td colspan="2">{sortPlanStart}</td></tr>' +
    '<tr id="endtime_tr"><td>End</td><td colspan="2">{sortPlanEnd}</td></tr>' +
    '<tr id="EstComp_tr"><td>Estimated Completion</td><td colspan="2">{estComp}</td>/tr>' +
    '<tr><td>Pieces Fed / RPG Vol.</td><td colspan="2">{peicesFed} / {rpgVol}</td></tr>' +
    '<tr id="Throughput_tr"><td>Throughput Act. / Exp.</td><td colspan="2">{throughput} / {expThroughput}</td></tr>' +
    '<tr id="fullbin_tr" style="display: none;"><td>Full Bins</td><td colspan="2" style="white-space: normal; word-wrap:break-word;">{fullBins}</td></tr>' +
    '<tr id="arsrec_tr" style="display: none;"><td>ARS Recirc. Rejects</td><td colspan="2">{arsRecirc}</td></tr>' +
    '<tr id="sweeprec_tr" style="display: none;"><td>Sweep Recirc. Rejects</td><td colspan="2">{sweepRecirc}</td></tr>' +
    '<tr id="machineChart_tr"><td colspan="3"><canvas id="machinechart"></canvas></td></tr>';

function formatdpstoprow(properties) {
    return $.extend(properties, {
        dpssortplans: properties.sortplan_name_perf,
        piecesfedfirstpass: digits(properties.pieces_fed_1st_cnt),
        piecesrejectedfirstpass: digits(properties.pieces_rejected_1st_cnt),
        piecestosecondpass: digits(properties.pieces_to_2nd_pass),
        piecesfedsecondpass: digits(properties.pieces_fed_2nd_cnt),
        piecesrejectedsecondpass: digits(properties.pieces_rejected_2nd_cnt),
        piecesremainingsecondpass: digits(properties.pieces_remaining),
        timetocompleteactual: digits(properties.time_to_comp_actual),
        timeleftsecondpassactual: digits(properties.time_to_2nd_pass_actual),
        recomendedstartactual: properties.rec_2nd_pass_start_actual,
        completiondateTime: properties.time_to_comp_actual_DateTime,
    });
}
let dpstop_row_template =
    '<tr><td>DPS Sort Plans</td><td>{dpssortplans}</td><td></td></tr>' +
    '<tr><td><b>First Pass</b></td><td></td><td></td></tr>' +
    '<tr><td>Pieces Fed</td><td>{piecesfedfirstpass}</td><td></td></tr>' +
    '<tr><td>Pieces Rejected</td><td>{piecesrejectedfirstpass}</td><td></td></tr>' +
    '<tr><td>Pieces To Second Pass</td><td>{piecestosecondpass}</td><td></td></tr>' +
    '<tr><td>Rec. 2nd Pass Start Time</td><td>{recomendedstartactual}</td><td></td></tr>' +
    '<tr><td><b>Second Pass</b></td><td></td><td></td></tr>' +
    '<tr><td>Pieces Fed</td><td>{piecesfedsecondpass}</td><td></td></tr>' +
    '<tr><td>Pieces Rejected</td><td>{piecesrejectedsecondpass}</td><td></td></tr>' +
    '<tr><td>Pieces Remaining</td><td>{piecesremainingsecondpass}</td><td></td></tr>' +
    '<tr><td>Est. Completion Time</td><td>{completiondateTime}</td><td></td></tr>'
    ;

async function LoadMachineDetails(selcValue) {
    try {
        if (polygonMachine.hasOwnProperty("_layers")) {
            $.map(polygonMachine._layers, function (layer, i) {
                if (layer.hasOwnProperty("feature")) {
                    if (layer.feature.properties.id === selcValue) {
                        var Center = new L.latLng(
                            (layer._bounds._southWest.lat + layer._bounds._northEast.lat) / 2,
                            (layer._bounds._southWest.lng + layer._bounds._northEast.lng) / 2);
                        map.setView(Center, 3);
                        if (/Machine/i.test(layer.feature.properties.Zone_Type)) {
                            LoadMachineTables(layer.feature.properties, 'machinetable');
                        }
                        return false;
                    }
                }
            });
        }
    } catch (e) {
        console.log(e)
    }

}
$('#zoneselect').change(function (e) {
    $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
    var selcValue = this.value;
    LoadMachineDetails(selcValue);

});
function getstatebadge(properties) {
    if (properties.hasOwnProperty("MPEWatchData")) {
        if (properties.MPEWatchData.hasOwnProperty("current_run_end")) {
            //var endtime = moment(properties.MPEWatchData.current_run_end);
            var endtime = properties.MPEWatchData.current_run_end == "0" ? "" : moment(properties.MPEWatchData.current_run_end);

            var starttime = moment(properties.MPEWatchData.current_run_start);
            var sortPlan = properties.MPEWatchData.cur_sortplan;

            if (starttime._isValid && !endtime._isValid) {
                if (sortPlan != "") {
                    return "badge badge-success";
                }
                else {
                    return "badge badge-info";
                }
            }
            else if (!starttime._isValid && !endtime._isValid) {
                return "badge badge-info";
            }
            else if (starttime._isValid && endtime._isValid) {
                return "badge badge-info";
            }
        }
        else {
            return "badge badge-secondary";
        }
    }
    else {
        return "badge badge-secondary";
    }
}
function getstateText(properties) {
    if (properties.hasOwnProperty("MPEWatchData")) {
        if (properties.MPEWatchData.hasOwnProperty("current_run_end")) {
            //var endtime = moment(properties.MPEWatchData.current_run_end);
            var endtime = properties.MPEWatchData.current_run_end == "0" ? "" : moment(properties.MPEWatchData.current_run_end);
            var starttime = moment(properties.MPEWatchData.current_run_start);
            var sortPlan = properties.MPEWatchData.cur_sortplan;

            if (starttime._isValid && !endtime._isValid) {
                if (sortPlan != "") {
                    return "Running";
                }
                else {
                    return "Idle";
                }
            }
            else if (!starttime._isValid && !endtime._isValid) {
                return "Unknown";
            }
            else if (starttime._isValid && endtime._isValid) {
                return "Idle";
            }
        }
        else {
            return "No Data";
        }
    }
    else {
        return "No Data";
    }
}
async function Edit_Machine_Info(id) {
    $('#modalZoneHeader_ID').text('Edit Machine Info');

    sidebar.close('connections');

    $('button[id=machinesubmitBtn]').prop('disabled', true);
    try {
        if (polygonMachine.hasOwnProperty("_layers")) {
            let layerindex = -0;
            let Data = {};
            $.map(polygonMachine._layers, function (layer, i) {
                if (layer.hasOwnProperty("feature")) {
                    if (layer.feature.properties.id === id) {
                        Data = layer.feature.properties;
                        layerindex = layer._leaflet_id;
                        return false;
                    }
                }
            });
            if (layerindex !== -0) {

                if (!$.isEmptyObject(Data)) {
                    fotfmanager.server.getMPEList().done(function (mpedata) {
                        if (mpedata.length > 0) {
                            mpedata.sort(SortByName);
                            mpedata.push('**Machine Not Listed');
                            //$('input[id=machine_zone_name]').css('display', 'none');
                            $('#machine_manual_row').css('display', 'none');
                            $('select[id=machine_zone_select_name]').css('display', '');
                            $('select[id=machine_zone_select_name]').empty();
                            $('<option/>').val("").html("").appendTo('select[id=machine_zone_select_name]');
                            $('select[id=machine_zone_select_name]').val("");
                            $.each(mpedata, function () {
                                $('<option/>').val(this).html(this).appendTo('#machine_zone_select_name');
                            })
                            $('select[id=machine_zone_select_name]').val(Data.name.toString());
                        }
                        else {
                            $('<option/>').val("").html("").appendTo('select[id=machine_zone_select_name]');
                            $('<option/>').val("**Machine Not Listed").html("**Machine Not Listed").appendTo('select[id=machine_zone_select_name]');
                            $('select[id=machine_zone_select_name]').val("**Machine Not Listed");
                            $('#machine_manual_row').css('display', '');
                            $('select[id=machine_zone_select_name]').css('display', 'none');
                        }
                    });
                    $('select[id=machine_zone_select_name]').change(function () {
                        if ($('select[name=machine_zone_select_name] option:selected').val() == '**Machine Not Listed') {
                            $('#machine_manual_row').css('display', '');
                        }
                        else {
                            $('#machine_manual_row').css('display', 'none');
                        }
                        if ($('select[name=machine_zone_select_name] option:selected').val() == '') {
                            $('button[id=machinesubmitBtn]').prop('disabled', true);
                        }
                        else {
                            $('button[id=machinesubmitBtn]').prop('disabled', false);
                        }
                    });
                    
                    $('input[type=text][name=machine_name]').val(Data.MPE_Type);
                    $('input[type=text][name=machine_number]').val(Data.MPE_Number);
                    $('input[type=text][name=zone_ldc]').val(Data.Zone_LDC);
                    $('input[type=text][name=machine_id]').val(Data.id);

                    $('button[id=machinesubmitBtn]').off().on('click', function () {
                        try {
                            var machineName = "";
                            var machineNumber = "";
                            $('button[id=machinesubmitBtn]').prop('disabled', true);
                            if ($('select[name=machine_zone_select_name] option:selected').val() != '**Machine Not Listed') {
                                var selectedMachine = $('select[name=machine_zone_select_name] option:selected').val().split("-");
                                machineName = selectedMachine[0];
                                machineNumber = selectedMachine[1];
                            }
                            else {
                                machineName = $('input[type=text][name=machine_name]').val();
                                machineNumber = $('input[type=text][name=machine_number]').val();
                            }

                            var jsonObject = {
                                MPE_Type: machineName,//$('input[type=text][name=machine_name]').val(),
                                MPE_Number: machineNumber,//$('input[type=text][name=machine_number]').val(),
                                Zone_LDC: $('input[type=text][name=zone_ldc]').val(),
                                floorId: baselayerid
                            };

                            if (!$.isEmptyObject(jsonObject)) {
                                jsonObject.id = Data.id;
                                fotfmanager.server.editZone(JSON.stringify(jsonObject)).done(function (updatedData) {

                                    $('span[id=error_machinesubmitBtn]').text(updatedData[0].properties.MPE_Type + " Zone has been Updated.");
                                    setTimeout(function () { $("#Zone_Modal").modal('hide'); }, 1500);

                                });
                            }
                        } catch (e) {
                            $('span[id=error_machinesubmitBtn]').text(e);
                        }
                    });
                    $('#Zone_Modal').modal();
                }
                else {
                    $('label[id=error_machinesubmitBtn]').text("Invalid Zone ID");
                    $('#Zone_Modal').modal();
                }
            }
            else {
                $('label[id=error_machinesubmitBtn]').text("Invalid Zone ID");
                $('#Zone_Modal').modal();
            }
        }
    } catch (e) {
        console.log(e);
    }
}
function enablezoneSubmit() {
    //AGV connections
    if ($('input[type=text][name=machine_name]').hasClass('is-valid') &&
        $('input[type=text][name=machine_number]').hasClass('is-valid')
    ) {
        $('button[id=machinesubmitBtn]').prop('disabled', false);
    }
    else {
        $('button[id=machinesubmitBtn]').prop('disabled', true);
    }
}
function GetMacineBackground(mpeWatchData, starttime) {
    var OkColor = '#3573b1';
    var WarningColor = '#ffc107';
    var AlertColor = '#dc3545';
    var bkColor = OkColor;
    try {
        var throughput_status = mpeWatchData.hasOwnProperty("throughput_status") ? mpeWatchData.throughput_status : "0";
        var unplan_maint_sp_status = mpeWatchData.hasOwnProperty("unplan_maint_sp_status") ? mpeWatchData.unplan_maint_sp_status : "0";
        var op_started_late_status = mpeWatchData.hasOwnProperty("op_started_late_status") ? mpeWatchData.op_started_late_status : "0";
        var op_running_late_status = mpeWatchData.hasOwnProperty("op_running_late_status") ? mpeWatchData.op_running_late_status : "0";
        var sortplan_wrong_status = mpeWatchData.hasOwnProperty("sortplan_wrong_status") ? mpeWatchData.sortplan_wrong_status : "0";

        var curtime = moment().format('YYYY-MM-DD HH:mm:ss');
        if (!$.isEmptyObject(timezone)) {
            if (timezone.hasOwnProperty("Facility_TimeZone")) {
                curtime = moment().tz(timezone.Facility_TimeZone).format('YYYY-MM-DD HH:mm:ss');
            }
        }
        var dt = moment(curtime);
        var st = moment(starttime);
        var timeduration = moment.duration(dt.diff(st));
        var minutes = parseInt(timeduration.asMinutes());
        if (minutes > 15) {
            if (throughput_status == "3") {
                return AlertColor;
            }
            else if (throughput_status == "2") {
                bkColor = WarningColor;
            }
        }
        if (unplan_maint_sp_status == "2" || op_started_late_status == "2" || op_running_late_status == "2" || sortplan_wrong_status == "2") {
            return AlertColor;
        }
        if (unplan_maint_sp_status == "1" || op_started_late_status == "1" || op_running_late_status == "1" || sortplan_wrong_status == "1") {
            return WarningColor;
        }
        return bkColor;
    }
    catch (e) {
        console.log(e);
    }

}
function FormatMachineRowColors(mpeWatchData, starttime) {
    var Throughput_tr_style = document.getElementById('Throughput_tr').style;
    var SortPlan_tr_style = document.getElementById('SortPlan_tr').style;
    var StartTime_tr_style = document.getElementById('StartTime_tr').style;
    var EstComp_tr_style = document.getElementById('EstComp_tr').style;
    var rowAlertColor = "rgba(220, 53, 69, 0.5)";
    var rowWarningColor = "rgba(255, 193, 7, 0.5)";
    try {
        var throughput_status = mpeWatchData.hasOwnProperty("throughput_status") ? mpeWatchData.throughput_status : "0";
        var unplan_maint_sp_status = mpeWatchData.hasOwnProperty("unplan_maint_sp_status") ? mpeWatchData.unplan_maint_sp_status : "0";
        var op_started_late_status = mpeWatchData.hasOwnProperty("op_started_late_status") ? mpeWatchData.op_started_late_status : "0";
        var op_running_late_status = mpeWatchData.hasOwnProperty("op_running_late_status") ? mpeWatchData.op_running_late_status : "0";
        var sortplan_wrong_status = mpeWatchData.hasOwnProperty("sortplan_wrong_status") ? mpeWatchData.sortplan_wrong_status : "0";
        var curtime = moment().format('YYYY-MM-DD HH:mm:ss');
        if (!$.isEmptyObject(timezone)) {
            if (timezone.hasOwnProperty("Facility_TimeZone")) {
                curtime = moment().tz(timezone.Facility_TimeZone).format('YYYY-MM-DD HH:mm:ss');
            }
        }
        var dt = moment(curtime);
        var st = moment(starttime);
        var timeduration = moment.duration(dt.diff(st));
        var minutes = parseInt(timeduration.asMinutes());
        if (minutes > 15) {
            if (Throughput_tr_style != null) {
                if (throughput_status == "3") {
                    Throughput_tr_style.backgroundColor = rowAlertColor;
                }
                else if (throughput_status == "2") {
                    Throughput_tr_style.backgroundColor = rowWarningColor;
                }
                //else {
                //    Throughput_tr_style.backgroundColor = "";
                //}
            }
            //else {
            //    Throughput_tr_style.backgroundColor = "";
            //}
        }

        if (StartTime_tr_style != null) {
            if (op_started_late_status == "2") {
                StartTime_tr_style.backgroundColor = rowAlertColor;
            }
            else if (op_started_late_status == "1") {
                StartTime_tr_style.backgroundColor = rowWarningColor;
            }
            else {
                //StartTime_tr_style.backgroundColor = "";
            }
        }

        if (EstComp_tr_style != null) {
            if (op_running_late_status == "2") {
                EstComp_tr_style.backgroundColor = rowAlertColor;
            }
            else if (op_running_late_status == "1") {
                EstComp_tr_style.backgroundColor = rowWarningColor;
            }
            else {
                //EstComp_tr_style.backgroundColor = "";
            }
        }

        if (SortPlan_tr_style != null) {
            if (unplan_maint_sp_status == "2" || sortplan_wrong_status == "2") {
                SortPlan_tr_style.backgroundColor = rowAlertColor;
            }
            else if (unplan_maint_sp_status == "1" || sortplan_wrong_status == "1") {
                SortPlan_tr_style.backgroundColor = rowWarningColor;
            }
            else {
                //SortPlan_tr_style.backgroundColor = "";
            }
        }

    }
    catch (e) {
        console.log(e);
    }
}
function GetMachinePerfGraph(dataproperties) {
    var xValues = [];
    var yValues = [];
    var total = 0;
    for (var i = 0; i < dataproperties.MPEWatchData.hourly_data.length; i++) {
        xValues.unshift(dataproperties.MPEWatchData.hourly_data[i].count);
        total += dataproperties.MPEWatchData.hourly_data[i].count;
        yValues.unshift(dataproperties.MPEWatchData.hourly_data[i].hour);
    }
    if (total > 0) {
        new Chart("machinechart", {
            type: "line",
            data: {
                labels: yValues,
                datasets: [{
                    fill: false,
                    lineTension: 0,
                    backgroundColor: "rgba(0,0,255,1.0)",
                    borderColor: "rgba(0,0,255,0.1)",
                    data: xValues
                }]
            },
            options: {
                legend: { display: false },
                title: {
                    display: true,
                    text: "Pieces Fed 24 Hours"
                },
                tooltips: {
                    callbacks: {
                        title: function (tooltipItem, data) {
                            var hour = ("0" + new Date(tooltipItem[0].xLabel).getHours().toString()).slice(-2);
                            return tooltipItem[0].xLabel.toString() + "-" + hour + ":59";
                        },

                        label: function (tooltipItem, data) {
                            return tooltipItem.yLabel.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + " pieces";
                        },
                    },
                },
                scales: {
                    xAxes: [{
                        gridLines: {
                            display: true
                        },
                        ticks: {
                            autoSkip: true,
                            maxTicksLimit: 12,
                            callback: function (value, index, values) {
                                //var hour = ("0" + new Date(value).getHours().toString()).slice(-2);
                                //return hour + ":00-" + hour + ":59";
                                var hour = (new Date(value).getHours().toString()).slice(-2);
                                return hour + ":00";
                            }
                        }
                    }],
                    yAxes: [{
                        ticks: {
                            beginAtZero: true,
                            callback: function (value, index, values) {
                                if (parseInt(value) >= 1000) {
                                    return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                                } else {
                                    return value;
                                }
                            }
                        }
                    }]
                }
            }
        });
    }
    else {
        var mpgtrStyle = document.getElementById('machineChart_tr').style;
        mpgtrStyle.display = 'none';
    }
}


/*
*this is for the Vehicle data load 
*/

$.extend(fotfmanager.client, {
    updateVehicleTagStatus: async (vehicleupdate,id) => { updateVehicleTag(vehicleupdate, id) }
});

async function updateVehicles(vehicleupadtes, id) {
    for (const _vehicle of vehicleupdates) {
        updateVehicleTag(vehicleupdate, id);
    }
}
async function updateVehicleTag(vehicleupdate, id) {
    try {
        if (id === baselayerid) {
            var layerindex = -0;
            map.whenReady(() => {
                if (map.hasOwnProperty("_layers")) {
                    $.map(map._layers, function (layer, i) {
                        if (layer.hasOwnProperty("feature")) {
                            if (layer.feature.properties.id === vehicleupdate.properties.id) {
                                layerindex = layer._leaflet_id;
                                layer.feature.geometry = vehicleupdate.geometry.coordinates;
                                layer.feature.properties = vehicleupdate.properties;
                                Promise.all([updateVehicleLocation(layerindex)]);
                                return false;
                            }
                        }
                    });
                }
                if (layerindex !== -0) {
                    if ($('div[id=vehicle_div]').is(':visible') && $('div[id=vehicle_div]').attr("data-id") === vehicleupdate.properties.id) {
                        updateVehicleInfo(layerindex);
                    }
                }
                else {
                    if (vehicleupdate.properties.Tag_Type === "Vehicle") {
                        piv_vehicles.addData(vehicleupdate);
                    }
                    if (vehicleupdate.properties.Tag_Type === "Autonomous Vehicle") {
                        agv_vehicles.addData(vehicleupdate);
                    }
                }
            });
        }
    } catch (e) {
        console.log(e);
    }
}
let piv_vehicles = new L.GeoJSON(null, {
    pointToLayer: function (feature, latlng) {
        let vehicleIcon = L.divIcon({
            id: feature.properties.id,
            className: get_pi_icon(feature.properties.name, feature.properties.Tag_Type) + ' iconXSmall',
            html: '<i>' +
                '<span class="path1"></span>' +
                '<span class="path2"></span>' +
                '<span class="path3"></span>' +
                '<span class="path4"></span>' +
                '<span class="path5"></span>' +
                '<span class="path6"></span>' +
                '<span class="path7"></span>' +
                '<span class="path8"></span>' +
                '<span class="path9"></span>' +
                '<span class="path10"></span>' +
                '</i>'
        });
        return L.marker(latlng, {
            icon: vehicleIcon,
            title: feature.properties.name,
            riseOnHover: true,
            bubblingMouseEvents: true,
            popupOpen: true
        })
    },
    onEachFeature: function (feature, layer) {
        let obstructedState = '';
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        updateVehicleDropdownClick(feature, layer, false);
        if (feature.properties.hasOwnProperty('state')) {
            let new_state = Get_Vehicle_Status(feature.properties.Vehicle_Status_Data.STATE.replace(/VState/ig, ""));
            if (/Obstructed|(Error)$/i.test(new_state)) {
                obstructedState = 'obstructedflash';
            }
        }
        layer.bindTooltip(feature.properties.name.replace(/[^0-9.]/g, '').replace(/^0+/, ''), {
            permanent: true,
            interactive: true,
            direction: 'top',
            opacity: 0.9,
            className: 'vehiclenumber ' + obstructedState
        }).openTooltip();
    }
});

let agv_vehicles = new L.GeoJSON(null, {
    pointToLayer: function (feature, latlng) {
        let vehicleIcon = L.divIcon({
            id: feature.properties.id,
            className: get_pi_icon(feature.properties.name, feature.properties.Tag_Type) + ' iconXSmall',
            html: '<i>' +
                '<span class="path1"></span>' +
                '<span class="path2"></span>' +
                '<span class="path3"></span>' +
                '<span class="path4"></span>' +
                '<span class="path5"></span>' +
                '<span class="path6"></span>' +
                '<span class="path7"></span>' +
                '<span class="path8"></span>' +
                '<span class="path9"></span>' +
                '<span class="path10"></span>' +
                '</i>'
        });
        return L.marker(latlng, {
            icon: vehicleIcon,
            title: feature.properties.name,
            riseOnHover: true,
            bubblingMouseEvents: true,
            popupOpen: true
        })
    },
    onEachFeature: function (feature, layer) {
        var obstructedState = '';
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        updateVehicleDropdownClick(feature, layer, false);
        if (feature.properties.Vehicle_Status_Data !== null) {
            var new_state = Get_Vehicle_Status(feature.properties.Vehicle_Status_Data.STATE.replace(/VState/ig, ""));
            if (/Obstructed|(Error)$/i.test(new_state)) {
                obstructedState = 'obstructedflash';
            }
        }
        layer.bindTooltip(feature.properties.name.replace(/[^0-9.]/g, '').replace(/^0+/, ''), {
            permanent: true,
            interactive: true,
            direction: 'top',
            opacity: 0.9,
            className: 'vehiclenumber ' + obstructedState
        }).openTooltip();
    }
});
function updateVehicleLocation(layerindex) {
    if (map._layers[layerindex].feature.geometry.length > 0) {
        var newLatLng = new L.latLng(map._layers[layerindex].feature.geometry[1] * .000793750, map._layers[layerindex].feature.geometry[0] * 0.000793750);
        var distanceTo = (newLatLng.distanceTo(map._layers[layerindex].getLatLng()).toFixed(0) / 1000);
        if (Math.round(distanceTo) > 4000) {
            map._layers[layerindex].setLatLng(newLatLng);
        }
        else {
            map._layers[layerindex].slideTo(newLatLng, { duration: 3000 });
        }
    }
    if (map._layers[layerindex].feature.properties.Vehicle_Status_Data !== null) {
        var new_state = Get_Vehicle_Status(map._layers[layerindex].feature.properties.Vehicle_Status_Data.STATE.replace(/VState/ig, ""));
        if (/Obstructed|(Error)$/i.test(new_state)) {
            if (map._layers[layerindex].hasOwnProperty("_icon")
                && map._layers[layerindex].hasOwnProperty("_tooltip")
                && map._layers[layerindex]._tooltip.hasOwnProperty("_container")) {
                if (!map._layers[layerindex]._tooltip._container.classList.contains('obstructedflash')) {
                    map._layers[layerindex]._tooltip._container.classList.add('obstructedflash');
                }

            }
        } else {
            if (map._layers[layerindex].hasOwnProperty("_tooltip") && map._layers[layerindex]._tooltip.hasOwnProperty("_container")) {
                if (map._layers[layerindex]._tooltip._container.classList.contains('obstructedflash')) {
                    map._layers[layerindex]._tooltip._container.classList.remove('obstructedflash');
                }
            }
        }
    }
    return null;
}
function updateVehicleInfo(layerindex) {
    try {
        if ($('input[type=checkbox][name=followvehicle]')[0].checked) {
            map.panTo(new L.LatLng(map._layers[layerindex].feature.geometry[1], map._layers[layerindex].feature.geometry[0]), 4);
        }

        if (/^AGV/i.test(map._layers[layerindex].feature.properties.name)) {
            AGVStausUpdate(layerindex);
        }

    } catch (e) {
        console.log(e);
    }
}
async function AGVStausUpdate(layerindex)
{
    try {        
        if (map._layers[layerindex].feature.properties.Vehicle_Status_Data !== null) {
            var new_state = Get_Vehicle_Status(map._layers[layerindex].feature.properties.Vehicle_Status_Data.STATE.replace(/VState/ig, ""));

                var current_state = $("button[id=" + map._layers[layerindex].feature.properties.name + "][name=vehicle]").text();
                if (new_state !== current_state) {
                    $("button[id=" + map._layers[layerindex].feature.properties.name + "][name=vehicle]").text(new_state);
                }
            var new_btn_category = Get_Vehicle_Catagory(map._layers[layerindex].feature.properties.Vehicle_Status_Data.CATEGORY.toString());
                var current_btn_category = Get_Current_Class($("button[id=" + map._layers[layerindex].feature.properties.name + "][name=vehicle]").attr("class"));
                if (new_btn_category !== current_btn_category) {
                    $("button[id=" + map._layers[layerindex].feature.properties.name + "][name=vehicle]").addClass(new_btn_category).removeClass(current_btn_category);
                }
                var batint = parseInt($("span[name=" + map._layers[layerindex].feature.properties.name + "_batter_level" + "]").attr("data-batter_lvl"));
                if (!map._layers[layerindex].feature.properties.hasOwnProperty("vehicleBatteryPercent")) {
                    map._layers[layerindex].feature.properties.vehicleBatteryPercent = 0;
                }
            if (parseInt(map._layers[layerindex].feature.properties.Vehicle_Status_Data.BATTERYPERCENT) !== batint) {
                    $("span[name=" + map._layers[layerindex].feature.properties.name + "_batter_level" + "]").text(map._layers[layerindex].feature.properties.Vehicle_Status_Data.BATTERYPERCENT + " % Charged").attr("data-batter_lvl", map._layers[layerindex].feature.properties.Vehicle_Status_Data.BATTERYPERCENT);
                    $("div[name=" + map._layers[layerindex].feature.properties.name + "_progressbar" + "]").attr("aria-valuenow", map._layers[layerindex].feature.properties.name).css("width", map._layers[layerindex].feature.properties.Vehicle_Status_Data.BATTERYPERCENT + "%");
                    new_btn_category = Get_Vehicle_Progress(map._layers[layerindex].feature.properties.Vehicle_Status_Data.BATTERYPERCENT);
                    current_btn_category = Get_Current_Class($("div[name=" + map._layers[layerindex].feature.properties.name + "_progressbar" + "]").attr("class"));
                    if (new_btn_category !== current_btn_category) {
                        $("div[name=" + map._layers[layerindex].feature.properties.name + "_progressbar" + "]").addClass(new_btn_category).removeClass(current_btn_category);
                    }
                }
            
            if (map._layers[layerindex].feature.properties.Mission !== null) {
                if ($('#vehicletagid').attr("data-id") === map._layers[layerindex].feature.properties.id) {
                    vehiclemission_Table_Body.empty();
                    if (!$.isEmptyObject(map._layers[layerindex].feature.properties.Mission)) {
                        vehiclemission_Table_Body.append(agvmissionrow_template.supplant(formatvehiclemissionrow(map._layers[layerindex].feature.properties.Mission)));

                    }
                }
            }
            else {
                if (vehiclemission_Table_Body.find("tr[id=inmission]").length > 0) {
                    vehiclemission_Table_Body.empty();
                }
               
            }
            //if (/^Charg/i.test(vehicleupdate.properties.state)) {
            //    var ChargeIcon = L.Icon.extend({
            //        options: {
            //            iconUrl: '<i class="bi-ligthning">',
            //            iconSize: [32, 32],
            //            iconAnchor: [10, 0],
            //            popupAnchor: [5, 5],
            //        }
            //    })
            //    vehicles._layers[layerindex].setIcon(new ChargeIcon);
            //}
            //else {
            //    if (/lightning/i.test(vehicles._layers[layerindex].options.icon.options.iconUrl)) {
            //        var ChargeIcon = L.Icon.extend({
            //            options: {
            //                iconSize: [32, 32],
            //                iconAnchor: [10, 0],
            //                popupAnchor: [5, 5],
            //                iconUrl: 'Content/icons/' + get_icon(vehicles._layers[layerindex].feature.properties.name, vehicles._layers[layerindex].feature.properties.Tag_Type)
            //            }
            //        })
            //        vehicles._layers[layerindex].setIcon(new ChargeIcon);
            //    }
            //}
        }
    } catch (e) {
        console.log(e);
    }
}
async function LoadVehicleTable(dataproperties, updateTagName) {
    try {
        $zoneSelect[0].selectize.setValue(-1, true);
        vehicletop_Table_Body.empty();
        vehiclemission_Table_Body.empty();
        hideSidebarLayerDivs();
        $zoneSelect[0].selectize.setValue(dataproperties.id, true);
        $('div[id=vehicle_div]').attr("data-id", dataproperties.id);
        $('div[id=vehicle_div]').css('display', 'block');


        if (!/AGV/i.test(dataproperties.name)) {
            vehicletop_Table_Body.append(pivrow_template.supplant(formatvehicleinforow(dataproperties)));
        }

        if (/^AGV/i.test(dataproperties.name)) {
            vehicletop_Table_Body.append(agvrow_template.supplant(formatvehicleinforow(dataproperties)));
            vehiclemission_Table_Body.empty();
            if (!$.isEmptyObject(dataproperties.Mission)) {
                vehiclemission_Table_Body.append(agvmissionrow_template.supplant(formatvehiclemissionrow(dataproperties.Mission)));

            }
        }
        if (updateTagName) {
            $zoneSelect[0].selectize.updateOption(dataproperties.id, { value: dataproperties.id, text: dataproperties.name });
        }
    } catch (e) {
        console.log(e);
    }
}
let pivrow_template = '<tr data-id="{TagId}" id="vehicletagid">' +
    '<td >Vehicle Name:</td>' +
    '<td class="text-left" id="vehicletag_name">{name}</td>' +
    '<td>Tag ID:</td>' +
    '<td  id="vehicletag_tagid">{TagId}</td>' +
    '</tr>' +
    '</td>' +
    '</tr>"';
let vehicletop_Table = $('table[id=vehicletable]');
let vehicletop_Table_Body = vehicletop_Table.find('tbody');

let agvrow_template = '<tr data-id="{TagId}" id="vehicletagid"><td>Vehicle Name:</td><td id="vehicletag_name" class="text-left">{name}</td><td  >Tag ID:</td><td  id="vehicletag_tagid">{TagId}</td></tr>' +
    '<tr><td>State:</td><td colspan="3"><button id="{name}" name="vehicle" class="btn btn-block btn-sm {vehiclestateCategory} vehiclehistory" style="margin-bottom: 2px;" value="{vehiclestate}">{vehiclestate}</button></td></tr>"' +
    '<tr><td>Battery:</td><td colspan="3">' +
    '<div class="progress">' +
    '<div class="progress-bar btn-sm {vehicleBatteryProgressBar}"  name="{name}_progressbar" role="progressbar" aria-valuenow="{batterylevelnum}" aria-valuemin="0" aria-valuemax="100" style="width: {batterylevelnum}%;">' +
    '<span name="{name}_batter_level" style="color: black; position: absolute;" data-batter_lvl="{batterylevelnum}">{batterylevelnum} % Charged</span>' +
    '</div>' +
    '</div>' + '</td></tr>"';

let agvmissionrow_template = '<tr class="text-center" id=inmission>' +
    '<td class="font-weight-bold" colspan="2">' +
        '<h6 class="control-label sectionHeader ml-1 mb-1 d-flex justify-content-between">' +
        'Mission Status' +
        '<span class="d-flex justify-content-between">' +
        'Request ID:' +
        '<span class="btn btn-secondary border-0 badge-dark badge">{RequestId}</span>  ' +
        '</span>' +
        '</h6 >' +
    '</td>' +
    '<tr id=pickupLocation_{TagId}><td>Pickup Location:</td><td>{PickupLocation}</td></tr>' +
    '<tr id=pickupLocationEta_{TagId}><td>ETA to Pickup:</td><td>{PickupLocationEta}</td></tr>' +
    '<tr id=dropoffLocation_{TagId}><td>Drop-Off Location:</td><td>{DropoffLocation}</td></tr>' +
    '<tr id=endLocation_{TagId}><td>End Location:</td><td>{EndLocation}</td></tr>' +
    '<tr id=Door_{TagId}><td>Door:</td><td>{Door}</td></tr>' +
    '<tr><td>Placard:</td><td>{Placard}</td></tr>' +
    '</tr>';
let vehiclemission_Table = $('table[id=vehiclemissiontable]');
let vehiclemission_Table_Body = vehiclemission_Table.find('tbody');
function formatvehicleinforow(properties) {
    return $.extend(properties, {
        TagId: properties.id,
        name: properties.name,
        batterylevelnum: properties.Vehicle_Status_Data !== null ? properties.Vehicle_Status_Data.BATTERYPERCENT : 0,
        vehiclenumber: properties.Vehicle_Status_Data !== null ? properties.name.replace(/[^0-9.]/g, '').replace(/^0+/, '') : "",
        vehiclestate: properties.Vehicle_Status_Data !== null ? Get_Vehicle_Status(properties.Vehicle_Status_Data.STATE.replace(/VState/ig, "")) : "N/A",
        vehicleBatteryProgressBar: Get_Vehicle_Progress(properties.Vehicle_Status_Data !== null ? properties.Vehicle_Status_Data.BATTERYPERCENT : 0),
        vehiclestateCategory: properties.Vehicle_Status_Data !== null ? Get_Vehicle_Catagory(properties.Vehicle_Status_Data.CATEGORY.toString()) : "btn-outline-info",
        vehiclestatecolor: properties.Vehicle_Status_Data !== null ? properties.Vehicle_Status_Data.STATE.replace(/VState/ig, "") : "red"
    });
}
function formatvehiclemissionrow(properties) {
    return $.extend(properties, {
        RequestId: properties.Request_Id,
        PickupLocation: properties.hasOwnProperty("Pickup_Location") ? Get_location_Code(properties.Pickup_Location) : "N/A",
        DropoffLocation: properties.hasOwnProperty("Dropoff_Location") ? Get_location_Code(properties.Dropoff_Location) : "N/A",
        EndLocation: properties.hasOwnProperty("End_Location") ? Get_location_Code(properties.End_Location) : "N/A",
        PickupLocationEta: properties.hasOwnProperty("ETA") ? properties.ETA : "N/A",
        Placard: properties.hasOwnProperty("Placard") ? properties.Placard : "N/A"
    });
}
function Get_Vehicle_Catagory(category) {
    if (category === "btn-outline-info") {
        return "btn-success";
    }
    if (category === "1") {
        return "btn-success";
    }
    if (category === "2") {
        return "btn-outline-info";
    }
    if (category === "3") {
        return "btn-outline-primary";
    }
    if (category === "4") {
        return "btn-danger";
    }
    if (category === "") {
        return "btn-outline-info";
    }
    if (category === undefined) {
        return "btn-outline-info";
    }
}
function Get_Vehicle_Status(status) {
    if (/Idle/i.test(status)) {
        return capitalize_Words("Available");
    }
    if (/PathResume|blocked/i.test(status)) {
        return capitalize_Words("Obstructed");
    }
    if (/PathFollow|Working|PathBuild/i.test(status)) {
        return capitalize_Words(status.replace(''.toUpperCase(), ''));
    }
    if (/Shutdown|Blocked/i.test(status)) {
        return capitalize_Words(status.replace(''.toUpperCase(), ''));
    }
    if (/ChargeStart/i.test(status)) {
        return capitalize_Words("Charging");
    }
    if (/Charging/i.test(status)) {
        return capitalize_Words("Charging");
    }
    if (/NO_STATUS_RCVD|NoComm|Unknown/i.test(status)) {
        return capitalize_Words("No Status");
    }
    if (!/NO_STATUS_RCVD|Shutdown|Charging|PathFollow|Idle|ChargeStart|Working|Blocked/i.test(status)) {
        return capitalize_Words(status.replace(''.toUpperCase(), ''), '');
    }

    return capitalize_Words(status);
}
function Get_Current_Class(classList) {
    if (checkValue(classList)) {
        var classArr = classList.split(/\s+/);
        var btn_val = "";
        $.each(classArr, function (index, value) {
            if (/^btn-outline/i.test(value)) {
                btn_val = value;
            }
            if (/^btn-success/i.test(value)) {
                btn_val = value;
            }
            if (/^btn-info/i.test(value)) {
                btn_val = value;
            }
            if (/^btn-danger/i.test(value)) {
                btn_val = value;
            }
            if (/^btn-secondary/i.test(value)) {
                btn_val = value;
            }
            if (/^btn-warning/i.test(value)) {
                btn_val = value;
            }
            if (/^btn-light/i.test(value)) {
                btn_val = value;
            }
            if (/^btn-dark/i.test(value)) {
                btn_val = value;
            }
            if (/^btn-link/i.test(value)) {
                btn_val = value;
            }
            if (/^bg-success/i.test(value)) {
                btn_val = value;
            }
            if (/^bg-info/i.test(value)) {
                btn_val = value;
            }
            if (/^bg-warning/i.test(value)) {
                btn_val = value;
            }
            if (/^bg-danger/i.test(value)) {
                btn_val = value;
            }
        });
        return btn_val;
    }
    else {
        return "";
    }
}
function Get_Vehicle_Progress(Level) {
    if (Level < 20) {
        return "bg-danger";
    }
    if (Level >= 20 && Level <= 50) {
        return "bg-warning";
    }
    if (Level >= 51) {
        return "bg-info";
    }
}
function get_pi_icon(name, type) {
    if (/Vehicle$/i.test(type)) {
        if (checkValue(name)) {
            if (/^(wr|walkingrider)/i.test(name)) {
                return "pi-iconLoader_wr ml--24";
            }
            if (/^(fl|forklift)/i.test(name)) {
                return "pi-iconLoader_forklift ml--8";
            }
            if (/^(t|tug|mule)/i.test(name)) {
                return "pi-iconLoader_tugger ml--16";
            }
            if (/^agv_t/i.test(name)) {
                return "pi-iconLoader_avg_t ml--8";
            }
            if (/^agv_p/i.test(name)) {
                return "pi-iconLoader_avg_pj ml--16";
            }
            if (/^ss/i.test(name)) {
                return "pi-iconVh_ss ml--16";
            }
            if (/^bf/i.test(name)) {
                return "pi-iconVh_bss ml--16";
            }
            if (/^Surfboard/i.test(name)) {
                return "pi-iconSurfboard ml--32";
            }
            return "pi-iconVh_ss ml--16";
        }
        else {
            return "pi-iconVh_ss ml--16";
        }
    }
    else {
        return "pi-iconVh_ss ml--16";
    }
}
//async function init_agvtags() {
//    //Get AGV Location list
//    fotfmanager.server.getVehicleTagsList().done(function (Data) {
//        if (Data.length > 0) {
//            $.each(Data, function () {
//                updateVehicleTag(this);
//            })
//            fotfmanager.server.joinGroup("VehiclsMarkers");
//        }
//    });
//}

async function Edit_Tag_Name(tagId, tagName) {
    $('#TagName_Modal_Header_ID').text('Edit Tag Name');
    sidebar.close('connections');
    $('#TagName_Modal').modal();
    $('#modal_tag_id').html(tagId);
    $('#edit_tag_name').val(tagName);

    $('#edittagsubmitBtn').off().on("click", function () {
        Edit_Tag_Name_Submit();
    });
}

/* known defects in tag rename - 
 * renaming no battery tag needs a page reload to update name
 * renaming a moving tag with an _ after (possibly other invalid names)
 * causes the circular tag marker to stop in its place, and the vehicle
 * continues to move without its circular marker
 */
async function Edit_Tag_Name_Submit() {
    let tagId = $('#modal_tag_id').html();
    let tagName = $('#edit_tag_name').val();

    fotfmanager.server.updateTagName(tagId, tagName).done(function (data) {
        let response = JSON.parse(data);
        if (response.status == "updated") {
            $("#error_edittagsubmitBtn").html("");
            $("#TagName_Modal").modal("toggle");

            updateVehicleDropdownForTagChange(tagId, tagName);
        }
        else {
            $("#error_edittagsubmitBtn").html("Error updating tag.");
        }
    });
}


$(function () {
    if ( ! /^Admin/i.test(User.Role) ) {
        $("#vehicle-info-edit-row").remove();
    }
    $('button[name=vehicleinfoedit]').off().on('click', function () {
        if (/^Admin/i.test(User.Role)) {
            /* close the sidebar */
            let tagId = $("#vehicletag_tagid").html().trim();
            let tagName = $("#vehicletag_name").html().trim();
            sidebar.close();
            if (checkValue(tagId)) {
                Edit_Tag_Name(tagId, tagName);
            }
        }
    });
});

$(function () {

})

function updateVehicleDropdownForTagChange(tagId, tagName) {
    $.map(agv_vehicles._layers, (mappedLayer, i) => {
        if (mappedLayer.feature.properties.id == tagId) {

            
            let layer = agv_vehicles._layers[i];
            updateVehicleDropdownClick(layer.feature, layer,
                true);
            layer.feature.properties.name = tagName;

        }
    });

    $.map(piv_vehicles._layers, (mappedLayer, i) => {
        if (mappedLayer.feature.properties.id == tagId) {

            let layer = piv_vehicles._layers[i];

            updateVehicleDropdownClick(layer.feature, layer,
                true);
            layer.feature.properties.name = tagName;
        }
    });
}

function updateVehicleDropdownClick(feature, layer, updateTagName) {

    
    
    layer.off().on('click', async function (e) {
        $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
        map.setView(e.latlng, 4);

        if ((' ' + document.getElementById('sidebar').className + ' ').indexOf(' ' + 'collapsed' + ' ') <= -1) {
            if ($('#zoneselect').val() == feature.properties.id) {
                sidebar.close('home');
            }
            else {
                sidebar.open('home');
                
            }
        }
        else {
            sidebar.open('home');
            
        }
        LoadVehicleTable(feature.properties, updateTagName);
        
    })
}
/*
this is for the person details.
 */
$.extend(fotfmanager.client, {
    updatePersonTagStatus: async (tagupdate,id) => { updatePersonTag(tagupdate, id) }
});
async function updatePersonTag(tagpositionupdate,id) {
    try {
        if (id == baselayerid) {
            if (tagsMarkersGroup.hasOwnProperty("_layers")) {


                var layerindex = -0;
                $.map(tagsMarkersGroup._layers, function (layer) {

                    if (layer.hasOwnProperty("feature")) {
                        if (layer.feature.properties.id === tagpositionupdate.properties.id) {
                            layer.feature.properties = tagpositionupdate.properties;
                            layer.feature.geometry = tagpositionupdate.geometry.coordinates;
                            layerindex = layer._leaflet_id;

                            if (layer.feature.properties.tacs != null) {
                                if (tagpositionupdate.properties.tacs != null) {
                                    layer.feature.properties.tacs.isOvertime = true;
                                }
                            }


                            Promise.all([updateTagLocation(layerindex)]);
                            

                            return false;
                        }
                    }
                });
                if (layerindex === -0) {
                    tagsMarkersGroup.addData(tagpositionupdate);
                }
            }
        }

    } catch (e) {
        console.log(e);
    }
}
var tagsMarkersGroup = new L.GeoJSON(null, {
    pointToLayer: function (feature, latlng) {
        return new L.circleMarker(latlng, {
            radius: 8,
            opacity: 0,
            fillOpacity: 0,
            className: 'marker-person'
        })
    },
    onEachFeature: function (feature, layer) {
        var VisiblefillOpacity = feature.properties.tagVisibleMils < 80000 ? "" : "tooltip-hidden";
        var isOT = false;
        var isOTAuth = false;
        var classname = 'persontag ' + VisiblefillOpacity;
        if (feature.properties.tacs != null) {
            isOT = feature.properties.tacs.isOvertime;
            isOTAuth = feature.properties.tacs.isOvertimeAuth
            if (isOT && isOTAuth) {
                classname = 'persontag_otAuth ' + VisiblefillOpacity;
            }
            else if (isOT && !isOTAuth) {
                classname = 'persontag_otNotAuth ' + VisiblefillOpacity;
            }
        }
        
        layer.bindTooltip("", {
            permanent: true,
            interactive: true,
            direction: 'center',
            opacity: 1,
            //className: 'persontag ' + VisiblefillOpacity
            className: classname
        }).openTooltip();
    },
    filter: function (feature, layer) {
        return feature.properties.tagVisible;
    }
})
async function updateTagLocation(layerindex)
{
    try {
        if (tagsMarkersGroup._layers[layerindex].feature.properties.tacs != null) {
            if (tagsMarkersGroup._layers[layerindex].feature.properties.tacs.isOvertime
                && tagsMarkersGroup._layers[layerindex].feature.properties.tacs.isOvertimeAuth
                && !tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('persontag_otAuth')) {
                if (tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('persontag')) { tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.remove('persontag'); }
                if (tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('persontag_otNotAuth')) { tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.remove('persontag_otNotAuth'); }
                tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.add('persontag_otAuth');
            }
            else if (tagsMarkersGroup._layers[layerindex].feature.properties.tacs.isOvertime
                && !tagsMarkersGroup._layers[layerindex].feature.properties.tacs.isOvertimeAuth
                && !tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('persontag_otNotAuth')) {
                if (tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('persontag')) { tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.remove('persontag'); }
                if (tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('persontag_otAuth')) { tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.remove('persontag_otAuth'); }
                tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.add('persontag_otNotAuth');
            }
            else if (!tagsMarkersGroup._layers[layerindex].feature.properties.tacs.isOvertime
                && !tagsMarkersGroup._layers[layerindex].feature.properties.tacs.isOvertimeAuth){
                if (tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('persontag_otNotAuth')) { tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.remove('persontag'); }
                if (tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('persontag_otAuth')) { tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.remove('persontag_otAuth'); }
                tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.add('persontag');
            }

        }
        else {
            if (!tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('persontag')) {
                if (tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('persontag_otNotAuth')) { tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.remove('persontag'); }
                if (tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('persontag_otAuth')) { tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.remove('persontag_otAuth'); }
                tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.add('persontag');
            }
        }
                //circleMarker._layers[layerindex].feature
        if (tagsMarkersGroup._layers[layerindex].feature.properties.tagVisibleMils > 80000) {
            if (tagsMarkersGroup._layers[layerindex].hasOwnProperty("_tooltip") && tagsMarkersGroup._layers[layerindex]._tooltip.hasOwnProperty("_container") &&
                !tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('tooltip-hidden')) {

                tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.add('tooltip-hidden');

            }
        }
        if (tagsMarkersGroup._layers[layerindex].feature.properties.tagVisibleMils < 80000) {
            //if the distance from the current location is more then 10000 meters the do not so the slide to
            var newLatLng = new L.latLng(tagsMarkersGroup._layers[layerindex].feature.geometry[1], tagsMarkersGroup._layers[layerindex].feature.geometry[0]);
       
            tagsMarkersGroup._layers[layerindex].slideTo(newLatLng, { duration: 2000 });
            
            if (tagsMarkersGroup._layers[layerindex].hasOwnProperty("_tooltip") && tagsMarkersGroup._layers[layerindex]._tooltip.hasOwnProperty("_container") &&
                tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('tooltip-hidden')) {
                tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.remove('tooltip-hidden');

            }
        }
    } catch (e) {
        console.log(e);
    }
    return null;
}

 /*
  this is for AGV location 
  */
$.extend(fotfmanager.client, {
    updateAGVLocationStatus: async (updateAGVLocation) => { updateAGVLocationZone(updateAGVLocation) }
});

var agvLocations = new L.GeoJSON(null, {
    style: function (feature) {
        if (feature.properties.visible) {
            //'green' : 'gray'
            let inmission = '#989ea4';
            if (feature.properties.MissionList !== null && feature.properties.MissionList.length > 0)
            {
                inmission = '#28a745';
            }
            return {
                weight: 1,
                opacity: 1,
                color: '#3573b1',
                fillOpacity: 0.2,
                fillColor: inmission,
                lastOpacity: 0.2
            };
        }
    },
    onEachFeature: function (feature, layer) {
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        layer.on('click', function (e) {
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();

            map.setView(e.latlng, 3);
            if ((' ' + document.getElementById('sidebar').className + ' ').indexOf(' ' + 'collapsed' + ' ') <= -1) {
                if ($('#zoneselect').val() == feature.properties.id) {
                    sidebar.close('home');
                }
                else {
                    sidebar.open('home');
                }
            }
            else {
                sidebar.open('home');
            }
            LoadAGVLocationTables(feature.properties)
        });
        layer.bindTooltip(Get_location_Code(feature.properties.name), {
            direction: 'center',
            interactive: true,
            opacity: 0.9,
        }).openTooltip();
        agvLocations.bringToFront();
    }
});
async function updateAGVLocationZone(locationupdate) {
    try {
        if (agvLocations.hasOwnProperty("_layers")) {
            var layerindex = -0;
            $.map(agvLocations._layers, function (layer, i) {
                if (layer.hasOwnProperty("feature")) {
                    if (layer.feature.properties.id === locationupdate.properties.id) {
                        layer.feature.properties = locationupdate.properties;
                        layerindex = layer._leaflet_id;
                    
                        return false;
                    }
                }
            });
            if (layerindex !== -0) {
                if (agvLocations._layers[layerindex].feature.properties.hasOwnProperty("MissionList")) {
                    updatelocation(layerindex);
                }
                if ($('div[id=agvlocation_div]').is(':visible') && $('div[id=agvlocation_div]').attr("data-id") === locationupdate.properties.id) {
                    LoadAGVLocationTables(locationupdate.properties);
                }

            }
            else {
                agvLocations.addData(locationupdate);
            }
        }
    } catch (e) {
        console.log(e);
    }
}
async function updatelocation(layerindex) {
    if (agvLocations._layers[layerindex].feature.properties.hasOwnProperty("MissionList") && agvLocations._layers[layerindex].feature.properties.MissionList.length > 0) {
        agvLocations._layers[layerindex].setStyle({
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillColor: '#28a745',
            fillOpacity: 0.5,
            lastOpacity: 0.5
        });
    }
    else {
        agvLocations._layers[layerindex].setStyle({
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillColor: '#989ea4',
            fillOpacity: 0.2,
            lastOpacity: 0.2
        });
    }
}
async function LoadAGVLocationTables(dataproperties) {
    try {
        $zoneSelect[0].selectize.setValue(dataproperties.id, true);
        hideSidebarLayerDivs();
        $('div[id=agvlocation_div]').attr("data-id", dataproperties.id);
        $('div[id=agvlocation_div]').css('display', 'block');
        $zoneSelect[0].selectize.setValue(-1, true);
        $('span[name=locationid]').text(Get_location_Code(dataproperties.name));

        agvlocationinmission_Table_Body.empty();
        if (!$.isEmptyObject(dataproperties.MissionList)) {
            $.each(dataproperties.MissionList, function () {
                agvlocationinmission_Table_Body.append(agvlocation_row_inmission_template.supplant(formatagvlocationinmissionrow(this)));
            });
        }

    } catch (e) {
        console.log(e)
    }
}


let agvlocationinmission_Table = $('table[id=locationinmissiontable]');
let agvlocationinmission_Table_Body = agvlocationinmission_Table.find('tbody');

let agvlocation_row_inmission_template = '<tr class="text-center" id={RequestId}>' +
    '<td class="font-weight-bold" colspan="2">' +
        '<h6 class="control-label sectionHeader ml-1 mb-1 d-flex justify-content-between">'+
        'Mission Status' +
            '<span class="d-flex justify-content-between">'+
            'Request ID:'+
            '<span class="btn btn-secondary border-0 badge-dark badge">{RequestId}</span>  '+
            '</span>'+
        '</h6 >'+
    '</td>' +
    '<tr id=pickupLocation_{RequestId}><td>Vehicle Name:</td><td>{VehicleName}</td></tr>' +
    '<tr id=pickupLocation_{RequestId}><td>Pickup Location:</td><td>{PickupLocation}</td></tr>' +
    '<tr id=pickupLocationEta_{RequestId}><td>ETA to Pickup:</td><td>{PickupLocationEta}</td></tr>' +
    '<tr id=dropoffLocation_{RequestId}><td>Drop-Off Location:</td><td>{DropoffLocation}</td></tr>' +
    '<tr id=endLocation_{RequestId}><td>End Location:</td><td>{EndLocation}</td></tr>' +
    '<tr id=door_{RequestId}><td>Door:</td><td>{Door}</td></tr>' +
    '<tr id=placard_{RequestId}><td>Placard:</td><td>{Placard}</td></tr>' +
    '</tr>';
function formatagvlocationinmissionrow(properties) {
    return $.extend(properties, {
        RequestId: properties.RequestId,
        VehicleName: properties.Vehicle !== null ? properties.Vehicle : "N/A",
        PickupLocation: properties.hasOwnProperty("Pickup_Location") ? Get_location_Code(properties.Pickup_Location) : "N/A",
        DropoffLocation: properties.hasOwnProperty("Dropoff_Location") ? Get_location_Code(properties.Dropoff_Location) : "N/A",
        EndLocation: properties.hasOwnProperty("End_Location") ? Get_location_Code(properties.End_Location) : "N/A",
        PickupLocationEta: properties.ETA !== null ? properties.ETA : "N/A",
        Door: properties.Door !== null ? properties.Door : "N/A",
        Placard: properties.Placard !== null ? properties.Placard : "N/A"
    });
}
function formatAGVzonetoprow(properties) {
    return $.extend(properties, {
        zoneId: Get_location_Code(properties.name),
        zone_type: properties.Zone_Type
    });
}
function Get_location_Code(location) {
    var location_temp = "";
    if (checkValue(location)) {
        var arr = location.match(/.{1,3}/g);
        if (arr.length === 4) {
            var a = checkValue(arr[0].replace(/^0+/, '')) ? arr[0].replace(/^0+/, '') : "0";
            var b = checkValue(arr[1].replace(/^0+/, '')) ? arr[1].replace(/^0+/, '') : "0";
            var c = checkValue(arr[2].replace(/^0+/, '')) ? arr[2].replace(/^0+/, '') : "0";
            var d = checkValue(arr[3].replace(/^0+/, '')) ? arr[3].replace(/^0+/, '') : "0";

            location_temp = a + b + "-" + c + ',' + d;
        }
    }
    return location_temp;
}
/*
 this is for the dock door and container details.
 */
$.extend(fotfmanager.client, {
    updateDockDoorStatus: async (dockdoorupdate) => { updateDockDoorZone(dockdoorupdate) }
});
$(function () {
    $('button[name=tripSelectorbtn]').off().on('click', function () {
        let jsonObject = {
            RouteTrip: $('select[name=tripSelector] option:selected').val(),
            DoorNumber: $('span[name=doornumberid]').text(),
        };
        fotfmanager.server.updateRouteTripDoorAssigment(jsonObject).done();
    });
});
async function updateDockDoorZone(dockdoorzoneupdate) {
    try {
        let layerindex = -0;
        map.whenReady(() => {
            if (map.hasOwnProperty("_layers")) {
                $.map(map._layers, function (layer, i) {
                    if (layer.hasOwnProperty("feature")) {
                        if (layer.feature.properties.id === dockdoorzoneupdate.properties.id) {
                            layer.feature.properties = dockdoorzoneupdate.properties;
                            layerindex = layer._leaflet_id;
                            layer.setTooltipContent(dockdoorzoneupdate.properties.doorNumber.toString() + getDoorTripIndc(dockdoorzoneupdate.properties.dockdoorData) );
                            return false;
                        }
                    }
                });
                if (layerindex !== -0) {
                    if ($('div[id=dockdoor_div]').is(':visible') &&
                        $('div[id=dockdoor_div]').attr("data-id") === dockdoorzoneupdate.properties.id) {
                        LoadDockDoorTable(dockdoorzoneupdate.properties);
                    }
                    updatedockdoor(layerindex);
                }
                else {
                    dockDoors.addData(dockdoorzoneupdate);
                }
            }
        });

    } catch (e) {
        console.log(e);
    }
}

document.addEventListener("layerscontentvisible", () => {
    zoneStatusClose(true);
    
});
var dockDoors = new L.GeoJSON(null, {
    style: function (feature) {

        try {
            if (feature.properties.dockdoorData !== null && feature.properties.dockdoorData.length > 0) {

                if (feature.properties.dockdoorData[0].tripDirectionInd === "O") {
                    if (feature.properties.dockdoorData[0].tripMin <= 30) {
                        return {
                            weight: 2,
                            opacity: 1,
                            color: '#3573b1',
                            fillColor: '#dc3545',   // Red. ff0af7 is Purple
                            fillOpacity: 0.5,
                            lastOpacity: 0.5
                        };

                    }
                    else {
                       return{
                            weight: 2,
                            opacity: 1,
                            color: '#3573b1',
                            fillColor: '#3573b1',
                            fillOpacity: 0.5,
                            lastOpacity: 0.5
                        };
                    }
                }
                if (feature.properties.dockdoorData[0].tripDirectionInd === "I") {
                    return {
                        weight: 2,
                        opacity: 1,
                        color: '#3573b1',
                        fillColor: '#3573b1',
                        fillOpacity: 0.2,
                        lastOpacity: 0.2
                    };
                }
            }
            else {
                return{
                    weight: 2,
                    opacity: 1,
                    color: '#3573b1',
                    fillColor: '#989ea4',
                    fillOpacity: 0.2,
                    lastOpacity: 0.2
                };
            }
        } catch (e) {
            console.log(e);
            return {
                weight: 2,
                opacity: 1,
                color: '#3573b1',
                fillColor: '#989ea4',
                fillOpacity: 0.2,
                lastOpacity: 0.2
            }
        }
    },
    onEachFeature: function (feature, layer) {

        let dockdookflash = "";
        let doorNumberdisplay = feature.properties.doorNumber.toString();

        if (feature.properties.dockdoorData !== null && feature.properties.dockdoorData.length > 0 ) {
            doorNumberdisplay = feature.properties.doorNumber.toString() + (feature.properties.dockdoorData[0].tripDirectionInd !== "" ? "-" + feature.properties.dockdoorData[0].tripDirectionInd : "")
            if (feature.properties.dockdoorData[0].tripDirectionInd === "O") {
                if (feature.properties.dockdoorData[0].tripMin <= 30 && feature.properties.dockdoorData[0].Notloadedcontainers > 0) {
                    dockdookflash = "doorflash"
                }
            }
        }
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
       
        layer.on('click', function () {
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            if ((' ' + document.getElementById('sidebar').className + ' ').indexOf(' ' + 'collapsed' + ' ') <= -1) {
                if ($('#zoneselect').val() == feature.properties.id) {
                    sidebar.close('home');
                }
                else {
                    sidebar.open('home');
                }
            }
            else {
                sidebar.open('home');
            }
            LoadDockDoorTable(feature.properties);
        })
        layer.bindTooltip(doorNumberdisplay, {
            permanent: true,
            interactive: true,
            direction: 'center',
            opacity: 0.9,
            className: 'dockdooknumber ' + dockdookflash
        }).openTooltip();
        dockDoors.bringToFront();
    },
    filter: function (feature, layer) {
        return feature.properties.visible;
    }
});
function getDoorTripIndc(data)
{
    try {
        if (data.length > 0) {
            return "-" + data[0].tripDirectionInd;
        }
        else {
            return "";
        }
     
    } catch (e) {
        console.log(e);
        return "";
    }
   
}
let dockdoortop_Table = $('table[id=dockdoortable]');
let dockdoortop_Table_Body = dockdoortop_Table.find('tbody');
let dockdoortop_row_template = '<tr data-id={zone_id} >' +
    '<td class="text-right" style="border-right-style:solid" >{name}</td>' +
    '<td class="text-left">{value}</td>' +
    '</tr>';
function getDoorStatus(status)
{
    return status !== "" ? capitalize_Words(status) : "Unknown";
}
function formatdockdoortoprow(properties, zoneid) {
    return $.extend(properties, {
        zone_id: zoneid,
        name: properties.name.replace(/^0+/, ''),
        value: properties.value
    });
}
let container_Table = $('table[id=containertable]');
let container_Table_Body = container_Table.find('tbody');
function formatctscontainerrow(properties, zoneid) {
    return $.extend(properties, {
        zone_id: zoneid,
        id: properties.placardBarcode,
        dest: checkValue(properties.dest) ? properties.dest : "",
        location: checkValue(properties.location) ? properties.location : "",
        placard: properties.placardBarcode,
        status: properties.constainerStatus,
        backgroundcolorstatus: properties.constainerStatus === "Unloaded" ? "table-secondary" : properties.constainerStatus === "Close" ? "table-primary" : properties.constainerStatus === "Loaded" ? "table-success" : "table-danger",
    });
}
let container_row_template = '<tr>' +
    '<td data-input="dest" class="text-center">{dest}</td>' +
    '<td data-input="location" class="text-center">{location}</td>' +
    '<td data-input="found-icon" class="text-center"></td>' +
    '<td data-input="placard" class="text-center"><a data-doorid={zone_id} data-placardid={placard} class="containerdetails">{placard}</a></td>' +
    '<td data-input="status" class="text-center {backgroundcolorstatus}">{status}</td>' +
    '</tr>"';
async function updatedockdoor(layerindex) {
    if (map._layers[layerindex].feature.properties.dockdoorData !== null && map._layers[layerindex].feature.properties.dockdoorData.length > 0) {

        if (map._layers[layerindex].feature.properties.dockdoorData[0].tripDirectionInd === "O") {
            if (map._layers[layerindex].feature.properties.dockdoorData[0].tripMin <= 30) {
                map._layers[layerindex].setStyle({
                    weight: 2,
                    opacity: 1,
                    color: '#3573b1',
                    fillColor: '#dc3545',   // Red. ff0af7 is Purple
                    fillOpacity: 0.5,
                    lastOpacity: 0.5
                });
                if (map._layers[layerindex].feature.properties.dockdoorData[0].Notloadedcontainers > 0) {
                    if (map._layers[layerindex].hasOwnProperty("_tooltip")) {
                        if (map._layers[layerindex]._tooltip.hasOwnProperty("_container")) {
                            if (!map._layers[layerindex]._tooltip._container.classList.contains('doorflash')) {
                                map._layers[layerindex]._tooltip._container.classList.add('doorflash');
                            }
                        }
                    }
                }
                else {
                    if (map._layers[layerindex].hasOwnProperty("_tooltip")) {
                        if (map._layers[layerindex]._tooltip.hasOwnProperty("_container")) {
                            if (map._layers[layerindex]._tooltip._container.classList.contains('doorflash')) {
                                map._layers[layerindex]._tooltip._container.classList.remove('doorflash');
                            }
                        }
                    }
                }

            }
            else {
                map._layers[layerindex].setStyle({
                    weight: 2,
                    opacity: 1,
                    color: '#3573b1',
                    fillColor: '#3573b1',
                    fillOpacity: 0.5,
                    lastOpacity: 0.5
                });
                if (map._layers[layerindex].hasOwnProperty("_tooltip")) {
                    if (map._layers[layerindex]._tooltip.hasOwnProperty("_container")) {
                        if (map._layers[layerindex]._tooltip._container.classList.contains('doorflash')) {
                            map._layers[layerindex]._tooltip._container.classList.remove('doorflash');
                        }
                    }
                }
            }
        }
        if (map._layers[layerindex].feature.properties.dockdoorData[0].tripDirectionInd === "I") {
            map._layers[layerindex].setStyle({
                weight: 2,
                opacity: 1,
                color: '#3573b1',
                fillColor: '#3573b1',
                fillOpacity: 0.2,
                lastOpacity: 0.2
            });
        }
    }
    else {
        map._layers[layerindex].setStyle({
            weight: 2,
            opacity: 1,
            color: '#3573b1',
            fillColor: '#989ea4',
            fillOpacity: 0.2,
            lastOpacity: 0.2
        });
        if (map._layers[layerindex].hasOwnProperty("_tooltip")) {
            if (map._layers[layerindex]._tooltip.hasOwnProperty("_container")) {
                if (map._layers[layerindex]._tooltip._container.classList.contains('doorflash')) {
                    map._layers[layerindex]._tooltip._container.classList.remove('doorflash');
                }
            }
        }
    }

    if ($('div[id=dockdoor_div]').is(':visible')) {
        var findtrdataid = dockdoortop_Table_Body.find('tr[data-id=' + map._layers[layerindex].feature.properties.id + ']');
        if (findtrdataid.length > 0) {
            LoadDockDoorTable(map._layers[layerindex].feature.properties);
        }
    }
}

let dockdoorloaddata = [];
async function LoadDockDoorTable(data) {
    try {
        dockdoorloaddata = [];
        hideSidebarLayerDivs();
        $('div[id=dockdoor_div]').attr("data-id", data.id);
        $('div[id=dockdoor_div]').css('display', 'block');
        $('div[id=dockdoor_tripdiv]').css('display', 'block');
        $('div[id=ctstabs_div]').css('display', 'none');
        $('div[id=trailer_div]').css('display', 'block');
        $('button[name=container_counts]').text(0 + "/" + 0);
        $('span[name=doornumberid]').text(data.doorNumber);
        $zoneSelect[0].selectize.setValue(-1, true);
        $zoneSelect[0].selectize.setValue(data.id, true);
        dockdoortop_Table_Body.empty();
        container_Table_Body.empty();
        assignedTrips_Table_Body.empty();

        if (data.dockdoorData.length > 0) {
           // loadAssignedTripDataTable(data.dockdoorData,"doortriptable")
            $.each(data.dockdoorData, function () {
                assignedTrips_Table_Body.append(assignedTrips_row_template.supplant(formatassignedTripsrow(this)));
            });
            let dataproperties = data.dockdoorData[0];
            let loadtriphisory = false;
            let tempdata = [];    
/*            if (checkValue(dataproperties.legSiteName)) {*/
                $('select[id=tripSelector]').val(dataproperties.id);
            
                if (dataproperties.legSiteName !== "") {
                    tempdata.push({
                        name: "Site Name",
                        value: "(" + dataproperties.legSiteId + ") " + dataproperties.legSiteName
                    })
                }
                if (dataproperties.route !== "") {
                    tempdata.push({
                        name: "Route-Trip",
                        value: dataproperties.route + "-" + dataproperties.trip
                    })
                }
                if (dataproperties.trailerBarcode !== "") {
                    tempdata.push({
                        name: "Trailer Barcode",
                        value: dataproperties.trailerBarcode
                    })
                }
                if (dataproperties.status !== "") {
                    if (checkValue(dataproperties.status)) {
                        $('span[name=doorstatus]').text(capitalize_Words(dataproperties.status));
                    }
                }
                else {
                    $('span[name=doorstatus]').text("Occupied");
                }
                if (dataproperties.loadPercent !== null) {
                    tempdata.push({
                        name: "Load Percent",
                        value: checkValue(dataproperties.loadPercent) ? dataproperties.loadPercent : 0
                    })
                }

                if (dataproperties.tripDirectionInd !== "") {
                    tempdata.push({
                        name: "Direction",
                        value: dataproperties.tripDirectionInd === "O" ? "Out-bound" : "In-bound"
                    })
                }
                if (dataproperties.scheduledDtm !== null) {
                    tempdata.push({
                        name: dataproperties.tripDirectionInd === "O" ? "Scheduled To Depart" : "Scheduled Arrive Time",
                        value: formatSVmonthdayTime(dataproperties.scheduledDtm)
                    })
                }
                if (dataproperties.tripDirectionInd === "I") {
                    if (dataproperties.actualDtm !== null) {
                        tempdata.push({
                            name: "Actual Arrive Time",
                            value: formatSVmonthdayTime(dataproperties.actualDtm)
                        })
                    }
                }
                $.each(tempdata, function () {
                    dockdoortop_Table_Body.append(dockdoortop_row_template.supplant(formatdockdoortoprow(this, dataproperties.id)));
                });

                $('button[name=container_counts]').text(0 + "/" + 0);
                if (dataproperties.tripDirectionInd) {
                    //var legSite = dataproperties.dockdoorData.tripDirectionInd === "I" ? User.NASS_Code : dataproperties.dockdoorData.legSiteId;
                    //$.connection.FOTFManager.server.getContainer(legSite, dataproperties.dockdoorData.tripDirectionInd, dataproperties.dockdoorData.route, dataproperties.dockdoorData.trip).done(function (Data) {

                    if (dataproperties.containerScans !== null && dataproperties.containerScans.length > 0) {
                        var loadedcount = 0;
                        var unloadedcount = 0;
                        var loaddata = [];
                        container_Table_Body.empty();
                        $.each(dataproperties.containerScans, function (index, d) {
                            if (!d.containerTerminate) {
                                if (dataproperties.tripDirectionInd === "O") {
                                    if (d.containerAtDest === false) {
                                        if (d.hasUnloadScans === true) {
                                            unloadedcount++
                                            d.constainerStatus = "Unloaded";
                                            d.sortind = 1;
                                            loaddata.push(d);
                                        }

                                        if (d.hasLoadScans === true) {
                                            loadedcount++
                                            d.constainerStatus = "Loaded";
                                            d.sortind = 2;
                                            loaddata.push(d);
                                        }
                                        if (d.hasLoadScans === false && d.hasAssignScans === true && d.hasCloseScans === true) {
                                            unloadedcount++;
                                            d.constainerStatus = "Close";
                                            d.sortind = 0;
                                            loaddata.push(d);
                                        }

                                    }
                                }
                                if (dataproperties.tripDirectionInd === "I") {
                                    if (d.hasUnloadScans === true && d.Itrailer === dataproperties.trailerBarcode) {
                                        unloadedcount++
                                        d.constainerStatus = "Unloaded";
                                        d.sortind = 1;
                                        loaddata.push(this);
                                    }
                                }
                            }
                        });
                        loaddata.sort(SortByind);
                        $.each(loaddata, function () {
                            container_Table_Body.append(container_row_template.supplant(formatctscontainerrow(this, dataproperties.id)));
                        });
                        dockdoorloaddata = JSON.parse(JSON.stringify(loaddata));

                        $('button[name=container_counts]').text(loadedcount + "/" + unloadedcount);
                    }
                    else {
                        $('button[name=container_counts]').text(0 + "/" + 0);
                        container_Table_Body.empty();
                    }
                    //});

                }
                else {
                    $('button[name=container_counts]').text(0 + "/" + 0);
                    container_Table_Body.empty();

                }

                if (loadtriphisory) {
                    $('div[id=trailer_div]').css('display', 'none');
                }
            //}
            //else {
            //    $('div[id=trailer_div]').css('display', 'none');
            //    $('select[id=tripSelector]').val("");
            //    $('span[name=doornumberid]').text(data.doorNumber);
            //    $('span[name=doorstatus]').text(getDoorStatus(dataproperties.status));
            //    $.each(tempdata, function () {
            //        dockdoortop_Table_Body.append(dockdoortop_row_template.supplant(formatdockdoortoprow(this, dataproperties.id)));
            //    });
            //}
        }
        else {
            $('span[name=doorstatus]').text(getDoorStatus(""));
            $('select[id=tripSelector]').val("");
            $('span[name=doornumberid]').text(data.doorNumber);
        }
    }
    catch (e) {
        console.log(e);
    }
}
async function LoadDoorDetails(door) {
    if (dockDoors.hasOwnProperty("_layers")) {
        $.map(dockDoors._layers, function (layer, i) {
            if (layer.hasOwnProperty("feature")) {
                if (layer.feature.properties.doorNumber === door) {
                    map.invalidateSize();
                    map.setZoom(1);
                 //$('<option/>').val(this.id).html(this.route + " - " + this.trip + " | " + this.destSiteName).appendTo;
                    LoadDockDoorTable(layer.feature.properties);

                    return false;
                }
            }
        });
    }
}
function SortByind(a, b) {
    // sonar lint doesn't like nested ternary operations, 
    // so this has been updated to two lines of code.
    if (a.sortind < b.sortind) return -1;
    return ((a.sortind > b.sortind) ? 1 : 0);
}

async function removeDockDoor(id)
{
    try {
        $.map(map._layers, function (layer, i) {
            if (layer.hasOwnProperty("feature")) {
                if (layer._leaflet_id === id) {
                    dockDoors.removeLayer(layer)
                }
            }
        });
    } catch (e) {
        console.log(e)
    }
}
async function LoadContainerDetails(id, placardid) {
    try {
        let d = {};
       
            if (dockDoors.hasOwnProperty("_layers")) {
                $.map(dockDoors._layers, function (layer, i) {
                    if (layer.hasOwnProperty("feature")) {
                        if (layer.feature.properties.id === id) {
                           d = layer.feature.properties 
                            return false;
                        }
                    }
                });
            }
        
        if (!$.isEmptyObject(d)) {
            loadcontainerHistory(d, placardid);
        }
        else {
            fotfmanager.server.getContainerInfo(placardid).done(function (container) {
                loadcontainerHistory(container, placardid);
            });
        }
    }
    catch (e) {
        console.log(e);
    }
}
async function loadcontainerHistory(d, placardid) {
    try {
        $('#modalContainerDetailsHeader_ID').text(placardid);
        let containerdetails_Table = $('table[id=containerdetailstable]');
        let containerdetails_Table_Body = containerdetails_Table.find('tbody');
        containerdetails_Table_Body.empty();
        let containerdetails_row_template = '<tr>' +
            '<td class="text-center">{site_nasscode}</td>' +
            '<td class="text-center">{name}</td>' +
            '<td class="text-center">{sitetype}</td>' +
            '<td class="text-center">{event}</td>' +
            '<td class="text-center">{eventdatetime}</td>' +
            '<td class="text-center">{location}</td>' +
            '<td class="text-center">{bin}</td>' +
            '<td class="text-center">{trailer}</td>' +
            '<td class="text-center">{route}</td>' +
            '<td class="text-center">{trip}</td>' +
            '<td class="text-center">{source}</td>' +
            '<td class="text-center">{user}</td>' +
            '</tr>"';

        let containerHistory = {};
        if (d.hasOwnProperty("dockdoorData")) {
            $.map(d.dockdoorData.containerScans, function (container, i) {
                if (container.placardBarcode === placardid) {
                    containerHistory = container.containerHistory
                    return false;
                }
            });
        }
        else {
            $.map(d[0], function (container, i) {
                containerHistory = d[0].containerHistory
                return false;
            });
        }
        if (!$.isEmptyObject(containerHistory)) {
            containerHistory.sort(SortByind);
            $.each(containerHistory, function () {
                containerdetails_Table_Body.append(containerdetails_row_template.supplant(formatcontainerdetailsrow(this)));
            });
        }
        $("#ContainerDetails_Modal").modal();
    } catch (e) {
        console.log(e);
    }
}
function formatcontainerdetailsrow(properties)
{
    return $.extend(properties, {
        site_nasscode: properties.siteId,
        name: properties.siteName,
        sitetype: properties.siteType,
        event: properties.event,
        eventdatetime: formatDateTime(properties.EventDtmfmt),
        location: checkValue(properties.location) ? properties.location : "",
        bin: checkValue(properties.binName) ? properties.binName: "",
        trailer: checkValue(properties.trailer) ? properties.trailer : "",
        route: checkValue(properties.route) ? properties.route : "", 
        trip: checkValue(properties.trip) ? properties.trip : "", 
        source: properties.source,
        user: properties.updtUserId
    });
}
//assigend trips to dock doors table setup and load
/*
Scheduled outbound trips 
*/
function formatassignedTripsrow(properties) {
    return $.extend(properties, {
        schd: objSVTime(properties.scheduledDtm),
        departed: !$.isEmptyObject(properties.actualDtm) ? objSVTime(properties.actualDtm) : "",
        routetrip: properties.route + properties.trip + properties.tripDirectionInd,
        door: checkValue(properties.doorNumber) ? properties.doorNumber : "0",
        route: properties.route,
        routeid: properties.id,
        trip: properties.trip,
        tripDirection: properties.tripDirectionInd === "O" ? "Out-bound" : "In-bound",
        firstlegDest: properties.legSiteId,
        firstlegSite: properties.legSiteName,
        btnloadDoor: Load_btn_door(properties)
    });
}
let assignedTrips_Table = $('table[id=doortriptable]');
let assignedTrips_Table_Body = assignedTrips_Table.find('tbody');
let assignedTrips_row_template = '<tr data-id={routeid} data-door={door} >' +
    '<td class="text-center">{schd}</td>' +
    '<td class="text-center">{departed}</td>' +
    '<td>' +
    '<button class="btn btn-outline-info btn-sm btn-block px-1 routetripdetails" data-routetrip="{routeid}" style="font-size:12px;">{route}-{trip}</button>' +
    '</td> ' +
    '<td class="{background}">{btnloadDoor}</td>' +
    '<td class="text-center">{tripDirection}</td>' +
    '<td data-toggle="tooltip" title={firstlegSite}>{firstlegSite}</td>' +
    '</tr>"';

/*
 this is for stage zone
 */
$.extend(fotfmanager.client, {
    updateStageZoneStatus: async (updateStage) => { updateStageZone(updateStage) }
});
async function updateStageZone() {
    try {
    } catch (e) {
        console.log(e);
    }
}

var exitAreas = new L.GeoJSON(null, {
    style: function (feature) {
        return style = {
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillOpacity: 0.2,
            fillColor: '#989ea4',//'gray'
            lastOpacity: 0.2
        };
    },
    onEachFeature: function (feature, layer) {
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        layer.on('click', function (e) {
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng);
            if ((' ' + document.getElementById('sidebar').className + ' ').indexOf(' ' + 'collapsed' + ' ') <= -1) {
                if ($('#zoneselect').val() == feature.properties.id) {
                    sidebar.close('home');
                }
                else {
                    sidebar.open('home');
                }
            }
            else {
                sidebar.open('home');
            }
            LoadstageTables(feature.properties);

        });
        layer.bindTooltip(feature.properties.name, {
            permanent: true,
            interactive: true,
            direction: 'center',
            //opacity: 0.8,
            opacity: 1,
            className: 'location'
        }).openTooltip();
        layer.bringToBack();

    }
})
var polyholesAreas = new L.GeoJSON(null, {
    style: function (feature) {
        return style = {
            weight: 0,
            opacity: 0,
            color: '#3573b1',
            fillOpacity: 0.0,
            fillColor: '#989ea4',
            lastOpacity: 0.0//'gray'
        };
    },
    onEachFeature: function (feature, layer) {
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        layer.on('click', function (e) {
            //set to the center of the polygon.
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng);
            if ((' ' + document.getElementById('sidebar').className + ' ').indexOf(' ' + 'collapsed' + ' ') <= -1) {
                if ($('#zoneselect').val() == feature.properties.id) {
                    sidebar.close('home');
                }
                else {
                    sidebar.open('home');
                }
            }
            else {
                sidebar.open('home');
            }
            LoadstageTables(feature.properties);
        });
        layer.bindTooltip(feature.properties.name, {
            permanent: true,
            interactive: true,
            direction: 'center',
            //opacity: 0.8,
            opacity: 1,
            className: 'location'
        }).openTooltip();
        layer.bringToBack();
    },
});
var ebrAreas = new L.GeoJSON(null, {
    style: function (feature) {
        return {
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillOpacity: 0.2,
            fillColor: '#989ea4',//'gray'
            lastOpacity: 0.2
        };
    },
    onEachFeature: function (feature, layer) {
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        layer.on('click', function (e) {
            //set to the center of the polygon.
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng);
            if ((' ' + document.getElementById('sidebar').className + ' ').indexOf(' ' + 'collapsed' + ' ') <= -1) {
                if ($('#zoneselect').val() == feature.properties.id) {
                    sidebar.close('home');
                }
                else {
                    sidebar.open('home');
                }
            }
            else {
                sidebar.open('home');
            }
            LoadstageTables(feature.properties);
        });
        layer.bindTooltip(feature.properties.name, {
            permanent: true,
            interactive: true,
            direction: 'center',
            //opacity: 0.8,
            opacity: 1,
            className: 'location'
        }).openTooltip();
        layer.bringToBack();
    },
});
var walkwayAreas = new L.GeoJSON(null, {
    style: function (feature) {
        return style = {
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillOpacity: 0.2,
            fillColor: '#989ea4',//'gray'
            lastOpacity: 0.2
        };
    },
    onEachFeature: function (feature, layer) {
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        layer.on('click', function (e) {
            //set to the center of the polygon.
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng);
            if ((' ' + document.getElementById('sidebar').className + ' ').indexOf(' ' + 'collapsed' + ' ') <= -1) {
                if ($('#zoneselect').val() == feature.properties.id) {
                    sidebar.close('home');
                }
                else {
                    sidebar.open('home');
                }
            }
            else {
                sidebar.open('home');
            }
            LoadstageTables(feature.properties);
        });
        layer.bindTooltip(feature.properties.name, {
            permanent: true,
            interactive: true,
            direction: 'center',
            //opacity: 0.8,
            opacity: 1,
            className: 'location'
        }).openTooltip();
        layer.bringToBack();
    },
});
var stagingAreas = new L.GeoJSON(null, {
    style: function (feature) {
        return style = {
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillOpacity: 0.2,
            fillColor: '#989ea4',//'gray'
            lastOpacity: 0.2
        };
    },
    onEachFeature: function (feature, layer) {
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        layer.on('click', function (e) {
            //set to the center of the polygon.
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng);
            if ((' ' + document.getElementById('sidebar').className + ' ').indexOf(' ' + 'collapsed' + ' ') <= -1) {
                if ($('#zoneselect').val() == feature.properties.id) {
                    sidebar.close('home');
                }
                else {
                    sidebar.open('home');
                }
            }
            else {
                sidebar.open('home');
            }
            LoadstageTables(feature.properties);
        
        });

        layer.bindTooltip(feature.properties.name.replace(/^Staging_/i, ''), {
            permanent: true,
            interactive: true,
            direction: 'center',
            //opacity: 0.8,
            opacity: 1,
            className: 'location'
        }).openTooltip();
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        layer.bringToBack();
        
    },
    filter: function (feature, layer) {
        return feature.properties.visible;
    }

});
async function LoadstageTables(dataproperties) {
    try {
        $zoneSelect[0].selectize.setValue(dataproperties.id, true);
        hideSidebarLayerDivs();
        $('div[id=area_div]').attr("data-id", dataproperties.id);
        $('div[id=area_div]').css('display', 'block');
        zonetop_Table_Body.empty();
        zonetop_Table_Body.append(zonetop_row_template.supplant(formatzonetoprow(dataproperties)));
        var p2pdata = dataproperties.hasOwnProperty("P2PData") ? dataproperties.P2PData : "";
        var CurrentStaff = [];
        GetPeopleInZone(dataproperties.id, p2pdata, CurrentStaff);
    } catch (e) {
        console.log(e)
    }
}
let zonetop_Table = $('table[id=areazonetoptable]');
let zonetop_Table_Body = zonetop_Table.find('tbody');
let zonetop_row_template = '<tr data-id="{zoneId}"><td>{zone_type}</td><td>{zoneId}</td></tr>"';
function formatzonetoprow(properties) {
    return $.extend(properties, {
        zoneId: properties.name,
        zone_type: properties.Zone_Type
    });
}
async function LoadstageDetails(selcValue) {
    try {
        if (polygonMachine.hasOwnProperty("_layers")) {
            $.map(stagingAreas._layers, function (layer, i) {
                if (layer.hasOwnProperty("feature")) {
                    if (layer.feature.properties.id === selcValue) {
                        var Center = new L.latLng(
                            (layer._bounds._southWest.lat + layer._bounds._northEast.lat) / 2,
                            (layer._bounds._southWest.lng + layer._bounds._northEast.lng) / 2);
                        map.setView(Center, 3);
                        if (/Area/i.test(layer.feature.properties.Zone_Type)) {
                            LoadstageTables(layer.feature.properties);
                        }
                        return false;
                    }
                }
            });
        }
    } catch (e) {
        console.log(e)
    }

}
$('#zoneselect').change(function (e) {
    $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
    var selcValue = this.value;
    LoadstageDetails(selcValue);

});
function init_zones(zoneData, id) {
    //Get Zones list
    var hasDockDoorZone = false;
    var hasMachineZone = false;
    var hasBinZone = false;


    $.each(zoneData, function () {
        if (/^ebr/i.test(this.properties.name)) {

            ebrAreas.addData(this);
        }
        else if (/^Staging/i.test(this.properties.name)) {

            stagingAreas.addData(this);
        }
        else if (/^Walkway/i.test(this.properties.name)) {

            walkwayAreas.addData(this);
        }
        else if (/^exit/i.test(this.properties.name)) {

            exitAreas.addData(this);
        }
        else if (/^(Poly|hol)/i.test(this.properties.name)) {

            polyholesAreas.addData(this);
        }
        else if (/^(DockDoor)/i.test(this.properties.Zone_Type)) {
            dockDoors.addData(this);
    
            hasDockDoorZone = true;
            //fotfmanager.server.joinGroup("DockDoorZones");
        }
        else if (/^(Machine)/i.test(this.properties.Zone_Type)) {

            polygonMachine.addData(this);
            hasMachineZone = true;
            //fotfmanager.server.joinGroup("MachineZones");
        }
        else if (/^(Bin)/i.test(this.properties.Zone_Type)) {

            binzonepoly.addData(this);
            hasBinZone = true;
            //fotfmanager.server.joinGroup("BinZones");
        }
        else if (/^(AGVLocation)/i.test(this.properties.Zone_Type)) {

            agvLocations.addData(this);
        }
        else if (/^(ViewPorts)/i.test(this.properties.Zone_Type)) {
            viewPortsAreas.addData(this);
        }
        else if (/^(Bullpen)/i.test(this.properties.Zone_Type)) {
            stagingBullpenAreas.addData(this);
            // fotfmanager.server.joinGroup("SVZones");

        }
        else {

            stagingAreas.addData(this);
        }
    })
    if (hasDockDoorZone) { fotfmanager.server.joinGroup("DockDoorZones"); }
    if (hasMachineZone) { fotfmanager.server.joinGroup("MachineZones"); }
    if (hasBinZone) { fotfmanager.server.joinGroup("BinZones"); }
    // setGreyedOut();
    fotfmanager.server.joinGroup("Zones");
    fotfmanager.server.joinGroup("QSM");
}
/* use this for locater data */

async function init_locators(marker, id) {
    $.each(marker, function () {
        if (this.properties.Tag_Type === "Vehicle") {
            piv_vehicles.addData(this);
        }
        else if (this.properties.Tag_Type === "Autonomous Vehicle") {
            agv_vehicles.addData(this);
        }
        else if (this.properties.Tag_Type === "Camera") {
            cameras.addData(this);
          
        }
        else {
            locatorMarker.addData(this)
        }
    });
    fotfmanager.server.joinGroup("VehiclsMarkers");
    fotfmanager.server.joinGroup("CameraMarkers");
}

var locatorMarker = new L.GeoJSON(null, {
    pointToLayer: function (feature, latlng) {
        var locaterIcon = L.divIcon({
            id: feature.properties.id,
            className: 'bi-broadcast',
            
        });
        return L.marker(latlng, {
            icon: locaterIcon,
            title: feature.properties.name,
            riseOnHover: true,
            bubblingMouseEvents: true,
            popupOpen: true
        })
    },
    onEachFeature: function (feature, layer) {
        layer.bindTooltip(feature.properties.name, {
            permanent: false,
            interactive: true,
            direction: 'top',
            opacity: 0.9
        }).openTooltip();
    }
})
/**
 *use this for the trips
 */
//remove trips
$.extend(fotfmanager.client, {
 
});
$.extend(fotfmanager.client, {
    updateSVTripsStatus: async (updatetripsstatus) => { updateTrips(updatetripsstatus) },
    removeSVTrips: async (tripid) => { removeTrips(tripid); }
});

async function init_arrive_depart_trips() {
    try {
        //Get trips list
        fotfmanager.server.getTripsList().done(function (trips) {
            if (trips.length > 0) {
                trips.sort(function (left, right) {
                    if (!$.isEmptyObject(left.scheduledDtm)) {
                        var leftt = moment().set({ 'year': left.scheduledDtm.year, 'month': left.scheduledDtm.month + 1, 'date': left.scheduledDtm.dayOfMonth, 'hour': left.scheduledDtm.hourOfDay, 'minute': left.scheduledDtm.minute, 'second': left.scheduledDtm.second });
                        var rightt = moment().set({ 'year': right.scheduledDtm.year, 'month': right.scheduledDtm.month + 1, 'date': right.scheduledDtm.dayOfMonth, 'hour': right.scheduledDtm.hourOfDay, 'minute': right.scheduledDtm.minute, 'second': right.scheduledDtm.second });

                        return leftt.diff(rightt);
                    }
                   
                });
                $.each(trips.sort(SortBySiteName), function () {
                    process_trips(this);
                    $('<option/>').attr("id",this.id).val(this.id).html(this.legSiteName + " | " + this.route + " - " + this.trip + " | " + tripDirectiontext(this)).appendTo('select[id=tripSelector]');
                });
            }
        });
    } catch (e) {
        console.log(e);
    }
}
async function updateTrips(trip) {
    let routetrip = trip.id;
    let trname = $.find('tbody tr[data-id=' + routetrip + ']');
   if (trname.length > 0) {
        for (let tr_name of trname) {
            let tablename = $(tr_name).closest('table').attr('id');
            if (checkValue(tablename)) {
                if (/remove/i.test(trip.state)) {
                      
                }
                else {
                    switch (tablename) {
                        case "ctsdockdepartedtable":
                            ob_trips_Table_Body.find('tbody tr[data-id=' + routetrip + ']').replaceWith(ob_trips_row_template.supplant(formatobtriprow(trip)));
                            break;
                        case "ctslocaldockdepartedtable":
                            oblocal_trips_Table_Body.find('tbody tr[data-id=' + routetrip + ']').replaceWith(oblocal_trips_row_template.supplant(formatobtriprow(trip)));
                            break;
                        case "ctsouttoptable":
                            scheouttrips_Table_Body.find('tbody tr[data-id=' + routetrip + ']').replaceWith(scheouttrips_row_template.supplant(formatobtriprow(trip)));
                            break;
                        case "ctsintoptable":
                            scheintrips_Table_Body.find('tbody tr[data-id=' + routetrip + ']').replaceWith(scheintrips_row_template.supplant(formatobtriprow(trip)));
                            break;
                        default:
                    }
                }
            }
        }
       
    }
    else {
        if (!/remove/i.test(trip.state)) {
            process_trips(trip);
        }
        
    }
}
async function process_trips(trip)
{
    try {
        switch (trip.isAODU) {
            case "Y":
                if (/^o$/i.test(trip.tripDirectionInd)) {
                    scheouttrips_Table_Body.append(scheouttrips_row_template.supplant(formatscheouttriprow(trip)));
                    oblocal_trips_Table_Body.append(oblocal_trips_row_template.supplant(formatoblocaltriprow(trip)));

                } else if (/^i$/i.test(trip.tripDirectionInd)) {
                    scheintrips_Table_Body.append(scheintrips_row_template.supplant(formatscheintriprow(trip)));

                }
                break;
            case "N":
                if (/^o$/i.test(trip.tripDirectionInd)) {
                    scheouttrips_Table_Body.append(scheouttrips_row_template.supplant(formatscheouttriprow(trip)));
                    ob_trips_Table_Body.append(ob_trips_row_template.supplant(formatobtriprow(trip)));
                } else if (/^i$/i.test(trip.tripDirectionInd)) {
                    scheintrips_Table_Body.append(scheintrips_row_template.supplant(formatscheintriprow(trip)));
                }
               
                break;
            default:
        }
       
    } catch (e) {
        console.log(e);
    }
}
/**
Outbound network trips
 */
function formatobtriprow(properties) {
    return $.extend(properties, {
        schd: objSVTime(properties.scheduledDtm), //properties.hasOwnProperty("ScheduledDtm") ? formatSVTime(properties.ScheduledDtm) : "" ,
        departed: !$.isEmptyObject(properties.actualDtm) ? objSVTime(properties.actualDtm) : "",
        door: checkValue(properties.doorNumber) ? properties.doorNumber : "0",
        routetrip: properties.route + properties.trip + properties.tripDirectionInd,
        route: properties.route,
        routeid: properties.id,
        trip: properties.trip,
        leg: properties.legSiteId,
        dest: properties.legSiteName,
        load: properties.hasOwnProperty("Load") ? properties.doorNumber : "0",
        trbackground: "",// Gettimediff(properties.ScheduledTZ),
        close: properties.hasOwnProperty("Closed") ? properties.doorNumber : "0",
        btnloadDoor: Load_btn_door(properties),
        btnloadPercent: "0 %",
        dataproperties: properties
    });
}
let ob_trips_Table = $('table[id=ctsdockdepartedtable]');
let ob_trips_Table_Body = ob_trips_Table.find('tbody');
let ob_trips_row_template = '<tr data-id={routeid} data-route={route} data-trip={trip}  data-door={door}  class={trbackground}>' +
    '<td><span class="ml-p25rem">{schd}</span></td>' +
    '<td>{departed}</td>' +
    '<td>' +
    '<button class="btn btn-outline-info btn-sm btn-block px-1 routetripdetails" data-routetrip="{routeid}" style="font-size:12px;">{route}-{trip}</button>' +
    '</td>' +
    '<td>{btnloadDoor}</td>' +
    '<td>{leg}</td>' +
    '<td data-toggle="tooltip" title="{dest}">{dest}</td>' +
    //'<td class="text-center">{close}</td>' +
    //'<td class="text-center">{load}</td>' +
    //'<td class="text-center">{btnloadPercent}</td>' +
    '</tr>"';

/*
 Outbound local trips 
 */
function formatoblocaltriprow(properties) {
    return $.extend(properties, {
        schd: objSVTime(properties.scheduledDtm),//properties.hasOwnProperty("ScheduledDtm") ? formatSVTime(properties.ScheduledDtm) : "",
        departed: !$.isEmptyObject(properties.actualDtm) ? objSVTime(properties.actualDtm) : "",
        door: checkValue(properties.doorNumber) ? properties.doorNumber : "0",
        routetrip: properties.route + properties.trip + properties.tripDirectionInd,
        route: properties.route,
        routeid: properties.id,
        trip: properties.trip,
        leg: properties.legSiteId,
        dest: properties.legSiteName,
        load: properties.hasOwnProperty("Load") ? properties.Load + '%' : "0",
        trbackground: "",
        close: properties.hasOwnProperty("Closed") ? properties.Closed + '%' : "0",
        loadPercent: properties.hasOwnProperty("LoadPercent") ? properties.LoadPercent + '%' : "0 %",
        btnloadDoor: Load_btn_door(properties),
        dataproperties: properties
    });
}
let oblocal_trips_Table = $('table[id=ctslocaldockdepartedtable]');
let oblocal_trips_Table_Body = oblocal_trips_Table.find('tbody');
let oblocal_trips_row_template = '<tr data-id={routeid} data-route={route} data-trip={trip} data-door={door} class={trbackground}>' +
    '<td class="text-center">{schd}</td>' +
    '<td class="text-center">{departed}</td>' +
    '<td>'+
    '<button class="btn btn-outline-info btn-sm btn-block px-1 routetripdetails" data-routetrip="{routeid}" style="font-size:12px;">{route}-{trip}</button>' +
    '</td> ' +
    '<td class="{background}">{btnloadDoor}</td>' +
    '<td class="text-center">{leg}</td>' +
    '<td data-toggle="tooltip" title="{dest}">{dest}</td>' +
    //'<td class="text-center">{close}</td>' +
    //'<td class="text-center">{load}</td>' +
    //'<td class="text-center">{loadPercent}</td>' +
    '</tr>"';



/*
Scheduled outbound trips 
*/
function formatscheouttriprow(properties) {
    return $.extend(properties, {
        schd: objSVTime(properties.scheduledDtm),//properties.hasOwnProperty("ScheduledDtm") ? formatSVTime(properties.ScheduledDtm) : "",
        departed: !$.isEmptyObject(properties.actualDtm) ? objSVTime(properties.actualDtm) : "",
        routetrip: properties.route + properties.trip + properties.tripDirectionInd,
        door: checkValue(properties.doorNumber) ? properties.doorNumber : "0",
        route: properties.route,
        routeid: properties.id,
        trip: properties.trip,
        firstlegDest: properties.legSiteId,
        firstlegSite: properties.legSiteName,
        btnloadDoor: Load_btn_door(properties)
    });
}
let scheouttrips_Table = $('table[id=ctsouttoptable]');
let scheouttrips_Table_Body = scheouttrips_Table.find('tbody');
let scheouttrips_row_template = '<tr data-id={routeid} data-door={door} >' +
    '<td class="text-center">{schd}</td>' +
    '<td class="text-center">{departed}</td>' +
    '<td>' +
    '<button class="btn btn-outline-info btn-sm btn-block px-1 routetripdetails" data-routetrip="{routeid}" style="font-size:12px;">{route}-{trip}</button>' +
    '</td> ' +
    '<td class="{background}">{btnloadDoor}</td>' +
    '<td class="text-center">{firstlegDest}</td>' +
    '<td data-toggle="tooltip" title={firstlegSite}>{firstlegSite}</td>' +
    '</tr>"';

/*
Scheduled inbound trips
*/
function formatscheintriprow(properties) {
    return $.extend(properties, {
        schd: objSVTime(properties.scheduledDtm),//properties.hasOwnProperty("LegScheduledDtm") ? formatSVTime(properties.LegScheduledDtm) : "",
        arrived: !$.isEmptyObject(properties.legActualDtm) ? objSVTime(properties.legActualDtm) : "",
        routetrip: properties.route + properties.trip + properties.tripDirectionInd,
        door: checkValue(properties.doorNumber) ? properties.doorNumber : "0",
        route: properties.route,
        routeid: properties.id,
        trip: properties.trip,
        inbackground: "", //GettimediffforInbound(properties.Scheduled, properties.Actual),
        leg_Origin: properties.legSiteId,
        site_Name: properties.legSiteName,
        btnloadDoor: Load_btn_door(properties)
    });
}
let scheintrips_Table = $('table[id=ctsintoptable]');
let scheintrips_Table_Body = scheintrips_Table.find('tbody');
let scheintrips_row_template = '<tr data-id="{routeid}" data-door="{door}">' +
    '<td  data-toggle="tooltip" title="{route} - {trip}" class="text-center" class="{inbackground}">{schd}</td>' +
    '<td class="text-center" class="{inbackground}">{arrived}</td>' +
    '<td>' +
    '<button class="btn btn-outline-info btn-sm btn-block px-1 routetripdetails" data-routetrip="{routeid}" style="font-size:12px;">{route}-{trip}</button>' +
    '</td> ' +
    '<td class="text-center" class="{background}">{btnloadDoor}</td>' +
    '<td class="text-center">{leg_Origin}</td>' +
    '<td data-toggle="tooltip" title="{site_Name}">{site_Name}</td>' +
    '</tr>';

function Load_btn_door(properties) {
    if (properties.hasOwnProperty("doorNumber")) {
        if (checkValue(properties.doorNumber)) {
            return '<button class="btn btn-outline-info purpleBg btn-sm btn-block px-1 doordetails" data-door="' + properties.doorNumber + '">' + properties.doorNumber + '</button>';
        } else {
            return '';
        }
    }
    else {
        return '';
    }
}
function tripDirectiontext(data) {
    return data.tripDirectionInd === "O" ? "Out-bound" : "In-bound"
}
$(document).on('click', '.doordetails', function () {
    var button = $(this);
    var doorid = button.attr('data-door');
    if (checkValue(doorid)) {
        LoadDoorDetails(doorid);
    }
});
$(document).on('click', '.routetripdetails', function () {
    var button = $(this);
    var routetripid = button.attr('data-routetrip');
    if (checkValue(routetripid)) {
        LoadRouteTripDetails(routetripid);
    }
});
function formatSVTime(value_time) {
    try {
        var time = moment(value_time);
        if (time._isValid) {
            if (time.year() === 1) {
                return "";
            }
            return time.format("HH:mm");
        }
    } catch (e) {
        console.log(e);
    }
}

function LoadRouteTripDetails(id) {
    try {

        fotfmanager.server.getRouteTripsInfo(id).done(function (routetrip) {
            if (routetrip.length > 0) {
                routetrip = routetrip[0];
                let DirectionInd = routetrip.tripDirectionInd === "I" ? "Inbound Trip Details" : "Outbound Trip Details";
                $('#modalRouteTripDetailsHeader_ID').text(DirectionInd);
                $('#RouteTripDetailscard').empty();
                if (routetrip.tripDirectionInd === "I" ) {
                    $('#RouteTripDetailscard').append(routedetailsInform_template.supplant(routedetailsform(routetrip)));
                }
                else {
                    $('#RouteTripDetailscard').append(routedetailsOutform_template.supplant(routedetailsform(routetrip)));
                }
  
            }
        });
        $("#RouteTripDetails_Modal").modal();
    } catch (e) {
        console.log(e);
    }
}
function routedetailsform(properties) {
    return $.extend(properties, {
        routetrip: properties.route +"-"+ properties.trip  ,
        servicetype: GetServiceTypeCode(properties.serviceTypeCode),
        legnumber: properties.legNumber,
        legdestination: properties.legSiteName + " (" + properties.legSiteId + ")",
        finaldestination: properties.destSiteName + " (" + properties.destSiteId + ")",
        legorigin: properties.legSiteName + " (" + properties.legSiteId + ")",
        initorigin: properties.destSiteName + " (" + properties.destSiteId + ")",
        schedtime: formatSVmonthdayTime(properties.scheduledDtm),
        actualtime: formatSVmonthdayTime(properties.actualDtm),
        loadstarttime: formatSVmonthdayTime(properties.LoadUnldStartDtm),
        loadendtime: formatSVmonthdayTime(properties.LoadUnldEndDtm),
        unloadstarttime: formatSVmonthdayTime(properties.loadUnldStartDtm),
        unloadendtime: formatSVmonthdayTime(properties.loadUnldEndDtm),
        doortime: formatSVmonthdayTime(properties.doorDtm),
        gpstime: formatSVmonthdayTime(properties.gpsSiteDtm),
        doornumber: checkValue(properties.doorNumber) ? properties.doorNumber : "",
        vannumber: checkValue(properties.vanNumber) ? properties.vanNumber :"",
        trailernumber: checkValue(properties.trailerBarcode) ? properties.trailerBarcode : "",
        trailerlength: checkValue(properties.trailerLengthCode) ? properties.trailerLengthCode : "",
        origgpssource: checkValue(properties.gpsIdSource) ? properties.gpsIdSource : "",
        origgpsid: checkValue(properties.gpsId) ? properties.gpsId : "",
        destgpssource: checkValue(properties.gpsIdSource) ? properties.gpsIdSource : "",
        destgpsid: checkValue(properties.gpsId) ? properties.gpsId : "",
        drivername: checkValue(properties.driverFirstName) ? properties.driverFirstName + " " + (checkValue(properties.driverLastName) ? properties.driverLastName : "") : "",
        driverphone: checkValue(properties.driverPhoneNumber) ? properties.driverPhoneNumber : "",
        msp: checkValue(properties.mspBarcode) ? properties.mspBarcode : "",
        originseal: checkValue(properties.originSeal) ? properties.originSeal : "",
        destseal: checkValue(properties.destSeal) ? properties.destSeal : "",
        delayreason: checkValue(properties.delayCode) ? properties.delayCode : "",
        origcomment: checkValue(properties.destComments) ? properties.destComments : "",
        destcomment: checkValue(properties.originComments) ? properties.originComments : "",
    });
}
function GetServiceTypeCode(data)
{
    try {
        switch (data) {
            case "A":
                return "Rail";
            case "D":
                return "Drop Shipment";
            case "H":
                return "HCR";
            case "P":
                return "Periodicals";
            case "V":
                return "PVS";
            default:
                return "";
        }
    } catch (e) {
        console.log(e)
    }
}
let routedetailsInform_template =
    '<div class="row">' +
    '<div class="col-sm-7">' +
            '<div class="card pb-0">' +
                    '<div class="card-body pb-0">' +
                                '<form style="float:left" class="panelForms">' +
                                '<label>Route-Trip:</label> {routetrip}' +
                                '<br>' +
                                '<label>Service Type:</label> {servicetype} ' +
                                '<label style="text-align: right;" class="checkBox">Leg:</label> {legnumber}' +
                                '<br><label>Leg Origin:</label> {legorigin}' +
                                '<br><label>Initial Origin:</label> {initorigin}' +
                                '<br><label>Sched Arr Time:</label> {schedtime}' +
                                '<br>' +
                                '<label>Door:</label> {doornumber}' +
                                '<br>                     ' +
                                '<label>Van Number:</label> {vannumber}' +
                                '<br>' +
                                '<label>Trailer:</label> {trailernumber}' +
                                '<br>' +
                                '<label>Trailer Length:</label> {trailerlength}' +
                                '<br>' +
                                '<br>' +
                                '<label>Origin GPS Source:</label> {origgpssource}' +
                                '<br>' +
                                '<label>Origin GPS ID:</label> {origgpsid}' +
                                '<br>' +
                                '<label>Dest GPS Source:</label> {destgpssource}' +
                                '<br>' +
                                '<label>Dest GPS ID:</label>  {destgpsid}' +
                                '<br>' +
                                '<label>Driver Name:</label> {drivername}' +
                                '<br>' +
                                '<label>Driver Phone Number:</label> {driverphone}' +
                                '<br>' +
                                '<label> MSP: </label> {msp}' +
                                '<br>' +
                                '<label >Origin Seal:</label> {originseal}' +
                                '<br>' +
                                '<label>Destination Seal:</label> {destseal}' +
                                '<br>' +
                                '<br>' +
                                '<label>Delay Reason</label> {delayreason}' +
                                '<br>' +
                                '<br>' +
                                '<label style="width: auto;">Origin Comments:</label> {origcomment}' +
                                '<br>' +
                                '<br>' +
                                '<label style="width: auto;">Destination Comments:</label> {destcomment}' +
                                '</form>' +
                    '</div>' +
            '</div>' +
    '</div>' +
    '<div class="col-sm-5">' +
        '<div class="card pb-0">' +
            '<div class="card-body pb-0">' +
                '<form style="float:left" class="panelForms">' +
                '<br><label>Door Arr Time:</label> {doortime}' +
                '<br><label>Manual Site Arr Time:</label> {actualtime}' +
                '<br><label>GPS Site Arr Time:</label> {gpstime}' +
                '<br><label>Actual Arr Time:</label> {actualtime}' +
                '</form>' +
            '</div>' +
        '</div>' +
    '</div>' +
    '</div>';

let routedetailsOutform_template =
    '<div class="row">' +
    '<div class="col-sm-7">' +
            '<div class="card pb-0">' +
                    '<div class="card-body">' +
                            '<form style="float:left" class="panelForms">' +
                            '<label>Route-Trip:</label> {routetrip}' +
                            '<br>' +
                            '<label>Service Type:</label> {servicetype} ' +
                            '<label style="text-align: right;" class="checkBox">Leg:</label> {legnumber}' +
                            '<br><label>Leg Destination:</label> {legdestination}' +
                            '<br><label>Final Destination:</label> {finaldestination}' +
                            '<br><label>Sched Dept Time:</label> {schedtime}' +
                            '<br>' +
                            '<label>Door:</label> {doornumber}' +
                            '<br>                     ' +
                            '<label>Van Number:</label> {vannumber}' +
                            '<br>' +
                            '<label>Trailer:</label> {trailernumber}' +
                            '<br>' +
                            '<label>Trailer Length:</label> {trailerlength}' +
                            '<br>' +
                            '<label>Origin GPS Source:</label> {destgpsid}' +
                            '<br>' +
                            '<label>Origin GPS ID:</label> {destgpsid}' +
                            '<br>' +
                            '<label>Dest GPS Source:</label> {destgpsid}' +
                            '<br>' +
                            '<label>Dest GPS ID:</label> {destgpsid}' +
                            '<br>' +
                            '<label >Driver Name:</label> {drivername}' +
                            '<br>' +
                            '<label>Driver Phone Number:</label> {driverphone}' +
                            '<br>' +
                            '<label> MSP: </label> {msp}' +
                            '<br>' +
                            '<label >Origin Seal:</label> {originseal}' +
                            '<br>' +
                            '<br>' +
                            '<label style="width: auto;">Origin Comments:</label>' +
                            '</form>' +
                    '</div>' +
            '</div>' +
    '</div>' +
        '<div class="col-5">' +
            '<div class="card pb-0">' +
                '<div class="card-body">' +
                    '<form style="float:left" class="panelForms">' +
                    '<br><label>Actual Dept Time:</label> {actualtime}' +
                    '<br><label>Door Dep Time:</label> {doortime}' +
                    '<br><label>Manual Site Dep Time:</label> {actualtime}' +
                    '<br><label>GPS Site Dep Time:</label> {gpstime}' +
                    '</form>' +
                '</div>' +
            '</div>' +
        '</div>' +
    '</div>';

let search_Table = $('table[id=tagresulttable]');
async function startSearch(sc) {
    try {
        let search = new RegExp(sc, 'i');
        let search_Table_Body = search_Table.find('tbody');
        search_Table_Body.empty();
        if (checkValue(sc)) {
            if (map.hasOwnProperty("_layers")) {
                $.map(map._layers, function (layer, i) {
                    if (layer.hasOwnProperty("feature")) {
                        if (/(person)|(Vehicle$)/i.test(layer.feature.properties.Tag_Type)) {
                            if (search.test(layer.feature.properties.name) || search.test(layer.feature.properties.id)) {
                                search_Table_Body.append(search_row_template.supplant(formatsearchrow(layer.feature.properties, layer._leaflet_id)));
                                if (layer.hasOwnProperty("options") && layer.options.hasOwnProperty("fillColor")) {

                                    if (!/red/i.test(layer.options.fillColor)) {
                                        layer.setStyle({
                                            fillColor: '#dc3545', //'red',
                                        });
                                    }
                                }
                                if (layer.hasOwnProperty("_tooltip") && layer._tooltip.hasOwnProperty("_container")) {

                                    if (layer._tooltip._container.classList.contains('tooltip-hidden')) {
                                        layer._tooltip._container.classList.remove('tooltip-hidden');
                                    }
                                    if (!layer._tooltip._container.classList.contains('searchflash')) {
                                        layer._tooltip._container.classList.add('searchflash');
                                    }
                                }
                            }
                            else {
                                if (!layer.feature.properties.hasOwnProperty("tagVisible")) {
                                    if (!layer._tooltip._container.classList.contains('tooltip-hidden')) {
                                        layer._tooltip._container.classList.add('tooltip-hidden');
                                    }
                                }
                                if (layer.hasOwnProperty("_tooltip")) {
                                    if (layer._tooltip.hasOwnProperty("_container")) {
                                        if (layer._tooltip._container.classList.contains('searchflash')) {
                                            layer._tooltip._container.classList.remove('searchflash');
                                        }
                                    }
                                }
                            }
                        }
                    }
                })
            }
        }
        else {
            if (map.hasOwnProperty("_layers")) {
                $.map(map._layers, function (layer, i) {
                    if (layer.hasOwnProperty("feature")) {
                        if (/(person)|(Vehicle$)/i.test(layer.feature.properties.Tag_Type)) {
                            if (layer.hasOwnProperty("_tooltip") && layer._tooltip.hasOwnProperty("_container")) {
                                if (layer._tooltip._container.classList.contains('searchflash')) {
                                    layer._tooltip._container.classList.remove('searchflash');
                                }
                                if (!layer.feature.properties.hasOwnProperty("tagVisible")) {
                                    if (!layer._tooltip._container.classList.contains('tooltip-hidden')) {
                                        layer._tooltip._container.classList.add('tooltip-hidden');
                                    }
                                }
                            }
                        }
                    }
                })
            }
        }
    } catch (e) {
        console.log(e);
    }
}
let search_row_template = '<tr data-id="{layer_id}" data-tag="{tag_id}">' +
    '<td><span class="ml-p25rem">{tag_name}</span></td>' +
    '<td class="align-middle text-center">' +
    '<button class="btn btn-light btn-sm mx-1 pi-iconEdit tagedit" name="tagedit"></button>' +
    '</td>' +
    '</tr>'
    ;
function formatsearchrow(properties, layer_id) {
    return $.extend(properties, {
        layer_id: layer_id,
        tag_id: properties.id,
        tag_name: checkValue(properties.name) ? properties.name : properties.id
    })
}

/**
* this is use to setup a the camera information and other function
*
* **/
$.extend(fotfmanager.client, {
    updateCameraStatus: async (cameraupdatesnew, id) => { CameraDataUpdate(cameraupdatesnew, id);  }
});
var brightRed = 255;
var darkRed = 200;
function drawAlertText(ctx, txt, font, viewWidth, viewHeight, x, y, txtBGColor, txtColor, bgHeight) {
    let padding = 4;
    y = y - 10 - bgHeight;
    ctx.save();
    ctx.font = font;
    /// draw text from top
    ctx.textBaseline = 'top';
    ctx.fillStyle = txtBGColor;
    var width = ctx.measureText(txt).width;
    var totalWidth = width + (padding * 2);
    var totalHeight = bgHeight + (padding * 2);
    if (x < 5) {
        x = 5;
    }
    if (x > (viewWidth - totalWidth - 5)) {
        x = viewWidth - totalWidth - 5;
    }
    if (y < 35) {
        y = 35;
    }
    if (y > (viewHeight - totalHeight - 5)) {
        y = viewHeight - totalHeight - 5;
    }
    ctx.fillRect(x - padding, y - padding, totalWidth, totalHeight);
    ctx.fillStyle = txtColor;
    ctx.fillText(txt, x, y);
}
let cameraupdates = [];
let allcameraslist = [];
let flashRate = 3000;
let flashCheckRate = 250;
let lastAlertBlinkChange = 0;
let lastAlertStatus = null;
let alertTurnoffThreshold = 60 * 1000;
let alertsOn = false; 
let camerathumbnailsupdating = false;
setInterval(() => {
    let thisTime = Date.now();
    let timeSinceLastUpdateBlink = thisTime - lastAlertBlinkChange;
    if (timeSinceLastUpdateBlink >= (flashRate - flashCheckRate)) {
        // keep waiting until update is done
        if (camerathumbnailsupdating) return;
        camerathumbnailsupdating = true;
        let timeSinceLastUpdate = thisTime - lastUpdateCamera;
        if (timeSinceLastUpdate >= alertTurnoffThreshold) {
            alertsOn = false;
        }
        else {
            alertsOn = true;
        }
        updateAllCameras(thisTime);
        camerathumbnailsupdating = false;
    }
}, flashCheckRate);
let lastUpdateCamera = 0;
function alertChanged(prevDarvisAlerts, darvisAlerts) {
    if (prevDarvisAlerts && !darvisAlerts) return true;
    if (!prevDarvisAlerts && darvisAlerts) return true;
    return JSON.stringify(prevDarvisAlerts) != JSON.stringify(darvisAlerts);
}
function CameraDataUpdate(cameradataUpdate, id) {
    try {
        if (id == baselayerid) {
            map.whenReady(() => {
                if (map.hasOwnProperty("_layers")) {
                    $.map(map._layers, function (layer, i) {
                        if (layer.hasOwnProperty("feature") && layer.feature.properties.id === cameradataUpdate.properties.id) {
                            let img = new Image();
                            //load Base64 image
                            img.src = cameradataUpdate.properties.Camera_Data.base64Image;
                            var mapsize = map.getZoom();
                            var iconsizeh = 64;
                            var iconsizew = 48
                            if (mapsize > 2) {
                                iconsizeh = 64 * mapsize;
                                iconsizew = 48 * mapsize;
                            }
                            let thumbnailsReplc = L.icon({
                                    iconUrl: img.src,
                                    iconSize: [iconsizeh, iconsizew], // size of the icon
                                    iconAnchor: [0, 0], // point of the icon which will correspond to marker's location
                                    shadowAnchor: [0, 0],  // the same for the shadow
                                    popupAnchor: [0, 0] // point from which the popup should open relative to the iconAnchor
                            });
                            layer.setIcon(thumbnailsReplc);
                           const index = allcameraslist.findIndex(object => object.properties.id === cameradataUpdate.properties.id);
                           if (index === -1) {allcameraslist.push(cameradataUpdate);}
                           else {allcameraslist[index].properties.Camera_Data.base64Image = cameradataUpdate.properties.Camera_Data.base64Image;}
                           return false;
                        }
                    });
                }
            });
        }
    } catch (e) {
        console.log(e);
    }
}
function UpdateCameraZoom() {
    for (var i = 0, l = allcameraslist.length; i < l; i++) {
        CameraDataUpdate(allcameraslist[i], allcameraslist[i].properties.floorId);
    }
}
function removecamfromalllist(markerID) {
    const index = allcameraslist.findIndex(object => object.properties.id === markerID);
    allcameraslist.splice(index, 1);
}
function addCameraUpdate(allcameras) {
    if (camerathumbnailsupdating) {
        // if the thumbnails are updating, ignore  this update to avoid flicker,
        // next update will reflect the data so it's okay
        return;
    }
    let cameraupdatescopy = JSON.parse(JSON.stringify(cameraupdates));
    for (const tuple of allcameras) {
        let cameraupdate = tuple.Item1;
        let id = tuple.Item2;
        var changed = false;
            try {
                cameraupdate.baselayerid = id;
                var foundIndex = -1;
                for (var i = 0; i < cameraupdatescopy.length; i++) {
                    if (cameraupdatescopy[i].properties.id === cameraupdate.properties.id) {
                        foundIndex = i;
                        break;
                    }
                }
                if (foundIndex == -1) {
                    cameraupdate.properties.lastNoAlert = 0;
                    cameraupdatescopy.push(cameraupdate);
                    changed = true;
                }
                else {
                    cameraupdate.properties.lastNoAlert = cameraupdatescopy[foundIndex].properties.lastNoAlert;
                    let compare1 = JSON.parse(JSON.stringify(cameraupdatescopy[foundIndex].properties.DarvisAlerts));
                    cameraupdatescopy[foundIndex] = cameraupdate;
                    changed = alertChanged(compare1, cameraupdate.properties.DarvisAlerts);
                }
            }
            catch (e) {
                console.log(e.message);
        }
        if (changed) {

            lastUpdateCamera = Date.now();
        }
    }
    cameraupdates = cameraupdatescopy;
}
async function getLayersAndIcons(datePassed) {
        
        let layersAndIconsUpdate = [];
        if (cameraupdates.length > 0) {
            for (var camera of cameraupdates) {

                try {
                    var layerAndIcon = await updateCameras(camera, camera.baselayerid, datePassed);
                    if (layerAndIcon !== null) {
                        layersAndIconsUpdate.push(layerAndIcon);
                    }
                }
                catch (e) {
                    console.log(e.message);
                }
            }
        }
        return layersAndIconsUpdate;
       
}
function updateAllCameras(datePassed) {
    getLayersAndIcons(datePassed).then((layersAndIconsUpdate) => {

        for (const layerAndIconToUpdate of layersAndIconsUpdate) {

            let layer = layerAndIconToUpdate[0];
            let icon = layerAndIconToUpdate[1];
            layer.setIcon(icon);
           
        }
    }).catch((e) => {
        console.log(e.message);
    });
}
async function updateCameras(cameraupdate, id, datePassed) {
    return new Promise((resolve, reject) => {
        try {
            if (id == baselayerid) {
                if (cameras.hasOwnProperty("_layers")) {
                    $.map(cameras._layers, async function (layer) {
                        if (layer.hasOwnProperty("feature")) {
                            if (layer.feature.properties.id === cameraupdate.properties.id) {

                                if (cameraupdate.properties.id === openCameraId) {
                                    webCameraViewData = JSON.parse(JSON.stringify(cameraupdate.properties));
                                }
                                if (alertsOn && cameraupdate.properties.DarvisAlerts &&
                                    cameraupdate.properties.DarvisAlerts.length > 0) {
                                    let red = brightRed;
                                    if (datePassed % (flashRate * 2) > flashRate) {
                                        red = darkRed;
                                        if (lastAlertStatus === 1) {
                                            lastAlertBlinkChange = datePassed;
                                        }
                                        lastAlertStatus = 0;
                                    }
                                    else {
                                        if (lastAlertStatus === 0) {
                                            lastAlertBlinkChange = datePassed;
                                        }
                                        lastAlertStatus = 1;
                                    }
                                    var base64Image = (cameraupdate.properties.base64Image ===
                                        "" ? "../../Content/images/NoImage.png" :
                                        cameraupdate.properties.base64Image);

                                    let img = await highlightCameraAlert(base64Image, red, 0, 0, 20);
                                    var mapsize = map.getZoom();
                                    var iconsizeh = 64;
                                    var iconsizew = 48
                                    if (mapsize > 2) {
                                        iconsizeh = 64 * mapsize;
                                        iconsizew = 48 * mapsize;
                                    }
                                    var locaterIcon = L.icon({
                                        iconUrl: img,
                                        iconSize: [iconsizeh, iconsizew], // size of the icon
                                        iconAnchor: [0, 0], // point of the icon which will correspond to marker's location
                                        shadowAnchor: [0, 0],  // the same for the shadow
                                        popupAnchor: [0, 0] // point from which the popup should open relative to the iconAnchor

                                    });
                                    resolve([layer, locaterIcon]);
                                }
                                else {
                                    var base64Image = (cameraupdate.properties.base64Image ===
                                        "" ? "../../Content/images/NoImage.png" : cameraupdate.properties.base64Image);
                                    var mapsize = map.getZoom();
                                    var iconsizeh = 64;
                                    var iconsizew = 48
                                    if (mapsize > 2) {
                                        iconsizeh = 64 * mapsize;
                                        iconsizew = 48 * mapsize;
                                    }
                                    var locaterIcon = L.icon({
                                        iconUrl: base64Image,
                                        iconSize: [iconsizeh, iconsizew], // size of the icon
                                        iconAnchor: [0, 0], // point of the icon which will correspond to marker's location
                                        shadowAnchor: [0, 0],  // the same for the shadow
                                        popupAnchor: [0, 0] // point from which the popup should open relative to the iconAnchor
                                    });
                                    resolve([layer, locaterIcon]);
                                }
                            }
                        }
                    });
                }
            }
            else {
                resolve(null);
            }

        } catch (e) {
            reject(e);
        }
    });
}
//on close clear all inputs
$('#Camera_Modal').on('hidden.bs.modal', function () {
    camera_modal_body = $('div[id=camera_modalbody]');
    camera_modal_body.empty();
});
// used to get alert boundaries
var getAlertBoundingBox = async (Alerts, width, height) => {
    return new Promise((resolve, reject) => {
            var canvas = document.createElement('canvas');
            canvas.width = width;
            canvas.height = height;
        var context = canvas.getContext('2d');
        if (alertsOn && Alerts && Alerts.length > 0) {
            var alert = null;
            for (alert of Alerts) {
                let left = alert.TOP * scaleWidth;
                let right = alert.BOTTOM * scaleWidth;
                let top = alert.LEFT * scaleHeight;
                let bottom = alert.RIGHT * scaleHeight;
                context.strokeStyle = "red";
                context.lineWidth = 5;
                context.beginPath();
                context.rect(left,  top,
                    (right - left), (bottom - top));
                context.stroke();
            }
            for (alert of Alerts) {
                let left = alert.TOP * scaleWidth;
                let right = alert.BOTTOM * scaleWidth;
                let top = alert.LEFT * scaleHeight;
                let bottom = alert.RIGHT * scaleHeight;
                let text = alert.TYPE + ": " + getHHMMSSFromSeconds(alert.DWELL_TIME);

                var posX = left;
                var posY = top;
                drawAlertText(context, text, "bold 18px Arial", width, height, posX,
                    posY, "#fff", "#000", 18, 4);
            }
        }
            resolve(canvas.toDataURL());
    });
}

var exclamation = new Image();
var exclamationImage = null;
var exclamationSize = 100;
var brightRedExclamation = new Image();
var darkRedExclamation = new Image();
var brightRedLoaded = false;
var darkRedLoaded = false;
brightRedExclamation.onload = function () {
    brightRedLoaded = true;
}
darkRedExclamation.onload = function () {
    darkRedLoaded = true;
}
exclamation.onload = function () {
    for (var i = 0; i <= 1; i++) {
        let red = brightRed;
        if (i === 1) {
            red = darkRed;
        }
    let canvas = document.createElement('canvas');
    canvas.width = exclamationSize;
        canvas.height = exclamationSize;

        let toCanvas = document.createElement('canvas');
        toCanvas.width = exclamationSize;
        toCanvas.height = exclamationSize;
        var context = canvas.getContext('2d');
        var toContext = toCanvas.getContext('2d');
        context.drawImage(exclamation, 0, 0, exclamationSize, exclamationSize);
    var imgData = context.getImageData(0, 0, exclamationSize, exclamationSize);
        var newImgData = toContext.getImageData(0, 0, exclamationSize, exclamationSize);
    for (var x = 0; x < exclamationSize; x++) {
        for (var y = 0; y < exclamationSize; y++) {
            var pixelStartPosition = (y * exclamationSize * 4) + (x * 4);
            if (imgData.data[pixelStartPosition]
                < 255 && imgData.data[pixelStartPosition + 3] > 0) {
                newImgData.data[pixelStartPosition] = red;
                newImgData.data[pixelStartPosition + 1] = 0;
                newImgData.data[pixelStartPosition + 2] = 0;
                newImgData.data[pixelStartPosition + 3] = 255;
            }
            else {
                newImgData.data[pixelStartPosition] = imgData.data[pixelStartPosition];
                newImgData.data[pixelStartPosition + 1] = imgData.data[pixelStartPosition + 1];
                newImgData.data[pixelStartPosition + 2] = imgData.data[pixelStartPosition + 2];
                newImgData.data[pixelStartPosition + 3] = imgData.data[pixelStartPosition + 3];
            }
 }
        }
        toContext.putImageData(newImgData, 0, 0);
        if (red == brightRed) {
            brightRedExclamation.src = toCanvas.toDataURL();
        }
        else {
            darkRedExclamation.src = toCanvas.toDataURL();
        }
    }
    var done = true;
}
/*exclamation.src = "../../Content/images/warning-signal.png";*/
var imageManipCanvas = document.createElement('canvas');
var highlightCameraAlert = async (base64Image, r, g, b, borderWidth) => {
    return new Promise((resolve, reject) => {
        var image = new Image();
        image.onerror = function () {
            reject("failed to load image");
        }
        image.onload = function () {
            let canvas = imageManipCanvas;
            canvas.width = image.width;
            canvas.height = image.height;
            var context = canvas.getContext('2d');
            context.drawImage(image, 0, 0);
            var alertColor = "rgb(" + r + ", " + g + ", " + b + ")";
            context.strokeStyle = alertColor;
            context.lineWidth = borderWidth;
            var warningHeight = "64px";
            context.strokeRect(borderWidth / 2, borderWidth / 2, image.width - 1 - borderWidth,
                image.height - 1 - borderWidth);
            if (r === brightRed && brightRedLoaded) {
                context.drawImage(brightRedExclamation, (image.width * .667) -
                    (exclamationSize / 2), (image.height * .333) - (exclamationSize / 2));
            }
            else if (darkRedLoaded) {
                context.drawImage(darkRedExclamation, (image.width * .667) -
                    (exclamationSize / 2), (image.height * .333) - (exclamationSize / 2));
            }
            resolve(canvas.toDataURL());
        };
        image.src = base64Image;
    });
}
let cameras = new L.GeoJSON(null, {
    pointToLayer: function (feature, latlng) {
        let img = new Image();
        //load Base64 image
        img.src = feature.properties.Camera_Data.base64Image;
        var mapsize = map.getZoom();
        var iconsizeh = 64;
        var iconsizew = 48
        if (mapsize > 2) {
            iconsizeh = 64 * mapsize;
            iconsizew = 48 * mapsize;
        }
        let locaterIcon = L.icon({
                iconUrl: img.src,
                iconSize: [iconsizeh, iconsizew], // size of the icon
                iconAnchor: [0, 0], // point of the icon which will correspond to marker's location
                shadowAnchor: [0, 0],  // the same for the shadow
                popupAnchor: [0, 0] // point from which the popup should open relative to the iconAnchor
        });
        const index = allcameraslist.findIndex(object => object.properties.id === feature.properties.id);
        if (index === -1) { allcameraslist.push(feature); }
        return L.marker(latlng, {
            icon: locaterIcon,
            title: feature.properties.empName,
            riseOnHover: true,
            bubblingMouseEvents: true,
            popupOpen: true
        });
    },
    onEachFeature: function (feature, layer) {
        layer.on('click', function (e) {
            View_Web_Camera(feature.properties);
        });
        layer.bindTooltip(feature.properties.empName, {
            permanent: true,
            interactive: true,
            direction: 'top',
            opacity: 1,
            className: 'location'
        }).openTooltip();
    }
})
function formatCameralayout(camera) {
    return $.extend(camera, {
        base64Background: camera.base64Image,
        camera_ip: camera.CAMERA_NAME,
        camera_description: camera.DESCRIPTION,
        camera_model: getModel(camera.MODEL_NUM)
    });
}
let imageWidth = 1280;
let imageHeight = 720;
let scaleWidth = imageWidth / 1920;
let scaleHeight = imageHeight / 1080;
let extraOffset = 5;
let verticalOffset = (-  (imageHeight + extraOffset)) + "px";
let camera_Table , camera_modal_body;
let camera_layout = '<div class="frameFlex">' +
    '<div style="overflow: scroll">' +
    '<div style="width: ' + imageWidth + '; height: ' + imageHeight + '; ">' +
    '<iframe id="cameraIframe" src="http://{camera_ip}/mjpg/video.mjpg?camera={camera_model}"' +
    ' scrolling="no" width= ' + imageWidth + ' height= ' + imageHeight + '>' +
    '</iframe> ' +
    '<div style="z-index: 10000000; margin-top: ' + verticalOffset + ' ">' +
    '<img id="openCameraOverlay" src={base64Background} width=' +
    imageWidth + ' height=' + imageHeight + 
    ' style="width: ' + imageWidth + ' height: ' + imageHeight + ' "/>' +
    '</div></div></div>';
let camera_row_template = '<tr data-id="{camera_ip}" data-model="{camera_model}" data-description="{camera_description}" id="_{camera_ip}">' +
    '<td class="align-middle">{camera_ip}</td>' +
    '<td class="align-middle">{camera_description} </td>' +
    '<td class="d-flex align-middle justify-content-center">' +
    '<button class="btn btn-light btn-sm mx-1 bi-camera-fill camera_view"></button>' +
    '</td>' +
    '</tr>';
function getModel(MODEL_NUM) {
    let model = 1;
    switch (MODEL_NUM) {
        case 3719:
            model = 'quad'
            break
        case 3727:
            model = '5'
            break
        default:
            model =  '1'
            break
    }
    return model;
}
function formatwebcameralayout(id, model, description, base64Image) {
    return $.extend(id, model, description, {
        base64Background: base64Image,
        camera_ip: id,
        camera_description: description,
        camera_model: getModel(model)
    });
}
var webCameraViewData = null;
function updateBoundingBox() {
    var Data = webCameraViewData;
    // uncommented until we understand how Darvis coordinates translate to the video stream coordinates
    //getAlertBoundingBox(Data.DarvisAlerts, imageWidth, imageHeight).then((img) => {
    //// getAlertBoundingBox(null, 1, 1).then((img) => {
    //    document.getElementById("openCameraOverlay").src = img;
    //});
}
var boundingInterval = null;
var openCameraId = null;
function View_Web_Camera(Data) {
    try {
        webCameraViewData = Data;
        openCameraId = Data.id;
        $('#cameramodalHeader').text('View Web Camera');
        var cameraname = checkValue(Data.empName) ? Data.empName : Data.name;
        $('#cameradescription').text(cameraname);
        camera_modal_body = $('div[id=camera_modalbody]');
        camera_modal_body.empty();
        
        if (boundingInterval) {
            boundingInterval = null;
        }
        boundingInterval = setInterval(() => { updateBoundingBox(); }, 1000);
        // to be uncommented later when Alert coordinates are able to match the video stream
        getAlertBoundingBox(Data.DarvisAlerts, imageWidth, imageHeight).then((img) => {
       //  getAlertBoundingBox(null, 1, 1).then((img) => {
            camera_modal_body.append(camera_layout.supplant(formatwebcameralayout(Data.name, Data.emptype, Data.empName,
                img)));
            $('#Camera_Modal').modal();
            sidebar.close('');
        });
    } catch (e) {
        $("#error_camera").text(e);
        console.log(e);
    }
}
/*uses this for geometry editing  */
$('#Zone_Modal').on('hidden.bs.modal', function () {
    $(this)
        .find("input[type=text],textarea,select")
        .val('')
        .end()
        .find("span[class=text]")
        .val('')
        .text("")
        .end()
        .find('input[type=checkbox]')
        .prop('checked', false).change()
        .end();
});
//remove layers
$('#Remove_Layer_Modal').on('hidden.bs.modal', function () {
    $(this)
        .find("input[type=text],textarea,select")
        .val('')
        .end()
        .find("span[class=text]")
        .val('')
        .text("")
        .end()
        .find('input[type=checkbox]')
        .prop('checked', false).change()
        .end();
});
$('#Remove_Layer_Modal').on('shown.bs.modal', function () {
});

function addCreatedZoneToMap(newZone) {
    switch (newZone.properties.Zone_Type) {
        case "AGVLocation":
            agvLocations.addData(newZone);
            break;
        case "Machine":
            polygonMachine.addData(newZone);
            break;
        case "Bullpen":
            stagingBullpenAreas.addData(newZone);
            break;
        case "ViewPorts":
            viewPortsAreas.addData(newZone);
            break;
        case "Bin":
            binzonepoly.addData(newZone);
            break;
        case "DockDoor":
            dockDoors.addData(newZone);
            break;

    }
}
////this for later
function init_geometry_editing() {
    var draw_options = {
        position: 'bottomright',
        oneBlock: false,
        snappingOption: false,
        drawRectangle: true,
        drawMarker: true,
        drawPolygon: true,
        drawPolyline: false,
        drawCircleMarker: false,
        drawCircle: false,
        editMode: false,
        cutPolygon: false,
        dragMode: false,
        removalMode: true
    };
    map.pm.addControls(draw_options);
    map.on('pm:create', (e) => {
        hideSidebarLayerDivs();
        $('div[id=layer_div]').css('display', 'block');
        $('select[name=zone_type]').prop('disabled', false);
        VaildateForm("");
        if (e.shape === "Polygon") {
            $('select[name=zone_type] option[value=Area]').remove();
            $('select[name=zone_type] option[value=AGVLocation]').remove();
            $('select[name=zone_type] option[value=Bin]').remove();
            $('select[name=zone_type] option[value=Camera]').remove();
            $('select[name=zone_type] option[value=DockDoor]').remove();
            $('select[name=zone_type] option[value=Machine]').remove();
            $('select[name=zone_type] option[value=Bullpen]').remove();
            $('select[name=zone_type] option[value=ViewPorts]').remove();
            $('<option/>').val("Area").html("Area Zone").appendTo('select[id=zone_type]');
            $('<option/>').val("AGVLocation").html("AGV Location Zone").appendTo('select[id=zone_type]');
            $('<option/>').val("Machine").html("Machine Zone").appendTo('select[id=zone_type]');
            $('<option/>').val("Bullpen").html("Staging Bullpen Zone").appendTo('select[id=zone_type]');
            $('<option/>').val("ViewPorts").html("View Ports").appendTo('select[id=zone_type]');
            $('<option/>').val("Bullpen").html("Bullpen Zone").appendTo('select[id=zone_type]');
            CreateZone(e);
            sidebar.open('home');
        }
        if (e.shape === "Rectangle") {
            $('select[name=zone_type] option[value=Area]').remove();
            $('select[name=zone_type] option[value=AGVLocation]').remove();
            $('select[name=zone_type] option[value=Bin]').remove();
            $('select[name=zone_type] option[value=Camera]').remove();
            $('select[name=zone_type] option[value=DockDoor]').remove();
            $('select[name=zone_type] option[value=Machine]').remove();
            $('select[name=zone_type] option[value=ViewPorts]').remove();
            $('select[name=zone_type] option[value=Bullpen]').remove();
            $('<option/>').val("Bin").html("BIN Zone").appendTo('select[id=zone_type]');
            $('<option/>').val("DockDoor").html("Dock Door Zone").appendTo('select[id=zone_type]');
           
            CreateBinZone(e);
            sidebar.open('home');
        }
        if (e.shape === "Marker") {
            $('select[name=zone_type] option[value=Area]').remove();
            $('select[name=zone_type] option[value=AGVLocation]').remove();
            $('select[name=zone_type] option[value=Bin]').remove();
            $('select[name=zone_type] option[value=Camera]').remove();
            $('select[name=zone_type] option[value=DockDoor]').remove();
            $('select[name=zone_type] option[value=Machine]').remove();
            $('select[name=zone_type] option[value=ViewPorts]').remove();
            $('select[name=zone_type] option[value=Bulllpen]').remove();
            $('<option/>').val("Camera").html("Camera").appendTo('select[id=zone_type]');
            CreateCamera(e);
            sidebar.open('home');
        }
        $('button[id=zonecloseBtn][type=button]').off().on('click', function () {
            sidebar.close();
            e.layer.remove();
        })
    });
    map.on('pm:edit', (e) => {
        if (e.shape === 'Marker') {
            VaildateForm("");
            e.layer.bindPopup().openPopup();
        }
    });
    map.on('pm:remove', (e) => {
        if (e.shape === 'Marker') {
            VaildateForm("");
            RemoveMarkerItem(e);
        }
        else {
            VaildateForm("");
            RemoveZoneItem(e);
        }
    });
    //zone type
    $('select[name=zone_type]').change(function () {
        if (!checkValue($('select[name=zone_type]').val())) {
            $('select[name=zone_type]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_zone_type]').text("Please Select Zone Type");
        }
        else {
            $('select[name=zone_type]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_zone_type]').text("");
            VaildateForm($('select[name=zone_type] option:selected').val());
        }
    });
    //mpe select
    $('select[name=zone_select_name]').change(function () {
        if (!checkValue($('select[name=zone_select_name]').val())) {
            $('select[name=zone_select_name]').removeClass('is-valid').addClass('is-invalid');
            $('input[type=text][name=zone_name]').val("")
            $('span[id=error_zone_name]').text("Please Enter Name");
         
        }
        else {
            $('select[name=zone_select_name]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_zone_name]').text("");
            $('input[type=text][name=zone_name]').removeClass('is-invalid').addClass('is-valid');

        }
        if ($('select[name=zone_type] option:selected').val() === "Bullpen") {

            enableSVZoneSubmit();
        }
    });
    //bins name
    $('textarea[id=bin_bins]').keyup(function () {
        if (!checkValue($('textarea[id=bin_bins]').val())) {
            $('textarea[id=bin_bins]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_bin_bins]').text("Please Enter Bin Numbers");
        }
        else {
            $('textarea[id=bin_bins]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_bin_bins]').text("");
        }
        enableBinZoneSubmit();
    });
   //zone name
    $('input[type=text][name=zone_name]').keyup(function () {
        if (!checkValue($('input[type=text][name=zone_name]').val())) {
            $('input[type=text][name=zone_name]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_zone_name]').text("Please Enter Name");
        }
        else {
            $('input[type=text][name=zone_name]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_zone_name]').text("");
        }

        if ($('select[name=zone_type] option:selected').val() === "Camera") {

            enableCameraSubmit();
        }
        else if ($('select[name=zone_type] option:selected').val() === "Bin") {

            enableBinZoneSubmit();
        }
        else if ($('select[name=zone_type] option:selected').val() === "Bullpen") {

            enableSVZoneSubmit();
        }
        else {
            enableZoneSubmit();
        }

    });
    //Camera URL
    $('select[name=cameraLocation]').change(function () {
        if (!checkValue($('select[name=cameraLocation]').val())) {
            $('select[name=cameraLocation]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_cameraLocation]').text("Please Select Type");
            $('input[type=text][name=zone_name]').val("")
            $('input[type=text][name=zone_name]').removeClass('is-invalid').removeClass('is-valid');
            $('span[id=error_zone_name]').text("");
        }
        else {
            $('select[name=cameraLocation]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_cameraLocation]').text("");
            $('input[type=text][name=zone_name]').val($('select[name=cameraLocation] option:selected').val())
            $('input[type=text][name=zone_name]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_zone_name]').text("");
        }
        enableCameraSubmit();
    });
    //machine name
    $('input[type=text][name=new_machine_name]').keyup(function () {
        if (!checkValue($('input[type=text][name=new_machine_name]').val())) {
            $('input[type=text][name=new_machine_name]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_new_machine_name]').text("Please Enter Machine Name");
        }
        else {
            $('input[type=text][name=new_machine_name]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_new_machine_name]').text("");
        }
        enableZoneSubmit();
    });
    $('input[type=text][name=new_machine_number]').keyup(function () {
        if (!checkValue($('input[type=text][name=new_machine_number]').val())) {
            $('input[type=text][name=new_machine_number]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_new_machine_number]').text("Please Enter Machine Name");
        }
        else {
            $('input[type=text][name=new_machine_number]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_new_machine_number]').text("");
        }
        enableZoneSubmit();
    });
}
function enableBinZoneSubmit() {
    if ($('select[name=zone_type]').hasClass('is-valid') &&
        $('input[type=text][name=zone_name]').hasClass('is-valid') &&
        $('textarea[id=bin_bins]').hasClass('is-valid')
    ) {
        $('button[id=zonesubmitBtn]').prop('disabled', false);
    }
    else {
        $('button[id=zonesubmitBtn]').prop('disabled', true);
    }
}

function enableSVZoneSubmit() {
    if ($('select[name=zone_type]').hasClass('is-valid') &&
        $('input[type=text][name=zone_name]').hasClass('is-valid')
    ) {
        $('button[id=zonesubmitBtn]').prop('disabled', false);
    }
    else {
        $('button[id=zonesubmitBtn]').prop('disabled', true);
    }
}
function enableCameraSubmit() {
    if ($('select[name=cameraLocation]').hasClass('is-valid') &&
        $('input[type=text][name=zone_name]').hasClass('is-valid') &&
        $('select[name=zone_type]').hasClass('is-valid')
    ) {
        $('button[id=zonesubmitBtn]').prop('disabled', false);
    }
    else {
        $('button[id=zonesubmitBtn]').prop('disabled', true);
    }
}
function enableZoneSubmit() {
    if ($('select[name=zone_select_name] option:selected').val() == '**Machine Not Listed' &&
        !$('input[type=text][name=new_machine_name]').hasClass('is-valid') &&
        !$('input[type=text][name=new_machine_number]').hasClass('is-valid')) {
        $('button[id=zonesubmitBtn]').prop('disabled', true);
    }
    else if ($('select[name=zone_type]').hasClass('is-valid') &&
        $('input[type=text][name=zone_name]').hasClass('is-valid')
    ) {
        $('button[id=zonesubmitBtn]').prop('disabled', false);
    }
    else {
        $('button[id=zonesubmitBtn]').prop('disabled', true);
    }
}
function CreateZone(newlayer)
{
    try {
        map.setView(newlayer.layer._bounds.getCenter());
        VaildateForm("");

        var togeo = newlayer.layer.toGeoJSON();
        var geoProp = {
            Zone_Type: "",
            floorid: baselayerid,
            name: "",
            visible: true
        }
        $('button[id=zonesubmitBtn][type=button]').off().on('click', function () {
            togeo.properties = geoProp;
            togeo.properties.Zone_Type = $('select[name=zone_type] option:selected').val();
            //togeo.properties.name = $('input[id=zone_name]').is(':visible') ? $('input[id=zone_name]').val() : $('select[name=zone_select_name] option:selected').val();


            togeo.properties.name = $('input[id=zone_name]').is(':visible')
                ? $('input[id=zone_name]').val()
                : $('select[name=zone_select_name] option:selected').val() == "**Machine Not Listed"
                    ? $('input[id=new_machine_name]').val() + "-" + $('input[id=new_machine_number]').val().padStart(3, '0')
                    : $('select[name=zone_select_name] option:selected').val();


            $.connection.FOTFManager.server.addZone(JSON.stringify(togeo)).done(function (Data) {
                if (!$.isEmptyObject(Data)) {
                    setTimeout(function () { sidebar.close('home'); }, 500);
                    addCreatedZoneToMap(Data);
                    newlayer.layer.remove();
                }
                else {
                    newlayer.layer.remove();
                }
            });
            $('input[id=new_machine_name]').val('');
            $('input[id=new_machine_number]').val('');
        });

    } catch (e) {
        console.log(e);
    }
}
function CreateBinZone(newlayer) {
    try {
        map.setView(newlayer.layer._bounds.getCenter());
        sidebar.open('home');
        var togeo = newlayer.layer.toGeoJSON();
        var geoProp = {
            Zone_Type: "",
            floorid: baselayerid,
            name: "",
            bins: "",
            visible: true
        }
        $('button[id=zonesubmitBtn][type=button]').off().on('click', function () {
            togeo.properties = geoProp;
            togeo.properties.Zone_Type = $('select[name=zone_type] option:selected').val();
            togeo.properties.name = $('input[id=zone_name]').is(':visible') ? $('input[id=zone_name]').val() : $('select[name=zone_select_name] option:selected').val() ;
            togeo.properties.bins = $('textarea[id="bin_bins"]').val();

            $.connection.FOTFManager.server.addZone(JSON.stringify(togeo)).done(function (Data) {
                if (!$.isEmptyObject(Data)) {
                    var tempObj = { data: Data }
                    setTimeout(function () { sidebar.close('home'); }, 500);
                    init_zones(tempObj, baselayerid);
                    newlayer.layer.remove();
                }
                else {
                    newlayer.layer.remove();
                }
            });
        });

    } catch (e) {
        console.log(e);
    }
}
function CreateCamera(newlayer)
{
    VaildateForm("Camera");
    sidebar.open('home');
    var togeo = newlayer.layer.toGeoJSON();
    map.setView(newlayer.layer._latlng, 3);
    var geoProp = {
        name: "",
        floorid: baselayerid,
        Tag_Type: "",
        visible: true
    }
    $('input[id=zone_name]').css('display', 'block');
    $('select[id=zone_select_name]').css('display', 'none');
    fotfmanager.server.getCameraList().done(function (cameradata) {
        if (cameradata.length > 0) {
            $('select[id=cameraLocation]').empty();
            $('<option/>').val("").html("").appendTo('select[id=cameraLocation]');
            $.each(cameradata, function () {
                $('<option/>').val(this.CAMERA_NAME).html(this.CAMERA_NAME + "/" + this.DESCRIPTION).appendTo('select[id=cameraLocation]');
            })
        }
    });
    $('button[id=zonesubmitBtn][type=button]').off().on('click', function () {
        togeo.properties = geoProp;
        togeo.properties.name = $('input[id=zone_name]').val();
        togeo.properties.Tag_Type = $('select[name=zone_type] option:selected').val();
        $.connection.FOTFManager.server.addMarker(JSON.stringify(togeo)).done(function (Data) {
            if (!$.isEmptyObject(Data)) {
                setTimeout(function () { sidebar.close('home'); }, 500);
                cameras.addData(Data, baselayerid);
                newlayer.layer.remove();
            }
            else {
                newlayer.layer.remove();
            }
        });
    });
}
function RemoveZoneItem(removeLayer)
{
    try {
        var layerId = removeLayer.layer.feature.properties.id;
        sidebar.close();
        fotfmanager.server.removeZone(layerId).done(function (Data) {
            if (!$.isEmptyObject(Data)) {
                setTimeout(function () { $("#Remove_Layer_Modal").modal('hide'); }, 500);
                if (Data.properties.Zone_Type === "DockDoor") {
                    removeDockDoor(removeLayer.layer._leaflet_id);
                }
                
            }
        });
    } catch (e) {
        console.log();
    }
}
function RemoveMarkerItem(removeLayer) {
    try {
        var layerId = removeLayer.layer.feature.properties.id;
        sidebar.close();
        fotfmanager.server.removeMarker(layerId).done(function (Data) {
            if (!$.isEmptyObject(Data)) {
                setTimeout(function () { $("#Remove_Layer_Modal").modal('hide'); }, 500);
                removeFromMapView(removeLayer.layer.id);
                removecamfromalllist(removeLayer.layer.id);
            }
        });
    } catch (e) {
        console.log();
    }
}
function removeFromMapView(id)
{
    $.map(map._layers, function (layer, i) {
        if (layer.hasOwnProperty("feature")) {
            if (layer._leaflet_id === id) {
                map.removeLayer(layer)
            }
        }
    });
}
function VaildateForm(FormType)
{
    $('select[name=zone_type]').val(FormType);
    $('select[name=zone_type]').prop('disabled', false);
    $('input[name=zone_name]').prop('disabled', false);
    $('input[id=zone_name]').css('display', 'block');
    $('select[id=zone_select_name]').css('display', 'none');
    $('select[id=zone_select_name]').val("");
    $('input[name=zone_name]').val("");
    $('select[name=cameraLocation]').val("");
    $('#camerainfo').css("display", "none");
    $('#binzoneinfo').css("display", "none");
    $('#new_machine_manual_row').css('display', 'none');
    if (!checkValue($('select[name=zone_type]').val())) {
        $('select[name=zone_type]').removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_zone_type]').text("Please Select Zone Type");
    }
    else {
        $('select[name=zone_type]').removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_zone_type]').text("");
    }
    //zone_name
    if (!checkValue($('input[type=text][name=zone_name]').val())) {
        $('input[type=text][name=zone_name]').removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_zone_name]').text("Please Enter Name");
    }
    else {
        $('input[type=text][name=zone_name]').removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_zone_name]').text("");
    }
    if (/Machine/i.test(FormType)) {
        fotfmanager.server.getMPEList().done(function (mpedata) {
            if (mpedata.length > 0) {
                //sort 
                mpedata.sort(SortByName);
                mpedata.push('**Machine Not Listed');
                $('input[id=zone_name]').css('display', 'none');
                $('select[id=zone_select_name]').css('display', 'block');
                $('select[id=zone_select_name]').empty();
                $('<option/>').val("").html("").appendTo('select[id=zone_select_name]');
                $('select[id=zone_select_name]').val("");
                $.each(mpedata, function () {
                    $('<option/>').val(this).html(this).appendTo('#zone_select_name');
                })
                $('select[name=zone_select_name]').removeClass('is-valid').addClass('is-invalid');
            }
        });

        $('select[id=zone_select_name]').change(function () {
            if ($('select[name=zone_select_name] option:selected').val() == '**Machine Not Listed') {
                $('#new_machine_manual_row').css('display', '');
                if (!checkValue($('input[type=text][name=new_machine_name]').val())) {
                    $('input[type=text][name=new_machine_name]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
                    $('span[id=error_new_machine_name]').text("Please Enter Machine Name");
                }
                else {
                    $('input[type=text][name=new_machine_name]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
                    $('span[id=error_new_machine_name]').text("");
                }
                if (!checkValue($('input[type=text][name=new_machine_number]').val())) {
                    $('input[type=text][name=new_machine_number]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
                    $('span[id=error_new_machine_number]').text("Please Enter Machine Name");
                }
                else {
                    $('input[type=text][name=new_machine_number]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
                    $('span[id=error_new_machine_number]').text("");
                }
            }
            else {
                $('#new_machine_manual_row').css('display', 'none');
            }
            if ($('select[name=zone_select_name] option:selected').val() == '') {
                $('button[id=zonesubmitBtn]').prop('disabled', true);
            }
            else {
                $('button[id=zonesubmitBtn]').prop('disabled', false);
            }
            enableZoneSubmit();
        });

        
    }
    if (/DockDoor/i.test(FormType)) {
        $('input[name=zone_name]').val("");
        $('textarea[id="bin_bins"]').val("");
        fotfmanager.server.getDockDoorList().done(function (DockDoordata) {
            if (DockDoordata.length > 0) {
                //sort 
                DockDoordata.sort(SortByNumber);
                $('input[id=zone_name]').css('display', 'none');
                $('select[id=zone_select_name]').css('display', 'block');
                $('select[id=zone_select_name]').empty();
                $('<option/>').val("").html("").appendTo('select[id=zone_select_name]');
                $('select[id=zone_select_name]').val("");
                $.each(DockDoordata, function () {
                    $('<option/>').val(this).html(this).appendTo('select[id=zone_select_name]');
                })
            }
        });
    }
    if (/bin/i.test(FormType)) {
        $('#camerainfo').css("display", "none");
        $('#binzoneinfo').css("display", "block");
        $('input[name=zone_name]').val("");
        $('textarea[id="bin_bins"]').val("");
        fotfmanager.server.getMPEList().done(function (mpedata) {
            if (mpedata.length > 0) {
                //sort 
                mpedata.sort(SortByName);
                $('input[id=zone_name]').css('display', 'none');
                $('select[id=zone_select_name]').css('display', 'block');
                $('select[id=zone_select_name]').empty();
                $('<option/>').val("").html("").appendTo('select[id=zone_select_name]');
                $('select[id=zone_select_name]').val("");
                $.each(mpedata, function () {
                    $('<option/>').val(this).html(this).appendTo('select[id=zone_select_name]');
                })
            }
        });
        if (!checkValue($('textarea[id=bin_bins]').val())) {
            $('textarea[id=bin_bins]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_bin_bins]').text("Please Bin Numbers");
        }
        else {
            $('textarea[id=bin_bins]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_bin_bins]').text("");
        }
        if (!checkValue($('select[name=zone_type]').val())) {
            $('select[name=zone_type]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_zone_type]').text("Please Select Zone Type");
        }
        else {
            $('select[name=zone_type]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_zone_type]').text("");
        }
        enableBinZoneSubmit();
    }

    if (/Bullpen/i.test(FormType)) {
        var parsedSV = null;
        var z = null;
        fotfmanager.server.getSVZoneNameList().done(function (svdata) {

            if (svdata.length > 0) {
                //sort 
                svdata.sort(SortByLocationName);
                $('input[id=zone_name]').css('display', 'none');
                $('select[id=zone_select_name]').css('display', 'block');
                $('select[id=zone_select_name]').empty();
                $('<option/>').val("").html("").appendTo('select[id=zone_select_name]');
                $('select[id=zone_select_name]').val("");
                $.each(svdata, function () {
                    $('<option/>').val(this.locationName).html(this.locationName).appendTo('select[id=zone_select_name]');
                })
            }
            $('select[name=zone_type]').prop('disabled', true);
            $('input[name=zone_name]').prop('disabled', true);
            $('input[name=zone_name]').val("");

            $('input[type=text][name=zone_name]').removeClass('is-invalid').removeClass('is-valid');
            $('span[id=error_zone_name]').text("");

            if (!checkValue($('select[name=zone_type]').val())) {
                $('select[name=zone_type]').removeClass('is-valid').addClass('is-invalid');
                $('span[id=error_zone_type]').text("Please Select Zone Type");
            }
            else {
                $('select[name=zone_type]').removeClass('is-invalid').addClass('is-valid');
                $('span[id=error_zone_type]').text("");
            }
            enableSVZoneSubmit();
        });
    }
    if (/camera/i.test(FormType)) {
        $('#camerainfo').css("display", "block");
        $('#binzoneinfo').css("display", "none");
      
        $('select[name=zone_type]').prop('disabled', true);
        $('input[name=zone_name]').prop('disabled', true);
        $('input[name=zone_name]').val("");

        $('input[type=text][name=zone_name]').removeClass('is-invalid').removeClass('is-valid');
        $('span[id=error_zone_name]').text("");
        //Camera URL
        if ($('select[name=cameraLocation]').val() === "") {
            $('select[name=cameraLocation]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_cameraLocation]').text("Please Select Camera URL");
        }
        else {
            $('input[type=text][name=cameraLocation]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_cameraLocation]').text("");
        }
        if (!checkValue($('select[name=zone_type]').val())) {
            $('select[name=zone_type]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_zone_type]').text("Please Select Zone Type");
        }
        else {
            $('select[name=zone_type]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_zone_type]').text("");
        }
        enableCameraSubmit();
    }
}

let sparklineMaximums = null;



function getMaxThroughput(machineType) {
    if (sparklineMaximums) {
        for (let dat of sparklineMaximums) {
            if (dat["MpeType"] == machineType) {
                return dat["MaxThroughput"];

            }
        }
    }
}

async function getSparklineMaximums() {
    fotfmanager.server.getMachineThroughputMaximums().done(function (data) {

        sparklineMaximums = data;

    });
}
async function updateAllMachineSparklinesDone(machineStatuses) {

    for (const machineStatus of machineStatuses) {
        updateSparklineCheck(machineStatus);


    }
    var forceUpdate = false;

    if (firstMachineSparklines) {
        forceUpdate = true;
    }
    checkSparklineVisibility(forceUpdate);
    let sparklinesChecked = $("#MPESparklines").prop("checked");
    if (firstMachineSparklines && sparklinesChecked) {

        updateSparklineTooltipDirection();
    }
    let mpeZoneDataChecked = $("#MPEWorkAreas").prop("checked");
    if (firstMPEZoneData && mpeZoneDataChecked) {

        updateMPEZoneTooltipDirection();
    }
    if (sparklinesChecked) {

        firstMachineSparklines = false;
    }
}
function updateSparklineCheck(machineStatus) {

    var sortPlan =
        machineStatus.Item1.properties.MPEWatchData.cur_sortplan;

    if (sortPlan != "") {
        updateMachineSparkline(machineStatus.Item1, machineStatus.Item2);
    }
}

function shouldUpdateSparkline(lastZoom, zoom, forceUpdate) {
    if (forceUpdate) return true;

    if (lastZoom > zoom && zoom < sparklineMinZoom) {
        return true;
    }

    if (zoom > lastZoom && lastZoom < sparklineMinZoom) {
        return true;
    }
    return false;
}
function checkSparklineVisibility(forceUpdate) {
    var zoom = map.getZoom();
    if (shouldUpdateSparkline(lastMapZoom, zoom, forceUpdate)) {

        var machineSparklineKeys = Object.keys(machineSparklines._layers);


        if (machineSparklineKeys.length == 0 ||
            !$("#MPESparklines").prop("checked")) {
            $("#sparkline-message").hide();

        }
        else
            if (zoom < sparklineMinZoom) {
                if (machineSparklineKeys.length > 0) {
                    $("#sparkline-message").show();
                }
                $.map(
                    machineSparklines._layers,
                    function (layer, i) {

                        layer.unbindTooltip();
                    });
            }
            else {

                $("#sparkline-message").hide();

                $.map(
                    machineSparklines._layers,
                    function (layer, i) {

                        updateMachineSparklineTooltip(layer.feature, layer);

                    });
            }


    }
    lastMapZoom = zoom;
}
// sets the sparkline graph cache so when it is ready to display,
// no graphs need to be created
// allow some time period in between function calls so that tags can continue to move
var firstMPEZoneData = true;
var firstMachineSparklines = true;

async function waitWithoutBlocking(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}
function updateAllMachineSparklines(machineStatuses, i) {
    if (i === machineStatuses.length) {

        updateAllMachineSparklinesDone(machineStatuses);
    }
    else {
        GetSparklineGraph(machineStatuses[i].Item1.properties,
            machineStatuses[i].Item1.properties.id);
        setTimeout(() => {
            updateAllMachineSparklines(machineStatuses, i + 1);
        }, 100)
            
       
    }
}


function getHourlyArrayMPESparkline(hourly_data, n) {
     let hourlyArray = JSON.parse(JSON.stringify(hourly_data));
    hourlyArray.splice(n + 1);
    hourlyArray.reverse();
    hourlyArray.pop();
    let currentColor = "";
    let latestHourIndex = hourlyArray.length - 1;
    let previousHourIndex = latestHourIndex - 1;
    let hourlyData = [];
    for (const obj of hourlyArray) {
        hourlyData.push(obj.count);
    }
    if ((hourlyArray[latestHourIndex].count < hourlyArray[previousHourIndex].count)
        || hourlyArray[latestHourIndex].count === 0) {
        currentColor = "rgba(231, 25, 33, 1)";
    }
    else {
        currentColor = "rgba(51, 51, 102, 1)";
    }
    return [hourlyData, currentColor];
}

function getLabelsMPESparkline(n) {
    let hourlyArray = [];
    for (var i = 0; i < n; i++) {
        hourlyArray.push("");
    }
    return hourlyArray;
}
var sparklineChart = null;
var sparklineCache = [];
function clearSparklineCache() {
    let date_time_old = Date.now() - (1000 * 60 * 61);
    for (var i = sparklineCache.length - 1; i >= 0; i--) {
        if (sparklineCache[i].date_time < date_time_old) {
            sparklineCache.splice(i, 1);
        }
    }
}
function checkSparklineCache(hourly_data) {
    for (var spCache of sparklineCache) {
        if (JSON.stringify(spCache.hourly_data) ==
            JSON.stringify(hourly_data)) {
            return spCache.dataURL;
        }
    }
    return null;
}

function addSparklineCache(hourly_data, dataURL) {
    for (var i = 0; i < sparklineCache; i++) {
        if (JSON.stringify(sparklineCache[i].hourly_data) ==
            JSON.stringify(hourly_data)) {
            return false;
        }
    }
    sparklineCache.push({
        hourly_data: hourly_data, dataURL:
            dataURL
    });
    return true;
}
let sparklineLength = 8;
function GetSparklineGraph(dataproperties, id) {
    var total = dataproperties.MPEWatchData.hourly_data.length;
    
    if (total > 0) {
       
        let hourlyArrays = getHourlyArrayMPESparkline(dataproperties.MPEWatchData.hourly_data, sparklineLength);
      
        let labels = getLabelsMPESparkline(sparklineLength);
        let hourlyArrayThis = hourlyArrays[0];
        let currentColor = hourlyArrays[1];
        let cacheCheck = checkSparklineCache(dataproperties.MPEWatchData.hourly_data);
        if (cacheCheck) {
            return cacheCheck;
        }
        if (sparklineChart === null) {


            let maxThroughput = getMaxThroughput(dataproperties.MPEWatchData.mpe_type);

            sparklineChart = new Chart("sparkline-canvas", {
                type: 'line',
                data:

                 {
                    labels: labels,


                    datasets: [


                {
                    label: '',
                    data: hourlyArrayThis,
                    backgroundColor:
                        "rgba(0, 0, 0, 0)"
                    ,
                    borderColor:
                        currentColor
                    ,
                    borderWidth: 50
                }
                    ]
                    
                },
                
                
                options: {
                    legend: {
                        display: false
                    },
                    animation: {
                        duration: 0
                    },
                    scales: {
                        xAxes: [{
                            gridLines: {
                                display: false
                            }

                        }],
                        yAxes: [{
                            gridLines: {
                                display: false
                            },
                            ticks: {
                                beginAtZero: true,
                                min: 0,
                                max: maxThroughput
                            }
                        }]
                    }
                }
            });
        }
        else {
            sparklineChart.data.datasets[0].data = hourlyArrayThis;
            sparklineChart.data.datasets[0].borderColor = currentColor;
            sparklineChart.update();
        }

        let b64 = sparklineChart.toBase64Image();
        addSparklineCache(dataproperties.MPEWatchData.hourly_data, b64);
        sparklineChart = null;
        return true;
    }
    else {
        return false;
    }
}

var machineSparklines = new L.GeoJSON(null, polyObj);

const sparklineWidthNormal = 40;
const sparklineHeightNormal = 20;

async function updateMachineSparklineTooltip(feature, layer) {
    let sparklineWidth = sparklineWidthNormal;
    let sparklineHeight = sparklineHeightNormal;
    let sparklineClass = 'leaflet-tooltip-sparkline';
    
    let imgUrl = checkSparklineCache(feature.properties.MPEWatchData.hourly_data);
   
    var htmlData = ((imgUrl === null) ? "<div></div>" : "<img src='" + imgUrl +
        "' width'" + sparklineWidth +
        "' height='" + sparklineHeight +
        "' style='width: " + sparklineWidth + "px; height: " + sparklineHeight +
        "px; ' />")
        ;
    if (layer._tooltip) {
        try {
            layer.unbindTooltip();
        }
        catch (e_) {

        }
    }
        layer.bindTooltip(htmlData, {
            permanent: true,
            interactive: false,
            direction: getSparklineTooltipDirection(),
            className: sparklineClass
        }).openTooltip();
    
    return true;
}


function layerMachineIdMatch(layer, machineupdate) {
    if (layer.feature.properties.id === machineupdate.properties.id) {
        
        return true;
    }
    return false;
}

let lastSparklineUpdate = 0;


function convertToSparkline(machineSparklineString) {
    let machineSparklinesNew = JSON.parse(machineSparklineString);

    for (var tuple of machineSparklinesNew) {
        tuple.Item1.properties.Zone_Type = "Sparkline";
        tuple.Item1.properties.sparkline = true;
        tuple.Item1.properties.color = "transparent";
        tuple.Item1.properties.fillColor = "transparent";
        tuple.Item1.properties.opacity = 0;
        tuple.Item1.properties.fillOpacity = 0;
        tuple.Item1.properties.id = tuple.Item1.properties.id + "-sp";
        tuple.Item1.properties.interactive = false;
    }
    return machineSparklinesNew;

}

async function updateMachineSparkline(machineupdate, id) {
    if (id == baselayerid) {
            if (machineupdate.properties.hasOwnProperty("MPEWatchData")) {
                let foundLayer = null;



                var machineSparklineKeys = Object.keys(machineSparklines._layers);
               
                for (var key of machineSparklineKeys) {
                    let layer = machineSparklines._layers[key];
                    if (layer.hasOwnProperty("options")) {
                        layer.options.fillColor = "transparent";
                        layer.options.color = "transparent";
                        layer.options.fillOpacity = 0;
                        layer.options.opacity = 0;
                        layer.options.interactive = false;
                        if (layerMachineIdMatch(layer, machineupdate))
                        {
                           
                            foundLayer = layer;
                        }

                    }
                }

                if (foundLayer) {
                    updateMachineSparklineTooltip(foundLayer.feature,
                        foundLayer);
                }
                else {
                    machineSparklines.addData(machineupdate);

                }
            }
        }
    
}
//side bar setup
let sidebar = L.control.sidebar({
    container: 'sidebar', position: 'left', autopan: false
});
let mainfloorOverlays = L.layerGroup();


let mainfloor = L.imageOverlay(null, [0, 0], { id:-1 ,zIndex: -1 }).addTo(mainfloorOverlays);
let baseLayers = {
    "Main Floor": mainfloor
};


let overlayMaps = {
    "AGV Vehicles": agv_vehicles,
    "PIV Vehicles": piv_vehicles,
    "Cameras": cameras,
    "Badge": tagsMarkersGroup,
    "AGV Locations": agvLocations,
    "MPE Work Areas": polygonMachine,
    "MPE Sparklines": machineSparklines,
    "MPE Bins": binzonepoly,
    "Dock Doors": dockDoors,
    "Staging Areas": stagingAreas,
    "Staging Bullpen Areas": stagingBullpenAreas,
    "View-ports": viewPortsAreas,
    "EBR Areas": ebrAreas,
    "Exit Areas": exitAreas,
    "Work Area": walkwayAreas,
    "Polygon Holes": polyholesAreas,
    "Locators": locatorMarker
};


$.urlParam = function (name) {
    var results = new RegExp('[\?&]' + name + '=([^&#]*)', 'i').exec(window.location.search);

    return (results !== null) ? results[1] || 0 : false;
}


let layersSelected = [mainfloor];

//mainfloor,
// polygonMachine,
//    piv_vehicles, agv_vehicles, agvLocations, container, stagingAreas, tagsMarkersGroup, dockDoors, binzonepoly
if ($.urlParam('specifyLayers')) {
    if ($.urlParam('agvVehicles')) layersSelected.push(agv_vehicles);
    if ($.urlParam('pivVehicles')) layersSelected.push(piv_vehicles);
    if ($.urlParam('cameras')) layersSelected.push(cameras);
    if ($.urlParam('badge')) layersSelected.push(tagsMarkersGroup);
    if ($.urlParam('agvLocations')) layersSelected.push(agvLocations);
    if ($.urlParam('mpeWorkAreas')) layersSelected.push(polygonMachine);
    if ($.urlParam('mpeSparklines')) layersSelected.push(machineSparklines);
    if ($.urlParam('mpeBins')) layersSelected.push(binzonepoly);
    if ($.urlParam('dockDoors')) layersSelected.push(dockDoors);

    if ($.urlParam('stagingAreas')) layersSelected.push(stagingAreas);
    if ($.urlParam('stagingBullpenAreas')) layersSelected.push(stagingBullpenAreas);
    if ($.urlParam('viewPorts')) layersSelected.push(viewPortsAreas);
    if ($.urlParam('ebrAreas')) layersSelected.push(ebrAreas);
    if ($.urlParam('exitAreas')) layersSelected.push(exitAreas);
    if ($.urlParam('workArea')) layersSelected.push(walkwayAreas);
    if ($.urlParam('polygonHoles')) layersSelected.push(polyholesAreas);
    if ($.urlParam('locators')) layersSelected.push(locatorMarker);

}
else {
    layersSelected = [mainfloor,
        polygonMachine,
        piv_vehicles,
        agv_vehicles,
        agvLocations,
        container,
        stagingAreas,
        stagingBullpenAreas,
        tagsMarkersGroup,
        dockDoors,
        binzonepoly
    ];

}


let historyPageState = { id: 0 };
let lastSelectedViewport = null;
function updateURIParametersForLayer(viewport) {
    let params = "?specifyLayers=true";
    if (viewport) {

        lastSelectedViewport = viewport;
        params += ("&viewport=" + viewport);
    }
    if (lastSelectedViewport === null && $.urlParam('viewport')) {
        params += ("&viewport=" + $.urlParam('viewport'));
    }
    $(".leaflet-control-layers-selector").each((_a, el) => {
        if (el.id && el.id.length > 0) {

            if ($("#" + el.id).is(":checked")) {
                let paramKey = getURIParameterFromId(el.id);
                params += "&" + paramKey + "=true";
            }
        }
    });
    historyPageState = { id: historyPageState.id + 1 };

    window.history.replaceState(historyPageState, "",
        "/Default.aspx" + params);
}

let mapView = {};
function saveMapView() {
    mapView.bounds = JSON.parse(JSON.stringify(map.getBounds()));
    mapView.zoom = map.getZoom() + 0;
}

function restoreMapView() {
    if (mapView.bounds) {
        let centerLat = (mapView.bounds._southWest.lat + mapView.bounds._northEast.lat) / 2;
        let centerLng = (mapView.bounds._southWest.lng + mapView.bounds._northEast.lng) / 2;
        let center = [centerLat, centerLng];
       map.setView(center, mapView.zoom);
    }
    mapView = {};
}
//setup map
map = L.map('map', {
    crs: L.CRS.Simple,
    renderer: L.canvas({ padding: 0.5 }),
    preferCanvas: true,
    pmIgnore: false,
    markerZoomAnimation: false,
    minZoom: 0,
    maxZoom: 18,
    zoomControl: false,
    measureControl: true,
    tap: false,
    layers: layersSelected
});
//map.attributionControl.setPrefix("USPS " + User.ApplicationFullName + " (" + User.SoftwareVersion + ") | " + User.Facility_Name);
let layerCheckboxIds = [];
function setLayerCheckboxId(thisCheckBox, innerHTML) {
    let name = innerHTML.replace(/ /g, '');
    thisCheckBox.id = name;
    layerCheckboxIds.push(thisCheckBox.id);
    return name;
}
let layersURIParameterMapping = [];


function getURIParameterFromText(txt) {
    txt = txt.replaceAll("-", " ");
    txt = txt.replaceAll("_", " ");
    let portions = txt.split(" ");
    let first = true;
    let uriParam = "";
    for (var portion of portions) {
        let newPortion = portion.toLowerCase();
        if (!first) {
            newPortion = portion.substring(0, 1).toUpperCase() +
                portion.substring(1);
        }
        uriParam += newPortion;
        first = false;
    }
    return uriParam;
}
function assignIdsToLayerCheckboxes() {
    var leafletSelectors
        = document.getElementsByClassName("leaflet-control-layers-selector");
    for (var selector of leafletSelectors) {
        let sp = selector.nextElementSibling;
        let keys = Object.keys(overlayMaps);
        for (const key of keys) {
            if (sp.innerHTML.trim() == key.trim()) {
                let checkboxId = setLayerCheckboxId(selector, sp.innerHTML);
                layersURIParameterMapping.push({
                    checkboxId: checkboxId,
                    urlParameter: getURIParameterFromText(key)
                });
                $("#" + checkboxId).on('click', function (e) {
                    updateURIParametersForLayer(checkboxId);
                });
            }
        }
    }
}

function getURIParameterFromId(checkboxId) {
    for (const data of layersURIParameterMapping) {
        if (data.checkboxId == checkboxId) return data.urlParameter;
    }
    return null;
}

function getIdFromURIParameter(urlParameter) {
    for (const data of layersURIParameterMapping) {
        if (data.urlParameter == urlParameter) return data.checkboxId;
    }
    return null;
}

map.on('baselayerchange', function (e) {
    baselayerid = e.layer.options.id;
    console.log(baselayerid);
    
    fotfmanager.server.getIndoorMapFloor(baselayerid).done(function (data) {
        sidebar.close('home');
        $zoneSelect[0].selectize.setValue(-1, true);
        $zoneSelect[0].selectize.clearOptions();
        ebrAreas.clearLayers();
        stagingAreas.clearLayers();
        walkwayAreas.clearLayers();
        exitAreas.clearLayers();
        polyholesAreas.clearLayers();
        dockDoors.clearLayers();
        polygonMachine.clearLayers();
        machineSparklines.clearLayers();
        binzonepoly.clearLayers();
        agvLocations.clearLayers();
        viewPortsAreas.clearLayers();
        stagingAreas.clearLayers();
        //markers
        tagsMarkersGroup.clearLayers();
        piv_vehicles.clearLayers();
        agv_vehicles.clearLayers();
        cameras.clearLayers();
        locatorMarker.clearLayers();
        if (data.length > 0) {
            init_zones(data[0].zones, baselayerid);
            init_locators(data[0].locators, baselayerid);
        }
        assignIdsToLayerCheckboxes();
        setLayerCheckUncheckEvents();
        checkViewportLoad();
    });


});
function setLayerCheckUncheckEvents() {
    $("#MPESparklines").click(function () {
       
        updateMPEZoneTooltipDirection();
        updateSparklineTooltipDirection();
    });
    $("#MPEWorkAreas").click(function () {
        updateMPEZoneTooltipDirection();
        updateSparklineTooltipDirection();
    })
}
var lastMapZoom = null;
map.on('zoomend', function () {
    setTimeout(checkSparklineVisibility, 100);
    //updateAllCameras(Date.now());
    UpdateCameraZoom();
    if (map.getZoom() != 2) {
        btnZoomReset.button.removeAttribute("style", "display:none;");
    }
});
let timedisplay = L.Control.extend({
    options: {
        position: 'topright'
    },
    onAdd: function () {
        let Domcntainer = L.DomUtil.create('input');
        Domcntainer.id = "localTime";
        Domcntainer.type = "button";
        Domcntainer.className = "btn btn-secondary btn-sm";
        return Domcntainer;
    }
});
sidebar.on('content', function (ev) {
    sidebar.options.autopan = false;
    switch (ev.id) {
        case 'autopan':
            break;
        case 'setting':
            Edit_AppSetting("app_settingtable");
            Load_Floorplan_Table("floorplantable");
            break;
        case 'reports':
            //GetUserInfo();
            break;
        case 'userprofile':
            GetUserProfile();
            break;
        case 'agvnotificationinfo':
            LoadNotification("vehicle", "agvnotificationtable");
            break;
        case 'notificationsetup':
            LoadNotificationsetup({}, "notificationsetuptable");
            break;
        case 'tripsnotificationinfo':
            LoadNotification("routetrip", "tripsnotificationtable");
            break;
        default:
            sidebar.options.autopan = false;
            break;
    }
});
map.addControl(sidebar);
map.addControl(new timedisplay());
// Add Layer Popover - Proposed
var layersControl = L.control.layers(baseLayers, overlayMaps, {
    sortLayers: true, sortFunction: function (layerA, layerB, nameA, nameB) {
        if (nameA.toUpperCase().includes("FLOOR")) {
            if (nameA.toUpperCase().includes("MAIN")) {
                return -1;
            }
            else {
                return nameA < nameB ? -1 : (nameB < nameA ? 1 : 0);
            }
        }
    }, position: 'bottomright', collapsed: false
}).addTo(map);
//Add zoom reset button
var btnZoomReset = L.easyButton({
    position: 'bottomright',
    states: [{
        stateName: 'viewreset',
        icon: '<div id="resetZoom"><i class="pi-iconZoomAll align-self-center" style="padding-bottom: 3px; padding-left: 1px; display: inline-flex; vertical-align: middle;" title="Reset Zoom"></i></div>',
        onClick: function () {
            map.setView(map.getBounds().getCenter(), 1.5);
            btnZoomReset.button.setAttribute("style", "display:none;");
            sidebar.close();
        }
    }]
});
btnZoomReset.addTo(map);
btnZoomReset.button.setAttribute("style", "display:none;");
//Add zoom button
new L.Control.Zoom({ position: 'bottomright' }).addTo(map);

//add View Ports
L.easyButton({
    position: 'bottomright',
    states: [{
        stateName: 'viewport',
        icon: '<div id="viewportsToggle" data-toggle="popover"><i class="pi-iconViewport align-self-center" title="Viewports"></i></div>'
    }]
}).addTo(map);

// Add Layer Control Button
L.easyButton({
    position: 'bottomright',
    states: [{
        stateName: 'layer',
        icon: '<div id="layersToggle" data-toggle="layerPopover"><i class="pi-iconLayer align-self-center" title="Layer Controls"></i></div>'
    }]
}).addTo(map);

//Full-screen button only for Chrome
if (window.chrome) {
    var fullscreentoggle = L.easyButton({
        position: 'bottomright',
        leafletClasses: true,
        states: [{
            stateName: 'enter-fullscreen',
            icon: '<i class="pi-iconFullScreen" title="Enter Full Screen"></i>',
            title: 'Enter Full Screen',
            onClick: function (control) {
                if (document.documentElement.requestFullscreen) {
                    document.documentElement.requestFullscreen();
                } else if (document.documentElement.msRequestFullscreen) {
                    document.documentElement.msRequestFullscreen();
                } else if (document.documentElement.mozRequestFullScreen) {
                    document.documentElement.mozRequestFullScreen();
                } else if (document.documentElement.webkitRequestFullscreen) {
                    document.documentElement.webkitRequestFullscreen(Element.ALLOW_KEYBOARD_INPUT);
                }
                control.state('exit-fullscreen');
            }
        }, {
            icon: '<i class="pi-iconFullScreenExit iconCenter" title="Exit Full Screen"></i>',
            stateName: 'exit-fullscreen',
            title: 'Exit Full Screen',
            onClick: function (control) {
                if (document.exitFullscreen) {
                    document.exitFullscreen();
                } else if (document.msExitFullscreen) {
                    document.msExitFullscreen();
                } else if (document.mozCancelFullScreen) {
                    document.mozCancelFullScreen();
                } else if (document.webkitExitFullscreen) {
                    document.webkitExitFullscreen();
                }
                control.state('enter-fullscreen');
            }
        }]
    });
    fullscreentoggle.addTo(map);
}
// only allow one popover to be open at once
// bind to body so that this applies to any dynamically generated popovers, such as #faqToggle / #legendToggle
$('body').popover({
    selector: '[data-toggle=popover]',
    trigger: "click"
}).on("show.bs.popover", function (e) {
    // hide all other popovers
    $('[data-toggle=popover]').not(e.target).popover('hide');
    $('#twentyfourmessage').not(e.target).popover('hide');
    $('#layersContent').hide();
});
//Hide sidebar
$('[role=tablist]').click(function (e) {
    $('[data-toggle=popover]').popover('hide');
    $('#twentyfourmessage').popover('hide');
    $('#layersContent').hide();
});
$('#viewportsToggle').popover({
    html: true,
    trigger: 'click',
    content: $('#viewportsContent'),
    title: 'Viewports'
});
$('#viewportsToggle').on('click', function () {
    // close the sidebar
    sidebar.close();
});
$('.leaflet-control-layers').addClass('layerPopover');
$('.layerPopover').attr('id', 'layersContent');
$('#layersContent').prepend('<div class="layersArrow"></div>');
$('.leaflet-control-layers').hide();
$('#layersToggle').on('click', function () {
    //Toggle layer Popover
    $('#layersContent').toggle();
    // close the sidebar
    sidebar.close();
    // close other popover
    $('[data-toggle=popover]').popover('hide');
    $('#twentyfourmessage').popover('hide');
});
async function init_mapSetup(MapData) {
    try {
        fotfmanager.server.getMap().done(function (MapData) {
            if (MapData.length > 0) {
                //map.attributionControl.setPrefix("USPS " + MapData[0].backgroundImages.applicationFullName + " (" + MapData[0].backgroundImages.softwareVersion + ")");
                //$('#fotf-site-facility-name').append(MapData[0].backgroundImages.facilityName);
                //map.attributionControl.addAttribution(MapData[0].backgroundImages.facilityName);
                //$(document).prop('title', MapData[0].backgroundImages.facilityName + ' ' + MapData[0].backgroundImages.applicationAbbr);
                $.each(MapData, function (index) {
                    //set new image
                    let img = new Image();
                    //load Base64 image
                    img.src = this.backgroundImages.base64;
                    //create he bound of the image.
                    let bounds = [[this.backgroundImages.yMeter, this.backgroundImages.xMeter], [this.backgroundImages.heightMeter + this.backgroundImages.yMeter, this.backgroundImages.widthMeter + this.backgroundImages.xMeter]];
                    var trackingarea = L.polygon(bounds, {});
                    if (index === 0) {
                        mainfloor.options.id = this.id;
                        mainfloor.setUrl(img.src);
                        mainfloor.setZIndex(index);
                        mainfloor.setBounds(trackingarea.getBounds());
                        //center image
                        map.setView(trackingarea.getBounds().getCenter(), 1.5);
                        //init_zones(this.zones, this.id);
                        //init_locators(this.locators, this.id);
                    }
                    else {
                        layersControl.addBaseLayer(L.imageOverlay(img.src, trackingarea.getBounds(), { id: this.id, zindex: index }), this.backgroundImages.name);
                        //init_zones(this.zones, this.id);
                        //init_locators(this.locators, this.id);
                    }
                });
                init_arrive_depart_trips();
                fotfmanager.server.joinGroup("Trips");
                //init_agvtags();
                LoadNotification("routetrip");
                LoadNotification("vehicle");
                fotfmanager.server.joinGroup("Notification");
                //add user to the tag groups only for none PMCCUser
                if (User.hasOwnProperty("UserId") && /^(Admin|OIE)/i.test(User.Role)) {
                    fotfmanager.server.joinGroup("PeopleMarkers");
                }
                if (/(^PMCCUser$)/i.test(User.UserId)) {
                    fotfmanager.server.leaveGroup("PeopleMarkers");
                }

            }
            //else {
            //    fotfmanager.server.GetIndoorMap().done(function (GetIndoorMap) {
            //        if (GetIndoorMap.length > 0) {
            //            $.each(GetIndoorMap, function () {
            //                map.attributionControl.setPrefix("USPS " + this.backgroundImages.applicationFullName + " (" + this.backgroundImages.softwareVersion + ")");
            //                $('#fotf-site-facility-name').append(this.backgroundImages.facilityName);
            //                map.attributionControl.addAttribution(this.backgroundImages.facilityName);
            //                $(document).prop('title', this.backgroundImages.facilityName + ' ' + this.backgroundImages.applicationAbbr);
            //            });
            //        }
            //    })
            //}
            if ($.isEmptyObject(map)) {
                $('div[id=map]').css('display', 'none');
                $('<div/>', { class: 'jumbotron text-center' })
                    .append($('<h1/>', { class: 'display-4', text: "Map has not been Configured " }))
                    .append($('<p/>', { class: 'lead', text: 'Please Configure Map' }))
                    .append($('<hr/>', { class: 'my-4' }))
                    .append($('<div/>', { class: 'row' })
                        .append($('<div>', { class: 'col' })
                            .append($('<button/>', { class: 'btn btn-outline-success ', type: 'button', id: 'API_connection', text: 'Configure Map' })))

                    ).append($('<div/>', { class: 'row' })
                        .append($('<div>', { class: 'col text-center' })
                            .append($('<span/>', { class: 'text-info ', id: 'error_remove_server_connection' })))
                    ).insertBefore('div[id=map]');
            }
        })
    } catch (e) {
        $('div[id=map]').css('display', 'none');
        $('<div/>', { class: 'jumbotron text-center' })
            .append($('<h1/>', { class: 'display-4', text: "Error loading Map" }))
            .append($('<p/>', { class: 'lead', text: 'Erro:' + e.message + ' ' + e.stack}))
            .append($('<hr/>', { class: 'my-4' }))
            .insertBefore('div[id=map]');
    }
}
$('#fotf-sidebar-close').on('click', function () {
    // close the sidebar
    sidebar.close();
});
//Sidebar BS Tooltips
$(function () {
    $('[data-toggle="tooltip"]').tooltip()
})
// Search Tag Name
$('input[id=inputsearchbtn').keyup(function () {
    startSearch(this.value)
}).keydown(function (event) {
    if (event.which == 13) {
        event.preventDefault();
    }
});
async function GetUserInfo() {
    try {
        // get people tags
        fotfmanager.server.getPersonTagsList().done(function (Data) {
            try {
                //staffing
                var inBuilding = [];
                var outBuilding = [];
                if (Data.length > 0) {
                    new L.GeoJSON(Data, {
                        onEachFeature: function (feature, layer) {
                            var craftName_id = "";
                            var nameId_id = "";

                            if (checkValue(feature.properties.name)) {
                                if (!/^n.a$/.test(feature.properties.name)) {
                                    var splitname = feature.properties.name.split('_');
                                    if (splitname.length > 1) {
                                        craftName_id = splitname[0];
                                        nameId_id = splitname[1];
                                    }
                                    else {
                                        craftName_id = feature.properties.name;
                                        nameId_id = "";
                                    }
                                    // var currenttime = moment().tz();  // 5am PDT
                                    let currenttime = moment().tz(timezone.Facility_TimeZone);
                                    let lastpositiontime = moment(feature.properties.positionTS);
                                    if (currenttime._isValid && lastpositiontime._isValid) {
                                        // calculate total duration
                                        var duration = moment.duration(currenttime.diff(lastpositiontime));
                                        // duration
                                        if (duration._milliseconds > 1800000) {
                                            outBuilding.push({
                                                craftName: craftName_id,
                                                nameId: nameId_id,
                                                id: feature.properties.id
                                            })
                                        }
                                        else {
                                            inBuilding.push({
                                                craftName: craftName_id,
                                                nameId: nameId_id,
                                                id: feature.properties.id
                                            })
                                        }
                                    }
                                }
                            }
                        }
                    });
                    var instaffCounts = [];
                    var suminBuildingCounts = {};
                    $.each(inBuilding, function (i, e) {
                        suminBuildingCounts[this.craftName] = (suminBuildingCounts[this.craftName] || 0) + 1;
                    });
                    $.each(suminBuildingCounts, function (index, value) {
                        instaffCounts.push({
                            name: index,
                            instaff: value
                        });
                    });
                    var outstaffCounts = [];
                    var outsumstaffCounts = {};
                    $.each(outBuilding, function (i, e) {
                        outsumstaffCounts[this.craftName] = (outsumstaffCounts[this.craftName] || 0) + 1;
                    });
                    $.each(outsumstaffCounts, function (index, value) {
                        outstaffCounts.push({
                            name: index,
                            outstaff: value
                        });
                    });
                    var staffing = $.extend(true, instaffCounts, outstaffCounts);
                    staffing.sort(SortByTagName);
                    staffing.push({
                        name: 'Total',
                        instaff: inBuilding.length,
                        outstaff: outBuilding.length
                    });
                    $userstop_Table = $('table[id=userstoptable]');
                    $userstop_Table_Body = $userstop_Table.find('tbody');
                    $userstop_Table_Body.empty();
                    $userstop_row_template = '<tr data-id={staffName}>' +
                        '<td>{staffName}</td>' +
                        '<td class="text-center">{instaff}</td>' +
                        '</tr>"';
                    function formatstafftoprow(properties) {
                        return $.extend(properties, {
                            staffName: properties.name,
                            staffId: properties.id,
                            instaff: properties.hasOwnProperty("instaff") ? properties.instaff : 0,
                            planstaff: properties.hasOwnProperty("planstaff") ? properties.planstaff : 0,
                            outstaff: properties.hasOwnProperty("outstaff") ? properties.outstaff : 0,
                        });
                    }
                    $.each(staffing, function () {
                        $userstop_Table_Body.append($userstop_row_template.supplant(formatstafftoprow(this)));
                    })
                }
            } catch (e) {
                console.log(e);
            }
        });
        //get undetected tags Data
        fotfmanager.server.getUndetectedTagsList().done(function (undetectedtagsData) {
            try {
                if (undetectedtagsData.length > 0) {
                    let compliance_Table = $('table[id=compliancetable]');
                    let compliance_Table_Body = compliance_Table.find('tbody');
                    $('span[name=undetected_count]').text(undetectedtagsData.length);
                    let compliance_row_template = '<tr data-tag="undetected_{tag_id}">' +
                        '<td data-input="badge">{tag_name}</td>' +
                        '<td data-input="ldc" class="text-center">{ldc}</td>' +
                        '<td data-input="opcode" class="text-center">{op_code}</td>' +
                        '<td data-input="paylocation" class="text-center">{paylocation}</td>' +
                        '</tr>'
                        ;
                    function formatcompliancerow(properties) {
                        return $.extend(properties, {
                            tag_id: properties.id,
                            tag_name: !/^n.a$/.test(properties.name) ? properties.name : /^n.a$/.test(properties.craftName) ? properties.craftName : properties.id,
                            ldc: properties.hasOwnProperty("tacs") ? properties.tacs.hasOwnProperty("ldc") ? properties.tacs.ldc : "No LDC" : "No Tacs",
                            op_code: properties.hasOwnProperty("tacs") ? properties.tacs.hasOwnProperty("operationId") ? properties.tacs.operationId : "No Op Code" : "No Tacs",
                            paylocation: properties.hasOwnProperty("tacs") ? properties.tacs.hasOwnProperty("payLocation") ? properties.tacs.payLocation : "No payLocation" : "No Tacs"
                        });
                    }
                    $.map(undetectedtagsData, async function (properties, i) {
                        var findtrdataid = compliance_Table_Body.find('tr[data-tag=undetected_' + properties.properties.id + ']');
                        if (findtrdataid.length === 0) {
                            var tacs = JSON.parse(properties.properties.tacs);
                            properties.properties.tacs = tacs;
                            if (tacs.fnAlert === "false") {
                                compliance_Table_Body.append(compliance_row_template.supplant(formatcompliancerow(properties.properties)));
                                sortTable(compliance_Table, 'asc');
                            }
                            else {
                                findtrdataid.remove();
                            }
                        }
                    })
                }
            } catch (e) {
                console.log(e);
            }
        });
        //get LDC alerts
        fotfmanager.server.getLDCAlertTagsList().done(function (ldcalertstagsData) {
            try {
                if (ldcalertstagsData.length > 0) {
                    let LDCAlert_Table = $('table[id=ldcalerttable]');
                    let LDCAlert_Table_Body = LDCAlert_Table.find('tbody');
                    $('span[name=ldcaler_count]').text(ldcalertstagsData.length);
                    let LDCAlert_row_template = '<tr data-tag="ldc_alert_{tag_id}">' +
                        '<td data-input="badge">{tag_name}</td>' +
                        '<td data-input="ldc" class="text-center">{ldc}</td>' +
                        '<td data-input="ldc" class="text-center">{sels_ldc}</td>' +
                        '<td data-input="opcode" class="text-center">{op_code}</td>' +
                        '<td data-input="paylocation" class="text-center">{paylocation}</td>' +
                        '</tr>'
                        ;
                    function formatldcalertsrow(properties) {
                        return $.extend(properties, {
                            tag_id: properties.id,
                            tag_name: !/^n.a$/.test(properties.name) ? properties.name : /^n.a$/.test(properties.craftName) ? properties.craftName : properties.id,
                            ldc: properties.hasOwnProperty("tacs") ? properties.tacs.hasOwnProperty("ldc") ? properties.tacs.ldc : "No LDC" : "No Tacs",
                            sels_ldc: properties.hasOwnProperty("sels") ? properties.sels.hasOwnProperty("currentLDCs") ? properties.sels.currentLDCs[0] : "No LDC" : "No Tacs",
                            op_code: properties.hasOwnProperty("tacs") ? properties.tacs.hasOwnProperty("operationId") ? properties.tacs.operationId : "No Op Code" : "No Tacs",
                            paylocation: properties.hasOwnProperty("tacs") ? properties.tacs.hasOwnProperty("payLocation") ? properties.tacs.payLocation : "No payLocation" : "No Tacs"
                        });
                    }
                    $.map(ldcalertstagsData, async function (properties, i) {
                        var findtrdataid = LDCAlert_Table_Body.find('tr[data-tag=ldc_alert_' + properties.properties.id + ']');
                        if (findtrdataid.length === 0) {
                            LDCAlert_Table_Body.append(LDCAlert_row_template.supplant(formatldcalertsrow(properties.properties)));
                            sortTable(LDCAlert_Table, 'asc');
                        }
                    })
                }
            } catch (e) {
                console.log(e);
            }
        });
    } catch (e) {
        console.log(e)
    }
}
async function GetUserProfile() {
    if (!$.isEmptyObject(User)) {
        var userid = checkValue(User.FirstName) ? User.FirstName + ' ' + User.SurName : User.UserId;
        $('#userfullname').text(userid);
        $('#useremail').text(User.EmailAddress);
        $('#userphone').text(User.Phone);
        $('#usertitel').text(User.Role);
    }
}
async function sortTable(table, order) {
    var asc = order === 'asc',
        tbody = table.find('tbody');
    tbody.find('tr').sort(function (a, b) {
        if (asc) {
            return $('td:first', a).text().localeCompare($('td:first', b).text());
        } else {
            return $('td:first', b).text().localeCompare($('td:first', a).text());
        }
    }).appendTo(tbody);
}
var QRCode;

(function () {
	//---------------------------------------------------------------------
	// QRCode for JavaScript
	//
	//---------------------------------------------------------------------
	function QR8bitByte(data) {
		this.mode = QRMode.MODE_8BIT_BYTE;
		this.data = data;
		this.parsedData = [];

		// Added to support UTF-8 Characters
		for (var i = 0, l = this.data.length; i < l; i++) {
			var byteArray = [];
			var code = this.data.charCodeAt(i);

			if (code > 0x10000) {
				byteArray[0] = 0xF0 | ((code & 0x1C0000) >>> 18);
				byteArray[1] = 0x80 | ((code & 0x3F000) >>> 12);
				byteArray[2] = 0x80 | ((code & 0xFC0) >>> 6);
				byteArray[3] = 0x80 | (code & 0x3F);
			} else if (code > 0x800) {
				byteArray[0] = 0xE0 | ((code & 0xF000) >>> 12);
				byteArray[1] = 0x80 | ((code & 0xFC0) >>> 6);
				byteArray[2] = 0x80 | (code & 0x3F);
			} else if (code > 0x80) {
				byteArray[0] = 0xC0 | ((code & 0x7C0) >>> 6);
				byteArray[1] = 0x80 | (code & 0x3F);
			} else {
				byteArray[0] = code;
			}

			this.parsedData.push(byteArray);
		}

		this.parsedData = Array.prototype.concat.apply([], this.parsedData);

		if (this.parsedData.length != this.data.length) {
			this.parsedData.unshift(191);
			this.parsedData.unshift(187);
			this.parsedData.unshift(239);
		}
	}

	QR8bitByte.prototype = {
		getLength: function (buffer) {
			return this.parsedData.length;
		},
		write: function (buffer) {
			for (var i = 0, l = this.parsedData.length; i < l; i++) {
				buffer.put(this.parsedData[i], 8);
			}
		}
	};

	function QRCodeModel(typeNumber, errorCorrectLevel) {
		this.typeNumber = typeNumber;
		this.errorCorrectLevel = errorCorrectLevel;
		this.modules = null;
		this.moduleCount = 0;
		this.dataCache = null;
		this.dataList = [];
	}

	QRCodeModel.prototype = {
		addData: function (data) { var newData = new QR8bitByte(data); this.dataList.push(newData); this.dataCache = null; }, isDark: function (row, col) {
			if (row < 0 || this.moduleCount <= row || col < 0 || this.moduleCount <= col) { throw new Error(row + "," + col); }
			return this.modules[row][col];
		}, getModuleCount: function () { return this.moduleCount; }, make: function () { this.makeImpl(false, this.getBestMaskPattern()); }, makeImpl: function (test, maskPattern) {
			this.moduleCount = this.typeNumber * 4 + 17; this.modules = new Array(this.moduleCount); for (var row = 0; row < this.moduleCount; row++) { this.modules[row] = new Array(this.moduleCount); for (var col = 0; col < this.moduleCount; col++) { this.modules[row][col] = null; } }
			this.setupPositionProbePattern(0, 0); this.setupPositionProbePattern(this.moduleCount - 7, 0); this.setupPositionProbePattern(0, this.moduleCount - 7); this.setupPositionAdjustPattern(); this.setupTimingPattern(); this.setupTypeInfo(test, maskPattern); if (this.typeNumber >= 7) { this.setupTypeNumber(test); }
			if (this.dataCache == null) { this.dataCache = QRCodeModel.createData(this.typeNumber, this.errorCorrectLevel, this.dataList); }
			this.mapData(this.dataCache, maskPattern);
		}, setupPositionProbePattern: function (row, col) { for (var r = -1; r <= 7; r++) { if (row + r <= -1 || this.moduleCount <= row + r) continue; for (var c = -1; c <= 7; c++) { if (col + c <= -1 || this.moduleCount <= col + c) continue; if ((0 <= r && r <= 6 && (c == 0 || c == 6)) || (0 <= c && c <= 6 && (r == 0 || r == 6)) || (2 <= r && r <= 4 && 2 <= c && c <= 4)) { this.modules[row + r][col + c] = true; } else { this.modules[row + r][col + c] = false; } } } }, getBestMaskPattern: function () {
			var minLostPoint = 0; var pattern = 0; for (var i = 0; i < 8; i++) { this.makeImpl(true, i); var lostPoint = QRUtil.getLostPoint(this); if (i == 0 || minLostPoint > lostPoint) { minLostPoint = lostPoint; pattern = i; } }
			return pattern;
		}, createMovieClip: function (target_mc, instance_name, depth) {
			var qr_mc = target_mc.createEmptyMovieClip(instance_name, depth); var cs = 1; this.make(); for (var row = 0; row < this.modules.length; row++) { var y = row * cs; for (var col = 0; col < this.modules[row].length; col++) { var x = col * cs; var dark = this.modules[row][col]; if (dark) { qr_mc.beginFill(0, 100); qr_mc.moveTo(x, y); qr_mc.lineTo(x + cs, y); qr_mc.lineTo(x + cs, y + cs); qr_mc.lineTo(x, y + cs); qr_mc.endFill(); } } }
			return qr_mc;
		}, setupTimingPattern: function () {
			for (var r = 8; r < this.moduleCount - 8; r++) {
				if (this.modules[r][6] != null) { continue; }
				this.modules[r][6] = (r % 2 == 0);
			}
			for (var c = 8; c < this.moduleCount - 8; c++) {
				if (this.modules[6][c] != null) { continue; }
				this.modules[6][c] = (c % 2 == 0);
			}
		}, setupPositionAdjustPattern: function () {
			var pos = QRUtil.getPatternPosition(this.typeNumber); for (var i = 0; i < pos.length; i++) {
				for (var j = 0; j < pos.length; j++) {
					var row = pos[i]; var col = pos[j]; if (this.modules[row][col] != null) { continue; }
					for (var r = -2; r <= 2; r++) { for (var c = -2; c <= 2; c++) { if (r == -2 || r == 2 || c == -2 || c == 2 || (r == 0 && c == 0)) { this.modules[row + r][col + c] = true; } else { this.modules[row + r][col + c] = false; } } }
				}
			}
		}, setupTypeNumber: function (test) {
			var bits = QRUtil.getBCHTypeNumber(this.typeNumber); for (var i = 0; i < 18; i++) { var mod = (!test && ((bits >> i) & 1) == 1); this.modules[Math.floor(i / 3)][i % 3 + this.moduleCount - 8 - 3] = mod; }
			for (var i = 0; i < 18; i++) { var mod = (!test && ((bits >> i) & 1) == 1); this.modules[i % 3 + this.moduleCount - 8 - 3][Math.floor(i / 3)] = mod; }
		}, setupTypeInfo: function (test, maskPattern) {
			var data = (this.errorCorrectLevel << 3) | maskPattern; var bits = QRUtil.getBCHTypeInfo(data); for (var i = 0; i < 15; i++) { var mod = (!test && ((bits >> i) & 1) == 1); if (i < 6) { this.modules[i][8] = mod; } else if (i < 8) { this.modules[i + 1][8] = mod; } else { this.modules[this.moduleCount - 15 + i][8] = mod; } }
			for (var i = 0; i < 15; i++) { var mod = (!test && ((bits >> i) & 1) == 1); if (i < 8) { this.modules[8][this.moduleCount - i - 1] = mod; } else if (i < 9) { this.modules[8][15 - i - 1 + 1] = mod; } else { this.modules[8][15 - i - 1] = mod; } }
			this.modules[this.moduleCount - 8][8] = (!test);
		}, mapData: function (data, maskPattern) {
			var inc = -1; var row = this.moduleCount - 1; var bitIndex = 7; var byteIndex = 0; for (var col = this.moduleCount - 1; col > 0; col -= 2) {
				if (col == 6) col--; while (true) {
					for (var c = 0; c < 2; c++) {
						if (this.modules[row][col - c] == null) {
							var dark = false; if (byteIndex < data.length) { dark = (((data[byteIndex] >>> bitIndex) & 1) == 1); }
							var mask = QRUtil.getMask(maskPattern, row, col - c); if (mask) { dark = !dark; }
							this.modules[row][col - c] = dark; bitIndex--; if (bitIndex == -1) { byteIndex++; bitIndex = 7; }
						}
					}
					row += inc; if (row < 0 || this.moduleCount <= row) { row -= inc; inc = -inc; break; }
				}
			}
		}
	}; QRCodeModel.PAD0 = 0xEC; QRCodeModel.PAD1 = 0x11; QRCodeModel.createData = function (typeNumber, errorCorrectLevel, dataList) {
		var rsBlocks = QRRSBlock.getRSBlocks(typeNumber, errorCorrectLevel); var buffer = new QRBitBuffer(); for (var i = 0; i < dataList.length; i++) { var data = dataList[i]; buffer.put(data.mode, 4); buffer.put(data.getLength(), QRUtil.getLengthInBits(data.mode, typeNumber)); data.write(buffer); }
		var totalDataCount = 0; for (var i = 0; i < rsBlocks.length; i++) { totalDataCount += rsBlocks[i].dataCount; }
		if (buffer.getLengthInBits() > totalDataCount * 8) {
			throw new Error("code length overflow. ("
				+ buffer.getLengthInBits()
				+ ">"
				+ totalDataCount * 8
				+ ")");
		}
		if (buffer.getLengthInBits() + 4 <= totalDataCount * 8) { buffer.put(0, 4); }
		while (buffer.getLengthInBits() % 8 != 0) { buffer.putBit(false); }
		while (true) {
			if (buffer.getLengthInBits() >= totalDataCount * 8) { break; }
			buffer.put(QRCodeModel.PAD0, 8); if (buffer.getLengthInBits() >= totalDataCount * 8) { break; }
			buffer.put(QRCodeModel.PAD1, 8);
		}
		return QRCodeModel.createBytes(buffer, rsBlocks);
	}; QRCodeModel.createBytes = function (buffer, rsBlocks) {
		var offset = 0; var maxDcCount = 0; var maxEcCount = 0; var dcdata = new Array(rsBlocks.length); var ecdata = new Array(rsBlocks.length); for (var r = 0; r < rsBlocks.length; r++) {
			var dcCount = rsBlocks[r].dataCount; var ecCount = rsBlocks[r].totalCount - dcCount; maxDcCount = Math.max(maxDcCount, dcCount); maxEcCount = Math.max(maxEcCount, ecCount); dcdata[r] = new Array(dcCount); for (var i = 0; i < dcdata[r].length; i++) { dcdata[r][i] = 0xff & buffer.buffer[i + offset]; }
			offset += dcCount; var rsPoly = QRUtil.getErrorCorrectPolynomial(ecCount); var rawPoly = new QRPolynomial(dcdata[r], rsPoly.getLength() - 1); var modPoly = rawPoly.mod(rsPoly); ecdata[r] = new Array(rsPoly.getLength() - 1); for (var i = 0; i < ecdata[r].length; i++) { var modIndex = i + modPoly.getLength() - ecdata[r].length; ecdata[r][i] = (modIndex >= 0) ? modPoly.get(modIndex) : 0; }
		}
		var totalCodeCount = 0; for (var i = 0; i < rsBlocks.length; i++) { totalCodeCount += rsBlocks[i].totalCount; }
		var data = new Array(totalCodeCount); var index = 0; for (var i = 0; i < maxDcCount; i++) { for (var r = 0; r < rsBlocks.length; r++) { if (i < dcdata[r].length) { data[index++] = dcdata[r][i]; } } }
		for (var i = 0; i < maxEcCount; i++) { for (var r = 0; r < rsBlocks.length; r++) { if (i < ecdata[r].length) { data[index++] = ecdata[r][i]; } } }
		return data;
	}; var QRMode = { MODE_NUMBER: 1 << 0, MODE_ALPHA_NUM: 1 << 1, MODE_8BIT_BYTE: 1 << 2, MODE_KANJI: 1 << 3 }; var QRErrorCorrectLevel = { L: 1, M: 0, Q: 3, H: 2 }; var QRMaskPattern = { PATTERN000: 0, PATTERN001: 1, PATTERN010: 2, PATTERN011: 3, PATTERN100: 4, PATTERN101: 5, PATTERN110: 6, PATTERN111: 7 }; var QRUtil = {
		PATTERN_POSITION_TABLE: [[], [6, 18], [6, 22], [6, 26], [6, 30], [6, 34], [6, 22, 38], [6, 24, 42], [6, 26, 46], [6, 28, 50], [6, 30, 54], [6, 32, 58], [6, 34, 62], [6, 26, 46, 66], [6, 26, 48, 70], [6, 26, 50, 74], [6, 30, 54, 78], [6, 30, 56, 82], [6, 30, 58, 86], [6, 34, 62, 90], [6, 28, 50, 72, 94], [6, 26, 50, 74, 98], [6, 30, 54, 78, 102], [6, 28, 54, 80, 106], [6, 32, 58, 84, 110], [6, 30, 58, 86, 114], [6, 34, 62, 90, 118], [6, 26, 50, 74, 98, 122], [6, 30, 54, 78, 102, 126], [6, 26, 52, 78, 104, 130], [6, 30, 56, 82, 108, 134], [6, 34, 60, 86, 112, 138], [6, 30, 58, 86, 114, 142], [6, 34, 62, 90, 118, 146], [6, 30, 54, 78, 102, 126, 150], [6, 24, 50, 76, 102, 128, 154], [6, 28, 54, 80, 106, 132, 158], [6, 32, 58, 84, 110, 136, 162], [6, 26, 54, 82, 110, 138, 166], [6, 30, 58, 86, 114, 142, 170]], G15: (1 << 10) | (1 << 8) | (1 << 5) | (1 << 4) | (1 << 2) | (1 << 1) | (1 << 0), G18: (1 << 12) | (1 << 11) | (1 << 10) | (1 << 9) | (1 << 8) | (1 << 5) | (1 << 2) | (1 << 0), G15_MASK: (1 << 14) | (1 << 12) | (1 << 10) | (1 << 4) | (1 << 1), getBCHTypeInfo: function (data) {
			var d = data << 10; while (QRUtil.getBCHDigit(d) - QRUtil.getBCHDigit(QRUtil.G15) >= 0) { d ^= (QRUtil.G15 << (QRUtil.getBCHDigit(d) - QRUtil.getBCHDigit(QRUtil.G15))); }
			return ((data << 10) | d) ^ QRUtil.G15_MASK;
		}, getBCHTypeNumber: function (data) {
			var d = data << 12; while (QRUtil.getBCHDigit(d) - QRUtil.getBCHDigit(QRUtil.G18) >= 0) { d ^= (QRUtil.G18 << (QRUtil.getBCHDigit(d) - QRUtil.getBCHDigit(QRUtil.G18))); }
			return (data << 12) | d;
		}, getBCHDigit: function (data) {
			var digit = 0; while (data != 0) { digit++; data >>>= 1; }
			return digit;
		}, getPatternPosition: function (typeNumber) { return QRUtil.PATTERN_POSITION_TABLE[typeNumber - 1]; }, getMask: function (maskPattern, i, j) { switch (maskPattern) { case QRMaskPattern.PATTERN000: return (i + j) % 2 == 0; case QRMaskPattern.PATTERN001: return i % 2 == 0; case QRMaskPattern.PATTERN010: return j % 3 == 0; case QRMaskPattern.PATTERN011: return (i + j) % 3 == 0; case QRMaskPattern.PATTERN100: return (Math.floor(i / 2) + Math.floor(j / 3)) % 2 == 0; case QRMaskPattern.PATTERN101: return (i * j) % 2 + (i * j) % 3 == 0; case QRMaskPattern.PATTERN110: return ((i * j) % 2 + (i * j) % 3) % 2 == 0; case QRMaskPattern.PATTERN111: return ((i * j) % 3 + (i + j) % 2) % 2 == 0; default: throw new Error("bad maskPattern:" + maskPattern); } }, getErrorCorrectPolynomial: function (errorCorrectLength) {
			var a = new QRPolynomial([1], 0); for (var i = 0; i < errorCorrectLength; i++) { a = a.multiply(new QRPolynomial([1, QRMath.gexp(i)], 0)); }
			return a;
		}, getLengthInBits: function (mode, type) { if (1 <= type && type < 10) { switch (mode) { case QRMode.MODE_NUMBER: return 10; case QRMode.MODE_ALPHA_NUM: return 9; case QRMode.MODE_8BIT_BYTE: return 8; case QRMode.MODE_KANJI: return 8; default: throw new Error("mode:" + mode); } } else if (type < 27) { switch (mode) { case QRMode.MODE_NUMBER: return 12; case QRMode.MODE_ALPHA_NUM: return 11; case QRMode.MODE_8BIT_BYTE: return 16; case QRMode.MODE_KANJI: return 10; default: throw new Error("mode:" + mode); } } else if (type < 41) { switch (mode) { case QRMode.MODE_NUMBER: return 14; case QRMode.MODE_ALPHA_NUM: return 13; case QRMode.MODE_8BIT_BYTE: return 16; case QRMode.MODE_KANJI: return 12; default: throw new Error("mode:" + mode); } } else { throw new Error("type:" + type); } }, getLostPoint: function (qrCode) {
			var moduleCount = qrCode.getModuleCount(); var lostPoint = 0; for (var row = 0; row < moduleCount; row++) {
				for (var col = 0; col < moduleCount; col++) {
					var sameCount = 0; var dark = qrCode.isDark(row, col); for (var r = -1; r <= 1; r++) {
						if (row + r < 0 || moduleCount <= row + r) { continue; }
						for (var c = -1; c <= 1; c++) {
							if (col + c < 0 || moduleCount <= col + c) { continue; }
							if (r == 0 && c == 0) { continue; }
							if (dark == qrCode.isDark(row + r, col + c)) { sameCount++; }
						}
					}
					if (sameCount > 5) { lostPoint += (3 + sameCount - 5); }
				}
			}
			for (var row = 0; row < moduleCount - 1; row++) { for (var col = 0; col < moduleCount - 1; col++) { var count = 0; if (qrCode.isDark(row, col)) count++; if (qrCode.isDark(row + 1, col)) count++; if (qrCode.isDark(row, col + 1)) count++; if (qrCode.isDark(row + 1, col + 1)) count++; if (count == 0 || count == 4) { lostPoint += 3; } } }
			for (var row = 0; row < moduleCount; row++) { for (var col = 0; col < moduleCount - 6; col++) { if (qrCode.isDark(row, col) && !qrCode.isDark(row, col + 1) && qrCode.isDark(row, col + 2) && qrCode.isDark(row, col + 3) && qrCode.isDark(row, col + 4) && !qrCode.isDark(row, col + 5) && qrCode.isDark(row, col + 6)) { lostPoint += 40; } } }
			for (var col = 0; col < moduleCount; col++) { for (var row = 0; row < moduleCount - 6; row++) { if (qrCode.isDark(row, col) && !qrCode.isDark(row + 1, col) && qrCode.isDark(row + 2, col) && qrCode.isDark(row + 3, col) && qrCode.isDark(row + 4, col) && !qrCode.isDark(row + 5, col) && qrCode.isDark(row + 6, col)) { lostPoint += 40; } } }
			var darkCount = 0; for (var col = 0; col < moduleCount; col++) { for (var row = 0; row < moduleCount; row++) { if (qrCode.isDark(row, col)) { darkCount++; } } }
			var ratio = Math.abs(100 * darkCount / moduleCount / moduleCount - 50) / 5; lostPoint += ratio * 10; return lostPoint;
		}
	}; var QRMath = {
		glog: function (n) {
			if (n < 1) { throw new Error("glog(" + n + ")"); }
			return QRMath.LOG_TABLE[n];
		}, gexp: function (n) {
			while (n < 0) { n += 255; }
			while (n >= 256) { n -= 255; }
			return QRMath.EXP_TABLE[n];
		}, EXP_TABLE: new Array(256), LOG_TABLE: new Array(256)
	}; for (var i = 0; i < 8; i++) { QRMath.EXP_TABLE[i] = 1 << i; }
	for (var i = 8; i < 256; i++) { QRMath.EXP_TABLE[i] = QRMath.EXP_TABLE[i - 4] ^ QRMath.EXP_TABLE[i - 5] ^ QRMath.EXP_TABLE[i - 6] ^ QRMath.EXP_TABLE[i - 8]; }
	for (var i = 0; i < 255; i++) { QRMath.LOG_TABLE[QRMath.EXP_TABLE[i]] = i; }
	function QRPolynomial(num, shift) {
		if (num.length == undefined) { throw new Error(num.length + "/" + shift); }
		var offset = 0; while (offset < num.length && num[offset] == 0) { offset++; }
		this.num = new Array(num.length - offset + shift); for (var i = 0; i < num.length - offset; i++) { this.num[i] = num[i + offset]; }
	}
	QRPolynomial.prototype = {
		get: function (index) { return this.num[index]; }, getLength: function () { return this.num.length; }, multiply: function (e) {
			var num = new Array(this.getLength() + e.getLength() - 1); for (var i = 0; i < this.getLength(); i++) { for (var j = 0; j < e.getLength(); j++) { num[i + j] ^= QRMath.gexp(QRMath.glog(this.get(i)) + QRMath.glog(e.get(j))); } }
			return new QRPolynomial(num, 0);
		}, mod: function (e) {
			if (this.getLength() - e.getLength() < 0) { return this; }
			var ratio = QRMath.glog(this.get(0)) - QRMath.glog(e.get(0)); var num = new Array(this.getLength()); for (var i = 0; i < this.getLength(); i++) { num[i] = this.get(i); }
			for (var i = 0; i < e.getLength(); i++) { num[i] ^= QRMath.gexp(QRMath.glog(e.get(i)) + ratio); }
			return new QRPolynomial(num, 0).mod(e);
		}
	}; function QRRSBlock(totalCount, dataCount) { this.totalCount = totalCount; this.dataCount = dataCount; }
	QRRSBlock.RS_BLOCK_TABLE = [[1, 26, 19], [1, 26, 16], [1, 26, 13], [1, 26, 9], [1, 44, 34], [1, 44, 28], [1, 44, 22], [1, 44, 16], [1, 70, 55], [1, 70, 44], [2, 35, 17], [2, 35, 13], [1, 100, 80], [2, 50, 32], [2, 50, 24], [4, 25, 9], [1, 134, 108], [2, 67, 43], [2, 33, 15, 2, 34, 16], [2, 33, 11, 2, 34, 12], [2, 86, 68], [4, 43, 27], [4, 43, 19], [4, 43, 15], [2, 98, 78], [4, 49, 31], [2, 32, 14, 4, 33, 15], [4, 39, 13, 1, 40, 14], [2, 121, 97], [2, 60, 38, 2, 61, 39], [4, 40, 18, 2, 41, 19], [4, 40, 14, 2, 41, 15], [2, 146, 116], [3, 58, 36, 2, 59, 37], [4, 36, 16, 4, 37, 17], [4, 36, 12, 4, 37, 13], [2, 86, 68, 2, 87, 69], [4, 69, 43, 1, 70, 44], [6, 43, 19, 2, 44, 20], [6, 43, 15, 2, 44, 16], [4, 101, 81], [1, 80, 50, 4, 81, 51], [4, 50, 22, 4, 51, 23], [3, 36, 12, 8, 37, 13], [2, 116, 92, 2, 117, 93], [6, 58, 36, 2, 59, 37], [4, 46, 20, 6, 47, 21], [7, 42, 14, 4, 43, 15], [4, 133, 107], [8, 59, 37, 1, 60, 38], [8, 44, 20, 4, 45, 21], [12, 33, 11, 4, 34, 12], [3, 145, 115, 1, 146, 116], [4, 64, 40, 5, 65, 41], [11, 36, 16, 5, 37, 17], [11, 36, 12, 5, 37, 13], [5, 109, 87, 1, 110, 88], [5, 65, 41, 5, 66, 42], [5, 54, 24, 7, 55, 25], [11, 36, 12], [5, 122, 98, 1, 123, 99], [7, 73, 45, 3, 74, 46], [15, 43, 19, 2, 44, 20], [3, 45, 15, 13, 46, 16], [1, 135, 107, 5, 136, 108], [10, 74, 46, 1, 75, 47], [1, 50, 22, 15, 51, 23], [2, 42, 14, 17, 43, 15], [5, 150, 120, 1, 151, 121], [9, 69, 43, 4, 70, 44], [17, 50, 22, 1, 51, 23], [2, 42, 14, 19, 43, 15], [3, 141, 113, 4, 142, 114], [3, 70, 44, 11, 71, 45], [17, 47, 21, 4, 48, 22], [9, 39, 13, 16, 40, 14], [3, 135, 107, 5, 136, 108], [3, 67, 41, 13, 68, 42], [15, 54, 24, 5, 55, 25], [15, 43, 15, 10, 44, 16], [4, 144, 116, 4, 145, 117], [17, 68, 42], [17, 50, 22, 6, 51, 23], [19, 46, 16, 6, 47, 17], [2, 139, 111, 7, 140, 112], [17, 74, 46], [7, 54, 24, 16, 55, 25], [34, 37, 13], [4, 151, 121, 5, 152, 122], [4, 75, 47, 14, 76, 48], [11, 54, 24, 14, 55, 25], [16, 45, 15, 14, 46, 16], [6, 147, 117, 4, 148, 118], [6, 73, 45, 14, 74, 46], [11, 54, 24, 16, 55, 25], [30, 46, 16, 2, 47, 17], [8, 132, 106, 4, 133, 107], [8, 75, 47, 13, 76, 48], [7, 54, 24, 22, 55, 25], [22, 45, 15, 13, 46, 16], [10, 142, 114, 2, 143, 115], [19, 74, 46, 4, 75, 47], [28, 50, 22, 6, 51, 23], [33, 46, 16, 4, 47, 17], [8, 152, 122, 4, 153, 123], [22, 73, 45, 3, 74, 46], [8, 53, 23, 26, 54, 24], [12, 45, 15, 28, 46, 16], [3, 147, 117, 10, 148, 118], [3, 73, 45, 23, 74, 46], [4, 54, 24, 31, 55, 25], [11, 45, 15, 31, 46, 16], [7, 146, 116, 7, 147, 117], [21, 73, 45, 7, 74, 46], [1, 53, 23, 37, 54, 24], [19, 45, 15, 26, 46, 16], [5, 145, 115, 10, 146, 116], [19, 75, 47, 10, 76, 48], [15, 54, 24, 25, 55, 25], [23, 45, 15, 25, 46, 16], [13, 145, 115, 3, 146, 116], [2, 74, 46, 29, 75, 47], [42, 54, 24, 1, 55, 25], [23, 45, 15, 28, 46, 16], [17, 145, 115], [10, 74, 46, 23, 75, 47], [10, 54, 24, 35, 55, 25], [19, 45, 15, 35, 46, 16], [17, 145, 115, 1, 146, 116], [14, 74, 46, 21, 75, 47], [29, 54, 24, 19, 55, 25], [11, 45, 15, 46, 46, 16], [13, 145, 115, 6, 146, 116], [14, 74, 46, 23, 75, 47], [44, 54, 24, 7, 55, 25], [59, 46, 16, 1, 47, 17], [12, 151, 121, 7, 152, 122], [12, 75, 47, 26, 76, 48], [39, 54, 24, 14, 55, 25], [22, 45, 15, 41, 46, 16], [6, 151, 121, 14, 152, 122], [6, 75, 47, 34, 76, 48], [46, 54, 24, 10, 55, 25], [2, 45, 15, 64, 46, 16], [17, 152, 122, 4, 153, 123], [29, 74, 46, 14, 75, 47], [49, 54, 24, 10, 55, 25], [24, 45, 15, 46, 46, 16], [4, 152, 122, 18, 153, 123], [13, 74, 46, 32, 75, 47], [48, 54, 24, 14, 55, 25], [42, 45, 15, 32, 46, 16], [20, 147, 117, 4, 148, 118], [40, 75, 47, 7, 76, 48], [43, 54, 24, 22, 55, 25], [10, 45, 15, 67, 46, 16], [19, 148, 118, 6, 149, 119], [18, 75, 47, 31, 76, 48], [34, 54, 24, 34, 55, 25], [20, 45, 15, 61, 46, 16]]; QRRSBlock.getRSBlocks = function (typeNumber, errorCorrectLevel) {
		var rsBlock = QRRSBlock.getRsBlockTable(typeNumber, errorCorrectLevel); if (rsBlock == undefined) { throw new Error("bad rs block @ typeNumber:" + typeNumber + "/errorCorrectLevel:" + errorCorrectLevel); }
		var length = rsBlock.length / 3; var list = []; for (var i = 0; i < length; i++) { var count = rsBlock[i * 3 + 0]; var totalCount = rsBlock[i * 3 + 1]; var dataCount = rsBlock[i * 3 + 2]; for (var j = 0; j < count; j++) { list.push(new QRRSBlock(totalCount, dataCount)); } }
		return list;
	}; QRRSBlock.getRsBlockTable = function (typeNumber, errorCorrectLevel) { switch (errorCorrectLevel) { case QRErrorCorrectLevel.L: return QRRSBlock.RS_BLOCK_TABLE[(typeNumber - 1) * 4 + 0]; case QRErrorCorrectLevel.M: return QRRSBlock.RS_BLOCK_TABLE[(typeNumber - 1) * 4 + 1]; case QRErrorCorrectLevel.Q: return QRRSBlock.RS_BLOCK_TABLE[(typeNumber - 1) * 4 + 2]; case QRErrorCorrectLevel.H: return QRRSBlock.RS_BLOCK_TABLE[(typeNumber - 1) * 4 + 3]; default: return undefined; } }; function QRBitBuffer() { this.buffer = []; this.length = 0; }
	QRBitBuffer.prototype = {
		get: function (index) { var bufIndex = Math.floor(index / 8); return ((this.buffer[bufIndex] >>> (7 - index % 8)) & 1) == 1; }, put: function (num, length) { for (var i = 0; i < length; i++) { this.putBit(((num >>> (length - i - 1)) & 1) == 1); } }, getLengthInBits: function () { return this.length; }, putBit: function (bit) {
			var bufIndex = Math.floor(this.length / 8); if (this.buffer.length <= bufIndex) { this.buffer.push(0); }
			if (bit) { this.buffer[bufIndex] |= (0x80 >>> (this.length % 8)); }
			this.length++;
		}
	}; var QRCodeLimitLength = [[17, 14, 11, 7], [32, 26, 20, 14], [53, 42, 32, 24], [78, 62, 46, 34], [106, 84, 60, 44], [134, 106, 74, 58], [154, 122, 86, 64], [192, 152, 108, 84], [230, 180, 130, 98], [271, 213, 151, 119], [321, 251, 177, 137], [367, 287, 203, 155], [425, 331, 241, 177], [458, 362, 258, 194], [520, 412, 292, 220], [586, 450, 322, 250], [644, 504, 364, 280], [718, 560, 394, 310], [792, 624, 442, 338], [858, 666, 482, 382], [929, 711, 509, 403], [1003, 779, 565, 439], [1091, 857, 611, 461], [1171, 911, 661, 511], [1273, 997, 715, 535], [1367, 1059, 751, 593], [1465, 1125, 805, 625], [1528, 1190, 868, 658], [1628, 1264, 908, 698], [1732, 1370, 982, 742], [1840, 1452, 1030, 790], [1952, 1538, 1112, 842], [2068, 1628, 1168, 898], [2188, 1722, 1228, 958], [2303, 1809, 1283, 983], [2431, 1911, 1351, 1051], [2563, 1989, 1423, 1093], [2699, 2099, 1499, 1139], [2809, 2213, 1579, 1219], [2953, 2331, 1663, 1273]];

	function _isSupportCanvas() {
		return typeof CanvasRenderingContext2D != "undefined";
	}

	// android 2.x doesn't support Data-URI spec
	function _getAndroid() {
		var android = false;
		var sAgent = navigator.userAgent;

		if (/android/i.test(sAgent)) { // android
			android = true;
			var aMat = sAgent.toString().match(/android ([0-9]\.[0-9])/i);

			if (aMat && aMat[1]) {
				android = parseFloat(aMat[1]);
			}
		}

		return android;
	}

	var svgDrawer = (function () {

		var Drawing = function (el, htOption) {
			this._el = el;
			this._htOption = htOption;
		};

		Drawing.prototype.draw = function (oQRCode) {
			var _htOption = this._htOption;
			var _el = this._el;
			var nCount = oQRCode.getModuleCount();
			var nWidth = Math.floor(_htOption.width / nCount);
			var nHeight = Math.floor(_htOption.height / nCount);

			this.clear();

			function makeSVG(tag, attrs) {
				var el = document.createElementNS('http://www.w3.org/2000/svg', tag);
				for (var k in attrs)
					if (attrs.hasOwnProperty(k)) el.setAttribute(k, attrs[k]);
				return el;
			}

			var svg = makeSVG("svg", { 'viewBox': '0 0 ' + String(nCount) + " " + String(nCount), 'width': '100%', 'height': '100%', 'fill': _htOption.colorLight });
			svg.setAttributeNS("http://www.w3.org/2000/xmlns/", "xmlns:xlink", "http://www.w3.org/1999/xlink");
			_el.appendChild(svg);

			svg.appendChild(makeSVG("rect", { "fill": _htOption.colorLight, "width": "100%", "height": "100%" }));
			svg.appendChild(makeSVG("rect", { "fill": _htOption.colorDark, "width": "1", "height": "1", "id": "template" }));

			for (var row = 0; row < nCount; row++) {
				for (var col = 0; col < nCount; col++) {
					if (oQRCode.isDark(row, col)) {
						var child = makeSVG("use", { "x": String(col), "y": String(row) });
						child.setAttributeNS("http://www.w3.org/1999/xlink", "href", "#template")
						svg.appendChild(child);
					}
				}
			}
		};
		Drawing.prototype.clear = function () {
			while (this._el.hasChildNodes())
				this._el.removeChild(this._el.lastChild);
		};
		return Drawing;
	})();

	var useSVG = document.documentElement.tagName.toLowerCase() === "svg";

	// Drawing in DOM by using Table tag
	var Drawing = useSVG ? svgDrawer : !_isSupportCanvas() ? (function () {
		var Drawing = function (el, htOption) {
			this._el = el;
			this._htOption = htOption;
		};

		/**
		 * Draw the QRCode
		 * 
		 * @param {QRCode} oQRCode
		 */
		Drawing.prototype.draw = function (oQRCode) {
			var _htOption = this._htOption;
			var _el = this._el;
			var nCount = oQRCode.getModuleCount();
			var nWidth = Math.floor(_htOption.width / nCount);
			var nHeight = Math.floor(_htOption.height / nCount);
			var aHTML = ['<table style="border:0;border-collapse:collapse;">'];

			for (var row = 0; row < nCount; row++) {
				aHTML.push('<tr>');

				for (var col = 0; col < nCount; col++) {
					aHTML.push('<td style="border:0;border-collapse:collapse;padding:0;margin:0;width:' + nWidth + 'px;height:' + nHeight + 'px;background-color:' + (oQRCode.isDark(row, col) ? _htOption.colorDark : _htOption.colorLight) + ';"></td>');
				}

				aHTML.push('</tr>');
			}

			aHTML.push('</table>');
			_el.innerHTML = aHTML.join('');

			// Fix the margin values as real size.
			var elTable = _el.childNodes[0];
			var nLeftMarginTable = (_htOption.width - elTable.offsetWidth) / 2;
			var nTopMarginTable = (_htOption.height - elTable.offsetHeight) / 2;

			if (nLeftMarginTable > 0 && nTopMarginTable > 0) {
				elTable.style.margin = nTopMarginTable + "px " + nLeftMarginTable + "px";
			}
		};

		/**
		 * Clear the QRCode
		 */
		Drawing.prototype.clear = function () {
			this._el.innerHTML = '';
		};

		return Drawing;
	})() : (function () { // Drawing in Canvas
		function _onMakeImage() {
			this._elImage.src = this._elCanvas.toDataURL("image/png");
			this._elImage.style.display = "block";
			this._elCanvas.style.display = "none";
		}

		// Android 2.1 bug workaround
		// http://code.google.com/p/android/issues/detail?id=5141
		if (this._android && this._android <= 2.1) {
			var factor = 1 / window.devicePixelRatio;
			var drawImage = CanvasRenderingContext2D.prototype.drawImage;
			CanvasRenderingContext2D.prototype.drawImage = function (image, sx, sy, sw, sh, dx, dy, dw, dh) {
				if (("nodeName" in image) && /img/i.test(image.nodeName)) {
					for (var i = arguments.length - 1; i >= 1; i--) {
						arguments[i] = arguments[i] * factor;
					}
				} else if (typeof dw == "undefined") {
					arguments[1] *= factor;
					arguments[2] *= factor;
					arguments[3] *= factor;
					arguments[4] *= factor;
				}

				drawImage.apply(this, arguments);
			};
		}

		/**
		 * Check whether the user's browser supports Data URI or not
		 * 
		 * @private
		 * @param {Function} fSuccess Occurs if it supports Data URI
		 * @param {Function} fFail Occurs if it doesn't support Data URI
		 */
		function _safeSetDataURI(fSuccess, fFail) {
			var self = this;
			self._fFail = fFail;
			self._fSuccess = fSuccess;

			// Check it just once
			if (self._bSupportDataURI === null) {
				var el = document.createElement("img");
				var fOnError = function () {
					self._bSupportDataURI = false;

					if (self._fFail) {
						self._fFail.call(self);
					}
				};
				var fOnSuccess = function () {
					self._bSupportDataURI = true;

					if (self._fSuccess) {
						self._fSuccess.call(self);
					}
				};

				el.onabort = fOnError;
				el.onerror = fOnError;
				el.onload = fOnSuccess;
				el.src = "data:image/gif;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg=="; // the Image contains 1px data.
				return;
			} else if (self._bSupportDataURI === true && self._fSuccess) {
				self._fSuccess.call(self);
			} else if (self._bSupportDataURI === false && self._fFail) {
				self._fFail.call(self);
			}
		};

		/**
		 * Drawing QRCode by using canvas
		 * 
		 * @constructor
		 * @param {HTMLElement} el
		 * @param {Object} htOption QRCode Options 
		 */
		var Drawing = function (el, htOption) {
			this._bIsPainted = false;
			this._android = _getAndroid();

			this._htOption = htOption;
			this._elCanvas = document.createElement("canvas");
			this._elCanvas.width = htOption.width;
			this._elCanvas.height = htOption.height;
			el.appendChild(this._elCanvas);
			this._el = el;
			this._oContext = this._elCanvas.getContext("2d");
			this._bIsPainted = false;
			this._elImage = document.createElement("img");
			this._elImage.alt = "Scan me!";
			this._elImage.style.display = "none";
			this._el.appendChild(this._elImage);
			this._bSupportDataURI = null;
		};

		/**
		 * Draw the QRCode
		 * 
		 * @param {QRCode} oQRCode 
		 */
		Drawing.prototype.draw = function (oQRCode) {
			var _elImage = this._elImage;
			var _oContext = this._oContext;
			var _htOption = this._htOption;

			var nCount = oQRCode.getModuleCount();
			var nWidth = _htOption.width / nCount;
			var nHeight = _htOption.height / nCount;
			var nRoundedWidth = Math.round(nWidth);
			var nRoundedHeight = Math.round(nHeight);

			_elImage.style.display = "none";
			this.clear();

			for (var row = 0; row < nCount; row++) {
				for (var col = 0; col < nCount; col++) {
					var bIsDark = oQRCode.isDark(row, col);
					var nLeft = col * nWidth;
					var nTop = row * nHeight;
					_oContext.strokeStyle = bIsDark ? _htOption.colorDark : _htOption.colorLight;
					_oContext.lineWidth = 1;
					_oContext.fillStyle = bIsDark ? _htOption.colorDark : _htOption.colorLight;
					_oContext.fillRect(nLeft, nTop, nWidth, nHeight);

					// 안티 앨리어싱 방지 처리
					_oContext.strokeRect(
						Math.floor(nLeft) + 0.5,
						Math.floor(nTop) + 0.5,
						nRoundedWidth,
						nRoundedHeight
					);

					_oContext.strokeRect(
						Math.ceil(nLeft) - 0.5,
						Math.ceil(nTop) - 0.5,
						nRoundedWidth,
						nRoundedHeight
					);
				}
			}

			this._bIsPainted = true;
		};

		/**
		 * Make the image from Canvas if the browser supports Data URI.
		 */
		Drawing.prototype.makeImage = function () {
			if (this._bIsPainted) {
				_safeSetDataURI.call(this, _onMakeImage);
			}
		};

		/**
		 * Return whether the QRCode is painted or not
		 * 
		 * @return {Boolean}
		 */
		Drawing.prototype.isPainted = function () {
			return this._bIsPainted;
		};

		/**
		 * Clear the QRCode
		 */
		Drawing.prototype.clear = function () {
			this._oContext.clearRect(0, 0, this._elCanvas.width, this._elCanvas.height);
			this._bIsPainted = false;
		};

		/**
		 * @private
		 * @param {Number} nNumber
		 */
		Drawing.prototype.round = function (nNumber) {
			if (!nNumber) {
				return nNumber;
			}

			return Math.floor(nNumber * 1000) / 1000;
		};

		return Drawing;
	})();

	/**
	 * Get the type by string length
	 * 
	 * @private
	 * @param {String} sText
	 * @param {Number} nCorrectLevel
	 * @return {Number} type
	 */
	function _getTypeNumber(sText, nCorrectLevel) {
		var nType = 1;
		var length = _getUTF8Length(sText);

		for (var i = 0, len = QRCodeLimitLength.length; i <= len; i++) {
			var nLimit = 0;

			switch (nCorrectLevel) {
				case QRErrorCorrectLevel.L:
					nLimit = QRCodeLimitLength[i][0];
					break;
				case QRErrorCorrectLevel.M:
					nLimit = QRCodeLimitLength[i][1];
					break;
				case QRErrorCorrectLevel.Q:
					nLimit = QRCodeLimitLength[i][2];
					break;
				case QRErrorCorrectLevel.H:
					nLimit = QRCodeLimitLength[i][3];
					break;
			}

			if (length <= nLimit) {
				break;
			} else {
				nType++;
			}
		}

		if (nType > QRCodeLimitLength.length) {
			throw new Error("Too long data");
		}

		return nType;
	}

	function _getUTF8Length(sText) {
		var replacedText = encodeURI(sText).toString().replace(/\%[0-9a-fA-F]{2}/g, 'a');
		return replacedText.length + (replacedText.length != sText ? 3 : 0);
	}

	/**
	 * @class QRCode
	 * @constructor
	 * @example 
	 * new QRCode(document.getElementById("test"), "http://jindo.dev.naver.com/collie");
	 *
	 * @example
	 * var oQRCode = new QRCode("test", {
	 *    text : "http://naver.com",
	 *    width : 128,
	 *    height : 128
	 * });
	 * 
	 * oQRCode.clear(); // Clear the QRCode.
	 * oQRCode.makeCode("http://map.naver.com"); // Re-create the QRCode.
	 *
	 * @param {HTMLElement|String} el target element or 'id' attribute of element.
	 * @param {Object|String} vOption
	 * @param {String} vOption.text QRCode link data
	 * @param {Number} [vOption.width=256]
	 * @param {Number} [vOption.height=256]
	 * @param {String} [vOption.colorDark="#000000"]
	 * @param {String} [vOption.colorLight="#ffffff"]
	 * @param {QRCode.CorrectLevel} [vOption.correctLevel=QRCode.CorrectLevel.H] [L|M|Q|H] 
	 */
	QRCode = function (el, vOption) {
		this._htOption = {
			width: 256,
			height: 256,
			typeNumber: 4,
			colorDark: "#000000",
			colorLight: "#ffffff",
			correctLevel: QRErrorCorrectLevel.H
		};

		if (typeof vOption === 'string') {
			vOption = {
				text: vOption
			};
		}

		// Overwrites options
		if (vOption) {
			for (var i in vOption) {
				this._htOption[i] = vOption[i];
			}
		}

		if (typeof el == "string") {
			el = document.getElementById(el);
		}

		if (this._htOption.useSVG) {
			Drawing = svgDrawer;
		}

		this._android = _getAndroid();
		this._el = el;
		this._oQRCode = null;
		this._oDrawing = new Drawing(this._el, this._htOption);

		if (this._htOption.text) {
			this.makeCode(this._htOption.text);
		}
	};

	/**
	 * Make the QRCode
	 * 
	 * @param {String} sText link data
	 */
	QRCode.prototype.makeCode = function (sText) {
		this._oQRCode = new QRCodeModel(_getTypeNumber(sText, this._htOption.correctLevel), this._htOption.correctLevel);
		this._oQRCode.addData(sText);
		this._oQRCode.make();
		this._el.title = sText;
		this._oDrawing.draw(this._oQRCode);
		this.makeImage();
	};

	/**
	 * Make the Image from Canvas element
	 * - It occurs automatically
	 * - Android below 3 doesn't support Data-URI spec.
	 * 
	 * @private
	 */
	QRCode.prototype.makeImage = function () {
		if (typeof this._oDrawing.makeImage == "function" && (!this._android || this._android >= 3)) {
			this._oDrawing.makeImage();
		}
	};

	/**
	 * Clear the QRCode
	 */
	QRCode.prototype.clear = function () {
		this._oDrawing.clear();
	};

	/**
	 * @name QRCode.CorrectLevel
	 */
	QRCode.CorrectLevel = QRErrorCorrectLevel;
})();