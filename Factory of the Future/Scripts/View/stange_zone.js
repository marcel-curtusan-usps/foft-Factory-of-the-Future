/*
 this is for stage zone
 */
$.extend(fotfmanager.client, {
    updateStageZoneStatus: async (updateStage) => { updateStageZone(updateStage) }
});
async function updateStageZone() {
    try {
    } catch (e) {
        console.log(e);
    }
}

var exitAreas = new L.GeoJSON(null, {
    style: function (feature) {
        return style = {
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillOpacity: 0.2,
            fillColor: '#989ea4'//'gray'
        };
    },
    onEachFeature: function (feature, layer) {
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        layer.on('click', function (e) {
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng);
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
            LoadstageTables(feature.properties);

        });
        layer.bindTooltip(feature.properties.name, {
            permanent: true,
            interactive: true,
            direction: 'center',
            //opacity: 0.8,
            opacity: 1,
            className: 'location'
        }).openTooltip();
        layer.bringToBack();

    }
})
var polyholesAreas = new L.GeoJSON(null, {
    style: function (feature) {
        return style = {
            weight: 0,
            opacity: 0,
            color: '#3573b1',
            fillOpacity: 0.0,
            fillColor: '#989ea4'//'gray'
        };
    },
    onEachFeature: function (feature, layer) {
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        layer.on('click', function (e) {
            //set to the center of the polygon.
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng);
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
            LoadstageTables(feature.properties);
        });
        layer.bindTooltip(feature.properties.name, {
            permanent: true,
            interactive: true,
            direction: 'center',
            //opacity: 0.8,
            opacity: 1,
            className: 'location'
        }).openTooltip();
        layer.bringToBack();
    },
});
var ebrAreas = new L.GeoJSON(null, {
    style: function (feature) {
        return {
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillOpacity: 0.2,
            fillColor: '#989ea4'//'gray'
        };
    },
    onEachFeature: function (feature, layer) {
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        layer.on('click', function (e) {
            //set to the center of the polygon.
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng);
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
            LoadstageTables(feature.properties);
        });
        layer.bindTooltip(feature.properties.name, {
            permanent: true,
            interactive: true,
            direction: 'center',
            //opacity: 0.8,
            opacity: 1,
            className: 'location'
        }).openTooltip();
        layer.bringToBack();
    },
});
var walkwayAreas = new L.GeoJSON(null, {
    style: function (feature) {
        return style = {
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillOpacity: 0.2,
            fillColor: '#989ea4'//'gray'
        };
    },
    onEachFeature: function (feature, layer) {
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        layer.on('click', function (e) {
            //set to the center of the polygon.
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng);
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
            LoadstageTables(feature.properties);
        });
        layer.bindTooltip(feature.properties.name, {
            permanent: true,
            interactive: true,
            direction: 'center',
            //opacity: 0.8,
            opacity: 1,
            className: 'location'
        }).openTooltip();
        layer.bringToBack();
    },
});
var stagingAreas = new L.GeoJSON(null, {
    style: function (feature) {
        return style = {
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillOpacity: 0.2,
            fillColor: '#989ea4'//'gray'
        };
    },
    onEachFeature: function (feature, layer) {
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        layer.on('click', function (e) {
            //set to the center of the polygon.
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng);
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
            LoadstageTables(feature.properties);
        
        });

        layer.bindTooltip(feature.properties.name.replace(/^Staging_/i, ''), {
            permanent: true,
            interactive: true,
            direction: 'center',
            //opacity: 0.8,
            opacity: 1,
            className: 'location'
        }).openTooltip();
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        layer.bringToBack();
        
    },
    filter: function (feature, layer) {
        return feature.properties.visible;
    }

});
async function LoadstageTables(dataproperties) {
    try {
        $zoneSelect[0].selectize.setValue(dataproperties.id, true);
        $('div[id=area_div]').attr("data-id", dataproperties.id);
        $('div[id=machine_div]').css('display', 'none');
        $('div[id=agvlocation_div]').css('display', 'none');
        $('div[id=dockdoor_div]').css('display', 'none');
        $('div[id=trailer_div]').css('display', 'none');
        $('div[id=ctstabs_div]').css('display', 'none');
        $('div[id=dps_div]').css('display', 'none');
        $('div[id=vehicle_div]').css('display', 'none');
        $('div[id=layer_div]').css('display', 'none');
        $('div[id=area_div]').css('display', 'block');
        zonetop_Table_Body.empty();
        zonetop_Table_Body.append(zonetop_row_template.supplant(formatzonetoprow(dataproperties)));
        var p2pdata = dataproperties.hasOwnProperty("P2PData") ? dataproperties.P2PData : "";
        var CurrentStaff = [];
        GetPeopleInZone(dataproperties.id, p2pdata, CurrentStaff);
    } catch (e) {
        console.log(e)
    }
}
let zonetop_Table = $('table[id=areazonetoptable]');
let zonetop_Table_Body = zonetop_Table.find('tbody');
let zonetop_row_template = '<tr data-id="{zoneId}"><td>{zone_type}</td><td>{zoneId}</td></tr>"';
function formatzonetoprow(properties) {
    return $.extend(properties, {
        zoneId: properties.name,
        zone_type: properties.Zone_Type
    });
}
async function LoadstageDetails(selcValue) {
    try {
        if (polygonMachine.hasOwnProperty("_layers")) {
            $.map(stagingAreas._layers, function (layer, i) {
                if (layer.hasOwnProperty("feature")) {
                    if (layer.feature.properties.id === selcValue) {
                        var Center = new L.latLng(
                            (layer._bounds._southWest.lat + layer._bounds._northEast.lat) / 2,
                            (layer._bounds._southWest.lng + layer._bounds._northEast.lng) / 2);
                        map.setView(Center, 3);
                        if (/Area/i.test(layer.feature.properties.Zone_Type)) {
                            LoadstageTables(layer.feature.properties);
                        }
                        return false;
                    }
                }
            });
        }
    } catch (e) {
        console.log(e)
    }

}
$('#zoneselect').change(function (e) {
    $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
    var selcValue = this.value;
    LoadstageDetails(selcValue);

});
 function init_zones(zoneData, id) {
    //Get Zones list
    $.each(zoneData, function () {
        if (/^ebr/i.test(this.properties.name)) {
          
            ebrAreas.addData(this);
        }
        else if (/^Staging/i.test(this.properties.name)) {
      
            stagingAreas.addData(this);
        }
        else if (/^Walkway/i.test(this.properties.name)) {
           
            walkwayAreas.addData(this);
        }
        else if (/^exit/i.test(this.properties.name)) {

            exitAreas.addData(this);
        }
        else if (/^(Poly|hol)/i.test(this.properties.name)) {
      
            polyholesAreas.addData(this);
        }
        else if (/^(DockDoor)/i.test(this.properties.Zone_Type)) {
            dockDoors.addData(this);
        }
        else if (/^(Machine)/i.test(this.properties.Zone_Type)) {
      
            polygonMachine.addData(this);
            fotfmanager.server.joinGroup("MachineZones");
        }
        else if (/^(Bin)/i.test(this.properties.Zone_Type)) {
         
            binzonepoly.addData(this);
            fotfmanager.server.joinGroup("BinZones");
        }
        else if (/^(AGVLocation)/i.test(this.properties.Zone_Type)) {
        
            agvLocations.addData(this);
        }
        else if (/^(ViewPorts)/i.test(this.properties.Zone_Type)) {
            viewPortsAreas.addData(this);
        }
        else {
            
            stagingAreas.addData(this);
        }
    })

    // setGreyedOut();
    fotfmanager.server.joinGroup("Zones");
}