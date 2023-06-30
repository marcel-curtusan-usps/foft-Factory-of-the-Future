/*
this is for the person details.
 */
$.extend(fotfmanager.client, {
    updateHVITagStatus: async (tagupdate, id) => { updatePersonTag(tagupdate, id) },
    updateMarkerCoordinates: async (coordinates, floorid, markerid) => { Promise.all([MarkerCoordinates(coordinates, floorid, markerid)]) }
});
let hvi_tags = new L.GeoJSON(null, {
    pointToLayer: function (feature, latlng) {
        return new L.circleMarker(latlng, {
            radius: 8,
            opacity: 0,
            fillOpacity: 0,
            className: 'marker-person'
        })
    },
    onEachFeature: function (feature, layer) {
        layer.markerId = feature.properties.id;
        let VisiblefillOpacity = feature.properties.tagVisibleMils < 80000 ? "" : "tooltip-hidden";
        let classname = 'persontag ' + VisiblefillOpacity;
    

        layer.bindTooltip("", {
            permanent: true,
            interactive: true,
            direction: 'center',
            opacity: 1,
            //className: 'persontag ' + VisiblefillOpacity
            className: classname
        }).openTooltip();
    },
    //filter: function (feature, layer) {
    filter: function (feature) {
        return feature.properties.tagVisible;
    }
})