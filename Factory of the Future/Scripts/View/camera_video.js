/**
* this is use to setup a the camera information and other function
*
* **/
$.extend(fotfmanager.client, {
    updateCameraStatus: async (cameraupdatesnew, id) => { CameraDataUpdate(cameraupdatesnew, id);  }
});
var brightRed = 255;
var darkRed = 200;
function drawAlertText(ctx, txt, font, viewWidth, viewHeight, x, y, txtBGColor, txtColor, bgHeight) {
    let padding = 4;
    y = y - 10 - bgHeight;
    ctx.save();
    ctx.font = font;
    /// draw text from top
    ctx.textBaseline = 'top';
    ctx.fillStyle = txtBGColor;
    var width = ctx.measureText(txt).width;
    var totalWidth = width + (padding * 2);
    var totalHeight = bgHeight + (padding * 2);
    if (x < 5) {
        x = 5;
    }
    if (x > (viewWidth - totalWidth - 5)) {
        x = viewWidth - totalWidth - 5;
    }
    if (y < 35) {
        y = 35;
    }
    if (y > (viewHeight - totalHeight - 5)) {
        y = viewHeight - totalHeight - 5;
    }
    ctx.fillRect(x - padding, y - padding, totalWidth, totalHeight);
    ctx.fillStyle = txtColor;
    ctx.fillText(txt, x, y);
}
let cameraupdates = [];
let allcameras = [];
let flashRate = 3000;
let flashCheckRate = 250;
let lastAlertBlinkChange = 0;
let lastAlertStatus = null;
let alertTurnoffThreshold = 60 * 1000;
let alertsOn = false; 
let camerathumbnailsupdating = false;
setInterval(() => {
    let thisTime = Date.now();
    let timeSinceLastUpdateBlink = thisTime - lastAlertBlinkChange;
    if (timeSinceLastUpdateBlink >= (flashRate - flashCheckRate)) {
        // keep waiting until update is done
        if (camerathumbnailsupdating) return;
        camerathumbnailsupdating = true;
        let timeSinceLastUpdate = thisTime - lastUpdateCamera;
        if (timeSinceLastUpdate >= alertTurnoffThreshold) {
            alertsOn = false;
        }
        else {
            alertsOn = true;
        }
        updateAllCameras(thisTime);
        camerathumbnailsupdating = false;
    }
}, flashCheckRate);
let lastUpdateCamera = 0;
function alertChanged(prevDarvisAlerts, darvisAlerts) {
    if (prevDarvisAlerts && !darvisAlerts) return true;
    if (!prevDarvisAlerts && darvisAlerts) return true;
    return JSON.stringify(prevDarvisAlerts) != JSON.stringify(darvisAlerts);
}
function CameraDataUpdate(cameradataUpdate, id) {
    try {
        if (id == baselayerid) {
            map.whenReady(() => {
                if (map.hasOwnProperty("_layers")) {
                    $.map(map._layers, function (layer, i) {
                        if (layer.hasOwnProperty("feature") && layer.feature.properties.id === cameradataUpdate.properties.id) {
                            let img = new Image();
                            //load Base64 image
                            img.src = cameradataUpdate.properties.Camera_Data.base64Image;
                            var mapsize = map.getZoom();
                            var iconsizeh = 64;
                            var iconsizew = 48
                            if (mapsize > 2) {
                                iconsizeh = 64 * mapsize;
                                iconsizew = 48 * mapsize;
                            }
                            let thumbnailsReplc = L.icon({
                                    iconUrl: img.src,
                                    iconSize: [iconsizeh, iconsizew], // size of the icon
                                    iconAnchor: [0, 0], // point of the icon which will correspond to marker's location
                                    shadowAnchor: [0, 0],  // the same for the shadow
                                    popupAnchor: [0, 0] // point from which the popup should open relative to the iconAnchor
                            });
                            layer.setIcon(thumbnailsReplc);
                            return false;
                        }
                    });
                }
            });
        }
    } catch (e) {
        console.log(e);
    }
}
function addCameraUpdate(allcameras) {
    if (camerathumbnailsupdating) {
        // if the thumbnails are updating, ignore  this update to avoid flicker,
        // next update will reflect the data so it's okay
        return;
    }
    let cameraupdatescopy = JSON.parse(JSON.stringify(cameraupdates));
    for (const tuple of allcameras) {
        let cameraupdate = tuple.Item1;
        let id = tuple.Item2;
        var changed = false;
            try {
                cameraupdate.baselayerid = id;
                var foundIndex = -1;
                for (var i = 0; i < cameraupdatescopy.length; i++) {
                    if (cameraupdatescopy[i].properties.id === cameraupdate.properties.id) {
                        foundIndex = i;
                        break;
                    }
                }
                if (foundIndex == -1) {
                    cameraupdate.properties.lastNoAlert = 0;
                    cameraupdatescopy.push(cameraupdate);
                    changed = true;
                }
                else {
                    cameraupdate.properties.lastNoAlert = cameraupdatescopy[foundIndex].properties.lastNoAlert;
                    let compare1 = JSON.parse(JSON.stringify(cameraupdatescopy[foundIndex].properties.DarvisAlerts));
                    cameraupdatescopy[foundIndex] = cameraupdate;
                    changed = alertChanged(compare1, cameraupdate.properties.DarvisAlerts);
                }
            }
            catch (e) {
                console.log(e.message);
        }
        if (changed) {

            lastUpdateCamera = Date.now();
        }
    }
    cameraupdates = cameraupdatescopy;
}
async function getLayersAndIcons(datePassed) {
        
        let layersAndIconsUpdate = [];
        if (cameraupdates.length > 0) {
            for (var camera of cameraupdates) {

                try {
                    var layerAndIcon = await updateCameras(camera, camera.baselayerid, datePassed);
                    if (layerAndIcon !== null) {
                        layersAndIconsUpdate.push(layerAndIcon);
                    }
                }
                catch (e) {
                    console.log(e.message);
                }
            }
        }
        return layersAndIconsUpdate;
       
}
function updateAllCameras(datePassed) {
    getLayersAndIcons(datePassed).then((layersAndIconsUpdate) => {

        for (const layerAndIconToUpdate of layersAndIconsUpdate) {

            let layer = layerAndIconToUpdate[0];
            let icon = layerAndIconToUpdate[1];
            layer.setIcon(icon);
           
        }
    }).catch((e) => {
        console.log(e.message);
    });
}
async function updateCameras(cameraupdate, id, datePassed) {
    return new Promise((resolve, reject) => {
        try {
            if (id == baselayerid) {
                if (cameras.hasOwnProperty("_layers")) {
                    $.map(cameras._layers, async function (layer) {
                        if (layer.hasOwnProperty("feature")) {
                            if (layer.feature.properties.id === cameraupdate.properties.id) {

                                if (cameraupdate.properties.id === openCameraId) {
                                    webCameraViewData = JSON.parse(JSON.stringify(cameraupdate.properties));
                                }
                                if (alertsOn && cameraupdate.properties.DarvisAlerts &&
                                    cameraupdate.properties.DarvisAlerts.length > 0) {
                                    let red = brightRed;
                                    if (datePassed % (flashRate * 2) > flashRate) {
                                        red = darkRed;
                                        if (lastAlertStatus === 1) {
                                            lastAlertBlinkChange = datePassed;
                                        }
                                        lastAlertStatus = 0;
                                    }
                                    else {
                                        if (lastAlertStatus === 0) {
                                            lastAlertBlinkChange = datePassed;
                                        }
                                        lastAlertStatus = 1;
                                    }
                                    var base64Image = (cameraupdate.properties.base64Image ===
                                        "" ? "../../Content/images/NoImage.png" :
                                        cameraupdate.properties.base64Image);

                                    let img = await highlightCameraAlert(base64Image, red, 0, 0, 20);
                                    var mapsize = map.getZoom();
                                    var iconsizeh = 64;
                                    var iconsizew = 48
                                    if (mapsize > 2) {
                                        iconsizeh = 64 * mapsize;
                                        iconsizew = 48 * mapsize;
                                    }
                                    var locaterIcon = L.icon({
                                        iconUrl: img,
                                        iconSize: [iconsizeh, iconsizew], // size of the icon
                                        iconAnchor: [0, 0], // point of the icon which will correspond to marker's location
                                        shadowAnchor: [0, 0],  // the same for the shadow
                                        popupAnchor: [0, 0] // point from which the popup should open relative to the iconAnchor

                                    });
                                    resolve([layer, locaterIcon]);
                                }
                                else {
                                    var base64Image = (cameraupdate.properties.base64Image ===
                                        "" ? "../../Content/images/NoImage.png" : cameraupdate.properties.base64Image);
                                    var mapsize = map.getZoom();
                                    var iconsizeh = 64;
                                    var iconsizew = 48
                                    if (mapsize > 2) {
                                        iconsizeh = 64 * mapsize;
                                        iconsizew = 48 * mapsize;
                                    }
                                    var locaterIcon = L.icon({
                                        iconUrl: base64Image,
                                        iconSize: [iconsizeh, iconsizew], // size of the icon
                                        iconAnchor: [0, 0], // point of the icon which will correspond to marker's location
                                        shadowAnchor: [0, 0],  // the same for the shadow
                                        popupAnchor: [0, 0] // point from which the popup should open relative to the iconAnchor
                                    });
                                    resolve([layer, locaterIcon]);
                                }
                            }
                        }
                    });
                }
            }
            else {
                resolve(null);
            }

        } catch (e) {
            reject(e);
        }
    });
}
//on close clear all inputs
$('#Camera_Modal').on('hidden.bs.modal', function () {
    camera_modal_body = $('div[id=camera_modalbody]');
    camera_modal_body.empty();
});
// used to get alert boundaries
var getAlertBoundingBox = async (Alerts, width, height) => {
    return new Promise((resolve, reject) => {
            var canvas = document.createElement('canvas');
            canvas.width = width;
            canvas.height = height;
        var context = canvas.getContext('2d');
        if (alertsOn && Alerts && Alerts.length > 0) {
            var alert = null;
            for (alert of Alerts) {
                let left = alert.TOP * scaleWidth;
                let right = alert.BOTTOM * scaleWidth;
                let top = alert.LEFT * scaleHeight;
                let bottom = alert.RIGHT * scaleHeight;
                context.strokeStyle = "red";
                context.lineWidth = 5;
                context.beginPath();
                context.rect(left,  top,
                    (right - left), (bottom - top));
                context.stroke();
            }
            for (alert of Alerts) {
                let left = alert.TOP * scaleWidth;
                let right = alert.BOTTOM * scaleWidth;
                let top = alert.LEFT * scaleHeight;
                let bottom = alert.RIGHT * scaleHeight;
                let text = alert.TYPE + ": " + getHHMMSSFromSeconds(alert.DWELL_TIME);

                var posX = left;
                var posY = top;
                drawAlertText(context, text, "bold 18px Arial", width, height, posX,
                    posY, "#fff", "#000", 18, 4);
            }
        }
            resolve(canvas.toDataURL());
    });
}

