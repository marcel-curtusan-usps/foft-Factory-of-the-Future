/**
 *use this for the trips
 */
$.extend(fotfmanager.client, {
    TripsAdd: async (tripsadd) => { Promise.all([addTrips(tripsadd)]) },
    TripsUpdate: async (updatetripsstatus) => { Promise.all([updateTrips(updatetripsstatus)]) },
    TripsRemove: async (tripid) => { Promise.all([ removeTrips(tripid)]) }
});

async function init_arrive_depart_trips() {
    try {
        //Get trips list
        fotfmanager.server.getTripsList().done(function (trips) {
            if (trips.length > 0) {
                trips.sort(function (left, right) {
                    if (!$.isEmptyObject(left.scheduledDtm)) {
                        var leftt = moment().set({ 'year': left.scheduledDtm.year, 'month': left.scheduledDtm.month + 1, 'date': left.scheduledDtm.dayOfMonth, 'hour': left.scheduledDtm.hourOfDay, 'minute': left.scheduledDtm.minute, 'second': left.scheduledDtm.second });
                        var rightt = moment().set({ 'year': right.scheduledDtm.year, 'month': right.scheduledDtm.month + 1, 'date': right.scheduledDtm.dayOfMonth, 'hour': right.scheduledDtm.hourOfDay, 'minute': right.scheduledDtm.minute, 'second': right.scheduledDtm.second });

                        return leftt.diff(rightt);
                    }
                   
                });
                $.each(trips.sort(SortBySiteName), function () {
                    process_trips(this);
                    $('<option/>').attr("id",this.id).val(this.id).html(this.legSiteName + " | " + this.route + " - " + this.trip + " | " + tripDirectiontext(this)).appendTo('select[id=tripSelector]');
                });
            }
        });
    } catch (e) {
        console.log(e);
    }
}
//async function updateTrips(trip) {
//    let routetrip = trip.id;
//    let trname = $.find('tbody tr[data-id=' + routetrip + ']');
//   if (trname.length > 0) {
//        for (let tr_name of trname) {
//            let tablename = $(tr_name).closest('table').attr('id');
//            if (checkValue(tablename)) {
//                if (/remove/i.test(trip.state)) {
                      
//                }
//                else {
//                    switch (tablename) {
//                        case "ctsdockdepartedtable":
//                            ob_trips_Table_Body.find('tbody tr[data-id=' + routetrip + ']').replaceWith(ob_trips_row_template.supplant(formatobtriprow(trip)));
//                            break;
//                        case "ctslocaldockdepartedtable":
//                            oblocal_trips_Table_Body.find('tbody tr[data-id=' + routetrip + ']').replaceWith(oblocal_trips_row_template.supplant(formatobtriprow(trip)));
//                            break;
//                        case "ctsouttoptable":
//                            scheouttrips_Table_Body.find('tbody tr[data-id=' + routetrip + ']').replaceWith(scheouttrips_row_template.supplant(formatobtriprow(trip)));
//                            break;
//                        case "ctsintoptable":
//                            scheintrips_Table_Body.find('tbody tr[data-id=' + routetrip + ']').replaceWith(scheintrips_row_template.supplant(formatobtriprow(trip)));
//                            break;
//                        default:
//                    }
//                }
//            }
//        }
       
//    }
//    else {
//        if (!/remove/i.test(trip.state)) {
//            process_trips(trip);
//        }
        
