
var binzonepoly = new L.GeoJSON(null, {
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
        layer.on('click', function (e) {
            //set to the center of the polygon.
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng);
            sidebar.open('home');
        });
        layer.bindTooltip(feature.properties.name, {
            permanent: true,
            direction: 'center',
            //opacity: 0.8,
            opacity: 1,
            className: 'location'
        }).openTooltip();
        binzonepoly.bringToBack();
    },
});


var draw_options = {
    position: 'topleft',
    oneBlock: true,
    snappingOption: false,
    drawRectangle: true,
    drawMarker: false,
    drawPolygon: false,
    drawPolyline: false,
    drawCircleMarker: false,
    drawCircle: false,
    editMode: false,
    cutPolygon: false,
    dragMode: false
};

function AddZoneDrawControls() {
    map.pm.addControls(draw_options);
}

map.on('pm:create', function (e) {
    var newlayer = e;
    $("#Custom_Zone_Modal").modal('show');
    $('button[id=customzonesubmitBtn][type=button]').off().on('click', function () {
        try {
            var ztsel = $('select[name=custom_zone_type] option:selected').val();
            if (ztsel != null) {
                if (ztsel == "Bin") {
                    
                    var togeo = newlayer.layer.toGeoJSON();
                    var geoProp = {
                        id: uuidv4(),
                        zone_type: "",
                        name: "",
                        machine_name: "",
                        machine_number: "",
                        bins: "",
                        location_Update: "",
                        visible: ""
                    }
                    togeo.properties = geoProp;
                    togeo.properties.zone_type = "Custom_" + ztsel;
                    togeo.properties.name = $('input[id="zone_name"]').val();
                    togeo.properties.machine_name = $('input[id="bin_machine_name"]').val();
                    togeo.properties.machine_number = $('input[id="bin_machine_number"]').val();
                    togeo.properties.bins = $('textarea[id="bin_bins"]').val();
                    togeo.properties.location_Update = true;
                    togeo.properties.visible = true;

                    $.connection.FOTFManager.server.addCustomZone(JSON.stringify(togeo)).done(function (Data) {
                        if (Data.length > 0) {
                            
                        }
                        else {

                        }
                    });
                    //$.connection.FOTFManager.server.AddCustomZone(JSON.stringify(togeo));


                }
            }
        }
        catch (ex) {

        }
        

    });
});

function zonetypeselectchange(zoneType) {
    if (zoneType.value == "Bin") {
        document.getElementById("customzonebinmachineinfo").style = "display:block;";

    }
    else {
        document.getElementById("customzonebinmachineinfo").style = "display:none;";
    }
}

function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

async function init_CustomBinZones() {
    fotfmanager.server.getCustomBinZonesList().done(function (bindatazone) {
        if (bindatazone.length > 0) {
            $.each(bindatazone, function () {
                try {
                    binzonepoly.addData(this);
                }
                catch (e) {
                    var err = e;
                }
               
            });
        }
    })
}

