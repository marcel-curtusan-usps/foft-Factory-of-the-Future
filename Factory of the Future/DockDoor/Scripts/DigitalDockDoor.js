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
let doornumber = "";
let fotfmanager = $.connection.FOTFManager;
let CurrentTrip = false;
let CurrentTripMin = 0;
let CurrentTripInd = "";
let connecttimer = 0;
let connectattp = 0;
let Timerinterval = 1;
let timer = false;
let tripschtimeduration = null;
//hold the id
let TimerID = -1;
let CountTimer = 0;
let TripDirectionInd = "";
let tripStatus = 0;
let ContainerNotloadedCount = 0;
let legdata = null;

/*
 this is for the dock door and container details.
 */
$.extend(fotfmanager.client, {
    updateDigitalDockDoorStatus: async (digitaldockdoorupdate) => { Promise.all([updateDockDoorStatus(digitaldockdoorupdate)]); }
});
async function updateDockDoorStatus(data) {
    try {

        //show the trip direction
        if (!!data && data.length > 0) {
            legdata = GetLegdata(data);
        }
        else {
            stopTimer();
            reset();
        }
    } catch (e) {
        throw new Error(e.toString());
    }
}
$(function () {
    setHeight();
    $('label[id=dockdoorNumber]').text($.urlParam("dockdoor"));
    if (doornumber !== "") {
        Promise.all([StartDataConnection()]);

        //start connection 
        $.connection.hub.qs = { 'page_type': "DockDoor".toUpperCase() };
        $.connection.hub.start({ waitForPageLoad: false })
            .done(function () {
                fotfmanager.server.getDigitalDockDoorList(doornumber).done(async (DockDoordata) => {
                    Promise.all([updateDockDoorStatus(DockDoordata)]);
                });
                fotfmanager.server.joinGroup("DockDoor_" + doornumber);
                //console.log("Connected at time: " + new Date($.now()));
            }).catch(
                function (err) {
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
                $.connection.hub.start().done(function () {
                    //console.log("Connected time: " + new Date($.now()));
                }).catch(function (err) {
                    throw new Error(err.toString());
                });
            }, 10000);
        });
        //Raised when the underlying transport has reconnected.
        $.connection.hub.reconnecting(function () {
            clearTimeout(connecttimer);
            //console.log("reconnected at time: " + new Date($.now()));
            fotfmanager.server.joinGroup("DockDoor_" + doornumber);
        });
    }
});
$.urlParam = function (name) {
    let results = new RegExp('[\?&]' + name + '=([^&#]*)', 'i').exec(window.location.search);
    doornumber = (results !== null) ? results[1] || 0 : "No Door Selected";
    return doornumber;
};
async function StartDataConnection() {
    // Start the connection
    createLegsTripDataTable("currentTripTable");
    createLegsTripDataTable("nextTriptable");
    createContainerDataTable("containerLocationtable");

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
function updateDataTable(ldata, table) {
    let load = true;
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows(function (idx, data, node) {
            load = false;
            if (data.id === ldata[0].id) {
                $('#' + table).DataTable().row(node).data(ldata[0]).draw().invalidate();
            }
        });
        if (load) {
            Promise.all([loadDataTable(ldata, table)]);
        }
    }
}
function updateContainerDataTable(ldata, table) {
    let load = true;
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows(function (idx, data, node) {
            load = false;
            if (data.Location === ldata[0].Location) {
                $('#' + table).DataTable().row(node).data(ldata[0]).draw().invalidate();
            }
        });
        if (load) {
            loadDataTable(ldata, table);
        }
    }
}
function loadDataTable(data, table) {
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows.add(data).draw();
    }
    return true;
}
function removeLegsTripDataTable(table) {
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows(function (idx, data, node) {
            $('#' + table).DataTable().row(node).remove().draw();
        });
    }
}

