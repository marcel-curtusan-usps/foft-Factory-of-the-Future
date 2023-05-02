/*
this is for the person details.
 */
$.extend(fotfmanager.client, {
    updateHVITagStatus: async (tagupdate, id) => { updatePersonTag(tagupdate, id) },
    updateMarkerCoordinates: async (coordinates, floorid, markerid) => { Promise.all([MarkerCoordinates(coordinates, floorid, markerid)]) }
});
