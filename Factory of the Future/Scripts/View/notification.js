/**
* this is use to setup a the Notification information and other function
*
* **/

$.extend(fotfmanager.client, {
    updateNotification: async (updatenotification) => { updateNotification(updatenotification) }
});
$('#Notification_Setup_Modal').on('hidden.bs.modal', function () {
    $(this)
        .find("input[type=text],textarea,select")
        .val('')
        .end()
        .find("span[class=text]")
        .val('')
        .text("")
        .end()
        .find('input[type=checkbox]')
        .prop('checked', false).change()
        .end();
});
$('#Notification_Setup_Modal').on('shown.bs.modal', function () {
    $('.warning_conditionpickvalue').html($('input[id=warning_condition]').val());
    $('input[id=warning_condition]').on('input change', () => {
        $('.warning_conditionpickvalue').html($('input[id=warning_condition]').val());
    });
    $('.critical_conditionpickvalue').html($('input[id=critical_condition]').val());
    $('input[id=critical_condition]').on('input change', () => {
        $('.critical_conditionpickvalue').html($('input[id=critical_condition]').val());
    });
    $('span[id=error_notificationsubmitBtn]').text("");
    //Condition name Validation
    if (!checkValue($('input[type=text][name=condition_name]').val())) {
        $('input[type=text][name=condition_name]').removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_condition_name]').text("Please enter Condition Name");
    }
    else {
        $('input[type=text][name=condition_name]').removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_condition_name]').text("");
    }
    // condition Validation
    if (!checkValue($('input[type=text][name=condition]').val())) {
        $('input[type=text][name=condition]').removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_condition]').text("Please Enter Condition");
    }
    else {
        $('input[type=text][name=condition]').removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_condition]').text("");
    }
    // condition_type type Validation
    //if (!checkValue($('select[name=condition_type]').val())) {
    //    $('select[name=condition_type]').removeClass('is-valid').addClass('is-invalid');
    //    $('span[id=error_condition_type]').text("Please Select Condition Type");
    //}
    //else {
    //    $('select[name=condition_type]').removeClass('is-invalid').addClass('is-valid');
    //    $('span[id=error_condition_type]').text("");
    //}
    //warning_condition Validation
    if (!checkValue($('input[type=text][name=warning_condition]').val())) {
        $('input[type=text][name=warning_condition]').removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_warning_condition]').text("Please Condition Time in Minutes");
    }
    else {
        $('input[type=text][name=warning_condition]').removeClass('is-invalid').addClass('is-valid');
        $('span[id=warning_condition]').text("");
    }
    //critical_condition Validation
    if (!checkValue($('input[type=text][name=critical_condition]').val())) {
        $('input[type=text][name=critical_condition]').removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_critical_condition]').text("Please Condition Time in Minutes");
    }
    else {
        $('input[type=text][name=critical_condition]').removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_critical_condition]').text("");
    }
    enableNotificationSubmit();

    $('input[type=text][name=critical_condition]').keyup(function () {
        if ($.isNumeric($('input[type=text][name=critical_condition]').val())) {
            if (!validateNum(parseInt($('input[type=text][name=critical_condition]').val()), 0, 60)) {
                $('input[type=text][name=critical_condition]').removeClass('is-valid').addClass('is-invalid');
                $('span[id=error_critical_condition]').text("Invalid Number");
            }
            else {
                $('input[type=text][name=critical_condition]').removeClass('is-invalid').addClass('is-valid');
                $('span[id=error_critical_condition]').text("");
            }
        }
        else if (checkValue($('input[type=text][name=critical_condition]').val())) {
            $('input[type=text][name=critical_condition]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_critical_condition]').text("Enter Number");
        }
        else if (!checkValue($('input[type=text][name=critical_condition]').val())) {
            $('input[type=text][name=critical_condition]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_critical_condition]').text("Enter Number");
        }
        enableNotificationSubmit();
    });
    $('input[type=text][name=warning_condition]').keyup(function () {
        if ($.isNumeric($('input[type=text][name=warning_condition]').val())) {
            if (!validateNum(parseInt($('input[type=text][name=warning_condition]').val()), 0, 60)) {
                $('input[type=text][name=warning_condition]').removeClass('is-valid').addClass('is-invalid');
                $('span[id=error_warning_condition]').text("Invalid Number");
            }
            else {
                $('input[type=text][name=warning_condition]').removeClass('is-invalid').addClass('is-valid');
                $('span[id=error_warning_condition]').text("");
            }
        }
        else if (checkValue($('input[type=text][name=warning_condition]').val())) {
            $('input[type=text][name=warning_condition]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_warning_condition]').text("Enter Number");
        }
        else if (!checkValue($('input[type=text][name=warning_condition]').val())) {
            $('input[type=text][name=warning_condition]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_warning_condition]').text("Enter Number");
        }
        enableNotificationSubmit();
    });
    $('input[type=text][name=condition]').keyup(function () {
        if (!checkValue($('input[type=text][name=condition]').val())) {
            $('input[type=text][name=condition]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_condition]').text("Please Enter Condition");
        }
        else {
            $('input[type=text][name=condition]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_condition]').text("");
        }
        enableNotificationSubmit();
    });
    $('input[type=text][name=condition_name]').keyup(function () {
        //hostname Validation

        if (!checkValue($('input[type=text][name=condition_name]').val())) {
            $('input[type=text][name=condition_name]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_condition_name]').text("Please Enter Condition Name");
        }
        else {
            $('input[type=text][name=condition_name]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_condition_name]').text("");
        }
        enableNotificationSubmit();
    });
    $('select[name=condition_type]').change(function () {
        if (!checkValue($('select[name=condition_type]').val())) {
            $('select[name=condition_type]').removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_servercontenttype]').text("Please Select Condition Type");
        }
        else {
            $('select[name=condition_type]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_condition_type]').text("");
        }
        enableNotificationSubmit();
    });
});
var notification = [];
async function updateNotification(updatenotification) {
    try {
        if (updatenotification) {
            var notificationindex = notification.filter(x => x.NOTIFICATIONGID === updatenotification.NOTIFICATIONGID).map(x => x).length;
            let indexobj = -0;
            var Vehiclecount = -0;
            var ctscount = -0;
            var machinecount = -0;

            if (notificationindex === 0) {
                notification.push(updatenotification);
            }
            if (notificationindex > 0) {
                if (updatenotification.hasOwnProperty("DELETE")) {
                    if (updatenotification.DELETE) {
                        notification.forEach(function (obj) {
                            if (obj.NOTIFICATIONGID === updatenotification.NOTIFICATIONGID) {
                                notification.splice(notification.indexOf(obj), 1);
                            }
                        })
                    }
                }
                else {
                    notification.forEach(function (obj) {
                        if (obj.NOTIFICATIONGID === updatenotification.NOTIFICATIONGID) {
                            indexobj = notification.indexOf(obj);
                        }
                    })
                }

                let Table = {};
                if (updatenotification.TYPE === "vehicle") {
                    Table = $('table[id=agvnotificationtable]');
                }
                if (updatenotification.TYPE === "CTS") {
                    Table = $('table[id=ctsnotificationtable]');
                }
                if (Table.length > 0) {
                    let Table_Body = Table.find('tbody');
                    var findtrdataid = Table_Body.find('tr[data-id=' + updatenotification.NOTIFICATIONGID + ']');
                    if (findtrdataid.length > 0) {
                        if (updatenotification.hasOwnProperty("DELETE")) {
                            Table_Body.find('tr[data-id=' + updatenotification.NOTIFICATIONGID + ']').remove();
                        }
                        else {
                            Table_Body.find('tr[data-id=' + updatenotification.NOTIFICATIONGID + ']').replaceWith(row_template.supplant(formatnotifirow(updatenotification)));
                        }
                    }
                    else {
                        Table_Body.append(row_template.supplant(formatnotifirow(updatenotification)));
                    }
                    //if (parseInt(indexobj) >= 0) {
                    //    if (notification[indexobj].hasOwnProperty("SHOWTOAST")) {
                    //        if (notification[indexobj].SHOWTOAST === true) {
                    //            ///////////////// need to development this more.///////////////////
                    //            // html template for critical toast alert message
                    //            $toast_alert_critical_template =
                    //                '<div id="{id}" class="toast alert-danger show" role="alert" data-autohide="false">' +
                    //                '<div class="toast-header alert-danger py-2">' +
                    //                '<i class="pi-iconCriticalTriangle rounded mr-2"></i> ' +
                    //                '<strong class="mr-auto">Critical</strong>' +
                    //                '<small id="{id}_duration">{duration}</small>' +
                    //                '<button type="button" class="ml-2 mb-1 close" data-dismiss="toast" data-index="{indexobj}" aria-label="Close">' +
                    //                '<span aria-hidden="true" class="iconSmall"><i class="pi-iconExit" style="color: #000;"></i></span>' +
                    //                '</button>' +
                    //                '</div>' +
                    //                '<div class="toast-body">' +
                    //                '<!-- Collapsible section -->' +
                    //                '<a class="btn btn-link d-flex justify-content-between" data-toggle="collapse" href="#collapseSection" role="button" aria-expanded="false" aria-controls="collapseSection">' +
                    //                '<div>{name} - {type} - {condition}</div>' +
                    //                '<div class="iconXSmall"><i class="pi-iconCaretDownFill"></i></div>' +
                    //                '</a>' +
                    //                '<div class="collapse" id="collapseSection">' +
                    //                '<div class="mt-1">' +
                    //                '<ol class="pl-4 mb-0">' +
                    //                '<li class="pb-1">{ResolutionText1}</li>' +
                    //                '<li class="pb-1">{ResolutionText2}</li>' +
                    //                '<li class="pb-1">{ResolutionText3}</li>' +
                    //                '</ol>' +
                    //                '</div>' +
                    //                '<div class="d-flex justify-content-between">' +
                    //                '<div class="col-8">' +
                    //                '<small>Are the instructions helpful?</small>' +
                    //                '<div class="col-12 d-flex justify-content-start">' +
                    //                '<button class="btn btn-light border-0 iconMedium px-2"><i class="pi-iconThumbUpOutline"></i></button>' +
                    //                '<button class="btn btn-light border-0 iconMedium px-2 ml-3"><i class="pi-iconThumbDownOutline"></i></button>' +
                    //                '</div>' +
                    //                '</div>' +
                    //                '<div class="col-4 d-flex justify-content-between px-0 pt-4">' +
                    //                '<button class="btn btn-light iconMedium px-2"><i class="pi-iconEdit"></i></button>' +
                    //                '<button class="btn btn-light iconMedium px-2"><i class="pi-iconSnooze"></i></button>' +
                    //                '<button class="btn btn-light iconMedium px-2"><i class="pi-iconSubmit"></i></button>' +
                    //                '</div>' +
                    //                '</div>' +
                    //                '</div>' +
                    //                '</div>' +
                    //                '</div>'
                    //                ;

                    //            // html template for warning toast alert message
                    //            $toast_alert_warning_template =
                    //                '<div id="{id}" class="toast alert-warning show" role="alert" data-autohide="false">' +
                    //                '<div class="toast-header alert-warning py-2">' +
                    //                '<i class="pi-iconWarningSquare rounded mr-2"></i>' +
                    //                '<strong class="mr-auto">Warning</strong>' +
                    //                '<small>{duration}</small>' +
                    //                '<button type="button" class="ml-2 mb-1 close" data-dismiss="toast" data-index="{indexobj}" aria-label="Close">' +
                    //                '<span aria-hidden="true" class="iconSmall"><i class="pi-iconExit"></i></span>' +
                    //                '</button>' +
                    //                '</div>' +
                    //                '<div class="toast-body">' +
                    //                '<!-- Collapsible section -->' +
                    //                '<a class="btn btn-link d-flex justify-content-between" data-toggle="collapse" href="#collapseSection1" role="button" aria-expanded="false" aria-controls="collapseSection1">' +
                    //                '<div>{name} - {type} - {condition}</div>' +
                    //                '<div class="iconXSmall"><i class="pi-iconCaretDownFill"></i></div>' +
                    //                '</a>' +
                    //                '<div class="collapse" id="collapseSection1">' +
                    //                '<div class="mt-1">' +
                    //                '<ol class="pl-4 mb-0">' +
                    //                '<li class="pb-1">{ResolutionText1}</li>' +
                    //                '<li class="pb-1">{ResolutionText2}</li>' +
                    //                '<li class="pb-1">{ResolutionText3}</li>' +
                    //                '</ol>' +
                    //                '</div>' +
                    //                '<div class="d-flex justify-content-between">' +
                    //                '<div class="col-8">' +
                    //                '<small>Are the instructions helpful?</small>' +
                    //                '<div class="col-12 d-flex justify-content-start">' +
                    //                '<button class="btn btn-light border-0 iconMedium px-2"><i class="pi-iconThumbUpOutline"></i></button>' +
                    //                '<button class="btn btn-light border-0 iconMedium px-2 ml-3"><i class="pi-iconThumbDownOutline"></i></button>' +
                    //                '</div>' +
                    //                '</div>' +
                    //                '<div class="col-4 d-flex justify-content-between px-0 pt-4">' +
                    //                '<button class="btn btn-light iconMedium px-2"><i class="pi-iconEdit"></i></button>' +
                    //                '<button class="btn btn-light iconMedium px-2"><i class="pi-iconSnooze"></i></button>' +
                    //                '<button class="btn btn-light iconMedium px-2"><i class="pi-iconSubmit"></i></button>' +
                    //                '</div>' +
                    //                '</div>' +
                    //                '</div>' +
                    //                '</div>' +
                    //                '</div>'
                    //                ;
                    //            var color = conditioncolor(updatenotification.VEHICLETIME, parseInt(updatenotification.WARNING), parseInt(updatenotification.CRITICAL));
                    //            var condition_div = $("div[id=" + updatenotification.NOTIFICATIONGID + "]")
                    //            // determine whether to generate a critical or a warning toast alert based on message criteria with default being warning
                    //            // determine whether notification needs to be deleted, updated or replaced.
                    //            // if notification already exists
                    //            if (findtrdataid.length > 0) {
                    //                // determine if notification is marked for deletion and delete
                    //                if (updatenotification.hasOwnProperty("DELETE")) {
                    //                    $("div[id=" + updatenotification.NOTIFICATIONGID + "]").remove();
                    //                }
                    //                // replace existing notification with updated notification
                    //                else {
                    //                    // critical toast alert
                    //                    if (color === "#bd213052") {
                    //                        if (condition_div.length === 0) {
                    //                            $('#toastnotification').append($toast_alert_critical_template.supplant(formatnotifirow(updatenotification, indexobj)));
                    //                        }

                    //                        if ($("div[id=" + updatenotification.NOTIFICATIONGID + "]").hasClass('show')) {
                    //                            $("small[id=" + updatenotification.NOTIFICATIONGID + "_duration]").text(calculateDuration(updatenotification.VEHICLETIME));
                    //                        }
                    //                        else {
                    //                            $("div[id=" + updatenotification.NOTIFICATIONGID + "]").toast('show');
                    //                        }

                    //                    }
                    //                    // warning toast alert
                    //                    if (color === "#ffff0080") {
                    //                        $("div[id=" + updatenotification.NOTIFICATIONGID + "]").toast('show');
                    //                    }
                    //                }
                    //            }
                    //            // when notification does not exist and must be added
                    //            else {
                    //                // critical toast alert
                    //                if (color === "#bd213052") {
                    //                    if (condition_div.length === 0) {
                    //                        $('#toastnotification').append($toast_alert_critical_template.supplant(formatnotifirow(updatenotification, indexobj)));
                    //                        $("div[id=" + updatenotification.NOTIFICATIONGID + "]").toast('show');
                    //                    }
                    //                }
                    //                // warning toast alert
                    //                if (color === "#ffff0080") {
                    //                    if (condition_div.length === 0) {
                    //                        $('#toastnotification').append($toast_alert_warning_template.supplant(formatnotifirow(updatenotification, indexobj)));
                    //                        $("div[id=" + updatenotification.NOTIFICATIONGID + "]").toast('show');
                    //                    }
                    //                }
                    //            }
                    //        };
                    //    }
                    //}
                }
            }

            //$('div[class=toast]').on('hidden.bs.toast', function () {
            //    if (notification[indexobj].hasOwnProperty("SHOWTOAST")) {
            //        if (notification[indexobj].SHOWTOAST = false) { }
            //    }
            //})

            Vehiclecount = notification.filter(x => x.TYPE === "vehicle").map(x => x).length
            ctscount = notification.filter(x => x.TYPE === "CTS").map(x => x).length
            machinecount = notification.filter(x => x.TYPE === "machine").map(x => x).length

            //AGV Counts
            if (Vehiclecount > 0) {
                if (parseInt($('#agvnotificaion_number').text()) !== Vehiclecount) {
                    $('#agvnotificaion_number').text(Vehiclecount);
                }

                //if ($('#agvnotificaion').hasClass("not_bell_ring")) {
                //    $('#agvnotificaion').removeClass("not_bell_ring").addClass("bell_ring");
                //}
            }
            else {
                $('#agvnotificaion_number').text("");
                //if ($('#agvnotificaion').hasClass("bell_ring")) {
                //    $('#agvnotificaion').removeClass("bell_ring").addClass("not_bell_ring");
                //}
            }
            // CTS Counts
            if (ctscount > 0) {
                $('#ctsnotificaion_number').text(Vehiclecount);
                //if ($('#ctsnotificaion').hasClass("not_bell_ring")) {
                //    $('#ctsnotificaion').removeClass("not_bell_ring").addClass("bell_ring");
                //}
            }
            else {
                $('#ctsnotificaion_number').text("");
                //if ($('#ctsnotificaion').hasClass("bell_ring")) {
                //    $('#ctsnotificaion').removeClass("bell_ring").addClass("not_bell_ring");
                //}
            }
            //machine counts
            if (machinecount > 0) {
                $('#machinenotificaion_number').text(Vehiclecount);
                //if ($('#machinenotificaion').hasClass("not_bell_ring")) {
                //    $('#machinenotificaion').removeClass("not_bell_ring").addClass("bell_ring");
                //}
            }
            else {
                $('#machinenotificaion_number').text("");
                //if ($('#machinenotificaion').hasClass("bell_ring")) {
                //    $('#machinenotificaion').removeClass("bell_ring").addClass("not_bell_ring");
                //}
            }
        }
    }
    catch (e) {
        console.log(e);
    }
}

