/*
this is for the person details.
 */
$.extend(fotfmanager.client, {
    updatePersonTagStatus: async (tagupdate, id) => { updatePersonTag(tagupdate, id) },
    updateMarkerCoordinates: async (coordinates, floorid, markerid) => { Promise.all([MarkerCoordinates(coordinates, floorid,markerid)]) }
});

async function MarkerCoordinates(Coordinates, flid, mkid)
{
    try {
        if (flid === baselayerid) {
            map.whenReady(() => {
                if (map.hasOwnProperty("_layers")) {
                    $.map(map._layers, function (layer, i) {
                        if (layer.hasOwnProperty("markerId") && layer.markerId === mkid) {
                            layer.feature.geometry = Coordinates
                            Promise.all([MarkerCoordinatesUpdate(layer)]);
                            return false;
                        }
                    });
                }
            });
        }
    } catch (e) {
        console.log();
    }
}
async function MarkerCoordinatesUpdate(layer)
{
    try {
        let newLatLng = new L.latLng(layer.feature.geometry.coordinates[1], layer.feature.geometry.coordinates[0]);
        layer.slideTo(newLatLng, { duration: 2000 });
        return false;
    } catch (e) {
        console.log();
    }
};
async function updatePersonTag(tagpositionupdate,id) {
    try {
        if (id == baselayerid) {
            let layerindex = -0;

            map.whenReady(() => {
                if (map.hasOwnProperty("_layers")) {
                    $.map(map._layers, function (layer, i) {
                        if (layer.hasOwnProperty("feature")) {
                            if (layer.feature.properties.id === tagpositionupdate.properties.id) {
                                layerindex = layer._leaflet_id;
                                layer.feature.geometry = tagpositionupdate.geometry.coordinates;
                                layer.feature.properties = tagpositionupdate.properties;
                                Promise.all([updateTagLocation(layerindex)]);
                                return false;
                            }
                        }
                    });
                }
            });
            if (layerindex === -0) {
                tagsMarkersGroup.addData(tagpositionupdate);
            }
            //if (tagsMarkersGroup.hasOwnProperty("_layers")) {
            //    $.map(tagsMarkersGroup._layers, function (layer) {

            //        if (layer.hasOwnProperty("feature")) {
            //            if (layer.feature.properties.id === tagpositionupdate.properties.id) {
            //                layer.feature.properties = tagpositionupdate.properties;
            //                layer.feature.geometry = tagpositionupdate.geometry.coordinates;
            //                layerindex = layer._leaflet_id;

            //                if (layer.feature.properties.tacs != null) {
            //                    if (tagpositionupdate.properties.tacs != null) {
            //                        layer.feature.properties.tacs.isOvertime = true;
            //                    }
            //                }


            //                Promise.all([updateTagLocation(layerindex)]);
                            

            //                return false;
            //            }
            //        }
            //    });
               
            //}
        }

    } catch (e) {
        console.log(e);
    }
}
let tagsMarkersGroup = new L.GeoJSON(null, {
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
        var VisiblefillOpacity = feature.properties.tagVisibleMils < 80000 ? "" : "tooltip-hidden";
        var isOT = false;
        var isOTAuth = false;
        var classname = 'persontag ' + VisiblefillOpacity;
        if (feature.properties.tacs != null) {
            isOT = feature.properties.tacs.isOvertime;
            isOTAuth = feature.properties.tacs.isOvertimeAuth
            if (isOT && isOTAuth) {
                classname = 'persontag_otAuth ' + VisiblefillOpacity;
            }
            else if (isOT && !isOTAuth) {
                classname = 'persontag_otNotAuth ' + VisiblefillOpacity;
            }
        }
        
        layer.bindTooltip("", {
            permanent: true,
            interactive: true,
            direction: 'center',
            opacity: 1,
            //className: 'persontag ' + VisiblefillOpacity
            className: classname
        }).openTooltip();
    },
    filter: function (feature, layer) {
        return feature.properties.tagVisible;
    }
})
async function updateTagLocation(layerindex)
{
    try {
        if (tagsMarkersGroup._layers[layerindex].feature.properties.tacs != null) {
            if (tagsMarkersGroup._layers[layerindex].feature.properties.tacs.isOvertime
                && tagsMarkersGroup._layers[layerindex].feature.properties.tacs.isOvertimeAuth
                && !tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('persontag_otAuth')) {
                if (tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('persontag')) { tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.remove('persontag'); }
                if (tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('persontag_otNotAuth')) { tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.remove('persontag_otNotAuth'); }
                tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.add('persontag_otAuth');
            }
            else if (tagsMarkersGroup._layers[layerindex].feature.properties.tacs.isOvertime
                && !tagsMarkersGroup._layers[layerindex].feature.properties.tacs.isOvertimeAuth
                && !tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('persontag_otNotAuth')) {
                if (tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('persontag')) { tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.remove('persontag'); }
                if (tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('persontag_otAuth')) { tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.remove('persontag_otAuth'); }
                tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.add('persontag_otNotAuth');
            }
            else if (!tagsMarkersGroup._layers[layerindex].feature.properties.tacs.isOvertime
                && !tagsMarkersGroup._layers[layerindex].feature.properties.tacs.isOvertimeAuth){
                if (tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('persontag_otNotAuth')) { tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.remove('persontag'); }
                if (tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('persontag_otAuth')) { tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.remove('persontag_otAuth'); }
                tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.add('persontag');
            }

        }
        else {
            if (!tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('persontag')) {
                if (tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('persontag_otNotAuth')) { tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.remove('persontag'); }
                if (tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('persontag_otAuth')) { tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.remove('persontag_otAuth'); }
                tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.add('persontag');
            }
        }
                //circleMarker._layers[layerindex].feature
        if (tagsMarkersGroup._layers[layerindex].feature.properties.tagVisibleMils > 80000) {
            if (tagsMarkersGroup._layers[layerindex].hasOwnProperty("_tooltip") && tagsMarkersGroup._layers[layerindex]._tooltip.hasOwnProperty("_container") &&
                !tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('tooltip-hidden')) {

                tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.add('tooltip-hidden');

            }
        }
        if (tagsMarkersGroup._layers[layerindex].feature.properties.tagVisibleMils < 80000) {
            //if the distance from the current location is more then 10000 meters the do not so the slide to
            let newLatLng = new L.latLng(tagsMarkersGroup._layers[layerindex].feature.geometry[1], tagsMarkersGroup._layers[layerindex].feature.geometry[0]);
       
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

async function getStaffInfo() {

}
