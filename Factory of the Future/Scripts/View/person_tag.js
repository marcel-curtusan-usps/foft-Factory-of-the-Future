/*
this is for the person details.
 */
$.extend(fotfmanager.client, {
    updatePersonTagStatus: async (tagupdate) => { updatePersonTag(tagupdate) }
});
async function updatePersonTag(tagpositionupdate) {
    try {
        map.whenReady(() => {
            if (tagpositionupdate) {
                if (circleMarker.hasOwnProperty("_layers")) {
                    var layerindex = -0;
                    $.map(circleMarker._layers, function (layer) {
                        if (layer.hasOwnProperty("feature")) {
                            if (layer.feature.properties.id === tagpositionupdate.properties.id) {
                                layer.feature.properties = tagpositionupdate.properties;
                                layerindex = layer._leaflet_id;
                                if (layer.feature.properties.tagVisibleMils > 80000) {
                                    if (layer.hasOwnProperty("_tooltip")) {
                                        if (layer._tooltip.hasOwnProperty("_container")) {
                                            if (!layer._tooltip._container.classList.contains('tooltip-hidden')) {
                                                layer._tooltip._container.classList.add('tooltip-hidden');
                                            }
                                        }
                                    }
                                    //if (layer.options.opacity !== 0) {
                                    //    layer.setStyle({
                                    //        opacity: 0,
                                    //        fillOpacity: 0,
                                    //        fillColor: ''
                                    //    });
                                    //}
                                }
                                if (layer.feature.properties.tagVisibleMils < 80000) {
                                    //if the distance from the current location is more then 10000 meters the do not so the slide to
                                    var newLatLng = new L.latLng(tagpositionupdate.geometry.coordinates[1], tagpositionupdate.geometry.coordinates[0]);
                                    var ty = (newLatLng.distanceTo(layer.getLatLng()).toFixed(0) / 1000);
                                    var tu = Math.round(ty);
                                    if (tu > 4000) {
                                        layer.setLatLng(newLatLng);
                                    }
                                    else {
                                        layer.slideTo(newLatLng, { duration: 2000 });
                                    }
                                    if (layer.hasOwnProperty("_tooltip")) {
                                        if (layer._tooltip.hasOwnProperty("_container")) {
                                            if (layer._tooltip._container.classList.contains('tooltip-hidden')) {
                                                layer._tooltip._container.classList.remove('tooltip-hidden');
                                            }
                                        }
                                    }
                                    //if (layer.options.opacity !== 1) {
                                    //    layer.setStyle({
                                    //        opacity: 1,
                                    //        fillOpacity: 1
                                    //    });
                                    //}
                                }
                                layer.feature.geometry = tagpositionupdate.geometry.coordinates;
                                return false;
                            }
                        }
                    });
                    if (layerindex === -0) {
                        circleMarker.addData(tagpositionupdate);
                    }
                }
            }
            ////this is to find EMPID that not complaint.
        });
    } catch (e) {
        console.log(e);
    }
}
var circleMarker = new L.GeoJSON(null, {
    pointToLayer: function (feature, latlng) {
        return new L.circleMarker(latlng, {
            radius: 8,
            opacity: 0,
            fillOpacity: 0,
            className: 'marker-person'
        })
    },
    onEachFeature: function (feature, layer) {
        var VisiblefillOpacity = feature.properties.tagVisibleMils < 80000 ? "" : "tooltip-hidden";
        layer.bindTooltip("", {
            permanent: true,
            direction: 'center',
            opacity: 1,
            className: 'persontag ' + VisiblefillOpacity
        }).openTooltip();
    }
})