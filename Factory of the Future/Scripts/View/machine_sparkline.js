
function updateAllMachineSparklinesDone(machineStatuses) {
    for (const machineStatus of machineStatuses) {
        updateSparklineCheck(machineStatus);


    }
    var forceUpdate = false;

    if (firstMachineSparklines) {
        forceUpdate = true;
    }
    checkSparklineVisibility(forceUpdate);
    let sparklinesChecked = $("#MPESparklines").prop("checked");
    if (firstMachineSparklines && sparklinesChecked) {

        updateSparklineTooltipDirection();
    }
    let mpeZoneDataChecked = $("#MPEWorkAreas").prop("checked");
    if (firstMPEZoneData && mpeZoneDataChecked) {

        updateMPEZoneTooltipDirection();
    }
    if (sparklinesChecked) {

        firstMachineSparklines = false;
    }
}
function updateSparklineCheck(machineStatus) {

    var sortPlan =
        machineStatus.Item1.properties.MPEWatchData.cur_sortplan;

    if (sortPlan != "") {
        updateMachineSparkline(machineStatus.Item1, machineStatus.Item2);
    }
}

function shouldUpdateSparkline(lastZoom, zoom, forceUpdate) {
    if (forceUpdate) return true;

    if (lastZoom > zoom && zoom < sparklineMinZoom) {
        return true;
    }

    if (zoom > lastZoom && lastZoom < sparklineMinZoom) {
        return true;
    }
    return false;
}
function checkSparklineVisibility(forceUpdate) {
    var zoom = map.getZoom();
    if (shouldUpdateSparkline(lastMapZoom, zoom, forceUpdate)) {

        var machineSparklineKeys = Object.keys(machineSparklines._layers);


        if (machineSparklineKeys.length == 0 ||
            !$("#MPESparklines").prop("checked")) {
            $("#sparkline-message").hide();

        }
        else
            if (zoom < sparklineMinZoom) {
                if (machineSparklineKeys.length > 0) {
                    $("#sparkline-message").show();
                }
                $.map(
                    machineSparklines._layers,
                    function (layer, i) {

                        layer.unbindTooltip();
                    });
            }
            else {

                $("#sparkline-message").hide();

                $.map(
                    machineSparklines._layers,
                    function (layer, i) {

                        updateMachineSparklineTooltip(layer.feature, layer);

                    });
            }


    }
    lastMapZoom = zoom;
}
// sets the sparkline graph cache so when it is ready to display,
// no graphs need to be created
// allow some time period in between function calls so that tags can continue to move
var firstMPEZoneData = true;
var firstMachineSparklines = true;

async function waitWithoutBlocking(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}
async function updateAllMachineSparklines(machineStatuses, i) {
    if (i === machineStatuses.length) {

        updateAllMachineSparklinesDone(machineStatuses);
    }
    else {
        GetSparklineGraph(machineStatuses[i].Item1.properties,
            machineStatuses[i].Item1.properties.id);
        waitWithoutBlocking(100).then(() => {
            updateAllMachineSparklines(machineStatuses, i + 1)
        });
    }
}


function getHourlyArrayMPESparkline(hourly_data, n) {
     let hourlyArray = JSON.parse(JSON.stringify(hourly_data));
    hourlyArray.splice(n + 1);
    hourlyArray.reverse();
    hourlyArray.pop();
    let currentColor = "";
    let latestHourIndex = hourlyArray.length - 1;
    let previousHourIndex = latestHourIndex - 1;
    let hourlyData = [];
    for (const obj of hourlyArray) {
        hourlyData.push(obj.count);
    }
    if ((hourlyArray[latestHourIndex].count < hourlyArray[previousHourIndex].count)
        || hourlyArray[latestHourIndex].count === 0) {
        currentColor = "rgba(231, 25, 33, 1)";
    }
    else {
        currentColor = "rgba(51, 51, 102, 1)";
    }
    return [hourlyData, currentColor];
}

function getLabelsMPESparkline(n) {
    let hourlyArray = [];
    for (var i = 0; i < n; i++) {
        hourlyArray.push("");
    }
    return hourlyArray;
}
var sparklineChart = null;
var sparklineCache = [];
function clearSparklineCache() {
    let date_time_old = Date.now() - (1000 * 60 * 61);
    for (var i = sparklineCache.length - 1; i >= 0; i--) {
        if (sparklineCache[i].date_time < date_time_old) {
            sparklineCache.splice(i, 1);
        }
    }
}
function checkSparklineCache(hourly_data) {
    for (var spCache of sparklineCache) {
        if (JSON.stringify(spCache.hourly_data) ==
            JSON.stringify(hourly_data)) {
            return spCache.dataURL;
        }
    }
    return null;
}

