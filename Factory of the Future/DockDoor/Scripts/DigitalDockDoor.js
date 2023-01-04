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
var doornumber = "";
var fotfmanager = $.connection.FOTFManager;
/*
 this is for the dock door and container details.
 */
$.extend(fotfmanager.client, {
    updateDigitalDockDoorStatus: async (digitaldockdoorupdate) => { updateDockDoorStatus(digitaldockdoorupdate) }
});
function updateDockDoorStatus(data) {
    try {
        //show the trip direction 
        // $('label[id=tripDirectionInd]').text(data.tripDirectionInd);
        if (data !== undefined && data.length !== 0) {
            if (data[0].tripDirectionInd === "I") {
                $('button[id=tripDirectionInd]').text("In-bound");
                $('div[id=countdowndiv]').css("display", "block");
                $('label[id=totalIndc]').text("Total Unloaded");
                $('label[id=containerLoaded]').text();

            }
            else if (data[0].tripDirectionInd === "O") {
                //timeCount
                $('div[id=countdowndiv]').css("display", "block");
                $('label[id=timeCount]').text(data.tripMin + " Min");
                $('label[id=totalIndc]').text("Total Unloaded");
            }
            else {
                $('label[id=containerLoaded]').text();


            }
            //load trips
            if (data[0].Legs !== null) {
                $.each(data[0].Legs, function () {
                    var legdata = this;
                    if (data[0].legNumber === legdata.legNumber) {

                        legdata["route"] = data[0].route;
                        legdata["trip"] = data[0].trip;
                        loadLegsTripDataTable([legdata], "currentTripTable");
                    }

                });
            }
            else {
                if (data[0].route !== null) {

                    //incoming trips
                    var indata = [
                        {
                            "route": data[0].route,
                            "trip": data[0].trip,
                            "routeTripLegId": data[0].routeTripLegId,
                            "routeTripId": data[0].routeTripId,
                            "legNumber": 1,
                            "legDestSiteID": data[0].legSiteName,
                            "legOriginSiteID": "230",
                            "scheduledArrDTM": "",
                            "scheduledDepDTM": data[0].scheduledDtm,
                            "actDepartureDtm": data[0].actualDtm,
                            "createdDtm": "",
                            "lastUpdtDtm": "",
                            "legDestSiteName": data[0].legSiteName,
                            "legOriginSiteName": data[0].legSiteName,

                        }
                    ]

                    loadLegsTripDataTable(indata, "currentTripTable");
                }
                else {
                    //remove trips info
                    removeLegsTripDataTable("currentTripTable");
                }
            }
            //else
            //{
            //   loadLegsTripDataTable([{}], "containerLocationtable");
            //}
            if (data.length > 1 && data.length <= 2 ) {
                loadLegsTripDataTable(data[1], "nextTriptable");
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
       // fotfmanager.server.leaveGroup("DockDoor_" + doornumber);
    });
    //Raised when the underlying transport has reconnected.
    $.connection.hub.reconnecting(function () {
        fotfmanager.server.joinGroup("DockDoor_" + doorNum);
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
function setHeight() {
    let height = (this.window.innerHeight > 0 ? this.window.innerHeight : this.screen.height) - 1;
    let screenTop = (this.window.screenTop > 0 ? this.window.screenTop : 1) - 1;
    let pageBottom = (height - screenTop);
    $("div.card").css("min-height", pageBottom + "px");
}
function SortByLegNumber(a, b) {
    let aName = parseInt(a.legNumber.match(/\d+/));
    let bName = parseInt(b.legNumber.match(/\d+/));
    return aName < bName ? -1 : aName > bName ? 1 : 0;
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
function createLegsTripDataTable(table) {
    let arrayColums = [{
            "scheduledArrDTM": "",
            "scheduledDepDTM": "",
            "route": "",
            "trip": "",
            "legDestSiteName": ""
        }]
    var columns = [];
    var tempc = {};
    $.each(arrayColums[0], function (key, value) {
        tempc = {};
        if (/legDestSiteName/i.test(key)) {
            tempc = {
                "title": "Destination",
                "mDataProp": key
            }
        }
        else if (/route/i.test(key)) {
            tempc = {
                "title": "Route",
                "mDataProp": key
            }
        }
        else if (/scheduledArrDTM/i.test(key)) {
            tempc = {
                "title": "Arrive Time",
                "mDataProp": key,
                "mRender": function (data, type, full) {
                    let time = moment().set({ 'year': data.year, 'month': data.month, 'date': data.dayOfMonth, 'hour': data.hourOfDay, 'minute': data.minute, 'second': data.second });
                    return time.format("HH:mm");
                }
            }
        }
        else if (/scheduledDepDTM/i.test(key)) {
            tempc = {
                "title": "Depart Time",
                "mDataProp": key,
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

