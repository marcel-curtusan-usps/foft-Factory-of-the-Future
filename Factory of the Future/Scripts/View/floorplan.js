let nextSibling, fileName = "", checked;
let APIAuthorization = "YmFzaWMgUVVkV1VFOVNWRUZNVlhObGNqcEJaM1pRYjNKMFlXeDFKR1Z5TURFPQ==";
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
        let progress = 10;
        $('button[id=btnUpload]').prop("disabled", true);
        let fileUpload = $("#fupload").get(0);
        let files = fileUpload.files;
        if (files.length > 0) {
            let data = new FormData();
            for (const element of files) {
                data.append(element.name, element);
             
            }
            data.append("name", $('input[type=text][id=floorname]').val());
            data.append("metersPerPixel", $("#metersPerPixel option:selected").val());
            $.ajax({
                url: window.location.href + "api/UploadFiles",
                headers:
                {
                    'APIAuthorization': APIAuthorization
                },
                type: "POST",
                data: data,
                cache: false,
                contentType: false,
                processData: false,
                beforeSend: function () {
                    $('button[id=btnUpload]').prop("disabled", true);
                    $('#progresbarrow').css('display', 'block');
                    $('span[id=error_btnUpload]').text("Loading File Please stand by");
              
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
function RemoveFloorPlan(id, value, table) {
    var jsonObject = {
        Id: value
    };
    if (!$.isEmptyObject(jsonObject)) {
        fotfmanager.server.removeFloorplan(JSON.stringify(jsonObject)).done(function () {

            $('span[id=error_appsettingvalue]').text("Data has been updated");
            setTimeout(function () { $("#AppSetting_value_Modal").modal('hide'); }, 1500);
            Edit_AppSetting(table);

        });
    }
}
async function Load_Floorplan_Table(table) {
    let FloorplanTable = $('table[id=' + table + ']');
    let FloorplanTable_Body = FloorplanTable.find('tbody');
    let FloorplanTable_row_template = '<tr data-id="{id}" data-value="{value}">' +
        '<td ><span class="ml-p25rem">{id}</span></td>' +
        '<td >{value}</td>' +
        '<td>{action}</td>' +
        '</tr>';

    FloorplanTable_Body.empty();
    fotfmanager.server.getFloorPlanData().done(function (FloorplanData) {
        if (FloorplanData) {
            $.each(FloorplanData, function () {
                FloorplanTable_Body.append(FloorplanTable_row_template.supplant(formatFloorplan(this)));
            });
        }
    });
    $('button[name=removeFloorplan]').on('click', function () {
        let td = $(this);
        let tr = $(td).closest('tr'),
            id = tr.attr('data-id'),
            value = tr.attr('data-value');
        RemoveFloorPlan(id, value, table);
    });
}
function formatFloorplan(data) {
    return $.extend(data, {
        number: data.id,
        id: data.name,
        value: data.id,
        action: Get_Floor_Action_State()
    });
}
function Get_Floor_Action_State() {
    if (/^Admin/i.test(User.Role)) {
        return '<div class="btn-toolbar" role="toolbar">' +
            '<div class="btn-group mr-2" role="group" >' +
            '<button class="btn btn-light btn-sm mx-1 pi-trashFill" name="removeFloorplan" onclick="RemoveFloorPlan($(this))"></button>' +
            '</div>';
    }
    else {
        return '';
    }
}
