/*
this is for the person details.
 */
$.extend(fotfmanager.client, {
    updatePersonTagStatus: async (tagupdate) => { updatePersonTag(tagupdate) }
});
async function updatePersonTag(tagpositionupdate) {
    try {
        if (tagsMarkersGroup.hasOwnProperty("_layers")) {
            var layerindex = -0;
            $.map(tagsMarkersGroup._layers, function (layer) {
                if (layer.hasOwnProperty("feature")) {
                    if (layer.feature.properties.id === tagpositionupdate.properties.id) {
                        layer.feature.properties = tagpositionupdate.properties;
                        layer.feature.geometry = tagpositionupdate.geometry.coordinates;
                        layerindex = layer._leaflet_id;
                         Promise.all([updateTagLocation(layerindex)]);

                        
                        return false;
                    }
                }
            });
            if (layerindex === -0) {
                tagsMarkersGroup.addData(tagpositionupdate);
            }
        }

    } catch (e) {
        console.log(e);
    }
}
var tagsMarkersGroup = new L.GeoJSON(null, {
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
    },
    filter: function (feature, layer) {
        return feature.properties.tagVisible;
    }
})
async function updateTagLocation(layerindex)
{
    try {
                //circleMarker._layers[layerindex].feature
        if (tagsMarkersGroup._layers[layerindex].feature.properties.tagVisibleMils > 80000) {
            if (tagsMarkersGroup._layers[layerindex].hasOwnProperty("_tooltip") && tagsMarkersGroup._layers[layerindex]._tooltip.hasOwnProperty("_container") &&
                !tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('tooltip-hidden')) {

                tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.add('tooltip-hidden');

            }
        }
        if (tagsMarkersGroup._layers[layerindex].feature.properties.tagVisibleMils < 80000) {
            //if the distance from the current location is more then 10000 meters the do not so the slide to
            var newLatLng = new L.latLng(tagsMarkersGroup._layers[layerindex].feature.geometry[1], tagsMarkersGroup._layers[layerindex].feature.geometry[0]);
       
            tagsMarkersGroup._layers[layerindex].slideTo(newLatLng, { duration: 2000 });
            
            if (tagsMarkersGroup._layers[layerindex].hasOwnProperty("_tooltip") && tagsMarkersGroup._layers[layerindex]._tooltip.hasOwnProperty("_container") &&
                tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('tooltip-hidden')) {
                tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.remove('tooltip-hidden');

            }
        }
    } catch (e) {
        console.log(e);
    }
    return null;
}
