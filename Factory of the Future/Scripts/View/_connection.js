﻿/**/
let connection_Table;
let connection_Table_Body;
$.extend(fotfmanager.client, {
    updateQSMStatus: async (Connectionupdate) => { updateConnection(Connectionupdate) }
});
//on close clear all inputs
$('#API_Connection_Modal').on('hidden.bs.modal', function () {
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
});

function enablewsSubmit() {
    if ($('input[type=text][name=ip_address]').hasClass('is-valid') &&
        $('input[type=text][name=url]').hasClass('is-valid') &&
        $('input[type=text][name=message_type]').hasClass('is-valid') &&
        $('input[type=text][name=port_number]').hasClass('is-valid') &&
        $('input[type=text][name=connection_name]').hasClass('is-valid')
    ) {
        $('button[id=apisubmitBtn]').prop('disabled', false);
    }
    else {
        $('button[id=apisubmitBtn]').prop('disabled', true);
    }
}
function onUpdateWS() {
    if ($('input[type=checkbox][name=ws_connection]').is(':checked')) {
        $('input[type=text][name=hostanme]').prop('disabled', true);
        $('input[type=text][name=hostanme]').val('');
        $('input[type=text][name=hostanme]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
        $('span[id=error_hostanme]').text("");

        $('select[name=data_retrieve]').val(' ');
        $('select[name=data_retrieve]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
        $('span[id=error_data_retrieve]').text("");
        $('select[name=data_retrieve]').prop("disabled", true);

        $('input[type=checkbox][name=udp_connection]').prop("checked", false);
        $('input[type=checkbox][name=tcpip_connection]').prop("checked", false);

        $('input[type=text][name=ip_address]').prop("disabled", false);
        $('input[type=text][name=url]').prop("disabled", false);
        $('input[type=text][name=message_type]').prop("disabled", false);
        $('input[type=text][name=port_number]').prop("disabled", false);
/*        $('input[type=text][name=admin_email_recepient]').prop("disabled", false);*/
        $('input[type=text][name=connection_name]').prop("disabled", false);

        $('input[type=text][name=url]').prop("disabled", false);
        if (!checkValue($('input[type=text][name=connection_name]').val())) {
            $('input[type=text][name=connection_name]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_connection_name]').text("Please Enter Connection Name");
        }
        else {
            $('input[type=text][name=connection_name]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_connection_name]').text("");
        }
        
        if (!checkValue($('input[type=text][name=port_number]').val())) {
            $('input[type=text][name=port_number]').css({ "border-color": "#FF0000" }).removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_port_number]').text("Please Enter Port Number");
        }
        else {
            $('input[type=text][name=port_number]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_port_number]').text("");
        }
        if (!checkValue($('input[type=text][name=url]').val())) {
            $('input[type=text][name=url]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_url]').text("Please Enter API URL");
        } else {
            $('input[type=text][name=url]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_url]').text("");
        }

        $('input[type=text][name=hostanme]').prop("disabled", false);
        //
        if ($('input[type=checkbox][name=ws_connection]').is(':checked')) {

        }
        else if (!checkValue($('select[name=data_retrieve] option:selected').val())) {
            $('select[name=data_retrieve]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_data_retrieve]').text("Select Data Retrieve Occurrences");
        }
        else {
            $('select[name=data_retrieve]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_data_retrieve]').text("");
        }
        $('input[type=text][name=ip_address]').prop("disabled", false);
        if (!checkValue($('input[type=text][name=ip_address]').val())) {
            $('input[type=text][name=ip_address]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_ip_address]').text("Please Enter Valid IP address");
        }
        else {
            $('input[type=text][name=ip_address]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_ip_address]').text("");
        }
        enablewsSubmit();
    }
    if ($('input[type=checkbox][name=udp_connection]').is(':checked')) {
        enableudpSubmit();
    }
    if ($('input[type=checkbox][name=tcpip_connection]').is(':checked')) {
        enabletcpipSubmit();
    }

    if (!$('input[type=checkbox][name=udp_connection]').is(':checked') &&
        !$('input[type=checkbox][name=ws_connection]').is(':checked') &&
        !$('input[type=checkbox][name=tcpip_connection]').is(':checked')) {
        onRegularConnection();
    }
}




//on open set rules
$('#API_Connection_Modal').on('shown.bs.modal', function () {
    $('span[id=error_apisubmitBtn]').text("");
    $('button[id=apisubmitBtn]').prop('disabled', true);

    $('.hoursforwardvalue').html($('input[id=hoursforward_range]').val());
    $('input[id=hoursforward_range]').on('input change', () => {
        $('.hoursforwardvalue').html($('input[id=hoursforward_range]').val());
    });
    $('.hoursbackvalue').html($('input[id=hoursback_range]').val());
    $('input[id=hoursback_range]').on('input change', () => {
        $('.hoursbackvalue').html($('input[id=hoursback_range]').val());
    });
    //$('.hoursforwardvalue').html($('input[id=hoursforward_range]').val());
    //$('input[id=hoursforward_range]').on('input change', () => {
    //    $('.hoursforwardvalue').html($('input[id=hoursforward_range]').val());
    //});
    //$('.hoursbackvalue').html($('input[id=hoursback_range]').val());
    //$('input[id=hoursback_range]').on('input change', () => {
    //    $('.hoursbackvalue').html($('input[id=hoursback_range]').val());
    //});
    //Connection name Validation
    if (!checkValue($('input[type=text][name=connection_name]').val())) {
        $('input[type=text][name=connection_name]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_connection_name]').text("Please Enter Connection Name");
    }
    else {
        $('input[type=text][name=connection_name]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_connection_name]').text("");
    }
    //ip address Keyup
    $('input[type=text][name=ip_address]').keyup(function () {
        if (IPAddress_validator($('input[type=text][name=ip_address]').val()) ===
            "Invalid IP Address") {
            $('input[type=text][name=ip_address]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=ip_address]').text("Please Enter Connection Name");
        }
        else {
            $('input[type=text][name=ip_address]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_ip_address]').text("");
        }
        if ($('input[type=checkbox][name=udp_connection]').is(':checked')) {

            enableudpSubmit();
        }
        else if ($('input[type=checkbox][name=tcpip_connection]').is(':checked')) {

            enabletcpipSubmit();
        }
        else if ($('input[type=checkbox][name=ws_connection]').is(':checked')) {
            enablewsSubmit();
        }
        else {
            enableConnectionSubmit();
        }
    });
    //Connection name Keyup
    $('input[type=text][name=connection_name]').keyup(function () {
        if (!checkValue($('input[type=text][name=connection_name]').val())) {
            $('input[type=text][name=connection_name]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_connection_name]').text("Please Enter Connection Name");
        }
        else {
            $('input[type=text][name=connection_name]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_connection_name]').text("");
        }
        if ($('input[type=checkbox][name=udp_connection]').is(':checked')) {

            enableudpSubmit();
        } else if ($('input[type=checkbox][name=tcpip_connection]').is(':checked')) {

            enabletcpipSubmit();
        }
        else if ($('input[type=checkbox][name=ws_connection]').is(':checked')) {
            enablewsSubmit();
        }
        else {
            enableConnectionSubmit();
        }
    });

    //message Type Validation
    if (!checkValue($('input[type=text][name=message_type]').val())) {
        $('input[type=text][name=message_type]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_message_type]').text("Please Enter Message Type");
    }
    else {
        $('input[type=text][name=message_type]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_message_type]').text("");
    }
    //message Type Keyup
    $('input[type=text][name=message_type]').keyup(function () {
        if (!checkValue($('input[type=text][name=message_type]').val())) {
            $('input[type=text][name=message_type]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_message_type]').text("Please Enter Message Type");
        }
        else {
            $('input[type=text][name=message_type]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_message_type]').text("");
        }
        if ($('input[type=checkbox][name=udp_connection]').is(':checked')) {
            enableudpSubmit();
        } else if ($('input[type=checkbox][name=tcpip_connection]').is(':checked')) {

            enabletcpipSubmit();
        }
        else if ($('input[type=checkbox][name=ws_connection]').is(':checked')) {

            enablewsSubmit();
        }
        else {
            enableConnectionSubmit();
        }
    });
    $('input[type=text][name=port_number]').keyup(function () {
        if (!checkValue($('input[type=text][name=port_number]').val())) {
            $('input[type=text][name=port_number]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=port_number]').text("Please Enter Message Type");
        }
        else {
            $('input[type=text][name=port_number]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=port_number]').text("");
        }
        if ($('input[type=checkbox][name=udp_connection]').is(':checked')) {
            enableudpSubmit();
        } else if ($('input[type=checkbox][name=tcpip_connection]').is(':checked')) {

            enabletcpipSubmit();
        }
        else if ($('input[type=checkbox][name=ws_connection]').is(':checked')) {

            enablewsSubmit();
        }
        else {
            enableConnectionSubmit();
        }
    });
    //Data Retrieve Occurrences Validation
    if ($('input[type=checkbox][name=ws_connection]').is(':checked')) {

    }
    if (!checkValue($('select[name=data_retrieve] option:selected').val())) {
        $('select[name=data_retrieve]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_data_retrieve]').text("Select Data Retrieve Occurrences");
    }
    else {
        $('select[name=data_retrieve]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_data_retrieve]').text("");
    }
    //Data Retrieve Occurrences Keyup
    $('select[name=data_retrieve]').change(function () {
        if ($('input[type=checkbox][name=ws_connection]').is(':checked')) {

        }
        if (!checkValue($('select[name=data_retrieve] option:selected').val())) {
            $('select[name=data_retrieve]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_data_retrieve]').text("Select Data Retrieve Occurrences");
        }
        else {
            $('select[name=data_retrieve]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_data_retrieve]').text("");
        }
        if ($('input[type=checkbox][name=udp_connection]').is(':checked')) {
            enableudpSubmit();
        } else if ($('input[type=checkbox][name=tcpip_connection]').is(':checked')) {

            enabletcpipSubmit();
        }
        else if ($('input[type=checkbox][name=ws_connection]').is(':checked')) {
            enablewsSubmit();
        }
        else {
            enableConnectionSubmit();
        }
    });
    // Address Validation
    if (!checkValue($('input[type=text][name=ip_address]').val())) {
        $('input[type=text][name=ip_address]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_ip_address]').text("Please Enter Valid IP address");
    }
    else {
        $('input[type=text][name=ip_address]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_ip_address]').text("");
    }
    //IP Address Keyup
    $('input[type=text][name=ip_address]').keyup(function () {
        if (IPAddress_validator($('input[type=text][name=ip_address]').val()) === 'Invalid IP Address') {
            $('input[type=text][name=ip_address]').css("border-color", "#FF0000");
            $('span[id=error_ip_address]').text("Please Enter Valid IP Address!");
        }
        else {
            $('input[type=text][name=ip_address]').css({ "border-color": "#2eb82e" }).removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_ip_address]').text("");
        }
        if ($('input[type=checkbox][name=udp_connection]').is(":checked")) {
            enableudpSubmit();
        } else if ($('input[type=checkbox][name=tcpip_connection]').is(':checked')) {

            enabletcpipSubmit();
        }
        else if ($('input[type=checkbox][name=ws_connection]').is(":checked")) {
            enablewsSubmit();
        }
        else {
            enableConnectionSubmit();
        }
    });
    //port Validation
    if (!checkValue($('input[type=text][name=port_number]').val())) {
        $('input[type=text][name=port_number]').css({ "border-color": "#FF0000" }).removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_port_number]').text("Please Enter Port Number");
    }
    else {
        $('input[type=text][name=port_number]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_port_number]').text("");
    }
    //Port Keyup
    $('input[type=text][name=port_number]').keyup(function () {
        if ($.isNumeric($('input[type=text][name=port_number]').val())) {
            if ($('input[type=text][name=port_number]').val().length > 65535) {
                $('input[type=text][name=port_number]').css({ "border-color": "#FF0000" }).removeClass('is-valid').addClass('is-invalid');
                $('span[id=error_port_number]').text("Please Enter Port Number!");
            }
            else if ($('input[type=text][name=port_number]').val().length === 0) {
                $('input[type=text][name=port_number]').css({ "border-color": "#FF0000" }).addClass('is-valid').removeClass('is-invalid');
                $('span[id=error_port_number]').text("Please Enter Port Number!");
            }
            else {
                $('input[type=text][name=port_number]').css({ "border-color": "#2eb82e" }).removeClass('is-invalid').addClass('is-valid');
                $('span[id=error_port_number]').text("");
            }
        }
        else {
            $('input[type=text][name=port_number]').css({ "border-color": "#FF0000" }).removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_port_number]').text("Please Enter Port Number!");
        }
        if ($('input[type=checkbox][name=udp_connection]').is(':checked')) {
            enableudpSubmit();
        } else if ($('input[type=checkbox][name=tcpip_connection]').is(':checked')) {

            enabletcpipSubmit();
        }
        else if ($('input[type=checkbox][name=ws_connection]').is(':checked')) {
            enablewsSubmit();
        }
        else {
            enableConnectionSubmit();
        }
    });
    //Vendor URL
    if (!checkValue($('input[type=text][name=url]').val())) {
        $('input[type=text][name=url]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_url]').text("Please Enter API URL");
    } else {
        $('input[type=text][name=url]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_url]').text("");
    }
    //URL Keyup
    $('input[type=text][name=url]').keyup(function () {
        if (!checkValue($('input[type=text][name=url]').val())) {
            $('input[type=text][name=url]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_url]').text("Please Enter API URL");
        }
        else {
            $('input[type=text][name=url]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_url]').text("");
        }
        if ($('input[type=checkbox][name=udp_connection]').is(':checked')) {
            enableudpSubmit();
        } else if ($('input[type=checkbox][name=tcpip_connection]').is(':checked')) {

            enabletcpipSubmit();
        }
        else if ($('input[type=checkbox][name=ws_connection]').is(':checked')) {
            enablewsSubmit();
        }
        else {
            enableConnectionSubmit();
        }
    });

    //Hour 
    $('input[type=checkbox][name=hour_range]').change(() => {
        if (!$('input[type=checkbox][name=hour_range]').is(':checked')) {
            $('.hours_range_row').css("display", "none");
        }
        else {
            $('.hours_range_row').css("display", "");
        }
    });
    //radio check
    if ($("input[type=radio][name='connectionType']").change(() => {
        let connTypeRadio = $('input[type=radio][name=connectionType]:checked').attr('id');
        if (/^(udp|tcp)/i.test(connTypeRadio)) {
            $('div[id="serveripmenu"]').css("display", "");
            //$('input[type=checkbox][name=tcpip_connection]').prop("checked", false);
            //$('input[type=checkbox][name=ws_connection]').prop("checked", false);
            $('input[type=text][name=url]').prop("disabled", true);
            $('input[type=text][name=url]').val('');
            $('input[type=text][name=url]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_url]').text("");
            $('input[type=text][name=ip_address]').prop("disabled", true);
            $('input[type=text][name=ip_address]').val('');
            $('input[type=text][name=ip_address]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_ip_address]').text("");
            $('input[type=text][name=hostanme]').prop("disabled", true);
            $('input[type=text][name=hostanme]').val('');
            $('input[type=text][name=hostanme]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_hostanme]').text("");
            $('select[name=data_retrieve]').prop("disabled", true);
            $('select[name=data_retrieve]').val('0');
            $('select[name=data_retrieve]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_data_retrieve]').text("");
        }
        else if (/^(api)/i.test(connTypeRadio)) {
            $('div[id="serveripmenu"]').css("display", "none");
            $('input[type=text][name=url]').prop("disabled", false);
            $('input[type=text][name=url]').val('');
            $('select[name=data_retrieve]').prop("disabled", false);
            $('select[name=data_retrieve]').val('');
            onAPIConnection();
        }
    }))

    if ($('input[type=checkbox][name=udp_connection]').is(':checked')) {
        enableudpSubmit();
    }
    else if ($('input[type=checkbox][name=tcpip_connection]').is(':checked')) {
        enabletcpipSubmit();
    }
    else if ($('input[type=checkbox][name=ws_connection]').is(':checked')) {
        enablewsSubmit();
    }
    else {
        enableConnectionSubmit();
    }
    $('input[type=checkbox][name=ws_connection]').change(() => {
        onUpdateWS();
    });
});
async function updateConnection(Connectionupdate) {
    try {
        if (/^Admin/i.test(User.Role)) {
            if (!$.isEmptyObject(Connectionupdate)) {
                let connectionNode = document.querySelector("[id=api_" + Connectionupdate.Id + "]");
                if (connectionNode) {
                    connection_Table_Body.find("tr[id=api_" + Connectionupdate.Id + "]").replaceWith(connection_row_template.supplant(formatQSMlayout(Connectionupdate)))
                }
                else {
                    connection_Table_Body.append(connection_row_template.supplant(formatQSMlayout(Connectionupdate)));
                }
            }
        }
    } catch (e) {
        console.log(e);
    }
}
async function init_connection(ConnectionList) {
    try {
 
        connection_Table = $('table[id=connectiontable]');
        connection_Table_Body = connection_Table.find('tbody');
        if (ConnectionList.length > 0) {
            ConnectionList.sort(SortByConnectionName);
            connection_Table_Body.empty();
            $.each(ConnectionList, function (index, element) {
                connection_Table_Body.append(connection_row_template.supplant(formatQSMlayout(element)));
            });
        }
    } catch (e) {
        console.log(e);
    }
}


