/**
 *use this for the trips
 */

//async function updateTrips(updatetripsstatus) {

//});
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
async function process_trips(trip)
{
    try {
        switch (trip.isAODU) {
            case "Y":
                if (/^o$/i.test(trip.tripDirectionInd)) {
                    oblocal_trips_Table_Body.append(oblocal_trips_row_template.supplant(formatoblocaltriprow(trip)));
                } else if (/^i$/i.test(trip.tripDirectionInd)) {

                }
             
                break;
            case "N":
                if (/^o$/i.test(trip.tripDirectionInd)) {
                    ob_trips_Table_Body.append(ob_trips_row_template.supplant(formatobtriprow(trip)));
                } else if (/^i$/i.test(trip.tripDirectionInd)) {

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
        schd: properties.hasOwnProperty("scheduledDate") ? formatSVTime(properties.scheduledDate) : "" ,
        departed: properties.hasOwnProperty("doorDate") ? formatSVTime(properties.doorDate) : "",
        door: properties.hasOwnProperty("doorNumber") ? properties.doorNumber : "",
        routetrip: properties.route + properties.rrip,
        route: properties.route,
        trip: properties.trip,
        leg: properties.legSiteId,
        dest: properties.legSiteName,
        load: properties.hasOwnProperty("Load") ? properties.doorNumber : "0",
        trbackground: "",// Gettimediff(properties.ScheduledTZ),
        close: properties.hasOwnProperty("Closed") ? properties.doorNumber : "0",
        btnloadDoor: Load_btn_door(properties),
        btnloadPercent: "0%",
        dataproperties: properties
    });
}
let ob_trips_Table = $('table[id=ctsdockdepartedtable]');
let ob_trips_Table_Body = ob_trips_Table.find('tbody');
let ob_trips_row_template = '<tr data-id=ctsOB_{routetrip} data-route={route} data-trip={trip}  data-door={door}  class={trbackground}>' +
    '<td class="text-center">{schd}</td>' +
    '<td class="text-center">{departed}</td>' +
    '<td class="text-center">{btnloadDoor}</td>' +
    '<td class="text-center">{leg}</td>' +
    '<td data-toggle="tooltip" title="{dest}">{dest}</td>' +
    '<td class="text-center">{close}</td>' +
    '<td class="text-center">{load}</td>' +
    '<td class="text-center">{btnloadPercent}</td>' +
    '</tr>"';

/*
 Outbound local trips 
 */

let oblocal_trips_Table = $('table[id=ctslocaldockdepartedtable]');
let oblocal_trips_Table_Body = oblocal_trips_Table.find('tbody');
let oblocal_trips_row_template = '<tr data-id=localctsOB_{routetrip} data-route={route} data-trip={trip} data-door={door} class={trbackground}>' +
    '<td class="text-center">{schd}</td>' +
    '<td class="text-center">{departed}</td>' +
    '<td class="{background}">{btnloadDoor}</td>' +
    '<td class="text-center">{leg}</td>' +
    '<td data-toggle="tooltip" title="{dest}">{dest}</td>' +
    '<td class="text-center">{close}</td>' +
    '<td class="text-center">{load}</td>' +
    '<td>{loadPercent}</td>' +
    '</tr>"';

function formatoblocaltriprow(properties) {
    return $.extend(properties, {
        schd: properties.hasOwnProperty("scheduledDate") ? formatSVTime(properties.scheduledDate) : "",
        departed: properties.hasOwnProperty("doorDate") ? formatSVTime(properties.doorDate) : "",
        door: properties.hasOwnProperty("doorNumber") ? properties.doorNumber : "",
        routetrip: properties.route + properties.trip,
        route: properties.route,
        trip: properties.trip,
        leg: properties.legSiteId,
        dest: properties.legSiteName,
        load: properties.hasOwnProperty("Load") ? properties.Load + '%' : "0",
        trbackground: "",
        close: properties.hasOwnProperty("Closed") ? properties.Closed + '%' : "0", 
        loadPercent: properties.hasOwnProperty("LoadPercent") ? properties.LoadPercent + '%' :"0",
        btnloadDoor: Load_btn_door(properties),
        dataproperties: properties
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
    var td = $(this);
    var doorid = td.attr('data-door');
    if (checkValue(doorid)) {
        LoadDoorDetails(doorid);
    }
});
function formatSVTime(value_time) {
    try {
        var time = moment(value_time);
        if (time._isValid) {
            return time.format("HH:mm");
        }
    } catch (e) {
        console.log(e);
    }
}
