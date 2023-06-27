let mpeGroupData = [];
let MPEGroupName = "";
class MPESummaryTemplet {
    constructor() {
        this.machineType = "";
        this.scheduledStaff = 0;
        this.actualStaff = 0;
        this.totalVolume = 0;
        this.plannedVolume = 0;
        this.totalThroughput = 0;
        this.plannedThroughput = 0;
        this.plannedEndTime = 0;
        this.currentRunStart = "";
        this.projectedEndTime = "";
        this.mpesRunning = 0;
        this.message = "";
    }
}
let mpeSummaryTemplate = new MPESummaryTemplet();
let mpeDetailsList = [];
let previousPlannedEndTime = moment('0001-01-01T00:00:00').format('M/DD/yyyy hh:mm:ss');
let previousRunStart = moment('0001-01-01T00:00:00').format('M/DD/yyyy hh:mm:ss');
let mpesRunning = 0;
let fotfmanager = $.connection.FOTFManager;

$.extend(fotfmanager.client, {
    UpdateMPESDOStatus: (mpeDataIncoming) => { Promise.all([processMPEIncomingData(mpeDataIncoming)]); },
    RemoveMPEFromGroup: (mpeToRemove) => { Promise.all([removeMPEFromGroup(mpeToRemove)]); },
    AddMPEToGroup: (mpeToAdd) => { Promise.all([addMPEToGroup(mpeToAdd)]); }
});

async function processMPEIncomingData(data) {
    try {
        if (!!data) {
            let index = mpeExistInGroup(mpeGroupData, data.MpeId);
            if (index > -1) {
                mpeGroupData[index] = data;
                initMPEGroupStatus(mpeGroupData);
            }
        }
    } catch (e) {

    }
}

async function removeMPEFromGroup(data) {
    try {
        if (!!data) {
            let index = mpeExistInGroup(mpeGroupData, data.MpeId);
            if (index > -1) {
                mpeGroupData.splice(index, 1); 
                initMPEGroupStatus(mpeGroupData);
            }
        }
    } catch (e) {

    }
}

async function addMPEToGroup(data) {
    try {
        if (!!data) {
            let index = mpeExistInGroup(mpeGroupData, data.MpeId);
            if (index === -1) {
                //insert in the last position
                mpeGroupData[mpeGroupData.length] = data; 
                initMPEGroupStatus(mpeGroupData);
            }
        }
    } catch (e) {

    }
}

function mpeExistInGroup(mpeGroupData, mpeId) {
    let index = -1;
    $.each(mpeGroupData, function (key) {
        if (this.MpeId === mpeId) {
            index = key;
            return;
        }
    });
    return index;
}

