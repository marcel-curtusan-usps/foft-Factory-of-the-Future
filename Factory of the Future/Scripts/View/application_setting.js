$('#AppSetting_value_Modal').on('hidden.bs.modal', function () {
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

    if (!$('#AppSetting_Modal').hasClass('in')) {
        $('#AppSetting_Modal').addClass('modal-open');
    }
});
$('#AppSetting_value_Modal').on('shown.bs.modal', function () {
    $('span[id=error_appsettingvalue]').text("");
    $('button[id=appsettingvalue]').prop('disabled', false);
    //Connection name Validation
    if (!checkValue($('input[id=modalValueID]').val())) {
        $('input[id=modalValueID]').removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_modalValueID]').text("Please Enter Value");
    }
    else {
        $('input[id=modalValueID]').removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_modalValueID]').text("");
    }
});
//app setting
function Edit_AppSetting(table) {
    fotfmanager.server.getAppSettingdata().done(function (AppsettingData) {
        if (AppsettingData) {
            LoadappSettingTable(AppsettingData, table);
        }
    });
}
function Get_Action_State() {
    if (/^Admin/i.test(User.Role)) {
        return '<div class="btn-toolbar" role="toolbar">' +
            '<div class="btn-group mr-2" role="group" >' +
            '<button class="btn btn-light btn-sm mx-1 pi-iconEdit" name="editappsetting"></button>' +
            '</div>';
    }
    else {
        return '';
    }
}
function Edit_AppSetting_Value(id, value, table) {
    $('#appsettingvaluemodalHeader').text('Edit ' + id + ' Setting');
    $('input[id=modalKeyID]').val(id);
    $('input[id=modalValueID]').val(value);
    if (/TIMEZONE/i.test(id)) {
        fotfmanager.server.getTimeZone().done(function (data) {
            $('.valuediv').css("display", "none");
            $('.timezonediv').css("display", "block");
            $('#timezoneValueID').empty();
            $('<option/>').val("").appendTo('#timezoneValueID');
            $.each(data, function () {
                $('<option/>').val(this).html(this).appendTo('#timezoneValueID');
            })
            $('#timezoneValueID').val(value);
        })
    }
    else {
        $('.valuediv').css("display", "block");
        $('.timezonediv').css("display", "none");
    }
    $('button[id=appsettingvalue]').off().on('click', function () {
        $('button[id=appsettingvalue]').prop('disabled', true);
        var jsonObject = {};
        if (/TIMEZONE/i.test(id)) {
            jsonObject[id] = $("#timezoneValueID option:selected").val();
        }
        else {
            jsonObject[id] = $('input[id=modalValueID]').val();
        }
        if (!$.isEmptyObject(jsonObject)) {
            fotfmanager.server.editAppSettingdata(JSON.stringify(jsonObject)).done(function () {

                $('span[id=error_appsettingvalue]').text("Data has been updated");
                setTimeout(function () { $("#AppSetting_value_Modal").modal('hide'); }, 1500);
                Edit_AppSetting(table);

            });
        }
    });
    $('#AppSetting_value_Modal').modal();
}
async function LoadappSettingTable(AppsettingData, table) {
    let AppSettingTable = $('table[id=' + table + ']');
    let AppSettingTable_Body = AppSettingTable.find('tbody');
    let AppSettingTable_row_template = '<tr data-id="{id}" data-value="{value}">' +
        '<td >{id}</td>' +
        '<td >{value}</td>' +
        '<td>{action}</td>' +
        '</tr>';

    AppSettingTable_Body.empty();
    var index = 0;
    function formatAppSetting(key, value, index) {
        return $.extend(key, value, index, {
            number: index,
            id: key,
            value: value,
            action: Get_Action_State()
        });
    }
    $.each(AppsettingData, function (key, value) {
        if (!/REMOTEDB|SERVER_ACTIVE|SERVER_ACTIVE_HOSTNAME/i.test(key)) {
            AppSettingTable_Body.append(AppSettingTable_row_template.supplant(formatAppSetting(key, value, index++)));
        }
    });
    $('button[name=editappsetting]').on('click', function () {
        var td = $(this);
        var tr = $(td).closest('tr'),
            id = tr.attr('data-id'),
            value = tr.attr('data-value');
        Edit_AppSetting_Value(id, value, table);
    });
}