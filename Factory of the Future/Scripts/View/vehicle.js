/*
*this is for the Vehicle data load 
*/

$.extend(fotfmanager.client, {
    updateVehicleTagStatus: async (vehicleupdate,id) => { updateVehicleTag(vehicleupdate, id) }
});

async function updateVehicleTag(vehicleupdate, id) {
    try {
        if (id == baselayerid) {
            var layerindex = -0;
            map.whenReady(() => {
                if (map.hasOwnProperty("_layers")) {
                    $.map(map._layers, function (layer, i) {
                        if (layer.hasOwnProperty("feature")) {
                            if (layer.feature.properties.id === vehicleupdate.properties.id) {
                                layerindex = layer._leaflet_id;
                                layer.feature.geometry = vehicleupdate.geometry.coordinates;
                                layer.feature.properties = vehicleupdate.properties;
                                Promise.all([updateVehicleLocation(layerindex)]);
                                return false;
                            }
                        }
                    });
                }
                if (layerindex !== -0) {
                    if ($('div[id=vehicle_div]').is(':visible') && $('div[id=vehicle_div]').attr("data-id") === vehicleupdate.properties.id) {
                        updateVehicleInfo(layerindex);
                    }
                }
                else {
                    if (vehicleupdate.properties.Tag_Type === "Vehicle") {
                        piv_vehicles.addData(vehicleupdate);
                    }
                    if (vehicleupdate.properties.Tag_Type === "Autonomous Vehicle") {
                        agv_vehicles.addData(vehicleupdate);
                    }
                }
            });
        }
    } catch (e) {
        console.log(e);
    }
}
var piv_vehicles = new L.GeoJSON(null, {
    pointToLayer: function (feature, latlng) {
        var vehicleIcon = L.divIcon({
            id: feature.properties.id,
            className: get_pi_icon(feature.properties.name, feature.properties.Tag_Type) + ' iconXSmall',
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
            icon: vehicleIcon,
            title: feature.properties.name,
            riseOnHover: true,
            bubblingMouseEvents: true,
            popupOpen: true
        })
    },
    onEachFeature: function (feature, layer) {
        var obstructedState = '';
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        layer.on('click', function (e) {
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng, 4);
            if ((' ' + document.getElementById('sidebar').className + ' ').indexOf(' ' + 'collapsed' + ' ') <= -1) {
                if ($('#zoneselect').val() == feature.properties.id) {
                    sidebar.close('home');
                }
                else {
                    sidebar.open('home');
                }
            }
            else {
                sidebar.open('home');
            }
            LoadVehicleTable(feature.properties);
        })
        if (feature.properties.hasOwnProperty('state')) {
            var new_state = Get_Vehicle_Status(feature.properties.Vehicle_Status_Data.STATE.replace(/VState/ig, ""));
            if (/Obstructed|(Error)$/i.test(new_state)) {
                obstructedState = 'obstructedflash';
            }
        }
        layer.bindTooltip(feature.properties.name.replace(/[^0-9.]/g, '').replace(/^0+/, ''), {
            permanent: true,
            interactive: true,
            direction: 'top',
            opacity: 0.9,
            className: 'vehiclenumber ' + obstructedState
        }).openTooltip();
    }
});
var agv_vehicles = new L.GeoJSON(null, {
    pointToLayer: function (feature, latlng) {
        var vehicleIcon = L.divIcon({
            id: feature.properties.id,
            className: get_pi_icon(feature.properties.name, feature.properties.Tag_Type) + ' iconXSmall',
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
            icon: vehicleIcon,
            title: feature.properties.name,
            riseOnHover: true,
            bubblingMouseEvents: true,
            popupOpen: true
        })
    },
    onEachFeature: function (feature, layer) {
        var obstructedState = '';
        $zoneSelect[0].selectize.addOption({ value: feature.properties.id, text: feature.properties.name });
        $zoneSelect[0].selectize.addItem(feature.properties.id);
        $zoneSelect[0].selectize.setValue(-1, true);
        layer.on('click', function (e) {
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng, 4);
            if ((' ' + document.getElementById('sidebar').className + ' ').indexOf(' ' + 'collapsed' + ' ') <= -1) {
                if ($('#zoneselect').val() == feature.properties.id) {
                    sidebar.close('home');
                }
                else {
                    sidebar.open('home');
                }
            }
            else {
                sidebar.open('home');
            }
            LoadVehicleTable(feature.properties);
        })
        if (feature.properties.Vehicle_Status_Data !== null) {
            var new_state = Get_Vehicle_Status(feature.properties.Vehicle_Status_Data.STATE.replace(/VState/ig, ""));
            if (/Obstructed|(Error)$/i.test(new_state)) {
                obstructedState = 'obstructedflash';
            }
        }
        layer.bindTooltip(feature.properties.name.replace(/[^0-9.]/g, '').replace(/^0+/, ''), {
            permanent: true,
            interactive: true,
            direction: 'top',
            opacity: 0.9,
            className: 'vehiclenumber ' + obstructedState
        }).openTooltip();
    }
});
function updateVehicleLocation(layerindex) {
    if (map._layers[layerindex].feature.geometry.length > 0) {
        var newLatLng = new L.latLng(map._layers[layerindex].feature.geometry[1], map._layers[layerindex].feature.geometry[0]);
        var distanceTo = (newLatLng.distanceTo(map._layers[layerindex].getLatLng()).toFixed(0) / 1000);
        if (Math.round(distanceTo) > 4000) {
            map._layers[layerindex].setLatLng(newLatLng);
        }
        else {
            map._layers[layerindex].slideTo(newLatLng, { duration: 3000 });
        }
    }
    if (map._layers[layerindex].feature.properties.Vehicle_Status_Data !== null) {
        var new_state = Get_Vehicle_Status(map._layers[layerindex].feature.properties.Vehicle_Status_Data.STATE.replace(/VState/ig, ""));
        if (/Obstructed|(Error)$/i.test(new_state)) {
            if (map._layers[layerindex].hasOwnProperty("_icon")
                && map._layers[layerindex].hasOwnProperty("_tooltip")
                && map._layers[layerindex]._tooltip.hasOwnProperty("_container")) {
                if (!map._layers[layerindex]._tooltip._container.classList.contains('obstructedflash')) {
                    map._layers[layerindex]._tooltip._container.classList.add('obstructedflash');
                }

            }
        } else {
            if (map._layers[layerindex].hasOwnProperty("_tooltip") && map._layers[layerindex]._tooltip.hasOwnProperty("_container")) {
                if (map._layers[layerindex]._tooltip._container.classList.contains('obstructedflash')) {
                    map._layers[layerindex]._tooltip._container.classList.remove('obstructedflash');
                }
            }
        }
    }
    return null;
}
function updateVehicleInfo(layerindex) {
    try {
        if ($('input[type=checkbox][name=followvehicle]')[0].checked) {
            map.panTo(new L.LatLng(map._layers[layerindex].feature.geometry[1], map._layers[layerindex].feature.geometry[0]), 4);
        }

        if (/^AGV/i.test(map._layers[layerindex].feature.properties.name)) {
            AGVStausUpdate(layerindex);
        }

    } catch (e) {
        console.log(e);
    }
}
async function AGVStausUpdate(layerindex)
{
    try {        
        if (map._layers[layerindex].feature.properties.Vehicle_Status_Data !== null) {
            var new_state = Get_Vehicle_Status(map._layers[layerindex].feature.properties.Vehicle_Status_Data.STATE.replace(/VState/ig, ""));

                var current_state = $("button[id=" + map._layers[layerindex].feature.properties.name + "][name=vehicle]").text();
                if (new_state !== current_state) {
                    $("button[id=" + map._layers[layerindex].feature.properties.name + "][name=vehicle]").text(new_state);
                }
            var new_btn_category = Get_Vehicle_Catagory(map._layers[layerindex].feature.properties.Vehicle_Status_Data.CATEGORY.toString());
                var current_btn_category = Get_Current_Class($("button[id=" + map._layers[layerindex].feature.properties.name + "][name=vehicle]").attr("class"));
                if (new_btn_category !== current_btn_category) {
                    $("button[id=" + map._layers[layerindex].feature.properties.name + "][name=vehicle]").addClass(new_btn_category).removeClass(current_btn_category);
                }
                var batint = parseInt($("span[name=" + map._layers[layerindex].feature.properties.name + "_batter_level" + "]").attr("data-batter_lvl"));
                if (!map._layers[layerindex].feature.properties.hasOwnProperty("vehicleBatteryPercent")) {
                    map._layers[layerindex].feature.properties.vehicleBatteryPercent = 0;
                }
            if (parseInt(map._layers[layerindex].feature.properties.Vehicle_Status_Data.BATTERYPERCENT) !== batint) {
                    $("span[name=" + map._layers[layerindex].feature.properties.name + "_batter_level" + "]").text(map._layers[layerindex].feature.properties.Vehicle_Status_Data.BATTERYPERCENT + " % Charged").attr("data-batter_lvl", map._layers[layerindex].feature.properties.Vehicle_Status_Data.BATTERYPERCENT);
                    $("div[name=" + map._layers[layerindex].feature.properties.name + "_progressbar" + "]").attr("aria-valuenow", map._layers[layerindex].feature.properties.name).css("width", map._layers[layerindex].feature.properties.Vehicle_Status_Data.BATTERYPERCENT + "%");
                    new_btn_category = Get_Vehicle_Progress(map._layers[layerindex].feature.properties.Vehicle_Status_Data.BATTERYPERCENT);
                    current_btn_category = Get_Current_Class($("div[name=" + map._layers[layerindex].feature.properties.name + "_progressbar" + "]").attr("class"));
                    if (new_btn_category !== current_btn_category) {
                        $("div[name=" + map._layers[layerindex].feature.properties.name + "_progressbar" + "]").addClass(new_btn_category).removeClass(current_btn_category);
                    }
                }
            
            if (map._layers[layerindex].feature.properties.Mission !== null) {
                if ($('#vehicletagid').attr("data-id") === map._layers[layerindex].feature.properties.id) {
                    vehiclemission_Table_Body.empty();
                    if (!$.isEmptyObject(map._layers[layerindex].feature.properties.Mission)) {
                        vehiclemission_Table_Body.append(agvmissionrow_template.supplant(formatvehiclemissionrow(map._layers[layerindex].feature.properties.Mission)));

                    }
                }
            }
            else {
                if (vehiclemission_Table_Body.find("tr[id=inmission]").length > 0) {
                    vehiclemission_Table_Body.empty();
                }
               
            }
            //if (/^Charg/i.test(vehicleupdate.properties.state)) {
            //    var ChargeIcon = L.Icon.extend({
            //        options: {
            //            iconUrl: '<i class="bi-ligthning">',
            //            iconSize: [32, 32],
            //            iconAnchor: [10, 0],
            //            popupAnchor: [5, 5],
            //        }
            //    })
            //    vehicles._layers[layerindex].setIcon(new ChargeIcon);
            //}
            //else {
            //    if (/lightning/i.test(vehicles._layers[layerindex].options.icon.options.iconUrl)) {
            //        var ChargeIcon = L.Icon.extend({
            //            options: {
            //                iconSize: [32, 32],
            //                iconAnchor: [10, 0],
            //                popupAnchor: [5, 5],
            //                iconUrl: 'Content/icons/' + get_icon(vehicles._layers[layerindex].feature.properties.name, vehicles._layers[layerindex].feature.properties.Tag_Type)
            //            }
            //        })
            //        vehicles._layers[layerindex].setIcon(new ChargeIcon);
            //    }
            //}
        }
    } catch (e) {
        console.log(e);
    }
}
async function LoadVehicleTable(dataproperties) {
    try {
        $zoneSelect[0].selectize.setValue(-1, true);
        vehicletop_Table_Body.empty();
        vehiclemission_Table_Body.empty();
        hideSidebarLayerDivs();
        $zoneSelect[0].selectize.setValue(dataproperties.id, true);
        $('div[id=vehicle_div]').attr("data-id", dataproperties.id);
        $('div[id=vehicle_div]').css('display', 'block');


        if (!/AGV/i.test(dataproperties.name)) {
            vehicletop_Table_Body.append(pivrow_template.supplant(formatvehicleinforow(dataproperties)));
        }

        if (/^AGV/i.test(dataproperties.name)) {
            vehicletop_Table_Body.append(agvrow_template.supplant(formatvehicleinforow(dataproperties)));
            vehiclemission_Table_Body.empty();
            if (!$.isEmptyObject(dataproperties.Mission)) {
                vehiclemission_Table_Body.append(agvmissionrow_template.supplant(formatvehiclemissionrow(dataproperties.Mission)));

            }
        }
    } catch (e) {
        console.log(e);
    }
}
let pivrow_template = '<tr data-id="{TagId}" id="vehicletagid">' +
    '<td >Vehicle Name:</td>' +
    '<td class="text-left" id="vehicletag_name">{name}</td>' +
    '<td>Tag ID:</td>' +
    '<td  id="vehicletag_tagid">{TagId}</td>' +
    '</tr>' +
    '</td>' +
    '</tr>"';
