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
        var legdata = null;
        //show the trip direction 
        // $('label[id=tripDirectionInd]').text(data.tripDirectionInd);
        if (data !== undefined && data.length > 0) {
            if (data[0].tripDirectionInd === "I") {
 
                $('div[id=countdowndiv]').css("display", "block");
                $('label[id=totalIndc]').text("Total Unloaded");
                $('label[id=containerLoaded]').text();
                $('label[id=countdowntext]').text("Countdown to Scheduled Arrive");
            }
            else if (data[0].tripDirectionInd === "O") {
                $('label[id=countdowntext]').text("Countdown to Scheduled Departure");
                //timeCount
                $('div[id=countdowndiv]').css("display", "block");
       
                $('label[id=totalIndc]').text("Total Loaded");
            }
            else {
                $('label[id=containerLoaded]').text();


            }
            //load trips
            if (data[0].Legs !== null) {
                $.each(data[0].Legs, function () {
                    var legdata = this;
                    if (data[0].legNumber === legdata.legNumber) {
                        $('button[id=currentTripDirectionInd]').text(data[0].tripDirectionInd === "I" ? "In-Bound" : "Out-bound");
                        $('label[id=timeCount]').text(data[0].tripMin + " Min");
                        legdata["route"] = data[0].route;
                        legdata["trip"] = data[0].trip;
                        updateLegsTripDataTable([legdata], "currentTripTable");
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
                            $('button[id=nextTripDirectionInd]').text(data[1].tripDirectionInd === "I" ? "In-Bound" : "Out-bound");
                            
                            legdata["id"] = data[0].id,
                            legdata["route"] = data[1].route;
                            legdata["trip"] = data[1].trip;
                     
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
                                "legNumber": 1,
                                "legDestSiteID": data[1].legSiteName,
                                "legOriginSiteID": "230",
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
                "mDataProp": key,
                "class": "row-cts-des"
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
        else if (/scheduledArrDTM/i.test(key)) {
            tempc = {
                "title": "Arrive",
                "mDataProp": key,
                "class": "row-cts-schd",
                "mRender": function (data, type, full) {
                    let time = moment().set({ 'year': data.year, 'month': data.month, 'date': data.dayOfMonth, 'hour': data.hourOfDay, 'minute': data.minute, 'second': data.second });
                    return time.format("HH:mm");
                }
            }
        }
        else if (/scheduledDepDTM/i.test(key)) {
            tempc = {
                "title": "Depart",
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

