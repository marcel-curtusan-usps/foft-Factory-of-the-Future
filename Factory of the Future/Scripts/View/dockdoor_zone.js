/*
 this is for the dock door and container details.
 */
$.extend(fotfmanager.client, {
    updateDockDoorStatus: async (dockdoorupdate) => { updateDockDoorZone(dockdoorupdate) }
});
let greyedOut = false;

let polygonMachineZIndex = null;

function greyOutBG() {
    /*
    if (dockdoorloaddata.length > 0) {
        var greyOut = false;
        for (const dat of dockdoorloaddata) {

            if (checkZone(dat.zone_id)) {
                greyOut = true;
            }
        }
    }
    */

        var obj = new L.Evented();
        obj.once('greyout', greyOutBGOnce);
        obj.fire('greyout');
}
function greyOutBGOnce() {
    greyedOut = true;
    $("#layersContent").hide();
    setGreyedOut();
}

function unGreyOutBG() {
    greyedOut = false;
    var obj = new L.Evented();
    obj.once('ungreyout', unGreyOutBGOnce);
    obj.fire('ungreyout');
}
function unGreyOutBGOnce() {
    greyedOut = false;
    setGreyedOut();
    var mapLayerKeys = Object.keys(map._layers);

    for (var key of mapLayerKeys) {
        let layer = map._layers[key];
        if (layer.hasOwnProperty("feature")) {
            let tt = layer.getTooltip();
            if (tt) {
                tt.setOpacity(tt.options.lastOpacity);
                tt.options.lastOpacity = null;

            }
            if (layer.setStyle) {
                let style = layer.options.style;

                if (!layer.options.lastOpacity) {
                    layer.options.lastOpacity = layer.options.fillOpacity;
                }
                else {
                    style.fillOpacity = layer.options.lastOpacity;
                }
                layer.setStyle(style);
            }
        }
    }

    $.map(stagingBullpenAreas._layers, function (layer, i) {
        layer.bringToBack();
    });
    map.invalidateSize();
}

let checkboxStateBeforeGreyOut = null;
let updateGreyedOut = true;
function setGreyedOut() {
    var z = 0;
    if (greyedOut) {
        updateGreyedOut = true;
        if (checkboxStateBeforeGreyOut === null) {
            checkboxStateBeforeGreyOut = {};
            for (const id of layerCheckboxIds) {

                checkboxStateBeforeGreyOut[id] = $("#" + id).is(":checked");
                if (id !== "MainFloor" && id !== "AGVVehicles" &&

                    id !== "PIVVehicles" && id !== "StagingBullpenAreas" &&
                    id !== "Badge" && id !== "DockDoors") {
                    if (checkboxStateBeforeGreyOut[id] !== false) {
                        $("#" + id).trigger("click");
                    }
                }

            }
        }
        greyedOutRectangle.setStyle({ color: "#000000", weight: 1, fillOpacity: .65, stroke: false, zIndex: 5000 });
    
        popZonesToBack();
        greyedOutRectangle.bringToFront();
        if (dockdoorloaddata.length > 0) {
            for (var dat of dockdoorloaddata) {
                if (dat.constainerStatus !== "Loaded") {
                    popZone(dat.location, "front");
                }
            }
        }
    }
    if (!greyedOut && updateGreyedOut) {
        updateGreyedOut = false;
       
        for (const id of layerCheckboxIds) {

                if (checkboxStateBeforeGreyOut[id] !== $("#" + id).is(":checked")) {
                    $("#" + id).trigger("click");
                }

            }
        checkboxStateBeforeGreyOut = null;

        popZonesToBack();
        greyedOutRectangle.setStyle({ color: "#000000", weight: 1, fillOpacity: 0, stroke: false });
    }

    return true;
}

function popZonesToBack() {
    var bullpenObjKeys = Object.keys(stagingBullpenAreas._layers);

    for (var key of bullpenObjKeys) {
        let layer = stagingBullpenAreas._layers[key];

        let style = layer.options.style;
        if (!layer.options.lastOpacity) {
            layer.options.lastOpacity = layer.options.fillOpacity + 0;
        }
        var tooltip = layer.getTooltip();
        layer.lastTooltipOpacity = tooltip.opacity + 0;
        tooltip.setOpacity(0);
        style.fillColor = "#c0c0c0";
        style.fillOpacity = 0;
        layer.setStyle(style);
    }
}

