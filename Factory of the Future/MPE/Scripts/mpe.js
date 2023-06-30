
/// A simple template method for replacing placeholders enclosed in curly braces.
if (!String.prototype.supplant) {
    String.prototype.supplant = function (o) {
        return this.replace(/{([^{}]*)}/g,
            function (a, b) {
                let r = o[b];
                return typeof r === 'string' || typeof r === 'number' ? r : a;
            }
        );
    };
}
let connecttimer = 0;
let connectattp = 0;
let MPEName = "";
let fotfmanager = $.connection.FOTFManager;
let DateTimeNow = new Date();
let CurrentTripMin = 0;
let CountTimer = 0;
let TimerID = -1;
let Timerinterval = 1;
$.extend(fotfmanager.client, {
    updateMPEStatus: (updatedata) => { Promise.all([updateMPEStatus(updatedata)]) }
});
async function updateMPEStatus(data)
{
    try {
        if (!!data) {
            $('label[id=mpeName]').text(data.MpeId);
            $('label[id=opn]').text(data.cur_operation_id);
            $('label[id=sortplan_name_text]').text(data.cur_sortplan);
            $('label[id=mpe_status]').text(MPEStatus(data));
            Promise.all([buildDataTable(data)]);
        }
    } catch (e) {
        console.log(e);
    }
}
async function StartDataConnection() {
    // Start the connection
    createMPEDataTable("mpeStatustable");

}
async function LoadData()
{
    console.log("Connected time: " + new Date($.now()));
    fotfmanager.server.getMPEStatusList(MPEName).done(async (data) => { Promise.all([updateMPEStatus(data[0])]) });
    fotfmanager.server.joinGroup("MPE_" + MPEName);
}
function createMPEDataTable(table) {
    let arrayColums = [{
        "order":"",
        "Name":"",
        "Planned": "",
        "Actual": ""
    }]
    let columns = [];
    let tempc = {};
    $.each(arrayColums[0], function (key) {
        tempc = {};
        if (/Planned/i.test(key)) {
            tempc = {
                "title": 'Planned',
                "mDataProp": key,
                "class": "col-planned text-center"
            }
        }
        else if (/Actual/i.test(key)) {
            tempc = {
                "title": "Actual",
                "mDataProp": key,
                "class": "col-actual text-center"
            }
        }
        else if (/Name/i.test(key)) {
            tempc = {
                "title": "",
                "mDataProp": key,
                "class": "col-name text-right"
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
        fnInitComplete: function () {
            if ($(this).find('tbody tr').length <= 1) {
                $('.odd').hide()
            }
        },
        dom: 'Bfrtip',
        bFilter: false,
        bdeferRender: true,
        paging: false,
        bPaginate: false,
        bAutoWidth: true,
        bInfo: false,
        destroy: true,
        aoColumns: columns,
        sorting: [[0, "asc"]],
        columnDefs: [{
            visible: false,
            targets: 0,
        }],
        //rowCallback: function (row, data, index) {
        rowCallback: function (row) {
            $(row).find('td').css('font-size', 'calc(0.1em + 2.6vw)');
        }
    });
}
function updateMpeDataTable(ldata, table) {
    let load = true;
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows(function (idx, data, node) {
            load = false;
            if (data.Name === ldata.Name) {
                $('#' + table).DataTable().row(node).data(ldata).draw().invalidate();
            }
        })
        if (load) {
            loadMpeDataTable(ldata, table);
        }
    }
}
function loadMpeDataTable(data, table) {
    if ($.fn.dataTable.isDataTable("#" + table) && !$.isEmptyObject(data)) {
        /*if (!$.isEmptyObject(data)) {*/
            $('#' + table).DataTable().rows.add(data).draw();
        //}
    }
}
function removeLegsTripDataTable(table) {
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows(function (idx, data, node) {
            $('#' + table).DataTable().row(node).remove().draw();
        })
    }
}
async function buildDataTable(data)
{
    let dataArray = [];
   
    $.each(data, function (key) {
        let tabledataObject = {};
        if (/cur_thruput_ophr/i.test(key)) {
            tabledataObject = {
                "order": 3,
                "Name": "Throughput",
                "Planned": data.expected_throughput,
                "Actual": data.cur_thruput_ophr,
            }
          dataArray.push(tabledataObject);
        }
        if (/tot_sortplan_vol/i.test(key)) {
            tabledataObject = {
                "order": 2,
                "Name": "Volume",
                "Planned": data.rpg_est_vol,
                "Actual": data.tot_sortplan_vol,
            }
            dataArray.push(tabledataObject);
        }
        if (/ars_recrej3/i.test(key)) {
            tabledataObject = {
                "order": 4,
                "Name": "Reject Rate",
                "Planned": 0,
                "Actual": data.ars_recrej3,
            }
            dataArray.push(tabledataObject);
        }
        if (/rpg_end_dtm/i.test(key)) {
            tabledataObject = {
                "order": 5,
                "Name": "End Time",
                "Planned": VaildateEstComplete(data.rpg_end_dtm),
                "Actual": VaildateEstComplete(data.rpg_est_comp_time),
            }
            if (CurrentTripMin === 0) {
                CountTimer = startTimer(data.rpg_est_comp_time);
                dataArray.push(tabledataObject);
            }
        }
        if (/cur_sortplan/i.test(key)) {
            tabledataObject = {
                "order": 1,
                "Name": "Sort Program",
                "Planned": data.cur_sortplan,
                "Actual": data.cur_sortplan,
            }
            dataArray.push(tabledataObject);
        }
        //if (true) {
        //    tabledataObject = {
        //        "Volume": '',
        //        "Throughput": "",
        //        "Reject Rate": "",
        //        "End Time": "",
        //    }
        //}
    });
    updateMpeDataTable(dataArray, "mpeStatustable");
}
function MPEStatus(data) {
    if (/^(0)/i.test(data.cur_sortplan)) {
        return "Idle";
    }
    else {
        return "Running";
    }
}
$(function () {
    setHeight();
    document.title = $.urlParam("mpeStatus");
    if (MPEName !== "") {
        Promise.all([StartDataConnection()]);
        $('label[id=mpeName]').text(MPEName);
    }

    //start connection 
    $.connection.hub.qs = { 'page_type': "MPE".toUpperCase() };
    $.connection.hub.start({ waitForPageLoad: false })
        .done(() => { Promise.all([LoadData()]) }).catch(
            function (err) {
               /* console.log(err.toString());*/
                throw new Error(err.toString());
            });
    // Raised when the connection state changes. Provides the old state and the new state (Connecting, Connected, Reconnecting, or Disconnected).
    //$.connection.hub.stateChanged(function (state) {
        //switch (state.newState) {
        //    case 1: $('label[id=dockdoorNumber]');
        //        break;
        //    case 4:
        //        $('label[id=dockdoorNumber]');
        //        break;
        //    default: $('label[id=dockdoorNumber]');
        //}
    //});
    //handling Disconnect
    $.connection.hub.disconnected(function () {
        connecttimer = setTimeout(function () {
            if (connectattp > 10) {
                clearTimeout(connecttimer);
            }
            connectattp += 1;
            $.connection.hub.start({ waitForPageLoad: false })
                .done(() => { Promise.all([LoadData()]) })
                .catch(function (err) {
                throw new Error(err.toString());
                //console.log(err.toString());
            });
        }, 10000); // Restart connection after 10 seconds.
        // fotfmanager.server.leaveGroup("DockDoor_" + doornumber);
    });
    //Raised when the underlying transport has reconnected.
    $.connection.hub.reconnecting(function () {
        clearTimeout(connecttimer);
        console.log("reconnected at time: " + new Date($.now()));
        fotfmanager.server.joinGroup("MPE_" + MPEName);
    });
});
$.urlParam = function (name) {
    let results = new RegExp('[\?&]' + name + '=([^&#]*)', 'i').exec(window.location.search);
    MPEName = (results !== null) ? results[1] || 0 : "";
    return MPEName;
}
function setHeight() {
    let height = (this.window.innerHeight > 0 ? this.window.innerHeight : this.screen.height) - 1;
    let screenTop = (this.window.screenTop > 0 ? this.window.screenTop : 1) - 1;
    let pageBottom = (height - screenTop);
    $("div.card").css("min-height", pageBottom + "px");
}
function capitalize_Words(str) {
    return str.replace(/\w\S*/g, function (txt) {
        return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
    });
}
function VaildateEstComplete(estComplet) {
    try {
        let est = moment(estComplet);
        if (est._isValid && est.year() === moment().year()) {
            return est.format("hh:mm:ss");
        }
        else {
            return "Not Available";
        }
    } catch (err) {
        throw new Error(err.toString());
    }
}
function startTimer(SVdtm) {
    if (!!SVdtm) {
        let duration = calculatescheduledDuration(SVdtm);
        let timer = setInterval(function () {
            if (!!duration && duration._isValid) {
                CurrentTripMin = duration.asMinutes();

                    duration = moment.duration(duration.asSeconds() - Timerinterval, 'seconds');
                    $('label[id=countdown]').html(duration.format("d [days] hh:mm:ss ", { trunc: true }));
                
            }
            else {
                stopTimer()
            }

        }, 1000);

        TimerID = timer;

        return timer;
    }
    else {
        stopTimer()
    }
};
function stopTimer() {
    clearInterval(CountTimer);
}
function calculatescheduledDuration(t) {
    if (!!t) {
        let timenow = moment(DateTimeNow);
        let conditiontime = moment(t);
        if (conditiontime._isValid && conditiontime.year() === timenow.year()) {
            if (timenow > conditiontime) {
                return moment.duration(timenow.diff(conditiontime));
            }
            else {
                return moment.duration(conditiontime.diff(timenow));
            }
        }
        else {
            return "";
        }
    }
}
//sample data 
let sample_data = {
    "mpe_type": "DBCS",
    "mpe_number": "68",
    "bins": "0",
    "cur_sortplan": "LV96734U",
    "cur_thruput_ophr": "4272",
    "tot_sortplan_vol": "338",
    "rpg_est_vol": "0",
    "act_vol_plan_vol_nbr": "0",
    "current_run_start": "2023-03-03 13:48:20",
    "current_run_end": "0",
    "cur_operation_id": "249",
    "bin_full_status": "0",
    "bin_full_bins": "",
    "throughput_status": "3",
    "unplan_maint_sp_status": "0",
    "op_started_late_status": "0",
    "op_running_late_status": "0",
    "sortplan_wrong_status": "0",
    "unplan_maint_sp_timer": "0",
    "op_started_late_timer": "0",
    "op_running_late_timer": "0",
    "sortplan_wrong_timer": "0",
    "hourly_data": [
        {
            "hour": "2023-03-03 13:00",
            "count": 4748
        },
        {
            "hour": "2023-03-03 12:00",
            "count": 8663
        },
        {
            "hour": "2023-03-03 11:00",
            "count": 0
        },
        {
            "hour": "2023-03-03 10:00",
            "count": 0
        },
        {
            "hour": "2023-03-03 09:00",
            "count": 0
        },
        {
            "hour": "2023-03-03 08:00",
            "count": 0
        },
        {
            "hour": "2023-03-03 07:00",
            "count": 0
        },
        {
            "hour": "2023-03-03 06:00",
            "count": 0
        },
        {
            "hour": "2023-03-03 05:00",
            "count": 0
        },
        {
            "hour": "2023-03-03 04:00",
            "count": 5516
        },
        {
            "hour": "2023-03-03 03:00",
            "count": 9895
        },
        {
            "hour": "2023-03-03 02:00",
            "count": 11514
        },
        {
            "hour": "2023-03-03 01:00",
            "count": 8401
        },
        {
            "hour": "2023-03-03 00:00",
            "count": 10234
        },
        {
            "hour": "2023-03-02 23:00",
            "count": 7439
        },
        {
            "hour": "2023-03-02 22:00",
            "count": 6544
        },
        {
            "hour": "2023-03-02 21:00",
            "count": 9227
        },
        {
            "hour": "2023-03-02 20:00",
            "count": 5937
        },
        {
            "hour": "2023-03-02 19:00",
            "count": 1366
        },
        {
            "hour": "2023-03-02 18:00",
            "count": 9387
        },
        {
            "hour": "2023-03-02 17:00",
            "count": 1035
        },
        {
            "hour": "2023-03-02 16:00",
            "count": 0
        },
        {
            "hour": "2023-03-02 15:00",
            "count": 3539
        },
        {
            "hour": "2023-03-02 14:00",
            "count": 6496
        }
    ],
    "rpg_expected_thruput": "25,000",
    "ars_recrej3": "1",
    "sweep_recrej3": "0"
}