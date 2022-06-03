$.extend(fotfmanager.client, {
    updateBinZoneStatus: async (binzoneupdate) => { updateBinZone(binzoneupdate) }
});

var binzonepoly = new L.GeoJSON(null, {
    style: function (feature) {
        return {
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillOpacity: 0.2,
            fillColor: '#989ea4',
            label: feature.properties.name
        };
    },
    onEachFeature: function (feature, layer) {
        var flash = "";
        if (feature.properties.MPE_Bins.length > 0)
        {
                flash = "doorflash";
        }
        layer.on('click', function (e) {
            //set to the center of the polygon.
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng);
            sidebar.open('home');
            LoadBinZoneTables(feature.properties);
        });
        layer.bindTooltip("", {
            permanent: true,
            direction: 'center',
            opacity: 1,
            className: 'dockdooknumber ' + flash
        }).openTooltip();
        binzonepoly.bringToFront();
    },
    filter: function (feature, layer) {
        return feature.properties.visible;
    }
});

async function LoadBinZoneTables(dataproperties) {
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
        $('div[id=staff_div]').css('display', 'none');
        $('div[id=layer_div]').css('display', 'none');
        $('div[id=area_div]').css('display', 'block');
        czzonetop_Table_Body.empty();
        czzonetop_Table_Body.append(czzonetop_row_template.supplant(formatczzonetoprow(dataproperties)));
    } catch (e) {
        console.log(e)
    }
}

let czzonetop_Table = $('table[id=areazonetoptable]');
let czzonetop_Table_Body = czzonetop_Table.find('tbody');
let czzonetop_row_template = '<tr data-id="{zoneId}"><td style="width: 22%;">Bin Zone Name</td><td>{zoneName}</td></tr>' +
    '<tr><td>Bins Configured</td><td>{AssignedBins}</td></tr>' +
    '<tr><td>Full Bins</td><td>{fullbins}</td></tr>';
function formatczzonetoprow(properties) {
    return $.extend(properties, {
        zoneId: properties.id,
        zoneName: properties.name,
        AssignedBins: properties.bins,
        fullbins: properties.MPE_Bins.toString()
    });
}
async function updateBinZone(binzoneupdate) {
    try {
        if (id == baselayerid) {
            let layerindex = -0;
            if (binzonepoly.hasOwnProperty("_layers")) {
                $.map(binzonepoly._layers, function (layer, i) {
                    if (layer.feature.properties.id === binzoneupdate.properties.id) {
                        layer.feature.properties = binzoneupdate.properties;
                        layerindex = layer._leaflet_id;
                        Promise.all([updatebin(layerindex)]);
                        return false;
                    }
                });
                if (layerindex !== -0) {
                    if ($('div[id=area_div]').is(':visible') && $('div[id=area_div]').attr("data-id") === binzoneupdate.properties.id) {

                    }

                }
                else {
                    binzonepoly.addData(binzoneupdate);
                }
            }
        }
    } catch (e) {
        console.log(e);
    }
}
async function updatebin(layerindex) {
    if (binzonepoly._layers[layerindex].feature.properties.MPE_Bins.length > 0) {
        if (binzonepoly._layers[layerindex].hasOwnProperty("_tooltip")) {
            if (binzonepoly._layers[layerindex]._tooltip.hasOwnProperty("_container")) {
                if (!binzonepoly._layers[layerindex]._tooltip._container.classList.contains('doorflash')) {
                    binzonepoly._layers[layerindex]._tooltip._container.classList.add('doorflash');
                }
            }
        }
    }
    else {
        if (binzonepoly._layers[layerindex].hasOwnProperty("_tooltip")) {
            if (binzonepoly._layers[layerindex]._tooltip.hasOwnProperty("_container")) {
                if (binzonepoly._layers[layerindex]._tooltip._container.classList.contains('doorflash')) {
                    binzonepoly._layers[layerindex]._tooltip._container.classList.remove('doorflash');
                }
            }
        }
    }
    return true;
}