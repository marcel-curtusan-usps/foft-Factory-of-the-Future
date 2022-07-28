﻿


function getHourlyArrayMPESparkline(hourly_data, n) {
     let hourlyArray = JSON.parse(JSON.stringify(hourly_data));
    hourlyArray.splice(n);
    hourlyArray.reverse();
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
    for (var i = 0; i < sparklineCache.length; i++) {
        if (JSON.stringify(sparklineCache[i].hourly_data) ==
            JSON.stringify(hourly_data)) {
            return sparklineCache[i].dataURL;
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
        // let hourlyArrayPrevious12 = hourlyArrays[0];
        // let hourlyArrayThis12 = hourlyArrays[1];
        // let currentColor = hourlyArrays[2];
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

                        /*{
                        label: '',
                        data: hourlyArrayPrevious12,
                        backgroundColor:
                            "rgba(0, 0, 0, 0)"
                        ,
                        borderColor:
                            "rgba(0, 0, 0, .6)"
                        ,
                        borderWidth: 50
                    },


                        {
                            label: '',
                            data: hourlyArrayThis12,
                            backgroundColor:
                                "rgba(0, 0, 0, 0)"
                            ,
                            borderColor:
                                currentColor
                            ,
                            borderWidth: 50
                        }

*/

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


       // var canvas = document.getElementById("sparkline-canvas");
        
        let dataURLFound = GetSparklineGraph(dataproperties);
        resolve(dataURLFound);
        /*
        let ctx = canvas.getContext("2d");
        ctx.fillStyle = "white";
        ctx.fillRect(0, 0, 50, 25);
        var img = new Image();
        img.onload = function () {

            ctx.drawImage(img, 0, 0);
            resolve(canvas.toDataURL());
        }
        img.onerror = function () {
            reject("could not load image");
        }
        img.src = dataURL;

*/
        //var hourlyArray = getHourlyArray(hourly_data);
        //const labels = Utils.months({ count: 7 });
        /*
        const myChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: ['Red', 'Blue', 'Yellow', 'Green', 'Purple', 'Orange'],
                datasets: [{
                    label: '# of Votes',
                    data: [12, 19, 3, 5, 2, 3],
                    backgroundColor: [
                        'rgba(255, 99, 132, 0.2)',
                        'rgba(54, 162, 235, 0.2)',
                        'rgba(255, 206, 86, 0.2)',
                        'rgba(75, 192, 192, 0.2)',
                        'rgba(153, 102, 255, 0.2)',
                        'rgba(255, 159, 64, 0.2)'
                    ],
                    borderColor: [
                        'rgba(255, 99, 132, 1)',
                        'rgba(54, 162, 235, 1)',
                        'rgba(255, 206, 86, 1)',
                        'rgba(75, 192, 192, 1)',
                        'rgba(153, 102, 255, 1)',
                        'rgba(255, 159, 64, 1)'
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
        */

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
var machineSparklines = new L.GeoJSON();

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
async  function getMachineSparkline(machineupdate) {
    let latlngArray = getSparklineCoords(machineupdate.geometry.coordinates[0]);
    let latlng = L.latLng(latlngArray[0], latlngArray[1]);
   // machineupdate.geometry.coordinates

    var imgUrlFound =
        await createSparkline(machineupdate.properties,
            machineupdate.properties.id);


 

    let imgUrl = null;
    if (imgUrlFound) {
         imgUrl = checkSparklineCache(machineupdate.properties.MPEWatchData.hourly_data);
    }
    var locaterIcon = L.divIcon({
        html: ((imgUrl === null) ? "<div></div>" : "<img src='" + imgUrl +
            "' width'50' height='25' style='margin-left: 40px; margin-top: -14px; width: 50px; height: 25px; position: relative' />"),
        iconSize: [0, 0]
    });
    let marker = L.marker(latlng, {
        hourly_data: machineupdate.properties.MPEWatchData.hourly_data,
        id: machineupdate.properties.id,
        icon: locaterIcon,
        title: machineupdate.properties.name,
        riseOnHover: true,
        riseOffset: 500,
        bubblingMouseEvents: true,
        popupOpen: true
    });
    marker.on('click', (e) => { onSparklineClick(e); });
    return marker;

}

function hideOrShowSparkline() {

}

function layerMachineUpdateCheck(layer, machineupdate) {
    if (layer.options.id === machineupdate.properties.id) {
        
        let hourlyData = JSON.stringify(layer.options.hourly_data);
        let hourlyData2 = JSON.stringify(machineupdate.properties.MPEWatchData.hourly_data);
        if (hourlyData !== hourlyData2) {
            return "update";
        }
        return "found";
    }
    return null;
}

let lastSparklineUpdate = 0;
async function updateMachineSparkline(machineupdate, id) {
    try {

        if (id == baselayerid) {

            if (machineupdate.properties.hasOwnProperty("MPEWatchData")) {
                var found = false;
                let layerIndex = -0;
                $.map(machineSparklines._layers, async function (layer, i) {

                    if (layer.hasOwnProperty("options")) {
                        let layerMachineUpdateVal =
                            layerMachineUpdateCheck(layer, machineupdate);
                        if (layerMachineUpdateVal === "update")
                        {
                            layerIndex = i;
                            found = true;
                        }
                        if (layerMachineUpdateVal === "found") {
                            found = true;
                        }

                    }
                });

                if (layerIndex !== -0 && checkSparklineCache(machineupdate.properties.MPEWatchData.hourly_data)) {
                    let sparklineReplace = await getMachineSparkline(machineupdate);
                    machineSparklines.removeLayer(machineSparklines._layers[layerIndex]);
                    machineSparklines.addLayer(sparklineReplace);
                }
                if (!found) {
                    let sparkline = await getMachineSparkline(machineupdate);
                    machineSparklines.addLayer(sparkline);
                }
            }
        }
    }
    catch (e) {
        console.log(e.message + ", " + e.stack);
    }
}