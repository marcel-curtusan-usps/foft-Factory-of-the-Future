/**
 * this is use to setup a the machine information and other function
 * 
 * **/

const sparklineTooltipOpacity = .5;
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
    //GPIO
    //$('input[type=text][name=GPIO]').keyup(function () {
    //    if (!checkValue($('input[type=text][name=GPIO]').val())) {
    //        $('input[type=text][name=GPIO]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
    //        $('span[id=errorgpio]').text("Please Enter GPIO ");
    //    }
    //    else {
    //        $('input[type=text][name=GPIO]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
    //        $('span[id=errorgpio]').text("");
    //    }

    //    enablezoneSubmit();
    //});
    //if (!checkValue($('input[type=text][name=GPIO]').val())) {
    //    $('input[type=text][name=GPIO]').css("border-color", "#FF0000").removeClass('is-valid').addClass('is-invalid');
    //    $('span[id=errorgpio]').text("Please Enter Machine Number");
    //}
    //else {
    //    $('input[type=text][name=GPIO]').css("border-color", "#2eb82e").removeClass('is-invalid').addClass('is-valid');
    //    $('span[id=errorgpio]').text("");
    //}
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


var lastMachineStatuses = "";
$.extend(fotfmanager.client, {
    updateMachineStatus: async (machineData, Id) => { Promise.all([updateMachineZone(machineData, Id)]) },
    updateMPEAlertStatus: async (status,floorId, zoneId) => { Promise.all([updateMPEAlertData(status, floorId, zoneId)]) }
});

var sparklineMinZoom = 2;
async function updateMachineZone(data, id) {
    try {
        if (id == baselayerid) {
            $.map(polygonMachine._layers, function (layer, i) {
                if (layer.hasOwnProperty("feature") && layer.feature.properties.id === data.properties.id) {
                    layer.feature.properties = data.properties;
                    if (layer.feature.properties.name != data.properties.name) {
                        layer.setTooltipContent(data.properties.name + "<br/>" + "Staffing: " + data.properties.CurrentStaff);
                    }

                    if ($('div[id=machine_div]').is(':visible') && $('div[id=machine_div]').attr("data-id") === data.properties.id) {
                        LoadMachineTables(data.properties, 'machinetable');
                    }
                    updateMPEZone(data.properties, layer._leaflet_id);
                    return false;
                }
            });
        }
    } catch (e) {
        console.log(e);
    }
}