//    }
//}
async function process_trips(trip)
{
    try {
        switch (trip.isAODU) {
            case "Y":
                if (/^o$/i.test(trip.tripDirectionInd)) {
                    scheouttrips_Table_Body.append(scheouttrips_row_template.supplant(formatscheouttriprow(trip)));
                    oblocal_trips_Table_Body.append(oblocal_trips_row_template.supplant(formatoblocaltriprow(trip)));

                } else if (/^i$/i.test(trip.tripDirectionInd)) {
                    scheintrips_Table_Body.append(scheintrips_row_template.supplant(formatscheintriprow(trip)));

                }
                break;
            case "N":
                if (/^o$/i.test(trip.tripDirectionInd)) {
                    scheouttrips_Table_Body.append(scheouttrips_row_template.supplant(formatscheouttriprow(trip)));
                    ob_trips_Table_Body.append(ob_trips_row_template.supplant(formatobtriprow(trip)));
                } else if (/^i$/i.test(trip.tripDirectionInd)) {
                    scheintrips_Table_Body.append(scheintrips_row_template.supplant(formatscheintriprow(trip)));
                }
               
                break;
            default:
        }
       
    } catch (e) {
        console.log(e);
    }
}
async function removeTrips(id) {
    removeTripsDoorAssignedDatatable(id, "doortriptable")
    //let trname = $.find('tbody tr[data-id=' + id + ']');

    //if (trname.length > 0) {
    //    for (let tr_name of trname) {
    //        let tablename = $(tr_name).closest('table').attr('id');
    //        $('#' + tablename).find('tbody tr[data-id=' + id + ']').remove();
    //    }
    //}
    if ($('#tripSelector option[id=' + id + ']').length > 0) {
        $('#tripSelector option[id=' + id + ']').remove();
    }
}
async function addTrips(trip) {
    if ($('#tripSelector option[id=' + trip.id + ']').length === 0) {
        $('<option/>').attr("id", trip.id).val(trip.id).html(trip.legSiteName + " | " + trip.route + " - " + trip.trip + " | " + tripDirectiontext(trip)).appendTo('select[id=tripSelector]');

        $('#tripSelector').html($('#tripSelector option').sort(SortByValue));
    }

}
async function updateTrips(trip) {


}
/**
Outbound network trips
 */
function formatobtriprow(properties) {
    return $.extend(properties, {
        schd: objSVTime(properties.scheduledDtm), //properties.hasOwnProperty("ScheduledDtm") ? formatSVTime(properties.ScheduledDtm) : "" ,
        departed: !$.isEmptyObject(properties.actualDtm) ? objSVTime(properties.actualDtm) : "",
        door: checkValue(properties.doorNumber) ? properties.doorNumber : "0",
        routetrip: properties.route + properties.trip + properties.tripDirectionInd,
        route: properties.route,
        routeid: properties.id,
        trip: properties.trip,
        leg: properties.legSiteId,
        dest: properties.legSiteName,
        load: properties.hasOwnProperty("Load") ? properties.doorNumber : "0",
        trbackground: "",// Gettimediff(properties.ScheduledTZ),
        close: properties.hasOwnProperty("Closed") ? properties.doorNumber : "0",
        btnloadDoor: Load_btn_door(properties),
        btnloadPercent: "0 %",
        dataproperties: properties
    });
}
let ob_trips_Table = $('table[id=ctsdockdepartedtable]');
let ob_trips_Table_Body = ob_trips_Table.find('tbody');
let ob_trips_row_template = '<tr data-id={routeid} data-route={route} data-trip={trip}  data-door={door}  class={trbackground}>' +
    '<td><span class="ml-p25rem">{schd}</span></td>' +
    '<td>{departed}</td>' +
    '<td>' +
    '<button class="btn btn-outline-info btn-sm btn-block px-1 routetripdetails" data-routetrip="{routeid}" style="font-size:12px;">{route}-{trip}</button>' +
    '</td>' +
    '<td>{btnloadDoor}</td>' +
    '<td>{leg}</td>' +
    '<td data-toggle="tooltip" title="{dest}">{dest}</td>' +
    //'<td class="text-center">{close}</td>' +
    //'<td class="text-center">{load}</td>' +
    //'<td class="text-center">{btnloadPercent}</td>' +
    '</tr>"';

/*
 Outbound local trips 
 */
