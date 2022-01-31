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
            var routetripcount = -0;
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
                if (/routetrip/i.test(updatenotification.TYPE)) {
                    var triptabvisible = sidebar._getTab("tripsnotificationinfo");
                    if (triptabvisible) {
                        if (triptabvisible.classList.length) {
                            if (triptabvisible.classList.contains('active')) {
                                Table = $('table[id=tripsnotificationtable]');
                                if (Table.length > 0) {
                                    let Table_Body = Table.find('tbody');
                                    let findtrdataid = Table_Body.find('tr[data-id=' + updatenotification.NOTIFICATIONGID + ']');
                                    if (findtrdataid.length > 0) {
                                        if (updatenotification.hasOwnProperty("DELETE")) {
                                            Table_Body.find('tr[data-id=' + updatenotification.NOTIFICATIONGID + ']').remove();
                                        }
                                        else {

                                            Table_Body.find('tr[data-id=' + updatenotification.NOTIFICATIONGID + ']').replaceWith(routetriprow_template.supplant(formatroutetripnotifirow(updatenotification)));
                                        }
                                    }
                                    else {
                                        Table_Body.append(routetriprow_template.supplant(formatroutetripnotifirow(updatenotification)));
                                    }
                                }
                            }
                        }
                    }
                }
                if (/vehicle/i.test(updatenotification.TYPE)) {
                    var agvtabvisible = sidebar._getTab("agvnotificationinfo");
                    if (agvtabvisible) {
                        if (agvtabvisible.classList.length) {
                            if (agvtabvisible.classList.contains('active')) {
                                Table = $('table[id=agvnotificationtable]');
                                if (Table.length > 0) {
                                    let Table_Body = Table.find('tbody');
                                    let findagvtrdataid = Table_Body.find('tr[data-id=' + updatenotification.NOTIFICATIONGID + ']');
                                    if (findagvtrdataid.length > 0) {
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
                                }
                            }
                        }
                    }
                }
            }


            Vehiclecount = notification.filter(x => x.TYPE === "vehicle").map(x => x).length
            routetripcount = notification.filter(x => x.TYPE === "routetrip").map(x => x).length
            //machinecount = notification.filter(x => x.TYPE === "machine").map(x => x).length

            //AGV Counts
            if (Vehiclecount > 0) {
                if (parseInt($('#agvnotificaion_number').text()) !== Vehiclecount) {
                    $('#agvnotificaion_number').text(Vehiclecount);
                }
            }
            else {
                $('#agvnotificaion_number').text("");
            }
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
            //machine counts
            //if (machinecount > 0) {
            //    $('#machinenotificaion_number').text(Vehiclecount);
            //}
            //else {
            //    $('#machinenotificaion_number').text("");
            //}
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
let routetriprow_template =
'<tr data-id={id} style=background-color:{conditioncolor} data-toggle=collapse  class=accordion-toggle>' +
    '<td class="text-center">{schd}</td>' +
    '<td class="text-center">{duration}</td>' +
    '<td class="text-center">{direction}</td>' +
    '<td>' +
    '<button class="btn btn-outline-info btn-sm btn-block px-1 routetripdetails" data-routetrip="{routetripid}" style="font-size:12px;">{route}-{trip}</button>' +
    '</td> ' +
    '<td data-toggle="tooltip" title="{dest}">{dest}</td>' +
    '</tr>"';
function formatroutetripnotifirow(properties, indx) {
    return $.extend(properties, {
        id: properties.NOTIFICATIONGID,
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
                    $('input[id=warning_condition]').val($.isNumeric(N_Data[0].WARNING) ? parseInt(N_Data[0].WARNING) : 0);
                    $('.critical_conditionpickvalue').html($.isNumeric(N_Data[0].CRITICAL) ? parseInt(N_Data[0].CRITICAL) : 0);
                    $('input[id=critical_condition]').val($.isNumeric(N_Data[0].CRITICAL) ? parseInt(N_Data[0].CRITICAL) : 0);
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
