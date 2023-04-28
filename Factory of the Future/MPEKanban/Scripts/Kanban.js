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
var fotfmanager = $.connection.FOTFManager;
var DateTimeNow = new Date();
let kanbandata;
let kanbandataDispatch;
let kanbandiv = $('#divkanban');
let kanbandiv_dispatch = $('#divkanban_dispatch');
let layout_templet = '<div class="col btn-group btn-border" role="group">' +
    '<button class="btn {BTN_COLOR} btn-kanban h-100" id="{MPENAME}" name="KANBAN" type="button" style="border-radius: 0.0rem;">' +
    '<div class="h-25" style="font-size: 0.9rem;font-weight:bold ">{MPENAME}</div>' +
    '<div class="h-25">&nbsp</div>' +
    '<div class="KANBAN_span_{MPENAME} h-50" style="font-size: 270%; margin-top:-10px; ">{STATUS}</div>' +
    '</button>' +
    '</div>';
/*
 this is for the dock door and container details.
 */
$(function () {


    //start connection 
    $.connection.hub.qs = { 'page_type': "MPEKanban".toUpperCase() };
    $.connection.hub.start({ waitForPageLoad: false })
        .done(function () {
            fotfmanager.server.getMPETestData("").done(function (testdata) {
                kanbandata = testdata;
                createKanbanRows(kanbandiv, kanbandata);
                console.log(testdata);
            });
            fotfmanager.server.getMPETestData("dispatch").done(function (testdata) {
                kanbandataDispatch = testdata;
                createKanbanRows(kanbandiv_dispatch, kanbandataDispatch);
                console.log(testdata);
            });
        }).catch(
            function (err) {
                console.log(err.toString());
            });
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
    });

});

function MPEKanbanInit() {
    //for (let i = 0; i < numberofboxes; i++) {
    //    kanbandiv.append(layout_templet.supplant(formatKANBANlayout(jsObjectTemplate)));
    //}
}

function bindBTN() {
    $(".btn").off().on('click', function () {
        let origBG = $(this).css("background-color");
        console.log(origBG);
        $(this).animate({
            opacity: 0.25
        }, 300);
        $(this).animate({
            opacity: 1
        }, 300);
        $(this).animate({
            opacity: 0.25
        }, 300);
        $(this).animate({
            opacity: 1
        }, 300);
    });
}

function createKanbanRows(divparent, data) {
    var colcount = 0;
    var collimit = 6;
    var rowid = 1;
    let divplaceholder;
    $.each(data["0"], function (key, value) {
        if (colcount == 0) {
            divplaceholder = $('<div/>', { class: "row h-50", id: ("kanbanrow_" + rowid) })
            //create new div, append to divkanban and assign to variable
        }
        colcount++;
        console.log(value);
        divplaceholder.append(layout_templet.supplant(formatKANBANlayout(value)));
        if (colcount > 5) {
            divparent.append(divplaceholder);
            colcount = 0;
        }
    })
    if (colcount != 0) {
        divparent.append(divplaceholder);
    }
    bindBTN();
}

function formatKANBANlayout(conn_status) {
    return $.extend(conn_status, {
        MPENAME: conn_status.MPENAME,
        STATUS: conn_status.STATUS,
        BTN_COLOR: Get_Kanban_Color(conn_status.STATUS)
    });
}
function Get_Kanban_Color(data) {
    value = "";
    const Number = $.isNumeric(data) ? parseInt(data) : data;
    if (typeof Number === 'number') {
        if (Number > 0) {
            value = "btn-success";
        }
        else {
            value = "btn-danger";
        }
    }
    else {
        if (/M$/i.test(data)) {
            value = "btn-primaryblue";
        } else if (/V$/i.test(data)) {
            value = "btn-warning";
        } else if (/D$/i.test(data)) {
            value = "btn-danger";
        } else {
            value = "btn-secondary";
        }
    }
    return value;
}

