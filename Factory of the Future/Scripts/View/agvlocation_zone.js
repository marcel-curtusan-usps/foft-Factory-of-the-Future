 /*
  this is for AGV location 
  */
$.extend(fotfmanager.client, {
    updateAGVLocationStatus: async (updateAGVLocation) => { updateAGVLocationZone(updateAGVLocation) }
});

var agvLocations = new L.GeoJSON(null, {
    style: function (feature) {
        if (feature.properties.visible) {
            //'green' : 'gray'
            let inmission = '#989ea4';
            if (feature.properties.MissionList !== null && feature.properties.MissionList.length > 0)
            {
                inmission = '#28a745';
            }
            return {
                weight: 1,
                opacity: 1,
                color: '#3573b1',
                fillOpacity: 0.2,
                fillColor: inmission,
                lastOpacity: 0.2
            };
        }
    },
    onEachFeature: function (feature, layer) {
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        layer.on('click', function (e) {
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();

            map.setView(e.latlng, 3);
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
            LoadAGVLocationTables(feature.properties)
        });
        layer.bindTooltip(Get_location_Code(feature.properties.name), {
            direction: 'center',
            interactive: true,
            opacity: 0.9,
        }).openTooltip();
        agvLocations.bringToFront();
    }
});
async function updateAGVLocationZone(locationupdate) {
    try {
        if (agvLocations.hasOwnProperty("_layers")) {
            var layerindex = -0;
            $.map(agvLocations._layers, function (layer, i) {
                if (layer.hasOwnProperty("feature")) {
                    if (layer.feature.properties.id === locationupdate.properties.id) {
                        layer.feature.properties = locationupdate.properties;
                        layerindex = layer._leaflet_id;
                    
                        return false;
                    }
                }
            });
            if (layerindex !== -0) {
                if (agvLocations._layers[layerindex].feature.properties.hasOwnProperty("MissionList")) {
                    updatelocation(layerindex);
                }
                if ($('div[id=agvlocation_div]').is(':visible') && $('div[id=agvlocation_div]').attr("data-id") === locationupdate.properties.id) {
                    LoadAGVLocationTables(locationupdate.properties);
                }

            }
            else {
                agvLocations.addData(locationupdate);
            }
        }
    } catch (e) {
        console.log(e);
    }
}
async function updatelocation(layerindex) {
    if (agvLocations._layers[layerindex].feature.properties.hasOwnProperty("MissionList") && agvLocations._layers[layerindex].feature.properties.MissionList.length > 0) {
        agvLocations._layers[layerindex].setStyle({
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillColor: '#28a745',
            fillOpacity: 0.5,
            lastOpacity: 0.5
        });
    }
    else {
        agvLocations._layers[layerindex].setStyle({
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillColor: '#989ea4',
            fillOpacity: 0.2,
            lastOpacity: 0.2
        });
    }
}
async function LoadAGVLocationTables(dataproperties) {
    try {
        $zoneSelect[0].selectize.setValue(dataproperties.id, true);
        hideSidebarLayerDivs();
        $('div[id=agvlocation_div]').attr("data-id", dataproperties.id);
        $('div[id=agvlocation_div]').css('display', 'block');
        $zoneSelect[0].selectize.setValue(-1, true);
        $('span[name=locationid]').text(Get_location_Code(dataproperties.name));

        agvlocationinmission_Table_Body.empty();
        if (!$.isEmptyObject(dataproperties.MissionList)) {
            $.each(dataproperties.MissionList, function () {
                agvlocationinmission_Table_Body.append(agvlocation_row_inmission_template.supplant(formatagvlocationinmissionrow(this)));
            });
        }

    } catch (e) {
        console.log(e)
    }
}


let agvlocationinmission_Table = $('table[id=locationinmissiontable]');
let agvlocationinmission_Table_Body = agvlocationinmission_Table.find('tbody');

let agvlocation_row_inmission_template = '<tr class="text-center" id={RequestId}>' +
    '<td class="font-weight-bold" colspan="2">' +
        '<h6 class="control-label sectionHeader ml-1 mb-1 d-flex justify-content-between">'+
        'Mission Status' +
            '<span class="d-flex justify-content-between">'+
            'Request ID:'+
            '<span class="btn btn-secondary border-0 badge-dark badge">{RequestId}</span>  '+
            '</span>'+
        '</h6 >'+
    '</td>' +
    '<tr id=pickupLocation_{RequestId}><td>Vehicle Name:</td><td>{VehicleName}</td></tr>' +
    '<tr id=pickupLocation_{RequestId}><td>Pickup Location:</td><td>{PickupLocation}</td></tr>' +
    '<tr id=pickupLocationEta_{RequestId}><td>ETA to Pickup:</td><td>{PickupLocationEta}</td></tr>' +
    '<tr id=dropoffLocation_{RequestId}><td>Drop-Off Location:</td><td>{DropoffLocation}</td></tr>' +
    '<tr id=endLocation_{RequestId}><td>End Location:</td><td>{EndLocation}</td></tr>' +
    '<tr id=door_{RequestId}><td>Door:</td><td>{Door}</td></tr>' +
    '<tr id=placard_{RequestId}><td>Placard:</td><td>{Placard}</td></tr>' +
    '</tr>';
function formatagvlocationinmissionrow(properties) {
    return $.extend(properties, {
        RequestId: properties.RequestId,
        VehicleName: properties.Vehicle !== null ? properties.Vehicle : "N/A",
        PickupLocation: properties.hasOwnProperty("Pickup_Location") ? Get_location_Code(properties.Pickup_Location) : "N/A",
        DropoffLocation: properties.hasOwnProperty("Dropoff_Location") ? Get_location_Code(properties.Dropoff_Location) : "N/A",
        EndLocation: properties.hasOwnProperty("End_Location") ? Get_location_Code(properties.End_Location) : "N/A",
        PickupLocationEta: properties.ETA !== null ? properties.ETA : "N/A",
        Door: properties.Door !== null ? properties.Door : "N/A",
        Placard: properties.Placard !== null ? properties.Placard : "N/A"
    });
}
function formatAGVzonetoprow(properties) {
    return $.extend(properties, {
        zoneId: Get_location_Code(properties.name),
        zone_type: properties.Zone_Type
    });
}
function Get_location_Code(location) {
    var location_temp = "";
    if (checkValue(location)) {
        var arr = location.match(/.{1,3}/g);
        if (arr.length === 4) {
            var a = checkValue(arr[0].replace(/^0+/, '')) ? arr[0].replace(/^0+/, '') : "0";
            var b = checkValue(arr[1].replace(/^0+/, '')) ? arr[1].replace(/^0+/, '') : "0";
            var c = checkValue(arr[2].replace(/^0+/, '')) ? arr[2].replace(/^0+/, '') : "0";
            var d = checkValue(arr[3].replace(/^0+/, '')) ? arr[3].replace(/^0+/, '') : "0";

            location_temp = a + b + "-" + c + ',' + d;
        }
    }
    return location_temp;
}