function formatoblocaltriprow(properties) {
    return $.extend(properties, {
        schd: objSVTime(properties.scheduledDtm),//properties.hasOwnProperty("ScheduledDtm") ? formatSVTime(properties.ScheduledDtm) : "",
        departed: !$.isEmptyObject(properties.actualDtm) ? objSVTime(properties.actualDtm) : "",
        door: checkValue(properties.doorNumber) ? properties.doorNumber : "0",
        routetrip: properties.route + properties.trip + properties.tripDirectionInd,
        route: properties.route,
        routeid: properties.id,
        trip: properties.trip,
        leg: properties.legSiteId,
        dest: properties.legSiteName,
        load: properties.hasOwnProperty("Load") ? properties.Load + '%' : "0",
        trbackground: "",
        close: properties.hasOwnProperty("Closed") ? properties.Closed + '%' : "0",
        loadPercent: properties.hasOwnProperty("LoadPercent") ? properties.LoadPercent + '%' : "0 %",
        btnloadDoor: Load_btn_door(properties),
        dataproperties: properties
    });
}
let oblocal_trips_Table = $('table[id=ctslocaldockdepartedtable]');
let oblocal_trips_Table_Body = oblocal_trips_Table.find('tbody');
let oblocal_trips_row_template = '<tr data-id={routeid} data-route={route} data-trip={trip} data-door={door} class={trbackground}>' +
    '<td class="text-center">{schd}</td>' +
    '<td class="text-center">{departed}</td>' +
    '<td>'+
    '<button class="btn btn-outline-info btn-sm btn-block px-1 routetripdetails" data-routetrip="{routeid}" style="font-size:12px;">{route}-{trip}</button>' +
    '</td> ' +
    '<td class="{background}">{btnloadDoor}</td>' +
    '<td class="text-center">{leg}</td>' +
    '<td data-toggle="tooltip" title="{dest}">{dest}</td>' +
    //'<td class="text-center">{close}</td>' +
    //'<td class="text-center">{load}</td>' +
    //'<td class="text-center">{loadPercent}</td>' +
    '</tr>"';



/*
Scheduled outbound trips 
*/
function formatscheouttriprow(properties) {
    return $.extend(properties, {
        schd: objSVTime(properties.scheduledDtm),//properties.hasOwnProperty("ScheduledDtm") ? formatSVTime(properties.ScheduledDtm) : "",
        departed: !$.isEmptyObject(properties.actualDtm) ? objSVTime(properties.actualDtm) : "",
        routetrip: properties.route + properties.trip + properties.tripDirectionInd,
        door: checkValue(properties.doorNumber) ? properties.doorNumber : "0",
        route: properties.route,
        routeid: properties.id,
        trip: properties.trip,
        firstlegDest: properties.legSiteId,
        firstlegSite: properties.legSiteName,
        btnloadDoor: Load_btn_door(properties)
    });
}
let scheouttrips_Table = $('table[id=ctsouttoptable]');
let scheouttrips_Table_Body = scheouttrips_Table.find('tbody');
let scheouttrips_row_template = '<tr data-id={routeid} data-door={door} >' +
    '<td class="text-center">{schd}</td>' +
    '<td class="text-center">{departed}</td>' +
    '<td>' +
    '<button class="btn btn-outline-info btn-sm btn-block px-1 routetripdetails" data-routetrip="{routeid}" style="font-size:12px;">{route}-{trip}</button>' +
    '</td> ' +
    '<td class="{background}">{btnloadDoor}</td>' +
    '<td class="text-center">{firstlegDest}</td>' +
    '<td data-toggle="tooltip" title={firstlegSite}>{firstlegSite}</td>' +
    '</tr>"';

