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

// sets the sparkline graph cache so when it is ready to display,
// no graphs need to be created
// allow some time period in between function calls so that tags can continue to move

function updateAllMachineSparklines(machineStatuses, index) {
    if (index === machineStatuses.length) {
        // at the end of all calls, now check for sparklines and add to map.
        for (var tuple of machineStatuses) {

            var sortPlan1 =
                tuple.Item1.properties.MPEWatchData.cur_sortplan;
            if (sortPlan1 != "") {
                updateMachineSparkline(tuple.Item1, tuple.Item2);
            }
        }
    }
    else {
        var sortPlan2 =
            machineStatuses[index].Item1.properties.MPEWatchData.cur_sortplan;

        if (sortPlan2 != "") {
            // get the graph and update it to sparkline cache
            GetSparklineGraph(machineStatuses[index].Item1.properties,
                machineStatuses[index].Item1.properties.id);
        }
        setTimeout(() => {
            updateAllMachineSparklines(machineStatuses, index + 1);
        }, 10);
    }
}
$.extend(fotfmanager.client, {
    updateMachineStatus: async (machineStatuses) =>
    {
        for (var tuple of machineStatuses) {
            
            updateMachineZone(tuple.Item1, tuple.Item2);
           
        }
        // clears the sparkline graph cache if it is old data
        clearSparklineCache();
        updateAllMachineSparklines(machineStatuses, 0);
    }
});

