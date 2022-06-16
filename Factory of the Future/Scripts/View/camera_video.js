

/**
* this is use to setup a the camera information and other function
*
* **/
$.extend(fotfmanager.client, {
    updateCameraStatus: async (cameraupdate, id) => { updateCameras(cameraupdate,id) }
});

async function updateCameras(cameraupdates, id) {
    try {
        if (id == baselayerid) {
            if (cameras.hasOwnProperty("_layers")) {
                $.map(cameras._layers, function (layer) {
                    if (layer.hasOwnProperty("feature")) {
                        if (layer.feature.properties.id === cameraupdates.properties.id) {
                            var locaterIcon = L.icon({
                                iconUrl: cameraupdates.properties.base64Image === "" ? "../../Content/images/NoImage.png" : cameraupdates.properties.base64Image,

                                iconSize: [64, 48], // size of the icon
                                iconAnchor: [0, 0], // point of the icon which will correspond to marker's location
                                shadowAnchor: [0, 0],  // the same for the shadow
                                popupAnchor: [0, 0] // point from which the popup should open relative to the iconAnchor
                            });
                            layer.setIcon(locaterIcon);
                        }
                    }
                });
            }
        }

    } catch (e) {
        console.log(e);
    }
}
//on close clear all inputs
$('#Camera_Modal').on('hidden.bs.modal', function () {
    camera_modal_body = $('div[id=camera_modalbody]');
    camera_modal_body.empty();
});
var cameras = new L.GeoJSON(null, {
    
    pointToLayer: function (feature, latlng) {
        var locaterIcon = L.icon({
            iconUrl: feature.properties.base64Image === "" ? "../../Content/images/NoImage.png" : feature.properties.base64Image,
            iconSize: [64, 48], // size of the icon
            iconAnchor: [0, 0], // point of the icon which will correspond to marker's location
            shadowAnchor: [0, 0],  // the same for the shadow
            popupAnchor: [0, 0] // point from which the popup should open relative to the iconAnchor
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
        var cameraname = checkValue(feature.properties.empName) ? feature.properties.empName : feature.properties.name;
        layer.on('click', function (e) {
            View_Web_Camera(feature.properties);
        });

        //var CameraFeedInterval = 120
        //layer.bindPopup(camera_layout.supplant(formatwebcameralayout(feature.properties.name, feature.properties.emptype, feature.properties.empName)), {
        //    className:'popupCustom'
        //});
        layer.bindTooltip(cameraname, {
            permanent: true,
            direction: 'top',
            opacity: 1,
            className: 'location'
        }).openTooltip();
    }
})

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