/*
Scheduled inbound trips
*/
function formatscheintriprow(properties) {
    return $.extend(properties, {
        schd: objSVTime(properties.scheduledDtm),//properties.hasOwnProperty("LegScheduledDtm") ? formatSVTime(properties.LegScheduledDtm) : "",
        arrived: !$.isEmptyObject(properties.legActualDtm) ? objSVTime(properties.legActualDtm) : "",
        routetrip: properties.route + properties.trip + properties.tripDirectionInd,
        door: checkValue(properties.doorNumber) ? properties.doorNumber : "0",
        route: properties.route,
        routeid: properties.id,
        trip: properties.trip,
        inbackground: "", //GettimediffforInbound(properties.Scheduled, properties.Actual),
        leg_Origin: properties.legSiteId,
        site_Name: properties.legSiteName,
        btnloadDoor: Load_btn_door(properties)
    });
}
let scheintrips_Table = $('table[id=ctsintoptable]');
let scheintrips_Table_Body = scheintrips_Table.find('tbody');
let scheintrips_row_template = '<tr data-id="{routeid}" data-door="{door}">' +
    '<td  data-toggle="tooltip" title="{route} - {trip}" class="text-center" class="{inbackground}">{schd}</td>' +
    '<td class="text-center" class="{inbackground}">{arrived}</td>' +
    '<td>' +
    '<button class="btn btn-outline-info btn-sm btn-block px-1 routetripdetails" data-routetrip="{routeid}" style="font-size:12px;">{route}-{trip}</button>' +
    '</td> ' +
    '<td class="text-center" class="{background}">{btnloadDoor}</td>' +
    '<td class="text-center">{leg_Origin}</td>' +
    '<td data-toggle="tooltip" title="{site_Name}">{site_Name}</td>' +
    '</tr>';

function Load_btn_door(properties) {
    if (properties.hasOwnProperty("doorNumber")) {
        if (checkValue(properties.doorNumber)) {
            return '<button class="btn btn-outline-info purpleBg btn-sm btn-block px-1 doordetails" data-door="' + properties.doorNumber + '">' + properties.doorNumber + '</button>';
        } else {
            return '';
        }
    }
    else {
        return '';
    }
}
function tripDirectiontext(data) {
    return data.tripDirectionInd === "O" ? "Out-bound" : "In-bound"
}
$(document).on('click', '.doordetails', function () {
    var button = $(this);
    var doorid = button.attr('data-door');
    if (checkValue(doorid)) {
        LoadDoorDetails(doorid);
    }
});
$(document).on('click', '.routetripdetails', function () {
    var button = $(this);
    var routetripid = button.attr('data-routetrip');
    if (checkValue(routetripid)) {
        LoadRouteTripDetails(routetripid);
    }
});
function formatSVTime(value_time) {
    try {
        var time = moment(value_time);
        if (time._isValid) {
            if (time.year() === 1) {
                return "";
            }
            return time.format("HH:mm");
        }
    } catch (e) {
        console.log(e);
    }
}

