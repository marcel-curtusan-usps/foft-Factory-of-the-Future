/*
 this is for the dock door and container details.
 */
$.extend(fotfmanager.client, {
    updateDockDoorStatus: async (dockdoorupdate) => { updateDockDoorZone(dockdoorupdate) }
});
async function updateDockDoorZone(dockdoorzoneupdate) {
    try {
        let layerindex = -0;
        if (dockDoors.hasOwnProperty("_layers")) {

            $.map(dockDoors._layers, function (layer, i) {
                if (layer.hasOwnProperty("feature")) {
                    if (layer.feature.properties.id === dockdoorzoneupdate.properties.id) {
                        layer.feature.properties = dockdoorzoneupdate.properties;
                        layerindex = layer._leaflet_id;
                        return false;
                    }
                }
            });
            if (layerindex !== -0) {
                if ($('div[id=dockdoor_div]').is(':visible') && $('div[id=dockdoor_div]').attr("data-id") === dockdoorzoneupdate.properties.id) {
                    updatedockdoor(layerindex);
                }
            }
            else {
                dockDoors.addData(dockdoorzoneupdate);
            }
        }

    } catch (e) {
        console.log(e);
    }
}

var dockDoors = new L.GeoJSON(null, {
    style: function (feature) {
        if (feature.properties.hasOwnProperty("dockDoorData")) {
            if (feature.properties.dockDoorData.hasOwnProperty("tripDirectionInd")) {
                if (feature.properties.dockDoorData.tripDirectionInd === "O") {
                    if (feature.properties.dockDoorData.tripMin <= 30) {
                        return {
                            weight: 2,
                            opacity: 1,
                            color: '#3573b1',       // Blue
                            fillColor: '#dc3545',   // Red. ff0af7 is Purple
                            fillOpacity: 0.5,
                            label: feature.properties.name
                        };
                    } 
                    else {
                        return {
                            weight: 2,
                            opacity: 1,
                            color: '#3573b1',       // Blue
                            fillColor: '#3573b1',   // Blue. #98c9fa is lighter blue.
                            fillOpacity: 0.5,
                            label: feature.properties.name
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
                        label: feature.properties.name
                    };
                }
            }
            else {
                return {
                    weight: 2,
                    opacity: 1,
                    color: '#3573b1',
                    fillColor: '#989ea4',
                    fillOpacity: 0.2,
                    label: feature.properties.name
                };
            }
        }
        else {
            return {
                weight: 2,
                opacity: 1,
                color: '#3573b1',
                fillColor: '#989ea4',
                fillOpacity: 0.2,
                label: feature.properties.name
            };
        }
    },
    onEachFeature: function (feature, layer) {
        var dockdookflash = "";
        if (feature.properties.hasOwnProperty("dockDoorData")) {
            if (feature.properties.dockDoorData.hasOwnProperty("tripDirectionInd")) {
                if (feature.properties.dockDoorData.tripDirectionInd === "O") {
                    if (feature.properties.dockDoorData.tripMin <= 30 && feature.properties.dockDoorData.unloadedContainers > 0) {
                        dockdookflash = "doorflash"
                    }
                }
            }
        }
        layer.on('click', function (e) {
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng, 3);
            sidebar.open('home');
            LoadDockDoorTable(feature.properties);
        })
        layer.bindTooltip(feature.properties.name.replace(/\D+/g, "").replace(/^0+/, ''), {
            permanent: true,
            direction: 'center',
            opacity: 0.9,
            className: 'dockdooknumber ' + dockdookflash
        }).openTooltip();
        dockDoors.bringToFront();
    }
});
async function LoadDockDoorTable(dataproperties) {
    try {
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
        $('div[id=dockdoor_div]').css('display', 'block');
        $('div[id=trailer_div]').css('display', 'block');

        $zoneSelect[0].selectize.setValue(-1, true);
        dockdoortop_Table_Body.empty();
        $('button[name=container_counts]').text(0 + "/" + 0);
        container_Table_Body.empty();
        if (dataproperties.hasOwnProperty("dockDoorData") && dataproperties.dockDoorData.hasOwnProperty("trailerBarcode")) {

            if (dataproperties.dockDoorData.hasOwnProperty("doorNumber")) {

                $('span[name=doornumberid]').text(dataproperties.dockDoorData.doorNumber);
            }
            if (dataproperties.dockDoorData.hasOwnProperty("legSiteName")) {
                tempdata.push({
                    name: "Site Name",
                    value: "(" + dataproperties.dockDoorData.legSiteId + ") " + dataproperties.dockDoorData.legSiteName
                })
            }
            if (dataproperties.dockDoorData.hasOwnProperty("route")) {
                tempdata.push({
                    name: "Route-Trip",
                    value: dataproperties.dockDoorData.route + "-" + dataproperties.dockDoorData.trip
                })
            }
            if (dataproperties.dockDoorData.hasOwnProperty("trailerBarcode")) {
                tempdata.push({
                    name: "Trailer Barcode",
                    value: dataproperties.dockDoorData.trailerBarcode
                })
            }
            if (dataproperties.dockDoorData.hasOwnProperty("status")) {
                if (checkValue(dataproperties.dockDoorData.status)) {
                    $('span[name=doorstatus]').text(capitalize_Words(dataproperties.dockDoorData.status));
                }
            }
            else {
                $('span[name=doorstatus]').text("Occupied");
            }
            //if (dataproperties.dockDoorData.hasOwnProperty("driverBarcode")) {
            //    tempdata.push({
            //        name: "Driver Barcode",
            //        value: dataproperties.dockDoorData.driverBarcode
            //    })
            //}
            //if (dataproperties.dockDoorData.hasOwnProperty("driverFirstName")) {
            //    var lastname = "";
            //    if (dataproperties.dockDoorData.hasOwnProperty("driverLastName")) {
            //        lastname = dataproperties.dockDoorData.driverLastName;
            //    }
            //    tempdata.push({
            //        name: "Driver Name",
            //        value: dataproperties.dockDoorData.driverFirstName + " " + lastname
            //    })
            //}
            //if (dataproperties.dockDoorData.hasOwnProperty("driverPhoneNumber")) {
            //    tempdata.push({
            //        name: "Driver Phone Number",
            //        value: dataproperties.dockDoorData.driverPhoneNumber
            //    })
            //}
            if (dataproperties.dockDoorData.hasOwnProperty("loadPercent")) {
                tempdata.push({
                    name: "Load Percent",
                    value: checkValue(dataproperties.dockDoorData.loadPercent) ? dataproperties.dockDoorData.loadPercent : 0
                })
            }
            if (dataproperties.dockDoorData.hasOwnProperty("mailerName")) {
                tempdata.push({
                    name: "Mailer",
                    value: checkValue(dataproperties.dockDoorData.mailerName) ? dataproperties.dockDoorData.mailerName : 0
                })
            }
            if (dataproperties.dockDoorData.hasOwnProperty("tripDirectionInd")) {
                tempdata.push({
                    name: "Direction",
                    value: dataproperties.dockDoorData.tripDirectionInd === "O" ? "Out-bound" : "In-bound"
                })
            }

            if (dataproperties.dockDoorData.hasOwnProperty("scheduledDtm")) {
                tempdata.push({
                    name: dataproperties.dockDoorData.tripDirectionInd === "O" ? "Scheduled To Depart" : "Scheduled Arrive Time",
                    value: (dataproperties.dockDoorData.scheduledDtm.month + 1) + "-" + dataproperties.dockDoorData.scheduledDtm.dayOfMonth + "-" + dataproperties.dockDoorData.scheduledDtm.year + " " +
                        pad(dataproperties.dockDoorData.scheduledDtm.hourOfDay, 2) + ":" + pad(dataproperties.dockDoorData.scheduledDtm.minute, 2) + ":" + pad(dataproperties.dockDoorData.scheduledDtm.second, 2)
                })
            }

            if (dataproperties.dockDoorData.tripDirectionInd === "I") {
                if (dataproperties.dockDoorData.hasOwnProperty("actualDtm")) {
                    tempdata.push({
                        name: "Actual Arrive Time",
                        value: (dataproperties.dockDoorData.actualDtm.month + 1) + "-" + dataproperties.dockDoorData.actualDtm.dayOfMonth + "-" + dataproperties.dockDoorData.actualDtm.year + " " +
                            pad(dataproperties.dockDoorData.actualDtm.hourOfDay, 2) + ":" + pad(dataproperties.dockDoorData.actualDtm.minute, 2) + ":" + pad(dataproperties.dockDoorData.actualDtm.second, 2)
                    })
                }
            }
            $.each(tempdata, function () {
                dockdoortop_Table_Body.append(dockdoortop_row_template.supplant(formatdockdoortoprow(this, dataproperties.id)));
            });

            $('button[name=container_counts]').text(0 + "/" + 0);



            if (dataproperties.dockDoorData.hasOwnProperty("tripDirectionInd")) {
                //var legSite = dataproperties.dockDoorData.tripDirectionInd === "I" ? User.NASS_Code : dataproperties.dockDoorData.legSiteId;
                //$.connection.FOTFManager.server.getContainer(legSite, dataproperties.dockDoorData.tripDirectionInd, dataproperties.dockDoorData.route, dataproperties.dockDoorData.trip).done(function (Data) {

                if (dataproperties.dockDoorData.containers.length > 0) {
                    var loadedcount = 0;
                    var unloadedcount = 0;
                    var loaddata = [];
                    container_Table_Body.empty();
                    $.each(dataproperties.dockDoorData.containers, function (index, d) {
                        if (!d.containerTerminate) {
                            if (dataproperties.dockDoorData.tripDirectionInd === "O") {
                                if (d.containerAtDest === false) {
                                    if (d.hasLoadScans === true && d.Otrailer === dataproperties.dockDoorData.trailerBarcode) {
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
                            if (dataproperties.dockDoorData.tripDirectionInd === "I") {
                                if (d.hasUnloadScans === true && d.Itrailer === dataproperties.dockDoorData.trailerBarcode) {
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
        }
        else {
            $('div[id=trailer_div]').css('display', 'none');
            if (dataproperties.dockDoorData.hasOwnProperty("doorNumber")) {
                $('span[name=doornumberid]').text(dataproperties.dockDoorData.doorNumber);
            }
            else {
                $('span[name=doornumberid]').text(parseInt(dataproperties.doorNumber));
            }
            if (dataproperties.dockDoorData.hasOwnProperty("status")) {
                $('span[name=doorstatus]').text(capitalize_Words(dataproperties.dockDoorData.status));
            }
            else {
                $('span[name=doorstatus]').text("Unknown");
            }
            $.each(tempdata, function () {
                dockdoortop_Table_Body.append(dockdoortop_row_template.supplant(formatdockdoortoprow(this, dataproperties.id)));
            });
        }


    } catch (e) {
        console.log(e);
    }
}
let dockdoortop_Table = $('table[id=dockdoortable]');
let dockdoortop_Table_Body = dockdoortop_Table.find('tbody');
let dockdoortop_row_template = '<tr data-id={zone_id} >' +
    '<td class="text-right" style="border-right-style:solid" >{name}</td>' +
    '<td class="text-left">{value}</td>' +
    '</tr>';

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
    if (dockDoors._layers[layerindex].feature.properties.hasOwnProperty("dockDoorData")) {
        if (dockDoors._layers[layerindex].feature.properties.dockDoorData.hasOwnProperty("tripDirectionInd")) {
         
            if (dockDoors._layers[layerindex].feature.properties.dockDoorData.tripDirectionInd === "O") {
                if (dockDoors._layers[layerindex].feature.properties.dockDoorData.tripMin <= 30) {
                    dockDoors._layers[layerindex].setStyle({
                        weight: 2,
                        opacity: 1,
                        color: '#3573b1',
                        fillColor: '#dc3545',   // Red. ff0af7 is Purple
                        fillOpacity: 0.5
                    });
                    if (dockDoors._layers[layerindex].feature.properties.dockDoorData.unloadedContainers > 0) {
                        if (dockDoors._layers[layerindex].hasOwnProperty("_tooltip")) {
                            if (dockDoors._layers[layerindex]._tooltip.hasOwnProperty("_container")) {
                                if (!dockDoors._layers[layerindex]._tooltip._container.classList.contains('doorflash')) {
                                    dockDoors._layers[layerindex]._tooltip._container.classList.add('doorflash');
                                }
                            }
                        }
                    }
                    else {
                        if (dockDoors._layers[layerindex].hasOwnProperty("_tooltip")) {
                            if (dockDoors._layers[layerindex]._tooltip.hasOwnProperty("_container")) {
                                if (dockDoors._layers[layerindex]._tooltip._container.classList.contains('doorflash')) {
                                    dockDoors._layers[layerindex]._tooltip._container.classList.remove('doorflash');
                                }
                            }
                        }
                    }

                }
                else {
                    dockDoors._layers[layerindex].setStyle({
                        weight: 2,
                        opacity: 1,
                        color: '#3573b1',
                        fillColor: '#3573b1',
                        fillOpacity: 0.5
                    });
                    if (dockDoors._layers[layerindex].hasOwnProperty("_tooltip")) {
                        if (dockDoors._layers[layerindex]._tooltip.hasOwnProperty("_container")) {
                            if (dockDoors._layers[layerindex]._tooltip._container.classList.contains('doorflash')) {
                                dockDoors._layers[layerindex]._tooltip._container.classList.remove('doorflash');
                            }
                        }
                    }
                }
            }
            if (dockDoors._layers[layerindex].feature.properties.dockDoorData.tripDirectionInd === "I") {
                dockDoors._layers[layerindex].setStyle({
                    weight: 2,
                    opacity: 1,
                    color: '#3573b1',
                    fillColor: '#3573b1',
                    fillOpacity: 0.5
                });
            }
            if ($('div[id=dockdoor_div]').is(':visible')) {
                var findtrdataid = dockdoortop_Table_Body.find('tr[data-id=' + dockDoors._layers[layerindex].feature.properties.id + ']');
                if (findtrdataid.length > 0) {
                    LoadDockDoorTable(dockDoors._layers[layerindex].feature.properties);
                }
            }
        }
        else {
            dockDoors._layers[layerindex].setStyle({
                weight: 2,
                opacity: 1,
                color: '#3573b1',
                fillColor: '#989ea4',
                fillOpacity: 0.2
            });
            if (dockDoors._layers[layerindex].hasOwnProperty("_tooltip")) {
                if (dockDoors._layers[layerindex]._tooltip.hasOwnProperty("_container")) {
                    if (dockDoors._layers[layerindex]._tooltip._container.classList.contains('doorflash')) {
                        dockDoors._layers[layerindex]._tooltip._container.classList.remove('doorflash');
                    }
                }
            }
        }
    }
}

async function LoadDoorDetails(door) {
    if (dockDoors.hasOwnProperty("_layers")) {
        $.map(dockDoors._layers, function (layer, i) {
            if (layer.hasOwnProperty("feature")) {
                if (parseInt(layer.feature.properties.name.replace(/\D+/g, "").replace(/^0+/, '')) === parseInt(door)) {
                    var Center = new L.latLng(
                        (layer._bounds._southWest.lat + layer._bounds._northEast.lat) / 2,
                        (layer._bounds._southWest.lng + layer._bounds._northEast.lng) / 2);
                    map.setView(Center, 3);
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
async function init_dockdoor() {
    //Get Doock Doorlist
    fotfmanager.server.getDockDoorZonesList().done(function (dockdoorzoneData) {
        if (dockdoorzoneData.length > 0) {
            $.each(dockdoorzoneData, function () {
                updateDockDoorZone(this);
            });
        }
    });
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
        $.map(d.dockDoorData.containers, function (container, i) {
            if (container.placardBarcode === placardid) {
                containerHistory = container.containerHistory
                return false;
            }
        });
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