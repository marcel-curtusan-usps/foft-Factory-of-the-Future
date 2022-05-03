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
                    togeo.properties.zone_type = ztsel;
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

//zone_type
$('#Custom_Zone_Modal').on('shown.bs.modal', function () {
    enableCustomZoneSubmit();
    if ($('select[name=custom_zone_type]').val() === "") {
        $('select[name=custom_zone_type]').removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_zone_type]').text("Please Select Zone Type");
    }
    $('select[name=custom_zone_type]').change(function () {
        if (!checkValue($('select[name=custom_zone_type]').val())) {
            $('select[name=custom_zone_type]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_zone_type]').text("Please Select Zone Type");
        }
        else {
            $('select[name=custom_zone_type]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_zone_type]').text("");
        }
        enableCustomZoneSubmit();
    });


    //zone_name
    if (!checkValue($('input[type=text][name=zone_name]').val())) {
        $('input[type=text][name=zone_name]').removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_zone_name]').text("Please enter Zone Name");
    }
    else {
        $('input[type=text][name=zone_name]').removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_zone_name]').text("");
    }
    $('input[type=text][name=zone_name]').keyup(function () {
        if (!checkValue($('input[type=text][name=zone_name]').val())) {
            $('input[type=text][name=zone_name]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_zone_name]').text("Please Enter Zone Name");
        }
        else {
            $('input[type=text][name=zone_name]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_zone_name]').text("");
        }
        enableCustomZoneSubmit();
    });

    //bin_machine_name
    if (!checkValue($('input[type=text][name=bin_machine_name]').val())) {
        $('input[type=text][name=bin_machine_name]').removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_bin_machine_name]').text("Please enter Machine Name");
    }
    else {
        $('input[type=text][name=bin_machine_name]').removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_bin_machine_name]').text("");
    }
    $('input[type=text][name=bin_machine_name]').keyup(function () {
        if (!checkValue($('input[type=text][name=bin_machine_name]').val())) {
            $('input[type=text][name=bin_machine_name]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_bin_machine_name]').text("Please Enter Machine Name");
        }
        else {
            $('input[type=text][name=bin_machine_name]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_bin_machine_name]').text("");
        }
        enableCustomZoneSubmit();
    });
    //bin_machine_number
    if (!checkValue($('input[type=text][name=bin_machine_number]').val())) {
        $('input[type=text][name=bin_machine_number]').css({ "border-color": "#FF0000" }).removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_bin_machine_number]').text("Please Enter Machine Number");
    }
    else {
        $('input[type=text][name=bin_machine_number]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_bin_machine_number]').text("");
    }
    $('input[type=text][name=bin_machine_number]').keyup(function () {
        if ($.isNumeric($('input[type=text][name=bin_machine_number]').val())) {
            $('input[type=text][name=bin_machine_number]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_bin_bin_machine_number]').text("");
        }
        else {
            $('input[type=text][name=bin_machine_number]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_bin_bin_machine_number]').text("Please Enter Machine Number");
        }
        enableCustomZoneSubmit();
    });
    //bin_bins
    //if (!checkValue($('textarea[type=text][name=bin_bins]').val())) {
    //    $('textarea[type=text][name=bin_bins]').removeClass('is-valid').addClass('is-invalid');
    //    $('span[id=error_bin_bins]').text("Please enter bin number(s)");
    //}
    //else {
    //    $('textarea[type=text][name=bin_bins]').removeClass('is-invalid').addClass('is-valid');
    //    $('span[id=error_bin_bins]').text("");
    //}


    //$('input[type=text][name=bin_bins]').keyup(function () {
    //    if (!checkValue($('textarea[name=bin_bins]').val())) {
    //        $('textarea[name=bin_bins]').removeClass('is-valid').addClass('is-invalid');
    //        $('span[id=error_bin_bins]').text("Please enter bin number(s)");
    //    }
    //    else {
    //        $('textarea[name=bin_bins]').removeClass('is-invalid').addClass('is-valid');
    //        $('span[id=error_bin_bins]').text("");
    //    }
    //    enableCustomZoneSubmit();
    //});

});

function enableCustomZoneSubmit() {
    if ($('select[name=custom_zone_type]').hasClass('is-valid') &&
        $('input[type=text][name=zone_name]').hasClass('is-valid') &&
        $('input[type=text][name=bin_machine_name]').hasClass('is-valid') &&
        $('input[type=text][name=bin_machine_number]').hasClass('is-valid') //&&
        //$('input[type=text][name=bin_bins]').hasClass('is-valid')
    ) {
        $('button[id=customzonesubmitBtn]').prop('disabled', false);
    }
    else {
        $('button[id=customzonesubmitBtn]').prop('disabled', true);
    }
}

function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}