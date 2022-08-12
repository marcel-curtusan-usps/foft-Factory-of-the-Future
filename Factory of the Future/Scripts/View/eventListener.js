
const sidebarcloseevent = new Event("sidebarclose", {
    bubbles: false
});

const layerscontentvisibleevent = new Event("layerscontentvisible", {
    bubbles: false
});
const eventIntervalFrequency = 250;
var customListenerElements = [];




const sidebarElCollection = [];

// custom listeners add documents individually before adding an event listener,
// for performance reasons (don't want to take up more processing/memory than necessary 
// even though it is a small amount in the first place anyway)
function addToSidebarListenCollection(el) {
    sidebarElCollection.push({ lastCollapsed: null, element: el });
}

var listenElObjCollapsedSetToTrue = false;
function listenForLayersContentVisible() {
    if (greyedOut && $("#layersContent").is(":visible")) {
        document.dispatchEvent(layerscontentvisibleevent);
    }
}
function listenForSidebarClose() {
    for (var elObj of sidebarElCollection) {
        listenElObjCollapsedSetToTrue = false;
        for (const cl of elObj.element.classList) {
            if (cl === "collapsed") {
                if (elObj.lastCollapsed === false) {

                    elObj.lastCollapsed = true;
                    listenElObjCollapsedSetToTrue = true;
                    elObj.element.dispatchEvent(sidebarcloseevent);
                    break;
                }
                elObj.lastCollapsed = true;
                listenElObjCollapsedSetToTrue = true;


            }
        }
        
        if (!listenElObjCollapsedSetToTrue ) {
            elObj.lastCollapsed = false;
        }
    }
}
let listenFunctionRunning = false;
function periodicListen() {
    if (!listenFunctionRunning) {
        listenFunctionRunning = true;
        try {
            listenForSidebarClose();
            listenForLayersContentVisible();
        }
        catch (e) {
            console.log(e.message + " " + e.stack);
        }
        listenFunctionRunning = false;
    }
   
    
}


setInterval(periodicListen, eventIntervalFrequency)