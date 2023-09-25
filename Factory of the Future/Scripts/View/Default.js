/// A simple template method for replacing placeholders enclosed in curly braces.
if (!String.prototype.supplant) {
    String.prototype.supplant = function (o) {
        return this.replace(/{([^{}]*)}/g,
            function (a, b) {
                let r = o[b];
                return typeof r === 'string' || typeof r === 'number' ? r : a;
            }
        );
    };
}
let tagsLoaded = false;
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
let emplschedule = [];
let tagsscheduled = 0;
let nIntervId;
$(function () {
 
    $(".bi").on("click", function () {
        $(this).toggleClass("bi-arrows-expand");
        $(this).toggleClass("bi-arrows-collapse");
    });
    $("form").submit(function () { return false; });
    
    $(window).resize(function () {
        setHeight();
    });
    User = $.parseJSON(localStorage.getItem('User'));
    if (!$.isEmptyObject(User)) {
        $(document).prop('title', User.Facility_Name + ' ' + User.ApplicationAbbr);
        $('#fotf-site-facility-name').empty();
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
                    '<div id="div_connectionNotification" class="container-fluid">' +
                    '<h4 class="ml-p5rem">Connections</h4>' +
                    '<button type="button" class="btn btn-primary float-left mb-2 ml-p5rem" name="addconnection">Add</button>' +
                    '<div class="card w-100 bg-white mt-2 pb-1">' +
                    '<div class="card-body">' +
                    '<div class="table-responsive">' +
                    '<table class="table table-sm table-hover table-condensed mb-1" id="connectiontable" style="border-collapse:collapse;">' +
                    '<thead class="thead-dark">' +
                    '<tr>' +
                    '<th class="row-connection-name"><span class="ml-p5rem">Name</span></th>' +
                    '<th class="row-connection-type">Message Type</th>' +
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
            Promise.all([init_connection($.parseJSON(User.ConnectionList))]);
        }
        if (User.hasOwnProperty("FacilityTimeZone") && checkValue(User.FacilityTimeZone)) {
            /* if (checkValue(User.FacilityTimeZone)) {*/
            timezone = { Facility_TimeZone: User.FacilityTimeZone };
            //if (!nIntervId) {
            //    nIntervId = setInterval(cBlock, 1000);
            //}
            setInterval(async () => {
                Promise.all([cBlock()]);
            }, 1000);
               /* Promise.all([cBlock()]);*/
            //}
        }
        if (User.hasOwnProperty("Environment") && /(DEV|SIT|CAT)/i.test(User.Environment)) {
            //Environment Status Controls
           /* if (/(DEV|SIT|CAT)/i.test(User.Environment)) {*/
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
         
            //}
        }
        ConnectionNameLoad($.parseJSON(User.AppSetting));
        if (/^Admin/i.test(User.Role)) {
         
           
            $("div[id=swaggercard").css('display', 'block');
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
                                        '<table class="tableSettings table-sm table-hover table-condensed" id="app_settingtable" style="border-collapse:collapse;">' +
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
            init_AppSetting($.parseJSON(User.AppSetting));
            init_Foolplan();
            $('button[name=addfloorpan]').off().on('click', function () {
                /* close the sidebar */
                sidebar.close();
                Add_Floorplan();
            });

        }
        if (/(^PMCCUser$)/i.test(User.UserId) || /^Admin/i.test(User.Role) ) {
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

            //let qrcode = new QRCode("qrcodeUrl", {
            //    text: window.location.href,
            //    width: 128,
            //    height: 128,
            //    colorDark: "#000000",
            //    colorLight: "#ffffff",
            //    correctLevel: QRCode.CorrectLevel.H
            //});
        }
    }
 
    // Search Tag Name
    $('input[id=inputsearchbtn').keyup(function () {
        startSearch(this.value)
    }).keydown(function (event) {
        if (event.which === 13) {
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
            init_mapSetup(MapData);
        },
        floorZones: async (Zonedata) => {
            $.each(Zonedata, function (_index, data) {
                if (data) {
                    Promise.all([init_zones(data, baselayerid)]);
                }
            });
        },
        floorLocators: async (Locatorsdata) => {
            //$.each(Locatorsdata, function (_index, data) {
            //    if (data) {
            Promise.all([init_locators(Locatorsdata, baselayerid)]);
            //    }
            //});
        },
        floorCameraMarkers: async (Cameradata) => {
            //$.each(Locatorsdata, function (_index, data) {
            //    if (data) {
            Promise.all([init_Cameradatalocators(Cameradata, baselayerid)]);
            //    }
            //});
        },
        floorVehiclesMarkers: async (Vehiclesdata) => {
            //$.each(Vehiclesdata, function (_index, data) {
            //    if (data) {
            Promise.all([init_VehiclesMarkers(Vehiclesdata, baselayerid)]);
            //    }
            //});
        },
        floorPeopleMarkers: async (Peopledata) => {
            //$.each(Peopledata, function (_index, data) {
            //    if (data) {
            Promise.all([init_Peoplelocators(Peopledata, baselayerid)]);
            //    }
            //});
        }
    });
  
    // Start the connection
    $.connection.hub.qs = { 'page_type': "CF".toUpperCase() };
    $.connection.hub.start({ waitForPageLoad: false })
        .done(function () {
         /*   getMapInitMap();*/
            createStaffingDataTable("staffingtable");
            fotfmanager.server.joinGroup("QSM");
            fotfmanager.server.getStaffSchedule().done(async (data) => {
               
                emplschedule = data;
                $.map(data, function (empl, i) {
                    if (/person/i.test(empl.tagType) && empl.isSch) {
                        tagsscheduled++;
                    }
                });
                $('div[id=schstaffingbutton]').text(tagsscheduled);
            });
            if (/^(Admin|OIE)/i.test(User.Role)) {
                init_geometry_editing();
            }
            if (!/^(Admin|OIE)/i.test(User.Role)) {
                fotfmanager.server.leaveGroup("PeopleMarkers");
            }
            Promise.all([initDoorDataTable()]);
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
        content: $('#legendContent')
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
        content: $('#faqContent')
    });
    $('#faqToggle').on('click', function () {
        // close the sidebar
        sidebar.close();
    });
    // SelectizeJs Init for searching select boxes.
    let options = {
        create: false,
        sortField: "text"
    }
    $('.selectize-dropdown').click(function (e) {
        e.stopPropagation();        // To fix zone select scroll bug. May need to be revisited
    })
    $zoneSelect = $("#zoneselect").selectize(options);
    /**User  Modal***/
    //function EditUserInfo(layerindex) {
    //    $('#modaluserHeader_ID').text('Edit User Info');

    //    if ($.isNumeric(layerindex)) {
    //        layerindex = parseInt(layerindex);
    //    }
    //    sidebar.close();
    //    try {
    //        if (map._layers[layerindex].hasOwnProperty("feature")) {
    //            if (map._layers[layerindex].feature.hasOwnProperty("properties")) {
    //                $('input[type=text][name=empId]').val(map._layers[layerindex].feature.properties.Employee_EIN);
    //                $('input[type=text][name=empName]').val(map._layers[layerindex].feature.properties.Employee_Name);
    //                $('input[type=text][name=tag_name]').val(map._layers[layerindex].feature.properties.name);
    //                $('input[type=text][name=tag_id]').val(map._layers[layerindex].feature.properties.id);
    //                $('button[id=usertagsubmitBtn]').off().on('click', function () {
    //                    try {
    //                        $('button[id=usertagsubmitBtn]').prop('disabled', true);
    //                        let jsonObject = {};
    //                        $('input[type=text][name=empId]').val() !== map._layers[layerindex].feature.properties.Employee_EIN ? jsonObject.Employee_EIN = $('input[type=text][name=employee_ein]').val() : "";
    //                        $('input[type=text][name=empName]').val() !== map._layers[layerindex].feature.properties.Employee_Name ? jsonObject.Employee_Name = $('input[type=text][name=employee_name]').val() : "";
    //                        if (!$.isEmptyObject(jsonObject)) {
    //                            jsonObject.id = map._layers[layerindex].feature.properties.id;
    //                            $.connection.FOTFManager.server.editTagInfo(JSON.stringify(jsonObject)).done(function (Data) {
    //                                if (Data.hasOwnProperty(ERROR_MESSAGE)) {
    //                                    $('span[id=error_usertagsubmitBtn]').text(Data.ERROR_MESSAGE);
    //                                }
    //                                if (Data[0].hasOwnProperty("MESSAGE_TYPE")) {
    //                                    $('span[id=error_usertagsubmitBtn]').text(Data.MESSAGE_TYPE);
    //                                    setTimeout(function () { $("#UserTag_Modal").modal('hide'); }, 3000);
    //                                }
    //                            });
    //                        }
    //                        else {
    //                            $('span[id=error_usertagsubmitBtn]').text("No Tag Data has been Updated");
    //                            setTimeout(function () { $("#UserTag_Modal").modal('hide'); }, 3000);
    //                        }
    //                    } catch (e) {
    //                        $('span[id=error_usertagsubmitBtn]').text(e);
    //                    }
    //                });

    //                $('#UserTag_Modal').modal();
    //            }
    //        }
    //    } catch (e) {
    //        console.log(e);
    //    }
    //}
    function EditUserInfo(layerIndex) {
        // Set the header text of the modal
        $('#modaluserHeader_ID').text('Edit User Info');

        // Convert the layer index to an integer if it's numeric
        if ($.isNumeric(layerIndex)) {
            layerIndex = parseInt(layerIndex, 10);
        }

        // Close the sidebar
        sidebar.close();

        try {
            // Check if the layer has a "feature" property
            const layer = map._layers[layerIndex];
            const feature = layer?.feature;
            if (!feature) {
                return; // Exit if the layer does not have a feature
            }

            // Check if the feature has a "properties" property
            const properties = feature.properties;
            if (!properties) {
                return; // Exit if the feature does not have properties
            }

            // Populate the input fields with the feature properties
            $('input[type=text][name=empId]').val(properties.Employee_EIN);
            $('input[type=text][name=empName]').val(properties.Employee_Name);
            $('input[type=text][name=tag_name]').val(properties.name);
            $('input[type=text][name=tag_id]').val(properties.id);

            // Set up the click event for the submit button
            $('button[id=usertagsubmitBtn]').off().on('click', function () {
                try {
                    // Disable the submit button to prevent multiple submissions
                    $(this).prop('disabled', true);

                    // Create an object to store the updated properties
                    const updatedProperties = {};

                    // Check if the "Employee_EIN" property has changed
                    if ($('input[type=text][name=empId]').val() !== properties.Employee_EIN) {
                        updatedProperties.Employee_EIN = $('input[type=text][name=employee_ein]').val();
                    }

                    // Check if the "Employee_Name" property has changed
                    if ($('input[type=text][name=empName]').val() !== properties.Employee_Name) {
                        updatedProperties.Employee_Name = $('input[type=text][name=employee_name]').val();
                    }

                    if (!$.isEmptyObject(updatedProperties)) {
                        // Add the "id" property to the object
                        updatedProperties.id = properties.id;

                        // Send the updated properties to the server
                        $.connection.FOTFManager.server.editTagInfo(JSON.stringify(updatedProperties)).done(function (data) {
                            if (data.hasOwnProperty(ERROR_MESSAGE)) {
                                $('span[id=error_usertagsubmitBtn]').text(data.ERROR_MESSAGE);
                            }
                            if (data[0].hasOwnProperty("MESSAGE_TYPE")) {
                                $('span[id=error_usertagsubmitBtn]').text(data.MESSAGE_TYPE);
                                setTimeout(function () { $("#UserTag_Modal").modal('hide'); }, 3000);
                            }
                        });
                    } else {
                        $('span[id=error_usertagsubmitBtn]').text("No Tag Data has been Updated");
                        setTimeout(function () { $("#UserTag_Modal").modal('hide'); }, 3000);
                    }
                } catch (error) {
                    $('span[id=error_usertagsubmitBtn]').text(error);
                }
            });

            // Show the modal
            $('#UserTag_Modal').modal();
        } catch (error) {
            //console.log(error);
            throw new Error(error.toString());
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
        if (connecttimer) {
            clearTimeout(connecttimer);
        }
        if (connectattp <= 10) {
            connectattp++;
            console.log("Connection lost. Attempting to reconnect... (attempt " + connectattp + ")");
            connecttimer = setTimeout(function () {
                $.connection.hub.start().done(function () {
                    console.log("Connected at " + new Date($.now()));
                }).fail(function (err) {
                    console.log("Could not reconnect: " + err);
                });
            }, 10000); // Restart connection after 10 seconds.
        } else {
            console.log("Connection lost permanently. Please refresh the page to try again.");
        }
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
                //$.map(map._layers, function (layer, i) {
                $.map(map._layers, function (layer) {
                    if (layer.hasOwnProperty("feature") && layer.feature.properties.id === tagid) {
                        //if (layer.feature.properties.id === tagid) {
                            map.setView(layer.getLatLng(), 3);
                            sidebar.open('home');
                            LoadVehicleTable(layer.feature.properties);
                            return false;
                        //}
                    }
                });
            }
        } catch (e) {
            //console.log(e);
            throw new Error(e.toString());
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
    //$(document).on('click', '.tagdetails', function (e) {
    $(document).on('click', '.tagdetails', function () {
        var td = $(this);
        var tagid = td.attr('data-tag');
        if (checkValue(tagid)) {
            LoadtagDetails(tagid);
        }
    });
    //search machine details
    //$(document).on('click', '.machinedetails', function (e) {
    $(document).on('click', '.machinedetails', function () {
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
                //$.map(viewPortsAreas._layers, function (layer, i) {
                $.map(viewPortsAreas._layers, function (layer) {
                    if (layer.hasOwnProperty("feature") && layer.feature.properties.id === selcValue) {
                        //if (layer.feature.properties.id === selcValue) {
                            var Center = new L.latLng(
                                (layer._bounds._southWest.lat + layer._bounds._northEast.lat) / 2,
                                (layer._bounds._southWest.lng + layer._bounds._northEast.lng) / 2);
                            map.setView(Center, 3);
                            Loadtable(layer.feature.properties.id, 'viewportstable');
                            return false;
                        //}
                    }
                });
            }
        } catch (e) {
            //console.log(e);
            throw new Error(e.toString());
        }
    });
    //$(document).on('click', '.connectionedit', function () {
    //    var td = $(this);
    //    var tr = $(td).closest('tr');
    //    var id = tr.attr('data-id');
    //    Edit_Connection(id);
    //});
    //$(document).on('click', '.connectiondelete', function () {
    //    var td = $(this);
    //    var tr = $(td).closest('tr'),
    //        id = tr.attr('data-id');
    //    Remove_Connection(id);
    //});
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