async function initMPEGroupStatus(data) {
    try {
        if (!!data) {
            if (!mpeGroupData || !mpeGroupData.length) {
                // array of MPEs empty? then assign the data coming from Server, so we only assign once during lifetime of page
                mpeGroupData = data.slice();
            }
            //Empty the array before adding the status of MPEs
            mpeDetailsList.splice(0, mpeDetailsList.length);
            //empty summary class
            mpeSummaryTemplate = new MPESummaryTemplet(); 

            /*$.each(data, function (key, value) {*/
            $.each(data, function () {
                /*Add up only data from running MPEs*/
                if (this.cur_operation_id !== 0) {
                    mpesRunning += 1;
                    mpeSummaryTemplate.machineType = this.mpe_type;
                    mpeSummaryTemplate.scheduledStaff += GetscheduledStaffing(this.scheduled_staff);
                    //mpeSummary.actualStaff += this.ActualStaff;
                    mpeSummaryTemplate.totalVolume += this.tot_sortplan_vol;
                    mpeSummaryTemplate.plannedVolume += this.rpg_est_vol;
                    mpeSummaryTemplate.totalThroughput += this.cur_thruput_ophr;
                    mpeSummaryTemplate.plannedThroughput += this.rpg_expected_thruput;
                    /*Capture the latest Planned End Time and save the current run start to determine whether we are within the planned estimated time of completion */
                    mpeSummaryTemplate.plannedEndTime = GetLatestPlannedEndTime(this, previousPlannedEndTime);
                    mpeSummaryTemplate.currentRunStart = GetLatestRunStart(this, previousPlannedEndTime, previousRunStart);
                    mpeSummaryTemplate.message = "";
                    previousPlannedEndTime = moment(this.rpg_end_dtm).format('M/DD/yyyy hh:mm:ss');
                    previousRunStart = moment(this.current_run_start).format('M/DD/yyyy hh:mm:ss');
                }
                /* 2. Create the list of MPE elements */
                mpeDetailsList.push(this);
            });
            mpeSummaryTemplate.mpesRunning = mpesRunning;
            mpeSummaryTemplate.projectedEndTime = getProjectedEndtime(mpeSummaryTemplate.totalVolume, mpeSummaryTemplate.totalThroughput, mpeSummaryTemplate.currentRunStart);
            populateFields(mpeSummaryTemplate, getSortedData(mpeDetailsList, 'mpe_number', 1));
        }
    } catch (e) {
    }
}

function GetLatestPlannedEndTime(mpeData, latestPlannedEndTime) {
    if (moment(mpeData.rpg_end_dtm) > latestPlannedEndTime) {
        return mpeData.rpg_end_dtm;
    }
    else {
        return latestPlannedEndTime;
    }
}

function GetLatestRunStart(mpeData, latestPlannedEndTime, latestRunStart) {
    if (moment(mpeData.rpg_end_dtm) > latestPlannedEndTime) {
        return moment(mpeData.current_run_start).format('M/DD/yyyy hh:mm:ss');
    }
    else {
       return latestRunStart;
    }
}

function GetscheduledStaffing(data) {
    let StaffTotal = 0;
    try {
        if (!!data) {
            StaffTotal += data.clerk;
            StaffTotal += data.mh;
        }
        return StaffTotal;
    } catch (e) {
        return StaffTotal;
    }
}

function getSortedData(data, prop, isAsc) {
    return data.sort((a, b) => {
        return (a[prop] < b[prop] ? -1 : 1) * (isAsc ? 1 : -1);
    });
}

function populateFields(machineSummary, mpesGroup) {
    $('h1[id=machineType]').text("Group Name: " + machineSummary.machineType);
    $('h1[id=scheduledStaff]').text("Scheduled Staff: " + machineSummary.scheduledStaff);
    $('h1[id=actualStaff]').text("Actual Staff: " + machineSummary.actualStaff);
    $('p[id=totalVolume]').text(addCommas(machineSummary.totalVolume));
    $('h1[id=plannedVolume]').text("Plan: " + addCommas(machineSummary.plannedVolume));
    /*$('p[id=avgThroughput]').text(addCommas(getThroughput(machineSummary.totalThroughput, machineSummary.mpesRunning*/
    $('p[id=avgThroughput]').text(getThroughput(machineSummary.totalThroughput, machineSummary.mpesRunning));
    $('h1[id=plannedThroughput]').text("Plan: " + addCommas(machineSummary.plannedThroughput));
    if (moment(machineSummary.plannedEndTime) > machineSummary.projectedEndTime) {
        $('p[id=projectedEndTime]').text(machineSummary.projectedEndTime).addClass('text-danger');
    } else {
        $('p[id=projectedEndTime]').text(machineSummary.projectedEndTime);
    }
    $('p[id=plannedEndTime]').text(moment(machineSummary.plannedEndTime).format('hh:mm'));
    $('p[id=mpeAlertMsg]').text(machineSummary.message);
    $('p[id=totalMPEs]').text(mpesGroup.length);
    /*Clear the groupList so we do not stack up the items*/
    removeChildElements("mpeGroupList");

    $.each(mpesGroup, function () {
    //$.each(mpesGroup, function (key, value) {
        
        let singleMPEDiv = document.createElement('div');
        singleMPEDiv.className = 'item2';
        /*Create inner elements*/
        let innerH5 = document.createElement('h5');
        innerH5.className = 'card-title';
        innerH5.textContent = this.MpeId;

        let innerH6 = document.createElement('h6');      
        innerH6.textContent = getMPEOperationStatus(this);
        let mpeStatus = getStatusDescription(innerH6.textContent);
        innerH6.className = 'card-text btn-' + mpeStatus;

        let innerP = document.createElement('p');
        innerP.className = 'card-text';
        innerP.textContent = mpeStatus === 'secondary' ? "N/A" : 'OP# ' + this.cur_operation_id;

        singleMPEDiv.appendChild(innerH5);
        singleMPEDiv.appendChild(innerH6);
        singleMPEDiv.appendChild(innerP);

        let groupList = document.getElementById("mpeGroupList");
        groupList.appendChild(singleMPEDiv);
    });
}
function getThroughput(Throughput, mpe) {
    let result = Throughput / mpe;

    if (isNaN(result)) {
        // Zero divided by zero
        return 0;
    } else {
        return Math.round(result);
        // Not zero divided by zero
    }
};
function addCommas(nStr) {
    nStr += '';
    let x = nStr.split('.');
    let x1 = x[0];
    let x2 = x.length > 1 ? '.' + x[1] : '';
    let rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}