function LoadRouteTripDetails(id) {
    try {

        fotfmanager.server.getRouteTripsInfo(id).done(function (routetrip) {
            if (routetrip.length > 0) {
                routetrip = routetrip[0];
                let DirectionInd = routetrip.tripDirectionInd === "I" ? "Inbound Trip Details" : "Outbound Trip Details";
                $('#modalRouteTripDetailsHeader_ID').text(DirectionInd);
                $('#RouteTripDetailscard').empty();
                if (routetrip.tripDirectionInd === "I" ) {
                    $('#RouteTripDetailscard').append(routedetailsInform_template.supplant(routedetailsform(routetrip)));
                }
                else {
                    $('#RouteTripDetailscard').append(routedetailsOutform_template.supplant(routedetailsform(routetrip)));
                }
  
            }
        });
        $("#RouteTripDetails_Modal").modal();
    } catch (e) {
        console.log(e);
    }
}
function routedetailsform(properties) {
    return $.extend(properties, {
        routetrip: properties.route +"-"+ properties.trip  ,
        servicetype: GetServiceTypeCode(properties.serviceTypeCode),
        legnumber: properties.legNumber,
        legdestination: properties.legSiteName + " (" + properties.legSiteId + ")",
        finaldestination: properties.destSiteName + " (" + properties.destSiteId + ")",
        legorigin: properties.legSiteName + " (" + properties.legSiteId + ")",
        initorigin: properties.destSiteName + " (" + properties.destSiteId + ")",
        schedtime: formatSVmonthdayTime(properties.scheduledDtm),
        actualtime: formatSVmonthdayTime(properties.actualDtm),
        loadstarttime: formatSVmonthdayTime(properties.LoadUnldStartDtm),
        loadendtime: formatSVmonthdayTime(properties.LoadUnldEndDtm),
        unloadstarttime: formatSVmonthdayTime(properties.loadUnldStartDtm),
        unloadendtime: formatSVmonthdayTime(properties.loadUnldEndDtm),
        doortime: formatSVmonthdayTime(properties.doorDtm),
        gpstime: formatSVmonthdayTime(properties.gpsSiteDtm),
        doornumber: checkValue(properties.doorNumber) ? properties.doorNumber : "",
        vannumber: checkValue(properties.vanNumber) ? properties.vanNumber :"",
        trailernumber: checkValue(properties.trailerBarcode) ? properties.trailerBarcode : "",
        trailerlength: checkValue(properties.trailerLengthCode) ? properties.trailerLengthCode : "",
        origgpssource: checkValue(properties.gpsIdSource) ? properties.gpsIdSource : "",
        origgpsid: checkValue(properties.gpsId) ? properties.gpsId : "",
        destgpssource: checkValue(properties.gpsIdSource) ? properties.gpsIdSource : "",
        destgpsid: checkValue(properties.gpsId) ? properties.gpsId : "",
        drivername: checkValue(properties.driverFirstName) ? properties.driverFirstName + " " + (checkValue(properties.driverLastName) ? properties.driverLastName : "") : "",
        driverphone: checkValue(properties.driverPhoneNumber) ? properties.driverPhoneNumber : "",
        msp: checkValue(properties.mspBarcode) ? properties.mspBarcode : "",
        originseal: checkValue(properties.originSeal) ? properties.originSeal : "",
        destseal: checkValue(properties.destSeal) ? properties.destSeal : "",
        delayreason: checkValue(properties.delayCode) ? properties.delayCode : "",
        origcomment: checkValue(properties.destComments) ? properties.destComments : "",
        destcomment: checkValue(properties.originComments) ? properties.originComments : "",
    });
}
function GetServiceTypeCode(data)
{
    try {
        switch (data) {
            case "A":
                return "Rail";
            case "D":
                return "Drop Shipment";
            case "H":
                return "HCR";
            case "P":
                return "Periodicals";
            case "V":
                return "PVS";
            default:
                return "";
        }
    } catch (e) {
        console.log(e)
    }
}
let routedetailsInform_template =
    '<div class="row">' +
    '<div class="col-sm-7">' +
            '<div class="card pb-0">' +
                    '<div class="card-body pb-0">' +
                                '<form style="float:left" class="panelForms">' +
                                '<label>Route-Trip:</label> {routetrip}' +
                                '<br>' +
                                '<label>Service Type:</label> {servicetype} ' +
                                '<label style="text-align: right;" class="checkBox">Leg:</label> {legnumber}' +
                                '<br><label>Leg Origin:</label> {legorigin}' +
                                '<br><label>Initial Origin:</label> {initorigin}' +
                                '<br><label>Sched Arr Time:</label> {schedtime}' +
                                '<br>' +
                                '<label>Door:</label> {doornumber}' +
                                '<br>                     ' +
                                '<label>Van Number:</label> {vannumber}' +
                                '<br>' +
                                '<label>Trailer:</label> {trailernumber}' +
                                '<br>' +
                                '<label>Trailer Length:</label> {trailerlength}' +
                                '<br>' +
                                '<br>' +
                                '<label>Origin GPS Source:</label> {origgpssource}' +
                                '<br>' +
                                '<label>Origin GPS ID:</label> {origgpsid}' +
                                '<br>' +
                                '<label>Dest GPS Source:</label> {destgpssource}' +
                                '<br>' +
                                '<label>Dest GPS ID:</label>  {destgpsid}' +
                                '<br>' +
                                '<label>Driver Name:</label> {drivername}' +
                                '<br>' +
                                '<label>Driver Phone Number:</label> {driverphone}' +
                                '<br>' +
                                '<label> MSP: </label> {msp}' +
                                '<br>' +
                                '<label >Origin Seal:</label> {originseal}' +
                                '<br>' +
                                '<label>Destination Seal:</label> {destseal}' +
                                '<br>' +
                                '<br>' +
                                '<label>Delay Reason</label> {delayreason}' +
                                '<br>' +
                                '<br>' +
                                '<label style="width: auto;">Origin Comments:</label> {origcomment}' +
                                '<br>' +
                                '<br>' +
                                '<label style="width: auto;">Destination Comments:</label> {destcomment}' +
                                '</form>' +
                    '</div>' +
            '</div>' +
    '</div>' +
    '<div class="col-sm-5">' +
        '<div class="card pb-0">' +
            '<div class="card-body pb-0">' +
                '<form style="float:left" class="panelForms">' +
                '<br><label>Door Arr Time:</label> {doortime}' +
                '<br><label>Manual Site Arr Time:</label> {actualtime}' +
                '<br><label>GPS Site Arr Time:</label> {gpstime}' +
                '<br><label>Actual Arr Time:</label> {actualtime}' +
                '</form>' +
            '</div>' +
        '</div>' +
    '</div>' +
    '</div>';