function checkZone(zoneId) {

    var bullpenObjKeys = Object.keys(stagingBullpenAreas._layers);

    for (var key of bullpenObjKeys) {
        let layer = stagingBullpenAreas._layers[key];
        if (layer.hasOwnProperty("feature")) {
            if (zoneId === layer.feature.properties.id) {
                return true;
            }
        }
    }
    return false;
}
function popZone(zoneName, frontOrBack) {
    $.map(stagingBullpenAreas._layers, function (layer, i) {
        if (layer.hasOwnProperty("feature")) {
            if (layer.feature.properties.name === zoneName) {
                if (frontOrBack === "front") {

                    let style = layer.options.style;
                    if (!layer.options.lastOpacity) {
                        layer.options.lastOpacity = layer.options.fillOpacity + 0;
                    }
                    var tooltip = layer.getTooltip();
                    layer.lastTooltipOpacity = tooltip.opacity + 0;
                    tooltip.setOpacity(1);
                    style.fillOpacity = 1;
                    style.fillColor = "#ffffff"
                    layer.setStyle(style);
                    layer.bringToFront();
                }
                else {

                    var tooltip = layer.getTooltip();
                    let style = layer.options.style;
                    layer.lastTooltipOpacity = tooltip.opacity + 0;
                    tooltip.setOpacity(0.2);
                    style.fillOpacity = layer.options.lastOpacity + 0;
                    layer.setStyle(style);
                    layer.bringToBack();
                }

            }
        }
    });
}


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


let rtdm = document.getElementById("RouteTripDetails_Modal");
function zoneStatusClose(closeSidebar) {
    if (closeSidebar) {
        sidebar.close("home");
    }
    unGreyOutBG();
}


addToSidebarListenCollection(document.getElementById("sidebar"));
document.getElementById("sidebar").addEventListener("sidebarclose", () => {
    zoneStatusClose(false);
});

