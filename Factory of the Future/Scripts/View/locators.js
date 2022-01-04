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
            className: 'pi-iconRepairZone ml--16 iconXSmall',
            html: '<i>' +
                '<span class="path1"></span>' +
                '<span class="path2"></span>' +
                '<span class="path3"></span>' +
                '<span class="path4"></span>' +
                '<span class="path5"></span>' +
                '<span class="path6"></span>' +
                '<span class="path7"></span>' +
                '<span class="path8"></span>' +
                '<span class="path9"></span>' +
                '<span class="path10"></span>' +
                '</i>'
        });
        return L.marker(latlng, {
            icon: locaterIcon,
            title: feature.properties.name,
            riseOnHover: true,
            bubblingMouseEvents: true,
            popupOpen: true
        })
    }
    
})