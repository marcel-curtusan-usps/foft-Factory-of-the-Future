

//on close clear all inputs
$('#Camera_Modal').on('hidden.bs.modal', function () {
    camera_modal_body = $('div[id=camera_modalbody]');
    camera_modal_body.empty();
});
var cameras = new L.GeoJSON(null, {
    
    pointToLayer: function (feature, latlng) {
        let m = map;
        var locaterIcon = L.icon({
            iconUrl: "../../Content/images/NoImage.png",
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
//var customOptions =
//{
//    'maxWidth': '800',
//    'width': '400',
//    'className': 'popupCustom'
//}

async function init_cameras() {
    try {
        if (!/^https/i.test(window.location.protocol)) {
            fotfmanager.server.joinGroup("CameraMarkers");
            fotfmanager.server.getCameraMarkerList().done(function (cameradata) {
                if (cameradata.length > 0) {
                    setTimeout(
                        () => {
                            cameras.addData(cameradata);
                            updateCameras(cameradata);},
                        1000);
                    
                }
            });
        }
	} catch (e) {
        console.log(e);
    }
}


$.extend(fotfmanager.client, {
    updateCameraStatus: async (cameraupdate) => { updateCameras(cameraupdate) }
});

async function updateCameras(cameraupdates) {
    try {
        for (var cameraupdate of cameraupdates) {
            if (cameras.hasOwnProperty("_layers")) {
                $.map(cameras._layers, function (layer) {
                    if (layer.hasOwnProperty("feature")) {
                        if (layer.feature.properties.id === cameraupdate.properties.id) {
                            var locaterIcon = L.icon({
                                iconUrl: cameraupdate.properties.base64Image,

                                iconSize: [64, 48], // size of the icon
                                iconAnchor: [0, 0], // point of the icon which will correspond to marker's location
                                shadowAnchor: [0, 0],  // the same for the shadow
                                popupAnchor: [0, 0] // point from which the popup should open relative to the iconAnchor
                            });
                            layer.setIcon(locaterIcon);
                            var a = 1;
                            
                        }
                    }
                });
            }
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
/*


			var fdb_id = 1449534
			var timedOut = false
			var timeSecs = 120

		//console.log("fdb_id",fdb_id);
		if(!timedOut){
			setInterval(function() {
				var div = document.querySelector("#counter")
				var count = div.textContent * 1 - 1
				div.textContent = count
				if (count <= 0) {
					$('.hide').html('')
					$('#counter').html('0')
					$('#cameras').html('<div style="text-align:center; font-size:2em;">Your connection has timed out</div>')
					timedOut = true
				}
			}, 1000)
		}
		var gridUrl = './CFC/index2.cfc?method='
			$(function(){
			$('#counter').html(timeSecs)
			$.ajax({
				type: "GET",
				cache: false,
				url: gridUrl + 'getOneSite',
				data: {fdb_id: fdb_id},
				dataType:"json",
				success: function(data){console.log('get',data)
					if(data.length > 0){
						var txt = '<a href="https://maps.google.com/maps?t=k&q=loc:'+ data[0].FACILITY_LATITUDE_NUM +'+'+data[0].FACILITY_LONGITUDE_NUM+ '" target="_blank">'+data[0].FACILITY_PHYS_ADDR_TXT+'</a>'
						$('#location').html(data[0].FACILITY_DISPLAY_NME)
						$('#address').html(txt)
						$('#region').html(data[0].GEO_PROC_DIVISION_NM +' Division of the  ' +data[0].GEO_PROC_REGION_NM + ' Region')
						var html = ''
						for(var i=0;i<data.length;i++){//
							if( i % 2 === 0) html+='<div class="frameFlex">'
								if(data[i].AUTH_KEY == ''){
									var url = 'http://'+data[i].CAMERA_NAME+'/mjpg/video.mjpg?camera='
								}else{
									var url = 'http://'+data[i].AUTH_KEY+'@'+data[i].CAMERA_NAME+'/mjpg/video.mjpg?camera='
								}
								switch(data[i].MODEL_NUM){
									case 3719:
										url+='quad'
										break
									case 3727:
										url+='5'
										break
									default:
										url+='1'
										break
								}
								if (data[i].AUTH_KEY.length > 0) {
									html+='<div class="passworded">'
									html+='<p>'//This camera is password protected. Click the link to view it directly.<br>'
									html+='<a target=_blank rel="noopener noreferrer" href="'+url+'">'+data[i].DESCRIPTION+'</a>'
									html+='</p>'
									html+='</div>'
								}else{
									html+='<div class="hide"><iframe src="'+url+'" height="400" width="600"></iframe></div>'
								}
							if( i % 2 === 1) html+='</div>'
						}
						if( i % 2 === 1) html+='<div></div>'
						html+='</div>'
						$('#cameras').html(html)
					}else{
						$('#error').html('The supplied FDB ID is not valid. Please try again.')
					}
				},
				error: function (xhr, ajaxOptions, thrownError) {
					console.log(xhr.status)
					console.log(ajaxOptions)
					console.log(thrownError)
				}
			})
		})

 
 */