function LoadNotificationsetup(Data, table) {
    $.connection.FOTFManager.server.getNotification_ConditionsList(0).done(function (NotificationData) {
        notificationTable_Body.empty();
        $.each(NotificationData, function () {
            notificationTable_Body.append(notificationTable_row_template.supplant(formatnotificationrow(this)));
        });
        $('button[name=notificationedit]').on('click', function () {
            var td = $(this);
            var tr = $(td).closest('tr'),
                id = tr.attr('data-id');
            Notificationsetup(id, table);
        });
        $('button[name=notificationdelete]').on('click', function () {
            var td = $(this);
            var tr = $(td).closest('tr'),
                id = tr.attr('data-id');
            NotificationRemove(id, table);
        });
    });
}
let row_template =
    '<tr data-id={id} style=background-color:{conditioncolor} data-toggle=collapse data-target=#{action_text} class=accordion-toggle>' +
    '<td>{name}</td>' +
    '<td><button class="btn btn-outline-info btn-sm btn-block tagdetails" data-tag="{tagid}" >{type}</button></td>' +
    '<td>{duration}</td>' +
    '</tr>'
    ;
function formatnotifirow(properties, indx) {
    return $.extend(properties, {
        id: properties.NOTIFICATIONGID,
        tagid: properties.TAGID,
        name: properties.NAME,
        type: properties.VEHICLENAME,
        condition: properties.CONDITIONS,
        duration: calculateDuration(properties.VEHICLETIME),
        conditioncolor: conditioncolor(properties.VEHICLETIME, parseInt(properties.WARNING), parseInt(properties.CRITICAL)),
        warning_action_text: properties.WARNING_ACTION,
        critical_action_text: properties.CRITICAL_ACTION,
        action_text: conditionaction_text(properties.VEHICLETIME, parseInt(properties.WARNING), parseInt(properties.CRITICAL)) + "_" + properties.NOTIFICATIONGID,
        indexobj: indx
    });
}
let notificationTable = $('table[id=notificationsetuptable]');
let notificationTable_Body = notificationTable.find('tbody');

