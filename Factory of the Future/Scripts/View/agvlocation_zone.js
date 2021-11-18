 /*
  this is for AGV location 
  */


var agvLocations = new L.GeoJSON(null, {
    style: function (feature) {
        if (feature.properties.visible) {
            //'green' : 'gray'
            let inmision = feature.properties.hasOwnProperty("inMission") ? '#28a745' : '#989ea4';

            let style = {
                weight: 1,
                opacity: 1,
                color: '#3573b1',
                fillOpacity: 0.2,
                fillColor: inmision
            };
            return style;
        }
    },
    onEachFeature: function (feature, layer) {
        //$('<option>', { text: feature.properties.name, value: feature.properties.id }).appendTo('select[id=zoneselect]');
        layer.on('click', function (e) {
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();

            map.setView(e.latlng, 3);
            sidebar.open('home');
            LoadAGVLocationTables(feature.properties, "agvlocationtable")
        });

        //if (inmision) {
        //    popupHtml = "<div>"
        //    popupHtml += "<table style='border:0px'>"
        //    popupHtml += "<tr><td>Name</td><td>: </td><td>" + feature.properties.name + "</td></tr>"

        //    popupHtml += "</table>"
        //    popupHtml += "</br>"
        //    popupHtml += "<div style='text-align:right'><input type='button' name='zoneBtnDelete' value='delete' class='zoneBtnDelete btn btn-outline-info' />&nbsp;&nbsp;<input type='button' name='zoneBtnEdit' value='edit' class='zoneBtnEdit btn btn-outline-info' /></div>"
        //    popupHtml += "</div > ";
        //    layer.bindPopup(popupHtml, {
        //        permanent: true,
        //        direction: 'center'
        //    }).openPopup();

        //}
        layer.bindTooltip(Get_location_Code(feature.properties.name), {
            direction: 'auto',
            opacity: 1
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
                if (agvLocations._layers[layerindex].hasOwnProperty("feature") && agvLocations._layers[layerindex].feature.properties.id === locationupdate.properties.id) {
                    if (locationupdate.properties.hasOwnProperty("inMission") && locationupdate.properties.inMission) {
                        agvLocations._layers[layerindex].setStyle({
                            weight: 1,
                            opacity: 1,
                            color: '#3573b1',
                            fillColor: '#98c9fa',
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
                //map._layers[layerindex].setTooltipContent(locationupdate.properties.hasOwnProperty("vehicleNumber") ? locationupdate.properties.vehicleNumber : 0);

            }
        }
        else {
            agvLocations.addData(locationupdate);
        }


    } catch (e) {
        console.log(e);
    }
}
async function LoadAGVLocationTables(dataproperties, table) {
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
        let agvlocation_Table = $('table[id=' + table + ']');
        let agvlocation_Table_Body = agvlocation_Table.find('tbody');
        agvlocation_Table_Body.empty();
        let agvlocation_row_template = '<tr data-id="{zoneId}"><td>{zone_type}</td><td>{zoneId}</td></tr>"';

       
        agvlocation_Table_Body.append(agvlocation_row_template.supplant(formatAGVzonetoprow(dataproperties)));
    } catch (e) {
        console.log(e)
    }
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

            location_temp = a + b + " - " + c + ',' + d;
        }
    }
    return location_temp;
}
async function init_agvlocation() {
    //Get AGV Location list
    fotfmanager.server.getAGVLocationZonesList().done(function (agvlocationzoneData) {
        if (agvlocationzoneData.length > 0) {
            agvLocations.addData(agvlocationzoneData);
        }
    });
}
