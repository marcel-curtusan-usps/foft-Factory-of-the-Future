/*
 this is for the dock door and container details.
 */
$.extend(fotfmanager.client, {
    updateDockDoorData: async (updateData, floorId, zoneId) => { Promise.all([UpdateDockDoorData(updateData, floorId, zoneId)]) }
});
$(function () {
    $('button[name=tripSelectorbtn]').off().on('click', function () {
        let jsonObject = {
            RouteTrip: $('select[name=tripSelector] option:selected').val(),
            DoorNumber: $('span[name=doornumberid]').text()
        };
        fotfmanager.server.updateRouteTripDoorAssigment(jsonObject).done();
    });
});

async function UpdateDockDoorData(data, floorId, zoneId) {
    try {
        if (baselayerid === floorId) {
            map.whenReady(() => {
                $.map(map._layers, function (layer, i) {
                    if (layer.hasOwnProperty("feature") && layer.zoneId === zoneId) {
                        layer.feature.properties = data.properties;
                        layer.setTooltipContent(data.properties.doorNumber.toString() + getDoorTripIndc(data.properties.dockdoorData));
                        if ($('div[id=dockdoor_div]').is(':visible') &&
                            $('div[id=dockdoor_div]').attr("data-id") === data.properties.id) {
                            Promise.all([LoadDockDoorTable(data.properties)]);
                        }
                        Promise.all([updatedockdoor(layer._leaflet_id)]);
                        return false;
                    }
                });

            });
        }
    }
    catch (e) {
    }
}

