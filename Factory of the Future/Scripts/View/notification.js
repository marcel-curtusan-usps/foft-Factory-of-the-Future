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
            $('span[id=error_condition_type]').text("Please Select Condition Type");
        }
        else {
            $('select[name=condition_type]').removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_condition_type]').text("");
        }
        enableNotificationSubmit();
    });
    if ($('select[name=condition_type]').val() === "") {
        $('select[name=condition_type]').removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_condition_type]').text("Please Select Condition Type");
    }
});
$('button[name=addnotificationsetup]').off().on('click', function () {
    /* close the sidebar */
    sidebar.close();
    Notificationsetup({}, "notificationsetuptable");
});
let notification = [];
async function updateNotification(updatenotification) {
    try {
        let notificationindex = notification.filter(x => x.notificationId === updatenotification.notificationId).map(x => x).length;
        //add notification
        if (notificationindex === 0) {
            notification.push(updatenotification);
        }
        if (notificationindex > 0) {
            if (updatenotification.hasOwnProperty("DELETE")) {
                if (updatenotification.DELETE) {
                    //delete notification 
                    Promise.all([delete_notification(updatenotification.notificationId)]);
                }
            }
            else {
                //update notification
                Promise.all([update_notification(updatenotification.notificationId)]);
            }
        }
       
        if (/vehicle/i.test(updatenotification.TYPE)) {
            Promise.all([updateagvTable(updatenotification)]);
        }
        if (/routetrip/i.test(updatenotification.TYPE)) {
            Promise.all([updatetripTable(updatenotification)]);
        }
    }
    catch (e) {
        console.log(e);
    }
}
async function updateagvTable(updatenotification) {
    try {
        let Vehiclecount = notification.filter(x => x.TYPE === "vehicle").map(x => x).length;
        //AGV Counts
        if (Vehiclecount > 0) {
            if (parseInt($('#agvnotificaion_number').text()) !== Vehiclecount) {
                $('#agvnotificaion_number').text(Vehiclecount);
            }
        }
        else {
            $('#agvnotificaion_number').text("");
        }
        let findagvtrdataid = agvnotificationtable_Body.find('tr[data-id=' + updatenotification.notificationId + ']');
        if (findagvtrdataid.length > 0) {
            if (updatenotification.hasOwnProperty("DELETE")) {
                agvnotificationtable_Body.find('tr[data-id=' + updatenotification.notificationId + ']').remove();
            }
            else {
                agvnotificationtable_Body.find('tr[data-id=' + updatenotification.notificationId + ']').replaceWith(agv_row_template.supplant(formatagvnotifirow(updatenotification)));
            }
        }
        else {
            agvnotificationtable_Body.append(agv_row_template.supplant(formatagvnotifirow(updatenotification)));
        }
    } catch (e) {
        console.log(e);
    }
    return null;
}
async function updatetripTable(updatenotification) {
    try {
        let routetripcount = notification.filter(x => x.TYPE === "routetrip").map(x => x).length;
        // routetrip Counts
        if (routetripcount > 0) {
            if (parseInt($('#tripsnotificaion_number').text()) !== routetripcount) {
                $('#tripsnotificaion_number').text(routetripcount);
            }
            $('#ctsnotificaion_number').text(routetripcount);
        }
        else {
            $('#tripsnotificaion_number').text("");
        }
        let findtrdataid = tripsnotificationtable_Body.find('tr[data-id=' + updatenotification.notificationId + ']');
        if (findtrdataid.length > 0) {
            if (updatenotification.hasOwnProperty("DELETE")) {
                tripsnotificationtable_Body.find('tr[data-id=' + updatenotification.notificationId + ']').remove();
                tripsnotificationtable_Body.find('tr[data-id=collapse_' + updatenotification.notificationId + ']').remove();
            }
            else {
                tripsnotificationtable_Body.find('tr[data-id=' + updatenotification.notificationId + ']').remove();
                tripsnotificationtable_Body.find('tr[data-id=collapse_' + updatenotification.notificationId + ']').remove();
                tripsnotificationtable_Body.append(trip_row_template.supplant(formattripnotifirow(updatenotification)));
            }
        }
        else {
            tripsnotificationtable_Body.append(trip_row_template.supplant(formattripnotifirow(updatenotification)));
        }
    } catch (e) {
        console.log(e);
    }
    return null;
}
async function delete_notification(id)
{
    try {
        notification.forEach(function (obj) {
            if (obj.notificationId === id) {
                notification.splice(notification.indexOf(obj), 1);
            }
        });
        return null;
    } catch (e) {
        console.log(e);
    }
  
}
async function update_notification(id) {
    try {
        notification.forEach(function (obj) {
            if (obj.notificationId === id) {
                let indexobj = notification.indexOf(obj);
            }
        });
        return null;
    } catch (e) {
        console.log(e);
    }

}
let agvnotificationtable = $('table[id=agvnotificationtable]');
let agvnotificationtable_Body = agvnotificationtable.find('tbody');
function LoadNotification(value) {
    try {
     
        $.connection.FOTFManager.server.getNotification(value).done(function (Data) {
            $.each(Data, function () {
                updateNotification(this);
            });
        });
    } catch (e) {
        console.log(e);
    }
}
function LoadNotificationsetup(Data, table) {
    $.connection.FOTFManager.server.getNotification_ConditionsList("").done(function (NotificationData) {
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
let agv_row_template =
    '<tr data-id={id} style=background-color:{conditioncolor} data-toggle=collapse data-target=#{action_text} class=accordion-toggle>' +
    '<td>{name}</td>' +
    '<td><button class="btn btn-outline-info btn-sm btn-block tagdetails" data-tag="{tagid}" >{type}</button></td>' +
    '<td>{duration}</td>' +
    '</tr>'
    ;
function formatagvnotifirow(properties, indx) {
    return $.extend(properties, {
        id: properties.notificationId,
        tagid: properties.TAGID,
        name: properties.NAME,
        type: properties.name,
        condition: properties.CONDITIONS,
        duration: calculatevehicleDuration(properties.vehicleTime),
        conditioncolor: conditioncolor(properties.TIME, parseInt(properties.WARNING), parseInt(properties.CRITICAL)),
        warning_action_text: properties.WARNING_ACTION,
        critical_action_text: properties.CRITICAL_ACTION,
        action_text: conditionaction_text(properties.vehicleTime, parseInt(properties.WARNING), parseInt(properties.CRITICAL)) + "_" + properties.notificationId,
        indexobj: indx
    });
}
let trip_row_template =
    '<tr data-id={id} class="accordion-toggle collapsed" id={id} data-toggle=collapse data-parent=#{id} href="#collapse_{id}">' +
  
    '<td class="text-center">{schd}</td>' +
    '<td class="text-center">{duration}</td>' +
    '<td class="text-center">{direction}</td>' +
    '<td>' +
    '<button class="btn btn-outline-info btn-sm btn-block px-1 routetripdetails" data-routetrip="{routetripid}" style="font-size:12px;">{route}-{trip}</button>' +
    '</td>' +
    '<td data-toggle="tooltip" title="{dest}" style="overflow: inherit;">{dest}</td> ' +
    '<td class="expand-button">' +
    '<a class="btn btn-link d-flex justify-content-end" data-toggle="collapse" href="#collapse_{id}" role="button" aria-expanded="false" aria-controls="collapseSection">' +
    '<div class="iconXSmall">' +
    ' <i class="pi-iconCaretDownFill" />' +
    '</div>' +
    '</a>' +
    '</td>' +
    '</tr>' +
    '<tr data-id=collapse_{id} class="hide-table-padding">' +
        '<td colspan="6">'+
            '<div class="collapse" id="collapse_{id}">' +
            '<div class="mt-1">' +
            '<ol class="pl-4 mb-0">' +
            '<p class="pb-1">{warning_action_text}</p> ' +
            '</ol>' +
            '</div>' +
            '</div>' +
        '</td>' +
    '</tr>'
    ;
let tripsnotificationtable = $('table[id=tripsnotificationtable]');
let tripsnotificationtable_Body = tripsnotificationtable.find('tbody');
function formattripnotifirow(properties, indx) {
    return $.extend(properties, {
        id: properties.notificationId,
        tagid: properties.TAGID,
        schd: objSVTime(properties.scheduledDtm),
        routetripid: properties.id,
        route: properties.route,
        trip: properties.trip,
        direction: properties.tripDirectionInd,
        leg: properties.legSiteId,
        dest: properties.legSiteName,
        condition: properties.CONDITIONS,
        door: properties.tripDirectionInd === "I" ? "" : properties.hasOwnProperty("doorNumber") ? properties.doorNumber.replace(/^0+/, '') :"",
        duration: calculateDuration(properties.scheduledDtm),
        conditioncolor: conditioncolor(properties.VEHICLETIME, parseInt(properties.WARNING), parseInt(properties.CRITICAL)),
        warning_action_text: properties.WARNING_ACTION,
        critical_action_text: properties.CRITICAL_ACTION,
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
            var jsonObject = {
                CREATED_BY_USERNAME: User.UserId,
                ACTIVE_CONDITION: $('input[type=checkbox][name=condition_active]')[0].checked
            };

            //condition active
            checkValue($('input[type=text][name=condition_name]').val()) ? jsonObject.NAME = $('input[type=text][name=condition_name]').val() : '';
            checkValue($('select[name=condition_type] option:selected').val()) ? jsonObject.TYPE = $('select[name=condition_type] option:selected').val() : '';
            checkValue($('input[type=text][name=condition]').val()) ? jsonObject.CONDITIONS = $('input[type=text][name=condition]').val() : '';
            checkValue($('input[id=warning_condition]').val()) ? jsonObject.WARNING = $('input[id=warning_condition]').val() : '';
            checkValue($('input[id=critical_condition]').val()) ? jsonObject.CRITICAL = $('input[id=critical_condition]').val() : '';
            checkValue($('textarea[id=warning_action]').val()) ? jsonObject.WARNING_ACTION = $('textarea[id=warning_action]').val() : '';
            checkValue($('textarea[id=critical_action]').val()) ? jsonObject.CRITICAL_ACTION = $('textarea[id=critical_action]').val() : '';
            if (!$.isEmptyObject(jsonObject)) {
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
            }
        });
    }
    else {
        $('#notificationsubmitBtn').css("display", "none");
        $('#editnotificationsubmitBtn').css("display", "block");
        try {
            $.connection.FOTFManager.server.getNotification_ConditionsList(data).done(function (N_Data) {
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
                    $('input[id=warning_condition]').val($.isNumeric(N_Data[0].WARNING) ? parseInt(N_Data[0].WARNING) : 0);
                    $('.critical_conditionpickvalue').html($.isNumeric(N_Data[0].CRITICAL) ? parseInt(N_Data[0].CRITICAL) : 0);
                    $('input[id=critical_condition]').val($.isNumeric(N_Data[0].CRITICAL) ? parseInt(N_Data[0].CRITICAL) : 0);
                    $('textarea[id=warning_action]').val(N_Data[0].WARNING_ACTION);
                    $('textarea[id=critical_action]').val(N_Data[0].CRITICAL_ACTION);
                }
                $('button[id=editnotificationsubmitBtn]').prop('disabled', false);
                $('button[id=editnotificationsubmitBtn]').off().on('click', function () {
                    $('button[id=editnotificationsubmitBtn]').prop('disabled', true);
                    var jsonObject = {
                        ACTIVE_CONDITION: $('input[type=checkbox][name=condition_active]')[0].checked,
                        id: data,
                        LASTUPDATE_BY_USERNAME: User.UserId
                    };
                    $('input[type=text][name=condition_name]').val() !== jsonObject.NAME ? jsonObject.NAME = $('input[type=text][name=condition_name]').val() : '';
                    $('select[name=condition_type] option:selected').val() !== jsonObject.TYPE ? jsonObject.TYPE = $('select[name=condition_type] option:selected').val() : '';
                    $('input[type=text][name=condition]').val() !== jsonObject.CONDITIONS ? jsonObject.CONDITIONS = $('input[type=text][name=condition]').val() : '';
                    $('input[id=warning_condition]').val() !== jsonObject.WARNING ? jsonObject.WARNING = $('input[id=warning_condition]').val() : '';
                    $('input[id=critical_condition]').val() !== jsonObject.CRITICAL ? jsonObject.CRITICAL = $('input[id=critical_condition]').val() : '';
                    $('textarea[id=warning_action]').val() !== jsonObject.WARNING_ACTION ? jsonObject.WARNING_ACTION = $('textarea[id=warning_action]').val() : '';
                    $('textarea[id=critical_action]').val() !== jsonObject.CRITICAL_ACTION ? jsonObject.CRITICAL_ACTION = $('textarea[id=critical_action]').val() : '';
                    if (!$.isEmptyObject(jsonObject)) {
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
        } catch (e) {
            console.log(e);
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
function calculateDuration(t) {
    if (checkValue(t)) {
        var conditiontime = moment().tz(timezone.Facility_TimeZone).set({ 'year': t.year, 'month': t.month, 'date': t.dayOfMonth, 'hour': t.hourOfDay, 'minute': t.minute, 'second': t.second });//moment(time);  // 5am PDT
        var curenttime = moment();
        if (conditiontime._isValid) {
            var d = moment.duration(curenttime.diff(conditiontime));
            return moment.duration(d._milliseconds, "milliseconds").format("d [days], h [hrs], m [min]", {
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
function calculatevehicleDuration(t) {
    if (checkValue(t)) {
        var conditiontime = moment(t).tz(timezone.Facility_TimeZone);//moment(time);  // 5am PDT
        var curenttime = moment();
        if (conditiontime._isValid) {
            var d = moment.duration(curenttime.diff(conditiontime));
            return moment.duration(d._milliseconds, "milliseconds").format("d [days], h [hrs], m [min]", {
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
function enableNotificationSubmit() {
    if ($('input[type=text][name=condition_name]').hasClass('is-valid') &&
        $('input[type=text][name=condition]').hasClass('is-valid') &&
        $('select[name=condition_type]').hasClass('is-valid')
    ) {
        $('button[id=notificationsubmitBtn]').prop('disabled', false);
    }
    else {
        $('button[id=notificationsubmitBtn]').prop('disabled', true);
    }
}