let vehicletop_Table = $('table[id=vehicletable]');
let vehicletop_Table_Body = vehicletop_Table.find('tbody');

let agvrow_template = '<tr data-id="{TagId}" id="vehicletagid"><td>Vehicle Name:</td><td id="vehicletag_name" class="text-left">{name}</td><td  >Tag ID:</td><td  id="vehicletag_tagid">{TagId}</td></tr>' +
    '<tr><td>State:</td><td colspan="3"><button id="{name}" name="vehicle" class="btn btn-block btn-sm {vehiclestateCategory} vehiclehistory" style="margin-bottom: 2px;" value="{vehiclestate}">{vehiclestate}</button></td></tr>"' +
    '<tr><td>Battery:</td><td colspan="3">' +
    '<div class="progress">' +
    '<div class="progress-bar btn-sm {vehicleBatteryProgressBar}"  name="{name}_progressbar" role="progressbar" aria-valuenow="{batterylevelnum}" aria-valuemin="0" aria-valuemax="100" style="width: {batterylevelnum}%;">' +
    '<span name="{name}_batter_level" style="color: black; position: absolute;" data-batter_lvl="{batterylevelnum}">{batterylevelnum} % Charged</span>' +
    '</div>' +
    '</div>' + '</td></tr>"';

let agvmissionrow_template = '<tr class="text-center" id=inmission>' +
    '<td class="font-weight-bold" colspan="2">' +
        '<h6 class="control-label sectionHeader ml-1 mb-1 d-flex justify-content-between">' +
        'Mission Status' +
        '<span class="d-flex justify-content-between">' +
        'Request ID:' +
        '<span class="btn btn-secondary border-0 badge-dark badge">{RequestId}</span>  ' +
        '</span>' +
        '</h6 >' +
    '</td>' +
    '<tr id=pickupLocation_{TagId}><td>Pickup Location:</td><td>{PickupLocation}</td></tr>' +
    '<tr id=pickupLocationEta_{TagId}><td>ETA to Pickup:</td><td>{PickupLocationEta}</td></tr>' +
    '<tr id=dropoffLocation_{TagId}><td>Drop-Off Location:</td><td>{DropoffLocation}</td></tr>' +
    '<tr id=endLocation_{TagId}><td>End Location:</td><td>{EndLocation}</td></tr>' +
    '<tr id=Door_{TagId}><td>Door:</td><td>{Door}</td></tr>' +
    '<tr><td>Placard:</td><td>{Placard}</td></tr>' +
    '</tr>';
