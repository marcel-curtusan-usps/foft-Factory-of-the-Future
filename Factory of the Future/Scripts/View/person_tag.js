/*
this is for the person details.
 */
$.extend(fotfmanager.client, {
    updatePersonTagStatus: async (tagupdate, id) => { Promise.all([updateFeature(tagupdate, id)]); },
    updatePersonTagRemove: async (tagupdate, id) => { Promise.all([deleteFeature(tagupdate, id)]); },
    updateMarkerCoordinates: async (coordinates, floorid, markerid) => { Promise.all([MarkerCoordinates(coordinates, floorid, markerid)]); }
});

///update geojson data
//var myFeaturesMap = {};

//var myGeoJsonLayerGroup = L.geoJson({
//    onEachFeature: function (feature, layer) {
//        myFeaturesMap[feature.properties.objectID] = layer;
//    }
//}).addTo(map);

//function addNewFeatureToGeoJsonLayerGroup(newGeoJsonData) {
//    myGeoJsonLayerGroup.addData(newGeoJsonData);
//}

//function updateFeature(updatedGeoJsonData) {
//    deleteFeature(updatedGeoJsonData); // Remove the previously created layer.
//    addNewFeatureToGeoJsonLayerGroup(updatedGeoJsonData); // Replace it by the new data.
//}

//function deleteFeature(deletedGeoJsonData) {
//    var deletedFeature = myFeaturesMap[deletedGeoJsonData.properties.objectID];
//    myGeoJsonLayerGroup.removeLayer(deletedFeature);
//}