async function updateMachineZone(machineupdate, id) {
    try {
        if (id == baselayerid) {
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
                    if ($('div[id=machine_div]').is(':visible') && $('div[id=machine_div]').attr("data-id") === machineupdate.properties.id) {
                        LoadMachineTables(machineupdate.properties, 'machinetable');
                    }
                    updateMPEZone(machineupdate.properties, layerindex);
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
            var endofrun = feature.properties.hasOwnProperty("MPEWatchData") ? feature.properties.MPEWatchData.hasOwnProperty("current_run_end") ? feature.properties.MPEWatchData.current_run_end != "0" ? feature.properties.MPEWatchData.current_run_end : "" : "" : "";
            var startofrun = feature.properties.hasOwnProperty("MPEWatchData") ? feature.properties.MPEWatchData.hasOwnProperty("current_run_start") ? feature.properties.MPEWatchData.current_run_start : "" : "";
            if (checkValue(sortplan) && !checkValue(endofrun)) {
                var thpCode = feature.properties.hasOwnProperty("MPEWatchData") ? feature.properties.MPEWatchData.hasOwnProperty("throughput_status") ? feature.properties.MPEWatchData.throughput_status : "0" : "0";
                var fillColor = GetMacineBackground(feature.properties.MPEWatchData, startofrun);
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
        layer.findId = feature.properties.id;
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        layer.on('click', function (e) {
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.sourceTarget.getCenter(), 3);
            if ((' ' + document.getElementById('sidebar').className + ' ').indexOf(' ' + 'collapsed' + ' ') <= -1) {
                if ($('#zoneselect').val() == feature.properties.id) {
                    sidebar.close('home');
                }
                else {
                    sidebar.open('home');
                }
            }
            else {
                sidebar.open('home');
            }
            LoadMachineTables(feature.properties, 'machinetable');
        });
        layer.bindTooltip(feature.properties.name + "<br/>" + "Staffing: " + (feature.properties.hasOwnProperty("CurrentStaff") ? feature.properties.CurrentStaff : "0"), {
            permanent: true,
            interactive: true,
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
async function updateMPEZone(properties, index) {
    var sortplan = properties.MPEWatchData.hasOwnProperty("cur_sortplan") ? properties.MPEWatchData.cur_sortplan : "";
    var endofrun = properties.MPEWatchData.hasOwnProperty("current_run_end") ? properties.MPEWatchData.current_run_end == "0" ? "" : properties.MPEWatchData.current_run_end : "";
    var startofrun = properties.MPEWatchData.hasOwnProperty("current_run_start") ? properties.MPEWatchData.current_run_start : "";
    if (checkValue(sortplan) && !checkValue(endofrun)) {
        var thpCode = properties.MPEWatchData.hasOwnProperty("throughput_status") ? properties.MPEWatchData.throughput_status : "0";
        var fillColor = GetMacineBackground(properties.MPEWatchData, startofrun);
        polygonMachine._layers[index].setStyle({
            weight: 1,
            opacity: 1,
            fillOpacity: 0.5,
            fillColor: fillColor
        });
    }
    else {
        if (polygonMachine._layers[index].options.fillColor !== '#989ea4') { //'gray'
            polygonMachine._layers[index].setStyle({
                weight: 1,
                opacity: 1,
                fillOpacity: 0.2,
                fillColor: '#989ea4'//'gray'
            });
        }
    }
}
async function LoadMachineTables(dataproperties, table) {
    try {
        if (!$.isEmptyObject(dataproperties)) {
            $('div[id=machine_div]').attr("data-id", dataproperties.id);
            $('div[id=dockdoor_div]').css('display', 'none');
            $('div[id=trailer_div]').css('display', 'none');
            $('div[id=area_div]').css('display', 'none');
            $('div[id=agvlocation_div]').css('display', 'none');
            $('div[id=vehicle_div]').css('display', 'none');
            $('div[id=layer_div]').css('display', 'none');
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
                        $("tr:visible").each(function (index) {
                            var curcolor = $(this).css("background-color");
                            if (curcolor == "" || curcolor == "rgba(0, 0, 0, 0)" || curcolor == "rgba(0, 0, 0, 0.05)") {
                                $(this).css("background-color", !!(index & 1) ? "rgba(0, 0, 0, 0)" : "rgba(0, 0, 0, 0.05)");
                            }
                        });
                    }
                }

                if (dataproperties.MPEWatchData.cur_operation_id == "918" || dataproperties.MPEWatchData.cur_operation_id == "919") {
                    LoadMachineTables(dataproperties, "dpstable");
                }
                var staffdata = dataproperties.hasOwnProperty("staffingData") ? dataproperties.staffingData : "";
                var MachineCurrentStaff = [];
                GetPeopleInZone(dataproperties.id, staffdata, MachineCurrentStaff);
                if (dataproperties.MPEWatchData.hasOwnProperty("current_run_end")) {
                    if (dataproperties.MPEWatchData.current_run_end == "" || dataproperties.MPEWatchData.current_run_end == "0") {
                        var runEndTR = document.getElementById('endtime_tr').style;
                        runEndTR.display = 'none';

                        $("tr:visible").each(function (index) {
                            var curcolor = $(this).css("background-color");
                            if (curcolor == "" || curcolor == "rgba(0, 0, 0, 0)" || curcolor == "rgba(0, 0, 0, 0.05)") {
                                $(this).css("background-color", !!(index & 1) ? "rgba(0, 0, 0, 0)" : "rgba(0, 0, 0, 0.05)");
                            }
                        });

                    }
                }
                if (dataproperties.MPEWatchData.hasOwnProperty("ars_recrej3")) {
                    if (dataproperties.MPEWatchData.ars_recrej3 != "0" && dataproperties.MPEWatchData.ars_recrej3 != "") {
                        var arsrec_tr = document.getElementById('arsrec_tr').style;
                        arsrec_tr.display = 'table-row';
                        $("tr:visible").each(function (index) {
                            var curcolor = $(this).css("background-color");
                            if (curcolor == "" || curcolor == "rgba(0, 0, 0, 0)" || curcolor == "rgba(0, 0, 0, 0.05)") {
                                $(this).css("background-color", !!(index & 1) ? "rgba(0, 0, 0, 0)" : "rgba(0, 0, 0, 0.05)");
                            }
                        });
                    }
                }
                if (dataproperties.MPEWatchData.hasOwnProperty("sweep_recrej3")) {
                    if (dataproperties.MPEWatchData.sweep_recrej3 != "0" && dataproperties.MPEWatchData.sweep_recrej3 != "") {
                        var sweeprec_tr = document.getElementById('sweeprec_tr').style;
                        sweeprec_tr.display = 'table-row';
                        $("tr:visible").each(function (index) {
                            var curcolor = $(this).css("background-color");
                            if (curcolor == "" || curcolor == "rgba(0, 0, 0, 0)" || curcolor == "rgba(0, 0, 0, 0.05)") {
                                $(this).css("background-color", !!(index & 1) ? "rgba(0, 0, 0, 0)" : "rgba(0, 0, 0, 0.05)");
                            }
                        });
                    }
                }
                document.getElementById('machineChart_tr').style.backgroundColor = 'rgba(0,0,0,0)';	
                if (dataproperties.MPEWatchData.hasOwnProperty("hourly_data")) {
                    GetMachinePerfGraph(dataproperties);
                }
                else {
                    var mpgtrStyle = document.getElementById('machineChart_tr').style;
                    mpgtrStyle.display = 'none';
                }
                var startofrun = dataproperties.hasOwnProperty("MPEWatchData") ? dataproperties.MPEWatchData.hasOwnProperty("current_run_start") ? dataproperties.MPEWatchData.current_run_start : "" : "";
                var expectedTP = dataproperties.hasOwnProperty("MPEWatchData") ? dataproperties.MPEWatchData.hasOwnProperty("expected_throughput") ? dataproperties.MPEWatchData.expected_throughput : "" : "";
                var throughput = dataproperties.hasOwnProperty("MPEWatchData") ? dataproperties.MPEWatchData.hasOwnProperty("cur_thruput_ophr") ? dataproperties.MPEWatchData.cur_thruput_ophr : "" : "";
                var thpCode = dataproperties.hasOwnProperty("MPEWatchData") ? dataproperties.MPEWatchData.hasOwnProperty("throughput_status") ? dataproperties.MPEWatchData.throughput_status : "0" : "0";
                FormatMachineRowColors(dataproperties.MPEWatchData, startofrun);
            }
            if (/dpstable/i.test(table)) {
                if (dataproperties.hasOwnProperty("DPSData")) {
                    if (checkValue(dataproperties.DPSData)) {
                        $zoneSelect[0].selectize.setValue(dataproperties.id, true);
                        $('div[id=dps_div]').css('display', 'block');
                        let dpstop_Table = $('table[id=' + table + ']');
                        let dpstop_Table_Body = dpstop_Table.find('tbody')
                        dpstop_Table_Body.empty();
                        dpstop_Table_Body.append(dpstop_row_template.supplant(formatdpstoprow(dataproperties.DPSData)));
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
        sortPlan: checkValue(properties.MPEWatchData.cur_sortplan) ? properties.MPEWatchData.cur_sortplan : "N/A" ,
        opNum:properties.MPEWatchData.cur_operation_id.padStart(3, "0"),
        sortPlanStart: properties.MPEWatchData.current_run_start,
        sortPlanEnd:  properties.MPEWatchData.current_run_end ,
        peicesFed:  digits(properties.MPEWatchData.tot_sortplan_vol) ,
        throughput: digits(properties.MPEWatchData.cur_thruput_ophr) ,
        rpgVol: digits(properties.MPEWatchData.rpg_est_vol),
        stateBadge: getstatebadge(properties),
        stateText: getstateText(properties),
        estComp: checkValue(properties.MPEWatchData.rpg_est_comp_time) ? properties.MPEWatchData.rpg_est_comp_time : "Estimate Not Available",
        rpgStart:moment(properties.MPEWatchData.rpg_start_dtm, "MM/DD/YYYY hh:mm:ss A").format("YYYY-MM-DD HH:mm:ss"),
        rpgEnd:  moment(properties.MPEWatchData.rpg_end_dtm, "MM/DD/YYYY hh:mm:ss A").format("YYYY-MM-DD HH:mm:ss"),
        expThroughput: digits(properties.MPEWatchData.expected_throughput),
        fullBins: properties.MPEWatchData.bin_full_bins,
        arsRecirc: properties.MPEWatchData.ars_recrej3,
        sweepRecirc: properties.MPEWatchData.sweep_recrej3,
    });
}
let machinetop_row_template =
    '<tr data-id="{zoneId}"><td>{zoneType}</td><td>{zoneName}</td><td><span class="badge badge-pill {stateBadge}" style="font-size: 12px;">{stateText}</span></td></tr>' +
    '<tr id="SortPlan_tr"><td>OPN / Sort Plan</td><td colspan="2">{opNum} / {sortPlan}</td></tr>' +
    '<tr id="StartTime_tr"><td>Start</td><td colspan="2">{sortPlanStart}</td></tr>' +
    '<tr id="endtime_tr"><td>End</td><td colspan="2">{sortPlanEnd}</td></tr>' +
    '<tr id="EstComp_tr"><td>Estimated Completion</td><td colspan="2">{estComp}</td>/tr>' +
    '<tr><td>Pieces Fed / RPG Vol.</td><td colspan="2">{peicesFed} / {rpgVol}</td></tr>' +
    '<tr id="Throughput_tr"><td>Throughput Act. / Exp.</td><td colspan="2">{throughput} / {expThroughput}</td></tr>' +
    '<tr id="fullbin_tr" style="display: none;"><td>Full Bins</td><td colspan="2" style="white-space: normal; word-wrap:break-word;">{fullBins}</td></tr>' +
    '<tr id="arsrec_tr" style="display: none;"><td>ARS Recirc. Rejects</td><td colspan="2">{arsRecirc}</td></tr>' +
    '<tr id="sweeprec_tr" style="display: none;"><td>Sweep Recirc. Rejects</td><td colspan="2">{sweepRecirc}</td></tr>' +
    '<tr id="machineChart_tr"><td colspan="3"><canvas id="machinechart"></canvas></td></tr>';

function formatdpstoprow(properties) {
    return $.extend(properties, {
        dpssortplans:  properties.sortplan_name_perf,
        piecesfedfirstpass:  digits(properties.pieces_fed_1st_cnt),
        piecesrejectedfirstpass:  digits(properties.pieces_rejected_1st_cnt),
        piecestosecondpass:  digits(properties.pieces_to_2nd_pass),
        piecesfedsecondpass:  digits(properties.pieces_fed_2nd_cnt),
        piecesrejectedsecondpass: digits(properties.pieces_rejected_2nd_cnt) ,
        piecesremainingsecondpass: digits(properties.pieces_remaining),
        timetocompleteactual: digits(properties.time_to_comp_actual),
        timeleftsecondpassactual:digits(properties.time_to_2nd_pass_actual),
        recomendedstartactual: properties.rec_2nd_pass_start_actual,
        completiondateTime:  properties.time_to_comp_actual_DateTime,
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
            //var endtime = moment(properties.MPEWatchData.current_run_end);
            var endtime = properties.MPEWatchData.current_run_end == "0" ? "" : moment(properties.MPEWatchData.current_run_end);
            
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
            //var endtime = moment(properties.MPEWatchData.current_run_end);
            var endtime = properties.MPEWatchData.current_run_end == "0" ? "" : moment(properties.MPEWatchData.current_run_end);
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
                            Zone_LDC: $('input[type=text][name=zone_ldc]').val(),
                            floorId: baselayerid
                           };

                            if (!$.isEmptyObject(jsonObject)) {
                                jsonObject.id = Data.id;
                                fotfmanager.server.editZone(JSON.stringify(jsonObject)).done(function (updatedData) {

                                    $('span[id=error_machinesubmitBtn]').text(updatedData[0].properties.MPE_Type + " Zone has been Updated.");
                                    setTimeout(function () { $("#Zone_Modal").modal('hide'); }, 1500);

                                });
                            }
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
function GetMacineBackground(mpeWatchData, starttime) {
    var OkColor = '#3573b1';
    var WarningColor = '#ffc107';
    var AlertColor = '#dc3545';
    var bkColor = OkColor;
    try {
        var throughput_status = mpeWatchData.hasOwnProperty("throughput_status") ? mpeWatchData.throughput_status : "0";
        var unplan_maint_sp_status = mpeWatchData.hasOwnProperty("unplan_maint_sp_status") ? mpeWatchData.unplan_maint_sp_status : "0";
        var op_started_late_status = mpeWatchData.hasOwnProperty("op_started_late_status") ? mpeWatchData.op_started_late_status : "0";
        var op_running_late_status = mpeWatchData.hasOwnProperty("op_running_late_status") ? mpeWatchData.op_running_late_status : "0";
        var sortplan_wrong_status = mpeWatchData.hasOwnProperty("sortplan_wrong_status") ? mpeWatchData.sortplan_wrong_status : "0";

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
            if (throughput_status == "3") {
                return AlertColor;
            }
            else if (throughput_status == "2") {
                bkColor = WarningColor;
            }
        }
        if (unplan_maint_sp_status == "2" || op_started_late_status == "2" || op_running_late_status == "2" || sortplan_wrong_status == "2") {
            return AlertColor;
        }
        if (unplan_maint_sp_status == "1" || op_started_late_status == "1" || op_running_late_status == "1" || sortplan_wrong_status == "1") {
            return WarningColor;
        }
        return bkColor;
    }
    catch (e) {
        console.log(e);
    }

}
function FormatMachineRowColors(mpeWatchData, starttime) {
    var Throughput_tr_style = document.getElementById('Throughput_tr').style;
    var SortPlan_tr_style = document.getElementById('SortPlan_tr').style;
    var StartTime_tr_style = document.getElementById('StartTime_tr').style;
    var EstComp_tr_style = document.getElementById('EstComp_tr').style;
    var rowAlertColor = "rgba(220, 53, 69, 0.5)";
    var rowWarningColor = "rgba(255, 193, 7, 0.5)";
    try {
        var throughput_status = mpeWatchData.hasOwnProperty("throughput_status") ? mpeWatchData.throughput_status : "0";
        var unplan_maint_sp_status = mpeWatchData.hasOwnProperty("unplan_maint_sp_status") ? mpeWatchData.unplan_maint_sp_status : "0";
        var op_started_late_status = mpeWatchData.hasOwnProperty("op_started_late_status") ? mpeWatchData.op_started_late_status : "0";
        var op_running_late_status = mpeWatchData.hasOwnProperty("op_running_late_status") ? mpeWatchData.op_running_late_status : "0";
        var sortplan_wrong_status = mpeWatchData.hasOwnProperty("sortplan_wrong_status") ? mpeWatchData.sortplan_wrong_status : "0";
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
            if (Throughput_tr_style != null) {
                if (throughput_status == "3") {
                    Throughput_tr_style.backgroundColor = rowAlertColor;
                }
                else if (throughput_status == "2") {
                    Throughput_tr_style.backgroundColor = rowWarningColor;
                }
                //else {
                //    Throughput_tr_style.backgroundColor = "";
                //}
            }
            //else {
            //    Throughput_tr_style.backgroundColor = "";
            //}
        }

        if (StartTime_tr_style != null) {
            if (op_started_late_status == "2") {
                StartTime_tr_style.backgroundColor = rowAlertColor;
            }
            else if (op_started_late_status == "1") {
                StartTime_tr_style.backgroundColor = rowWarningColor;
            }
            else {
                //StartTime_tr_style.backgroundColor = "";
            }
        }

        if (EstComp_tr_style != null) {
            if (op_running_late_status == "2") {
                EstComp_tr_style.backgroundColor = rowAlertColor;
            }
            else if (op_running_late_status == "1") {
                EstComp_tr_style.backgroundColor = rowWarningColor;
            }
            else {
                //EstComp_tr_style.backgroundColor = "";
            }
        }

        if (SortPlan_tr_style != null) {
            if (unplan_maint_sp_status == "2" || sortplan_wrong_status == "2") {
                SortPlan_tr_style.backgroundColor = rowAlertColor;
            }
            else if (unplan_maint_sp_status == "1" || sortplan_wrong_status == "1") {
                SortPlan_tr_style.backgroundColor = rowWarningColor;
            }
            else {
                //SortPlan_tr_style.backgroundColor = "";
            }
        }

    }
    catch (e) {
        console.log(e);
    }
}
function GetMachinePerfGraph(dataproperties) {
    var xValues = [];
    var yValues = [];
    var total = 0;
    for (var i = 0; i < dataproperties.MPEWatchData.hourly_data.length; i++) {
        xValues.unshift(dataproperties.MPEWatchData.hourly_data[i].count);
        total += dataproperties.MPEWatchData.hourly_data[i].count;
        yValues.unshift(dataproperties.MPEWatchData.hourly_data[i].hour);
    }
    if (total > 0) {
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
                                //var hour = ("0" + new Date(value).getHours().toString()).slice(-2);
                                //return hour + ":00-" + hour + ":59";
                                var hour = (new Date(value).getHours().toString()).slice(-2);
                                return hour + ":00";
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
    else {
        var mpgtrStyle = document.getElementById('machineChart_tr').style;
        mpgtrStyle.display = 'none';
    }
}

