/* use this for locater data */

async function init_locators(marker, id) {
    fotfmanager.server.joinGroup("VehiclsMarkers");
    fotfmanager.server.joinGroup("CameraMarkers");
    $.each(marker, function () {
        if (this.properties.Tag_Type === "Vehicle") {
            piv_vehicles.addData(this);
        }
        else if (this.properties.Tag_Type === "Autonomous Vehicle") {
            agv_vehicles.addData(this);
        }
        else if (this.properties.Tag_Type === "Camera") {
            cameras.addData(this);
          
        }
        else {
            locatorMarker.addData(this)
        }
    });
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
            interactive: true,
            direction: 'top',
            opacity: 0.9
        }).openTooltip();
    }
})