document.addEventListener("layerscontentvisible", () => {
    zoneStatusClose(true);
    
});
let dockDoors = new L.GeoJSON(null, {
    style: function (feature) {
        return {
            weight: 2,
            opacity: 1,
            color: '#3573b1',
            fillColor: GetDockDoorZoneColor(feature.properties.dockdoorData),//'#989ea4',
            fillOpacity: 0.5,
            lastOpacity: 0.5
        }
    },
    onEachFeature: function (feature, layer) {
        layer.zoneId = feature.properties.id;
        let dockdookflash = GetDockDoorFlash();
        let doorNumberdisplay = GetDockDoorTripDirection(feature.properties.dockdoorData, feature.properties.doorNumber);
     
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
       
        layer.on('click', function (e) {
            map.setView(e.sourceTarget.getCenter(), 4);
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
    }
    //,
    //filter: function (feature, layer) {
    //    return feature.properties.visible;
    //}
});
function GetDockDoorTripDirection(data, doorNumber)
{
    try {
        if (!!data && data.length > 0) {
            if (data[0].atDoor) {
                return doorNumber + "-" + getDoorTripIndc(data);
            }
            else {
                return doorNumber;
            }
        }
        else {
            return doorNumber;
        }
    } catch (e) {

    }
}
function GetDockDoorFlash(data) {
    try {
        if (!!data && data.length > 0) {
            if (data[0].atDoor) {
                if (data[0].tripDirectionInd === "O" && data[0].tripMin <= 30 && data[0].Notloadedcontainers > 0) {
                    return "doorflash";
                }
                else {
                    return "";
                }
            }
            else {
                return "";
            }
        }
        else {
            return "";
        }
    } catch (e) {
        return "";
    }
}
function GetDockDoorZoneColor(data)
{
    let activeTrip30MissingContainer = '#dc3545'; //red
    let activeTrip = '#3573b1'; //blue 
    let notTrip = '#989ea4'; //clear
    try {
        if (!!data && data.length > 0) {
            if (data[0].atDoor)
                if (data[0].tripDirectionInd === "O" && data[0].tripMin <= 30) {
                    return activeTrip30MissingContainer;
                }
                else {
                    return activeTrip;
                }
            else {
                return notTrip;
            }
        }
        else {
            return notTrip;
        }

    } catch (e) {
        return notTrip;
    }
}
function getDoorTripIndc(data)
{
    try {
        if (data.length > 0 && data[0].atDoor) {
            return "-" + data[0].tripDirectionInd;
        }
        else {
            return "";
        }
     
    } catch (e) {

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
    return !!status ? capitalize_Words(status) : "Unknown";
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
        backgroundcolorstatus: properties.constainerStatus === "Unloaded" ? "table-secondary" : properties.constainerStatus === "Close" ? "table-primary" : properties.constainerStatus === "Loaded" ? "table-success" : "table-danger"
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
        if (map._layers[layerindex].feature.properties.dockdoorData[0].atDoor) {
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
                    fillOpacity: 0.5,
                    lastOpacity: 0.5
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
        if ($('#doornumberid').val() === map._layers[layerindex].feature.properties.doorNumber ) {
            Promise.all([LoadDockDoorTable(map._layers[layerindex].feature.properties)]);
        }
      
    }
}
async function initDoorDataTable() {
    createDoorTripDataTable("dockdoortable");
    createTripAssignedDataTable("doortriptable");
}
let dockdoorloaddata = [];
async function createTripAssignedDataTable(table) {
    let arrayColums = [{
        "scheduledDtm": "",
        "actualDtm": "",
        "routeTrip": "",
        "doorNumber": "",
        "tripDirectionInd": "",
        "legSiteName": ""
    }]
    var columns = [];
    var tempc = {};
    $.each(arrayColums[0], function (key, value) {
        tempc = {};
        if (/scheduledDtm/i.test(key)) {
            tempc = {
                "title": "Schd",
                "mDataProp": key,
                "class": "row-cts-route",
                "mRender": function (data, type, full) {
                    return objSVTime(full["scheduledDtm"])
                }
            }
        }
        else if (/actualDtm/i.test(key)) {
            tempc = {
                "title": "I/O Time",
                "mDataProp": key,
                "class": "row-cts-trip",
                "mRender": function (data, type, full) {
                    return objSVTime(full["actualDtm"])
                }
            }
        }
        else if (/routeTrip/i.test(key)) {
            tempc = {
                "title": "Route-Trip",
                "mDataProp": key,
                "class": "row-cts-trip",
                "mRender": function (data, type, full) {
                    return full["route"] + " - " + full["trip"]
                }
            }
        }
        else if (/doorNumber/i.test(key)) {
            tempc = {
                "title": "Door",
                "mDataProp": key,
                "class": "row-cts-trip",
                "mRender": function (data, type, full) {
                    return full["doorNumber"]
                }
            }
        }
        else if (/tripDirectionInd/i.test(key)) {
            tempc = {
                "title": "Direction",
                "mDataProp": key,
                "class": "row-cts-trip",
                "mRender": function (data, type, full) {
                    return full["tripDirectionInd"] === "O" ? "Out-bound" : "In-bound"
                }
            }
        }
        else if (/legSiteName/i.test(key)) {
            tempc = {
                "title": "Dest",
                "mDataProp": key,
                "class": "row-cts-trip",
                "mRender": function (data, type, full) {
                    return full["legSiteName"]
                }
            }
        }
        columns.push(tempc);
    });
    $('#' + table).DataTable({
        dom: 'Bfrtip',
        bFilter: false,
        bdeferRender: true,
        paging: false,
        bPaginate: false,
        bAutoWidth: true,
        bInfo: false,
        destroy: true,
        language: {
            zeroRecords: "No Trip at Dock Door"
        },
        aoColumns: columns,
        columnDefs: [],
        rowCallback: function (row, data, index) {
            $(row).find('td:eq(0)').css('text-align', 'right');
            $(row).find('td:eq(1)').css('text-align', 'left');

        }
    });
}
async function createDoorTripDataTable(table) {
    let arrayColums = [{
        "name": "",
        "value": ""
    }]
    var columns = [];
    var tempc = {};
    $.each(arrayColums[0], function (key, value) {
        tempc = {};
        tempc = {
            "title": capitalize_Words(key.replace(/\_/, ' ')),
            "mDataProp": key
        }
        columns.push(tempc);
    });
    $('#' + table).DataTable({
        dom: 'Bfrtip',
        bFilter: false,
        bdeferRender: true,
        paging: false,
        bPaginate: false,
        bAutoWidth: true,
        bInfo: false,
        destroy: true,
        aoColumns: columns,
        columnDefs: [],
        rowCallback: function (row, data, index) {
            $(row).find('td:eq(0)').css('text-align', 'right');
            $(row).find('td:eq(1)').css('text-align', 'left');

        }
    });
}
async function removeTripsDoorAssignedDatatable(id, table) {
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows(function (idx, data, node) {
            if (data.id === id) {
                $('#' + table).DataTable().row(node).remove().draw();
            }
        })
    }
}
async function updateTripAssignedDataTable(ldata, table) {
    var load = true;
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows(function (idx, data, node) {
            load = false;
            if (data.id === ldata.id) {

                $('#' + table).DataTable().row(node).data(ldata).draw().invalidate();
            }

        });
        if (load) {
            loadDatatable(ldata, table);
        }
    }
}
async function updateDoorTripDataTable(ldata, table) {
    var load = true;
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows(function (idx, data, node) {
            load = false;
            if (data.name === ldata.name) {

                $('#' + table).DataTable().row(node).data(ldata).draw().invalidate();
            }

        });
        if (load) {
            loadDatatable(ldata, table);
        }
    }
}
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
        $('select[id=tripSelector]').val("");
        $('span[name=doorview]').empty();
        $("<a/>").attr({ target: "_blank", href: URLconstructor(window.location) + 'Dockdoor/Dockdoor.aspx?DockDoor=' + data.doorNumber, style:'color:white;' }).html("View").appendTo($('span[name=doorview]'));
   /*     $('span[name=doorview]').text('<a herf="' + window.location.origin +'/dockdoor/dockdoor.aspx?dockdoor=' + data.doorNumber +'">View<a/>');*/
        $('span[name=doorstatus]').text("Unknown");
        $zoneSelect[0].selectize.setValue(-1, true);
        $zoneSelect[0].selectize.setValue(data.id, true);
        //dockdoortop_Table_Body.empty();
        container_Table_Body.empty();
        //assignedTrips_Table_Body.empty();
        $('#dockdoortable').DataTable().clear().draw();
        $('#doortriptable').DataTable().clear().draw();
        if (data.dockdoorData.length > 0) {
            if (data.dockdoorData[0].atDoor) {
                $('span[name=doorstatus]').text(getDoorStatus(data.status));
                let dataproperties = data.dockdoorData[0];
                let tempdata = [];
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
                updateDoorTripDataTable(tempdata, "dockdoortable");
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
                                    if (d.hasUnloadScans === true && d.trailer === dataproperties.trailerBarcode) {
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
                }
            }
            else {
                
            }
            updateTripAssignedDataTable(data.dockdoorData, "doortriptable")
        }

    }
    catch (e) {

    }
}
async function LoadDoorDetails(door) {
    if (dockDoors.hasOwnProperty("_layers")) {
        $.map(dockDoors._layers, function (layer, i) {
            if (layer.hasOwnProperty("feature")) {
                if (layer.feature.properties.doorNumber === door) {
                    map.invalidateSize();
                    map.setZoom(1);
                    Promise.all([LoadDockDoorTable(layer.feature.properties)]);

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
