function AddZoneDrawControls() {
    map.pm.addControls(draw_options);
}
$('#Custom_Zone_Modal').on('hidden.bs.modal', function () {
    $(this)
        .find("input[type=text],textarea,select")
        .val('')
        .end()
        .find("span[class=text]")
        .val('')
        .text("")
        .end()
        .find('input[type=checkbox]')
        .prop('checked', false).change()
        .end();
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
    dragMode: false,
    removalMode: false
};

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
                            init_CustomBinZones();
                        }
                        else {
                            e.layer.remove();
                        }
                    });
                    setTimeout(function () { $("#Custom_Zone_Modal").modal('hide'); }, 1000);
                }
            }
        }
        catch (ex) {

        }


    });
    $('button[id=customzonecloseBtn][type=button]').off().on('click', function () {
        e.layer.remove();
    })
});

function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}