document.addEventListener("layerscontentvisible", () => {
    zoneStatusClose(true);
    
});
var dockDoors = new L.GeoJSON(null, {
    style: function (feature) {
        try {
            if (feature.properties.dockdoorData !== null) {
                if (feature.properties.dockdoorData.tripDirectionInd !== "") {
                    if (feature.properties.dockdoorData.tripDirectionInd === "O") {
                        if (feature.properties.dockdoorData.tripMin <= 30) {
                            return {
                                weight: 2,
                                opacity: 1,
                                color: '#3573b1',       // Blue
                                fillColor: '#dc3545',   // Red. ff0af7 is Purple
                                fillOpacity: 0.5,
                                label: feature.properties.doorNumber.toString()
                            };
                        }
                        else {
                            return {
                                weight: 2,
                                opacity: 1,
                                color: '#3573b1',       // Blue
                                fillColor: '#3573b1',   // Blue. #98c9fa is lighter blue.
                                fillOpacity: 0.5,
                                label: feature.properties.doorNumber.toString()
                            };
                        }
                    }
                    else {
                        return {
                            weight: 2,
                            opacity: 1,
                            color: '#3573b1',       // Blue
                            fillColor: '#3573b1',   // Blue. #98c9fa is lighter blue.
                            fillOpacity: 0.5,
                            label: feature.properties.doorNumber.toString()
                        };
                    }

                }
                else {
                    return {
                        weight: 1,
                        opacity: 1,
                        color: '#3573b1',
                        fillColor: '#989ea4',
                        fillOpacity: 0.2,
                        label: feature.properties.doorNumber.toString()
                    };
                }
            }
            else {
                return {
                    weight: 1,
                    opacity: 1,
                    color: '#3573b1',
                    fillColor: '#989ea4',
                    fillOpacity: 0.2 ,
                    label: feature.properties.doorNumber.toString()
                };
            }
        } catch (e) {
            console.log(e);
        }
    },
    onEachFeature: function (feature, layer) {
        var dockdookflash = "";
        if (feature.properties.dockdoorData !== null) {
            if (feature.properties.dockdoorData.tripDirectionInd === "O") {
                if (feature.properties.dockdoorData.tripMin <= 30 && feature.properties.dockdoorData.Notloadedcontainers > 0) {
                    dockdookflash = "doorflash"
                }
            }
        }
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
       
        layer.on('click', function (e) {
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            let bounds = mainfloor.getBounds();
            let latNorth = bounds._northEast.lat;
            let latSouth = bounds._southWest.lat;
            let lngWest = bounds._southWest.lng;
            let latMiddle = (latNorth + latSouth) / 2;
            let centerLeft = [latMiddle, lngWest];
            map.setView(centerLeft, 0);
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
        layer.bindTooltip(feature.properties.doorNumber.toString(), {
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
    '<td data-input="placard" class="text-center"><a data-doorid={zone_id} data-placardid={placard} class="containerdetails">{placard}</a></td>' +
    '<td data-input="status" class="text-center {backgroundcolorstatus}">{status}</td>' +
    '</tr>"';
async function updatedockdoor(layerindex) {
    if (map._layers[layerindex].feature.properties.dockdoorData !== null) {
        if (checkValue(map._layers[layerindex].feature.properties.dockdoorData.tripDirectionInd)) {

            if (map._layers[layerindex].feature.properties.dockdoorData.tripDirectionInd === "O") {
                if (map._layers[layerindex].feature.properties.dockdoorData.tripMin <= 30) {
                    map._layers[layerindex].setStyle({
                        weight: 2,
                        opacity: 1,
                        color: '#3573b1',
                        fillColor: '#dc3545',   // Red. ff0af7 is Purple
                        fillOpacity: 0.5
                    });
                    if (map._layers[layerindex].feature.properties.dockdoorData.Notloadedcontainers > 0) {
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
                        fillOpacity: 0.5
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
            if (map._layers[layerindex].feature.properties.dockdoorData.tripDirectionInd === "I") {
                map._layers[layerindex].setStyle({
                    weight: 2,
                    opacity: 1,
                    color: '#3573b1',
                    fillColor: '#3573b1',
                    fillOpacity: 0.2
                });
            }
        }
        else {
            map._layers[layerindex].setStyle({
                weight: 2,
                opacity: 1,
                color: '#3573b1',
                fillColor: '#989ea4',
                fillOpacity: 0.2
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
    if ($('div[id=dockdoor_div]').is(':visible')) {
        var findtrdataid = dockdoortop_Table_Body.find('tr[data-id=' + map._layers[layerindex].feature.properties.id + ']');
        if (findtrdataid.length > 0) {
            LoadDockDoorTable(map._layers[layerindex].feature.properties);
        }
    }
}

let dockdoorloaddata = [];
async function LoadDockDoorTable(dataproperties) {
    try {
        dockdoorloaddata = [];
        let loadtriphisory = false;
        let tempdata = [];
        $('div[id=dockdoor_div]').attr("data-id", dataproperties.id);
        $('div[id=machine_div]').css('display', 'none');
        $('div[id=agvlocation_div]').css('display', 'none');
        $('div[id=ctstabs_div]').css('display', 'none');
        $('div[id=vehicle_div]').css('display', 'none');
        $('div[id=staff_div]').css('display', 'none');
        $('div[id=area_div]').css('display', 'none');
        $('div[id=dps_div]').css('display', 'none');
        $('div[id=layer_div]').css('display', 'none');
        $('div[id=dockdoor_div]').css('display', 'block');
        $('div[id=trailer_div]').css('display', 'block');

        $zoneSelect[0].selectize.setValue(-1, true);
        $zoneSelect[0].selectize.setValue(dataproperties.id, true);
        dockdoortop_Table_Body.empty();
        $('button[name=container_counts]').text(0 + "/" + 0);
        container_Table_Body.empty();
        if (checkValue(dataproperties.dockdoorData.legSiteName)) {

            $('span[name=doornumberid]').text(dataproperties.doorNumber);
            if (dataproperties.dockdoorData.legSiteName !== "") {
                tempdata.push({
                    name: "Site Name",
                    value: "(" + dataproperties.dockdoorData.legSiteId + ") " + dataproperties.dockdoorData.legSiteName
                })
            }
            if (dataproperties.dockdoorData.route !== "") {
                tempdata.push({
                    name: "Route-Trip",
                    value: dataproperties.dockdoorData.route + "-" + dataproperties.dockdoorData.trip
                })
            }
            if (dataproperties.dockdoorData.trailerBarcode !== "") {
                tempdata.push({
                    name: "Trailer Barcode",
                    value: dataproperties.dockdoorData.trailerBarcode
                })
            }
            if (dataproperties.dockdoorData.status !== "") {
                if (checkValue(dataproperties.dockdoorData.status)) {
                    $('span[name=doorstatus]').text(capitalize_Words(dataproperties.dockdoorData.status));
                }
            }
            else {
                $('span[name=doorstatus]').text("Occupied");
            }
            if (dataproperties.dockdoorData.loadPercent !== null) {
                tempdata.push({
                    name: "Load Percent",
                    value: checkValue(dataproperties.dockdoorData.loadPercent) ? dataproperties.dockdoorData.loadPercent : 0
                })
            }
        //if (dataproperties.dockdoorData.supplier !== null) {
        //        tempdata.push({
        //            name: "Mailer",
        //            value: checkValue(dataproperties.dockdoorData.supplier) ? dataproperties.dockdoorData.supplier : 0
        //        })
        //    }
            if (dataproperties.dockdoorData.tripDirectionInd !== "") {
                tempdata.push({
                    name: "Direction",
                    value: dataproperties.dockdoorData.tripDirectionInd === "O" ? "Out-bound" : "In-bound"
                })
            }
            if (dataproperties.dockdoorData.scheduledDtm !== null) {
                tempdata.push({
                    name: dataproperties.dockdoorData.tripDirectionInd === "O" ? "Scheduled To Depart" : "Scheduled Arrive Time",
                    value: formatSVmonthdayTime(dataproperties.dockdoorData.scheduledDtm)
                })
            }
            if (dataproperties.dockdoorData.tripDirectionInd === "I") {
                if (dataproperties.dockdoorData.actualDtm !== null) {
                    tempdata.push({
                        name: "Actual Arrive Time",
                        value: formatSVmonthdayTime(dataproperties.dockdoorData.actualDtm)
                    })
                }
            }
            $.each(tempdata, function () {
                dockdoortop_Table_Body.append(dockdoortop_row_template.supplant(formatdockdoortoprow(this, dataproperties.id)));
            });

            $('button[name=container_counts]').text(0 + "/" + 0);
            if (dataproperties.dockdoorData.tripDirectionInd) {
                //var legSite = dataproperties.dockdoorData.tripDirectionInd === "I" ? User.NASS_Code : dataproperties.dockdoorData.legSiteId;
                //$.connection.FOTFManager.server.getContainer(legSite, dataproperties.dockdoorData.tripDirectionInd, dataproperties.dockdoorData.route, dataproperties.dockdoorData.trip).done(function (Data) {

                if (dataproperties.dockdoorData.containerScans !== null && dataproperties.dockdoorData.containerScans.length > 0) {
                    var loadedcount = 0;
                    var unloadedcount = 0;
                    var loaddata = [];
                    container_Table_Body.empty();
                    $.each(dataproperties.dockdoorData.containerScans, function (index, d) {
                        if (!d.containerTerminate) {
                            if (dataproperties.dockdoorData.tripDirectionInd === "O") {
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
                            if (dataproperties.dockdoorData.tripDirectionInd === "I") {
                                if (d.hasUnloadScans === true && d.Itrailer === dataproperties.dockdoorData.trailerBarcode) {
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
                   
                    
                    greyOutBG();
                    if (loaddata.length > 0) {
                        $.each(loaddata, function () {
                            container_Table_Body.append(container_row_template.supplant
                                (formatctscontainerrow(this, dataproperties.dockdoorData.id)));
                         
                        });
                    }

                    $('button[name=container_counts]').text(loadedcount + "/" + unloadedcount);
                }
                else {
                    $('button[name=container_counts]').text(0 + "/" + 0);
                    container_Table_Body.empty();

                    greyOutBG();
                }
                //});

            }
            else {
                $('button[name=container_counts]').text(0 + "/" + 0);
                container_Table_Body.empty();

                greyOutBG();
            }

            if (loadtriphisory) {
                $('div[id=trailer_div]').css('display', 'none');
            }
        }
        else {
            $('div[id=trailer_div]').css('display', 'none');
            $('span[name=doornumberid]').text(dataproperties.doorNumber.toString());
            $('span[name=doorstatus]').text(getDoorStatus(dataproperties.dockdoorData.status));
            $.each(tempdata, function () {
                dockdoortop_Table_Body.append(dockdoortop_row_template.supplant(formatdockdoortoprow(this, dataproperties.id)));
            });
            greyOutBG();
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
                    LoadDockDoorTable(layer.feature.properties);
                    return false;
                }
            }
        });
    }
}
function SortByind(a, b) {
    return a.sortind < b.sortind ? -1 : a.sortind > b.sortind ? 1 : 0;
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
        var d = {};
       
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