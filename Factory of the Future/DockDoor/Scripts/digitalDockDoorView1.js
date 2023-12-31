﻿/// A simple template method for replacing placeholders enclosed in curly braces.
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
var doornumber = "";
var fotfmanager = $.connection.FOTFManager;
var countdowntimerid = 0;
var scheduledDateTime = new Date();
var CurrentTrip = false;
var CurrentTripMin = 0;
var CurrentTripInd = "";
var connecttimer = 0;
var connectattp = 0;
var Timerinterval = 1;
var TimerID = -1;//hold the id
var CountTimer = 0;
var TripDirectionInd = "";
var tripStatus = 0;
/*
 this is for the dock door and container details.
 */
$.extend(fotfmanager.client, {
    updateDigitalDockDoorStatus: async (digitaldockdoorupdate) => { updateDockDoorStatus(digitaldockdoorupdate) }
});
function updateDockDoorStatus(data) {
    try {
        var legdata = null;
        //show the trip direction 
        // $('label[id=tripDirectionInd]').text(data.tripDirectionInd);
        if (data !== undefined && data.length > 0) {
            if (data[0].containerScans !== null && data[0].containerScans.length > 0) {
                CreateContainerCount(data[0].containerScans);
            }
            TripDirectionInd = data[0].tripDirectionInd;
            if (TripDirectionInd === "I") {

                $('div[id=countdowndiv]').css("display", "block");
                $('label[id=countdowntext]').text("Scheduled to Arrive");
            }
            else if (TripDirectionInd === "O") {
                $('label[id=countdowntext]').text("Scheduled to Departure");
                //timeCount
                $('div[id=countdowndiv]').css("display", "block");
            }
            //load trips
            if (data[0].Legs.length > 0) {
                $.each(data[0].Legs, function () {
                    var legdata = this;
                    if (data[0].legNumber === legdata.legNumber) {
                        $('button[id=currentTripDirectionInd]').text(TripDirectionInd === "I" ? "In-Bound" : "Out-bound");
                        if (data[0].tripMin !== CurrentTripMin) {
                            //start
                            CountTimer = startTimer(data[0].scheduledDtm);
                        }
                        legdata["id"] = data[0].id;
                        legdata["route"] = data[0].route;
                        legdata["trip"] = data[0].trip;
                        legdata["legSiteId"] = data[0].legSiteId;
                        legdata["legSiteName"] = data[0].legSiteName;
                        legdata["scheduledDepDTM"] = data[0].scheduledDtm;
                        updateLegsTripDataTable([legdata], "currentTripTable");
                        loadContainersDatatable(data[0].containerScans, "containerLocationtable");
                    }

                });
            }
            else {
                if (data[0].route !== null) {

                    //incoming trips
                    var indata = [
                        {
                            "id": data[0].id,
                            "route": data[0].route,
                            "trip": data[0].trip,
                            "routeTripLegId": data[0].routeTripLegId,
                            "routeTripId": data[0].routeTripId,
                            "legNumber": data[0].legNumber,
                            "legSiteId": data[0].legSiteId,
                            "legSiteName": data[0].legSiteName,
                            "legOriginSiteID": "",
                            "scheduledArrDTM": "",
                            "scheduledDepDTM": data[0].scheduledDtm,
                            "actDepartureDtm": data[0].actualDtm,
                            "createdDtm": "",
                            "lastUpdtDtm": "",
                            "legDestSiteName": data[0].legSiteName,
                            "legOriginSiteName": data[0].legSiteName,

                        }
                    ]
                    if (data[0].tripMin !== CurrentTripMin) {
                        //start
                        CountTimer = startTimer(data[0].scheduledDtm);
                    }
                    updateLegsTripDataTable(indata, "currentTripTable");
                }
                else {
                    //remove trips info
                    removeLegsTripDataTable("currentTripTable");
                }
            }
            if (data.length > 1 ) {
                if (data[1].Legs !== null) {
                    $.each(data[1].Legs, function () {
                        legdata = this;
                        if (data[1].legNumber === legdata.legNumber) {
                            $('button[id=nextTripDirectionInd]').text(TripDirectionInd=== "I" ? "In-Bound" : "Out-bound");
                            
                            legdata["id"] = data[0].id;
                            legdata["route"] = data[1].route;
                            legdata["trip"] = data[1].trip;
                            legdata["legSiteId"] = data[1].legSiteId;
                            legdata["legSiteName"] = data[1].legSiteName;
                            legdata["scheduledDepDTM"] = data[1].scheduledDtm;
                            if (data[1].tripDirectionInd === "I") {
                                legdata.legDestSiteName = legdata.legOriginSiteName;
                            }
                            updateLegsTripDataTable([legdata], "nextTriptable");
                        }

                    });
                }
                else {
                    if (data[1].route !== null) {

                        //incoming trips
                        var indata = [
                            {
                                "id": data[1].id,
                                "route": data[1].route,
                                "trip": data[1].trip,
                                "routeTripLegId": data[1].routeTripLegId,
                                "routeTripId": data[1].routeTripId,
                                "legNumber": data[1].legNumber,
                                "legSiteId": data[1].legSiteId,
                                "legSiteName": data[1].legSiteName,
                                "legDestSiteID": data[1].legSiteName,
                                "legOriginSiteID": "",
                                "scheduledArrDTM": "",
                                "scheduledDepDTM": data[1].scheduledDtm,
                                "actDepartureDtm": data[1].actualDtm,
                                "createdDtm": "",
                                "lastUpdtDtm": "",
                                "legDestSiteName": data[1].legSiteName,
                                "legOriginSiteName": data[1].legSiteName,

                            }
                        ]
                        if (data[1].tripDirectionInd === "I") {
                            data[1].legDestSiteName = "";
                        }

                        updateLegsTripDataTable(indata, "nextTriptable");
                    }
                    else {
                        //remove trips info
                        removeLegsTripDataTable("nextTriptable");
                    }
                }

            }
        }
    } catch (e) {
        console.log(e);
    }
}
$(function () {
    setHeight();
    $('label[id=dockdoorNumber]').text($.urlParam("dockdoor"));
    if (doornumber !== "") {
        StartDataConnection(doornumber);
    }
    // Raised when the connection state changes. Provides the old state and the new state (Connecting, Connected, Reconnecting, or Disconnected).
    $.connection.hub.stateChanged(function (state) {
        //switch (state.newState) {
        //    case 1: $('label[id=dockdoorNumber]');
        //        break;
        //    case 4:
        //        $('label[id=dockdoorNumber]');
        //        break;
        //    default: $('label[id=dockdoorNumber]');
        //}
    });
    //handling Disconnect
    $.connection.hub.disconnected(function () {
        connecttimer = setTimeout(function () {
            if (connectattp > 10) {
                clearTimeout(connecttimer);
            }
            connectattp += 1;
            $.connection.hub.start().done(function () {
                console.log("Connected time: " + new Date($.now()));
            }).catch(function (err) {
                console.log(err.toString());
            });
        }, 10000); // Restart connection after 10 seconds.
        // fotfmanager.server.leaveGroup("DockDoor_" + doornumber);
    });
    //Raised when the underlying transport has reconnected.
    $.connection.hub.reconnecting(function () {
        clearTimeout(connecttimer);
        console.log("reconnected at time: " + new Date($.now()));
        fotfmanager.server.joinGroup("DockDoor_" + doornumber);
    });
});
$.urlParam = function (name) {
    var results = new RegExp('[\?&]' + name + '=([^&#]*)','i').exec(window.location.search);
    doornumber = (results !== null) ? results[1] || 0 : "No Door Selected";
    return doornumber;
}
function StartDataConnection(doorNum) {
    // Start the connection
    createLegsTripDataTable("currentTripTable");
    createLegsTripDataTable("nextTriptable");
    createContainerDataTable("containerLocationtable");
    $.connection.hub.qs = { 'page_type': "DockDoor".toUpperCase() };
    $.connection.hub.start({ withCredentials: true, waitForPageLoad: false })
        .done(function () {
            fotfmanager.server.getDigitalDockDoorList(doorNum).done(async (DockDoordata) => {updateDockDoorStatus(DockDoordata)});
            fotfmanager.server.joinGroup("DockDoor_" + doorNum);

        }).catch(
            function (err) {
                console.log(err.toString());
            });
}
function updateCountdown(tripMin, tripdirInd) {
    CurrentTripMin = tripMin;
    CurrentTripInd = tripdirInd;
    if (CurrentTripInd === "I") {
        scheduledDateTime = new Date();
        scheduledDateTime.setMinutes(scheduledDateTime.getMinutes() - CurrentTripMin);
        CurrentTrip = true;
    }
    else if (CurrentTripInd === "O") {
        scheduledDateTime = new Date();
        scheduledDateTime.setMinutes(scheduledDateTime.getMinutes() - CurrentTripMin);
        CurrentTrip = true;
    }
    else {
        CurrentTrip = false;
    }
    if (CurrentTrip === true && countdowntimerid === 0) {
        countdown();
        countdowntimerid = setInterval(countdown, 1000);
    }
    else {
        clearInterval(countdowntimerid);
        countdowntimerid = setInterval(countdown, 1000);
    }
}
function setHeight() {
    let height = (this.window.innerHeight > 0 ? this.window.innerHeight : this.screen.height) - 1;
    let screenTop = (this.window.screenTop > 0 ? this.window.screenTop : 1) - 1;
    let pageBottom = (height - screenTop);
    $("div.card").css("min-height", pageBottom + "px");
}
function SortByLegNumber(a, b) {
    let aName = parseInt(a.legNumber.match(/\d+/), 10);
    let bName = parseInt(b.legNumber.match(/\d+/), 10);
    return aName < bName ? -1 : aName > bName ? 1 : 0;
}
function updateLegsTripDataTable(ldata, table) {
    var load = true;
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows(function (idx, data, node) {
            load = false;
            if (data.id === ldata[0].id) {
                $('#' + table).DataTable().row(node).data(ldata[0]).draw().invalidate();
            }
        })
        if (load) {
            loadLegsTripDataTable(ldata, table);
        }
    }
}
function loadLegsTripDataTable(data, table) {
    if ($.fn.dataTable.isDataTable("#" + table)) {
        if (!$.isEmptyObject(data)) {
            $('#' + table).DataTable().rows.add(data).draw();
        }
    }
}
function removeLegsTripDataTable( table) {
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows(function (idx, data, node) {
                $('#' + table).DataTable().row(node).remove().draw();
        })
    }
}
function loadContainersDatatable(ConatinerData, table)
{

}
function createLegsTripDataTable(table) {
    let arrayColums = [{
        
            "scheduledDepDTM": "",
            "route": "",
            "trip": "",
            "legSiteName": ""
        }]
    var columns = [];
    var tempc = {};
    $.each(arrayColums[0], function (key, value) {
        tempc = {};
        if (/legSiteName/i.test(key)) {
            tempc = {
                "title": "Site",
                "mDataProp": key,
                "class": "row-cts-des",
                "mRender": function (data, type, full) {
                    return full["legSiteName"] + " (" + full["legSiteId"] + ")";
                }
            }
        }
        else if (/route/i.test(key)) {
            tempc = {
                "title": "Route",
                "mDataProp": key,
                "class": "row-cts-route"
            }
        }
        else if (/trip/i.test(key)) {
            tempc = {
                "title": "Trip",
                "mDataProp": key,
                "class": "row-cts-trip"
            }
        }
        else if (/scheduledDepDTM/i.test(key)) {
            tempc = {
                "title": "Sched",
                "mDataProp": key,
                "class": "row-cts-depart",
                "mRender": function (data, type, full) {
                    let time = moment().set({ 'year': data.year, 'month': data.month, 'date': data.dayOfMonth, 'hour': data.hourOfDay, 'minute': data.minute, 'second': data.second });
                    return time.format("HH:mm");
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
        bFilter: false,
        bdeferRender: true,
        paging: false,
        bPaginate: false,
        bAutoWidth: true,
        bInfo: false,
        destroy: true,
        language: {
            zeroRecords: "No Trips Assigned ",
            processing: "<div class='overlay custom-loader-background'><i class='fa fa-cog fa-spin custom-loader-color'></i></div>"
        },
        aoColumns: columns,
        columnDefs: []
    });
}
function createContainerDataTable(table) {

    let arrayColums = [{
        "Location": "",
        "Assigned": "",
        "Close": "",
        "Stage": "",
        "Xdk": "",
        "Loaded":""
    }]
    var columns = [];
    var tempc = {};
    $.each(arrayColums[0], function (key, value) {
        tempc = {};
        tempc = {
            "title": capitalize_Words(key.replace(/\_/, ' ')),
            "mDataProp": key
        }
        columns.push(tempc);
    });
    $('#' + table).DataTable({
        dom: 'Bfrtip',
        bFilter: false,
        bdeferRender: true,
        paging: false,
        bPaginate: false,
        bAutoWidth: true,
        bInfo: false,
        destroy: true,
        language: {
            zeroRecords: "No Containers Found ",
            processing: "<div class='overlay custom-loader-background'><i class='fa fa-cog fa-spin custom-loader-color'></i></div>"
        },        
        aoColumns: columns,
        columnDefs: []
    });
}
function capitalize_Words(str) {
    return str.replace(/\w\S*/g, function (txt) {
        return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
    });
}
function startTimer(SVdtm) {
    if (!!SVdtm) {
        var duration = calculatescheduledDuration(SVdtm);

        var timer = setInterval(function () {
            if (!!duration && duration._isValid) {
                CurrentTripMin = duration.asMinutes();

                if (TripDirectionInd === "O") {
                    //When the trip departure clock is at 00:10:00, the entire screen for that dock door will turn YELLOW
                    if (CurrentTripMin => 5 && CurrentTripMin <= 10) {
                        //10 minutes before scheduled trip departure
                        Tripdisplay("yellow");
                    }
                    //When the trip departure clock is at 00:05:00, AND there are closed but containers that have not been loaded for that trip, the entire screen will turn RED
                    else if (CurrentTripMin => 0 && CurrentTripMin <= 5) {
                        //5 minutes before scheduled trip departure
                        Tripdisplay("red");
                    }
                    else {
                        Tripdisplay("");
                    }
                }

                duration = moment.duration(duration.asSeconds() - Timerinterval, 'seconds');
                $('.timecounter').html(duration.format("d [days] hh:mm:ss ", { trunc: true }));
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
    $('.timecounter').html("");
    $('div[id=countdowndiv]').css("display", "none");
}
function calculatescheduledDuration(t) {
    if (!!t) {
        var timenow = moment();
        var conditiontime = moment().set({ 'year': t.year, 'month': t.month, 'date': t.dayOfMonth, 'hour': t.hourOfDay, 'minute': t.minute, 'second': t.second });
        if (conditiontime._isValid) {

            if (timenow > conditiontime) {
                tripStatus = 1;
                return moment.duration(timenow.diff(conditiontime));
            }
            else {
                tripStatus = 0;
                return moment.duration(conditiontime.diff(timenow));
            }
        }
        else {
            return "";
        }
    }
}
function Tripdisplay(color) {
    if (!!color) {
        switch (color) {
            case "yellow":
                $('div.card').addClass('.cardYellow');
                break;
            case "red":
                $('div.card').addClass('.cardRed');
                break;
            default:
                $('div.card').removeClass('cardRed').removeClass('cardYellow');
                break;
        }
    }
}
function CreateContainerCount(data) {
    let ContainerSumCounts = {};
    containerArray = [];
    $.map(data, function (contatiner) {
        if (contatiner.hasCloseScans && !contatiner.hasLoadScans) {
            if (containerArray.hasOwnProperty(contatiner.location)) {
                containerArray.push({
                    Location: contatiner.location,
                    count: 1
                })
            }
            else {
                containerArray.push({
                    Location: contatiner.location,
                    Count: 1
                })
            }
        }
    });
    $('label[id=totalcontainertext]').text(containerArray.length);
    loadContainersDatatable(containerArray, "containerLocationtable");
}



