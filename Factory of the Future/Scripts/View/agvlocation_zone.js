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
            if (feature.properties.hasOwnProperty("inMission") && feature.properties.inMission) {
                inmission = '#28a745';
            }

            return {
                weight: 1,
                opacity: 1,
                color: '#3573b1',
                fillOpacity: 0.2,
                fillColor: inmission
            };
        }
    },
    onEachFeature: function (feature, layer) {

        layer.on('click', function (e) {
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();

            map.setView(e.latlng, 3);
            sidebar.open('home');
            LoadAGVLocationTables(feature.properties)
        });
        layer.bindTooltip(Get_location_Code(feature.properties.name), {
            direction: 'center',
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
                updatelocation(layerindex)
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
    if (agvLocations._layers[layerindex].feature.properties.hasOwnProperty("inMission") && agvLocations._layers[layerindex].feature.properties.inMission) {
        agvLocations._layers[layerindex].setStyle({
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillColor: '#28a745',
            fillOpacity: 0.5
        });
    }
    else {
        agvLocations._layers[layerindex].setStyle({
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillColor: '#9a9ea4',
            fillOpacity: 0.2
        });
    }
}
async function LoadAGVLocationTables(dataproperties) {
    try {
        $('div[id=agvlocation_div]').css('display', 'block');
        $('div[id=area_div]').css('display', 'none');
        $('div[id=dockdoor_div]').css('display', 'none');
        $('div[id=trailer_div]').css('display', 'none');
        $('div[id=machine_div]').css('display', 'none');
        $('div[id=staff_div]').css('display', 'none');
        $('div[id=ctstabs_div]').css('display', 'none');
        $('div[id=vehicle_div]').css('display', 'none');
        $('div[id=dps_div]').css('display', 'none');

        agvlocation_Table_Body.empty();
       
        agvlocation_Table_Body.append(agvlocation_row_template.supplant(formatAGVzonetoprow(dataproperties)));
    } catch (e) {
        console.log(e)
    }
}
let agvlocation_Table = $('table[id=agvlocationtable]');
let agvlocation_Table_Body = agvlocation_Table.find('tbody');
let agvlocation_row_template = '<tr data-id="{zoneId}"><td>{zone_type}</td><td>{zoneId}</td></tr>"';
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
async function init_agvlocation() {
    //Get AGV Location list
    fotfmanager.server.getAGVLocationZonesList().done(function (agvlocationzoneData) {
        if (agvlocationzoneData.length > 0) {
            $.each(agvlocationzoneData, function () {
                updateAGVLocationZone(this);
            });
        }
    });
}
