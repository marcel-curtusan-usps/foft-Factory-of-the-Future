/**
* this is use to setup a the camera information and other function
*
* **/
$.extend(fotfmanager.client, {
    updateCameraStatus: async (cameraupdatesnew, id) => { CameraDataUpdate(cameraupdatesnew, id); },
    addCamera: async (cameraupdatesnew, id) => { AddMarker(cameraupdatesnew, id); },
    removeCamera: async (cameraupdatesnew, id) => { RemoveMarker(cameraupdatesnew, id); }
});
let cameraList = {};
async function init_Cameradatalocators(marker, id) {
    $.each(marker, function (_index, data) {
        Promise.all([AddMarker(data, data.properties.floorId)]);
    });
    fotfmanager.server.joinGroup("CameraMarkers");
}
async function AddMarker(data, floorId) {
    try {
        if (floorId === baselayerid) {
            cameras.addData(data);
        }
    } catch (e) {
     
    }
}
async function RemoveMarker(id, floorId) {
    try {
        if (floorId === baselayerid) {
            let delId = cameraList[id];
            if (typeof delId !== 'undefined') {
                cameras.removeLayer(delId);
            }
        }
    } catch (e) {

    }
}
let allcameraslist = [];

function CameraDataUpdate(cameradataUpdate, id) {
    try {
        if (id === baselayerid) {
            map.whenReady(() => {
                if (map.hasOwnProperty("_layers")) {
                    $.map(map._layers, function (layer, i) {
           
                    });
                }
            });
        }
    } catch (e) {
   
    }
}
$('#Camera_Modal').on('hidden.bs.modal', function () {
    camera_modal_body = $('div[id=camera_modalbody]');
    camera_modal_body.empty();
});

let cameras = new L.GeoJSON(null, {
    pointToLayer: function (feature, latlng) {
        let icon = L.divIcon({
            className: 'custom-div-icon',
            html: "<div style='background-color:#c30b82;' class='marker-pin' ></div> <i class='bi-camera-fill'></i>",
            iconSize: [30, 42],
            iconAnchor: [15, 42]
        });
        return L.marker(latlng, {
            icon: icon,
            riseOnHover: true,
            bubblingMouseEvents: true,
            popupOpen: true
        });
    },
    onEachFeature: function (feature, layer) {
        cameraList[feature.properties.id] = layer;
        layer.on('click', function (e) {
            View_Web_Camera(feature.properties);
        });
        layer.bindTooltip("", {
            permanent: true,
            interactive: true,
            direction: 'top',
            opacity: 1,
            className: 'location'
        }).openTooltip();
    },
    filter: function (feature, layer) {
        return feature.properties.Tag_Type === "CameraMarker" ? true : false;
    
    }
})

let imageWidth = 1280;
let imageHeight = 720;
let scaleWidth = imageWidth / 1920;
let scaleHeight = imageHeight / 1080;
let extraOffset = 5;
let verticalOffset = (-  (imageHeight + extraOffset)) + "px";
let camera_Table , camera_modal_body;
let camera_layout = '<div class="frameFlex">' +
    '<div style="overflow: scroll">' +
    '<div style="width: ' + imageWidth + '; height: ' + imageHeight + '; ">' +
    '<iframe id="cameraIframe" src="http://{camera_ip}/mjpg/video.mjpg?camera={camera_model}"' +
    ' scrolling="no" width= ' + imageWidth + ' height= ' + imageHeight + '>' +
    '</iframe> ' +
    '<div style="z-index: 10000000; margin-top: ' + verticalOffset + ' ">' +
    '<img id="openCameraOverlay" src={base64Background} width=' +
    imageWidth + ' height=' + imageHeight + 
    ' style="width: ' + imageWidth + ' height: ' + imageHeight + ' "/>' +
    '</div></div></div>';
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
var webCameraViewData = null;

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

    }
}