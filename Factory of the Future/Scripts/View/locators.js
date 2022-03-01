/* use this for locater data */

async function init_locators() {
    //Get view ports list
    fotfmanager.server.getLocatorsList().done(function (Data) {
        if (Data.length > 0) {
            locatorMarker.addData(Data);
        }
    })
}

var locatorMarker = new L.GeoJSON(null, {
    pointToLayer: function (feature, latlng) {
        var locaterIcon = L.divIcon({
            id: feature.properties.id,
            className: 'bi-broadcast',
            
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
        layer.bindTooltip(feature.properties.name, {
            permanent: false,
            direction: 'top',
            opacity: 0.9
        }).openTooltip();
    }
})