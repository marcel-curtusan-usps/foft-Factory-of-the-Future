
var binzonepoly = new L.GeoJSON(null, {
    style: function (feature) {
        return {
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillOpacity: 0.2,
            fillColor: '#989ea4'
        };
    },
    onEachFeature: function (feature, layer) {
        layer.on('click', function (e) {
            //set to the center of the polygon.
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng);
            sidebar.open('home');
            LoadBinZoneTables(feature.properties);
        });
        layer.bindTooltip(feature.properties.name, {
            permanent: false,
            direction: 'center',
            //opacity: 0.8,
            opacity: 1,
            className: 'location'
        }).openTooltip();
        binzonepoly.bringToFront();
    },
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

function zonetypeselectchange(zoneType) {
    if (zoneType.value == "Bin") {
        document.getElementById("customzonebinmachineinfo").style = "display:block;";

    }
    else {
        document.getElementById("customzonebinmachineinfo").style = "display:none;";
    }
}

async function init_CustomBinZones() {
    fotfmanager.server.getCustomBinZonesList().done(function (bindatazone) {
        if (bindatazone.length > 0) {
            $.each(bindatazone, function () {
                binzonepoly.addData(this);
            });
        }
    })
}

