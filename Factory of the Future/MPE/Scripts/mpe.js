
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
let connecttimer = 0;
let connectattp = 0;
let MPEName = "";
let fotfmanager = $.connection.FOTFManager;
$.extend(fotfmanager.client, {
    updateMPEStatus: (updatedata) => { Promise.all([updateMPEStatus(updatedata)]) }
});
async function updateMPEStatus(data)
{
    try {
        if (!!data) {
            $('label[id=mpeName]').text(data.MpeId);
            $('label[id=opn]').text(data.cur_operation_id);
            $('label[id=sortplan_name_text]').text(data.cur_sortplan);
        }
    } catch (e) {
        console.log(e);
    }
}
async function StartDataConnection() {
    // Start the connection
    createMPEDataTable("mpeStatustable");

}
async function LoadData()
{
    console.log("Connected time: " + new Date($.now()));
    fotfmanager.server.getMPEStatusList(MPEName).done(async (data) => { Promise.all([updateMPEStatus(data[0])]) });
    fotfmanager.server.joinGroup("MPE_" + MPEName);
}
function createMPEDataTable(table) {
    let arrayColums = [{
        "":"",
        "Planned": "",
        "Actual": ""
    }]
    let columns = [];
    let tempc = {};
    $.each(arrayColums[0], function (key, value) {
        tempc = {};
        if (/Planned/i.test(key)) {
            tempc = {
                "title": 'Planned',
                "mDataProp": key,
                "class": "row-cts-count text-center"
            }
        }
        else if (/Actual/i.test(key)) {
            tempc = {
                "title": "Actual",
                "mDataProp": key,
                "class": "row-cts-des text-center"
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
        fnInitComplete: function () {
            if ($(this).find('tbody tr').length <= 1) {
                $('.odd').hide()
            }
        },
        dom: 'Bfrtip',
        bFilter: false,
        bdeferRender: true,
        paging: false,
        bPaginate: false,
        bAutoWidth: true,
        bInfo: false,
        destroy: true,
        aoColumns: columns,
        columnDefs: [{
            visible: false,
            targets: 0,
        }]
    });
}
function loadDataTable(data, table) {
    if ($.fn.dataTable.isDataTable("#" + table)) {
        if (!$.isEmptyObject(data)) {
            $('#' + table).DataTable().rows.add(data).draw();
        }
    }
}
function removeLegsTripDataTable(table) {
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows(function (idx, data, node) {
            $('#' + table).DataTable().row(node).remove().draw();
        })
    }
}
$(function () {
    setHeight();
    document.title = $.urlParam("mpeStatus");
    if (MPEName !== "") {
        Promise.all([StartDataConnection()]);
        $('label[id=mpeName]').text(MPEName);
    }

    //start connection 
    $.connection.hub.qs = { 'page_type': "MPE".toUpperCase() };
    $.connection.hub.start({ waitForPageLoad: false })
        .done(() => { Promise.all([LoadData()]) }).catch(
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
            $.connection.hub.start({ waitForPageLoad: false })
                .done(() => { Promise.all([LoadData()]) })
                .catch(function (err) {
                console.log(err.toString());
            });
        }, 10000); // Restart connection after 10 seconds.
        // fotfmanager.server.leaveGroup("DockDoor_" + doornumber);
    });
    //Raised when the underlying transport has reconnected.
    $.connection.hub.reconnecting(function () {
        clearTimeout(connecttimer);
        console.log("reconnected at time: " + new Date($.now()));
        fotfmanager.server.joinGroup("MPE_" + MPEName);
    });
});
$.urlParam = function (name) {
    let results = new RegExp('[\?&]' + name + '=([^&#]*)', 'i').exec(window.location.search);
    MPEName = (results !== null) ? results[1] || 0 : "";
    return MPEName;
}
function setHeight() {
    let height = (this.window.innerHeight > 0 ? this.window.innerHeight : this.screen.height) - 1;
    let screenTop = (this.window.screenTop > 0 ? this.window.screenTop : 1) - 1;
    let pageBottom = (height - screenTop);
    $("div.card").css("min-height", pageBottom + "px");
}
function capitalize_Words(str) {
    return str.replace(/\w\S*/g, function (txt) {
        return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
    });
}