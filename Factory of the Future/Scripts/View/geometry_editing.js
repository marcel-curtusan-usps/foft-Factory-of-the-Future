/*uses this for geometry editing  */

$('#Zone_Modal').on('hidden.bs.modal', function () {
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


//remove layers
$('#Remove_Layer_Modal').on('hidden.bs.modal', function () {
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
$('#Remove_Layer_Modal').on('shown.bs.modal', function () {
});
////this for later
function init_geometry_editing() {
    var draw_options = {
        position: 'bottomright',
        oneBlock: false,
        snappingOption: false,
        drawRectangle: true,
        drawMarker: true,
        drawPolygon: true,
        drawPolyline: false,
        drawCircleMarker: false,
        drawCircle: false,
        editMode: false,
        cutPolygon: false,
        dragMode: false,
        removalMode: true
    };
    map.pm.addControls(draw_options);
    map.on('pm:create', (e) => {
        $('div[id=zoneselectlist_div]').css('display', 'none');
        $('div[id=machine_div]').css('display', 'none');
        $('div[id=agvlocation_div]').css('display', 'none');
        $('div[id=dockdoor_div]').css('display', 'none');
        $('div[id=trailer_div]').css('display', 'none');
        $('div[id=ctstabs_div]').css('display', 'none');
        $('div[id=dps_div]').css('display', 'none');
        $('div[id=vehicle_div]').css('display', 'none');
        $('div[id=area_div]').css('display', 'none');
        $('div[id=staff_div]').css('display', 'none');
        $('div[id=layer_div]').css('display', 'block');
        if (e.shape === "Polygon") {
            $('select[name=zone_type] option[value=Bin]').prop('disabled', true);
            $('select[name=zone_type] option[value=Camera]').prop('disabled', true);
            CreateZone(e);
        }
        if (e.shape === "Rectangle") {
            $('select[name=zone_type] option[value=Bin]').prop('disabled', false);
            CreateBinZone(e);
        }
        if (e.shape === "Marker") {
            $('select[name=zone_type] option[value=Camera]').prop('disabled', false);
            CreateCamera(e);

        }
        $('button[id=zonecloseBtn][type=button]').off().on('click', function () {
            sidebar.close();
            e.layer.remove();
        })
    });
    map.on('pm:edit', (e) => {
        if (e.shape === 'Marker') {
            e.layer.bindPopup().openPopup();
        }
    });
    map.on('pm:remove', (e) => {
        if (e.shape === 'Marker') {
            RemoveMarkerItem(e);
        }
        else {
            RemoveZoneItem(e);
        }
    });

    //zone type
    $('select[name=zone_type]').change(function () {
        if (!checkValue($('select[name=zone_type]').val())) {
            $('select[name=zone_type]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_zone_type]').text("Please Select Zone Type");
        }
        else {
            $('select[name=zone_type]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_zone_type]').text("");
            
        }
    });
    //mpe select
    $('select[name=zone_select_name]').change(function () {
        if (!checkValue($('select[name=zone_select_name]').val())) {
            $('select[name=zone_select_name]').removeClass('is-valid').addClass('is-invalid');
            $('input[type=text][name=zone_name]').val("")
            $('span[id=error_zone_name]').text("Please Enter Name");
         
        }
        else {
            $('select[name=zone_select_name]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_zone_name]').text("");
            $('input[type=text][name=zone_name]').removeClass('is-invalid').addClass('is-valid');

        }
    });
    //bins name
    $('textarea[id=bin_bins]').keyup(function () {
        if (!checkValue($('textarea[id=bin_bins]').val())) {
            $('textarea[id=bin_bins]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_bin_bins]').text("Please Enter Bin Numbers");
        }
        else {
            $('textarea[id=bin_bins]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_bin_bins]').text("");
        }
        enableBinZoneSubmit();
    });
   //zone name
    $('input[type=text][name=zone_name]').keyup(function () {
        if (!checkValue($('input[type=text][name=zone_name]').val())) {
            $('input[type=text][name=zone_name]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_zone_name]').text("Please Enter Name");
        }
        else {
            $('input[type=text][name=zone_name]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_zone_name]').text("");
        }

        if ($('select[name=zone_type] option:selected').val() === "Camera") {

            enableCameraSubmit();
        }
        else if ($('select[name=zone_type] option:selected').val() === "Bin") {

            enableBinZoneSubmit();
        }
        else {
            enableZoneSubmit();
        }

    });
    //Camera URL
    $('select[name=cameraLocation]').change(function () {
        if (!checkValue($('select[name=cameraLocation]').val())) {
            $('select[name=cameraLocation]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_cameraLocation]').text("Please Select Type");
            $('input[type=text][name=zone_name]').val("")
            $('input[type=text][name=zone_name]').removeClass('is-invalid').removeClass('is-valid');
            $('span[id=error_zone_name]').text("");
        }
        else {
            $('select[name=cameraLocation]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_cameraLocation]').text("");
            $('input[type=text][name=zone_name]').val($('select[name=cameraLocation] option:selected').val())
            $('input[type=text][name=zone_name]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_zone_name]').text("");
        }
        enableCameraSubmit();
    });

}
function enableBinZoneSubmit() {
    if ($('select[name=zone_type]').hasClass('is-valid') &&
        $('input[type=text][name=zone_name]').hasClass('is-valid') &&
        $('textarea[id=bin_bins]').hasClass('is-valid')
    ) {
        $('button[id=zonesubmitBtn]').prop('disabled', false);
    }
    else {
        $('button[id=zonesubmitBtn]').prop('disabled', true);
    }
}
function enableCameraSubmit() {
    if ($('select[name=cameraLocation]').hasClass('is-valid') &&
        $('input[type=text][name=zone_name]').hasClass('is-valid') &&
        $('select[name=zone_type]').hasClass('is-valid')
    ) {
        $('button[id=zonesubmitBtn]').prop('disabled', false);
    }
    else {
        $('button[id=zonesubmitBtn]').prop('disabled', true);
    }
}
function enableZoneSubmit() {
    if ($('select[name=zone_type]').hasClass('is-valid') &&
        $('input[type=text][name=zone_name]').hasClass('is-valid')
    ) {
        $('button[id=zonesubmitBtn]').prop('disabled', false);
    }
    else {
        $('button[id=zonesubmitBtn]').prop('disabled', true);
    }
}
function CreateZone(newlayer)
{
    try {
        map.setView(newlayer.layer._bounds.getCenter());
        VaildateForm("");
        sidebar.open('home');
        var togeo = newlayer.layer.toGeoJSON();
        var geoProp = {
            Zone_Type: "",
            name: "",
            visible: true
        }
        $('input[id=zone_name]').css('display', 'block');
        $('select[id=zone_select_name]').css('display', 'none');
        $('select[name=zone_type]').change(function () {
            if ($('select[name=zone_type] option').filter(':selected').val() === "Machine")
            {
                fotfmanager.server.getMPEList().done(function (mpedata) {
                    if (mpedata.length > 0) {
                        //sort 
                        mpedata.sort(SortByName);
                        $('input[id=zone_name]').css('display', 'none');
                        $('select[id=zone_select_name]').css('display', 'block');
                        $('<option/>').empty();
                        $('<option/>').val("");
                        $.each(mpedata, function () {
                            $('<option/>').val(this).html(this).appendTo('#zone_select_name');
                        })
                    }
                });
            }
            if ($('select[name=zone_type] option').filter(':selected').val() === "DockDoor") {
                fotfmanager.server.getDockDoorList().done(function (DockDoordata) {
                    if (DockDoordata.length > 0) {
                        //sort 
                        DockDoordata.sort(SortByNumber);
                        $('input[id=zone_name]').css('display', 'none');
                        $('select[id=zone_select_name]').css('display', 'block');
                        $('<option/>').empty();
                        $('<option/>').val("");
                        $.each(DockDoordata, function () {
                            $('<option/>').val(this).html(this).appendTo('#zone_select_name');
                        })
                    }
                });
            }
            $('input[id="zone_name"]').val("");
            $('input[id=zone_name]').css('display', 'block');
            $('select[id=zone_select_name]').css('display', 'none');
        });

        $('button[id=zonesubmitBtn][type=button]').off().on('click', function () {
            togeo.properties = geoProp;
            togeo.properties.Zone_Type = $('select[name=zone_type] option:selected').val();
            togeo.properties.name = $('input[id="zone_name"]').is(':visible') ? $('input[id="zone_name"]').val() : $('select[name=zone_select_name] option:selected').val();

            $.connection.FOTFManager.server.addZone(JSON.stringify(togeo)).done(function (Data) {
                if (!$.isEmptyObject(Data)) {
                    setTimeout(function () { sidebar.close('home'); }, 500);
                    newlayer.layer.remove();
                }
                else {
                    newlayer.layer.remove();
                }
            });
        });

    } catch (e) {
        console.log(e);
    }
}
function CreateBinZone(newlayer) {
    try {
        map.setView(newlayer.layer._bounds.getCenter());
        VaildateForm("Bin");
        sidebar.open('home');
        var togeo = newlayer.layer.toGeoJSON();
        var geoProp = {
            Zone_Type: "",
            name: "",
            bins: "",
            visible: true
        }
        $('input[id=zone_name]').css('display', 'block');
        $('select[id=zone_select_name]').css('display', 'none');
        fotfmanager.server.getMPEList().done(function (mpedata) {
            if (mpedata.length > 0) {
                //sort 
                mpedata.sort(SortByName);
                $('input[id=zone_name]').css('display', 'none');
                $('select[id=zone_select_name]').css('display', 'block');
                $('<option/>').empty();
                $('<option/>').val("");
                $.each(mpedata, function () {
                    $('<option/>').val(this).html(this).appendTo('#zone_select_name');
                })
            }
        });

        $('button[id=zonesubmitBtn][type=button]').off().on('click', function () {
            togeo.properties = geoProp;
            togeo.properties.Zone_Type = $('select[name=zone_type] option:selected').val();
            togeo.properties.name = $('input[id="zone_name"]').is(':visible') ? $('input[id="zone_name"]').val() : $('select[name=zone_select_name] option:selected').val() ;
            togeo.properties.bins = $('textarea[id="bin_bins"]').val();

            $.connection.FOTFManager.server.addZone(JSON.stringify(togeo)).done(function (Data) {
                if (!$.isEmptyObject(Data)) {
                    setTimeout(function () { sidebar.close('home'); }, 1000);
                    newlayer.layer.remove();
                    updateBinZone(Data);
                }
                else {
                    newlayer.layer.remove();
                }
            });
        });

    } catch (e) {
        console.log(e);
    }

}
function CreateCamera(newlayer)
{
    VaildateForm("Camera");
    sidebar.open('home');
    var togeo = newlayer.layer.toGeoJSON();
    map.setView(newlayer.layer._latlng, 3);
    var geoProp = {
        name: "",
        Tag_Type: "",
        visible: true
    }
    $('input[id=zone_name]').css('display', 'block');
    $('select[id=zone_select_name]').css('display', 'none');
    fotfmanager.server.getCameraList().done(function (cameradata) {
        if (cameradata.length > 0) {
            $('<option/>').empty();
            $.each(cameradata, function () {
                $('<option/>').val(this.CAMERA_NAME).html(this.CAMERA_NAME + "/"+ this.DESCRIPTION).appendTo('#cameraLocation');
            })
        }
    });
    $('button[id=zonesubmitBtn][type=button]').off().on('click', function () {
        togeo.properties = geoProp;
        togeo.properties.name = $('input[id="zone_name"]').val();
        togeo.properties.Tag_Type = $('select[name=zone_type] option:selected').val();
        $.connection.FOTFManager.server.addMarker(JSON.stringify(togeo)).done(function (Data) {
            if (!$.isEmptyObject(Data)) {
                setTimeout(function () { sidebar.close('home'); }, 500);
                cameras.addData(Data);
                newlayer.layer.remove();
            }
            else {
                newlayer.layer.remove();
            }
        });
    });

}
function RemoveZoneItem(removeLayer)
{
    try {
        var layerId = removeLayer.layer.feature.properties.id;
        sidebar.close();
        fotfmanager.server.removeZone(layerId).done(function (Data) {
            if (!$.isEmptyObject(Data)) {
                setTimeout(function () { $("#Remove_Layer_Modal").modal('hide'); }, 500);
                if (Data.properties.Zone_Type === "DockDoor") {
                    removeDockDoor(removeLayer.layer._leaflet_id);
                }
                
            }
        });

    } catch (e) {
        console.log();
    }
}
function RemoveMarkerItem(removeLayer) {
    try {
        var layerId = removeLayer.layer.feature.properties.id;
        sidebar.close();
        fotfmanager.server.removeMarker(layerId).done(function (Data) {
            if (!$.isEmptyObject(Data)) {
                setTimeout(function () { $("#Remove_Layer_Modal").modal('hide'); }, 500);
                removeFromMapView(removeLayer.layer.id);
            }
        });

    } catch (e) {
        console.log();
    }
}
function removeFromMapView(id)
{
    $.map(map._layers, function (layer, i) {
        if (layer.hasOwnProperty("feature")) {
            if (layer._leaflet_id === id) {
                map.removeLayer(layer)
            }
        }
    });
}
function VaildateForm(FormType)
{
    $('select[name=zone_type]').prop('disabled', false);
    $('input[name=zone_name]').prop('disabled', false);
    $('input[name=zone_name]').val("");
    $('select[name=zone_type]').val("");
    $('#camerainfo').css("display", "none");
    $('#binzoneinfo').css("display", "none");
    if (!checkValue($('select[name=zone_type]').val())) {
        $('select[name=zone_type]').removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_zone_type]').text("Please Select Zone Type");
    }
    else {
        $('select[name=zone_type]').removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_zone_type]').text("");
    }
    //zone_name
    if (!checkValue($('input[type=text][name=zone_name]').val())) {
        $('input[type=text][name=zone_name]').removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_zone_name]').text("Please Enter Name");
    }
    else {
        $('input[type=text][name=zone_name]').removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_zone_name]').text("");
    }
    if (/bin/i.test(FormType)) {
        $('#camerainfo').css("display", "none");
        $('#binzoneinfo').css("display", "block");
        $('select[name=zone_type]').val("Bin");
        $('select[name=zone_type]').prop('disabled', true);
        $('input[name=zone_name]').prop('disabled', false);
        $('input[name=zone_name]').val("");
        $('textarea[id="bin_bins"]').val("");
        
        if (!checkValue($('textarea[id=bin_bins]').val())) {
            $('textarea[id=bin_bins]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_bin_bins]').text("Please Bin Numbers");
        }
        else {
            $('textarea[id=bin_bins]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_bin_bins]').text("");
        }
        if (!checkValue($('select[name=zone_type]').val())) {
            $('select[name=zone_type]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_zone_type]').text("Please Select Zone Type");
        }
        else {
            $('select[name=zone_type]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_zone_type]').text("");

        }
        enableBinZoneSubmit();
    }
    if (/camera/i.test(FormType)) {
        $('#camerainfo').css("display", "block");
        $('#binzoneinfo').css("display", "none");
        $('select[name=cameraLocation]').val("");
        $('select[name=zone_type]').val("Camera");
        $('select[name=zone_type]').prop('disabled', true);
        $('input[name=zone_name]').prop('disabled', true);
        $('input[name=zone_name]').val("");

        $('input[type=text][name=zone_name]').removeClass('is-invalid').removeClass('is-valid');
        $('span[id=error_zone_name]').text("");
        //Camera URL
        if ($('select[name=cameraLocation]').val() === "") {
            $('select[name=cameraLocation]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_cameraLocation]').text("Please Select Camera URL");
        }
        else {
            $('input[type=text][name=cameraLocation]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_cameraLocation]').text("");
        }
        if (!checkValue($('select[name=zone_type]').val())) {
            $('select[name=zone_type]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_zone_type]').text("Please Select Zone Type");
        }
        else {
            $('select[name=zone_type]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_zone_type]').text("");

        }
        enableCameraSubmit();
    }
}
