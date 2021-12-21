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
        layer.on('click', function (e) {
            //set to the center of the polygon.
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng);
            sidebar.open('home');
            //$('select[id=zoneselect]').val(feature.properties.id);
            $zoneSelect[0].selectize.setValue(feature.properties.id, true);
            $('div[id=div_area]').empty();
            $divhome = $('div[id=div_area]');
            $('div[id=machine_div]').css('display', 'none');
            $('div[id=agvlocation_div]').css('display', 'none');
            $('div[id=dockdoor_div]').css('display', 'none');
            $('div[id=trailer_div]').css('display', 'none');
            $('div[id=ctstabs_div]').css('display', 'none');
            $('div[id=area_div]').css('display', 'block');
            $('div[id=dps_div]').css('display', 'none');
            //var zonecardTemplate = '<div class="card">' +
            var zonecardTemplate = '<div class="card-body">' +
                //'<div class="card-body">' +
                '<div class="table-responsive">' +
                '<table class="table table-sm table-hover" id="areazonetoptable">' +
                '<tbody></tbody>' +
                '</table>' +
                '</div>' +
                //'</div>' +
                '</div >';
            $divhome.append(zonecardTemplate);
            $zonetop_Table = $('table[id=areazonetoptable]');
            $zonetop_Table_Body = $zonetop_Table.find('tbody');
            $zonetop_row_template = '<tr data-id="{zoneId}"><td>{zone_type}</td><td>{zoneId}</td></tr>"';

            function formatzonetoprow(properties) {
                return $.extend(properties, {
                    zoneId: properties.name,
                    zone_type: properties.Zone_Type
                });
            }
        });
        exitAreas.bringToBack();
    },
    filter: function (feature, layer) {
        if (feature.properties.visible) {
            return false;
        }
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
        layer.on('click', function (e) {
            //set to the center of the polygon.
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng);
            sidebar.open('home');
            //$('select[id=zoneselect]').val(feature.properties.id);
            $zoneSelect[0].selectize.setValue(feature.properties.id, true);
            $('div[id=div_area]').empty();
            $divhome = $('div[id=div_area]');
            $('div[id=machine_div]').css('display', 'none');
            $('div[id=agvlocation_div]').css('display', 'none');
            $('div[id=dockdoor_div]').css('display', 'none');
            $('div[id=trailer_div]').css('display', 'none');
            $('div[id=ctstabs_div]').css('display', 'none');
            $('div[id=area_div]').css('display', 'block');
            $('div[id=dps_div]').css('display', 'none');
            //var zonecardTemplate = '<div class="card">' +
            var zonecardTemplate = '<div class="card-body">' +
                //'<div class="card-body">' +
                '<div class="table-responsive">' +
                '<table class="table table-sm table-hover" id="areazonetoptable">' +
                '<tbody></tbody>' +
                '</table>' +
                '</div>' +
                //'</div>' +
                '</div >';
            $divhome.append(zonecardTemplate);
            $zonetop_Table = $('table[id=areazonetoptable]');
            $zonetop_Table_Body = $zonetop_Table.find('tbody');
            $zonetop_row_template = '<tr data-id="{zoneId}"><td>{zone_type}</td><td>{zoneId}</td></tr>"';

            function formatzonetoprow(properties) {
                return $.extend(properties, {
                    zoneId: properties.name,
                    zone_type: properties.Zone_Type
                });
            }
            $zonetop_Table_Body.append($zonetop_row_template.supplant(formatzonetoprow(feature.properties)));
            var p2pdata = feature.properties.hasOwnProperty("P2PData") ? feature.properties.P2PData : "";
            var CurrentStaff = [];
            GetPeopleInZone(feature.properties.id, p2pdata, CurrentStaff);
        });
        layer.bindTooltip(feature.properties.name.replace(/^Staging_/i, ''), {
            permanent: true,
            direction: 'center',
            //opacity: 0.8,
            opacity: 1,
            className: 'location'
        }).openTooltip();
        polyholesAreas.bringToBack();
    },
});
var ebrAreas = new L.GeoJSON(null, {
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
        layer.on('click', function (e) {
            //set to the center of the polygon.
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng);
            sidebar.open('home');
            //$('select[id=zoneselect]').val(feature.properties.id);
            $zoneSelect[0].selectize.setValue(feature.properties.id, true);
            $('div[id=div_area]').empty();
            $divhome = $('div[id=div_area]');
            $('div[id=machine_div]').css('display', 'none');
            $('div[id=agvlocation_div]').css('display', 'none');
            $('div[id=dockdoor_div]').css('display', 'none');
            $('div[id=trailer_div]').css('display', 'none');
            $('div[id=ctstabs_div]').css('display', 'none');
            $('div[id=area_div]').css('display', 'block');
            $('div[id=dps_div]').css('display', 'none');
            //var zonecardTemplate = '<div class="card">' +
            var zonecardTemplate = '<div class="card-body">' +
                //'<div class="card-body">' +
                '<div class="table-responsive">' +
                '<table class="table table-sm table-hover" id="areazonetoptable">' +
                '<tbody></tbody>' +
                '</table>' +
                '</div>' +
                //'</div>' +
                '</div >';
            $divhome.append(zonecardTemplate);
            $zonetop_Table = $('table[id=areazonetoptable]');
            $zonetop_Table_Body = $zonetop_Table.find('tbody');
            $zonetop_row_template = '<tr data-id="{zoneId}"><td>{zone_type}</td><td>{zoneId}</td></tr>"';

            function formatzonetoprow(properties) {
                return $.extend(properties, {
                    zoneId: properties.name,
                    zone_type: properties.Zone_Type
                });
            }
            $zonetop_Table_Body.append($zonetop_row_template.supplant(formatzonetoprow(feature.properties)));
            var p2pdata = feature.properties.hasOwnProperty("P2PData") ? feature.properties.P2PData : "";
            var CurrentStaff = [];
            GetPeopleInZone(feature.properties.id, p2pdata, CurrentStaff);
        });
        layer.bindTooltip(feature.properties.name.replace(/^Staging_/i, ''), {
            permanent: true,
            direction: 'center',
            //opacity: 0.8,
            opacity: 1,
            className: 'location'
        }).openTooltip();
        ebrAreas.bringToBack();
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
        layer.on('click', function (e) {
            //set to the center of the polygon.
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng);
            sidebar.open('home');
            //$('select[id=zoneselect]').val(feature.properties.id);
            $zoneSelect[0].selectize.setValue(feature.properties.id, true);
            $('div[id=div_area]').empty();
            $divhome = $('div[id=div_area]');
            $('div[id=machine_div]').css('display', 'none');
            $('div[id=agvlocation_div]').css('display', 'none');
            $('div[id=dockdoor_div]').css('display', 'none');
            $('div[id=trailer_div]').css('display', 'none');
            $('div[id=ctstabs_div]').css('display', 'none');
            $('div[id=area_div]').css('display', 'block');
            $('div[id=dps_div]').css('display', 'none');
            //var zonecardTemplate = '<div class="card">' +
            var zonecardTemplate = '<div class="card-body">' +
                //'<div class="card-body">' +
                '<div class="table-responsive">' +
                '<table class="table table-sm table-hover" id="areazonetoptable">' +
                '<tbody></tbody>' +
                '</table>' +
                '</div>' +
                //'</div>' +
                '</div >';
            $divhome.append(zonecardTemplate);
            $zonetop_Table = $('table[id=areazonetoptable]');
            $zonetop_Table_Body = $zonetop_Table.find('tbody');
            $zonetop_row_template = '<tr data-id="{zoneId}"><td>{zone_type}</td><td>{zoneId}</td></tr>"';

            function formatzonetoprow(properties) {
                return $.extend(properties, {
                    zoneId: properties.name,
                    zone_type: properties.Zone_Type
                });
            }
            $zonetop_Table_Body.append($zonetop_row_template.supplant(formatzonetoprow(feature.properties)));
            var p2pdata = feature.properties.hasOwnProperty("P2PData") ? feature.properties.P2PData : "";
            var CurrentStaff = [];
            GetPeopleInZone(feature.properties.id, p2pdata, CurrentStaff);
        });
        layer.bindTooltip(feature.properties.name.replace(/^Staging_/i, ''), {
            permanent: true,
            direction: 'center',
            //opacity: 0.8,
            opacity: 1,
            className: 'location'
        }).openTooltip();
        walkwayAreas.bringToBack();
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
        layer.on('click', function (e) {
            //set to the center of the polygon.
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng);
            sidebar.open('home');
            LoadstageTables(feature.properties, "areazonetoptable");
        
        });
        layer.bindTooltip(feature.properties.name.replace(/^Staging_/i, ''), {
            permanent: true,
            direction: 'center',
            //opacity: 0.8,
            opacity: 1,
            className: 'location'
        }).openTooltip();
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        stagingAreas.bringToBack();
    }
});
async function LoadstageTables(dataproperties, table) {
    try {
        $zoneSelect[0].selectize.setValue(dataproperties.id, true);
        $('div[id=div_area]').empty();
        $divhome = $('div[id=div_area]');
        $('div[id=machine_div]').css('display', 'none');
        $('div[id=agvlocation_div]').css('display', 'none');
        $('div[id=dockdoor_div]').css('display', 'none');
        $('div[id=trailer_div]').css('display', 'none');
        $('div[id=ctstabs_div]').css('display', 'none');
        $('div[id=area_div]').css('display', 'block');
        $('div[id=dps_div]').css('display', 'none');
        //var zonecardTemplate = '<div class="card">' +
        var zonecardTemplate = '<div class="card-body">' +
            //'<div class="card-body">' +
            '<div class="table-responsive">' +
            '<table class="table table-sm table-hover" id="areazonetoptable">' +
            '<tbody></tbody>' +
            '</table>' +
            '</div>' +
            //'</div>' +
            '</div >';
        $divhome.append(zonecardTemplate);
        $zonetop_Table = $('table[id=areazonetoptable]');
        $zonetop_Table_Body = $zonetop_Table.find('tbody');
        $zonetop_row_template = '<tr data-id="{zoneId}"><td>{zone_type}</td><td>{zoneId}</td></tr>"';

        function formatzonetoprow(properties) {
            return $.extend(properties, {
                zoneId: properties.name,
                zone_type: properties.Zone_Type
            });
        }
        $zonetop_Table_Body.append($zonetop_row_template.supplant(formatzonetoprow(dataproperties)));
        var p2pdata = dataproperties.hasOwnProperty("P2PData") ? dataproperties.P2PData : "";
        var CurrentStaff = [];
        GetPeopleInZone(dataproperties.id, p2pdata, CurrentStaff);
    } catch (e) {
        console.log(e)
    }
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
                            LoadstageTables(layer.feature.properties, 'areazonetoptable');
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
async function init_zones() {
    //Get Zones list
    fotfmanager.server.getZonesList().done(function (zoneData) {
        if (zoneData.length > 0) {
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
                else {
                    stagingAreas.addData(this);
                }
            })
        }
    })
}