﻿let nextSibling, fileName ="", checked;
$(function () {
  
    $("#fupload").keyup(function (e) {
        if (this.value.length === 0) {
            $("button[id=btnUpload]").prop("disabled", true);
        }
        else {
            $("button[id=btnUpload]").prop("disabled", false);
        }

    });
    $('button[id=btnUpload]').on('click', function () {
        $('button[id=btnUpload]').prop("disabled", true);
        let fileUpload = $("#fupload").get(0);
        let files = fileUpload.files;
        if (files.length > 0) {
            let data = new FormData();
            for (const element of files) {
                data.append(element.name, element);
            }
            data.append("metersPerPixel", $("#metersPerPixel option:selected").val());
            $.ajax({
                url: "/api/UploadFiles",
                type: "POST",
                data: data,
                cache: false,
                contentType: false,
                processData: false,
                beforeSend: function () {
                    $('button[id=btnUpload]').prop("disabled", true);
                    $('#progresbarrow').css('display', 'block');
                    $('span[id=error_btnUpload]').text("Loading File Please stand by");
                    let progress = 10
                    $('#file_upload_progressbar').css('width', progress + '%');
                },
                xhr: function () {
                    let xhr = $.ajaxSettings.xhr();
                    if (xhr.upload) {
                        xhr.upload.addEventListener("progress", function (evt) {
                            if (evt.lengthComputable) {
                                let percentComplete = evt.loaded / evt.total;
                                percentComplete = parseInt(percentComplete * 100);
                                $('#file_upload_progressbar').attr('aria-valuenow', percentComplete).css('width', percentComplete + '%');
                                if (percentComplete === 100) {
                                    $('span[id=error_btnUpload]').text("File Transfer Complete -->> Processing File ");
                                }
                            }
                        }, false);
                    }
                    return xhr;
                },
                success: function (response) {
                    if (response !== "") {
                        try {
                            $('span[id=error_btnUpload]').text("File Processing Completed");
                            setTimeout(function () { Clear(); }, 500);
                        }
                        catch (e) {
                            $('span[id=error_btnUpload]').text(e);
                        }
                    }
                },
                error: function (response) {
                    $('span[id=error_btnUpload]').text(response.statusText);
                    $('#progresbarrow').css('display', 'none');
                    setTimeout(function () { Clear(); }, 10000);
                },
                failure: function (response) {
                    $('span[id=error_btnUpload]').text(response.statusText);
                    $('#progresbarrow').css('display', 'none');
                    setTimeout(function () { Clear(); }, 10000);
                }
            })

        }
    });
});
//on open set rules
$('#FloorPlan_Modal').on('shown.bs.modal', function () {
    if (fileName === "" && $('select[id=metersPerPixel] option:selected').val() === "") {
        $("button[id=btnUpload]").prop("disabled", true);
    }
    else {
        $("button[id=btnUpload]").prop("disabled", false );
    }
    $('select[name=metersPerPixel]').change(function () {
        if (fileName !== "" && $('select[id=metersPerPixel] option:selected').val() !== "") {
            $("button[id=btnUpload]").prop("disabled", false);
        }
        else {
            $("button[id=btnUpload]").prop("disabled", true);
        }
    });
    $("#fupload").change(function (e) {
        if (document.getElementById("fupload").files.length > 0) {
            fileName = document.getElementById("fupload").files[0].name;
            nextSibling = e.target.nextElementSibling
            nextSibling.innerText = fileName
        }
        else {
            fileName = "";
            nextSibling = e.target.nextElementSibling
            nextSibling.innerText = 'Choose file'
        }

        $('span[id=error_btnUpload]').text("");
        if (fileName === "" && $('select[id=metersPerPixel] option:selected').val() !== "") {
            $("button[id=btnUpload]").prop("disabled", false);
        }
        else {
            $("button[id=btnUpload]").prop("disabled", true);
        }
    });
});
$('#FloorPlan_Modal').on('hidden.bs.modal', function () {
    $(this)
        .find("input[type=text],textarea,select")
        .css({ "border-color": "#D3D3D3" })
        .val('')
        .end()
        .find("span[class=text]")
        .css("border-color", "#FF0000")
        .val('')
        .text("")
        .end()
        .find('input[type=checkbox]')
        .prop('checked', false).change();
    let progress = 0;
    $('#progresbarrow').css('display', 'none');
    $('#file_upload_progressbar').css('width', progress + '%');
    $('input[id=fupload][type=file]').val("");
    document.getElementById("fupload").nextElementSibling.innerText = 'Choose file';
    $('span[id=error_btnUpload]').text("");
    $('button[id=btnUpload]').prop("disabled", true);
    sidebar.open('setting');
});
function Add_Floorplan() {
    $('#floormodalHeader_ID').text('Add New Floor Plan');
    $('#FloorPlan_Modal').modal();
}