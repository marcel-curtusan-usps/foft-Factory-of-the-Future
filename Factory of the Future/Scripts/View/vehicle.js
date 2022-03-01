/*
*this is for the Vehicle data load 
*/

$.extend(fotfmanager.client, {
    updateVehicleTagStatus: async (vehicleupdate) => { updateVehicleTag(vehicleupdate) }
});

async function updateVehicleTag(vehicleupdate) {
    try {
        map.whenReady(() => {
            if (vehicles.hasOwnProperty("_layers")) {
                var layerindex = -0;
                $.map(vehicles._layers, function (layer, i) {
                    if (layer.hasOwnProperty("feature")) {
                        if (layer.feature.properties.id === vehicleupdate.properties.id) {
                            if (layerindex === -0) {
                                layerindex = layer._leaflet_id;
                                layer.feature.geometry = vehicleupdate.geometry.coordinates;
                                layer.feature.properties = vehicleupdate.properties;
                                Promise.all([updateVehicleLocation(layerindex)]);
                            }
                            return false;
                        }
                    }

                });
                if (layerindex !== -0) {
                    if ($('div[id=vehicle_div]').is(':visible') && $('div[id=vehicle_div]').attr("data-id") === vehicleupdate.properties.id) {
                        updateVehicleInfo(layerindex);
                    }
                }
                else {
                    vehicles.addData(vehicleupdate);
                }
            }

        });
    } catch (e) {
        console.log(e);
    }
}
var vehicles = new L.GeoJSON(null, {
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
        layer.on('click', function (e) {
            $('input[type=checkbox][name=followvehicle]').prop('checked', false).change();
            map.setView(e.latlng, 4);
            sidebar.open('home');
            LoadVehicleTable(feature.properties);
        })
        if (feature.properties.hasOwnProperty('state')) {
            var new_state = Get_Vehicle_Status(feature.properties.state.replace(/VState/ig, ""));
            if (/Obstructed|(Error)$/i.test(new_state)) {
                obstructedState = 'obstructedflash';
            }
        }
        layer.bindTooltip(feature.properties.name.replace(/[^0-9.]/g, '').replace(/^0+/, ''), {
            permanent: true,
            direction: 'top',
            opacity: 0.9,
            className: 'vehiclenumber ' + obstructedState
        }).openTooltip();
    }
});
function updateVehicleLocation(layerindex) {
    if (vehicles._layers[layerindex].feature.geometry.length > 0) {
        var newLatLng = new L.latLng(vehicles._layers[layerindex].feature.geometry[1], vehicles._layers[layerindex].feature.geometry[0]);
        var distanceTo = (newLatLng.distanceTo(vehicles._layers[layerindex].getLatLng()).toFixed(0) / 1000);
        if (Math.round(distanceTo) > 4000) {
            vehicles._layers[layerindex].setLatLng(newLatLng);
        }
        else {
            vehicles._layers[layerindex].slideTo(newLatLng, { duration: 3000 });
        }
    }
    if (vehicles._layers[layerindex].feature.properties.hasOwnProperty("state")) {
        var new_state = Get_Vehicle_Status(vehicles._layers[layerindex].feature.properties.state.replace(/VState/ig, ""));
        if (/Obstructed|(Error)$/i.test(new_state)) {
            if (vehicles._layers[layerindex].hasOwnProperty("_icon")
                && vehicles._layers[layerindex].hasOwnProperty("_tooltip")
                && vehicles._layers[layerindex]._tooltip.hasOwnProperty("_container")) {
                if (!vehicles._layers[layerindex]._tooltip._container.classList.contains('obstructedflash')) {
                    vehicles._layers[layerindex]._tooltip._container.classList.add('obstructedflash');
                }

            }
        } else {
            if (vehicles._layers[layerindex].hasOwnProperty("_tooltip") && vehicles._layers[layerindex]._tooltip.hasOwnProperty("_container")) {
                if (vehicles._layers[layerindex]._tooltip._container.classList.contains('obstructedflash')) {
                    vehicles._layers[layerindex]._tooltip._container.classList.remove('obstructedflash');
                }
            }
        }
    }
    return null;

}
function updateVehicleInfo(layerindex) {
    try {
        if ($('input[type=checkbox][name=followvehicle]')[0].checked) {
            map.panTo(new L.LatLng(vehicles._layers[layerindex].feature.geometry[1], vehicles._layers[layerindex].feature.geometry[0]), 4);
        }

        if (/^AGV/i.test(vehicles._layers[layerindex].feature.properties.name)) {
            AGVStausUpdate(layerindex);
        }

    } catch (e) {
        console.log(e);
    }
}
async function AGVStausUpdate(layerindex)
{
    try {
        if (vehicles._layers[layerindex].feature.properties.hasOwnProperty("state")) {
            var new_state = Get_Vehicle_Status(vehicles._layers[layerindex].feature.properties.state.replace(/VState/ig, ""));
            var vehicle_div = $('div[id=div_vehicle]').find('button[id=' + vehicles._layers[layerindex].feature.properties.name + '][name=vehicle]');

            if (vehicle_div.length > 0) {
                var current_state = $("button[id=" + vehicles._layers[layerindex].feature.properties.name + "][name=vehicle]").text();
                if (new_state !== current_state) {
                    $("button[id=" + vehicles._layers[layerindex].feature.properties.name + "][name=vehicle]").text(new_state);
                }
                var new_btn_category = Get_Vehicle_Catagory(vehicles._layers[layerindex].feature.properties.vehicleCategory);
                var current_btn_category = Get_Current_Class($("button[id=" + vehicles._layers[layerindex].feature.properties.name + "][name=vehicle]").attr("class"));
                if (new_btn_category !== current_btn_category) {
                    $("button[id=" + vehicles._layers[layerindex].feature.properties.name + "][name=vehicle]").addClass(new_btn_category).removeClass(current_btn_category);
                }
                var batint = parseInt($("span[name=" + vehicles._layers[layerindex].feature.properties.name + "_batter_level" + "]").attr("data-batter_lvl"));
                if (!vehicles._layers[layerindex].feature.properties.hasOwnProperty("vehicleBatteryPercent")) {
                    vehicles._layers[layerindex].feature.properties.vehicleBatteryPercent = 0;
                }
                if (parseInt(vehicles._layers[layerindex].feature.properties.vehicleBatteryPercent) !== batint) {
                    $("span[name=" + vehicles._layers[layerindex].feature.properties.name + "_batter_level" + "]").text(vehicles._layers[layerindex].feature.properties.vehicleBatteryPercent + " % Charged").attr("data-batter_lvl", vehicles._layers[layerindex].feature.properties.vehicleBatteryPercent);
                    $("div[name=" + vehicles._layers[layerindex].feature.properties.name + "_progressbar" + "]").attr("aria-valuenow", vehicles._layers[layerindex].feature.properties.name).css("width", vehicles._layers[layerindex].feature.properties.vehicleBatteryPercent + "%");
                    new_btn_category = Get_Vehicle_Progress(vehicles._layers[layerindex].feature.properties.vehicleBatteryPercent);
                    current_btn_category = Get_Current_Class($("div[name=" + vehicles._layers[layerindex].feature.properties.name + "_progressbar" + "]").attr("class"));
                    if (new_btn_category !== current_btn_category) {
                        $("div[name=" + vehicles._layers[layerindex].feature.properties.name + "_progressbar" + "]").addClass(new_btn_category).removeClass(current_btn_category);
                    }
                }
            }
            if (vehicles._layers[layerindex].feature.properties.hasOwnProperty("Mission") && vehicles._layers[layerindex].feature.properties.Mission.length > 0) {
                if ($('#vehicletagid').attr("data-id") === vehicles._layers[layerindex].feature.properties.id) {
                    vehiclemission_Table_Body.empty();
                    if (!$.isEmptyObject(vehicles._layers[layerindex].feature.properties.Mission)) {
                        $.each(vehicles._layers[layerindex].feature.properties.Mission, function () {
                            vehiclemission_Table_Body.append(agvmissionrow_template.supplant(formatvehiclemissionrow(this)));
                        });
                    }
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
            $('div[id=vehicle_div]').attr("data-id", dataproperties.id);
            $('div[id=agvlocation_div]').css('display', 'none');
            $('div[id=dockdoor_div]').css('display', 'none');
            $('div[id=trailer_div]').css('display', 'none');
            $('div[id=machine_div]').css('display', 'none');
            $('div[id=staff_div]').css('display', 'none');
            $('div[id=ctstabs_div]').css('display', 'none');
            $('div[id=area_div]').css('display', 'none');
            $('div[id=dps_div]').css('display', 'none');
            $('div[id=vehicle_div]').css('display', 'block');

      
            if (!/AGV/i.test(dataproperties.name)) {
                vehicletop_Table_Body.append(pivrow_template.supplant(formatvehicleinforow(dataproperties)));
            }

            if (/^AGV/i.test(dataproperties.name)) {
                vehicletop_Table_Body.append(agvrow_template.supplant(formatvehicleinforow(dataproperties)));
                vehiclemission_Table_Body.empty();
                if (!$.isEmptyObject(dataproperties.Mission)) {
                    $.each(dataproperties.Mission, function () {
                        vehiclemission_Table_Body.append(agvmissionrow_template.supplant(formatvehiclemissionrow(this)));
                    });
                }
            }
    } catch (e) {
        console.log(e);
    }
}
let pivrow_template = '<tr data-id="{TagId}" id="vehicletagid">' +
    '<td>Vehicle Name:</td>' +
    '<td class="text-left">{name}</td>' +
    '<td>Tag ID:</td>' +
    '<td>{TagId}</td>' +
    '</tr>' +
    '</td>' +
    '</tr>"';
let vehicletop_Table = $('table[id=vehicletable]');
let vehicletop_Table_Body = vehicletop_Table.find('tbody');

let agvrow_template = '<tr data-id="{TagId}" id="vehicletagid"><td>Vehicle Name:</td><td class="text-left">{name}</td><td>Tag ID:</td><td>{TagId}</td></tr>' +
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
        batterylevelnum: properties.hasOwnProperty("vehicleBatteryPercent") ? properties.vehicleBatteryPercent : 0,
        vehiclenumber: properties.name.replace(/[^0-9.]/g, '').replace(/^0+/, ''),
        vehiclestate: properties.hasOwnProperty("state") ? Get_Vehicle_Status(properties.state.replace(/VState/ig, "")) : "N/A",
        vehicleBatteryProgressBar: Get_Vehicle_Progress(properties.hasOwnProperty("vehicleBatteryPercent") ? properties.vehicleBatteryPercent : 0),
        vehiclestateCategory: properties.hasOwnProperty("vehicleCategory") ? Get_Vehicle_Catagory(properties.vehicleCategory) : "btn-outline-info",
        vehiclestatecolor: properties.hasOwnProperty("state") ? properties.state : "red"
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