var exclamation = new Image();
var exclamationImage = null;
var exclamationSize = 100;
var brightRedExclamation = new Image();
var darkRedExclamation = new Image();
var brightRedLoaded = false;
var darkRedLoaded = false;
brightRedExclamation.onload = function () {
    brightRedLoaded = true;
}
darkRedExclamation.onload = function () {
    darkRedLoaded = true;
}
exclamation.onload = function () {
    for (var i = 0; i <= 1; i++) {
        let red = brightRed;
        if (i === 1) {
            red = darkRed;
        }
    let canvas = document.createElement('canvas');
    canvas.width = exclamationSize;
        canvas.height = exclamationSize;

        let toCanvas = document.createElement('canvas');
        toCanvas.width = exclamationSize;
        toCanvas.height = exclamationSize;
        var context = canvas.getContext('2d');
        var toContext = toCanvas.getContext('2d');
        context.drawImage(exclamation, 0, 0, exclamationSize, exclamationSize);
    var imgData = context.getImageData(0, 0, exclamationSize, exclamationSize);
        var newImgData = toContext.getImageData(0, 0, exclamationSize, exclamationSize);
    for (var x = 0; x < exclamationSize; x++) {
        for (var y = 0; y < exclamationSize; y++) {
            var pixelStartPosition = (y * exclamationSize * 4) + (x * 4);
            if (imgData.data[pixelStartPosition]
                < 255 && imgData.data[pixelStartPosition + 3] > 0) {
                newImgData.data[pixelStartPosition] = red;
                newImgData.data[pixelStartPosition + 1] = 0;
                newImgData.data[pixelStartPosition + 2] = 0;
                newImgData.data[pixelStartPosition + 3] = 255;
            }
            else {
                newImgData.data[pixelStartPosition] = imgData.data[pixelStartPosition];
                newImgData.data[pixelStartPosition + 1] = imgData.data[pixelStartPosition + 1];
                newImgData.data[pixelStartPosition + 2] = imgData.data[pixelStartPosition + 2];
                newImgData.data[pixelStartPosition + 3] = imgData.data[pixelStartPosition + 3];
            }
 }
        }
        toContext.putImageData(newImgData, 0, 0);
        if (red == brightRed) {
            brightRedExclamation.src = toCanvas.toDataURL();
        }
        else {
            darkRedExclamation.src = toCanvas.toDataURL();
        }
    }
    var done = true;
}
exclamation.src = "../../Content/images/warning-signal.png";
var imageManipCanvas = document.createElement('canvas');
var highlightCameraAlert = async (base64Image, r, g, b, borderWidth) => {
    return new Promise((resolve, reject) => {
        var image = new Image();
        image.onerror = function () {
            reject("failed to load image");
        }
        image.onload = function () {
            let canvas = imageManipCanvas;
            canvas.width = image.width;
            canvas.height = image.height;
            var context = canvas.getContext('2d');
            context.drawImage(image, 0, 0);
            var alertColor = "rgb(" + r + ", " + g + ", " + b + ")";
            context.strokeStyle = alertColor;
            context.lineWidth = borderWidth;
            var warningHeight = "64px";
            context.strokeRect(borderWidth / 2, borderWidth / 2, image.width - 1 - borderWidth,
                image.height - 1 - borderWidth);
            if (r === brightRed && brightRedLoaded) {
                context.drawImage(brightRedExclamation, (image.width * .667) -
                    (exclamationSize / 2), (image.height * .333) - (exclamationSize / 2));
            }
            else if (darkRedLoaded) {
                context.drawImage(darkRedExclamation, (image.width * .667) -
                    (exclamationSize / 2), (image.height * .333) - (exclamationSize / 2));
            }
            resolve(canvas.toDataURL());
        };
        image.src = base64Image;
    });
}
let cameras = new L.GeoJSON(null, {
    pointToLayer: function (feature, latlng) {
        let img = new Image();
        //load Base64 image
        img.src = feature.properties.Camera_Data.base64Image;
        var mapsize = map.getZoom();
        var iconsizeh = 64;
        var iconsizew = 48
        if (mapsize > 2) {
            iconsizeh = 64 * mapsize;
            iconsizew = 48 * mapsize;
        }
        let locaterIcon = L.icon({
                iconUrl: img.src,
                iconSize: [iconsizeh, iconsizew], // size of the icon
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
        });
    },
    onEachFeature: function (feature, layer) {
        layer.on('click', function (e) {
            View_Web_Camera(feature.properties);
        });
        layer.bindTooltip(feature.properties.empName, {
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
function formatwebcameralayout(id, model, description, base64Image) {
    return $.extend(id, model, description, {
        base64Background: base64Image,
        camera_ip: id,
        camera_description: description,
        camera_model: getModel(model)
    });
}
var webCameraViewData = null;
function updateBoundingBox() {
    var Data = webCameraViewData;
    // uncommented until we understand how Darvis coordinates translate to the video stream coordinates
    //getAlertBoundingBox(Data.DarvisAlerts, imageWidth, imageHeight).then((img) => {
    //// getAlertBoundingBox(null, 1, 1).then((img) => {
    //    document.getElementById("openCameraOverlay").src = img;
    //});
}
var boundingInterval = null;
var openCameraId = null;
function View_Web_Camera(Data) {
    try {
        webCameraViewData = Data;
        openCameraId = Data.id;
        $('#cameramodalHeader').text('View Web Camera');
        var cameraname = checkValue(Data.empName) ? Data.empName : Data.name;
        $('#cameradescription').text(cameraname);
        camera_modal_body = $('div[id=camera_modalbody]');
        camera_modal_body.empty();
        
        if (boundingInterval) {
            boundingInterval = null;
        }
        boundingInterval = setInterval(() => { updateBoundingBox(); }, 1000);
        // to be uncommented later when Alert coordinates are able to match the video stream
        getAlertBoundingBox(Data.DarvisAlerts, imageWidth, imageHeight).then((img) => {
       //  getAlertBoundingBox(null, 1, 1).then((img) => {
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