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
    AddNotification("notificationsetuptable");
});
let notification = [];
async function updateNotification(updatenotification) {
    try {
        let notificationindex = notification.filter(x => x.NotificationID === updatenotification.NotificationID).map(x => x).length;
        //add notification
        if (notificationindex === 0) {
            notification.push(updatenotification);
        }
        if (notificationindex > 0) {
            if (updatenotification.Delete) {
                    //delete notification 
                Promise.all([delete_notification(updatenotification.NotificationID)]);
            }
            else {
                //update notification
                Promise.all([update_notification(updatenotification.NotificationID)]);
            }
        }
       
        if (/vehicle/i.test(updatenotification.Type)) {
            Promise.all([updateagvTable(updatenotification)]);
        }
        if (/routetrip/i.test(updatenotification.Type)) {
            Promise.all([updatetripTable(updatenotification)]);
        }
        if (/mpe/i.test(updatenotification.Type)) {
            Promise.all([updateMPETable(updatenotification)]);
        }
        if (/dockdoor/i.test(updatenotification.Type)) {
            Promise.all([updateDockDoorTables(updatenotification)]);
        }
    }
    catch (e) {
        console.log(e);
    }
}
async function updateagvTable(updatenotification) {
    try {
        let Vehiclecount = notification.filter(x => x.Type === "vehicle").map(x => x).length;
        //AGV Counts
        if (Vehiclecount > 0) {
            if (parseInt($('#agvnotificaion_number').text()) !== Vehiclecount) {
                $('#agvnotificaion_number').text(Vehiclecount);
            }
        }
        else {
            $('#agvnotificaion_number').text("");
        }
        let findagvtrdataid = agvnotificationtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']');
        if (findagvtrdataid.length > 0) {
            if (updatenotification.Delete) {
                agvnotificationtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']').remove();
            }
            else {
                agvnotificationtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']').replaceWith(agv_row_template.supplant(formatagvnotifirow(updatenotification)));
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
        //let routetripcount = notification.filter(x => x.Type === "routetrip").map(x => x).length;
        let routetripcount = notification.filter(x => x.Type === "routetrip" || x.type === "dockdoor").map(x => x).length;
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
        let findtrdataid = tripsnotificationtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']');
        if (findtrdataid.length > 0) {
            if (updatenotification.Delete) {
                tripsnotificationtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']').remove();
                tripsnotificationtable_Body.find('tr[data-id=collapse_' + updatenotification.NotificationID + ']').remove();
            }
            else {
                tripsnotificationtable_Body.find('tr[data-id=collapse_' + updatenotification.NotificationID + ']').remove();
                tripsnotificationtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']').replaceWith(trip_row_template.supplant(formattripnotifirow(updatenotification)));
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
async function updateMPETable(updatenotification) {
    try {
        let mpeCounter = notification.filter(x => x.Type === "mpe" && x.Delete != true).map(x => x).map(x => x).length;
        if (mpeCounter > 0) {
            if (parseInt($('#mpenotification_number').text()) !== mpeCounter) {
                $('#mpenotification_number').text(mpeCounter);
            }
        }
        else {
            $('#mpenotification_number').text("");
        }
        let findmpedataid = mpenotificationtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']');
        if (findmpedataid.length > 0) {
            if (updatenotification.hasOwnProperty("DELETE")) {
                mpenotificationtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']').remove();
            }
            else {
                mpenotificationtable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']').replaceWith(mpe_row_template.supplant(formatmpenotifirow(updatenotification)));
            }
        }
        else {
            mpenotificationtable_Body.append(mpe_row_template.supplant(formatmpenotifirow(updatenotification)));
        }
        sortMPETable();
    }
    catch (e) {
        console.log(e);
    }
    return null;
}
async function updateDockDoorTables(updatenotification) {
    if (updatenotification.Name === "Load Scan After Departure") {
        updatedockdoorloadafterdeparttable(updatenotification);
    }
    return null;
}
async function updatedockdoorloadafterdeparttable(updatenotification) {
    try {
        let dockdoorandtripscount = notification.filter(x => x.Type === "routetrip" || x.type === "dockdoor").map(x => x).length;
        if (dockdoorandtripscount > 0) {
            if(parseInt($('#tripsnotificaion_number').text()) !== dockdoorandtripscount) {
                $('#tripsnotificaion_number').text(dockdoorandtripscount);
            }
            $('#tripsnotificaion_number').text(dockdoorandtripscount);
        }
        else {
            $('#tripsnotificaion_number').text("");
        }
        let findddladdataid = dockdoorloadafterdeparttable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']');
        if (findddladdataid.length > 0) {
            if (updatenotification.hasOwnProperty("DELETE")) {
                dockdoorloadafterdeparttable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']').remove();
            }
            //else {
            //    dockdoorloadafterdeparttable_Body.find('tr[data-id=' + updatenotification.NotificationID + ']').replaceWith(dockdoorloadafterdeparttable_row_template.supplant(formatdockdoorloadafterdeparttablerow(updatenotification)));
            //}
        }
        else {
            dockdoorloadafterdeparttable_Body.append(dockdoorloadafterdeparttable_row_template.supplant(formatdockdoorloadafterdeparttablerow(updatenotification)));
        }
    }
    catch (e) {
        console.log(e);
    }
    return null;
}
async function sortMPETable() {
    var table, rows, switching, i, x, y, shouldSwitch;
    table = document.getElementById("mpenotificationtable");
    switching = true;
    while (switching) {
        switching = false;
        rows = table.rows;
        for (i = 1; i < (rows.length - 1); i++) {
            shouldSwitch = false;

            var id = $(rows[i]).attr("data-id");
            var notificationrw = notification.filter(y => y.Type === "mpe" && y.id == id);

            if (notificationrw.length < 1 || notificationrw[0].hasOwnProperty("DELETE")) {
                mpenotificationtable_Body.find('tr[data-id=' + id + ']').remove();
            }
            else {
                x = rows[i].getElementsByTagName("TD")[3];
                y = rows[i + 1].getElementsByTagName("TD")[3];
                if (Number(x.innerHTML) < Number(y.innerHTML)) {
                    shouldSwitch = true;
                    break;
                }
            }
        }
        if (shouldSwitch) {
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;
        }
    }
}
let mpenotificationtable = $('table[id=mpenotificationtable]');
let mpenotificationtable_Body = mpenotificationtable.find('tbody');
async function delete_notification(id)
{
    try {
        notification.forEach(function (obj) {
            if (obj.NotificationID === id) {
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
            if (obj.NotificationID === id) {
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
    $.connection.FOTFManager.server.getNotification_ConditionsList().done(function (NotificationData) {
        notificationTable_Body.empty();
        $.each(NotificationData, function () {
            notificationTable_Body.append(notificationTable_row_template.supplant(formatnotificationrow(this)));
        });
        $('button[name=notificationedit]').on('click', function () {
            var td = $(this);
            var tr = $(td).closest('tr'),
                id = tr.attr('data-id');
            EditNotification(id, table);
        });
        $('button[name=notificationdelete]').on('click', function () {
            var td = $(this);
            var tr = $(td).closest('tr'),
                id = tr.attr('data-id');
            RemoveNotification(id, table);
        });
    });
}
let agv_row_template =
    '<tr data-id={id} style=background-color:{conditioncolor} data-toggle=collapse data-target=#{action_text} class=accordion-toggle>' +
    '<td><span class="ml-p25rem">{name}</span></td>' +
    '<td><button class="btn btn-outline-info btn-sm btn-block tagdetails" data-tag="{tagid}" >{type}</button></td>' +
    '<td>{duration}</td>' +
    '</tr>'
    ;
function formatagvnotifirow(properties, indx) {
    return $.extend(properties, {
        id: properties.NotificationID,
        tagid: properties.TypeID,
        name: properties.Name,
        type: properties.TypeName,
        condition: properties.Conditions,
        duration: calculatevehicleDuration(properties.TypeTime),
        conditioncolor: conditioncolor(properties.TIME, parseInt(properties.Warning), parseInt(properties.Critical)),
        warning_action_text: properties.WarningAction,
        critical_action_text: properties.CriticalAction,
        action_text: conditionaction_text(properties.vehicleTime, parseInt(properties.Warning), parseInt(properties.Critical)) + "_" + properties.notificationId,
        indexobj: indx
    });
}
let trip_row_template =
    '<tr data-id={id} class="accordion-toggle collapsed" id={id} data-toggle=collapse data-parent=#{id} href="#collapse_{id}">' +
  
    '<td><span class="ml-p25rem">{schd}</span></td>' +
    '<td>{duration}</td>' +
    '<td>{direction}</td>' +
    '<td>' +
    '<button class="btn btn-outline-info btn-sm btn-block px-1 routetripdetails" data-routetrip="{routetripid}" style="font-size:12px;">{routedispaly}</button>' +
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
        id: properties.NotificationID,
        tagid: properties.TypeID,
        schd: moment(properties.TypeTime).format("HH:mm"),
        routetripid: properties.TypeID,
        routedispaly: spitName(properties.TypeName, 0),
        trip: properties.trip,
        direction: properties.TypeID.substr(properties.TypeID.length - 1),
        leg: properties.legSiteId,
        dest: spitName(properties.TypeName, 1),
        condition: properties.Conditions,
        duration: calculatevehicleDuration(properties.TypeTime),
        conditioncolor: conditioncolor(properties.VEHICLETIME, parseInt(properties.Warning), parseInt(properties.Critical)),
        warning_action_text: properties.WarningAction,
        critical_action_text: properties.CriticalAction,
        indexobj: indx
    });
}
let mpe_row_template =
    '<tr data-id={id} style=background-color:{conditioncolor} data-toggle=collapse data-target=#{action_text} class=accordion-toggle>' +
    '<td><span class="ml-p25rem">{name}</span></td>' +
    '<td><button class="btn btn-outline-info btn-sm btn-block machinedetails" data-machine="{zoneid}" >{mpeName}</button></td>' +
    '<td style="text-align:center">{duration}</td>' +
    '<td style="display:none;">{durationtime}</td>' +
    '</tr>'
    ;
function formatmpenotifirow(properties, indx) {
    return $.extend(properties, {
        id: properties.NotificationID,
        mpeName: properties.TypeName,//properties.mpe_type + "-" + properties.mpe_number.padStart(3, "0"),
        zoneid: properties.TypeID,
        name: properties.Name,
        //type: properties.Type,
        //condition: properties.Conditions,
        duration: ConverMPENotificationTime(properties.TypeDuration),
        durationtime: properties.TypeDuration,
        conditioncolor: conditioncolor(GetMPENotificationTime(properties.TypeDuration), parseInt(properties.Warning), parseInt(properties.Critical)),
        warning_action_text: properties.WarningAction,
        critical_action_text: properties.CriticalAction,
        action_text: conditionaction_text(GetMPENotificationTime(properties.TypeDuration), parseInt(properties.Warning), parseInt(properties.Critical)) + "_" + properties.notificationId,
        indexobj: indx
    });
}
let dockdoorloadafterdeparttable = $('table[id=loadafterdeparttable]');
let dockdoorloadafterdeparttable_Body = dockdoorloadafterdeparttable.find('tbody');
let dockdoorloadafterdeparttable_row_template = '<tr data-id={id} style=background-color:{conditioncolor} class="accordion-toggle collapsed" id={id} data-toggle=collapse data-parent=#{id} href="#collapse_{id}">' +
    //'<td><button class="btn btn-outline-info btn-sm btn-block px-1 containerdetails" data-placardid={placard}" style="font-size:12px;" >{placard}</button ></td>' +
    '<td colspan="2" data-input="placard" class="text-center">' +
    '<a data-doorid={zone_id} data-placardid={placard} class="containerdetails">{placard}</a>' +
    //'<a data-doorid={zone_id} data-placardid={placard} class="containerdetails" title={placard}>{placard}</a>' +
    '</td>' +
    '<td><a title={loadscan}>{loadscan}</a></td>' +
    '<td colspan="2"><a title={trailer}>{trailer}</a></td>' +
    '<td><a title={departscan}>{departscan}</a></td>' +
    '<td class="expand-button">' +
    '<a class="btn btn-link d-flex justify-content-end" data-toggle="collapse" href="#collapse_{id}" role="button" aria-expanded="false" aria-controls="collapseSection">' +
    '<div class="iconXSmall">' +
    ' <i class="pi-iconCaretDownFill" />' +
    '</div>' +
    '</a>' +
    '</td>' +
    '</tr>' +
    '<tr data-id=collapse_{id} class="hide-table-padding">' +
    '<td colspan="7">' +
    '<div class="collapse" id="collapse_{id}">' +
    '<div class="mt-1">' +
    '<ol class="pl-4 mb-0">' +
    '<p class="pb-1">Placard: {placard}</br>Load Scan: {loadscan}</br>Trailer: {trailer}</br>Depart Scan: {departscan}</p> ' +
    '<p class="pb-1">{action_text}</p> ' +
    '</ol>' +
    '</div>' +
    '</div>' +
    '</td>' +
    '</tr>';
function formatdockdoorloadafterdeparttablerow(properties, indx) {
    return $.extend(properties, {
        id: properties.NotificationID,
        placard: properties.TypeID,
        loadscan: parseloadafterdeparttypename(properties.TypeName, 'loadscan'),
        trailer: parseloadafterdeparttypename(properties.TypeName, 'trailer'),
        departscan: parseloadafterdeparttypename(properties.TypeName, 'departscan'),
        action_text: getLADactiontext(properties),
        conditioncolor: getLADconditioncolor(properties),
        indexobj: indx,
        zone_id: "0"
    });
}
function getLADactiontext(properties) {
    if (properties.TypeStatus == 'Warning') { return properties.WarningAction; }
    if (properties.TypeStatus == 'Critical') { return properties.CriticalAction; }
    return "";
}
function getLADconditioncolor(properties) {
    if (properties.status == 'Warning') { return "#ffff0080" }
    if (properties.status == 'Critical') { return "#bd213052"; }
    return "white";
}
function parseloadafterdeparttypename(typename, name) {
    var typenames = typename.split('|', 4);
    if (name == 'loadscan') {
        return typenames[2];
    }
    if (name == 'trailer') {
        return typenames[1];
    }
    if (name == 'departscan') {
        return typenames[3]
    }
}
let notificationTable = $('table[id=notificationsetuptable]');
let notificationTable_Body = notificationTable.find('tbody');
let notificationTable_row_template = '<tr data-id="{id}" class="{button_collor}">' +
    '<td><span class="ml-p5rem">{name}</span></td>' +
    '<td>{warning}</td>' +
    '<td>{critical}</td>' +
    '<td>{status}</td>' +
    '<td>' +
    '<button class="btn btn-light btn-sm mx-1 pi-iconEdit" name="notificationedit"></button>' +
    '<button class="btn btn-light btn-sm mx-1 pi-trashFill" name="notificationdelete"></button>' +
    '</td>' +
    '</tr>';

function formatnotificationrow(properties) {
    return $.extend(properties, {
        id: properties.Id,
        name: properties.Name,
        type: properties.Type,
        condition: properties.Conditions,
        warning: properties.Warning,
        warning_action: properties.WarningAction,
        warning_color: properties.WarningColor,
        critical: properties.Critical,
        critical_action: properties.CriticalAction,
        critical_color: properties.CriticalColor,
        status: GetnotificationStatus(properties),
        button_collor: Get_notificationColor(properties)
    })
}
async function AddNotification(table) {
    $('.warning_conditionpickvalue').html(0);
    $('input[id=warning_condition]').val(0);
    $('.critical_conditionpickvalue').html(0);
    $('input[id=critical_condition]').val(0);
    $('#notification_SetupHeader').text('Add New Notification');
    sidebar.close('notificationsetup');
    $('#notificationsubmitBtn').css("display", "block");
    $('#editnotificationsubmitBtn').css("display", "none");
    $('#Notification_Setup_Modal').modal();
    $('button[id=notificationsubmitBtn]').prop('disabled', false);
    $('button[id=notificationsubmitBtn]').off().on('click', function () {
        $('button[id=notificationsubmitBtn]').prop('disabled', true);
        var jsonObject = {
            CreatedByUsername: User.UserId,
            ActiveCondition: $('input[type=checkbox][name=condition_active]')[0].checked,
            Name: $('input[type=text][name=condition_name]').val(),
            Type: $('select[name=condition_type] option:selected').val(),
            Conditions: $('input[type=text][name=condition]').val(),
            Warning: $('input[id=warning_condition]').val(),
            Critical: $('input[id=critical_condition]').val(),
            WarningAction: $('textarea[id=warning_action]').val(),
            CriticalAction: $('textarea[id=critical_action]').val()
        };
        $.connection.FOTFManager.server.addNotification_Conditions(JSON.stringify(jsonObject)).done(function (Data) {
            if (Data.length === 0) {
                $('span[id=error_notificationsubmitBtn]').text("Error Adding Condition");
                setTimeout(function () { $("#Notification_Setup_Modal").modal('hide'); }, 1000);
            }
            else {
                $('span[id=error_notificationsubmitBtn]').text("Condition has been Added");
                LoadNotificationsetup(Data, table);
                setTimeout(function () {$("#Notification_Setup_Modal").modal('hide'); sidebar.open('notificationsetup');}, 1000);
            }
        });
    });
}
async function EditNotification(Id, table) {

    $('#notificationsubmitBtn').css("display", "none");
    $('#editnotificationsubmitBtn').css("display", "block");
    $('button[id=editnotificationsubmitBtn]').prop('disabled', false);
    $('#notification_SetupHeader').text('Edit Notification');
    sidebar.close('notificationsetup');
    try {
        $.connection.FOTFManager.server.getNotification_Conditions(Id).done(function (N_Data) {
            if (N_Data.length > 0) {
                var NotificationData = N_Data[0];
                if (!$.isEmptyObject(NotificationData)) {
                
                    if (NotificationData.ActiveCondition) {
                        if (!$('input[type=checkbox][name=condition_active]')[0].checked) {
                            $('input[type=checkbox][name=condition_active]').prop('checked', true).change();
                        }
                    }
                    else {
                        if ($('input[type=checkbox][name=condition_active]')[0].checked) {
                            $('input[type=checkbox][name=condition_active]').prop('checked', false).change();
                        }
                    }
                    $('input[type=text][name=condition_name]').val(NotificationData.Name);
                    $('select[name=condition_type]').val(NotificationData.Type)
                    $('input[type=text][name=condition]').val(NotificationData.Conditions);
                    $('.warning_conditionpickvalue').html(NotificationData.Warning);
                    $('input[id=warning_condition]').val(NotificationData.Warning);
                    $('.critical_conditionpickvalue').html(NotificationData.Critical);
                    $('input[id=critical_condition]').val(NotificationData.Critical);
                    $('textarea[id=warning_action]').val(NotificationData.WarningAction);
                    $('textarea[id=critical_action]').val(NotificationData.CriticalAction);
                }

                $('button[id=editnotificationsubmitBtn]').off().on('click', function () {
                    $('button[id=editnotificationsubmitBtn]').prop('disabled', true);
                    var jsonObject = {
                        Id: Id,
                        LastupdateByUsername: User.UserId,
                        ActiveCondition: $('input[type=checkbox][name=condition_active]')[0].checked,
                        Name: $('input[type=text][name=condition_name]').val(),
                        Type: $('select[name=condition_type] option:selected').val(),
                        Conditions: $('input[type=text][name=condition]').val(),
                        Warning: $('input[id=warning_condition]').val(),
                        Critical: $('input[id=critical_condition]').val(),
                        WarningAction: $('textarea[id=warning_action]').val(),
                        CriticalAction: $('textarea[id=critical_action]').val()
                    };
                    $.connection.FOTFManager.server.editNotification_Conditions(JSON.stringify(jsonObject)).done(function (Data) {
                        if (Data.length === 0) {
                            $('span[id=error_notificationsubmitBtn]').text("Unable to loaded Condition");
                            setTimeout(function () { $("#Notification_Setup_Modal").modal('hide'); }, 3000);
                        }
                        else {
                            $('span[id=error_notificationsubmitBtn]').text("Condition has been Edited");
                            LoadNotificationsetup(Data, table);
                            setTimeout(function () { $("#Notification_Setup_Modal").modal('hide'); sidebar.open('notificationsetup'); }, 1000);
                        }
                    });
                })
                $('#Notification_Setup_Modal').modal();
            }
            else {
               //Add error reason     
            }
        });
    } catch (e) {
        console.log(e);
    }
}
async function RemoveNotification(id, table) {
    //RemoveNotificationModal
    sidebar.close('notificationsetup');
    $('#removeNotificationHeader').text('Remove Notification');
    $('button[id=removeNotification]').off().on('click', function () {
        var jsonObject = { Id: id };
        $.connection.FOTFManager.server.deleteNotification_Conditions(JSON.stringify(jsonObject)).done(function (Data) {
            if (Data.length === 0) {
                $('span[id=error_notificationsubmitBtn]').text("Unable to loaded Condition");
                setTimeout(function () { $("#Notification_Setup_Modal").modal('hide'); }, 3000);
            }
            else {
                $('span[id=error_notificationsubmitBtn]').text("Condition has been Edited");
                LoadNotificationsetup(Data, table);
                setTimeout(function () { $("#RemoveNotificationModal").modal('hide'); sidebar.open('notificationsetup'); }, 1000);
         
            }
        })
    });
    $('#RemoveNotificationModal').modal();

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
        var conditiontime = moment(t);//moment(time);  // 5am PDT
        var curenttime = moment().tz(timezone.Facility_TimeZone);
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
function GetnotificationStatus(data) {
    if (data.ActiveCondition) {
        return "Active";
    }
    else {
        return "Disabled";
    }
}
function Get_notificationColor(data) {
    if (data.ActiveCondition) {
        return "table-success";
    }
    else {
        return "table-warning";
    }
}
function spitName(name, index) {
    try {
        var tempName = name.split("|");
        return tempName[index];
    } catch (e) {
        console.log(e);
    }
}
function ConverMPENotificationTime(secs) {
    try {
        if (secs != 0) {
            var sec_num = parseInt(secs, 10)
            var hours = Math.floor(sec_num / 3600)
            var minutes = Math.floor(sec_num / 60) % 60
            var seconds = sec_num % 60

            return [hours, minutes, seconds]
                .map(v => v < 10 ? "0" + v : v)
                .filter((v, i) => v !== "00" || i > 0)
                .join(":")
        }
        return "";
    } catch (e) {
        console.log(e);
    }
}
function GetMPENotificationTime(secs) {
    if (secs != 0) {
        return moment().subtract(secs, 'seconds');
    }
    
    return moment().add(1, 'minutes');
}
