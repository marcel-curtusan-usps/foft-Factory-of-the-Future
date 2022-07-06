

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
                            var base64Image = (cameraupdates.properties.base64Image ===
                                "" ? "../../Content/images/NoImage.png" : cameraupdates.properties.base64Image);
                            
                            if (layer.feature.properties.DarvisAlert &&
                                layer.feature.properties.DarvisAlert.TYPE) {

                                highlightCameraAlert(base64Image, 255, 0, 0, 16).then((img) => {

                                    var locaterIcon = L.icon({
                                        iconUrl: img,
                                        iconSize: [64, 48], // size of the icon
                                        iconAnchor: [0, 0], // point of the icon which will correspond to marker's location
                                        shadowAnchor: [0, 0],  // the same for the shadow
                                        popupAnchor: [0, 0] // point from which the popup should open relative to the iconAnchor

                                    });
                                    layer.setIcon(locaterIcon);
                                }).catch((err) => {
                                    console.log(err.message);
                                });
                            }
                            else {
                                var locaterIcon = L.icon({
                                    iconUrl: base64Image,
                                    iconSize: [64, 48], // size of the icon
                                    iconAnchor: [0, 0], // point of the icon which will correspond to marker's location
                                    shadowAnchor: [0, 0],  // the same for the shadow
                                    popupAnchor: [0, 0] // point from which the popup should open relative to the iconAnchor

                                });
                                layer.setIcon(locaterIcon);
                            }
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

// used to get alert boundaries
var getAlertBoundingBox = async (Data, width, height) => {
    
    return new Promise((resolve, reject) => {

        
            var canvas = document.createElement('canvas');
            canvas.width = width;
            canvas.height = height;

        var context = canvas.getContext('2d');

        // to be completed...

        //context.strokeStyle = "red";
        //context.lineWidth = 5;
           // context.beginPath();
          //  context.rect(0, 0, width, height);
          //  context.stroke();

            var imageData = context.getImageData(0, 0, canvas.width, canvas.height);


            context.putImageData(imageData, 0, 0);
            resolve(canvas.toDataURL());
        
    });
}
var highlightCameraAlert = async (base64Image, r, g, b, borderWidth) => {
    return new Promise((resolve, reject) => {


        var image = new Image();
        image.onerror = function () {
            reject("failed to load image");
        }
        image.onload = function () {
            var canvas = document.createElement('canvas');
            canvas.width = image.width;
            canvas.height = image.height;

            var context = canvas.getContext('2d');
            context.drawImage(image, 0, 0);

            var imageData = context.getImageData(0, 0, canvas.width, canvas.height);

            var x = 0;
            var y = 0;
            var index = 0;
          
            for (x = 0; x < imageData.width; x++) {
                
                for (y = 0; y < imageData.height; y++) {
                    if (x < borderWidth || x >= imageData.width - borderWidth
                        || y < borderWidth || y >= imageData.height - borderWidth) {
                        index = (y * imageData.width + x) * 4;
                        imageData.data[index] = r;
                        imageData.data[index + 1] = g;
                        imageData.data[index + 2] = b;
                    }
                }
            }
            context.putImageData(imageData, 0, 0);
            resolve(canvas.toDataURL());
        };
        image.src = base64Image;
    });
}
var cameras = new L.GeoJSON(null, {

    pointToLayer: function (feature, latlng) {

        var locaterIcon = null;
        if (feature.properties.Camera_Data &&
            feature.properties.Camera_Data.CAMERA_ALERT &&
            feature.properties.Camera_Data.CAMERA_ALERT.TYPE) {

            locaterIcon = L.icon({
                iconUrl: feature.properties.base64Image === "" ? "../../Content/images/NoImage.png" : feature.properties.base64Image,
                iconSize: [64, 48], // size of the icon
                iconAnchor: [0, 0], // point of the icon which will correspond to marker's location
                shadowAnchor: [0, 0],  // the same for the shadow
                popupAnchor: [0, 0], // point from which the popup should open relative to the iconAnchor
                fillColor: '#ff0000',   // Red. 
                fillOpacity: 0.3
            });
        }
        else {
            locaterIcon = L.icon({
                iconUrl: feature.properties.base64Image === "" ? "../../Content/images/NoImage.png" : feature.properties.base64Image,
                iconSize: [64, 48], // size of the icon
                iconAnchor: [0, 0], // point of the icon which will correspond to marker's location
                shadowAnchor: [0, 0],  // the same for the shadow
                popupAnchor: [0, 0] // point from which the popup should open relative to the iconAnchor
               
            });
        }
       

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
            interactive: true,
            direction: 'top',
            opacity: 1,
            className: 'location'
        }).openTooltip();
    }
})




function formatCameralayout(camera) {
    return $.extend(camera, {
        base64Background: camera.base64Image,
        camera_ip: camera.CAMERA_NAME,
        camera_description: camera.DESCRIPTION,
        camera_model: getModel(camera.MODEL_NUM)
    });
}



let imageWidth = 1020;
let imageHeight = 700;
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
function formatwebcameralayout(id, model, description, base64Image) {
    return $.extend(id, model, description, {
        base64Background: base64Image,
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
        
        getAlertBoundingBox(Data, imageWidth, imageHeight).then((img) => {
            
            camera_modal_body.append(camera_layout.supplant(formatwebcameralayout(Data.name, Data.emptype, Data.empName,
                img)));
            $('#Camera_Modal').modal();
            sidebar.close('');
        });
        
    } catch (e) {
        $("#error_camera").text(e);
        console.log(e);
    }
}