function removeChildElements(domElement) {
    let elementToEmpty = document.getElementById(domElement);
    while (elementToEmpty.firstChild) {
        elementToEmpty.removeChild(elementToEmpty.lastChild);
    }
}

function getStatusDescription(status) {
    switch (status) {
        case "On Schedule":
            return 'success';
        case "Behind Schedule":
            return 'danger';
        case "Not Started":
            return 'secondary';
        default:
            return 'warning';
    }
}

function getMPEOperationStatus(machine) {
    if (machine.cur_operation_id > 0) {
        let projectedEndTime = getProjectedEndtime(machine.tot_sortplan_vol, machine.throughput_status, machine.current_run_start);
        if (projectedEndTime > moment(machine.rpg_end_dtm)) {
            return "On Schedule";
        } else {
            return "Behind Schedule";
        }
    } else {
        return 'Not Started';
    }
}

function getProjectedEndtime(totalSortPlan, throughput, currentRunStart) {
    let mpeRuntimeHrs = Math.round(totalSortPlan / throughput);
    return moment(currentRunStart).add(mpeRuntimeHrs, 'hours').format('hh:mm');
}

async function LoadSDOData() {
    fotfmanager.server.getMPESDOStatus(MPEGroupName).done(async (data) => { Promise.all([initMPEGroupStatus(data)]); });
    fotfmanager.server.joinGroup("MPE_" + MPEGroupName);
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
    document.title = $.urlParam("MPEGroupName");
    //start connection 
    $.connection.hub.qs = { 'page_type': "MPE".toUpperCase() };
    $.connection.hub.start({ waitForPageLoad: false })
        .done(() => { Promise.all([LoadSDOData()]); }).catch(
            function (err) {
                console.error(err.toString());
            });

})
$.urlParam = function (name) {
    let results = new RegExp('[\?&]' + name + '=([^&#]*)', 'i').exec(window.location.search);
    MPEGroupName = (results !== null) ? decodeURIComponent(results[1]) || 0 : "";
    return MPEGroupName;
}

function setHeight() {
    let height = (this.window.innerHeight > 0 ? this.window.innerHeight : this.screen.height) - 1;
    let screenTop = (this.window.screenTop > 0 ? this.window.screenTop : 1) - 1;
    let pageBottom = (height - screenTop);
    $("div.card").css("min-height", pageBottom + "px");
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
};