let connection_row_template = '<tr data-id="{id}" class="{button_color}" id="api_{id}">' +
    '<td><span class="ml-p5rem">{name}</span></td>' +
    '<td>{messagetype}</td>' +
    '<td id="port_{id}">{port}</td>' +
    '<td class="font-weight-bold" id="apistatus_{id}">{connected}</td>' +
    '<td class="d-flex">' +
    '<button class="btn btn-light btn-sm mx-1 pi-iconEdit connectionedit"></button>' +
    '<button class="btn btn-light btn-sm mx-1 pi-trashFill connectiondelete"></button>' +
    '</td>' +
    '</tr>';
function formatQSMlayout(conn_status) {
    return $.extend(conn_status, {
        id: conn_status.Id,
        name: conn_status.ConnectionName,
        messagetype: conn_status.MessageType,
        port: conn_status.Port,
        connected: GetConnectionStatus(conn_status),
        button_color: Get_Color(conn_status)
    });
}
function Add_Connection() {
    $('#modalHeader_ID').text('Add Connection');
    $('button[id=apisubmitBtn]').off().on('click', function () {
        $('button[id=apisubmitBtn]').prop('disabled', true);

        $('input[type=checkbox][name=ws_connection]').prop('disabled', false);
        $('input[type=checkbox][name=udp_connection]').prop('disabled', false);
        $('input[type=checkbox][name=tcpip_connection]').prop('disabled', false);
        let jsonObject = {
            ActiveConnection: $('input[type=checkbox][name=active_connection]').is(':checked'),
            UdpConnection: $('input[type=checkbox][name=udp_connection]').is(':checked'),
            TcpIpConnection: $('input[type=checkbox][name=tcpip_connection]').is(':checked'),
            WsConnection: $('input[type=checkbox][name=ws_connection]').is(':checked'),
            HoursBack: parseInt($('input[id=hoursback_range]').val()),
            HoursForward: parseInt($('input[id=hoursforward_range]').val()),
            DataRetrieve: $('select[name=data_retrieve] option:selected').val(),
            ConnectionName: $('input[type=text][name=connection_name]').val(),
            IpAddress: $('input[type=text][name=ip_address]').val(),
            Port: $('input[type=text][name=port_number]').val(),
            Url: $('input[type=text][name=url]').val(),
            MessageType: $('input[type=text][name=message_type]').val(),
            /*AdminEmailRecepient: $('input[type=text][name=admin_email_recepient').val(),*/
            CreatedByUsername: User.UserId,
            NassCode: User.Facility_NASS_Code,
        };
        if (!$.isEmptyObject(jsonObject)) {
            $.connection.FOTFManager.server.addAPI(JSON.stringify(jsonObject)).done(function (Data) {
                if (Data.length === 1) {
                    updateConnection(Data[0]);
                    $('span[id=error_apisubmitBtn]').text(Data[0].ConnectionName + " " + Data[0].MessageType + " Connection has been Added");
                    setTimeout(function () { $("#API_Connection_Modal").modal('hide'); sidebar.open('connections'); }, 1500);
                }
                else {
                    $('span[id=error_apisubmitBtn]').text("Error Adding Connection");
                }
            });
        }
    });
    $('#API_Connection_Modal').modal();
}
function Edit_Connection(id) {
    $('#modalHeader_ID').text('Edit Connection');
    sidebar.close('connections');
    $('button[id=apisubmitBtn]').prop('disabled', true);
    $('input[type=text][id=connection_name]').val(Data.ConnectionName);
    $('input[type=text][id=ip_address]').val(Data.IpAddress);
    $('input[type=text][id=hostname]').val(Data.Hostname);
    $('input[type=text][id=port_number]').val(Data.Port);
    $('input[type=text][id=url]').val(Data.Url);
    $('input[type=text][id=message_type]').val(Data.MessageType);
    $('select[name=data_retrieve]').val(Data.DataRetrieve);
    if (Data.ApiConnection) {
        $('input[type=radio][id=api_connection]').prop('checked', true).change();
    }
    if (Data.UdpConnection) {
        $('input[type=radio][id=udp_connection]').prop('checked', true).change();
    }
    if (Data.TcpIpConnection) {
        $('input[type=radio][id=tcpip_connection]').prop('checked', true).change();
    }
    if (Data.WsConnection) {
        $('input[type=radio][id=ws_connection]').prop('checked', true).change();
    }
    if (Data.HoursBack > 0 || Data.HoursForward > 0) {

        $('.hoursbackvalue').html($.isNumeric(Data.HoursBack) ? parseInt(Data.HoursBack) : 0);
        $('input[id=hoursback_range]').val($.isNumeric(Data.HoursBack) ? parseInt(Data.HoursBack) : 0);
        $('.hours_range_row').css("display", "");
        $('.hoursforwardvalue').html($.isNumeric(Data.HoursForward) ? parseInt(Data.HoursForward) : 0);
        $('input[id=hoursforward_range]').val($.isNumeric(Data.HoursForward) ? parseInt(Data.HoursForward) : 0);
        $('.hours_range_row').css("display", "");

        $('input[type=checkbox][name=hour_range]').prop('checked', true).change();

    }
    else {
        $('.hoursbackvalue').html($.isNumeric(Data.HoursBack) ? parseInt(Data.HoursBack) : 0);
        $('input[id=hoursback_range]').val($.isNumeric(Data.HoursBack) ? parseInt(Data.HoursBack) : 0);
        $('.hours_range_row').css("display", "none");
        $('.hoursforwardvalue').html($.isNumeric(Data.HoursForward) ? parseInt(Data.HoursForward) : 0);
        $('input[id=hoursforward_range]').val($.isNumeric(Data.HoursForward) ? parseInt(Data.HoursForward) : 0);
        $('.hours_range_row').css("display", "none");
        $('input[type=checkbox][name=hour_range]').prop('checked', false).change();
    }
    $('button[id=apisubmitBtn]').prop('disabled', true);
    try {
        $.connection.FOTFManager.server.getAPIList(id).done(function (svrData) {
            var Data = svrData[0];
            if (!$.isEmptyObject(Data)) {

                $('input[type=text][name=connection_name]').val(Data.ConnectionName);
                $('input[type=text][name=ip_address]').val(Data.IpAddress);
                $('input[type=text][name=hostname]').val(Data.Hostname);
                $('input[type=text][name=port_number]').val(Data.Port);
                $('input[type=text][name=url]').val(Data.Url);
                /*$('input[type=text][name=outgoingapikey]').val(Data.OUTGOING_APIKEY);*/
                $('input[type=text][name=message_type]').val(Data.MessageType);
                $('input[type=checkbox][name=active_connection]').prop('checked', Data.ActiveConnection).change();
                if (Data.HoursBack > 0 || Data.HoursForward > 0) {

                    $('.hoursbackvalue').html($.isNumeric(Data.HoursBack) ? parseInt(Data.HoursBack) : 0);
                    $('input[id=hoursback_range]').val($.isNumeric(Data.HoursBack) ? parseInt(Data.HoursBack) : 0);
                    $('.hours_range_row').css("display", "");
                    $('.hoursforwardvalue').html($.isNumeric(Data.HoursForward) ? parseInt(Data.HoursForward) : 0);
                    $('input[id=hoursforward_range]').val($.isNumeric(Data.HoursForward) ? parseInt(Data.HoursForward) : 0);
                    $('.hours_range_row').css("display", "");
                
                    $('input[type=checkbox][name=hour_range]').prop('checked', true).change();
                    
                }
                else {
                    $('.hoursbackvalue').html($.isNumeric(Data.HoursBack) ? parseInt(Data.HoursBack) : 0);
                    $('input[id=hoursback_range]').val($.isNumeric(Data.HoursBack) ? parseInt(Data.HoursBack) : 0);
                    $('.hours_range_row').css("display", "none");
                    $('.hoursforwardvalue').html($.isNumeric(Data.HoursForward) ? parseInt(Data.HoursForward) : 0);
                    $('input[id=hoursforward_range]').val($.isNumeric(Data.HoursForward) ? parseInt(Data.HoursForward) : 0);
                    $('.hours_range_row').css("display", "none");
                    $('input[type=checkbox][name=hour_range]').prop('checked', false).change();
                }
                $('select[name=data_retrieve]').val(Data.DataRetrieve);
                /*$('input[type=text][name=admin_email_recepient]').val(Data.AdminEmailRecepient);*/

                //API connection
                if (Data.ActiveConnection) {
                    if (!$('input[type=checkbox][name=active_connection]').is(':checked')) {
                        $('input[type=checkbox][name=active_connection]').prop('checked', true).change();
                    }
                }
                else {
                    if ($('input[type=checkbox][name=active_connection]').is(':checked')) {
                        $('input[type=checkbox][name=active_connection]').prop('checked', false).change();
                    }
                }
                if (Data.UdpConnection) {
                    $('input[type=checkbox][name=hour_range]').prop('disabled', true);
                    $('input[type=checkbox][name=udp_connection]').prop('disabled', true);
                    $('input[type=checkbox][name=tcpip_connection]').prop('disabled', true);
                    $('input[type=checkbox][name=ws_connection]').prop('disabled', true);
                    $('input[type=checkbox][name=udp_connection]').prop('checked', true).change();
                }
                else {
                    $('input[type=checkbox][name=udp_connection]').prop('disabled', true);
                }
                if (Data.TcpIpConnection) {
                    $('input[type=checkbox][name=hour_range]').prop('disabled', true);
                    $('input[type=checkbox][name=udp_connection]').prop('disabled', true);
                    $('input[type=checkbox][name=tcpip_connection]').prop('disabled', true);
                    $('input[type=checkbox][name=ws_connection]').prop('disabled', true);
                    $('input[type=checkbox][name=tcpip_connection]').prop('checked', true).change();
                }
                else {
                    $('input[type=checkbox][name=tcpip_connection]').prop('disabled', true);
                }

                if (Data.WsConnection) {
                    $('input[type=checkbox][name=hour_range]').prop('disabled', true);
                    $('input[type=checkbox][name=udp_connection]').prop('disabled', true);
                    $('input[type=checkbox][name=ws_connection]').prop('checked', true);
                    onUpdateWS();

                }
                else {
                    $('input[type=checkbox][name=ws_connection]').prop('disabled', true);
                    if ($('input[type=checkbox][name=ws_connection]').is(':checked')) {
                        $('input[type=checkbox][name=ws_connection]').prop('checked', false);
                        onUpdateWS();
                    }
                }

                $('button[id=apisubmitBtn]').off().on('click', function () {
                    try {
                        $('button[id=apisubmitBtn]').prop('disabled', true);
                        let jsonObject = {
                            ActiveConnection: $('input[type=checkbox][name=active_connection]').is(':checked'),
                            UdpConnection: $('input[type=checkbox][name=udp_connection]').is(':checked'),
                            TcpIpConnection: $('input[type=checkbox][name=tcpip_connection]').is(':checked'),
                            WsConnection: $('input[type=checkbox][name=ws_connection]').is(':checked'),
                            HoursBack: $('input[type=checkbox][name=hour_range]').is(':checked') ? parseInt($('input[id=hoursback_range]').val()) : 0,
                            HoursForward: $('input[type=checkbox][name=hour_range]').is(':checked') ? parseInt($('input[id=hoursforward_range]').val()) : 0,
                            DataRetrieve: $('select[name=data_retrieve] option:selected').val(),
                            ConnectionName: $('input[type=text][name=connection_name]').val(),
                            IpAddress: $('input[type=text][name=ip_address]').val(),
                            Port: $('input[type=text][name=port_number]').val(),
                            Url: $('input[type=text][name=url]').val(),
                            MessageType: $('input[type=text][name=message_type]').val(),
                           /* AdminEmailRecepient: $('input[type=text][name=admin_email_recepient').val(),*/
                            LastupdateByUsername: User.UserId,
                            NassCode: User.Facility_NASS_Code,
                            Id: id
                        }
                        if (!$.isEmptyObject(jsonObject)) {
                            $.connection.FOTFManager.server.editAPI(JSON.stringify(jsonObject)).done(function (rData) {
                                if (rData.length === 1) {
                                    updateConnection(rData[0]);
                                    $('span[id=error_apisubmitBtn]').text(rData[0].ConnectionName + " " + rData[0].MessageType + " Connection has been Updated.");
                                    setTimeout(function () { $("#API_Connection_Modal").modal('hide'); sidebar.open('connections'); }, 1500);
                                }
                                else {
                                    $('span[id=error_apisubmitBtn]').text("Error editing Connection");
                                }
                            });
                        }

                    } catch (e) {
                        $('span[id=error_apisubmitBtn]').text(e);
                    }
                });
                $('#API_Connection_Modal').modal();
            }
            else {
                $('label[id=error_apisubmitBtn]').text("Invalid API ID");
                $('#API_Connection_Modal').modal();
            }
        });
    } catch (e) {
        console.log(e);
    }

}
function GetConnectionStatus(data) {
    if (data.ActiveConnection) {
        if (data.ApiConnected) {
            return "Online";
        }
        else {
            return "Off-Line";
        }
    }
    else {
        return "Disabled";
    }
}
function Get_Color(data) {
    if (data.ActiveConnection) {
        if (data.ApiConnected) {
            return "table-success";
        }
        else {
            return "table-danger";
        }
    }
    else {
        return "table-warning";
    }
}
function Remove_Connection(id) {
    try {
        sidebar.close('connections');
        $('#removeAPImodalHeader_ID').text('Remove Connection');
        $('#RemoveConfirmationModal').modal();

        $('button[id=remove_server_connection]').off().on('click', function () {
            var jsonObject = { Id: id };
            $.connection.FOTFManager.server.removeAPI(JSON.stringify(jsonObject)).done(function (Data) {
                $("#api_" + id).remove();
                setTimeout(function () {
                    $("#RemoveConfirmationModal").modal('hide');
                    sidebar.open('connections');
                }, 1500);
                $('#RemoveConfirmationModal').modal();
            })
        });

    } catch (e) {
        console.log(e);
    }

}

function enableConnectionSubmit() {
    //AGV connections
    if ($('input[type=text][name=ip_address]').hasClass('is-valid') &&
        $('input[type=text][name=url]').hasClass('is-valid') &&
        $('input[type=text][name=message_type]').hasClass('is-valid') &&
        $('select[name=data_retrieve]').hasClass('is-valid') &&
        $('input[type=text][name=connection_name]').hasClass('is-valid')
    ) {
        $('button[id=apisubmitBtn]').prop('disabled', false);
    }
    else {
        $('button[id=apisubmitBtn]').prop('disabled', true);
    }
}

function SortByConnectionName(a, b) {
    return a.ConnectionName < b.ConnectionName ? -1 : a.ConnectionName > b.ConnectionName ? 1 : 0;
}