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
/*
 this is for the dock door and container details.
 */
$.extend(fotfmanager.client, {
    updateDigitalDockDoorStatus: async (digitaldockdoorupdate) => { updateDockDoorStatus(digitaldockdoorupdate) }
});
function updateDockDoorStatus(data) {
    try {
        $('label[id=tripDirectionInd]').text(data.tripDirectionInd);
        if (data.tripDirectionInd === "I") {
            $('button[id=tripDirectionInd]').text("In-bound");
            $('button[id=tripDirectionInd]').removeClass("btn-light").removeClass("btn-success").addClass("btn-warning");
        }
        else if (data.tripDirectionInd === "O") {
            $('button[id=tripDirectionInd]').text("Out-Bound");
            $('button[id=tripDirectionInd]').removeClass("btn-light").removeClass("btn-warning").addClass("btn-success");
        }
        else {
            $('button[id=tripDirectionInd]').text("Not Trip");
            $('button[id=tripDirectionInd]').removeClass("btn-success").removeClass("btn-warning").addClass("btn-light");
        }
    } catch (e) {
        console.log(e);
    }
}
$(function () {
    setHeight();
    $('button[id=dockdoorNumber]').text($.urlParam("Dockdoor"));
    if (doornumber !== "") {
        StartDataConnection(doornumber);
       
    }
    // Raised when the connection state changes. Provides the old state and the new state (Connecting, Connected, Reconnecting, or Disconnected).
    $.connection.hub.stateChanged(function (state) {
        switch (state.newState) {
            case 1: $('button[id=dockdoorNumber]').removeClass("btn-danger").addClass("btn-dark");
                break;
            case 4:
                $('button[id=dockdoorNumber]').removeClass("btn-dark").addClass("btn-danger");
                break;
            default: $('button[id=dockdoorNumber]').removeClass("btn-danger").addClass("btn-dark");
        }
    });
    //handling Disconnect
    $.connection.hub.disconnected(function () {
        fotfmanager.server.leaveGroup("DockDoor_" + doornumber);
    });
    //Raised when the underlying transport has reconnected.
    $.connection.hub.reconnecting(function () { });
});
$.urlParam = function (name) {
    let results = new RegExp('[\?&]' + name + '=([^&#]*)')
        .exec(window.location.search);
    doornumber = (results !== null) ? results[1] || 0 : "No Door Selected";
    return doornumber;
}
function StartDataConnection(doorNum) {
    // Start the connection
    $.connection.hub.qs = { 'page_type': "DockDoor".toUpperCase() };
    $.connection.hub.start({ withCredentials: true, waitForPageLoad: false })
        .done(function () {
            fotfmanager.server.getDigitalDockDoorList(doorNum).done(async (DockDoordata) => { updateDockDoorStatus(DockDoordata) });
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