let vehiclemission_Table = $('table[id=vehiclemissiontable]');
let vehiclemission_Table_Body = vehiclemission_Table.find('tbody');
function formatvehicleinforow(properties) {
    return $.extend(properties, {
        TagId: properties.id,
        name: properties.name,
        batterylevelnum: properties.Vehicle_Status_Data !== null ? properties.Vehicle_Status_Data.BATTERYPERCENT : 0,
        vehiclenumber: properties.Vehicle_Status_Data !== null ? properties.name.replace(/[^0-9.]/g, '').replace(/^0+/, '') : "",
        vehiclestate: properties.Vehicle_Status_Data !== null ? Get_Vehicle_Status(properties.Vehicle_Status_Data.STATE.replace(/VState/ig, "")) : "N/A",
        vehicleBatteryProgressBar: Get_Vehicle_Progress(properties.Vehicle_Status_Data !== null ? properties.Vehicle_Status_Data.BATTERYPERCENT : 0),
        vehiclestateCategory: properties.Vehicle_Status_Data !== null ? Get_Vehicle_Catagory(properties.Vehicle_Status_Data.CATEGORY.toString()) : "btn-outline-info",
        vehiclestatecolor: properties.Vehicle_Status_Data !== null ? properties.Vehicle_Status_Data.STATE.replace(/VState/ig, "") : "red"
    });
}
function formatvehiclemissionrow(properties) {
    return $.extend(properties, {
        RequestId: properties.Request_Id,
        PickupLocation: properties.hasOwnProperty("Pickup_Location") ? Get_location_Code(properties.Pickup_Location) : "N/A",
        DropoffLocation: properties.hasOwnProperty("Dropoff_Location") ? Get_location_Code(properties.Dropoff_Location) : "N/A",
        EndLocation: properties.hasOwnProperty("End_Location") ? Get_location_Code(properties.End_Location) : "N/A",
        PickupLocationEta: properties.hasOwnProperty("ETA") ? properties.ETA : "N/A",
        Placard: properties.hasOwnProperty("Placard") ? properties.Placard : "N/A"
    });
}
function Get_Vehicle_Catagory(category) {
    if (category === "btn-outline-info") {
        return "btn-success";
    }
    if (category === "1") {
        return "btn-success";
    }
    if (category === "2") {
        return "btn-outline-info";
    }
    if (category === "3") {
        return "btn-outline-primary";
    }
    if (category === "4") {
        return "btn-danger";
    }
    if (category === "") {
        return "btn-outline-info";
    }
    if (category === undefined) {
        return "btn-outline-info";
    }
}
function Get_Vehicle_Status(status) {
    if (/Idle/i.test(status)) {
        return capitalize_Words("Available");
    }
    if (/PathResume|blocked/i.test(status)) {
        return capitalize_Words("Obstructed");
    }
    if (/PathFollow|Working|PathBuild/i.test(status)) {
        return capitalize_Words(status.replace(''.toUpperCase(), ''));
    }
    if (/Shutdown|Blocked/i.test(status)) {
        return capitalize_Words(status.replace(''.toUpperCase(), ''));
    }
    if (/ChargeStart/i.test(status)) {
        return capitalize_Words("Charging");
    }
    if (/Charging/i.test(status)) {
        return capitalize_Words("Charging");
    }
    if (/NO_STATUS_RCVD|NoComm|Unknown/i.test(status)) {
        return capitalize_Words("No Status");
    }
    if (!/NO_STATUS_RCVD|Shutdown|Charging|PathFollow|Idle|ChargeStart|Working|Blocked/i.test(status)) {
        return capitalize_Words(status.replace(''.toUpperCase(), ''), '');
    }

    return capitalize_Words(status);
}
function Get_Current_Class(classList) {
    if (checkValue(classList)) {
        var classArr = classList.split(/\s+/);
        var btn_val = "";
        $.each(classArr, function (index, value) {
            if (/^btn-outline/i.test(value)) {
                btn_val = value;
            }
            if (/^btn-success/i.test(value)) {
                btn_val = value;
            }
            if (/^btn-info/i.test(value)) {
                btn_val = value;
            }
            if (/^btn-danger/i.test(value)) {
                btn_val = value;
            }
            if (/^btn-secondary/i.test(value)) {
                btn_val = value;
            }
            if (/^btn-warning/i.test(value)) {
                btn_val = value;
            }
            if (/^btn-light/i.test(value)) {
                btn_val = value;
            }
            if (/^btn-dark/i.test(value)) {
                btn_val = value;
            }
            if (/^btn-link/i.test(value)) {
                btn_val = value;
            }
            if (/^bg-success/i.test(value)) {
                btn_val = value;
            }
            if (/^bg-info/i.test(value)) {
                btn_val = value;
            }
            if (/^bg-warning/i.test(value)) {
                btn_val = value;
            }
            if (/^bg-danger/i.test(value)) {
                btn_val = value;
            }
        });
        return btn_val;
    }
    else {
        return "";
    }
}
function Get_Vehicle_Progress(Level) {
    if (Level < 20) {
        return "bg-danger";
    }
    if (Level >= 20 && Level <= 50) {
        return "bg-warning";
    }
    if (Level >= 51) {
        return "bg-info";
    }
}
function get_pi_icon(name, type) {
    if (/Vehicle$/i.test(type)) {
        if (checkValue(name)) {
            if (/^(wr|walkingrider)/i.test(name)) {
                return "pi-iconLoader_wr ml--24";
            }
            if (/^(fl|forklift)/i.test(name)) {
                return "pi-iconLoader_forklift ml--8";
            }
            if (/^(t|tug|mule)/i.test(name)) {
                return "pi-iconLoader_tugger ml--16";
            }
            if (/^agv_t/i.test(name)) {
                return "pi-iconLoader_avg_t ml--8";
            }
            if (/^agv_p/i.test(name)) {
                return "pi-iconLoader_avg_pj ml--16";
            }
            if (/^ss/i.test(name)) {
                return "pi-iconVh_ss ml--16";
            }
            if (/^bf/i.test(name)) {
                return "pi-iconVh_bss ml--16";
            }
            if (/^Surfboard/i.test(name)) {
                return "pi-iconSurfboard ml--32";
            }
            return "pi-iconVh_ss ml--16";
        }
        else {
            return "pi-iconVh_ss ml--16";
        }
    }
    else {
        return "pi-iconVh_ss ml--16";
    }
}
//async function init_agvtags() {
//    //Get AGV Location list
//    fotfmanager.server.getVehicleTagsList().done(function (Data) {
//        if (Data.length > 0) {
//            $.each(Data, function () {
//                updateVehicleTag(this);
//            })
//            fotfmanager.server.joinGroup("VehiclsMarkers");
//        }
//    });
//}

