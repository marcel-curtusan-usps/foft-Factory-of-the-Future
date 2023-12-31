﻿//side bar setup
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
/*    "Maintenance Call": "",*/
    "Cameras": cameras,
    "Badge": tagsMarkersGroup,
    "AGV Locations": agvLocations,
    "MPE Work Areas": polygonMachine,
    /*  "MPE Sparklines": machineSparklines,*/
    "High Value Items": hvi_tags,
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
    let results = new RegExp('[\?&]' + name + '=([^&#]*)', 'i').exec(window.location.search);

    return (results !== null) ? results[1] || 0 : false;
}


let layersSelected = [mainfloor];

//mainfloor,
// polygonMachine,
//    piv_vehicles, agv_vehicles, agvLocations, container, stagingAreas, tagsMarkersGroup, dockDoors, binzonepoly
if ($.urlParam('specifyLayers')) {
    if ($.urlParam('agvVehicles')) layersSelected.push(agv_vehicles);
    if ($.urlParam('pivVehicles')) layersSelected.push(piv_vehicles);
    if ($.urlParam('hviTags')) layersSelected.push(hvi_tags);
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
    layersSelected = [
        mainfloor,
        polygonMachine,
        piv_vehicles,
        agv_vehicles,
        hvi_tags,
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
    if (e.layer.options.id != baselayerid) {
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
            /*  *//* machineSparklines.clearLayers();*/
            binzonepoly.clearLayers();
            agvLocations.clearLayers();
            viewPortsAreas.clearLayers();
            stagingAreas.clearLayers();
            //markers
            tagsMarkersGroup.clearLayers();
            piv_vehicles.clearLayers();
            agv_vehicles.clearLayers();
            hvi_tags.clearLayers();
            cameras.clearLayers();
            locatorMarker.clearLayers();
            if (data.length > 0) {
                Promise.all([init_zones(data[0].zones, baselayerid)]);
                Promise.all([init_locators(data[0].locators, baselayerid)]);
            }
            assignIdsToLayerCheckboxes();
            setLayerCheckUncheckEvents();
            checkViewportLoad();
        });
    }

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
    //setTimeout(checkSparklineVisibility, 100);
    //updateAllCameras(Date.now());
    //UpdateCameraZoom();
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
           /* Edit_AppSetting("app_settingtable");*/
          
            break;
        case 'reports':
            Promise.all([getStaffInfo()]);
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
}).addTo(map);

