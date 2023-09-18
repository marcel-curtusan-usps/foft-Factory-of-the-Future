/* use this for locater data */
$.extend(fotfmanager.client, {
    addMarker: async (data, id) => { Promise.all([AddMarker(data, id)]); },
    removeMarker: async (data, id) => { Promise.all([RemoveMarker(data, id)]); }
});
async function init_locators(marker, id) {
    $.each(marker, function (_index, data) {
        Promise.all([AddMarker(data, data.properties.floorId)]);
    });
    fotfmanager.server.joinGroup("CameraMarkers");
}
async function AddMarker(data, floorId) {
    try {
        if (floorId === baselayerid) {
       
            if (/^(Camera|CameraMarker)/i.test(data.properties.Tag_Type)) {
                cameras.addData(data);
            }
            if (data.geometry.coordinates.length > 0) {
                locatorMarker.addData(data);
            }
        }
    } catch (e) {
        console.log(e);
    }
}
async function RemoveMarker(data, floorId) {
    try {
        if (floorId === baselayerid) {
            map.eachLayer(function (layer) {
                if (layer.markerId === data.properties.id) {
                    map.removeLayer(layer);
                }
            });
        }
    } catch (e) {
        console.log(e);
    }
}
let locatorMarker = new L.GeoJSON(null, {
    pointToLayer: function (feature, latlng) {
        let locaterIcon = L.divIcon({
            id: feature.properties.id,
            className: 'bi-broadcast'
            
        });
        return L.marker(latlng, {
            icon: locaterIcon,
            title: feature.properties.name,
            riseOnHover: true,
            bubblingMouseEvents: true,
            popupOpen: true
        })
    },
    onEachFeature: function (feature, layer) {
        layer.markerId = feature.properties.id;
        layer.bindTooltip(feature.properties.name, {
            permanent: false,
            interactive: true,
            direction: 'top',
            opacity: 0.9
        }).openTooltip();
    }
})