let notificationTable_row_template = '<tr data-id="{id}" class="{button_collor}">' +
    '<td>{name}</td>' +
    '<td class="text-center">{warning}</td>' +
    '<td class="text-center">{critical}</td>' +
    '<td class="text-center">{status}</td>' +
    '<td>' +
    '<button class="btn btn-light btn-sm mx-1 pi-iconEdit" name="notificationedit"></button>' +
    '<button class="btn btn-light btn-sm mx-1 pi-trashFill" name="notificationdelete"></button>' +
    '</td>' +
    '</tr>';
function GetnotificationStatus(data) {
    if (data.ACTIVE_CONDITION) {
        return "Active";
    }
    else {
        return "Disabled";
    }
}
function Get_notificationColor(data) {
    if (data.ACTIVE_CONDITION) {
        return "table-success";
    }
    else {
        return "table-warning";
    }
}
function formatnotificationrow(properties) {
    return $.extend(properties, {
        id: properties.ID,
        name: properties.NAME,
        type: properties.TYPE,
        condition: properties.CONDITIONS,
        warning: properties.WARNING,
        warning_action: properties.WARNING_ACTION,
        warning_color: properties.WARNING_COLOR,
        critical: properties.CRITICAL,
        critical_action: properties.CRITICAL_ACTION,
        critical_color: properties.CRITICAL_COLOR,
        status: GetnotificationStatus(properties),
        button_collor: Get_notificationColor(properties)
    })
}
async function Notificationsetup(data,table) {
    if ($.isEmptyObject(data)) {
        $('#notification_SetupHeader').text('Add New Notification');
        sidebar.close('notificationsetup');
        $('#notificationsubmitBtn').css("display", "block");
        $('#editnotificationsubmitBtn').css("display", "none");
        $('#Notification_Setup_Modal').modal();
        $('button[id=notificationsubmitBtn]').prop('disabled', false);
        $('button[id=notificationsubmitBtn]').off().on('click', function () {
            $('button[id=notificationsubmitBtn]').prop('disabled', true);
            var jsonObject = {};

            //condition active
            jsonObject.ACTIVE_CONDITION = $('input[type=checkbox][name=condition_active]')[0].checked;
            checkValue($('input[type=text][name=condition_name]').val()) ? jsonObject.NAME = $('input[type=text][name=condition_name]').val() : '';
            checkValue($('select[name=condition_type] option:selected').val()) ? jsonObject.TYPE = $('select[name=condition_type] option:selected').val() : '';
            checkValue($('input[type=text][name=condition]').val()) ? jsonObject.CONDITIONS = $('input[type=text][name=condition]').val() : '';
            checkValue($('input[id=warning_condition]').val()) ? jsonObject.WARNING = $('input[id=warning_condition]').val() : '';
            checkValue($('input[id=critical_condition]').val()) ? jsonObject.CRITICAL = $('input[id=critical_condition]').val() : '';
            checkValue($('textarea[id=warning_action]').val()) ? jsonObject.WARNING_ACTION = $('textarea[id=warning_action]').val() : '';
            checkValue($('textarea[id=critical_action]').val()) ? jsonObject.CRITICAL_ACTION = $('textarea[id=critical_action]').val() : '';
            if (!$.isEmptyObject(jsonObject)) {
                jsonObject.CREATED_BY_USERNAME = User.UserId;
                $.connection.FOTFManager.server.addNotification_Conditions(JSON.stringify(jsonObject)).done(function (Data) {
                    if (Data.length > 0) {
                        if (Data[0].hasOwnProperty("ERROR_MESSAGE")) {
                            $('span[id=error_notificationsubmitBtn]').text("Error " + Data[0].ERROR_MESSAGE);
                            setTimeout(function () { $("#Notification_Setup_Modal").modal('hide'); }, 1500);
                        }
                        else {
                            $('span[id=error_notificationsubmitBtn]').text("Condition has been Added");
                            LoadNotificationsetup(Data, table);
                            setTimeout(function () { $("#Notification_Setup_Modal").modal('hide'); sidebar.open('notificationsetup'); }, 1500);
                        }
                    }
                    else {
                        $('span[id=error_notificationsubmitBtn]').text("Error with the Data");
                    }
                });
            };
        });
    }
    else {
        if ($.isNumeric(data)) {
            var id = parseInt(data);
            $('#notificationsubmitBtn').css("display", "none");
            $('#editnotificationsubmitBtn').css("display", "block");
            $.connection.FOTFManager.server.getNotification_ConditionsList(id).done(function (N_Data) {
                if (N_Data.length > 0) {
                    $('#notification_SetupHeader').text('Edit Notification');
                    if (N_Data[0].ACTIVE_CONDITION) {
                        if (!$('input[type=checkbox][name=condition_active]')[0].checked) {
                            $('input[type=checkbox][name=condition_active]').prop('checked', true).change();
                        }
                    }
                    else {
                        if ($('input[type=checkbox][name=condition_active]')[0].checked) {
                            $('input[type=checkbox][name=condition_active]').prop('checked', false).change();
                        }
                    }
                    $('select[name=condition_type]').val(N_Data[0].TYPE)
                    $('input[type=text][name=condition_name]').val(N_Data[0].NAME);
                    $('input[type=text][name=condition]').val(N_Data[0].CONDITIONS);
                    $('.warning_conditionpickvalue').html($.isNumeric(N_Data[0].WARNING) ? parseInt(N_Data[0].WARNING) : 0);
                    $('input[id=warning_condition]').val($.isNumeric(N_Data[0].WARNING) ? parseInt(N_Data[0].WARNING) : 0)
                    $('.critical_conditionpickvalue').html($.isNumeric(N_Data[0].CRITICAL) ? parseInt(N_Data[0].CRITICAL) : 0);
                    $('input[id=critical_condition]').val($.isNumeric(N_Data[0].CRITICAL) ? parseInt(N_Data[0].CRITICAL) : 0)
                    $('textarea[id=warning_action]').val(N_Data[0].WARNING_ACTION);
                    $('textarea[id=critical_action]').val(N_Data[0].CRITICAL_ACTION);
                }
                $('button[id=editnotificationsubmitBtn]').prop('disabled', false);
                $('button[id=editnotificationsubmitBtn]').off().on('click', function () {
                    $('button[id=editnotificationsubmitBtn]').prop('disabled', true);
                    var jsonObject = {};

                    //condition active
                    jsonObject.ACTIVE_CONDITION = $('input[type=checkbox][name=condition_active]')[0].checked;
                    jsonObject.ID = id;
                    $('input[type=text][name=condition_name]').val() !== jsonObject.NAME ? jsonObject.NAME = $('input[type=text][name=condition_name]').val() : '';
                    $('select[name=condition_type] option:selected').val() !== jsonObject.TYPE ? jsonObject.TYPE = $('select[name=condition_type] option:selected').val() : '';
                    $('input[type=text][name=condition]').val() !== jsonObject.CONDITIONS ? jsonObject.CONDITIONS = $('input[type=text][name=condition]').val() : '';
                    $('input[id=warning_condition]').val() !== jsonObject.WARNING ? jsonObject.WARNING = $('input[id=warning_condition]').val() : '';
                    $('input[id=critical_condition]').val() !== jsonObject.CRITICAL ? jsonObject.CRITICAL = $('input[id=critical_condition]').val() : '';
                    $('textarea[id=warning_action]').val() !== jsonObject.WARNING_ACTION ? jsonObject.WARNING_ACTION = $('textarea[id=warning_action]').val() : '';
                    $('textarea[id=critical_action]').val() !== jsonObject.CRITICAL_ACTION ? jsonObject.CRITICAL_ACTION = $('textarea[id=critical_action]').val() : '';
                    if (!$.isEmptyObject(jsonObject)) {
                        jsonObject.LASTUPDATE_BY_USERNAME = User.UserId;
                        $.connection.FOTFManager.server.editNotification_Conditions(JSON.stringify(jsonObject)).done(function (Data) {
                            if (Data.length === 0) {
                                $('span[id=error_notificationsubmitBtn]').text("Unable to loaded Condition");
                                setTimeout(function () { $("#Notification_Setup_Modal").modal('hide'); }, 3000);
                            }
                            else {
                                $('span[id=error_notificationsubmitBtn]').text("Condition has been Edited");
                                LoadNotificationsetup(Data, table);
                                setTimeout(function () { $("#Notification_Setup_Modal").modal('hide'); sidebar.open('notificationsetup'); }, 1500);
                            }
                        });
                    };
                });

                sidebar.close('notificationsetup');
                $('#Notification_Setup_Modal').modal();
            });
        }
    }
}
async function NotificationRemove(data, table) {
    //RemoveNotificationModal
    if ($.isNumeric(data)) {
        var id = parseInt(data);
        sidebar.close('notificationsetup');
        $('#removeNotificationHeader').text('Remove Notification');
        $('#RemoveNotificationModal').modal();

        $('button[id=removeNotification]').off().on('click', function () {
            var jsonObject = { ID: id };
            $.connection.FOTFManager.server.deleteNotification_Conditions(JSON.stringify(jsonObject)).done(function (Data) {
                LoadNotificationsetup(Data, table);
                setTimeout(function () { $("#RemoveNotificationModal").modal('hide'); sidebar.open('notificationsetup'); }, 1500);
                $('#RemoveNotificationModal').modal();
            })
        });
    }
}
async function GetAGVnotificationinfoInfo(table, type) {
    // sort the data
    notification.sort(SortByVehicleName);
    $Table = $('table[id=' + table + ']');
    $Table_Body = $Table.find('tbody');
    $Table_Body.empty();
    $row_template =
        '<tr data-id={id} style=background-color:{conditioncolor} data-toggle=collapse data-target=#{action_text} class=accordion-toggle>' +
        '<td>{name}</td>' +
        '<td><button class="btn btn-outline-info btn-sm btn-block tagdetails" data-tag="{tagid}">{type}</button></td>' +
        '<td>{duration}</td>' +
        '</tr>'
        ;
    function formatnotifirow(properties) {
        return $.extend(properties, {
            id: properties.NOTIFICATIONGID,
            tagid: properties.TAGID,
            name: properties.NAME,
            type: properties.VEHICLENAME,
            condition: properties.CONDITIONS,
            duration: calculateDuration(properties.VEHICLETIME),
            conditioncolor: conditioncolor(properties.VEHICLETIME, parseInt(properties.WARNING), parseInt(properties.CRITICAL)),
            warning_action_text: properties.WARNING_ACTION,
            critical_action_text: properties.CRITICAL_ACTION,
            action_text: conditionaction_text(properties.VEHICLETIME, parseInt(properties.WARNING), parseInt(properties.CRITICAL)) + properties.NOTIFICATIONGID,
        });
    }
    $.each(notification, function () {
        $Table_Body.append($row_template.supplant(formatnotifirow(this)));
    });
}
function conditioncolor(time, war_min, crit_min) {
    if (checkValue(time)) {
        var conditiontime = moment(time);  // 5am PDT
        var curenttime = moment();
        if (conditiontime._isValid) {
            var d = moment.duration(curenttime.diff(conditiontime));
            var minutes = Math.floor(d._milliseconds / 60000);

            if (minutes < war_min && minutes < crit_min) {
                return "white";
            }
            else if (minutes >= war_min && minutes < crit_min) {
                return "#ffff0080";
            }
            else if (minutes >= crit_min) {
                return "#bd213052";
            }
        }
        else {
            return "";
        }
    }
}
function conditionaction_text(time, war_min, crit_min) {
    if (checkValue(time)) {
        var conditiontime = moment(time);  // 5am PDT
        var curenttime = moment();
        if (conditiontime._isValid) {
            var d = moment.duration(curenttime.diff(conditiontime));
            var minutes = Math.floor(d._milliseconds / 60000);
            if (minutes < war_min && minutes < crit_min) {
                return "W_";
            }
            if (minutes >= war_min && minutes < crit_min) {
                return "W_";
            }
            else if (minutes >= crit_min) {
                return "C_";
            }
        }
        else {
            return "NONE";
        }
    }
}
function calculateDuration(time) {
    if (checkValue(time)) {
        var conditiontime = moment(time);  // 5am PDT
        var curenttime = moment();
        if (conditiontime._isValid) {
            var d = moment.duration(curenttime.diff(conditiontime));
            return moment.duration(d._milliseconds, "milliseconds").format("d [days], h [hrs], m [min], s [sec], SS [ms]", {
                useSignificantDigits: true,
                trunc: true,
                precision: 3
            });
        }
        else {
            return "";
        }
    }
}