function getEnv(env) {
    if (/CAT/i.test(env)) {
        return "btn btn-outline-primary btn-sm";
    }
    //else if (/SIT/i.test(env)) {
    if (/SIT/i.test(env)) {
        return "btn btn-outline-warning btn-sm";
    }
    //else if (/DEV/i.test(env)) {
    if (/DEV/i.test(env)) {
        return "btn btn-outline-danger btn-sm";
    }
}
function Page_Update(data) {
    $('#fotf-site-facility-name').empty();
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
        let keyA;
        let keyB;
        if (isInput) {
            keyA = $(rowA).children('td').eq(column).children('input').val().toUpperCase();
            keyB = $(rowB).children('td').eq(column).children('input').val().toUpperCase();
        } else {
            keyA = $(rowA).children('td').eq(column).text().toUpperCase();
            keyB = $(rowB).children('td').eq(column).text().toUpperCase();
        }
        if (isSelected) { return OrderBy(keyA, keyB, isNum); }
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
    return a.VEHICLENAME < b.VEHICLENAME ? -1 : a.VEHICLENAME > b.VEHICLENAME ? 1 : 0;
}
function formatTime(value_time) {
    try {
        if (!$.isEmptyObject(timezone) && timezone.hasOwnProperty("Facility_TimeZone")) {
            //if (timezone.hasOwnProperty("Facility_TimeZone")) {
                let time = moment(value_time);
                if (time._isValid) {
                    return time.format("HH:mm");
                }
            //}
        }
    } catch (e) {
        //console.log(e);
        throw new Error(e.toString());
    }
}
function formatDateTime(value_time) {
    try {

        let time = moment(value_time);
        if (time._isValid) {
            return time.format("M/D/YYYY h:mm a");
        }

    } catch (e) {
        //console.log(e);
        throw new Error(e.toString());
    }
}
function capitalize_Words(str) {
    try {
        return str.replace(/\w\S*/g, function (txt) {
            return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
        });
    } catch (e) {
        //console.log(e);
        throw new Error(e.toString());
    }
  
}
function IPAddress_validator(value) {
    let ipPattern = /^(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})$/;
    let ipArray = value.match(ipPattern);
    if (value === "0.0.0.0" || value === "255.255.255.255" || ipArray === null) {
        return "Invalid IP Address";
    } else {
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
    if (n) { return a - b; }
    if (a < b) { return -1; }
    if (a > b) { return 1; }
    return 0;
}
function SortByTagName(a, b) {
    return a.name < b.name ? -1 : a.name > b.name ? 1 : 0;
}
function SortByName(a, b) {
    let aName = a.toLowerCase();
    let bName = b.toLowerCase();
    return ((aName < bName) ? -1 : ((aName > bName) ? 1 : 0));
}
function SortBySiteName(a, b) {
    let aName = a.legSiteName.toLowerCase();
    let bName = b.legSiteName.toLowerCase();
    return ((aName < bName) ? -1 : ((aName > bName) ? 1 : 0));
}
function SortByLocationName(a, b) {
    let aName = a.locationName.toLowerCase();
    let bName = b.locationName.toLowerCase();
    return ((aName < bName) ? -1 : ((aName > bName) ? 1 : 0));
}
function SortByNumber(a, b) {
    return a - b;
}
function SortByValue(a, b) {
    a = a.value;
    b = b.value;
    return a - b;
}
async function GetPeopleInZone(zone, P2Pdata, staffarray) {
    //staffing
    let planstaffarray = [];
    let planstaffCounts = [];
    let plansumstaffCounts = {};
    let staffCounts = [];
    let sumstaffCounts = {};
    try {
        if (staffarray.length === 0 && tagsMarkersGroup.hasOwnProperty("_layers")) {
            //if (tagsMarkersGroup.hasOwnProperty("_layers")) {
            //$.map(tagsMarkersGroup._layers, function (layer, i) {
            $.map(tagsMarkersGroup._layers, function (layer) {
                    if (layer.hasOwnProperty("feature") && /person/i.test(layer.feature.properties.Tag_Type) && layer.feature.properties.hasOwnProperty("zones")) {
                        //if (/person/i.test(layer.feature.properties.Tag_Type)) {
                            //if (layer.feature.properties.hasOwnProperty("zones")) {
                                $.map(layer.feature.properties.zones, function (p_zone) {
                                    if (p_zone === zone && layer.hasOwnProperty("_tooltip") && layer._tooltip.hasOwnProperty("_container") && !layer._tooltip._container.classList.contains('tooltip-hidden')) {
                                        //if (layer.hasOwnProperty("_tooltip")) {
                                            //if (layer._tooltip.hasOwnProperty("_container")) {
                                                //if (!layer._tooltip._container.classList.contains('tooltip-hidden')) {
                                                    staffarray.push({
                                                        name: checkValue(layer.feature.properties.craftName) ? layer.feature.properties.craftName : layer.feature.properties.id,
                                                        nameId: checkValue(layer.feature.properties.id) ? layer.feature.properties.id : layer.feature.properties.id,
                                                        id: layer.feature.properties.id
                                                    })
                                                //}
                                            //}
                                        //}
                                    }
                                });
                            //}
                        //}
                    }
                });
            //}
        }

        if (checkValue(P2Pdata) && P2Pdata.hasOwnProperty("clerk") && P2Pdata.clerk >= 1) {
            //if (P2Pdata.hasOwnProperty("clerk")) {
                //if (P2Pdata.clerk >= 1) {
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
                //}
            //}
            if (P2Pdata.hasOwnProperty("mh") && P2Pdata.mh >= 1) {
                //if (P2Pdata.mh >= 1) {
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
                //}
            }
        }
       
        //$.each(staffarray, function (i, e) {
        $.each(staffarray, function () {
            sumstaffCounts[this.name] = (sumstaffCounts[this.name] || 0) + 1;
        });
        
        $.each(sumstaffCounts, function (index, value) {
            staffCounts.push({
                name: index,
                currentstaff: value
            });
        });

        //$.each(planstaffarray, function (i, e) {
        $.each(planstaffarray, function () {
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
        //console.log(e);
        throw new Error(e.toString());
    }
}
let stafftop_row_template = '<tr data-id={staffName}{style}>' +
    '<td class="text-center">{staffName}</td>' +
    '<td class="text-center">{currentstaff}</td>' +
    '<td class="text-center">{planstaff}</td>' +
    '</tr>"';
function formatstafftoprow(properties) {
    return $.extend(properties, {
        staffName: properties.name,
        staffId: properties.id,
        currentstaff: properties.hasOwnProperty("currentstaff") ? properties.currentstaff : 0,
        planstaff: properties.hasOwnProperty("planstaff") ? properties.planstaff : 0,
        style: GetStaffRowStyle(properties.hasOwnProperty("currentstaff") ? properties.currentstaff : 0, properties.hasOwnProperty("planstaff") ? properties.planstaff : 0)
    });
}
function GetStaffRowStyle(currStaff, planStaff) {
    let rowAlertColor = ' style="background-color:rgba(220, 53, 69, 0.5);"';
    let rowWarningColor = ' style="background-color:rgba(255, 193, 7, 0.5);"';
    if (planStaff > 4 && currStaff < planStaff) {
        //if (currStaff < planStaff) {
            if ((currStaff / planStaff) * 100 > 95) {
                return rowWarningColor
            }
            else {
                return rowAlertColor
            }
        //}
    }
    return "";
}
function digits(num) {
    if (!!num) {
        return num.replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
    }
    
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
        //console.log(e);
        throw new Error(e.toString());
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
                if (time.year() <= 1) {
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
        //console.log(e);
        throw new Error(e.toString());
       //return "";
    }
}
async function cBlock() {
    let t = moment($.now());
    $('#localTime').val(t.format('H:mm:ss'));
    $('#twentyfourmessage').text(GetTwentyFourMessage(t));
    if ($("#tfhcContent").length > 0) {
        SetClockHands(t);
    }
    Promise.all([zonecurrentStaff()]);
    if (tagsLoaded) {
        Promise.all([staffCounts()]);
    }

}

// current zone staff
//TODO: look in to this more.
async function zonecurrentStaff() {
    try {
 
        ////clear Old tags 
        //if (tagsMarkersGroup.hasOwnProperty("_layers")) {
        //    $.map(tagsMarkersGroup._layers, (layer) => {

        //        if (/person/i.test(layer.feature.properties.Tag_Type) && layer.feature.properties.tagVisible === true && layer.feature.properties.hasOwnProperty("positionTS")) {
        //            //if (layer.feature.properties.tagVisible === true) {
        //                //if (layer.feature.properties.hasOwnProperty("positionTS")) {
        //                    let startTime = moment(layer.feature.properties.positionTS);
        //                    let endTime = moment();
        //                    let diffmillsec = endTime.diff(startTime, "milliseconds");

        //                    if (diffmillsec > layer.feature.properties.tagVisibleMils) {
        //                        layer.feature.properties.tagVisibleMils = diffmillsec;
        //                    }
        //                //}
        //                if (layer.feature.properties.tagVisibleMils > 80000) {
        //                    layer.feature.properties.tagVisible = false;
        //                    //this to hide tooltip
        //                    if (layer.hasOwnProperty("_tooltip") && layer._tooltip.hasOwnProperty("_container") && !layer._tooltip._container.classList.contains('tooltip-hidden')) {
        //                        //if (layer._tooltip.hasOwnProperty("_container")) {
        //                            //if (!layer._tooltip._container.classList.contains('tooltip-hidden')) {
        //                                layer._tooltip._container.classList.add('tooltip-hidden');
        //                            //}
        //                        //}
        //                    }
        //                }
        //            //}
        //        }

        //    })
        //}
        // Machine zone
        if (polygonMachine.hasOwnProperty("_layers")) {
            //$.map(polygonMachine._layers, function (Machinelayer, i) {
            $.map(polygonMachine._layers, function (Machinelayer) {
                let MachineCurrentStaff = [];
                if (tagsMarkersGroup.hasOwnProperty("_layers")) {
                    //$.map(tagsMarkersGroup._layers, function (layer, i) {
                    $.map(tagsMarkersGroup._layers, function (layer) {
                        if (/person/i.test(layer.feature.properties.Tag_Type) && layer.feature.properties.zones !== null && layer.feature.properties.zones.length > 0) {
                            
                            $.map(layer.feature.properties.zones, function (p_zone) {

                                if (p_zone === Machinelayer.feature.properties.id) {
                                    
                                               MachineCurrentStaff.push({
                                                    name: checkValue(layer.feature.properties.craftName) ? layer.feature.properties.craftName : layer.feature.properties.id,
                                                    nameId:layer.feature.properties.id,
                                                    id: layer.feature.properties.id
                                                })
                               
                                }
                            });
                        }
                    });
                }

                if (Machinelayer.hasOwnProperty("_tooltip") && MachineCurrentStaff.length !== Machinelayer.feature.properties.CurrentStaff) {
                    //if (MachineCurrentStaff.length !== Machinelayer.feature.properties.CurrentStaff) {
                        Machinelayer.setTooltipContent(Machinelayer.feature.properties.name + "<br/>" + "Staffing: " + MachineCurrentStaff.length);
                        Machinelayer.feature.properties.CurrentStaff = MachineCurrentStaff.length;
                        if ($('select[id=zoneselect] option:selected').val() === Machinelayer.feature.properties.id) {
                            let p2pdata = Machinelayer.feature.properties.hasOwnProperty("P2PData") ? Machinelayer.feature.properties.P2PData : "";
                            GetPeopleInZone(Machinelayer.feature.properties.id, p2pdata, MachineCurrentStaff);
                        }
                    //}
                }
            });
        }
        //// other zone
        //if (stagingAreas.hasOwnProperty("_layers")) {
        //    //$.map(stagingAreas._layers, function (stagelayer, i)
        //    $.map(stagingAreas._layers, function (stagelayer)
        //    {
        //        let CurrentStaff = [];
        //        if (tagsMarkersGroup.hasOwnProperty("_layers")) {
        //            //$.map(tagsMarkersGroup._layers, function (layer, i) {
        //            $.map(tagsMarkersGroup._layers, function (layer) {
                       
        //                if (/person/i.test(layer.feature.properties.Tag_Type) && layer.feature.properties.zones != null) {
                           
        //                    $.map(layer.feature.properties.zones, function (p_zone) {
        //                        if (p_zone.id === stagelayer.feature.properties.id) {
                                    
        //                                      CurrentStaff.push({
        //                                            name: checkValue(layer.feature.properties.craftName) ? layer.feature.properties.craftName : layer.feature.properties.id,
        //                                            nameId: checkValue(layer.feature.properties.id) ? layer.feature.properties.id : layer.feature.properties.id,
        //                                            id: layer.feature.properties.id
        //                                        })
                                       
        //                        }
        //                    });
        //                }

        //            });
        //        }

        //        if (stagelayer.hasOwnProperty("_tooltip") && CurrentStaff.length !== stagelayer.feature.properties.CurrentStaff) {
        //            //if (CurrentStaff.length !== stagelayer.feature.properties.CurrentStaff) {
        //                stagelayer.setTooltipContent(stagelayer.feature.properties.name + "<br/>" + "Staffing: " + CurrentStaff.length);
        //                stagelayer.feature.properties.CurrentStaff = CurrentStaff.length;
        //            if (stagingAreas.hasOwnProperty("feature") && $('select[id=zoneselect] option:selected').val() === stagingAreas.feature.properties.id) {
        //                    //if ($('select[id=zoneselect] option:selected').val() === stagingAreas.feature.properties.id) {
        //                        GetPeopleInZone(stagelayer.feature.properties.idp2pdata, CurrentStaff);
        //                    //}
        //                }
        //            //}
        //        }
        //    });
        //}
        return true;
    } catch (e) {
        throw new Error(e.toString());
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
    if (selcValue === null) { return; }
    if (viewPortsAreas.hasOwnProperty("_layers")) {
        //$.map(viewPortsAreas._layers, function (layer, i) {
        $.map(viewPortsAreas._layers, function (layer) {
            if (layer.hasOwnProperty("feature") && layer.feature.properties.id === selcValue) {
                //if (layer.feature.properties.id === selcValue) {
                    let Center = new L.latLng(
                        (layer._bounds._southWest.lat + layer._bounds._northEast.lat) / 2,
                        (layer._bounds._southWest.lng + layer._bounds._northEast.lng) / 2);
                    map.setView(Center, 3);
                    Loadtable(layer.feature.properties.id, 'viewportstable');
                    return false;
                //}
            }
        });
    }
}
async function loadDatatable(data, table) {
    if ($.fn.dataTable.isDataTable("#" + table) && !$.isEmptyObject(data)) {
        //if (!$.isEmptyObject(data)) {
            $('#' + table).DataTable().rows.add(data).draw();
        //}
    }
}
function URLconstructor(winLoc) {
    if (/^(.CF)/i.test(winLoc.pathname)) {
        return winLoc.origin + "/CF/";
    }
    else {
        return winLoc.origin + "/";
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