//async function updateMPEAlertData(data, id) {
//    try {
//        if (id == baselayerid) {
//            $.map(polygonMachine._layers, function (layer, i) {
//                if (layer.hasOwnProperty("feature") && layer.feature.properties.id === id) {
//                    layer.feature.properties.GpioValue = data;
//                    updateMPEAlert(layer._leaflet_id);
//                    return false;
//                }
//            });
//        }
//    } catch (e) {
//        console.log(e);
//    }
//}
async function updateMPEAlertData(data, floorId, zoneId) {
    try {
        if (baselayerid === floorId) {
            map.whenReady(() => {
                $.map(map._layers, function (layer, i) {
                    if (layer.hasOwnProperty("zoneId") && layer.zoneId === zoneId) {
                        layer.feature.properties.GpioValue = data;
                        if (data === 1 && layer.hasOwnProperty("_tooltip") && layer._tooltip.hasOwnProperty("_container")) {
                            if (!layer._tooltip._container.classList.contains('obstructedflash')) {
                                layer._tooltip._container.classList.add('obstructedflash');
                            }
                        }
                        else {
                            if (layer.hasOwnProperty("_tooltip") && layer._tooltip.hasOwnProperty("_container")) {
                                if (layer._tooltip._container.classList.contains('obstructedflash')) {
                                    layer._tooltip._container.classList.remove('obstructedflash');
                                }
                            }
                        }
                        return false;
                    }
                });

            });
        }
    }
    catch (e) {
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

function updateMPEZoneTooltipDirection() {

    let mpeWorkAreasChecked = $("#MPEWorkAreas").prop("checked");
    if (mpeWorkAreasChecked) {
        let keys = Object.keys(polygonMachine._layers);
        let tooltipDirection = getMPEZoneTooltipDirection();
        for (const key of keys) {
            let layer = polygonMachine._layers[key];
            if (layer && layer._tooltip !== null) {

                layer.getTooltip().options.direction = tooltipDirection;
                layer.closeTooltip();
                layer.openTooltip();
            }


        }
    }
}
function getMPEZoneTooltipDirection() {
    //let mpeSparklinesChecked = $("#MPESparklines").prop("checked");
    //if (Object.keys(machineSparklines._layers).length == 0 ||
    //    !mpeSparklinesChecked) {
    //    return "center";
    //}
    return "center";
}

function updateSparklineTooltipDirection() {

    let mpeSparklinesChecked = $("#MPESparklines").prop("checked");

    if (mpeSparklinesChecked) {
        let keys = Object.keys(machineSparklines._layers);

        var tooltipDirection = getSparklineTooltipDirection();
        for (const key of keys) {
            let layer = machineSparklines._layers[key];
            if (layer && layer._tooltip !== null) {
                layer.getTooltip().options.direction = tooltipDirection;
                layer.closeTooltip();
                layer.openTooltip();
            }


        }
    }
}
function getSparklineTooltipDirection() {

    let mpeWorkAreasChecked = $("#MPEWorkAreas").prop("checked");
    if (Object.keys(polygonMachine._layers).length == 0 ||
        !mpeWorkAreasChecked) {
        return "center";
    }
    return "right";
}

function getMachineStyle(data) {
    try {
        return {
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillOpacity: 0.5,
            fillColor: '#989ea4',
            lastOpacity: 0.5
        };

    } catch (e) {
        return {
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillOpacity: 0.2,
            fillColor: '#989ea4',
            lastOpacity: 0.2
        };
    }
    //var style = {};
    //var sortplan = feature.properties.hasOwnProperty("MPEWatchData") ? feature.properties.MPEWatchData.hasOwnProperty("cur_sortplan") ? feature.properties.MPEWatchData.cur_sortplan : "" : "";
    //var endofrun = feature.properties.hasOwnProperty("MPEWatchData") ? feature.properties.MPEWatchData.hasOwnProperty("current_run_end") ? feature.properties.MPEWatchData.current_run_end != "0" ? feature.properties.MPEWatchData.current_run_end : "" : "" : "";
    //var startofrun = feature.properties.hasOwnProperty("MPEWatchData") ? feature.properties.MPEWatchData.hasOwnProperty("current_run_start") ? feature.properties.MPEWatchData.current_run_start : "" : "";
    //if (checkValue(sortplan) && !checkValue(endofrun)) {

    //    var fillColor = GetMacineBackground(feature.properties.MPEWatchData, startofrun);
    //    style = {
    //        weight: 1,
    //        opacity: 1,
    //        color: '#3573b1',
    //        fillOpacity: 0.5,
    //        fillColor: fillColor,
    //        lastOpacity: 0.5
    //    };
    //}
    //else {
    //    style = {
    //        weight: 1,
    //        opacity: 1,
    //        color: '#3573b1',
    //        fillOpacity: 0.2,
    //        fillColor: '#989ea4',
    //        lastOpacity: 0.2
    //    };
    //}
    //return style;
}
let polygonMachine = new L.GeoJSON(null, {
    style: function (feature) {
        return{
            weight: 1,
            opacity: 1,
            color: '#3573b1',
            fillOpacity: 0.2,
            fillColor: GetMacineBackground(feature.properties.MPEWatchData),
            lastOpacity: 0.2
        };
    },
    onEachFeature: function (feature, layer) {

        layer.zoneId = feature.properties.id;
        let locationFlash = "";
        if (feature.properties.GpioValue === 1) {
            locationFlash = "obstructedflash";
        }
        if (feature.properties.sparkline) {
            updateMachineSparklineTooltip(feature, layer);
        }
        else {
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
                direction: getMPEZoneTooltipDirection(),
                opacity: 1,
                className: 'location ' + locationFlash
            }).openTooltip();
        }
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
    polygonMachine._layers[index].setStyle({
        weight: 1,
        opacity: 1,
        fillOpacity: 0.2,
        fillColor: GetMacineBackground(properties.MPEWatchData),//'gray'
        lastOpacity: 0.2
    });
}

async function updateMPEAlert(layerindex) {
    //add flashing to the MPE 
    if (polygonMachine._layers[layerindex].feature.properties.GpioValue === 1) {
        if (polygonMachine._layers[layerindex].hasOwnProperty("_tooltip")) {
            if (polygonMachine._layers[layerindex]._tooltip.hasOwnProperty("_container")) {
                if (!polygonMachine._layers[layerindex]._tooltip._container.classList.contains('obstructedflash')) {
                    polygonMachine._layers[layerindex]._tooltip._container.classList.add('obstructedflash');
                }
            }
        }
    }
    else {
        if (polygonMachine._layers[layerindex].hasOwnProperty("_tooltip")) {
            if (polygonMachine._layers[layerindex]._tooltip.hasOwnProperty("_container")) {
                if (polygonMachine._layers[layerindex]._tooltip._container.classList.contains('obstructedflash')) {
                    polygonMachine._layers[layerindex]._tooltip._container.classList.remove('obstructedflash');
                }
            }
        }
    }
    return true;
}

async function LoadMachineTables(dataproperties, table) {
    try {
        if (!$.isEmptyObject(dataproperties)) {
            $('span[name=mpeview]').empty();
            $('span[name=mpeSDO]').empty();
            $('div[id=machine_div]').attr("data-id", dataproperties.id);
            hideSidebarLayerDivs();
            $('div[id=machine_div]').css('display', 'block');
            $('div[id=ctstabs_div]').css('display', 'block');
            if (/machinetable/i.test(table)) {

                $zoneSelect[0].selectize.setValue(dataproperties.id, true);
                $('button[name=machineinfoedit]').attr('id', dataproperties.id);
                $("<a/>").attr({ target: "_blank", href: window.location.origin + '/MPE/MPE.aspx?MPEStatus=' + dataproperties.name, style: 'color:white;' }).html("View").appendTo($('span[name=mpeview]'));
                if (dataproperties.MPE_Group !== "") {
                    $("<a/>").attr({ target: "_blank", href: window.location.origin + '/MPESDO/MPESDO.aspx?MPEGroupName=' + dataproperties.MPE_Group, style: 'color:white;' }).html("SDO View").appendTo($('span[name=mpeSDO]'));
                }
                $('button[name=machineinfoedit]').attr('id', dataproperties.id);
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
                let staffdata = dataproperties.hasOwnProperty("staffingData") ? dataproperties.staffingData : "";
                let MachineCurrentStaff = [];
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
        sortPlan: Vaildatesortplan(properties.MPEWatchData),// ? properties.MPEWatchData.cur_sortplan : "N/A",
        opNum: properties.MPEWatchData.cur_operation_id,
        sortPlanStart: VaildateMPEtime(properties.MPEWatchData.current_run_start),
        sortPlanEnd: VaildateMPEtime(properties.MPEWatchData.current_run_end),
        peicesFed: properties.MPEWatchData.tot_sortplan_vol,
        throughput: properties.MPEWatchData.cur_thruput_ophr,
        rpgVol: properties.MPEWatchData.rpg_est_vol,
        stateBadge: getstatebadge(properties),
        stateText: getstateText(properties),
        estComp: VaildateEstComplete(properties.MPEWatchData.rpg_est_comp_time),// checkValue(properties.MPEWatchData.rpg_est_comp_time) ? properties.MPEWatchData.rpg_est_comp_time : "Estimate Not Available",
        rpgStart: moment(properties.MPEWatchData.rpg_start_dtm, "MM/DD/YYYY hh:mm:ss A").format("YYYY-MM-DD HH:mm:ss"),
        rpgEnd: moment(properties.MPEWatchData.rpg_end_dtm, "MM/DD/YYYY hh:mm:ss A").format("YYYY-MM-DD HH:mm:ss"),
        expThroughput: properties.MPEWatchData.expected_throughput,
        fullBins: properties.MPEWatchData.bin_full_bins,
        arsRecirc: properties.MPEWatchData.ars_recrej3,
        sweepRecirc: properties.MPEWatchData.sweep_recrej3
    });
}
function Vaildatesortplan(data) {
    try {
        if (!!data && data.cur_sortplan.length > 3) {
            return data.cur_sortplan;
        }
        else {
            return "N/A"
        }
    }
    catch (e) {

    }
}
function VaildateMPEtime(data) {
    try {
        let time = moment(data);
        if (time._isValid && time.year() === moment().year()) {
            return time.format("MM/DD/YYYY hh:mm:ss A");
        }
        else {
            return " ";
        }
    } catch (e) {

    }
}
function VaildateEstComplete(estComplet)
{
    try {
        let est = moment(estComplet);
        if (est._isValid && est.year() === moment().year()) {
            return est.format("MM/DD/YYYY hh:mm:ss A");
        }
        else {
            return "Estimate Not Available";
        }
    } catch (e) {

    }
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
    '<tr id="machineChart_tr"><td colspan="3"><canvas id="machinechart" width="470" height="250"></canvas></td></tr>';

function formatdpstoprow(properties) {
    return $.extend(properties, {
        dpssortplans: properties.sortplan_name_perf,
        piecesfedfirstpass: properties.pieces_fed_1st_cnt,
        piecesrejectedfirstpass: properties.pieces_rejected_1st_cnt,
        piecestosecondpass: properties.pieces_to_2nd_pass,
        piecesfedsecondpass: properties.pieces_fed_2nd_cnt,
        piecesrejectedsecondpass: properties.pieces_rejected_2nd_cnt,
        piecesremainingsecondpass: properties.pieces_remaining,
        timetocompleteactual: properties.time_to_comp_actual,
        timeleftsecondpassactual: properties.time_to_2nd_pass_actual,
        recomendedstartactual: properties.rec_2nd_pass_start_actual,
        completiondateTime: properties.time_to_comp_actual_DateTime
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
                    fotfmanager.server.getMPEList().done(function (mpedata) {
                        if (mpedata.length > 0) {
                            mpedata.sort(SortByName);
                            mpedata.push('**Machine Not Listed');
                            $('#machine_manual_row').css('display', 'none');
                            $('select[id=machine_zone_select_name]').css('display', '');
                            $('select[id=machine_zone_select_name]').empty();
                            $('<option/>').val("").html("").appendTo('select[id=machine_zone_select_name]');
                            $('select[id=machine_zone_select_name]').val("");
                            $.each(mpedata, function () {
                                $('<option/>').val(this).html(this).appendTo('#machine_zone_select_name');
                            })
                            $('select[id=machine_zone_select_name]').val(Data.name.toString());
                        }
                        else {
                            $('<option/>').val("").html("").appendTo('select[id=machine_zone_select_name]');
                            $('<option/>').val("**Machine Not Listed").html("**Machine Not Listed").appendTo('select[id=machine_zone_select_name]');
                            $('select[id=machine_zone_select_name]').val("**Machine Not Listed");
                            $('#machine_manual_row').css('display', '');
                            $('select[id=machine_zone_select_name]').css('display', 'none');
                        }
                    });
                    $('select[id=machine_zone_select_name]').change(function () {
                        if ($('select[name=machine_zone_select_name] option:selected').val() == '**Machine Not Listed') {
                            $('#machine_manual_row').css('display', '');
                        }
                        else {
                            $('#machine_manual_row').css('display', 'none');
                        }
                        if ($('select[name=machine_zone_select_name] option:selected').val() == '') {
                            $('button[id=machinesubmitBtn]').prop('disabled', true);
                        }
                        else {
                            $('button[id=machinesubmitBtn]').prop('disabled', false);
                        }
                    });
                    /*Populate Machine Group Name*/
                    fotfmanager.server.getMPEGroupList().done(function (mpeGroupData) {
                        $('select[id=mpe_group_select]').empty();
                        if (mpeGroupData.length > 0) {
                            mpeGroupData.sort(SortByName);
                            mpeGroupData.push('**Group Not Listed');
                            $('#mpegroupname_div').css('display', 'none');
                            $('select[id=mpe_group_select]').css('display', '');
                            $.each(mpeGroupData, function () {
                                $('<option/>').val(this).html(this).appendTo('#mpe_group_select');
                            })
                            $('select[id=mpe_group_select]').val(Data.MPE_Group.toString());
                        }
                        else {
                            $('<option/>').val("**Group Not Listed").html("**Group Not Listed").appendTo('select[id=mpe_group_select]');
                            $('<option/>').val("").html("").appendTo('select[id=mpe_group_select]');
                            $('select[id=mpe_group_select]').val("");
                            $('#mpegroupname_div').css('display', 'none');
                            /*enableNewGroupName();*/
                        }
                    });
                    /*Onchange Validate Machine Group Name*/
                    $('select[id=mpe_group_select]').change(function () {
                        if ($('select[name=mpe_group_select] option:selected').val() == '**Group Not Listed') {
                            $('#mpegroupname_div').css('display', '');
                            enableNewGroupName();
                        }
                        else {
                            $('#mpegroupname_div').css('display', 'none');
                            $('input[id=mpegroupname]').val("");
                            $('button[id=machinesubmitBtn]').prop('disabled', false);
                        }
                    });

                    /*Validate new group name textbox not empty*/
                    $('input[type=text][name=mpegroupname]').keyup(function () {
                        enableNewGroupName();
                    });
                   
                    $('input[type=text][name=machine_name]').val(Data.MPE_Type);
                    $('input[type=text][name=machine_number]').val(Data.MPE_Number);
                    $('input[type=text][name=zone_ldc]').val(Data.Zone_LDC);
                    $('input[type=text][name=machine_id]').val(Data.id);

                    $('button[id=machinesubmitBtn]').off().on('click', function () {
                        try {
                            var machineName = "";
                            var machineNumber = "";
                            var mpeGroupName = "";
                            $('button[id=machinesubmitBtn]').prop('disabled', true);
                            if ($('select[name=machine_zone_select_name] option:selected').val() != '**Machine Not Listed') {
                                var selectedMachine = $('select[name=machine_zone_select_name] option:selected').val().split("-");
                                machineName = selectedMachine[0];
                                machineNumber = selectedMachine[1];
                            }
                            else {
                                machineName = $('input[type=text][name=machine_name]').val();
                                machineNumber = $('input[type=text][name=machine_number]').val();
                            }
                            /*Assign values for Group Name*/
                            if ($('select[name=mpe_group_select] option:selected').val() != '**Group Not Listed') {
                                mpeGroupName = $('select[name=mpe_group_select] option:selected').val();
                            }
                            else {
                                mpeGroupName = $('input[type=text][name=mpegroupname]').val();
                            }

                            var jsonObject = {
                                MPE_Type: machineName,//$('input[type=text][name=machine_name]').val(),
                                MPE_Number: machineNumber,//$('input[type=text][name=machine_number]').val(),
                                Zone_LDC: $('input[type=text][name=zone_ldc]').val(),
                                MPE_Group: mpeGroupName,
                                floorId: baselayerid
                            };

                            if (!$.isEmptyObject(jsonObject)) {
                                jsonObject.id = Data.id;
                                fotfmanager.server.editZone(JSON.stringify(jsonObject)).done(function (updatedData) {

                                    $('span[id=error_machinesubmitBtn]').text(" Zone has been Updated.");
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
function enableNewGroupName() {
    if (!checkValue($('input[type=text][name=mpegroupname]').val())) {
        $('input[type=text][name=mpegroupname]').removeClass('is-valid border border-success').addClass('is-invalid border border-danger');
        $('span[id=error_mpegroupname]').text("Please Enter Group Name");
        $('button[id=machinesubmitBtn]').prop('disabled', true);
    }
    else {
        $('input[type=text][name=mpegroupname]').removeClass('is-invalid border border-danger').addClass('is-valid border border-success');
        $('span[id=error_mpegroupname]').text("");
        $('button[id=machinesubmitBtn]').prop('disabled', false);
    }
}
function GetMacineBackground(mpeWatchData) {
    let NotRunningbkColor = '#989ea4';
    let RunningColor = '#3573b1';
    let WarningColor = '#ffc107';
    let AlertColor = '#dc3545';
    let bkColor = RunningColor;
    try {
        if (mpeWatchData.cur_sortplan === "0") {
            return NotRunningbkColor;
        }
        else {
            let curtime = moment();
            if (!$.isEmptyObject(timezone)) {
                if (timezone.hasOwnProperty("Facility_TimeZone")) {
                    curtime = moment().tz(timezone.Facility_TimeZone);
                }
            }
            let st = moment(mpeWatchData.current_run_start);
            let timeduration = moment.duration(curtime.diff(st));
            let minutes = parseInt(timeduration.asMinutes());
            if (minutes > 15) {
                if (mpeWatchData.throughput_status == 3) {
                    return AlertColor;
                }
                else if (mpeWatchData.throughput_status == 2) {
                    bkColor = WarningColor;
                }
            }
            if (mpeWatchData.unplan_maint_sp_status === 2 || mpeWatchData.op_started_late_status === 2 || mpeWatchData.op_running_late_status === 2 || mpeWatchData.sortplan_wrong_status === 2) {
                return AlertColor;
            }
            if (mpeWatchData.unplan_maint_sp_status === 1 || mpeWatchData.op_started_late_status === 1 || mpeWatchData.op_running_late_status === 1 || mpeWatchData.sortplan_wrong_status === 1) {
                return WarningColor;
            }
            return NotRunningbkColor;
        }
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
    let xValues = [];
    let yValues = [];
    let total = 0;
    for (let i = 0; i < dataproperties.MPEWatchData.hourly_data.length; i++) {
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
                responsive: false,
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
                        }
                    }
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

