/*
 this is for the dock door and container details.
 */
async function updateDockDoorZone(dockdoorzoneupdate) {
    try {
        if (dockdoorzoneupdate) {
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
                    if (dockDoors._layers[layerindex].feature.properties.hasOwnProperty("svDoorData")) {
                        if (dockDoors._layers[layerindex].feature.properties.svDoorData.hasOwnProperty("tripDirectionInd")) {
                            $dockdoortop_Table = $('table[id=dockdoortable]');
                            $dockdoortop_Table_Body = $dockdoortop_Table.find('tbody');
                            if (dockDoors._layers[layerindex].feature.properties.svDoorData.tripDirectionInd === "O") {
                                if (dockDoors._layers[layerindex].feature.properties.timeToDepart <= 30) {
                                    dockDoors._layers[layerindex].setStyle({
                                        weight: 2,
                                        opacity: 1,
                                        color: '#3573b1',
                                        fillColor: '#ff0af7',
                                        fillOpacity: 0.5
                                    });
                                    if (dockDoors._layers[layerindex].feature.properties.unloadedcount > 0) {
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
                                var findtrdataid = $dockdoortop_Table_Body.find('tr[data-id=' + dockdoorzoneupdate.properties.id + ']');
                                if (findtrdataid.length > 0) {
                                    LoadDockDoorTable(dockdoorzoneupdate.properties, 'dockdoortable');
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
                else {
                    dockDoors.addData(dockdoorzoneupdate);
                }
            }
        }
        
    } catch (e) {
        console.log(e);
    }
}

var dockDoors = new L.GeoJSON(null, {
    style: function (feature) {
        if (feature.properties.hasOwnProperty("svDoorData")) {
            if (feature.properties.svDoorData.hasOwnProperty("tripDirectionInd")) {
                if (feature.properties.svDoorData.tripDirectionInd === "O") {
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
        // $('<option>', { text: feature.properties.name, value: feature.properties.id }).appendTo('select[id=zoneselect]');
        var dockdookflash = "";
        if (feature.properties.hasOwnProperty("svDoorData")) {
            if (feature.properties.svDoorData.hasOwnProperty("tripDirectionInd")) {
                if (feature.properties.svDoorData.tripDirectionInd === "O") {
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
            LoadDockDoorTable(feature.properties, 'dockdoortable');
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
async function LoadDockDoorTable(dataproperties, table) {
    try {
        if (!$.isEmptyObject(dataproperties)) {
            $('table[id=' + table + '] tbody').empty();
            if (/dockdoortable/i.test(table)) {
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
                if (dataproperties.hasOwnProperty("svDoorData")) {
                    if (dataproperties.svDoorData.hasOwnProperty("doorNumber")) {
                        tempdata.push({
                            name: "Dock Door",
                            value: dataproperties.svDoorData.doorNumber
                        })
                    }
                    if (dataproperties.svDoorData.hasOwnProperty("legSiteName")) {
                        tempdata.push({
                            name: "Site Name",
                            value: "(" + dataproperties.svDoorData.legSiteId + ") " + dataproperties.svDoorData.legSiteName
                        })
                    }
                    if (dataproperties.svDoorData.hasOwnProperty("route")) {
                        tempdata.push({
                            name: "Route-Trip",
                            value: dataproperties.svDoorData.route + "-" + dataproperties.svDoorData.trip
                        })
                    }
                    if (dataproperties.svDoorData.hasOwnProperty("trailerBarcode")) {
                        tempdata.push({
                            name: "Trailer Barcode",
                            value: dataproperties.svDoorData.trailerBarcode
                        })
                    }
                    if (dataproperties.svDoorData.hasOwnProperty("status")) {
                        tempdata.push({
                            name: "Load Status",
                            value: dataproperties.svDoorData.status
                        })
                    }
                    //if (dataproperties.svDoorData.hasOwnProperty("driverBarcode")) {
                    //    tempdata.push({
                    //        name: "Driver Barcode",
                    //        value: dataproperties.svDoorData.driverBarcode
                    //    })
                    //}
                    //if (dataproperties.svDoorData.hasOwnProperty("driverFirstName")) {
                    //    var lastname = "";
                    //    if (dataproperties.svDoorData.hasOwnProperty("driverLastName")) {
                    //        lastname = dataproperties.svDoorData.driverLastName;
                    //    }

                    //    tempdata.push({
                    //        name: "Driver Name",
                    //        value: dataproperties.svDoorData.driverFirstName + " " + lastname
                    //    })
                    //}
                    //if (dataproperties.svDoorData.hasOwnProperty("driverPhoneNumber")) {
                    //    tempdata.push({
                    //        name: "Driver Phone Number",
                    //        value: dataproperties.svDoorData.driverPhoneNumber
                    //    })
                    //}
                    //if (dataproperties.svDoorData.hasOwnProperty("loadPercent")) {
                    //    tempdata.push({
                    //        name: "Load Percent",
                    //        value: dataproperties.svDoorData.loadPercent
                    //    })
                    //}
                    if (dataproperties.svDoorData.hasOwnProperty("tripDirectionInd")) {
                        tempdata.push({
                            name: "Direction",
                            value: dataproperties.svDoorData.tripDirectionInd === "O" ? "Out-bound" : "In-bound"
                        })
                    }
                    if (dataproperties.svDoorData.hasOwnProperty("scheduledDtm")) {
                        tempdata.push({
                            name: "Scheduled To Depart",
                            value: (dataproperties.svDoorData.scheduledDtm.month + 1) + "-" + dataproperties.svDoorData.scheduledDtm.dayOfMonth + "-" + dataproperties.svDoorData.scheduledDtm.year + " " +
                                pad(dataproperties.svDoorData.scheduledDtm.hourOfDay, 2) + ":" + pad(dataproperties.svDoorData.scheduledDtm.minute, 2) + ":" + pad(dataproperties.svDoorData.scheduledDtm.second, 2)
                        })
                    }
                    if (dataproperties.svDoorData.tripDirectionInd === "I") {
                        if (dataproperties.svDoorData.hasOwnProperty("actualDtm")) {
                            tempdata.push({
                                name: "Actual Arrive Time ",
                                value: (dataproperties.svDoorData.actualDtm.month + 1) + "-" + dataproperties.svDoorData.actualDtm.dayOfMonth + "-" + dataproperties.svDoorData.actualDtm.year + " " +
                                    pad(dataproperties.svDoorData.actualDtm.hourOfDay, 2) + ":" + pad(dataproperties.svDoorData.actualDtm.minute, 2) + ":" + pad(dataproperties.svDoorData.actualDtm.second, 2)
                            })
                        }
                        else {
                            tempdata.push({
                                name: "Actual Arrive Time ",
                                value: "Has Not Arrived"
                            })
                        }
                    }
                }
                $dockdoortop_Table = $('table[id=' + table + ']');
                $dockdoortop_Table_Body = $dockdoortop_Table.find('tbody');
                $dockdoortop_Table_Body.empty();
                $dockdoortop_row_template = '<tr data-id=' + dataproperties.id + '>' +
                    '<td class="text-right" style="border-right-style:solid" >{name}</td>' +
                    '<td class="text-left">{value}</td>' +
                    '</tr>';

                function formatdockdoortoprow(properties) {
                    return $.extend(properties, {
                        name: properties.name.replace(/^0+/, ''),
                        value: properties.value
                    });
                }
                $.each(tempdata, function () {
                    $dockdoortop_Table_Body.append($dockdoortop_row_template.supplant(formatdockdoortoprow(this)));
                });
                $container_Table = $('table[id=containertable]');
                $container_Table_Body = $container_Table.find('tbody');

                $('button[name=container_counts]').text(0 + "/" + 0);
                $container_row_template = '<tr>' +
                    '<td data-input="dest" class="text-center">{dest}</td>' +
                    '<td data-input="location" class="text-center">{location}</td>' +
                    '<td data-input="placard" class="text-center"><a data-doorid=' + dataproperties.id + ' data-placardid={placard} class=containerdetails>{placard}</a></td>' +
                    '<td data-input="status" class="text-center {backgroundcolorstatus}">{status}</td>' +
                    '</tr>"';

              
                if (dataproperties.svDoorData.hasOwnProperty("tripDirectionInd")) {
                    //var legSite = dataproperties.svDoorData.tripDirectionInd === "I" ? User.NASS_Code : dataproperties.svDoorData.legSiteId;
                    //$.connection.FOTFManager.server.getContainer(legSite, dataproperties.svDoorData.tripDirectionInd, dataproperties.svDoorData.route, dataproperties.svDoorData.trip).done(function (Data) {

                    if (dataproperties.svDoorData.containers.length > 0) {
                        var loadedcount = 0;
                        var unloadedcount = 0;
                        var loaddata = [];
                        $container_Table_Body.empty();
                        $.each(dataproperties.svDoorData.containers, function (index, d) {
                            if (!d.containerTerminate) {
                                if (dataproperties.svDoorData.tripDirectionInd === "O") {
                                    if (d.containerAtDest === false) {
                                        if (d.hasLoadScans === true && d.Otrailer === dataproperties.svDoorData.trailerBarcode) {
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
                                        //if (d.containerRedirectedDest === true && d.hasLoadScans === false) {
                                        //    unloadedcount++
                                        //    d.constainerStatus = "Unloaded";
                                        //    d.sortind = 1;
                                        //    loaddata.push(d);
                                        //}
                                        //if (d.containerRedirectedDest === false && d.hasLoadScans === true) {
                                        //    loadedcount++
                                        //    d.constainerStatus = "Loaded";
                                        //    d.sortind = 2;
                                        //    loaddata.push(d);
                                        //}
                                    }
                                }
                                if (dataproperties.svDoorData.tripDirectionInd === "I") {
                                    if (d.hasUnloadScans === true && d.Itrailer === dataproperties.svDoorData.trailerBarcode) {
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
                            $container_Table_Body.append($container_row_template.supplant(formatctscontainerrow(this)));
                        });

                        $('button[name=container_counts]').text(loadedcount + "/" + unloadedcount);
                    }
                    else {
                        $('button[name=container_counts]').text(0 + "/" + 0);
                        $container_Table_Body.empty();
                    }
                    //});
                }
                else {
                    $('button[name=container_counts]').text(0 + "/" + 0);
                    $container_Table_Body.empty();
                }
            }
        }

    } catch (e) {
        console.log(e);
    }
};
function formatctscontainerrow(properties) {
    return $.extend(properties, {
        id: properties.placardBarcode,
        dest: checkValue(properties.dest) ? properties.dest : "",
        location: checkValue(properties.location) ? properties.location : "",
        placard: properties.placardBarcode,
        status: properties.constainerStatus,
        backgroundcolorstatus: properties.constainerStatus === "Unloaded" ? "table-secondary" : properties.constainerStatus === "Close" ? "table-primary" : properties.constainerStatus === "Loaded" ? "table-success" : "table-danger",
    });
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
                    LoadDockDoorTable(layer.feature.properties, 'dockdoortable');
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
            dockDoors.addData(dockdoorzoneData);
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
            $.map(d.svDoorData.containers, function (container, i) {
                if (container.id === placardid) {
                    containerHistory = container.containerHistory
                    return false;
                }
            });
            if (!$.isEmptyObject(containerHistory))
            {
                containerHistory.sort(SortByind);
                $.each(containerHistory, function () {
                    containerdetails_Table_Body.append(containerdetails_row_template.supplant(formatcontainerdetailsrow(this)));
                });
            }
            $("#ContainerDetails_Modal").modal();
        }
    }
    catch (e) {
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