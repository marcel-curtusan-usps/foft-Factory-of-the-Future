
let MPEGroupName = "";
let fotfmanager = $.connection.FOTFManager;
$.extend(fotfmanager.client, {
    UpdateMPESDOStatus: (updatedata) => { Promise.all([updateMPEGroupStatus(updatedata)]) }
});
async function updateMPEGroupStatus(data)
{
    try {
        if (!!data) {
            /* For the list of MPEs*/
            function mpeDetails(mpeName, mpeStatus, curOpId) {
                this.mpeName = mpeName;
                this.mpeStatus = mpeStatus;
                this.curOpId = curOpId;
            }
            /*To store the calculations for each MPE in the group*/
            function mpeGroup(machineType, scheduledStaff, actualStaff, totalVolume, plannedVolume, totalThroughput, plannedThroughput, plannedEndTime, currentRunStart, projectedEndTime, mpesRunning, message) {
                this.machineType = machineType;
                this.scheduledStaff = scheduledStaff;
                this.actualStaff = actualStaff;
                this.totalVolume = totalVolume;
                this.plannedVolume = plannedVolume;
                this.totalThroughput = totalThroughput;
                this.plannedThroughput = plannedThroughput;
                this.plannedEndTime = plannedEndTime;
                this.currentRunStart = currentRunStart;
                this.projectedEndTime = projectedEndTime;
                this.mpesRunning = mpesRunning
                this.message = message;
            }
            let mpeSummary = new mpeGroup('', 0, 0, 0, 0, 0, 0, '', '');
            let mpeDetailsList = new Array();
            let previousPlannedEndTime = moment('01/01/1970 00:03:44');
            let previousRunStart = moment('01/01/1970 00:03:44');
            let mpesRunning = 0;
           
            $.each(data, function (key, value) {
                /*Add up only data from running MPEs*/
                if (this.CurrentOperationId != 0) { 
                    mpesRunning += 1;
                    mpeSummary.machineType = this.MachineType; /*this one could be a single field and the rest a list of items **Analize it...**/
                    mpeSummary.scheduledStaff += this.ScheduledStaff;
                    mpeSummary.actualStaff += this.ActualStaff;
                    mpeSummary.totalVolume += this.TotalVolume;
                    mpeSummary.plannedVolume += this.PlannedVolume;
                    mpeSummary.totalThroughput += this.TotalThroughput;
                    mpeSummary.plannedThroughput += this.PlannedThroughput;
                    /*Capture the latest Planned End Time and save the current run start to determine whether we are within
                     * the planned estimated time of completion */
                    if (moment(this.PlannedEndTime) > previousPlannedEndTime) {
                        mpeSummary.plannedEndTime = this.PlannedEndTime;
                        mpeSummary.currentRunStart = moment(this.CurrentRunStart).format('M/DD/yyyy hh:mm:ss');
                    }
                    else
                    {
                        mpeSummary.plannedEndTime = previousPlannedEndTime;
                        mpeSummary.currentRunStart = previousRunStart;
                    }
                    
                    mpeSummary.message = this.Message; /*this one could be a single field and the rest a list of items **Analize it...**/
                    previousPlannedEndTime = moment(this.PlannedEndTime);
                    previousRunStart = moment(this.CurrentRunStart).format('mm/dd/yyy hh:mm:ss');
                }
                
                /* 2. Create the list of MPE elements */
                let status = getMPEOperationStatus(this);
                mpeDetailsList.push(new mpeDetails(this.MachineName, status, this.CurrentOperationId));
            });

            mpeSummary.mpesRunning = mpesRunning;
            mpeSummary.projectedEndTime = getProjectedEndtime(mpeSummary);
            populateFields(mpeSummary, getSortedData(mpeDetailsList, 'mpeName', 1));
        }
    } catch (e) {
        console.log(e);
    }
}

function getSortedData(data, prop, isAsc) {
    return data.sort((a, b) => {
        return (a[prop] < b[prop] ? -1 : 1) * (isAsc ? 1 : -1)
    });
}

function populateFields(machineSummary, mpesGroup) {
    $('h1[id=machineType]').text("Machine Type: " + machineSummary.machineType);
    $('h1[id=scheduledStaff]').text("Scheduled Staff: " + machineSummary.scheduledStaff);
    $('h1[id=actualStaff]').text("Actual Staff: " + machineSummary.actualStaff);
    $('p[id=totalVolume]').text(machineSummary.totalVolume);
    $('h1[id=plannedVolume]').text("Plan: " + machineSummary.plannedVolume);
    $('p[id=avgThroughput]').text(Math.round(machineSummary.totalThroughput/machineSummary.mpesRunning));
    $('h1[id=plannedThroughput]').text("Plan: " + machineSummary.plannedThroughput);
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

    $.each(mpesGroup, function (key, value) {
        let singleMPEDiv = document.createElement('div');
        singleMPEDiv.className = 'item2';
        /*Create inner elements*/
        let innerH5 = document.createElement('h5');
        innerH5.className = 'card-title';
        innerH5.textContent = this.mpeName;
        
        let innerH6 = document.createElement('h6');
        innerH6.className = 'card-text btn-' + getStatusDescription(this.mpeStatus);
        innerH6.textContent = this.mpeStatus;

        let innerP = document.createElement('p');
        innerP.className = 'card-text';
        innerP.textContent = this.mpeStatus === 'warning' ? "N/A" : 'OP# ' + this.curOpId;

        singleMPEDiv.appendChild(innerH5);
        singleMPEDiv.appendChild(innerH6);
        singleMPEDiv.appendChild(innerP);

        let groupList = document.getElementById("mpeGroupList");
        groupList.appendChild(singleMPEDiv);
    });
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
    if (machine.CurrentOperationId != 0) {
        let projectedEndTime = getProjectedEndtime(machine);
        if (projectedEndTime > moment(machine.plannedEndTime)) {    
            return "On Schedule";
        } else {
            return "Behind Schedule";
        }
    } else {
        return 'Not Started';
    }   
}

function getProjectedEndtime(machine) {
    let mpeRuntimeHrs = Math.round(machine.totalVolume / machine.totalThroughput);
    return moment(machine.currentRunStart).add(mpeRuntimeHrs, 'hours').format('hh:mm');
}

async function LoadSDOData()
{
    console.log("SDO Connected time: " + new Date($.now()));
    fotfmanager.server.getMPESDOStatus(MPEGroupName).done(async (data) => { Promise.all([updateMPEGroupStatus(data)]) });
    fotfmanager.server.joinGroup("MPE_" + MPEGroupName);
    console.log("MPE_" + MPEGroupName);
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
        .done(() => { Promise.all([LoadSDOData()]) }).catch(
            function (err) {
                console.log(err.toString());
            });
    
});
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
}