let routedetailsOutform_template =
    '<div class="row">' +
    '<div class="col-sm-7">' +
            '<div class="card pb-0">' +
                    '<div class="card-body">' +
                            '<form style="float:left" class="panelForms">' +
                            '<label>Route-Trip:</label> {routetrip}' +
                            '<br>' +
                            '<label>Service Type:</label> {servicetype} ' +
                            '<label style="text-align: right;" class="checkBox">Leg:</label> {legnumber}' +
                            '<br><label>Leg Destination:</label> {legdestination}' +
                            '<br><label>Final Destination:</label> {finaldestination}' +
                            '<br><label>Sched Dept Time:</label> {schedtime}' +
                            '<br>' +
                            '<label>Door:</label> {doornumber}' +
                            '<br>                     ' +
                            '<label>Van Number:</label> {vannumber}' +
                            '<br>' +
                            '<label>Trailer:</label> {trailernumber}' +
                            '<br>' +
                            '<label>Trailer Length:</label> {trailerlength}' +
                            '<br>' +
                            '<label>Origin GPS Source:</label> {destgpsid}' +
                            '<br>' +
                            '<label>Origin GPS ID:</label> {destgpsid}' +
                            '<br>' +
                            '<label>Dest GPS Source:</label> {destgpsid}' +
                            '<br>' +
                            '<label>Dest GPS ID:</label> {destgpsid}' +
                            '<br>' +
                            '<label >Driver Name:</label> {drivername}' +
                            '<br>' +
                            '<label>Driver Phone Number:</label> {driverphone}' +
                            '<br>' +
                            '<label> MSP: </label> {msp}' +
                            '<br>' +
                            '<label >Origin Seal:</label> {originseal}' +
                            '<br>' +
                            '<br>' +
                            '<label style="width: auto;">Origin Comments:</label>' +
                            '</form>' +
                    '</div>' +
            '</div>' +
    '</div>' +
        '<div class="col-5">' +
            '<div class="card pb-0">' +
                '<div class="card-body">' +
                    '<form style="float:left" class="panelForms">' +
                    '<br><label>Actual Dept Time:</label> {actualtime}' +
                    '<br><label>Door Dep Time:</label> {doortime}' +
                    '<br><label>Manual Site Dep Time:</label> {actualtime}' +
                    '<br><label>GPS Site Dep Time:</label> {gpstime}' +
                    '</form>' +
                '</div>' +
            '</div>' +
        '</div>' +
    '</div>';
