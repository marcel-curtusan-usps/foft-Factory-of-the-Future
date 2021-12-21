/*
*this is for the Vehicle data load 
*/

$.extend(fotfmanager.client, {
    updateVehicleTagStatus: async (vehicleupdate) => { updateVehicleTag(vehicleupdate) }
});

async function updateVehicleTag(vehicleupdate) {
    try {
        map.whenReady(() => {
            if (vehicleupdate) {
                if (vehicles.hasOwnProperty("_layers")) {
                    var layerindex = -0;
                    $.map(vehicles._layers, function (layer, i) {
                        if (layer.hasOwnProperty("feature")) {
                            if (layer.feature.properties.id === vehicleupdate.properties.id) {
                                if (layerindex === -0) {
                                    layerindex = layer._leaflet_id;
                                    layer.feature.properties = vehicleupdate.properties;
                                    if (vehicleupdate.geometry.coordinates.length > 0) {
                                        layer.feature.geometry = vehicleupdate.geometry.coordinates;
                                        var newLatLng = new L.latLng(vehicleupdate.geometry.coordinates[1], vehicleupdate.geometry.coordinates[0]);
                                        var distanceTo = (newLatLng.distanceTo(layer.getLatLng()).toFixed(0) / 1000);
                                        if (Math.round(distanceTo) > 4000) {
                                            vehicles._layers[layer._leaflet_id].setLatLng(newLatLng);
                                        }
                                        else {
                                            vehicles._layers[layer._leaflet_id].slideTo(newLatLng, { duration: 3000 });
                                        }
                                    }
                                    return false;
                                }
                            }
                        }
                    });
                    if (layerindex !== -0) {
                        if (vehicles._layers[layerindex].feature.properties.id === vehicleupdate.properties.id) {
                            if (/^AGV/i.test(vehicleupdate.properties.name)) {
                                try {
                                    var data_Id = $('#vehicletagid').attr("data-id");
                                    if (data_Id === vehicleupdate.properties.id) {
                                        if ($('input[type=checkbox][name=followvehicle]')[0].checked) {
                                            map.setView(new L.LatLng(vehicleupdate.geometry.coordinates[1], vehicleupdate.geometry.coordinates[0]), 4);
                                        }
                                    }
                                    if (vehicleupdate.properties.hasOwnProperty("state")) {
                                        var new_state = Get_Vehicle_Status(vehicleupdate.properties.state.replace(/VState/ig, ""));
                                        if (/Obstructed/i.test(new_state)) {
                                            if (vehicles._layers[layerindex].hasOwnProperty("_icon")) {
                                                if (vehicles._layers[layerindex].hasOwnProperty("_tooltip")) {
                                                    if (vehicles._layers[layerindex]._tooltip.hasOwnProperty("_container")) {
                                                        if (!vehicles._layers[layerindex]._tooltip._container.classList.contains('obstructedflash')) {
                                                            vehicles._layers[layerindex]._tooltip._container.classList.add('obstructedflash');
                                                        }
                                                    }
                                                }
                                            }
                                        } else {
                                            if (vehicles._layers[layerindex].hasOwnProperty("_tooltip")) {
                                                if (vehicles._layers[layerindex]._tooltip.hasOwnProperty("_container")) {
                                                    if (vehicles._layers[layerindex]._tooltip._container.classList.contains('obstructedflash')) {
                                                        vehicles._layers[layerindex]._tooltip._container.classList.remove('obstructedflash');
                                                    }
                                                }
                                            }
                                        }
                                        var vehicle_div = $('div[id=div_vehicle]').find('button[id=' + vehicleupdate.properties.name + '][name=vehicle]');
                                        //$("button[id=" + vehicleupdate.properties.name + "][name=vehicle]");
                                        //$('div[id=div_vehicle]').find('button[id=' + vehicleupdate.properties.name + '][name=vehicle]');
                                        if (vehicle_div.length > 0) {
                                            var current_state = $("button[id=" + vehicleupdate.properties.name + "][name=vehicle]").text();
                                            if (new_state !== current_state) {
                                                $("button[id=" + vehicleupdate.properties.name + "][name=vehicle]").text(new_state);
                                            }
                                            var new_btn_category = Get_Vehicle_Catagory(vehicleupdate.properties.vehicleCategory);
                                            var current_btn_category = Get_Current_Class($("button[id=" + vehicleupdate.properties.name + "][name=vehicle]").attr("class"));
                                            if (new_btn_category !== current_btn_category) {
                                                $("button[id=" + vehicleupdate.properties.name + "][name=vehicle]").addClass(new_btn_category).removeClass(current_btn_category);
                                            }
                                            var batint = parseInt($("span[name=" + vehicleupdate.properties.name + "_batter_level" + "]").attr("data-batter_lvl"));
                                            if (!vehicleupdate.properties.hasOwnProperty("vehicleBatteryPercent")) {
                                                vehicleupdate.properties.vehicleBatteryPercent = 0;
                                            }
                                            if (vehicleupdate.properties.vehicleBatteryPercent !== batint) {
                                                $("span[name=" + vehicleupdate.properties.name + "_batter_level" + "]").text(vehicleupdate.properties.vehicleBatteryPercent + " % Charged").attr("data-batter_lvl", vehicleupdate.properties.vehicleBatteryPercent);
                                                $("div[name=" + vehicleupdate.properties.name + "_progressbar" + "]").attr("aria-valuenow", vehicleupdate.properties.name).css("width", vehicleupdate.properties.vehicleBatteryPercent + "%");
                                                new_btn_category = Get_Vehicle_Progress(vehicleupdate.properties.vehicleBatteryPercent);
                                                current_btn_category = Get_Current_Class($("div[name=" + vehicleupdate.properties.name + "_progressbar" + "]").attr("class"));
                                                if (new_btn_category !== current_btn_category) {
                                                    $("div[name=" + vehicleupdate.properties.name + "_progressbar" + "]").addClass(new_btn_category).removeClass(current_btn_category);
                                                }
                                            }
                                        }
                                        if (vehicleupdate.properties.hasOwnProperty("inMission")) {
                                            let tagid = $('#vehicletagid').attr("data-id");
                                            if (tagid === vehicleupdate.properties.id) {
                                                if (vehicleupdate.properties.inMission) {
                                                    //this is when vhicle is in a mission
                                                    $tagtop_Table = $('table[id=vehiclemissiontable]');
                                                    $tagtop_Table_Body = $tagtop_Table.find('tbody');
                                                    $tagtop_Table_Body.empty();
                                                    $tagtop_Table_Body.append(agvmissionrow_template.supplant(formatvehiclemissionrow(vehicleupdate.properties)));

                                                }
                                                else {
                                                    var vehicle_div = $("tr[id=inmission]")
                                                    //$('div[id=div_vehicle]').find('button[id=' + vehicleupdate.properties.name + '][name=vehicle]');
                                                    if (vehicle_div.length > 0) {
                                                        $(vehicle_div).remove();
                                                    }
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
                                }
                                catch (e) {
                                    console.log(e);
                                }
                            }
                            else if (!/^AGV/i.test(vehicleupdate.properties.name)) {
                                var dataId = $('#vehicletagid').attr("data-id");
                                if (dataId === vehicleupdate.properties.id) {
                                    if ($('input[type=checkbox][name=followvehicle]')[0].checked) {
                                        map.setView(new L.LatLng(vehicleupdate.geometry.coordinates[1], vehicleupdate.geometry.coordinates[0]), 4);
                                    }
                                }
                            }
                        }
                    }
                    else {
                        vehicles.addData(vehicleupdate);
                    }
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
            LoadVehicleTable(feature.properties, 'vehicletable');
        })
        if (feature.properties.hasOwnProperty('state')) {
            var new_state = Get_Vehicle_Status(feature.properties.state.replace(/VState/ig, ""));
            if (/Obstructed/i.test(new_state)) {
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
async function LoadVehicleTable(dataproperties, table) {
    try {
        if (!$.isEmptyObject(dataproperties)) {
            if (/vehicletable/i.test(table)) {
                let vehicletop_Table = $('table[id=' + table + ']');
                let vehicletop_Table_Body = vehicletop_Table.find('tbody');
                vehicletop_Table_Body.empty();
                let vehiclemission_Table = $('table[id=vehiclemissiontable]');
                let vehiclemission_Table_Body = vehiclemission_Table.find('tbody');
                vehiclemission_Table_Body.empty();
                $('div[id=agvlocation_div]').css('display', 'none');
                $('div[id=dockdoor_div]').css('display', 'none');
                $('div[id=trailer_div]').css('display', 'none');
                $('div[id=machine_div]').css('display', 'none');
                $('div[id=staff_div]').css('display', 'none');
                $('div[id=ctstabs_div]').css('display', 'none');
                $('div[id=area_div]').css('display', 'none')
                $('div[id=vehicle_div]').css('display', 'block');
                $('div[id=dps_div]').css('display', 'none');
                $zoneSelect[0].selectize.setValue(-1, true);
                if (!/AGV/i.test(dataproperties.name)) {
                    vehicletop_Table_Body.append(pivrow_template.supplant(formatvehicleinforow(dataproperties)));
                }

                if (/^AGV/i.test(dataproperties.name)) {
                    vehicletop_Table_Body.append(agvrow_template.supplant(formatvehicleinforow(dataproperties)));
                    if (dataproperties.hasOwnProperty("inMission")) {
                        if (dataproperties.inMission) {
                            vehiclemission_Table_Body.empty();
                            vehiclemission_Table_Body.append(agvmissionrow_template.supplant(formatvehiclemissionrow(dataproperties)));
                        }
                        else {
                            vehiclemission_Table_Body.empty();
                        }
                    } else {
                        vehiclemission_Table_Body.empty();
                    }
                }
            }
        }
    } catch (e) {
        console.log(e);
    }
}
let pivrow_template = '<tr data-id="{TagId}" id="vehicletagid"><td>Vehicle Name:</td><td class="text-left">{name}</td><td>Tag ID:</td><td>{TagId}</td></tr>' +
    '</td></tr>"';
let agvrow_template = '<tr data-id="{TagId}" id="vehicletagid"><td>Vehicle Name:</td><td class="text-left">{name}</td><td>Tag ID:</td><td>{TagId}</td></tr>' +
    '<tr><td>State:</td><td colspan="3"><button id="{name}" name="vehicle" class="btn btn-block btn-sm {vehiclestateCategory} vehiclehistory" style="margin-bottom: 2px;" value="{vehiclestate}">{vehiclestate}</button></td></tr>"' +
    '<tr><td>Battery:</td><td colspan="3">' +
    '<div class="progress">' +
    '<div class="progress-bar btn-sm {vehicleBatteryProgressBar}"  name="{name}_progressbar" role="progressbar" aria-valuenow="{batterylevelnum}" aria-valuemin="0" aria-valuemax="100" style="width: {batterylevelnum}%;">' +
    '<span name="{name}_batter_level" style="color: black; position: absolute;" data-batter_lvl="{batterylevelnum}">{batterylevelnum} % Charged</span>' +
    '</div>' +
    '</div>' + '</td></tr>"';
let agvmissionrow_template = '<tr class="text-center" id=inmission><td class="font-weight-bold" colspan="2" >In Mission Status</td>' +
    '<tr id=pickupLocation_{TagId}><td>Pickup Location:</td><td>{pickuplocation}</td></tr>' +
    '<tr id=pickupLocationEta_{TagId}><td>ETA to Pickup:</td><td>{pickuplocationEta}</td></tr>' +
    '<tr id=dropoffLocation_{TagId}><td>Drop-Off Location:</td><td>{dropofflocation}</td></tr>' +
    '<tr id=endLocation_{TagId}><td>End Location:</td><td>{endlocation}</td></tr>' +
    '<tr><td>Placard:</td><td>{mtelplacard}</td></tr>' +
    '</tr>';
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
        TagId: properties.id,
        name: properties.name,
        inMission: properties.hasOwnProperty("inMission") ? properties.inMission === true ? "Yes" : "No" : "No",
        pickuplocation: properties.hasOwnProperty("pickupLocation") ? Get_location_Code(properties.pickupLocation) : "N/A",
        dropofflocation: properties.hasOwnProperty("dropoffLocation") ? Get_location_Code(properties.dropoffLocation) : "N/A",
        endlocation: properties.hasOwnProperty("endLocation") ? Get_location_Code(properties.endLocation) : "N/A",
        pickuplocationEta: properties.hasOwnProperty("etaToPickup") ? properties.etaToPickup : "N/A",
        mtelplacard: properties.hasOwnProperty("placard") ? properties.placard : "N/A"
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