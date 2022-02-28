/**
 * this is use to setup a the machine information and other function
 * 
 * **/
//on close clear all inputs
$('#Zone_Modal').on('hidden.bs.modal', function () {
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
//on open set rules
$('#Zone_Modal').on('shown.bs.modal', function () {
    $('span[id=error_machinesubmitBtn]').text("");
    $('button[id=machinesubmitBtn]').prop('disabled', true);
    //Request Type Keyup
    $('input[type=text][name=machine_name]').keyup(function () {
        if (!checkValue($('input[type=text][name=machine_name]').val())) {
            $('input[type=text][name=machine_name]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_machine_name]').text("Please Enter Machine Name");
        }
        else {
            $('input[type=text][name=machine_name]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_machine_name]').text("");
        }

        enablezoneSubmit();
    });
    //Connection name Validation
    if (!checkValue($('input[type=text][name=machine_name]').val())) {
        $('input[type=text][name=machine_name]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_machine_name]').text("Please Enter Machine Name");
    }
    else {
        $('input[type=text][name=machine_name]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_machine_name]').text("");
    }
    //Request Type Keyup
    $('input[type=text][name=machine_number]').keyup(function () {
        if (!checkValue($('input[type=text][name=machine_number]').val())) {
            $('input[type=text][name=machine_number]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
            $('span[id=error_machine_number]').text("Please Enter Machine Number");
        }
        else {
            $('input[type=text][name=machine_number]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_machine_number]').text("");
        }

        enablezoneSubmit();
    });
    //Request Type Validation
    if (!checkValue($('input[type=text][name=machine_number]').val())) {
        $('input[type=text][name=machine_number]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
        $('span[id=error_machine_number]').text("Please Enter Machine Number");
    }
    else {
        $('input[type=text][name=machine_number]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_machine_number]').text("");
    }
    //Request Type Validation
    if (checkValue($('input[type=text][name=zone_ldc]').val())) {
        $('input[type=text][name=zone_ldc]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
        $('span[id=error_zone_ldc]').text("");
    }
    else {
        $('input[type=text][name=zone_ldc]').css("border-color", "#D3D3D3").removeClass('is-invalid').removeClass('is-valid');
        $('span[id=error_zone_ldc]').text("");
    }
    //Request zone LDC Keyup
    $('input[type=text][name=zone_ldc]').keyup(function () {
        if (checkValue($('input[type=text][name=zone_ldc]').val())) {
            $('input[type=text][name=zone_ldc]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
            $('span[id=error_zone_ldc]').text("");
        }
        else {
            $('input[type=text][name=zone_ldc]').css("border-color", "#D3D3D3").removeClass('is-invalid').removeClass('is-valid');
            $('span[id=error_zone_ldc]').text("");
        }
    });
});
$.extend(fotfmanager.client, {
    updateMachineStatus: async (updateMachine) => { updateMachineZone(updateMachine) }
});

async function updateMachineZone(machineupdate) {
    try {
        if (machineupdate) {
            if (polygonMachine.hasOwnProperty("_layers")) {
                var layerindex = -0;
                $.map(polygonMachine._layers, function (layer, i) {
                    if (layer.hasOwnProperty("feature")) {
                        if (layer.feature.properties.id === machineupdate.properties.id) {
                            if (layer.feature.properties.name != machineupdate.properties.name) {
                                layer.setTooltipContent(machineupdate.properties.name + "<br/>" + "Staffing: " + machineupdate.properties.CurrentStaff);
                            }
                            layer.feature.properties = machineupdate.properties;
                            layerindex = layer._leaflet_id;
                            return false;
                        }
                    }
                });
                if (layerindex !== -0) {
                    if (machineupdate.properties.hasOwnProperty("MPEWatchData")) {
                        if (polygonMachine._layers[layerindex].feature.properties.id === machineupdate.properties.id) {
                            //Have to update polygonMachine._layers[layerindex].feature.properties with update or when you navigate away from a machine and then back it loads old information.
                            polygonMachine._layers[layerindex].feature.properties.MPEWatchData = machineupdate.properties.MPEWatchData;
                            polygonMachine._layers[layerindex].feature.properties.DPSData = machineupdate.properties.DPSData;
                            polygonMachine._layers[layerindex].feature.properties.CurrentStaff = machineupdate.properties.CurrentStaff;
                            if ($('select[id=zoneselect] option:selected').val() === machineupdate.properties.id) {
                                LoadMachineTables(machineupdate.properties, 'machinetable');
                            }
                        }
                        var sortplan =  machineupdate.properties.MPEWatchData.hasOwnProperty("cur_sortplan") ? machineupdate.properties.MPEWatchData.cur_sortplan : "";
                        var endofrun =  machineupdate.properties.MPEWatchData.hasOwnProperty("current_run_end") ? machineupdate.properties.MPEWatchData.current_run_end : "";
                        var startofrun = machineupdate.properties.MPEWatchData.hasOwnProperty("current_run_start") ? machineupdate.properties.MPEWatchData.current_run_start : "" ;
                        var expectedTP = machineupdate.properties.MPEWatchData.hasOwnProperty("expected_throughput") ? machineupdate.properties.MPEWatchData.expected_throughput : "" ;
                        var throughput = machineupdate.properties.MPEWatchData.hasOwnProperty("cur_thruput_ophr") ? machineupdate.properties.MPEWatchData.cur_thruput_ophr : "" ;

                        if (checkValue(sortplan) && !checkValue(endofrun)) {
                            var fillColor = GetMacineBackground(startofrun, throughput, expectedTP);
                            polygonMachine._layers[layerindex].setStyle({
                                weight: 1,
                                opacity: 1,
                                fillOpacity: 0.5,
                                fillColor: fillColor
                            });
                        }
                        else {
                            if (polygonMachine._layers[layerindex].options.fillColor !== '#989ea4') { //'gray'
                                polygonMachine._layers[layerindex].setStyle({
                                    weight: 1,
                                    opacity: 1,
                                    fillOpacity: 0.2,
                                    fillColor: '#989ea4'//'gray'
                                });
                            }
                        }
                    }
                }
                else {
                    polygonMachine.addData(machineupdate);
                }
            }
        }
    } catch (e) {
        console.log(e);
    }
}
$(function () {
    $('button[name=machineinfoedit]').off().on('click', function () {
        /* close the sidebar */
        sidebar.close();
        var id = $(this).attr('id');
        if (checkValue(id)) {
            Edit_Machine_Info(id);
        }

    });
    

});
var polygonMachine = new L.GeoJSON(null, {
    style: function (feature) {
        if (feature.properties.visible) {
            var style = {};
            var sortplan = feature.properties.hasOwnProperty("MPEWatchData") ? feature.properties.MPEWatchData.hasOwnProperty("cur_sortplan") ? feature.properties.MPEWatchData.cur_sortplan : "" : "";
            var endofrun = feature.properties.hasOwnProperty("MPEWatchData") ? feature.properties.MPEWatchData.hasOwnProperty("current_run_end") ? feature.properties.MPEWatchData.current_run_end : "" : "";
            var startofrun = feature.properties.hasOwnProperty("MPEWatchData") ? feature.properties.MPEWatchData.hasOwnProperty("current_run_start") ? feature.properties.MPEWatchData.current_run_start : "" : "";
            var expectedTP = feature.properties.hasOwnProperty("MPEWatchData") ? feature.properties.MPEWatchData.hasOwnProperty("expected_throughput") ? feature.properties.MPEWatchData.expected_throughput : "" : "";
            var throughput = feature.properties.hasOwnProperty("MPEWatchData") ? feature.properties.MPEWatchData.hasOwnProperty("cur_thruput_ophr") ? feature.properties.MPEWatchData.cur_thruput_ophr : "" : "";
            if (checkValue(sortplan) && !checkValue(endofrun)) {
                var fillColor = GetMacineBackground(startofrun, throughput, expectedTP);
                style = {
                    weight: 1,
                    opacity: 1,
                    color: '#3573b1',
                    fillOpacity: 0.5,
                    fillColor: fillColor
                };
            }
            else {
                style = {
                    weight: 1,
                    opacity: 1,
                    color: '#3573b1',
                    fillOpacity: 0.2,
                    fillColor: '#989ea4'
                };
            }
            return style;
        }
    },
    onEachFeature: function (feature, layer) {
        //$('<option>', { text: feature.properties.name, value: feature.properties.id }).appendTo('select[id=zoneselect]');
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        layer.on('click', function (e) {
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.sourceTarget.getCenter(), 3);
            sidebar.open('home');
            LoadMachineTables(feature.properties, 'machinetable');
        });
        layer.bindTooltip(feature.properties.name + "<br/>" + "Staffing: " + (feature.properties.hasOwnProperty("CurrentStaff") ? feature.properties.CurrentStaff : "0"), {
            permanent: true,
            direction: 'center',
            opacity: 1,
            className: 'location'
        }).openTooltip();
    },
    filter: function (feature, layer) {
        if (/(way)$/i.test(feature.properties.name)) {
            return false;
        }
        else {
            return feature.properties.visible;
        }
    }
});
async function LoadMachineTables(dataproperties, table) {
    try {
        if (!$.isEmptyObject(dataproperties)) {
            $('div[id=dockdoor_div]').css('display', 'none');
            $('div[id=trailer_div]').css('display', 'none');
            $('div[id=area_div]').css('display', 'none');
            $('div[id=agvlocation_div]').css('display', 'none');
            $('div[id=vehicle_div]').css('display', 'none');
            $('div[id=machine_div]').css('display', 'block');
            $('div[id=ctstabs_div]').css('display', 'block');
            if (/machinetable/i.test(table)) {
                
                $zoneSelect[0].selectize.setValue(dataproperties.id, true);
                $('button[name=machineinfoedit]').attr('id', dataproperties.id)
                $('div[id=dps_div]').css('display', 'none');
                let machinetop_Table = $('table[id=' + table + ']');
                let machinetop_Table_Body = machinetop_Table.find('tbody');
                machinetop_Table_Body.empty();
                machinetop_Table_Body.append(machinetop_row_template.supplant(formatmachinetoprow(dataproperties)));

                if (dataproperties.MPEWatchData.hasOwnProperty("bin_full_bins")) {
                    if (dataproperties.MPEWatchData.bin_full_bins != "") {
                        var result_style = document.getElementById('fullbin_tr').style;
                        result_style.display = 'table-row';
                    }
                }

                if (dataproperties.MPEWatchData.cur_operation_id == "918" || dataproperties.MPEWatchData.cur_operation_id == "919") {
                    LoadMachineTables(dataproperties, "dpstable");
                }
                var p2pdata = dataproperties.hasOwnProperty("P2PData") ? dataproperties.P2PData : "";
                var MachineCurrentStaff = [];
                GetPeopleInZone(dataproperties.id, p2pdata, MachineCurrentStaff);
                if (dataproperties.MPEWatchData.hasOwnProperty("hourly_data")) {
                    GetMachinePerfGraph(dataproperties);
                }
                else {
                    var mpgtrStyle = document.getElementById('machineChart_tr').style;
                    mpgtrStyle.display = 'none';
                }
                
            }
            if (/dpstable/i.test(table)) {
                if (dataproperties.hasOwnProperty("DPSData")) {
                    if (checkValue(dataproperties.DPSData)) {
                        $zoneSelect[0].selectize.setValue(dataproperties.id, true);
                        $('div[id=dps_div]').css('display', 'block');
                        let dpstop_Table = $('table[id=' + table + ']');
                        let dpstop_Table_Body = dpstop_Table.find('tbody')
                        dpstop_Table_Body.empty();
                        dpstop_Table_Body.append(dpstop_row_template.supplant(formatdpstoprow(dataproperties)));
                    }
                }
            }

        }
    } catch (e) {
        console.log(e);
    }
}
function formatmachinetoprow(properties) {
    return $.extend(properties, {
        zoneId: properties.id,
        zoneName: properties.name,
        zoneType: properties.Zone_Type,
        sortPlan: properties.hasOwnProperty("MPEWatchData") ? properties.MPEWatchData.hasOwnProperty("cur_sortplan") ? properties.MPEWatchData.cur_sortplan : "N/A" : "",
        opNum: properties.hasOwnProperty("MPEWatchData") ? properties.MPEWatchData.hasOwnProperty("cur_operation_id") ? properties.MPEWatchData.cur_operation_id.padStart(3, "0") : "000" : "000",
        sortPlanStart: properties.hasOwnProperty("MPEWatchData") ? properties.MPEWatchData.hasOwnProperty("current_run_start") ? properties.MPEWatchData.current_run_start : "00:00:00" : "",
        sortPlanEnd: properties.hasOwnProperty("MPEWatchData") ? properties.MPEWatchData.hasOwnProperty("current_run_end") ? checkValue(properties.MPEWatchData.current_run_end) ? properties.MPEWatchData.current_run_end : "00:00:00" : "00:00:00" : "",
        peicesFed: properties.hasOwnProperty("MPEWatchData") ? properties.MPEWatchData.hasOwnProperty("tot_sortplan_vol") ? digits(properties.MPEWatchData.tot_sortplan_vol) : 0 : "",
        throughput: properties.hasOwnProperty("MPEWatchData") ? properties.MPEWatchData.hasOwnProperty("cur_thruput_ophr") ? digits(properties.MPEWatchData.cur_thruput_ophr) : 0 : "",
        rpgVol: properties.hasOwnProperty("MPEWatchData") ? properties.MPEWatchData.hasOwnProperty("rpg_est_vol") ? digits(properties.MPEWatchData.rpg_est_vol) : 0 : "",
        stateBadge: getstatebadge(properties),
        stateText: getstateText(properties),
        estComp: properties.hasOwnProperty("MPEWatchData") ? properties.MPEWatchData.hasOwnProperty("rpg_est_comp_time") ? properties.MPEWatchData.rpg_est_comp_time : "Not Available" : "Estimated Not Available",
        rpgStart: properties.hasOwnProperty("MPEWatchData") ? properties.MPEWatchData.hasOwnProperty("rpg_start_dtm") ? moment(properties.MPEWatchData.rpg_start_dtm, "MM/DD/YYYY hh:mm:ss A").format("YYYY-MM-DD HH:mm:ss") : "" : "",
        rpgEnd: properties.hasOwnProperty("MPEWatchData") ? properties.MPEWatchData.hasOwnProperty("rpg_end_dtm") ? moment(properties.MPEWatchData.rpg_end_dtm, "MM/DD/YYYY hh:mm:ss A").format("YYYY-MM-DD HH:mm:ss") : "" : "",
        expThroughput: properties.hasOwnProperty("MPEWatchData") ? properties.MPEWatchData.hasOwnProperty("expected_throughput") ? digits(properties.MPEWatchData.expected_throughput) : 0 : "",
        fullBins: properties.hasOwnProperty("MPEWatchData") ? properties.MPEWatchData.hasOwnProperty("bin_full_bins") ? properties.MPEWatchData.bin_full_bins : "" : "",
    });
}
let machinetop_row_template = '<tr data-id="{zoneId}"><td>{zoneType}</td><td>{zoneName}</td><td><span class="badge badge-pill {stateBadge}" style="font-size: 12px;">{stateText}</span></td></tr>' +
    '<tr><td>Sort Plan</td><td colspan="2">{sortPlan}</td></tr>' +
    '<tr><td>Operation Number</td><td colspan="2">{opNum}</td></tr>' +
    '<tr><td>Start</td><td colspan="2">{sortPlanStart}</td></tr>' +
    '<tr><td>End</td><td colspan="2">{sortPlanEnd}</td></tr>' +
    '<tr><td>Estimated Completion</td><td colspan="2">{estComp}</td>/tr>' +
    '<tr><td>Pieces Fed</td><td colspan="2">{peicesFed}</td></tr>' +
    '<tr><td>Throughput</td><td colspan="2">{throughput}</td></tr>' +
    '<tr><td>RPG Vol</td><td colspan="2">{rpgVol}</td></tr>' +
    '<tr><td>Expected Throughput</td><td colspan="2">{expThroughput}</td></tr>' +
    '<tr id="fullbin_tr" style="display: none;"><td>Full Bins</td><td colspan="2" style="white-space: normal; word-wrap:break-word;">{fullBins}</td></tr>' + 
    '<tr id="machineChart_tr"><td colspan="3"><canvas id="machinechart"></canvas></td></tr>'
    ;
function formatdpstoprow(properties) {
    return $.extend(properties, {
        dpssortplans: properties.hasOwnProperty("DPSData") ? properties.DPSData.hasOwnProperty("sortplan_name_perf") ? properties.DPSData.sortplan_name_perf : "" : "",
        piecesfedfirstpass: properties.hasOwnProperty("DPSData") ? properties.DPSData.hasOwnProperty("pieces_fed_1st_cnt") ? digits(properties.DPSData.pieces_fed_1st_cnt) : "" : "",
        piecesrejectedfirstpass: properties.hasOwnProperty("DPSData") ? properties.DPSData.hasOwnProperty("pieces_rejected_1st_cnt") ? digits(properties.DPSData.pieces_rejected_1st_cnt) : "" : "",
        piecestosecondpass: properties.hasOwnProperty("DPSData") ? properties.DPSData.hasOwnProperty("pieces_to_2nd_pass") ? digits(properties.DPSData.pieces_to_2nd_pass) : "" : "",
        piecesfedsecondpass: properties.hasOwnProperty("DPSData") ? properties.DPSData.hasOwnProperty("pieces_fed_2nd_cnt") ? digits(properties.DPSData.pieces_fed_2nd_cnt) : "" : "",
        piecesrejectedsecondpass: properties.hasOwnProperty("DPSData") ? properties.DPSData.hasOwnProperty("pieces_rejected_2nd_cnt") ? digits(properties.DPSData.pieces_rejected_2nd_cnt) : "" : "",
        piecesremainingsecondpass: properties.hasOwnProperty("DPSData") ? properties.DPSData.hasOwnProperty("pieces_remaining") ? digits(properties.DPSData.pieces_remaining) : "" : "",
        timetocompleteactual: properties.hasOwnProperty("DPSData") ? properties.DPSData.hasOwnProperty("time_to_comp_actual") ? digits(properties.DPSData.time_to_comp_actual) : "" : "",
        timeleftsecondpassactual: properties.hasOwnProperty("DPSData") ? properties.DPSData.hasOwnProperty("time_to_2nd_pass_actual") ? digits(properties.DPSData.time_to_2nd_pass_actual) : "" : "",
        recomendedstartactual: properties.hasOwnProperty("DPSData") ? properties.DPSData.hasOwnProperty("rec_2nd_pass_start_actual") ? properties.DPSData.rec_2nd_pass_start_actual : "" : "",
        completiondateTime: properties.hasOwnProperty("DPSData") ? properties.DPSData.hasOwnProperty("time_to_comp_actual_DateTime") ? properties.DPSData.time_to_comp_actual_DateTime : "" : "",
    });
}
let dpstop_row_template =
    '<tr><td>DPS Sort Plans</td><td>{dpssortplans}</td><td></td></tr>' +
    '<tr><td><b>First Pass</b></td><td></td><td></td></tr>' +
    '<tr><td>Pieces Fed</td><td>{piecesfedfirstpass}</td><td></td></tr>' +
    '<tr><td>Pieces Rejected</td><td>{piecesrejectedfirstpass}</td><td></td></tr>' +
    '<tr><td>Pieces To Second Pass</td><td>{piecestosecondpass}</td><td></td></tr>' +
    '<tr><td>Rec. 2nd Pass Start Time</td><td>{recomendedstartactual}</td><td></td></tr>' +
    '<tr><td><b>Second Pass</b></td><td></td><td></td></tr>' +
    '<tr><td>Pieces Fed</td><td>{piecesfedsecondpass}</td><td></td></tr>' +
    '<tr><td>Pieces Rejected</td><td>{piecesrejectedsecondpass}</td><td></td></tr>' +
    '<tr><td>Pieces Remaining</td><td>{piecesremainingsecondpass}</td><td></td></tr>' +
    '<tr><td>Est. Completion Time</td><td>{completiondateTime}</td><td></td></tr>'
    ;

async function LoadMachineDetails(selcValue) {
    try {
        if (polygonMachine.hasOwnProperty("_layers")) {
            $.map(polygonMachine._layers, function (layer, i) {
                if (layer.hasOwnProperty("feature")) {
                    if (layer.feature.properties.id === selcValue) {
                        var Center = new L.latLng(
                            (layer._bounds._southWest.lat + layer._bounds._northEast.lat) / 2,
                            (layer._bounds._southWest.lng + layer._bounds._northEast.lng) / 2);
                        map.setView(Center, 3);
                        if (/Machine/i.test(layer.feature.properties.Zone_Type)) {
                            LoadMachineTables(layer.feature.properties, 'machinetable');
                        }
                        return false;
                    }
                }
            });
        }
    } catch (e) {
        console.log(e)
    }

}
$('#zoneselect').change(function (e) {
    $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
    var selcValue = this.value;
    LoadMachineDetails(selcValue);

});
function getstatebadge(properties) {
    if (properties.hasOwnProperty("MPEWatchData")) {
        if (properties.MPEWatchData.hasOwnProperty("current_run_end")) {
            var endtime = moment(properties.MPEWatchData.current_run_end);
            var starttime = moment(properties.MPEWatchData.current_run_start);
            var sortPlan = properties.MPEWatchData.cur_sortplan;

            if (starttime._isValid && !endtime._isValid) {
                if (sortPlan != "") {
                    return "badge badge-success";
                }
                else {
                    return "badge badge-info";
                }
            }
            else if (!starttime._isValid && !endtime._isValid) {
                return "badge badge-info";
            }
            else if (starttime._isValid && endtime._isValid) {
                return "badge badge-info";
            }
        }
        else {
            return "badge badge-secondary";
        }
    }
    else {
        return "badge badge-secondary";
    }
}
function getstateText(properties) {
    if (properties.hasOwnProperty("MPEWatchData")) {
        if (properties.MPEWatchData.hasOwnProperty("current_run_end")) {
            var endtime = moment(properties.MPEWatchData.current_run_end);
            var starttime = moment(properties.MPEWatchData.current_run_start);
            var sortPlan = properties.MPEWatchData.cur_sortplan;

            if (starttime._isValid && !endtime._isValid) {
                if (sortPlan != "") {
                    return "Running";
                }
                else {
                    return "Idle";
                }
            }
            else if (!starttime._isValid && !endtime._isValid) {
                return "Unknown";
            }
            else if (starttime._isValid && endtime._isValid) {
                return "Idle";
            }
        }
        else {
            return "No Data";
        }
    }
    else {
        return "No Data";
    }
}
async function Edit_Machine_Info(id) {
    $('#modalZoneHeader_ID').text('Edit Machine Info');

    sidebar.close('connections');

    $('button[id=machinesubmitBtn]').prop('disabled', true);
    try {
        if (polygonMachine.hasOwnProperty("_layers")) {
            let layerindex = -0;
            let Data = {};
            $.map(polygonMachine._layers, function (layer, i) {
                if (layer.hasOwnProperty("feature")) {
                    if (layer.feature.properties.id === id) {
                        Data = layer.feature.properties;
                        layerindex = layer._leaflet_id;
                        return false;
                    }
                }
            });
            if (layerindex !== -0) {

                if (!$.isEmptyObject(Data)) {
                    $('input[type=text][name=machine_name]').val(Data.MPE_Type);
                    $('input[type=text][name=machine_number]').val(Data.MPE_Number);
                    $('input[type=text][name=zone_ldc]').val(Data.Zone_LDC);
                    $('input[type=text][name=machine_id]').val(Data.id);


                    $('button[id=machinesubmitBtn]').off().on('click', function () {
                        try {
                            $('button[id=machinesubmitBtn]').prop('disabled', true);
                            var jsonObject = {
                            MPE_Type: $('input[type=text][name=machine_name]').val(),
                            MPE_Number: $('input[type=text][name=machine_number]').val(),
                            Zone_LDC: $('input[type=text][name=zone_ldc]').val()
                            };

                            if (!$.isEmptyObject(jsonObject)) {
                                jsonObject.id = Data.id;
                                fotfmanager.server.editZone(JSON.stringify(jsonObject)).done(function (updatedData) {
                                    if (updatedData.length === 1) {
                                        if (updatedData[0].hasOwnProperty("properties")) {
                                            if (updatedData[0].properties.hasOwnProperty("id")) {
                                                $('span[id=error_machinesubmitBtn]').text(updatedData[0].properties.MPE_Type + " Zone has been Updated.");
                                                updateMachineZone(updatedData[0]);
                                                setTimeout(function () { $("#Zone_Modal").modal('hide'); }, 1500);
                                            }
                                            else {
                                                $('span[id=error_machinesubmitBtn]').text(updatedData[0].properties.MPE_Type + " Zone error Updating ");
                                                setTimeout(function () { $("#Zone_Modal").modal('hide'); }, 1500);
                                            }
                                        }
                                    }
                                    else {
                                        $('span[id=error_machinesubmitBtn]').text("Error editing Zone");
                                    }
                                });
                            };
                        } catch (e) {
                            $('span[id=error_machinesubmitBtn]').text(e);
                        }
                    });
                    $('#Zone_Modal').modal();
                }
                else {
                    $('label[id=error_machinesubmitBtn]').text("Invalid Zone ID");
                    $('#Zone_Modal').modal();
                }
            }
            else {
                $('label[id=error_machinesubmitBtn]').text("Invalid Zone ID");
                $('#Zone_Modal').modal();
            }
        }
    } catch (e) {
        console.log(e);
    }
}
function enablezoneSubmit() {
    //AGV connections
    if ($('input[type=text][name=machine_name]').hasClass('is-valid') &&
        $('input[type=text][name=machine_number]').hasClass('is-valid') 
    ) {
        $('button[id=machinesubmitBtn]').prop('disabled', false);
    }
    else {
        $('button[id=machinesubmitBtn]').prop('disabled', true);
    }
}
async function init_machine() {
    //Get MachineZones list
    fotfmanager.server.getMachineZonesList().done(function (machinedatazone) {
        if (machinedatazone.length > 0) {
            $.each(machinedatazone, function () {
                updateMachineZone(this);
            });
            var options = $('#zoneselect option');
            var arr = options.map(function (_, o) { return { t: $(o).text(), v: o.value }; }).get();
            arr.sort(function (o1, o2) { return o1.t > o2.t ? 1 : o1.t < o2.t ? -1 : 0; });
            options.each(function (i, o) {
                o.value = arr[i].v;
                $(o).text(arr[i].t);
            });
        }
    })
}
function GetMacineBackground(starttime, throughput, expectedthr) {
    var curtime = moment().format('YYYY-MM-DD HH:mm:ss');

    if (!$.isEmptyObject(timezone)) {
        if (timezone.hasOwnProperty("Facility_TimeZone")) {
            curtime = moment().tz(timezone.Facility_TimeZone).format('YYYY-MM-DD HH:mm:ss');
        }
    }
    var dt = moment(curtime);
    var st = moment(starttime);
    var timeduration = moment.duration(dt.diff(st));
    var minutes = parseInt(timeduration.asMinutes());
    if (minutes > 15) {
        if (expectedthr != '' && throughput != '') {
            var expTP = parseInt(expectedthr, 10);
            var ThrPt = parseInt(throughput, 10);
            if ($.isNumeric(expTP) && $.isNumeric(ThrPt)) {
                var percent = (ThrPt / expTP) * 100;
                if (percent >= 100) {
                    return '#3573b1'; //'#cce5ff'
                }
                if (percent >= 90) {
                    return '#ffc107'; //'#FFFF88'
                }
                if (percent < 90) {
                    return '#dc3545'; //'#FF4444'
                }
            }
        }
    }
    return '#3573b1'; //'#cce5ff'
}
function GetMachinePerfGraph(dataproperties)
{
    var xValues = [];
    var yValues = [];
    for (var i = 0; i < dataproperties.MPEWatchData.hourly_data.length; i++) {
        xValues.unshift(dataproperties.MPEWatchData.hourly_data[i].count);
        yValues.unshift(dataproperties.MPEWatchData.hourly_data[i].hour);
    }

    new Chart("machinechart", {
        type: "line",
        data: {
            labels: yValues,
            datasets: [{
                fill: false,
                lineTension: 0,
                backgroundColor: "rgba(0,0,255,1.0)",
                borderColor: "rgba(0,0,255,0.1)",
                data: xValues
            }]
        },
        options: {
            legend: { display: false },
            title: {
                display: true,
                text: "Pieces Fed 24 Hours"
            },
            tooltips: {
                callbacks: {
                    title: function (tooltipItem, data) {
                        var hour = ("0" + new Date(tooltipItem[0].xLabel).getHours().toString()).slice(-2);
                        return tooltipItem[0].xLabel.toString() + "-" + hour + ":59";
                    },

                    label: function (tooltipItem, data) {
                        return tooltipItem.yLabel.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + " pieces";
                    },
                },
            },
            scales: {
                xAxes: [{
                    gridLines: {
                        display: true
                    },
                    ticks: {
                        autoSkip: true,
                        maxTicksLimit: 12,
                        callback: function (value, index, values) {
                            var hour = ("0" + new Date(value).getHours().toString()).slice(-2);
                            return hour + ":00-" + hour + ":59";

                        }
                    }
                }],
                yAxes: [{
                    ticks: {
                        beginAtZero: true,
                        callback: function (value, index, values) {
                            if (parseInt(value) >= 1000) {
                                return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                            } else {
                                return value;
                            }
                        }
                    }
                }]
            }
        }
    });
}

