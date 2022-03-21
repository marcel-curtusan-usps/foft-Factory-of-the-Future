﻿var map = null;
var bounds = [];
var container = new L.FeatureGroup();
//side bar setup
var sidebar = L.control.sidebar({
    container: 'sidebar', position: 'left', autopan: false
});
//setup map
map = L.map('map', {
    crs: L.CRS.Simple,
    renderer: L.canvas({ padding: 0.5 }),
    preferCanvas: true,
    markerZoomAnimation: false,
    minZoom: 1,
    maxZoom: 5,
    zoomControl: false,
    measureControl: true,
    tap: false,
    layers: [polygonMachine, vehicles, agvLocations, container, stagingAreas, tagsMarkersGroup, dockDoors]
});
var overlayMaps = {
    "Vehicles Tag": vehicles,
    "SELS Tag": tagsMarkersGroup,
    "AGV Locations": agvLocations,
    "MPE Work Areas": polygonMachine,
    "Dock Doors": dockDoors,
    "Staging Areas": stagingAreas,
    "View-ports": viewPortsAreas,
    "EBR Areas": ebrAreas,
    "Exit Areas": exitAreas,
    "Work Area": walkwayAreas,
    "Polygon Holes": polyholesAreas,
    "Locator's": locatorMarker
};
var timedisplay = L.Control.extend({
    options: {
        position: 'topright'
    },
    onAdd: function () {
        var Domcntainer = L.DomUtil.create('input');
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
            break;
        case 'reports':
            GetUserInfo();
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
L.control.layers(null, overlayMaps, { position: 'bottomright', collapsed: false }).addTo(map);
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
$('div .layerPopover').attr('id', 'layersContent');
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

/****add map items start */
async function init_Map() {
    
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
                icon: '<i class="pi-iconFullScreenExit" title="Exit Full Screen"></i>',
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
    $.connection.FOTFManager.server.getMap().done(function (MapData) {
        try {
            if (MapData.length === 1) {
                MapData = MapData[0];
                if (!$.isEmptyObject(MapData)) {
                    if (MapData.hasOwnProperty("Facility_TimeZone")) {
                        if (checkValue(MapData.Facility_TimeZone)) {
                            timezone = { Facility_TimeZone: MapData.Facility_TimeZone }
                        }
                    }
                    if (MapData.hasOwnProperty("Software_Version")) {
                        if (checkValue(MapData.Software_Version)) {
                            map.attributionControl.setPrefix("USPS Factory of the Future (" + MapData.Software_Version + ")");
                        }
                    }
                    if (MapData.hasOwnProperty("Facility_Name")) {
                        if (checkValue(MapData.Facility_Name)) {
                            $('#fotf-site-facility-name').append(MapData.Facility_Name);
                            map.attributionControl.addAttribution(MapData.Facility_Name);
                            $(document).prop('title', MapData.Facility_Name + ' FOTF');
                        }
                    }

                    if (MapData.hasOwnProperty("Environment")) {
                        //Environment Status Controls
                        if (/(DEV|SIT|CAT)/i.test(MapData.Environment)) {
                            var Environment = L.Control.extend({
                                options: {
                                    position: 'topright'
                                },
                                onAdd: function () {
                                    var Domcntainer = L.DomUtil.create('input');
                                    Domcntainer.type = "button";
                                    Domcntainer.id = "environment";
                                    Domcntainer.className = getEnv(MapData.Environment);
                                    Domcntainer.value = MapData.Environment;
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
                    //set new image
                    var img = new Image();
                    //load Base64 image
                    img.src = MapData.base64;
                    //create he bound of the image.
                    bounds = [[MapData.yMeter, MapData.xMeter], [MapData.heightMeter + MapData.yMeter, MapData.widthMeter + MapData.xMeter]];
                    var trackingarea = L.polygon(bounds, {});
                    //add the image to the map
                    L.imageOverlay(img.src, trackingarea.getBounds()).addTo(map);

                    //center image
                    map.setView(trackingarea.getBounds().getCenter(), 1.5);

                }
            }
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
                    ).appendTo('.body-content');
            }
        } catch (e) {
            $('div[id=map]').css('display', 'none');
            $('<div/>', { class: 'jumbotron text-center' })
                .append($('<h1/>', { class: 'display-4', text: "Error loading Map" }))
                .append($('<p/>', { class: 'lead', text: 'Erro:' + e }))
                .append($('<hr/>', { class: 'my-4' }))
                .appendTo('.body-content');
        }
    })
        .then(init_viewports())
        .then(init_machine())
        .then(init_dockdoor())
        .then(init_agvlocation())
        .then(init_zones())
        .then(function () { init_arrive_depart_trips(); })
        .then(init_locators())
        .then(LoadNotification("routetrip"))
        .then(LoadNotification("vehicle"))
        .catch(
            function (err) {
                console.log(err.toString());
            });
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
        $.connection.FOTFManager.server.getPersonTagsList().done(function (Data) {
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
        $.connection.FOTFManager.server.getUndetectedTagsList().done(function (undetectedtagsData) {
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
                            if (properties.properties.tacs.fnAlert === "false") {
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
        $.connection.FOTFManager.server.getLDCAlertTagsList().done(function (ldcalertstagsData) {
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
    $.connection.FOTFManager.server.getADUserProfile().done(function (User) {
        $('#userfullname').text(User.FirstName + ' ' + User.SurName);
        $('#useremail').text(User.EmailAddress);
        $('#userphone').text(User.Phone);
        $('#usertitel').text(User.Role);

    });
}