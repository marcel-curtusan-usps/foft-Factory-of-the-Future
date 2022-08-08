
var initialCheckedLayersKeys = ['mainfloor', 'polygonMachine',
    'piv_vehicles', 'agv_vehicles', 'agvLocations', 'container',
    'stagingAreas', 'tagsMarkersGroup', 'dockDoors', 'binzonepoly'];
var checkedLayersKeys = [];
var fotfLocal = new PouchDB("fotfLocal");
function getLayersFromKeys(layerKeys) {
    let layers = [];
    for (const str of layerKeys) {
        // gets the variable associated with the string
        layers.push(window[str]);
    }
    return layers;
}
var checkedLayers = [];
var initLayersDone = false;
function storeSelected() {
    let m = map;
    let r = b;
}

function updateMapLayersAll(theseLayers) {
    map.eachLayer(function (layerInMap) {
        var found = false;
        for (const layer of theseLayers) {
            if (layer === layerInMap) {
                found = true;
            }
        }
        if (!found) {
            map.removeLayer(layerInMap);
        }
    });
    for (const layer of theseLayers) {
        if (!map.hasLayer(layer)) {
            map.addLayer(layer);
        }
    }
}
async function initLayerKeys() {

    try {

        var doc = await fotfLocal.get('layerData');
        checkedLayersKeys = JSON.parse(doc.layerString);

        if (map) {
            updateMapLayersAll(getLayersFromKeys(checkedLayersKeys))
           
        }
        var abc = 0;
    }
    catch (e) {
        let layerStr = JSON.stringify(initialCheckedLayersKeys);
        try {
            await fotfLocal.put({
                _id: 'layerData',
                layerString: layerStr
            });

            if (map) {
                updateMapLayersAll(getLayersFromKeys(initialCheckedLayersKeys));
            }
        }
        catch (e2) {
            console.log(e2.message);
        }
        checkedLayersKeys = initialCheckedLayersKeys;
        if (map) {
            updateMapLayersAll(getLayersFromKeys(checkedLayersKeys));
        }
    }
    initLayersDone = true;
}



initLayerKeys();