async function Edit_Tag_Name(tagId, tagName) {
    $('#TagName_Modal_Header_ID').text('Edit Tag Name');
    sidebar.close('connections');
    $('#TagName_Modal').modal();
    $('#modal_tag_id').html(tagId);
    $('#edit_tag_name').val(tagName);
    $('#edittagsubmitBtn').off().on("click", function () {
        Edit_Tag_Name_Submit();
    });
}
async function Edit_Tag_Name_Submit() {
    let tagId = $('#modal_tag_id').html();
    let tagName = $('#edit_tag_name').val();
    fotfmanager.server.updateTagName(tagId, tagName).done(function (data) {
        let response = JSON.parse(data);
        if (response.status == "updated") {
            $("#error_edittagsubmitBtn").html("");
            $("#TagName_Modal").modal("toggle");
        }
        else {
            $("#error_edittagsubmitBtn").html("Error updating tag.");
        }
    });
}


$(function () {
    if ( ! /^Admin/i.test(User.Role) ) {
        $("#vehicle-info-edit-row").remove();
    }
    $('button[name=vehicleinfoedit]').off().on('click', function () {
        if (/^Admin/i.test(User.Role)) {
            /* close the sidebar */
            let tagId = $("#vehicletag_tagid").html().trim();
            let tagName = $("#vehicletag_name").html().trim();
            sidebar.close();
            if (checkValue(tagId)) {
                Edit_Tag_Name(tagId, tagName);
            }
        }
    });
});

$(function () {

})