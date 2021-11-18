/**/
async function updateConnection(Connectionupdate) {
    try {
        if (/^Admin/i.test(User.Role)) {
            if (!$.isEmptyObject(Connectionupdate)) {
                var api_tr = $("#api_" + Connectionupdate.ID);
                if (api_tr.length > 0) {
                    var new_status = GetConnectionStatus(Connectionupdate);
                    var currentstatus = $("#apistatus_" + Connectionupdate.ID).text();
                    if (new_status !== currentstatus) {
                        $("#apistatus_" + Connectionupdate.ID).text(new_status);
                    }
                    var new_btn_category = Get_Color(Connectionupdate);
                    var current_btn_category = $("#api_" + Connectionupdate.ID).attr("class");
                    if (new_btn_category !== current_btn_category) {
                        $("#api_" + Connectionupdate.ID).addClass(new_btn_category).removeClass(current_btn_category);
                    }
                }
                else {
                    $Table = $('table[id=connectiontable]');
                    $Table_Body = $Table.find('tbody');
                    $row_template = '<tr data-id="{id}" class="{button_color}" id="api_{id}">' +
                        '<td class="align-middle">{name}</td>' +
                        '<td class="align-middle">{messagetype}</td>' +
                        '<td class="font-weight-bold align-middle text-center" id="apistatus_{id}">{connected}</td>' +
                        '<td class="d-flex align-middle justify-content-center">' +
                        '<button class="btn btn-light btn-sm mx-1 pi-iconEdit connectionedit"></button>' +
                        '<button class="btn btn-light btn-sm mx-1 pi-trashFill connectiondelete"></button>' +
                        '</td>' +
                        '</tr>';
                    function formatQSMlayout(conn_status) {
                        return $.extend(conn_status, {
                            id: conn_status.ID,
                            name: conn_status.CONNECTION_NAME,
                            messagetype: conn_status.MESSAGE_TYPE,
                            connected: GetConnectionStatus(conn_status),
                            button_color: Get_Color(conn_status)
                        });
                    }

                    $Table_Body.append($row_template.supplant(formatQSMlayout(Connectionupdate)));
                }
            }
        }
    } catch (e) {
        console.log(e);
    }
}
function Add_Connection() {
    $('#modalHeader_ID').text('Add API Connection');

    $('button[id=apisubmitBtn]').off().on('click', function () {
        $('button[id=apisubmitBtn]').prop('disabled', true);
        var jsonObject = {};

        //connection active
        jsonObject.ACTIVE_CONNECTION = $('input[type=checkbox][name=active_connection]').is(':checked');
        jsonObject.UDP_CONNECTION = $('input[type=checkbox][name=udp_connection]').is(':checked');
        // assign values to Json object if they are not empty
        checkValue($('input[type=text][name=connection_name]').val()) ? jsonObject.CONNECTION_NAME = $('input[type=text][name=connection_name]').val() : '';
        checkValue($('input[type=text][name=hostname]').val()) ? jsonObject.HOSTNAME = $('input[type=text][name=hostname]').val() : '';
        checkValue($('input[type=text][name=ip_address]').val()) ? jsonObject.IP_ADDRESS = $('input[type=text][name=ip_address]').val() : '';
        checkValue($('input[type=text][name=port_number]').val()) ? jsonObject.PORT = $('input[type=text][name=port_number]').val() : '';
        checkValue($('input[type=text][name=url]').val()) ? jsonObject.URL = $('input[type=text][name=url]').val() : '';
        checkValue($('input[type=text][name=outgoingapikey]').val()) ? jsonObject.OUTGOING_APIKEY = $('input[type=text][name=outgoingapikey]').val() : '';
        checkValue($('input[type=text][name=message_type]').val()) ? jsonObject.MESSAGE_TYPE = $('input[type=text][name=message_type]').val() : '';
        checkValue($('select[name=data_retrieve] option:selected').val()) ? jsonObject.DATA_RETRIEVE = $('select[name=data_retrieve] option:selected').val() : '';
        checkValue($('input[type=text][name=admin_email_recepient]').val()) ? jsonObject.ADMIN_EMAIL_RECEPIENT = $('input[type=text][name=admin_email_recepient]').val() : '';

        if (!$.isEmptyObject(jsonObject)) {
            jsonObject.CREATED_BY_USERNAME = User.UserId;
            $.connection.FOTFManager.server.addAPI(JSON.stringify(jsonObject)).done(function (Data) {
                if (Data.length === 1) {
                    if (Data.hasOwnProperty("ID")) {
                        $('span[id=error_apisubmitBtn]').text(Data[0].CONNECTION_NAME + " " + Data[0].MESSAGE_TYPE + " Connection has been Added");
                        setTimeout(function () { $("#API_Connection_Modal").modal('hide'); sidebar.open('connections'); }, 1500);
                    }
                    else {
                        $('span[id=error_apisubmitBtn]').text(Data[0].CONNECTION_NAME + " " + Data[0].MESSAGE_TYPE + " Connection has been Added");
                        setTimeout(function () { $("#API_Connection_Modal").modal('hide'); sidebar.open('connections'); }, 1500);
                    }
                }
                else {
                    $('span[id=error_apisubmitBtn]').text("Error Adding Connection");
                }
            });
        };
    });
    $('#API_Connection_Modal').modal();
}
function Edit_Connection(id) {
    $('#modalHeader_ID').text('Edit API Connection');
    if ($.isNumeric(id)) {
        id = parseInt(id);
    }
    sidebar.close('connections');
    $('button[id=apisubmitBtn]').prop('disabled', true);
    try {
        $.connection.FOTFManager.server.getAPIList(id).done(function (svrData) {
            var Data = svrData[0];
            $('input[type=checkbox][name=active_connection]').change(() => {
                if (Data.CONECTION_CONNECTED !== $('input[type=checkbox][name=active_connection]').is(':checked')) {
                    $('button[id=apisubmitBtn]').prop('disabled', false);
                    if ($('input[type=checkbox][name=udp_connection]').is(':checked')) {
                        enableudpSubmit();
                    }
                    else {
                        enableConnectionSubmit();
                    }

                }
                if (Data.CONECTION_CONNECTED === $('input[type=checkbox][name=active_connection]').is(':checked')) {
                    $('button[id=apisubmitBtn]').prop('disabled', true);
                    if ($('input[type=checkbox][name=udp_connection]').is(':checked')) {
                        enableudpSubmit();
                    }
                    else {
                        enableConnectionSubmit();
                    }
                }
            });
            $('input[type=checkbox][name=udp_connection]').change(() => {
                if (Data.UDP_CONNECTION !== $('input[type=checkbox][name=udp_connection]').is(':checked')) {
                    $('button[id=apisubmitBtn]').prop('disabled', false);
                    enableudpSubmit();
                }
                if (Data.UDP_CONNECTION === $('input[type=checkbox][name=udp_connection]').is(':checked')) {
                    $('button[id=apisubmitBtn]').prop('disabled', true);
                    enableudpSubmit();
                }
            });
            if (!$.isEmptyObject(Data)) {
                $('input[type=text][name=connection_name]').val(Data.CONNECTION_NAME);
                $('input[type=text][name=ip_address]').val(Data.IP_ADDRESS);
                $('input[type=text][name=hostname]').val(Data.HOSTNAME);
                $('input[type=text][name=port_number]').val(Data.PORT);
                $('input[type=text][name=url]').val(Data.URL);
                $('input[type=text][name=outgoingapikey]').val(Data.OUTGOING_APIKEY);
                $('input[type=text][name=message_type]').val(Data.MESSAGE_TYPE);
                $('select[name=data_retrieve]').val(Data.DATA_RETRIEVE);
                $('input[type=text][name=admin_email_recepient]').val(Data.ADMIN_EMAIL_RECEPIENT);
                //API connection
                if (Data.ACTIVE_CONNECTION) {
                    if (!$('input[type=checkbox][name=active_connection]').is(':checked')) {
                        $('input[type=checkbox][name=active_connection]').prop('checked', true).change();
                    }
                }
                else {
                    if ($('input[type=checkbox][name=active_connection]').is(':checked')) {
                        $('input[type=checkbox][name=active_connection]').prop('checked', false).change();
                    }
                }
                if (Data.UDP_CONNECTION) {
                    if (!$('input[type=checkbox][name=udp_connection]').is(':checked')) {
                        $('input[type=checkbox][name=udp_connection]').prop('checked', true).change();
                    }
                }
                else {
                    if ($('input[type=checkbox][name=udp_connection]').is(':checked')) {
                        $('input[type=checkbox][name=udp_connection]').prop('checked', false).change();
                    }
                }

                $('button[id=apisubmitBtn]').off().on('click', function () {
                    try {
                        $('button[id=apisubmitBtn]').prop('disabled', true);
                        var jsonObject = {};
                        //API connection active
                        if (Data.ACTIVE_CONNECTION !== $('input[type=checkbox][name=active_connection]').is(':checked')) {
                            if ($('input[type=checkbox][name=active_connection]').is(':checked')) {
                                jsonObject.ACTIVE_CONNECTION = true;
                            }
                            else {
                                jsonObject.ACTIVE_CONNECTION = false;
                            }
                        }
                        if (Data.UDP_CONNECTION !== $('input[type=checkbox][name=udp_connection]').is(':checked')) {
                            if ($('input[type=checkbox][name=udp_connection]').is(':checked')) {
                                jsonObject.UDP_CONNECTION = true;
                            }
                            else {
                                jsonObject.UDP_CONNECTION = false;
                            }
                        }
                        parseInt($('select[name=data_retrieve] option:selected').val()) !== parseInt(Data.DATA_RETRIEVE) ? jsonObject.DATA_RETRIEVE = $('select[name=data_retrieve] option:selected').val() : "";
                        $('input[type=text][name=connection_name]').val() !== Data.CONNECTION_NAME ? jsonObject.CONNECTION_NAME = $('input[type=text][name=connection_name]').val() : "";
                        $('input[type=text][name=ip_address]').val() !== Data.IP_ADDRESS ? jsonObject.IP_ADDRESS = $('input[type=text][name=ip_address]').val() : "";
                        $('input[type=text][name=hostname]').text() !== Data.HOSTNAME ? jsonObject.HOSTNAME = $('input[type=text][name=hostname]').text() : "";
                        parseInt($('input[type=text][name=port_number]').val()) !== parseInt(Data.PORT) ? jsonObject.PORT = $('input[type=text][name=port_number]').val() : "";
                        $('input[type=text][name=url]').val() !== Data.URL ? jsonObject.URL = $('input[type=text][name=url]').val() : "";
                        $('input[type=text][name=outgoingapikey]').val() !== Data.OUTGOING_APIKEY ? jsonObject.OUTGOING_APIKEY = $('input[type=text][name=outgoingapikey]').val() : "";
                        $('input[type=text][name=message_type]').val() !== Data.MESSAGE_TYPE ? jsonObject.MESSAGE_TYPE = $('input[type=text][name=message_type]').val() : "";
                        if (!$.isEmptyObject(jsonObject)) {
                            jsonObject.LASTUPDATE_BY_USERNAME = User.UserId;
                            jsonObject.ID = Data.ID;
                            if (checkValue(User.Facility_NASS_Code)) {
                                checkValue($('input[type=text][name=url]').val()) ? jsonObject.URL = $('input[type=text][name=url]').val().supplant(formatURL(User.Facility_NASS_Code)) : '';
                            }
                            $.connection.FOTFManager.server.editAPI(JSON.stringify(jsonObject)).done(function (Data) {
                                if (Data.length === 1) {
                                    if (Data[0].hasOwnProperty("ID")) {
                                        $('span[id=error_apisubmitBtn]').text(Data[0].CONNECTION_NAME + " " + Data[0].MESSAGE_TYPE + " Connection has been Updated.");
                                        setTimeout(function () { $("#API_Connection_Modal").modal('hide'); }, 1500);
                                    }
                                    else {
                                        $('span[id=error_apisubmitBtn]').text(Data[0].CONNECTION_NAME + " " + Data[0].MESSAGE_TYPE + "Connection error been Added");
                                        setTimeout(function () { $("#API_Connection_Modal").modal('hide'); }, 1500);
                                    }
                                }
                                else {
                                    $('span[id=error_apisubmitBtn]').text("Error editing Connection");
                                }
                            });
                        };
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
    if (data.ACTIVE_CONNECTION) {
        if (data.API_CONNECTED) {
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
    if (data.ACTIVE_CONNECTION) {
        if (data.API_CONNECTED) {
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
function Remove_Connection(data) {
    try {
        //RemoveNotificationModal
        if ($.isNumeric(data)) {
            var id = parseInt(data);
            sidebar.close('connections');
            $('#removeAPImodalHeader_ID').text('Remove API Connection');
            $('#RemoveConfirmationModal').modal();

            $('button[id=remove_server_connection]').off().on('click', function () {
                var jsonObject = { ID: id };
                $.connection.FOTFManager.server.removeAPI(JSON.stringify(jsonObject)).done(function (Data) {
                    $("#api_" + id).remove();
                    setTimeout(function () {
                        $("#RemoveConfirmationModal").modal('hide');
                        sidebar.open('connections');
                    }, 1500);
                    $('#RemoveConfirmationModal').modal();
                })
            });
        }
    } catch (e) {
        console.log(e);
    }
}

function enableConnectionSubmit() {
    //AGV connections
    if ($('input[type=text][name=ip_address]').hasClass('is-valid') &&
        $('input[type=text][name=url]').hasClass('is-valid') &&
        $('input[type=text][name=outgoingapikey]').hasClass('is-valid') &&
        $('input[type=text][name=message_type]').hasClass('is-valid') &&
        $('select[name=data_retrieve]').hasClass('is-valid') &&
        $('input[type=text][name=admin_email_recepient]').hasClass('is-valid') &&
        $('input[type=text][name=connection_name]').hasClass('is-valid')
    ) {
        $('button[id=apisubmitBtn]').prop('disabled', false);
    }
    else {
        $('button[id=apisubmitBtn]').prop('disabled', true);
    }
}
function enableudpSubmit() {
    if ($('input[type=text][name=message_type]').hasClass('is-valid') &&
        $('input[type=text][name=port_number]').hasClass('is-valid') &&
        $('input[type=text][name=admin_email_recepient]').hasClass('is-valid') &&
        $('input[type=text][name=connection_name]').hasClass('is-valid')
    ) {
        $('button[id=apisubmitBtn]').prop('disabled', false);
    }
    else {
        $('button[id=apisubmitBtn]').prop('disabled', true);
    }
}
function SortByConnectionName(a, b) {
    return a.CONNECTION_NAME < b.CONNECTION_NAME ? -1 : a.CONNECTION_NAME > b.CONNECTION_NAME ? 1 : 0;
}