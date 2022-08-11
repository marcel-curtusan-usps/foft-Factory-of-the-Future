


var stagingBullpenAreas = new L.GeoJSON(null, {
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
            LoadStagingBullpenTables(feature.properties);

        });

        layer.bindTooltip(feature.properties.name, {
            permanent: true,
            interactive: true,
            direction: 'center',
            opacity: 1,
            className: 'location'
        }).openTooltip();
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        stagingBullpenAreas.bringToBack();
    },
    filter: function (feature, layer) {
        return feature.properties.visible;
    }

});


async function LoadStagingBullpenTables(dataproperties) {
    try {
        $zoneSelect[0].selectize.setValue(dataproperties.id, true);
        $('div[id=area_div]').css('display', 'none');
        $('div[id=machine_div]').css('display', 'none');
        $('div[id=agvlocation_div]').css('display', 'none');
        $('div[id=dockdoor_div]').css('display', 'none');
        $('div[id=trailer_div]').css('display', 'none');
        $('div[id=ctstabs_div]').css('display', 'none');
        $('div[id=dps_div]').css('display', 'none');
        $('div[id=vehicle_div]').css('display', 'none');
        $('div[id=layer_div]').css('display', 'none');
        $('div[id=bullpen_div]').attr("data-id", dataproperties.id);
        $('div[id=bullpen_div]').css('display', 'block');
        zonetop_Table_Body.empty();
        zonetop_Table_Body.append(zonetop_row_template.supplant(formatzonetoprow(dataproperties)));
       
    } catch (e) {
        console.log(e.message + ", " + e.stack);
    }
}