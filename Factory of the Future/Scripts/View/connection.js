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
    sidebar.open('connections');
});
//on open set rules
$('#API_Connection_Modal').on('shown.bs.modal', function () {
    sidebar.close('connections');
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
        if (!$('input[type=checkbox][id=hour_range]').is(':checked')) {
            $('.hours_range_row').css("display", "none");
            $('input[id=hoursback_range]').val(0);
            $('.hoursbackvalue').html(0);
            $('input[id=hoursforward_range]').val(0);
            $('.hoursforwardvalue').html(0);
        }
        else {
            $('.hours_range_row').css("display", "");
        }
    });
    //radio check
    if ($("input[type=radio][name='connectionType']").change(() => {

        if ($(this).id === "udp_connection") {
            $('input[type=checkbox][name=tcpip_connection]').prop("checked", false);
            $('input[type=checkbox][name=ws_connection]').prop("checked", false);
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
            $('select[name=data_retrieve]').val(' ');
            $('select[name=data_retrieve]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_data_retrieve]').text("");
        }
    }))
        if ($('input[type=checkbox][name=udp_connection]').is(':checked')) {
            $('input[type=text][name=url]').prop("disabled", true);
            $('input[type=text][name=url]').val('');
            $('input[type=text][name=url]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_url]').text("");
            //$('input[type=text][name=outgoingapikey]').prop("disabled", true);
            //$('input[type=text][name=outgoingapikey]').val('');
            //$('input[type=text][name=outgoingapikey]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            //$('span[id=error_outgoingapikey]').text("");
            $('input[type=text][name=ip_address]').prop("disabled", true);
            $('input[type=text][name=ip_address]').val('');
            $('input[type=text][name=ip_address]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_ip_address]').text("");
            $('input[type=text][name=hostanme]').prop("disabled", true);
            $('input[type=text][name=hostanme]').val('');
            $('input[type=text][name=hostanme]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_hostanme]').text("");
            $('select[name=data_retrieve]').prop("disabled", true);
            $('select[name=data_retrieve]').val(' ');
            $('select[name=data_retrieve]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_data_retrieve]').text("");
        }
    if ($('input[type=checkbox][name=tcpip_connection]').is(':checked')) {
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
        $('select[name=data_retrieve]').val(' ');
        $('select[name=data_retrieve]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
        $('span[id=error_data_retrieve]').text("");
    }
    $('input[type=checkbox][name=udp_connection]').change(() => {
        if (!$('input[type=checkbox][name=udp_connection]').is(':checked')) {
            onRegularConnection();
        }
        else {
            $('input[type=checkbox][name=tcpip_connection]').prop("checked", false);
            $('input[type=checkbox][name=ws_connection]').prop("checked", false);
            $('input[type=text][name=url]').prop("disabled", true);
            $('input[type=text][name=url]').val('');
            $('input[type=text][name=url]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_url]').text("");
            //$('input[type=text][name=outgoingapikey]').prop("disabled", true);
            //$('input[type=text][name=outgoingapikey]').val('');
            //$('input[type=text][name=outgoingapikey]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            //$('span[id=error_outgoingapikey]').text("");
            $('input[type=text][name=ip_address]').prop("disabled", true);
            $('input[type=text][name=ip_address]').val('');
            $('input[type=text][name=ip_address]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_ip_address]').text("");
            $('input[type=text][name=hostanme]').prop("disabled", true);
            $('input[type=text][name=hostanme]').val('');
            $('input[type=text][name=hostanme]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_hostanme]').text("");
            $('select[name=data_retrieve]').prop("disabled", true);
            $('select[name=data_retrieve]').val(' ');
            $('select[name=data_retrieve]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_data_retrieve]').text("");
            // enableUdpSubmit();
        }
    });
    $('input[type=checkbox][name=tcpip_connection]').change(() => {
        if (!$('input[type=checkbox][name=tcpip_connection]').is(':checked')) {
            onRegularConnection();
        }
        else {
            $('input[type=checkbox][name=udp_connection]').prop("checked", false);
            $('input[type=checkbox][name=ws_connection]').prop("checked", false);
            $('input[type=text][name=url]').prop("disabled", true);
            $('input[type=text][name=url]').val('');
            $('input[type=text][name=url]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_url]').text("");
            //$('input[type=text][name=outgoingapikey]').prop("disabled", true);
            //$('input[type=text][name=outgoingapikey]').val('');
            //$('input[type=text][name=outgoingapikey]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            //$('span[id=error_outgoingapikey]').text("");
            $('input[type=text][name=ip_address]').prop("disabled", true);
            $('input[type=text][name=ip_address]').val('');
            $('input[type=text][name=ip_address]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_ip_address]').text("");
            $('input[type=text][name=hostanme]').prop("disabled", true);
            $('input[type=text][name=hostanme]').val('');
            $('input[type=text][name=hostanme]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_hostanme]').text("");
            $('select[name=data_retrieve]').prop("disabled", true);
            $('select[name=data_retrieve]').val(' ');
            $('select[name=data_retrieve]').css("border-color", "#D3D3D3").removeClass('is-valid').removeClass('is-invalid');
            $('span[id=error_data_retrieve]').text("");
            enabletcpipSubmit();
        }
    });
    if ($('input[type=radio][id=udp_connection]').is(':checked')) {
        $('input[type=radio][id=hour_range]').prop('disabled', true);
        enableudpSubmit();
    }
    else if ($('input[type=radio][id=tcpip_connection]').is(':checked')) {
        $('input[type=radio][id=hour_range]').prop('disabled', true);
        enabletcpipSubmit();
    }
    else if ($('input[type=radio][id=ws_connection]').is(':checked')) {
        $('input[type=radio][id=hour_range]').prop('disabled', true);
        enablewsSubmit();
    }
    else {
        enableConnectionSubmit();
    }
    //$('input[type=checkbox][name=ws_connection]').change(() => {
    //    onUpdateWS();
    //});
});
$('#RemoveConfirmationModal').on('shown.bs.modal', function () {
    $('#removeAPImodalHeader_ID').text('Remove Connection');
    sidebar.close('connections');
});
$('#RemoveConfirmationModal').on('hidden.bs.modal', function () {
    sidebar.open('connections');
});
async function updateConnection(Connectionupdate) {
    try {
        updateConnectionDataTable(Connectionupdate, "connectiontable");
    } catch (e) {
        console.log(e);
    }
}
async function init_connection(ConnectionList) {
    try {
        createConnectionDataTable("connectiontable");
        if (ConnectionList.length > 0) {
            loadConnectionDatatable(ConnectionList.sort(SortByConnectionName), "connectiontable")
        }
    } catch (e) {
        console.log(e);
    }
}
//start table function
function createConnectionDataTable(table) {
    let arrayColums = [{
        "ConnectionName": "",
        "MessageType": "",
        "Port": "",
        "Status": "",
        "Action": ""
    }]
    var columns = [];
    var tempc = {};
    $.each(arrayColums[0], function (key, value) {
        tempc = {};
        if (/ConnectionName/i.test(key)) {
            tempc = {
                "title": 'Name',
                "mDataProp": key
            }
        }
        else if (/MessageType/i.test(key)) {
            tempc = {
                "title": "Message Type",
                "mDataProp": key
            }
        }
        else if (/Port/i.test(key)) {
            tempc = {
                "title": "Port",
                "mDataProp": key
            }
        }
        else if (/Status/i.test(key)) {
            tempc = {
                "title": "Status",
                "mDataProp": key

            }
        }
        else if (/Action/i.test(key)) {
            tempc = {
                "title": "Action",
                "mDataProp": key,
                "mRender": function (data, type, full) {
                    return '<button class="btn btn-light btn-sm mx-1 pi-iconEdit connectionedit" name="connectionedit"></button>' + '<button class="btn btn-light btn-sm mx-1 pi-trashFill connectiondelete" name="connectiondelete"></button>'
                }
            }
        }
        else {
            tempc = {
                "title": capitalize_Words(key.replace(/\_/, ' ')),
                "mDataProp": key
            }
        }
        columns.push(tempc);

    });
    $('#' + table).DataTable({
        dom: 'Bfrtip',
        buttons: {
            buttons:
                [
                    {
                        text: "Add",
                        action: function () { Add_Connection(); }
                    }
                ]
        },
        bFilter: false,
        bdeferRender: true,
        bpaging: false,
        bPaginate: false,
        bAutoWidth: true,
        bInfo: false,
        destroy: true,
        language: {
            zeroRecords: "No Data",
        },
        aoColumns: columns,
        columnDefs: [],
        sorting: [[0, "asc"]],
        rowCallback: function (row, data, index) {
            $(row).find('td:eq(0)').css('text-align', 'left');
            if (data.ActiveConnection) {
                if (data.ApiConnected) {
                    $(row).find('td:eq(3)').css('background-color', 'green');
                }
                else {
                    $(row).find('td:eq(3)').css('background-color', 'red');
                }
            }
            else {
                $(row).find('td:eq(3)').css('background-color', 'orange');
            }


        }
    });
    // Edit/remove record
    $('#' + table + ' tbody').on('click', 'button', function () {
        let td = $(this);
        let table = $(td).closest('table');
        let row = $(table).DataTable().row(td.closest('tr'));
        if (/connectionedit/ig.test(this.name)) {
            sidebar.close();
            Edit_Connection(row.data());
        }
        else if (/connectiondelete/ig.test(this.name)) {
            sidebar.close();
            Remove_Connection(row.data());
        }
    });
}
function loadConnectionDatatable(data, table) {
    if ($.fn.dataTable.isDataTable("#" + table)) {
        if (!$.isEmptyObject(data)) {
            $('#' + table).DataTable().rows.add(data).draw();
        }
    }
}
function updateConnectionDataTable(newdata, table) {
    let loadnew = true;
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows(function (idx, data, node) {
            loadnew = false;
            if (data.Id === newdata.Id) {
                $('#' + table).DataTable().row(node).data(newdata).draw().invalidate();
            }
        })
        if (loadnew) {
            loadConnectionDatatable(newdata, table);
        }
    }
}
function removeConnectionDataTable(table) {
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows(function (idx, data, node) {
            $('#' + table).DataTable().row(node).remove().draw();
        })
    }
}
//end table function 
function Add_Connection() {
    $('#modalHeader_ID').text('Add Connection');
    $('button[id=apisubmitBtn]').off().on('click', function () {
        $('button[id=apisubmitBtn]').prop('disabled', true);
        $('input[type=checkbox][name=ws_connection]').prop('disabled', false);
        $('input[type=checkbox][name=udp_connection]').prop('disabled', false);
        $('input[type=checkbox][name=tcpip_connection]').prop('disabled', false);
        let jsonObject = {
            ActiveConnection: $('input[type=checkbox][name=active_connection]').is(':checked'),
            ApiConnection: $('input[type=checkbox][name=api_connection]').is(':checked'),
            UdpConnection: $('input[type=checkbox][name=udp_connection]').is(':checked'),
            TcpIpConnection: $('input[type=checkbox][name=tcpip_connection]').is(':checked'),
            WsConnection: $('input[type=checkbox][name=ws_connection]').is(':checked'),
            HoursBack: parseInt($('input[id=hoursback_range]').val(), 10),
            HoursForward: parseInt($('input[id=hoursforward_range]').val(), 10),
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
            fotfmanager.server.addAPI(JSON.stringify(jsonObject)).done(function (Data) {
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
function Edit_Connection(Data) {
    $('#modalHeader_ID').text('Edit Connection');
    $('input[type=checkbox][id=active_connection]').prop('checked', Data.ActiveConnection);
    $('input[type=text][id=connection_name]').val(Data.ConnectionName);
    $('input[type=text][id=ip_address]').val(Data.IpAddress);
    $('input[type=text][id=hostname]').val(Data.Hostname);
    $('input[type=text][id=port_number]').val(Data.Port);
    $('input[type=text][id=url]').val(Data.Url);
    $('input[type=text][id=message_type]').val(Data.MessageType);
    $('select[name=data_retrieve]').val(Data.DataRetrieve);
    if (Data.ApiConnection) {
        $('input[type=radio][id=api_connection]').prop('checked', Data.ApiConnection);
    }
    if (Data.UdpConnection) {
        $('input[type=radio][id=udp_connection]').prop('checked', Data.UdpConnection);
    }
    if (Data.TcpIpConnection) {
        $('input[type=radio][id=tcpip_connection]').prop('checked', Data.TcpIpConnection);
    }
    if (Data.WsConnection) {
        $('input[type=radio][id=ws_connection]').prop('checked', Data.WsConnection);
    }
    if (Data.HoursBack > 0 || Data.HoursForward > 0) {

        $('.hoursbackvalue').html($.isNumeric(Data.HoursBack) ? parseInt(Data.HoursBack, 10) : 0);
        $('input[id=hoursback_range]').val($.isNumeric(Data.HoursBack) ? parseInt(Data.HoursBack, 10) : 0);
        $('.hours_range_row').css("display", "");
        $('.hoursforwardvalue').html($.isNumeric(Data.HoursForward) ? parseInt(Data.HoursForward, 10) : 0);
        $('input[id=hoursforward_range]').val($.isNumeric(Data.HoursForward) ? parseInt(Data.HoursForward, 10) : 0);
        $('.hours_range_row').css("display", "");

        $('input[type=checkbox][id=hour_range]').prop('checked', true);

    }
    else {
        $('.hoursbackvalue').html($.isNumeric(Data.HoursBack) ? parseInt(Data.HoursBack, 10) : 0);
        $('input[id=hoursback_range]').val($.isNumeric(Data.HoursBack) ? parseInt(Data.HoursBack, 10) : 0);
        $('.hours_range_row').css("display", "none");
        $('.hoursforwardvalue').html($.isNumeric(Data.HoursForward) ? parseInt(Data.HoursForward, 10) : 0);
        $('input[id=hoursforward_range]').val($.isNumeric(Data.HoursForward) ? parseInt(Data.HoursForward, 10) : 0);
        $('.hours_range_row').css("display", "none");
        $('input[type=checkbox][id=hour_range]').prop('checked', false);
    }
    $('button[id=apisubmitBtn]').prop('disabled', true);
    $('button[id=apisubmitBtn]').off().on('click', function () {
        try {
            $('button[id=apisubmitBtn]').prop('disabled', true);
            let jsonObject = {
                ActiveConnection: $('input[type=checkbox][name=active_connection]').is(':checked'),
                ApiConnection: $('input[type=radio][id=api_connection]').is(':checked'),
                UdpConnection: $('input[type=radio][id=udp_connection]').is(':checked'),
                TcpIpConnection: $('input[type=radio][id=tcpip_connection]').is(':checked'),
                WsConnection: $('input[type=radio][name=ws_connection]').is(':checked'),
                HoursBack: $('input[type=checkbox][id=hour_range]').is(':checked') ? parseInt($('input[id=hoursback_range]').val(), 10) : 0,
                HoursForward: $('input[type=checkbox][id=hour_range]').is(':checked') ? parseInt($('input[id=hoursforward_range]').val(), 10) : 0,
                DataRetrieve: $('select[name=data_retrieve] option:selected').val(),
                ConnectionName: $('input[type=text][name=connection_name]').val(),
                IpAddress: $('input[type=text][id=ip_address]').val(),
                Port: $('input[type=text][name=port_number]').val(),
                Url: $('input[type=text][name=url]').val(),
                MessageType: $('input[type=text][name=message_type]').val(),
                LastupdateByUsername: User.UserId,
                Id: Data.Id
            }
            if (!$.isEmptyObject(jsonObject)) {
                fotfmanager.server.editAPI(JSON.stringify(jsonObject)).done(function (rData) {
                    setTimeout(function () { $("#API_Connection_Modal").modal('hide'); sidebar.open('connections'); }, 500);
                });
            }

        } catch (e) {
            $('span[id=error_apisubmitBtn]').text(e);
        }
    });
    $('#API_Connection_Modal').modal();
}
function Remove_Connection(id) {
    try {
        $('button[id=remove_server_connection]').off().on('click', function () {
            let jsonObject = { Id: id };
            fotfmanager.server.removeAPI(JSON.stringify(jsonObject)).done(function (Data) {
                $("#api_" + id).remove();
                setTimeout(function () {
                    $("#RemoveConfirmationModal").modal('hide');
                    sidebar.open('connections');
                }, 1500);
            })
        });
        $('#RemoveConfirmationModal').modal();
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