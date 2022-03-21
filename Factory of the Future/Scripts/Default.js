﻿/// A simple template method for replacing placeholders enclosed in curly braces.
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
var connectattp = 0;
var connecttimer;
let fotfmanager = $.connection.FOTFManager;
$(function () {
    $("form").submit(function () { return false; });
  
    setHeight();
    $(window).resize(function () {
        setHeight();
    });

    //on close clear all IDS inputs

    $('#CTS_Details_Modal').on('hidden.bs.modal', () => {
        $CTSDetails_Table = $('table[id=ctsdetailstable]');
        $CTSDetails_Table_Header = $CTSDetails_Table.find('thead');
        $CTSDetails_Table_Header.empty();
        $CTSDetails_Table_Body = $CTSDetails_Table.find('tbody')
        $CTSDetails_Table_Body.empty();
    });
    $('#CTS_Details_Modal').on('shown.bs.modal', () => {
        $("#modal-preloader").show();
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
        updateClock: async (timer) => {
            updateTime(timer);
            $('#twentyfourmessage').text(GetTwentyFourMessage(timer));
            if ($("#tfhcContent").length > 0) {
                SetClockHands(timer);
            }
            ///badgecomplaint();
            zonecurrentStaff();
            var visible = sidebar._getTab("reports");
            if (visible) {
                if (visible.classList.length) {
                    if (visible.classList.contains('active')) {
                        GetUserInfo();
                    }
                }
            }

        }
        
    });

    Initiate24HourClock();

    


    //add connection status
    var conntoggle = L.easyButton({
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
            icon: '<div id="faqToggle" data-toggle="popover" title="Frequently Asked Questions" ><i class="pi-iconFAQ align-self-center"></i></div> ',
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

  

    /******Connection status start******/
    async function init_Connection() {
        fotfmanager.server.getUserProfile().done(function (User_profile) {
            User = User_profile;
    
            if (/^Admin/i.test(User.Role)) {
                sidebar.addPanel({
                    id: 'connections',
                    tab: '<i class="pi-iconDiagramOutline"></i>',
                    position: 'top',
                    pane: '<div class="btn-toolbar" role="toolbar" id="connection_div">' +
                        '<div id="div_agvnotification" class="container-fluid">' +
                        '<h4>API Settings</h4>' +
                        '<button type="button" class="btn btn-primary float-left mb-2" name="addconnection">Add</button>' +
                        '<div class="card w-100 bg-white mt-2 pb-1">' +
                        '<div class="card-body">' +
                        '<div class="table-responsive">' +
                        '<table class="table table-sm table-hover table-condensed mb-1" id="connectiontable" style="border-collapse:collapse;">' +
                        '<thead class="thead-dark">' +
                        '<tr>' +
                        '<th class="row-connection-name">Name</th><th class="row-connection-type">Message Type</th><th class="row-connection-status">Status</th><th class="row-connection-action">Action</th>' +
                        '</tr>' +
                        '</thead>' +
                        '<tbody></tbody>' +
                        '</table>' +
                        '</div>' +
                        '</div>' +
                        '</div >' +
                        '</div></div>'
                });
                init_connection();
                if (!/^https/i.test(window.location.protocol)) {
                    sidebar.addPanel({
                        id: 'camera_video',
                        tab: '<i class="bi-camera-video"></i>',
                        position: 'top',
                        pane: '<div class="btn-toolbar" role="toolbar" id="camera_display">' +
                            '<div id="div_camera" class="container-fluid">' +
                            '<h4>Web Cameras</h4>' +
                            '<button type="button" class="btn btn-primary float-left mb-2" name="addcamera">Add</button>' +
                            '<div class="card w-100 bg-white mt-2 pb-1">' +
                            '<div class="card-body">' +
                            '<div class="table-responsive">' +
                            '<table class="table table-sm table-hover table-condensed mb-1" id="cameratable" style="border-collapse:collapse;">' +
                            '<thead class="thead-dark">' +
                            '<tr>' +
                            '<th class="row-connection-name">Name</th><th class="row-connection-type">Description</th><th class="row-connection-action">View Camera</th>' +
                            '</tr>' +
                            '</thead>' +
                            '<tbody></tbody>' +
                            '</table>' +
                            '</div>' +
                            '</div>' +
                            '</div >' +
                            '</div></div>'
                    });
                    init_cameras();
                    $('button[name=addcamera]').off().on('click', function () {
                        /* close the sidebar */
                        sidebar.close();
                        Add_Camera();
                    });
                }
               // init_geometry_editing();
                $('button[name=addconnection]').off().on('click', function () {
                    /* close the sidebar */
                    sidebar.close();
                    Add_Connection();
                });
                $('button[name=machineinfoedit]').css('display', 'block');
            
            }

         
            if (/^Admin/i.test(User.Role)) {
                sidebar.addPanel({
                    id: 'setting',
                    tab: '<i class="pi-iconGearFill"></i>',
                    position: 'bottom',
                    pane: '<div class="btn-toolbar" role="toolbar" id="app_setting">' +
                        '<div id="div_app_settingtable" class="container-fluid">' +
                        '<div class="card w-100">' +
                        '<div class="card-body">' +
                        '<div class="table-responsive fixedHeader" style="max-height: calc(100vh - 100px); ">' +
                        '<table class="table table-sm table-hover table-condensed" id="app_settingtable" style="border-collapse:collapse;">' +
                        '<thead class="thead-dark">' +
                        '<tr>' +
                        '<th class="row-name">Name</th><th class="row-value">Value</th><th class="row-action">Action</th>' +
                        '</tr>' +
                        '</thead>' +
                        '<tbody></tbody>' +
                        '</table>' +
                        '</div>' +
                        '</div>' +
                        '</div >' +
                        '</div></div>'
                });
               
            }
            if (/(^PMCCUser$)/i.test(User.UserId)) {
                map.removeLayer(tagsMarkersGroup);
                //add QRCode
                var QRCodedisplay = L.Control.extend({
                    options: {
                        position: 'topright'
                    },
                    onAdd: function () {
                        var Domcntainer = L.DomUtil.create('div');
                        Domcntainer.id = "qrcodeUrl";
                        return Domcntainer;
                    }
                });
                map.addControl(new QRCodedisplay());

                var qrcode = new QRCode("qrcodeUrl", {
                    text: window.location.href,
                    width: 128,
                    height: 128,
                    colorDark: "#000000",
                    colorLight: "#ffffff",
                    correctLevel: QRCode.CorrectLevel.H
                });
            }
        });
    }
    /******Connection status end******/

    // SelectizeJs Init for searching select boxes.
    var options = {
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
                            var jsonObject = {};
                            $('input[type=text][name=empId]').val() !== map._layers[layerindex].feature.properties.Employee_EIN ? jsonObject.Employee_EIN = $('input[type=text][name=employee_ein]').val() : "";
                            $('input[type=text][name=empName]').val() !== map._layers[layerindex].feature.properties.Employee_Name ? jsonObject.Employee_Name = $('input[type=text][name=employee_name]').val() : "";
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

    // Start the connection
    $.connection.hub.qs = { 'page_type': "FOTF".toUpperCase() };
    $.connection.hub.start()
        .then(init_Connection)
        .then(init_Map)
        .done(function () {
            conntoggle.state('conn-on');
        }).catch(
            function (err) {
                console.log(err.toString());
            });
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
    // current zone staff
    //TODO: look in to this more.
    async function zonecurrentStaff() {
        try {
            //clear Old tags 
            if (tagsMarkersGroup.hasOwnProperty("_layers")) {
                $.map(tagsMarkersGroup._layers, (layer) => {
                    if (layer.hasOwnProperty("feature")) {
                        if (layer.feature.properties.hasOwnProperty("Tag_Type")) {
                            if (/person/i.test(layer.feature.properties.Tag_Type)) {
                                if (layer.feature.properties.tagVisible === true) {
                                    if (layer.feature.properties.hasOwnProperty("positionTS")) {
                                        var startTime = moment(layer.feature.properties.positionTS);
                                        var endTime = moment();
                                        var diffmillsec = endTime.diff(startTime, "milliseconds");

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
                        }
                    }
                })
            }
            // Machine zone
            if (polygonMachine.hasOwnProperty("_layers")) {
                $.map(polygonMachine._layers, function (Machinelayer, i) {
                    var MachineCurrentStaff = [];
                    if (tagsMarkersGroup.hasOwnProperty("_layers")) {
                        $.map(tagsMarkersGroup._layers, function (layer, i) {
                            if (layer.hasOwnProperty("feature")) {
                                if (/person/i.test(layer.feature.properties.Tag_Type)) {
                                    if (layer.feature.properties.hasOwnProperty("zones")) {
                                        $.map(layer.feature.properties.zones, function (p_zone) {
                                            if (p_zone.id == Machinelayer.feature.properties.id) {
                                                if (layer.hasOwnProperty("_tooltip")) {
                                                    if (layer._tooltip.hasOwnProperty("_container")) {
                                                        if (!layer._tooltip._container.classList.contains('tooltip-hidden')) {
                                                            MachineCurrentStaff.push({
                                                                name: checkValue(layer.feature.properties.craftName) ? layer.feature.properties.craftName : layer.feature.properties.id,
                                                                nameId: checkValue(layer.feature.properties.id) ? layer.feature.properties.id : layer.feature.properties.id,
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

                    if (Machinelayer.hasOwnProperty("_tooltip")) {
                        if (MachineCurrentStaff.length !== Machinelayer.feature.properties.CurrentStaff) {
                            Machinelayer.setTooltipContent(Machinelayer.feature.properties.name + "<br/>" + "Staffing: " + MachineCurrentStaff.length);
                            Machinelayer.feature.properties.CurrentStaff = MachineCurrentStaff.length;
                            if ($('select[id=zoneselect] option:selected').val() === Machinelayer.feature.properties.id) {
                                var p2pdata = Machinelayer.feature.properties.hasOwnProperty("P2PData") ? Machinelayer.feature.properties.P2PData : "";
                                GetPeopleInZone(Machinelayer.feature.properties.id, p2pdata, MachineCurrentStaff);
                            }
                        }
                    }
                });
            }
            // other zone
            if (stagingAreas.hasOwnProperty("_layers")) {
                $.map(stagingAreas._layers, function (stagelayer, i) {
                    var CurrentStaff = [];
                    if (tagsMarkersGroup.hasOwnProperty("_layers")) {
                        $.map(tagsMarkersGroup._layers, function (layer, i) {
                            if (layer.hasOwnProperty("feature")) {
                                if (/person/i.test(layer.feature.properties.Tag_Type)) {
                                    if (layer.feature.properties.hasOwnProperty("zones")) {
                                        $.map(layer.feature.properties.zones, function (p_zone) {
                                            if (p_zone.id == stagelayer.feature.properties.id) {
                                                if (layer.hasOwnProperty("_tooltip")) {
                                                    if (layer._tooltip.hasOwnProperty("_container")) {
                                                        if (!layer._tooltip._container.classList.contains('tooltip-hidden')) {
                                                            CurrentStaff.push({
                                                                name: checkValue(layer.feature.properties.craftName) ? layer.feature.properties.craftName : layer.feature.properties.id,
                                                                nameId: checkValue(layer.feature.properties.id) ? layer.feature.properties.id : layer.feature.properties.id,
                                                                id: layer.feature.properties.id
                                                            })
                                                            //CurrentStaff.push({
                                                            //    id: layer.feature.properties.id
                                                            //})
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

                    if (stagelayer.hasOwnProperty("_tooltip")) {
                        if (CurrentStaff.length !== stagelayer.feature.properties.CurrentStaff) {
                            stagelayer.setTooltipContent(stagelayer.feature.properties.name + "<br/>" + "Staffing: " + CurrentStaff.length);
                            stagelayer.feature.properties.CurrentStaff = CurrentStaff.length;
                            if (stagingAreas.hasOwnProperty("feature")) {
                                if ($('select[id=zoneselect] option:selected').val() === stagingAreas.feature.properties.id) {
                                    var p2pdata = stagelayer.feature.properties.hasOwnProperty("P2PData") ? stagelayer.feature.properties.P2PData : "";
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
    $(document).on('click', '.viewportszones', function () {
        try {
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            var selcValue = this.id;
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
    $(document).on('click', '.connectionedit', function () {
        var td = $(this);
        var tr = $(td).closest('tr'),
            id = tr.attr('data-id');
        Edit_Connection(id);
    });
    $(document).on('click', '.connectiondelete', function () {
        var td = $(this);
        var tr = $(td).closest('tr'),
            id = tr.attr('data-id');
        Remove_Connection(id);
    });
    $(document).on('click', '.camera_view', function () {
        var td = $(this);
        var tr = $(td).closest('tr');
        let id = tr.attr('data-id');
        let model = tr.attr('data-model');
        let description = tr.attr('data-description');
        View_Web_Camera(id, model, description);
    });


});
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
    var height = (this.window.innerHeight > 0 ? this.window.innerHeight : this.screen.height) - 1;
    $('div[id=map]').css("min-height", height + "px");
}
function SortByVehicleName(a, b) {
    return a.VEHICLENAME < b.VEHICLENAME ? -1 : a.VEHICLENAME > b.VEHICLENAME ? 1 : 0;
}
function formatTime(value_time) {
    try {
        if (!$.isEmptyObject(timezone)) {
            if (timezone.hasOwnProperty("Facility_TimeZone")) {
                var time = moment(value_time);
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

        var time = moment(value_time);
        if (time._isValid) {
            return time.format("M/D/YYYY h:mm a");
        }

    } catch (e) {
        console.log(e);
    }
}
function capitalize_Words(str) {
    return str.replace(/\w\S*/g, function (txt) {
        return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
    });
}
function IPAddress_validator(value) {
    var ipPattern = /^(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})$/;
    var ipArray = value.match(ipPattern);
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
    return a.name < b.name ? -1 : a.name > b.name ? 1 : 0;
}

function GetPeopleInZone(zone, P2Pdata, staffarray) {
    //staffing
    var planstaffarray = [];
    var planstaffCounts = [];
    var plansumstaffCounts = {};
    var staffCounts = [];
    var sumstaffCounts = {};
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
                                                    staffarray.push({
                                                        name: checkValue(layer.feature.properties.craftName) ? layer.feature.properties.craftName : layer.feature.properties.id,
                                                        nameId: checkValue(layer.feature.properties.id) ? layer.feature.properties.id : layer.feature.properties.id,
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
                    for (var i = 0; i < P2Pdata.clerk; i++) {
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
                    for (var r = 0; r < P2Pdata.mh; r++) {
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
        var staffing = $.extend(true, staffCounts, planstaffCounts);
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
let stafftop_row_template = '<tr data-id={staffName}>' +
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
    });
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
        var time = moment().set({ 'year': t.year, 'month': t.month + 1, 'date': t.dayOfMonth, 'hour': t.hourOfDay, 'minute': t.minute, 'second': t.second });
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
async function updateTime(t) {
    $('#localTime').val(moment(t).format('H:mm'));
}