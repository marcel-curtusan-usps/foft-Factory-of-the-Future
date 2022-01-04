/*uses this for geometry editing  */

////this for later
function init_geometry_editing() {
    var draw_options = {
        position: 'topleft',
        oneBlock: true,
        snappingOption: false,
        drawRectangle: false,
        drawMarker: true,
        drawPolygon: true,
        drawPolyline: true,
        drawCircleMarker: false,
        drawCircle: false,
        editMode: true,
        cutPolygon: false,
        dragMode: false
    };
    map.pm.addControls(draw_options);


    map.on('pm:create', (e) => {
        var togeo = e.layer.toGeoJSON();
        var geoProp = {
            id: uuidv4(),
            name: "",
            location: "",
            location_Type: "",
            location_Update: "",
            visible: false
        }
        togeo.properties = geoProp;
        togeo.properties.visible = true;
    });
    map.on('pm:edit', (e) => {
        if (e.shape === '') {
            e.layer.bindPopup().openPopup();
        }
    })
}
function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}