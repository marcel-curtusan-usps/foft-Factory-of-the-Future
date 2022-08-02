


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

async function  createSparkline(dataproperties, id) {
    return new Promise((resolve, reject) => {


        let dataURLFound = GetSparklineGraph(dataproperties);
        resolve(dataURLFound);
       

    });
}

const onSparklineClick = (e) => {

    $.map(polygonMachine._layers, function (pmObj, i) {

        if (pmObj.findId === e.target.options.id) {

            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            try {
                map.setView(pmObj.getCenter(), 3);
            }
            catch (e_) {

                map.setView(e.target._latlng);
            }
            if ((' ' + document.getElementById('sidebar').className + ' ').indexOf(' ' + 'collapsed' + ' ') <= -1) {
                if ($('#zoneselect').val() == pmObj.findId) {
                    sidebar.close('home');
                }
                else {
                    sidebar.open('home');
                }
            }
            else {
                sidebar.open('home');
            }
            LoadMachineTables(pmObj.feature.properties, 'machinetable');
        }

    });
};
var machineSparklines = new L.GeoJSON(null, polyObj);

function getSparklineCoords(coordinates) {
    var minLat = 10000000000000;
    var maxLat = -10000000000000;
    var minLng = 10000000000000;
    var maxLng = -10000000000000;
    for (const c of coordinates) {
        if (c[1] < minLat) minLat = c[1];
        if (c[1] > maxLat) maxLat = c[1];
        if (c[0] < minLng) minLng = c[0];
        if (c[0] > maxLng) maxLng = c[0];

    }
    let centerLat = (minLat + maxLat) / 2;
    let centerLng = (minLng + maxLng) / 2;
    return [centerLat, centerLng];
}

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
   // let tt = new tooltip()
    if (layer._tooltip) {
        layer.unbindTooltip();

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
    if (layer.findId === machineupdate.properties.id) {
        
        return true;
    }
    return false;
}

let lastSparklineUpdate = 0;

async function getSparklineLayerFromId(id) {
    return new Promise((resolve, reject) => {

        $.map(machineSparklines._layers, function (layer, i) {

            if (layer.hasOwnProperty("feature") && layer.feature.properties.id == id) {
                resolve(layer);
            }
        });
            resolve(null);
    });
}
async function updateMachineSparkline(machineupdate, id) {
    machineupdate.properties.transparent = true;
    machineupdate.properties.sparkline = true;
        if (id == baselayerid) {

            if (machineupdate.properties.hasOwnProperty("MPEWatchData")) {
                let foundLayer = null;



                var machineSparklineKeys = Object.keys(machineSparklines._layers);
               
                for (var key of machineSparklineKeys) {
                    let layer = machineSparklines._layers[key];
                    if (layer.hasOwnProperty("options")) {
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