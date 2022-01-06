

//on close clear all inputs
$('#Camera_Modal').on('hidden.bs.modal', function () {
    camera_modal_body = $('div[id=camera_modalbody]');
    camera_modal_body.empty();
});
async function init_cameras() {
	try {
        fotfmanager.server.getCameraList().done(function (cameradata) {
            if (cameradata.length > 0) {
                camera_Table = $('table[id=cameratable]');
                let camera_Table_Body = camera_Table.find('tbody');

                camera_Table_Body.empty();
                $.each(cameradata, function () {
                    camera_Table_Body.append(camera_row_template.supplant(formatCameralayout(this)));
                });
            }
		});
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
    '<button class="btn btn-light btn-sm mx-1 bi-camera-video camera_view"></button>' +
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
function View_Web_Camera(id, model, description) {
    try {
        $('#cameramodalHeader').text('View Web Camera');
        $('#cameradescription').text(description);
        camera_modal_body = $('div[id=camera_modalbody]');
        camera_modal_body.empty();
        camera_modal_body.append(camera_layout.supplant(formatwebcameralayout(id, model, description)));
        $('#Camera_Modal').modal();
        sidebar.close('camera_video');
    } catch (e) {
        $("#error_camera").text(e);
        console.log(e);
    }
}
function Add_Camera() {
    try {
        $('#AddEdit_Camera_Modal').modal();

    } catch (e) {
        console.log(e);
    }
}