/*
 this is for the dock door and container details.
 */
$.extend(fotfmanager.client, {
    updateDockDoorStatus: async (dockdoorupdate) => { updateDockDoorZone(dockdoorupdate) }
});
async function updateDockDoorZone(dockdoorzoneupdate) {
    try {
            if (dockDoors.hasOwnProperty("_layers")) {
                var layerindex = -0;
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
                    updatedockdoor(layerindex);
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
        if (feature.properties.hasOwnProperty("routetripData")) {
            if (feature.properties.routetripData.hasOwnProperty("tripDirectionInd")) {
                if (feature.properties.routetripData.tripDirectionInd === "O") {
                    if (feature.properties.timeToDepart <= 30) {
                        style = {
                            weight: 2,
                            opacity: 1,
                            color: '#3573b1',
                            fillColor: '#ff0af7',
                            fillOpacity: 0.5,
                            label: feature.properties.name
                        };
                    }
                    else {
                        style = {
                            weight: 2,
                            opacity: 1,
                            color: '#3573b1',
                            fillColor: '#3573b1',//#98c9fa
                            fillOpacity: 0.5,
                            label: feature.properties.name
                        };
                    }
                }
                else {
                    style = {
                        weight: 2,
                        opacity: 1,
                        color: '#3573b1',
                        fillColor: '#3573b1',//#98c9fa
                        fillOpacity: 0.5,
                        label: feature.properties.name
                    };
                }

                return style;
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
        if (feature.properties.hasOwnProperty("routetripData")) {
            if (feature.properties.routetripData.hasOwnProperty("tripDirectionInd")) {
                if (feature.properties.routetripData.tripDirectionInd === "O") {
                    if (feature.properties.timeToDepart <= 30 && feature.properties.unloadedcount > 0) {
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
        layer.bindTooltip(feature.properties.doorNumber.replace(/^0+/, ''), {
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
        if (!$.isEmptyObject(dataproperties)) {
            let loadtriphisory = false;
            let tempdata = [];
            $('div[id=dockdoor_div]').css('display', 'block');
            $('div[id=trailer_div]').css('display', 'block');
            $('div[id=machine_div]').css('display', 'none');
            $('div[id=agvlocation_div]').css('display', 'none');
            $('div[id=ctstabs_div]').css('display', 'none');
            $('div[id=vehicle_div]').css('display', 'none');
            $('div[id=staff_div]').css('display', 'none');
            $('div[id=area_div]').css('display', 'none');
            $('div[id=dps_div]').css('display', 'none');
            $zoneSelect[0].selectize.setValue(-1, true);
            dockdoortop_Table_Body.empty();
            $('button[name=container_counts]').text(0 + "/" + 0);
            container_Table_Body.empty();
            if (dataproperties.hasOwnProperty("routetripData") && dataproperties.routetripData.hasOwnProperty("trailerBarcode")) {
                
                if (dataproperties.routetripData.hasOwnProperty("doorNumber")) {
                   
                    $('span[name=doornumberid]').text(dataproperties.routetripData.doorNumber);
                }
                if (dataproperties.routetripData.hasOwnProperty("legSiteName")) {
                    tempdata.push({
                        name: "Site Name",
                        value: "(" + dataproperties.routetripData.legSiteId + ") " + dataproperties.routetripData.legSiteName
                    })
                }
                if (dataproperties.routetripData.hasOwnProperty("route")) {
                    tempdata.push({
                        name: "Route-Trip",
                        value: dataproperties.routetripData.route + "-" + dataproperties.routetripData.trip
                    })
                }
                if (dataproperties.routetripData.hasOwnProperty("trailerBarcode")) {
                    tempdata.push({
                        name: "Trailer Barcode",
                        value: dataproperties.routetripData.trailerBarcode
                    })
                }
                if (dataproperties.routetripData.hasOwnProperty("status")) {
                    if (checkValue(dataproperties.routetripData.status)) {
                        $('span[name=doorstatus]').text(capitalize_Words(dataproperties.routetripData.status));
                    }
                }
                else {
                    $('span[name=doorstatus]').text("Occupied");
                }
                //if (dataproperties.routetripData.hasOwnProperty("driverBarcode")) {
                //    tempdata.push({
                //        name: "Driver Barcode",
                //        value: dataproperties.routetripData.driverBarcode
                //    })
                //}
                //if (dataproperties.routetripData.hasOwnProperty("driverFirstName")) {
                //    var lastname = "";
                //    if (dataproperties.routetripData.hasOwnProperty("driverLastName")) {
                //        lastname = dataproperties.routetripData.driverLastName;
                //    }
                //    tempdata.push({
                //        name: "Driver Name",
                //        value: dataproperties.routetripData.driverFirstName + " " + lastname
                //    })
                //}
                //if (dataproperties.routetripData.hasOwnProperty("driverPhoneNumber")) {
                //    tempdata.push({
                //        name: "Driver Phone Number",
                //        value: dataproperties.routetripData.driverPhoneNumber
                //    })
                //}
                if (dataproperties.routetripData.hasOwnProperty("loadPercent")) {
                    tempdata.push({
                        name: "Load Percent",
                        value: dataproperties.routetripData.loadPercent
                    })
                }
                if (dataproperties.routetripData.hasOwnProperty("tripDirectionInd")) {
                    tempdata.push({
                        name: "Direction",
                        value: dataproperties.routetripData.tripDirectionInd === "O" ? "Out-bound" : "In-bound"
                    })
                }
                if (dataproperties.routetripData.hasOwnProperty("scheduledDtm")) {
                    tempdata.push({
                        name: "Scheduled To Depart",
                        value: (dataproperties.routetripData.scheduledDtm.month + 1) + "-" + dataproperties.routetripData.scheduledDtm.dayOfMonth + "-" + dataproperties.routetripData.scheduledDtm.year + " " +
                            pad(dataproperties.routetripData.scheduledDtm.hourOfDay, 2) + ":" + pad(dataproperties.routetripData.scheduledDtm.minute, 2) + ":" + pad(dataproperties.routetripData.scheduledDtm.second, 2)
                    })
                }
                //if (dataproperties.routetripData.tripDirectionInd === "I") {
                //    if (dataproperties.routetripData.hasOwnProperty("actualDtm")) {
                //        tempdata.push({
                //            name: "Actual Arrive Time ",
                //            value: (dataproperties.routetripData.actualDtm.month + 1) + "-" + dataproperties.routetripData.actualDtm.dayOfMonth + "-" + dataproperties.routetripData.actualDtm.year + " " +
                //                pad(dataproperties.routetripData.actualDtm.hourOfDay, 2) + ":" + pad(dataproperties.routetripData.actualDtm.minute, 2) + ":" + pad(dataproperties.routetripData.actualDtm.second, 2)
                //        })
                //    }
                //    else {
                //        tempdata.push({
                //            name: "Actual Arrive Time ",
                //            value: "Has Not Arrived"
                //        })
                //    }
                //}
                $.each(tempdata, function () {
                    dockdoortop_Table_Body.append(dockdoortop_row_template.supplant(formatdockdoortoprow(this, dataproperties.id)));
                });
  
                $('button[name=container_counts]').text(0 + "/" + 0);
          


                if (dataproperties.routetripData.hasOwnProperty("tripDirectionInd")) {
                    //var legSite = dataproperties.routetripData.tripDirectionInd === "I" ? User.NASS_Code : dataproperties.routetripData.legSiteId;
                    //$.connection.FOTFManager.server.getContainer(legSite, dataproperties.routetripData.tripDirectionInd, dataproperties.routetripData.route, dataproperties.routetripData.trip).done(function (Data) {

                    if (dataproperties.routetripData.containers.length > 0) {
                        var loadedcount = 0;
                        var unloadedcount = 0;
                        var loaddata = [];
                        container_Table_Body.empty();
                        $.each(dataproperties.routetripData.containers, function (index, d) {
                            if (!d.containerTerminate) {
                                if (dataproperties.routetripData.tripDirectionInd === "O") {
                                    if (d.containerAtDest === false) {
                                        if (d.hasLoadScans === true && d.Otrailer === dataproperties.routetripData.trailerBarcode) {
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
                                if (dataproperties.routetripData.tripDirectionInd === "I") {
                                    if (d.hasUnloadScans === true && d.Itrailer === dataproperties.routetripData.trailerBarcode) {
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
                if (dataproperties.routetripData.hasOwnProperty("doorNumber")) {
                    $('span[name=doornumberid]').text(dataproperties.routetripData.doorNumber);
                }
                else {
                    $('span[name=doornumberid]').text(parseInt(dataproperties.doorNumber));
                }
                if (dataproperties.routetripData.hasOwnProperty("status")) {
                    $('span[name=doorstatus]').text(capitalize_Words(dataproperties.routetripData.status));
                }
                else {
                    $('span[name=doorstatus]').text("Unknown");
                }
                $.each(tempdata, function () {
                    dockdoortop_Table_Body.append(dockdoortop_row_template.supplant(formatdockdoortoprow(this, dataproperties.id)));
                });
            }
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
    if (dockDoors._layers[layerindex].feature.properties.hasOwnProperty("routetripData")) {
        if (dockDoors._layers[layerindex].feature.properties.routetripData.hasOwnProperty("tripDirectionInd")) {
         
            if (dockDoors._layers[layerindex].feature.properties.routetripData.tripDirectionInd === "O") {
                if (dockDoors._layers[layerindex].feature.properties.routetripData.timeToDepart <= 30) {
                    dockDoors._layers[layerindex].setStyle({
                        weight: 2,
                        opacity: 1,
                        color: '#3573b1',
                        fillColor: '#ff0af7',
                        fillOpacity: 0.5
                    });
                    if (dockDoors._layers[layerindex].feature.properties.routetripData.unloadedcontainers > 0) {
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
                if (parseInt(layer.feature.properties.doorNumber) === parseInt(door)) {
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
        $.map(d.routetripData.containers, function (container, i) {
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