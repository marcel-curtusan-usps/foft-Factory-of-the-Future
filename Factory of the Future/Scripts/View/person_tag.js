/*
this is for the person details.
 */
$.extend(fotfmanager.client, {
    updatePersonTagStatus: async (tagupdate, id) => { Promise.all([ updatePersonTag(tagupdate, id)]) },
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
           /*Promise.all(hideOldTag());*/
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
        layer.on('click', function (e) {
            hidestafftables();
            $('div[id=div_taginfo]').css('display', '');
            sidebar.open('reports');
            if ($.fn.dataTable.isDataTable("#tagInfotable")) {
                updateTagDataTable(formattagdata(feature.properties),"tagInfotable");
            }
            else {
                createTagDataTable('tagInfotable');
                updateTagDataTable(formattagdata(feature.properties),"tagInfotable");
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

        if (tagsMarkersGroup._layers[layerindex].feature.properties.tagVisibleMils > 80000 && tagsMarkersGroup._layers[layerindex].hasOwnProperty("_tooltip") && tagsMarkersGroup._layers[layerindex]._tooltip.hasOwnProperty("_container") &&
            !tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.contains('tooltip-hidden')) {

            tagsMarkersGroup._layers[layerindex]._tooltip._container.classList.add('tooltip-hidden');

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
async function hideOldTag()
{
    $.map(map._layers, function (layer, i) {
        if (layer.hasOwnProperty("feature") && layer.feature.hasOwnProperty("properties") && /person/i.test(layer.feature.properties.Tag_Type))
        {
            if (layer.feature.properties.tagVisibleMils > 80000) {

                layer._tooltip._container.classList.add('tooltip-hidden');
            } 
        }
    })
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
 
        $.map(map._layers, function (layer, i) {
            if (layer.hasOwnProperty("feature") && layer.feature.hasOwnProperty("properties") && /person/i.test(layer.feature.properties.Tag_Type)
                && layer.feature.properties.tagVisible) {
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
    $.map(map._layers, function (layer, i) {
        if (layer.hasOwnProperty("feature") && layer.feature.hasOwnProperty("properties") && /person/i.test(layer.feature.properties.Tag_Type)) {
            taglist.push(layer.feature.properties)
        }
    });
    //find emp type
    $.map(emplschedule, function (list) {
        if ($.inArray(list.emptype, empType) == -1) {
            if (!!list.emptype) {
                empType.push(list.emptype)
            }
        }
    });

    $.map(empType, function (list) {
        stafftable.push({
            type: list,
            sche: emplschedule.filter(r => r.emptype === list && r.isSch).length,
            in_building: taglist.filter(r => r.emptype === list && r.isPosition).length
        });
    });
    // add the total
    stafftable.push({
        type: "Total",
        sche: emplschedule.filter(r => r.isSch).length,
        in_building: taglist.filter(r => r.isPosition).length
    });

    //console.log(stafftable);
    updateStaffingDataTable(stafftable,'staffingtable')
}
function createStaffingDataTable(table) {
    let arrayColums = [{
        "type": "",
        "sche": "",
        "in_building": ""
    }]
    let columns = [];
    let tempc = {};
    $.each(arrayColums[0], function (key) {
        tempc = {};
        if (/type/i.test(key)) {
            tempc = {
                "title": 'Type',
                "width": "30%",
                "mDataProp": key
            }
        }
        if (/sche/i.test(key)) {
            tempc = {
                "title": "Scheduled",
                "width": "50%",
                "mDataProp": key
            }
        }
        if (/in_building/i.test(key)) {
            tempc = {
                "title": "Work-floor",
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
            if (!$.isPlainObject(result[key]) && /^(id|daysOff|floorId|empId|elunch|edate|isePacs|isPosition|isSch|isTacs|isePacs|locationMovementStatus|Tag_Type|tourNumber|visible)/ig.test(key)) {

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