map.addControl(new timedisplay());
// Add Layer Popover - Proposed
let layersControl = L.control.layers(baseLayers, overlayMaps, {
    sortLayers: true, sortFunction: function (layerA, layerB, nameA, nameB) {
        if ( /FLOOR/i.test(nameA)) {
            if (/MAIN/i.test(nameA)) {
                return -1;
            }
            else {
                return nameA < nameB ? -1 : (nameB < nameA ? 1 : 0);
            }
        }
    }, position: 'bottomright', collapsed: false
}).addTo(map);
//Add staffing button
var staffBtn = L.easyButton({
    position: 'topcenter',
    states: [{
        stateName: 'openstaffing',
        icon: '<div class="row staffing-row">' +
            '<div class="col-sm-6 no-top-border">Schedule</div>' +
            '<div class="col-sm-6 no-top-border">WorkZone</div>' +
            '<div class="col-sm-6 no-bottom-border" id="schstaffingbutton" style="font-size:1.2vw">0</div>' +
            '<div class="col-sm-6 no-bottom-border" id="staffingbutton" style="font-size:1.2vw"></div>' +
            '</div>',
        onClick: function (control) {
         
            Promise.all([showstaffdiv()]);
            sidebar.open('reports');
            control.state('closestaffing');
            $('div[id=schstaffingbutton]').text(tagsscheduled);
        }
    },
    {
        stateName: 'closestaffing',
        icon: '<div class="row staffing-row">' +
            '<div class="col-sm-6 no-top-border">Schedule</div>' +
            '<div class="col-sm-6 no-top-border">WorkZone</div>' +
            '<div class="col-sm-6 no-bottom-border" id="schstaffingbutton" style="font-size:1.2vw">0</div>' +
            '<div class="col-sm-6 no-bottom-border" id="staffingbutton" style="font-size:1.2vw"></div>' +
            '</div>',
        onClick: function (control) {
  
            Promise.all([hideTagdiv()]);
            sidebar.close('reports');
            control.state('openstaffing');
            $('div[id=schstaffingbutton]').text(tagsscheduled);
        }
    }]
});
async function hideTagdiv() {
    $('div[id=div_taginfo]').css('display', 'none');
}
async function showstaffdiv() {
    $('div[id=div_taginfo]').css('display', 'none');
    $('div[id=div_userinfo]').css('display', '');
    $('div[id=div_overtimeinfo]').css('display', '');
    $('div[id=div_staffinfo]').css('display', '');
}
staffBtn.button.style.width = '150px';
staffBtn.button.style.height = '70px';
staffBtn.addTo(map);
//Add zoom reset button
let btnZoomReset = L.easyButton({
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
      //  fotfmanager.server.getMap().done(function (MapData) {
            if (MapData.length > 0) {

                $.each(MapData, function (index, backgroundImages) {
                    if (!!backgroundImages) {
                        Promise.all([loadFloorPlanDatatable([this], "floorplantable")]);
                        //set new image
                        let trackingarea = L.polygon([[100, 150]], [[500, 5000]]);
                        let img = new Image();
                            //load Base64 image
                        img.src = backgroundImages.base64;
                            //create he bound of the image.
                            let bounds = [[backgroundImages.yMeter, backgroundImages.xMeter], [backgroundImages.heightMeter + backgroundImages.yMeter, backgroundImages.widthMeter + backgroundImages.xMeter]];
                            trackingarea = L.polygon(bounds, {});
                        
                       // if (index === 0) {
                            baselayerid = backgroundImages.coordinateSystemId;
                            mainfloor.options.id = backgroundImages.coordinateSystemId;
                            mainfloor.setUrl(img.src);
                            mainfloor.setZIndex(index);
                            mainfloor.setBounds(trackingarea.getBounds());
                            //center image
                            map.setView(trackingarea.getBounds().getCenter(), 1.5);
                            //Promise.all([init_zones(this.zones, baselayerid)]);
                            //Promise.all([init_locators(this.locators, baselayerid)]);
                        //}
                        //else if (!!this.backgroundImages) {
                        //    layersControl.addBaseLayer(L.imageOverlay(img.src, trackingarea.getBounds(), { id: this.id, zindex: index }), this.backgroundImages.name);

                        //}
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
        ///})
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
                    
                    $.each(staffing, function () {
                        $userstop_Table_Body.append($userstop_row_template.supplant(formatstafftoprow(this)));
                    })
                }
            } catch (e) {
              
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
                   
                    $.map(ldcalertstagsData, async function (properties, i) {
                        var findtrdataid = LDCAlert_Table_Body.find('tr[data-tag=ldc_alert_' + properties.properties.id + ']');
                        if (findtrdataid.length === 0) {
                            LDCAlert_Table_Body.append(LDCAlert_row_template.supplant(formatldcalertsrow(properties.properties)));
                            sortTable(LDCAlert_Table, 'asc');
                        }
                    })
                }
            } catch (e) {
             
            }
        });
    } catch (e) {
    }
}
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
function formatcompliancerow(properties) {
    return $.extend(properties, {
        tag_id: properties.id,
        tag_name: !/^n.a$/.test(properties.name) ? properties.name : /^n.a$/.test(properties.craftName) ? properties.craftName : properties.id,
        ldc: properties.hasOwnProperty("tacs") ? properties.tacs.hasOwnProperty("ldc") ? properties.tacs.ldc : "No LDC" : "No Tacs",
        op_code: properties.hasOwnProperty("tacs") ? properties.tacs.hasOwnProperty("operationId") ? properties.tacs.operationId : "No Op Code" : "No Tacs",
        paylocation: properties.hasOwnProperty("tacs") ? properties.tacs.hasOwnProperty("payLocation") ? properties.tacs.payLocation : "No payLocation" : "No Tacs"
    });
}
function formatstafftoprow(properties) {
    return $.extend(properties, {
        staffName: properties.name,
        staffId: properties.id,
        instaff: properties.hasOwnProperty("instaff") ? properties.instaff : 0,
        planstaff: properties.hasOwnProperty("planstaff") ? properties.planstaff : 0,
        outstaff: properties.hasOwnProperty("outstaff") ? properties.outstaff : 0
    });
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
    let asc = order === 'asc',
        tbody = table.find('tbody');
    tbody.find('tr').sort(function (a, b) {
        if (asc) {
            return $('td:first', a).text().localeCompare($('td:first', b).text());
        } else {
            return $('td:first', b).text().localeCompare($('td:first', a).text());
        }
    }).appendTo(tbody);
}