let markerList = {} ;
async function init_Peoplelocators(marker, id) {
    $('div[id=staffingbutton]').text(marker.length);
    tagsMarkersGroup.addData(marker);
    /*Promise.all([AddPeopleMarker(marker, id)]);*/
    //$.each(marker, function (_index, data) {
    //    Promise.all([AddPeopleMarker(data, data.properties.floorId)]);
    //});
    fotfmanager.server.joinGroup("PeopleMarkers");
    tagsLoaded = true;
    return tagsLoaded;
}
async function addFeature(data) {
    try {
        if (data.properties.floorId === baselayerid) {
            $('div[id=staffingbutton]').text(markerList.lenght);
            tagsMarkersGroup.addData(data);
        }
        return true;
    }
    catch (e) {
        console.log(e);
    }
}
async function updateFeature(data, floorId) {
    try {
        if (floorId === baselayerid) {
            let updateId = markerList[data.properties.id];
            if (typeof updateId !== 'undefined' && map.hasLayer(updateId)) {
                tagsMarkersGroup._layers[updateId._leaflet_id].feature = data
                tagsMarkersGroup._layers[updateId._leaflet_id].slideTo(new L.LatLng(data.geometry.coordinates[1], data.geometry.coordinates[0]), { duration: 10000 });

                markerList[data.properties.id] = tagsMarkersGroup._layers[updateId._leaflet_id]

            }
            else {
                Promise.all([addFeature(data)]);
            }
        }

        //Promise.all([deleteFeature(data.properties.id)]); // Remove the previously created layer.
        //Promise.all([addFeature(data)]); // Replace it by the new data.

        //if (id === baselayerid) {
        // let layerindex = -0;
        //  Promise.all([hideOldTag()]);
        // map.whenReady(() => {
        //    if (map.hasOwnProperty("_layers")) {
        //        $.map(map._layers, function (layer, i) {
        //            if (layer.hasOwnProperty("feature")) {
        //                if (layer.feature.properties.id === tagpositionupdate.properties.id) {
        //                    layerindex = layer._leaflet_id;
        //                    layer.feature.geometry = tagpositionupdate.geometry.coordinates;
        //                    layer.feature.properties = tagpositionupdate.properties;
        //                    Promise.all([updateTagLocation(layerindex)]);
        //                    return false;
        //                }
        //            }
        //        });
        //    }
        //});
        // if (layerindex === -0) {
        //    tagsMarkersGroup.addData(tagpositionupdate);
        //}
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
        //}

    } catch (e) {
        console.log(e);
    }
}
async function deleteFeature(id, floorId) {
    try {
        let delId = markerList[id];
        if (typeof delId !== 'undefined') {
            tagsMarkersGroup.removeLayer(delId);
        }
        $('div[id=staffingbutton]').text(markerList.length);
    } catch (e) {
        console.log(e);
    }
    
   
    //$.map(map._layers, function (layer, i) {
    //    if (layer.hasOwnProperty("feature") && layer.feature.properties.id === id) {
    //        layer.removeLayer();
    //    }
    //});
}
let tagsMarkersGroup = new L.GeoJSON(null, {
    pointToLayer: function (feature, latlng) {
        return new L.circleMarker(latlng, {
            radius: 8,
            opacity: 0,
            fillOpacity: 0,
            className: 'marker-person'
        });
    },
    onEachFeature: function (feature, layer) {
        markerList[feature.properties.id] = layer;
        //layer.markerId = feature.properties.id;
        let VisiblefillOpacity = feature.properties.visible ? "" : "tooltip-hidden";
        //var isOT = false;
        //var isOTAuth = false;
        let classname = getmarkerType(feature.properties.craftName) + VisiblefillOpacity;
        //if (feature.properties.tacs != null) {
        //    isOT = feature.properties.tacs.isOvertime;
        //    isOTAuth = feature.properties.tacs.isOvertimeAuth;
        //    if (isOT && isOTAuth) {
        //        classname = 'persontag_otAuth ' + VisiblefillOpacity;
        //    }
        //    else if (isOT && !isOTAuth) {
        //        classname = 'persontag_otNotAuth ' + VisiblefillOpacity;
        //    }
        //}
        layer.on('click', function (e) {
            hidestafftables();
            $('div[id=div_taginfo]').css('display', '');
            sidebar.open('reports');
            if ($.fn.dataTable.isDataTable("#tagInfotable")) {
                updateTagDataTable(formattagdata(feature.properties), "tagInfotable");
            }
            else {
                createTagDataTable('tagInfotable');
                updateTagDataTable(formattagdata(feature.properties), "tagInfotable");
            }
        });

        layer.bindTooltip("", {
            permanent: true,
            interactive: true,
            direction: 'center',
            opacity: 1,
            //className: 'persontag ' + VisiblefillOpacity
            className: classname
        }).openTooltip();
    }
    ,
    filter: function (feature, layer) {
        return feature.properties.visible;
    }
});
function getmarkerType(type) {
    try {
        if (/supervisor/.test(type)) {
            return 'persontag_supervisor ';
        }
        else if (/maintenance/.test(type)) {
            return 'persontag_maintenance ';
        }
        else if (/pse/.test(type)) {
            return 'persontag_pse ';
        }
        else if (/inplantsupport/.test(type)) {
            return 'persontag_inplantsupport ';
        }
        else if (/(clerk|mailhandler)/.test(type)) {
            return 'persontag ';
        }
        else {
            return 'persontag_unknown ';
        }

    } catch (e) {
        return 'persontag ';
    }
   
}
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
                && !tagsMarkersGroup._layers[layerindex].feature.properties.tacs.isOvertimeAuth) {
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

        if (tagsMarkersGroup._layers[layerindex].feature.properties.posAge > 80000 && tagsMarkersGroup._layers[layerindex].hasOwnProperty("_tooltip") && tagsMarkersGroup._layers[layerindex]._tooltip.hasOwnProperty("_container") &&
            !tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('tooltip-hidden')) {

            tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.add('tooltip-hidden');

        }

        if (tagsMarkersGroup._layers[layerindex].feature.properties.posAge < 80000) {
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
async function hideOldTag()
{
    $.map(map._layers, function (layer, i) {
        if (layer.hasOwnProperty("feature") && layer.feature.hasOwnProperty("properties") && /person/i.test(layer.feature.properties.Tag_Type)) {
            let lastseenTS = moment(layer.feature.properties.lastSeenTS);
            let positionTS = moment(layer.feature.properties.positionTS)
            if (layer.feature.properties.posAge > 80000 || layer.feature.properties.locationMovementStatus === "noData") {

                layer._tooltip._container.classList.add('tooltip-hidden');
            }
            if (/noData/i.test(layer.feature.properties.locationMovementStatus)) {
                layer._tooltip._container.classList.add('tooltip-hidden');
            }
            if (positionTS.year() === 1) {
                layer._tooltip._container.classList.add('tooltip-hidden');
            }
          
        }
    });
}
async function hidestafftables()
{
    $('div[id=div_taginfo]').css('display', '');
    $('div[id=div_userinfo]').css('display', 'none');
    $('div[id=div_overtimeinfo]').css('display', 'none');
    $('div[id=div_staffinfo]').css('display', 'none');
    
}
async function staffCounts()
{
    try {
        let tagsOnWorkfloor = 0;
 
        $.map(tagsMarkersGroup._layers, function (layer, i) {
            if (layer.hasOwnProperty("feature") && layer.feature.hasOwnProperty("properties") && /person/i.test(layer.feature.properties.Tag_Type)
                && layer.feature.properties.visible) {
                tagsOnWorkfloor++;
            }
        });
        $('div[id=staffingbutton]').text(tagsOnWorkfloor);
       
        if ((' ' + document.getElementById('sidebar').className + ' ').indexOf(' ' + 'collapsed' + ' ') <= -1) {
            getStaffInfo();
        }
    }
    catch (e) {
        console.log(e)
    }
}
async function getStaffInfo() {
    let taglist = [];
    let empType = [];
    let stafftable = [];
    $.map(tagsMarkersGroup._layers, function (layer, i) {
        if (layer.hasOwnProperty("feature") && layer.feature.hasOwnProperty("properties") && /person/i.test(layer.feature.properties.Tag_Type)
            && layer.feature.properties.visible) {
            taglist.push(layer.feature.properties);
        }
    });
    //find emp type
    $.map(emplschedule, function (list) {
        if ($.inArray(list.emptype, empType) === -1 && !/Unknoun/i.test(list.emptype)) {
                empType.push(list.emptype);
        }
    });

    $.map(empType, function (list) {
        stafftable.push({
            type: list,
            sche: emplschedule.filter(r => r.emptype === list && r.isSch).length,
            in_building: taglist.filter(r => r.emptype === list && r.isPosition).length,
            epacs: taglist.filter(r => r.emptype === list && r.isePacs).length
        });
    });
    // add the total
    stafftable.push({
        type: "Total",
        sche: emplschedule.filter(r => r.isSch).length,
        in_building: taglist.filter(r => r.isPosition).length,
        epacs: taglist.filter(r => r.isePacs).length
    });

    //console.log(stafftable);
    updateStaffingDataTable(stafftable, 'staffingtable');
}
function createStaffingDataTable(table) {
    let arrayColums = [{
        "type": "",
        "sche": "",
        "in_building": "",
        "epacs": ""
    }];
    let columns = [];
    let tempc = {};
    $.each(arrayColums[0], function (key) {
        tempc = {};
        if (/type/i.test(key)) {
            tempc = {
                "title": 'Type',
                "width": "30%",
                "mDataProp": key
            };
        }
        if (/sche/i.test(key)) {
            tempc = {
                "title": "Scheduled",
                "width": "20%",
                "mDataProp": key
            };
        }
        if (/in_building/i.test(key)) {
            tempc = {
                "title": "WorkZone",
                "width": "20%",
                "mDataProp": key
            };
        }
        if (/epacs/i.test(key)) {
            tempc = {
                "title": "ePACS",
                "width": "20%",
                "mDataProp": key
            };
        }
        columns.push(tempc);

    });
    $('#' + table).DataTable({
        dom: 'Bfrtip',
        bFilter: false,
        bdeferRender: true,
        bpaging: false,
        bPaginate: false,
        autoWidth: false,
        bInfo: false,
        destroy: true,
        language: {
            zeroRecords: "No Data"
        },
        aoColumns: columns,
        columnDefs: [
        ],
        sorting: [[0, "asc"]]
    });
}
function loadStaffingDatatable(data, table) {
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows.add(data).draw();
    }
}
function updateStaffingDataTable(newdata, table) {
    let loadnew = true;
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows(function (idx, data, node) {
            loadnew = false;
            for (const element of newdata) {
                if (data.type === element.type) {
                    $('#' + table).DataTable().row(node).data(element).draw().invalidate();
                }
            }

        })
        if (loadnew) {
            loadStaffingDatatable(newdata, table);
        }
    }
}
function createTagDataTable(table) {
    let arrayColums = [{
        "KEY_NAME": "",
        "VALUE": ""
    }]
    let columns = [];
    let tempc = {};
    //$.each(arrayColums[0], function (key, value) {
    $.each(arrayColums[0], function (key) {
        tempc = {};
        if (/KEY_NAME/i.test(key)) {
            tempc = {
                "title": 'Name',
                "width": "30%",
                "mDataProp": key
            }
        }
        //else if (/VALUE/i.test(key)) {
        if (/VALUE/i.test(key)) {
            tempc = {
                "title": "Value",
                "width": "50%",
                "mDataProp": key
            }
        }
      
        columns.push(tempc);

    });
    $('#' + table).DataTable({
        dom: 'Bfrtip',
        bFilter: false,
        bdeferRender: true,
        bpaging: false,
        bPaginate: false,
        autoWidth: false,
        bInfo: false,
        destroy: true,
        language: {
            zeroRecords: "No Data"
        },
        aoColumns: columns,
        columnDefs: [
        ],
        sorting: [[0, "asc"]]

    })
}
function updateTagDataTable(newdata, table) {
    let loadnew = true;
    if ($.fn.dataTable.isDataTable("#" + table)) {
        $('#' + table).DataTable().rows(function (idx, data, node) {
            loadnew = false;
            for (const element of newdata) {
                if (data.KEY_NAME === element.KEY_NAME) {
                    $('#' + table).DataTable().row(node).data(element).draw().invalidate();
                }
            }
        })
        if (loadnew) {
            loadStaffingDatatable(newdata, table);
        }
    }
}
function formattagdata(result) {
    let reformatdata = [];
    try {
        for (let key in result) {
            if (!$.isPlainObject(result[key]) && /^(id|craftName|lastSeenTS|lastSeenTS_txt|positionTS|locationType|daysOff|floorId|empId|elunch|edate|isePacs|isPosition|isSch|isTacs|isePacs|locationMovementStatus|Tag_Type|tourNumber|visible)/ig.test(key)) {

                let temp = {
                    "KEY_NAME": "",
                    "VALUE": ""
                };
                temp['KEY_NAME'] = key;
                temp['VALUE'] = result[key];
                reformatdata.push(temp);
            }
        }

    } catch (e) {
        console.log(e);
    }

    return reformatdata;
}