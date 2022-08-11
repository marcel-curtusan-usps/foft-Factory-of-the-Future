//side bar setup
var sidebar = L.control.sidebar({
    container: 'sidebar', position: 'left', autopan: false
});
var mainfloorOverlays = L.layerGroup();
var mainfloor = L.imageOverlay(null, [0, 0], { id:-1 ,zindex: -1 }).addTo(mainfloorOverlays);
var baseLayers = {
  "Main Floor": mainfloor
};


var overlayMaps = {
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
    var results = new RegExp('[\?&]' + name + '=([^&#]*)')
        .exec(window.location.search);

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
        piv_vehicles, agv_vehicles, agvLocations, container, stagingAreas, stagingBullpenAreas, tagsMarkersGroup, dockDoors, binzonepoly
    ];

}

//setup map
map = L.map('map', {
    crs: L.CRS.Simple,
    renderer: L.canvas({ padding: 0.5 }),
    preferCanvas: true,
    pmIgnore: false,
    markerZoomAnimation: false,
    minZoom: 1,
    maxZoom: 7,
    zoomControl: false,
    measureControl: true,
    tap: false,
    layers: layersSelected
});

function setLayerCheckboxId(thisCheckBox, innerHTML) {
    let name = innerHTML.replace(/ /g, '');
    thisCheckBox.id = name;
}
function assignIdsToLayerCheckboxes() {
    var leafletSelectors
        = document.getElementsByClassName("leaflet-control-layers-selector");
    for (var selector of leafletSelectors) {
        let sp = selector.nextElementSibling;
        let keys = Object.keys(overlayMaps);
        for (const key of keys) {
            if (sp.innerHTML.trim() == key.trim()) {
                setLayerCheckboxId(selector, sp.innerHTML);
            }
        }
    }
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
        init_zones(data[0].zones, baselayerid);
        init_locators(data[0].locators, baselayerid);
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
});
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
    }, position: 'bottomright', collapsed: false }).addTo(map);
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
function init_mapSetup(MapData) {
    try {
        if (MapData.length > 0) {
            map.attributionControl.setPrefix("USPS " + MapData[0].backgroundImages.applicationFullName + " (" + MapData[0].backgroundImages.softwareVersion + ")");
            $('#fotf-site-facility-name').append(MapData[0].backgroundImages.facilityName);
            map.attributionControl.addAttribution(MapData[0].backgroundImages.facilityName);
            $(document).prop('title', MapData[0].backgroundImages.facilityName + ' ' + MapData[0].backgroundImages.applicationAbbr);
            $.each(MapData, function (index) {
                //set new image
                var img = new Image();
                //load Base64 image
                img.src = this.backgroundImages.base64;
                //create he bound of the image.
                bounds = [[this.backgroundImages.yMeter, this.backgroundImages.yMeter], [this.backgroundImages.heightMeter + this.backgroundImages.yMeter, this.backgroundImages.widthMeter + this.backgroundImages.xMeter]];
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
            //init_agvtags();
            LoadNotification("routetrip");
            LoadNotification("vehicle");
            //add user to the tag groups only for none PMCCUser
            if (!/(^PMCCUser$)/i.test(User.UserId)) {
                fotfmanager.server.joinGroup("PeopleMarkers");
            }
        }
        else {
            fotfmanager.server.GetIndoorMap().done(function (GetIndoorMap) {
                if (GetIndoorMap.length > 0) {
                    $.each(GetIndoorMap, function () {
                        map.attributionControl.setPrefix("USPS " + this.backgroundImages.applicationFullName + " (" + this.backgroundImages.softwareVersion + ")");
                        $('#fotf-site-facility-name').append(this.backgroundImages.facilityName);
                        map.attributionControl.addAttribution(this.backgroundImages.facilityName);
                        $(document).prop('title', this.backgroundImages.facilityName + ' ' + this.backgroundImages.applicationAbbr);
                    });
                }
            })
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
                ).insertBefore('div[id=map]');
        }
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