function addSparklineCache(hourly_data, dataURL) {
    for (var i = 0; i < sparklineCache; i++) {
        if (JSON.stringify(sparklineCache[i].hourly_data) ==
            JSON.stringify(hourly_data)) {
            return false;
        }
    }
    sparklineCache.push({
        hourly_data: hourly_data, dataURL:
            dataURL
    });
    return true;
}
let sparklineLength = 8;
function GetSparklineGraph(dataproperties, id) {
    var total = dataproperties.MPEWatchData.hourly_data.length;
    
    if (total > 0) {
        let hourlyArrays = getHourlyArrayMPESparkline(dataproperties.MPEWatchData.hourly_data, sparklineLength);
      
        let labels = getLabelsMPESparkline(sparklineLength);
        let hourlyArrayThis = hourlyArrays[0];
        let currentColor = hourlyArrays[1];
        let cacheCheck = checkSparklineCache(dataproperties.MPEWatchData.hourly_data);
        if (cacheCheck) {
            return cacheCheck;
        }
        if (sparklineChart === null) {


            sparklineChart = new Chart("sparkline-canvas", {
                type: 'line',
                data:

                 {
                    labels: labels,


                    datasets: [


                {
                    label: '',
                    data: hourlyArrayThis,
                    backgroundColor:
                        "rgba(0, 0, 0, 0)"
                    ,
                    borderColor:
                        currentColor
                    ,
                    borderWidth: 50
                }
                    ]
                    
                },
                
                
                options: {
                    legend: {
                        display: false
                    },
                    animation: {
                        duration: 0
                    },
                    scales: {
                        y: {
                            beginAtZero: true
                        }
                    }
                }
            });
        }
        else {
            sparklineChart.data.datasets[0].data = hourlyArrayThis;
            sparklineChart.data.datasets[0].borderColor = currentColor;
            sparklineChart.update();
        }

        let b64 = sparklineChart.toBase64Image();
        addSparklineCache(dataproperties.MPEWatchData.hourly_data, b64);
        return true;
    }
    else {
        return false;
    }
}

var machineSparklines = new L.GeoJSON(null, polyObj);

const sparklineWidthNormal = 40;
const sparklineHeightNormal = 20;

async function updateMachineSparklineTooltip(feature, layer) {
    let sparklineWidth = sparklineWidthNormal;
    let sparklineHeight = sparklineHeightNormal;
    let sparklineClass = 'leaflet-tooltip-sparkline';
    
    let imgUrl = checkSparklineCache(feature.properties.MPEWatchData.hourly_data);
   
    var htmlData = ((imgUrl === null) ? "<div></div>" : "<img src='" + imgUrl +
        "' width'" + sparklineWidth +
        "' height='" + sparklineHeight +
        "' style='width: " + sparklineWidth + "px; height: " + sparklineHeight +
        "px; ' />")
        ;
    if (layer._tooltip) {
        try {
            layer.unbindTooltip();
        }
        catch (e_) {

        }
    }
        layer.bindTooltip(htmlData, {
            permanent: true,
            interactive: true,
            direction: getSparklineTooltipDirection(),
            className: sparklineClass
        }).openTooltip();
    
    return true;
}


function layerMachineIdMatch(layer, machineupdate) {
    if (layer.feature.properties.id === machineupdate.properties.id) {
        
        return true;
    }
    return false;
}

let lastSparklineUpdate = 0;


function convertToSparkline(machineSparklineString) {
    let machineSparklinesNew = JSON.parse(machineSparklineString);

    for (var tuple of machineSparklinesNew) {
        tuple.Item1.properties.Zone_Type = "Sparkline";
        tuple.Item1.properties.sparkline = true;
        tuple.Item1.properties.color = "transparent";
        tuple.Item1.properties.fillColor = "transparent";
        tuple.Item1.properties.opacity = 0;
        tuple.Item1.properties.fillOpacity = 0;
        tuple.Item1.properties.id = tuple.Item1.properties.id + "-sp";
    }
    return machineSparklinesNew;

}

async function updateMachineSparkline(machineupdate, id) {
    if (id == baselayerid) {
            if (machineupdate.properties.hasOwnProperty("MPEWatchData")) {
                let foundLayer = null;



                var machineSparklineKeys = Object.keys(machineSparklines._layers);
               
                for (var key of machineSparklineKeys) {
                    let layer = machineSparklines._layers[key];
                    if (layer.hasOwnProperty("options")) {
                        layer.options.fillColor = "transparent";
                        layer.options.color = "transparent";
                        layer.options.fillOpacity = 0;
                        layer.options.opacity = 0;
                        if (layerMachineIdMatch(layer, machineupdate))
                        {
                           
                            foundLayer = layer;
                        }

                    }
                }

                if (foundLayer) {
                    updateMachineSparklineTooltip(foundLayer.feature,
                        foundLayer);
                }
                else {
                    machineSparklines.addData(machineupdate);

                }
            }
        }
    
}