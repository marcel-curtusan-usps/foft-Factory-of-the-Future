/**
 *use this for the trips
 */

$.extend(fotfmanager.client, {
    updateSVTripsStatus: async (updatetripsstatus) => { updateTrips(updatetripsstatus) }
});

async function init_arrive_depart_trips() {
    try {
        //Get trips list
        fotfmanager.server.getTripsList().done(function (trips) {
            if (trips.length > 0) {
                trips.sort(function (left, right) {
                    return moment(left.scheduledDate).diff(moment(right.scheduledDate))
                });
                $.each(trips, function () {
                    process_trips(this);
                });
            }
        });
    } catch (e) {
        console.log(e);
    }
}
async function updateTrips() {

}
async function process_trips(trip)
{
    try {
        switch (trip.isAODU) {
            case "Y":
                if (/^o$/i.test(trip.tripDirectionInd)) {
                    if (formatSVTime(trip.actDepartureDtm) === "" && trip.doorNumber === null) {
                        scheouttrips_Table_Body.append(scheouttrips_row_template.supplant(formatscheouttriprow(trip)));
                    }
                    else {
                        oblocal_trips_Table_Body.append(oblocal_trips_row_template.supplant(formatoblocaltriprow(trip)));
                    }
                } else if (/^i$/i.test(trip.tripDirectionInd)) {
                    scheintrips_Table_Body.append(scheintrips_row_template.supplant(formatscheintriprow(trip)));

                }
                break;
            case "N":
                if (/^o$/i.test(trip.tripDirectionInd)) {
                    if (formatSVTime(trip.actDepartureDtm) === "" && trip.doorNumber === null) {
                        scheouttrips_Table_Body.append(scheouttrips_row_template.supplant(formatscheouttriprow(trip)));
                    }
                    else {
                        ob_trips_Table_Body.append(ob_trips_row_template.supplant(formatobtriprow(trip)));
                    }
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
/**
Outbound network trips
 */
function formatobtriprow(properties) {
    return $.extend(properties, {
        schd: properties.hasOwnProperty("ScheduledDtm") ? formatSVTime(properties.ScheduledDtm) : "" ,
        departed: properties.hasOwnProperty("ActualDtm") ? formatSVTime(properties.ActualDtm) : "",
        door: checkValue(properties.doorNumber) ? properties.doorNumber : "0",
        routetrip: properties.route + properties.trip + properties.tripDirectionInd,
        route: properties.route,
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
let ob_trips_row_template = '<tr data-id=ctsOB_{routetrip} data-route={route} data-trip={trip}  data-door={door}  class={trbackground}>' +
    '<td class="text-center">{schd}</td>' +
    '<td class="text-center">{departed}</td>' +
    '<td>' +
    '<button class="btn btn-outline-info btn-sm btn-block px-1 routetripdetails" data-routetrip="{routetrip}" style="font-size:12px;">{route}-{trip}</button>' +
    '</td>' +
    '<td class="text-center">{btnloadDoor}</td>' +
    '<td class="text-center">{leg}</td>' +
    '<td data-toggle="tooltip" title="{dest}">{dest}</td>' +
    //'<td class="text-center">{close}</td>' +
    //'<td class="text-center">{load}</td>' +
    //'<td class="text-center">{btnloadPercent}</td>' +
    '</tr>"';

/*
 Outbound local trips 
 */

let oblocal_trips_Table = $('table[id=ctslocaldockdepartedtable]');
let oblocal_trips_Table_Body = oblocal_trips_Table.find('tbody');
let oblocal_trips_row_template = '<tr data-id=localctsOB_{routetrip} data-route={route} data-trip={trip} data-door={door} class={trbackground}>' +
    '<td class="text-center">{schd}</td>' +
    '<td class="text-center">{departed}</td>' +
    '<td>'+
    '<button class="btn btn-outline-info btn-sm btn-block px-1 routetripdetails" data-routetrip="{routetrip}" style="font-size:12px;">{route}-{trip}</button>' +
    '</td> ' +
    '<td class="{background}">{btnloadDoor}</td>' +
    '<td class="text-center">{leg}</td>' +
    '<td data-toggle="tooltip" title="{dest}">{dest}</td>' +
    //'<td class="text-center">{close}</td>' +
    //'<td class="text-center">{load}</td>' +
    //'<td class="text-center">{loadPercent}</td>' +
    '</tr>"';

function formatoblocaltriprow(properties) {
    return $.extend(properties, {
        schd: properties.hasOwnProperty("ScheduledDtm") ? formatSVTime(properties.ScheduledDtm) : "",
        departed: properties.hasOwnProperty("ActualDtm") ? formatSVTime(properties.ActualDtm) : "",
        door: checkValue(properties.doorNumber) ? properties.doorNumber : "0",
        routetrip: properties.route + properties.trip + properties.tripDirectionInd,
        route: properties.route,
        trip: properties.trip,
        leg: properties.legSiteId,
        dest: properties.legSiteName,
        load: properties.hasOwnProperty("Load") ? properties.Load + '%' : "0",
        trbackground: "",
        close: properties.hasOwnProperty("Closed") ? properties.Closed + '%' : "0", 
        loadPercent: properties.hasOwnProperty("LoadPercent") ? properties.LoadPercent + '%' :"0 %",
        btnloadDoor: Load_btn_door(properties),
        dataproperties: properties
    });
}

/*
Scheduled outbound trips 
*/
let scheouttrips_Table = $('table[id=ctsouttoptable]');
let scheouttrips_Table_Body = scheouttrips_Table.find('tbody');
let scheouttrips_row_template = '<tr data-id=out_{routetrip} data-door={door} >' +
    '<td class="text-center">{schd}</td>' +
    '<td class="text-center">{departed}</td>' +
    '<td>' +
    '<button class="btn btn-outline-info btn-sm btn-block px-1 routetripdetails" data-routetrip="{routetrip}" style="font-size:12px;">{route}-{trip}</button>' +
    '</td> ' +
    '<td class="{background}">{btnloadDoor}</td>' +
    '<td class="text-center">{firstlegDest}</td>' +
    '<td data-toggle="tooltip" title={firstlegSite}>{firstlegSite}</td>' +
    '</tr>"';
function formatscheouttriprow(properties) {
    return $.extend(properties, {
        schd: properties.hasOwnProperty("ScheduledDtm") ? formatSVTime(properties.ScheduledDtm) : "",
        departed: properties.hasOwnProperty("ActualDtm") ? formatSVTime(properties.ActualDtm) : "",
        routetrip: properties.route + properties.trip + properties.tripDirectionInd,
        door: checkValue(properties.doorNumber) ? properties.doorNumber : "0",
        route: properties.route,
        trip: properties.trip,
        firstlegDest: properties.legSiteId,
        firstlegSite: properties.legSiteName,
        btnloadDoor: Load_btn_door(properties)
    });
}
/*
Scheduled inbound trips
*/
let scheintrips_Table = $('table[id=ctsintoptable]');
let scheintrips_Table_Body = scheintrips_Table.find('tbody');
let scheintrips_row_template = '<tr data-id="in_{routetrip}" data-door="{door}">' +
    '<td  data-toggle="tooltip" title="{route} - {trip}" class="text-center" class="{inbackground}">{schd}</td>' +
    '<td class="text-center" class="{inbackground}">{arrived}</td>' +
    '<td>' +
    '<button class="btn btn-outline-info btn-sm btn-block px-1 routetripdetails" data-routetrip="{routetrip}" style="font-size:12px;">{route}-{trip}</button>' +
    '</td> ' +
    '<td class="text-center" class="{background}">{btnloadDoor}</td>' +
    '<td class="text-center">{leg_Origin}</td>' +
    '<td data-toggle="tooltip" title="{site_Name}">{site_Name}</td>' +
    '</tr>';
function formatscheintriprow(properties) {
    return $.extend(properties, {
        schd: properties.hasOwnProperty("LegScheduledDtm") ? formatSVTime(properties.LegScheduledDtm) : "",
        arrived: properties.hasOwnProperty("ActArrivalDtm") ? formatSVTime(properties.ActArrivalDtm) : "",
        routetrip: properties.route + properties.trip + properties.tripDirectionInd ,
        door: checkValue(properties.doorNumber) ? properties.doorNumber : "0",
        route: properties.route,
        trip: properties.trip,
        inbackground: "", //GettimediffforInbound(properties.Scheduled, properties.Actual),
        leg_Origin: properties.legSiteId,
        site_Name: properties.legSiteName,
        btnloadDoor: Load_btn_door(properties)
    });
}
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
function formatSVmonthdayTime(value_time) {
    try {
        var time = moment(value_time);
        if (time._isValid) {
            if (time.year() === 1) {
                return "";
            }
            return time.format("MM/DD HH:mm");
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
        schedarrtime: formatSVmonthdayTime(properties.LegScheduledDtm),
        scheddeparttime: formatSVmonthdayTime(properties.ScheduledDtm),
        loadstarttime: formatSVmonthdayTime(properties.LoadUnldStartDtm),
        loadendtime: formatSVmonthdayTime(properties.LoadUnldEndDtm),
        doordeparttime: formatSVmonthdayTime(properties.DoorDtm),
        gpsdeparttime: formatSVmonthdayTime(properties.GpsSiteDtm),
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
                                '<br><label>Sched Arr Time:</label> {schedarrtime}' +
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
                '<br><label>Door Arr Time:</label> {doordeparttime}' +
                '<br><label>Manual Site Arr Time:</label> {schedarrtime}' +
                '<br><label>GPS Site Arr Time:</label> {gpsdeparttime}' +
                '<br><label>Actual Arr Time:</label> {schedarrtime}' +
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
                            '<br><label>Sched Dept Time:</label> {scheddeparttime}' +
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
                    '<br><label>Actual Dept Time:</label> {doordeparttime}' +
                    '<br><label>Door Dep Time:</label> {schedarrtime}' +
                    '<br><label>Manual Site Dep Time:</label> {gpsdeparttime}' +
                    '<br><label>GPS Site Dep Time:</label> {schedarrtime}' +
                    '</form>' +
                '</div>' +
            '</div>' +
        '</div>' +
    '</div>';
