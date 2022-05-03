

//on close clear all inputs
$('#Camera_Modal').on('hidden.bs.modal', function () {
    camera_modal_body = $('div[id=camera_modalbody]');
    camera_modal_body.empty();
});
var cameras = new L.GeoJSON(null, {
    pointToLayer: function (feature, latlng) {
        var locaterIcon = L.divIcon({
            id: feature.properties.id,
            className: 'bi-camera-fill',

        });
        return L.marker(latlng, {
            icon: locaterIcon,
            title: feature.properties.empName,
            riseOnHover: true,
            bubblingMouseEvents: true,
            popupOpen: true
        })
    },
    onEachFeature: function (feature, layer) {
        layer.on('click', function (e) {
            View_Web_Camera(feature.properties);
        });
        var cameraname = checkValue(feature.properties.empName) ? feature.properties.empName : feature.properties.name;
        layer.bindTooltip(cameraname, {
            permanent: true,
            direction: 'top',
            opacity: 1,
            className: 'location'
        }).openTooltip();
    }
})
async function init_cameras() {
    try {
        if (!/^https/i.test(window.location.protocol)) {
            fotfmanager.server.getCameraMarkerList().done(function (cameradata) {
                if (cameradata.length > 0) {
                    cameras.addData(cameradata);
                }
            });
        }
	} catch (e) {
        console.log(e);
    }
}
function formatCameralayout(camera) {
    return $.extend(camera, {
        camera_ip: camera.CAMERA_NAME,
        camera_description: camera.DESCRIPTION,
        camera_model: getModel(camera.MODEL_NUM)
    });
}
let camera_Table , camera_modal_body;
let camera_layout = '<div class="frameFlex">' +
    '<div class="hide">' +
    '<iframe src="http://{camera_ip}/mjpg/video.mjpg?camera={camera_model}" style="width:91.3%; height: 45.3rem;">' +
    '</iframe>' +
    '</div>' +
    '</div>';
let camera_row_template = '<tr data-id="{camera_ip}" data-model="{camera_model}" data-description="{camera_description}" id="_{camera_ip}">' +
    '<td class="align-middle">{camera_ip}</td>' +
    '<td class="align-middle">{camera_description} </td>' +
    '<td class="d-flex align-middle justify-content-center">' +
    '<button class="btn btn-light btn-sm mx-1 bi-camera-fill camera_view"></button>' +
    '</td>' +
    '</tr>';

function getModel(MODEL_NUM) {
    let model = 1;
    switch (MODEL_NUM) {
        case 3719:
            model = 'quad'
            break
        case 3727:
            model = '5'
            break
        default:
            model =  '1'
            break
    }
    return model;
}
function formatwebcameralayout(id, model, description) {
    return $.extend(id, model, description, {
        camera_ip: id,
        camera_description: description,
        camera_model: getModel(model)
    });
}
function View_Web_Camera(Data) {
    try {
        $('#cameramodalHeader').text('View Web Camera');
        var cameraname = checkValue(Data.empName) ? Data.empName : Data.name;
        $('#cameradescription').text(cameraname);
        camera_modal_body = $('div[id=camera_modalbody]');
        camera_modal_body.empty();
        camera_modal_body.append(camera_layout.supplant(formatwebcameralayout(Data.name, Data.emptype, Data.empName)));
        $('#Camera_Modal').modal();
        sidebar.close('');
    } catch (e) {
        $("#error_camera").text(e);
        console.log(e);
    }
}