function createLegsTripDataTable(table) {
    let arrayColums = [{
        "route": "",
        "trip": "",
        "legSiteName": "",
        "scheduledDtm": ""
    }];
    let columns = [];
    let tempc = {};
    $.each(arrayColums[0], function (key) {
        tempc = {};
        if (/legSiteName/i.test(key)) {
            tempc = {
                "title": '<label class="control-label" style="font-size: 4rem; font-weight: bolder;" id="tirpIncdtext"></label>',
                "mDataProp": key,
                "class": "row-cts-des",
                //"mRender": function (data, type, full) {
                "mRender": function (full, data) {
                    return full;
                    //return full["legSiteName"] + " (" + full["legSiteId"] + ")";
                }
            };
        }
        else if (/route/i.test(key)) {
            tempc = {
                "title": "Route",
                "mDataProp": key,
                "class": "row-cts-route"
            };
        }
        else if (/trip/i.test(key)) {
            tempc = {
                "title": "Trip",
                "mDataProp": key,
                "class": "row-cts-trip"
            };
        }
        else if (/scheduledArrDTM/i.test(key)) {
            tempc = {
                "title": "Arrive",
                "mDataProp": key,
                "class": "row-cts-schd",
                "mRender": function (data) {
                    let time = moment().set({ 'year': data.year, 'month': data.month, 'date': data.dayOfMonth, 'hour': data.hourOfDay, 'minute': data.minute, 'second': data.second });
                    return time.format("HH:mm");
                }
            };
        }
        else if (/scheduledDtm/i.test(key)) {
            tempc = {
                "title": "Sched",
                "mDataProp": key,
                "class": "row-cts-depart",
                //"mRender": function (data, type, full) {
                "mRender": function (data) {
                    let time = moment().set({ 'year': data.year, 'month': data.month, 'date': data.dayOfMonth, 'hour': data.hourOfDay, 'minute': data.minute, 'second': data.second });
                    return time.format("HH:mm");
                }
            };
        }
        else {
            tempc = {
                "title": capitalize_Words(key.replace(/\_/, ' ')),
                "mDataProp": key
            };
        }
        columns.push(tempc);

    });
    $('#' + table).DataTable({
        dom: 'Bfrtip',
        bInfo: false,
        bFilter: false,
        bdeferRender: false,
        scrollX: false,
        scrollCollapse: true,
        paging: false,
        bPaginate: false,
        bAutoWidth: true,
        destroy: true,
        language: {
            zeroRecords: "No Trip Data Available"
        },
        aoColumns: columns,
        columnDefs: [],
        rowCallback: function (row) {
            $(row).find('td:eq(2)').css('font-size', 'calc(0.1em + 2.5vw)');
            $(row).find('td:eq(2)').css('vertical-align', 'middle');

        }
    });
}
function createContainerDataTable(table) {

    let arrayColums = [{
        "Location": "",
        "Count": ""
    }];
    let columns = [];
    let tempc = {};
    $.each(arrayColums[0], function (key) {
        tempc = {};
        if (/Count/i.test(key)) {
            tempc = {
                "title": '<label class="control-label" id=containertext></label> ' + '<label class="control-label" style="font-size: 4rem; font-weight: bolder;" id=totalcontainertext></label>',
                "mDataProp": key,
                "class": "row-cts-count"
            };
        }
        else if (/Location/i.test(key)) {
            tempc = {
                "title": "Location",
                "mDataProp": key,
                "class": "row-cts-des"
            };
        }
        else {

            tempc = {
                "title": capitalize_Words(key.replace(/\_/, ' ')),
                "mDataProp": key
            };
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
        let scheduleDuration = calculatescheduledDuration(SVdtm);

        let timer = setInterval(function () {
            if (!!scheduleDuration && scheduleDuration._isValid) {
                SetDisplayColor(scheduleDuration);
            }
            else {
                stopTimer();
            }

        }, 1000);

        TimerID = timer;

        return timer;
    }
    else {
        stopTimer();
    }
};

function SetDisplayColor() {
    CurrentTripMin = tripschtimeduration.asMinutes();

    if (tripStatus <= 0 && TripDirectionInd === "O") {
        //if (TripDirectionInd === "O") {
        //When the trip departure clock is at 00:10:00, the entire screen for that dock door will turn YELLOW
        if ((CurrentTripMin > 5 && CurrentTripMin <= 10) && ContainerNotloadedCount > 0) {
            //10 minutes before scheduled trip departure
            Tripdisplay("yellow");
        }
        //  When the trip departure clock is at 00:05:00, AND there are closed but containers that have not been loaded for that trip, the entire screen will turn RED
        else if ((CurrentTripMin > 0 && CurrentTripMin < 5) && ContainerNotloadedCount > 0) {
            //5 minutes before scheduled trip departure
            Tripdisplay("red");
        }
        else {
            Tripdisplay("");
        }
        //}
        tripschtimeduration = moment.duration(tripschtimeduration.asSeconds() - Timerinterval, 'seconds');
        $('.timecounter').html(tripschtimeduration.format("d [days] hh:mm:ss ", { trunc: true }));
    }
    else {
        if (!!legdata) {
            let timenow = moment(new Date());
            let triparvtime = moment().set({ 'year': legdata.doorDtm.year, 'month': legdata.doorDtm.month, 'date': legdata.doorDtm.dayOfMonth, 'hour': legdata.doorDtm.hourOfDay, 'minute': legdata.doorDtm.minute, 'second': legdata.doorDtm.second });

            let tripschtime = moment().set({ 'year': legdata.scheduledDtm.year, 'month': legdata.scheduledDtm.month, 'date': legdata.scheduledDtm.dayOfMonth, 'hour': legdata.scheduledDtm.hourOfDay, 'minute': legdata.scheduledDtm.minute, 'second': legdata.scheduledDtm.second });
            let triparvuration = null;
            let tripschduration = null;
            //get the tip schedule time duration
            if (triparvtime._isValid && triparvtime < timenow) {
                triparvuration = moment.duration(triparvtime.diff(timenow)).minutes();
            }
            else {
                triparvuration = moment.duration(timenow.diff(triparvtime)).minutes();
            }
            //get the tip schedule time duration
            if (tripschtime._isValid && tripschtime < timenow) {
                tripschduration = moment.duration(tripschtime.diff(timenow)).minutes();
            }
            else {
                tripschduration = moment.duration(timenow.diff(tripschtime)).minutes();
            }
            if (triparvuration < 0) {
                $('.timecounter').html("");
                $('label[id=incountdowntext]').text("Scheduled Trip " + "\n" + " Arrived");
                $('.timecounter').html("At Door");
                Tripdisplay("");
                if (timer) {
                    stop();
                }
            }
            else {
                $('.timecounter').html("Late");
                Tripdisplay("red");
                if (timer) {
                    stop();
                }
            }
        }
    }
}

function reset() {
    $('#inboundDiv').css("display", "none");
    $('#outboundDiv').css("display", "none");
    $('#currentTripTable').DataTable().clear().draw();
    $('#containerLocationtable').DataTable().clear().draw();
    $('label[id=totalcontainertext]').text("");
    $('label[id=tirpIncdtext]').text("");
    $('label[id=countdowntext]').text("");
    //timeCount
    $('div[id=countdowndiv]').css("display", "none");
    $('.timecounter').html("");

    Tripdisplay("normal");
    if (time) {
        stop();
    }
}
function calculatescheduledDuration(t) {
    if (!!t) {
        let timenow = moment(new Date());
        let conditiontime = moment().set({ 'year': t.year, 'month': t.month, 'date': t.dayOfMonth, 'hour': t.hourOfDay, 'minute': t.minute, 'second': t.second });
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
                $('div.card').addClass('cardYellow');
                break;
            case "red":
                $('div.card').addClass('cardRed');
                $('table').addClass('tablewhite');
                break;
            /*case "normal":*/
            default:
                $('div.card').removeClass('cardRed').removeClass('cardYellow');
                $('table').removeClass('tablewhite');
                break;
        }
    }
}
function SortByName(a, b) {
    let aName = a.location;
    let bName = b.location;
    return ((aName < bName) ? -1 : ((aName > bName) ? 1 : 0));
}
function CreateContainerCount(data) {
    try {
        let ContainerSumCounts = [];
        let filtered = data.filter(function (item) {
            return item.hasCloseScans === true && item.hasLoadScans === false;
        });
        $.map(filtered.sort(SortByName), function (contatiner) {
            ContainerSumCounts.push({
                Location: contatiner.location,
                Count: filtered.filter(function (item) {
                    return item.location === contatiner.location && item.hasCloseScans === true && item.hasLoadScans === false;
                }).length
            });
        });
        let finalCount = remove_duplicates(ContainerSumCounts);
        //console.log(finalCount);
        ContainerNotloadedCount = filtered.length;
        $('label[id=totalcontainertext]').text(ContainerNotloadedCount);
        updateContainerDataTable(finalCount, "containerLocationtable");
        return true;
    } catch (e) {
        throw new Error(e.toString());
        //return false;
    }
}
function remove_duplicates(objectsArray) {
    var usedObjects = {};

    for (var i = objectsArray.length - 1; i >= 0; i--) {
        var so = JSON.stringify(objectsArray[i]);

        if (usedObjects[so]) {
            objectsArray.splice(i, 1);

        } else {
            usedObjects[so] = true;
        }
    }

    return objectsArray;

}
function GetLegdata(legdata) {
    try {
        let displayTrip = getdisplayTrip(legdata)

        if (!!displayTrip) {

            TripDirectionInd = displayTrip.tripDirectionInd;
            $('button[id=currentTripDirectionInd]').text(displayTrip.tripDirectionInd === "I" ? "In-Bound" : "Out-bound");

            if (!timer) {
                tripschtimeduration = calculatescheduledDuration(displayTrip.scheduledDtm);
                start();
            }
            updateDataTable([displayTrip], "currentTripTable");
            if (TripDirectionInd === "I") {
                $('label[id=containertext]').text("Ready to Unload:");
                $('#inboundDiv').css("display", "block");
                $('#outboundDiv').css("display", "none");
                $('label[id=tirpIncdtext]').text("Inbound");
                $('div[id=incountdowndiv]').css("display", "block");
                $('label[id=incountdowntext]').text("Scheduled" + "\n" + " Arrival");
            }
            if (TripDirectionInd === "O") {
                //else if (TripDirectionInd === "O") {
                if (!!displayTrip.containerScans && displayTrip.containerScans.length > 0) {
                    Promise.all([CreateContainerCount(displayTrip.containerScans)]);
                }
                $('label[id=containertext]').text("Ready not Loaded:");
                $('#inboundDiv').css("display", "none");
                $('#outboundDiv').css("display", "block");
                $('label[id=tirpIncdtext]').text("Outbound");
                $('label[id=outcountdowntext]').text("Scheduled " + "\n" + "Departure");
                //timeCount
                $('div[id=outcountdowndiv]').css("display", "block");
            }
            return displayTrip;
        }
        else {
            if (time) {
                stop();
            }
            return null;
        }


    } catch (e) {
        return null;
    }

}
function getdisplayTrip(data) {
    let timenow = moment(new Date());
    let reustltrip = null;
    if (!!data) {
        $.each(data, function (index, trip) {
            let tripschduration = null;
            let tripdptduration = null;
            if (trip.atDoor) {
                reustltrip = trip;
                return true;
            }
            let tripschtime = moment().set({ 'year': trip.scheduledDtm.year, 'month': trip.scheduledDtm.month, 'date': trip.scheduledDtm.dayOfMonth, 'hour': trip.scheduledDtm.hourOfDay, 'minute': trip.scheduledDtm.minute, 'second': trip.scheduledDtm.second });
            let tripdpttime = moment().set({ 'year': trip.actualDtm.year, 'month': trip.actualDtm.month, 'date': trip.actualDtm.dayOfMonth, 'hour': trip.actualDtm.hourOfDay, 'minute': trip.actualDtm.minute, 'second': trip.actualDtm.second });
           //get the tip schedule time duration
            if (tripschtime._isValid && tripschtime < timenow) {
                tripschduration = moment.duration(tripschtime.diff(timenow)).hours();
            }
            else {
                tripschduration = moment.duration(timenow.diff(tripschtime)).hours();
            }
            //get the tip depart time duration
            if (tripdpttime._isValid && tripdpttime < timenow) {
                tripdptduration = moment.duration(timenow.diff(tripdpttime)).hours();
            }
            if (tripschduration > -4 && tripdptduration > 0) {
                reustltrip = trip;
            }
        

        });
    }
    else {
        return reustltrip;
    }
    return reustltrip;
}
function start() {
    //if(!timer) // allow only one timer at a time, working
    //if(timer != true) /* not working */
    //if(timer !== true) /* not working */
    if (timer === false)
        timer = setInterval(function () { SetDisplayColor() }, 1000)
}

function stop() {
    clearInterval(timer);
    timer = false; // false so you can go in if(!timer) condition
}